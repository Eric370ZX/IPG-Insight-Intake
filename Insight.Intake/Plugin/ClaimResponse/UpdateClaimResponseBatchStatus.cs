using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;

namespace Insight.Intake.Plugin.ClaimResponse
{
    public class UpdateClaimResponseBatchStatus : PluginBase
    {

        public UpdateClaimResponseBatchStatus() : base(typeof(UpdateClaimResponseBatchStatus))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_claimresponseheader.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimresponseheader.EntityLogicalName, PostOperationHandler);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Delete, ipg_claimresponseheader.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                ipg_claimresponseheader claimResponseHeaderRef = null;

                if (context.InputParameters["Target"] is Entity)
                {
                    var claimResponseHeader = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponseheader>();
                    claimResponseHeaderRef = service.Retrieve(claimResponseHeader.LogicalName, claimResponseHeader.Id, new ColumnSet("ipg_claimresponsebatchid")).ToEntity<ipg_claimresponseheader>();
                }
                else if (context.InputParameters["Target"] is EntityReference claimResponseHeader)
                {
                    claimResponseHeaderRef = service.Retrieve(claimResponseHeader.LogicalName, claimResponseHeader.Id, new ColumnSet("ipg_claimresponsebatchid")).ToEntity<ipg_claimresponseheader>();
                }

                var queryExpression = new QueryExpression(ipg_claimresponseheader.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower(), nameof(ipg_claimresponseheader.ipg_CaseId).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(), ConditionOperator.Equal, claimResponseHeaderRef.ipg_ClaimResponseBatchId.Id)
                        }
                    }
                };
                if (context.MessageName == MessageNames.Delete)
                {
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_claimresponseheader.ipg_claimresponseheaderId).ToLower(), ConditionOperator.NotEqual, claimResponseHeaderRef.Id));
                }

                EntityCollection claimResponseHeaders = service.RetrieveMultiple(queryExpression);
                List<string> list = new List<string>();
                bool emptyCase = false;
                foreach (Entity entity in claimResponseHeaders.Entities)
                {
                    list.Add(entity.GetAttributeValue<string>(nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower()) == null ? null : entity.GetAttributeValue<string>("ipg_poststatus").ToLower());
                    emptyCase = emptyCase || (entity.GetAttributeValue<EntityReference>(nameof(ipg_claimresponseheader.ipg_CaseId).ToLower()) == null);
                }
                int statuscode = 1;
                if (list.FindAll(x => x == "review").Count > 0)
                {
                    statuscode = (int)ipg_claimresponsebatch_StatusCode.InReview;
                }
                else if (list.FindAll(x => x == "posted").Count == claimResponseHeaders.Entities.Count)
                {
                    statuscode = (int)ipg_claimresponsebatch_StatusCode.Completed;
                }
                else if (list.FindAll(x => x == "error").Count > 0)
                {
                    statuscode = (int)ipg_claimresponsebatch_StatusCode.PartialPost_Errors;
                }
                else if (list.FindAll(x => x == "posted").Count > 0)
                {
                    statuscode = (int)ipg_claimresponsebatch_StatusCode.InProcess;
                }
                else
                {
                    statuscode = (int)ipg_claimresponsebatch_StatusCode.New;
                }

                if (emptyCase)
                {
                    statuscode = (int)ipg_claimresponsebatch_StatusCode.PartialPost_Errors;
                }

                CreateTask(service, claimResponseHeaderRef, statuscode);

                var batch = new ipg_claimresponsebatch();
                batch.Id = claimResponseHeaderRef.ipg_ClaimResponseBatchId.Id;
                batch.StatusCode = new OptionSetValue(statuscode);
                batch.ipg_RecordCount = claimResponseHeaders.Entities.Count;

                service.Update(batch);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private void CreateTask(IOrganizationService service, ipg_claimresponseheader header, int status)
        {
            if(status != (int)ipg_claimresponsebatch_StatusCode.PartialPost_Errors)
            {
                return;
            }

            var crmContext = new OrganizationServiceContext(service);
            var financeTeams = (from team in crmContext.CreateQuery<Team>()
                                where team.Name.Contains("Finance")
                                select team).ToList();

            if (financeTeams.Count > 0)
            {
                Task task = new Task();
                task.Subject = "Zpay Batch Requires Review";
                task.PriorityCode = new OptionSetValue((int)Task_PriorityCode.Normal);
                task.RegardingObjectId = header.ToEntityReference();
                task.OwnerId = financeTeams.First().ToEntityReference();
                service.Create(task);
            }
        }

    }
}