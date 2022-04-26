using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating.Common
{
    public class GateManagerTests : PluginTestsBase
    {
        [Fact]
        public void ObjectToString_OptionSet()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            Assert.Equal("Open", gateManager.ObjectToString(ipg_CaseStatus.Open));
        }

        [Fact]
        public void ObjectToString_EntityReference()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            Assert.Equal(incident.Id.ToString(), gateManager.ObjectToString(incident.ToEntityReference()));
        }

        [Fact]
        public void ObjectToString_bool()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            Assert.Equal("1", gateManager.ObjectToString(true));
        }

        [Fact]
        public void ObjectToString_DateTime()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            var date = System.DateTime.Now;
            Assert.Equal(date.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")), gateManager.ObjectToString(date));
        }

        [Fact]
        public void ObjectToString_Money()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            var money = new Money(100);
            Assert.Equal(money.Value.ToString(), gateManager.ObjectToString(money));
        }


        [Fact]
        public void ObjectPresentation_OptionSet()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            Assert.Equal("Open", gateManager.ObjectPresentation(ipg_CaseStatus.Open));
        }

        [Fact]
        public void ObjectPresentation_EntityReference()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            incident.Title = "test";
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            Assert.Equal(incident.Title, gateManager.ObjectPresentation(incident.ToEntityReference()));
        }

        [Fact]
        public void ObjectPresentation_bool()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            Assert.Equal("1", gateManager.ObjectPresentation(true));
        }

        [Fact]
        public void ObjectPresentation_DateTime()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            var date = System.DateTime.Now;
            Assert.Equal(date.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")), gateManager.ObjectPresentation(date));
        }

        [Fact]
        public void ObjectPresentation_Money()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            var money = new Money(100);
            Assert.Equal(money.Value.ToString(), gateManager.ObjectPresentation(money));
        }


        [Fact]
        public void GetCaseNumber_Case()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            incident.Title = "test";
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            Assert.Equal(incident.Title, gateManager.GetCaseNumber(incident));
        }

        [Fact]
        public void GetCaseNumber_Referral_ReferralNumber()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referral = new ipg_referral().Fake();
            referral.ipg_referralnumber = "test";
            var listForInit = new List<Entity>() { referral };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), null);
            Assert.Equal(referral.ipg_referralnumber, gateManager.GetCaseNumber(referral));
        }

        [Fact]
        public void GetCaseNumber_Referral_ReferralCaseNumber()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referral = new ipg_referral().Fake();
            referral.ipg_referralcasenumber = "test";
            var listForInit = new List<Entity>() { referral };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), null);
            Assert.Equal(referral.ipg_referralcasenumber, gateManager.GetCaseNumber(referral));
        }

        [Fact]
        public void CloseOutstandingTasks()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            ipg_statementgenerationtask task = new ipg_statementgenerationtask().Fake().WithCaseReference(incident);
            task.StateCode = ipg_statementgenerationtaskState.Active;
            task.ipg_IsSent = false;
            var listForInit = new List<Entity>() { incident, task };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            gateManager.CloseOutstandingTasks();
            var fakedService = fakedContext.GetOrganizationService();
            var fakedTask = fakedService.Retrieve(task.LogicalName, task.Id, new ColumnSet(ipg_statementgenerationtask.Fields.StateCode, ipg_statementgenerationtask.Fields.StatusCode)).ToEntity<ipg_statementgenerationtask>();
            Assert.True(ipg_statementgenerationtaskState.Inactive == fakedTask.StateCode);
            Assert.True((int)ipg_statementgenerationtask_StatusCode.Completed == fakedTask.StatusCode.Value);
        }

        [Fact]
        public void CheckPatientPrimaryInfo_NotReferral()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Incident incident = new Incident().Fake();
            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), incident.ToEntityReference());
            Assert.True(gateManager.CheckPatientPrimaryInfo().Succeeded);
        }

        [Fact]
        public void CheckPatientPrimaryInfo_RequiredFieldIsEmpty()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referral = new ipg_referral().Fake();
            referral.ipg_PatientFirstName = "FirstName";
            referral.ipg_PatientLastName = "LastName";
            referral.ipg_PatientDateofBirth = null;
            var listForInit = new List<Entity>() { referral };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), referral.ToEntityReference());
            Assert.False(gateManager.CheckPatientPrimaryInfo().Succeeded);
        }

        [Fact]
        public void CheckPatientPrimaryInfo_RequiredFieldsArePopulated()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referral = new ipg_referral().Fake();
            referral.ipg_PatientFirstName = "FirstName";
            referral.ipg_PatientLastName = "LastName";
            referral.ipg_PatientDateofBirth = System.DateTime.Now;
            var listForInit = new List<Entity>() { referral };
            fakedContext.Initialize(listForInit);

            //Assert
            var gateManager = new GateManager(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService(), referral.ToEntityReference());
            Assert.True(gateManager.CheckPatientPrimaryInfo().Succeeded);
        }
    }
}
