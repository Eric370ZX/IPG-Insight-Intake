using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Case
{
    public class RemoveCaseFromPool : PluginBase
    {
        public RemoveCaseFromPool() : base(typeof(RemoveCaseFromPool))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Task.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.SystemOrganizationService;

            var target = localPluginContext.Target<Task>();
            var preImage = localPluginContext.PreImage<Task>();
            var caseRef = preImage != null && preImage.Contains(Task.Fields.RegardingObjectId)
                && preImage.RegardingObjectId.LogicalName == Incident.EntityLogicalName
                ? preImage.RegardingObjectId : null;
            if (target.Contains(Task.Fields.StateCode) 
                && (target.StateCode == TaskState.Completed || target.StateCode == TaskState.Canceled) 
                && caseRef != null 
                && CheckTaskCategoryAndTaskType(preImage.ipg_taskcategoryid, preImage.ipg_tasktypeid, service))
            {
                UpdateCaseManagerAndReason(caseRef.Id, service);
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

        private void UpdateCaseManagerAndReason(Guid caseId, IOrganizationService service)
        {
            var incident = service.Retrieve(Incident.EntityLogicalName,
                                            caseId,
                                            new ColumnSet(Incident.Fields.ipg_cmassignedid))
                                  .ToEntity<Incident>();
            service.Update(new Incident()
            {
                Id = incident.Id,
                ipg_Reasons = new OptionSetValue((int)ipg_CaseReasons.ProcedureCompletePendingInformation)
            });
        }
    }
}