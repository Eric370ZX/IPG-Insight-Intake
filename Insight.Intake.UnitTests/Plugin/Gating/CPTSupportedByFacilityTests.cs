using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CPTSupportedByFacilityTests : PluginTestsBase
    {
        [Fact]
        public void NoCPTExclusionInFacility_Referral()
        {
            var fakedContext = new XrmFakedContext();

            ipg_referral referral = new ipg_referral().Fake();
            referral.ipg_FacilityId = new EntityReference()
            {
                Id = Guid.NewGuid()
            };
            referral.ipg_CPTCodeId1 = new EntityReference()
            {
                Id = Guid.NewGuid(),
                Name = "Test CPT"
            };
            referral.ipg_SurgeryDate = new DateTime(2019, 11, 12);
            
            var listForInit = new List<Entity> { referral };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacility",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacility>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void NoCPTExclusionInFacility_Incident()
        {
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();
            incident.ipg_FacilityId = new EntityReference()
            {
                Id = Guid.NewGuid()
            };
            incident.ipg_CPTCodeId1 = new EntityReference()
            {
                Id = Guid.NewGuid(),
                Name = "Test CPT"
            };
            incident.ipg_SurgeryDate = new DateTime(2019, 11, 12);

            var listForInit = new List<Entity> { incident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCPTSupportedByFacility",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CPTSupportedByFacility>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void FacilityWithCPTExclusion_Incident()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            var cpt = new ipg_cptcode().Fake();
            Intake.Account facility = new Intake.Account().Fake();
            facility.ipg_FacilityCimId = new EntityReference(SystemUser.EntityLogicalName, Guid.NewGuid());
            Incident incident = new Incident().Fake()
                .FakeWithCptCode(cpt)
                .WithFacilityReference(facility)
                .WithActualDos(new DateTime(2020, 02, 19));

            OrganizationServiceMock.WithRetrieveCrud(incident);           
            OrganizationServiceMock.WithRetrieveCrud(facility);

            var facilityCPT = new ipg_facilitycpt()
                                  .Fake()
                                  .WithCPT(cpt)
                                  .Generate(1);

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_facilitycpt.EntityLogicalName,
                facilityCPT.Cast<Entity>().ToList()
            );

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGGatingCPTSupportedByFacility");

            var request = new ipg_IPGGatingCPTSupportedByFacilityCarrierRequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id)
            };
            PluginExecutionContextMock.Setup(x => x.Stage).Returns(40);
            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);
            var outputParameters = new ParameterCollection();
            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin
            var plugin = new CPTSupportedByFacility();

            plugin.Execute(ServiceProvider);

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            Assert.False(Convert.ToBoolean(pluginExecutionContext.OutputParameters["Succeeded"]));
            Assert.NotNull(pluginExecutionContext.OutputParameters["PortalNote"]);
            Assert.NotNull(pluginExecutionContext.OutputParameters["CaseNote"]);

            #endregion
        }
    }
}