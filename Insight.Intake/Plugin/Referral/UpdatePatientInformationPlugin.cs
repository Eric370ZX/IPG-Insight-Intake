using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Referral
{
    public class UpdatePatientInformationPlugin : PluginBase
    {
        public UpdatePatientInformationPlugin() : base(typeof(UpdatePatientInformationPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_referral.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_document.EntityLogicalName, BindDocsToPatient);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_document.EntityLogicalName, BindDocsToPatient);
        }

        private void BindDocsToPatient(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            if (context.Depth > 1)
            {
                return;
            }
            var service = localPluginContext.OrganizationService;

            var target = localPluginContext.PostImage<ipg_document>();
            var crmContext = new OrganizationServiceContext(service);
            
            if (target.ipg_ReferralId != null)
            {
                var referral = service.Retrieve(target.ipg_ReferralId.LogicalName, target.ipg_ReferralId.Id
                    , new ColumnSet(nameof(ipg_referral.ipg_SurgeryDate).ToLower()
                    , nameof(ipg_referral.ipg_PatientId).ToLower()
                    )).ToEntity<ipg_referral>();

                if (isMostRecentReferral(referral, crmContext))
                {
                    service.Update(new ipg_document() { Id = target.Id, ipg_patientid = referral.ipg_PatientId });
                }

            }
        }

        private bool isMostRecentReferral(ipg_referral target, OrganizationServiceContext crmContext)
        {
            if (target.ipg_SurgeryDate == null || target.ipg_PatientId == null)
            {
                return false;
            }

            var mostRecentCase = (from incident in crmContext.CreateQuery<Incident>()
                                  where incident.ipg_PatientId.Id == target.ipg_PatientId.Id
                                  orderby incident.ipg_SurgeryDate descending
                                  select new Incident()
                                  {
                                      Id = (Guid)incident.IncidentId,
                                      ipg_SurgeryDate = incident.ipg_SurgeryDate
                                  }).FirstOrDefault();

            if (mostRecentCase == null || mostRecentCase.ipg_SurgeryDate == null
                || target.ipg_SurgeryDate.Value.ToLocalTime().Date > mostRecentCase.ipg_SurgeryDate.Value.ToLocalTime().Date)
            {
                return true;
            }

            return false;
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var target = localPluginContext.PostImage<ipg_referral>();
            var crmContext = new OrganizationServiceContext(service);

            if (isMostRecentReferral(target, crmContext))
            {        
                    var docs = (from doc in crmContext.CreateQuery<ipg_document>()
                                where doc.ipg_ReferralId.Id == target.Id
                                && doc.StateCode == ipg_documentState.Active
                                && doc.ipg_patientid.Id != target.ipg_PatientId.Id
                                select new ipg_document() { Id = (Guid)doc.ipg_documentId }).ToList();

                    foreach (var doc in docs)
                    {
                        service.Update(new ipg_document() { 
                            Id = doc.Id,
                            ipg_patientid = target.ipg_PatientId });
                    }
            }
        }
    }
}
