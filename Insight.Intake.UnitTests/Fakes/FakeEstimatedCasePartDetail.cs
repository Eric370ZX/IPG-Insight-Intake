using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeEstimatedCasePartDetail
    {
        public static Faker<ipg_estimatedcasepartdetail> Fake(this ipg_estimatedcasepartdetail self)
        {
            return new Faker<ipg_estimatedcasepartdetail>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(cp => cp.StateCode, cp => ipg_estimatedcasepartdetailState.Active);
        }

        public static Faker<ipg_estimatedcasepartdetail> WithProductReference(this Faker<ipg_estimatedcasepartdetail> self, Product product)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (product == null) throw new ArgumentNullException(nameof(product));

            self.RuleFor(x => x.ipg_productid, x => new EntityReference
            {
                LogicalName = product.LogicalName,
                Id = product.Id,
            });

            return self;
        }

        public static Faker<ipg_estimatedcasepartdetail> WithCaseReference(this Faker<ipg_estimatedcasepartdetail> self, Incident caseEntity)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (caseEntity == null) throw new ArgumentNullException(nameof(caseEntity));

            self.RuleFor(x => x.ipg_caseid, x => new EntityReference
            {
                LogicalName = caseEntity.LogicalName,
                Id = caseEntity.Id,
            });

            return self;
        }

        public static Faker<ipg_estimatedcasepartdetail> WithPOType(this Faker<ipg_estimatedcasepartdetail> self, int poType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_potypecode, x => new OptionSetValue(poType));
            self.RuleFor(x => x.ipg_quantity, x => new Random().Next(1, 10));

            return self;
        }

        public static Faker<ipg_estimatedcasepartdetail> WithStatusCode(this Faker<ipg_estimatedcasepartdetail> self, int statusCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.StatusCode, x => new OptionSetValue(statusCode));
            return self;
        }
    }
}
