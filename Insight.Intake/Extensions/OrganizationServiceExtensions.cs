using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Extensions
{
    public static class OrganizationServiceExtensions
    {
        public static OrganizationResponse ExecuteAction(this IOrganizationService _service, string processUniqueName, Dictionary<string, object> parameters = null)
        {
            OrganizationRequest organizationRequest = new OrganizationRequest(processUniqueName);
            if (parameters != null)
            {
                foreach (var iParam in parameters)
                {
                    organizationRequest[iParam.Key] = iParam.Value;
                }
            }
            OrganizationResponse response = _service.Execute(organizationRequest);

            return response;
        }

        public static Guid ExecuteWorkflow(this IOrganizationService service, string workflowName, Guid entityId) =>
            ((ExecuteWorkflowResponse)service.Execute(new ExecuteWorkflowRequest
            {
                WorkflowId = service.RetrieveWorkflowId(workflowName),
                EntityId = entityId
            })).Id;

        public static Guid ExecuteWorkflow(this IOrganizationService service, Guid workflowId, Guid entityId) =>
            ((ExecuteWorkflowResponse)service.Execute(new ExecuteWorkflowRequest
            {
                WorkflowId = workflowId,
                EntityId = entityId
            })).Id;

        public static Guid RetrieveWorkflowId(this IOrganizationService service, string workflowName) =>
            service.RetrieveMultiple(new QueryExpression("workflow")
            {
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, workflowName),
                        new ConditionExpression("type", ConditionOperator.Equal, 1)
                    }
                }
            }).Entities.FirstOrDefault()?.Id ?? Guid.Empty;

        public static DateTime ConvertToLocalTime(this IOrganizationService _service, Guid userId, DateTime datetimeToConvert)
        {
            var timeZone = _service.RetrieveMultiple(
             new QueryExpression("usersettings")
             {
                 ColumnSet = new ColumnSet("localeid", "timezonecode"),
                 Criteria = new FilterExpression
                 {
                     Conditions =
                        {
                            new ConditionExpression("systemuserid", ConditionOperator.Equal, userId)
                        }
                 }
             }
             )
             .Entities[0]
             .ToEntity<Entity>()
             .GetAttributeValue<int>("timezonecode");

            var request = new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = timeZone,
                UtcTime = datetimeToConvert.ToUniversalTime()
            };
            var response = (LocalTimeFromUtcTimeResponse)_service.Execute(request);
            return response.LocalTime;
        }

        public static T Retrieve<T>(this IOrganizationService service, string logicalName, Guid id, ColumnSet columns) where T : Entity
        {
            return service.Retrieve(logicalName, id, columns).ToEntity<T>();
        }

        public static T Retrieve<T>(this IOrganizationService service, Guid id, ColumnSet columns) where T : Entity
        {
            T instance = (T)Activator.CreateInstance(typeof(T));
            return service.Retrieve(instance.LogicalName, id, columns).ToEntity<T>();
        }
    }
}
