using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeEntityCollection
    {
        public static EntityCollection FakeEntityCollectionForDeriveHomeplan(this EntityCollection self, string carriername, ipg_homeplancarriermap map)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            var ac = new AttributeCollection();
            ac.Add("ipg_carrierid", map.ipg_CarrierId);
            ac.Add("ipg_medicareadvantage", map.ipg_MedicareAdvantage);
            ac.Add("carrier.name", new AliasedValue(Account.EntityLogicalName, nameof(Account.Name), carriername));

            self.Entities.Add(new Entity
            {
                Attributes = ac
            });
            return self;
        }
    }
}
