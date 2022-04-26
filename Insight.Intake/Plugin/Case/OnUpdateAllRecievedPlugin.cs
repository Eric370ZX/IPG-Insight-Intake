using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    //this plugin is deprecated according to CPI-22045
    [Obsolete]
    public class OnUpdateAllRecievedPlugin : PluginBase
    {
        public OnUpdateAllRecievedPlugin() : base(typeof(OnUpdateAllRecievedPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var userRef = new SystemUser() { Id = context.InitiatingUserId }.ToEntityReference();
            var crmService = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(crmService);

            var target = localPluginContext.PostImage<Incident>();
            var isAllRecieved = target.GetAttributeValue<bool>(nameof(Incident.ipg_isallreceived));

            var verifyAllRecievedTask = (from task in crmContext.CreateQuery<Task>()
                                         where task.RegardingObjectId.Id == target.Id
                                         && task.ipg_tasktypecodeEnum == ipg_TaskType1.VerifyAllreceived
                                         select new Task() {Id = (Guid)task.ActivityId }).FirstOrDefault();

            var state = isAllRecieved ? TaskState.Completed : TaskState.Open;
            var status = isAllRecieved ? Task_StatusCode.Resolved : Task_StatusCode.InProgress;

            if (verifyAllRecievedTask != null)
            {
                var task = new Task()
                {
                    Id = verifyAllRecievedTask.Id,
                    StateCode = state,
                    StatusCodeEnum = status,
                };

                crmService.Update(task);
            }

            crmService.Create(new ipg_gateactivity()
            {
                Subject = "Verify All Received",
                ipg_taskstatuscodeEnum = status == Task_StatusCode.Resolved ? ipg_gateactivity_ipg_taskstatuscode.Completed : (ipg_gateactivity_ipg_taskstatuscode)Enum.Parse(typeof(ipg_gateactivity_ipg_taskstatuscode), status.ToString(), true),
                ActualStart = target.ModifiedOn,
                OwnerId = target.ModifiedBy,
                RegardingObjectId = target.ToEntityReference()
            });
        }
    }
}
