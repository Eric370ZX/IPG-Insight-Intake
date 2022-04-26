using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Xunit;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Plugin.Document;
using System;

namespace Insight.Intake.UnitTests.Plugin.Document
{

    public class AutoCloseCaseTasksTests : PluginTestsBase
    {
        private static readonly Guid MissingInformation = new Guid("7647bc90-4dfe-ea11-a815-000d3a3156c1");
        [Fact]
        public void AutoCloseCaseTasks__WhenUploadDocumentWithMissingInformationTasks_UpdateStateCodeToCompleted()
        {
            #region Arrange

            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake();
            Incident incident = new Incident().Fake();
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().RuleFor(c => c.Id, s => MissingInformation);


            var targetEntity = new ipg_document().Fake();
            targetEntity.WithCaseReference(incident)
            .WithDocumentTypeReference(documentType);

            var task = new Task().Fake();
            task.WithCase(incident.ToEntityReference())
                .WithDocumentType(documentType.ToEntityReference())
                .WithTaskCategory(taskCategory.ToEntityReference());

            var listForInit = new List<Entity> { task, targetEntity };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", targetEntity.Generate() } },
                PostEntityImages = new EntityImageCollection() { { "PostImage", targetEntity.Generate() } },
                OutputParameters = new ParameterCollection()
            };
            #endregion

            #region Act

            fakedContext.ExecutePluginWith<AutoCloseCaseTasks>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            var query = new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression( Task.Fields.ipg_caseid, ConditionOperator.Equal, incident.Id)
                    }
                }
            };
            //retrieve all related documents to Case
            var tasks = fakedService.RetrieveMultiple(query).Entities;
            var actual = tasks.FirstOrDefault()?.ToEntity<Task>();
            var expectedStateCode = TaskState.Completed;
            var expectedStatusCode = new OptionSetValue((int)Task_StatusCode.Resolved);
            #endregion

            #region Assert

            Assert.Equal(expectedStateCode, actual?.StateCode);
            Assert.Equal(expectedStatusCode, actual?.StatusCode);

            #endregion
        }
    }
}
