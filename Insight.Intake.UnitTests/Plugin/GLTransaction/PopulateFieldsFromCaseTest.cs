using FakeXrmEasy;
using Insight.Intake.Plugin.GLTransaction;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.GLTransaction
{
    public class PopulateFieldsFromCaseTest : PluginTestsBase
    {
        [Fact]
        public void SetFieldsFromCaseTest()
        {
            var fakedContext = new XrmFakedContext();
            ServiceProvider = ServiceProviderMock.Object;
           
            ipg_carriernetwork network = new ipg_carriernetwork().Fake();
            Incident incident = new Incident().Fake();
            incident.ipg_carriernetwork = network.ToEntityReference();
            incident.ipg_carriernetwork.Name = "Test";
            
            ipg_GLTransaction gLTransaction = new ipg_GLTransaction().Fake().WithCase(incident);

            
            var listForInit = new List<Entity> { network, incident, gLTransaction};
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", gLTransaction } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = ipg_GLTransaction.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<PopulateFieldsFromCase>(pluginContext);

            Assert.Equal(gLTransaction.ipg_NetworkType, incident.ipg_carriernetwork.Name);
        }
    }
}
