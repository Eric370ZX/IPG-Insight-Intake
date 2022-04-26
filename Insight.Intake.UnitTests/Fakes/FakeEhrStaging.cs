using Bogus;
using System;
using System.Collections.Generic;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeEhrStaging
    {
        public static Faker<ipg_EHRstaging> Fake(this ipg_EHRstaging self)
        {
            return new Faker<ipg_EHRstaging>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_EHRstaging> WithPatientAddress(this Faker<ipg_EHRstaging> self)
        {
            return self
                .RuleFor(x => x.ipg_PatientZip, x => x.Random.String())
                .RuleFor(x => x.ipg_PatientState, x => x.Random.String())
                .RuleFor(x => x.ipg_PatientCity, x => x.Random.String())
                .RuleFor(x => x.ipg_PatientAddress, x => x.Random.String());
        }

    }
}