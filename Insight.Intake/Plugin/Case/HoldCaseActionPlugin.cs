using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class HoldCaseActionPlugin : PluginBase
    {
        public HoldCaseActionPlugin() : base(typeof(HoldCaseActionPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGCaseActionsHoldCase", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var caseRef = localPluginContext.TargetRef();
            var holdReason = localPluginContext.GetNullAbleInput<OptionSetValue>("HoldReason");
            var isOnHold = localPluginContext.GetInput<bool>("IsOnHold");
            var caseManager = new CaseManager(localPluginContext.OrganizationService, localPluginContext.TracingService, caseRef);
            var importantEventManager = new ImportantEventManager(service);
            string eventID;
            string[] eventDescriptionParams = null;

            if (holdReason == null && isOnHold)
            {
                localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = false;
                localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "Hold reason is empty";
                return;
            }
            var holdNote = localPluginContext.GetNullAbleInput<string>("HoldNote");
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            var caseHoldReason = holdReason?.ToEnum<ipg_Caseholdreason>();

            try
            {
                var incident = service.Retrieve(caseRef.LogicalName, caseRef.Id,
                    new ColumnSet(Incident.Fields.ipg_casestatusdisplayedid,
                                  Incident.Fields.Title,
                                  Incident.Fields.StateCode,
                                  Incident.Fields.ipg_StateCode)).ToEntity<Incident>();

                if (isOnHold)
                {
                    if (holdReason == null)
                    {
                        throw new Exception("HoldReason cannot be empty on put Case on Hold!");
                    }

                    caseManager.SetHold(isOnHold, caseHoldReason);
                    if (incident.ipg_casestatusdisplayedid != null)
                    {
                        var holdConfigs = caseManager.GetCaseholdParameters();
                        var actualHoldConfigs = holdConfigs
                            .Where(p => p.CaseStatus?.Id == incident.ipg_casestatusdisplayedid.Id
                            && (int)p.CaseHoldReason == (int)caseHoldReason);
                        if (actualHoldConfigs.Any())
                        {
                            caseManager.ClouseOutstangindTasks();
                        }
                    }
                    caseManager.CreateHoldNote(holdNote, caseHoldReason, true);
                    eventID = Constants.EventIds.ET5;
                    eventDescriptionParams = new string[] { caseHoldReason.ToString() };
                    CreateHoldTask(incident, holdReason.Value, holdNote, caseManager, service);
                }
                else
                {
                    caseManager.SetHold(isOnHold, null);
                    caseManager.ClouseOutstangindTasks();
                    caseManager.CreateHoldNote(holdNote, null, false, "Patient case is not on hold");
                    eventID = Constants.EventIds.ET6;
                }
                importantEventManager.CreateImportantEventLog(incident, localPluginContext.PluginExecutionContext.InitiatingUserId, eventID, eventDescriptionParams);
                importantEventManager.SetCaseOrReferralPortalHeader(incident, eventID);
            }
            catch (Exception ex)
            {
                localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = false;
                localPluginContext.PluginExecutionContext.OutputParameters["Output"] = ex.Message;
            }

        }
        private void CreateHoldTask(Incident incident, int reason, string holdNote, CaseManager manager, IOrganizationService service)
        {
            var caseHoldConfiguration = new OrganizationServiceContext(service).CreateQuery<ipg_caseholdconfiguration>()
                .FirstOrDefault(config => config.ipg_casestate == incident.ipg_StateCode.Value.ToOptionSetValue()
                && config.ipg_caseholdreason.Value == reason);
            if (caseHoldConfiguration != null && !string.IsNullOrEmpty(caseHoldConfiguration.ipg_taskid))
            {
                manager.CreateTask(incident.ToEntityReference(), caseHoldConfiguration.ipg_taskid);
            }
        }
    }
}
