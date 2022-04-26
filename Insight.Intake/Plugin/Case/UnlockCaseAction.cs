using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Case
{
    public class UnlockCaseAction : PluginBase
    {
        public UnlockCaseAction() : base(typeof(UnlockCaseAction))
        {
            RegisterEvent(PipelineStages.PostOperation, new ipg_IPGCaseActionsUnlockCaseRequest().RequestName, Incident.EntityLogicalName, PostOperationHandlerUnlockCase);
        }

        private void PostOperationHandlerUnlockCase(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var request = new ipg_IPGCaseActionsUnlockCaseRequest() { Parameters = context.InputParameters };

            var incident = service.Retrieve(request.Target.LogicalName, request.Target.Id, new ColumnSet(true)).ToEntity<Incident>();

            if(incident.ipg_islocked != true)
            {
                throw new Exception("Case not Locked!");
            }

            service.Update(new Incident() {Id = incident.Id,  ipg_islocked = false });

            CreateET10Log(service, context, incident, request.Reason, request.Notes);
        }

        private void CreateET10Log(IOrganizationService service, IPluginExecutionContext context, Incident target, params string[] eventDescriptionParam)
        {
            var importantEventManager = new ImportantEventManager(service);
            importantEventManager.CreateImportantEventLog(target, context.InitiatingUserId, Constants.EventIds.ET10, eventDescriptionParam);
            importantEventManager.SetCaseOrReferralPortalHeader(target, Constants.EventIds.ET10);
        }
    }
}
