using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.GatingV2.CommonWfTask
{
    public class WFTaskResult
    {

        public WFTaskResult()
        {

        }
        public WFTaskResult(bool succeeded, string caseNote = "", string portalNote = "", string gatingOutcome = "")
        {
            Succeeded = succeeded;
            CaseNote = caseNote;
            PortalNote = portalNote;
            GatingOutcome = gatingOutcome;
        }
        //public WFTaskResult(ParameterCollection parameters)
        //{
        //    Succeeded = (bool)parameters["Succeeded"];
        //    CaseNote = parameters.Contains("CaseNote") ? (string)parameters["CaseNote"] : "";
        //    PortalNote = parameters.Contains("PortalNote") ? (string)parameters["PortalNote"] : "";
        //    CustomMessage = parameters.Contains("NegativeMessage") ? (string)parameters["NegativeMessage"] : "";
        //    TaskSubject = parameters.Contains("TaskSubject") ? (string)parameters["TaskSubject"] : "";
        //    TaskDescripton = parameters.Contains("TaskDescripton") ? (string)parameters["TaskDescripton"] : "";
        //    GatingOutcome = parameters.Contains("GatingOutcome") ? (string)parameters["GatingOutcome"] : "";
        //}

        public bool Succeeded { get; set; }
        public string CaseNote { get; set; }
        public string PortalNote { get; set; }
        public string CustomMessage { get; set; }
        public string TaskSubject { get; set; }
        public string TaskDescripton { get; set; }
        public string GatingOutcome { get; set; }
    }
}
