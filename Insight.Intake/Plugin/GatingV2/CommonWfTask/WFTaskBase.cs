using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.GatingV2.CommonWfTask
{
    public abstract class WFTaskBase
    {
        //protected readonly Guid wfTaskId;

        //public WFTaskBase(Guid wfTaskId)
        //{
        //    this.wfTaskId = wfTaskId;
        //}
        
        public abstract WFTaskResult Run(WFTaskContext ctx);
    }
}
