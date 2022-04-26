using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class AssociateCaseToReferralTasks : PluginBase
    {
        public AssociateCaseToReferralTasks() : base(typeof(AssociateCaseToReferralTasks))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var incident = localPluginContext.Target<Incident>();
            if (incident.Contains(Incident.Fields.ipg_ReferralId) && incident.ipg_ReferralId != null)
            {
                var multipleUpdateRequest = new ExecuteMultipleRequest
                {
                    Settings = new ExecuteMultipleSettings()
                    {
                        ContinueOnError = false,
                        ReturnResponses = false
                    },
                    Requests = new OrganizationRequestCollection()
                };

                var tasks = service.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(false),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, incident.ipg_ReferralId.Id)
                        }
                    }
                }).Entities;
                if (tasks.Count > 0)
                {
                    foreach (var t in tasks)
                    {
                        var task = t.ToEntity<Task>();
                        task.ipg_caseid = incident.ToEntityReference();
                        multipleUpdateRequest.Requests.Add(new UpdateRequest { Target = task });
                    }
                }
                service.Execute(multipleUpdateRequest);
            }
        }
    }
}
