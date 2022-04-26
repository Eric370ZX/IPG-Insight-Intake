using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Case
{
    public class UpdateCollectionVendorValueToSCG : PluginBase
    {
        public UpdateCollectionVendorValueToSCG() : base(typeof(UpdateCollectionVendorValueToSCG))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeActionsUpdateCollectionVendorValueToSCG", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);

            var incidents = (from task in crmContext.CreateQuery<ipg_statementgenerationtask>()
                         join incident in crmContext.CreateQuery<Incident>()
                         on task.ipg_caseid.Id equals incident.IncidentId
                         join configuration in crmContext.CreateQuery<ipg_statementgenerationeventconfiguration>()
                         on task.ipg_eventid.Id equals configuration.ipg_statementgenerationeventconfigurationId
                             where (task.ipg_EndDate <= DateTime.Today.Date)
                             && (configuration.ipg_name.Contains("S1") || configuration.ipg_name.Contains("S3"))
                             && (incident.ipg_CollectionVendor.Value == (int)Incident_ipg_CollectionVendor.IPG)
                             && (incident.ipg_BillToPatient.Value != (int)ipg_TwoOptions.No)
                         select incident.IncidentId).ToList();
            foreach(var guid in incidents)
            {
                if (!PatientPayment(crmContext, new EntityReference(Incident.EntityLogicalName, (Guid)guid)))
                {
                    Incident entity = new Incident
                    {
                        Id = (Guid)guid,
                        ipg_CollectionVendor = new OptionSetValue((int)Incident_ipg_CollectionVendor.SCG)
                    };
                    service.Update(entity);
                }
            }

        }

        private bool PatientPayment(OrganizationServiceContext crmContext, EntityReference incidentRef)
        {
            var payments = (from payment in crmContext.CreateQuery<ipg_payment>()
                             where (payment.ipg_CaseId.Id == incidentRef.Id) &&
                                (payment.ipg_MemberPaid_new != null) &&
                                (payment.ipg_MemberPaid_new.Value > 0)
                             select payment).ToList();
            return (payments.Count() > 0);
        }
    }
}
