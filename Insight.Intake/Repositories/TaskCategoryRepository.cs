using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Repositories
{
    public class TaskCategoryRepository
    {
        private IOrganizationService _crmService;

        public TaskCategoryRepository(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public Entity GetByName(string name, ColumnSet columns)
        {
            QueryExpression query = new QueryExpression(ipg_taskcategory.EntityLogicalName)
            {
                ColumnSet = columns,
                TopCount = 1,
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_taskcategory.PrimaryNameAttribute, ConditionOperator.Equal, name),
                        new ConditionExpression(ipg_taskcategory.Fields.StateCode, ConditionOperator.Equal, (int)ipg_taskcategoryState.Active)
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities.FirstOrDefault();
        }
    }
}
