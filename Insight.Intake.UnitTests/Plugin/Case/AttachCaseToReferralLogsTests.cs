using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class AttachCaseToReferralLogsTests : PluginTestsBase
    {
        [Fact]
        public void CreateCaseFromReferralWithEventLogs_ShouldAttachCaseToReferralEventLogs()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            ipg_referral fakedReferral = new ipg_referral().Fake();
            Incident fakedIncident = new Incident().Fake().WithReferral(fakedReferral.ToEntityReference());
            var fakedReferralId = fakedReferral.Id.ToString().Replace("{", String.Empty).Replace("}", String.Empty);
            var referralLog1 = new ipg_importanteventslog() { Id = Guid.NewGuid(), ipg_referralid = fakedReferralId };
            var referralLog2 = new ipg_importanteventslog() { Id = Guid.NewGuid(), ipg_referralid = fakedReferralId };
            var notReferralLog = new ipg_importanteventslog() { Id = Guid.NewGuid(), ipg_referralid = "g1j714a8-2100-hg70-bb65-73994533c5f9" };
            var inputParameters = new ParameterCollection { { "Target", fakedIncident } };

            var listForInit = new List<Entity>() { fakedReferral, fakedIncident, referralLog1, referralLog2, notReferralLog };
            fakedContext.Initialize(listForInit);
            
            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<AttachCaseToReferralLogs>(fakedPluginExecutionContext);
            var log1 = (ipg_importanteventslog)fakedService.Retrieve(ipg_importanteventslog.EntityLogicalName, referralLog1.Id, new ColumnSet(ipg_importanteventslog.Fields.ipg_caseid));
            var log2 = (ipg_importanteventslog)fakedService.Retrieve(ipg_importanteventslog.EntityLogicalName, referralLog2.Id, new ColumnSet(ipg_importanteventslog.Fields.ipg_caseid));
            var log3 = (ipg_importanteventslog)fakedService.Retrieve(ipg_importanteventslog.EntityLogicalName, notReferralLog.Id, new ColumnSet(ipg_importanteventslog.Fields.ipg_caseid));

            // Assert
            Assert.True(Guid.Parse(log1.ipg_caseid) == fakedIncident.Id && Guid.Parse(log2.ipg_caseid) == fakedIncident.Id);
            Assert.Null(log3.ipg_caseid);
        }
    }
}