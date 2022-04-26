using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeAuthorization
    {
        public static Faker<ipg_authorization> Fake(this ipg_authorization self)
        {
            return new Faker<ipg_authorization>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_authorization> WithPhoneNumber(this Faker<ipg_authorization> self, string phoneNumber)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_csrphone, x => phoneNumber);

            return self;
        }

        public static Faker<ipg_authorization> WithPhoneReference(this Faker<ipg_authorization> self, string phoneReference)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_callreference, x => phoneReference);

            return self;
        }

        public static Faker<ipg_authorization> WithIncidentRef(this Faker<ipg_authorization> self, EntityReference incidentRef)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_incidentid, x => incidentRef);

            return self;
        }

        public static Faker<ipg_authorization> WithCarrier(this Faker<ipg_authorization> self, EntityReference carrierRef)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_carrierid, x => carrierRef);

            return self;
        }
    }
}
