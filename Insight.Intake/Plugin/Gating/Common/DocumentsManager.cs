using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Gating.Common
{
    public class DocumentsManager
    {
        private IOrganizationService _service;

        public DocumentsManager(IOrganizationService service)
        {
            _service = service;
        }

        internal List<ipg_document> GetTargetDocuments(EntityReference targetRef)
        {
            var query = new QueryExpression(ipg_document.EntityLogicalName);
            query.ColumnSet = new ColumnSet(true);
            var parentReferenceField = targetRef.LogicalName == Incident.EntityLogicalName ?
                nameof(ipg_document.ipg_CaseId).ToLower()
                : nameof(ipg_document.ipg_ReferralId).ToLower();
            query.Criteria.AddCondition(parentReferenceField, ConditionOperator.Equal, targetRef.Id);
            query.Criteria.AddCondition(nameof(ipg_document.StateCode).ToLower(), ConditionOperator.Equal, (int)ipg_documentState.Active);
            var result = _service.RetrieveMultiple(query);
            return result.Entities
                .Select(p => p.ToEntity<ipg_document>())
                .ToList();


        }
    }
}
