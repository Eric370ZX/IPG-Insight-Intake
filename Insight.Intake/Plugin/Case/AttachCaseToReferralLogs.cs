using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class AttachCaseToReferralLogs : PluginBase
    {
        public AttachCaseToReferralLogs() : base(typeof(AttachCaseToReferralLogs))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var caseEntity = localPluginContext.Target<Incident>();
            var referralRef = caseEntity.ipg_ReferralId;

            if (referralRef != null)
            {
                var referralLogs = RetrieveReferralLogs(referralRef.Id, service);
                if (referralLogs.Entities.Count > 0)
                {
                    var importantEventLog = new ipg_importanteventslog();
                    foreach (var referralLog in referralLogs.Entities.Cast<ipg_importanteventslog>())
                    {
                        importantEventLog.Id = referralLog.Id;
                        importantEventLog.ipg_caseid = Helpers.D365Helpers.GetIdAsString(caseEntity.Id);
                        service.Update(importantEventLog);
                    }
                }
            }
        }

        private EntityCollection RetrieveReferralLogs(Guid referralId, IOrganizationService service)
        {
            var referralLogsQuery = new QueryExpression(ipg_importanteventslog.EntityLogicalName);
            referralLogsQuery.ColumnSet = new ColumnSet(ipg_importanteventslog.Fields.ipg_caseid);
            referralLogsQuery.Criteria = new FilterExpression();
            referralLogsQuery.Criteria.AddCondition(ipg_importanteventslog.Fields.ipg_referralid, ConditionOperator.Equal, Helpers.D365Helpers.GetIdAsString(referralId));

            return service.RetrieveMultiple(referralLogsQuery);
        }
    }
}
