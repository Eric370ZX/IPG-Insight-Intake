using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeStatement
    {
        public static Faker<ipg_statementgenerationtask> Fake(this ipg_statementgenerationtask self)
        {
            return new Faker<ipg_statementgenerationtask>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.StateCode, x => ipg_statementgenerationtaskState.Active)
                .RuleFor(x => x.StatusCodeEnum, x => ipg_statementgenerationtask_StatusCode.Open);
        }

        public static Faker<ipg_statementgenerationtask> WithCaseReference(this Faker<ipg_statementgenerationtask> self, Incident caseEntity)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (caseEntity == null) throw new ArgumentNullException(nameof(caseEntity));

            self.RuleFor(
                x => x.ipg_caseid,
                x => caseEntity.ToEntityReference()
            );

            return self;
        }

        public static Faker<ipg_statementgenerationtask> WithStatementEventConfig(this Faker<ipg_statementgenerationtask> self, ipg_statementgenerationeventconfiguration eventconfig)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (eventconfig == null) throw new ArgumentNullException(nameof(eventconfig));

            self.RuleFor(
                x => x.ipg_eventid,
                x => eventconfig.ToEntityReference()
            );

            return self;
        }

        public static Faker<ipg_statementgenerationeventconfiguration> Fake(this ipg_statementgenerationeventconfiguration self)
        {
            return new Faker<ipg_statementgenerationeventconfiguration>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.StateCode, ipg_statementgenerationeventconfigurationState.Active);
        }
        public static Faker<ipg_statementgenerationeventconfiguration> Fake(this ipg_statementgenerationeventconfiguration self, string name)
        {
            return new Faker<ipg_statementgenerationeventconfiguration>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, name)
                .RuleFor(x => x.StateCode, ipg_statementgenerationeventconfigurationState.Active);
        }
    }
}
