using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class GatingResponse
    {

        public GatingResponse()
        {

        }
        public GatingResponse(bool succeeded, string caseNote = "", string portalNote = "", string gatingOutcome = "", int codeOutput = 0)
        {
            Succeeded = succeeded;
            CaseNote = caseNote;
            PortalNote = portalNote;
            GatingOutcome = gatingOutcome;
            CodeOutput = codeOutput;
        }

        public GatingResponse(ParameterCollection parameters, EntityReference workflowTask, OrganizationServiceContext crmContext)
        {
            Succeeded = (bool)parameters["Succeeded"];
            CustomMessage = parameters.Contains("NegativeMessage") ? (string)parameters["NegativeMessage"] : "";
            TaskSubject = parameters.Contains("TaskSubject") ? (string)parameters["TaskSubject"] : "";
            TaskDescripton = parameters.Contains("TaskDescripton") ? (string)parameters["TaskDescripton"] : "";
            GatingOutcome = parameters.Contains("GatingOutcome") ? (string)parameters["GatingOutcome"] : "";
            CodeOutput = parameters.Contains("CodeOutput") ? (int)parameters["CodeOutput"] : 0;
            CaseNote = parameters.Contains("CaseNote") ? (string)parameters["CaseNote"] : "";
            PortalNote = parameters.Contains("PortalNote") ? (string)parameters["PortalNote"] : "";
            if (workflowTask != null)
            {
                ipg_workflowtaskoutputconfig config = null;
                if (CodeOutput == 0)
                {
                    config = (from wtoc in crmContext.CreateQuery<ipg_workflowtaskoutputconfig>()
                              where wtoc.ipg_WorkflowTaskId.Id == workflowTask.Id
                              && wtoc.ipg_OutcomeType == Succeeded
                              select wtoc).FirstOrDefault();
                }
                else
                {
                    config = (from wtoc in crmContext.CreateQuery<ipg_workflowtaskoutputconfig>()
                              where wtoc.ipg_WorkflowTaskId.Id == workflowTask.Id
                              && wtoc.ipg_OutcomeType == Succeeded
                              && wtoc.ipg_CodeOutput == CodeOutput
                              select wtoc).FirstOrDefault();
                }
                if (config != null)
                {
                    this.CrmReason = config.ipg_CRMReason;
                    CaseNote += (!string.IsNullOrEmpty(config.ipg_CRMReason) ? Environment.NewLine + config.ipg_CRMReason : "");
                    PortalNote += (!string.IsNullOrEmpty(config.ipg_PortalReason) ? Environment.NewLine + config.ipg_PortalReason : "");
                    return;
                }
            }
        }

        public bool Succeeded { get; set; }
        public string CaseNote { get; set; }
        public string PortalNote { get; set; }
        public string CustomMessage { get; set; }
        public string TaskSubject { get; set; }
        public string TaskDescripton { get; set; }
        public string GatingOutcome { get; set; }
        public int CodeOutput { get; set; }
        public string CrmReason { get; set; }
    }


}
