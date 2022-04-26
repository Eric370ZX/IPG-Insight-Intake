using FakeXrmEasy;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class VerifyAllMissingInformationForPoolTaskTests : PluginTestsBase
    {
        [Fact]
        public void Throw_exception_when_all_criteria_not_met()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create 'Pool' task with task category and task type, associated case with not approved documents and open Missing Information tasks,
            //without PPP document and without actual DOS
            ipg_documenttype PPPDocumentType = new ipg_documenttype()
                .Fake("PPP", "PPP");

            ipg_taskcategory MITaskCategory = new ipg_taskcategory()
                .Fake()
                .WithName("Missing Information");

            Incident incident = new Incident()
                .Fake();

            Task openMITask = new Task()
                .Fake()
                .WithState(TaskState.Open)
                .WithRegarding(incident.ToEntityReference())
                .WithTaskCategoryRef(MITaskCategory.ToEntityReference());

            ipg_document document = new ipg_document()
                .Fake()
                .WithReviewStatus(ipg_document_ipg_ReviewStatus.PendingReview)
                .WithCaseReference(incident);

            ipg_taskcategory CPTaskCategory = new ipg_taskcategory()
                .Fake()
                .WithName("Case Processing");

            ipg_tasktype poolTaskType = new ipg_tasktype()
                .Fake()
                .WithName("Request to Complete Case Mgmt. Work (Pool)");

            Task createdPoolTask = new Task()
                .Fake()
                .WithTaskCategoryRef(CPTaskCategory.ToEntityReference())
                .WithTypeRef(poolTaskType.ToEntityReference())
                .WithRegarding(incident.ToEntityReference());

            var listForInit = new List<Entity> { MITaskCategory, incident, openMITask, document, CPTaskCategory, poolTaskType, createdPoolTask, PPPDocumentType };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdPoolTask } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin and Asserts
            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<VerifyAllMissingInformationForPoolTask>(pluginContext));
            Assert.Contains("Before place Case into 'The Pool' please resolve the following issues:", ex.Message);
            Assert.Contains("There are some open Missing Information tasks in case.", ex.Message);
            Assert.Contains("Not all documents have Review Status 'Approved'.", ex.Message);
            Assert.Contains("The Post-Procedure Packet document is not generated or attached to case.", ex.Message);
            Assert.Contains("Actual Procedure Date should be populated.", ex.Message);

            #endregion
        }

        [Fact]
        public void Not_throw_exception_when_all_criteria_met()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create 'Pool' task with task category and task type, associated case with approved documents and closed Missing Information tasks,
            //with PPP document and actual DOS
            ipg_documenttype PPPDocumentType = new ipg_documenttype()
                .Fake("PPP", "PPP");

            ipg_taskcategory MITaskCategory = new ipg_taskcategory()
                .Fake()
                .WithName("Missing Information");

            Incident incident = new Incident()
                .Fake()
                .WithActualDos(DateTime.Now);

            Task closedMITask = new Task()
                .Fake()
                .WithState(TaskState.Completed)
                .WithRegarding(incident.ToEntityReference())
                .WithTaskCategoryRef(MITaskCategory.ToEntityReference());

            ipg_document pppDocument = new ipg_document()
                .Fake()
                .WithDocumentTypeReference(PPPDocumentType)
                .WithCaseReference(incident)
                .WithReviewStatus(ipg_document_ipg_ReviewStatus.Approved);

            ipg_document document = new ipg_document()
                .Fake()
                .WithReviewStatus(ipg_document_ipg_ReviewStatus.Approved)
                .WithCaseReference(incident);

            ipg_taskcategory CPTaskCategory = new ipg_taskcategory()
                .Fake()
                .WithName("Case Processing");

            ipg_tasktype poolTaskType = new ipg_tasktype()
                .Fake()
                .WithName("Request to Complete Case Mgmt. Work (Pool)");

            Task createdPoolTask = new Task()
                .Fake()
                .WithTaskCategoryRef(CPTaskCategory.ToEntityReference())
                .WithTypeRef(poolTaskType.ToEntityReference())
                .WithRegarding(incident.ToEntityReference());

            var listForInit = new List<Entity> { MITaskCategory, incident, closedMITask, document, pppDocument, CPTaskCategory, poolTaskType, createdPoolTask, PPPDocumentType };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdPoolTask } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin and Asserts
            fakedContext.ExecutePluginWith<VerifyAllMissingInformationForPoolTask>(pluginContext);
            #endregion
        }
    }
}
