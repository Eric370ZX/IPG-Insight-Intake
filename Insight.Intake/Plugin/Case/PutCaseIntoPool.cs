using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Case
{
    public class PutCaseIntoPool : PluginBase
    {
        public PutCaseIntoPool() : base(typeof(PutCaseIntoPool))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Task.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            var target = localPluginContext.Target<Task>();
            var caseRef = target.Contains(Task.Fields.RegardingObjectId)
                && target.RegardingObjectId != null
                && target.RegardingObjectId.LogicalName == Incident.EntityLogicalName
                ? target.RegardingObjectId : null;
            if (caseRef != null && CheckTaskCategoryAndTaskType(target.ipg_taskcategoryid, target.ipg_tasktypeid, service))
            {
                service.Update(new Incident()
                {
                    Id = caseRef.Id,
                    ipg_Reasons = new OptionSetValue((int)ipg_CaseReasons.ProcedureCompleteQueuedforBilling)
                });
            }
        }

        private bool CheckTaskCategoryAndTaskType(EntityReference taskCategoryRef, EntityReference taskTypeRef, IOrganizationService service)
        {
            if (taskCategoryRef != null && taskTypeRef != null)
            {
                var taskCategoryName = taskCategoryRef.Name ?? service.Retrieve(ipg_taskcategory.EntityLogicalName,
                                                                                taskCategoryRef.Id,
                                                                                new ColumnSet(ipg_taskcategory.Fields.ipg_name))
                                                                                .ToEntity<ipg_taskcategory>()
                                                                                .ipg_name;
                var taskTypeName = taskTypeRef.Name ?? service.Retrieve(ipg_tasktype.EntityLogicalName,
                                                                        taskTypeRef.Id,
                                                                        new ColumnSet(ipg_tasktype.Fields.ipg_name))
                                                                        .ToEntity<ipg_tasktype>()
                                                                        .ipg_name;
                return taskCategoryName == Constants.TaskCategoryNames.CaseProcessing
                    && taskTypeName == Constants.TaskTypeNames.RequestToCompleteCaseMgmtWork;
            }
            return false;
        }
    }
}