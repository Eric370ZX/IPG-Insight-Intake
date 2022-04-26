using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.CptCode
{
    public class SetProcedureNameOnCase : PluginBase
    {
        private IOrganizationService _service;
        private OrganizationServiceContext _crmContext;

        public SetProcedureNameOnCase() : base(typeof(SetProcedureNameOnCase))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_cptcode.EntityLogicalName, PostUpdateHandler);
        }

        private void PostUpdateHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.PostImage<ipg_cptcode>();
            var procedureRef = target?.ipg_procedurename;
            if (procedureRef == null) return;

            _service = localPluginContext.OrganizationService;
            var relatedIncidents = GetRelatedIncidents(_service, target.Id);
            if (relatedIncidents != null && relatedIncidents.Count > 0)
            {
                foreach (var inc in relatedIncidents)
                {
                    var incidentUpd = new Entity(Incident.EntityLogicalName, inc.Id).ToEntity<Incident>();
                    incidentUpd.ipg_procedureid = procedureRef;
                    _service.Update(incidentUpd);
                }
            }
        }

        private List<Entity> GetRelatedIncidents(IOrganizationService service, Guid cptCodeId)
        {
            var query = new QueryExpression(Incident.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(Incident.Fields.ipg_BilledCPTId, ConditionOperator.Equal, cptCodeId),
                        new ConditionExpression(Incident.Fields.StateCode, ConditionOperator.Equal, 0),
                        new ConditionExpression(Incident.Fields.ipg_CaseStatus, ConditionOperator.Equal, (int) ipg_CaseStatus.Open)
                    }
                }
            };
            return service.RetrieveMultiple(query).Entities?.ToList();
        }
    }
}
