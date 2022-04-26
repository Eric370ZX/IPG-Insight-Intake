using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Insight.Intake.Helpers;
using static Insight.Intake.Plugin.Managers.TaskManager;
using Insight.Intake.Plugin.Managers;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.Payment
{
    public class ProcessPayment : PluginBase
    {
        private IOrganizationService service;
        private ITracingService tracingService;
        private ipg_payment payment;

        public ProcessPayment() : base(typeof(ProcessPayment))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_payment.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            service = localPluginContext.OrganizationService;
            tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(ProcessPayment)} plugin started");

            PaymentManager paymentManager = new PaymentManager(service, tracingService);
            var taskManager = new TaskManager(service, tracingService);

            payment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_payment>();
            int headerType = paymentManager.GetHeaderType(payment);
            decimal paymentAmount = paymentManager.GetPaymentAmount(payment);
            bool isManualBatch = paymentManager.isManualBatch(payment);
            bool isAutoCarrierPayment = paymentManager.IsAutoCarrierPayment(payment);
            bool isSecondaryCarrierPayment = paymentManager.IsSecondaryCarrierPayment(payment);
            var claim = payment.ipg_Claim != null ? (Invoice)service?.Retrieve(Invoice.EntityLogicalName, payment.ipg_Claim.Id, new ColumnSet(Invoice.Fields.InvoiceNumber, Invoice.Fields.ipg_icn)) : null;
            var isPostedByCarrier = paymentManager.isPostedByCarrier(payment);

            UpdateClaimLineItems();
            UpdateClaimStatus(headerType, paymentAmount);
            UpdateAdjustmentRemarkCodesDescription();
            TransferResponsibility(headerType, paymentAmount, isManualBatch, isSecondaryCarrierPayment, taskManager);
            ProcessICN(claim);
            UpdateCaseStatus(headerType);
            UpdateHeaderStatus();
            CreateNote(headerType);
            CreateTask(headerType, isAutoCarrierPayment, paymentManager, taskManager, isPostedByCarrier);
            UpdatePendingAdjustmentTask();
            UpdatePatientOutreachTasks(payment, paymentManager, service);
        }
        private void ProcessICN(Invoice claim)
        {
            if (claim != null)
            {
                if (string.IsNullOrEmpty(claim.ipg_icn))
                {
                    CreateUpdateICNTask(payment.ipg_CaseId, TaskTypeIds.MISSING_ICN_VALUE, claim.InvoiceNumber);
                }
                else UpdateICNOnClaim(claim);
            }
        }

        private void CreateUpdateICNTask(EntityReference incident, TaskTypeIds taskTypeId, string claimNumber)
        {
            var taskManager = new TaskManager(service, tracingService);
            var taskType = taskManager.GetTaskTypeById(taskTypeId);
            var taskTempalte = new Task() { Description = string.Format(taskType.ipg_description, claimNumber) };
            taskManager.CreateTask(incident, taskType.ToEntityReference(), taskTempalte);
        }

        private void UpdateICNOnClaim(Invoice claim)
        {
            if (payment.ipg_icn != claim.ipg_icn)
            {
                service.Update(new Invoice
                {
                    Id = claim.Id,
                    ipg_icn = payment.ipg_icn
                });
            }
        }

        private void UpdateClaimLineItems()
        {
            tracingService.Trace("Update Claim Line Items");
            var headerRef = payment.ipg_ClaimResponseHeader;
            var claimRef = payment.ipg_Claim;
            if (headerRef == null || claimRef == null)
            {
                tracingService.Trace("The payment contains an empty header or claim");
                return;
            }

            var header = service.Retrieve(headerRef.LogicalName, headerRef.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_claimresponseheaderId).ToLower(), nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower())).ToEntity<ipg_claimresponseheader>();
            var batch = service.Retrieve(header.ipg_ClaimResponseBatchId.LogicalName, header.ipg_ClaimResponseBatchId.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_IsManualBatch).ToLower())).ToEntity<ipg_claimresponsebatch>();

            QueryExpression queryExpression = new QueryExpression(ipg_claimresponseline.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_claimresponseline.ipg_claimresponselineId).ToLower(), nameof(ipg_claimresponseline.ipg_AdjProc).ToLower(), nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower(), nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower(), nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower(), nameof(ipg_claimresponseline.ipg_PaidUnits).ToLower(), nameof(ipg_claimresponseline.ipg_DenialCodeString).ToLower(), nameof(ipg_claimresponseline.ipg_RemarkCodeString).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(nameof(ipg_claimresponseline.ipg_ClaimResponseHeaderId).ToLower(), ConditionOperator.Equal, headerRef.Id)
                    }
                }
            };
            EntityCollection claimResponseLines = service.RetrieveMultiple(queryExpression);

            queryExpression = new QueryExpression(ipg_claimlineitem.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower(), nameof(ipg_claimlineitem.ipg_hcpcsid).ToLower(), nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower(), nameof(ipg_claimlineitem.ipg_allowed).ToLower(), nameof(ipg_claimlineitem.ipg_paid).ToLower(), nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower(), nameof(ipg_claimlineitem.ipg_billedchg).ToLower(), nameof(ipg_claimlineitem.ipg_quantity).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(nameof(ipg_claimlineitem.ipg_claimid).ToLower(), ConditionOperator.Equal, claimRef.Id)
                    }
                }
            };
            EntityCollection claimLineItems = service.RetrieveMultiple(queryExpression);

            List<Guid> claimLineItemsList = new List<Guid>();
            foreach (Entity claimReponseLine in claimResponseLines.Entities)
            {
                IEnumerable<Entity> claimLineItemsHCPCS = claimLineItems.Entities.Where(claimLineItem => service.Retrieve(ipg_masterhcpcs.EntityLogicalName, claimLineItem.GetAttributeValue<EntityReference>(nameof(ipg_claimlineitem.ipg_hcpcsid).ToLower()).Id, new ColumnSet(nameof(ipg_masterhcpcs.ipg_name).ToLower())).GetAttributeValue<string>(nameof(ipg_masterhcpcs.ipg_name).ToLower()).ToLower() == claimReponseLine.GetAttributeValue<string>(nameof(ipg_claimresponseline.ipg_AdjProc).ToLower()).ToLower());
                if (claimLineItemsHCPCS.Count() == 1)
                {
                    Entity corrClaimLineItem = claimLineItemsHCPCS.FirstOrDefault();
                    UpdateClaimLineItem(corrClaimLineItem, claimReponseLine, batch.ipg_IsManualBatch);
                    continue;
                }
                else if (claimLineItemsHCPCS.Count() > 1)
                {
                    claimLineItemsHCPCS = claimLineItems.Entities.Where(claimLineItem => (claimLineItem.GetAttributeValue<EntityReference>(nameof(ipg_claimlineitem.ipg_hcpcsid).ToLower()).Name.ToLower() == claimReponseLine.GetAttributeValue<string>(nameof(ipg_claimresponseline.ipg_AdjProc).ToLower()).ToLower()) && ((claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_billedchg).ToLower()) == null ? 0 : claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_billedchg).ToLower()).Value) == (claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()).Value)));
                    if (claimLineItemsHCPCS.Count() == 1)
                    {
                        Entity corrClaimLineItem = claimLineItemsHCPCS.FirstOrDefault();
                        UpdateClaimLineItem(corrClaimLineItem, claimReponseLine, batch.ipg_IsManualBatch);
                        continue;
                    }
                }

                claimLineItemsHCPCS = claimLineItems.Entities.Where(claimLineItem => (claimLineItem.GetAttributeValue<decimal>(nameof(ipg_claimlineitem.ipg_quantity).ToLower()) == claimReponseLine.GetAttributeValue<int>(nameof(ipg_claimresponseline.ipg_PaidUnits).ToLower())) && ((claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_billedchg).ToLower()) == null ? 0 : claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_billedchg).ToLower()).Value) == (claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()).Value)));
                if (claimLineItemsHCPCS.Count() == 1)
                {
                    Entity corrClaimLineItem = claimLineItemsHCPCS.FirstOrDefault();
                    UpdateClaimLineItem(corrClaimLineItem, claimReponseLine, batch.ipg_IsManualBatch);
                }
                else if (claimLineItemsHCPCS.Count() > 1)
                {
                    foreach (Entity claimLineItemEntity in claimLineItemsHCPCS)
                    {
                        if (claimLineItemsList.FindIndex(claimLineItem => claimLineItem.Equals(claimLineItemEntity.GetAttributeValue<Guid>(nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower()))) < 0)
                        {
                            UpdateClaimLineItem(claimLineItemEntity, claimReponseLine, batch.ipg_IsManualBatch);
                            claimLineItemsList.Add(claimLineItemEntity.GetAttributeValue<Guid>(nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower()));
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateClaimLineItem(Entity corrClaimLineItem, Entity claimReponseLine, bool? manualBatch)
        {
            ipg_claimlineitem cli = new ipg_claimlineitem();
            cli.Id = corrClaimLineItem.Id;
            if (manualBatch ?? false)
            {
                cli.ipg_allowed = new Money(claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()).Value);
                cli.ipg_paid = new Money(claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()).Value);
            }
            else
            {
                cli.ipg_allowed = new Money((corrClaimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_allowed).ToLower()) == null ? 0 : corrClaimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_allowed).ToLower()).Value) + (claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()).Value));
                cli.ipg_paid = new Money((corrClaimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_paid).ToLower()) == null ? 0 : corrClaimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_paid).ToLower()).Value) + (claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()).Value));
            }
            cli.ipg_DenialCodeString = claimReponseLine.GetAttributeValue<string>(nameof(ipg_claimresponseline.ipg_DenialCodeString).ToLower());
            cli.ipg_RemarkCodeString = claimReponseLine.GetAttributeValue<string>(nameof(ipg_claimresponseline.ipg_RemarkCodeString).ToLower());
            service.Update(cli);
        }

        private void UpdateClaimStatus(int headerType, decimal paymentAmount)
        {
            tracingService.Trace("Update Claim Status");
            var claimRef = payment.ipg_Claim;
            if (claimRef == null)
            {
                tracingService.Trace("The payment contains an empty claim");
                return;
            }

            var queryExpression = new QueryExpression(ipg_claimconfiguration.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_claimconfiguration.ipg_ClaimStatus).ToLower(), nameof(ipg_claimconfiguration.ipg_ClaimReason).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(nameof(ipg_claimconfiguration.ipg_ClaimEvent).ToLower(), ConditionOperator.Equal, (int)ipg_ClaimEvent._835fileprocessing),
                        new ConditionExpression(nameof(ipg_claimconfiguration.ipg_ClaimSubEvent).ToLower(), ConditionOperator.Equal, headerType)
                    }
                }
            };
            EntityCollection claimConfigurations = service.RetrieveMultiple(queryExpression);

            Entity claim = service.Retrieve(claimRef.LogicalName, claimRef.Id, new ColumnSet("ipg_paid"));
            decimal prevPaid = (claim.GetAttributeValue<Money>(nameof(Invoice.ipg_Paid).ToLower()) == null ? 0 : claim.GetAttributeValue<Money>(nameof(Invoice.ipg_Paid).ToLower()).Value);

            Invoice invoice = new Invoice();
            invoice.Id = claimRef.Id;
            invoice.ipg_Paid = new Money(prevPaid + paymentAmount);
            if (claimConfigurations.Entities.Count > 0)
            {
                invoice.ipg_Status = claimConfigurations.Entities[0].GetAttributeValue<OptionSetValue>(nameof(ipg_claimconfiguration.ipg_ClaimStatus).ToLower());
                invoice.ipg_Reason = claimConfigurations.Entities[0].GetAttributeValue<OptionSetValue>(nameof(ipg_claimconfiguration.ipg_ClaimReason).ToLower());
            }
            service.Update(invoice);
        }

        private void UpdateAdjustmentRemarkCodesDescription()
        {
            tracingService.Trace("Update Adjustment Remark Codes Description");
            var claimRef = payment.ipg_Claim;
            if (claimRef == null)
            {
                tracingService.Trace("The payment contains an empty claim");
                return;
            }

            var queryExpression = new QueryExpression(ipg_claimlineitem.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_claimlineitem.ipg_DenialCodeString).ToLower(), nameof(ipg_claimlineitem.ipg_RemarkCodeString).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(nameof(ipg_claimlineitem.ipg_claimid).ToLower(), ConditionOperator.Equal, claimRef.Id)
                    }
                }
            };
            EntityCollection claimLineItems = service.RetrieveMultiple(queryExpression);

            List<string> adjustmentCodesList = new List<string>();
            List<string> remarkCodesList = new List<string>();
            foreach (Entity claimLineItem in claimLineItems.Entities)
            {
                string adjustmentCodesString = claimLineItem.GetAttributeValue<string>(nameof(ipg_claimlineitem.ipg_DenialCodeString).ToLower());
                if (adjustmentCodesString != null)
                {
                    string[] adjustmentCodes = adjustmentCodesString.Split(',');
                    foreach (string adjsutmentCode in adjustmentCodes)
                    {
                        if (!adjustmentCodesList.Exists(x => (x == adjsutmentCode.Trim())))
                        {
                            adjustmentCodesList.Add(adjsutmentCode.Trim());
                        }
                    }
                }
                string remarkCodesString = claimLineItem.GetAttributeValue<string>(nameof(ipg_claimlineitem.ipg_RemarkCodeString).ToLower());
                if (remarkCodesString != null)
                {
                    string[] remarkCodes = remarkCodesString.Split(',');
                    foreach (string remarkCode in remarkCodes)
                    {
                        if (!remarkCodesList.Exists(x => (x == remarkCode.Trim())))
                        {
                            remarkCodesList.Add(remarkCode.Trim());
                        }
                    }
                }
            }
            string adjustmentCodesDescription = GetAdjustmentCodesDescription(adjustmentCodesList);
            string remarkCodesDescription = GetRemarkCodesDescription(remarkCodesList);

            Invoice invoice = new Invoice();
            invoice.Id = claimRef.Id;
            invoice.ipg_AdjustmentCodesDescription = adjustmentCodesDescription;
            invoice.ipg_RemarkCodesDescription = remarkCodesDescription;
            service.Update(invoice);
        }

        private string GetAdjustmentCodesDescription(List<string> adjustmentCodes)
        {
            string adjustmentCodesString = "";
            if (adjustmentCodes.Count > 0)
            {
                QueryExpression queryExpression = new QueryExpression(ipg_claimstatuscode.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimstatuscode.ipg_name).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.Or)
                    {
                    }
                };
                foreach (string adjustmentCode in adjustmentCodes)
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_claimstatuscode.ipg_name).ToLower(), ConditionOperator.BeginsWith, adjustmentCode + " "));
                EntityCollection claimStatusCodes = service.RetrieveMultiple(queryExpression);
                List<string> codeNames = new List<string>();
                foreach (Entity claimStatusCode in claimStatusCodes.Entities)
                    codeNames.Add(claimStatusCode.GetAttributeValue<string>(nameof(ipg_claimstatuscode.ipg_name).ToLower()));

                foreach (string adjustmentCode in adjustmentCodes)
                {
                    string codeName = codeNames.FirstOrDefault(x => x.Substring(0, adjustmentCode.Length) == adjustmentCode);
                    adjustmentCodesString += (String.IsNullOrEmpty(adjustmentCodesString) ? "" : "\n") + adjustmentCode;
                    adjustmentCodesString += (String.IsNullOrEmpty(codeName) ? "" : "(" + codeName + ")");
                }
            }
            return adjustmentCodesString;
        }

        private string GetRemarkCodesDescription(List<string> remarkCodes)
        {
            string remarkCodesString = "";
            if (remarkCodes.Count > 0)
            {
                QueryExpression queryExpression = new QueryExpression(ipg_claimresponseremarkcode.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimresponseremarkcode.ipg_name).ToLower(), nameof(ipg_claimresponseremarkcode.ipg_Description).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.Or)
                    {
                    }
                };
                foreach (string remarkCode in remarkCodes)
                {
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_claimresponseremarkcode.ipg_name).ToLower(), ConditionOperator.Equal, remarkCode));
                }
                EntityCollection claimResponseRemarkCodes = service.RetrieveMultiple(queryExpression);
                Dictionary<string, string> codeNames = new Dictionary<string, string>();
                foreach (Entity claimResponseRemarkCode in claimResponseRemarkCodes.Entities)
                {
                    codeNames.Add(claimResponseRemarkCode.GetAttributeValue<string>(nameof(ipg_claimresponseremarkcode.ipg_name).ToLower()), claimResponseRemarkCode.GetAttributeValue<string>(nameof(ipg_claimresponseremarkcode.ipg_Description).ToLower()));
                }

                foreach (string remarkCode in remarkCodes)
                {
                    remarkCodesString += (String.IsNullOrEmpty(remarkCodesString) ? "" : "\n") + remarkCode;
                    if (codeNames.ContainsKey(remarkCode))
                    {
                        remarkCodesString += (String.IsNullOrEmpty(codeNames[remarkCode]) ? "" : "(" + codeNames[remarkCode] + ")");
                    }
                }
            }
            return remarkCodesString;
        }

        private void TransferResponsibility(int headerType, decimal paymentAmount, bool isManualBatch, bool isSecondaryCarrierPayment, TaskManager taskManager)
        {
            tracingService.Trace("Transfer Responsibility");
            if (!isSecondaryCarrierPayment && (headerType == (int)ipg_ClaimSubEvent.FullPatientResponsibility || headerType == (int)ipg_ClaimSubEvent.PatientResponsibilitybuthasSecondary || headerType == (int)ipg_ClaimSubEvent.Partialpatientresponsibility))
            {
                decimal adjustmentAmount = 0;
                Incident incident = service.Retrieve(payment.ipg_CaseId.LogicalName, payment.ipg_CaseId.Id, new ColumnSet(nameof(Incident.ipg_RemainingCarrierBalance).ToLower(), nameof(Incident.ipg_RemainingSecondaryCarrierBalance).ToLower(), nameof(Incident.ipg_RemainingPatientBalance).ToLower(), nameof(Incident.ipg_SecondaryCarrierId).ToLower(), nameof(Incident.ipg_BillToPatient).ToLower())).ToEntity<Incident>();
                if (payment.ipg_ApplyAdjustment ?? false)
                {
                    bool ipg_IsSecondaryCarrier = false;
                    if ((payment.ipg_Claim != null) && (incident.ipg_SecondaryCarrierId != null))
                    {
                        Invoice claim = service.Retrieve(Invoice.EntityLogicalName, payment.ipg_Claim.Id, new ColumnSet(nameof(Invoice.CustomerId).ToLower())).ToEntity<Invoice>();
                        ipg_IsSecondaryCarrier = (claim.CustomerId != null) && claim.CustomerId.Equals(incident.ipg_SecondaryCarrierId);
                    }

                    if ((payment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.SalesAdjustment)
                        || (payment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.WriteOff))
                    {
                        if (!ipg_IsSecondaryCarrier && payment.ipg_ApplyTo.Value == (int)ipg_PayerType.PrimaryCarrier)
                        {
                            adjustmentAmount = (payment.ipg_AmountToApply == null ? 0 : payment.ipg_AmountToApply.Value);
                        }
                    }
                    else if (payment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.TransferofResponsibility)
                    {
                        if (!ipg_IsSecondaryCarrier && payment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier)
                        {
                            adjustmentAmount = (payment.ipg_AmountToApply == null ? 0 : payment.ipg_AmountToApply.Value);
                        }
                    }
                }

                decimal amount = (incident.ipg_RemainingCarrierBalance?.Value ?? 0)- paymentAmount - adjustmentAmount;

                if (amount > 0)
                {
                    if (headerType == (int)ipg_ClaimSubEvent.FullPatientResponsibility && incident.ipg_BillToPatientEnum == ipg_TwoOptions.No)
                    {
                        if (!isManualBatch)
                        {
                            taskManager.CreateTask(payment.ipg_CaseId, TaskTypeIds.BILL_TO_PATIENT_NO);
                        }
                    }
                    else
                    {
                        ipg_adjustment adjustment = new ipg_adjustment()
                        {
                            ipg_CaseId = payment.ipg_CaseId,
                            ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.TransferofResponsibility),
                            ipg_From = new OptionSetValue((int)ipg_PayerType.PrimaryCarrier),
                            ipg_To = (headerType == (int)ipg_ClaimSubEvent.FullPatientResponsibility || headerType == (int)ipg_ClaimSubEvent.Partialpatientresponsibility ? new OptionSetValue((int)ipg_PayerType.Patient) : new OptionSetValue((int)ipg_PayerType.SecondaryCarrier)),
                            ipg_Amount = new Money(amount),
                            ipg_AmountToApply = new Money(amount),
                            ipg_SkipGatingExecution = true
                        };
                        service.Create(adjustment);
                    }
                }
            }
        }

        private void UpdateCaseStatus(int headerType)
        {
            tracingService.Trace("Update Case Status");
            Dictionary<int, int> mapping = new Dictionary<int, int>();
            mapping.Add((int)ipg_ClaimSubEvent.PartialDenialFromCarrier, (int)ipg_CaseReasons.CarrierCollectionsinProgress);
            mapping.Add((int)ipg_ClaimSubEvent.FullPatientResponsibility, (int)ipg_CaseReasons.FullPatientResponsibility);
            mapping.Add((int)ipg_ClaimSubEvent.FullDenialFromCarrier, (int)ipg_CaseReasons.CarrierCollectionsFullDenial);
            mapping.Add((int)ipg_ClaimSubEvent.Partialpatientresponsibility, (int)ipg_CaseReasons.PartialPatientResponsibility);
            mapping.Add((int)ipg_ClaimSubEvent.PatientResponsibilitybuthasSecondary, (int)ipg_CaseReasons.PendingSecondaryClaim);
            mapping.Add((int)ipg_ClaimSubEvent.Recoupment, (int)ipg_CaseReasons.CarrierCollectionsRecoupment);

            if (mapping.ContainsKey(headerType))
            {
                var incident = new Incident();
                incident.Id = payment.ipg_CaseId.Id;
                incident[nameof(incident.ipg_Reasons).ToLower()] = new OptionSetValue(mapping[headerType]);
                service.Update(incident);
            }
        }

        private void UpdateHeaderStatus()
        {
            tracingService.Trace("Update Header Status");
            var headerRef = payment.ipg_ClaimResponseHeader;
            if (payment.ipg_ClaimResponseHeader != null)
            {
                var header = new ipg_claimresponseheader();
                header.Id = payment.ipg_ClaimResponseHeader.Id;
                header.ipg_PostStatus = "posted";
                header.ipg_PostedDate = System.DateTime.Now;
                service.Update(header);
            }
        }

        private void CreateNote(int headerType)
        {
            tracingService.Trace("Create note");
            var claimRef = payment.ipg_Claim;
            if (claimRef == null)
            {
                tracingService.Trace("The payment contains an empty claim");
                return;
            }

            var claim = service.Retrieve(claimRef.LogicalName, claimRef.Id, new ColumnSet(nameof(Invoice.CustomerId).ToLower(), nameof(Invoice.Name).ToLower())).ToEntity<Invoice>();
            Dictionary<int, string> mapping = new Dictionary<int, string>();
            mapping.Add((int)ipg_ClaimSubEvent.PartialDenialFromCarrier, "Claim " + claim.Name + " partially denied by " + claim.GetAttributeValue<EntityReference>(nameof(Invoice.CustomerId).ToLower()).Name + ". Carrier collections process initiated.");
            mapping.Add((int)ipg_ClaimSubEvent.FullDenialFromCarrier, "Claim " + claim.Name + " denied in full. Carrier collections process initiated.");
            mapping.Add((int)ipg_ClaimSubEvent.Partialpatientresponsibility, "Claim " + claim.Name + " Partial Patient Responsibility by " + claim.GetAttributeValue<EntityReference>(nameof(Invoice.CustomerId).ToLower()).Name + ". Patient collections process initiated.");
            mapping.Add((int)ipg_ClaimSubEvent.FullPatientResponsibility, "Claim " + claim.Name + " Full Patient Responsibility by " + claim.GetAttributeValue<EntityReference>(nameof(Invoice.CustomerId).ToLower()).Name + ". Patient collections process initiated.");
            mapping.Add((int)ipg_ClaimSubEvent.Recoupment, "Claim " + claim.Name + " recouped. Carrier collections process initiated.");

            if (mapping.ContainsKey(headerType))
            {
                var annotation = new Annotation();
                annotation.ObjectId = payment.ipg_CaseId;
                annotation.ObjectTypeCode = payment.ipg_CaseId.LogicalName;
                annotation.Subject = "Claim processing";
                annotation.NoteText = mapping[headerType];
                service.Create(annotation);
            }
        }

        private void CreateTask(int headerType, bool isAutoCarrierPayment, PaymentManager paymentManager, TaskManager taskManager, bool isPostedByCarrier)
        {
            tracingService.Trace("Create Task");
            if(!isPostedByCarrier)
            {
                return;
            }
            taskManager.CloseCategoryTasks(payment.ipg_CaseId.Id, TaskCategoryNames.CarrierOutreach);

            var headerRef = payment.ipg_ClaimResponseHeader;
            if (headerRef == null)
            {
                tracingService.Trace("The payment contains an empty header");
                return;
            }

            var mapping = new Dictionary<int, TaskTypeIds>();
            mapping.Add((int)ipg_ClaimSubEvent.PartialDenialFromCarrier, TaskTypeIds.CLAIM_RESPONSE_PARTIAL_DENIAL);
            mapping.Add((int)ipg_ClaimSubEvent.FullDenialFromCarrier, TaskTypeIds.CLAIM_RESPONSE_FULL_DENIAL);

            var claimNumber = string.Empty;
            if(payment.ipg_Claim != null)
            {
                var claim = service?.Retrieve(Invoice.EntityLogicalName, payment.ipg_Claim.Id, new ColumnSet(Invoice.Fields.Name)).ToEntity<Invoice>();
                claimNumber = claim.Name;
            }

            if (!isAutoCarrierPayment && mapping.ContainsKey(headerType))
            {
                var taskType = taskManager.GetTaskTypeById(mapping[headerType]);
                var taskTempalte = new Task() { Description = string.Format(taskType.ipg_description, claimNumber) };
                taskManager.CreateTask(payment.ipg_CaseId, taskType.ToEntityReference(), taskTempalte);
            }
            else if (isAutoCarrierPayment && headerType > 0 && headerType != (int)ipg_ClaimSubEvent.Recoupment && headerType != (int)ipg_ClaimSubEvent.Fullpayment)
            {
                var taskType = taskManager.GetTaskTypeById(TaskTypeIds.REVIEW_PROCESSED_AUTO_CLAIM);
                var taskTempalte = new Task() { Description = string.Format(taskType.ipg_description, claimNumber) };
                taskManager.CreateTask(payment.ipg_CaseId, taskType.ToEntityReference(), taskTempalte);
            }
            if (!isAutoCarrierPayment && (headerType == (int)ipg_ClaimSubEvent.PatientResponsibilitybuthasSecondary) && paymentManager.HasSecondary(payment.ipg_CaseId))
            {
                var taskType = taskManager.GetTaskTypeById(TaskTypeIds.Secondary_Claim_Request);
                var caseNumber = service.Retrieve(Incident.EntityLogicalName, payment.ipg_CaseId.Id, new ColumnSet(Incident.Fields.Title))?.ToEntity<Incident>()?.Title;
                var taskTempalte = new Task() { Description = string.Format(taskType.ipg_description, caseNumber) };
                taskManager.CreateTask(payment.ipg_CaseId, taskType.ToEntityReference(), taskTempalte);
            }
        }

        private void UpdatePendingAdjustmentTask()
        {
            tracingService.Trace("Update Pending Adjustment Task");

            var caseRef = new EntityReference(payment.ipg_CaseId.LogicalName, payment.ipg_CaseId.Id);
            QueryExpression queryExpression = new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(Task.ActivityId).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Task.RegardingObjectId).ToLower(), ConditionOperator.Equal, caseRef.Id),
                            new ConditionExpression(nameof(Task.ipg_tasktypecode).ToLower(), ConditionOperator.Equal, (int)ipg_TaskType1.PendingCarrierorPatientAdjustment),
                            new ConditionExpression(nameof(Task.StateCode).ToLower(), ConditionOperator.Equal, (int)TaskState.Open)
                        }
                }
            };
            EntityCollection tasks = service.RetrieveMultiple(queryExpression);

            foreach (Entity task in tasks.Entities)
            {
                var updTask = new Task();
                updTask.Id = task.Id;
                updTask.ScheduledEnd = DateTime.Today.AddDays(1).AddTicks(-1);
                service.Update(updTask);
            }
        }

        private void UpdatePatientOutreachTasks(ipg_payment payment, PaymentManager paymentManager, IOrganizationService service)
        {
            if (paymentManager.isPostedByCarrier(payment))
            {
                return;
            }

            var incident = service.Retrieve(payment.ipg_CaseId.LogicalName, payment.ipg_CaseId.Id, new ColumnSet(Incident.Fields.ipg_PaymentPlanType)).ToEntity<Incident>();
            if (incident.ipg_PaymentPlanTypeEnum == Incident_ipg_PaymentPlanType.AutoDraft 
                || incident.ipg_PaymentPlanTypeEnum == Incident_ipg_PaymentPlanType.Manual)
            {
                var tasks = service.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(Task.Fields.ScheduledStart, Task.Fields.ScheduledEnd),
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                        {
                            new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, payment.ipg_CaseId.Id),
                            new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal,(int)TaskState.Open)
                        }
                    },
                    LinkEntities =
                    {
                        new LinkEntity(Task.EntityLogicalName, ipg_taskcategory.EntityLogicalName, ipg_taskcategory.PrimaryIdAttribute, Task.Fields.ipg_taskcategoryid, JoinOperator.Inner)
                        {
                            LinkCriteria = new FilterExpression(LogicalOperator.Or)
                            {
                                Conditions = 
                                {
                                    new ConditionExpression(ipg_taskcategory.Fields.ipg_name, ConditionOperator.Equal, Constants.TaskCategoryNames.PatientOutreach)
                                }
                            }
                        }
                    }
                });

                foreach (var task in tasks.Entities.Select(t=>t.ToEntity<Task>()))
                {
                    var updatedTask = new Task()
                    {
                        Id = task.Id,
                        ScheduledStart = task.ScheduledStart?.AddDays(30),
                        ScheduledEnd = task.ScheduledEnd?.AddDays(30)
                    };

                    service.Update(updatedTask);
                }

            }
        }
    }
}