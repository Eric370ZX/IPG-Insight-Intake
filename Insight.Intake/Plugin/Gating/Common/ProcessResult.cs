using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.Common
{
    public class ProcessResult
    {
        public bool successed { get; set; }
        public Entity gateconfigdetail { get; set; }
        public int severityLevel { get; set; }
        public string caseNote { get; set; }
        public string portalNote { get; set; }
        public string resultMessage { get; set; }
        public string gatingOutcome { get; set; }
        public ipg_caseworkflowtask caseWorkflowTask { get; set; }
        public int codeOutput { get; set; }
        public EntityReference workflowTask { get; set; }

        public ProcessResult()
        {
            codeOutput = 0;
        }
    }

}
