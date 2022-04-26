using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.EntitlementNamespace
{
    public class SetBillToPatient : PluginBase
    {
        public SetBillToPatient() : base(typeof(SetBillToPatient))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Intake.Entitlement.EntityLogicalName, PostOperationCreateHandler);
        }
        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var targetEntitlement = localPluginContext.Target<Intake.Entitlement>();
            var postEntitlement = localPluginContext.PostImage<Intake.Entitlement>();
            if (postEntitlement.ipg_EntitlementTypeEnum != ipg_EntitlementTypes.FacilityCarrier|| postEntitlement.ipg_CarrierId==null|| postEntitlement.ipg_FacilityId == null)
            {
                return;
            }

            if (targetEntitlement.ipg_Billtopatient == false)
            {
                var relatedIncidents = GetIncidentsByCarrierAndFacility(localPluginContext.SystemOrganizationService, postEntitlement);
                foreach (var iIncident in relatedIncidents)
                {
                    localPluginContext.OrganizationService.Update(new Incident()
                    {
                        Id = iIncident.Id,
                        ipg_BillToPatientEnum = ipg_TwoOptions.No
                    });
                }
            }
        }

        private IEnumerable<Entity> GetIncidentsByCarrierAndFacility(Microsoft.Xrm.Sdk.IOrganizationService crmService, Intake.Entitlement entitlement)
        {
            var query = new QueryExpression(Incident.EntityLogicalName);
            query.Criteria.AddCondition(Incident.Fields.ipg_CarrierId, ConditionOperator.Equal, entitlement.ipg_CarrierId.Id);
            query.Criteria.AddCondition(Incident.Fields.ipg_FacilityId, ConditionOperator.Equal, entitlement.ipg_FacilityId.Id);
            if (entitlement.StartDate != null)
            {
                query.Criteria.AddCondition(Incident.Fields.ipg_SurgeryDate, ConditionOperator.OnOrAfter, entitlement.StartDate);
            }
            if (entitlement.EndDate != null)
            {
                query.Criteria.AddCondition(Incident.Fields.ipg_SurgeryDate, ConditionOperator.OnOrBefore, entitlement.EndDate);
            }
            var result = crmService.RetrieveMultiple(query);
            return result.Entities;
        }
    }
}
