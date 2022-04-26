using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class ValidateEhrPatientAddressTests : PluginTestsBase
    {
        [Fact]
        public void SuccessIfNoTask()
        {
            //ARRANGE
            var fakedContext = new XrmFakedContext();

            var referral = new ipg_referral().Fake().Generate();

            var listForInit = new List<Entity>() {  referral  };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidateEhrPatientAddress",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<ValidateEhrPatientAddress>(pluginContext);

            //ASSERT
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void NegativeResult_If_TaskExists()
        {
            //ARRANGE
            var fakedContext = new XrmFakedContext();

            var incident = new Incident().Fake().Generate();
            var taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_OR_INVALID_PATIENT_ADDRESS).Generate();
            var task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithMetaTag("Warning message from EHR").WithRegarding(incident.ToEntityReference()).Generate();

            var listForInit = new List<Entity>() {  incident, taskType, task };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidateEhrPatientAddress",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<ValidateEhrPatientAddress>(pluginContext);

            //ASSERT
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.False(pluginContext.OutputParameters["Succeeded"] as bool?);
            Assert.True(pluginContext.OutputParameters.Contains("NegativeMessage") && pluginContext.OutputParameters["NegativeMessage"] is string);
            Assert.Equal(task.ipg_metatag, pluginContext.OutputParameters["NegativeMessage"] as string);
        }

        [Fact]
        public void NegativeResultWithTaskDescription_If_TaskExists()
        {
            //ARRANGE
            var fakedContext = new XrmFakedContext();

            var incident = new Incident().Fake().Generate();
            var taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_OR_INVALID_PATIENT_ADDRESS).Generate();
            var task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithDescription("Task description").WithRegarding(incident.ToEntityReference()).Generate();

            var listForInit = new List<Entity>() { incident, taskType, task };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidateEhrPatientAddress",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<ValidateEhrPatientAddress>(pluginContext);

            //ASSERT
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.False(pluginContext.OutputParameters["Succeeded"] as bool?);
            Assert.True(pluginContext.OutputParameters.Contains("NegativeMessage") && pluginContext.OutputParameters["NegativeMessage"] is string);
            Assert.Equal(task.Description, pluginContext.OutputParameters["NegativeMessage"] as string);
        }
    }
}
