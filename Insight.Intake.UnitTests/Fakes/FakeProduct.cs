using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeProduct
    {
        public static Faker<Product> Fake(this Product self)
        {
            return new Faker<Product>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_manufacturerpartnumber, x => new Random().Next().ToString())
                .RuleFor(x => x.StateCode, x => ProductState.Active);
        }

        public static Faker<Product> WithProductTypeOptionSet(this Faker<Product> self, int productType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ProductTypeCode, x => new OptionSetValue(productType));

            return self;
        }

        public static Faker<Product> WithStatus(this Faker<Product> self, Product_ipg_status status)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(p => p.ipg_status, p => new OptionSetValue((int)status));
            self.RuleFor(p => p.ipg_statusEnum, p => status);

            return self;
        }

        public static Faker<Product> WithManufacturerReference(this Faker<Product> self, Account manufacturer)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (manufacturer == null) throw new ArgumentNullException(nameof(manufacturer));

            self.RuleFor(
                x => x.ipg_manufacturerid,
                x => new EntityReference
                {
                    Id = manufacturer.Id,
                    LogicalName = manufacturer.LogicalName
                }
            );

            return self;
        }
        public static Faker<Product> WithBoxQuantity(this Faker<Product> self, int boxquantity = 1)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_boxquantity,
                x => boxquantity
            );

            return self;
        }
    }
}