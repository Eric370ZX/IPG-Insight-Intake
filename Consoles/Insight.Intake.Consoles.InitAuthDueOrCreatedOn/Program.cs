using Insight.Intake.Consoles.InitAuthDueOrCreatedOn.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Consoles.InitAuthDueOrCreatedOn
{
    class Program
    {
        static void Main(string[] args)
        {
            var crmService = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(ConfigurationManager.ConnectionStrings["DevCrm"].ConnectionString);

            new IncidentService(crmService)
                .ProcessWithoutAuthDueDates();

            //new IncidentService(crmService)
            //    .ProcessRecordsWithAuthTasks();
        }
    }
}
