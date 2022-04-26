using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.SLA
{
    public class CompleteGeneratePo : PluginBase
    {
        public CompleteGeneratePo() : base(typeof(CompleteGeneratePo))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "Update",
                SalesOrder.EntityLogicalName,
                OnUpdateAsync);

            RegisterEvent(
                PipelineStages.PostOperation,
                "Create",
                SalesOrder.EntityLogicalName,
                OnCreateAsync);
        }

        private void OnCreateAsync(LocalPluginContext pluginContext)
        {
            SalesOrder target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<SalesOrder>();

            ProcessSalesOrder(target, pluginContext);
        }

        private void OnUpdateAsync(LocalPluginContext pluginContext)
        {
            SalesOrder target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<SalesOrder>();
            SalesOrder salesOrder = pluginContext.SystemOrganizationService.Retrieve<SalesOrder>(target.Id, new ColumnSet("ipg_caseid", "statuscode"));

            ProcessSalesOrder(salesOrder, pluginContext);
        }

        private void ProcessSalesOrder(SalesOrder salesOrder, LocalPluginContext pluginContext)
        {
            if (salesOrder.StatusCode.Value == (int)SalesOrder_StatusCode.Generated)
            {
                using (CrmServiceContext context = new CrmServiceContext(pluginContext.SystemOrganizationService))
                {
                    var taskManager = new TaskManager(pluginContext.OrganizationService,
                    pluginContext.TracingService,
                    null,
                    pluginContext.PluginExecutionContext.InitiatingUserId);

                    var taskType = taskManager.RetrieveTaskTypeByName(Constants.TaskTypeNames.SLAGeneratePO);

                    var slaTask = context.TaskSet
                        .FirstOrDefault(t =>
                            t.RegardingObjectId.Id == salesOrder.ipg_CaseId.Id && t.ipg_tasktypeid.Id == taskType.Id &&
                            t.StateCode.Value == TaskState.Open);

                    if (slaTask != null)
                    {
                        slaTask.StateCode = TaskState.Completed;
                        slaTask.StatusCode = TaskStatus.RanToCompletion.ToOptionSetValue();
                        slaTask.ActualEnd = DateTime.UtcNow;
                        slaTask.Subcategory = slaTask.ActualEnd <= slaTask.ScheduledEnd ? Constants.TaskSubCategoryNames.SLAMet : Constants.TaskSubCategoryNames.SLANotMet;

                        context.UpdateObject(slaTask);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}