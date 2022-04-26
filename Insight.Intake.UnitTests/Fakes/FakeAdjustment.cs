using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeAdjustment
    {
        public static Faker<ipg_adjustment> Fake(this ipg_adjustment self)
        {
            return new Faker<ipg_adjustment>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.StateCode, x => ipg_adjustmentState.Active);
        }

        public static Faker<ipg_adjustment> FakeSmallWOAdjustment(this ipg_adjustment self, decimal amount = 50)
        {
            return self.Fake()
                .RuleFor(x => x.ipg_AdjustmentTypeEnum, x => ipg_AdjustmentTypes.WriteOff)
                .RuleFor(x => x.ipg_AmountType, x => false)
                .RuleFor(x => x.ipg_ApplyToEnum, x => ipg_PayerType.Patient)
                .RuleFor(x => x.ipg_Percent, x => 100)
                .RuleFor(x => x.ipg_ReasonEnum, x => ipg_AdjustmentReasons.WOSmallBalance)
                .RuleFor(x => x.ipg_Amount, x => new Money(amount))
                .RuleFor(x => x.ipg_AmountToApply, x => new Money(amount));
        }

        public static Faker<ipg_adjustment> FakeTransferofPayment(this ipg_adjustment self, decimal amount = 50)
        {
            return self.Fake()
                .RuleFor(x => x.ipg_AdjustmentTypeEnum, x => ipg_AdjustmentTypes.TransferofPayment)
                .RuleFor(x => x.ipg_ReasonEnum, x => ipg_AdjustmentReasons.TransferPayment)
                .RuleFor(x => x.ipg_Amount, x => new Money(amount))
                .RuleFor(x => x.ipg_AmountToApply, x => new Money(amount));
        }


        public static Faker<ipg_adjustment> WithCase(this Faker<ipg_adjustment> self, Incident incident)
        {
            return self.RuleFor(x => x.ipg_CaseId, x => incident.ToEntityReference());
        }

        public static Faker<ipg_adjustment> WithFromCase(this Faker<ipg_adjustment> self, Incident incident)
        {
            return self.RuleFor(x => x.ipg_FromCase, x => incident.ToEntityReference());
        }
    }
}
