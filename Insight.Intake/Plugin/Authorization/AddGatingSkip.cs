using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Authorization
{
    public class AddGatingSkip : PluginBase
    {
        private const int AUTHORIZATION_REQUIRED_TASK_TYPE_ID = 1123;

        public AddGatingSkip() : base(typeof(CaseRecentAuthorization))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                MessageNames.Create,
                ipg_authorization.EntityLogicalName,
                OnCreatePostOperation);
        }

        private void OnCreatePostOperation(LocalPluginContext pluginContext)
        { 
        
        }
    }
}
