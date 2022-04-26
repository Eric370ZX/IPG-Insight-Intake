using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class UnlockCaseActionTest: PluginTestsBase
    {
        [Fact]
        public void UnlockCaseTest()
        {
            var fakedContext = new XrmFakedContext();

            Incident target = new Incident().Fake().WithLock(true);
            ipg_importanteventconfig eventConfig = new ipg_importanteventconfig().Fake(Constants.EventIds.ET10, "Case Unlocked", "Case unlocked for changes due to <Facility Error or Internal Error>. <Unlock Case Notes>.", "Case is unlocked by user");

            fakedContext.Initialize(new List<Entity> { target, eventConfig });
            
            var request = new ipg_IPGCaseActionsUnlockCaseRequest() { Target = target.ToEntityReference(), Reason = "Facility Error", Notes = "12" };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = request.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = request.Parameters
            };

            fakedContext.ExecutePluginWith<UnlockCaseAction>(pluginContext);

            var context = new CrmServiceContext(fakedContext.GetOrganizationService());
            
            var et = context.ipg_importanteventslogSet.FirstOrDefault();
            var incident = context.IncidentSet.FirstOrDefault();

            Assert.NotNull(et);
            Assert.Contains(request.Reason, et.ipg_activitydescription);
            Assert.Contains(request.Notes, et.ipg_activitydescription);
            Assert.False(incident.ipg_islocked);
        }
    }
}
