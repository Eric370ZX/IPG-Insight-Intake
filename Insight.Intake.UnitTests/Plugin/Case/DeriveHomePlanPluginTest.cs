using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class DeriveHomePlanPluginTest : PluginTestsBase
    {
        [Fact]
        public void HomeplanCarrierChangedAccordingToMemberId()
        {
            #region Setup Services
            ServiceProvider = ServiceProviderMock.Object;

            Intake.Account primaryCarrier = new Intake.Account().FakeCarrierForDeriveHomePlan("PrimaryCarrier");
            Intake.Account secondaryCarrier = new Intake.Account().FakeCarrierForDeriveHomePlan("SecondaryCarrier");
            Contact contact = new Contact().FakeContactWithMemberId("", "");
            Incident preImageIncident = new Incident().FakeIncidentWithPrimaryCarrierSecondaryCarrier(primaryCarrier, secondaryCarrier, contact);

            Contact updatedContact = contact.FakeContactWithMemberId("AAK123456");
            Incident postImageIncident =
                preImageIncident.FakeIncidentWithPrimaryCarrierSecondaryCarrier(primaryCarrier, secondaryCarrier, updatedContact);

            Task task = new Task().CreateTaskForDeriveHomePlan("Mapped to carrier", "", preImageIncident);

            var homeplanCarrierMap = new ipg_homeplancarriermap().FakeForDeriveHomePlan(primaryCarrier,
                new DateTime(2018, 10, 01), new DateTime(2018, 12, 31), code: "AAK");

            OrganizationServiceMock.Setup(x => x.Execute(
        It.Is<RetrieveAttributeRequest>(r => r.EntityLogicalName == Intake.Account.EntityLogicalName)))
                                                 .Returns(new RetrieveAttributeResponseWrapper(
                                                     new RetrieveAttributeResponse())
                                                 {
                                                     AttributeMetadata = FakeOptionSetMetadata.FakeHomeplanNetworkOptionset()
                                                 });
            OrganizationServiceMock.Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(r => r.EntityName == ipg_homeplancarriermap.EntityLogicalName)))
                .Returns(new EntityCollection().FakeEntityCollectionForDeriveHomeplan(primaryCarrier.Name, homeplanCarrierMap));

            Task task1 = new Task().FakeTaskForDerivedHomePlan($"Mapped to carrier"
                                                        , $"BCBS: Code [AAK] mapped to {primaryCarrier.Name} for the DOS {postImageIncident.ipg_SurgeryDate.Value.ToString()}." +
                                                        $" Please set the Home plan carrier to appropriate carrier", primaryCarrier.Id);
            OrganizationServiceMock.Setup(x => x.Create(It.Is<Task>(r => r.LogicalName == Task.EntityLogicalName)))
                .Returns(task1.Id);
            OrganizationServiceMock.Setup(x => x.Update(It.Is<Entity>(r => r.LogicalName == Incident.EntityLogicalName)));

            OrganizationService = OrganizationServiceMock.Object;

            #endregion

            #region Setup Execution Context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("Update");

            var request = new CreateRequest
            {
                Target = preImageIncident.ToEntity<Entity>()
            };
            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var preImagesCollection = new EntityImageCollection
            {
                { "CaseImage", preImageIncident }
            };

            PluginExecutionContextMock.Setup(x => x.PreEntityImages).Returns(preImagesCollection);

            var postImagesCollection = new EntityImageCollection
            {
                { "CaseImage",postImageIncident }
            };

            OrganizationServiceMock.Setup(os => os.Retrieve(Intake.Account.EntityLogicalName, primaryCarrier.Id,
    It.Is<ColumnSet>(cs => ContainsColumns(cs, nameof(Intake.Account.ipg_HomePlanNetworkOptionSet).ToLower(),
        nameof(Intake.Account.ipg_ZirMedID).ToLower())))).Returns(primaryCarrier);

            PluginExecutionContextMock.Setup(x => x.PostEntityImages).Returns(postImagesCollection);

            var outputParameters = new ParameterCollection
            {
                { "id", Guid.NewGuid() },
                { "ipg_HomeplanCarrierId", primaryCarrier.Id},
                { "Target", preImageIncident.ToEntity<Entity>() }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            var plugin = new DeriveHomePlanPlugin();
            plugin.Execute(ServiceProvider);
            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            if (!pluginExecutionContext.InputParameters.Contains("Target") || !(pluginExecutionContext.InputParameters["Target"] is Entity))
            {
                throw new Exception("Input target should be Entity.");
            }
            var record = ((Entity)pluginExecutionContext.InputParameters["Target"]).ToEntity<Incident>();
            #endregion

            //Assert
            Assert.Equal(primaryCarrier.Id, record.ipg_CarrierId.Id);
        }
        private bool ContainsColumns(ColumnSet columnSet, params string[] columns)
        {
            return Enumerable.Any(columns, c => columnSet.Columns.Contains(c));
        }

        [Fact(Skip = "Fore real data test")]
        //[Fact]
        public void DeriveHomePlan_ActualTest()
        {
            var fakedContext = new XrmRealContext();

            //Incident caseEntity = new Incident().Fake();
            //caseEntity.Id = new Guid("13c16925-6ac7-eb11-bacc-000d3a366c3f");
            ipg_referral referral = new ipg_referral().Fake();
            referral.Id = new Guid("7b02f9c1-90b9-4047-a7dd-3b99afdbd4e5");

            var listForInit = new List<Entity>() { };
            fakedContext.Initialize(listForInit);

            //var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCptCodesValidation",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId=new Guid("066d36b8-a8be-e911-a836-000d3a31550e")
            };
            //ACT
            fakedContext.ExecutePluginWith<CptCodesValidation>(pluginContext);
             
            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
