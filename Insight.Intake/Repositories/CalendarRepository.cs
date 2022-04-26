using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Repositories
{
    public class CalendarRepository
    {
        private readonly IOrganizationService _crmService;

        public CalendarRepository(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public Entity GetBusinessClosureCalendar()
        {
            QueryExpression query = new QueryExpression("calendar")
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, "Business Closure Calendar")
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities.FirstOrDefault();
        }
    }
}
