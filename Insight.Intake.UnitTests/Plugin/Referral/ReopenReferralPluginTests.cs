using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class ReopenReferralPluginTests : PluginTestsBase
    {
        private const string CaseStatusDisplayedId = "fbf272d4-42ac-e911-a987-000d3a370909";
        private const string IntakeStep2LifecycleStepId = "871532c4-d3c1-e911-a983-000d3a37043b";
        private const string EhrLifecycleStepId = "1f5d0ed1-1136-ea11-a813-000d3a33fc30";
        private const string Gate2Config = "3c407929-aa4c-e911-a982-000d3a37062b";
        private const string EhrGateId = "1c3f4e0e-0236-ea11-a813-000d3a33fc30";

        [Fact]
        public void ReopenFaxReferralPluginTest()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referral = new ipg_referral().Fake().WithOrigin(Incident_CaseOriginCode.Fax);
            ipg_casestatusdisplayed caseStatusDisplayed = new ipg_casestatusdisplayed().Fake();
            caseStatusDisplayed.Id = new Guid(CaseStatusDisplayedId);
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake("Gate 2");
            gateConfiguration.Id = new Guid(Gate2Config);
            ipg_lifecyclestep lifecycleStep = new ipg_lifecyclestep().Fake(gateConfiguration);
            lifecycleStep.Id = new Guid(IntakeStep2LifecycleStepId);

            var listForInit = new List<Entity> { referral, lifecycleStep, caseStatusDisplayed, gateConfiguration };
            fakedContext.Initialize(listForInit);
            
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGReferralReopenReferral",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = new ParameterCollection() { { "Referral", referral } },
                OutputParameters = new ParameterCollection()
            };

            //Act

            fakedContext.ExecutePluginWith<ReopenReferralPlugin>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var query = new QueryExpression(ipg_referral.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_referral.Fields.ipg_casestatus, ipg_referral.Fields.ipg_lifecyclestepid, ipg_referral.Fields.ipg_casestatusdisplayedid,
                                          ipg_referral.Fields.StateCode, ipg_referral.Fields.ipg_gateconfigurationid)
            };
            var updatedReferral = fakedService.RetrieveMultiple(query).Entities.FirstOrDefault().ToEntity<ipg_referral>();

            ///Assert
            Assert.Equal((int)ipg_CaseStatus.Open, updatedReferral.ipg_casestatus.Value);
            Assert.Equal(lifecycleStep.Id, updatedReferral.ipg_lifecyclestepid.Id);
            Assert.Equal(caseStatusDisplayed.Id, updatedReferral.ipg_casestatusdisplayedid.Id);
            Assert.Equal(ipg_referralState.Active, updatedReferral.StateCode);
            Assert.Equal(gateConfiguration.Id, updatedReferral.ipg_gateconfigurationid.Id);
        }

        [Fact]
        public void ReopenEhrReferralPluginTest()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referral = new ipg_referral().Fake().WithOrigin(Incident_CaseOriginCode.EHR);
            ipg_casestatusdisplayed caseStatusDisplayed = new ipg_casestatusdisplayed().Fake();
            caseStatusDisplayed.Id = new Guid(CaseStatusDisplayedId);
            ipg_gateconfiguration gateConfiguration = new ipg_gateconfiguration().Fake("Gate EHR");
            gateConfiguration.Id = new Guid(EhrGateId);
            ipg_lifecyclestep lifecycleStep = new ipg_lifecyclestep().Fake(gateConfiguration);
            lifecycleStep.Id = new Guid(EhrLifecycleStepId);

            var listForInit = new List<Entity> { referral, lifecycleStep, caseStatusDisplayed, gateConfiguration };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGReferralReopenReferral",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = new ParameterCollection() { { "Referral", referral } },
                OutputParameters = new ParameterCollection()
            };

            //Act

            fakedContext.ExecutePluginWith<ReopenReferralPlugin>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var query = new QueryExpression(ipg_referral.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_referral.Fields.ipg_casestatus, ipg_referral.Fields.ipg_lifecyclestepid, ipg_referral.Fields.ipg_casestatusdisplayedid,
                                          ipg_referral.Fields.StateCode, ipg_referral.Fields.ipg_gateconfigurationid)
            };
            var updatedReferral = fakedService.RetrieveMultiple(query).Entities.FirstOrDefault().ToEntity<ipg_referral>();

            ///Assert
            Assert.Equal((int)ipg_CaseStatus.Open, updatedReferral.ipg_casestatus.Value);
            Assert.Equal(lifecycleStep.Id, updatedReferral.ipg_lifecyclestepid.Id);
            Assert.Equal(caseStatusDisplayed.Id, updatedReferral.ipg_casestatusdisplayedid.Id);
            Assert.Equal(ipg_referralState.Active, updatedReferral.StateCode);
            Assert.Equal(gateConfiguration.Id, updatedReferral.ipg_gateconfigurationid.Id);
        }
    }
}
