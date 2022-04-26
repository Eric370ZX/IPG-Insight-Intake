using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Insight.Intake.Data;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class CaseIsClosedPluginTest : PluginTestsBase
    {
        [Fact]
        public void CaseIsClosed_Success()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Closed);
            Task relTask = new Task().Fake()
                .RuleFor(p => p.ipg_caseid, p => caseEntity.ToEntityReference())
                .RuleFor(p=>p.StateCode, p=>TaskState.Open);
            relTask.ipg_taskcategorycodeEnum = ipg_Taskcategory1.System;
            
            ipg_statementgenerationtask psEventsTask = new ipg_statementgenerationtask().Fake().WithCaseReference(caseEntity);

            var listForInit = new List<Entity>() { caseEntity, relTask, psEventsTask };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity },
            };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 10,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CaseIsClosed>(pluginContext);
            
            var service = fakedContext.GetOrganizationService();
            var resultTask= service
                .Retrieve(Task.EntityLogicalName, relTask.Id, new ColumnSet("statecode"))
                .ToEntity<Task>();

            var psresultTask = service
             .Retrieve(ipg_statementgenerationtask.EntityLogicalName, psEventsTask.Id, new ColumnSet(ipg_statementgenerationtask.Fields.StatusCode))
             .ToEntity<ipg_statementgenerationtask>();
            //Assert

            Assert.Equal(TaskState.Canceled, resultTask.StateCode);
            Assert.Equal(ipg_statementgenerationtask_StatusCode.Canceled, psresultTask.StatusCodeEnum);
        }
    }
}