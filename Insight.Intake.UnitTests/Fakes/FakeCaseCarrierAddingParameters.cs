using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeCaseCarrierAddingParameters
    {
        public static Faker<ipg_CaseCarrierAddingParameters> Fake(this ipg_CaseCarrierAddingParameters self, Incident incident, Account carrier)
        {
            return new Faker<ipg_CaseCarrierAddingParameters>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x=> x.StateCode, x => ipg_CaseCarrierAddingParametersState.Active)
                .RuleFor(x => x.ipg_CaseId, x => new EntityReference(Incident.EntityLogicalName, incident.Id))
                .RuleFor(x => x.ipg_CarrierId, x => new EntityReference(Account.EntityLogicalName, carrier.Id))
                .RuleFor(x => x.ipg_MemberIdNumber, x => x.Random.AlphaNumeric(10))
                .RuleFor(x => x.ipg_GroupNumber, x => x.Random.AlphaNumeric(10));
        }

    }
}