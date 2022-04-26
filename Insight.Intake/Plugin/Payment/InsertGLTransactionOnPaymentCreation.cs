using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Payment
{
    public class InsertGLTransactionOnPaymentCreation : PluginBase
    {
        public InsertGLTransactionOnPaymentCreation() : base(typeof(InsertGLTransactionOnPaymentCreation))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_payment.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var orgService = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            tracingService.Trace($"{typeof(InsertGLTransactionOnPaymentCreation)} plugin started");

            tracingService.Trace($"Getting Target input parameter ({nameof(ipg_payment)} entity)");
            var payment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_payment>();
            if (payment.LogicalName != ipg_payment.EntityLogicalName)
            {
                tracingService.Trace($"Target entity is not {nameof(ipg_payment)}. Return");
                return;
            }

            if(payment.ipg_PaymentType == null)
            {
                throw new Exception("Payment must have a payment type");
            }

            if(payment.ipg_PaymentType.Value != (int)ipg_PaymentType.Actual)
            {
                tracingService.Trace("Payment type is not Actual. Return");
                return;
            }

            if (payment.ipg_CaseId == null)
            {
                throw new Exception("Payment must have an Incident reference");
            }

            tracingService.Trace("Retrieving the related Incident");
            Incident incident = orgService.Retrieve(Incident.EntityLogicalName, payment.ipg_CaseId.Id,
                new ColumnSet(
                        nameof(Incident.ipg_CarrierId).ToLower(),
                        nameof(Incident.ipg_FacilityId).ToLower(),
                        nameof(Incident.Title).ToLower(),
                        nameof(Incident.ipg_SecondaryCarrierId).ToLower(),
                        nameof(Incident.ipg_AutoCarrier).ToLower(),
                        nameof(Incident.ipg_AutoCarrierId).ToLower()
                    )).ToEntity<Incident>();
            if (incident == null)
            {
                throw new Exception($"Could not find the requested Case (Incident.id={payment.ipg_CaseId.Id})");
            }

            string networkName = CaseHelper.DetermineNetworkName(incident, orgService, tracingService);
            ipg_claimresponsebatch batch = DetermineBatch(payment, orgService, tracingService);
            string payer = "";
            DateTime? paymentDate = null;
            string checkNumber = "";
            if (batch != null)
            {
                payer = batch.ipg_OrigCompanyName;
                paymentDate = batch.ipg_PaymentDate;
                checkNumber = batch.ipg_CheckNumber;
            }

            var caseHasAutoCarrier = ((incident.ipg_AutoCarrier ?? false) && (incident.ipg_AutoCarrierId != null));
            bool ipg_IsSecondaryCarrier = false;
            if (!caseHasAutoCarrier && (payment.ipg_Claim != null) && (incident.ipg_SecondaryCarrierId != null))
            {
                Invoice claim = orgService.Retrieve(Invoice.EntityLogicalName, payment.ipg_Claim.Id, new ColumnSet(nameof(Invoice.CustomerId).ToLower())).ToEntity<Invoice>();
                ipg_IsSecondaryCarrier = (claim.CustomerId != null) && claim.CustomerId.Equals(incident.ipg_SecondaryCarrierId);
            }

            InsertTransactions(payment, incident, networkName, orgService, tracingService, payer, paymentDate, checkNumber, batch, ipg_IsSecondaryCarrier);

            tracingService.Trace($"{typeof(InsertGLTransactionOnPaymentCreation)} plugin finished");
        }

        private void InsertTransactions(ipg_payment payment, Incident incident, string networkName, 
            IOrganizationService orgService, ITracingService tracingService, string payer, DateTime? paymentDate, string checkNumber, ipg_claimresponsebatch batch, bool ipg_IsSecondaryCarrier)
        {
            double? patientCash = (double)(payment.ipg_MemberPaid_new == null ? 0 : payment.ipg_MemberPaid_new.Value);
            double? payorCash = payment.ipg_TotalInsurancePaid;
            if (payorCash.HasValue && payorCash != 0
                || patientCash.HasValue && patientCash != 0) //todo: why not decimals?
            {
                tracingService.Trace("Inserting a Cash transaction");
                InsertCashTransaction(payment, incident, networkName, payorCash ?? 0, patientCash ?? 0, orgService, payer, paymentDate, checkNumber, batch, ipg_IsSecondaryCarrier);
            }

            double? patientRevenueAdjustment = (payment.ipg_ar_memberadj ?? 0) * -1;
            double? payorRevenueAdjustment = (payment.ipg_ar_carrieradj ?? 0) * -1;
            if (patientRevenueAdjustment.HasValue && patientRevenueAdjustment != 0
                || payorRevenueAdjustment.HasValue && payorRevenueAdjustment != 0) //todo: why not decimals?
            {
                tracingService.Trace("Inserting a Revenue Adjustment transaction");
                InsertRevenueAdjustmentTransaction(payment, incident, networkName, payorRevenueAdjustment,
                    patientRevenueAdjustment, orgService, payer, paymentDate, checkNumber, batch, ipg_IsSecondaryCarrier);
            }

            double? patientSalesAdjustment = (payment.ipg_ar_membersalesadj ?? 0) * -1;
            double? payorSalesAdjustment = (payment.ipg_ar_carriersalesadj ?? 0) * -1;
            if (patientSalesAdjustment.HasValue && patientSalesAdjustment != 0
                || payorSalesAdjustment.HasValue && payorSalesAdjustment != 0) //todo: why not decimals?
            {
                tracingService.Trace("Inserting a Sales Adjustment transaction");
                InsertSalesAdjustmentTransaction(payment, incident, networkName, payorSalesAdjustment,
                    patientSalesAdjustment, orgService, payer, paymentDate, checkNumber, batch, ipg_IsSecondaryCarrier);
            }

            double? patientWO = payment.ipg_ARMemberWriteoff;
            double? payorWO = payment.ipg_ar_carrierwriteoff;
            if (patientWO.HasValue && patientWO.Value != 0
                || payorWO.HasValue && payorWO.Value != 0) //todo: why not decimals?
            {
                tracingService.Trace("Inserting a Write-off transaction");
                InsertWriteoffTransaction(payment, incident, networkName, payorWO, patientWO, orgService, payer, paymentDate, checkNumber, batch, ipg_IsSecondaryCarrier);
            }

            Money interestPayment = payment.ipg_InterestPayment;
            if (interestPayment != null && interestPayment.Value != 0)
            {
                tracingService.Trace("Inserting a Interest transaction");
                InsertInterestTransaction(payment, incident, networkName, interestPayment, orgService, payer, paymentDate, checkNumber, batch, ipg_IsSecondaryCarrier);
            }

            decimal? interestAdjustment = (payment.ipg_InterestAdjustment ?? 0) * -1;
            if (interestAdjustment.HasValue && interestAdjustment != 0)
            {
                tracingService.Trace("Inserting a Interest adjustment transaction");
                InsertInterestAdjustmentTransaction(payment, incident, networkName, interestAdjustment, orgService, payer, paymentDate, checkNumber, batch, ipg_IsSecondaryCarrier);
            }
        }

        private void InsertCashTransaction(ipg_payment payment, Incident incident, string networkName, 
            double? payorCashAmount, double? patientCashAmount, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber, ipg_claimresponsebatch batch, bool ipg_IsSecondaryCarrier)
        {
            var newTransaction = new ipg_GLTransaction()
            {
                ipg_TransactionType = "C",
                ipg_name = incident.Title,
                ipg_NetworkType = networkName,
                ipg_PayorCashAmount = payorCashAmount != null  ? new Money(Convert.ToDecimal(payorCashAmount)) : null,
                ipg_PatientCashAmount = patientCashAmount != null ? new Money(Convert.ToDecimal(patientCashAmount)) : null,
                ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                ipg_Payer = payer,
                ipg_PaymentDate = paymentDate,
                ipg_CheckNumber = checkNumber,
                ipg_ClaimResponseBatch = batch?.ToEntityReference(),
                ipg_Payment = payment.ToEntityReference(),
                ipg_IsSecondaryCarrier = ipg_IsSecondaryCarrier
                //todo: claim response id
            };
            orgService.Create(newTransaction);
            UpdateClaimStatus(payment, orgService);
        }

        private void InsertRevenueAdjustmentTransaction(ipg_payment payment, Incident incident, string networkName,
            double? payorRevenueAdjustment, double? patientRevenueAdjustment, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber, ipg_claimresponsebatch batch, bool ipg_IsSecondaryCarrier)
        {
            var newTransaction = new ipg_GLTransaction()
            {
                ipg_TransactionType = "S",
                ipg_name = incident.Title,
                ipg_NetworkType = networkName,
                ipg_PayorRevenue = new Money(Convert.ToDecimal(payorRevenueAdjustment)),
                ipg_PatientRevenue = new Money(Convert.ToDecimal(patientRevenueAdjustment)),
                ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                ipg_Payer = payer,
                ipg_PaymentDate = paymentDate,
                ipg_CheckNumber = checkNumber,
                ipg_ClaimResponseBatch = batch?.ToEntityReference(),
                ipg_IsSecondaryCarrier = ipg_IsSecondaryCarrier
                //todo: claim response id
            };
            orgService.Create(newTransaction);
            UpdateClaimStatus(payment, orgService);
        }

        private void InsertSalesAdjustmentTransaction(ipg_payment payment, Incident incident, string networkName,
            double? payorSalesAdjustment, double? patientSalesAdjustment, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber, ipg_claimresponsebatch batch, bool ipg_IsSecondaryCarrier)
        {
            var newTransaction = new ipg_GLTransaction()
            {
                ipg_TransactionType = "A",
                ipg_name = incident.Title,
                ipg_NetworkType = networkName,
                ipg_PayorAdjustment = new Money(Convert.ToDecimal(payorSalesAdjustment)),
                ipg_PatientAdjustment = new Money(Convert.ToDecimal(patientSalesAdjustment)),
                ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                ipg_Payer = payer,
                ipg_PaymentDate = paymentDate,
                ipg_CheckNumber = checkNumber,
                ipg_ClaimResponseBatch = batch.ToEntityReference(),
                ipg_IsSecondaryCarrier = ipg_IsSecondaryCarrier
                //todo: claim response id
            };
            orgService.Create(newTransaction);
            UpdateClaimStatus(payment, orgService);
        }

        private void InsertWriteoffTransaction(ipg_payment payment, Incident incident, string networkName,
            double? payorWriteoff, double? patientWriteoff, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber, ipg_claimresponsebatch batch, bool ipg_IsSecondaryCarrier)
        {
            var newTransaction = new ipg_GLTransaction()
            {
                ipg_TransactionType = "A",
                ipg_name = incident.Title,
                ipg_NetworkType = networkName,
                ipg_PayorWriteOff = new Money(Convert.ToDecimal(payorWriteoff)),
                ipg_PatientWriteOff = new Money(Convert.ToDecimal(patientWriteoff)),
                ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                ipg_Payer = payer,
                ipg_PaymentDate = paymentDate,
                ipg_CheckNumber = checkNumber,
                ipg_ClaimResponseBatch = batch.ToEntityReference(),
                ipg_IsSecondaryCarrier = ipg_IsSecondaryCarrier
                //todo: claim response id
            };
            orgService.Create(newTransaction);
            UpdateClaimStatus(payment, orgService);
        }

        private void InsertInterestTransaction(ipg_payment payment, Incident incident, string networkName,
            Money interestPayment, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber, ipg_claimresponsebatch batch, bool ipg_IsSecondaryCarrier)
        {
            var newTransaction = new ipg_GLTransaction()
            {
                ipg_TransactionType = "C",
                ipg_name = incident.Title,
                ipg_NetworkType = networkName,
                ipg_PayorCashAmount = interestPayment,
                ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                ipg_Description = "Interest",
                ipg_Payer = payer,
                ipg_PaymentDate = paymentDate,
                ipg_CheckNumber = checkNumber,
                ipg_ClaimResponseBatch = batch.ToEntityReference(),
                ipg_IsSecondaryCarrier = ipg_IsSecondaryCarrier
                //todo: claim response id
            };
            orgService.Create(newTransaction);
            UpdateClaimStatus(payment, orgService);
        }

        private void InsertInterestAdjustmentTransaction(ipg_payment payment, Incident incident, string networkName,
            decimal? interestAdjustment, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber, ipg_claimresponsebatch batch, bool ipg_IsSecondaryCarrier)
        {
            var newTransaction = new ipg_GLTransaction()
            {
                ipg_TransactionType = "A",
                ipg_name = incident.Title,
                ipg_NetworkType = networkName,
                ipg_PayorAdjustment = new Money(Convert.ToDecimal(interestAdjustment)),
                ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                ipg_Description = "Interest",
                ipg_Payer = payer,
                ipg_PaymentDate = paymentDate,
                ipg_CheckNumber = checkNumber,
                ipg_ClaimResponseBatch = batch.ToEntityReference(),
                ipg_IsSecondaryCarrier = ipg_IsSecondaryCarrier
                //todo: claim response id
            };
            orgService.Create(newTransaction);
            UpdateClaimStatus(payment, orgService);
        }

        private void UpdateClaimStatus(ipg_payment payment, IOrganizationService orgService)
        {
            if (payment.ipg_Claim != null)
            {
                var queryExpression = new QueryExpression("ipg_claimconfiguration")
                {
                    ColumnSet = new ColumnSet("ipg_claimstatus", "ipg_claimreason"),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression("ipg_claimevent", ConditionOperator.Equal, 427880004)
                        }
                    }
                };
                EntityCollection claimConfigurations = orgService.RetrieveMultiple(queryExpression);
                if (claimConfigurations.Entities.Count > 0)
                {
                    Invoice invoice = new Invoice();
                    invoice.Id = payment.ipg_Claim.Id;
                    invoice.ipg_Status = claimConfigurations.Entities[0].GetAttributeValue<OptionSetValue>("ipg_claimstatus");
                    invoice.ipg_Reason = claimConfigurations.Entities[0].GetAttributeValue<OptionSetValue>("ipg_claimreason");
                    orgService.Update(invoice);
                }

            }
        }

        private ipg_claimresponsebatch DetermineBatch(ipg_payment payment, IOrganizationService orgService, ITracingService tracingService)
        {

            tracingService.Trace("Determine Payer");
            if (payment.ipg_ClaimResponseHeader == null)
            {
                return null;
            }

            tracingService.Trace("Retrieving the related Claim Response Batch");
            var crmContext = new OrganizationServiceContext(orgService);
            List<ipg_claimresponsebatch> payers = (from batch in crmContext.CreateQuery<ipg_claimresponsebatch>()
                                   join header in crmContext.CreateQuery<ipg_claimresponseheader>()
                                   on batch.Id equals header.ipg_ClaimResponseBatchId.Id
                                   where header.ipg_claimresponseheaderId == payment.ipg_ClaimResponseHeader.Id
                                   select batch).ToList();
            if(payers.Count > 0)
            {
                return payers.First();
            }
            else
            {
                return null;
            }

        }

    }
}
