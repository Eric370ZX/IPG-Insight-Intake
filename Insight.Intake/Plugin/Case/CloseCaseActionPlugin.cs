using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class CloseCaseActionPlugin : PluginBase
    {
        public CloseCaseActionPlugin() : base(typeof(CloseCaseActionPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGCaseCloseCase", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            var pluginExecutionContext = localPluginContext.PluginExecutionContext;
            var caseRef = localPluginContext.TargetRef();
            var closeReason = localPluginContext.GetNullAbleInput<int>("CloseReason");
            var closeNote = localPluginContext.GetNullAbleInput<string>("CloseNotes");
            var closeReasonDescription = localPluginContext.GetNullAbleInput<string>("CloseReasonDescr");
            var closedBy = localPluginContext.GetNullAbleInput<string>("ClosedBy");
            var facilityCommunication = GetFacilityCommunication(pluginExecutionContext.InputParameters);
            var skipChecks = pluginExecutionContext.InputParameters.Contains("SkipChecks") ? localPluginContext.GetInput<bool>("SkipChecks") : false;
            var fillTargetFields = pluginExecutionContext.InputParameters.Contains("FillTargetFields") ? localPluginContext.GetInput<bool>("FillTargetFields") : true;
            var target = service.Retrieve(caseRef.LogicalName, caseRef.Id, new ColumnSet(true));

            var caseManager = new CaseManager(localPluginContext.OrganizationService, localPluginContext.TracingService, caseRef);
            if (!skipChecks && string.Equals(caseRef.LogicalName, Incident.EntityLogicalName))
            {
                var checkIfCaseCanBeClosed = caseManager.CheckIfCaseCanBeClosed();
                if (!checkIfCaseCanBeClosed.Succeeded)
                {
                    pluginExecutionContext.OutputParameters["Succeeded"] = false;
                    pluginExecutionContext.OutputParameters["Output"] = checkIfCaseCanBeClosed.Output;
                    return;
                }
            }

            if (string.Equals(caseRef.LogicalName, Incident.EntityLogicalName))
            {
                var relatedOrders = caseManager.RelatedActivePOs();
                if (relatedOrders.Any())
                {
                    caseManager.CreateAlertTask(relatedOrders);
                }
            }

            if (fillTargetFields)
            {
                var caseClosureReason = (ipg_Caseclosurereason)Enum.Parse(typeof(ipg_Caseclosurereason), closeReason.ToString());
                var closeStatus = caseManager.CreateCaseStatusResult(closedBy, facilityCommunication,
                    caseClosureReason, pluginExecutionContext.InitiatingUserId);

                closeStatus.Description = closeNote;
                caseManager.SetState(closeStatus);
                caseManager.CreateCloseCaseNote(closeReasonDescription, closeNote, closedBy, pluginExecutionContext.InitiatingUserId);
                var importantEventManager = new ImportantEventManager(service);
                //var eventId = (incident.ipg_StateCodeEnum == ipg_CaseStateCodes.Authorization || incident.ipg_StateCodeEnum == ipg_CaseStateCodes.Intake)
                //    ? Constants.EventIds.ET12 : Constants.EventIds.ET11;
                var eventId = Constants.EventIds.ET11;
                var eventDescriptionParam = new string[] { closeReasonDescription };
                importantEventManager.CreateImportantEventLog(target, pluginExecutionContext.InitiatingUserId, eventId, eventDescriptionParam);
                importantEventManager.SetCaseOrReferralPortalHeader(target, eventId);
            }
            else
            {
                var targetUpdate = new Entity(caseRef.LogicalName)
                {
                    Id = caseRef.Id
                };
                targetUpdate[string.Equals(caseRef.LogicalName, Incident.EntityLogicalName) ? Incident.Fields.ipg_CaseStatus : ipg_referral.Fields.ipg_casestatus] = new OptionSetValue((int)ipg_CaseStatus.Closed);
                service.Update(targetUpdate);
            }
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
        }

        private string GetFacilityCommunication(ParameterCollection inputParameters)
        {
            if (inputParameters.Contains("FacilityCommunication") && inputParameters["FacilityCommunication"] != null)
                return inputParameters["FacilityCommunication"] as string;

            return string.Empty;
        }
    }
}