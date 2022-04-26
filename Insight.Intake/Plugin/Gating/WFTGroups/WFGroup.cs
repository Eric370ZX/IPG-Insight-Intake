using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.Common
{
    public class WFGroup
    {
        public ipg_wftaskgroup Group { get; set; }
        public List<ipg_gateconfigurationdetail> GateDetails { get; set; }
    }
}
