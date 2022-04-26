using System;
using Bogus;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeEntitilementRelationship
    {
        public static Faker<Entitlement> Fake(this Entitlement self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return new Faker<Entitlement>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_CarrierUnsupported, false)
                .RuleFor(x => x.StateCode, EntitlementState.Active);
        }

        public static Faker<Entitlement> Fake(this Entitlement self, EntitlementState entitlementState)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            return new Faker<Entitlement>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_CarrierUnsupported, false)
                .RuleFor(x => x.StateCode, entitlementState)
                .RuleFor(x => x.StatusCode, new OptionSetValue((int)entitlementState));
        }

        public static Faker<Entitlement> WithFacilityReference(this Faker<Entitlement> self, Account facility)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_FacilityId, x => new EntityReference(facility.LogicalName, facility.Id));

            return self;
        }

        public static Faker<Entitlement> WithCarrierReference(this Faker<Entitlement> self, Account carrier)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_CarrierId,
                x => new EntityReference(
                    carrier.LogicalName,
                    carrier.Id
                )
            );

            return self;
        }

        public static Faker<Entitlement> WithEffectiveDateRange(this Faker<Entitlement> self, DateTime from, DateTime thru)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.StartDate, x => from)
                .RuleFor(x => x.EndDate, x => thru);

            return self;
        }

        public static Faker<Entitlement> WithNetworkStatus(this Faker<Entitlement> self, OptionSetValue networkStatus)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_NetworkStatus, x => networkStatus);

            return self;
        }

        public static Faker<Entitlement> WithIPGCarrierNetworkStatus(this Faker<Entitlement> self, OptionSetValue networkStatus)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_carrier_network_status, x => networkStatus);

            return self;
        }

        public static Faker<Entitlement> WithEntitlementType(this Faker<Entitlement> self, OptionSetValue entitlementType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_EntitlementType, x => entitlementType);

            return self;
        }

        public static Faker<Entitlement> WithContractStatus(this Faker<Entitlement> self, OptionSetValue contractStatus)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_contract_status,
                x => contractStatus
            );

            return self;
        }
    }
}