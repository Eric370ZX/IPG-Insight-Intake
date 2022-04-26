using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Adjustment
{
    public class InsertGLTransactionOnAdjustmentCreation : PluginBase
    {
        public InsertGLTransactionOnAdjustmentCreation() : base(typeof(InsertGLTransactionOnAdjustmentCreation))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_adjustment.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var orgService = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            tracingService.Trace($"{typeof(InsertGLTransactionOnAdjustmentCreation)} plugin started");

            tracingService.Trace($"Getting Target input parameter ({nameof(ipg_adjustment)} entity)");
            var adjustment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_adjustment>();
            if (adjustment.LogicalName != ipg_adjustment.EntityLogicalName)
            {
                tracingService.Trace($"Target entity is not {nameof(ipg_adjustment)}. Return");
                return;
            }

            if ((adjustment.ipg_AdjustmentType.Value != (int)ipg_AdjustmentTypes.WriteOff) && (adjustment.ipg_AdjustmentType.Value != (int)ipg_AdjustmentTypes.SalesAdjustment) && (adjustment.ipg_AdjustmentType.Value != (int)ipg_AdjustmentTypes.TransferofPayment) && (adjustment.ipg_AdjustmentType.Value != (int)ipg_AdjustmentTypes.TransferofResponsibility))
            {
                tracingService.Trace("Adjustment Type is not appropriate. Return");
                return;
            }

            if (adjustment.ipg_CaseId == null)
            {
                throw new Exception("Adjustment must have an Incident reference");
            }

            tracingService.Trace("Retrieving the related Incident");
            Incident incident = orgService.Retrieve(Incident.EntityLogicalName, adjustment.ipg_CaseId.Id,
                new ColumnSet(
                        nameof(Incident.ipg_CarrierId).ToLower(),
                        nameof(Incident.ipg_SecondaryCarrierId).ToLower(),
                        nameof(Incident.ipg_FacilityId).ToLower(),
                        nameof(Incident.Title).ToLower()
                    )).ToEntity<Incident>();
            if (incident == null)
            {
                throw new Exception($"Could not find the requested Case (Incident.id={adjustment.ipg_CaseId.Id})");
            }

            Incident incidentNew = null;
            string networkNameNew = "";
            if ((adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.TransferofPayment) && (adjustment.ipg_TransferofPaymentType == false))
            {
                incidentNew = orgService.Retrieve(Incident.EntityLogicalName, adjustment.ipg_ToCase.Id,
                new ColumnSet(
                        nameof(Incident.ipg_CarrierId).ToLower(),
                        nameof(Incident.ipg_SecondaryCarrierId).ToLower(),
                        nameof(Incident.ipg_FacilityId).ToLower(),
                        nameof(Incident.Title).ToLower()
                    )).ToEntity<Incident>();
                if (incidentNew == null)
                {
                    throw new Exception($"Could not find the requested Case (Incident.id={adjustment.ipg_ToCase.Id})");
                }
                networkNameNew = DetermineNetworkName(incidentNew, orgService, tracingService);
            }

            string networkName = DetermineNetworkName(incident, orgService, tracingService);
            string payer = "";
            DateTime? paymentDate = adjustment.CreatedOn;
            string checkNumber = "";

            InsertTransactions(adjustment, incident, networkName, orgService, tracingService, payer, paymentDate, checkNumber, incidentNew, networkNameNew);

            tracingService.Trace($"{typeof(InsertGLTransactionOnAdjustmentCreation)} plugin finished");
        }

        private string DetermineNetworkName(Incident incident, IOrganizationService orgService, ITracingService tracingService)
        {
            //todo: this method can be replaced with a Network field on Case

            tracingService.Trace("Determine Network Name");

            if (incident.ipg_CarrierId == null)
            {
                throw new Exception("Case (Incident) must have a Carrier reference");
            }
            if (incident.ipg_FacilityId == null)
            {
                throw new Exception("Case (Incident) must have a Facility reference");
            }

            string carrierField = nameof(incident.ipg_CarrierId).ToLower();

            tracingService.Trace("Retrieving the related Carrier");
            Intake.Account carrier = orgService.Retrieve(Intake.Account.EntityLogicalName, ((EntityReference)incident[carrierField]).Id,
                new ColumnSet(
                    nameof(Intake.Account.ipg_contract).ToLower(),
                    Intake.Account.Fields.ipg_CarrierType,
                    nameof(Intake.Account.ipg_carriernetworkid).ToLower()
                )).ToEntity<Intake.Account>();
            if (carrier == null)
            {
                throw new Exception($"Could not find the requested Carrier (Account.id={((EntityReference)incident[carrierField]).Id})");
            }

            var networksRetrievalResult = GetCarrierNetworks(incident, carrier, orgService, tracingService);
            IEnumerable<ipg_carriernetwork> networks = networksRetrievalResult.Entities.ToList().Select(e => e.ToEntity<ipg_carriernetwork>());

            tracingService.Trace("Selecting a Carrier Network");
            foreach (var network in networks)
            {
                if (network.ipg_CarrierAssignmentOnly == true)
                {
                    if (carrier.ipg_carriernetworkid != null)
                    {
                        var carrierNetwork = networks.FirstOrDefault(n => n.Id == carrier.ipg_carriernetworkid.Id);
                        if (carrierNetwork != null)
                        {
                            return carrierNetwork.ipg_AbbreviatedNameforGP;
                        }
                    }
                }
                else
                {
                    if (network.ipg_ContractedPayorsOnly == true)
                    {
                        if (carrier.ipg_contract == true)
                        {
                            return network.ipg_AbbreviatedNameforGP;
                        }
                    }
                    else
                    {
                        return network.ipg_AbbreviatedNameforGP;
                    }
                }
            }

            return null;
        }

        private EntityCollection GetCarrierNetworks(Incident incident, Intake.Account carrier, IOrganizationService orgService, ITracingService tracingService)
        {
            if (carrier.ipg_CarrierType == null)
            {
                throw new Exception("Carrier SupportedPlanTypes cannot be null");
            }

            tracingService.Trace("Retrieving the related Facility");
            Intake.Account facility = orgService.Retrieve(Intake.Account.EntityLogicalName, incident.ipg_FacilityId.Id,
                new ColumnSet(
                    nameof(facility.ipg_StateId).ToLower()
                )).ToEntity<Intake.Account>();
            if (facility == null)
            {
                throw new Exception($"Could not find the requested Facility (Account.id={incident.ipg_FacilityId.Id})");
            }
            if (facility.ipg_StateId == null)
            {
                throw new Exception("Facility must have a State reference");
            }

            tracingService.Trace("Building QueryExpression to retrieve Carrier Networks");
            var queryExpression = new QueryExpression(ipg_carriernetwork.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(
                    nameof(ipg_carriernetwork.ipg_AbbreviatedNameforGP).ToLower()
                    ),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(
                            nameof(ipg_carriernetwork.StateCode).ToLower(),
                            ConditionOperator.Equal, 0) //0 means active
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        ipg_carriernetwork.EntityLogicalName,
                        ipg_ipg_carriernetwork_ipg_state.EntityLogicalName,
                        nameof(ipg_carriernetwork.ipg_carriernetworkId).ToLower(),
                        nameof(ipg_ipg_carriernetwork_ipg_state.ipg_carriernetworkid).ToLower(),
                        JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(nameof(ipg_ipg_carriernetwork_ipg_state.ipg_ipg_carriernetwork_ipg_stateId).ToLower()),
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression(
                                    nameof(ipg_ipg_carriernetwork_ipg_state.ipg_stateid).ToLower(),
                                    ConditionOperator.Equal, facility.ipg_StateId.Id)
                            }
                        }
                    }
                }
            };

            //add plan type filters

            queryExpression.Criteria.Conditions.Add(new ConditionExpression(
                nameof(ipg_carriernetwork.ipg_plantype).ToLower(),
                ConditionOperator.ContainValues,
                carrier.ipg_CarrierType.Value));

            tracingService.Trace("Retrieving Carrier Networks");
            return orgService.RetrieveMultiple(queryExpression);
        }

        private void InsertTransactions(ipg_adjustment adjustment, Incident incident, string networkName,
            IOrganizationService orgService, ITracingService tracingService, string payer, DateTime? paymentDate, string checkNumber, Incident incidentNew, string networkNameNew)
        {
            double amount = (double)(adjustment.ipg_AmountToApply == null ? 0 : adjustment.ipg_AmountToApply.Value);
            if ((amount != 0) && (adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.WriteOff))
            {
                tracingService.Trace("Inserting a WriteOff transaction");
                if (adjustment.ipg_ApplyTo.Value == (int)ipg_PayerType.Patient)
                {
                    InsertWriteoffTransaction(adjustment, incident, networkName, 0, amount, orgService, payer, paymentDate, checkNumber);
                }
                else if ((adjustment.ipg_ApplyTo.Value == (int)ipg_PayerType.PrimaryCarrier) || (adjustment.ipg_ApplyTo.Value == (int)ipg_PayerType.SecondaryCarrier))
                {
                    InsertWriteoffTransaction(adjustment, incident, networkName, amount, 0, orgService, payer, paymentDate, checkNumber);
                }
            }
            else if ((amount != 0) && (adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.SalesAdjustment))
            {
                if (adjustment.ipg_ApplyTo.Value == (int)ipg_PayerType.Patient)
                {
                    InsertSalesAdjustmentTransaction(adjustment, incident, networkName, 0, -amount, orgService, payer, paymentDate, checkNumber);
                }
                else if ((adjustment.ipg_ApplyTo.Value == (int)ipg_PayerType.PrimaryCarrier) || (adjustment.ipg_ApplyTo.Value == (int)ipg_PayerType.SecondaryCarrier))
                {
                    InsertSalesAdjustmentTransaction(adjustment, incident, networkName, -amount, 0, orgService, payer, paymentDate, checkNumber);
                }
            }
            else if ((adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.TransferofPayment) && (adjustment.ipg_TransferofPaymentType == false))
            {
                TransferTransactionsToAnotherCase(adjustment, incident, networkName, adjustment.ipg_Payment, orgService, payer, paymentDate, checkNumber, incidentNew, networkNameNew);
            }
            else if ((adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.TransferofPayment) && (adjustment.ipg_TransferofPaymentType == true))
            {
                TransferTransactionsToAnotherPayer(adjustment, incident, networkName, adjustment.ipg_Payment, orgService, payer, paymentDate, checkNumber);
            }
            else if ((amount != 0) && (adjustment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.TransferofResponsibility))
            {
                InserTransferOfResponsibilityTransaction(adjustment, incident, networkName, -amount, orgService, payer, paymentDate, checkNumber);
            }

        }

        private void InsertWriteoffTransaction(ipg_adjustment adjustment, Incident incident, string networkName,
            double? payorWriteoff, double? patientWriteoff, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber)
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
                ipg_ClaimResponseBatch = null,
                ipg_IsSecondaryCarrier = (adjustment.ipg_ApplyTo.Value == (int)ipg_PayerType.SecondaryCarrier)
            };
            orgService.Create(newTransaction);
        }

        private void InsertSalesAdjustmentTransaction(ipg_adjustment adjustment, Incident incident, string networkName,
            double? payorSalesAdjustment, double? patientSalesAdjustment, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber)
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
                ipg_ClaimResponseBatch = null,
                ipg_IsSecondaryCarrier = (adjustment.ipg_ApplyTo.Value == (int)ipg_PayerType.SecondaryCarrier)
            };
            orgService.Create(newTransaction);
        }

        private void TransferTransactionsToAnotherCase(ipg_adjustment adjustment, Incident incident, string networkName,
            EntityReference paymentRef, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber, Incident incidentNew, string networkNameNew)
        {
            ipg_payment payment = orgService.Retrieve(paymentRef.LogicalName, paymentRef.Id, new ColumnSet(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower(), nameof(ipg_payment.ipg_InterestPayment).ToLower(), nameof(ipg_payment.ipg_MemberPaid_new).ToLower())).ToEntity<ipg_payment>();
            bool isSecondaryCarrierPayment = IsSecondaryCarrierPayment(orgService, paymentRef);
            double? payorCash = payment.GetAttributeValue<double>(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower());
            if (payorCash.HasValue && payorCash != 0)
            {
                InsertCashTransaction(incident, networkName, -payorCash, 0, orgService, payer, paymentDate, checkNumber, "", isSecondaryCarrierPayment);
                InsertCashTransaction(incidentNew, networkNameNew, payorCash, 0, orgService, payer, paymentDate, checkNumber, "", isSecondaryCarrierPayment);
            }

            Money interestPayment = payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_InterestPayment).ToLower());
            if (interestPayment != null && interestPayment.Value != 0)
            {
                InsertCashTransaction(incident, networkName, (double)-interestPayment.Value, 0, orgService, payer, paymentDate, checkNumber, "Interest", isSecondaryCarrierPayment);
                InsertCashTransaction(incidentNew, networkNameNew, (double)interestPayment.Value, 0, orgService, payer, paymentDate, checkNumber, "Interest", isSecondaryCarrierPayment);
            }

            decimal? memberPaid = (payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_MemberPaid_new).ToLower()) == null ? 0 : payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_MemberPaid_new).ToLower()).Value);
            if (memberPaid.HasValue && memberPaid != 0)
            {
                InsertCashTransaction(incident, networkName, 0, (double)-memberPaid, orgService, payer, paymentDate, checkNumber, "", isSecondaryCarrierPayment);
                InsertCashTransaction(incidentNew, networkNameNew, 0, (double)memberPaid, orgService, payer, paymentDate, checkNumber, "", isSecondaryCarrierPayment);
            }
        }

        private void TransferTransactionsToAnotherPayer(ipg_adjustment adjustment, Incident incident, string networkName,
            EntityReference paymentRef, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber)
        {
            ipg_payment payment = orgService.Retrieve(paymentRef.LogicalName, paymentRef.Id, new ColumnSet(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower(), nameof(ipg_payment.ipg_InterestPayment).ToLower(), nameof(ipg_payment.ipg_MemberPaid_new).ToLower())).ToEntity<ipg_payment>();

            if (((adjustment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier) || (adjustment.ipg_From.Value == (int)ipg_PayerType.SecondaryCarrier)) && (adjustment.ipg_To.Value == (int)ipg_PayerType.Patient))
            {
                double? payorCash = payment.GetAttributeValue<double>(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower());
                if (payorCash.HasValue && payorCash != 0)
                {
                    InsertCashTransaction(incident, networkName, -payorCash, 0, orgService, payer, paymentDate, checkNumber, "", (adjustment.ipg_From.Value == (int)ipg_PayerType.SecondaryCarrier));
                    InsertCashTransaction(incident, networkName, 0, payorCash, orgService, payer, paymentDate, checkNumber);
                }

                Money interestPayment = payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_InterestPayment).ToLower());
                if (interestPayment != null && interestPayment.Value != 0)
                {
                    InsertCashTransaction(incident, networkName, (double)-interestPayment.Value, 0, orgService, payer, paymentDate, checkNumber, "Interest", (adjustment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier));
                    InsertCashTransaction(incident, networkName, 0, (double)interestPayment.Value, orgService, payer, paymentDate, checkNumber, "Interest");
                }
            }

            else if (((adjustment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier) && (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier)) || ((adjustment.ipg_From.Value == (int)ipg_PayerType.SecondaryCarrier) && (adjustment.ipg_To.Value == (int)ipg_PayerType.PrimaryCarrier)))
            {
                double? payorCash = payment.GetAttributeValue<double>(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower());
                if (payorCash.HasValue && payorCash != 0)
                {
                    InsertCashTransaction(incident, networkName, -payorCash, 0, orgService, payer, paymentDate, checkNumber, "", (adjustment.ipg_From.Value == (int)ipg_PayerType.SecondaryCarrier));
                    InsertCashTransaction(incident, networkName, payorCash, 0, orgService, payer, paymentDate, checkNumber, "", (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier));
                }

                Money interestPayment = payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_InterestPayment).ToLower());
                if (interestPayment != null && interestPayment.Value != 0)
                {
                    InsertCashTransaction(incident, networkName, (double)-interestPayment.Value, 0, orgService, payer, paymentDate, checkNumber, "Interest", (adjustment.ipg_From.Value == (int)ipg_PayerType.SecondaryCarrier));
                    InsertCashTransaction(incident, networkName, (double)interestPayment.Value, 0, orgService, payer, paymentDate, checkNumber, "Interest", (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier));
                }
            }

            else if ((adjustment.ipg_From.Value == (int)ipg_PayerType.Patient) && ((adjustment.ipg_To.Value == (int)ipg_PayerType.PrimaryCarrier) || (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier)))
            {
                decimal? memberPaid = (payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_MemberPaid_new).ToLower()) == null ? 0 : payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_MemberPaid_new).ToLower()).Value);
                if (memberPaid.HasValue && memberPaid != 0)
                {
                    InsertCashTransaction(incident, networkName, 0, (double)-memberPaid, orgService, payer, paymentDate, checkNumber);
                    InsertCashTransaction(incident, networkName, (double)memberPaid, 0, orgService, payer, paymentDate, checkNumber, "", (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier));
                }

                Money interestPayment = payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_InterestPayment).ToLower());
                if (interestPayment != null && interestPayment.Value != 0)
                {
                    InsertCashTransaction(incident, networkName, 0, (double)-interestPayment.Value, orgService, payer, paymentDate, checkNumber, "Interest");
                    InsertCashTransaction(incident, networkName, (double)interestPayment.Value, 0, orgService, payer, paymentDate, checkNumber, "Interest", (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier));
                }
            }
        }

        private void InsertCashTransaction(Incident incident, string networkName,
            double? payorCashAmount, double? patientCashAmount, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber, string description = "", bool isSecondaryCarrier = false)
        {
            var newTransaction = new ipg_GLTransaction()
            {
                ipg_TransactionType = "C",
                ipg_name = incident.Title,
                ipg_NetworkType = networkName,
                ipg_PayorCashAmount = payorCashAmount != null ? new Money(Convert.ToDecimal(payorCashAmount)) : null,
                ipg_PatientCashAmount = patientCashAmount != null ? new Money(Convert.ToDecimal(patientCashAmount)) : null,
                ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                ipg_Description = description,
                ipg_Payer = payer,
                ipg_PaymentDate = paymentDate,
                ipg_CheckNumber = checkNumber,
                ipg_ClaimResponseBatch = null,
                ipg_IsSecondaryCarrier = isSecondaryCarrier
            };
            orgService.Create(newTransaction);
        }

        private void InserTransferOfResponsibilityTransaction(ipg_adjustment adjustment, Incident incident, string networkName,
            double? amount, IOrganizationService orgService, string payer, DateTime? paymentDate, string checkNumber)
        {
            var newTransaction = new ipg_GLTransaction()
            {
                ipg_TransactionType = "A",
                ipg_name = incident.Title,
                ipg_NetworkType = networkName,
                ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                ipg_Payer = payer,
                ipg_PaymentDate = paymentDate,
                ipg_CheckNumber = checkNumber,
                ipg_ClaimResponseBatch = null,
                ipg_IsSecondaryCarrier = false
            };
            if(((adjustment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier) || (adjustment.ipg_From.Value == (int)ipg_PayerType.SecondaryCarrier)) && (adjustment.ipg_To.Value == (int)ipg_PayerType.Patient))
            {
                newTransaction.ipg_PayorAdjustment = new Money(Convert.ToDecimal(amount));
                newTransaction.ipg_PatientAdjustment = new Money(Convert.ToDecimal(-amount));
                newTransaction.ipg_IsSecondaryCarrier = (adjustment.ipg_From.Value == (int)ipg_PayerType.SecondaryCarrier);
                orgService.Create(newTransaction);
            }
            else if ((adjustment.ipg_From.Value == (int)ipg_PayerType.Patient) && ((adjustment.ipg_To.Value == (int)ipg_PayerType.PrimaryCarrier) || (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier)))
            {
                newTransaction.ipg_PayorAdjustment = new Money(Convert.ToDecimal(-amount));
                newTransaction.ipg_PatientAdjustment = new Money(Convert.ToDecimal(amount));
                newTransaction.ipg_IsSecondaryCarrier = (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier);
                orgService.Create(newTransaction);
            }
            else if ((adjustment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier) && (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier))
            {
                newTransaction.ipg_PayorAdjustment = new Money(Convert.ToDecimal(amount));
                orgService.Create(newTransaction);
            }
            else if ((adjustment.ipg_From.Value == (int)ipg_PayerType.SecondaryCarrier) && (adjustment.ipg_To.Value == (int)ipg_PayerType.PrimaryCarrier))
            {
                newTransaction.ipg_PayorAdjustment = new Money(Convert.ToDecimal(-amount));
                orgService.Create(newTransaction);
            }

            if (((adjustment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier) && (adjustment.ipg_To.Value == (int)ipg_PayerType.SecondaryCarrier)) ||
                    ((adjustment.ipg_From.Value == (int)ipg_PayerType.SecondaryCarrier) && (adjustment.ipg_To.Value == (int)ipg_PayerType.PrimaryCarrier)))
            {
                newTransaction = new ipg_GLTransaction()
                {
                    ipg_TransactionType = "A",
                    ipg_name = incident.Title,
                    ipg_NetworkType = networkName,
                    ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                    ipg_Payer = payer,
                    ipg_PaymentDate = paymentDate,
                    ipg_CheckNumber = checkNumber,
                    ipg_ClaimResponseBatch = null,
                    ipg_IsSecondaryCarrier = true
                };
                newTransaction.ipg_PayorAdjustment = new Money(Convert.ToDecimal((adjustment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier) ? -amount : amount));
                orgService.Create(newTransaction);
            }
        }

        private bool IsSecondaryCarrierPayment(IOrganizationService orgService, EntityReference paymentRef)
        {
            ipg_payment payment = orgService.Retrieve(paymentRef.LogicalName, paymentRef.Id, new ColumnSet(nameof(ipg_payment.ipg_Claim).ToLower(), nameof(ipg_payment.ipg_CaseId).ToLower())).ToEntity<ipg_payment>();
            EntityReference claimRef = payment.GetAttributeValue<EntityReference>(nameof(ipg_payment.ipg_Claim).ToLower());
            EntityReference caseRef = payment.GetAttributeValue<EntityReference>(nameof(ipg_payment.ipg_CaseId).ToLower());
            if (claimRef != null && caseRef != null)
            {
                Incident incident = orgService.Retrieve(caseRef.LogicalName, caseRef.Id, new ColumnSet(nameof(Incident.ipg_SecondaryCarrier).ToLower(), nameof(Incident.ipg_SecondaryCarrierId).ToLower())).ToEntity<Incident>();
                if (incident.GetAttributeValue<bool>(nameof(Incident.ipg_SecondaryCarrier).ToLower()) && (incident.GetAttributeValue<EntityReference>(nameof(Incident.ipg_SecondaryCarrierId).ToLower()) != null))
                {
                    Invoice claim = orgService.Retrieve(claimRef.LogicalName, claimRef.Id, new ColumnSet(nameof(Invoice.CustomerId).ToLower())).ToEntity<Invoice>();
                    return Guid.Equals(incident.GetAttributeValue<EntityReference>(nameof(Incident.ipg_SecondaryCarrierId).ToLower()).Id, claim.GetAttributeValue<EntityReference>(nameof(Invoice.CustomerId).ToLower()).Id);
                }
            }
            return false;
        }
    }
}