using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Text;

namespace Insight.Intake.Plugin.Referral
{
    public class PopulatePrimaryAttributePlugin : PluginBase
    {
        public PopulatePrimaryAttributePlugin() : base(typeof(PopulatePrimaryAttributePlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_referral.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            ipg_referral target = localPluginContext.Target<ipg_referral>();

            ipg_referral referral = null;
            if (localPluginContext.PluginExecutionContext.MessageName == MessageNames.Create)
            {
                referral = target;
            }
            else if (localPluginContext.PluginExecutionContext.MessageName == MessageNames.Update)
            {
                referral = localPluginContext.PostImage<ipg_referral>();
            }
            else
            {
                throw new InvalidPluginExecutionException("Unexpected message name: " + localPluginContext.PluginExecutionContext.MessageName);
            }

            var referralUpdate = new ipg_referral
            {
                Id = target.Id,
                ipg_name = BuildReferralName(service, referral),
            };

            if (localPluginContext.PluginExecutionContext.MessageName == MessageNames.Create)
            {
                //ipg_referralcasenumber is used in IPG.Intake.Actions.GenerateCaseRecord to set incident.title
               // referralUpdate.ipg_referralcasenumber = referral.ipg_referralnumber;
            }

            service.Update(referralUpdate);
        }

        private string BuildReferralName(IOrganizationService organizationService, ipg_referral referral)
        {
            var patientNameStringBuilder = new StringBuilder();
            patientNameStringBuilder.Append(referral.ipg_PatientFirstName);
            if (string.IsNullOrWhiteSpace(referral.ipg_PatientMiddleName) == false)
            {
                patientNameStringBuilder.Append(" ").Append(referral.ipg_PatientMiddleName);
            }
            patientNameStringBuilder.Append(" ").Append(referral.ipg_PatientLastName);


            string procedureName = "";
            if (referral.ipg_CPTCodeId1 != null)
            {
                var cptCode = organizationService.Retrieve(ipg_cptcode.EntityLogicalName, referral.ipg_CPTCodeId1.Id, new ColumnSet(nameof(ipg_cptcode.ipg_procedurename).ToLower())).ToEntity<ipg_cptcode>();
                if (cptCode.ipg_procedurename != null)
                {
                    procedureName = cptCode.ipg_procedurename.Name;
                }
            }
            else if (referral.ipg_ProcedureNameId != null)
            {
                var procedureNameRef = organizationService.Retrieve(ipg_procedurename.EntityLogicalName, referral.ipg_ProcedureNameId.Id, new ColumnSet(ipg_procedurename.Fields.ipg_name)).ToEntity<ipg_procedurename>();
                procedureName = procedureNameRef.ipg_name;
            }

            return $"Referral {referral.ipg_referralcasenumber} - {patientNameStringBuilder.ToString()} - {procedureName}";
        }
    }
}
