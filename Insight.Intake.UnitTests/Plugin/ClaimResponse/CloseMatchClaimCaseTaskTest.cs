using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Insight.Intake.Plugin.ClaimResponseHeader;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.UnitTests.Plugin.ClaimResponse
{
    public class CloseMatchClaimCaseTaskTest : PluginTestsBase
    {
        [Fact]
        public void CheckCaseTaskState()
        {
            var fakedContext = new XrmFakedContext();
            ServiceProvider = ServiceProviderMock.Object;
            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_claimresponseheader header = new ipg_claimresponseheader().Fake().WithClaimResponseBatchReference(batch);

            Incident incident = new Incident().Fake();
            header.ipg_CaseId = incident.ToEntityReference();

            Task task = new Task().Fake();
            task.Subject = "Match Patient Payment to a Case";
            task.RegardingObjectId = header.ToEntityReference();

            var listForInit = new List<Entity> { batch, header, incident, task };
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", header } };
            var PostEntityImages = new EntityImageCollection() { { "PostImage", header } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = ipg_claimresponseheader.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = PostEntityImages,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CloseMatchClaimCaseTask>(pluginContext);
            var taskRef = (fakedContext.GetOrganizationService().Retrieve(task.LogicalName, task.Id, new ColumnSet(nameof(Task.StateCode).ToLower(), nameof(Task.StatusCode).ToLower())).ToEntity<Task>());
            Assert.Equal(TaskState.Completed, taskRef.StateCode.Value);
            Assert.Equal((int)Task_StatusCode.Resolved, taskRef.StatusCode.Value);
        }

        [Fact]
        public void CheckClaimTaskState()
        {
            var fakedContext = new XrmFakedContext();
            ServiceProvider = ServiceProviderMock.Object;
            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_claimresponseheader header = new ipg_claimresponseheader().Fake().WithClaimResponseBatchReference(batch);

            Invoice invoice = new Invoice().Fake();
            header.ipg_ClaimId = invoice.ToEntityReference();

            Task task = new Task().Fake();
            task.Subject = "Match Carrier Payment to a Claim";
            task.RegardingObjectId = header.ToEntityReference();

            var listForInit = new List<Entity> { batch, header, invoice, task };
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", header } };
            var PostEntityImages = new EntityImageCollection() { { "PostImage", header } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = ipg_claimresponseheader.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = PostEntityImages,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CloseMatchClaimCaseTask>(pluginContext);
            var taskRef = (fakedContext.GetOrganizationService().Retrieve(task.LogicalName, task.Id, new ColumnSet(nameof(Task.StateCode).ToLower(), nameof(Task.StatusCode).ToLower())).ToEntity<Task>());
            Assert.Equal(TaskState.Completed, taskRef.StateCode.Value);
            Assert.Equal((int)Task_StatusCode.Resolved, taskRef.StatusCode.Value);
        }

    }
}
