using FakeXrmEasy;
using Insight.Intake.Plugin.ProcedureName;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.ProcedureName
{
    public class CheckProcedureNameForDuplicateTests : PluginTestsBase
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

            var procedureNameDuplicate = new ipg_procedurename()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Test Procedure"
               ,
                ipg_Description = "Description"
            };

            fakedContext.Initialize(new List<Entity> { procedureName, procedureNameDuplicate });

            var inputParameters = new ParameterCollection { { "Target", procedureName } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = ipg_procedurename.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<CheckProcedureNameForDuplicate>(pluginContext));
            Assert.Equal(string.Format("Error: The Procedure Name {0} already exists!", procedureName.ipg_name), ex.Message);
        }
    }
}