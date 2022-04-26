using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Contact
{
    public class Impersonation:PluginBase
    {
        public Impersonation() : base(typeof(Impersonation))
        {
            RegisterEvent(PipelineStages.PostOperation, new ipg_IPGIntakeContactActionsImpersonateRequest().RequestName, Intake.Contact.EntityLogicalName, PostOperationImpersonate);
            RegisterEvent(PipelineStages.PostOperation, new ipg_IPGIntakeContactActionsRevertBackImpersonationRequest().RequestName, Intake.Contact.EntityLogicalName, PostOperationRevertBack);
        }

        private void PostOperationRevertBack(LocalPluginContext obj)
        {
            var targetRef = obj.TargetRef();
            var contact = obj.OrganizationService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(Intake.Contact.Fields.ipg_backupjson)).ToEntity<Intake.Contact>();
            var sysUserRef = new EntityReference(SystemUser.EntityLogicalName, obj.PluginExecutionContext.InitiatingUserId);
            var portalContactMgr = new PortalContactManager(obj.OrganizationService, obj.TracingService);
            var sysUser = portalContactMgr.GetSysUserWithAdminSecurityRole(sysUserRef);
            if (portalContactMgr.IsSysUserHaveAdminRole(sysUser) && !string.IsNullOrEmpty(contact.ipg_backupjson))
            {
                portalContactMgr.RestoreFacilityRelationShipAndWebRoles(contact);
            }
        }

        private void PostOperationImpersonate(LocalPluginContext obj)
        {
            var target = obj.TargetRef();
            var sysUserRef = new EntityReference(SystemUser.EntityLogicalName, obj.PluginExecutionContext.InitiatingUserId);
            var portalContactMgr = new PortalContactManager(obj.OrganizationService, obj.TracingService);
            var sysUser = portalContactMgr.GetSysUserWithAdminSecurityRole(sysUserRef);
            if (portalContactMgr.IsSysUserHaveAdminRole(sysUser))
            {
                var sysUserContactRef = portalContactMgr.GetContactFromSysAdminUser(sysUser)?.ToEntityReference();
                if (sysUserContactRef != null)
                {
                    portalContactMgr.BackUpFacilityRelationShipAndWebRoles(sysUserContactRef);
                    portalContactMgr.Impersonate(target, sysUserContactRef);
                }
            }
            else
            {
                throw new Exception("You don't have Portal Contact with the same email as your User!");
            }
        }
    }
}
