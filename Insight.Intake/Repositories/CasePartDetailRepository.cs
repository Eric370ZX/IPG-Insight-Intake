using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace Insight.Intake.Repositories
{
    public class CasePartDetailRepository
    {
        private readonly IOrganizationService _crmService;

        public CasePartDetailRepository(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public IReadOnlyCollection<Entity> GetActiveByOrder(Guid orderId, ColumnSet columns)
        {
            QueryExpression query = new QueryExpression(ipg_casepartdetail.EntityLogicalName)
            {
                ColumnSet = columns,
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_casepartdetail.Fields.StateCode, ConditionOperator.Equal, (int)ipg_casepartdetailState.Active),
                        new ConditionExpression(ipg_casepartdetail.Fields.ipg_PurchaseOrderId, ConditionOperator.Equal, orderId)
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities;
        }
    }
}
