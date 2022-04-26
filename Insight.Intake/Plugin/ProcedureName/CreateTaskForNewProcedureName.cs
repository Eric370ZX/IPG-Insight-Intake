using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.ProcedureName
{
    public class CreateTaskForNewProcedureName: PluginBase
    {
        public CreateTaskForNewProcedureName() : base(typeof(CreateTaskForNewProcedureName))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_procedurename.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);

            var procedureName = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_procedurename>(); 
            
            CreateTask(service, crmContext, procedureName, context.InitiatingUserId);
        }

        private void CreateTask(IOrganizationService service, OrganizationServiceContext crmContext, ipg_procedurename procedureName, Guid InitiatingUserId)
        {
            var implementationMgrTeams = (from team in crmContext.CreateQuery<Team>()
                                     where team.Name.Contains(Constants.TeamNames.ImplementationMgr)
                                     select team).ToList();

            Task task = new Task();
            task.Subject = string.Format("Please verify the Procedure Name '{0}'", procedureName.ipg_name);
            task.Description = "Please verify the Procedure Name.";
            task.RegardingObjectId = new EntityReference(procedureName.LogicalName, procedureName.Id);
            task.ScheduledEnd = DateTime.Now.AddDays(2);
            task.ipg_assignedtoteamid = implementationMgrTeams.Count > 0 ? implementationMgrTeams.First().ToEntityReference() : new EntityReference(SystemUser.EntityLogicalName, InitiatingUserId);

            service.Create(task);
        }
    }
}