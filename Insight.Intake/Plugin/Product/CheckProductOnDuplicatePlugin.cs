using System;
using System.Linq;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Client;

namespace Insight.Intake.Plugin.Product
{
    public class CheckProductOnDuplicatePlugin : PluginBase
    {
        public CheckProductOnDuplicatePlugin() : base(typeof(CheckProductOnDuplicatePlugin))
        {
            RegisterEvent(PipelineStages.PreOperation, "Create", Intake.Product.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, "Update", Intake.Product.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext context)
        {
            var target = context.Target<Intake.Product>();
            var preImage = context.PreImage<Intake.Product>();

            var entityData = preImage == null ? target : target.Merge(preImage);

            var crmContext = new OrganizationServiceContext(context.OrganizationService);

            var duplicate = (from p in crmContext.CreateQuery<Intake.Product>()
                             where p.ipg_manufacturerid.Equals(entityData.ipg_manufacturerid)
                                   && p.ipg_manufacturerpartnumber.Equals(entityData.ipg_manufacturerpartnumber)
                                   && p.ProductId != target.Id
                             select p.ProductId).FirstOrDefault();

            if (duplicate != null)
            {
                throw new Exception($"Product with {entityData.ipg_manufacturerpartnumber} Number alredy exist for {entityData.ipg_manufacturerid?.Name} Manufacturer!");
            }
        }
    }
}
