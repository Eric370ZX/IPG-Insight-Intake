using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.FacilityEhrConnection
{
    public class UpdateStatusOnFacility : PluginBase
    {
        private ITracingService _tracingService;
        
        public UpdateStatusOnFacility() : base(typeof(EhrConnectionDatesValidationPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_FacilityEhrConnection.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_FacilityEhrConnection.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext context)
        {
            _tracingService = context.TracingService;

            _tracingService.Trace("Getting Target");
            var target = context.Target<ipg_FacilityEhrConnection>();
            var preImage = context.PreImage<ipg_FacilityEhrConnection>();
            SetIsActiveEhrConnection(target, preImage, context.OrganizationService);           
        }

        private void SetIsActiveEhrConnection(ipg_FacilityEhrConnection target, ipg_FacilityEhrConnection preImage, IOrganizationService crmService)
        {
            _tracingService.Trace("Getting entity values");
            var facilityReference = target.Contains(ipg_FacilityEhrConnection.Fields.ipg_FacilityId) ? target.ipg_FacilityId : preImage?.ipg_FacilityId;
            if(facilityReference != null)
            {
                var facility = crmService.Retrieve(Intake.Account.EntityLogicalName, facilityReference.Id, new ColumnSet(Intake.Account.Fields.ipg_enrolledinehr)).ToEntity<Intake.Account>();

                using (var crmContext = new OrganizationServiceContext(crmService))
                {
                    _tracingService.Trace("Retrieving facility ranges");
                    var now = DateTime.Now;
                    var activeRange = (from range in crmContext.CreateQuery<ipg_FacilityEhrConnection>()
                                  where range.StateCode == ipg_FacilityEhrConnectionState.Active
                                        && range.ipg_FacilityId.Id == facilityReference.Id
                                        && (range.ipg_EhrProcedureEffectiveDate == null || range.ipg_EhrProcedureEffectiveDate <= now)
                                        && (range.ipg_EhrProcedureExpirationDate == null || range.ipg_EhrProcedureExpirationDate >= now)
                                  select range).FirstOrDefault();

                    Account_ipg_enrolledinehr newValue = activeRange != null ? Account_ipg_enrolledinehr.Yes : Account_ipg_enrolledinehr.No;

                    if(facility.ipg_enrolledinehrEnum != newValue)
                    {
                        _tracingService.Trace("Updating the indicator on facility");
                        facility.ipg_enrolledinehrEnum = newValue;
                        crmService.Update(facility);
                    }
                }
            }
        }
    }
}
