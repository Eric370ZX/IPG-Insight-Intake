using FakeXrmEasy;
using Insight.Intake.Plugin.Payment;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Payment
{
    public class PreValidatePaymentTest : PluginTestsBase
    {
        [Fact]
        public void EmptyNotification()
        {
            var fakedContext = new XrmFakedContext();

            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch()
            {
                Id = Guid.NewGuid(),
                ipg_IsManualBatch = true
            };
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid()
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                ipg_carrierid = carrier.ToEntityReference()
            };
            ipg_claimresponseheader header = new ipg_claimresponseheader()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseBatchId = batch.ToEntityReference(),
                ipg_AmountPaid_new = new Money(amountPaid),
                ipg_PostStatus = "new",
                ipg_ClaimId = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };
            ipg_claimresponseline claimResponseLine = new ipg_claimresponseline()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeaderId = header.ToEntityReference(),
                ipg_AdjProc = hcpcsName,
                ipg_AllowedActual_new = new Money(amountPaid),
                ipg_AmountPaid_new = new Money(amountPaid),
                ipg_DenialCodeString = denialCodeString,
                ipg_RemarkCodeString = remarkCodeString
            };
            ipg_masterhcpcs hcpcs = new ipg_masterhcpcs().Fake().RuleFor(p => p.ipg_name, p => hcpcsName);
            ipg_claimlineitem claimLineItem = new ipg_claimlineitem()
            {
                Id = Guid.NewGuid(),
                ipg_claimid = claim.ToEntityReference(),
                ipg_hcpcsid = hcpcs.ToEntityReference(),
                ipg_ExpectedReimbursement = new Money(amountPaid)
            };
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeader = header.ToEntityReference(),
                ipg_Claim = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };

            fakedContext.Initialize(new List<Entity> { batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem });

            var inputParameters = new ParameterCollection { { "Target", payment.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGPaymentPreValidatePayment",
                Stage = 40,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
            };
            fakedContext.ExecutePluginWith<PreValidatePayment>(pluginContext);

            Assert.Equal("", pluginContext.OutputParameters["Message"]);
        }

        [Fact]
        public void FullPatientResponsibility()
        {
            var fakedContext = new XrmFakedContext();

            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch()
            {
                Id = Guid.NewGuid(),
                ipg_IsManualBatch = true
            };
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_RemainingCarrierBalance = new Money(200),
                ipg_BillToPatient = new OptionSetValue((int)ipg_TwoOptions.No)
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                ipg_carrierid = carrier.ToEntityReference(),
                Name = "claim"
            };
            ipg_claimresponseheader header = new ipg_claimresponseheader()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseBatchId = batch.ToEntityReference(),
                ipg_AmountPaid_new = new Money(amountPaid),
                ipg_PostStatus = "new",
                ipg_ClaimId = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };
            ipg_claimresponseline claimResponseLine = new ipg_claimresponseline()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeaderId = header.ToEntityReference(),
                ipg_AdjProc = hcpcsName,
                ipg_AllowedActual_new = new Money(amountPaid),
                ipg_AmountPaid_new = new Money(0),
                ipg_DenialCodeString = denialCodeString,
                ipg_RemarkCodeString = remarkCodeString,
            };
            ipg_masterhcpcs hcpcs = new ipg_masterhcpcs().Fake().RuleFor(p => p.ipg_name, p => hcpcsName);
            ipg_claimlineitem claimLineItem = new ipg_claimlineitem()
            {
                Id = Guid.NewGuid(),
                ipg_claimid = claim.ToEntityReference(),
                ipg_hcpcsid = hcpcs.ToEntityReference(),
                ipg_ExpectedReimbursement = new Money(amountPaid)
            };
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeader = header.ToEntityReference(),
                ipg_Claim = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };

            fakedContext.Initialize(new List<Entity> { batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem });

            var inputParameters = new ParameterCollection { { "Target", payment.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGPaymentPreValidatePayment",
                Stage = 40,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
            };
            fakedContext.ExecutePluginWith<PreValidatePayment>(pluginContext);

            Assert.Equal("A payment for the Case has been received and posted. Remaining balance cannot be transferred to Patient due to “Bill to Patient” being set to “No”. Please review and adjust manually.", pluginContext.OutputParameters["Message"]);
        }

        [Fact]
        public void PartialDenialFromCarrier()
        {
            var fakedContext = new XrmFakedContext();

            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch()
            {
                Id = Guid.NewGuid(),
                ipg_IsManualBatch = true
            };
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_BillToPatient = new OptionSetValue((int)ipg_TwoOptions.Yes)
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                ipg_carrierid = carrier.ToEntityReference(),
                Name = "claim"
            };
            ipg_claimresponseheader header = new ipg_claimresponseheader()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseBatchId = batch.ToEntityReference(),
                ipg_AmountPaid_new = new Money(amountPaid),
                ipg_PostStatus = "new",
                ipg_ClaimId = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };
            ipg_claimresponseline claimResponseLine = new ipg_claimresponseline()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeaderId = header.ToEntityReference(),
                ipg_AdjProc = hcpcsName,
                ipg_AllowedActual_new = new Money(amountPaid),
                ipg_AmountPaid_new = new Money(amountPaid),
                ipg_DenialCodeString = denialCodeString,
                ipg_RemarkCodeString = remarkCodeString,
            };
            ipg_masterhcpcs hcpcs = new ipg_masterhcpcs().Fake().RuleFor(p => p.ipg_name, p => hcpcsName);
            ipg_claimlineitem claimLineItem = new ipg_claimlineitem()
            {
                Id = Guid.NewGuid(),
                ipg_claimid = claim.ToEntityReference(),
                ipg_hcpcsid = hcpcs.ToEntityReference(),
                ipg_ExpectedReimbursement = new Money(amountPaid + 1)
            };
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeader = header.ToEntityReference(),
                ipg_Claim = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };

            fakedContext.Initialize(new List<Entity> { batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem });

            var inputParameters = new ParameterCollection { { "Target", payment.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGPaymentPreValidatePayment",
                Stage = 40,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };
            fakedContext.ExecutePluginWith<PreValidatePayment>(pluginContext);

            Assert.Equal("Carrier Collections - Partial Denial", pluginContext.OutputParameters["Message"]);
        }

        [Fact]
        public void FullDenialFromCarrier()
        {
            var fakedContext = new XrmFakedContext();

            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch()
            {
                Id = Guid.NewGuid(),
                ipg_IsManualBatch = true
            };
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_BillToPatient = new OptionSetValue((int)ipg_TwoOptions.Yes)
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                ipg_carrierid = carrier.ToEntityReference(),
                Name = "claim"
            };
            ipg_claimresponseheader header = new ipg_claimresponseheader()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseBatchId = batch.ToEntityReference(),
                ipg_AmountPaid_new = new Money(amountPaid),
                ipg_PostStatus = "new",
                ipg_ClaimId = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };
            ipg_claimresponseline claimResponseLine = new ipg_claimresponseline()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeaderId = header.ToEntityReference(),
                ipg_AdjProc = hcpcsName,
                ipg_AllowedActual_new = new Money(0),
                ipg_AmountPaid_new = new Money(0),
                ipg_DenialCodeString = denialCodeString,
                ipg_RemarkCodeString = remarkCodeString,
            };
            ipg_masterhcpcs hcpcs = new ipg_masterhcpcs().Fake().RuleFor(p => p.ipg_name, p => hcpcsName);
            ipg_claimlineitem claimLineItem = new ipg_claimlineitem()
            {
                Id = Guid.NewGuid(),
                ipg_claimid = claim.ToEntityReference(),
                ipg_hcpcsid = hcpcs.ToEntityReference(),
                ipg_ExpectedReimbursement = new Money(amountPaid)
            };
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeader = header.ToEntityReference(),
                ipg_Claim = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };

            fakedContext.Initialize(new List<Entity> { batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem });

            var inputParameters = new ParameterCollection { { "Target", payment.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGPaymentPreValidatePayment",
                Stage = 40,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };
            fakedContext.ExecutePluginWith<PreValidatePayment>(pluginContext);

            Assert.Equal("Carrier Collections - Full Denial", pluginContext.OutputParameters["Message"]);
        }

        [Fact]
        public void Recoupment()
        {
            var fakedContext = new XrmFakedContext();

            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch()
            {
                Id = Guid.NewGuid(),
                ipg_IsManualBatch = true
            };
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_BillToPatient = new OptionSetValue((int)ipg_TwoOptions.Yes)
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                ipg_carrierid = carrier.ToEntityReference(),
                Name = "claim"
            };
            ipg_claimresponseheader header = new ipg_claimresponseheader()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseBatchId = batch.ToEntityReference(),
                ipg_AmountPaid_new = new Money(amountPaid),
                ipg_PostStatus = "new",
                ipg_ClaimId = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };
            ipg_claimresponseline claimResponseLine = new ipg_claimresponseline()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeaderId = header.ToEntityReference(),
                ipg_AdjProc = hcpcsName,
                ipg_AllowedActual_new = new Money(amountPaid),
                ipg_AmountPaid_new = new Money(amountPaid),
                ipg_DenialCodeString = denialCodeString,
                ipg_RemarkCodeString = remarkCodeString,
                ipg_AmountSubmitted_new = new Money(-1)
            };
            ipg_masterhcpcs hcpcs = new ipg_masterhcpcs().Fake().RuleFor(p => p.ipg_name, p => hcpcsName);
            ipg_claimlineitem claimLineItem = new ipg_claimlineitem()
            {
                Id = Guid.NewGuid(),
                ipg_claimid = claim.ToEntityReference(),
                ipg_hcpcsid = hcpcs.ToEntityReference(),
                ipg_ExpectedReimbursement = new Money(amountPaid)
            };
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeader = header.ToEntityReference(),
                ipg_Claim = claim.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };

            fakedContext.Initialize(new List<Entity> { batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem });

            var inputParameters = new ParameterCollection { { "Target", payment.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGPaymentPreValidatePayment",
                Stage = 40,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };
            fakedContext.ExecutePluginWith<PreValidatePayment>(pluginContext);

            Assert.Equal("Carrier Collections - Recoupment follow-up", pluginContext.OutputParameters["Message"]);
        }
    }
}
