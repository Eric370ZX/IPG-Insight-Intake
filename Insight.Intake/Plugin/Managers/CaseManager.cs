using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Helpers;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Extensions;
using Insight.Intake.Repositories;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.Managers
{
    public class CaseManager
    {
        private readonly IOrganizationService _crmService;
        private readonly ITracingService _tracingService;
        private readonly EntityReference _caseRef;
        private readonly PatientStatementTaskRepository _PSTaskRepo;
        private readonly TaskManager _taskManager;

        public CaseManager(IOrganizationService organizationService, ITracingService tracingService, EntityReference caseRef)
        {
            this._crmService = organizationService;
            this._tracingService = tracingService;
            this._caseRef = caseRef;
            _taskManager = new TaskManager(organizationService, tracingService);
            _PSTaskRepo = new PatientStatementTaskRepository(organizationService, tracingService);
        }

        internal IEnumerable<SalesOrder> RelatedActivePOs()
        {
            var query = new QueryExpression(SalesOrder.EntityLogicalName);
            query.ColumnSet = new ColumnSet("name");
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)SalesOrderState.Active);
            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _caseRef.Id);
            query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, (int)SalesOrder_StatusCode.Voided);
            var result = _crmService.RetrieveMultiple(query).Entities.Select(p => p.ToEntity<SalesOrder>());
            return result;
        }

        public void CloseOutstandingTasks()
        {
            var query = new QueryExpression(Task.EntityLogicalName);
            query.ColumnSet = new ColumnSet(Task.Fields.StateCode);
            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _caseRef.Id);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)TaskState.Open);

            var outstandingTasks = _crmService.RetrieveMultiple(query)
                .Entities
                .Select(p => p.ToEntity<Task>());
            var exMultRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = false
                },
                Requests = new OrganizationRequestCollection()
            };
            foreach (var iTask in outstandingTasks)
            {

                var updRequest = new UpdateRequest()
                {
                    Target = new Entity(iTask.LogicalName, iTask.Id)
                    {
                        [Task.Fields.StateCode] = new OptionSetValue((int)TaskState.Canceled),
                        [Task.Fields.StatusCode] = new OptionSetValue((int)Task_StatusCode.Caseclosed),
                        [Task.Fields.ipg_taskreason] = new EntityReference(ipg_taskreason.EntityLogicalName, TaskReason.CaseIsClosed),
                    }
                };
                exMultRequest.Requests.Add(updRequest);
            }
            _crmService.Execute(exMultRequest);
        }

        public void ClosePSTasks()
        {
            var openPSTasks = _PSTaskRepo.GetActiveStatements(_caseRef);

            openPSTasks.ForEach(t => _crmService.Update(new ipg_statementgenerationtask()
            {
                Id = t.Id
                ,
                StateCode = ipg_statementgenerationtaskState.Inactive
                ,
                StatusCodeEnum = ipg_statementgenerationtask_StatusCode.Canceled
            }));
        }

        public ValidationResult CheckIfCaseCanBeClosed()
        {
            var checkActiveClaim = CheckActiveClaim();
            var checkCaseIsOnHold = CheckCaseIsOnHold();
            if (!(checkActiveClaim.Succeeded && checkCaseIsOnHold.Succeeded))
            {
                //var outputStringsErrors = new[] { checkPatientPaymentOrPayer.Output, checkActiveClaim.Output, checkCaseIsOnHold.Output, checkPO.Output };
                var outputErrors = new[] { checkActiveClaim, checkCaseIsOnHold };
                var outputStringsErrors = outputErrors
                    .Where(p => !p.Succeeded)
                    .Select(p => p.Output);
                return new ValidationResult(false, string.Join($"\n", outputStringsErrors)) { SeverityLevel = ipg_SeverityLevel.Error };
            }
            var checkBalances = CheckCaseBalances();
            var isSucceeded = checkActiveClaim.Succeeded && checkCaseIsOnHold.Succeeded && checkBalances.Succeeded;

            var outputResultValidation = new[] { checkActiveClaim, checkCaseIsOnHold, checkBalances };
            var outputStrings = outputResultValidation.Where(p => !p.Succeeded).Select(p => p.Output);
            var outputResult = outputStrings
                .Where(p => !string.IsNullOrEmpty(p));
            var result = new ValidationResult(isSucceeded, string.Join($"\n", outputResult)) { SeverityLevel = isSucceeded ? ipg_SeverityLevel.Info : ipg_SeverityLevel.Warning };
            return result;
        }

        internal void CreateTask(EntityReference caseRef, string ipg_taskid)
        {
            _taskManager.CreateTaskByTaskTypeID(caseRef, ipg_taskid);   
        }

        public ValidationResult CheckCalcRev()
        {
            var targetCase = _crmService.Retrieve(_caseRef.LogicalName, _caseRef.Id, new ColumnSet("ipg_actualrevenue")).ToEntity<Incident>();

            var paymentsQuery = new QueryExpression(ipg_payment.EntityLogicalName);
            paymentsQuery.ColumnSet = new ColumnSet("ipg_totalinsurancepaid", "ipg_memberpaid_new", "ipg_ar_carrierwriteoff",
                "ipg_armemberwriteoff", "ipg_ar_carriersalesadj", "ipg_ar_membersalesadj", "ipg_ar_carrieradj", "ipg_ar_memberadj");
            paymentsQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _caseRef.Id);
            paymentsQuery.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ipg_paymentState.Active);
            var payments = _crmService.RetrieveMultiple(paymentsQuery)
                .Entities
                .Select(p => p.ToEntity<ipg_payment>());
            var totalPayments = payments.Sum(p => p.ipg_TotalInsurancePaid) + payments.Sum(p => (double)(p.ipg_MemberPaid_new == null ? 0 : p.ipg_MemberPaid_new.Value));
            var totalwriteOff = payments.Sum(p => p.ipg_ar_carrierwriteoff) + payments.Sum(p => p.ipg_ARMemberWriteoff);
            var totaladjustment = payments.Sum(p => p.ipg_ar_carrieradj) + payments.Sum(p => p.ipg_ar_memberadj);
            var totalSalesAdjustment = payments.Sum(p => p.ipg_ar_carriersalesadj) + payments.Sum(p => p.ipg_ar_membersalesadj);

            var lastBalance = ((double?)targetCase.ipg_ActualRevenue?.Value) ?? 0 - totalPayments - totalwriteOff - totaladjustment - totalSalesAdjustment;
            if ((lastBalance <= 0.01 && lastBalance >= -0.01) || lastBalance == null)
            {
                return new ValidationResult(true);
            }
            return new ValidationResult(false, $"Calc rev doesn't exist  or doesn't show revenue");
        }

        private IEnumerable<ipg_casepartdetail> GetCasePartDetails()
        {
            var query = new QueryExpression(ipg_casepartdetail.EntityLogicalName);
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _caseRef.Id);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ipg_casepartdetailState.Active);
            return _crmService.RetrieveMultiple(query).Entities.Select(p => p.ToEntity<ipg_casepartdetail>());
        }
        public ValidationResult CheckCaseParts()
        {
            var cpd = GetCasePartDetails();
            if (!cpd.Any())
            {
                return new ValidationResult(false, "No Actual Parts on Case");
            }
            var byProduct = cpd.GroupBy(p => p.ipg_productid);
            var allPartsAreDebited = CheckDebitedParts(byProduct);
            if (allPartsAreDebited)
            {
                return new ValidationResult(false, "All case part details were debited");
            }
            return new ValidationResult(true, $"At Least One Actual Part on Case");
        }

        private bool CheckDebitedParts(IEnumerable<IGrouping<EntityReference, ipg_casepartdetail>> cpdProductGroup)
        {
            foreach (var iPo in cpdProductGroup)
            {
                var sum = iPo.Sum(p => p.ipg_quantity);
                if (sum != 0 && sum != null)
                {
                    return false;
                }
            }
            return true;
        }

        public ValidationResult CheckPo()
        {
            var pos = RelatedActivePOs();
            if (!pos.Any())
            {
                return new ValidationResult(true, "No Active POs On Case");
            }
            return new ValidationResult(false, $"At Least One Active PO on Case");
        }

        public ValidationResult CheckCaseBalances()
        {
            var sourceCase = _crmService.Retrieve(_caseRef.LogicalName, _caseRef.Id, new ColumnSet("ipg_casehold", "ipg_remainingpatientbalance", "ipg_remainingcarrierbalance", "ipg_totalbalance"))
               .ToEntity<Incident>();
            var hasPatientBalance = sourceCase.ipg_RemainingPatientBalance != null && sourceCase.ipg_RemainingPatientBalance.Value != 0;
            var hasPayerBalance = sourceCase.ipg_RemainingCarrierBalance != null && sourceCase.ipg_RemainingCarrierBalance.Value != 0;
            var hasTotalBalance = sourceCase.ipg_TotalBalance != null && sourceCase.ipg_TotalBalance.Value != 0;
            if (!(hasTotalBalance))
            {
                return new ValidationResult(true);
            }
            return new ValidationResult(false, $"Case cannot be closed due to AR Balance <> $0.00");
        }

        public ValidationResult CheckCaseIsOnHold()
        {
            var sourceCase = _crmService.Retrieve(_caseRef.LogicalName, _caseRef.Id, new ColumnSet("ipg_casehold"))
                .ToEntity<Incident>();
            if (sourceCase.ipg_casehold != true)
            {
                return new ValidationResult(true);
            }
            return new ValidationResult(false, "Case cannot be closed due to Case Hold. Please release the Hold prior to attempting to close this Case.");
        }

        public ValidationResult CheckActiveClaim()
        {
            var query = new QueryExpression(Invoice.EntityLogicalName);
            query.Criteria.AddCondition(Invoice.Fields.ipg_active, ConditionOperator.Equal, true);
            query.Criteria.AddCondition(Invoice.Fields.ipg_caseid, ConditionOperator.Equal, _caseRef.Id);
            query.Criteria.AddCondition(Invoice.Fields.ipg_Status, ConditionOperator.NotIn, (int)ipg_ClaimStatus.Voided, (int)ipg_ClaimStatus.Adjudicated);
            var hasActiveClaims = _crmService.RetrieveMultiple(query).Entities.Any();
            if (!hasActiveClaims)
            {
                return new ValidationResult(true, "No Pending Claim on Case");
            }
            return new ValidationResult(false, "Case cannot be closed due to an active Claim");
        }

        public ValidationResult CheckPatientPaymentOrPayer()
        {
            var sourceCase = _crmService.Retrieve(_caseRef.LogicalName, _caseRef.Id, new ColumnSet("ipg_ipapayerid"))
                .ToEntity<Incident>();
            if (sourceCase.ipg_ipapayerid != null)
            {
                return new ValidationResult(false, "ipg_ipapayerid is populated");
            }
            var paymentQuery = new QueryExpression(ipg_payment.EntityLogicalName);
            paymentQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _caseRef.Id);
            paymentQuery.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ipg_paymentState.Active);
            var payments = _crmService.RetrieveMultiple(paymentQuery).Entities;
            if (payments.Any())
            {
                return new ValidationResult(false, "There are payments associated with the case\n");
            }
            return new ValidationResult(true);
        }

        internal IEnumerable<CaseHoldConfig> GetCaseholdParameters()
        {
            var resultConfigs = new List<CaseHoldConfig>();
            var query = new QueryExpression("ipg_caseholdconfiguration");
            query.ColumnSet = new ColumnSet(true);
            var initCaseHoldConfig = _crmService.RetrieveMultiple(query).Entities;

            foreach (var iHoldConfig in initCaseHoldConfig)
            {

                var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                  <entity name='ipg_casestatusdisplayed'>
                    <attribute name='ipg_casestatusdisplayedid' />
                    <attribute name='ipg_name' />
                    <order attribute='ipg_name' descending='false' />
                    <link-entity name='ipg_ipg_caseholdconfiguration_ipg_casestatu' from='ipg_casestatusdisplayedid' to='ipg_casestatusdisplayedid' visible='false' intersect='true'>
                      <link-entity name='ipg_caseholdconfiguration' from='ipg_caseholdconfigurationid' to='ipg_caseholdconfigurationid' alias='aa'>
                        <filter type='and'>
                          <condition attribute='ipg_caseholdconfigurationid' operator='eq' value='{iHoldConfig.Id}' />
                        </filter>
                      </link-entity>
                    </link-entity>
                  </entity>
                </fetch>";
                var caseStatuses = _crmService
                    .RetrieveMultiple(new FetchExpression(fetch))
                    .Entities
                    .Select(p => p.ToEntity<ipg_casestatusdisplayed>());
                foreach (var iStatus in caseStatuses)
                {
                    resultConfigs.Add(new CaseHoldConfig()
                    {
                        CaseHoldReason = (ipg_Caseholdreason)iHoldConfig.GetAttributeValue<OptionSetValue>("ipg_caseholdreason")?.Value,
                        CaseStatus = iStatus.ToEntityReference()
                    });
                }
            }
            return resultConfigs;
        }

        internal void CreateHoldNote(string holdNote, ipg_Caseholdreason? holdReason, bool isHold, string holdReasonValue = null)
        {
            var reason = "";
            switch (holdReason)
            {
                case ipg_Caseholdreason.Other:
                    reason = "Patient case is on hold";
                    break;
                case ipg_Caseholdreason.PatientLitigation:
                    reason = "Patient case is in litigation. Case on hold pending final decision";
                    break;
                case ipg_Caseholdreason.Patientbankruptcy:
                    reason = "Patient filed for bankruptcy. Case on hold pending final decision";
                    break;
                case ipg_Caseholdreason.PatientSettlementPending:
                    reason = "Patient case is pending a settlement decision..";
                    break;
                case ipg_Caseholdreason.CollectionsHold:
                    reason = "Collections Hold";
                    break;
                case ipg_Caseholdreason.FacilityRecoveryAPInvoiceIssued:
                    reason = "Facility Recovery AP Invoice Issued";
                    break;
                case ipg_Caseholdreason.FacilityRecoveryDebitPending:
                    reason = "Facility Recovery DebitPending";
                    break;
                case ipg_Caseholdreason.FacilityRecoveryLetterSent:
                    reason = "Facility Recovery Letter Sent";
                    break;
                case ipg_Caseholdreason.FacilityRecoveryManufacturerCreditPending:
                    reason = "Facility Recovery Manufacturer Credit Pending";
                    break;
                case ipg_Caseholdreason.FacilityRecoveryResearchApproved:
                    reason = "Facility Recovery Research Approved";
                    break;
                case ipg_Caseholdreason.FacilityRecoveryResearchPending:
                    reason = "Facility Recovery Research Pending";
                    break;
                case ipg_Caseholdreason.FeeScheduleHold:
                    reason = "Fee Schedule Hold";
                    break;
                case ipg_Caseholdreason.PendingCourtesyClaimDocuments:
                    reason = "Pending Courtesy Claim Documents";
                    break;
                case ipg_Caseholdreason.PendingFacilityContract:
                    reason = "Pending Facility Contract";
                    break;
                case ipg_Caseholdreason.PostClaimCorrections:
                    reason = "Post Claim Corrections";
                    break;
                case ipg_Caseholdreason.QueuedforBilling:
                    reason = "Queued for Billing";
                    break;
                case ipg_Caseholdreason.SettlementPending:
                    reason = "Settlement Pending";
                    break;
            }
            if (holdReason == null)
                reason = holdReasonValue;
            var note = new Annotation()
            {
                ObjectId = _caseRef,
                Subject = reason,
                NoteText = isHold
                    ? $"Case on User Hold for {reason}. Additional Details: {holdNote}."
                    : $"Case released from hold due to {holdNote}"
            };
            _crmService.Create(note);
        }

        internal void ClouseOutstangindTasks()
        {
            var holdTaskTypes = new[] { ipg_TaskType1.CaseisonholdBankruptcy_Followuptask, ipg_TaskType1.CaseisonholdBankruptcy_Initialtask,
            ipg_TaskType1.CaseisonholdLitigation_Followuptask,ipg_TaskType1.CaseisonholdLitigation_Initialtask,
            ipg_TaskType1.CaseisonholdSettlementPending_Initialtask,ipg_TaskType1.CaseisonholdSettlementPending_Followuptask};
            var holdTaskTypesNums = holdTaskTypes.Select(p => (int)p).ToArray();
            var query = new QueryExpression(Task.EntityLogicalName);
            query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, _caseRef.Id);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)TaskState.Open);
            query.Criteria.AddCondition(new ConditionExpression("ipg_tasktypecode", ConditionOperator.In, holdTaskTypesNums));
            var outstandingTasks = _crmService.RetrieveMultiple(query).Entities;
            foreach (var iTask in outstandingTasks)
            {
                var request = new SetStateRequest()
                {
                    EntityMoniker = iTask.ToEntityReference(),
                    State = new OptionSetValue((int)TaskState.Completed),
                    Status = new OptionSetValue((int)Task_StatusCode.Resolved)
                };
                _crmService.Execute(request);
            }
        }

        internal void SetHold(bool isOnHold, ipg_Caseholdreason? caseHoldReason)
        {
            var caseUpd = new Incident
            {
                Id = _caseRef.Id,
                ipg_casehold = isOnHold,
                ["ipg_caseholdreason"] = caseHoldReason != null ? new OptionSetValue((int)caseHoldReason) : null
            };
            _crmService.Update(caseUpd);
        }

        public ipg_tasktype GetTaskType(string taskTypeSettingName)
        {
            var taskTypeName = _crmService.GetGlobalSettingValueByKey(taskTypeSettingName);
            var query = new QueryExpression(ipg_tasktype.EntityLogicalName);
            query.ColumnSet = new ColumnSet(nameof(ipg_tasktype.ipg_name).ToLower());
            query.Criteria.AddCondition(nameof(ipg_tasktype.ipg_name).ToLower(), ConditionOperator.Equal, taskTypeName);
            var taskTypes = _crmService.RetrieveMultiple(query).Entities;
            if (!taskTypes.Any())
                throw new InvalidPluginExecutionException($"Unable to find a task type with name {taskTypeName}, setting name is {taskTypeSettingName}");
            return taskTypes.First().ToEntity<ipg_tasktype>();
        }
        public Task CreateOriginalSettlementPendingHoldTask()
            => CreateHoldTask(ipg_TaskType1.CaseisonholdSettlementPending_Initialtask, 45, "$0 AR - Settlement Pending Follow - up", "Recieve documentation and make final decision");
        public void CreateFollowUpHoldTask_SettlementPending()
            => CreateHoldTask(ipg_TaskType1.CaseisonholdSettlementPending_Followuptask, 45, "$0 AR - Settlement Pending 2nd Follow-up", "Recieve documentation and make final decision");
        public Task CreateOriginalBankruptcyHoldTask()
            => CreateHoldTask(ipg_TaskType1.CaseisonholdBankruptcy_Initialtask, 120, "Case is on Hold due to Patient Bankruptcy", "Recieve documentation and make final decision");
        public void CreateFollowUpHoldTask_Bankruptcy()
            => CreateHoldTask(ipg_TaskType1.CaseisonholdBankruptcy_Followuptask, 120, "Case is on Hold due to Patient Bankruptcy", "Recieve documentation and make final decision");
        public void CreateFollowUpHoldTask_Litigation()
           => CreateHoldTask(ipg_TaskType1.CaseisonholdLitigation_Followuptask, 45, "Case is on Hold due to Patient Litigation", "Recieve documentation and make final decision");
        public Task CreateOriginalHoldTask()
            => CreateHoldTask(ipg_TaskType1.CaseisonholdLitigation_Initialtask, 45, "Case is on Hold due to Patient Litigation", "Recieve documentation and make final decision");
        public void CreateFollowUpHoldTask(ipg_TaskType1? taskType)
        {
            switch (taskType)
            {
                case ipg_TaskType1.CaseisonholdLitigation_Initialtask:
                    CreateFollowUpHoldTask_Litigation();
                    break;
                case ipg_TaskType1.CaseisonholdBankruptcy_Initialtask:
                    CreateFollowUpHoldTask_Bankruptcy();
                    break;
                case ipg_TaskType1.CaseisonholdSettlementPending_Initialtask:
                    CreateFollowUpHoldTask_SettlementPending();
                    break;
            }
        }


        private Task CreateHoldTask(ipg_TaskType1 taskType, int dueShift, string subject, string body)
        {
            var targetTask = new Task()
            {
                RegardingObjectId = _caseRef,
                ipg_caseid = _caseRef,
                Subject = subject,
                Description = body,
                ScheduledEnd = DateTime.Now.AddDays(dueShift),
                ipg_tasktypecode = new OptionSetValue((int)taskType)
            };
            targetTask.Id = _crmService.Create(targetTask);
            return targetTask;
        }

        public void SetState(CaseStatusResult closeStatus)
        {
            var caseUpdate = new Incident()
            {
                Id = _caseRef.Id,
                ipg_CaseStatus = new OptionSetValue((int)closeStatus.CaseStatus),
                ipg_providerstatus = new OptionSetValue((int)closeStatus.ProviderStatus),
                ipg_Reasons = new OptionSetValue((int)closeStatus.CaseReason),
                StateCode = closeStatus.StateCode,
                StatusCode = new OptionSetValue((int)closeStatus.StatusCode),
                ipg_casestatusdisplayedid = closeStatus.CaseStatusDisplayed,
                ipg_facilitycommunication = closeStatus.FacilityCommunication,
                ipg_closedby = closeStatus.ClosedBy,
            };
            _crmService.Update(caseUpdate);
        }
        internal CaseStatusResult CreateCaseStatusResult(string closedBy, string facilityCommunication, ipg_Caseclosurereason closeReason, Guid callingUserId)
        {
            var result = new CaseStatusResult()
            {
                StateCode = IncidentState.Canceled,
                StatusCode = Incident_StatusCode.Canceled,
                CaseStatus = ipg_CaseStatus.Closed,
            };

            result.ClosedBy = GetClosedBy(closedBy, callingUserId);
            result.FacilityCommunication = GetFacilityCommunication(facilityCommunication);
            result.ProviderStatus = GetProviderStatusFromCaseClosureReason(closeReason);
            result.CaseReason = GetCaseReasonFromCaseClosureReason(closeReason);

            var caseDisplayedStatus = GetCaseDisplayedStatus(closedBy);
            if (caseDisplayedStatus != null)
            {
                result.CaseStatusDisplayed = caseDisplayedStatus;
            }

            return result;
        }

        public void CreateCloseCaseNote(string closeReasonDescription, string closeNote, string closedBy, Guid callingUserId)
        {
            string closedByUser = GetClosedBy(closedBy, callingUserId);
            Annotation note = new Annotation
            {

                ObjectId = _caseRef,
                ObjectTypeCode = Incident.EntityLogicalName,
                NoteText = $"Case closed by {closedByUser} due to {closeReasonDescription}. Additional details:\n{closeNote}",
                IsDocument = false
            };
            _crmService.Create(note);
        }

        private EntityReference GetCaseDisplayedStatus(string closedBy)
        {
            var columns = new ColumnSet(Incident.Fields.ipg_caseoutcome, Incident.Fields.ipg_gateconfigurationid);
            var sourceCase = _crmService.Retrieve(_caseRef.LogicalName, _caseRef.Id, columns).ToEntity<Incident>();

            if (sourceCase.ipg_gateconfigurationid == null)
            {
                return null;
            }
            var currentGate = _crmService
                .Retrieve(sourceCase.ipg_gateconfigurationid.LogicalName, sourceCase.ipg_gateconfigurationid.Id, new ColumnSet("ipg_cancelstatusfacilityid", "ipg_cancelstatusipg"))
                .ToEntity<ipg_gateconfiguration>();
            var caseStatusFieldName = closedBy == "Facility" ? "ipg_cancelstatusfacilityid" : "ipg_cancelstatusipg";
            return currentGate.GetAttributeValue<EntityReference>(caseStatusFieldName);
        }

        private string GetClosedBy(string closedBy, Guid callingUserId)
        {
            if (string.IsNullOrEmpty(closedBy) || !closedBy.Contains(ClosureCasePlaseholder.UserName))
            {
                return closedBy;
            }

            var columns = new ColumnSet(SystemUser.Fields.FullName);
            var user = _crmService.Retrieve(SystemUser.EntityLogicalName, callingUserId, columns);
            if (user == null)
            {
                throw new InvalidPluginExecutionException($"There are no users in the system with ID: {callingUserId}");
            }

            var userName = user.GetAttributeValue<string>(SystemUser.Fields.FullName);
            var result = closedBy.Replace(ClosureCasePlaseholder.UserName, userName);
            return result;
        }

        private string GetFacilityCommunication(string facilityCommunication)
        {
            if (string.IsNullOrEmpty(facilityCommunication))
                return facilityCommunication;

            var columns = new ColumnSet(
                Incident.Fields.ipg_procedureid,
                Incident.Fields.ipg_CarrierId,
                Incident.Fields.ipg_Facility,
                Incident.Fields.ipg_SurgeryDate);
            var incident = _crmService.Retrieve(Incident.EntityLogicalName, _caseRef.Id, columns);

            if (incident == null)
                throw new InvalidPluginExecutionException($"There are no cases in the system with ID: {_caseRef.Id}");

            var procedureName = incident.GetAttributeValue<EntityReference>(Incident.Fields.ipg_procedureid)?.Name;
            var carrier = incident.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId)?.Name;
            var facility = incident.GetAttributeValue<string>(Incident.Fields.ipg_Facility);
            var scheduledProcedureDate = incident.GetAttributeValue<DateTime>(Incident.Fields.ipg_SurgeryDate);

            var result = facilityCommunication
                .Replace(ClosureCasePlaseholder.ProcedureName, procedureName)
                .Replace(ClosureCasePlaseholder.PrimaryCarrierName, carrier)
                .Replace(ClosureCasePlaseholder.ScheduledProcedureDate, scheduledProcedureDate.ToShortDateString());
            return result;
        }

        private ipg_ProviderStatus GetProviderStatusFromCaseClosureReason(ipg_Caseclosurereason closureReason)
        {
            switch (closureReason)
            {
                case ipg_Caseclosurereason.DuplicateReferralorCase:
                    return ipg_ProviderStatus.DuplicateReferralorCase;
                case ipg_Caseclosurereason.FacilityRecoveryAPCreditIssued:
                    return ipg_ProviderStatus.IPGProcessCompleteCaseClosed;
                case ipg_Caseclosurereason.FacilityRecoveryManufacturerCreditReceived:
                    return ipg_ProviderStatus.IPGProcessCompleteCaseClosed;
                case ipg_Caseclosurereason.MedicareorOtherGovernmentFundedHealthPlan:
                    return ipg_ProviderStatus.MedicareorOtherGovernmentFundedHealthPlan;
                case ipg_Caseclosurereason.NoActivePrimaryHealthPlan:
                    return ipg_ProviderStatus.NoActivePrimaryHealthPlan;
                case ipg_Caseclosurereason.NoBillableParts:
                    return ipg_ProviderStatus.NoBillableParts;
                case ipg_Caseclosurereason.SentinError:
                    return ipg_ProviderStatus.SentinError;
                case ipg_Caseclosurereason.NoImplantsUsed:
                    return ipg_ProviderStatus.NoImplantsUsed;
                case ipg_Caseclosurereason.NonParticipatingHealthPlan:
                    return ipg_ProviderStatus.NonParticipatingHealthPlan;
                case ipg_Caseclosurereason.PatientisSelfPayorOtherwiseUninsured:
                    return ipg_ProviderStatus.PatientisSelfPayorOtherwiseUninsured;
                case ipg_Caseclosurereason.ProcedureCancelled:
                    return ipg_ProviderStatus.ProcedureCancelledbyFacility;
                case ipg_Caseclosurereason.ProcedureNotAuthorized:
                    return ipg_ProviderStatus.ProcedureNotAuthorized;
                case ipg_Caseclosurereason.SubmittedPastTimelyFilingLimits:
                    return ipg_ProviderStatus.SubmittedPastTimelyFilingLimits;
                case ipg_Caseclosurereason.UnabletoCoordinatewithCarrier:
                    return ipg_ProviderStatus.UnabletoCoordinatewithCarrier;
                case ipg_Caseclosurereason.UnabletoCoordinatewithFacility:
                    return ipg_ProviderStatus.UnabletoCoordinatewithFacility;
                default:
                    throw new InvalidPluginExecutionException($"Mapping for {Enum.GetName(typeof(ipg_Caseclosurereason), closureReason)} is not implemented");
            }
        }

        private ipg_CaseReasons GetCaseReasonFromCaseClosureReason(ipg_Caseclosurereason closureReason)
        {
            switch (closureReason)
            {
                case ipg_Caseclosurereason.DuplicateReferralorCase:
                    return ipg_CaseReasons.DuplicateReferralorCase;
                case ipg_Caseclosurereason.FacilityRecoveryAPCreditIssued:
                    return ipg_CaseReasons.FacilityRecoveryAPCreditIssued;
                case ipg_Caseclosurereason.FacilityRecoveryManufacturerCreditReceived:
                    return ipg_CaseReasons.FacilityRecoveryManufacturerCreditReceived;
                case ipg_Caseclosurereason.MedicareorOtherGovernmentFundedHealthPlan:
                    return ipg_CaseReasons.MedicareorOtherGovernmentFundedHealthPlan;
                case ipg_Caseclosurereason.NoActivePrimaryHealthPlan:
                    return ipg_CaseReasons.NoActivePrimaryHealthPlan;
                case ipg_Caseclosurereason.NoBillableParts:
                    return ipg_CaseReasons.NoBillableParts;
                case ipg_Caseclosurereason.NoImplantsUsed:
                    return ipg_CaseReasons.NoImplantsUsed;
                case ipg_Caseclosurereason.NonParticipatingHealthPlan:
                    return ipg_CaseReasons.NonParticipatingHealthPlan;
                case ipg_Caseclosurereason.PatientisSelfPayorOtherwiseUninsured:
                    return ipg_CaseReasons.PatientisSelfPayorOtherwiseUninsured;
                case ipg_Caseclosurereason.ProcedureCancelled:
                    return ipg_CaseReasons.ProcedureCancelled;
                case ipg_Caseclosurereason.ProcedureNotAuthorized:
                    return ipg_CaseReasons.ProcedureNotAuthorized;
                case ipg_Caseclosurereason.SentinError:
                    return ipg_CaseReasons.SentinError;
                case ipg_Caseclosurereason.SubmittedPastTimelyFilingLimits:
                    return ipg_CaseReasons.SubmittedPastTimelyFilingLimits;
                case ipg_Caseclosurereason.UnabletoCoordinatewithCarrier:
                    return ipg_CaseReasons.UnabletoCoordinatewithCarrier;
                case ipg_Caseclosurereason.UnabletoCoordinatewithFacility:
                    return ipg_CaseReasons.UnabletoCoordinatewithFacility;
                default:
                    throw new InvalidPluginExecutionException($"Mapping for {Enum.GetName(typeof(ipg_Caseclosurereason), closureReason)} is not implemented");
            }
        }

        internal void CreateAlertTask(IEnumerable<SalesOrder> orders)
        {
            var sourceCase = _crmService.RetrieveMultiple(new QueryExpression(Incident.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(Incident.Fields.Title),
                Criteria = new FilterExpression()
                {
                    Conditions = { new ConditionExpression(Incident.PrimaryIdAttribute, ConditionOperator.Equal, _caseRef.Id) }
                },
                LinkEntities =
                {
                    new LinkEntity(Incident.EntityLogicalName, Intake.Account.EntityLogicalName,Incident.Fields.ipg_FacilityId,  Intake.Account.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        EntityAlias = "facility",
                        Columns =  new ColumnSet(Intake.Account.Fields.ipg_FacilityCaseMgrId)
                    }
                }
            }).Entities.FirstOrDefault().ToEntity<Incident>();
            var orderNames = string.Join(", ", orders.Select(p => p.Name));
            var caseMgr = sourceCase.GetAttributeValue<AliasedValue>($"facility.{Intake.Account.Fields.ipg_FacilityCaseMgrId}")?.Value as EntityReference;

            if (caseMgr == null)
            {
                throw new InvalidPluginExecutionException("Case manager is empty. It is required to create tasks");
            }
            var alertTask = new Task()
            {
                RegardingObjectId = _caseRef,
                Subject = "Case with PO is being Closed",
                OwnerId = caseMgr,
                Description = $"Case was closed with outstanding PO(s). Case is {sourceCase.Title}. Related orders: {orderNames}"
            };

            _crmService.Create(alertTask);
        }

        internal bool IsCaseRequiresTissueRequestForm(Incident incident)
        {
            if (IsTissueRequestFormDocumentMissing(incident.Id))
            {
                _tracingService.Trace("Tissue Request Form Document Missing");

                var tissueRequestFormCpts = new SettingsHelper(_crmService)
                   .GetTissueRequestFormCpts();

                _tracingService.Trace($"cpts: {string.Join(",", tissueRequestFormCpts)}");

                var cptCodeFields = new[]
                {
                    Incident.Fields.ipg_CPTCodeId1, Incident.Fields.ipg_CPTCodeId2, Incident.Fields.ipg_CPTCodeId3,
                    Incident.Fields.ipg_CPTCodeId4, Incident.Fields.ipg_CPTCodeId5, Incident.Fields.ipg_CPTCodeId6
                };

                _tracingService.Trace($"Attribute keys: {string.Join(";", incident.Attributes.Keys)}");

                foreach (var field in cptCodeFields)
                {
                    if (incident.GetAttributeValue<EntityReference>(field) is EntityReference cptCodeRef)
                    {
                        ipg_cptcode cptCode = _crmService.Retrieve<ipg_cptcode>(cptCodeRef.Id, new ColumnSet(ipg_cptcode.Fields.ipg_cptcode1));
                        _tracingService.Trace($"cpt code to check: {cptCode.ipg_cptcode1}");

                        if (tissueRequestFormCpts.Contains(cptCode.ipg_cptcode1))
                            return true;
                    }
                }
            }

            return false;
        }

        public DateTime AddBusinessDays(DateTime date, int days)
        {
            if (days == 0) return date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
                days -= 1;
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
                days -= 1;
            }

            date = date.AddDays(days / 5 * 7);
            int extraDays = days % 5;

            if ((int)date.DayOfWeek + extraDays > 5)
            {
                extraDays += 2;
            }

            return date.AddDays(extraDays);

        }

        internal bool IsTissueRequestFormDocumentMissing(Guid incidentId)
        {
            var docs = new DocumentRepository(_crmService)
                .GetNotRejectedActiveTissueRequestForms(incidentId, new ColumnSet(false));

            return !docs.Any();
        }

        internal ipg_benefit GetBenefit(IOrganizationService service)
        {
            string[] fields = { Incident.Fields.ipg_CarrierId,
                                Incident.Fields.ipg_BenefitTypeCode,
                                Incident.Fields.ipg_inoutnetwork,
                                Incident.Fields.ipg_CoverageLevelDeductible,
                              };

            Incident incident = service.Retrieve(_caseRef.LogicalName, _caseRef.Id, new ColumnSet(fields)).ToEntity<Incident>();

            if (fields.Any(field => incident.GetAttributeValue<object>(field) == null))
            {
                return null;
            }

            var queryExpression = new QueryExpression(ipg_benefit.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_benefit.ipg_CaseId).ToLower(), ConditionOperator.Equal, incident.Id),
                            new ConditionExpression(nameof(ipg_benefit.ipg_CarrierId).ToLower(), ConditionOperator.Equal, incident.ipg_CarrierId.Id),
                            new ConditionExpression(nameof(ipg_benefit.ipg_BenefitType).ToLower(), ConditionOperator.Equal, incident.ipg_BenefitTypeCode.Value),
                            new ConditionExpression(nameof(ipg_benefit.ipg_InOutNetwork).ToLower(), ConditionOperator.Equal, incident.ipg_inoutnetwork.Value),
                            new ConditionExpression(nameof(ipg_benefit.ipg_CoverageLevel).ToLower(), ConditionOperator.Equal, incident.ipg_CoverageLevelDeductible.Value)
                        }
                }
            };
            EntityCollection ec = service.RetrieveMultiple(queryExpression);
            return ec.Entities.Count() > 0 ? ec.Entities.First().ToEntity<ipg_benefit>() : null;
        }

        internal int GetCaseStateValue()
        {
            return _crmService.Retrieve(Incident.EntityLogicalName,
                    _caseRef.Id, new ColumnSet(Incident.Fields.ipg_StateCode))
                    .ToEntity<Incident>()
                    .ipg_StateCode.Value;
        }

        private ipg_caseholdconfiguration RetriveCaseHoldConfiguration(int caseState, int holdReason)
        {
            return _crmService.RetrieveMultiple(new QueryExpression(ipg_caseholdconfiguration.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_caseholdconfiguration.Fields.ipg_taskid),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_caseholdconfiguration.Fields.ipg_casestate, ConditionOperator.Equal, caseState),
                        new ConditionExpression(ipg_caseholdconfiguration.Fields.ipg_caseholdreason, ConditionOperator.Equal, holdReason)
                    }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_caseholdconfiguration>();
        }

        public ipg_tasktype RetrieveTaskTypeByTaskTypeId(string taskTypeId)
        {
            return _crmService.RetrieveMultiple(new QueryExpression(ipg_tasktype.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_tasktype.Fields.ipg_description, ipg_tasktype.Fields.ipg_casestatecodes,
                                          ipg_tasktype.Fields.ipg_taskcategoryid, ipg_tasktype.Fields.ipg_generatedbycode, ipg_tasktype.Fields.ipg_name,
                                          ipg_tasktype.Fields.ipg_tasktitle, ipg_tasktype.Fields.ipg_assigntoteam, ipg_tasktype.Fields.ipg_startdate,
                                          ipg_tasktype.Fields.ipg_duedate, ipg_tasktype.Fields.ipg_priority),
                Criteria = new FilterExpression()
                {
                    Conditions =
                        {
                            new ConditionExpression(ipg_tasktype.Fields.ipg_typeid, ConditionOperator.Equal, int.Parse(taskTypeId))
                        }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_tasktype>();
        }

        private Task CreateHoldTask(EntityReference taskTypeRef, string description = null)
        {
            var targetTask = new Task()
            {
                RegardingObjectId = _caseRef,
                ipg_caseid = _caseRef,
                ipg_tasktypeid = taskTypeRef,
                Description = description
            };
            targetTask.Id = _crmService.Create(targetTask);
            return targetTask;
        }
    }

    public class CaseStatusResult
    {
        public IncidentState StateCode { get; set; }
        public Incident_StatusCode StatusCode { get; set; }
        public string Description { get; set; }
        public ipg_CaseStatus CaseStatus { get; set; }
        public EntityReference CaseStatusDisplayed { get; set; }
        public string FacilityCommunication { get; set; }
        public string ClosedBy { get; set; }
        public ipg_ProviderStatus ProviderStatus { get; set; }
        public ipg_CaseReasons CaseReason { get; set; }
    }
    public class CaseHoldConfig
    {
        public EntityReference CaseStatus { get; set; }
        public ipg_Caseholdreason CaseHoldReason { get; set; }
    }
    public class ValidationResult
    {
        public ValidationResult()
        {

        }
        public ValidationResult(bool succeeded, string output = "")
        {
            Succeeded = succeeded;
            Output = output;
        }
        public bool Succeeded { get; set; }
        public string Output { get; set; }
        public ipg_SeverityLevel SeverityLevel { get; set; }
    }

    internal class ClosureCasePlaseholder
    {
        internal const string UserName = "<Name>";
        internal const string ProcedureName = "<Procedure Name>";
        internal const string MDMName = "<MDM Name>";
        internal const string PrimaryCarrierName = "<primary carrier name>";
        internal const string TerminationDate = "<termination date>";
        internal const string ScheduledProcedureDate = "<Scheduled Procedure Date>";
    }
}