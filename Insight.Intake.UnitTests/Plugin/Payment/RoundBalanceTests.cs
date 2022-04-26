using FakeXrmEasy;
using Insight.Intake.Plugin.Payment;
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
    public class RoundBalanceTests : PluginTestsBase
    {
        [Fact]
        public void PrimaryCarrierBalance()
        {

            var fakedContext = new XrmFakedContext();

            double amountPaid = 100;
            double initialBalance = 0.50;
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_RemainingCarrierBalance = new Money((decimal)initialBalance),
            };
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_TotalInsurancePaid = amountPaid,
            };

            fakedContext.Initialize(new List<Entity> { incident, payment });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<RoundBalance>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            QueryExpression query = new QueryExpression(ipg_adjustment.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_adjustment.ipg_AmountToApply).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_adjustment.ipg_CaseId).ToLower(), ConditionOperator.Equal, incident.ToEntityReference())
                        }
                }
            };
            EntityCollection adjustmentItems = fakedService.RetrieveMultiple(query);
            Assert.Single(adjustmentItems.Entities);
            Assert.Equal(adjustmentItems.Entities[0].GetAttributeValue<Money>(nameof(ipg_adjustment.ipg_AmountToApply).ToLower()).Value, (decimal)initialBalance);

        }

        [Fact]
        public void SecondaryCarrierBalance()
        {

            var fakedContext = new XrmFakedContext();

            double amountPaid = 100;
            double initialBalance = -0.50;
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_RemainingSecondaryCarrierBalance = new Money((decimal)initialBalance),
            };
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_TotalInsurancePaid = amountPaid,
            };

            fakedContext.Initialize(new List<Entity> { incident, payment });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<RoundBalance>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            QueryExpression query = new QueryExpression(ipg_adjustment.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_adjustment.ipg_AmountToApply).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_adjustment.ipg_CaseId).ToLower(), ConditionOperator.Equal, incident.ToEntityReference())
                        }
                }
            };
            EntityCollection adjustmentItems = fakedService.RetrieveMultiple(query);
            Assert.Single(adjustmentItems.Entities);
            Assert.Equal(adjustmentItems.Entities[0].GetAttributeValue<Money>(nameof(ipg_adjustment.ipg_AmountToApply).ToLower()).Value, (decimal)initialBalance);

        }

        [Fact]
        public void PatientBalance()
        {

            var fakedContext = new XrmFakedContext();

            double amountPaid = 100;
            double initialBalance = 1.50;
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_RemainingPatientBalance = new Money((decimal)initialBalance),
            };
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_TotalInsurancePaid = amountPaid,
            };

            fakedContext.Initialize(new List<Entity> { incident, payment });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<RoundBalance>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            QueryExpression query = new QueryExpression(ipg_adjustment.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_adjustment.ipg_AmountToApply).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_adjustment.ipg_CaseId).ToLower(), ConditionOperator.Equal, incident.ToEntityReference())
                        }
                }
            };
            EntityCollection adjustmentItems = fakedService.RetrieveMultiple(query);
            Assert.Empty(adjustmentItems.Entities);

        }

    }
}
