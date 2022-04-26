using Insight.Intake.Plugin.Gating.Constants;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using static Insight.Intake.Helpers.Constants;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.Plugin.Case
{
    public class CaseIsUnlocked : PluginBase
    {
        public CaseIsUnlocked() : base(typeof(CaseIsUnlocked))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var traceService = localPluginContext.TracingService;
            var target = localPluginContext.Target<Incident>();
            var tragetRef = target.ToEntityReference();
            var taskMgr = new TaskManager(service, traceService);

            if (!IsCaseUnlocked(target))
            {
                return;
            }

            CancelPatientStatementTasks(service, target);
            taskMgr.CancelCategoryTasks(tragetRef, TaskStatusReasonNames.CaseUnlocked, TaskCategoryNames.CarrierOutreach, TaskCategoryNames.PatientOutreach);
            taskMgr.CompleteTasks(tragetRef, TaskTypeIds.REQUEST_TO_UNLOCK_CASE);
            taskMgr.CancelTasks(tragetRef, TaskStatusReasonNames.CaseUnlocked, TaskTypeIds.GENERATE_SUBMIT_CLAIM);

            UpdateRecordDueToGatingProcessingRule(service, target);
        }
        private void CancelPatientStatementTasks(IOrganizationService service, Incident target)
        {
            var targetRef = target.ToEntityReference();

            new OrganizationServiceContext(service).CreateQuery<ipg_statementgenerationtask>()
                .Where(statementTask => statementTask.ipg_caseid == targetRef)
                .ToList()
                .ForEach(statementTask =>
                {
                    service.Update(new ipg_statementgenerationtask()
                    {
                        Id = statementTask.Id,
                        StatusCodeEnum = ipg_statementgenerationtask_StatusCode.Canceled,
                        StateCode = ipg_statementgenerationtaskState.Inactive
                    });
                });
        }

        private bool IsCaseUnlocked(Incident target)
        {
            return target?.ipg_islocked == false;
        }

        private ipg_gateprocessingrule GetGatingProcessingRule(IOrganizationService crmService, Guid lcStepId, ipg_SeverityLevel severityLevel)
        {
            var query = new QueryExpression(ipg_gateprocessingrule.EntityLogicalName);
            query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_severitylevel, ConditionOperator.Equal, (int)severityLevel);
            query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_lifecyclestepid, ConditionOperator.Equal, lcStepId);
            query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.StateCode, ConditionOperator.Equal, (int)ipg_gateprocessingruleState.Active);
            query.ColumnSet = new ColumnSet(true);

            var rules = crmService.RetrieveMultiple(query);
            var result = rules
                .Entities
                .Select(p => p.ToEntity<ipg_gateprocessingrule>())
                .FirstOrDefault();

            return result;
        }

        private void UpdateRecordDueToGatingProcessingRule(IOrganizationService service,  Incident target)
        {
            var targetProcessingRule = GetGatingProcessingRule(service, LifeCycleStep.ADD_PARTS_GATE6, ipg_SeverityLevel.Info);
            service.Update(new Incident()
            {
                Id = target.Id,
                ipg_casestatusdisplayedid = targetProcessingRule.ipg_casestatusdisplayedid,
                ipg_CaseStatusEnum = targetProcessingRule.ipg_casestatusEnum,
                ipg_StateCodeEnum = targetProcessingRule.ipg_casestateEnum,
                ipg_lifecyclestepid = targetProcessingRule.ipg_lifecyclestepid,
                ipg_caseunlockedon = DateTime.Now,
                ipg_ReasonsEnum = ipg_CaseReasons.ProcedureCompletePendingInformation
            });

        }
    }
}