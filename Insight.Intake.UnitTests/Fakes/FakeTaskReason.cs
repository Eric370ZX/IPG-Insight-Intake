using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
   public static class FakeTaskReason
    {
        public static Faker<ipg_taskreason> Fake(this ipg_taskreason self)
        {
            return new Faker<ipg_taskreason>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_taskreason> WithTaskType(this Faker<ipg_taskreason> self, ipg_tasktype taskType)
        {
            return self.RuleFor(x => x.ipg_tasktype, x => taskType.ToEntityReference());
        }
        public static Faker<ipg_taskreason> WithRules(this Faker<ipg_taskreason> self, string Rules)
        {
            return self.RuleFor(x => x.ipg_rules, x => Rules);
        }
        public static Faker<ipg_taskreason> WithTaskReasonCode(this Faker<ipg_taskreason> self, ipg_TaskReasons taskReasonCode)
        {
            return self.RuleFor(x => x.ipg_TaskReasonCodeEnum, x => taskReasonCode);
        }
    }
}
