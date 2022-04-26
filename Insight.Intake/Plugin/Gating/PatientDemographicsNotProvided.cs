using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class PatientDemographicsNotProvided : PluginBase
    {
        private readonly string[] incidentColumnSet =
        {
            nameof(Incident.ipg_PatientLastName).ToLower(),
            nameof(Incident.ipg_PatientFirstName).ToLower(),
            nameof(Incident.ipg_PatientDateofBirth).ToLower(),
            nameof(Incident.ipg_PatientAddress).ToLower(),
            nameof(Incident.ipg_PatientGender).ToLower(),
            nameof(Incident.ipg_PatientCity).ToLower(),
            nameof(Incident.ipg_PatientState).ToLower(),
            nameof(Incident.ipg_PatientZipCodeId).ToLower(),
            nameof(Incident.ipg_PatientCellPhone).ToLower(),
            nameof(Incident.ipg_PatientWorkPhone).ToLower(),
            nameof(Incident.ipg_PatientHomePhone).ToLower(),
        };
        //Incident should have at least one of the above fields but not necessary all of them
        private readonly string[] incidentOrColumnSet =
        {
            nameof(Incident.ipg_PatientCellPhone).ToLower(),
            nameof(Incident.ipg_PatientWorkPhone).ToLower(),
            nameof(Incident.ipg_PatientHomePhone).ToLower(),
        };
        private readonly string[] referralColumnSet =
        {
            ipg_referral.Fields.ipg_PatientLastName,
            ipg_referral.Fields.ipg_PatientFirstName,
            ipg_referral.Fields.ipg_PatientDateofBirth,
            ipg_referral.Fields.ipg_PatientCity,
            ipg_referral.Fields.ipg_PatientAddress,
            ipg_referral.Fields.ipg_gender,
            ipg_referral.Fields.ipg_PatientState,
            ipg_referral.Fields.ipg_PatientZipCodeId,
            ipg_referral.Fields.ipg_cellphone,
        };
        //Referrla should have at least one of the above fields but not necessary all of them
        private readonly string[] referralOrColumnSet =
        {
        };
        private ColumnSet GetFieldsColumnSet(string logicalName) => new ColumnSet(logicalName == Incident.EntityLogicalName ? incidentColumnSet : referralColumnSet);
        private string[] GetFieldsArray(string logicalName) => logicalName == Incident.EntityLogicalName ? incidentColumnSet : referralColumnSet;
        private string[] GetOrFieldsArray(string logicalName) => logicalName == Incident.EntityLogicalName ? incidentOrColumnSet : referralOrColumnSet;

        public PatientDemographicsNotProvided() : base(typeof(PatientDemographicsNotProvided))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPatientDemographicsNotProvided", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            EntityReference targetRef = context.InputParameters["Target"] as EntityReference;
            context.OutputParameters["Succeeded"] = false;

            if (targetRef != null)
            {
                Entity target = service.Retrieve(targetRef.LogicalName, targetRef.Id, GetFieldsColumnSet(targetRef.LogicalName));
                List<string> missingInfoFields = GetMissingInformationFields(service, target);

                if (missingInfoFields.Any())
                {
                    var caseNote = string.Format("Please provide the following Patient information:  {0}", string.Join(",", missingInfoFields));
                    
                    context.OutputParameters["CaseNote"] = caseNote;
                    context.OutputParameters["TaskDescripton"] = caseNote;
                }
                else
                {
                    context.OutputParameters["Succeeded"] = true;
                }
            }
        }

        private List<string> GetMissingInformationFields(IOrganizationService service, Entity target)
        {
            var entityMetaData = service.GetEntityMetadata(target.LogicalName);

            List<string> missingInfoFields = new List<string>();
            var orFields = GetOrFieldsArray(target.LogicalName);
            var columns = GetFieldsArray(target.LogicalName).Except(orFields);
            foreach (var columnName in columns)
            {
                if (!target.Contains(columnName))
                {
                    var columnMetaData = entityMetaData.Attributes.Where(x => x.LogicalName == columnName).FirstOrDefault();
                    var displayName = columnMetaData?.DisplayName.UserLocalizedLabel.Label;
                    missingInfoFields.Add(displayName);
                }
            }
            var hasOrFields = target.Attributes
                .Any(p => orFields.Contains(p.Key) && p.Value != null);
            if (!hasOrFields)
            {
                var columnsMetaDatas = entityMetaData.Attributes.Where(x => orFields.Contains(x.LogicalName));
                foreach (var columnMetaData in columnsMetaDatas)
                {
                    var displayName = columnMetaData.DisplayName.UserLocalizedLabel.Label;
                    missingInfoFields.Add(displayName);
                }

            }

            return missingInfoFields;
        }
    }
}