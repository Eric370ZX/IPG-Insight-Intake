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
    public class CreateAdjustmentTest : PluginTestsBase
    {
        [Fact]
        public void CheckAdjustmentCreattion()
        {
            var payment = CreateActualPaymentWithCaseReference(out Incident incident,
                out Intake.Account carrier, out Intake.Account facility);
            payment.ipg_MemberPaid_new = new Money(1);
            payment.ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff);
            payment.ipg_ApplyTo = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier);
            payment.ipg_AmountType = true;
            payment.ipg_Amount = new Money(100);
            payment.ipg_AmountToApply = new Money(((Money)payment["ipg_amount"]).Value);
            payment.ipg_ApplyAdjustment = true;

            var inputParameters = new ParameterCollection
            {
                { "Target", payment}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);
            ServiceProvider = ServiceProviderMock.Object;
            SetPluginExecutionContext();

            var plugin = new CreateAdjustment();
            plugin.Execute(ServiceProvider);

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            OrganizationServiceMock.Verify(org =>
                org.Create(It.IsAny<Entity>()), Times.Once);

            OrganizationServiceMock.Verify(org =>
                org.Create(It.Is<ipg_adjustment>(t =>
                    t.ipg_AdjustmentType.Value == ((OptionSetValue)payment["ipg_adjustmenttype"]).Value
                    && t.ipg_ApplyTo.Value == ((OptionSetValue)payment["ipg_applyto"]).Value
                    && t.ipg_AmountType == (bool)payment["ipg_amounttype"]
                    && t.ipg_Amount.Value == ((Money)payment["ipg_amount"]).Value
                    && t.ipg_AmountToApply.Value == ((Money)payment["ipg_amounttoapply"]).Value
                    && t.ipg_CaseId.Id == incident.Id
                    )), Times.Once);
        }

        private ipg_payment CreateActualPaymentWithCaseReference(out Incident incident, out Intake.Account carrier, out Intake.Account facility)
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

        private void SetPluginExecutionContext()
        {
            PluginExecutionContextMock.Setup(c => c.Stage).Returns(PipelineStages.PostOperation);
            PluginExecutionContextMock.Setup(c => c.MessageName).Returns(MessageNames.Create);
            PluginExecutionContextMock.Setup(c => c.PrimaryEntityName).Returns(ipg_payment.EntityLogicalName);
        }
    }
}
