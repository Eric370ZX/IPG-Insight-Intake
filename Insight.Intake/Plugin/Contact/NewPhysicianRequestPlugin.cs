using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Contact
{
    public class NewPhysicianRequestPlugin : PluginBase
    {
        IOrganizationService CrmService { get; set; }
        const string PhysicianTypeName = "Physician";

        public NewPhysicianRequestPlugin() : base(typeof(NewPhysicianRequestPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Intake.Contact.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localContext)
        {
            var contact = localContext.Target<Intake.Contact>();
            CrmService = localContext.OrganizationService;
            if (IsContactPhysician(contact.ipg_ContactTypeId))
            {
                contact.ParentCustomerId = contact?.ipg_currentfacilityid;
                var relation = CreateFacilityPhysicianRelation(contact?.ipg_currentfacilityid, new EntityReference(Intake.Contact.EntityLogicalName, contact.Id));
            }
        }

        private bool IsContactPhysician(EntityReference contactTypeRef)
        {
            if (contactTypeRef == null)
            {
                return false;
            }
            var query = new QueryExpression
            {
                EntityName = ipg_contacttype.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_contacttype.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_contacttype.Fields.ipg_contacttypeId, ConditionOperator.Equal, contactTypeRef.Id),
                        new ConditionExpression(ipg_contacttype.Fields.StateCode, ConditionOperator.Equal, (int)ipg_contacttypeState.Active),
                        new ConditionExpression(ipg_contacttype.Fields.ipg_name, ConditionOperator.Equal, PhysicianTypeName),
                    }
                }
            };
            return CrmService.RetrieveMultiple(query).Entities.Any();
        }

        private System.Guid CreateFacilityPhysicianRelation(EntityReference facilityId, EntityReference physicianId)
        {
            return CrmService.Create(new ipg_facilityphysician
            {
                ipg_facilityid = facilityId,
                ipg_physicianid = physicianId
            });
        }
    }
}
