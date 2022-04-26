using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.ClaimResponse
{
    public class ProcessClaimResponseBatch : PluginBase
    {
        private IOrganizationService service;
        private ITracingService tracingService;

        public ProcessClaimResponseBatch() : base(typeof(ProcessClaimResponseBatch))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeActionsProcessClaimResponseBatch", ipg_claimresponsebatch.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                service = localPluginContext.OrganizationService;
                tracingService = localPluginContext.TracingService;

                tracingService.Trace($"{typeof(ProcessClaimResponseBatch)} plugin started");

                EntityReference batchRef = (EntityReference)context.InputParameters["Target"];
                bool patientPayment = PatientPayment(batchRef);

                var queryExpression = new QueryExpression(ipg_claimresponseheader.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimresponseheader.ipg_claimresponseheaderId).ToLower(), nameof(ipg_claimresponseheader.ipg_ClaimNumber).ToLower(), nameof(ipg_claimresponseheader.ipg_ClaimId).ToLower(), nameof(ipg_claimresponseheader.ipg_CaseId).ToLower(), nameof(ipg_claimresponseheader.ipg_CaseId).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(), ConditionOperator.Equal, batchRef.Id)
                        }
                    },
                    Orders =
                        {
                            new OrderExpression(nameof(ipg_claimresponseheader.ipg_Offset).ToLower(), OrderType.Ascending)
                        }
                };

                EntityCollection claimResponseHeaders = service.RetrieveMultiple(queryExpression);
                foreach (Entity header in claimResponseHeaders.Entities)
                {
                    if (header.GetAttributeValue<EntityReference>(nameof(ipg_claimresponseheader.ipg_CaseId).ToLower()) == null)
                    {
                        SetHeaderErrorStatus(header);
                        CreateWrongCaseClaimTask(header, "Match " + (patientPayment ? "Patient" : "Carrier") + " Payment to a Case");
                    }
                    else
                    {
                        Incident incident = service.Retrieve(Incident.EntityLogicalName, header.GetAttributeValue<EntityReference>(nameof(ipg_claimresponseheader.ipg_CaseId).ToLower()).Id, new ColumnSet(nameof(Incident.ipg_CaseStatus).ToLower())).ToEntity<Incident>();
                        if (incident.ipg_CaseStatus?.Value == (int)ipg_CaseStatus.Closed)
                        {
                            SetHeaderErrorStatus(header);
                            CreateClosedCaseTask(incident.ToEntityReference());
                        }
                        else if (!patientPayment)
                        {
                            ProcessCarrierClaimHeader(header);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private void SetHeaderErrorStatus(Entity header)
        {
            header[nameof(ipg_claimresponseheader.ipg_PostStatus).ToLower()] = "error";
            service.Update(header);
        }

        private void ProcessCarrierClaimHeader(Entity header)
        {
            EntityReference claimRef = header.GetAttributeValue<EntityReference>(nameof(ipg_claimresponseheader.ipg_ClaimId).ToLower());
            if (claimRef != null)
            {
                CreatePayment(header, claimRef);
            }
            else
            {
                SetHeaderErrorStatus(header);
                CreateWrongCaseClaimTask(header, "Match Carrier Payment to a Claim");
            }
        }

        private bool PatientPayment(EntityReference batchRef)
        {
            var batch = service.Retrieve(batchRef.LogicalName, batchRef.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_PaymentFrom).ToLower())).ToEntity<ipg_claimresponsebatch>();
            return ((batch.ipg_PaymentFrom != null) && (batch.ipg_PaymentFrom.Value == (int)ipg_PaymentFrom.Patient));
        }

        private void CreatePayment(Entity header, EntityReference claim)
        //to do: fill payment fields including ipg_ar_carrierpaid, ipg_ar_memberpd, ipg_memberrespnew and so on
        {
            tracingService.Trace("Payment creation");
            Invoice claimEnt = service.Retrieve(claim.LogicalName, claim.Id, new ColumnSet(nameof(Invoice.ipg_caseid).ToLower())).ToEntity<Invoice>();
            ipg_claimresponseheader headerEnt = service.Retrieve(header.LogicalName, header.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_Interest_new).ToLower())).ToEntity<ipg_claimresponseheader>();
            decimal interest = (headerEnt.GetAttributeValue<Money>(nameof(ipg_claimresponseheader.ipg_Interest_new).ToLower()) == null ? 0 : headerEnt.GetAttributeValue<Money>(nameof(ipg_claimresponseheader.ipg_Interest_new).ToLower()).Value);
            var payment = new ipg_payment()
            {
                ipg_CaseId = claimEnt.ipg_caseid,
                ipg_Claim = claim,
                ipg_ClaimResponseHeader = header.ToEntityReference(),
                ipg_PaymentType = new OptionSetValue((int)ipg_PaymentType.Actual), //actual payment
                ipg_InterestPayment = new Money(interest)
            };
            service.Create(payment);
        }

        private void CreateWrongCaseClaimTask(Entity header, string subject)
        {
            var crmContext = new OrganizationServiceContext(service);
            var financeTeams = (from team in crmContext.CreateQuery<Team>()
                                where team.Name.Contains("Finance")
                                select team).ToList();
            if (financeTeams.Count > 0)
            {
                Task task = new Task();
                task.Subject = subject;
                task.PriorityCode = new OptionSetValue((int)Task_PriorityCode.Normal);
                task.RegardingObjectId = header.ToEntityReference();
                task.OwnerId = financeTeams.First().ToEntityReference();
                task.ScheduledStart = System.DateTime.Now;
                service.Create(task);
            }
        }

        private void CreateClosedCaseTask(EntityReference caseRef)
        {
            var crmContext = new OrganizationServiceContext(service);
            var teams = (from team in crmContext.CreateQuery<Team>()
                                where team.Name.Contains("Cash Posting")
                                select team).ToList();
            if (teams.Count > 0)
            {
                Task task = new Task();
                task.Subject = "Payment or adjustment transaction pending for a closed case";
                task.Description = "This case is currently closed. Therefore, payment or adjustment transactions cannot be applied at this time. Please reopen the case to proceed.";
                task.PriorityCode = new OptionSetValue((int)Task_PriorityCode.Normal);
                task.RegardingObjectId = caseRef;
                task.OwnerId = teams.First().ToEntityReference();
                task.ScheduledStart = DateTime.Now;
                task.ScheduledEnd = DateTime.Now.AddDays(1);
                service.Create(task);
            }
        }
    }
}
