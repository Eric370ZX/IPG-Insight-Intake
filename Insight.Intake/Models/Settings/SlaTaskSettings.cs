using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Models.Settings
{
    public class SlaTaskSettings
    {
        public int CreateRefferalDueDateOffset { get; set; }

        public int DecisionStatCaseDueDateOffset { get; set; }

        public int DecisionStandardCaseDueDateOffset { get; set; }

        public int DecisionUrgentCaseDueDateOffset { get; set; }

        public int DecisionRetroCaseDueDateOffset { get; set; }

        public int GeneratePoDueDateOffset { get; set; }

        public int PayProviderDueDateOffset { get; set; }
    }
}
