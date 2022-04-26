using System;
using Bogus;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeSalesOrderDetail
    {
        public static Faker<SalesOrderDetail> Fake(this SalesOrderDetail self)
        {
            return new Faker<SalesOrderDetail>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<SalesOrderDetail> Fake(this SalesOrderDetail self, Guid id)
        {
            return new Faker<SalesOrderDetail>()
                .RuleFor(x => x.Id, x => id);
        }

        public static Faker<SalesOrderDetail> WithOrder(this Faker<SalesOrderDetail> self, SalesOrder order)
        {
            return self.RuleFor(x => x.SalesOrderId, x => order.ToEntityReference());
        }

        public static Faker<SalesOrderDetail> WithProduct(this Faker<SalesOrderDetail> self, Intake.Product product)
        {
            return self.RuleFor(x => x.ProductId, x => product.ToEntityReference());
        }
    }
}
