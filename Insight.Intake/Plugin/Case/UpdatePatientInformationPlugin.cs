using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Person = Insight.Intake.Contact;

namespace Insight.Intake.Plugin.Case
{
    public class UpdatePatientInformationPlugin : PluginBase
    {
        private static readonly Dictionary<string, string> PATIENTCASEMAPPING = new Dictionary<string, string>()
        {
            //Demographics
            {"firstname","ipg_patientfirstname"},
            {"middlename","ipg_patientmiddlename"},
            {"lastname","ipg_patientlastname"},
            {"birthdate","ipg_patientdateofbirth"},
            {"gendercode","ipg_patientgender"},

            {"mobilephone", "ipg_patientcellphone"},
            {"address1_telephone1", "ipg_patienthomephone"},
            {"emailaddress1", "ipg_patientemail"},

            {"ipg_notes", "ipg_patientnotes"},
            {"ipg_workphone", "ipg_patientworkphone"},

            {"address1_city", "ipg_patientcity"},
            {"address1_stateorprovince", "ipg_patientstate"},
            {"address1_line1", "ipg_patientaddress" },
            {"address1_postalcode", "ipg_patientzip"},
            {"ipg_zipcodeid", "ipg_patientzipcodeid"},

            //Insurance
            {"ipg_primarycarrierid", "ipg_carrierid"},
            {"ipg_memberid", "ipg_memberidnumber"},
            {"ipg_primarymemberid", "ipg_memberidnumber"},
            {"ipg_primarygroupid", "ipg_primarycarriergroupidnumber"},
            {"ipg_relationtoprimaryinsured", "ipg_relationtoinsured"},
            {"ipg_primaryeffectivedate", "ipg_primarycarriereffectivedate"},
            {"ipg_primaryexpirationdate", "ipg_primarycarrierexpirationdate"},
            {"ipg_primarycarrierstatus", "ipg_primarycarrierstatus"},
            {"ipg_secondarycarrierid", "ipg_secondarycarrierid"},
            {"ipg_secondarymemberid", "ipg_secondarymemberidnumber"},
            {"ipg_secondarygroupid", "ipg_secondarycarriergroupidnumber"},
            {"ipg_secondaryeffectivedate", "ipg_secondarycarriereffectivedate"},
            {"ipg_secondaryexpirationdate", "ipg_secondarycarrierexpirationdate"},
            {"ipg_relationtosecondaryinsured", "ipg_secondarycarrierrelationtoinsured"},
            {"ipg_secondarycarrierstatus", "ipg_secondarycarrierstatus"},
            {"ipg_autocarrierid", "ipg_autocarrierid"},
            {"ipg_homeplanid", "ipg_homeplancarrierid"},
            {"ipg_adjustername", "ipg_autoadjustername"},
            {"ipg_dateofaccident", "ipg_autodateofincident"},
            {"ipg_claimnumber", "ipg_autoclaimnumber"},
            {"ipg_medicalbenefitsexhausted", "ipg_medicalbenefitsexhausted"},
            {"ipg_facilityexhaustletter", "ipg_facilityexhaustletteronfile"},
            {"ipg_ipgexhaustletter", "ipg_exhaustletterreceived"},
            {"ipg_plansponsor", "ipg_plansponsor"},
            {"ipg_plantype", "ipg_primarycarrierplantype"},
            {"ipg_network", "ipg_carrierid//ipg_network"}, // '//' means that we want to take information from entity reference, so it will retrieve ipg_network from carrier

            
            {"ipg_coinsurance", "ipg_patientcoinsurance"},
            //{"ipg_coinsurancedme", "ipg_patientcoinsurancedme"}, -ipg_coinsurancedme is obsolete and shouldn't be used

            //Balance
            {"ipg_remainingbalance","ipg_remainingpatientbalance" }


    };

        private static readonly List<string> CASEFIELDS = PATIENTCASEMAPPING.Values.Select(v => v.Contains("//") ? v.Split('/').First() : v).ToList();

        public UpdatePatientInformationPlugin() : base(typeof(UpdatePatientInformationPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PostOperationHandler);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_document.EntityLogicalName, OnRelatedRecordCreatedUpdated);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_document.EntityLogicalName, OnRelatedRecordCreatedUpdated);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_payment.EntityLogicalName, OnRelatedRecordCreatedUpdated);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_payment.EntityLogicalName, OnRelatedRecordCreatedUpdated);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Invoice.EntityLogicalName, OnRelatedRecordCreatedUpdated);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Invoice.EntityLogicalName, OnRelatedRecordCreatedUpdated);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_benefit.EntityLogicalName, OnRelatedRecordCreatedUpdated);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_benefit.EntityLogicalName, OnRelatedRecordCreatedUpdated);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_statementgenerationtask.EntityLogicalName, OnRelatedRecordCreatedUpdated);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_statementgenerationtask.EntityLogicalName, OnRelatedRecordCreatedUpdated);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, SalesOrder.EntityLogicalName, OnRelatedRecordCreatedUpdated);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, SalesOrder.EntityLogicalName, OnRelatedRecordCreatedUpdated);

            CASEFIELDS.Add("ipg_patientid");
            CASEFIELDS.Add("ipg_surgerydate");
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var target = localPluginContext.Target<Incident>();
            target = service.Retrieve(target.LogicalName, target.Id
                , new ColumnSet(CASEFIELDS.ToArray())).ToEntity<Incident>();

            var crmContext = new OrganizationServiceContext(service);

            if (IsMostRecentCase(target, crmContext))
            {
                var patient = service.Retrieve(target.ipg_PatientId.LogicalName, target.ipg_PatientId.Id, new ColumnSet(PATIENTCASEMAPPING.Keys.ToArray())).ToEntity<Person>();

                Person PatientForUpdate;

                if (ShouldUpdatePatient(target, patient, out PatientForUpdate, service))
                {
                    service.Update(PatientForUpdate);
                }

                LinkPatientToCaseData(patient.ToEntityReference(), target, service, crmContext);
            }       
        }

        private void LinkPatientToCaseData(EntityReference patientRef, Incident target, IOrganizationService service, OrganizationServiceContext crmContext)
        {
            List<Entity> updates = new List<Entity>();

            var POs = (from po in crmContext.CreateQuery<SalesOrder>()
                       where po.ipg_CaseId.Id == target.Id
                       && po.StateCode == SalesOrderState.Active
                       && po.ipg_patientid.Id != patientRef.Id
                       select new SalesOrder() { Id = (Guid)po.SalesOrderId }).ToList();

            updates.AddRange(POs);

            var benefits = (from benefit in crmContext.CreateQuery<ipg_benefit>()
                            where benefit.ipg_CaseId.Id == target.Id
                            && benefit.StateCode == ipg_benefitState.Active
                            && benefit.ipg_patientid.Id != patientRef.Id
                            select new ipg_benefit() { Id = (Guid)benefit.ipg_benefitId }).ToList();

            updates.AddRange(benefits);

            var claims = (from claim in crmContext.CreateQuery<Invoice>()
                          where claim.ipg_caseid.Id == target.Id
                          && claim.StateCode == InvoiceState.Active
                          && claim.ipg_patientid.Id != patientRef.Id
                          select new Invoice() { Id = (Guid)claim.InvoiceId }).ToList();

            updates.AddRange(claims);

            var statements = (from statement in crmContext.CreateQuery<ipg_statementgenerationtask>()
                              where statement.ipg_caseid.Id == target.Id
                              && statement.StateCode == ipg_statementgenerationtaskState.Active
                              && statement.ipg_patientid.Id != patientRef.Id
                              select new ipg_statementgenerationtask() { Id = (Guid)statement.ipg_statementgenerationtaskId }).ToList();

            updates.AddRange(statements);

            var payments = (from payment in crmContext.CreateQuery<ipg_payment>()
                            where payment.ipg_CaseId.Id == target.Id
                            && payment.StateCode == ipg_paymentState.Active
                            && payment.ipg_patientid.Id != patientRef.Id
                            select new ipg_payment() { Id = (Guid)payment.ipg_paymentId }).ToList();

            updates.AddRange(payments);

            var documents = (from document in crmContext.CreateQuery<ipg_document>()
                             where document.ipg_CaseId.Id == target.Id
                             && document.StateCode == ipg_documentState.Active
                             && document.ipg_patientid.Id != patientRef.Id
                             select new ipg_document() { Id = (Guid)document.ipg_documentId }).ToList();

            updates.AddRange(documents);

            foreach (var update in updates)
            {
                service.Update(new Entity(update.LogicalName)
                {
                    Id = update.Id,
                    Attributes = { { "ipg_patientid", patientRef } }
                });
            }
        }

        private bool ShouldUpdatePatient(Incident sourceCase, Person patient, out Intake.Contact patientForUpdate, IOrganizationService crmService)
        {
            patientForUpdate = new Intake.Contact() { Id = patient.Id };
            foreach (var patientField in PATIENTCASEMAPPING.Keys)
            {
                var caseField = PATIENTCASEMAPPING[patientField];
                object sourceValue = sourceCase.GetAttributeValue<object>(caseField);
                object patientValue = patient.GetAttributeValue<object>(patientField);

                var index = caseField.IndexOf("//");
                if (index > -1)
                {
                    var field = caseField.Substring(index + 2);
                    var lookUpName = caseField.Substring(0, index);

                    var entRef = sourceCase.GetAttributeValue<EntityReference>(lookUpName);

                    sourceValue = entRef == null ? null :
                        crmService.Retrieve(entRef.LogicalName, entRef.Id, new ColumnSet(field)).GetAttributeValue<object>(field);
                }

                if (patientField == nameof(Person.GenderCode).ToLower())
                {
                    if (sourceCase.ipg_PatientGenderEnum?.ToString() != patient.GenderCodeEnum?.ToString() && sourceCase.ipg_PatientGenderEnum != ipg_Gender.Other)
                    {
                        patientForUpdate.GenderCodeEnum = sourceCase.ipg_PatientGenderEnum == ipg_Gender.Female 
                            ? Contact_GenderCode.Female 
                            : sourceCase.ipg_PatientGenderEnum == ipg_Gender.Male 
                                ? Contact_GenderCode.Male 
                                : (Contact_GenderCode?)null;
                    }
                }
                else if (!object.Equals(patientValue, sourceValue))
                {
                    patientForUpdate[patientField] = sourceValue;
                }
            }

            return patientForUpdate.Attributes.Count > 1;//Attributes contains entity id we don't need count it 
        }

        private bool IsMostRecentCase(Incident target, OrganizationServiceContext crmContext)
        {
            if (target.ipg_PatientId != null && target.ipg_SurgeryDate.HasValue)
            {
                var mostRecentCase = (from incident in crmContext.CreateQuery<Incident>()
                                      where incident.ipg_PatientId.Id == target.ipg_PatientId.Id
                                      orderby incident.ipg_SurgeryDate descending
                                      select new Incident() { Id = (Guid)incident.IncidentId }
                                      ).FirstOrDefault();

                if (mostRecentCase.Id == target.Id)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnRelatedRecordCreatedUpdated(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var context = localPluginContext.PluginExecutionContext;

            var target = context.MessageName == MessageNames.Create ? localPluginContext.Target<Entity>() : localPluginContext.PostImage<Entity>();
            var crmContext = new OrganizationServiceContext(service);
            var caseRef = target.GetAttributeValue<EntityReference>("ipg_caseid");

            if (caseRef != null)
            {
                var caseEnt = service.Retrieve(caseRef.LogicalName, caseRef.Id, new ColumnSet(
                    nameof(Incident.ipg_PatientId).ToLower(),
                    nameof(Incident.ipg_SurgeryDate).ToLower()
                    )).ToEntity<Incident>();

                if (IsMostRecentCase(caseEnt, crmContext))
                {
                    var relatedEnt = new Entity(target.LogicalName) 
                    { 
                        Id = target.Id,                   
                        Attributes = { { "ipg_patientid", caseEnt.ipg_PatientId} }
                    };

                    service.Update(relatedEnt);
                }
            }
        }
    }
}
