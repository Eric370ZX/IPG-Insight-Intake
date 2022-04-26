using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeFacilityCarrierCPTRuleContract
    {
        public static Faker<ipg_facilitycarriercptrulecontract> Fake(this ipg_facilitycarriercptrulecontract self)
        {
            return new Faker<ipg_facilitycarriercptrulecontract>()
                       .RuleFor(x => x.Id, x => Guid.NewGuid())
                       .RuleFor(x => x.StatusCode, x => ipg_facilitycarriercptrulecontract_StatusCode.Active);
            //.RuleFor(x => x.StatusCodeEnum, x => ipg_facilitycarriercptrulecontract_StatusCode.Active);
        }


        public static Faker<ipg_facilitycarriercptrulecontract> FakeWithEntitlement(this Faker<ipg_facilitycarriercptrulecontract> self, Entitlement entitlement)
        {
            return self.RuleFor(x => x.ipg_EntitlementId, x => new EntityReference()
            {
                Id = entitlement.Id,
                LogicalName = entitlement.LogicalName,
                Name = entitlement.Name
            });
        }

        public static Faker<ipg_facilitycarriercptrulecontract> WithCPTInclusionType(this Faker<ipg_facilitycarriercptrulecontract> self, ipg_cptinclusiontyperule cptInclusionType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_CPTInclusionType, x => cptInclusionType);

            return self;
        }

        public static Faker<ipg_facilitycarriercptrulecontract> WithEffectivesDates(this Faker<ipg_facilitycarriercptrulecontract> self, DateTime startDate, DateTime endDate)
        {
            return self.RuleFor(x => x.ipg_EffectiveDate, x => startDate)
                .RuleFor(x => x.ipg_ExpirationDate, x => endDate);
        }

    }
}
