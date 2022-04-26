using System;
using System.Collections.Generic;
using Bogus;
using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class PopulatePrimaryAttributePluginTests : PluginTestsBase
    {
        [Fact]
        public void GetsProcedureNameFromCpt()
        {
            //ARRANGE
            var procedureName = new ipg_procedurename().Fake().Generate();
            var cptCode = new ipg_cptcode().Fake().WithProcedureNameReference(procedureName.ToEntityReference()).Generate();
            cptCode.ipg_procedurename.Name = procedureName.ipg_name;
            string referralNumber = new Faker().Random.Int(10000).ToString();
            var referral = new ipg_referral().Fake().WithReferralNumber(referralNumber).WithCptCodeReferences(new List<ipg_cptcode> { cptCode }).Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>{ procedureName, cptCode, referral });

            var inputParameters = new ParameterCollection { { "Target", referral } };


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection { { "PostImage", referral } },
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<PopulatePrimaryAttributePlugin>(pluginContext);

            //ASSERT
            var orgService = fakedContext.GetOrganizationService();
            var updatedReferral = orgService.Retrieve(ipg_referral.EntityLogicalName, referral.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true)).ToEntity<ipg_referral>();
            Assert.Equal($"Referral {referralNumber} - {referral.ipg_PatientFirstName} {referral.ipg_PatientMiddleName} {referral.ipg_PatientLastName} - {procedureName.ipg_name}", updatedReferral.ipg_name);
            Assert.Equal(referralNumber, updatedReferral.ipg_referralcasenumber);
        }

        [Fact]
        public void GetsProcedureNameFromReferralProcedure()
        {
            //ARRANGE
            var procedureName = new ipg_procedurename().Fake().Generate();
            string referralNumber = new Faker().Random.Int(10000).ToString();
            var referral = new ipg_referral().Fake().WithReferralNumber(referralNumber).WithProcedureNameReference(procedureName.ToEntityReference()).Generate();
            referral.ipg_ProcedureNameId.Name = procedureName.ipg_name;

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { procedureName, referral });

            var inputParameters = new ParameterCollection { { "Target", referral }, { "PostImage", referral } };


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection { { "PostImage", referral } },
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<PopulatePrimaryAttributePlugin>(pluginContext);

            //ASSERT
            var orgService = fakedContext.GetOrganizationService();
            var updatedReferral = orgService.Retrieve(ipg_referral.EntityLogicalName, referral.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true)).ToEntity<ipg_referral>();
            Assert.Equal($"Referral {referralNumber} - {referral.ipg_PatientFirstName} {referral.ipg_PatientMiddleName} {referral.ipg_PatientLastName} - {procedureName.ipg_name}", updatedReferral.ipg_name);
            Assert.Equal(referralNumber, updatedReferral.ipg_referralcasenumber);
        }

        [Fact]
        public void SkipsMissingPatientMiddleName()
        {
            //ARRANGE
            var procedureName = new ipg_procedurename().Fake().Generate();
            var cptCode = new ipg_cptcode().Fake().WithProcedureNameReference(procedureName.ToEntityReference()).Generate();
            cptCode.ipg_procedurename.Name = procedureName.ipg_name;
            string referralNumber = new Faker().Random.Int(10000).ToString();
            var referral = new ipg_referral().Fake().WithReferralNumber(referralNumber).WithCptCodeReferences(new List<ipg_cptcode> { cptCode }).Generate();
            referral.ipg_PatientMiddleName = null;

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { procedureName, cptCode, referral });

            var inputParameters = new ParameterCollection { { "Target", referral }, { "PostImage", referral } };


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection { { "PostImage", referral } },
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<PopulatePrimaryAttributePlugin>(pluginContext);

            //ASSERT
            var orgService = fakedContext.GetOrganizationService();
            var updatedReferral = orgService.Retrieve(ipg_referral.EntityLogicalName, referral.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true)).ToEntity<ipg_referral>();
            Assert.Equal($"Referral {referralNumber} - {referral.ipg_PatientFirstName} {referral.ipg_PatientLastName} - {procedureName.ipg_name}", updatedReferral.ipg_name);
            Assert.Equal(referralNumber, updatedReferral.ipg_referralcasenumber);
        }

    }
}
