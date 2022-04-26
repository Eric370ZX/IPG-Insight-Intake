using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.User
{
    public class CreateContactAccount : PluginBase
    {
        public CreateContactAccount() : base(typeof(CreateContactAccount))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Associate, "systemuserroles_association", PostOperationHandlerOnAssociateSysAdmin);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Intake.Account.EntityLogicalName, PostOperationHandlerOnAccountCreate);
        }

        private void PostOperationHandlerOnAssociateSysAdmin(LocalPluginContext obj)
        {
            var portalContactMgr = new PortalContactManager(obj.OrganizationService, obj.TracingService);
            var targetRef = obj.TargetRef();
            var target = obj.OrganizationService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(SystemUser.Fields.InternalEMailAddress)).ToEntity<SystemUser>();
            var roles = obj.GetNullAbleInput<EntityReferenceCollection>("RelatedEntities");


            if (portalContactMgr.IsSysAdmineRole(roles.ToList()))
            {
                var contact = portalContactMgr.GetContactFromSysAdminUser(target);
                if(contact == null)
                {
                    obj.TracingService.Trace($"System Administrator cannot be associate to active facilities, Portal Contact must be created for that System User first with email {target.InternalEMailAddress}!");
                }
                else
                {
                    var facilities = portalContactMgr.GetActiveFacilitiesNotConnectedWithContact(contact.ToEntityReference());
                    
                    foreach (var facility in facilities)
                    {
                        portalContactMgr.CreateAdminContactRelationShipWithFacility(contact.ToEntityReference(), facility.ToEntityReference());
                    }
                }
            }

        }

        private void PostOperationHandlerOnAccountCreate(LocalPluginContext obj)
        {
            var portalContactMgr = new PortalContactManager(obj.OrganizationService, obj.TracingService);
            var target = obj.Target<Intake.Account>();

            if (portalContactMgr.IsActiveFacility(target))
            {
                var portalContacts = portalContactMgr.GetPortalContactsFromSysAdminUsers();

                foreach (var portalContact in portalContacts)
                {
                    portalContactMgr.CreateAdminContactRelationShipWithFacility(portalContact.ToEntityReference(), target.ToEntityReference());
                }
            }
        }
    }
}
