using System;
using Microsoft.Xrm.Sdk;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Insight.Intake.Plugin.Gating;

namespace Insight.Intake.Plugin.GatingV3
{
    public class FillWorkflowTaskList : PluginBase
    {
        public FillWorkflowTaskList() : base(typeof(FillWorkflowTaskList))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var tracingService = localPluginContext.TracingService;
                var crmContext = new OrganizationServiceContext(service);
                var entity = (Entity)context.InputParameters["Target"];
                var gateManager = new GateManager(service, tracingService, entity.ToEntityReference());

                var caseNumber = gateManager.GetCaseNumber(entity, context.MessageName);
                if (string.IsNullOrEmpty(caseNumber))
                {
                    return;
                }

                if (string.Equals(entity.LogicalName, ipg_referral.EntityLogicalName))
                {
                    var workflowTasks = (from workflowTask in crmContext.CreateQuery<ipg_workflowtask>()
                                         select workflowTask);
                    foreach (var workflowTask in workflowTasks)
                    {
                        var record = new ipg_caseworkflowtask()
                        {
                            ipg_CaseNumber = caseNumber,
                            ipg_ReferralId = entity.ToEntityReference(),
                            ipg_WorkflowTaskId = workflowTask.ToEntityReference(),
                            ipg_AlwaysExecuted = workflowTask.ipg_AlwaysExecuted,
                            ipg_Passed = false
                        };
                        service.Create(record);
                    }
                }
                else if (string.Equals(entity.LogicalName, Incident.EntityLogicalName))
                {
                    var referral = entity.ToEntity<Incident>().ipg_ReferralId;
                    var caseWorkflowTasks = (from caseWorkflowTask in crmContext.CreateQuery<ipg_caseworkflowtask>()
                                             where caseWorkflowTask.ipg_ReferralId.Id == referral.Id
                                             select caseWorkflowTask);
                    foreach (var caseWorkflowTask in caseWorkflowTasks)
                    {
                        var record = new ipg_caseworkflowtask()
                        {
                            Id = caseWorkflowTask.Id,
                            ipg_CaseId = entity.ToEntityReference(),
                        };
                        service.Update(record);
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}