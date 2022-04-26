using System;
using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Case
{
    public class DetermineCMtoAssign : PluginBase
    {
        public DetermineCMtoAssign() : base(typeof(DetermineCStoAssign))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGCaseActionsDetermineCMtoAssign", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            localPluginContext.PluginExecutionContext.OutputParameters["AssignToTeamId"] = Constants.TeamGuids.CaseManagement.ToString();

            var service = localPluginContext.OrganizationService;
            var context = localPluginContext.PluginExecutionContext;

            var caseRef = context.InputParameters.Contains("Target")
                ? localPluginContext.GetInput<EntityReference>("Target")
                : null;

            var facilityRef = context.InputParameters.Contains("FacilityRef")
                ? localPluginContext.GetInput<EntityReference>("FacilityRef")
                : null;

            if (caseRef == null) return;

            var caseUpd = new Entity(caseRef.LogicalName, caseRef.Id)
            {
                [Incident.Fields.ipg_assignedtoteamid] = new EntityReference(Team.EntityLogicalName, Constants.TeamGuids.CaseManagement)
            }.ToEntity<Intake.Incident>();


            if (facilityRef != null)
            {
                var facility = service.Retrieve(Intake.Account.EntityLogicalName, facilityRef.Id, new ColumnSet(Intake.Account.Fields.ipg_FacilityCaseMgrId)).ToEntity<Intake.Account>();
                var caseMngr = facility?.ipg_FacilityCaseMgrId;

                if (caseMngr != null)
                {
                    caseUpd.OwnerId = caseMngr;
                    caseUpd.ipg_assignedtoteamid = null;
                    localPluginContext.PluginExecutionContext.OutputParameters["AssignToUserId"] = caseMngr.Id.ToString();
                }
            }

            service.Update(caseUpd);
        }
    }
}