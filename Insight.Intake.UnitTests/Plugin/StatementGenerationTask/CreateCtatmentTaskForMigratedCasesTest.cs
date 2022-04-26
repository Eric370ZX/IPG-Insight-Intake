using FakeXrmEasy;
using Insight.Intake.Plugin.StatementGenerationTask;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.UnitTests.Plugin.StatementGenerationTask
{
    public class CreateCtatmentTaskForMigratedCasesTest : PluginTestsBase
    {
        [Fact]
        public void StatementTaskiCreateTest()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            ipg_statementgenerationeventconfiguration statementType = new ipg_statementgenerationeventconfiguration().Fake();
            statementType.StateCode = (int)ipg_statementgenerationeventconfigurationState.Active;
            statementType.ipg_name = "Test statementName";

            SystemUser systemUser = new SystemUser().Fake(UserNames.ImportUserName);
            systemUser[SystemUser.Fields.FullName] = UserNames.ImportUserName;

            Incident incident = new Incident().Fake()
                .WithCaseStatus((int)ipg_CaseStatus.Open)
                .WithState((int)ipg_CaseStateCodes.CarrierServices)
                .WithLastStatementType(statementType);

            incident[Incident.Fields.CreatedBy] = systemUser.ToEntityReference();
            incident.ipg_LastStatementType.Name = "Test statementName";

            var listForInit = new List<Entity>() { incident, systemUser, statementType };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            
            //ACT
            fakedContext.ExecutePluginWith<CreateCtatmentTaskForMigratedCases>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var task = (from statementTask in crmContext.CreateQuery<ipg_statementgenerationtask>()
                          where statementTask.ipg_eventid.Id == statementType.Id
                          select statementTask).FirstOrDefault();

            Assert.NotNull(task);
        }
    }
}
