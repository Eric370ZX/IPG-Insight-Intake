using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeFacilityManufacturerRelationship
    {
        public static Faker<ipg_facilitymanufacturerrelationship> Fake(this ipg_facilitymanufacturerrelationship self)
        {
            return new Faker<ipg_facilitymanufacturerrelationship>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_facilitymanufacturerrelationship> WithFacilityReference(this Faker<ipg_facilitymanufacturerrelationship> self, Account facility)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            if (facility == null) throw new ArgumentNullException(nameof(facility));

            self.RuleFor(x => x.ipg_FacilityId, x => new EntityReference
            {
                LogicalName = facility.LogicalName,
                Id = facility.Id,
            });

            self.RuleFor(x => x.ipg_Active, x => true);
            self.RuleFor(x => x.StateCode, x => ipg_facilitymanufacturerrelationshipState.Active);

            return self;
        }
        
        public static Faker<ipg_facilitymanufacturerrelationship> WithManufacturerReference(this Faker<ipg_facilitymanufacturerrelationship> self, Account manufacturer)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            if (manufacturer == null) throw new ArgumentNullException(nameof(manufacturer));

            self.RuleFor(x => x.ipg_ManufacturerId, x => new EntityReference
            {
                LogicalName = manufacturer.LogicalName,
                Id = manufacturer.Id,
            });

            return self;
        }

        public static Faker<ipg_facilitymanufacturerrelationship> WithManufacturerAccountNumber(this Faker<ipg_facilitymanufacturerrelationship> self, string accounNumber)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (accounNumber == null) throw new ArgumentNullException(nameof(accounNumber));

            self.RuleFor(x => x.ipg_ManufacturerAccountNumber, x => accounNumber);

            return self;
        }
    }
}