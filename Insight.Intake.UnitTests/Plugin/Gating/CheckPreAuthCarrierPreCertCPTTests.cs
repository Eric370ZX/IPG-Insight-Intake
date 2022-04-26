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
    public class CheckPreAuthCarrierPreCertCPTTests : PluginTestsBase
    {
        [Fact]
        public void CheckPreAuthCarrierPreCertCPT_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            var carrier = new Intake.Account()
                .Fake();
            var cptCode = new ipg_cptcode()
                .Fake();

            Incident caseEntity = new Incident()
                .Fake()
                .WithCarrierReference(carrier)
                .FakeWithCptCode(cptCode)
                .RuleFor(p => p.ipg_ActualDOS, DateTime.Now);

            ipg_carrierprecertcpt CarrierPrecertCPT = new ipg_carrierprecertcpt()
                .Fake()
                .FakeWithCarrier(carrier)
                .FakeWithCptCode(cptCode)
                .FakeWithEffectiveStartAndEndDate(DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1));


            var listForInit = new List<Entity>() { caseEntity, CarrierPrecertCPT };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPreAuthCarrierPreCertCPT",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPreAuthCarrierPreCertCPT>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
