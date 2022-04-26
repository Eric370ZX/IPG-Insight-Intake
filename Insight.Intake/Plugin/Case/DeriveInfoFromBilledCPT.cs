using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Case
{
    public class DeriveInfoFromBilledCPT: PluginBase
    {
        public DeriveInfoFromBilledCPT():base(typeof(DeriveInfoFromBilledCPT))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, DeriveOnCase);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Incident.EntityLogicalName, DeriveOnCase);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_cptcode.EntityLogicalName, DeriveOnCPTChange);
        }

        private void DeriveOnCPTChange(LocalPluginContext localContext)
        {
            var target = localContext.Target<ipg_cptcode>();
            var postImage = localContext.PostImage<ipg_cptcode>();

            if (target.Attributes.Contains(ipg_cptcode.Fields.ipg_procedurename))
            {
                var incidents = localContext.OrganizationService.RetrieveMultiple(new QueryExpression(Incident.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(false),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression(Incident.Fields.StateCode, ConditionOperator.Equal, (int)IncidentState.Active),
                            new ConditionExpression(Incident.Fields.ipg_CaseStatus, ConditionOperator.Equal, (int)ipg_CaseStatus.Open),
                            new ConditionExpression(Incident.Fields.ipg_BilledCPTId, ConditionOperator.Equal, target.Id),
                        }
                    }
                }).Entities;

                var updateCase = new Incident()
                {
                    ipg_procedureid = postImage.ipg_procedurename
                };

                foreach (var incident in incidents)
                {
                    updateCase.Id = incident.Id;
                    localContext.OrganizationService.Update(updateCase);
                }
            }
        }

        private void DeriveOnCase(LocalPluginContext localContext)
        {
            var target = localContext.Target<Incident>();
            var service = localContext.OrganizationService;

            if (target.Contains(Incident.Fields.ipg_BilledCPTId) && target.ipg_BilledCPTId != null)
            {
                var billedcpt = target.ipg_BilledCPTId != null ?  service.Retrieve(target.ipg_BilledCPTId.LogicalName, target.ipg_BilledCPTId.Id
              , new ColumnSet(ipg_cptcode.Fields.ipg_procedurename)).ToEntity<ipg_cptcode>() : null;

                target.ipg_procedureid = billedcpt?.ipg_procedurename;
            }
        }
    }
}
