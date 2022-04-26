using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace Insight.Intake.Plugin.ClaimResponse
{
    public class FillDenialCodePresentation : PluginBase
    {

        public FillDenialCodePresentation() : base(typeof(FillDenialCodePresentation))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_claimresponselineadjustment.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimresponselineadjustment.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Delete, ipg_claimresponselineadjustment.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                ipg_claimresponselineadjustment claimResponseLineAdjustmentRef = null;
                if (context.InputParameters["Target"] is Entity)
                {
                    var claimResponseLineAdjustment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponselineadjustment>();
                    claimResponseLineAdjustmentRef = service.Retrieve(claimResponseLineAdjustment.LogicalName, claimResponseLineAdjustment.Id, new ColumnSet(nameof(ipg_claimresponselineadjustment.ipg_ClaimResponseLineId).ToLower())).ToEntity<ipg_claimresponselineadjustment>();
                }
                else
                {
                    var claimResponseLineAdjustment = ((EntityReference)context.InputParameters["Target"]);
                    claimResponseLineAdjustmentRef = service.Retrieve(claimResponseLineAdjustment.LogicalName, claimResponseLineAdjustment.Id, new ColumnSet(nameof(ipg_claimresponselineadjustment.ipg_ClaimResponseLineId).ToLower())).ToEntity<ipg_claimresponselineadjustment>();

                    if(claimResponseLineAdjustmentRef.ipg_ClaimResponseLineId == null)
                    {
                        return;
                    }
                }

                var queryExpression = new QueryExpression(ipg_claimresponselineadjustment.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(claimResponseLineAdjustmentRef.ipg_Code).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponselineadjustment.ipg_ClaimResponseLineId).ToLower(), ConditionOperator.Equal, claimResponseLineAdjustmentRef.ipg_ClaimResponseLineId.Id)
                        }
                    }
                };
                if (context.MessageName == MessageNames.Delete)
                {
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_claimresponselineadjustment.ipg_claimresponselineadjustmentId).ToLower(), ConditionOperator.NotEqual, claimResponseLineAdjustmentRef.Id));
                }
                EntityCollection claimResponseHeaders = service.RetrieveMultiple(queryExpression);
                List<string> list = new List<string>();
                foreach (Entity entity in claimResponseHeaders.Entities)
                {
                    if((entity.GetAttributeValue<string>(nameof(claimResponseLineAdjustmentRef.ipg_Code).ToLower()) != null) && (!list.Exists(x => x == entity.GetAttributeValue<string>(nameof(claimResponseLineAdjustmentRef.ipg_Code).ToLower()))))
                        list.Add(entity.GetAttributeValue<string>(nameof(claimResponseLineAdjustmentRef.ipg_Code).ToLower()));
                }
                string denialCodeString = "";
                list.ForEach(denialCode => { denialCodeString += (string.IsNullOrEmpty(denialCodeString) ? "" : ", ") + denialCode; });

                ipg_claimresponseline claimResponseLine = new ipg_claimresponseline();
                claimResponseLine.Id = claimResponseLineAdjustmentRef.ipg_ClaimResponseLineId.Id;
                claimResponseLine.ipg_DenialCodeString = denialCodeString;
                service.Update(claimResponseLine);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}