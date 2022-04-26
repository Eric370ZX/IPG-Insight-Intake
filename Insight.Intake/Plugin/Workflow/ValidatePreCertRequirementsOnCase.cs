using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Workflow
{
    public class ValidatePreCertRequirementsOnCase : IPlugin
    {
        private ITracingService tracingService;
        private IOrganizationService organizationService;
        private EntityReference targetEntityReference;

        public void Execute(IServiceProvider serviceProvider)
        {
            targetEntityReference = Initialize(serviceProvider);
            tracingService.Trace("ValidatePreCertRequirementsOnCase: Initialization Complete.");
            var caseEntity = organizationService.Retrieve(Incident.EntityLogicalName, targetEntityReference.Id,
                                new ColumnSet(nameof(Incident.ipg_CarrierId).ToLower(),
                                              nameof(Incident.ipg_CPTCodeId1).ToLower(),
                                              nameof(Incident.ipg_CPTCodeId2).ToLower(),
                                              nameof(Incident.ipg_CPTCodeId3).ToLower(),
                                              nameof(Incident.ipg_CPTCodeId4).ToLower(),
                                              nameof(Incident.ipg_CPTCodeId5).ToLower(),
                                              nameof(Incident.ipg_CPTCodeId6).ToLower(),
                                              nameof(Incident.ipg_ActualDOS).ToLower(),
                                              nameof(Incident.Title).ToLower(),
                                              nameof(Incident.ipg_SurgeryDate).ToLower())).ToEntity<Incident>();

            var dos = caseEntity.ipg_ActualDOS != null ? caseEntity.ipg_ActualDOS : caseEntity.ipg_SurgeryDate;
            tracingService.Trace($"ValidatePreCertRequirementsOnCase: Starting Validating Precert for case {caseEntity.Title}.");
            if (!dos.HasValue)
            {
                tracingService.Trace($"DOS is null for case {caseEntity.Title}");
            }
            else
            {
                ValidatePrecerts(caseEntity, dos.Value);
            }
        }

        /// <summary>
        /// This method should be called as the first thing when calling this plugin.
        /// It initialized the following private fields:
        /// 1. tracingService
        /// 2. organizationService
        /// 3. targetEntityReference
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private EntityReference Initialize(IServiceProvider serviceProvider)
        {
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            tracingService.Trace("Validating Target input parameter");

            if (context.InputParameters.Contains("Target") == false
                || !(context.InputParameters["Target"] is EntityReference targetEntityReference)
                || targetEntityReference.LogicalName != Incident.EntityLogicalName)
            {
                throw new Exception("Target entity Case is required");
            }

            tracingService.Trace("Getting Organization service");
            organizationService = serviceProvider.GetOrganizationService(context.UserId);

            return targetEntityReference;
        }

        /// <summary>
        /// Fetch all the precerts for given CPT code.
        /// </summary>
        /// <param name="cptId"></param>
        /// <param name="carrierId"></param>
        /// <param name="dos"></param>
        /// <returns></returns>
        private IEnumerable<ipg_carrierprecertcpt> GetCptPrecertList(Incident incident, DateTime dos)
        {
            tracingService.Trace($"ValidatePreCertRequirementsOnCase: Getting precert list for DOS - {dos.ToShortDateString()}, Carrier - {incident.ipg_CarrierId.Name}");
            var query = new QueryExpression(ipg_carrierprecertcpt.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet(nameof(ipg_carrierprecertcpt.ipg_RequirementType).ToLower(),
                                          nameof(ipg_carrierprecertcpt.ipg_threshold).ToLower(),
                                          nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(),
                                          nameof(ipg_carrierprecertcpt.ipg_CarrierId).ToLower(),
                                          nameof(ipg_carrierprecertcpt.ipg_EffectiveStartDate).ToLower(),
                                          nameof(ipg_carrierprecertcpt.ipg_EffectiveEndDate).ToLower(),
                                          nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower())
            };
            query.Criteria.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CarrierId).ToLower(), ConditionOperator.Equal, incident.ipg_CarrierId.Id);
            query.Criteria.AddCondition(nameof(ipg_carrierprecertcpt.ipg_EffectiveStartDate).ToLower(), ConditionOperator.OnOrBefore, dos);

            var filterEndDate = new FilterExpression(LogicalOperator.Or);
            filterEndDate.AddCondition(nameof(ipg_carrierprecertcpt.ipg_EffectiveEndDate).ToLower(), ConditionOperator.OnOrAfter, dos);
            filterEndDate.AddCondition(nameof(ipg_carrierprecertcpt.ipg_EffectiveEndDate).ToLower(), ConditionOperator.Null);
            query.Criteria.AddFilter(filterEndDate);

            var filterRequirementType = new FilterExpression(LogicalOperator.Or);
            filterRequirementType.AddCondition(nameof(ipg_carrierprecertcpt.ipg_RequirementType).ToLower(), ConditionOperator.Equal, (int)ipg_CarrierPrecertCPTRequirementType.CPTLOMN);
            filterRequirementType.AddCondition(nameof(ipg_carrierprecertcpt.ipg_RequirementType).ToLower(), ConditionOperator.Equal, (int)ipg_CarrierPrecertCPTRequirementType.HighDollar);
            query.Criteria.AddFilter(filterRequirementType);

            LinkEntity cpt = new LinkEntity(ipg_carrierprecertcpt.EntityLogicalName,
                               ipg_cptcode.EntityLogicalName,
                               nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(),
                               nameof(ipg_cptcode.ipg_cptcodeId).ToLower(),
                               JoinOperator.Inner);
            cpt.EntityAlias = "cpt";
            cpt.Columns.AddColumn("ipg_cptcode");
            query.LinkEntities.Add(cpt);

            var filterCpt = new FilterExpression(LogicalOperator.Or);
            if (incident.ipg_CPTCodeId1 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId1.Id);
            if (incident.ipg_CPTCodeId2 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId2.Id);
            if (incident.ipg_CPTCodeId3 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId3.Id);
            if (incident.ipg_CPTCodeId4 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId4.Id);
            if (incident.ipg_CPTCodeId5 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId5.Id);
            if (incident.ipg_CPTCodeId6 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId6.Id);
            query.Criteria.AddFilter(filterCpt);

            var queryResponse = organizationService.RetrieveMultiple(query);
            return Enumerable.Cast<ipg_carrierprecertcpt>(queryResponse.Entities);
        }

        /// <summary>
        /// Fetch all the precerts for given CPT code.
        /// </summary>
        /// <param name="cptId"></param>
        /// <param name="carrierId"></param>
        /// <param name="dos"></param>
        /// <returns></returns>
        private IEnumerable<ipg_carrierprecerthcpcs> GetHcpcsLOMNPrecertList(Incident incident, DateTime dos)
        {
            var query = new QueryExpression(ipg_carrierprecerthcpcs.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet(true)
            };

            query.Criteria.AddCondition(nameof(ipg_carrierprecerthcpcs.ipg_EffectiveStartDate).ToLower(), ConditionOperator.OnOrBefore, dos);

            var filterEndDate = new FilterExpression(LogicalOperator.Or);
            filterEndDate.AddCondition(nameof(ipg_carrierprecerthcpcs.ipg_EffectiveEndDate).ToLower(), ConditionOperator.OnOrAfter, dos);
            filterEndDate.AddCondition(nameof(ipg_carrierprecerthcpcs.ipg_EffectiveEndDate).ToLower(), ConditionOperator.Null);
            query.Criteria.AddFilter(filterEndDate);

            LinkEntity linkCptPrecert = new LinkEntity(ipg_carrierprecerthcpcs.EntityLogicalName, 
                                                       ipg_carrierprecertcpt.EntityLogicalName,
                                                       nameof(ipg_carrierprecerthcpcs.ipg_CarrierPrecertCPTId).ToLower(),
                                                       nameof(ipg_carrierprecertcpt.ipg_carrierprecertcptId).ToLower(),
                                                       JoinOperator.Inner);
            linkCptPrecert.EntityAlias = "icp";
            linkCptPrecert.LinkCriteria = new FilterExpression(LogicalOperator.And);
            linkCptPrecert.LinkCriteria.AddCondition(nameof(ipg_carrierprecertcpt.ipg_EffectiveStartDate).ToLower(), ConditionOperator.OnOrBefore, dos);
            linkCptPrecert.LinkCriteria.AddCondition(nameof(ipg_carrierprecertcpt.ipg_RequirementType).ToLower(), ConditionOperator.Equal, (int)ipg_CarrierPrecertCPTRequirementType.HCPCSLOMN);
            linkCptPrecert.LinkCriteria.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CarrierId).ToLower(), ConditionOperator.Equal, incident.ipg_CarrierId.Id);
            linkCptPrecert.Columns.AddColumn(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower());

            LinkEntity cpt = new LinkEntity(ipg_carrierprecertcpt.EntityLogicalName,
                                           ipg_cptcode.EntityLogicalName,
                                           nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(),
                                           nameof(ipg_cptcode.ipg_cptcodeId).ToLower(),
                                           JoinOperator.Inner);
            cpt.EntityAlias = "cpt";
            cpt.Columns.AddColumn("ipg_cptcode");
            linkCptPrecert.LinkEntities.Add(cpt);

            var filterEndDateCptPrecert = new FilterExpression(LogicalOperator.Or);
            filterEndDateCptPrecert.AddCondition(nameof(ipg_carrierprecertcpt.ipg_EffectiveEndDate).ToLower(), ConditionOperator.OnOrAfter, dos);
            filterEndDateCptPrecert.AddCondition(nameof(ipg_carrierprecertcpt.ipg_EffectiveEndDate).ToLower(), ConditionOperator.Null);
            linkCptPrecert.LinkCriteria.AddFilter(filterEndDateCptPrecert);

            var filterCpt = new FilterExpression(LogicalOperator.Or);
            if (incident.ipg_CPTCodeId1 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId1.Id);
            if (incident.ipg_CPTCodeId2 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId2.Id);
            if (incident.ipg_CPTCodeId3 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId3.Id);
            if (incident.ipg_CPTCodeId4 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId4.Id);
            if (incident.ipg_CPTCodeId5 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId5.Id);
            if (incident.ipg_CPTCodeId6 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId6.Id);
            linkCptPrecert.LinkCriteria.AddFilter(filterCpt);
            query.LinkEntities.Add(linkCptPrecert);


            LinkEntity linkCpt = new LinkEntity(ipg_carrierprecertcpt.EntityLogicalName,
                                                ipg_cptcode.EntityLogicalName,
                                                nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(),
                                                nameof(ipg_cptcode.ipg_cptcodeId).ToLower(),
                                                JoinOperator.Inner);
            linkCptPrecert.LinkEntities.Add(linkCpt);

            var queryResponse = organizationService.RetrieveMultiple(query);
            return Enumerable.Cast<ipg_carrierprecerthcpcs>(queryResponse.Entities);
        }

        /// <summary>
        /// Fetch all the precerts for given CPT code.
        /// </summary>
        /// <param name="cptId"></param>
        /// <param name="carrierId"></param>
        /// <param name="dos"></param>
        /// <returns></returns>
        private IEnumerable<ipg_carrierprecerthcpcs> GetThresholdPrecertList(Incident incident, DateTime dos)
        {
            var query = new QueryExpression(ipg_carrierprecerthcpcs.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet(true)
            };

            query.Criteria.AddCondition(nameof(ipg_carrierprecerthcpcs.ipg_EffectiveStartDate).ToLower(), ConditionOperator.OnOrBefore, dos);
            var filterEndDate = new FilterExpression(LogicalOperator.Or);
            filterEndDate.AddCondition(nameof(ipg_carrierprecerthcpcs.ipg_EffectiveEndDate).ToLower(), ConditionOperator.OnOrAfter, dos);
            filterEndDate.AddCondition(nameof(ipg_carrierprecerthcpcs.ipg_EffectiveEndDate).ToLower(), ConditionOperator.Null);
            query.Criteria.AddFilter(filterEndDate);

            LinkEntity linkCptPrecert = new LinkEntity(ipg_carrierprecerthcpcs.EntityLogicalName,
                                                       ipg_carrierprecertcpt.EntityLogicalName,
                                                       nameof(ipg_carrierprecerthcpcs.ipg_CarrierPrecertCPTId).ToLower(),
                                                       nameof(ipg_carrierprecertcpt.ipg_carrierprecertcptId).ToLower(),
                                                       JoinOperator.Inner);
            linkCptPrecert.EntityAlias = "icp";
            linkCptPrecert.LinkCriteria = new FilterExpression(LogicalOperator.And);
            linkCptPrecert.LinkCriteria.AddCondition(nameof(ipg_carrierprecertcpt.ipg_EffectiveStartDate).ToLower(), ConditionOperator.OnOrBefore, dos);
            linkCptPrecert.LinkCriteria.AddCondition(nameof(ipg_carrierprecertcpt.ipg_RequirementType).ToLower(), ConditionOperator.Equal, (int)ipg_CarrierPrecertCPTRequirementType.Threshold);
            linkCptPrecert.LinkCriteria.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CarrierId).ToLower(), ConditionOperator.Equal, incident.ipg_CarrierId.Id);
            linkCptPrecert.Columns.AddColumn(nameof(ipg_carrierprecertcpt.ipg_threshold).ToLower());
            linkCptPrecert.Columns.AddColumn(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower());

            var filterEndDateCptPrecert = new FilterExpression(LogicalOperator.Or);
            filterEndDateCptPrecert.AddCondition(nameof(ipg_carrierprecertcpt.ipg_EffectiveEndDate).ToLower(), ConditionOperator.OnOrAfter, dos);
            filterEndDateCptPrecert.AddCondition(nameof(ipg_carrierprecertcpt.ipg_EffectiveEndDate).ToLower(), ConditionOperator.Null);
            linkCptPrecert.LinkCriteria.AddFilter(filterEndDateCptPrecert);

            var filterCpt = new FilterExpression(LogicalOperator.Or);
            if (incident.ipg_CPTCodeId1 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId1.Id);
            if (incident.ipg_CPTCodeId2 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId2.Id);
            if (incident.ipg_CPTCodeId3 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId3.Id);
            if (incident.ipg_CPTCodeId4 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId4.Id);
            if (incident.ipg_CPTCodeId5 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId5.Id);
            if (incident.ipg_CPTCodeId6 != null) filterCpt.AddCondition(nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(), ConditionOperator.Equal, incident.ipg_CPTCodeId6.Id);
            linkCptPrecert.LinkCriteria.AddFilter(filterCpt);
            query.LinkEntities.Add(linkCptPrecert);

            LinkEntity cpt = new LinkEntity(ipg_carrierprecertcpt.EntityLogicalName,
                               ipg_cptcode.EntityLogicalName,
                               nameof(ipg_carrierprecertcpt.ipg_CPTId).ToLower(),
                               nameof(ipg_cptcode.ipg_cptcodeId).ToLower(),
                               JoinOperator.Inner);
            cpt.EntityAlias = "cpt";
            cpt.Columns.AddColumn("ipg_cptcode");
            linkCptPrecert.LinkEntities.Add(cpt);

            var queryResponse = organizationService.RetrieveMultiple(query);
            return Enumerable.Cast<ipg_carrierprecerthcpcs>(queryResponse.Entities);
        }

        private void ValidatePrecerts(Incident incident, DateTime dos)
        {
            //Combine all the precerts for different CPT codes on the case.
            var precerts = GetCptPrecertList(incident, dos);

            //If CPT LOMN, create task
            if(precerts.Any(x => x.ipg_RequirementType.Value == (int)ipg_CarrierPrecertCPTRequirementType.CPTLOMN))
            {
                tracingService.Trace($"ValidatePreCertRequirementsOnCase: Found {precerts.Count(x => x.ipg_RequirementType.Value == (int)ipg_CarrierPrecertCPTRequirementType.CPTLOMN)} CPT Precerts");
                var cptCodes = precerts.Where(x => x.ipg_RequirementType.Value == (int)ipg_CarrierPrecertCPTRequirementType.CPTLOMN)
                                       .Select(x => x.ipg_CPTId.Name).ToList();
                CreateTask(incident, "CPTLOMN", $"Carrier [{incident.ipg_CarrierId.Name}] requires a LOMN to be submitted for preauthorization when CPT [{string.Join(",", cptCodes)}] is used");
            }
            
            //If High Dollar, create task
            if (precerts.Any(x => x.ipg_RequirementType.Value == (int)ipg_CarrierPrecertCPTRequirementType.HighDollar))
            {
                var cptCodes = precerts.Where(x => x.ipg_RequirementType.Value == (int)ipg_CarrierPrecertCPTRequirementType.HighDollar)
                                       .Select(x => x.ipg_CPTId.Name).ToList();
                CreateTask(incident, "HighDollar", $"Carrier [{incident.ipg_CarrierId.Name}] may require preauthorization for CPT(s) [{string.Join(",", cptCodes)}] as these are often high dollar procedures");
            }

            //For HCPCS LOMN Precerts
            var hcpcsLOMNPrecertList = GetHcpcsLOMNPrecertList(incident, dos);

            if (hcpcsLOMNPrecertList.Any())
            {
                foreach(var hcpcsLomn in hcpcsLOMNPrecertList.GroupBy(x => x.ipg_CarrierPrecertCPTId, (key, g) => new { PrecertCptRef = key, HcpcsPrecertList = g.ToList() }))
                {
                    var cptCode = hcpcsLomn.HcpcsPrecertList[0].GetAttributeValue<AliasedValue>("cpt.ipg_cptcode");
                    var hcpcsLomnCodes = hcpcsLomn.HcpcsPrecertList.Select(x => x.ipg_ChargeCenterHCPCS.Name).ToList();
                    CreateTask(incident, "HCPCSLOMN", $"Carrier [{incident.ipg_CarrierId.Name}] requires a LOMN to be submitted for preauthorization when the following HCPCS codes are used with CPT [{cptCode.Value}]: [{string.Join(",", hcpcsLomnCodes)}]");
                }
            }

            var thresholdHcpcsList = GetThresholdPrecertList(incident, dos);
            if (thresholdHcpcsList.Any())
            {
                foreach (var hcpcsThreshold in thresholdHcpcsList.GroupBy(x => x.ipg_CarrierPrecertCPTId, (key, g) => new { PrecertCptRef = key, HcpcsPrecertList = g.ToList() }))
                {
                    var cptCode = hcpcsThreshold.HcpcsPrecertList.FirstOrDefault().GetAttributeValue<AliasedValue>("cpt.ipg_cptcode");
                    var hcpcsList = hcpcsThreshold.HcpcsPrecertList.Select(x => x.ipg_ChargeCenterHCPCS.Name).ToList();
                    decimal maxBilledChrg = GetMaxBilledCharges(incident, dos, hcpcsList);
                    var maxThreshold = hcpcsThreshold.HcpcsPrecertList.Max(x => Convert.ToDecimal(x.GetAttributeValue<AliasedValue>("icp.ipg_threshold").Value));
                    if (maxBilledChrg >= maxThreshold)
                    {
                        CreateTask(incident, "Threshold", $"Carrier [{incident.ipg_CarrierId.Name}] requires pre-cert for CPT [{cptCode.Value}] costs in excess of $[{maxThreshold}]");
                    }
                }
            }

        }

        private decimal GetMaxBilledCharges(Incident incident, DateTime dos, List<string> hcpcsList)
        {
            var query = new QueryExpression(ipg_chargecenter.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet(nameof(ipg_chargecenter.ipg_billedcharge))
            };
            query.TopCount = 1;
            query.Criteria.AddCondition(nameof(ipg_chargecenter.ipg_name), ConditionOperator.In, hcpcsList.ToArray());
            query.Criteria.AddCondition(nameof(ipg_chargecenter.ipg_EffectiveDate).ToLower(), ConditionOperator.OnOrBefore, dos);

            var filterEndDate = new FilterExpression(LogicalOperator.Or);
            filterEndDate.AddCondition(nameof(ipg_chargecenter.ipg_ExpirationDate).ToLower(), ConditionOperator.OnOrAfter, dos);
            filterEndDate.AddCondition(nameof(ipg_chargecenter.ipg_ExpirationDate).ToLower(), ConditionOperator.Null);
            query.Criteria.AddFilter(filterEndDate);

            query.AddOrder(nameof(ipg_chargecenter.ipg_billedcharge), OrderType.Descending);

            var maxBilledChrg = organizationService.RetrieveMultiple(query).Entities.FirstOrDefault();
            if(maxBilledChrg != null)
            {
                if (maxBilledChrg.ToEntity<ipg_chargecenter>().ipg_billedcharge != null) return maxBilledChrg.ToEntity<ipg_chargecenter>().ipg_billedcharge.Value;
            }
            return decimal.Zero;
        }

        internal void CreateTask(Incident incident, string category, string subject)
        {
            var requiredInformationTaskQuery = new QueryExpression
            {
                EntityName = Task.EntityLogicalName,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression(LogicalOperator.And),
                PageInfo = new PagingInfo
                {
                    ReturnTotalRecordCount = true
                },
            };

            requiredInformationTaskQuery.Criteria.AddCondition("category", ConditionOperator.Equal, category);
            requiredInformationTaskQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, incident.Id);

            if (organizationService.RetrieveMultiple(requiredInformationTaskQuery).TotalRecordCount > 0) return;

            // Create a task activity to 
            Task precertTask = new Task();

            precertTask.Subject = subject;
            precertTask.Category = category;

            // Refer to the case in the task activity.
            precertTask.RegardingObjectId = new EntityReference(incident.LogicalName, incident.Id);

            // Create the task in Microsoft Dynamics CRM.
            tracingService.Trace("Precert Task Creation: " + subject);
            organizationService.Create(precertTask);
        }

    }
}
