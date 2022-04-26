using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Linq;
using Insight.Intake.Data;
using Insight.Intake.Plugin.Gating.Common;

namespace Insight.Intake.Plugin.Gating
{
    /// <summary>
    /// Used for action calls only, no gate config details
    /// </summary>
    public class CheckCptCodesPlugin : GatingPluginBase
    {
        public CheckCptCodesPlugin() : base("ipg_IPGGatingCheckCPTCodes") { }

        /// <summary>
        /// Get List of CPT Codes form the Referral record
        /// </summary>
        /// <returns>List of CPT Codes related to the Referral record</returns>
        private IList<ipg_cptcode> GetCptCodes( ipg_referral referral, IOrganizationService organizationService)
        {
            var cptCodes = new List<ipg_cptcode>();
             
            var referralType = referral.GetType();

            var index = 1;

            while (true)
            {
                var expectedCptCodeProperty = referralType.GetProperty($"ipg_CPTCodeId{index}");

                if (expectedCptCodeProperty == null)
                {
                    break;
                }

                var cptCodeReference = (EntityReference)expectedCptCodeProperty.GetValue(referral);

                if (cptCodeReference != null)
                {
                    var cptCode = organizationService.Retrieve(cptCodeReference.LogicalName, cptCodeReference.Id, new ColumnSet(true));

                    cptCodes.Add(cptCode.ToEntity<ipg_cptcode>());
                }

                index++;
            }

            return cptCodes;
        }

        /// <summary>
        /// Get List of Blocked By IPG CPT Codes
        /// </summary>
        /// <param name="cptCodes">Reference to List of CPT Codes</param>
        /// <returns>List of Blocked By IPG CPT Codes</returns>
        private IList<ipg_cptcode> GetCptCodesBlockedByIpg(ref IList<ipg_cptcode> cptCodes)
        {
            var blockedCptCodes = new List<ipg_cptcode>();

            if (cptCodes.Count > 0)
            {
                foreach (var cptCode in cptCodes.ToList())
                {
                    if (cptCode.ipg_ImplantUsed.Value == ImplantUsedOptionSet.No)
                    {
                        blockedCptCodes.Add(cptCode);

                        cptCodes.Remove(cptCode);
                    }
                }
            }

            return blockedCptCodes;
        }

        /// <summary>
        /// Get List of Unsupported by IPG CPT Codes
        /// </summary>
        /// <param name="cptCodes">Reference to List of CPT Codes</param>
        /// <returns>List of Unsupported by IPG CPT Codes</returns>
        private IList<ipg_cptcode> GetCptCodesUnsupportedByIPG(ref IList<ipg_cptcode> cptCodes)
        {
            var blockedCptCodes = new List<ipg_cptcode>();

            if (cptCodes.Count > 0)
            {
                foreach (var cptCode in cptCodes.ToList())
                {
                    if (cptCode.ipg_ImplantUsed?.Value == ImplantUsedOptionSet.No)
                    {
                        blockedCptCodes.Add(cptCode);

                        cptCodes.Remove(cptCode);
                    }
                }
            }

            return blockedCptCodes;
        }

        /// <summary>
        /// Get List of Blocked By Facility CPT Codes
        /// </summary>
        /// <param name="cptCodes">Reference to List of CPT Codes</param>
        /// <returns>List of Blocked By Facility CPT Codes</returns>
        private IList<ipg_cptcode> GetCptCodesBlockedByFacility(ref IList<ipg_cptcode> cptCodes, ipg_referral referral, IOrganizationService organizationService)
        {
            var blockedCptCodes = new List<ipg_cptcode>();

            if (cptCodes.Count > 0 && referral.ipg_FacilityId != null)
            {
                foreach (var cptCode in cptCodes.ToList())
                {
                    var queryExpression = new QueryExpression
                    {
                        EntityName = ipg_associatedcpt.EntityLogicalName,
                        ColumnSet = new ColumnSet(nameof(ipg_associatedcpt.ipg_associatedcptId).ToLower()),
                        Criteria = new FilterExpression(LogicalOperator.And)
                    };

                    queryExpression.Criteria.AddCondition("ipg_cptcodeid", ConditionOperator.Equal, cptCode.Id);

                    queryExpression.Criteria.AddCondition("ipg_facilityid", ConditionOperator.Equal, referral.ipg_FacilityId.Id);

                    var associatedCptCodesRequest = organizationService.RetrieveMultiple(queryExpression);

                    if (associatedCptCodesRequest.Entities.Count == 0)
                    {                        
                        blockedCptCodes.Add(cptCode);

                        cptCodes.Remove(cptCode);
                    }
                }
            }

            return blockedCptCodes;
        }

        /// <summary>
        /// Get List of Blocked By Carrier CPT Codes
        /// </summary>
        /// <param name="cptCodes">Reference to List of CPT Codes</param>
        /// <returns>List of Blocked By Carrier CPT Codes</returns>
        private IList<ipg_cptcode> GetCptCodesBlockedByCarrier(ref IList<ipg_cptcode> cptCodes, ipg_referral referral, IOrganizationService organizationService)
        {
            var blockedCptCodes = new List<ipg_cptcode>();

            if (cptCodes.Count > 0 && referral.ipg_CarrierId != null)
            {
                foreach (var cptCode in cptCodes.ToList())
                {
                    var queryExpression = new QueryExpression
                    {
                        EntityName = ipg_associatedcpt.EntityLogicalName,
                        ColumnSet = new ColumnSet(nameof(ipg_associatedcpt.ipg_associatedcptId).ToLower()),
                        Criteria = new FilterExpression(LogicalOperator.And)
                    };

                    queryExpression.Criteria.AddCondition("ipg_cptcodeid", ConditionOperator.Equal, cptCode.Id);

                    queryExpression.Criteria.AddCondition("ipg_carrierid", ConditionOperator.Equal, referral.ipg_CarrierId.Id);

                    var associatedCptCodesRequest = organizationService.RetrieveMultiple(queryExpression);

                    if (associatedCptCodesRequest.Entities.Count == 0)
                    {
                        blockedCptCodes.Add(cptCode);

                        cptCodes.Remove(cptCode);
                    }
                }
            }

            return blockedCptCodes;
        }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var tracingService = ctx.TracingService;

            try
            {
                //var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                //var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var organizationService = ctx.OrganizationService; //organizationServiceFactory.CreateOrganizationService(context.UserId);

                var referralReference = ctx.TargetRef(); // (EntityReference)context.InputParameters["Target"];
                if (referralReference.LogicalName != ipg_referral.EntityLogicalName)
                {
                    return new GatingResponse(true);
                }

                var referral = organizationService.Retrieve(referralReference.LogicalName, referralReference.Id, new ColumnSet(true)).ToEntity<ipg_referral>();

                var cptCodes = GetCptCodes(referral, organizationService);

                var cptCodesBlockedByIpg = GetCptCodesBlockedByIpg(ref cptCodes);

                var cptCodesUnsupportedByIpg = GetCptCodesUnsupportedByIPG(ref cptCodes);

                var cptCodesBlockedByFacility = GetCptCodesBlockedByFacility(ref cptCodes, referral, organizationService);

                var cptCodesBlockedByCarrier = GetCptCodesBlockedByCarrier(ref cptCodes, referral, organizationService);

                ctx.PluginExecutionContext.OutputParameters["NotBlockedCount"] = cptCodes.Count;

                ctx.PluginExecutionContext.OutputParameters["BlockedByIPGCount"] = cptCodesBlockedByIpg.Count;

                ctx.PluginExecutionContext.OutputParameters["UnsupportedByIPGCount"] = cptCodesUnsupportedByIpg.Count;

                ctx.PluginExecutionContext.OutputParameters["BlockedByFacilityCount"] = cptCodesBlockedByFacility.Count;

                ctx.PluginExecutionContext.OutputParameters["BlockedByCarrierCount"] = cptCodesBlockedByCarrier.Count;

                ctx.PluginExecutionContext.OutputParameters["NotBlockedList"] = string.Join(", ", cptCodes.Select(x => x.ipg_name));

                ctx.PluginExecutionContext.OutputParameters["UnsupportedByIPGList"] = string.Join(", ", cptCodesUnsupportedByIpg.Select(x => x.ipg_name));

                ctx.PluginExecutionContext.OutputParameters["BlockedByIPGList"] = string.Join(", ", cptCodesBlockedByIpg.Select(x => x.ipg_name));

                ctx.PluginExecutionContext.OutputParameters["BlockedByFacilityList"] = string.Join(", ", cptCodesBlockedByFacility.Select(x => x.ipg_name));

                ctx.PluginExecutionContext.OutputParameters["BlockedByCarrierList"] = string.Join(", ", cptCodesBlockedByCarrier.Select(x => x.ipg_name));

                return new GatingResponse(true);

            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(CheckCptCodesPlugin)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace("{0}: {1}", nameof(CheckCptCodesPlugin), exception.ToString());
                throw;
            }
        }
    }
}