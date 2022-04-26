using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using Insight.Intake.Plugin.Managers;

namespace Insight.Intake.Plugin.Claim
{
    public class CreateClaimTask : PluginBase
    {

        public CreateClaimTask() : base(typeof(CreateClaimTask))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Invoice.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Invoice.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            CreateTask(localPluginContext);
            UpdateStatementTask(localPluginContext);

        }
        private void CreateTask(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var crmContext = new OrganizationServiceContext(service);
                var taskMgr = new TaskManager(service, localPluginContext.TracingService);

                var target = ((Entity)context.InputParameters["Target"]).ToEntity<Invoice>();
                var invoice = service.Retrieve(target.LogicalName, target.Id,
                    new ColumnSet(Invoice.Fields.ipg_Status, Invoice.Fields.ipg_Reason,
                    Invoice.Fields.ipg_caseid))
                    .ToEntity<Invoice>();

                if (invoice.ipg_Status != null && invoice.ipg_Reason != null)
                {
                    var configs = (from config in crmContext.CreateQuery<ipg_claimtaskconfiguration>()
                                   where config.ipg_ClaimStatus.Value == invoice.ipg_Status.Value
                                   && config.ipg_ClaimReason.Value == invoice.ipg_Reason.Value
                                   && config.ipg_TaskType != null
                                   select config).ToList();

                    foreach (var config in configs)
                    {
                        taskMgr.CreateTask(invoice.ipg_caseid, config.ipg_TaskType);
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
        private void UpdateStatementTask(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var traceService = localPluginContext.TracingService;

            traceService.Trace($"UpdateStatementTask Started...");

            var relatedIncidentId = localPluginContext.Target<Invoice>()?.ipg_caseid?.Id;
            traceService.Trace($"Related Incident id = {relatedIncidentId.ToString()}");

            if (relatedIncidentId != null && relatedIncidentId != Guid.Empty)
            {
                var getStatementTaskByIncidentIdFilter = new FilterExpression();
                getStatementTaskByIncidentIdFilter.AddCondition(ipg_statementgenerationtask.Fields.ipg_caseid, ConditionOperator.Equal, relatedIncidentId);


                var getStatementTaskByIncidentIdQuery = new QueryExpression(ipg_statementgenerationtask.EntityLogicalName) { Criteria = getStatementTaskByIncidentIdFilter };

                traceService.Trace($"Getting statement generation task");
                var statementTask = service.RetrieveMultiple(getStatementTaskByIncidentIdQuery).Entities.FirstOrDefault();
                traceService.Trace($"Statement generation task Id = {statementTask?.Id.ToString() ?? "null"}");

                if (statementTask != null)
                {
                    try
                    {
                        traceService.Trace($"Updating the statement generation task");
                        var statementTaskToUpdate = new ipg_statementgenerationtask
                        {
                            Id = statementTask.Id,
                            ipg_ActiveStatement = false
                        };

                        service.Update(statementTaskToUpdate);
                        traceService.Trace($"Updated successfuly");
                    }
                    catch (Exception ex)
                    {
                        traceService.Trace($"Error while updating. Massage : {ex.Message}");
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                }else traceService.Trace($"Statement generation task is empty, nothing to update");
            }else traceService.Trace($"Related Incident id is empty, nothing to update");
        }
    }
}