using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeCaseHoldConfiguration
    {
        public static Faker<ipg_caseholdconfiguration> Fake(this ipg_caseholdconfiguration self)
        {
            return new Faker<ipg_caseholdconfiguration>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_caseholdconfiguration> WithTaskId(this Faker<ipg_caseholdconfiguration> self, string taskId)
        {
            return self
                .RuleFor(x => x.ipg_taskid, x => taskId);
        }

        public static Faker<ipg_caseholdconfiguration> WithCaseState(this Faker<ipg_caseholdconfiguration> self, ipg_CaseStateCodes caseState)
        {
            return self
                .RuleFor(x => x.ipg_casestate, x => new OptionSetValue((int)caseState));
        }

        public static Faker<ipg_caseholdconfiguration> WithHoldReason(this Faker<ipg_caseholdconfiguration> self, ipg_Caseholdreason holdReason)
        {
            return self
                .RuleFor(x => x.ipg_caseholdreason, x => new OptionSetValue((int)holdReason));
        }
    }
}
