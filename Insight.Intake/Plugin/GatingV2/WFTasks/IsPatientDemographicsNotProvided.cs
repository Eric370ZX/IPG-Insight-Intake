using Insight.Intake.Plugin.GatingV2.CommonWfTask;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Extensions;

namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class IsPatientDemographicsNotProvided : WFTaskBase
    {
        private readonly string[] incidentColumnSet =
        {
            nameof(Incident.ipg_PatientLastName).ToLower(),
            nameof(Incident.ipg_PatientFirstName).ToLower(),
            nameof(Incident.ipg_PatientDateofBirth).ToLower(),
            nameof(Incident.ipg_PatientAddress).ToLower(),
            nameof(Incident.ipg_PatientCity).ToLower(),
            nameof(Incident.ipg_PatientState).ToLower(),
            nameof(Incident.ipg_PatientZipCodeId).ToLower(),
            nameof(Incident.ipg_PatientCellPhone).ToLower()
        };

        public override WFTaskResult Run(WFTaskContext ctx)
        {
            var tracingService = ctx.TraceService;
            var gatingResponse = new WFTaskResult(false);
            var incident = ctx.dbContext.Case;
            List<string> missingInfoFields = GetMissingInformationFields(incident);

            if (missingInfoFields.Any())
            {
                var caseNote = string.Format("Patient {0} is missing. Missing information disclaimer sent to facility. ", string.Join(",", missingInfoFields));
                gatingResponse.CaseNote = caseNote;
            }
            else
            {
                gatingResponse.Succeeded = true;
            }
            return gatingResponse;
        }


        private List<string> GetMissingInformationFields(Incident incident)
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