using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.CPTValidation
{
    public class CPTValidationResult
    {
        public string Output { get; set; }
        public List<Guid> ValidCPTCodes { get; set; }
    }
}
