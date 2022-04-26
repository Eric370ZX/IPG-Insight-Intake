using System;
using System.Collections.Generic;
using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class CloseCaseActionPluginTest : PluginTestsBase
    {
        [Fact]
        public void CloseCaseAction_Success()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13A}");
            caseEntity.StateCode = IncidentState.Active;
            caseEntity.ipg_StateCode = new OptionSetValue((int)ipg_CaseStateCodes.Authorization);

            SystemUser user = new SystemUser().Fake();
            var userId = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C45A}");
            user.Id = userId;
            user.FirstName = "John";
            user.LastName = "Wick";

            var listForInit = new List<Entity>() { caseEntity, user };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity.ToEntityReference() },
                { "CloseReason", 427880015},
                { "CloseReasonDescr","test note" },
                { "CloseNotes", "test note" },
                { "ClosedBy", "IPG User <Name>" },
                { "FacilityCommunication", "IPG has rejected this Referral because the patient’s procedure, <Procedure Name>, does not include an implant. If the physician uses a covered implant, please notify your IPG Customer Service Rep, <MDM Name>, for reconsideration of this Case.  " },
                { "EventLogEntry", "Referral Rejected in Intake due to No Billable Parts" }
            };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGCaseCloseCase",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = userId
            };
            //ACT
            fakedContext.ExecutePluginWith<CloseCaseActionPlugin>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}