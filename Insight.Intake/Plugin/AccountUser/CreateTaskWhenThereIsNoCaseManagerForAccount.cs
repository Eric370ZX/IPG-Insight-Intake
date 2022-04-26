using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.AccountUser
{
    public class CreateTaskWhenThereIsNoCaseManagerForAccount: PluginBase
    {
        public CreateTaskWhenThereIsNoCaseManagerForAccount() : base(typeof(CreateTaskWhenThereIsNoCaseManagerForAccount))
        {
            RegisterEvent(PipelineStages.PreOperation, "Delete", "ipg_accountuser", PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);

            //var accountUser = (EntityReference)context.InputParameters["Target"]; //PreImage
            var preEntity = ((Entity)context.PreEntityImages["PreImage"]).ToEntity<ipg_accountuser>();
            if (preEntity.ipg_rolecode?.Value == (int)ipg_accountuser_ipg_rolecode.CaseManager)
            {
                var existingCaseManagers = (from caseManager in crmContext.CreateQuery<ipg_accountuser>()
                                                  where caseManager.ipg_accountid.Id == preEntity.ipg_accountid.Id
                                                  && caseManager.Id != preEntity.Id
                                                  && caseManager.ipg_rolecode.Value == (int)ipg_accountuser_ipg_rolecode.CaseManager
                                                  select caseManager).ToList();

                if (existingCaseManagers.Count == 0)
                {
                    CreateTask(service, crmContext, preEntity, context.InitiatingUserId);
                }
            }

        }

        private void CreateTask(IOrganizationService service, OrganizationServiceContext crmContext, ipg_accountuser accountUser, Guid InitiatingUserId)
        {
            var caseManagersTeams = (from team in crmContext.CreateQuery<Team>()
                                    where team.Name.Contains("Case Management")
                                    select team).ToList();

            Task task = new Task();
            task.Subject = string.Format("The facility {0} does not have a Case Manager.", accountUser.ipg_accountid.Name);
            task.Description = "Please add a case manager to the facility.";
            task.RegardingObjectId = new EntityReference(accountUser.ipg_accountid.LogicalName, accountUser.ipg_accountid.Id);
            task.ScheduledEnd = DateTime.Now.AddDays(3);
            task.OwnerId = caseManagersTeams.Count > 0 ? caseManagersTeams.First().ToEntityReference() : new EntityReference(SystemUser.EntityLogicalName, InitiatingUserId);

            service.Create(task);
        }
    }
}
