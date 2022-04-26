using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.Common
{
    public class GatingDocumentManager
    {
        private readonly IOrganizationService _service;

        public GatingDocumentManager(IOrganizationService service)
        {
            _service = service;
        }

        internal List<ipg_documentbygate> GetGateDocuments(ipg_gateconfiguration gate)
        {
            var query = new QueryExpression(ipg_documentbygate.EntityLogicalName);
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition(nameof(ipg_documentbygate.ipg_gateid), ConditionOperator.Equal, gate.Id);
            query.Criteria.AddCondition(nameof(ipg_documentbygate.StateCode).ToLower(), ConditionOperator.Equal, (int)ipg_documentbygateState.Active);
            var result = _service.RetrieveMultiple(query);
            return result.Entities
                .Select(p => p.ToEntity<ipg_documentbygate>())
                .ToList();
        }
    }
}
