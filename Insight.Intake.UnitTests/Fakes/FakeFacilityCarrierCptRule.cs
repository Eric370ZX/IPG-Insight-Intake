using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeFacilityCarrierCptRule
    {
        public static Faker<ipg_facilitycarriercptrule> Fake(this ipg_facilitycarriercptrule self)
        {
            return new Faker<ipg_facilitycarriercptrule>()
                       .RuleFor(x => x.Id, x => Guid.NewGuid())
                       .RuleFor(x => x.StatusCode, x => new OptionSetValue((int)ipg_facilitycarriercptrule_StatusCode.Active))
                       .RuleFor(x => x.StatusCodeEnum, x => ipg_facilitycarriercptrule_StatusCode.Active);
        }

        public static Faker<ipg_facilitycarriercptrule> FakeWithCptCode(this Faker<ipg_facilitycarriercptrule> self, ipg_cptcode cptCode)
        {
            return self.RuleFor(x => x.ipg_CptId, x => new EntityReference()
            {
                Id = cptCode.Id,
                LogicalName = cptCode.LogicalName,
                Name = cptCode.ipg_name
            });
        }

        public static Faker<ipg_facilitycarriercptrule> FakeWithContract(this Faker<ipg_facilitycarriercptrule> self, ipg_facilitycarriercptrulecontract contract)
        {
            return self.RuleFor(x => x.ipg_FacilityCarrierCPTRuleContractId, x => new EntityReference()
            {
                Id = contract.Id,
                LogicalName = contract.LogicalName,
                Name = contract.ipg_name
            });
        }

        public static Faker<ipg_facilitycarriercptrule> WithEffectivesDates(this Faker<ipg_facilitycarriercptrule> self, DateTime startDate, DateTime endDate)
        {
            return self.RuleFor(x => x.ipg_EffectiveDate, x => startDate)
                .RuleFor(x => x.ipg_ExpirationDate, x => endDate);
        }
        
    }
}
