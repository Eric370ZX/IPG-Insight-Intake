using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;

namespace Insight.Intake.Plugin.ClaimResponseHeader
{
    public class CloseMatchClaimCaseTask : PluginBase
    {

        public CloseMatchClaimCaseTask() : base(typeof(CreateClaimResponseLinesFromClaim))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimresponseheader.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var crmContext = new OrganizationServiceContext(service);
                var claimResponseHeader = context.PostEntityImages["PostImage"].ToEntity<ipg_claimresponseheader>();

                if(claimResponseHeader.ipg_ClaimId != null)
                {
                    CloseTask(service, crmContext, claimResponseHeader, "Match Carrier Payment to a Claim");
                }

                if (claimResponseHeader.ipg_CaseId != null)
                {
                    CloseTask(service, crmContext, claimResponseHeader, "Match Carrier Payment to a Case");
                    CloseTask(service, crmContext, claimResponseHeader, "Match Patient Payment to a Case");
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private void CloseTask(IOrganizationService service, OrganizationServiceContext crmContext, Entity claimResponseHeader, string subject)
        {
            var tasks = (from task in crmContext.CreateQuery<Task>()
                         where ((task.RegardingObjectId.Id == claimResponseHeader.Id)
                          && (task.Subject == subject)
                          && task.StateCode != TaskState.Completed)
                         select task).ToList();

            foreach (var task in tasks)
            {
                var setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = task.ToEntityReference(),
                    State = new OptionSetValue((int)TaskState.Completed),
                    Status = new OptionSetValue((int)Task_StatusCode.Resolved)
                };
                var closeResponse = service.Execute(setStateRequest);
            }
        }
    }
}