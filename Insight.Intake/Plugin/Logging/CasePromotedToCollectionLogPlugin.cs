using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Logging
{
    //Case Passed Gate 9 (promoted to collection)
    public class CasePromotedToCollectionLogPlugin: PluginBase
    {
        private static readonly string _lifeCycleFieldName = nameof(Incident.ipg_lifecyclestepid);
        private static readonly string _casepassedGate9LogText = "Case was promoted to Collections (passed Gate 9)";

        public CasePromotedToCollectionLogPlugin(): base(typeof(CasePromotedToCollectionLogPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, LogCasePromotedToCollection);
        }

        private void LogCasePromotedToCollection(LocalPluginContext localContext)
        {
            var tracingService = localContext.TracingService;

            tracingService.Trace($"{nameof(CasePromotedToCollectionLogPlugin)}  - started");

            var preImage = localContext.PreImage<Incident>();
            var postImage = localContext.PostImage<Incident>();


            if (preImage.ipg_lifecyclestepid != null
                && postImage.ipg_lifecyclestepid != null
                && preImage.ipg_lifecyclestepid.Id != postImage.ipg_lifecyclestepid.Id)
            {
                tracingService.Trace($"{_lifeCycleFieldName}  - different");

                var crmService = localContext.OrganizationService;
                using (var crmContext = new OrganizationServiceContext(crmService))
                { 
                    tracingService.Trace($"Retrieving Gate from prev LifeCycle with id {preImage.ipg_lifecyclestepid.Id}");
                  
                    var prevGate = (from lfstep in crmContext.CreateQuery<ipg_lifecyclestep>()
                                    join gateconf in crmContext.CreateQuery<ipg_gateconfiguration>() on lfstep.ipg_gateconfigurationid.Id equals gateconf.ipg_gateconfigurationId
                                    where lfstep.ipg_lifecyclestepId == preImage.ipg_lifecyclestepid.Id
                                    select new ipg_gateconfiguration()
                                    {
                                        Id = (Guid)gateconf.ipg_gateconfigurationId,
                                        ipg_name = gateconf.ipg_name
                                    }).FirstOrDefault();


                    if (prevGate != null
                        && string.Equals(prevGate.ipg_name, Constants.GateNames.Gate9, StringComparison.InvariantCultureIgnoreCase))
                    {

                        tracingService.Trace($"Retrieving gateprocessingrule with next gate different from {prevGate.ipg_name}");
                        var procRule = (from processingRule in crmContext.CreateQuery<ipg_gateprocessingrule>()
                                        join nextlfstep in crmContext.CreateQuery<ipg_lifecyclestep>() on processingRule.ipg_nextlifecyclestepid.Id equals nextlfstep.ipg_lifecyclestepId
                                        where processingRule.ipg_lifecyclestepid.Id == preImage.ipg_lifecyclestepid.Id 
                                        && nextlfstep.ipg_gateconfigurationid.Id != prevGate.Id
                                        && (processingRule.ipg_severitylevelEnum == ipg_SeverityLevel.Info || processingRule.ipg_severitylevelEnum == ipg_SeverityLevel.Warning)
                                        select new ipg_gateprocessingrule()
                                        {
                                            Id = (Guid)processingRule.ipg_gateprocessingruleId,
                                            ipg_nextlifecyclestepid = processingRule.ipg_nextlifecyclestepid
                                        }).FirstOrDefault();

                        if (procRule != null)
                        {
                            tracingService.Trace($"Next lifecycle from next gate have id {procRule.ipg_nextlifecyclestepid.Id}");

                            if (postImage.ipg_lifecyclestepid.Id == procRule.ipg_nextlifecyclestepid.Id)
                            {
                                tracingService.Trace($"Case passed Gate {prevGate.ipg_name}");

                                tracingService.Trace($"Created Log that Case passed {prevGate.ipg_name} for Case with id {postImage.Id}");

                                var logId = crmService.Create(new ipg_gateactivity()
                                {
                                    Subject = _casepassedGate9LogText,
                                    ActualStart = postImage.ModifiedOn,
                                    RegardingObjectId = postImage.ToEntityReference()
                                });

                                tracingService.Trace($"Log Activity with Id {logId} created");
                            }
                            else
                            {
                                tracingService.Trace($"Case not passed Gate {prevGate.ipg_name}");
                            }
                        }
                        else
                        {
                            tracingService.Trace($"Next lifecycle from next gate not found!");
                        }
                    }                
                }
            }

            tracingService.Trace($"{nameof(CasePromotedToCollectionLogPlugin)} - done");
        }
    }
}
