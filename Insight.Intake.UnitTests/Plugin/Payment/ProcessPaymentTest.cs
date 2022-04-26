using FakeXrmEasy;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Payment;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.Repositories;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.Payment
{
    public class ProcessPaymentTest : PluginTestsBase
    {
        [Fact]
        public void FullPayment()
        {
            var fakedContext = new XrmFakedContext();

            int headerType = (int)ipg_ClaimSubEvent.Fullpayment;
            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_ICN_VALUE).WithDescription("Value of{0} missing");
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid()
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                CustomerId = carrier.ToEntityReference()
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
            int claimStatus = (int)ipg_ClaimStatus.Adjudicated;
            int claimReason = (int)ipg_ClaimReason.AcceptedbyIntermediary;
            ipg_claimconfiguration claimConfiguration = new ipg_claimconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimEvent = new OptionSetValue((int)ipg_ClaimEvent._835fileprocessing),
                ipg_ClaimSubEvent = new OptionSetValue(headerType),
                ipg_ClaimStatus = new OptionSetValue(claimStatus),
                ipg_ClaimReason = new OptionSetValue(claimReason)
            };
            ipg_claimstatuscode claimStatusCodeDenial = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "denial1 description"
            };
            ipg_claimstatuscode claimStatusCodeRemark = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "remark1 description"
            };
            ipg_statementgenerationeventconfiguration configuration = new ipg_statementgenerationeventconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Promoted to Collections2",
                StateCode = (int)ipg_statementgenerationeventconfigurationState.Active,
                ipg_EventDueDaysStart = 1,
                ipg_EventDueDaysEnd = 1
            };

            fakedContext.Initialize(new List<Entity> { new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment), taskType, batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem, claimConfiguration, claimStatusCodeDenial, claimStatusCodeRemark, configuration });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            ipg_claimlineitem claimLineItemFaked = fakedService.Retrieve(claimLineItem.LogicalName, claimLineItem.Id, new ColumnSet(nameof(ipg_claimlineitem.ipg_paid).ToLower())).ToEntity<ipg_claimlineitem>();
            Assert.Equal(claimLineItemFaked.ipg_paid.Value, amountPaid);

            Invoice claimFaked = fakedService.Retrieve(claim.LogicalName, claim.Id, new ColumnSet(nameof(Invoice.ipg_Status).ToLower(), nameof(Invoice.ipg_Reason).ToLower(), nameof(Invoice.ipg_AdjustmentCodesDescription).ToLower(), nameof(Invoice.ipg_RemarkCodesDescription).ToLower())).ToEntity<Invoice>();
            Assert.Equal(claimStatus, claimFaked.ipg_Status.Value);
            Assert.Equal(claimReason, claimFaked.ipg_Reason.Value);
            Assert.Equal("denial1(denial1 description)\ndenial2", claimFaked.ipg_AdjustmentCodesDescription);
            Assert.Equal("remark1\nremark2", claimFaked.ipg_RemarkCodesDescription);

            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_Reasons).ToLower())).ToEntity<Incident>();
            Assert.Null(incidentFaked.ipg_Reasons);

            ipg_claimresponseheader headerFaked = fakedService.Retrieve(header.LogicalName, header.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower())).ToEntity<ipg_claimresponseheader>();
            Assert.Equal("posted", headerFaked.ipg_PostStatus);
        }

        [Fact]
        public void FullPatientResponsibility()
        {
            var fakedContext = new XrmFakedContext();

            int headerType = (int)ipg_ClaimSubEvent.FullPatientResponsibility;
            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_tasktype taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_ICN_VALUE).WithDescription("Value of{0} missing");
            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid(),
                ipg_RemainingCarrierBalance = new Money(200)
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                CustomerId = carrier.ToEntityReference(),
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
            int claimStatus = (int)ipg_ClaimStatus.Adjudicated;
            int claimReason = (int)ipg_ClaimReason.AcceptedbyIntermediary;
            ipg_claimconfiguration claimConfiguration = new ipg_claimconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimEvent = new OptionSetValue((int)ipg_ClaimEvent._835fileprocessing),
                ipg_ClaimSubEvent = new OptionSetValue(headerType),
                ipg_ClaimStatus = new OptionSetValue(claimStatus),
                ipg_ClaimReason = new OptionSetValue(claimReason)
            };
            ipg_claimstatuscode claimStatusCodeDenial = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "denial1 description"
            };
            ipg_claimstatuscode claimStatusCodeRemark = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "remark1 description"
            };
            Team team = new Team()
            {
                Id = Guid.NewGuid(),
                Name = "Carrier Services"
            };
            ipg_statementgenerationeventconfiguration configuration = new ipg_statementgenerationeventconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Promoted to Collections2",
                StateCode = (int)ipg_statementgenerationeventconfigurationState.Active,
                ipg_EventDueDaysStart = 1,
                ipg_EventDueDaysEnd = 1
            };

            fakedContext.Initialize(new List<Entity> { new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment), taskType, batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem, claimConfiguration, claimStatusCodeDenial, claimStatusCodeRemark, team, configuration });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            ipg_claimlineitem claimLineItemFaked = fakedService.Retrieve(claimLineItem.LogicalName, claimLineItem.Id, new ColumnSet(nameof(ipg_claimlineitem.ipg_paid).ToLower())).ToEntity<ipg_claimlineitem>();
            Assert.Equal(claimLineItemFaked.ipg_paid.Value, 0);

            Invoice claimFaked = fakedService.Retrieve(claim.LogicalName, claim.Id, new ColumnSet(nameof(Invoice.ipg_Status).ToLower(), nameof(Invoice.ipg_Reason).ToLower(), nameof(Invoice.ipg_AdjustmentCodesDescription).ToLower(), nameof(Invoice.ipg_RemarkCodesDescription).ToLower())).ToEntity<Invoice>();
            Assert.Equal(claimStatus, claimFaked.ipg_Status.Value);
            Assert.Equal(claimReason, claimFaked.ipg_Reason.Value);
            Assert.Equal("denial1(denial1 description)\ndenial2", claimFaked.ipg_AdjustmentCodesDescription);
            Assert.Equal("remark1\nremark2", claimFaked.ipg_RemarkCodesDescription);

            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_Reasons).ToLower())).ToEntity<Incident>();
            Assert.Equal((int)ipg_CaseReasons.FullPatientResponsibility, incidentFaked.ipg_Reasons.Value);

            ipg_claimresponseheader headerFaked = fakedService.Retrieve(header.LogicalName, header.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower())).ToEntity<ipg_claimresponseheader>();
            Assert.Equal("posted", headerFaked.ipg_PostStatus);

            var notes = (from t in fakedContext.CreateQuery<Annotation>()
                         where (t.ObjectId.Id == incident.Id &&
                         t.Subject == "Claim processing" &&
                         t.NoteText == "Claim " + claim.Name + " Full Patient Responsibility by . Patient collections process initiated."
                         )
                         select t).ToList();
            Assert.Single(notes);

            var adjustments = (from t in fakedContext.CreateQuery<ipg_adjustment>()
                         where (t.ipg_CaseId.Id == incident.Id &&
                         t.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.TransferofResponsibility &&
                         t.ipg_Amount.Value == incident.ipg_RemainingCarrierBalance.Value
                         )
                         select t).ToList();
            Assert.Single(adjustments);
        }

        [Fact]
        public void Partialpatientresponsibility()
        {
            var fakedContext = new XrmFakedContext();

            int headerType = (int)ipg_ClaimSubEvent.Partialpatientresponsibility;
            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_ICN_VALUE).WithDescription("Value of{0} missing");
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid()
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                CustomerId = carrier.ToEntityReference(),
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
                ipg_AmountPaid_new = new Money(amountPaid - 1),
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
            int claimStatus = (int)ipg_ClaimStatus.Adjudicated;
            int claimReason = (int)ipg_ClaimReason.AcceptedbyIntermediary;
            ipg_claimconfiguration claimConfiguration = new ipg_claimconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimEvent = new OptionSetValue((int)ipg_ClaimEvent._835fileprocessing),
                ipg_ClaimSubEvent = new OptionSetValue(headerType),
                ipg_ClaimStatus = new OptionSetValue(claimStatus),
                ipg_ClaimReason = new OptionSetValue(claimReason)
            };
            ipg_claimstatuscode claimStatusCodeDenial = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "denial1 description"
            };
            ipg_claimstatuscode claimStatusCodeRemark = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "remark1 description"
            };
            Team team = new Team()
            {
                Id = Guid.NewGuid(),
                Name = "Carrier Services"
            };
            ipg_statementgenerationeventconfiguration configuration = new ipg_statementgenerationeventconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Promoted to Collections2",
                StateCode = (int)ipg_statementgenerationeventconfigurationState.Active,
                ipg_EventDueDaysStart = 1,
                ipg_EventDueDaysEnd = 1
            };

            fakedContext.Initialize(new List<Entity> { new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment), taskType, batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem, claimConfiguration, claimStatusCodeDenial, claimStatusCodeRemark, team, configuration });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            ipg_claimlineitem claimLineItemFaked = fakedService.Retrieve(claimLineItem.LogicalName, claimLineItem.Id, new ColumnSet(nameof(ipg_claimlineitem.ipg_paid).ToLower())).ToEntity<ipg_claimlineitem>();
            Assert.Equal(claimLineItemFaked.ipg_paid.Value, amountPaid - 1);

            Invoice claimFaked = fakedService.Retrieve(claim.LogicalName, claim.Id, new ColumnSet(nameof(Invoice.ipg_Status).ToLower(), nameof(Invoice.ipg_Reason).ToLower(), nameof(Invoice.ipg_AdjustmentCodesDescription).ToLower(), nameof(Invoice.ipg_RemarkCodesDescription).ToLower())).ToEntity<Invoice>();
            Assert.Equal(claimStatus, claimFaked.ipg_Status.Value);
            Assert.Equal(claimReason, claimFaked.ipg_Reason.Value);
            Assert.Equal("denial1(denial1 description)\ndenial2", claimFaked.ipg_AdjustmentCodesDescription);
            Assert.Equal("remark1\nremark2", claimFaked.ipg_RemarkCodesDescription);

            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_Reasons).ToLower())).ToEntity<Incident>();
            Assert.Equal((int)ipg_CaseReasons.PartialPatientResponsibility, incidentFaked.ipg_Reasons.Value);

            ipg_claimresponseheader headerFaked = fakedService.Retrieve(header.LogicalName, header.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower())).ToEntity<ipg_claimresponseheader>();
            Assert.Equal("posted", headerFaked.ipg_PostStatus);

            var notes = (from t in fakedContext.CreateQuery<Annotation>()
                         where (t.ObjectId.Id == incident.Id &&
                         t.Subject == "Claim processing" &&
                         t.NoteText == "Claim " + claim.Name + " Partial Patient Responsibility by . Patient collections process initiated."
                         )
                         select t).ToList();
            Assert.Single(notes);
        }

        [Fact]
        public void PendingAdjustmentTask()
        {
            var fakedContext = new XrmFakedContext();

            int headerType = (int)ipg_ClaimSubEvent.Fullpayment;
            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_ICN_VALUE).WithDescription("Value of{0} missing");
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid()
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                CustomerId = carrier.ToEntityReference()
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
                ipg_AmountPaid_new = new Money(amountPaid)
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
            int claimStatus = (int)ipg_ClaimStatus.Adjudicated;
            int claimReason = (int)ipg_ClaimReason.AcceptedbyIntermediary;
            ipg_claimconfiguration claimConfiguration = new ipg_claimconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimEvent = new OptionSetValue((int)ipg_ClaimEvent._835fileprocessing),
                ipg_ClaimSubEvent = new OptionSetValue(headerType),
                ipg_ClaimStatus = new OptionSetValue(claimStatus),
                ipg_ClaimReason = new OptionSetValue(claimReason)
            };
            ipg_claimstatuscode claimStatusCodeDenial = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "denial1 description"
            };
            ipg_claimstatuscode claimStatusCodeRemark = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "remark1 description"
            };
            Task task = new Task()
            {
                Id = Guid.NewGuid(),
                RegardingObjectId = incident.ToEntityReference(),
                StateCode = TaskState.Open,
                ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.PendingCarrierorPatientAdjustment)
            };
            ipg_statementgenerationeventconfiguration configuration = new ipg_statementgenerationeventconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Promoted to Collections2",
                StateCode = (int)ipg_statementgenerationeventconfigurationState.Active,
                ipg_EventDueDaysStart = 1,
                ipg_EventDueDaysEnd = 1
            };

            fakedContext.Initialize(new List<Entity> { new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment), taskType, batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem, claimConfiguration, claimStatusCodeDenial, claimStatusCodeRemark, task, configuration });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            Task taskFaked = fakedService.Retrieve(task.LogicalName, task.Id, new ColumnSet(nameof(Task.ScheduledEnd).ToLower())).ToEntity<Task>();
            Assert.Equal(taskFaked.ScheduledEnd, DateTime.Today.AddDays(1).AddTicks(-1));
        }


        [Fact]
        public void CheckThatCarrierOutReachTaskClosedWhenCarrirPayment()
        {
            var fakedContext = new XrmFakedContext();
            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            Incident incident = new Incident().Fake();
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName(Constants.TaskCategoryNames.CarrierOutreach);
            ipg_tasktype fullDenial = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.CLAIM_RESPONSE_FULL_DENIAL);
            ipg_tasktype partialDenial = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.CLAIM_RESPONSE_PARTIAL_DENIAL).WithDescription("{0}");
            ipg_tasktype missingICN = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_ICN_VALUE).WithDescription("Value of{0} missing"); ;
            Task carrierCollectionTask = new Task().Fake().WithRegarding(incident.ToEntityReference()).WithTaskCategoryRef(taskCategory.ToEntityReference()).WithState(TaskState.Open).WithTypeRef(fullDenial.ToEntityReference());
            //ipg_payment payment = new ipg_payment().Fake().WithCaseReference(incident).PostedByCarrier();

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                CustomerId = carrier.ToEntityReference(),
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
            //int claimStatus = (int)ipg_ClaimStatus.Adjudicated;
            //int claimReason = (int)ipg_ClaimReason.AcceptedbyIntermediary;
            /*ipg_claimconfiguration claimConfiguration = new ipg_claimconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimEvent = new OptionSetValue((int)ipg_ClaimEvent._835fileprocessing),
                ipg_ClaimSubEvent = new OptionSetValue(headerType),
                ipg_ClaimStatus = new OptionSetValue(claimStatus),
                ipg_ClaimReason = new OptionSetValue(claimReason)
            };*/

            fakedContext.Initialize(new List<Entity> { payment, incident, taskCategory, carrierCollectionTask, carrier, batch, header, claimResponseLine, claim, claimLineItem, hcpcs, fullDenial, partialDenial, missingICN });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.AddFakeMessageExecutor<ipg_IPGTaskActionsCloseTaskRequest>(new FakeMessageExecutor<ipg_IPGTaskActionsCloseTaskRequest, ipg_IPGTaskActionsCloseTaskResponse>(
            (req, ctx) =>
            {
                var outputparameters = new ParameterCollection();
                var innerpluginContext = new XrmFakedPluginExecutionContext()
                {
                    MessageName = req.RequestName,
                    Stage = PipelineStages.PostOperation,
                    PrimaryEntityName = req.Target.LogicalName,
                    InputParameters = req.Parameters,
                    OutputParameters = outputparameters

                };

                fakedContext.ExecutePluginWith<CloseTaskActionPlugin>(innerpluginContext);

                return new ipg_IPGTaskActionsCloseTaskResponse() {Results = outputparameters };
            }));

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            Task taskFaked = fakedService.Retrieve(carrierCollectionTask.LogicalName, carrierCollectionTask.Id, new ColumnSet(Task.Fields.StateCode)).ToEntity<Task>();

            Assert.Equal(TaskState.Completed, taskFaked.StateCode);
        }

        [Fact]
        public void CheckThat_PromotedToCollectio2_PSTask_Not_Closed_And_CarrierPayment_Not_Created_WhenPatientPayment()
        {
            var fakedContext = new XrmFakedContext();
            Incident Case = new Incident().Fake();

            ipg_statementgenerationeventconfiguration taskconfig1 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment);
            ipg_statementgenerationeventconfiguration taskconfig2 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.PromotedToCollection2);

            ipg_statementgenerationtask promotedToCollection2task = new ipg_statementgenerationtask().Fake().WithCaseReference(Case).WithStatementEventConfig(taskconfig2);

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName(Constants.TaskCategoryNames.CarrierCollections);
            ipg_payment payment = new ipg_payment().Fake().WithCaseReference(Case).PostedByPatient();

            fakedContext.Initialize(new List<Entity> { payment, Case, taskCategory, promotedToCollection2task, taskconfig2, taskconfig1 });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var context = new CrmServiceContext(fakedService);
            ipg_statementgenerationtask taskFaked = fakedService.Retrieve(promotedToCollection2task.LogicalName, promotedToCollection2task.Id, new ColumnSet(ipg_statementgenerationtask.Fields.StateCode)).ToEntity<ipg_statementgenerationtask>();

            var PSCarrierPaymenTask = (from pstasks in context.ipg_statementgenerationtaskSet
                                       join configevent in context.ipg_statementgenerationeventconfigurationSet on pstasks.ipg_eventid.Id equals configevent.ipg_statementgenerationeventconfigurationId
                                       where pstasks.ipg_caseid.Id == Case.Id && configevent.ipg_name == PSEvents.CarrierPayment
                                       select pstasks).FirstOrDefault();

            Assert.Equal(ipg_statementgenerationtaskState.Active, taskFaked.StateCode);
            Assert.Null(PSCarrierPaymenTask);
        }

        [Fact]
        public void UpdatePatientOutreachTasks()
        {
            var fakedContext = new XrmFakedContext();
            var now = DateTime.Today;
            Incident incident = new Incident().Fake().WithPaymentPlanType((int)Incident_ipg_PaymentPlanType.AutoDraft);
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName(Constants.TaskCategoryNames.PatientOutreach);
            Task task = new Task().Fake().WithRegarding(incident.ToEntityReference()).WithTaskCategoryRef(taskCategory.ToEntityReference()).WithState(TaskState.Open).WithScheduledStart(now).WithScheduledEnd(now);
            ipg_payment payment = new ipg_payment().Fake().WithCaseReference(incident).PostedByPatient();

            fakedContext.Initialize(new List<Entity> { payment, incident, taskCategory, task });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var context = new CrmServiceContext(fakedService);

            var taskFaked = fakedService.Retrieve(task.LogicalName, task.Id, new ColumnSet(Task.Fields.ScheduledStart, Task.Fields.ScheduledEnd)).ToEntity<Task>();
            Assert.Equal(taskFaked.ScheduledStart, now.AddDays(30));
            Assert.Equal(taskFaked.ScheduledEnd, now.AddDays(30));
        }

        [Fact(Skip = "Relevant for integation tests only")]
        //[Fact]
        public void PaymentProcessOnRealDataTest()
        {
            //Use to catch specific error

            var fakedContext = new XrmRealContext();
            IOrganizationService service = fakedContext.GetOrganizationService();
            var payment = service.Retrieve(ipg_payment.EntityLogicalName, new Guid("{6906974F-C1B1-EC11-9840-000D3A349136}"), new ColumnSet(true)).ToEntity<ipg_payment>().ToEntity<ipg_payment>();

            var outputParams = new ParameterCollection();
            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);
        }

        [Fact]
        public void PartialDenialFromCarrier()
        {
            var fakedContext = new XrmFakedContext();

            int headerType = (int)ipg_ClaimSubEvent.PartialDenialFromCarrier;
            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.CLAIM_RESPONSE_PARTIAL_DENIAL).WithDescription("Claim response indicates a partial denial for Claim {0}. Please review this Case to determine next action.");
            ipg_tasktype taskTypeMissingICN = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_ICN_VALUE).WithDescription("Value of{0} missing"); ;
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid()
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                CustomerId = carrier.ToEntityReference(),
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
            int claimStatus = (int)ipg_ClaimStatus.Adjudicated;
            int claimReason = (int)ipg_ClaimReason.AcceptedbyIntermediary;
            ipg_claimconfiguration claimConfiguration = new ipg_claimconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimEvent = new OptionSetValue((int)ipg_ClaimEvent._835fileprocessing),
                ipg_ClaimSubEvent = new OptionSetValue(headerType),
                ipg_ClaimStatus = new OptionSetValue(claimStatus),
                ipg_ClaimReason = new OptionSetValue(claimReason)
            };
            ipg_claimstatuscode claimStatusCodeDenial = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "denial1 description"
            };
            ipg_claimstatuscode claimStatusCodeRemark = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "remark1 description"
            };
            Team team = new Team()
            {
                Id = Guid.NewGuid(),
                Name = "Carrier Services"
            };
            ipg_statementgenerationeventconfiguration configuration = new ipg_statementgenerationeventconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Promoted to Collections2",
                StateCode = (int)ipg_statementgenerationeventconfigurationState.Active,
                ipg_EventDueDaysStart = 1,
                ipg_EventDueDaysEnd = 1
            };

            fakedContext.Initialize(new List<Entity> { new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment), taskType, batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem, claimConfiguration, claimStatusCodeDenial, claimStatusCodeRemark, team, configuration, taskTypeMissingICN });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            ipg_claimlineitem claimLineItemFaked = fakedService.Retrieve(claimLineItem.LogicalName, claimLineItem.Id, new ColumnSet(nameof(ipg_claimlineitem.ipg_paid).ToLower())).ToEntity<ipg_claimlineitem>();
            Assert.Equal(claimLineItemFaked.ipg_paid.Value, amountPaid);

            Invoice claimFaked = fakedService.Retrieve(claim.LogicalName, claim.Id, new ColumnSet(nameof(Invoice.ipg_Status).ToLower(), nameof(Invoice.ipg_Reason).ToLower(), nameof(Invoice.ipg_AdjustmentCodesDescription).ToLower(), nameof(Invoice.ipg_RemarkCodesDescription).ToLower())).ToEntity<Invoice>();
            Assert.Equal(claimStatus, claimFaked.ipg_Status.Value);
            Assert.Equal(claimReason, claimFaked.ipg_Reason.Value);
            Assert.Equal("denial1(denial1 description)\ndenial2", claimFaked.ipg_AdjustmentCodesDescription);
            Assert.Equal("remark1\nremark2", claimFaked.ipg_RemarkCodesDescription);

            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_Reasons).ToLower())).ToEntity<Incident>();
            Assert.Equal((int)ipg_CaseReasons.CarrierCollectionsinProgress, incidentFaked.ipg_Reasons.Value);

            ipg_claimresponseheader headerFaked = fakedService.Retrieve(header.LogicalName, header.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower())).ToEntity<ipg_claimresponseheader>();
            Assert.Equal("posted", headerFaked.ipg_PostStatus);

            var notes = (from t in fakedContext.CreateQuery<Annotation>()
                         where (t.ObjectId.Id == incident.Id &&
                         t.Subject == "Claim processing" &&
                         t.NoteText == "Claim " + claim.Name + " partially denied by . Carrier collections process initiated."
                         )
                         select t).ToList();
            Assert.Single(notes);

            var tasks = (from t in fakedContext.CreateQuery<Task>()
                         where t.RegardingObjectId.Id == incident.Id && t.ipg_tasktypeid.Id == taskType.Id
                         select t).ToList();
            Assert.Single(tasks);
        }

        [Fact]
        public void FullDenialFromCarrier()
        {
            var fakedContext = new XrmFakedContext();

            int headerType = (int)ipg_ClaimSubEvent.FullDenialFromCarrier;
            decimal amountPaid = 100;
            string hcpcsName = "hcpcs";
            string denialCodeString = "denial1,denial2";
            string remarkCodeString = "remark1,remark2";

            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.CLAIM_RESPONSE_FULL_DENIAL).WithDescription("Claim response indicates a full denial for Claim {0}. Please review this Case to determine next action.");
            ipg_tasktype taskTypeMissingICN = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_ICN_VALUE).WithDescription("Value of{0} missing");
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident()
            {
                Id = Guid.NewGuid()
            };
            Invoice claim = new Invoice()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                CustomerId = carrier.ToEntityReference(),
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
            int claimStatus = (int)ipg_ClaimStatus.Adjudicated;
            int claimReason = (int)ipg_ClaimReason.AcceptedbyIntermediary;
            ipg_claimconfiguration claimConfiguration = new ipg_claimconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimEvent = new OptionSetValue((int)ipg_ClaimEvent._835fileprocessing),
                ipg_ClaimSubEvent = new OptionSetValue(headerType),
                ipg_ClaimStatus = new OptionSetValue(claimStatus),
                ipg_ClaimReason = new OptionSetValue(claimReason)
            };
            ipg_claimstatuscode claimStatusCodeDenial = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "denial1 description"
            };
            ipg_claimstatuscode claimStatusCodeRemark = new ipg_claimstatuscode()
            {
                Id = Guid.NewGuid(),
                ipg_name = "remark1 description"
            };
            Team team = new Team()
            {
                Id = Guid.NewGuid(),
                Name = "Carrier Services"
            };
            ipg_statementgenerationeventconfiguration configuration = new ipg_statementgenerationeventconfiguration()
            {
                Id = Guid.NewGuid(),
                ipg_name = "Promoted to Collections2",
                StateCode = (int)ipg_statementgenerationeventconfigurationState.Active,
                ipg_EventDueDaysStart = 1,
                ipg_EventDueDaysEnd = 1
            };

            fakedContext.Initialize(new List<Entity> { new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment), batch, incident, claim, header, claimResponseLine, payment, hcpcs, claimLineItem, claimConfiguration, claimStatusCodeDenial, claimStatusCodeRemark, team, configuration, taskType, taskTypeMissingICN });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<ProcessPayment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            ipg_claimlineitem claimLineItemFaked = fakedService.Retrieve(claimLineItem.LogicalName, claimLineItem.Id, new ColumnSet(nameof(ipg_claimlineitem.ipg_paid).ToLower())).ToEntity<ipg_claimlineitem>();
            Assert.Equal(0, claimLineItemFaked.ipg_paid.Value);

            Invoice claimFaked = fakedService.Retrieve(claim.LogicalName, claim.Id, new ColumnSet(nameof(Invoice.ipg_Status).ToLower(), nameof(Invoice.ipg_Reason).ToLower(), nameof(Invoice.ipg_AdjustmentCodesDescription).ToLower(), nameof(Invoice.ipg_RemarkCodesDescription).ToLower())).ToEntity<Invoice>();
            Assert.Equal(claimStatus, claimFaked.ipg_Status.Value);
            Assert.Equal(claimReason, claimFaked.ipg_Reason.Value);
            Assert.Equal("denial1(denial1 description)\ndenial2", claimFaked.ipg_AdjustmentCodesDescription);
            Assert.Equal("remark1\nremark2", claimFaked.ipg_RemarkCodesDescription);

            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_Reasons).ToLower())).ToEntity<Incident>();
            Assert.Equal((int)ipg_CaseReasons.CarrierCollectionsFullDenial, incidentFaked.ipg_Reasons.Value);

            ipg_claimresponseheader headerFaked = fakedService.Retrieve(header.LogicalName, header.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower())).ToEntity<ipg_claimresponseheader>();
            Assert.Equal("posted", headerFaked.ipg_PostStatus);

            var notes = (from t in fakedContext.CreateQuery<Annotation>()
                         where (t.ObjectId.Id == incident.Id &&
                         t.Subject == "Claim processing" &&
                         t.NoteText == "Claim " + claim.Name + " denied in full. Carrier collections process initiated."
                         )
                         select t).ToList();
            Assert.Single(notes);

            var tasks = (from t in fakedContext.CreateQuery<Task>()
                         where t.RegardingObjectId.Id == incident.Id && t.ipg_tasktypeid.Id == taskType.Id
                         select t).ToList();
            Assert.Single(tasks);
        }
    }
}
