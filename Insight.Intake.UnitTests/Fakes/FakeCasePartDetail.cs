using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeCasePartDetail
    {
        public static Faker<ipg_casepartdetail> Fake(this ipg_casepartdetail self)
        {
            return new Faker<ipg_casepartdetail>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(cp=> cp.StateCode, cp=> ipg_casepartdetailState.Active);
        }

        public static Faker<ipg_casepartdetail> WithProductReference(this Faker<ipg_casepartdetail> self, Product product)
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

        public static Faker<ipg_casepartdetail> WithPO(this Faker<ipg_casepartdetail> self, SalesOrder po)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (po == null)
            {
                throw new ArgumentNullException(nameof(po));
            }

            self.RuleFor(x => x.ipg_PurchaseOrderId, x => new EntityReference
            {
                LogicalName = po.LogicalName,
                Id = po.Id,
            });

            return self;
        }

        public static Faker<ipg_casepartdetail> WithPayFacility(this Faker<ipg_casepartdetail> self, bool payFacility)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_payfacility, x => payFacility);

            return self;
        }

        public static Faker<ipg_casepartdetail> WithManufacturerReference(this Faker<ipg_casepartdetail> self, Account manufacturer)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            if (manufacturer == null) throw new ArgumentNullException(nameof(manufacturer));

            self.RuleFor(x => x.ipg_manufacturerid, x => new EntityReference
            {
                LogicalName = manufacturer.LogicalName,
                Id = manufacturer.Id,
            });

            return self;
        }

        public static Faker<ipg_casepartdetail> WithFacilityOrders(this Faker<ipg_casepartdetail> self, int facilityOrder)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_facilityorders, x => new OptionSetValue(facilityOrder));

            return self;
        }

        public static Faker<ipg_casepartdetail> WithCaseReference(this Faker<ipg_casepartdetail> self, Incident caseEntity)
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

        public static Faker<ipg_casepartdetail> WithOrderRef(this Faker<ipg_casepartdetail> self, SalesOrder order)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (order == null) throw new ArgumentNullException(nameof(order));

            self.RuleFor(x => x.ipg_PurchaseOrderId, x => new EntityReference
            {
                LogicalName = order.LogicalName,
                Id = order.Id,
            });

            return self;
        }


        public static Faker<ipg_casepartdetail> WithOrderReference(this Faker<ipg_casepartdetail> self, EntityReference orderRef)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (orderRef == null) throw new ArgumentNullException(nameof(orderRef));

            self.RuleFor(x => x.ipg_PurchaseOrderId, x => orderRef);

            return self;
        }

        public static Faker<ipg_casepartdetail> WithClaimRelatedData(this Faker<ipg_casepartdetail> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.RuleFor(x => x.ipg_quantity, x => x.Random.Int(10))
                .RuleFor(x => x.ipg_multiplier, x => x.Random.Int(3))
                .RuleFor(x => x.ipg_costprice, x => new Money(x.Random.Decimal() * 100))
                .RuleFor(x => x.ipg_unitprice, x => new Money(x.Random.Decimal() * 100))
                .RuleFor(x => x.ipg_billedchg, x => new Money(x.Random.Decimal() * 1000))
                .RuleFor(x => x.ipg_manufacturerpartnumber, x => x.Random.String(8))
                ;

            return self;
        }

        public static Faker<ipg_casepartdetail> WithIsChangedFlag(this Faker<ipg_casepartdetail> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.RuleFor(x => x.ipg_IsChanged, x => true);
            return self;
        }

        public static Faker<ipg_casepartdetail> WithPOType(this Faker<ipg_casepartdetail> self, int poType) {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_potypecode, x => new OptionSetValue(poType));
            self.RuleFor(x => x.ipg_quantity, x => new Random().Next(1, 10));

            return self;
        }

        public static Faker<ipg_casepartdetail> WithStatusCode(this Faker<ipg_casepartdetail> self, int statusCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.StatusCode, x => new OptionSetValue(statusCode));
            return self;
        }

        public static Faker<ipg_casepartdetail> WithHCPCS(this Faker<ipg_casepartdetail> self, ipg_masterhcpcs hcpcs)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return self.RuleFor(x => x.ipg_hcpcscode, x => hcpcs.ToEntityReference());
        }

        public static Faker<ipg_casepartdetail> WithBillableInfo(this Faker<ipg_casepartdetail> self, int qty = 1, decimal price = 5, decimal billedCharges = 0, decimal autobilledCharg = 0)
        {

            if (self == null) throw new ArgumentNullException(nameof(self));

            return self.RuleFor(x => x.ipg_quantity, x => qty)
                    .RuleFor(x => x.ipg_unitprice, x => new Money(price))
                    .RuleFor(x => x.ipg_billedchg, x => new Money(billedCharges))
                    .RuleFor(x => x.ipg_autobilledcharges, x => new Money(autobilledCharg));
        }
    }
}