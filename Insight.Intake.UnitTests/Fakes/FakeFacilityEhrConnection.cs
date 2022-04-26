using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeFacilityEhrConnection
    {
        public static Faker<ipg_FacilityEhrConnection> Fake(this ipg_FacilityEhrConnection self)
        {
            return new Faker<ipg_FacilityEhrConnection>()
                .RuleFor(x => x.ipg_FacilityEhrConnectionId, x => Guid.NewGuid());
        }

        public static Faker<ipg_FacilityEhrConnection> WithId(this Faker<ipg_FacilityEhrConnection> self, Guid? guid)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_FacilityEhrConnectionId,
                x => guid
            );

            return self;
        }

        public static Faker<ipg_FacilityEhrConnection> WithFacilityReference(this Faker<ipg_FacilityEhrConnection> self, Account facility)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (facility == null) throw new ArgumentNullException(nameof(facility));

            self.RuleFor(
                x => x.ipg_FacilityId,
                x => new EntityReference
                {
                    Id = facility.Id,
                    LogicalName = facility.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_FacilityEhrConnection> WithProcedureDates(this Faker<ipg_FacilityEhrConnection> self, DateTime? procedureEffectiveDate, DateTime? procedureExpirationDate)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_EhrProcedureEffectiveDate, x => procedureEffectiveDate)
                .RuleFor(x => x.ipg_EhrProcedureExpirationDate, x => procedureExpirationDate);

            return self;
        }

        public static Faker<ipg_FacilityEhrConnection> Active(this Faker<ipg_FacilityEhrConnection> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.StateCode,
                x => ipg_FacilityEhrConnectionState.Active
            );

            return self;
        }

        public static Faker<ipg_FacilityEhrConnection> WithStateCode(this Faker<ipg_FacilityEhrConnection> self, ipg_FacilityEhrConnectionState stateCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.StateCode,
                x => stateCode
            );

            return self;
        }

        public static Faker<ipg_FacilityEhrConnection> WithSoftware(this Faker<ipg_FacilityEhrConnection> self, ipg_EHRsoftware? ehrSoftware)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_EhrSoftwareEnum,
                x => ehrSoftware
            );

            return self;
        }
    }
}