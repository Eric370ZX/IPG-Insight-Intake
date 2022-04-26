using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeAssociatedCptDateRange
    {
        public static Faker<ipg_associatedcptdaterange> Fake(this ipg_associatedcptdaterange self)
        {
            return new Faker<ipg_associatedcptdaterange>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_associatedcptdaterange> FakeWithFacilityReference(this Faker<ipg_associatedcptdaterange> self, 
            ipg_associatedcpt associatedCpt,DateTime effectiveDate,DateTime expirationDate)
        {
            self.RuleFor(
                x => x.ipg_AssociatedCPT,
                x => new EntityReference
                {
                    Id = associatedCpt.Id,
                    LogicalName = associatedCpt.LogicalName,
                }
            )
            .RuleFor(x => x.ipg_EffectiveDate, x => effectiveDate)
            .RuleFor(x => x.ipg_ExpirationDate, x => expirationDate);

            return self;
        }

    }
}
