using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace Insight.Intake.Plugin.Case
{
    public class SetBilledCptOnCase : PluginBase
    {
        readonly string[] COLUMNSETINCIDENT = {
                           Incident.Fields.ipg_CarrierId,
                            Incident.Fields.ipg_CPTCodeId1,
                            Incident.Fields.ipg_CPTCodeId2,
                            Incident.Fields.ipg_CPTCodeId3,
                            Incident.Fields.ipg_CPTCodeId4,
                            Incident.Fields.ipg_CPTCodeId5,
                            Incident.Fields.ipg_CPTCodeId6,
                            Incident.Fields.ipg_SurgeryDate,
                            Incident.Fields.ipg_ActualDOS,
                            Incident.Fields.CreatedOn
        };

        readonly string[] COLUMNSET_CHARGE_CENTER =
        {
            ipg_chargecenter.Fields.ipg_billedcharge,
            ipg_chargecenter.Fields.ipg_name,
            ipg_chargecenter.Fields.ipg_CPTNameId,
            ipg_chargecenter.Fields.ipg_PriceFactor,
        };

        public SetBilledCptOnCase() : base(typeof(SetBilledCptOnCase))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Incident.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;

            var service = localPluginContext.OrganizationService;
            var target = localPluginContext.Target<Incident>();

            var incident = ((Entity)context.InputParameters["Target"]).ToEntity<Incident>();

            if (context.MessageName == MessageNames.Create &&
               (!target.Contains(Incident.Fields.ipg_CarrierId)
               || (!target.Contains(Incident.Fields.ipg_ActualDOS) &&
                   !target.Contains(Incident.Fields.ipg_SurgeryDate))
               || (!target.Contains(Incident.Fields.ipg_CPTCodeId1) &&
                   !target.Contains(Incident.Fields.ipg_CPTCodeId2) &&
                   !target.Contains(Incident.Fields.ipg_CPTCodeId3) &&
                   !target.Contains(Incident.Fields.ipg_CPTCodeId4) &&
                   !target.Contains(Incident.Fields.ipg_CPTCodeId5) &&
                   !target.Contains(Incident.Fields.ipg_CPTCodeId6))))
            {
                return;
            }

            if (incident != null)
            {

                var caseReference = context.MessageName == MessageNames.Create ? incident :
                    service.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(COLUMNSETINCIDENT)).ToEntity<Incident>();

                //return if no carrier on the case
                if (caseReference.ipg_CarrierId.Id == null) return;

                //get the carrier reference
                var carrierReference = service.Retrieve(Intake.Account.EntityLogicalName, caseReference.ipg_CarrierId.Id,
                    new ColumnSet(Intake.Account.Fields.ipg_CarrierCPTPricing)).ToEntity<Intake.Account>();

                var cptList = BuildCptList(caseReference, incident);


                if (cptList.Count == 0)
                {
                    //Remove the existing billed cpt if no value is found
                    incident.ipg_BilledCPTId = null;
                    incident[Incident.Fields.ipg_billedcpt] = "";
                    return;
                }

                var feeScheduleQueryExpression = new QueryExpression
                {
                    EntityName = ipg_carrierfeeschedule.EntityLogicalName,
                    ColumnSet = new ColumnSet(ipg_carrierfeeschedule.Fields.ipg_feescheduleid),
                    Criteria = new FilterExpression
                    {

                        Conditions =
                            {
                                new ConditionExpression
                                {
                                    AttributeName = ipg_carrierfeeschedule.Fields.ipg_effectivedate,
                                    Operator = ConditionOperator.LessEqual,
                                    Values = {caseReference.ipg_SurgeryDate}
                                },
                                new ConditionExpression
                                {
                                    AttributeName = ipg_carrierfeeschedule.Fields.ipg_expiredate,
                                    Operator = ConditionOperator.GreaterEqual,
                                    Values = {caseReference.ipg_SurgeryDate}
                                },
                                new ConditionExpression
                                {
                                    AttributeName = ipg_carrierfeeschedule.Fields.ipg_carrierid,
                                    Operator = ConditionOperator.Equal,
                                    Values = {caseReference.ipg_CarrierId.Id }
                                }
                            },
                        FilterOperator = LogicalOperator.And
                    }
                };
                //   Retrieve the carrier feeschedule for this carrier



                var carrierFeeschedules = service.RetrieveMultiple(feeScheduleQueryExpression).Entities;



                if (carrierFeeschedules.Count > 0)
                {
                    var carrierFeescheduleId = (ipg_carrierfeeschedule)carrierFeeschedules[0];
                    //retrieve pricingmodel from the feeschedule

                    if (carrierFeescheduleId.ipg_feescheduleid != null)
                    {
                        var feeScheduleReference = service.Retrieve(ipg_feeschedule.EntityLogicalName, carrierFeescheduleId.ipg_feescheduleid.Id,
                        new ColumnSet(ipg_feeschedule.Fields.ipg_pricingmodel)).ToEntity<ipg_feeschedule>();

                        var pricingModel = feeScheduleReference?.ipg_pricingmodel?.Value;

                        //retrieve the carrier cpt_pricing if pricingmodel is not set on the feeschedule
                        string carrierCptPricing = carrierReference.FormattedValues[Intake.Account.Fields.ipg_CarrierCPTPricing];

                        var cptPricing = pricingModel ?? Convert.ToInt32(carrierCptPricing);

                        //RT # 10252  If there is not actual dos then use the scheduled date for effective date compare.  If neither use the date_entered
                        // 2.4.15-tb. RT #14841: Contract--IPG Fee Schedule--Effective dates are not comparing against Sch/Actual DOS.
                        // Moved the following line of code out of the following if() block because it's needed for the if() immediately after.
                        DateTime dos = caseReference.ipg_ActualDOS != null ? (DateTime)caseReference.ipg_ActualDOS :
                            (caseReference.ipg_SurgeryDate != null ? (DateTime)caseReference.ipg_SurgeryDate : (DateTime)caseReference.CreatedOn);


                        if (cptPricing == 1 || cptPricing == 2)
                        {
                            var queryExpression = new QueryExpression
                            {
                                EntityName = ipg_chargecenter.EntityLogicalName,
                                ColumnSet = new ColumnSet(ipg_chargecenter.Fields.ipg_billedcharge,
                                                          ipg_chargecenter.Fields.ipg_name,
                                                          ipg_chargecenter.Fields.ipg_PriceFactor,
                                                          ipg_chargecenter.Fields.ipg_CPTNameId),
                                LinkEntities =
                                    {
                                        new LinkEntity(ipg_chargecenter.EntityLogicalName,ipg_feeschedule.EntityLogicalName,
                                        ipg_chargecenter.Fields.ipg_FeeScheduleId, ipg_feeschedule.Fields.ipg_feescheduleId, JoinOperator.Inner)
                                        {
                                            LinkEntities = {
                                                new LinkEntity(ipg_feeschedule.EntityLogicalName,ipg_carrierfeeschedule.EntityLogicalName
                                            , ipg_chargecenter.Fields.ipg_FeeScheduleId, ipg_feeschedule.Fields.ipg_feescheduleId, JoinOperator.Inner)
                                                {
                                                    LinkEntities =
                                                    {
                                                        new LinkEntity(ipg_carrierfeeschedule.EntityLogicalName,Intake.Account.EntityLogicalName,
                                                        ipg_carrierfeeschedule.Fields.ipg_carrierid, Intake.Account.Fields.AccountId, JoinOperator.Inner)
                                                        {
                                                            LinkCriteria = new FilterExpression
                                                            {
                                                                Conditions =
                                                                {
                                                                    new ConditionExpression
                                                                    {
                                                                        AttributeName = Intake.Account.Fields.AccountId,
                                                                        Operator = ConditionOperator.Equal,
                                                                        Values = { caseReference.ipg_CarrierId.Id }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    },
                                Criteria = new FilterExpression
                                {
                                    Conditions =
                                        {
                                            new ConditionExpression
                                            {
                                                AttributeName = ipg_chargecenter.Fields.ipg_Supported,
                                                Operator = ConditionOperator.NotEqual,
                                                Values = {false}
                                            },
                                            new ConditionExpression
                                            {
                                                AttributeName = ipg_chargecenter.Fields.ipg_EffectiveDate,
                                                Operator = ConditionOperator.LessEqual,
                                                Values = {dos}
                                            },
                                            //TODO: If possible add the default condition of isnull(expiry_date,'9000-01-01') 
                                            new ConditionExpression
                                            {
                                                AttributeName = ipg_chargecenter.Fields.ipg_ExpirationDate,
                                                Operator = ConditionOperator.GreaterEqual,
                                                Values = {dos}
                                            }
                                        },
                                    FilterOperator = LogicalOperator.And
                                },
                                Orders =
                                    {
                                        new OrderExpression
                                        {
                                            AttributeName = ipg_chargecenter.Fields.ipg_billedcharge,
                                            OrderType = OrderType.Descending
                                        }
                                    }
                            };
                            //Adding link to CPT code 
                            queryExpression.LinkEntities.Add(new LinkEntity(ipg_chargecenter.EntityLogicalName, ipg_cptcode.EntityLogicalName
                                    , ipg_chargecenter.Fields.ipg_CPTNameId, ipg_cptcode.Fields.ipg_cptcodeId, JoinOperator.Inner)
                            { Columns = new ColumnSet("ipg_cptcode", ipg_cptcode.Fields.ipg_cptcodeId), EntityAlias = "cptcode" });

                            ///we will be using this filter expression 
                            #region CPT_FILTER_EXPERSSION
                            var filterExpression = new FilterExpression();

                            for (int i = 0; i < cptList.Count; i++)
                            {
                                filterExpression.Conditions.Add(new ConditionExpression("ipg_cptcodeid", ConditionOperator.Equal, cptList[i]));

                            }
                            filterExpression.FilterOperator = LogicalOperator.Or;
                            #endregion

                            queryExpression.LinkEntities[1].LinkCriteria = filterExpression;

                            var chargeCenterRecordsWithCptPricing = service.RetrieveMultiple(queryExpression).Entities;


                            if (chargeCenterRecordsWithCptPricing.Count > 0)
                            {
                                incident.ipg_BilledCPTId = ((ipg_chargecenter)chargeCenterRecordsWithCptPricing[0]).ipg_CPTNameId;
                                incident[Incident.Fields.ipg_billedcpt] = ((ipg_chargecenter)chargeCenterRecordsWithCptPricing[0]).ipg_CPTNameId.Name.Substring(0, 25);

                                //get the procedure name reference
                                var cptReference = service.Retrieve(ipg_cptcode.EntityLogicalName, incident.ipg_BilledCPTId.Id,
                                    new ColumnSet(ipg_cptcode.Fields.ipg_procedurename)).ToEntity<Intake.ipg_cptcode>();
                                incident.ipg_procedureid = cptReference.ipg_procedurename;
                                return;
                            }
                        }

                        ///<Query>
                        ///$query = "SELECT TOP 1 billedchg, name as cpt_code 
                        ///FROM ipg_charge_master
                        ///WHERE deleted = 0
                        ///AND feeschedule_id = '?'
                        ///AND name in ({$codeString})
                        ///AND '{$dateOfService}' BETWEEN effective_date AND ISNULL(expire_date, '9000-01-01')
                        ///ORDER BY ranking DESC";
                        ///</Query>

                        var chargeCenterqueryExpression = new QueryExpression
                        {
                            EntityName = ipg_chargecenter.EntityLogicalName,
                            ColumnSet = new ColumnSet(ipg_chargecenter.Fields.ipg_billedcharge, ipg_chargecenter.Fields.ipg_name, ipg_chargecenter.Fields.ipg_PriceFactor, ipg_chargecenter.Fields.ipg_CPTRank, ipg_chargecenter.Fields.ipg_CPTNameId),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                        {
                                            new ConditionExpression
                                            {
                                                AttributeName = ipg_chargecenter.Fields.ipg_EffectiveDate,
                                                Operator = ConditionOperator.LessEqual,
                                                Values = {dos}
                                            },
                                            new ConditionExpression
                                            {
                                                AttributeName = ipg_chargecenter.Fields.ipg_ExpirationDate,
                                                Operator = ConditionOperator.GreaterEqual,
                                                Values = {dos}
                                            },
                                            new ConditionExpression
                                            {
                                                AttributeName = ipg_chargecenter.Fields.ipg_FeeScheduleId,
                                                Operator = ConditionOperator.Equal,
                                                Values = { Constants.FeeScheduleGuids.IPG }
                                            }
                                        },
                                FilterOperator = LogicalOperator.And
                            },
                            Orders =
                                    {
                                        new OrderExpression
                                        {
                                            AttributeName = ipg_chargecenter.Fields.ipg_CPTRank,
                                            OrderType = OrderType.Descending
                                        }
                                    },
                            TopCount = 1
                        };
                        var queryFilterExpression = new FilterExpression();
                        for (int i = 0; i < cptList.Count; i++)
                        {
                            queryFilterExpression.Conditions.Add(new ConditionExpression(ipg_chargecenter.Fields.ipg_CPTNameId, ConditionOperator.Equal, cptList[i]));
                        }
                        queryFilterExpression.FilterOperator = LogicalOperator.Or;
                        chargeCenterqueryExpression.Criteria.AddFilter(queryFilterExpression);
                        var chargeCenterRecords = service.RetrieveMultiple(chargeCenterqueryExpression).Entities;
                        if (chargeCenterRecords.Count > 0)
                        {
                            //Update the Billed CPT on the case
                            incident.ipg_BilledCPTId = ((ipg_chargecenter)chargeCenterRecords[0]).ipg_CPTNameId;
                            incident[Incident.Fields.ipg_billedcpt] = ((ipg_chargecenter)chargeCenterRecords[0]).ipg_CPTNameId.Name.Substring(0, 25);

                            //get the procedure name reference
                            var cptReference = service.Retrieve(ipg_cptcode.EntityLogicalName, incident.ipg_BilledCPTId.Id,
                                new ColumnSet(ipg_cptcode.Fields.ipg_procedurename)).ToEntity<Intake.ipg_cptcode>();
                            incident.ipg_procedureid = cptReference.ipg_procedurename;
                        }
                        else
                        {
                            //Remove the existing billed cpt if no value is found
                            incident.ipg_BilledCPTId = null;
                            incident[Incident.Fields.ipg_billedcpt] = "";
                        }
                    }
                }
            }
        }

        private List<Guid> BuildCptList(Incident caseReference, Incident incident)
        {
            if (incident == null)
            {
                throw new ArgumentNullException(nameof(incident));
            }

            var cptIds = new List<Guid>();

            object cptCode1 = null;
            object cptCode2 = null;
            object cptCode3 = null;
            object cptCode4 = null;
            object cptCode5 = null;
            object cptCode6 = null;



            if (incident.ipg_CPTCodeId1 != null)
            {
                cptCode1 = incident.ipg_CPTCodeId1;
                var c = (EntityReference)cptCode1;
                if (cptCode1 != null) cptIds.Add(c.Id);
            }
            else if (caseReference.ipg_CPTCodeId1 != null)
                cptIds.Add(caseReference.ipg_CPTCodeId1.Id);

            if (incident.ipg_CPTCodeId2 != null)
            {
                cptCode2 = incident.ipg_CPTCodeId2;
                if (cptCode2 != null) cptIds.Add((cptCode2 as EntityReference).Id);
            }
            else if (caseReference.ipg_CPTCodeId2 != null)
                cptIds.Add(caseReference.ipg_CPTCodeId2.Id);

            if (incident.ipg_CPTCodeId3 != null)
            {
                cptCode3 = incident.ipg_CPTCodeId3;
                if (cptCode3 != null) cptIds.Add((cptCode3 as EntityReference).Id);
            }
            else if (caseReference.ipg_CPTCodeId3 != null)
                cptIds.Add(caseReference.ipg_CPTCodeId3.Id);

            if (incident.ipg_CPTCodeId4 != null)
            {
                cptCode4 = incident.ipg_CPTCodeId4;
                if (cptCode4 != null) cptIds.Add((cptCode4 as EntityReference).Id);
            }
            else if (caseReference.ipg_CPTCodeId4 != null)
                cptIds.Add(caseReference.ipg_CPTCodeId4.Id);

            if (incident.ipg_CPTCodeId5 != null)
            {
                cptCode5 = incident.ipg_CPTCodeId5;
                if (cptCode5 != null) cptIds.Add((cptCode5 as EntityReference).Id);
            }
            else if (caseReference.ipg_CPTCodeId5 != null)
                cptIds.Add(caseReference.ipg_CPTCodeId5.Id);

            if (incident.ipg_CPTCodeId6 != null)
            {
                cptCode6 = incident.ipg_CPTCodeId6;
                if (cptCode6 != null) cptIds.Add((cptCode6 as EntityReference).Id);
            }
            else if (caseReference.ipg_CPTCodeId6 != null)
                cptIds.Add(caseReference.ipg_CPTCodeId6.Id);


            return cptIds;
        }
    }
}