using Bogus;
using System;
using System.Collections.Generic;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeGlobalSetting
    {
        public static Faker<ipg_globalsetting> Fake(this ipg_globalsetting self)
        {
            return new Faker<ipg_globalsetting>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_globalsetting> Fake(this ipg_globalsetting self, string name, string value)
        {
            return new Faker<ipg_globalsetting>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => name)
                .RuleFor(x => x.ipg_value, x => value);
        }

        public static Faker<ipg_globalsetting> Fake(this ipg_globalsetting self, string name, string value, string type)
        {
            return new Faker<ipg_globalsetting>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => name)
                .RuleFor(x => x.ipg_value, x => value)
                .RuleFor(x => x.ipg_Type, x => type);
        }

        public static Faker<ipg_globalsetting> WithName(this Faker<ipg_globalsetting> self, string name)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_name, x => name);

            return self;
        }

        public static Faker<ipg_globalsetting> WithValue(this Faker<ipg_globalsetting> self, string value)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_value, x => value);

            return self;
        }

        public static IEnumerable<ipg_globalsetting> GetSlaSettings()
        {
            return new[]
            {
                new ipg_globalsetting().Fake("sla-due-date-offset-create-referral", "24", "hours").Generate(),
                new ipg_globalsetting().Fake("sla-due-date-offset-decision-retro-case", "72", "hours").Generate(),
                new ipg_globalsetting().Fake("sla-due-date-offset-decision-standard-case", "72", "hours").Generate(),
                new ipg_globalsetting().Fake("sla-due-date-offset-decision-stat-case", "24", "hours").Generate(),
                new ipg_globalsetting().Fake("sla-due-date-offset-decision-urgent-case", "48", "hours").Generate(),
                new ipg_globalsetting().Fake("sla-due-date-offset-generate-po", "3", "days").Generate(),
                new ipg_globalsetting().Fake("sla-due-date-offset-pay-provider", "30", "days").Generate()
            };
        }
    }
}