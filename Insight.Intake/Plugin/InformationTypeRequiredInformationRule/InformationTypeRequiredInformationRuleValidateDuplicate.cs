using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.InformationTypeRequiredInformationRule
{
    public class InformationTypeRequiredInformationRuleValidateDuplicate : PluginBase
    {
        public InformationTypeRequiredInformationRuleValidateDuplicate() : base(typeof(InformationTypeRequiredInformationRuleValidateDuplicate))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_informationtyperequiredinformationrule.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_informationtyperequiredinformationrule.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext context)
        {
            var target = context.Target<ipg_informationtyperequiredinformationrule>();

            var entityData = context.OrganizationService.Retrieve(ipg_informationtyperequiredinformationrule.EntityLogicalName,
                                                                  target.Id,
                                                                  new ColumnSet(ipg_informationtyperequiredinformationrule.Fields.ipg_name,
                                                                  ipg_informationtyperequiredinformationrule.Fields.ipg_DocumentTypeId))
                                                                  .ToEntity<ipg_informationtyperequiredinformationrule>();

            var crmContext = new OrganizationServiceContext(context.OrganizationService);

            var duplicate = (from rule in crmContext.CreateQuery<ipg_informationtyperequiredinformationrule>()
                             where rule.ipg_DocumentTypeId.Equals(entityData.ipg_DocumentTypeId)
                                   && rule.ipg_name == entityData.ipg_name
                                   && rule.ipg_informationtyperequiredinformationruleId != target.Id
                             select rule.ipg_informationtyperequiredinformationruleId).FirstOrDefault();

            if (duplicate != null)
            {
                throw new Exception($"Rule with new \"{entityData.ipg_name}\" already exist for \"{entityData.ipg_DocumentTypeId.Name}\" document type.");
            }
        }
    }
}

