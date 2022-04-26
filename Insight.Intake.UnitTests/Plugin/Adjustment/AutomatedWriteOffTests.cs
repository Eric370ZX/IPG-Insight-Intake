using FakeXrmEasy;
using Insight.Intake.Plugin.Adjustment;
using Insight.Intake.Plugin.GLTransaction;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Payment
{
    public class AutomatedWriteOffTests : PluginTestsBase
    {
        [Fact]
        public void CheckAdjustmentCreation()
        {

            var fakedContext = new XrmFakedContext();

            decimal patientBalance = 100;
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_RemainingPatientBalance = new Money(patientBalance)
            };

            fakedContext.Initialize(new List<Entity> { incident });

            var inputParameters = new ParameterCollection { { "Cases", new EntityCollection(new List<Entity> { incident }) } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsAutomatedWriteOff",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<AutomatedWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var adjustments = (from adjustment in crmContext.CreateQuery<ipg_adjustment>()
                                  where adjustment.ipg_CaseId.Id == incident.Id
                                  select adjustment).ToList();

            Assert.True(adjustments.Count == 1);
        }

    }
}
