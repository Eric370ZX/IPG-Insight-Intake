using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Common
{
    /// <summary>
    /// This class will be used to determine if the part added to the case is billable on not.
    /// </summary>
    public class CasePartValidator
    {
        private ITracingService _tracing;
        private IOrganizationService _crmService { get; }
        private Incident _caseDetail { get; }
        private DateTime _dos { get; }
        private Intake.Product _product { get; }
        private ipg_PurchaseOrderTypes? _poType { get; }
        private ipg_casepartdetail _target { get; }
        private ipg_casepartdetail _preImage { get; }


        public CasePartValidator(ipg_casepartdetail target, ipg_casepartdetail preImage, IOrganizationService crmService, ITracingService tracing)
        {
            _tracing = tracing;
            _crmService = crmService;

            _target = target;
            _preImage = preImage;

            _tracing.Trace($"init CasePartValidator for {nameof(ipg_casepartdetail)} with id {_target.Id}");

            var caseId = target.Contains(ipg_casepartdetail.Fields.ipg_caseid) ? target.ipg_caseid : preImage?.ipg_caseid;
            var productId = target.Contains(ipg_casepartdetail.Fields.ipg_productid) ? target.ipg_productid : preImage?.ipg_productid;

            _poType = target.Contains(nameof(ipg_casepartdetail.ipg_potypecode)) ? target.ipg_potypecodeEnum : preImage?.ipg_potypecodeEnum;

            _tracing.Trace($"Retrieving Case with id {caseId.Id}");
            _caseDetail = _crmService.Retrieve(Incident.EntityLogicalName, caseId.Id, new ColumnSet(
                                Incident.Fields.ipg_ActualDOS
                                , Incident.Fields.ipg_SurgeryDate
                                , Incident.Fields.ipg_CarrierId
                                , Incident.Fields.ipg_HomePlanCarrierId
                                , Incident.Fields.ipg_FacilityId)).ToEntity<Incident>();

            _dos = _caseDetail.ipg_ActualDOS.HasValue ? _caseDetail.ipg_ActualDOS.Value : _caseDetail.ipg_SurgeryDate.Value;
            _tracing.Trace($"DOS from Case {_dos}");

            _tracing.Trace($"Retrieving Product with id {productId.Id}");
            _product = _crmService.Retrieve(Intake.Product.EntityLogicalName, productId.Id, new ColumnSet(
                        Intake.Product.Fields.ipg_HCPCSCodeId,
                        Intake.Product.Fields.ipg_boxquantity
                        )).ToEntity<Intake.Product>();
        }

        internal bool IsPOTypeChangeValid()
        {
            _tracing.Trace($"Running {nameof(this.IsPOTypeChangeValid)}");

            if (_target.Contains(ipg_casepartdetail.Fields.ipg_potypecode))
            {
                var poRef = _target.Contains(ipg_casepartdetail.Fields.ipg_PurchaseOrderId) ? _target.ipg_PurchaseOrderId : _preImage?.ipg_PurchaseOrderId;
                var po = poRef != null ? _crmService.Retrieve(poRef.LogicalName, poRef.Id, new ColumnSet(SalesOrder.Fields.StateCode)).ToEntity<SalesOrder>() : null;

                if ((po?.StateCode ?? SalesOrderState.Active) != SalesOrderState.Active)
                {
                    return false;
                }
            }

            return true;
        }


        public CasePartValidator(ipg_estimatedcasepartdetail target, ipg_estimatedcasepartdetail preImage, IOrganizationService crmService, ITracingService tracing)
        {
            _tracing = tracing;
            _crmService = crmService;

            _tracing.Trace($"init CasePartValidator for {nameof(ipg_casepartdetail)} with id {target.Id}");

            var caseId = target.Contains(ipg_estimatedcasepartdetail.Fields.ipg_caseid) ? target.ipg_caseid : preImage?.ipg_caseid;
            var productId = target.Contains(ipg_estimatedcasepartdetail.Fields.ipg_productid) ? target.ipg_productid : preImage?.ipg_productid;

            _poType = target.Contains(nameof(ipg_casepartdetail.ipg_potypecode)) ? target.ipg_potypecodeEnum : preImage?.ipg_potypecodeEnum;

            _tracing.Trace($"Retrieving Case with id {caseId.Id}");
            _caseDetail = _crmService.Retrieve(Incident.EntityLogicalName, caseId.Id, new ColumnSet(
                                Incident.Fields.ipg_ActualDOS, Incident.Fields.ipg_SurgeryDate, Incident.Fields.ipg_CarrierId, Incident.Fields.ipg_HomePlanCarrierId)).ToEntity<Incident>();

            _dos = _caseDetail.ipg_ActualDOS.HasValue ? _caseDetail.ipg_ActualDOS.Value : _caseDetail.ipg_SurgeryDate.Value;
            _tracing.Trace($"DOS from Case {_dos}");

            _tracing.Trace($"Retrieving Product with id {productId.Id}");
            _product = _crmService.Retrieve(Intake.Product.EntityLogicalName, productId.Id, new ColumnSet(
                        Intake.Product.Fields.ipg_HCPCSCodeId,
                        Intake.Product.Fields.ipg_boxquantity
                        )).ToEntity<Intake.Product>();
        }

        /// <summary>
        /// Determine is the carrier part detail record exists for the primary carrier and part being added to the case.
        /// If yes, then this part should not be added to the case. 
        /// </summary>
        /// <returns></returns>
        public bool IsValidAsPerCarrierPart()
        {
            _tracing.Trace($"Running {nameof(this.IsValidAsPerCarrierPart)}");

            Func<Guid, QueryExpression> queryCarrierPart = (carrierId) =>
            {
                //Fetch all carrier part details with the following criteria:
                //1. Carrier is the same as the primary carrier on the case.
                //2. Part is the same as the part user is trying to add to the case.
                //3. Case DOS falls withing effective and expiration date(Nullable) on the carrier part detail.
                return new QueryExpression(ipg_carrierpart.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression(LogicalOperator.Or)
                    {
                        Filters =
                    {
                        new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                                            {
                                                new ConditionExpression(ipg_carrierpart.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierId),
                                                new ConditionExpression(ipg_carrierpart.Fields.ipg_PartId, ConditionOperator.Equal, _product.Id),
                                                new ConditionExpression(ipg_carrierpart.Fields.ipg_EffectiveDate, ConditionOperator.LessEqual, _dos),
                                                new ConditionExpression(ipg_carrierpart.Fields.ipg_ExpirationDate, ConditionOperator.GreaterEqual, _dos)
                                            }
                        },
                        new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                                            {
                                                new ConditionExpression(ipg_carrierpart.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierId),
                                                new ConditionExpression(ipg_carrierpart.Fields.ipg_PartId, ConditionOperator.Equal, _product.Id),
                                                new ConditionExpression(ipg_carrierpart.Fields.ipg_EffectiveDate, ConditionOperator.LessEqual, _dos),
                                                new ConditionExpression(ipg_carrierpart.Fields.ipg_ExpirationDate, ConditionOperator.Null)//Expiration date can be null.
                                            }
                        }
                    }

                    }
                };
            };

            if (_caseDetail.ipg_HomePlanCarrierId != null)
            {
                var queryCarrierPartHomePlan = queryCarrierPart(_caseDetail.ipg_HomePlanCarrierId.Id);
                //Part is not supported if the record is found.
                if (_crmService.RetrieveMultiple(queryCarrierPartHomePlan).Entities.Any()) return false; 
                else return true; 
            }

            var queryCarrierPartPrimaryCarrier = queryCarrierPart(_caseDetail.ipg_CarrierId.Id);
            //Part is not supported if the record is found.
            if (_crmService.RetrieveMultiple(queryCarrierPartPrimaryCarrier).Entities.Any()) return false;

            return true;
        }

        /// <summary>
        /// Check that MP part can't be created for ZPO
        /// </summary>
        public bool IsValidMultipackForZPO()
        {
            _tracing.Trace($"Running {nameof(this.IsValidMultipackForZPO)}");

            if (_poType == ipg_PurchaseOrderTypes.ZPO && _product != null)
            {
                if (_product.ipg_boxquantity > 1)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsValidAsPerPartId()
        {
            _tracing.Trace($"Running {nameof(this.IsValidAsPerPartId)}");

            var queryExpressionCFR = new QueryExpression(ipg_carrierfeeschedule.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_carrierfeeschedule.EntityLogicalName, ipg_carrierfeeschedule.Fields.ipg_carrierid, ConditionOperator.Equal, _caseDetail.ipg_CarrierId.Id),
                        new ConditionExpression(ipg_carrierfeeschedule.EntityLogicalName, ipg_carrierfeeschedule.Fields.ipg_effectivedate, ConditionOperator.LessEqual, _dos),
                        new ConditionExpression(ipg_carrierfeeschedule.EntityLogicalName, ipg_carrierfeeschedule.Fields.ipg_expiredate, ConditionOperator.GreaterEqual, _dos)
                    }
                }
            };

            var feeScheduleEntityCollection = _crmService.RetrieveMultiple(queryExpressionCFR);

            if (feeScheduleEntityCollection.Entities.Any())
            {
                var carrierfeeschedule = feeScheduleEntityCollection.Entities[0].ToEntity<ipg_carrierfeeschedule>();

                if (carrierfeeschedule.ipg_feescheduleid != null)
                {
                    var queryCM = new QueryExpression(ipg_chargecenter.EntityLogicalName)
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                        {
                            new ConditionExpression(ipg_chargecenter.Fields.ipg_FeeScheduleId, ConditionOperator.Equal, carrierfeeschedule.ipg_feescheduleid.Id),
                            new ConditionExpression(ipg_chargecenter.Fields.ipg_EffectiveDate, ConditionOperator.LessEqual, _dos),
                            new ConditionExpression(ipg_chargecenter.Fields.ipg_ExpirationDate, ConditionOperator.GreaterEqual, _dos),
                            new ConditionExpression(ipg_chargecenter.Fields.ipg_PartCategoryId, ConditionOperator.Equal, _product.Id)
                        }
                        }
                    };

                    var chargemasterEntityCollection = _crmService.RetrieveMultiple(queryCM);
                    if (chargemasterEntityCollection.Entities.Any())
                    {
                        return chargemasterEntityCollection.Entities[0].ToEntity<ipg_chargecenter>().ipg_Supported.Value;
                    }
                }

            }

            return true;
        }


        public bool IsValidAsPerPartHCPCS()
        {
            _tracing.Trace($"Running {nameof(this.IsValidAsPerPartHCPCS)}");

            if (_caseDetail.ipg_CarrierId != null)
            {
                var queryExpressionCFR = new QueryExpression(ipg_carrierfeeschedule.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                    {
                        new ConditionExpression(ipg_carrierfeeschedule.Fields.ipg_carrierid, ConditionOperator.Equal, _caseDetail.ipg_CarrierId.Id),
                        new ConditionExpression(ipg_carrierfeeschedule.Fields.ipg_effectivedate, ConditionOperator.LessEqual, _dos),
                        new ConditionExpression(ipg_carrierfeeschedule.Fields.ipg_expiredate, ConditionOperator.GreaterEqual, _dos)
                    }
                    }
                };

                var feeScheduleEntityCollection = _crmService.RetrieveMultiple(queryExpressionCFR);

                if (feeScheduleEntityCollection.Entities.Any())
                {
                    var carrierfeeschedule = feeScheduleEntityCollection.Entities[0].ToEntity<ipg_carrierfeeschedule>();

                    if (carrierfeeschedule.ipg_feescheduleid != null)
                    {
                        var queryCM = new QueryExpression(ipg_chargecenter.EntityLogicalName)
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression(LogicalOperator.And)
                            {
                                Conditions =
                        {
                            new ConditionExpression(ipg_chargecenter.Fields.ipg_FeeScheduleId, ConditionOperator.Equal, carrierfeeschedule.ipg_feescheduleid.Id),
                            new ConditionExpression(ipg_chargecenter.Fields.ipg_EffectiveDate, ConditionOperator.LessEqual, _dos),
                            new ConditionExpression(ipg_chargecenter.Fields.ipg_ExpirationDate, ConditionOperator.GreaterEqual, _dos),
                            new ConditionExpression(ipg_chargecenter.Fields.ipg_HCPCSCodeId, ConditionOperator.Equal, _product.ipg_HCPCSCodeId.Id)
                        }
                            }
                        };

                        var chargemasterEntityCollection = _crmService.RetrieveMultiple(queryCM);
                        if (chargemasterEntityCollection.Entities.Any())
                        {
                            return chargemasterEntityCollection.Entities[0].ToEntity<ipg_chargecenter>().ipg_Supported.Value;
                        }

                    }
                }
            }

            return true;
        }

        public bool IsDuplicate()
        {
            _tracing.Trace($"Running {nameof(this.IsDuplicate)}");
            var queryExpression = new QueryExpression(ipg_casepartdetail.EntityLogicalName)
            {
                TopCount = 1,
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                         new ConditionExpression(ipg_casepartdetail.Fields.ipg_productid, ConditionOperator.Equal, _product.Id),
                         new ConditionExpression(ipg_casepartdetail.Fields.ipg_caseid,  ConditionOperator.Equal, _caseDetail.Id),
                    }
                }
            };

            if (_preImage?.Id != null)
            {
                queryExpression.Criteria.AddCondition(ipg_casepartdetail.PrimaryIdAttribute, ConditionOperator.NotEqual, _preImage.Id);
            }
            else
            {
                queryExpression.Criteria.AddCondition(ipg_casepartdetail.Fields.ipg_quantity, ConditionOperator.LessThan, (decimal)0);
            }

            return _crmService.RetrieveMultiple(queryExpression).Entities.Any();
        }

        public bool IsPOTypeValid()
        {
            _tracing.Trace($"Running {nameof(this.IsPOTypeValid)}");
            var facility = _caseDetail.ipg_FacilityId != null ?
                _crmService.Retrieve(_caseDetail.ipg_FacilityId.LogicalName, _caseDetail.ipg_FacilityId.Id
                , new ColumnSet(Intake.Account.Fields.ipg_cpaonlyfacility, Intake.Account.Fields.ipg_dtmmember)).ToEntity<Intake.Account>() : null;

            if (_target.Contains(ipg_casepartdetail.Fields.ipg_potypecode))
            {
                if (_target.ipg_potypecodeEnum == null)
                {
                    _tracing.Trace($"PO type cannot be null");

                    return false;
                }
                else
                {
                    _tracing.Trace($"PO Type: {_poType}");

                    if ((facility?.ipg_dtmmemberEnum == Account_ipg_dtmmember.No
                        || facility?.ipg_cpaonlyfacilityEnum == Account_ipg_cpaonlyfacility.Yes
                        || ((_product.ipg_boxquantity ?? 0) > 1)) && _poType != ipg_PurchaseOrderTypes.CPA)
                    {
                        _tracing.Trace($"CPA only allowed, product box quantity > 1 or facility CPA only or DTMMember flag on facility is false");

                        return false;
                    }
                    else if (DateTime.Now.Date <= _dos.Date && _poType != ipg_PurchaseOrderTypes.TPO)
                    {
                        _tracing.Trace($"DOS {_dos} is before today {DateTime.Now}. Actual Part with po type TPO only allowed!");

                        return false;
                    }
                    else if (DateTime.Now.Date > _dos.Date)
                    {
                        return _poType == ipg_PurchaseOrderTypes.CPA || _poType == ipg_PurchaseOrderTypes.MPO || _poType == ipg_PurchaseOrderTypes.ZPO;
                    }

                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
