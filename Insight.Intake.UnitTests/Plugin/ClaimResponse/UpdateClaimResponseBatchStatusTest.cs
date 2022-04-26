using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Insight.Intake.Plugin.ClaimResponse;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.UnitTests.Plugin.ClaimResponse
{
    public class UpdateClaimResponseBatchStatusTest : PluginTestsBase
    {
        [Fact]
        public void CheckReviewStatus()
        {
            var fakedContext = new XrmFakedContext();
            ServiceProvider = ServiceProviderMock.Object;
            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_claimresponseheader header1 = new ipg_claimresponseheader().Fake().WithClaimResponseBatchReference(batch);
            ipg_claimresponseheader header2 = new ipg_claimresponseheader().Fake().WithClaimResponseBatchReference(batch);
            header1.ipg_PostStatus = "new";
            header2.ipg_PostStatus = "review";

            Incident case1 = new Incident().Fake();
            Incident case2 = new Incident().Fake();
            header1.ipg_CaseId = case1.ToEntityReference();
            header2.ipg_CaseId = case1.ToEntityReference();

            var listForInit = new List<Entity> { batch, header1, header2, case1, case2 };
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", header1 } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = ipg_claimresponseheader.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpdateClaimResponseBatchStatus>(pluginContext);
            var batchRef = (fakedContext.GetOrganizationService().Retrieve(batch.LogicalName, batch.Id, new ColumnSet("statuscode")).ToEntity<ipg_claimresponsebatch>());
            Assert.Equal(427880002, batchRef.StatusCode.Value);
        }

        [Fact]
        public void CheckPartialPostErrorsStatus()
        {
            var fakedContext = new XrmFakedContext();
            ServiceProvider = ServiceProviderMock.Object;
            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_claimresponseheader header1 = new ipg_claimresponseheader().Fake().WithClaimResponseBatchReference(batch);
            header1.ipg_PostStatus = "new";

            var listForInit = new List<Entity> { batch, header1 };
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", header1 } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = ipg_claimresponseheader.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpdateClaimResponseBatchStatus>(pluginContext);
            var batchRef = (fakedContext.GetOrganizationService().Retrieve(batch.LogicalName, batch.Id, new ColumnSet("statuscode")).ToEntity<ipg_claimresponsebatch>());
            Assert.Equal(427880004, batchRef.StatusCode.Value);
        }
    }
}
