using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Common.Interfaces
{
    public interface ICaseBenefitSwitcher
    {
        void UpdateInOutNetwork(Guid incidentId, Guid carrierId);
    }
}
