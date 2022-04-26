using Insight.Intake.Plugin.Managers;
using Insight.Intake.Helpers;
using Insight.Intake.Repositories;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.Case
{
    public class InitPatientOutreachTaskOnStatement : PluginBase
    {
        public InitPatientOutreachTaskOnStatement() : base(typeof(InitPatientOutreachTaskOnStatement))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_statementgenerationtask.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var tracing = localPluginContext.TracingService;
            var taskManager = new TaskManager(service, tracing);
            var docRepo = new DocumentRepository(service);

            var target = localPluginContext.Target<Entity>();

            if(target.LogicalName == ipg_statementgenerationtask.EntityLogicalName)
            {
                tracing.Trace("Triggered on PS Event Created");

                var statementevent = target.ToEntity<ipg_statementgenerationtask>();
                var incident = service.Retrieve(Incident.EntityLogicalName, statementevent.ipg_caseid.Id, new ColumnSet(Incident.Fields.ipg_DoNotContactPatient)).ToEntity<Incident>();
                var openA2Docs = docRepo.GetActivePSDocs(statementevent.ipg_caseid, PSType.A2);

                if (statementevent.ipg_caseid != null && statementevent.ipg_eventid != null && incident.ipg_DoNotContactPatient == false && openA2Docs.Count == 1)
                {
                    var taskReasonDetail = taskManager.GetTaskReasonDetailOnStatementEventRef(statementevent.ipg_eventid);
                    if(taskReasonDetail != null)
                    {
                        tracing.Trace("Task Type found");
                        taskManager.CloseCategoryTasks(statementevent.ipg_caseid.Id, TaskCategoryNames.PatientOutreach);
                        taskManager.CreateTask(incident.ToEntityReference(), taskManager.GetTaskTemplateFromTaskReasonDetail(taskReasonDetail));

                    }
                }
            }
            else
            {
                tracing.Trace("Triggered on Case update");
                var incident = service.Retrieve(Incident.EntityLogicalName, target.ToEntity<Incident>().Id
                    , new ColumnSet(Incident.Fields.ipg_DoNotContactPatient, Incident.Fields.ipg_StateCode)).ToEntity<Incident>();

                var preIncident = localPluginContext.PreImage<Incident>();
                              
                if (incident.ipg_StateCodeEnum == ipg_CaseStateCodes.PatientServices && preIncident.ipg_DoNotContactPatient != incident.ipg_DoNotContactPatient)
                {
                    if(incident.ipg_DoNotContactPatient == false)
                    {
                        tracing.Trace("ipg_DoNotContactPatient field changed");

                        var taskReasonDetail = taskManager.GetTaskReasonDetailOnStatementEventName(PSEvents.A2Generated);

                        if (taskReasonDetail != null)
                        {
                            tracing.Trace("Task Type found");
                            taskManager.CreateTask(incident.ToEntityReference(), taskManager.GetTaskTemplateFromTaskReasonDetail(taskReasonDetail));
                        }
                    }
                }
            }
        }
    }
}