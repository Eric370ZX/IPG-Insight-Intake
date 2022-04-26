using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.ClaimResponseLineAdjustment
{
    public class UpdateClaimResponseLine : PluginBase
    {
        public UpdateClaimResponseLine() : base(typeof(UpdateClaimResponseLine))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_claimresponselineadjustment.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimresponselineadjustment.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Delete, ipg_claimresponselineadjustment.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                ipg_claimresponselineadjustment adjustment = null;

                if (context.InputParameters["Target"] is Entity)
                {
                    var adjustmentRef = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponselineadjustment>();
                    adjustment = service.Retrieve(adjustmentRef.LogicalName, adjustmentRef.Id, new ColumnSet("ipg_claimresponselineid")).ToEntity<ipg_claimresponselineadjustment>();
                }
                else if (context.InputParameters["Target"] is EntityReference adjustmentRef)
                {
                    adjustment = service.Retrieve(adjustmentRef.LogicalName, adjustmentRef.Id, new ColumnSet("ipg_claimresponselineid")).ToEntity<ipg_claimresponselineadjustment>();
                }

                var claimResponseLineAdjustments = GetClaimResponseAdjustmentsForLine(adjustment, context.MessageName, service);
                decimal? amountPatientResponsibility = claimResponseLineAdjustments.Where(t => t.ipg_Code.ToLower().StartsWith("pr")).Sum(t => (t.ipg_Amount_new == null ? 0 : t.ipg_Amount_new.Value));
                var claimResponseLine = new ipg_claimresponseline()
                {
                    Id = adjustment.ipg_ClaimResponseLineId.Id,
                    ipg_AmountPatientResponsibility_new = new Money(amountPatientResponsibility ?? 0)
                    //ipg_DenialCodeString = string.Join(", ", claimResponseLineAdjustments.Select(t => t.ipg_Code).ToArray()).ToUpper()
                };
                service.Update(claimResponseLine);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private List<ipg_claimresponselineadjustment> GetClaimResponseAdjustmentsForLine(ipg_claimresponselineadjustment adjustment, string messageName, IOrganizationService service)
        {
            var adjustments = new List<ipg_claimresponselineadjustment>();
            // Define Condition Values
            var QEipg_claimresponselineadjustment_ipg_claimresponselineid = adjustment.ipg_ClaimResponseLineId.Id;

            // Instantiate QueryExpression QEipg_claimresponselineadjustment
            var QEipg_claimresponselineadjustment = new QueryExpression("ipg_claimresponselineadjustment");

            // Add columns to QEipg_claimresponselineadjustment.ColumnSet
            QEipg_claimresponselineadjustment.ColumnSet.AddColumns("ipg_amount_new", "ipg_code", "ipg_claimresponselineid");

            // Define filter QEipg_claimresponselineadjustment.Criteria
            QEipg_claimresponselineadjustment.Criteria.AddCondition("ipg_claimresponselineid", ConditionOperator.Equal, QEipg_claimresponselineadjustment_ipg_claimresponselineid);
            if (messageName == MessageNames.Delete)
            {
                QEipg_claimresponselineadjustment.Criteria.AddCondition("ipg_claimresponselineadjustmentid", ConditionOperator.NotEqual, adjustment.Id);
            }

            var result = service.RetrieveMultiple(QEipg_claimresponselineadjustment);
            if (result.Entities.Count > 0)
            {
                adjustments.AddRange(result.Entities.Cast<ipg_claimresponselineadjustment>().ToList());
            }
            return adjustments;
        }
    }
}
