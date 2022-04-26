using System.Collections.Generic;
using Xunit;
using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Insight.Intake.Plugin.Claim;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace Insight.Intake.UnitTests.Plugin.Claim
{
    public class UpdateClaimStatusTest : PluginTestsBase
    {
        [Fact]
        public void CheckClaim()
        {

            var fakedContext = new XrmFakedContext();
            ServiceProvider = ServiceProviderMock.Object;
            Invoice claim = new Invoice().Fake();
            ipg_claimzirmedeventcode eventCode = new ipg_claimzirmedeventcode().Fake();
            eventCode.ipg_name = "A-I";
            ipg_claimzirmedstatus zirmedStatus = new ipg_claimzirmedstatus().Fake().FakeClaimZirmedStatusForClaims(claim, eventCode);

            ipg_claimconfiguration claimconfiguration = new ipg_claimconfiguration().Fake().WithEvent(eventCode, (int)ipg_ClaimEvent._277fileprocessing, (int)ipg_ClaimSubEvent.PaperSubmission, (int)ipg_ClaimStatus.Processed, (int)ipg_ClaimReason.AcceptedbyIntermediary);

            var listForInit = new List<Entity> { claim, eventCode, zirmedStatus, claimconfiguration };
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", zirmedStatus } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = ipg_claimzirmedstatus.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpdateClaimStatus>(pluginContext);
            var invoceRef = (fakedContext.GetOrganizationService().Retrieve(claim.LogicalName, claim.Id, new ColumnSet(nameof(Invoice.ipg_Status).ToLower(), nameof(Invoice.ipg_Reason).ToLower())).ToEntity<Invoice>());
            Assert.Equal((int)ipg_ClaimStatus.Processed, invoceRef.ipg_Status.Value);
            Assert.Equal((int)ipg_ClaimReason.AcceptedbyIntermediary, invoceRef.ipg_Reason.Value);
        }
    }
}
