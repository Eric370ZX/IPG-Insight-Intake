using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Claim
{
    public class UpdateClaimStatus : PluginBase
    {

        struct ClaimStatusReason
        {
            public int status;
            public int reason;
        }

        public UpdateClaimStatus() : base(typeof(UpdateClaimStatus))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_claimzirmedstatus.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimzirmedstatus.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    var claimzirmedstatus = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimzirmedstatus>();
                    if (context.MessageName == "Create" || context.MessageName == "Update")
                    {
                        var status = service.Retrieve(claimzirmedstatus.LogicalName, claimzirmedstatus.Id, new ColumnSet("ipg_claimid", "ipg_eventcode")).ToEntity<ipg_claimzirmedstatus>();
                        if ((status.ipg_ClaimId != null) && (status.ipg_EventCode != null))
                        {
                            Invoice invoice = new Invoice();
                            ClaimStatusReason claimStatusReson = GetClaimStatusReason(service, status.ipg_EventCode);
                            invoice.Id = status.ipg_ClaimId.Id;
                            invoice.ipg_Status = (claimStatusReson.status != 0) ? new OptionSetValue(claimStatusReson.status) : null;
                            invoice.ipg_Reason = (claimStatusReson.reason != 0) ? new OptionSetValue(claimStatusReson.reason) : null;
                            service.Update(invoice);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        //Status codes:     427880000 Adjudicated
        //                  427880001 Processed
        //                  427880002 Submitted
        //                  427880003 Voided
        //Reason codes:     427880000 Accepted by Intermediary
        //                  427880001 Accepted by Payor
        //                  427880002 On hold
        //                  427880003 One of the adjudicated responses based on the remittance
        //                  427880004 Rejected by Intermediary
        //                  427880005 Rejected by Payor
        //                  427880006 Submitted Paper
        //                  427880007 Submitted Paper or EDI
        //                  427880008 Voided by IPG
        private ClaimStatusReason GetClaimStatusReason(IOrganizationService service, EntityReference eventCode)
        {
            /*ClaimStatusReason result = new ClaimStatusReason();
            switch (eventCode.Name)
            {
                case "A-I":
                case "A-R":
                case "A-T":
                    result.status = 427880001;
                    result.reason = 427880000;
                    break;
                case "A-P":
                case "AM-P":
                case "B-T":
                case "FI-I":
                case "FI-T":
                case "FP-I":
                case "FP-R":
                case "FP-T":
                case "FR-T":
                case "H-I":
                case "K-I":
                case "K-P":
                case "K-R":
                case "T-T":
                case "XR-T":
                    result.status = 427880002;
                    result.reason = 427880007;
                    break;
                case "C":
                    result.status = 427880003;
                    result.reason = 427880008;
                    break;
                case "D-P":
                    result.status = 427880001;
                    result.reason = 427880005;
                    break;
                case "FPM-I":
                case "FPM-R":
                    result.status = 427880002;
                    result.reason = 427880006;
                    break;
                case "H-P":
                case "H-R":
                    result.status = 427880001;
                    result.reason = 427880002;
                    break;
                case "I-I":
                case "I-P":
                case "X-T":
                    result.status = 427880001;
                    break;
                case "P-P":
                case "P-R":
                    result.status = 427880000;
                    result.reason = 427880003;
                    break;
                case "R-I":
                case "RF-I":
                    result.status = 427880001;
                    result.reason = 427880004;
                    break;
                case "R-P":
                case "R-R":
                case "R-T":
                case "R-TCI":
                case "R-TCL":
                case "R-TCN":
                case "R-TPQ":
                case "R-Z":
                case "RT-T":
                    result.status = 427880001;
                    result.reason = 427880005;
                    break;
                case "Z-P":
                    result.status = 427880001;
                    result.reason = 427880001;
                    break;
            }
            return result;*/

            ClaimStatusReason result = new ClaimStatusReason();
            var queryExpression = new QueryExpression("ipg_claimconfiguration")
            {
                ColumnSet = new ColumnSet("ipg_claimstatus", "ipg_claimreason"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression("ipg_claimevent", ConditionOperator.Equal, 427880002),
                            new ConditionExpression("ipg_eventcode", ConditionOperator.Equal, eventCode.Id)
                        }
                }
            };
            EntityCollection claimConfigurations = service.RetrieveMultiple(queryExpression);
            if(claimConfigurations.Entities.Count > 0)
            {
                result.status = (claimConfigurations.Entities[0].GetAttributeValue<OptionSetValue>("ipg_claimstatus")).Value;
                result.reason = (claimConfigurations.Entities[0].GetAttributeValue<OptionSetValue>("ipg_claimreason")).Value;
            }
            return result;


        }
    }
}