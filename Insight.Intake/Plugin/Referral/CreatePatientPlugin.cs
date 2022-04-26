using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Referral
{
    public class CreatePatientPlugin : IPlugin
    {
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
                    var referral = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_referral>();

                    if (referral.LogicalName != ipg_referral.EntityLogicalName)
                    {
                        return;
                    }

                    if (context.MessageName == "Create")
                    {
                        var patientType = GetContactTypeByName("Patient", organizationService);

                        if (referral.ipg_PatientId == null && patientType != null)
                        {
                            if (string.IsNullOrEmpty(referral.ipg_PatientFirstName) || string.IsNullOrEmpty(referral.ipg_PatientLastName) ||
                                referral.ipg_PatientDateofBirth == null)
                            {
                                throw new Exception("Required fields do not contain data.");
                            }

                            // Search for existing Patient. Create one if not found
                            var QEcontact = new QueryExpression("contact");
                            QEcontact.Criteria.AddCondition("ipg_contacttypeid", ConditionOperator.Equal, patientType.Id.ToString());
                            QEcontact.Criteria.AddCondition("birthdate", ConditionOperator.Equal, referral.ipg_PatientDateofBirth);
                            if (!string.IsNullOrEmpty(referral.ipg_PatientFirstName))
                            {
                                QEcontact.Criteria.AddCondition("firstname", ConditionOperator.Equal, referral.ipg_PatientFirstName);
                            }
                            if (!string.IsNullOrEmpty(referral.ipg_PatientLastName))
                            {
                                QEcontact.Criteria.AddCondition("lastname", ConditionOperator.Equal, referral.ipg_PatientLastName);
                            }
                            var existingPatientsCollection = organizationService.RetrieveMultiple(QEcontact);

                            Guid patientGuid = new Guid();
                            if (existingPatientsCollection.Entities.Count() > 0)
                            {
                                patientGuid = existingPatientsCollection.Entities.FirstOrDefault().Id;
                            }
                            else
                            {
                                var patient = new Intake.Contact
                                {
                                    FirstName = referral.ipg_PatientFirstName,
                                    LastName = referral.ipg_PatientLastName,
                                    MiddleName = referral.ipg_PatientMiddleName ?? "",
                                    BirthDate = referral.ipg_PatientDateofBirth,
                                    ipg_ContactTypeId = patientType.ToEntityReference()
                                };
                                patientGuid = organizationService.Create(patient);
                            }

                            var updatedReferral = new Entity(referral.LogicalName, referral.Id).ToEntity<ipg_referral>();
                            updatedReferral.ipg_PatientId = new EntityReference(Intake.Contact.EntityLogicalName, patientGuid);
                            organizationService.Update(updatedReferral);
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(CreatePatientPlugin)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace("{0}: {1}", nameof(CreatePatientPlugin), exception.ToString());
                throw;
            }
        }
        public Entity GetContactTypeByName(string typeName, IOrganizationService service)
        {
            // Instantiate QueryExpression QEipg_contacttype
            var QEipg_contacttype = new QueryExpression(ipg_contacttype.EntityLogicalName);

            // Define filter QEipg_contacttype.Criteria
            QEipg_contacttype.Criteria.AddCondition(nameof(ipg_contacttype.ipg_name).ToLower(), ConditionOperator.Equal, typeName);

            return service.RetrieveMultiple(QEipg_contacttype).Entities.FirstOrDefault();
        }
    }
}