using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Case
{
    public class SetCaseManagerFromPoolTask : PluginBase
    {
        //create new method in task manager to check Pool task

        public SetCaseManagerFromPoolTask() : base(typeof(SetCaseManagerFromPoolTask))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Task.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            var target = localPluginContext.Target<Task>();
            var preImage = localPluginContext.PreImage<Task>();
            var caseRef = preImage != null && preImage.Contains(Task.Fields.RegardingObjectId)
                && preImage.RegardingObjectId.LogicalName == Incident.EntityLogicalName
                ? preImage.RegardingObjectId : null;
            if (target.Contains(Task.Fields.OwnerId) && target.OwnerId.LogicalName == SystemUser.EntityLogicalName 
                && caseRef != null 
                && CheckTaskCategoryAndTaskType(preImage.ipg_taskcategoryid, preImage.ipg_tasktypeid, service))
            {
                service.Update(new Incident()
                {
                    Id = caseRef.Id,
                    ipg_CaseManagerId = target.OwnerId
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