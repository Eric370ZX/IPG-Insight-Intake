using FakeXrmEasy;
using Insight.Intake.Plugin.Adjustment;
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

namespace Insight.Intake.UnitTests.Plugin.Adjustment
{
    public class CalculateCaseAdjustmentAndWriteOffTests : PluginTestsBase
    {
        decimal initialCarrierRespAdjustments = 100;
        decimal initialSecondaryCarrierRespAdjustments = 110;
        decimal initialPatientAdjustments = 120;
        decimal initialCarrierWriteOff = 130;
        decimal initialSecondaryCarrierWriteOff = 140;
        decimal initialPatientWriteOff = 150;
        decimal amountToApply = 10;
        double totalInsurancePaid = 20;

        [Fact]
        public void PrimaryCarrierWriteOff()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff),
                ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_TotalCarrierRespAdjustments).ToLower(), nameof(Incident.ipg_TotalCarrierWriteoff).ToLower())).ToEntity<Incident>();
            Assert.Equal(initialCarrierRespAdjustments + amountToApply, incidentFaked.ipg_TotalCarrierRespAdjustments.Value);
            Assert.Equal(initialCarrierWriteOff + amountToApply, incidentFaked.ipg_TotalCarrierWriteoff.Value);
        }

        [Fact]
        public void SecondaryCarrierWriteOff()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff),
                ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_TotalSecondaryCarrierRespAdjustments).ToLower(), nameof(Incident.ipg_TotalSecondaryCarrierWriteoff).ToLower())).ToEntity<Incident>();
            Assert.Equal(initialSecondaryCarrierRespAdjustments + amountToApply, incidentFaked.ipg_TotalSecondaryCarrierRespAdjustments.Value);
            Assert.Equal(initialSecondaryCarrierWriteOff + amountToApply, incidentFaked.ipg_TotalSecondaryCarrierWriteoff.Value);
        }

        [Fact]
        public void PatientWriteOff()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff),
                ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.Patient),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_TotalPatientRespAdjustments).ToLower(), nameof(Incident.ipg_TotalPatientWriteoff).ToLower())).ToEntity<Incident>();
            Assert.Equal(initialPatientAdjustments + amountToApply, incidentFaked.ipg_TotalPatientRespAdjustments.Value);
            Assert.Equal(initialPatientWriteOff + amountToApply, incidentFaked.ipg_TotalPatientWriteoff.Value);
        }

        [Fact]
        public void PrimaryCarrierSalesAdjustment()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.SalesAdjustment),
                ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_TotalCarrierRespAdjustments).ToLower(), nameof(Incident.ipg_TotalCarrierWriteoff).ToLower())).ToEntity<Incident>();
            Assert.Equal(initialCarrierRespAdjustments + amountToApply, incidentFaked.ipg_TotalCarrierRespAdjustments.Value);
            Assert.Equal(initialCarrierWriteOff, incidentFaked.ipg_TotalCarrierWriteoff.Value);
        }

        [Fact]
        public void SecondaryCarrierSalesAdjustment()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.SalesAdjustment),
                ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_TotalSecondaryCarrierRespAdjustments).ToLower(), nameof(Incident.ipg_TotalSecondaryCarrierWriteoff).ToLower())).ToEntity<Incident>();
            Assert.Equal(initialSecondaryCarrierRespAdjustments + amountToApply, incidentFaked.ipg_TotalSecondaryCarrierRespAdjustments.Value);
            Assert.Equal(initialSecondaryCarrierWriteOff, incidentFaked.ipg_TotalSecondaryCarrierWriteoff.Value);
        }

        [Fact]
        public void PatientSalesAdjustment()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.SalesAdjustment),
                ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.Patient),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_TotalPatientRespAdjustments).ToLower(), nameof(Incident.ipg_TotalPatientWriteoff).ToLower())).ToEntity<Incident>();
            Assert.Equal(initialPatientAdjustments + amountToApply, incidentFaked.ipg_TotalPatientRespAdjustments.Value);
            Assert.Equal(initialPatientWriteOff, incidentFaked.ipg_TotalPatientWriteoff.Value);
        }

        [Fact]
        public void PrimaryCarrierToSecondaryCarrierTransfer()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofResponsibility),
                ipg_From = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier),
                ipg_To = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(initialCarrierRespAdjustments - amountToApply, incidentFaked.ipg_TotalCarrierRespAdjustments.Value);
            Assert.Equal(initialCarrierWriteOff, incidentFaked.ipg_TotalCarrierWriteoff.Value);
            Assert.Equal(initialSecondaryCarrierRespAdjustments + amountToApply, incidentFaked.ipg_TotalSecondaryCarrierRespAdjustments.Value);
            Assert.Equal(initialSecondaryCarrierWriteOff, incidentFaked.ipg_TotalSecondaryCarrierWriteoff.Value);
        }

        [Fact]
        public void PrimaryCarrierToPatientTransfer()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofResponsibility),
                ipg_From = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier),
                ipg_To = new OptionSetValue((int)ipg_PayerType.Patient),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(initialCarrierRespAdjustments - amountToApply, incidentFaked.ipg_TotalCarrierRespAdjustments.Value);
            Assert.Equal(initialCarrierWriteOff, incidentFaked.ipg_TotalCarrierWriteoff.Value);
            Assert.Equal(initialPatientAdjustments + amountToApply, incidentFaked.ipg_TotalPatientRespAdjustments.Value);
            Assert.Equal(initialPatientWriteOff, incidentFaked.ipg_TotalPatientWriteoff.Value);
        }

        [Fact]
        public void SecondaryCarrierToPrimaryCarrierTransfer()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofResponsibility),
                ipg_From = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier),
                ipg_To = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(initialSecondaryCarrierRespAdjustments - amountToApply, incidentFaked.ipg_TotalSecondaryCarrierRespAdjustments.Value);
            Assert.Equal(initialSecondaryCarrierWriteOff, incidentFaked.ipg_TotalSecondaryCarrierWriteoff.Value);
            Assert.Equal(initialCarrierRespAdjustments + amountToApply, incidentFaked.ipg_TotalCarrierRespAdjustments.Value);
            Assert.Equal(initialCarrierWriteOff, incidentFaked.ipg_TotalCarrierWriteoff.Value);
        }

        [Fact]
        public void SecondaryCarrierToPatientTransfer()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofResponsibility),
                ipg_From = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier),
                ipg_To = new OptionSetValue((int)ipg_PayerType.Patient),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(initialSecondaryCarrierRespAdjustments - amountToApply, incidentFaked.ipg_TotalSecondaryCarrierRespAdjustments.Value);
            Assert.Equal(initialSecondaryCarrierWriteOff, incidentFaked.ipg_TotalSecondaryCarrierWriteoff.Value);
            Assert.Equal(initialPatientAdjustments + amountToApply, incidentFaked.ipg_TotalPatientRespAdjustments.Value);
            Assert.Equal(initialPatientWriteOff, incidentFaked.ipg_TotalPatientWriteoff.Value);
        }

        [Fact]
        public void PatientToPrimaryCarrierTransfer()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofResponsibility),
                ipg_From = new OptionSetValue((int)ipg_PayerType.Patient),
                ipg_To = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(initialPatientAdjustments - amountToApply, incidentFaked.ipg_TotalPatientRespAdjustments.Value);
            Assert.Equal(initialPatientWriteOff, incidentFaked.ipg_TotalPatientWriteoff.Value);
            Assert.Equal(initialCarrierRespAdjustments + amountToApply, incidentFaked.ipg_TotalCarrierRespAdjustments.Value);
            Assert.Equal(initialCarrierWriteOff, incidentFaked.ipg_TotalCarrierWriteoff.Value);
        }

        [Fact]
        public void PatientToSecondaryCarrierTransfer()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofResponsibility),
                ipg_From = new OptionSetValue((int)ipg_PayerType.Patient),
                ipg_To = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(initialPatientAdjustments - amountToApply, incidentFaked.ipg_TotalPatientRespAdjustments.Value);
            Assert.Equal(initialPatientWriteOff, incidentFaked.ipg_TotalPatientWriteoff.Value);
            Assert.Equal(initialSecondaryCarrierRespAdjustments + amountToApply, incidentFaked.ipg_TotalSecondaryCarrierRespAdjustments.Value);
            Assert.Equal(initialSecondaryCarrierWriteOff, incidentFaked.ipg_TotalSecondaryCarrierWriteoff.Value);
        }

        [Fact]
        public void TransferOfPaymentWithinCase()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_TotalInsurancePaid = totalInsurancePaid,
            };
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofPayment),
                ipg_TransferofPaymentType = true,
                ipg_Payment = payment.ToEntityReference(),
                ipg_From = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier),
                ipg_To = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, payment, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(initialCarrierRespAdjustments - amountToApply, incidentFaked.ipg_TotalCarrierRespAdjustments.Value);
            Assert.Equal(initialCarrierWriteOff, incidentFaked.ipg_TotalCarrierWriteoff.Value);
            Assert.Equal(initialSecondaryCarrierRespAdjustments + amountToApply, incidentFaked.ipg_TotalSecondaryCarrierRespAdjustments.Value);
            Assert.Equal(initialSecondaryCarrierWriteOff, incidentFaked.ipg_TotalSecondaryCarrierWriteoff.Value);
        }

        [Fact]
        public void TransferOfPaymentToAnotherCase()
        {
            var fakedContext = new XrmFakedContext();
            Incident incident = CreateIncident();
            Incident toIncident = CreateIncident();
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_TotalInsurancePaid = totalInsurancePaid,
            };
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = incident.ToEntityReference(),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofPayment),
                ipg_TransferofPaymentType = false,
                ipg_Payment = payment.ToEntityReference(),
                ipg_ToCase = toIncident.ToEntityReference(),
                ipg_AmountToApply = new Money(amountToApply)
            };

            fakedContext.Initialize(new List<Entity> { incident, toIncident, payment, adjustment });
            var inputParameters = new ParameterCollection { { "Target", adjustment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_adjustment.EntityLogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<CalculateCaseAdjustmentAndWriteOff>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(initialCarrierRespAdjustments - (decimal)totalInsurancePaid, incidentFaked.ipg_TotalCarrierRespAdjustments.Value);
            Assert.Equal(initialCarrierWriteOff, incidentFaked.ipg_TotalCarrierWriteoff.Value);

            Incident toIncidentFaked = fakedService.Retrieve(toIncident.LogicalName, toIncident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(initialCarrierRespAdjustments + (decimal)totalInsurancePaid, toIncidentFaked.ipg_TotalCarrierRespAdjustments.Value);
            Assert.Equal(initialCarrierWriteOff, toIncidentFaked.ipg_TotalCarrierWriteoff.Value);
        }

        private Incident CreateIncident()
        {
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_TotalCarrierRespAdjustments = new Money(initialCarrierRespAdjustments),
                ipg_TotalSecondaryCarrierRespAdjustments = new Money(initialSecondaryCarrierRespAdjustments),
                ipg_TotalPatientRespAdjustments = new Money(initialPatientAdjustments),
                ipg_TotalCarrierWriteoff = new Money(initialCarrierWriteOff),
                ipg_TotalSecondaryCarrierWriteoff = new Money(initialSecondaryCarrierWriteOff),
                ipg_TotalPatientWriteoff = new Money(initialPatientWriteOff),
            };
            return incident;
        }

    }
}
