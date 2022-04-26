using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeTaskReasonDetail
    {
        public static Faker<ipg_taskreasondetails> Fake(this ipg_taskreasondetails self)
        {
            return new Faker<ipg_taskreasondetails>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.StateCode, x => ipg_taskreasondetailsState.Active);
        }

        public static Faker<ipg_taskreasondetails> WithTaskType(this Faker<ipg_taskreasondetails> self, ipg_tasktype taskType)
        {
            return self.RuleFor(x => x.ipg_tasktypeid, x => taskType.ToEntityReference());
        }

        public static Faker<ipg_taskreasondetails> WithCarrierPatientRules(this Faker<ipg_taskreasondetails> self, ipg_Conditions carrierCondition, decimal carrierBalance, ipg_Conditions patientCondition, decimal patientBalance)
        {
            return self.RuleFor(x => x.ipg_carrierbalanceconditioncodeEnum, x => carrierCondition)
                .RuleFor(x => x.ipg_carrierbalance, x => carrierBalance)
                .RuleFor(x => x.ipg_patientbalanceconditioncodeEnum, x => patientCondition)
                .RuleFor(x => x.ipg_patientbalance, x => patientBalance);
        }

        public static Faker<ipg_taskreasondetails> WithNoPatientStatementGenerated(this Faker<ipg_taskreasondetails> self)
        {
            return self.RuleFor(x => x.ipg_nopsgeneratedcodeEnum, x => ipg_TwoOptions.Yes);
        }
        
        public static Faker<ipg_taskreasondetails> WithStartDueDate(this Faker<ipg_taskreasondetails> self, int start, int due)
        {
            return self.RuleFor(x => x.ipg_taskstartdate, x => start)
                    .RuleFor(x => x.ipg_taskduedate, x => due);
        }

        public static Faker<ipg_taskreasondetails> WithOnStatementEvent(this Faker<ipg_taskreasondetails> self, ipg_statementgenerationeventconfiguration configEvent)
        {
            return self.RuleFor(x => x.ipg_onstatementeventid, x => configEvent.ToEntityReference());
        }

        
    }
}
