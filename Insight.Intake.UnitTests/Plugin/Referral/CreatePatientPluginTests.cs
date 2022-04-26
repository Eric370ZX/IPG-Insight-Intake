using System;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using FakeXrmEasy;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class CreatePatientPluginTests : PluginTestsBase
    {
        [Fact]
        public void ReferralCreated_PatientCreated()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            var patientGuid = new Guid();          
       
            ipg_contacttype ipg_contacttypeEnt = new ipg_contacttype();
            ipg_contacttypeEnt.ipg_name = "Patient";
            ipg_contacttypeEnt.Id = new Guid("3ed7420f-e687-47ec-8c44-a675305137bd");

            Contact existingContact = new Contact().Fake();
            existingContact.FirstName = "PatientFirstName";
            existingContact.LastName = "AnotherPatientLastName";
            existingContact.BirthDate = new DateTime(2018,05,04);
            existingContact.ipg_ContactTypeId =ipg_contacttypeEnt.ToEntityReference();

            ipg_referral createdReferral = new ipg_referral().Fake().WithReferralNumber("123456");
            createdReferral.ipg_PatientFirstName = "PatientFirstName";
            createdReferral.ipg_PatientLastName = "PatientLastName";
            createdReferral.ipg_PatientDateofBirth = new DateTime(2018,05,04);
            var listForInit = new List<Entity>() { existingContact,createdReferral,ipg_contacttypeEnt };
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", createdReferral } };
          

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,              
                PostEntityImages = null,
                PreEntityImages = null                
            };
            //ACT
            var newTarget = fakedContext.ExecutePluginWith<CreatePatientPlugin>(pluginContext);
           
             var patientsT = (from t in fakedContext.CreateQuery<Contact>()
                 
                         select t).ToList();

            var patients = (from t in fakedContext.CreateQuery<Contact>()
                         where (t.ipg_ContactTypeId.Id == ipg_contacttypeEnt.Id)
                         select t).ToList();

            var patient = (from t in fakedContext.CreateQuery<Contact>()
                         where (t.ipg_ContactTypeId.Id == ipg_contacttypeEnt.Id &&
                         t.LastName == createdReferral.ipg_PatientLastName &&
                         t.FirstName == createdReferral.ipg_PatientFirstName &&
                         t.BirthDate == createdReferral.ipg_PatientDateofBirth
                         )
                         select t).ToList().FirstOrDefault();

            Assert.Equal(2, patients.Count());
            Assert.Equal(patient.FirstName, createdReferral.ipg_PatientFirstName);

            Assert.Equal(patient.LastName, createdReferral.ipg_PatientLastName);

            Assert.Equal(patient.BirthDate, createdReferral.ipg_PatientDateofBirth);
            Assert.Equal(patient.MiddleName, createdReferral.ipg_PatientMiddleName);

            Assert.Equal(patient.ipg_ContactTypeId.Id, ipg_contacttypeEnt.Id);
            #endregion
        }

        [Fact]
        public void ReferralCreated_PatientExists()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            var patientGuid = new Guid();          
       
            ipg_contacttype ipg_contacttypeEnt = new ipg_contacttype();
            ipg_contacttypeEnt.ipg_name = "Patient";
            ipg_contacttypeEnt.Id = new Guid("3ed7420f-e687-47ec-8c44-a675305137bd");

            Contact existingContact = new Contact().Fake();
            existingContact.Id = new Guid("a6c423ac-268f-4262-9030-8433e1294848");
            existingContact.FirstName = "PatientFirstName";
            existingContact.MiddleName = "Ms.";
            existingContact.LastName = "ExistingPatientLastName";
            existingContact.BirthDate = new DateTime(2018,05,04);
            existingContact.ipg_ContactTypeId = ipg_contacttypeEnt.ToEntityReference();

            ipg_referral createdReferral = new ipg_referral().Fake().WithReferralNumber("123456");
            createdReferral.ipg_PatientFirstName = "PatientFirstName";
            createdReferral.ipg_PatientMiddleName = "Ms.";
            createdReferral.ipg_PatientLastName = "ExistingPatientLastName";
            createdReferral.ipg_PatientDateofBirth = new DateTime(2018,05,04);
            var listForInit = new List<Entity>() { existingContact,createdReferral,ipg_contacttypeEnt };
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", createdReferral } };
          

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,              
                PostEntityImages = null,
                PreEntityImages = null                
            };
            //ACT
            var newTarget = fakedContext.ExecutePluginWith<CreatePatientPlugin>(pluginContext);
           

            var patients = (from t in fakedContext.CreateQuery<Contact>()
                         where (t.ipg_ContactTypeId.Id == ipg_contacttypeEnt.Id)
                         select t).ToList();

            Assert.Single(patients);

            Assert.Equal(patients.FirstOrDefault().FirstName, createdReferral.ipg_PatientFirstName);

            Assert.Equal(patients.FirstOrDefault().MiddleName, createdReferral.ipg_PatientMiddleName);

            Assert.Equal(patients.FirstOrDefault().LastName, createdReferral.ipg_PatientLastName);

            Assert.Equal(patients.FirstOrDefault().BirthDate, createdReferral.ipg_PatientDateofBirth);

            Assert.Equal(patients.FirstOrDefault().ipg_ContactTypeId.Id, ipg_contacttypeEnt.Id);
            #endregion
        }

        [Fact]
        public void CheckWhatPatientNotCreatedIfPatientIdExist()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            var patientGuid = new Guid();          
       
            ipg_contacttype ipg_contacttypeEnt = new ipg_contacttype();
            ipg_contacttypeEnt.ipg_name = "Patient";
            ipg_contacttypeEnt.Id = new Guid("3ed7420f-e687-47ec-8c44-a675305137bd");

            Contact existingContact = new Contact().Fake();
            existingContact.Id = new Guid("a6c423ac-268f-4262-9030-8433e1294848");
            existingContact.FirstName = "PatientFirstName";
            existingContact.MiddleName = "Ms.";
            existingContact.LastName = "ExistingPatientLastName";
            existingContact.BirthDate = new DateTime(2018,05,04);
            existingContact.ipg_ContactTypeId = ipg_contacttypeEnt.ToEntityReference();

            ipg_referral createdReferral = new ipg_referral().Fake().WithReferralNumber("123456");
            createdReferral.ipg_PatientId = existingContact.ToEntityReference();
            var listForInit = new List<Entity>() { existingContact,createdReferral,ipg_contacttypeEnt };
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", createdReferral } };
          

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,              
                PostEntityImages = null,
                PreEntityImages = null                
            };
            //ACT
            var newTarget = fakedContext.ExecutePluginWith<CreatePatientPlugin>(pluginContext);
           

            var patients = (from t in fakedContext.CreateQuery<Contact>()
                         where (t.ipg_ContactTypeId.Id == ipg_contacttypeEnt.Id)
                         select t).ToList();

            Assert.Single(patients);

            Assert.Equal(patients.FirstOrDefault().Id, createdReferral.ipg_PatientId.Id);
            #endregion
        }        
    }
}
