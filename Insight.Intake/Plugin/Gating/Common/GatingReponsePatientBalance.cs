using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.Common
{
    public class GatingReponsePatientBalance : GatingResponse
    {
        public GatingReponsePatientBalance(bool succeeded, string caseNote = "", string portalNote = "", string subject=""):base(succeeded,caseNote,portalNote)
        {
            Subject = subject;
        }
        public string Subject { get; set; }
    }
}
