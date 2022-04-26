using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Contact
{
    /// <summary>
    /// This plugin is not needed anymore because we send to Zirmed PDF files (not XML).
    /// 
    /// This plugin was needed to generate Patient ID (contact.ipg_authpatientid).
    /// Auto Number D365 solution needs an option set to generate numbers only for patient contacts.
    /// But we use Contact Type custom entity instead of OOB contact.customertypecode. That's why this plugin
    /// updates contact.customertypecode (only for patients currently).
    /// </summary>
    public class SyncContactCustomerTypeCodeWithContactType : PluginBase
    {
        public static readonly string TargetInputParameterName = "Target";
        private static readonly string PatientContactTypeName = "Patient"; //maybe move to Global Settings


        public SyncContactCustomerTypeCodeWithContactType() : base(typeof(SyncContactCustomerTypeCodeWithContactType))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Intake.Contact.EntityLogicalName, PreOperationCreateHandler);
        }


        private void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var organizationService = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            tracingService.Trace($"{this.GetType()} plugin started");

            var contact = ((Entity)context.InputParameters[TargetInputParameterName]).ToEntity<Intake.Contact>();
            if (contact.LogicalName != Intake.Contact.EntityLogicalName)
            {
                throw new Exception($"{TargetInputParameterName} input parameter must be a contact entity");
            }

            tracingService.Trace("Checking if Contact Type is changed");
            if (contact.Attributes.Contains(nameof(Intake.Contact.ipg_ContactTypeId).ToLower()) == false)
            {
                return;
            }

            tracingService.Trace("Getting new Contact Type reference");
            EntityReference newContactTypeReference = (EntityReference)contact.Attributes[nameof(Intake.Contact.ipg_ContactTypeId).ToLower()];

            if (newContactTypeReference == null)
            {
                tracingService.Trace("Setting customertypecode");
                contact.CustomerTypeCode = new OptionSetValue((int)Contact_CustomerTypeCode.DefaultValue);
            }
            else
            {
                tracingService.Trace("Getting new Contact Type");
                var newContactType = (ipg_contacttype)organizationService.Retrieve(ipg_contacttype.EntityLogicalName, newContactTypeReference.Id, new ColumnSet(nameof(ipg_contacttype.ipg_name).ToLower()));

                tracingService.Trace("Setting customertypecode");
                if (string.Equals(newContactType.ipg_name, PatientContactTypeName, StringComparison.OrdinalIgnoreCase))
                {
                    contact.CustomerTypeCode = new OptionSetValue((int)Contact_CustomerTypeCode.Patient);
                }
                else
                {
                    contact.CustomerTypeCode = new OptionSetValue((int)Contact_CustomerTypeCode.DefaultValue);
                }
            }

            tracingService.Trace($"Setting {TargetInputParameterName} input parameter");
            context.InputParameters[TargetInputParameterName] = contact.ToEntity<Entity>();

            tracingService.Trace($"{this.GetType()} plugin finished");
        }
    }
}