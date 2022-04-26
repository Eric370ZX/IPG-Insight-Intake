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
    public class CompletePayProvider : PluginBase
    {
        public CompletePayProvider() : base(typeof(CompletePayProvider))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "Update",
                SalesOrder.EntityLogicalName,
                OnUpdateAsync);
        }

        private void OnUpdateAsync(LocalPluginContext pluginContext)
        {
            SalesOrder target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<SalesOrder>();
            SalesOrder salesOrder = pluginContext.SystemOrganizationService.Retrieve<SalesOrder>(target.Id, new ColumnSet("ipg_caseid", "statuscode"));

            if (salesOrder.StatusCode.Value == (int)SalesOrder_StatusCode.Paid)
            {
                using (CrmServiceContext context = new CrmServiceContext(pluginContext.SystemOrganizationService))
                {
                    var taskManager = new TaskManager(pluginContext.OrganizationService,
                    pluginContext.TracingService,
                    null,
                    pluginContext.PluginExecutionContext.InitiatingUserId);

                    var taskType = taskManager.RetrieveTaskTypeByName(Constants.TaskTypeNames.SLAPayProvider);

                    var slaTask = context.TaskSet
                        .FirstOrDefault(t =>
                            t.RegardingObjectId.Id == salesOrder.ipg_CaseId.Id && t.ipg_tasktypeid.Id == taskType.Id &&
                            t.StateCode.Value == TaskState.Open);

                    if (slaTask != null)
                    {
                        slaTask.StateCode = TaskState.Completed;
                        slaTask.StatusCode = TaskStatus.RanToCompletion.ToOptionSetValue();
                        slaTask.ActualEnd = DateTime.Now;
                        slaTask.Subcategory = slaTask.ActualEnd <= slaTask.ScheduledEnd ? Constants.TaskSubCategoryNames.SLAMet : Constants.TaskSubCategoryNames.SLANotMet;

                        context.UpdateObject(slaTask);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
