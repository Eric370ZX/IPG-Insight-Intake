using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.ClaimResponseLine
{
    public class UpdateClaimResponseHeaderAmounts : PluginBase
    {
        public UpdateClaimResponseHeaderAmounts() : base(typeof(UpdateClaimResponseHeaderAmounts))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_claimresponseline.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimresponseline.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Delete, ipg_claimresponseline.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                ipg_claimresponseline claimResponseLine = null;

                if (context.InputParameters["Target"] is Entity)
                {
                    var claimResponseLineRef = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponseline>();
                    claimResponseLine = service.Retrieve(claimResponseLineRef.LogicalName, claimResponseLineRef.Id, new ColumnSet("ipg_claimresponseheaderid")).ToEntity<ipg_claimresponseline>();
                }
                else if (context.InputParameters["Target"] is EntityReference claimResponseLineRef)
                {
                    claimResponseLine = service.Retrieve(claimResponseLineRef.LogicalName, claimResponseLineRef.Id, new ColumnSet("ipg_claimresponseheaderid")).ToEntity<ipg_claimresponseline>();
                }

                ipg_claimresponseheader crh = service.Retrieve(ipg_claimresponseheader.EntityLogicalName, claimResponseLine.ipg_ClaimResponseHeaderId.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower())).ToEntity<ipg_claimresponseheader>();
                ipg_claimresponsebatch crb = service.Retrieve(ipg_claimresponsebatch.EntityLogicalName, crh.ipg_ClaimResponseBatchId.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_IsManualBatch).ToLower())).ToEntity<ipg_claimresponsebatch>();

                if(!(crb.ipg_IsManualBatch ?? false))
                {
                    return;
                }

                var claimResponseLines = GetClaimResponseLinesForHeader(claimResponseLine, context.MessageName, service);
                var amountPaid = claimResponseLines.Sum(t => (t.ipg_AmountPaid_new == null ? 0 : t.ipg_AmountPaid_new.Value));
                var amountSubmitted = claimResponseLines.Sum(t => (t.ipg_AmountSubmitted_new == null ? 0 : t.ipg_AmountSubmitted_new.Value));
                var amountPatientResponsibility = claimResponseLines.Sum(t => (t.ipg_AmountPatientResponsibility_new == null ? 0 : t.ipg_AmountPatientResponsibility_new.Value));


                localPluginContext.Trace($"AmountPaidSum: {amountPaid}, AmountSubmittedSum: {amountSubmitted}, AmountPatientRespSum: {amountPatientResponsibility}");
                var claimResponseHeader = new ipg_claimresponseheader()
                {
                    Id = claimResponseLine.ipg_ClaimResponseHeaderId.Id,
                    ipg_AmountPaid_new = new Money(amountPaid),
                    ipg_AmountSubmitted_new = new Money(amountSubmitted),
                    ipg_AmountPatientresponsibility_new = new Money(amountPatientResponsibility)
                };
                service.Update(claimResponseHeader);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private List<ipg_claimresponseline> GetClaimResponseLinesForHeader(ipg_claimresponseline claimResponseLine, string messageName, IOrganizationService service)
        {
            var lines = new List<ipg_claimresponseline>();

            // Define Condition Values
            var QEipg_claimresponseline_ipg_claimresponseheaderid = claimResponseLine.ipg_ClaimResponseHeaderId.Id;

            // Instantiate QueryExpression QEipg_claimresponseline
            var QEipg_claimresponseline = new QueryExpression("ipg_claimresponseline");

            // Add columns to QEipg_claimresponseline.ColumnSet
            QEipg_claimresponseline.ColumnSet.AddColumns("ipg_claimresponselineid", "ipg_amountsubmitted_new", "ipg_amountpaid_new", "ipg_allowedactual_new");

            // Define filter QEipg_claimresponseline.Criteria
            QEipg_claimresponseline.Criteria.AddCondition("ipg_claimresponseheaderid", ConditionOperator.Equal, QEipg_claimresponseline_ipg_claimresponseheaderid);
            if (messageName == MessageNames.Delete)
            {
                QEipg_claimresponseline.Criteria.AddCondition("ipg_claimresponselineid", ConditionOperator.NotEqual, claimResponseLine.Id);
            }

            var result = service.RetrieveMultiple(QEipg_claimresponseline);
            if (result.Entities.Count > 0)
            {
                lines.AddRange(result.Entities.Cast<ipg_claimresponseline>().ToList());
            }
            return lines;
        }
    }
}
