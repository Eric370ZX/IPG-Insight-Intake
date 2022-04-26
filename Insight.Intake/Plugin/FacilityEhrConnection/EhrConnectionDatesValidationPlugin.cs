using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.FacilityEhrConnection
{
    public class EhrConnectionDatesValidationPlugin : PluginBase
    {
        public static readonly string InvalidProcedureRangeExceptionMessage = "'EHR Procedure Effective Date' cannot be greater than 'EHR Procedure Expiration Date'";
        public static readonly string OverlappingExceptionMessage = "EHR Procedure Effective and Expiration Dates cannot overlap for the same EHR Software Name";

        private ITracingService _tracingService;
        
        public EhrConnectionDatesValidationPlugin() : base(typeof(EhrConnectionDatesValidationPlugin))
        {
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Update, ipg_FacilityEhrConnection.EntityLogicalName, PreValidationHandler);
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Create, ipg_FacilityEhrConnection.EntityLogicalName, PreValidationHandler);
        }

        private void PreValidationHandler(LocalPluginContext context)
        {
            _tracingService = context.TracingService;

            _tracingService.Trace("Getting Target and PreImage");
            var target = context.Target<ipg_FacilityEhrConnection>();
            var preImage = context.PreImage<ipg_FacilityEhrConnection>();

            DoValidations(target, preImage, context.OrganizationService);           
        }

        private void DoValidations(ipg_FacilityEhrConnection target, ipg_FacilityEhrConnection preImage, IOrganizationService crmService)
        {
            _tracingService.Trace("Retrieving dates");
            var procedureEffectiveDate = target.Contains(nameof(ipg_FacilityEhrConnection.ipg_EhrProcedureEffectiveDate).ToLower()) ? target.ipg_EhrProcedureEffectiveDate : preImage?.ipg_EhrProcedureEffectiveDate;
            var procedureExpirationDate = target.Contains(nameof(ipg_FacilityEhrConnection.ipg_EhrProcedureExpirationDate).ToLower()) ? target.ipg_EhrProcedureExpirationDate : preImage?.ipg_EhrProcedureExpirationDate;

            _tracingService.Trace("Validating procedure date range");
            if (procedureEffectiveDate > procedureExpirationDate)
            {
                throw new InvalidPluginExecutionException(InvalidProcedureRangeExceptionMessage);
            }

            _tracingService.Trace("Getting entity values");
            var id = target.Contains(nameof(ipg_FacilityEhrConnection.ipg_FacilityEhrConnectionId).ToLower()) ? target.ipg_FacilityEhrConnectionId : preImage?.ipg_FacilityEhrConnectionId;
            var statecode = target.Contains(nameof(ipg_FacilityEhrConnection.StateCode).ToLower()) ? target.StateCode : preImage?.StateCode;
            var facilityReference = target.Contains(nameof(ipg_FacilityEhrConnection.ipg_FacilityId).ToLower()) ? target.ipg_FacilityId : preImage?.ipg_FacilityId;
            var ehrSoftware = target.Contains(nameof(ipg_FacilityEhrConnection.ipg_EhrSoftware).ToLower()) ? target.ipg_EhrSoftwareEnum : preImage?.ipg_EhrSoftwareEnum;
            if ((statecode == ipg_FacilityEhrConnectionState.Active || id == null /*new record*/)
                && facilityReference != null)
            {
                using(var crmContext = new OrganizationServiceContext(crmService))
                {
                    _tracingService.Trace("Retrieving ranges");
                    var ranges = (from range in crmContext.CreateQuery<ipg_FacilityEhrConnection>()
                                  where range.StateCode == ipg_FacilityEhrConnectionState.Active
                                        && range.ipg_FacilityId.Id == facilityReference.Id
                                        && range.ipg_EhrSoftwareEnum == ehrSoftware
                                  select range).ToList();

                    if (ranges.Any())
                    {
                        //comparing dates in .NET because CRM does not support DateTime Min and Max values
                        _tracingService.Trace("Searching for conflicting ranges");
                        if (ranges.Any(r => (id == null || r.ipg_FacilityEhrConnectionId != id)
                            && (procedureEffectiveDate >= (r.ipg_EhrProcedureEffectiveDate ?? DateTime.MinValue) && procedureEffectiveDate <= (r.ipg_EhrProcedureExpirationDate ?? DateTime.MaxValue)
                                || procedureExpirationDate >= (r.ipg_EhrProcedureEffectiveDate ?? DateTime.MinValue) && procedureExpirationDate <= (r.ipg_EhrProcedureExpirationDate ?? DateTime.MaxValue)
                                || r.ipg_EhrProcedureEffectiveDate >= (procedureEffectiveDate ?? DateTime.MinValue) && r.ipg_EhrProcedureEffectiveDate <= (procedureExpirationDate ?? DateTime.MaxValue)
                                || r.ipg_EhrProcedureExpirationDate >= (procedureEffectiveDate ?? DateTime.MinValue) && r.ipg_EhrProcedureExpirationDate <= (procedureExpirationDate ?? DateTime.MaxValue))))
                        {
                            throw new InvalidPluginExecutionException(OverlappingExceptionMessage);
                        }
                    }
                }
            }
        }
    }
}
