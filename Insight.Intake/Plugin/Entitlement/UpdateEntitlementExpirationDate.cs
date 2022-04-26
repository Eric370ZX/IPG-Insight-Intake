using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.EntitlementNamespace
{
    //Soumitra: This looks like a very convoluted piece of functionality when expiration date on another entitlement is updated
    //when user creates an entitlement for the same facility and carrier. This is done without any warning to the user and instead of
    //allowing user to fix the existing entitlement or extend it, it is expiring the old contract which can lead to confusion. 
    public class UpdateEntitlementExpirationDate : PluginBase
    {

        public UpdateEntitlementExpirationDate() : base(typeof(UpdateEntitlementExpirationDate))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Entitlement.EntityLogicalName, PreOperationCreateHandler);
        }

        private void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            //var tracingService = localPluginContext.TracingService;
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    var entitlement = ((Entity)context.InputParameters["Target"]).ToEntity<Entitlement>();
                    if (context.MessageName == "Create")
                    {
                        if (entitlement.ipg_EntitlementType.Value == (int)ipg_EntitlementTypes.FacilityCarrier)
                        {
                            var query = new QueryExpression
                            {
                                EntityName = entitlement.LogicalName,
                                ColumnSet = new ColumnSet(Entitlement.Fields.Id,
                                                          Entitlement.Fields.StartDate,
                                                          Entitlement.Fields.EndDate),
                                Criteria =
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression(nameof(entitlement.ipg_CarrierId).ToLower(), ConditionOperator.Equal, entitlement.ipg_CarrierId.Id),
                                        new ConditionExpression(nameof(entitlement.ipg_FacilityId).ToLower(), ConditionOperator.Equal, entitlement.ipg_FacilityId.Id),
                                        new ConditionExpression(nameof(entitlement.ipg_EntitlementType).ToLower(), ConditionOperator.Equal, (int)ipg_EntitlementTypes.FacilityCarrier)
                                    }
                                },
                                Orders =
                                {
                                    new OrderExpression(nameof(entitlement.StartDate).ToLower(), OrderType.Descending)
                                },
                                PageInfo =
                                {
                                    Count = 1,
                                    PageNumber = 1
                                }
                            };
                            EntityCollection ec = service.RetrieveMultiple(query);
                            foreach (Entity ent in ec.Entities)
                            {
                                var newEndDate = entitlement.StartDate.Value.Date.AddDays(-1);
                                if (ent.ToEntity<Entitlement>().StartDate.Value.Date <= newEndDate)
                                {
                                    ent[nameof(entitlement.EndDate).ToLower()] = newEndDate;
                                    service.Update(ent);
                                }
                                else
                                {
                                    throw new Exception($"Existing entitlement {ent.Id} for the same facility and carrier can't be expired because of conflicting dates.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}
