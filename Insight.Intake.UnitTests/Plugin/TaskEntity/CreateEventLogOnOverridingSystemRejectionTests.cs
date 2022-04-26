using FakeXrmEasy;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class CreateEventLogOnOverridingSystemRejectionTests
    {
        [Fact]
        public void ImportantEventLogCreated()
        {
            //ARRANGE
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            var incident = new Incident().Fake().Generate();
            var taskBefore = new Task().Fake().WithCase(incident.ToEntityReference()).Generate();
            taskBefore.ipg_is_exception_approved = false;
            var taskAfter = new Task().Fake().WithCase(incident.ToEntityReference()).Generate();
            taskAfter.ipg_is_exception_approved = true;

            var fakedImportantEventConfig = new ipg_importanteventconfig()
            {
                Id = new Guid(),
                ipg_name = "ET25",
                ipg_eventdescription = "System-recommended rejection overridden.",
                ipg_eventtype = "Rejection Override"
            };

            var fakedActivityType = new ipg_activitytype()
            {
                Id = new Guid(),
                ipg_name = "Rejection Override"
            };

            fakedService.Create(fakedImportantEventConfig);
            fakedService.Create(fakedActivityType);

            var listForInit = new List<Entity>() { incident, taskBefore, taskAfter };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", taskBefore } };

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection() { { "PreImage", taskBefore } },
                PostEntityImages = new EntityImageCollection() { { "PostImage", taskAfter } }
            };

            //ACT
            fakedContext.ExecutePluginWith<CreateEventLogOnOverridingSystemRejection>(fakedPluginContext);
            var query = new QueryExpression(ipg_importanteventslog.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false)
            };
            var resultLog = fakedService.RetrieveMultiple(query).Entities.SingleOrDefault();

            //ASSERT
            Assert.NotNull(resultLog);
        }
    }
}