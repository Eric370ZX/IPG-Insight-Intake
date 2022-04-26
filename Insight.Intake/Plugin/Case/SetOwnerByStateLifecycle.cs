using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Case
{
    public class SetOwnerByStateLifecycle : PluginBase
    {
        public SetOwnerByStateLifecycle() : base(typeof(CheckIfCaseCanBeClosed))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var incident = localPluginContext.PostImage<Incident>();

            //do not determine owners if state or Lifecycle step are empty
            if (incident.ipg_StateCodeEnum == null || incident.ipg_lifecyclestepid == null)
            {
                return;
            }

            var assignToRuleConfig = GetAssignToRuleConfig(incident.ipg_lifecyclestepid.Id, incident.ipg_StateCodeEnum, localPluginContext.SystemOrganizationService);
            if (assignToRuleConfig == null)
            {
                return;
            }
            var updIncident = new Incident() { Id = incident.Id };
            updIncident.ipg_assignedtoteamid = assignToRuleConfig.ipg_assigntoteamid;



            if (!string.IsNullOrEmpty(assignToRuleConfig.ipg_processname))
            {
                localPluginContext.OrganizationService.ExecuteWorkflow(assignToRuleConfig.ipg_processname, incident.Id);
            }
            else if (assignToRuleConfig.ipg_assigntoteamid != null)
            {
                updIncident.OwnerId = assignToRuleConfig.ipg_assigntoteamid;
            }
            localPluginContext.OrganizationService.Update(updIncident);
        }

        private ipg_assigntorule GetAssignToRuleConfig(Guid lifeCycleStepId, ipg_CaseStateCodes? caseState, IOrganizationService service)
        {
            var fetchXml = $@"
                <fetch top='1' >
                  <entity name='{ipg_assigntorule.EntityLogicalName}' >
                    <attribute name='{ipg_assigntorule.Fields.ipg_processname}' />
                    <attribute name='{ipg_assigntorule.Fields.ipg_assigntoteamid}' />
                    <attribute name='{ipg_assigntorule.Fields.OwnerId}' />
                    <filter>
                      <condition attribute='{ipg_assigntorule.Fields.StatusCode}' operator='eq' value='{(int)ipg_assigntorule_StatusCode.Active}' />
                      <condition attribute='{ipg_assigntorule.Fields.ipg_casestate}' operator='eq' value='{(int)caseState.Value}' />
                      <condition attribute='{ipg_assigntorule.Fields.ipg_lifecyclestepid}' operator='eq' value='{lifeCycleStepId}' />
                    </filter>
                  </entity>
                </fetch>";

            var config = service.RetrieveMultiple(new FetchExpression(fetchXml))?.Entities
            .Select(e => e.ToEntity<ipg_assigntorule>()).ToList().FirstOrDefault();

            return config;
        }
    }
}
