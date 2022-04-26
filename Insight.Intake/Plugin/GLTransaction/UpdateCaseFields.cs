using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

namespace Insight.Intake.Plugin.GLTransaction
{
    public class UpdateCaseFields : PluginBase
    {

        public UpdateCaseFields() : base(typeof(UpdateCaseFields))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_GLTransaction.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_GLTransaction.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Delete, ipg_GLTransaction.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                ipg_GLTransaction transactionRef = null;
                if (context.InputParameters["Target"] is Entity)
                {
                    var transaction = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_GLTransaction>();
                    transactionRef = service.Retrieve(transaction.LogicalName, transaction.Id, new ColumnSet(nameof(ipg_GLTransaction.ipg_IncidentId).ToLower())).ToEntity<ipg_GLTransaction>();
                }
                else
                {
                    var transaction = ((EntityReference)context.InputParameters["Target"]);
                    transactionRef = service.Retrieve(transaction.LogicalName, transaction.Id, new ColumnSet(nameof(ipg_GLTransaction.ipg_IncidentId).ToLower())).ToEntity<ipg_GLTransaction>();
                }

                var queryExpression = new QueryExpression(ipg_GLTransaction.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_GLTransaction.ipg_PayorRevenue).ToLower(), nameof(ipg_GLTransaction.ipg_PayorAdjustment).ToLower(), nameof(ipg_GLTransaction.ipg_PayorCashAmount).ToLower(), nameof(ipg_GLTransaction.ipg_PayorWriteOff).ToLower(), nameof(ipg_GLTransaction.ipg_PatientRevenue).ToLower(), nameof(ipg_GLTransaction.ipg_PatientAdjustment).ToLower(), nameof(ipg_GLTransaction.ipg_PatientCashAmount).ToLower(), nameof(ipg_GLTransaction.ipg_PatientWriteOff).ToLower(), nameof(ipg_GLTransaction.ipg_IsSecondaryCarrier).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_GLTransaction.ipg_IncidentId).ToLower(), ConditionOperator.Equal, transactionRef.ipg_IncidentId.Id)
                        }
                    }
                };
                if (context.MessageName == MessageNames.Delete)
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_GLTransaction.ipg_GLTransactionId).ToLower(), ConditionOperator.NotEqual, transactionRef.Id));
                EntityCollection payments = service.RetrieveMultiple(queryExpression);
                decimal carrierSum = 0;
                decimal secondaryCarrierSum = 0;
                decimal memberSum = 0;
                decimal totalBilledToCarrier = 0;
                decimal totalBilledToPatient = 0;
                List<string> list = new List<string>();
                foreach (Entity entity in payments.Entities)
                {
                    if (entity.GetAttributeValue<bool>(nameof(ipg_GLTransaction.ipg_IsSecondaryCarrier).ToLower()) == true)
                    {
                        secondaryCarrierSum += (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorRevenue).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorRevenue).ToLower()).Value)
                            + (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorAdjustment).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorAdjustment).ToLower()).Value)
                            - (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorCashAmount).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorCashAmount).ToLower()).Value)
                            - (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorWriteOff).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorWriteOff).ToLower()).Value);
                    }
                    else
                    {
                        carrierSum += (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorRevenue).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorRevenue).ToLower()).Value)
                            + (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorAdjustment).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorAdjustment).ToLower()).Value)
                            - (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorCashAmount).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorCashAmount).ToLower()).Value)
                            - (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorWriteOff).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorWriteOff).ToLower()).Value);
                    }
                    totalBilledToCarrier += (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorRevenue).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PayorRevenue).ToLower()).Value);
                    memberSum += (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientRevenue).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientRevenue).ToLower()).Value)
                        + (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientAdjustment).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientAdjustment).ToLower()).Value)
                        - (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientCashAmount).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientCashAmount).ToLower()).Value)
                        - (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientWriteOff).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientWriteOff).ToLower()).Value);
                    totalBilledToPatient += (entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientRevenue).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_GLTransaction.ipg_PatientRevenue).ToLower()).Value);

                }

                Incident incidentEnt = service.Retrieve(Incident.EntityLogicalName,
                                                     transactionRef.ipg_IncidentId.Id,
                                                     new ColumnSet(true))
                                                     .ToEntity<Incident>();

                var incident = new Incident();
                incident.Id = transactionRef.ipg_IncidentId.Id;
                incident.ipg_RemainingCarrierBalance = new Money(carrierSum);
                incident.ipg_RemainingSecondaryCarrierBalance = new Money(secondaryCarrierSum);
                incident.ipg_RemainingPatientBalance = new Money(memberSum);
                incident.ipg_TotalBilledtoCarrier = new Money(totalBilledToCarrier);
                incident.ipg_TotalBilledtoPatient = new Money(totalBilledToPatient);
                incident.ipg_CaseBalance = new Money(carrierSum + secondaryCarrierSum + memberSum);
                service.Update(incident);

                if (incidentEnt.ipg_BillToPatient == new OptionSetValue((int)ipg_TwoOptions.No)
                    && memberSum > 0)
                {
                    CreateTaskForCollectionTeam(service, incident);
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private void CreateTaskForCollectionTeam(IOrganizationService service, Incident incident)
        {
            var crmContext = new OrganizationServiceContext(service);
            var collectionTeams = (from team in crmContext.CreateQuery<Team>()
                                   where team.Name.Contains("Carrier Services")
                                   select team).ToList();

            Task task = new Task();
            task.Subject = "Balance moved to patient even though bill to patient is false on the case.";
            task.PriorityCode = new OptionSetValue((int)Task_PriorityCode.Normal);
            task.Description = "Please confirm.";
            task.RegardingObjectId = new EntityReference(incident.LogicalName, incident.Id);
            task.ScheduledEnd = DateTime.Now.AddDays(10);
            task.OwnerId = collectionTeams.First().ToEntityReference();
            service.Create(task);
        }
    }
}