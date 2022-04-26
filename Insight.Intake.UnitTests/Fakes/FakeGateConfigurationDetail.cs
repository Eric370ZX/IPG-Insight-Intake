using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeGateConfigurationDetail
    {
        public static Faker<ipg_gateconfigurationdetail> Fake(this ipg_gateconfigurationdetail self, ipg_gateconfiguration gateConfiguration)
        {
            return new Faker<ipg_gateconfigurationdetail>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_gateconfigurationid, x => gateConfiguration.ToEntityReference());
        }

        public static Faker<ipg_gateconfigurationdetail> Fake(this ipg_gateconfigurationdetail self
            , ipg_gateconfiguration gateConfiguration
            , Intake.Workflow workflow, int executionOrder
            , string name
            , ipg_SeverityLevel sevirityLevel
            , string passmessage
            , string failmessage)
        {
            return new Faker<ipg_gateconfigurationdetail>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_gateconfigurationid, x => gateConfiguration.ToEntityReference())
                .RuleFor(x => x.ipg_processid, x => workflow.ToEntityReference())
                .RuleFor(x => x.ipg_executionorder, x => executionOrder)
                .RuleFor(x => x.ipg_name, x => name)
                .RuleFor(x => x.ipg_severitylevel, x => new OptionSetValue((int)sevirityLevel))
                .RuleFor(x => x.ipg_passmessage, x => passmessage)
                .RuleFor(x => x.ipg_failmessage, x => failmessage);
        }

        public static Faker<ipg_gateconfigurationdetail> WithStateCode(this Faker<ipg_gateconfigurationdetail> self, ipg_gateconfigurationdetailState statecode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.StateCode, x => statecode);

            return self;
        }
    }
}