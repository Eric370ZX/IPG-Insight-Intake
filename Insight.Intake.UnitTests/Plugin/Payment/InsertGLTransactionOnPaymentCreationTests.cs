using Insight.Intake.Plugin.Payment;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Payment
{
    public class InsertGLTransactionOnPaymentCreationTests : PluginTestsBase
    {
        [Fact]
        public void Inserts_Cash_Transaction_For_Assigned_Carrier()
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
            var payment = CreateActualPaymentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility);
            carrier.ipg_carriernetworkid = new EntityReference(
                    ipg_carriernetwork.EntityLogicalName, carrierNetworks[0].Id);
            SetupNetworksQuery(carrierNetworks, carrier, facility);
            
            payment.ipg_MemberPaid_new = new Money(1);

            var inputParameters = new ParameterCollection
            {
                { "Target", payment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnPaymentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "C"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[0].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorCashAmount.Value == 0
                    && t.ipg_PatientCashAmount.Value == payment.ipg_MemberPaid_new.Value
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        [Fact]
        public void Inserts_RevenueAdjustment_Transaction_For_Contracted_Carrier()
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
            var payment = CreateActualPaymentWithCaseReference(carrierNetworks, out Incident incident,
                out Intake.Account carrier, out Intake.Account facility);
            carrier.ipg_contract = true;
            SetupNetworksQuery(carrierNetworks, carrier, facility);
            
            payment.ipg_ar_carrieradj = -10;

            var inputParameters = new ParameterCollection
            {
                { "Target", payment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            ServiceProvider = ServiceProviderMock.Object;

            SetPluginExecutionContext();


            //ACT

            var plugin = new InsertGLTransactionOnPaymentCreation();
            plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_GLTransaction>(t =>
                    t.ipg_TransactionType == "S"
                    && t.ipg_name == incident.Title
                    && t.ipg_NetworkType == carrierNetworks[1].ipg_AbbreviatedNameforGP
                    && t.ipg_PayorRevenue.Value == Convert.ToDecimal(-payment.ipg_ar_carrieradj)
                    && t.ipg_PatientRevenue.Value == 0
                    && t.ipg_IncidentId.Id == incident.Id
                    )), Times.Once);
        }

        //todo: add more tests. It is important financial data


        private ipg_payment CreateActualPaymentWithCaseReference(IList<ipg_carriernetwork> carrierNetworks, out Incident incident, out Intake.Account carrier, out Intake.Account facility)
        {
            carrier = new Intake.Account
            {
                Id = Guid.NewGuid(),
                ipg_contract = true,
                ipg_CarrierType = new OptionSetValue((int)ipg_CarrierType.Auto)

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
                Title = "123456"
            };

            Guid incidentId = incident.Id;
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Incident.EntityLogicalName, incidentId, It.IsAny<ColumnSet>()))
                .Returns(incident);

            Guid carrierId = carrier.Id;
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Intake.Account.EntityLogicalName, carrierId, It.IsAny<ColumnSet>()))
                .Returns(carrier);

            Guid facilityId = facility.Id;
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Intake.Account.EntityLogicalName, facilityId, It.IsAny<ColumnSet>()))
                .Returns(facility);

            return new ipg_payment()
            {
                ipg_PaymentType = new OptionSetValue((int)ipg_PaymentType.Actual),
                ipg_CaseId = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };
        }

        private IList<ipg_carriernetwork> SetupNetworksQuery(IList<ipg_carriernetwork> carrierNetworks,
            Intake.Account carrier, Intake.Account facility)
        {
            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(q => q.EntityName == ipg_carriernetwork.EntityLogicalName
                    && q.Criteria.FilterOperator == LogicalOperator.And
                    && q.Criteria.Conditions.Count == 2
                    && q.Criteria.Conditions.Any(c => c.AttributeName == nameof(ipg_carriernetwork.StateCode).ToLower() && c.Operator == ConditionOperator.Equal && c.Values.Count == 1 && (int)c.Values[0] == 0) //0 means active
                    && q.Criteria.Conditions.Any(c => c.AttributeName == nameof(ipg_carriernetwork.ipg_plantype).ToLower() && c.Operator == ConditionOperator.ContainValues && c.Values.Count == 1 && (int)c.Values[0] == carrier.ipg_CarrierType.Value)
                    && q.LinkEntities[0].LinkCriteria.Conditions.Count == 1
                    && q.LinkEntities[0].LinkCriteria.Conditions.Any(c => c.AttributeName == nameof(ipg_ipg_carriernetwork_ipg_state.ipg_stateid).ToLower() && c.Operator == ConditionOperator.Equal && c.Values.Count == 1 && (Guid)c.Values[0] == facility.ipg_StateId.Id))))
                .Returns(new EntityCollection(carrierNetworks.Cast<Entity>().ToList()));

            return carrierNetworks;
        }

        private void SetPluginExecutionContext()
        {
            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PostOperation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Create);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(ipg_payment.EntityLogicalName);
        }
    }
}
