using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Plugin.Managers;

namespace Insight.Intake.Plugin.ClaimResponse
{
    public class FillRemarkCodePresentation : PluginBase
    {
        private const string RARC_TASK_NAME = "Add New RARC and Description";

        public FillRemarkCodePresentation() : base(typeof(FillRemarkCodePresentation))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_claimresponselineremark.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimresponselineremark.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Delete, ipg_claimresponselineremark.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var crmService = localPluginContext.OrganizationService;
                var tracingService = localPluginContext.TracingService;

                ipg_claimresponselineremark claimResponseLineRemarkRef = null;
                if (context.InputParameters["Target"] is Entity)
                {
                    var claimResponseLineRemark = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponselineremark>();
                    claimResponseLineRemarkRef = crmService.Retrieve(claimResponseLineRemark.LogicalName, claimResponseLineRemark.Id, new ColumnSet(nameof(ipg_claimresponselineremark.ipg_ClaimResponseLineId).ToLower())).ToEntity<ipg_claimresponselineremark>();
                }
                else
                {
                    var claimResponseLineRemark = ((EntityReference)context.InputParameters["Target"]);
                    claimResponseLineRemarkRef = crmService.Retrieve(claimResponseLineRemark.LogicalName, claimResponseLineRemark.Id, new ColumnSet(nameof(ipg_claimresponselineremark.ipg_ClaimResponseLineId).ToLower())).ToEntity<ipg_claimresponselineremark>();

                    if (claimResponseLineRemarkRef.ipg_ClaimResponseLineId == null)
                    {
                        return;
                    }
                }

                var queryExpression = new QueryExpression(ipg_claimresponselineremark.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(claimResponseLineRemarkRef.ipg_Code).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponselineremark.ipg_ClaimResponseLineId).ToLower(), ConditionOperator.Equal, claimResponseLineRemarkRef.ipg_ClaimResponseLineId.Id)
                        }
                    }
                };
                if (context.MessageName == MessageNames.Delete)
                {
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_claimresponselineremark.ipg_claimresponselineremarkId).ToLower(), ConditionOperator.NotEqual, claimResponseLineRemarkRef.Id));
                }
                EntityCollection claimResponseHeaders = crmService.RetrieveMultiple(queryExpression);
                List<string> list = new List<string>();
                foreach (Entity entity in claimResponseHeaders.Entities)
                {
                    if ((entity.GetAttributeValue<string>(nameof(claimResponseLineRemarkRef.ipg_Code).ToLower()) != null) && (!list.Exists(x => x == entity.GetAttributeValue<string>(nameof(claimResponseLineRemarkRef.ipg_Code).ToLower()))))
                        list.Add(entity.GetAttributeValue<string>(nameof(claimResponseLineRemarkRef.ipg_Code).ToLower()));
                }
                string remarkCodeString = "";
                list.ForEach(remarkCode => { remarkCodeString += (string.IsNullOrEmpty(remarkCodeString) ? "" : ", ") + remarkCode; });

                ipg_claimresponseline claimResponseLine = new ipg_claimresponseline();
                claimResponseLine.Id = claimResponseLineRemarkRef.ipg_ClaimResponseLineId.Id;
                claimResponseLine.ipg_RemarkCodeString = remarkCodeString;
                GetRemarkCodesDescription(crmService, tracingService, list);
                crmService.Update(claimResponseLine);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private string GetRemarkCodesDescription(IOrganizationService service, ITracingService tracing, List<string> remarkCodes)
        {
            string remarkCodesString = "";
            if (remarkCodes.Count > 0)
            {
                QueryExpression queryExpression = new QueryExpression(ipg_claimresponseremarkcode.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimresponseremarkcode.ipg_name).ToLower(), nameof(ipg_claimresponseremarkcode.ipg_Description).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.Or)
                    {
                    }
                };
                foreach (string remarkCode in remarkCodes)
                {
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_claimresponseremarkcode.ipg_name).ToLower(), ConditionOperator.Equal, remarkCode));
                }
                EntityCollection claimResponseRemarkCodes = service.RetrieveMultiple(queryExpression);
                Dictionary<string, string> codeNames = new Dictionary<string, string>();
                foreach (Entity claimResponseRemarkCode in claimResponseRemarkCodes.Entities)
                {
                    codeNames.Add(claimResponseRemarkCode.GetAttributeValue<string>(nameof(ipg_claimresponseremarkcode.ipg_name).ToLower()), claimResponseRemarkCode.GetAttributeValue<string>(nameof(ipg_claimresponseremarkcode.ipg_Description).ToLower()));
                }

                foreach (string remarkCode in remarkCodes)
                {
                    remarkCodesString += (String.IsNullOrEmpty(remarkCodesString) ? "" : ", ") + remarkCode;
                    if (codeNames.ContainsKey(remarkCode))
                    {
                        remarkCodesString += (String.IsNullOrEmpty(codeNames[remarkCode]) ? "" : " - " + codeNames[remarkCode]);
                        if (String.IsNullOrEmpty(codeNames[remarkCode]))
                        {
                            CreateTask(service, tracing, remarkCode);
                        }
                    }
                    else
                    {
                        CreateTask(service, tracing, remarkCode);
                    }
                }
            }
            return remarkCodesString;
        }

        private void CreateTask(IOrganizationService service, ITracingService tracing, string remarkCode)
        {
            Task task = new Task();
            var taskManager = new TaskManager(service, tracing);
            var rarcTask = taskManager.GetTaskTypeByName(RARC_TASK_NAME);

            if (rarcTask == null)
            {
                throw new InvalidPluginExecutionException($"Task Type with name {RARC_TASK_NAME} Does Not Exist!");
            }

            task.ipg_tasktypeid = rarcTask.ToEntityReference();
            task.Description = "Add new Remittance Advice Remark Code (RARC) and description to the database for: " + remarkCode;
            service.Create(task);
        }

    }
}