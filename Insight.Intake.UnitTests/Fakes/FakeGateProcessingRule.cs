using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeGateProcessingRule
    {
        public static Faker<ipg_gateprocessingrule> Fake(this ipg_gateprocessingrule self
            , ipg_lifecyclestep lfstep
            , ipg_lifecyclestep nextlfstep
            , string name = null
            , ipg_SeverityLevel sevirityLevel = ipg_SeverityLevel.Info
            , ipg_gateprocessingruleState state = ipg_gateprocessingruleState.Active)
        {
            return new Faker<ipg_gateprocessingrule>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_gateconfigurationid, x => lfstep.ipg_gateconfigurationid)
                .RuleFor(x => x.ipg_lifecyclestepid, x => lfstep.ToEntityReference())
                .RuleFor(x => x.ipg_nextlifecyclestepid, x => nextlfstep.ToEntityReference())
                .RuleFor(x => x.ipg_severitylevelEnum, x => sevirityLevel)
                .RuleFor(x => x.ipg_name, name ?? "test")
                .RuleFor(x => x.StateCode, x => state);
        }
        public static Faker<ipg_gateprocessingrule> WithCaseStatusDisplayed(this Faker<ipg_gateprocessingrule> self, EntityReference caseStatusDisplayedRef)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_casestatusdisplayedid, x => caseStatusDisplayedRef);
            return self;
        }
        public static Faker<ipg_gateprocessingrule> WithCaseStatusEnum(this Faker<ipg_gateprocessingrule> self, ipg_CaseStatus caseStatusEnum)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_casestatusEnum, x => caseStatusEnum);
            return self;
        }
        public static Faker<ipg_gateprocessingrule> WithCaseStateEnum(this Faker<ipg_gateprocessingrule> self, ipg_CaseStateCodes caseStateEnum)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_casestateEnum, x => caseStateEnum);
            return self;
        }
    }
}
