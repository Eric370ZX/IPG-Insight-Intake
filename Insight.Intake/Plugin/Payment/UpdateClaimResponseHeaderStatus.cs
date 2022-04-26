using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;

namespace Insight.Intake.Plugin.Payment
{
    public class UpdateClaimResponseHeaderStatus : PluginBase
    {

        public UpdateClaimResponseHeaderStatus() : base(typeof(UpdateClaimResponseHeaderStatus))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_payment.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var payment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_payment>();
                if(payment.ipg_ClaimResponseHeader != null)
                {
                    var header = new ipg_claimresponseheader()
                    {
                        Id = payment.ipg_ClaimResponseHeader.Id,
                        ipg_PostStatus = "posted",
                        ipg_PostedDate = System.DateTime.Now
                    };
                    service.Update(header);

                    var headerEnt = service.Retrieve(header.LogicalName, header.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower())).ToEntity<ipg_claimresponseheader>();
                    QueryExpression queryExpression = new QueryExpression(ipg_claimresponseheader.EntityLogicalName)
                    {
                        ColumnSet = new ColumnSet(nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower(), nameof(ipg_claimresponseheader.ipg_AmountPaid_new).ToLower()),
                        Criteria = new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                            {
                                new ConditionExpression(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(), ConditionOperator.Equal, headerEnt.GetAttributeValue<EntityReference>(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower()).Id),
                                new ConditionExpression(nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower(), ConditionOperator.Equal, "posted")
                            }
                        }
                    };
                    EntityCollection claimResponseHeaders = service.RetrieveMultiple(queryExpression);
                    decimal appliedAmount = claimResponseHeaders.Entities.Sum(claimResponseHeader => (claimResponseHeader.GetAttributeValue<Money>(nameof(ipg_claimresponseheader.ipg_AmountPaid_new).ToLower()) == null ? 0 : claimResponseHeader.GetAttributeValue<Money>(nameof(ipg_claimresponseheader.ipg_AmountPaid_new).ToLower()).Value));
                    int recordCount = claimResponseHeaders.Entities.Count();

                    var batch = new ipg_claimresponsebatch()
                    {
                        Id = headerEnt.GetAttributeValue<EntityReference>(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower()).Id,
                        ipg_appliedamount_new = new Money(appliedAmount),
                        ipg_PostedRecordCount = recordCount
                    };
                    service.Update(batch);

                    CloseObsoleteTask(service, payment.ipg_CaseId);
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private void CloseObsoleteTask(IOrganizationService service, EntityReference caseRef)
        {
            const string subject = "Payment or adjustment transaction pending for a closed case";
            QueryExpression queryExpression = new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Task.RegardingObjectId).ToLower(), ConditionOperator.Equal, caseRef.Id),
                            new ConditionExpression(nameof(Task.Subject).ToLower(), ConditionOperator.Equal, subject)
                        }
                }
            };
            EntityCollection tasks = service.RetrieveMultiple(queryExpression);
            foreach(var task in tasks.Entities)
            {
                var setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = task.ToEntityReference(),
                    State = new OptionSetValue((int)TaskState.Completed),
                    Status = new OptionSetValue((int)Task_StatusCode.Resolved)
                };
                service.Execute(setStateRequest);
            }
        }
    }
}