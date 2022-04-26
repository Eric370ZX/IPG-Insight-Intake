using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;

namespace Insight.Intake.Plugin.ClaimResponse
{
    public class UpdateManualHeaderName : PluginBase
    {

        public UpdateManualHeaderName() : base(typeof(UpdateManualHeaderName))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_claimresponseheader.EntityLogicalName, PreOperationCreateHandler);
        }

        private void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var claimResponseHeader = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponseheader>();

                if (!string.IsNullOrWhiteSpace(claimResponseHeader.ipg_name))
                {
                    return;
                }

                var batch = service.Retrieve(ipg_claimresponsebatch.EntityLogicalName, claimResponseHeader.ipg_ClaimResponseBatchId.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_name).ToLower())).ToEntity<ipg_claimresponsebatch>();
                string batchName = batch.GetAttributeValue<string>(nameof(ipg_claimresponsebatch.ipg_name).ToLower());

                var queryExpression = new QueryExpression(ipg_claimresponseheader.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimresponseheader.ipg_name).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(), ConditionOperator.Equal, claimResponseHeader.ipg_ClaimResponseBatchId.Id)
                        }
                    }
                };
                EntityCollection claimResponseHeaders = service.RetrieveMultiple(queryExpression);
                int currentNumber = 1;
                foreach (Entity header in claimResponseHeaders.Entities)
                {
                    string headerName = header.GetAttributeValue<string>(nameof(ipg_claimresponseheader.ipg_name).ToLower());
                    int index = headerName.LastIndexOf('_');
                    if(index >= 0)
                    {
                        int number = 0;
                        if(Int32.TryParse(headerName.Substring(index + 1), out number))
                        {
                            if(number >= currentNumber)
                            {
                                currentNumber = number + 1;
                            }
                        }
                    }
                }

                claimResponseHeader.ipg_name = batchName + "_" + currentNumber.ToString("D4");
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}