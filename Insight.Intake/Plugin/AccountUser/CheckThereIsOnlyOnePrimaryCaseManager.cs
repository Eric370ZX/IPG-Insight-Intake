using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.AccountUser
{
    public class CheckThereIsOnlyOnePrimaryCaseManager :PluginBase
    {
        public CheckThereIsOnlyOnePrimaryCaseManager() : base(typeof(CheckThereIsOnlyOnePrimaryCaseManager))
        {
            RegisterEvent(PipelineStages.PreOperation, "Create", "ipg_accountuser", PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);

            var accountUser = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_accountuser>();

            if (accountUser.ipg_rolecode?.Value == (int)ipg_accountuser_ipg_rolecode.CaseManager)
            {
                var existingPrimaryCaseManagers = (from caseManager in crmContext.CreateQuery<ipg_accountuser>()
                                                  where caseManager.ipg_accountid.Id == accountUser.ipg_accountid.Id
                                                  && caseManager.ipg_rolecode.Value == (int)ipg_accountuser_ipg_rolecode.CaseManager
                                                  && caseManager.ipg_isprimary == true
                                                  select caseManager).ToList();

                if (existingPrimaryCaseManagers.Count > 0 && accountUser.ipg_isprimary == true)
                {
                    throw new Exception("Primary case manager already exists!");
                }
                else if (existingPrimaryCaseManagers.Count == 0 && accountUser.ipg_isprimary != true) {
                    accountUser.ipg_isprimary = true;
                }
            }
        }
    }
}
