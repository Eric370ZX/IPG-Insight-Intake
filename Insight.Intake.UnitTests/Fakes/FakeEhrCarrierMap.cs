using Bogus;
using System;
using System.Collections.Generic;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeEhrCarrierMap
    {
        public static Faker<ipg_ehrcarriermap> Fake(this ipg_ehrcarriermap self)
        {
            return new Faker<ipg_ehrcarriermap>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_ehrcarriermap> WithFacility(this Faker<ipg_ehrcarriermap> self, Intake.Account facility)
        {
            return self
                .RuleFor(x => x.ipg_facilityid, x => facility.ToEntityReference());
        }

        public static Faker<ipg_ehrcarriermap> WithCarrier(this Faker<ipg_ehrcarriermap> self, Intake.Account carrier)
        {
            return self
                .RuleFor(x => x.ipg_carrierid, x => carrier.ToEntityReference());
        }

        public static Faker<ipg_ehrcarriermap> WithStatus(this Faker<ipg_ehrcarriermap> self, ipg_EHRCarrierMappingStatuses status)
        {
            return self
                .RuleFor(x => x.ipg_StatusEnum, x => status);
        }

        public static Faker<ipg_ehrcarriermap> WithTake(this Faker<ipg_ehrcarriermap> self, bool take)
        {
            return self
                .RuleFor(x => x.ipg_take, x => take);
        }

    }
}