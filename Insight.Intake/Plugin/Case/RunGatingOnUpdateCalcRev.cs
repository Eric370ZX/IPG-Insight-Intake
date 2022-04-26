using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace Insight.Intake.Plugin.Case
{
    public class RunGatingOnUpdateCalcRev : PluginBase
    {
        public RunGatingOnUpdateCalcRev() : base(typeof(RunGatingOnUpdateCalcRev))
        {
            RegisterEvent(PipelineStages.PostOperation, "Update", Incident.EntityLogicalName, PostOperationHandler);
        }
        private void RunGateByName(EntityReference caseRef, Guid InitiatingUserId, OrganizationServiceContext crmContext, IOrganizationService service)
        {
            var gateExecutionAction = new OrganizationRequest(Constants.ActionNames.GatingStartGateProcessing);
            gateExecutionAction.Parameters.Add("Target", new EntityReference(Incident.EntityLogicalName, caseRef.Id));
            gateExecutionAction.Parameters.Add("InitiatingUser", new EntityReference(SystemUser.EntityLogicalName, InitiatingUserId));
            service.Execute(gateExecutionAction);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var incident = (Entity)context.InputParameters["Target"];
            var crmContext = new OrganizationServiceContext(service);

            RunGateByName(incident.ToEntityReference(), context.InitiatingUserId, crmContext, service);
        }
    }
}