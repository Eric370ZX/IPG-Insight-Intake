using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace Insight.Intake.Plugin.Case
{
    public class SavePatientDemographicInformation : IPlugin
    {
        #region Constants
        readonly string[] COLUMNSETCONTACT = {
                            "contactid",
                            "address1_city",
                            "address1_country",
                            "address1_postalcode",
                            "address1_line1",
                            "address1_line2",
                            "address1_line3",
                            "emailaddress1"
                        };

        readonly string[] COLUMNSETINCIDENT = {
                            "ipg_patientid",
                            "ipg_patientaddress",
                            "ipg_patientcity",
                            "ipg_patientstate",
                            "ipg_patientzip",
                            "ipg_patienthomephone",
                            "ipg_patientworkphone",
                            "ipg_patientcellphone",
                            "ipg_patientemail"
        }; 
        #endregion

        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    var caseEntity = ((Entity)context.InputParameters["Target"]).ToEntity<Incident>();

                    if (caseEntity.LogicalName != Incident.EntityLogicalName)
                    {
                        return;
                    }

                    if (context.MessageName == "Create" || context.MessageName == "Update")
                    {

                        var caseReference = organizationService.Retrieve(caseEntity.LogicalName, caseEntity.Id, new ColumnSet(COLUMNSETINCIDENT)).ToEntity<Incident>();

                        //If patient reference did not come in with referral, do not proceed with update. 
                        //Need confirmation, do I need to create contact if there is no patient referral?
                        if (caseReference.ipg_PatientId is null) return;

                        Intake.Contact relatedPatient = organizationService.Retrieve(Intake.Contact.EntityLogicalName, caseReference.ipg_PatientId.Id, new ColumnSet(COLUMNSETCONTACT)).ToEntity<Intake.Contact>();

                        relatedPatient.Address1_Line1 = caseEntity.ipg_PatientAddress ?? caseReference.ipg_PatientAddress;
                        relatedPatient.Address1_City = caseEntity.ipg_PatientCity ?? caseReference.ipg_PatientCity;
                        relatedPatient.Address1_StateOrProvince = caseEntity.ipg_PatientState ?? caseReference.ipg_PatientState;
                        //relatedPatient.Address1_PostalCode = caseEntity.ipg_PatientZip ?? caseReference.ipg_PatientZip;
                        relatedPatient.Address1_Telephone1 = caseEntity.ipg_PatientHomePhone ?? caseReference.ipg_PatientHomePhone;
                        relatedPatient.Address1_Telephone2 = caseEntity.ipg_PatientWorkPhone ?? caseReference.ipg_PatientWorkPhone;
                        relatedPatient.Address1_Telephone3 = caseEntity.ipg_PatientCellPhone ?? caseReference.ipg_PatientCellPhone;
                        relatedPatient.EMailAddress1 = caseEntity.ipg_PatientEmail ?? caseReference.ipg_PatientEmail;

                        organizationService.Update(relatedPatient);
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(SavePatientDemographicInformation)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace("{0}: {1}", nameof(SavePatientDemographicInformation), exception.ToString());
                throw;
            }
        }
    }
}
