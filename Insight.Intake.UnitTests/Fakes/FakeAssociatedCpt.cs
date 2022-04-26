using System;
using Bogus;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeAssociatedCpt
    {
        public static Faker<ipg_associatedcpt> Fake(this ipg_associatedcpt self)
        {
            return new Faker<ipg_associatedcpt>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_Supported, x => x.Random.Bool());
        }

        public static Faker<ipg_associatedcpt> WithCptCodeReference(this Faker<ipg_associatedcpt> self, ipg_cptcode cptCode)
        {
            self.RuleFor(
                x => x.ipg_CPTCodeId, 
                x => new EntityReference
                {
                    Id = cptCode.Id,
                    LogicalName = cptCode.LogicalName
                }
            );

            return self;
        }
        
        /*public static Faker<ipg_associatedcpt> WithFacilityReference(this Faker<ipg_associatedcpt> self, Account facility)
        {
            self.RuleFor(
                x => x.ipg_FacilityId, 
                x => new EntityReference
                {
                    Id = facility.Id,
                    LogicalName = facility.LogicalName
                }
            );

            return self;
        }*/

        public static Faker<ipg_associatedcpt> WithCarrierReference(this Faker<ipg_associatedcpt> self, Account carrier)
        {
            self.RuleFor(
                x => x.ipg_CarrierId,
                x => new EntityReference
                {
                    Id = carrier.Id,
                    LogicalName = carrier.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_associatedcpt> WithSupportedCptCodeFlag(this Faker<ipg_associatedcpt> self, bool isSupported)
        {
            self.RuleFor(x => x.ipg_Supported, x => isSupported);
            
            return self;
        }

        public static Faker<ipg_associatedcpt> WithEffectiveDate(this Faker<ipg_associatedcpt> self, DateTime date)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_EffectiveDate, x => date);
            return self;
        }

        public static Faker<ipg_associatedcpt> WithExpirationDate(this Faker<ipg_associatedcpt> self, DateTime date)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_ExpirationDate, x => date);
            return self;
        }
    }
}