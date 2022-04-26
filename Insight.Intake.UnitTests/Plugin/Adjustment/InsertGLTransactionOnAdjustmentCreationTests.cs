using FakeXrmEasy;
using Insight.Intake.Plugin.Adjustment;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Adjustment
{
    public class InsertGLTransactionOnAdjustmentCreationTests : PluginTestsBase
    {
        [Fact]
        public void Inserts_WriteOff_Transaction_For_Patient()
        {
            //ARRANGE

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            adjustment.ipg_AmountToApply = new Money(1);
            adjustment.ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.Patient);
            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorWriteOff.Value == 0
                    && t.ipg_PatientWriteOff.Value == adjustment.ipg_AmountToApply.Value
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_WriteOff_Transaction_For_PrimaryCarrier()
        {
            //ARRANGE

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            adjustment.ipg_AmountToApply = new Money(1);
            adjustment.ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier);
            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorWriteOff.Value == adjustment.ipg_AmountToApply.Value
                    && t.ipg_PatientWriteOff.Value == 0
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_WriteOff_Transaction_For_SecondaryCarrier()
        {
            //ARRANGE

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            secondaryCarrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[1].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            adjustment.ipg_AmountToApply = new Money(1);
            adjustment.ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier);
            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorWriteOff.Value == adjustment.ipg_AmountToApply.Value
                    && t.ipg_PatientWriteOff.Value == 0
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_Discount_Transaction_For_Patient()
        {
            //ARRANGE

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            adjustment.ipg_AmountToApply = new Money(1);
            adjustment.ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.Patient);
            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff);
            adjustment.ipg_Reason = new OptionSetValue((int)ipg_AdjustmentReasons.WOPatientDiscount);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    //t.ipg_TransactionType == "S"
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    //&& t.ipg_PayorRevenue.Value == 0
                    && t.ipg_PayorWriteOff.Value == 0
                    //&& t.ipg_PatientRevenue.Value == -adjustment.ipg_AmountToApply.Value
                    && t.ipg_PatientWriteOff.Value == adjustment.ipg_AmountToApply.Value
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_SalesAdjustment_Transaction_For_Patient()
        {
            //ARRANGE

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            adjustment.ipg_AmountToApply = new Money(1);
            adjustment.ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.Patient);
            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.SalesAdjustment);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorAdjustment.Value == 0
                    && t.ipg_PatientAdjustment.Value == -adjustment.ipg_AmountToApply.Value
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_SalesAdjustment_Transaction_For_PrimaryCarrier()
        {
            //ARRANGE

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            adjustment.ipg_AmountToApply = new Money(1);
            adjustment.ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier);
            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.SalesAdjustment);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorAdjustment.Value == -adjustment.ipg_AmountToApply.Value
                    && t.ipg_PatientAdjustment.Value == 0
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_SalesAdjustment_Transaction_For_SecondaryCarrier()
        {
            //ARRANGE

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            secondaryCarrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[1].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            adjustment.ipg_AmountToApply = new Money(1);
            adjustment.ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier);
            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.SalesAdjustment);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorAdjustment.Value == -adjustment.ipg_AmountToApply.Value
                    && t.ipg_PatientAdjustment.Value == 0
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_TransferOfReponsiblityFromPrimaryCarrierToPatient()
        {
            var fakedContext = new XrmFakedContext();

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            decimal amount = 1;

            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofResponsibility);
            adjustment.ipg_TransferofPaymentType = true;
            adjustment.ipg_From = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier);
            adjustment.ipg_To = new OptionSetValue((int)ipg_PayerType.Patient);
            adjustment.ipg_AmountToApply = new Money(amount);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorAdjustment.Value == -adjustment.ipg_AmountToApply.Value
                    && t.ipg_PatientAdjustment.Value == adjustment.ipg_AmountToApply.Value
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_TransferOfReponsiblityFromPrimaryCarrierToSecondaryCarrier()
        {
            var fakedContext = new XrmFakedContext();

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            decimal amount = 1;

            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofResponsibility);
            adjustment.ipg_From = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier);
            adjustment.ipg_To = new OptionSetValue((int)ipg_PayerType.SecondaryCarrier);
            adjustment.ipg_AmountToApply = new Money(amount);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Exactly(2));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorAdjustment.Value == -adjustment.ipg_AmountToApply.Value
                    //&& t.ipg_IsSecondaryCarrier == false
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "A"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorAdjustment.Value == adjustment.ipg_AmountToApply.Value
                    && t.ipg_IsSecondaryCarrier == true
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }


        [Fact]
        public void Inserts_TransferOfPaymentToAnotherPayer()
        {
            var fakedContext = new XrmFakedContext();

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            var payment = CreateActualPaymentWithCaseReference(incident);
            payment.ipg_TotalInsurancePaid = 1;

            OrganizationServiceMock.WithRetrieveCrud(payment);

            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofPayment);
            adjustment.ipg_TransferofPaymentType = true;
            adjustment.ipg_From = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier);
            adjustment.ipg_To = new OptionSetValue((int)ipg_PayerType.Patient);
            adjustment.ipg_Payment = payment.ToEntityReference();
            adjustment.ipg_AmountToApply = new Money((decimal)payment.ipg_TotalInsurancePaid);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Exactly(2));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "C"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorCashAmount.Value == -adjustment.ipg_AmountToApply.Value
                    && t.ipg_PatientCashAmount.Value == 0
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "C"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorCashAmount.Value == 0
                    && t.ipg_PatientCashAmount.Value == adjustment.ipg_AmountToApply.Value
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_TransferOfPaymentToAnotherCaseCarrierPayment()
        {
            var fakedContext = new XrmFakedContext();

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            var incidentSecond = CreateCase(carrierNetworks, secondaryCarrier, facility, carrier);

            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            secondaryCarrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[1].Id);
            SetupNetworksQuery(carrierNetworks, secondaryCarrier, facility);

            var payment = CreateActualPaymentWithCaseReference(incident);
            payment.ipg_TotalInsurancePaid = 1;
            payment.ipg_InterestPayment = new Money(2);

            OrganizationServiceMock.WithRetrieveCrud(payment);

            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofPayment);
            adjustment.ipg_TransferofPaymentType = false;
            adjustment.ipg_FromCase = incident.ToEntityReference();
            adjustment.ipg_ToCase = incidentSecond.ToEntityReference();
            adjustment.ipg_Payment = payment.ToEntityReference();
            adjustment.ipg_AmountToApply = new Money((decimal)payment.ipg_TotalInsurancePaid + payment.ipg_InterestPayment.Value);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Exactly(4));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "C"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorCashAmount.Value == (decimal)-payment.ipg_TotalInsurancePaid
                    && t.ipg_PatientCashAmount.Value == 0
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "C"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorCashAmount.Value == -payment.ipg_InterestPayment.Value
                    && t.ipg_PatientCashAmount.Value == 0
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "C"
                    && t.ipg_name == incidentSecond.Title
                    && t.ipg_NetworkType == carrierNetworks[1].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorCashAmount.Value == (decimal)payment.ipg_TotalInsurancePaid
                    && t.ipg_PatientCashAmount.Value == 0
                    && t.ipg_IncidentId.Id == incidentSecond.Id
                    )), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "C"
                    && t.ipg_name == incidentSecond.Title
                    && t.ipg_NetworkType == carrierNetworks[1].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorCashAmount.Value == payment.ipg_InterestPayment.Value
                    && t.ipg_PatientCashAmount.Value == 0
                    && t.ipg_IncidentId.Id == incidentSecond.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_TransferOfPaymentToAnotherCasePatientPayment()
        {
            var fakedContext = new XrmFakedContext();

            var carrierNetworks = new List<ipg_carriernetwork>()
            {
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_CarrierAssignmentOnly = true,
                    ipg_AbbreviatedNameforGP = "network1"
                },
                new ipg_carriernetwork()
                {
                    Id = Guid.NewGuid(),
                    ipg_ContractedPayorsOnly = true,
                    ipg_AbbreviatedNameforGP = "network2"
                }
            };
            var adjustment = CreateAdjustmentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier);
            var incidentSecond = CreateCase(carrierNetworks, secondaryCarrier, facility, carrier);

            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);

            secondaryCarrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[1].Id);
            SetupNetworksQuery(carrierNetworks, secondaryCarrier, facility);

            var payment = CreateActualPaymentWithCaseReference(incident);
            payment.ipg_MemberPaid_new = new Money(1);

            OrganizationServiceMock.WithRetrieveCrud(payment);

            adjustment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofPayment);
            adjustment.ipg_TransferofPaymentType = false;
            adjustment.ipg_FromCase = incident.ToEntityReference();
            adjustment.ipg_ToCase = incidentSecond.ToEntityReference();
            adjustment.ipg_Payment = payment.ToEntityReference();
            adjustment.ipg_AmountToApply = new Money(payment.ipg_MemberPaid_new.Value);

            var inputParameters = new ParameterCollection
            {
                { "Target", adjustment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnAdjustmentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Exactly(2));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "C"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorCashAmount.Value == 0
                    && t.ipg_PatientCashAmount.Value == -adjustment.ipg_AmountToApply.Value
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "C"
                    && t.ipg_name == incidentSecond.Title
                    && t.ipg_NetworkType == carrierNetworks[1].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorCashAmount.Value == 0
                    && t.ipg_PatientCashAmount.Value == adjustment.ipg_AmountToApply.Value
                    && t.ipg_IncidentId.Id == incidentSecond.Id
                    )), Times.Once);
        }

        private ipg_adjustment CreateAdjustmentWithCaseReference(IList<ipg_carriernetwork> carrierNetworks, out Incident incident, out Intake.Account carrier, out Intake.Account facility, out Intake.Account secondaryCarrier)
        {
            carrier = new Intake.Account
            {
                Id = Guid.NewGuid(),
                ipg_contract = true,
                ipg_CarrierType = new OptionSetValue((int)ipg_CarrierType.Auto)
            };

            secondaryCarrier = new Intake.Account
            {
                Id = Guid.NewGuid(),
                ipg_contract = true,
                ipg_CarrierType = new OptionSetValue((int)ipg_CarrierType.Auto),
               };

            facility = new Intake.Account
            {
                Id = Guid.NewGuid(),
                ipg_StateId = new EntityReference(ipg_state.EntityLogicalName, Guid.NewGuid())
            };

            incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_CarrierId = new EntityReference(Intake.Account.EntityLogicalName, carrier.Id),
                ipg_FacilityId = new EntityReference(Intake.Account.EntityLogicalName, facility.Id),
                Title = "123456",
                ipg_SecondaryCarrierId = new EntityReference(Intake.Account.EntityLogicalName, secondaryCarrier.Id)
            };

            Guid incidentId = incident.Id;
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Incident.EntityLogicalName, incidentId, It.IsAny<ColumnSet>()))
                .Returns(incident);

            Guid carrierId = carrier.Id;
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Intake.Account.EntityLogicalName, carrierId, It.IsAny<ColumnSet>()))
                .Returns(carrier);

            Guid secondaryCarrierId = secondaryCarrier.Id;
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Intake.Account.EntityLogicalName, secondaryCarrierId, It.IsAny<ColumnSet>()))
                .Returns(secondaryCarrier);

            Guid facilityId = facility.Id;
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Intake.Account.EntityLogicalName, facilityId, It.IsAny<ColumnSet>()))
                .Returns(facility);

            return new ipg_adjustment()
            {
                Id = Guid.NewGuid(),
                ipg_CaseId = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };
        }

        private Incident CreateCase(IList<ipg_carriernetwork> carrierNetworks, Intake.Account carrier, Intake.Account facility, Intake.Account secondaryCarrier)
        {
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_CarrierId = new EntityReference(Intake.Account.EntityLogicalName, carrier.Id),
                ipg_FacilityId = new EntityReference(Intake.Account.EntityLogicalName, facility.Id),
                Title = "654321",
                ipg_SecondaryCarrierId = new EntityReference(Intake.Account.EntityLogicalName, secondaryCarrier.Id)
            };

            Guid incidentId = incident.Id;
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Incident.EntityLogicalName, incidentId, It.IsAny<ColumnSet>()))
                .Returns(incident);

            return incident;
        }

        private IList<ipg_carriernetwork> SetupNetworksQuery(IList<ipg_carriernetwork> carrierNetworks,
            Intake.Account carrier, Intake.Account facility)
        {
            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(q => q.EntityName == ipg_carriernetwork.EntityLogicalName
                    && q.Criteria.FilterOperator == LogicalOperator.And
                    && q.Criteria.Conditions.Count == 2
                    && q.Criteria.Conditions.Any(c => c.AttributeName == nameof(ipg_carriernetwork.StateCode).ToLower() && c.Operator == ConditionOperator.Equal && c.Values.Count == 1 && (int)c.Values[0] == 0) //0 means active
                    && q.Criteria.Conditions.Any(c => c.AttributeName == nameof(ipg_carriernetwork.ipg_plantype).ToLower() && c.Operator == ConditionOperator.ContainValues && c.Values.Count == 1 && (int)c.Values[0] == (carrier.ipg_CarrierType).Value)
                    && q.LinkEntities[0].LinkCriteria.Conditions.Count == 1
                    && q.LinkEntities[0].LinkCriteria.Conditions.Any(c => c.AttributeName == nameof(ipg_ipg_carriernetwork_ipg_state.ipg_stateid).ToLower() && c.Operator == ConditionOperator.Equal && c.Values.Count == 1 && (Guid)c.Values[0] == facility.ipg_StateId.Id))))
                .Returns(new EntityCollection(carrierNetworks.Cast<Entity>().ToList()));

            return carrierNetworks;
        }

        private void SetPluginExecutionContext()
        {
            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PostOperation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Create);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(ipg_adjustment.EntityLogicalName);
        }

        private ipg_payment CreateActualPaymentWithCaseReference(Incident incident)
        {
            return new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_PaymentType = new OptionSetValue((int)ipg_PaymentType.Actual),
                ipg_CaseId = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };
        }
    }
}