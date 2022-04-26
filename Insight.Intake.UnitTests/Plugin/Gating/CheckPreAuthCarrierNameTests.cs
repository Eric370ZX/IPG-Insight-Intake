using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckPreAuthCarrierNameTests : PluginTestsBase
    {
        [Fact]
        public void CheckPreAuthCarrierNameTests_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident()
            {
                ipg_CarrierId = new EntityReference("ds", Guid.NewGuid()) { Name = "carrier1" }
            }.Fake();
            ipg_globalsetting carrierSetting = new ipg_globalsetting()
            {
                ipg_name = "Carrier name: Capital BlueCross",
                ipg_value = "carrier1"
            }.Fake();


            var listForInit = new List<Entity>() { caseEntity, carrierSetting };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPreAuthCarrierName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPreAuthCarrierName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
