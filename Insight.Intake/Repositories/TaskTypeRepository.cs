using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace Insight.Intake.Repositories
{
    public class TaskTypeRepository
    {
        public IOrganizationService _crmService;

        public TaskTypeRepository(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public IEnumerable<Entity> GetByName(string name, ColumnSet columns)
        {
            QueryExpression query = new QueryExpression(ipg_tasktype.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = columns,
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_tasktype.Fields.ipg_name, ConditionOperator.Equal, name),
                        new ConditionExpression(ipg_tasktype.Fields.StateCode, ConditionOperator.Equal, (int)ipg_tasktypeState.Active)
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities;
        }
    }
}
