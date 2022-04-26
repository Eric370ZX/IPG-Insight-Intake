using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;

namespace Insight.Intake.Plugin.ClaimResponseHeader
{
    public class CreateClaimResponseLinesFromClaim : PluginBase
    {

        public CreateClaimResponseLinesFromClaim() : base(typeof(CreateClaimResponseLinesFromClaim))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_claimresponseheader.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimresponseheader.EntityLogicalName, PostOperationCreateHandler);

        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var crmContext = new OrganizationServiceContext(service);
                var claimResponseHeader = context.MessageName.ToLower().Equals("create") ? ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponseheader>()
                    : context.PostEntityImages["PostImage"].ToEntity<ipg_claimresponseheader>();

                if (claimResponseHeader.ipg_CaseId == null || claimResponseHeader.ipg_ClaimId == null)
                {
                    return;
                }

                var batch = service.Retrieve(ipg_claimresponsebatch.EntityLogicalName, claimResponseHeader.ipg_ClaimResponseBatchId.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_IsManualBatch).ToLower())).ToEntity<ipg_claimresponsebatch>();
                if(!(batch.ipg_IsManualBatch ?? false))
                {
                    return;
                }

                var claim = service.Retrieve(Invoice.EntityLogicalName, claimResponseHeader.ipg_ClaimId.Id, new ColumnSet(true)).ToEntity<Invoice>();
                var oldPaymentLines = (from crl in crmContext.CreateQuery<ipg_claimresponseline>()
                                    where crl.ipg_ClaimResponseHeaderId.Id == claimResponseHeader.Id
                                    select crl).ToList();


                foreach (var paymentLine in oldPaymentLines)
                {
                    service.Delete(paymentLine.LogicalName, paymentLine.Id);
                }

                var claimLines = GetClaimLineItems(claim, service);
                foreach (var claimLine in claimLines)
                {
                    var claimResponseLine = new ipg_claimresponseline()
                    {
                        ipg_ClaimResponseHeaderId = new EntityReference(claimResponseHeader.LogicalName, claimResponseHeader.Id),
                        ipg_AmountSubmitted_new = new Money(claimLine.ipg_billedchg != null? claimLine.ipg_billedchg.Value : 0),
                        ipg_AmountPaid_new = new Money(claimLine.ipg_paid != null ? claimLine.ipg_paid.Value : 0),
                        ipg_AllowedActual_new = new Money(claimLine.ipg_allowed != null ? claimLine.ipg_allowed.Value : 0),
                        //ipg_DenialCodeString = claimLine.ipg_DenialCodeString,
                        ipg_AdjProc = claimLine.ipg_name,
                        ipg_AdjProcCode = "HCPCS",
                        ipg_PaidUnits = claimLine.ipg_quantity != null ? Convert.ToInt32(claimLine.ipg_quantity) : 0,
                        ipg_name = claimLine.ipg_name,
                        ipg_AmountPatientResponsibility_new = new Money(claimLine.ipg_PatientResponsibility != null ? claimLine.ipg_PatientResponsibility.Value : 0)
                        //ipg_RemarkCodeString = claimLine.ipg_RemarkCodeString 
                    };
                    service.Create(claimResponseLine);
                }                
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private List<ipg_claimlineitem> GetClaimLineItems(Invoice claim, IOrganizationService service)
        {
            var lineItems = new List<ipg_claimlineitem>();
            
            // Define Condition Values
            var QEipg_claimlineitem_ipg_claimid = claim.Id;

            // Instantiate QueryExpression QEipg_claimlineitem
            var QEipg_claimlineitem = new QueryExpression("ipg_claimlineitem");

            // Add all columns to QEipg_claimlineitem.ColumnSet
            QEipg_claimlineitem.ColumnSet.AllColumns = true;

            // Define filter QEipg_claimlineitem.Criteria
            QEipg_claimlineitem.Criteria.AddCondition("ipg_claimid", ConditionOperator.Equal, QEipg_claimlineitem_ipg_claimid);

            var result = service.RetrieveMultiple(QEipg_claimlineitem);
            if (result.Entities.Count > 0)
            {
                lineItems.AddRange(result.Entities.Cast<ipg_claimlineitem>().ToList());
            }
            return lineItems;
        }
    }
} 