using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Adjustment
{
    public class ProcessAdjustment : PluginBase
    {
        public ProcessAdjustment() : base(typeof(ProcessAdjustment))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_adjustment.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var adjustment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_adjustment>();

            if (adjustment.ipg_CaseId == null)
            {
                throw new Exception("Adjustment must have an Incident reference");
            }
            CreateNote(service, adjustment, adjustment.ipg_CaseId);
            UpdateCaseStatus(service, adjustment, adjustment.ipg_CaseId);
            CloseAdjustmentTask(service, adjustment, adjustment.ipg_CaseId);

        }

        private void CreateNote(IOrganizationService service, ipg_adjustment adjustment, EntityReference caseRef)
        {
            var annotation = new Annotation();
            annotation.ObjectId = caseRef;
            annotation.ObjectTypeCode = caseRef.LogicalName;
            annotation.Subject = "Adjustment";
            if (adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.WriteOff)
            {
                annotation.NoteText = adjustment.FormattedValues[nameof(ipg_adjustment.ipg_ApplyTo).ToLower()] + " balance of $" + adjustment.ipg_AmountToApply.Value.ToString("0.##") + " was written off due to " + (adjustment.ipg_Reason == null ? "" : adjustment.FormattedValues[nameof(ipg_adjustment.ipg_Reason).ToLower()]) + ". " + adjustment.ipg_Note;
            }
            else if (adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.SalesAdjustment)
            {
                if (adjustment.ipg_AmountType ?? false)
                {
                    annotation.NoteText = adjustment.FormattedValues[nameof(ipg_adjustment.ipg_ApplyTo).ToLower()] + " " + adjustment.FormattedValues[nameof(ipg_adjustment.ipg_AdjustmentType).ToLower()] + " of $" + adjustment.ipg_AmountToApply.Value.ToString("0.##") + " applied. Notes:" + adjustment.ipg_Note;
                }
                else
                {
                    annotation.NoteText = adjustment.FormattedValues[nameof(ipg_adjustment.ipg_ApplyTo).ToLower()] + " " + adjustment.FormattedValues[nameof(ipg_adjustment.ipg_AdjustmentType).ToLower()] + " of " + ((decimal)adjustment.ipg_Percent).ToString("0.##") + "% applied. Notes:" + adjustment.ipg_Note;
                }
            }
            else if (adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.TransferofResponsibility)
            {
                annotation.NoteText = "Balance of " + adjustment.FormattedValues[nameof(ipg_adjustment.ipg_AmountToApply).ToLower()] + " transferred from " + adjustment.FormattedValues[nameof(ipg_adjustment.ipg_From).ToLower()] + " to " + adjustment.FormattedValues[nameof(ipg_adjustment.ipg_To).ToLower()] + ". Notes:" + adjustment.ipg_Note;
            }
            service.Create(annotation);
        }

        private void UpdateCaseStatus(IOrganizationService service, ipg_adjustment adjustment, EntityReference caseRef)
        {
            if (adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.TransferofResponsibility)
            {
                if((adjustment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier) && (adjustment.ipg_To.Value == (int)ipg_PayerType.Patient))
                {
                    var incident = new Incident();
                    incident.Id = caseRef.Id;
                    incident[nameof(incident.ipg_Reasons).ToLower()] = new OptionSetValue((int)ipg_CaseReasons.PartialPatientResponsibility);
                    service.Update(incident);
                }
                else if ((adjustment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier) && (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier))
                {
                    var incident = new Incident();
                    incident.Id = caseRef.Id;
                    incident[nameof(incident.ipg_Reasons).ToLower()] = new OptionSetValue((int)ipg_CaseReasons.PendingSecondaryClaim);
                    service.Update(incident);
                }
            }
        }

        private void CloseAdjustmentTask(IOrganizationService service, ipg_adjustment adjustment, EntityReference caseRef)
        {
            var crmContext = new OrganizationServiceContext(service);
            var openedTasks = (from task in crmContext.CreateQuery<Task>()
                         where task.RegardingObjectId.Id == caseRef.Id &&
                             task.StateCode == (int)TaskState.Open &&
                             task.ipg_tasktypecode != null &&
                             task.ipg_tasktypecode.Value == 427880065
                             select task).ToList();
            foreach (var task in openedTasks)
            {
                var updTask = new Task();
                updTask.Id = task.Id;
                var setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = task.ToEntityReference(),
                    State = new OptionSetValue((int)TaskState.Completed),
                    Status = new OptionSetValue((int)Task_StatusCode.Resolved)
                };
                service.Execute(setStateRequest);
            }
        }
    }
}