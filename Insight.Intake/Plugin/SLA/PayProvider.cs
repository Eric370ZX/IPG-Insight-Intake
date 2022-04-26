using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.SLA
{
    public class PayProvider : PluginBase
    {
        private readonly int[] _statusesToProceed = new[]
        {
            (int)SalesOrder_StatusCode.CommtoMFG, (int)SalesOrder_StatusCode.CommtoFacility
        };

        public PayProvider() : base(typeof(PayProvider))
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
            if (salesOrder != null && _statusesToProceed.Contains(salesOrder.StatusCode.Value) && salesOrder.ipg_CaseId != null)
            {
                using (CrmServiceContext context = new CrmServiceContext(pluginContext.SystemOrganizationService))
                {
                    var taskManager = new TaskManager(pluginContext.OrganizationService,
                    pluginContext.TracingService,
                    null,
                    pluginContext.PluginExecutionContext.InitiatingUserId);

                    var taskType = taskManager.RetrieveTaskTypeByName(Constants.TaskTypeNames.SLAPayProvider);

                    var existingTask = context.TaskSet
                        .FirstOrDefault(t =>
                            t.RegardingObjectId != null && t.RegardingObjectId.Id == salesOrder.ipg_CaseId.Id &&
                            t.ipg_tasktypeid != null && t.ipg_tasktypeid == taskType.ToEntityReference());

                    if (existingTask == null)
                    {
                        var startDate = DateTime.Now;
                        var caseRef = salesOrder.ipg_CaseId;

                        context.AddObject(new Task()
                        {
                            Subject = Constants.TaskTypeNames.SLAPayProvider,
                            ScheduledStart = startDate,
                            ActualStart = startDate,
                            RegardingObjectId = caseRef,
                            ipg_caseid = caseRef,
                            ipg_taskcategoryid = taskType.ipg_taskcategoryid,
                            ipg_tasktypeid = taskType.ToEntityReference()
                        });

                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
