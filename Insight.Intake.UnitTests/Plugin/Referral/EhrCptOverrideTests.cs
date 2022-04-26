using FakeXrmEasy;
using Insight.Intake.Models;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class EhrCptOverrideTests : PluginTestsBase
    {
        [Fact]
        public void DoesNotRemovesCptCodesIfInvalidReferral()
        {
            //Arrange

            var cpt1 = new ipg_cptcode().Fake().Generate();
            var referral = new ipg_referral().Fake()
                .WithCaseStatus(ipg_CaseStatus.Closed)
                .WithCaseOutcome(ipg_CaseOutcomeCodes.Gate2Fail)
                .WithCptCodeReferences(new List<ipg_cptcode> { cpt1 })
                .Generate();

            var listForInit = new List<Entity> { referral, cpt1 };
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGReferralActionsEhrCptOverride",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = new ParameterCollection() { { "Target", referral.ToEntityReference() } },
                OutputParameters = new ParameterCollection()
            };

            //Act

            fakedContext.ExecutePluginWith<EhrCptOverride>(pluginContext);

            //Assert

            var orgService = fakedContext.GetOrganizationService();
            var updatedReferral = orgService.Retrieve(ipg_referral.EntityLogicalName, referral.Id, new ColumnSet(true)).ToEntity<ipg_referral>();
            Assert.NotNull(updatedReferral.ipg_CPTCodeId1);
            Assert.Equal(cpt1.Id, updatedReferral.ipg_CPTCodeId1.Id);
            Assert.False((bool)pluginContext.OutputParameters["IsSuccess"]);
        }

        [Fact]
        public void RemovesCptCodesIfValidReferral()
        {
            //Arrange

            var cpt1 = new ipg_cptcode().Fake().Generate();
            var cpt2 = new ipg_cptcode().Fake().Generate();
            var procedureName = new ipg_procedurename().Fake().Generate();
            var defaultProcedureName = new ipg_procedurename().Fake().Generate();
            var globalSetting = new ipg_globalsetting().Fake(GlobalSettingConstants.DefaultProcedureIdSettingName, defaultProcedureName.Id.ToString());
            var procedureNameReference = procedureName.ToEntityReference();
            procedureNameReference.Name = procedureName.ipg_name;
            var referral = new ipg_referral().Fake()
                .WithCaseStatus(ipg_CaseStatus.Closed)
                .WithCaseOutcome(ipg_CaseOutcomeCodes.GateEHRFail)
                .WithCptCodeReferences(new List<ipg_cptcode> { cpt1, cpt2 })
                .WithProcedureNameReference(procedureNameReference)
                .Generate();

            var listForInit = new List<Entity> { referral, cpt1, cpt2, procedureName, defaultProcedureName, globalSetting };
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGReferralActionsEhrCptOverride",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = new ParameterCollection() { { "Target", referral.ToEntityReference() } },
                OutputParameters = new ParameterCollection()
            };

            //Act

            fakedContext.ExecutePluginWith<EhrCptOverride>(pluginContext);

            //Assert

            Assert.True((bool)pluginContext.OutputParameters["IsSuccess"]);
            var orgService = fakedContext.GetOrganizationService();
            var updatedReferral = orgService.Retrieve(ipg_referral.EntityLogicalName, referral.Id, new ColumnSet(true)).ToEntity<ipg_referral>();
            Assert.Null(updatedReferral.ipg_CPTCodeId1);
            Assert.Null(updatedReferral.ipg_CPTCodeId2);
            Assert.NotNull(updatedReferral.ipg_ProcedureNameId);
            Assert.Equal(defaultProcedureName.Id, updatedReferral.ipg_ProcedureNameId.Id);
            Assert.Equal(procedureName.ipg_name, updatedReferral.ipg_EHRProcedureName);

            var annotationsQuery = new QueryExpression(Annotation.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            var annotations = orgService.RetrieveMultiple(annotationsQuery).Entities.Select(e => e.ToEntity<Annotation>());
            Assert.Single(annotations);
            var annotation = annotations.FirstOrDefault();
            Assert.Equal(referral.Id, annotation.ObjectId.Id);

        }

    }
}
