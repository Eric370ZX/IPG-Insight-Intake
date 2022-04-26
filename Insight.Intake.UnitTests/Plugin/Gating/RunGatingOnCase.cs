using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class RunGatingOnCase : PluginTestsBase
    {
        [Fact(Skip ="Relevant for integation tests only")]
        //[Fact]
        public void RunGatingOnCaseTest()
        {

            var fakedContext = new XrmRealContext();
            var targetCase = new EntityReference(Incident.EntityLogicalName, new Guid("{C8AC664F-1579-EC11-8D21-00224809CFFA}"));
            var inputParameters = new ParameterCollection { { "Target", targetCase } };
            var outputParameters = new ParameterCollection { { "Succeeded", false },{ "AllowReject",false } };


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingStartGateProcessing",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = new Guid("066d36b8-a8be-e911-a836-000d3a31550e")//M Tikhonov
            };
            //Intake.Task task = new Task() {
            //    OwnerId = new EntityReference(Team.EntityLogicalName,new Guid("8c4ff2f9-5bd3-eb11-bacc-000d3a3b9cd7")),
            //    RegardingObjectId=new EntityReference(Incident.EntityLogicalName,new Guid("7415e30f-add8-eb11-bacb-000d3a5b0b29")),
            //    ipg_tasktypecodeEnum= ipg_TaskType1.CheckBandingRules
            //};
            //var taskId=fakedContext.GetOrganizationService().Create(task);


            fakedContext.ExecutePluginWith<GateProcessing>(pluginContext);
            Assert.True(true);
        }

    }
}
