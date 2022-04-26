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
    public class UpdateCaseId : PluginBase
    {

        public UpdateCaseId() : base(typeof(CreateClaimResponseLinesFromClaim))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_claimresponseheader.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var crmContext = new OrganizationServiceContext(service);
                ipg_claimresponseheader claimResponseHeader = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponseheader>();

                if (String.IsNullOrWhiteSpace(claimResponseHeader.ipg_CorrectedCaseNumber))
                {
                    return;
                }

                var cases = (from incident in crmContext.CreateQuery<Incident>()
                                where incident.Title == claimResponseHeader.ipg_CorrectedCaseNumber
                                select incident).ToList();
                if (cases.Count > 0)
                {
                    claimResponseHeader.ipg_CaseId = cases[0].ToEntityReference();
                    var claimResponseHeaderEnt = service.Retrieve(claimResponseHeader.LogicalName, claimResponseHeader.Id, new ColumnSet(nameof(claimResponseHeader.ipg_PostStatus).ToLower()));
                    if (claimResponseHeaderEnt.GetAttributeValue<string>(nameof(claimResponseHeader.ipg_PostStatus).ToLower()) == "error")
                    {
                        claimResponseHeader.ipg_PostStatus = "new";
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}