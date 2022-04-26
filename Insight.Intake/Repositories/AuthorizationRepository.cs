using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace Insight.Intake.Repositories
{
    public class AuthorizationRepository
    {
        private readonly IOrganizationService _crmService;

        public AuthorizationRepository(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public IEnumerable<Entity> GetRecentActiveByCase(Guid caseId, ColumnSet columns, int topCount = 5000)
        {
            QueryExpression query = new QueryExpression(ipg_authorization.EntityLogicalName)
            {
                TopCount = topCount,
                ColumnSet = columns,
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_authorization.Fields.ipg_incidentid, ConditionOperator.Equal, caseId),
                        new ConditionExpression(ipg_authorization.Fields.StateCode, ConditionOperator.Equal, (int)ipg_authorizationState.Active)
                    }
                },
                Orders =
                {
                    new OrderExpression(ipg_authorization.Fields.CreatedOn, OrderType.Descending)
                }
            };

            return _crmService.RetrieveMultiple(query).Entities;
        }
    }
}
