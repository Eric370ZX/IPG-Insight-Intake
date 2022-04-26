using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Account
{
    public class SetBillToPatient : PluginBase
    {
        public SetBillToPatient() : base(typeof(SetBillToPatient))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Intake.Account.EntityLogicalName, PostOperationCreateHandler);
        }
        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var targetCarrier = localPluginContext.Target<Intake.Account>();
            var postCarrier = localPluginContext.PostImage<Intake.Account>();
            if (postCarrier.CustomerTypeCodeEnum != Account_CustomerTypeCode.Carrier || targetCarrier.ipg_StatementToPatient == null)
            {
                return;
            }

            if (targetCarrier.ipg_StatementToPatient == false)
            {
                var relatedIncidents = GetIncidentsByCarrier(localPluginContext.SystemOrganizationService, postCarrier.Id);
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

        private IEnumerable<Entity> GetIncidentsByCarrier(IOrganizationService crmService, Guid carrierId)
        {
            var query = new QueryExpression(Incident.EntityLogicalName);
            query.Criteria.AddCondition(Incident.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierId);
            query.Criteria.AddCondition(Incident.Fields.StateCode, ConditionOperator.Equal, (int)IncidentState.Active);
            var result = crmService.RetrieveMultiple(query);
            return result.Entities;
        }
    }
}
