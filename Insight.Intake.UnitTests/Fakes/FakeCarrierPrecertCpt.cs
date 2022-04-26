using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeCarrierPrecertCpt
    {
        public static Faker<ipg_carrierprecertcpt> Fake(this ipg_carrierprecertcpt self)
        {
            return new Faker<ipg_carrierprecertcpt>()
                       .RuleFor(x => x.Id, x => Guid.NewGuid())
                       .RuleFor(x => x.StateCode, x => ipg_carrierprecertcptState.Active)
                       .RuleFor(x => x.ipg_RequirementType, x => new OptionSetValue((int)ipg_CarrierPrecertCPTRequirementType.CPTLOMN));
        }

        public static Faker<ipg_carrierprecertcpt> FakeWithCptCode(this Faker<ipg_carrierprecertcpt> self, ipg_cptcode cptCode)
        {
            return self.RuleFor(x => x.ipg_CPTId, x => new EntityReference()
            {
                Id = cptCode.Id,
                LogicalName = cptCode.LogicalName,
                Name = cptCode.ipg_name
            });
        }

        public static Faker<ipg_carrierprecertcpt> FakeWithCarrier(this Faker<ipg_carrierprecertcpt> self, Account carrier)
        {
            return self.RuleFor(x => x.ipg_CPTId, x => new EntityReference()
            {
                Id = carrier.Id,
                LogicalName = carrier.LogicalName,
                Name = carrier.Name
            });
        }

        public static Faker<ipg_carrierprecertcpt> FakeWithRequirementType(this Faker<ipg_carrierprecertcpt> self, int requirementType)
        {
            return self.RuleFor(x => x.ipg_RequirementType, x => new OptionSetValue(requirementType));
        }

        public static Faker<ipg_carrierprecertcpt> FakeWithEffectiveStartAndEndDate(this Faker<ipg_carrierprecertcpt> self, DateTime effectiveDate, DateTime expirationDate)
        {
            return self.RuleFor(x => x.ipg_EffectiveStartDate, x => effectiveDate)
                       .RuleFor(x => x.ipg_EffectiveEndDate, x => expirationDate);
        }
    }
}
