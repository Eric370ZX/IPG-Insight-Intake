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
    public class UpdateClaimId : PluginBase
    {

        public UpdateClaimId() : base(typeof(CreateClaimResponseLinesFromClaim))
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

                if (String.IsNullOrWhiteSpace(claimResponseHeader.ipg_ClaimNumber))
                {
                    return;
                }

                var invoices = (from invoice in crmContext.CreateQuery<Invoice>()
                             where invoice.Name == claimResponseHeader.ipg_ClaimNumber
                             select invoice).ToList();
                if(invoices.Count > 0)
                {
                    claimResponseHeader.ipg_ClaimId = invoices[0].ToEntityReference();
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