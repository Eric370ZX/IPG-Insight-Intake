using Bogus;
using Insight.Intake.Extensions;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeSalesOrder
    {
        public static Faker<SalesOrder> Fake(this SalesOrder self)
        {
            return new Faker<SalesOrder>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<SalesOrder> FakeEstimatedTPO(this SalesOrder self)
        {
            return new Faker<SalesOrder>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.StateCode, x => SalesOrderState.Active)
                .RuleFor(x => x.ipg_isestimatedpo, x => true)
                .RuleFor(x => x.ipg_potypecode, x => ipg_PurchaseOrderTypes.TPO.ToOptionSetValue());
        }

        public static Faker<SalesOrder> Fake(this SalesOrder self, Guid id)
        {
            return new Faker<SalesOrder>()
                .RuleFor(x => x.Id, x => id);
        }

        public static Faker<SalesOrder> WithMfgRef(this Faker<SalesOrder> self, Account mfg)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (mfg == null) throw new ArgumentNullException(nameof(mfg));

            self.RuleFor(
                x => x.ipg_Manufacturer_id
                , XamlGeneratedNamespace => mfg.ToEntityReference()
            );

            return self;
        }

        public static Faker<SalesOrder> WithCaseReference(this Faker<SalesOrder> self, Incident caseEntity)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (caseEntity == null) throw new ArgumentNullException(nameof(caseEntity));

            self.RuleFor(
                x => x.ipg_CaseId,
                x => caseEntity.ToEntityReference()
            );

            return self;
        }

        public static Faker<SalesOrder> WithStatusCode(this Faker<SalesOrder> self, SalesOrder_StatusCode statusCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.StatusCode, x => statusCode.ToOptionSetValue());

            return self;
        }

        public static Faker<SalesOrder> WithStateCode(this Faker<SalesOrder> self, SalesOrderState satecode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.StateCode, x => satecode);

            return self;
        }

        public static Faker<SalesOrder> WithCreatedOnDate(this Faker<SalesOrder> self, DateTime createdOnDate)
        {
            return self.RuleFor(x => x.CreatedOn, x => createdOnDate);
        }

        public static Faker<SalesOrder> WithPoTypeCode(this Faker<SalesOrder> self, ipg_PurchaseOrderTypes poTypeCode)
        {
            return self.RuleFor(x => x.ipg_potypecode, x => poTypeCode.ToOptionSetValue());
        }

        public static Faker<SalesOrder> WithDocumentImage(this Faker<SalesOrder> self, ipg_document doc)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (doc == null) throw new ArgumentNullException(nameof(doc));

            self.RuleFor(
                x => x.ipg_documentid,
                x => doc.ToEntityReference()
            );

            return self;
        }
    }
}
