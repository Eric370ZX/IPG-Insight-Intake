using Insight.Intake.Plugin.GatingV2.CommonWfTask;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class IsPatientInsuranceNotProvided : WFTaskBase
    {
        private readonly string[] incidentAutoCarrierColumnSet =
        {
              nameof(Incident.ipg_MemberIdNumber).ToLower(),
              nameof(Incident.ipg_CarrierId).ToLower(),
              nameof(Incident.ipg_AutoClaimNumber).ToLower(),
              nameof(Incident.ipg_AutoAdjusterName).ToLower(),
              nameof(Incident.ipg_AutoDateofIncident).ToLower()
        };

        private readonly string[] incidentNotAutoCarrierColumnSet =
        {
              nameof(Incident.ipg_MemberIdNumber).ToLower(),
              nameof(Incident.ipg_CarrierId).ToLower()
        };

        public override WFTaskResult Run(WFTaskContext ctx)
        {
            var tracingService = ctx.TraceService;
            var gatingResponse = new WFTaskResult(false);
            var incident = ctx.dbContext.Case;
            if (incident.ipg_AutoCarrier == true)
            {
                var missingInfoFields = GetMissingInformationFields(incident, incidentAutoCarrierColumnSet);
                if (incident.ipg_MemberIdNumber != null && incident.ipg_CarrierId != null && incident.ipg_AutoClaimNumber != null && incident.ipg_AutoAdjusterName != null && incident.ipg_AutoDateofIncident != null)
                {
                    gatingResponse.Succeeded = true;
                }
                else
                {
                    var caseNote = string.Format("Patient {0} is missing. Missing information disclaimer sent to facility. ", string.Join(",", missingInfoFields));
                    gatingResponse.CaseNote = caseNote;
                }
            }
            else
            {
                var missingInfoFields = GetMissingInformationFields(incident, incidentNotAutoCarrierColumnSet);
                if (incident.ipg_MemberIdNumber != null && incident.ipg_CarrierId != null)
                {
                    gatingResponse.Succeeded = true;
                }
                else
                {
                    var caseNote = string.Format("Patient {0} is missing. Missing information disclaimer sent to facility. ", string.Join(",", missingInfoFields));
                    gatingResponse.CaseNote = caseNote;
                }
            }
            return gatingResponse;
        }

        private List<string> GetMissingInformationFields(Incident incident, string[] incidentColumnSet)
        {
            var incidentMetaData = incident.Metadata;
            Type type = incident.GetType();

            List<string> missingInfoFields = new List<string>();
            foreach (var columnName in incidentColumnSet)
            {
                if (!incident.Contains(columnName))
                {
                    var columnMetaData = incidentMetaData.Attributes.Where(x => x.LogicalName == columnName).FirstOrDefault();
                    var displayName = columnMetaData.DisplayName.UserLocalizedLabel.Label;
                    missingInfoFields.Add(displayName);
                }
            }

            return missingInfoFields;
        }
    }
}