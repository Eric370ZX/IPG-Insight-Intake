using FakeXrmEasy;
using Insight.Intake.Models;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class TriggerEHRUpdateFeedProcessTest : PluginTestsBase
    {
        [Fact]
        public void TriggerEHRUpdateFeedProcess_ValidationFails()
        {
            var fakedContext = new XrmFakedContext();
            Incident targetIncident = new Incident().Fake();
            targetIncident.ipg_EHRUpdate = false;

            Incident postImageIncident = new Incident().Fake();
            postImageIncident.ipg_lifecyclestepid = new EntityReference(ipg_lifecyclestep.EntityLogicalName, LifecycleStepsConstants.CalculateRevenueGate7);

            var listForInit = new List<Entity>() { targetIncident};
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", targetIncident } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };
            var postImage = new EntityImageCollection { { "PostImage", postImageIncident } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = postImage,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<TriggerEHRUpdateFeedProcess>(pluginContext);

            Assert.NotEqual(new EntityReference(ipg_lifecyclestep.EntityLogicalName, LifecycleStepsConstants.AddPartsGate6), targetIncident.ipg_lifecyclestepid);
        }
    }
}
