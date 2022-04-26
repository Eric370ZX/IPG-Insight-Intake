using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeHomePlanCarrierMap
    {
        public static ipg_homeplancarriermap FakeForDeriveHomePlan(this ipg_homeplancarriermap homeplancarriermap,
            Account carrier,DateTime effectiveDate, DateTime endDate,bool medicareAdvantage = true, string code = "AAK")
        {
            return new ipg_homeplancarriermap
            {
                Id = Guid.NewGuid(),
                LogicalName = ipg_homeplancarriermap.EntityLogicalName,
                ipg_name = "BCBS",
                ipg_CarrierId = new EntityReference
                {
                    Id = carrier?.Id ?? Guid.Empty,
                    LogicalName = Account.EntityLogicalName
                },
                ipg_code = code,
                ipg_EffectiveDate = effectiveDate,
                ipg_Enddate = endDate,
                ipg_MedicareAdvantage = medicareAdvantage
            };
        }
    }
}
