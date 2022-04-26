using FakeXrmEasy;
using Insight.Intake.Plugin.Adjustment;
using Insight.Intake.Plugin.GLTransaction;
using Insight.Intake.Plugin.Case;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Adjustment
{
    public class AutoCaseCloseTest : PluginTestsBase
    {
        [Fact]
        public void CheckCaseClosing()
        {
            var fakedContext = new XrmFakedContext();

            decimal patientBalance = 100;
            Intake.Account carrier = new Intake.Account()
            {
                Id = Guid.NewGuid(),
                ipg_CarrierType = new OptionSetValue((int)ipg_CarrierType.Auto)
            };
            ipg_state state = new ipg_state()
            {
                Id = Guid.NewGuid()
            };
            Intake.Account facility = new Intake.Account()
            {
                Id = Guid.NewGuid(),
                ipg_StateId = state.ToEntityReference()
            };
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_RemainingPatientBalance = new Money(patientBalance),
                ipg_CarrierId = carrier.ToEntityReference(),
                ipg_FacilityId = facility.ToEntityReference()
            };
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff),
                ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.Patient),
                ipg_AmountType = true,
                ipg_Amount = new Money(patientBalance),
                ipg_AmountToApply = new Money(patientBalance)
            };
            ipg_GLTransaction GLTransaction = new ipg_GLTransaction()
            {
                Id = Guid.NewGuid(),
                ipg_IncidentId = incident.ToEntityReference(),
                ipg_PatientRevenue = new Money(patientBalance)
            };

            SystemUser user = new SystemUser();
            var userId = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C45A}");
            user.Id = userId;
            user.FirstName = "John";
            user.LastName = "Wick";

            fakedContext.Initialize(new List<Entity> { carrier, state, facility, incident, adjustment, GLTransaction, user });

            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<InsertGLTransactionOnAdjustmentCreation>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            var GLTransactions = (from t in crmContext.CreateQuery<ipg_GLTransaction>()
                                  select t).ToList();
            var inputParameters2 = new ParameterCollection { { "Target", GLTransactions[0] } };
            var pluginContext2 = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_GLTransaction.EntityLogicalName,
                InputParameters = inputParameters2
            };
            fakedContext.ExecutePluginWith<UpdateCaseFields>(pluginContext2);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_RemainingPatientBalance).ToLower())).ToEntity<Incident>();
            Assert.Equal(0, incidentFaked.ipg_RemainingPatientBalance.Value);

            var inputParameters3 = new ParameterCollection { 
                { "Target", adjustment.ipg_CaseId }, 
                { "CloseReason", 427880015 }, 
                { "CloseNotes", "100% Write-off Adjustment" }, 
                { "SkipChecks", true }, 
                { "CloseReasonDescr", "" },
                { "ClosedBy", "IPG User <Name>" },
                { "FacilityCommunication", "IPG has rejected this Referral because the patient’s procedure, <Procedure Name>, does not include an implant. If the physician uses a covered implant, please notify your IPG Customer Service Rep, <MDM Name>, for reconsideration of this Case.  " },
                { "EventLogEntry", "Referral Rejected in Intake due to No Billable Parts" }
            };
            var pluginContext3 = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGCaseCloseCase",
                Stage = PipelineStages.PostOperation,
                InputParameters = inputParameters3,
                OutputParameters = new ParameterCollection { { "Succeeded", false } },
                InitiatingUserId = userId
            };
            fakedContext.ExecutePluginWith<CloseCaseActionPlugin>(pluginContext3);
            incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_CaseStatus).ToLower())).ToEntity<Incident>();
            Assert.Equal((int)ipg_CaseStatus.Closed, incidentFaked.ipg_CaseStatus.Value);
        }
    }
}
