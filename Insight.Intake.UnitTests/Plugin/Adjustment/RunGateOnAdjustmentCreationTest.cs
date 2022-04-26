using FakeXrmEasy;
using Insight.Intake.Plugin.Adjustment;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.UnitTests.Plugin.Adjustment
{
    public class RunGateOnAdjustmentCreationTest : PluginTestsBase
    {
        [Fact]
        public void CheckGateRunOnAdjsutmentWhenCaseIn_Collection()
        {
            var GateRunFor = new List<EntityReference>();
            var fakedContext = new XrmFakedContext();
            var gateConfig = new ipg_gateconfiguration().Fake("11");
            ipg_lifecyclestep ps_ServiceLF = new ipg_lifecyclestep().Fake(gateConfig, Settings.PS_Service_LF_Step_Id);
            ipg_lifecyclestep collectionLF = new ipg_lifecyclestep().Fake(gateConfig, Settings.Collection_LF_Step_Id);
            var ps_Service_GL_Setting = new ipg_globalsetting().Fake().WithName(Settings.PS_Service_LF_Step_Id).WithValue(ps_ServiceLF.Id.ToString());
            var collectionLF_GL_Setting = new ipg_globalsetting().Fake().WithName(Settings.Collection_LF_Step_Id).WithValue(collectionLF.Id.ToString());
            Incident incident = new Incident().Fake().WithLFStep(collectionLF);
            Incident fromincident = new Incident().Fake().WithLFStep(collectionLF); ;
            ipg_adjustment adjustment = new ipg_adjustment().FakeTransferofPayment().WithCase(incident).WithFromCase(fromincident);

            fakedContext.Initialize(new List<Entity>() { gateConfig, ps_ServiceLF, collectionLF, incident, fromincident, ps_Service_GL_Setting, collectionLF_GL_Setting });

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", adjustment } }
            };

            fakedContext.AddFakeMessageExecutor<ipg_IPGGatingStartGateProcessingRequest>(new FakeMessageExecutor<ipg_IPGGatingStartGateProcessingRequest, ipg_IPGGatingStartGateProcessingResponse>(
            (req, ctx) =>
            {
                GateRunFor.Add(req.Target);
                return new ipg_IPGGatingStartGateProcessingResponse();
            }));

            fakedContext.ExecutePluginWith<AdjustmentGateExecution>(pluginContext);

            Assert.Contains(incident.ToEntityReference(), GateRunFor);
            Assert.Contains(fromincident.ToEntityReference(), GateRunFor);
        }
    }
}
