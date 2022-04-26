using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.GatingDocumentActions
{
    //Doesn't have dependencies
    //TDO: remove from d365 and assembly
    public class TestActionAlwaysTrue : PluginBase
    {
        public TestActionAlwaysTrue() : base(typeof(TestActionAlwaysTrue))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingDocumentActionsTestActionAlwaysTrue", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
        }
    }
}
