using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Linq;
using Insight.Intake.Data;
using Insight.Intake.Helpers;

namespace Insight.Intake.Plugin.Workflow
{
    public class CheckCptCodesPlugin : IPlugin
    {   
        private IOrganizationService _organizationService;

        private ipg_referral _referral;
        
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                _organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);
                
                if (context.InputParameters.Contains("Target"))
                {
                    if (context.MessageName == Constants.ActionNames.IntakeActionsCheckCPTCodes)
                    {
                        var referralReference = (EntityReference)context.InputParameters["Target"];
                        
                        _referral = _organizationService.Retrieve(referralReference.LogicalName, referralReference.Id, new ColumnSet(true)).ToEntity<ipg_referral>();

                        var cptCodes = GetCptCodes();

                        var cptCodesBlockedByIpg = GetCptCodesBlockedByIpg(ref cptCodes);

                        var cptCodesUnsupportedByIpg = GetCptCodesUnsupportedByIPG(ref cptCodes);

                        var cptCodesBlockedByFacility = GetCptCodesBlockedByFacility(ref cptCodes);

                        var cptCodesBlockedByCarrier = GetCptCodesBlockedByCarrier(ref cptCodes);

                        context.OutputParameters["NotBlockedCount"] = cptCodes.Count;

                        context.OutputParameters["BlockedByIPGCount"] = cptCodesBlockedByIpg.Count;

                        context.OutputParameters["UnsupportedByIPGCount"] = cptCodesUnsupportedByIpg.Count;

                        context.OutputParameters["BlockedByFacilityCount"] = cptCodesBlockedByFacility.Count;

                        context.OutputParameters["BlockedByCarrierCount"] = cptCodesBlockedByCarrier.Count;

                        context.OutputParameters["NotBlockedList"] = string.Join(", ", cptCodes.Select(x => x.ipg_name));
                        
                        context.OutputParameters["UnsupportedByIPGList"] = string.Join(", ", cptCodesUnsupportedByIpg.Select(x => x.ipg_name));
                        
                        context.OutputParameters["BlockedByIPGList"] = string.Join(", ", cptCodesBlockedByIpg.Select(x => x.ipg_name));
                        
                        context.OutputParameters["BlockedByFacilityList"] = string.Join(", ", cptCodesBlockedByFacility.Select(x => x.ipg_name));
                        
                        context.OutputParameters["BlockedByCarrierList"] = string.Join(", ", cptCodesBlockedByCarrier.Select(x => x.ipg_name));
                    }
                }
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

        /// <summary>
        /// Get List of CPT Codes form the Referral record
        /// </summary>
        /// <returns>List of CPT Codes related to the Referral record</returns>
        private IList<ipg_cptcode> GetCptCodes()
        {
            var cptCodes = new List<ipg_cptcode>();
            
            var referralType = _referral.GetType();
            
            var index = 1;
            
            while (true)
            {
                var expectedCptCodeProperty = referralType.GetProperty($"ipg_CPTCodeId{index}");

                if (expectedCptCodeProperty == null)
                {
                    break;
                }
                
                var cptCodeReference = (EntityReference)expectedCptCodeProperty.GetValue(_referral);

                if (cptCodeReference != null)
                {
                    var cptCode = _organizationService.Retrieve(cptCodeReference.LogicalName, cptCodeReference.Id, new ColumnSet(true));

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
        private IList<ipg_cptcode> GetCptCodesBlockedByFacility(ref IList<ipg_cptcode> cptCodes)
        {
            var blockedCptCodes = new List<ipg_cptcode>();

            if (cptCodes.Count > 0 && _referral.ipg_FacilityId != null)
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
                                
                    queryExpression.Criteria.AddCondition("ipg_facilityid", ConditionOperator.Equal, _referral.ipg_FacilityId.Id);
                    
                    var associatedCptCodesRequest = _organizationService.RetrieveMultiple(queryExpression);

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
        private IList<ipg_cptcode> GetCptCodesBlockedByCarrier(ref IList<ipg_cptcode> cptCodes)
        {
            var blockedCptCodes = new List<ipg_cptcode>();

            if (cptCodes.Count > 0 && _referral.ipg_CarrierId != null)
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
                                        
                    queryExpression.Criteria.AddCondition("ipg_carrierid", ConditionOperator.Equal, _referral.ipg_CarrierId.Id);
                    
                    var associatedCptCodesRequest = _organizationService.RetrieveMultiple(queryExpression);

                    if (associatedCptCodesRequest.Entities.Count == 0)
                    {
                        blockedCptCodes.Add(cptCode);

                        cptCodes.Remove(cptCode);
                    }
                }
            }

            return blockedCptCodes;
        }
    }
}