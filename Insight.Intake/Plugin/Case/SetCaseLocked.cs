using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Case
{
    public class SetCaseLocked : PluginBase 
    {
        public SetCaseLocked() : base(typeof(SetCaseLocked))
        {
            RegisterEvent(PipelineStages.PreOperation, "Update", Incident.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.Target<Incident>();
            if (target.ipg_StateCodeEnum != ipg_CaseStateCodes.Billing)
            {
                return;
            }

            target.ipg_islocked = true;
        }
    }
}
