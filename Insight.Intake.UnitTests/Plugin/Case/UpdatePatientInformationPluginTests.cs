using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class UpdatePatientInformationPluginTests: PluginTestsBase
    {
        [Fact]
        public void MostRecentCaseUpdateTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            //Create a fake patient contact and incident. Bind the contact with the incident.

            Contact patient = new Contact().Fake();
            Intake.ipg_carriernetwork netWork = new ipg_carriernetwork().Fake();
            Intake.Account primaryCarrier = new Intake.Account().Fake().WithNetworkReference(netWork);
            Incident olderIncident = new Incident().Fake().WithPatientReference(patient);
            Incident updatedIncident = new Incident().Fake().WithPatientReference(patient).WithCarrierReference(primaryCarrier);

            SalesOrder po = new SalesOrder().Fake().WithCaseReference(updatedIncident);
            Invoice claim = new Invoice().Fake().WithCaseReference(updatedIncident);
            ipg_payment payment = new ipg_payment().Fake().WithCaseReference(updatedIncident);
            ipg_benefit benefit = new ipg_benefit().Fake().WithCaseReference(updatedIncident);
            ipg_document document = new ipg_document().Fake().WithCaseReference(updatedIncident);
            ipg_statementgenerationtask statement = new ipg_statementgenerationtask().Fake().WithCaseReference(updatedIncident);


            //Change demographics fields on case.
            updatedIncident.ipg_MemberIdNumber = "1234primem";
            updatedIncident.ipg_primarycarriergroupidnumber = "1234prigroup";
            updatedIncident.ipg_SecondaryMemberIdNumber = "1234secmem";
            updatedIncident.ipg_SecondaryCarrierGroupIdNumber = "1234prigroup";
            updatedIncident.ipg_AutoAdjusterName = "testAutoAdjuster";
            updatedIncident.ipg_PlanSponsor = "test plan sponsor";
            updatedIncident.ipg_SurgeryDate = DateTime.Today;

            olderIncident.ipg_SurgeryDate = DateTime.Today.AddDays(-1);


            var listForInit = new List<Entity> { patient, updatedIncident, primaryCarrier, po, claim, payment, benefit, document, statement};

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", updatedIncident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { {"PostImage", updatedIncident } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<UpdatePatientInformationPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            patient = fakedService.Retrieve(patient.LogicalName, patient.Id, new ColumnSet(true)).ToEntity<Contact>();
            var incident = fakedService.Retrieve(updatedIncident.LogicalName, updatedIncident.Id, new ColumnSet(true)).ToEntity<Incident>();

            //Validate if fields have updated.
            Assert.Equal(patient.ipg_PrimaryCarrierId, incident.ipg_CarrierId);
            Assert.Equal(patient.ipg_MemberId, incident.ipg_MemberIdNumber);
            Assert.Equal(patient.ipg_PrimaryMemberId, incident.ipg_MemberIdNumber);
            Assert.Equal(patient.ipg_PrimaryGroupId, incident.ipg_primarycarriergroupidnumber);
            Assert.Equal(patient.ipg_RelationtoPrimaryInsured, incident.ipg_RelationToInsured);
            Assert.Equal(patient.ipg_PrimaryEffectiveDate, incident.ipg_primarycarriereffectivedate);
            Assert.Equal(patient.ipg_PrimaryExpirationDate, incident.ipg_primarycarrierexpirationdate);
            Assert.Equal(patient.ipg_PrimaryCarrierStatus, incident.ipg_primarycarrierstatus);

            Assert.Equal(patient.ipg_SecondaryCarrierId, incident.ipg_SecondaryCarrierId);
            Assert.Equal(patient.ipg_SecondaryMemberId, incident.ipg_SecondaryMemberIdNumber);
            Assert.Equal(patient.ipg_SecondaryGroupId, incident.ipg_SecondaryCarrierGroupIdNumber);
            Assert.Equal(patient.ipg_SecondaryEffectiveDate, incident.ipg_SecondaryCarrierEffectiveDate);
            Assert.Equal(patient.ipg_SecondaryExpirationDate, incident.ipg_SecondaryCarrierExpirationDate);
            Assert.Equal(patient.ipg_RelationtoSecondaryInsured, incident.ipg_SecondaryCarrierRelationToInsured);
            Assert.Equal(patient.ipg_SecondaryCarrierStatus, incident.ipg_secondarycarrierstatus);
            
            Assert.Equal(patient.ipg_AutoCarrierId, incident.ipg_AutoCarrierId);
            Assert.Equal(patient.ipg_HomePlanId, incident.ipg_HomePlanCarrierId);
            Assert.Equal(patient.ipg_AdjusterName, incident.ipg_AutoAdjusterName);
            Assert.Equal(patient.ipg_DateofAccident, incident.ipg_AutoDateofIncident);
            Assert.Equal(patient.ipg_ClaimNumber, incident.ipg_AutoClaimNumber);
            Assert.Equal(patient.ipg_MedicalBenefitsExhausted, incident.ipg_medicalbenefitsexhausted);
            Assert.Equal(patient.ipg_FacilityExhaustLetter, incident.ipg_facilityexhaustletteronfile);
            Assert.Equal(patient.ipg_IPGExhaustLetter, incident.ipg_ExhaustLetterReceived);
            Assert.Equal(patient.ipg_PlanSponsor, incident.ipg_PlanSponsor);
            Assert.Equal(patient.ipg_PlanType, incident.ipg_primarycarrierplantype);
            Assert.Equal(patient.ipg_Network, primaryCarrier.ipg_Network);

            Assert.Equal(patient.ipg_coinsurance, incident.ipg_patientcoinsurance);
            // Assert.Equal(patient.ipg_coinsurancedme, incident.ipg_patientcoinsurancedme); -ipg_coinsurancedme is obsolete and shouldn't be used

            Assert.Equal(patient.ipg_remainingbalance, incident.ipg_RemainingPatientBalance);

            Assert.Equal(patient.FirstName, incident.ipg_PatientFirstName);
            Assert.Equal(patient.LastName, incident.ipg_PatientLastName);
            Assert.Equal(patient.MiddleName, incident.ipg_PatientMiddleName);
            Assert.Equal(patient.BirthDate, incident.ipg_PatientDateofBirth);
            Assert.Equal(patient.GenderCode, incident.ipg_PatientGender);
            Assert.Equal(patient.MobilePhone, incident.ipg_PatientCellPhone);
            Assert.Equal(patient.Address1_Telephone1, incident.ipg_PatientHomePhone);
            Assert.Equal(patient.EMailAddress1, incident.ipg_PatientEmail);
            Assert.Equal(patient.ipg_workphone, incident.ipg_PatientWorkPhone);
            Assert.Equal(patient.ipg_notes, incident.ipg_PatientNotes);
            Assert.Equal(patient.Address1_City, incident.ipg_PatientCity);
            Assert.Equal(patient.Address1_StateOrProvince, incident.ipg_PatientAddress);
            Assert.Equal(patient.Address1_PostalCode, incident.ipg_PatientZip);
            Assert.Equal(patient.ipg_zipcodeid, incident.ipg_PatientZipCodeId);


            CheckRelatedRecords(patient, incident, crmContext);

            #endregion
        }

        private void CheckRelatedRecords(Contact patient, Incident incident, OrganizationServiceContext crmContext)
        {
           //documents
            Assert.All((from ent in crmContext.CreateQuery<ipg_document>()
                        where ent.ipg_CaseId.Id == incident.Id
                        select ent), 
                        item => (from ent in crmContext.CreateQuery<ipg_document>()
                        where ent.ipg_patientid.Id == patient.Id
                        select ent).ToList().Contains(item));

            //PO
            Assert.All((from ent in crmContext.CreateQuery<SalesOrder>()
                        where ent.ipg_CaseId.Id == incident.Id
                        select ent),
                        item => (from ent in crmContext.CreateQuery<SalesOrder>()
                                 where ent.ipg_patientid.Id == patient.Id
                                 select ent).ToList().Contains(item));

            //Claims
            Assert.All((from ent in crmContext.CreateQuery<Invoice>()
                        where ent.ipg_caseid.Id == incident.Id
                        select ent),
                        item => (from ent in crmContext.CreateQuery<Invoice>()
                                 where ent.ipg_patientid.Id == patient.Id
                                 select ent).ToList().Contains(item));

            //Payments
            Assert.All((from ent in crmContext.CreateQuery<ipg_payment>()
                        where ent.ipg_CaseId.Id == incident.Id
                        select ent),
                        item => (from ent in crmContext.CreateQuery<ipg_payment>()
                                 where ent.ipg_patientid.Id == patient.Id
                                 select ent).ToList().Contains(item));

            //Benefits
            Assert.All((from ent in crmContext.CreateQuery<ipg_benefit>()
                        where ent.ipg_CaseId.Id == incident.Id
                        select ent),
                        item => (from ent in crmContext.CreateQuery<ipg_benefit>()
                                 where ent.ipg_patientid.Id == patient.Id
                                 select ent).ToList().Contains(item));

            //Statements
            Assert.All((from ent in crmContext.CreateQuery<ipg_statementgenerationtask>()
                        where ent.ipg_caseid.Id == incident.Id
                        select ent),
                        item => (from ent in crmContext.CreateQuery<ipg_statementgenerationtask>()
                                 where ent.ipg_patientid.Id == patient.Id
                                 select ent).ToList().Contains(item));
        }

        [Fact]
        public void DocumentCreationForMostRecentCaseTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            //Create a fake patient contact and incident. Bind the contact with the incident.

            Contact patient = new Contact().Fake();
            Incident mainIncident = new Incident().Fake().WithPatientReference(patient);
            Incident updatedIncident = new Incident().Fake().WithPatientReference(patient);

            ipg_document document = new ipg_document().Fake().WithCaseReference(mainIncident);
           
            updatedIncident.ipg_SurgeryDate = DateTime.Today.AddDays(-1);

            mainIncident.ipg_SurgeryDate = DateTime.Today;


            var listForInit = new List<Entity> { patient, updatedIncident, mainIncident, document};

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", document } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<UpdatePatientInformationPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var relatedEnt = fakedService.Retrieve(document.LogicalName, document.Id, new ColumnSet(true)).ToEntity<ipg_document>();

            Assert.Equal(relatedEnt.ipg_patientid, patient.ToEntityReference());

            #endregion
        }

        [Fact]
        public void PaymentUpdateForMostRecentCaseTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            //Create a fake patient contact and incident. Bind the contact with the incident.

            Contact patient = new Contact().Fake();
            Incident mainIncident = new Incident().Fake().WithPatientReference(patient);
            Incident updatedIncident = new Incident().Fake().WithPatientReference(patient);

            ipg_payment relatedRecord = new ipg_payment().Fake().WithCaseReference(mainIncident);

            updatedIncident.ipg_SurgeryDate = DateTime.Today.AddDays(-1);

            mainIncident.ipg_SurgeryDate = DateTime.Today;


            var listForInit = new List<Entity> { patient, updatedIncident, mainIncident, relatedRecord };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", relatedRecord } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { {"PostImage", relatedRecord } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<UpdatePatientInformationPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var relatedEnt = fakedService.Retrieve(relatedRecord.LogicalName, relatedRecord.Id, new ColumnSet(true)).ToEntity<ipg_payment>();

            Assert.Equal(relatedEnt.ipg_patientid, patient.ToEntityReference());

            #endregion
        }


        [Fact]
        public void StatementCreateForMostRecentCaseTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            //Create a fake patient contact and incident. Bind the contact with the incident.

            Contact patient = new Contact().Fake();
            Incident mainIncident = new Incident().Fake().WithPatientReference(patient);
            Incident updatedIncident = new Incident().Fake().WithPatientReference(patient);

            ipg_statementgenerationtask relatedRecord = new ipg_statementgenerationtask().Fake().WithCaseReference(mainIncident);

            updatedIncident.ipg_SurgeryDate = DateTime.Today.AddDays(-1);

            mainIncident.ipg_SurgeryDate = DateTime.Today;


            var listForInit = new List<Entity> { patient, updatedIncident, mainIncident, relatedRecord };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", relatedRecord } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", relatedRecord } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<UpdatePatientInformationPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var relatedEnt = fakedService.Retrieve(relatedRecord.LogicalName, relatedRecord.Id, new ColumnSet(true)).ToEntity<ipg_statementgenerationtask>();

            Assert.Equal(relatedEnt.ipg_patientid, patient.ToEntityReference());

            #endregion
        }

        [Fact]
        public void ClaimCreateForMostRecentCaseTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            //Create a fake patient contact and incident. Bind the contact with the incident.

            Contact patient = new Contact().Fake();
            Incident mainIncident = new Incident().Fake().WithPatientReference(patient);
            Incident updatedIncident = new Incident().Fake().WithPatientReference(patient);

            Invoice relatedRecord = new Invoice().Fake().WithCaseReference(mainIncident);

            updatedIncident.ipg_SurgeryDate = DateTime.Today.AddDays(-1);

            mainIncident.ipg_SurgeryDate = DateTime.Today;


            var listForInit = new List<Entity> { patient, updatedIncident, mainIncident, relatedRecord };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", relatedRecord } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", relatedRecord } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<UpdatePatientInformationPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var relatedEnt = fakedService.Retrieve(relatedRecord.LogicalName, relatedRecord.Id, new ColumnSet(true)).ToEntity<Invoice>();

            Assert.Equal(relatedEnt.ipg_patientid, patient.ToEntityReference());

            #endregion
        }

        [Fact]
        public void BenefitUpdateForMostRecentCaseTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            //Create a fake patient contact and incident. Bind the contact with the incident.

            Contact patient = new Contact().Fake();
            Incident mainIncident = new Incident().Fake().WithPatientReference(patient);
            Incident updatedIncident = new Incident().Fake().WithPatientReference(patient);

            ipg_benefit relatedRecord = new ipg_benefit().Fake().WithCaseReference(mainIncident);

            updatedIncident.ipg_SurgeryDate = DateTime.Today.AddDays(-1);

            mainIncident.ipg_SurgeryDate = DateTime.Today;


            var listForInit = new List<Entity> { patient, updatedIncident, mainIncident, relatedRecord };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", relatedRecord } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", relatedRecord } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<UpdatePatientInformationPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var relatedEnt = fakedService.Retrieve(relatedRecord.LogicalName, relatedRecord.Id, new ColumnSet(true)).ToEntity<ipg_benefit>();

            Assert.Equal(relatedEnt.ipg_patientid, patient.ToEntityReference());

            #endregion
        }

        [Fact]
        public void POCreateForMostRecentCaseTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            //Create a fake patient contact and incident. Bind the contact with the incident.

            Contact patient = new Contact().Fake();
            Incident mainIncident = new Incident().Fake().WithPatientReference(patient);
            Incident updatedIncident = new Incident().Fake().WithPatientReference(patient);

            SalesOrder relatedRecord = new SalesOrder().Fake().WithCaseReference(mainIncident);

            updatedIncident.ipg_SurgeryDate = DateTime.Today.AddDays(-1);

            mainIncident.ipg_SurgeryDate = DateTime.Today;


            var listForInit = new List<Entity> { patient, updatedIncident, mainIncident, relatedRecord };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", relatedRecord } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", relatedRecord } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<UpdatePatientInformationPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var relatedEnt = fakedService.Retrieve(relatedRecord.LogicalName, relatedRecord.Id, new ColumnSet(true)).ToEntity<SalesOrder>();

            Assert.Equal(relatedEnt.ipg_patientid, patient.ToEntityReference());

            #endregion
        }
    }
}
