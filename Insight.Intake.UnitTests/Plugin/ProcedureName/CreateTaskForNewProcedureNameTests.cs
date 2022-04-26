using FakeXrmEasy;
using Insight.Intake.Plugin.ProcedureName;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.ProcedureName
{
    public class CreateTaskForNewProcedureNameTests : PluginTestsBase
    {

        [Fact]
        public void CreateTaskForNewProcedureName_ThereIsNewProcedureName_returnTaskIsCreated()
        {
            var fakedContext = new XrmFakedContext();

            var procedureName = new ipg_procedurename()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Test Procedure"
               ,
                ipg_Description = "Description"
            };

            fakedContext.Initialize(new List<Entity> { procedureName });

            var inputParameters = new ParameterCollection { { "Target", procedureName } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = ipg_procedurename.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CreateTaskForNewProcedureName>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in crmContext.CreateQuery<Task>()
                         where taskRecord.RegardingObjectId.Id == procedureName.Id
                         select taskRecord).ToList();

            Assert.True(tasks.Count == 1);
        }
    }
}