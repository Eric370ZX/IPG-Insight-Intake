using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeTaskreasondetailsStatement
    {
        public static Faker<ipg_ipg_taskreasondetails_ipg_statementgene> Fake(this ipg_ipg_taskreasondetails_ipg_statementgene self)
        {
            return new Faker<ipg_ipg_taskreasondetails_ipg_statementgene>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_ipg_taskreasondetails_ipg_statementgene> WithTaskReasonDetails(this Faker<ipg_ipg_taskreasondetails_ipg_statementgene> self, ipg_taskreasondetails detail)
        {
            return self.RuleFor(x => x.ipg_taskreasondetailsid, x => detail.Id);
        }
        public static Faker<ipg_ipg_taskreasondetails_ipg_statementgene> WithStatementConfig(this Faker<ipg_ipg_taskreasondetails_ipg_statementgene> self, ipg_statementgenerationeventconfiguration stconfig)
        {
            return self.RuleFor(x => x.ipg_statementgenerationeventconfigurationid, x => stconfig.Id);
        }
    }
}
