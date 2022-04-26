using System;
using System.Collections.Generic;
using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class CreateImportantEventLogCaseReopenedTests : PluginTestsBase
    {
        [Theory]
        [InlineData((ipg_CaseStatus.Open))]
        [InlineData((ipg_CaseStatus.Closed))]
        public void CaseIsReopened_ShouldCreateImportantEventLog(ipg_CaseStatus caseStatus)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();

            Incident fakedCase = new Incident().Fake();
            fakedCase.Id = Guid.NewGuid();
            fakedCase.Title = "Test case";
            fakedCase.ipg_CaseStatus = new OptionSetValue((int)caseStatus);

            SystemUser fakedUser = new SystemUser().Fake();
            fakedUser.Id = Guid.NewGuid();
            fakedUser.FirstName = "John";
            fakedUser.LastName = "Smith";

            ipg_importanteventconfig fakedImportantEventConfig = new ipg_importanteventconfig()
            {
                Id = Guid.NewGuid(),
                ipg_name = "ET27",
                ipg_eventdescription = "Closed Case record reopened",
                ipg_eventtype = "Case Reopened"
            };

            ipg_activitytype fakedActivityType = new ipg_activitytype()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Case Reopened"
            };

            fakedContext.Initialize(new List<Entity> { fakedCase, fakedImportantEventConfig, fakedActivityType });

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                PostEntityImages = new EntityImageCollection { { "PostImage", fakedCase } },
                UserId = fakedUser.Id,
                InputParameters = new ParameterCollection { { "Target", fakedCase } },
                OutputParameters = new ParameterCollection { { "Success", false } }
            };

            // Act
            fakedContext.ExecutePluginWith<CreateImportantEventLogCaseReopened>(fakedPluginContext);

            // Assert
            switch (caseStatus)
            {
                case ipg_CaseStatus.Closed:
                    Assert.True(fakedPluginContext.OutputParameters["Success"] as bool? == false);
                    break;
                case ipg_CaseStatus.Open:
                    Assert.True(fakedPluginContext.OutputParameters["Success"] as bool? == true);
                    break;
                default:
                    break;
            }
        }
    }
}
