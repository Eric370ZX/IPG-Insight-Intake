using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckCarrierIsAccepted : PluginBase
    {
        public CheckCarrierIsAccepted() : base(typeof(CheckCarrierIsAccepted))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCarrierIsAccepted", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var gateManager = new GateManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var result = gateManager.CheckCarrierIsAccepted();

            var target = localPluginContext.SystemOrganizationService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(ipg_referral.Fields.OwnerId,ipg_referral.Fields.ipg_CarrierId));
            var ownerName = "";
            if (target.GetAttributeValue<EntityReference>(ipg_referral.Fields.OwnerId).LogicalName == SystemUser.EntityLogicalName)
            {
                var user = localPluginContext.SystemOrganizationService
                    .Retrieve(target.GetAttributeValue<EntityReference>(ipg_referral.Fields.OwnerId).LogicalName, target.GetAttributeValue<EntityReference>(ipg_referral.Fields.OwnerId).Id, new ColumnSet(SystemUser.Fields.FullName))
                    .ToEntity<SystemUser>();
                ownerName = user.FullName;
            }
            else if(target.GetAttributeValue<EntityReference>(ipg_referral.Fields.OwnerId).LogicalName == Team.EntityLogicalName)
            {
                var team = localPluginContext.SystemOrganizationService
                    .Retrieve(target.GetAttributeValue<EntityReference>(ipg_referral.Fields.OwnerId).LogicalName, target.GetAttributeValue<EntityReference>(ipg_referral.Fields.OwnerId).Id, new ColumnSet(Team.Fields.Name))
                    .ToEntity<Team>();
                ownerName = team.Name;
            }
            var carrier= localPluginContext.SystemOrganizationService.Retrieve(target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CarrierId).LogicalName, target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CarrierId).Id, new ColumnSet(Intake.Account.Fields.Name));

            var caseNote = $@"IPG has rejected this case due to the patient’s health plan, 
            {carrier.GetAttributeValue<string>(Intake.Account.Fields.Name)}, is non-participating with IPG. If the patient has updated insurance information 
            which indicates an IPG covered health plan is primary, please notify your CIM, {ownerName}, 
            for reconsideration of this case.";
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = result;
            localPluginContext.PluginExecutionContext.OutputParameters["CaseNote"] = caseNote;
        }
    }
}
