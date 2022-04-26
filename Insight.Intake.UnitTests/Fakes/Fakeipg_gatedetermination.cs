using Bogus;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class Fakeipg_gatedetermination
    {
        public static Faker<ipg_gatedetermination> Fake(this ipg_gatedetermination self)
        {
            return new Faker<ipg_gatedetermination>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_gatedetermination> WithLifecycleStepReference(this Faker<ipg_gatedetermination> self, EntityReference lifecycleStepReference)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_LifecycleStepId, x => lifecycleStepReference);
            return self;
        }

        public static Faker<ipg_gatedetermination> WithGateConfigurationReference(this Faker<ipg_gatedetermination> self, EntityReference gateConfigurationReference)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_GateConfigurationId, x => gateConfigurationReference);
            return self;
        }

        public static Faker<ipg_gatedetermination> WithCaseState(this Faker<ipg_gatedetermination> self, ipg_CaseStateCodes caseStateCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_CaseState, x => caseStateCode.ToOptionSetValue());

            return self;
        }

        public static Faker<ipg_gatedetermination> WithTriggeredBy(this Faker<ipg_gatedetermination> self, bool triggeredBy)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_TriggeredBy, x => triggeredBy);

            return self;
        }
    }
}
