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
    public class CreateTaskImportantEventLogTests : PluginTestsBase
    {
        [Fact]
        public void UpdateClaimGenerationTaskStartDate_GenerateImportantEventLog_ReturnSuccess()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            SystemUser fakedUser = new SystemUser().Fake();
            fakedUser.Id = Guid.NewGuid();
            fakedUser.FirstName = "John";
            fakedUser.LastName = "Smith";

            Incident fakedCase = new Incident().Fake();
            fakedCase.Id = Guid.NewGuid();
            fakedCase.Title = "Case Title Example";


            Task fakedTaskEntity = new Task().Fake();
            fakedTaskEntity.Id = Guid.NewGuid();
            fakedTaskEntity.Subcategory = "Subcategory Example";
            fakedTaskEntity.ipg_caseid = fakedCase.ToEntityReference();

            ipg_importanteventconfig fakedImportantEventConfig = new ipg_importanteventconfig()
            {
                Id = Guid.NewGuid(),
                ipg_name = "ET9",
                ipg_eventdescription = "Claim generation hold released by <User>",
                ipg_eventtype = "Claim Hold Released"
            };

            ipg_activitytype fakedActivityType = new ipg_activitytype()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Claim Hold Released"
            };

            fakedService.Create(fakedImportantEventConfig);
            fakedService.Create(fakedActivityType);

            var inputParameters = new ParameterCollection { { "Target", fakedTaskEntity } };
            var outputParameters = new ParameterCollection { { "Success", false } };

            fakedContext.Initialize(new List<Entity>() { fakedCase, fakedUser });

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                InitiatingUserId = fakedUser.Id,
                PreEntityImages = new EntityImageCollection() { { "PreImage", fakedTaskEntity } }
            };

            // Act
            fakedContext.ExecutePluginWith<CreateTaskImportantEventLog>(fakedPluginContext);

            // Assert
            Assert.True(fakedPluginContext.OutputParameters["Success"] as bool? == true);
        }

        [Fact]
        public void UpdateClaimGenerationTaskStartDateWithNoClaimHoldsSubCategory_GenerateImportantEventLog_ReturnFalse()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            Task fakedTaskEntity = new Task().Fake();
            fakedTaskEntity.Subcategory = "No Claim Holds";

            var inputParameters = new ParameterCollection { { "Target", fakedTaskEntity } };
            var outputParameters = new ParameterCollection { { "Success", false } };

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                OutputParameters = outputParameters,
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection() { { "PreImage", fakedTaskEntity } }
            };

            // Act
            fakedContext.ExecutePluginWith<CreateTaskImportantEventLog>(fakedPluginContext);

            // Assert
            Assert.True(fakedPluginContext.OutputParameters["Success"] as bool? == false);
        }
    }
}
