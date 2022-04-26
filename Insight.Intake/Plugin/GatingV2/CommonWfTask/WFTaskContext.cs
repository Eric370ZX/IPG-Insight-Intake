using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.GatingV2.CommonWfTask
{
    public class WFTaskContext
    {
        public IOrganizationService CrmService { get; set; }
        public ITracingService TraceService { get; set; }
        public GatingContext dbContext { get; set; }
    }
}
