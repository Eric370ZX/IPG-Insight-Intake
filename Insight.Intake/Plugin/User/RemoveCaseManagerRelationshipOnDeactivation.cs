using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.User
{
    public class RemoveCaseManagerRelationshipOnDeactivation: PluginBase
    {
        public RemoveCaseManagerRelationshipOnDeactivation() : base(typeof(RemoveCaseManagerRelationshipOnDeactivation))
        {
            RegisterEvent(PipelineStages.PostOperation, "Update", "systemuser", PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);

            var user = ((Entity)context.InputParameters["Target"]).ToEntity<SystemUser>();

            if (user.IsDisabled == true) {
                var accountUsers = (from accountUser in crmContext.CreateQuery<ipg_accountuser>()
                                    where accountUser.ipg_rolecode.Value == (int)ipg_accountuser_ipg_rolecode.CaseManager
                                    && accountUser.ipg_userid.Id == user.Id
                                    select accountUser).ToList();

                foreach (var accountUser in accountUsers)
                {
                    service.Delete(accountUser.LogicalName, accountUser.Id);
                }
            }
        }
    }
}
