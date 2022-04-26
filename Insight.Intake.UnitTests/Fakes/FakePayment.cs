using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakePayment
    {
        public static Faker<ipg_payment> Fake(this ipg_payment self)
        {
            return new Faker<ipg_payment>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_payment> WithCaseReference(this Faker<ipg_payment> self, Incident caseEntity)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (caseEntity == null) throw new ArgumentNullException(nameof(caseEntity));

            self.RuleFor(
                x => x.ipg_CaseId,
                x => caseEntity.ToEntityReference()
            );

            return self;
        }

        public static Faker<ipg_payment> PostedByCarrier(this Faker<ipg_payment> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_TotalInsurancePaid,
                x => 100
            );

            return self;
        }

        public static Faker<ipg_payment> PostedByPatient(this Faker<ipg_payment> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_MemberPaid_new,
                x => new Money(100)
            );

            return self;
        }
        
    }
}
