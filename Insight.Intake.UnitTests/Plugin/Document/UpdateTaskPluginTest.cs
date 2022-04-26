using FakeXrmEasy;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class UpdateTaskPluginTest
    {
        [Fact]
        public void TaskReviewStatusRejectedOnDocumentRejected()
        {
            var fakedContext = new XrmFakedContext();

            Task task = new Task().Fake();
            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");

            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithOriginatingTaskReference(task)
                .WithReviewStatus(ipg_document_ipg_ReviewStatus.Rejected);


            var listForInit = new List<Entity> { documentType, document, task };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };
            var postImages = new EntityImageCollection { { "PostImage", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = postImages,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpdateTaskPlugin>(pluginContext);
            
            task = fakedContext.GetOrganizationService().Retrieve(task.LogicalName, task.Id, new ColumnSet(true)).ToEntity<Task>();

            Assert.Equal(Task_ipg_reviewstatuscode.Rejected, task.ipg_reviewstatuscodeEnum);
        }
        
        [Fact]
        public void TaskReviewStatusApprowedOnDocumentApproved()
        {
            var fakedContext = new XrmFakedContext();

            Task task = new Task().Fake();
            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");

            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithOriginatingTaskReference(task)
                .WithReviewStatus(ipg_document_ipg_ReviewStatus.Approved);


            var listForInit = new List<Entity> { documentType, document, task };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };
            var postImages = new EntityImageCollection { { "PostImage", document } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = postImages,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpdateTaskPlugin>(pluginContext);

            task = fakedContext.GetOrganizationService().Retrieve(task.LogicalName, task.Id, new ColumnSet(true)).ToEntity<Task>();

            Assert.Equal(Task_ipg_reviewstatuscode.Approved, task.ipg_reviewstatuscodeEnum);
        }
        [Fact]
        public void TaskPendingReviewOnPendingReviewDocument()
        {
            var fakedContext = new XrmFakedContext();

            Task task = new Task().Fake();
            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");

            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithOriginatingTaskReference(task)
                .WithReviewStatus(ipg_document_ipg_ReviewStatus.PendingReview);


            var listForInit = new List<Entity> { documentType, document, task };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };
            var postImages = new EntityImageCollection { { "PostImage", document } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = postImages,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpdateTaskPlugin>(pluginContext);

            task = fakedContext.GetOrganizationService().Retrieve(task.LogicalName, task.Id, new ColumnSet(true)).ToEntity<Task>();

            Assert.Equal(Task_ipg_reviewstatuscode.PendingReview, task.ipg_reviewstatuscodeEnum);
        }
    }
}
