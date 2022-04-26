using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class ValidateCPTCodesByApprovement: PluginBase
    {
        public ValidateCPTCodesByApprovement() : base(typeof(ValidateCPTCodesByApprovement))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingValidateCPTCodesByApprovement", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
             
            var targetRef = (EntityReference)context.InputParameters["Target"];
            context.OutputParameters["Succeeded"] = false;
            if (targetRef != null)
            {
                var target = service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet("ipg_surgerydate", "ipg_cptcodeid1", "ipg_cptcodeid2", "ipg_cptcodeid3",
                                                                                                    "ipg_cptcodeid4", "ipg_cptcodeid5", "ipg_cptcodeid6"));

                var cptCodes = GetInvalidCptCodes(service, target);

                if (cptCodes.Entities.Count == 0)
                {
                    context.OutputParameters["Succeeded"] = true;
                }
                else {
                    var caseNote = string.Join(",", cptCodes.Entities.Select(x => string.Format("CPT {0} is not approved", x.GetAttributeValue<string>("ipg_name"))));
                    context.OutputParameters["CaseNote"] = caseNote;
                }
            }
        }

        private EntityCollection GetInvalidCptCodes(IOrganizationService service, Entity target)
        {
            var invalidCptCodes = new EntityCollection();
            var targetCptCodes = GetCptCodeIds(target);

            if (targetCptCodes.Count > 0) {
                var query = new QueryExpression()
                {
                    EntityName = ipg_cptcode.EntityLogicalName,
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Filters = {
                        new FilterExpression {
                            Conditions = {
                                new ConditionExpression("ipg_cptcodeid", ConditionOperator.In, targetCptCodes)
                            }
                        },
                        new FilterExpression{
                            FilterOperator = LogicalOperator.Or,
                            Conditions = {
                                new ConditionExpression("statuscode", ConditionOperator.Equal, (int)ipg_cptcode_StatusCode.Draft)
                                //new ConditionExpression("ipg_expirationdate", ConditionOperator.LessThan, target.GetAttributeValue<DateTime>("ipg_surgerydate")),
                                //new ConditionExpression("ipg_effectivedate", ConditionOperator.GreaterThan, target.GetAttributeValue<DateTime>("ipg_surgerydate")),
                            }
                        }
                    }
                    }
                };
                
                invalidCptCodes = service.RetrieveMultiple(query);
            }
            return invalidCptCodes;
        }

        private List<Guid> GetCptCodeIds(Entity target)
        {
            var cptCodes = new List<Guid>();
            var cptAttributesNames = new List<string>() { "ipg_cptcodeid1", "ipg_cptcodeid2", "ipg_cptcodeid3", "ipg_cptcodeid4", "ipg_cptcodeid5", "ipg_cptcodeid6" };

            foreach (var attributeName in cptAttributesNames)
            {
                if (target.Contains(attributeName))
                {
                    cptCodes.Add(target.GetAttributeValue<EntityReference>(attributeName).Id);
                }
            }

            return cptCodes;
        }
    }
}
