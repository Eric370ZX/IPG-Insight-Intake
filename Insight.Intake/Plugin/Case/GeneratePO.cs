using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Models;
using Insight.Intake.Plugin.Managers;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Insight.Intake.Plugin.Case
{
    public class GeneratePO : PluginBase
    {
        const string PDFTEMPLATE = "POTemplate";
        const string GENERATEPDF = "GeneratePOPDFDoc";
        const string NOT_REQUIRED = "Not Required";
        const string EMAILTEMPLATE = "PO Comunication";
        const string QUEUEForEmailComunication = "POComunication";

        public GeneratePO() : base(typeof(GeneratePO))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeCaseActionsGeneratePO", Incident.EntityLogicalName, PostOperationHandlerGeneratePO);
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeOrderActionsAfterGeneration", SalesOrder.EntityLogicalName, PostOperationHandlerCreatePDfComunicatePO);
            RegisterEvent(PipelineStages.PostOperation, "ipg_ManualPOCommunication", null, PostOperationHandlerManualCommunicatePO);
        }

        private void PostOperationHandlerCreatePDfComunicatePO(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            SystemUser user = service.Retrieve(SystemUser.EntityLogicalName, context.InitiatingUserId, new ColumnSet("fullname")).ToEntity<SystemUser>();
            EntityReference userRef = user.ToEntityReference();
            userRef.Name = user.FullName;

            var OrderRef = localPluginContext.TargetRef();
            var po = service.Retrieve(OrderRef.LogicalName, OrderRef.Id, new ColumnSet(true)).ToEntity<SalesOrder>();
            var incident = service.Retrieve(po.ipg_CaseId.LogicalName, po.ipg_CaseId.Id, new ColumnSet(true)).ToEntity<Incident>();

            string email = context.InputParameters.Contains("CommunicateTo") ? (string)context.InputParameters["CommunicateTo"] : "";
            bool communicatePo = context.InputParameters.Contains("CommunicatePo") ? (bool)context.InputParameters["CommunicatePo"] : false;

            AfterGenerationPO(po, incident, userRef, service, tracingService, localPluginContext.PluginExecutionContext, email, communicatePo);
            context.OutputParameters["Success"] = true;
        }

        private void PostOperationHandlerGeneratePO(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracing = localPluginContext.TracingService;

            var crmContext = new CrmServiceContext(service);

            SystemUser user = service.Retrieve(SystemUser.EntityLogicalName, context.InitiatingUserId, new ColumnSet("fullname")).ToEntity<SystemUser>();
            EntityReference userRef = user.ToEntityReference();
            userRef.Name = user.FullName;

            var incidentRef = (EntityReference)context.InputParameters["Target"];
            var incident = service.Retrieve(incidentRef.LogicalName, incidentRef.Id, new ColumnSet(true)).ToEntity<Incident>();

            EntityReference manufacturerRef = context.InputParameters.Contains("manufacturer") ? (EntityReference)context.InputParameters["manufacturer"] : null;
            EntityReference previousPORef = context.InputParameters.Contains("previousPO") ? (EntityReference)context.InputParameters["previousPO"] : null;

            var poType = context.InputParameters.Contains("POType") ? (string)context.InputParameters["POType"] : null;
            var estimatedPO = context.InputParameters.Contains("EstimatedPO") ? (bool)context.InputParameters["EstimatedPO"] : false;
            bool communicatePo = context.InputParameters.Contains("CommunicatePo") ? (bool)context.InputParameters["CommunicatePo"] : false;

            CheckCase(incident);

            SalesOrder po = ProcessPOGeneration(service, crmContext, tracing, incident, previousPORef, manufacturerRef, poType, estimatedPO, communicatePo);

            if (po != null)
            {
                context.OutputParameters["Success"] = true;
                context.OutputParameters["poId"] = po.ToEntityReference();
            }
            else
            {
                context.OutputParameters["Success"] = false;
            }
        }

        private void CheckCase(Incident incident)
        {
            if (incident.ipg_FacilityId == null)
            {
                throw new Exception("Case doesn't have Facillity!");
            }
        }

        private SalesOrder ProcessPOGeneration(
            IOrganizationService service, CrmServiceContext crmContext, ITracingService tracing, Incident incident,
            EntityReference previousPORef, EntityReference manufacturerRef, string poType, bool estimatedPO,
            bool communicatePo = true)
        {
            tracing.Trace($"Start Generation PO of {poType} type with mfg id: {manufacturerRef?.Id}");

            SalesOrder poRef = null;

            ipg_PurchaseOrderTypes poTypeCode;

            if (Enum.TryParse(poType, true, out poTypeCode))
            {
                manufacturerRef = poTypeCode == ipg_PurchaseOrderTypes.CPA ? null : manufacturerRef;

                poRef = GeneratePurchaseOrder(service, crmContext, tracing, incident, poTypeCode, previousPORef, manufacturerRef, estimatedPO, communicatePo);
            }

            return poRef;
        }

        private SalesOrder GetPO(
            IOrganizationService service, CrmServiceContext crmContext, ITracingService tracing, Incident incident,
            ipg_PurchaseOrderTypes pOTypeCode, EntityReference manufacturerRef, bool estimatedPO,
            SalesOrder previousPO = null)
        {
            tracing.Trace("Check that there is no active PO with the same type");
            if (estimatedPO)
            {
                var purchaseOrders = (from order in crmContext.SalesOrderSet
                                      where order.ipg_CaseId.Id == incident.Id
                                              && order.ipg_potypecode.Value == (int)pOTypeCode
                                              && order.StateCode.Value == SalesOrderState.Active
                                              && order.ipg_isestimatedpo == estimatedPO
                                      select order).ToList();

                if (manufacturerRef != null)
                {
                    purchaseOrders = purchaseOrders.Where(o => o.ipg_Manufacturer_id?.Id == manufacturerRef.Id).ToList();
                }

                if (purchaseOrders.Any())
                {
                    throw new Exception($@"PO with type {pOTypeCode.ToString()} can't be generated as it already exists!");
                }
            }



            //TODO need to clarify which price list to use
            var priceList = (from priceL in crmContext.PriceLevelSet
                             select priceL).FirstOrDefault()?.ToEntityReference();

            var facility = (from account in crmContext.AccountSet
                            where account.Id == incident.ipg_FacilityId.Id
                            select account).FirstOrDefault();

            //if facility have parent get information from there
            //From Legacy
            // Revised 3.4.11.
            // With new Inventory functionality, we can't just automatically assume that we use the value stored in ipg_patient_procedure's facility_id field
            if (facility.ParentAccountId != null)
            {
                facility = (from account in crmContext.AccountSet
                            where account.Id == facility.ParentAccountId.Id
                            select account).FirstOrDefault();
            }

            tracing.Trace("Get IPG Address");
            Tuple<string, string, string, string> IPGAddress = D365Helpers.GetIPGAddressFromConfiguration(service);

            tracing.Trace("Create SAles ORder entity");

            var purchaseOrder = new SalesOrder()
            {
                ipg_potypecodeEnum = pOTypeCode,
                ipg_CaseId = incident.ToEntityReference(),
                ipg_FacilityId = incident.ipg_FacilityId,
                PriceLevelId = priceList,
                ipg_Manufacturer_id = manufacturerRef,
                Name = previousPO?.Name != null ? GetPOName(previousPO.Name, tracing) : GetPOName(service, tracing, pOTypeCode.ToString(), incident),
                StatusCode = new OptionSetValue((int)SalesOrder_StatusCode.Generated),
                ipg_IsExportedToGPAPCredits = false,
                ipg_vendorid = facility.ipg_VendorIdNumber,
                ipg_previousorderid = previousPO?.ToEntityReference(),

                //marked it so it's comunicated to facility
                ipg_zpodownloaded = false,

                ShipTo_Line1 = facility.Address1_Line1,
                ShipTo_City = facility.Address1_City,
                ShipTo_AddressId = facility.Address1_AddressId,
                ShipTo_PostalCode = facility.Address1_PostalCode,
                ShipTo_StateOrProvince = facility.ipg_StateId?.Name,

                //TODO reate an IPG address config
                BillTo_PostalCode = IPGAddress.Item1,
                BillTo_StateOrProvince = IPGAddress.Item2,
                BillTo_Line1 = IPGAddress.Item3,
                BillTo_City = IPGAddress.Item4,
                ipg_isestimatedpo = estimatedPO,
                ipg_actualPO = !estimatedPO,
            };

            tracing.Trace("Get ipg_suppresshipaa from mfg");

            var suppressHipaa = manufacturerRef == null ? false : service.Retrieve("account", manufacturerRef.Id, new ColumnSet("ipg_suppresshipaa")).GetAttributeValue<bool>("ipg_suppresshipaa");

            if (purchaseOrder.ipg_potypecode.Value != (int)ipg_PurchaseOrderTypes.CPA)
            {
                tracing.Trace("Get GetAccountNumberFromFacilityMFGRelationShip");

                purchaseOrder.ipg_accountnumber = GetAccountNumberFromFacilityMFGRelationShip(service, purchaseOrder);

                if (suppressHipaa == true)
                {
                    purchaseOrder.ipg_casepatientfullname = String.Empty;

                }
                else
                {


                    tracing.Trace("Build ipg_casepatientfullname");

                    var patientFirstName = incident.ipg_PatientFirstName;
                    var patientLName = incident.ipg_PatientLastName;

                    var patientFNameTrimmed = patientFirstName?.Length > 2 ? patientFirstName?.Substring(0, 3) : patientFirstName ?? String.Empty;
                    var patientLNameTrimmed = patientLName?.Length > 2 ? patientLName?.Substring(0, 3) : patientLName ?? String.Empty;

                    purchaseOrder.ipg_casepatientfullname = (patientLNameTrimmed + "_" + patientFNameTrimmed).ToUpper();

                    tracing.Trace($"Built ipg_casepatientfullname = {purchaseOrder.ipg_casepatientfullname}");
                }
            }
            else
            {
                purchaseOrder.ipg_accountnumber = NOT_REQUIRED;
            }

            purchaseOrder.Id = service.Create(purchaseOrder);

            return purchaseOrder;
        }

        private Intake.Account GetManufacturer(IOrganizationService service, EntityReference manufacturerRef)
        {
            return manufacturerRef == null ? null : (Intake.Account)service.Retrieve(manufacturerRef.LogicalName, manufacturerRef.Id
                , new ColumnSet(Intake.Account.Fields.ipg_ManufacturerIsFacilityAcctRequired, Intake.Account.Fields.ipg_ParentAccound, Intake.Account.Fields.ipg_manufactureraccountnumber));
        }

        private string GetAccountNumberFromFacilityMFGRelationShip(IOrganizationService service, SalesOrder po)
        {
            var crmContext = new OrganizationServiceContext(service);

            var mfg = GetManufacturer(service, po.ipg_Manufacturer_id);

            var number = (from facilityManufacturerRelationShip in crmContext.CreateQuery<ipg_facilitymanufacturerrelationship>()
                          where facilityManufacturerRelationShip.ipg_ManufacturerId.Id == po.ipg_Manufacturer_id.Id
                          && facilityManufacturerRelationShip.ipg_FacilityId.Id == po.ipg_FacilityId.Id
                          select facilityManufacturerRelationShip.ipg_ManufacturerAccountNumber).FirstOrDefault();

            while (mfg?.ipg_ParentAccound != null)
            {
                mfg = GetManufacturer(service, mfg.ipg_ParentAccound);
            }

            if (mfg.ipg_ManufacturerIsFacilityAcctRequired != true)
            {
                number = NOT_REQUIRED;
            }

            return number;
        }

        private void CreateSalesOrderDetailsForPurchaseOrder(IOrganizationService service, SalesOrder purchaseOrder, IReadOnlyList<Entity> casePartDetails)
        {
            bool isEstimated = (purchaseOrder.ipg_isestimatedpo != null && purchaseOrder.ipg_isestimatedpo == true);
            foreach (var detail in casePartDetails)
            {
                if (detail.GetAttributeValue<decimal?>(ipg_casepartdetail.Fields.ipg_quantity) > 0)
                {
                    var orderDetail = new SalesOrderDetail()
                    {
                        SalesOrderId = purchaseOrder.ToEntityReference(),
                        ProductId = detail.GetAttributeValue<EntityReference>(ipg_casepartdetail.Fields.ipg_productid),
                        Quantity = detail.GetAttributeValue<decimal?>(ipg_casepartdetail.Fields.ipg_quantity),
                        PricePerUnit = detail.GetAttributeValue<Money>(ipg_casepartdetail.Fields.ipg_enteredunitcost),
                        UoMId = detail.GetAttributeValue<EntityReference>(ipg_casepartdetail.Fields.ipg_uomid),
                        ipg_caseid = purchaseOrder.ipg_CaseId,
                        ipg_enteredshipping = detail.GetAttributeValue<Money>(ipg_casepartdetail.Fields.ipg_enteredshipping),
                        ipg_enteredunitcost = detail.GetAttributeValue<Money>(ipg_casepartdetail.Fields.ipg_enteredunitcost),
                        ipg_poextcost = detail.GetAttributeValue<Money>(ipg_casepartdetail.Fields.ipg_extcost),
                        ipg_pounitcost = detail.GetAttributeValue<Money>(ipg_casepartdetail.Fields.ipg_truecost),
                        ipg_lotnumber = detail.GetAttributeValue<string>(ipg_casepartdetail.Fields.ipg_lotnumber),
                        ipg_serialnumber = detail.GetAttributeValue<string>(ipg_casepartdetail.Fields.ipg_serialnumber),
                        Tax = detail.GetAttributeValue<Money>(ipg_casepartdetail.Fields.ipg_enteredtax)
                    };
                    orderDetail.Id = service.Create(orderDetail);
                    if (isEstimated)
                    {
                        var estimatedPart = new ipg_estimatedcasepartdetail()
                        {
                            Id = detail.Id,
                            ipg_salesorder_ipg_estimatedcasepartdetail_purchaseorderid = purchaseOrder
                        };

                        service.Update(estimatedPart);
                    }
                    else
                    {
                        var casePart = new ipg_casepartdetail()
                        {
                            Id = detail.Id,
                            ipg_SalesOrderDetail = orderDetail.ToEntityReference(),
                            ipg_PurchaseOrderId = purchaseOrder.ToEntityReference(),
                            ipg_IsChanged = false,
                            ipg_islocked = true                           
                        };

                        service.Update(casePart);
                    }
                }
            }
        }

        private string GetPOName(IOrganizationService service, ITracingService tracing, string poType, Incident incident)
        {
            tracing.Trace($"Run {nameof(GetPOName)}");

            Random rnd = new Random();
            string POName = poType.Substring(0, 1) + "IPG" + incident.Title + "-" + rnd.Next(100000, 999999).ToString();
            while (!POUnique(service, POName))
                POName = poType.Substring(0, 1) + "IPG" + incident.Title + "-" + rnd.Next(100000, 999999).ToString();

            tracing.Trace($"DOne with  {nameof(GetPOName)}, PO Name: {POName}");

            return POName;
        }
        private string GetPOName(string previousPOName, ITracingService tracing)
        {
            tracing.Trace($"Run {nameof(GetPOName)}");

            Random rnd = new Random();
            var versRegEx = Regex.Match(previousPOName, @"(?<=\.)\d+$");
            var POVersionstr = versRegEx.Success ? versRegEx.Value : null;
            var poVersion = string.IsNullOrEmpty(POVersionstr) ? 0 : int.Parse(POVersionstr);
            poVersion++;

            var poNameRegEx = Regex.Match(previousPOName, @".+(?=\.\d+$)|^.+");
            string POName = poNameRegEx.Success ? poNameRegEx.Value : "";

            tracing.Trace($"DOne with  {nameof(GetPOName)}, PO Name: {POName}");

            return $"{POName}.{poVersion}";
        }

        private bool POUnique(IOrganizationService service, string poName)
        {
            var queryExpression = new QueryExpression(SalesOrder.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("name"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(SalesOrder.Name).ToLower(), ConditionOperator.Equal, poName)
                        }
                }
            };
            EntityCollection POs = service.RetrieveMultiple(queryExpression);
            return (POs.Entities.Count == 0);

        }

        private IReadOnlyList<ipg_casepartdetail> GetCasePartDetails(IOrganizationService crmService, EntityReference incidentRef, EntityReference previousPORef, EntityReference manufacturerRef, ipg_PurchaseOrderTypes pOTypeCode)
        {
            var query = new QueryExpression(ipg_casepartdetail.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_casepartdetail.PrimaryIdAttribute
                , ipg_casepartdetail.Fields.ipg_productid
                , ipg_casepartdetail.Fields.ipg_extcost
                , ipg_casepartdetail.Fields.ipg_truecost
                , ipg_casepartdetail.Fields.ipg_quantity
                , ipg_casepartdetail.Fields.ipg_enteredunitcost
                , ipg_casepartdetail.Fields.ipg_serialnumber
                , ipg_casepartdetail.Fields.ipg_lotnumber
                , ipg_casepartdetail.Fields.ipg_caseid
                , ipg_casepartdetail.Fields.ipg_potypecode
                , ipg_casepartdetail.Fields.ipg_enteredshipping
                , ipg_casepartdetail.Fields.ipg_enteredtax
                , ipg_casepartdetail.Fields.ipg_uomid
                , ipg_casepartdetail.Fields.ipg_manufacturerid
                , ipg_casepartdetail.Fields.ipg_parttotal
                , ipg_casepartdetail.Fields.StateCode
                , ipg_casepartdetail.Fields.ipg_PurchaseOrderId
                , ipg_casepartdetail.Fields.StatusCode),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_casepartdetail.Fields.ipg_caseid, ConditionOperator.Equal, incidentRef.Id)
                        , new ConditionExpression(ipg_casepartdetail.Fields.ipg_potypecode, ConditionOperator.Equal, (int)pOTypeCode)
                        , new ConditionExpression(ipg_casepartdetail.Fields.ipg_caseid, ConditionOperator.Equal, incidentRef.Id)
                        , new ConditionExpression(ipg_casepartdetail.Fields.StateCode, ConditionOperator.Equal, (int) ipg_casepartdetailState.Active)
                        , new ConditionExpression(ipg_casepartdetail.Fields.ipg_productid,  Intake.Product.Fields.StateCode, ConditionOperator.Equal, (int)ProductState.Active)
                        , new ConditionExpression(ipg_casepartdetail.Fields.ipg_IsChanged, ConditionOperator.Equal, true)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(ipg_casepartdetail.EntityLogicalName,Intake.Product.EntityLogicalName, ipg_casepartdetail.Fields.ipg_productid, Intake.Product.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        EntityAlias = ipg_casepartdetail.Fields.ipg_productid,
                        Columns = new ColumnSet(Intake.Product.PrimaryIdAttribute, Intake.Product.Fields.ipg_manufacturerid, Intake.Product.Fields.ipg_status, Intake.Product.Fields.StateCode)
                    },
                    new LinkEntity(ipg_casepartdetail.EntityLogicalName,SalesOrder.EntityLogicalName, ipg_casepartdetail.Fields.ipg_PurchaseOrderId, SalesOrder.PrimaryIdAttribute, JoinOperator.LeftOuter)
                    {
                        EntityAlias = ipg_casepartdetail.Fields.ipg_PurchaseOrderId,
                        Columns = new ColumnSet(SalesOrder.PrimaryIdAttribute, SalesOrder.Fields.ipg_potypecode,  SalesOrder.Fields.StatusCode, SalesOrder.PrimaryNameAttribute, SalesOrder.Fields.ipg_documentid)
                    }
                }
            };

            var parts = crmService.RetrieveMultiple(query).Entities.Select(p =>
            {
                var convertedPart = p.ToEntity<ipg_casepartdetail>();
                convertedPart.ipg_product_ipg_casepartdetail_productid = new Intake.Product()
                {
                    Id = (Guid)p.GetAttributeValue<AliasedValue>($"{ipg_casepartdetail.Fields.ipg_productid}.{Intake.Product.PrimaryIdAttribute}").Value,
                    ipg_status = (OptionSetValue)p.GetAttributeValue<AliasedValue>($"{ipg_casepartdetail.Fields.ipg_productid}.{Intake.Product.Fields.ipg_status}")?.Value,
                    StateCode = (ProductState?)((OptionSetValue)p.GetAttributeValue<AliasedValue>($"{ipg_casepartdetail.Fields.ipg_productid}.{Intake.Product.Fields.StateCode}")?.Value)?.Value,
                    ipg_manufacturerid = (EntityReference)p.GetAttributeValue<AliasedValue>($"{ipg_casepartdetail.Fields.ipg_productid}.{Intake.Product.Fields.ipg_manufacturerid}")?.Value,
                };
                var poId = (Guid?)p.GetAttributeValue<AliasedValue>($"{ipg_casepartdetail.Fields.ipg_PurchaseOrderId}.{SalesOrder.PrimaryIdAttribute}")?.Value;
                convertedPart.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId = poId.HasValue ? new SalesOrder()
                {
                    Id = poId.Value,
                    ipg_potypecode = (OptionSetValue)p.GetAttributeValue<AliasedValue>($"{ipg_casepartdetail.Fields.ipg_PurchaseOrderId}.{SalesOrder.Fields.ipg_potypecode}")?.Value,
                    StateCode = (SalesOrderState?)((OptionSetValue)p.GetAttributeValue<AliasedValue>($"{ipg_casepartdetail.Fields.ipg_PurchaseOrderId}.{SalesOrder.Fields.StateCode}")?.Value)?.Value,
                    Name = (string)p.GetAttributeValue<AliasedValue>($"{ipg_casepartdetail.Fields.ipg_PurchaseOrderId}.{SalesOrder.PrimaryNameAttribute}")?.Value,
                    ipg_documentid = (EntityReference)p.GetAttributeValue<AliasedValue>($"{ipg_casepartdetail.Fields.ipg_PurchaseOrderId}.{SalesOrder.Fields.ipg_documentid}")?.Value,
                }
                : null;

                return convertedPart;
            });
            if (previousPORef != null)
            {
                if (previousPORef.Id != Guid.Empty)
                {
                    parts = parts.Where(part => part.ipg_PurchaseOrderId?.Id == previousPORef.Id);
                }
                else
                {
                    parts = parts.Where(part => part.ipg_PurchaseOrderId == null);
                }
            }
            //If PO not TPO filter approved parts
            if (pOTypeCode != ipg_PurchaseOrderTypes.TPO)
            {
                parts = parts.Where(data => data.ipg_product_ipg_casepartdetail_productid.ipg_statusEnum == Product_ipg_status.Approved);
            }


            if (manufacturerRef != null)
            {
                parts = parts.Where(data => data.ipg_product_ipg_casepartdetail_productid.ipg_manufacturerid.Id == manufacturerRef.Id);
            }

            return parts.ToList();
        }

        /// <summary>
        /// Generate PO based on Type
        /// </summary>
        /// <param name="service">CRM Service</param>
        /// <param name="crmContext">CRM Context</param>
        /// <param name="casePartDetails">Case Part Details</param>
        /// <param name="incident">Incident</param>
        /// <param name="pOTypeCode">Purchase Order Type</param>
        private SalesOrder GeneratePurchaseOrder(
            IOrganizationService service, CrmServiceContext crmContext, ITracingService tracing, Incident incident,
            ipg_PurchaseOrderTypes poTypeCode, EntityReference previousPORef, EntityReference manufacturerRef, bool estimatedPO,
            bool communicatePo = true)
        {
            SalesOrder po = null;

            tracing.Trace($"Retrieve {(estimatedPO ? "Estimated" : "Actual")} Case Parts ");

            IReadOnlyList<Entity> casePartDetails;
            List<SalesOrder> oldPOs = new List<SalesOrder>();
            if (estimatedPO)
            {
                casePartDetails = GetEstimatedCasePartDetails(crmContext, incident.ToEntityReference(), manufacturerRef, poTypeCode);
            }
            else
            {
                var actualParts = GetCasePartDetails(service, incident.ToEntityReference(), previousPORef, manufacturerRef, poTypeCode);
                casePartDetails = actualParts;
                oldPOs = actualParts.Select(p => p.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId).Where(order => order?.Id != null).GroupBy(o => o.Id).Select(gr => gr.First()).ToList();

                foreach (var order in oldPOs.Where(order => order.StatusCodeEnum != SalesOrder_StatusCode.Voided))
                {
                    service.Update(new SalesOrder() { Id = order.Id, StateCode = SalesOrderState.Fulfilled, StatusCodeEnum = SalesOrder_StatusCode.Voided });

                    if (order.ipg_documentid != null)
                    {
                        service.Update(new ipg_document() { Id = order.ipg_documentid.Id, StateCode = ipg_documentState.Inactive, StatusCodeEnum = ipg_document_StatusCode.Inactive });
                    }
                }
            }

            tracing.Trace($"{casePartDetails.Count} Found");

            if (casePartDetails.Any(part => part.GetAttributeValue<decimal?>(ipg_casepartdetail.Fields.ipg_quantity) > 0))
            {
                tracing.Trace("Build PO");
                //TODO create new field for Part Total on Actual PArt with type Money instead of decimal 
                var poTotal = estimatedPO ? casePartDetails.Sum(p => p.GetAttributeValue<Money>(ipg_casepartdetail.Fields.ipg_parttotal)?.Value ?? 0) 
                    : casePartDetails.Sum(p => p.GetAttributeValue<Money>(ipg_casepartdetail.Fields.ipg_extcost)?.Value ?? 0);
                var previousPO = oldPOs.Where(o => o.ipg_potypecodeEnum == poTypeCode).FirstOrDefault();

                tracing.Trace($"Old PO {previousPO?.Name}");
                po = GetPO(service, crmContext, tracing, incident, poTypeCode, manufacturerRef, estimatedPO, previousPO);

                if (po != null)
                {
                    tracing.Trace($"{nameof(CreateSalesOrderDetailsForPurchaseOrder)}");

                    CreateSalesOrderDetailsForPurchaseOrder(service, po, casePartDetails);
                    service.Update(new SalesOrder() { Id = po.Id, TotalAmount = new Money(poTotal) });
                }
            }
            else if (casePartDetails.Any())
            {
                return null;
            }
            else
            {
                throw new Exception($@"Please Check that Case have Actual Parts{(poTypeCode != ipg_PurchaseOrderTypes.TPO ? " and Parts(products) approved" : "") }!");
            }

            return po;
        }

        private IReadOnlyList<Entity> GetEstimatedCasePartDetails(CrmServiceContext crmContext, EntityReference entityReference, EntityReference manufacturerRef, ipg_PurchaseOrderTypes poTypeCode)
        {
            IReadOnlyList<ipg_estimatedcasepartdetail> EstimatedCasePartDetails;

            var parts = (from part in crmContext.CreateQuery<ipg_estimatedcasepartdetail>()
                         join product in crmContext.CreateQuery<Intake.Product>()
                          on part["ipg_productid"] equals product["productid"]
                         where part.ipg_caseid.Id == entityReference.Id
                         && part.ipg_purchaseorderid == null
                         && part.ipg_potypecode.Value == (int)poTypeCode
                         && part.StateCode.Value == ipg_estimatedcasepartdetailState.Active
                         && product.StateCode == ProductState.Active
                         && part.ipg_quantity.Value > 0
                         select new
                         {
                             actualPart = new ipg_estimatedcasepartdetail()
                             {
                                 Id = part.ipg_estimatedcasepartdetailId.Value,
                                 ipg_quantity = part.ipg_quantity,
                                 ipg_caseid = part.ipg_caseid,
                                 ipg_potypecode = part.ipg_potypecode,
                                 ipg_productid = part.ipg_productid,
                                 ipg_uomid = part.ipg_uomid
                             },
                             part = new Intake.Product()
                             {
                                 Id = product.ProductId.Value,
                                 ipg_status = product.ipg_status,
                                 ipg_manufacturerid = product.ipg_manufacturerid,
                             }
                         }).ToList();


            if (manufacturerRef != null)
            {
                parts = parts.Where(data => data.part.ipg_manufacturerid.Id == manufacturerRef.Id).ToList();
            }

            EstimatedCasePartDetails = parts.Select(data => data.actualPart).ToList();

            return EstimatedCasePartDetails;
        }

        private void AfterGenerationPO(
            SalesOrder poEnt, Incident caseEnt, EntityReference fromUser,
            IOrganizationService service, ITracingService tracingService, IPluginExecutionContext context, string email = null, bool communicatePo = true)
        {
            ActivityMimeAttachment pdfdoc = CreatePDF(poEnt, service, tracingService);

            if (communicatePo)
            {
                Communicate(poEnt, caseEnt, fromUser, service, tracingService, context, email, pdfdoc);
            }

            string ToEnt = (poEnt.ipg_potypecodeEnum == ipg_PurchaseOrderTypes.TPO
                            || poEnt.ipg_potypecodeEnum == ipg_PurchaseOrderTypes.ZPO
                            || poEnt.ipg_potypecodeEnum == ipg_PurchaseOrderTypes.CPA) ? "Facility"
                : poEnt.ipg_potypecodeEnum == ipg_PurchaseOrderTypes.MPO ? "Manufacturer" : null;

            service.Create(GetNote(poEnt, fromUser, ToEnt, poEnt.ToEntityReference()));

            service.Create(GetNote(poEnt, fromUser, ToEnt, caseEnt.ToEntityReference()));
        }

        private ActivityMimeAttachment CreatePDF(SalesOrder poEnt, IOrganizationService service, ITracingService tracing)
        {
            tracing.Trace($"Start {nameof(CreatePDF)}");

            var crmContext = new OrganizationServiceContext(service);
            var url = (from gc in crmContext.CreateQuery<ipg_globalsetting>()
                       where gc.ipg_name == GENERATEPDF
                       select gc.ipg_value).FirstOrDefault();

            tracing.Trace($"FOUND function app URL {url}");

            if (!string.IsNullOrEmpty(url))
            {
                var requestBody = new GeneratePDFFromDocTemplateRequest()
                {
                    Target = poEnt.ToEntityReference(),
                    TemplateName = PDFTEMPLATE
                };

                var serializer = new DataContractJsonSerializer(requestBody.GetType());

                var ms = new MemoryStream();
                serializer.WriteObject(ms, requestBody);
                var content = ms.ToArray();
                ms.Close();


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json; encoding='utf-8'";
                request.Method = "POST";
                request.ContentLength = content.Length;
                request.KeepAlive = false;


                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(content, 0, content.Length);
                }

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    tracing.Trace($"Get Response StatusCode {response.StatusCode}");

                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                var respString = reader.ReadToEnd();
                                JavaScriptSerializer jss = new JavaScriptSerializer();
                                var data = jss.Deserialize<dynamic>(respString);

                                if ((bool?)data["Success"] == true && !string.IsNullOrWhiteSpace((string)data["Body"]))
                                {
                                    tracing.Trace("pdf file has been generated");

                                    return new ActivityMimeAttachment()
                                    {
                                        MimeType = "application/pdf",
                                        Body = (string)data["Body"],
                                        FileName = $"{poEnt.Name}.pdf"
                                    };
                                }
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    tracing.Trace($"There is error while generation PDF for PO {poEnt.Id}");
                    tracing.Trace($"Error: {error.Message}");
                    tracing.Trace("pdf file has not been generated");
                }

                return null;
            }

            return null;
        }

        private bool Communicate(SalesOrder salesOrder, Incident incident, EntityReference fromUser,
            IOrganizationService service, ITracingService tracing, IPluginExecutionContext context, string email = null, ActivityMimeAttachment pdfDoc = null)
        {
            Tuple<SalesOrder_StatusCode?, ipg_POCommStatus?> poStatus = null;

            switch (salesOrder.ipg_potypecodeEnum)
            {
                case ipg_PurchaseOrderTypes.TPO:
                case ipg_PurchaseOrderTypes.ZPO:
                    CreatePortalMessage(salesOrder, incident, fromUser, service);
                    poStatus = new Tuple<SalesOrder_StatusCode?, ipg_POCommStatus?>(SalesOrder_StatusCode.CommtoFacility, ipg_POCommStatus.AlreadyCommunicated);
                    salesOrder.StatusCode = SalesOrder_StatusCode.CommtoFacility.ToOptionSetValue();
                    break;
                case ipg_PurchaseOrderTypes.CPA:
                    CreatePortalMessage(salesOrder, incident, fromUser, service);
                    poStatus = new Tuple<SalesOrder_StatusCode?, ipg_POCommStatus?>(SalesOrder_StatusCode.VerifiedforPayment, ipg_POCommStatus.Complete);
                    break;
                case ipg_PurchaseOrderTypes.MPO:
                    if (!string.IsNullOrEmpty(email))
                    {
                        var attachment = pdfDoc ?? GetPDFFileFromPO(salesOrder, service, tracing);
                        var emails = email.Split(';').Select(s => s.Trim());

                        foreach (var e in emails)
                        {
                            if (!string.IsNullOrEmpty(e) && new EmailAddressAttribute().IsValid(e))
                            {
                                CommunicateByEmail(salesOrder, incident, fromUser, service, tracing, e, attachment);
                            }
                        }

                        poStatus = new Tuple<SalesOrder_StatusCode?, ipg_POCommStatus?>(SalesOrder_StatusCode.CommtoMFG, ipg_POCommStatus.AlreadyCommunicated);
                    }
                    break;
                default:
                    break;
            }

            if (poStatus != null)
            {
                service.Update(new SalesOrder() { Id = salesOrder.Id, StatusCodeEnum = poStatus.Item1, ipg_commstatusEnum = poStatus.Item2 });
            }

            ProcessE8LogCreation(incident, salesOrder, service, context);

            return poStatus != null;
        }

        private void ProcessE8LogCreation(Incident caseEntity, SalesOrder salesOrderEntity, IOrganizationService service, IPluginExecutionContext context)
        {
            var importantEventManager = new ImportantEventManager(service);
            if (salesOrderEntity?.StatusCodeEnum == SalesOrder_StatusCode.CommtoFacility
                || salesOrderEntity?.StatusCodeEnum == SalesOrder_StatusCode.CommtoMFG)
            {
                string source = string.Empty;
                switch (salesOrderEntity.ipg_potypecodeEnum)
                {
                    case ipg_PurchaseOrderTypes.CPA:
                    case ipg_PurchaseOrderTypes.TPO:
                    case ipg_PurchaseOrderTypes.ZPO:
                        source = "Facility";
                        break;
                    case ipg_PurchaseOrderTypes.MPO:
                        source = "Manufacturer";
                        break;
                    default:
                        break;
                }
                importantEventManager.CreateImportantEventLog(caseEntity, context.UserId, Constants.EventIds.ET8, new[] { salesOrderEntity.Name, source });
                importantEventManager.SetCaseOrReferralPortalHeader(caseEntity, Constants.EventIds.ET8);
            }
        }

        private void CommunicateByEmail(SalesOrder salesOrder, Incident incident, EntityReference fromUser, IOrganizationService service, ITracingService tracing, string email, ActivityMimeAttachment attachment)
        {
            var queue = service.RetrieveMultiple(new QueryExpression(Queue.EntityLogicalName)
            {
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Queue.Fields.Name, ConditionOperator.Equal, QUEUEForEmailComunication)
                    }
                }
            }).Entities.FirstOrDefault()?.ToEntity<Queue>();

            var emailtemplate = service.RetrieveMultiple(new QueryExpression(Template.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Template.Fields.SubjectSafeHtml, Template.Fields.Body),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Template.Fields.Title, ConditionOperator.Equal, EMAILTEMPLATE)
                    }
                }
            }).Entities.FirstOrDefault()?.ToEntity<Template>();

            SystemUser cm = null;
            if (incident.ipg_CaseManagerId != null)
            {
                cm = service.Retrieve(incident.ipg_CaseManagerId.LogicalName, incident.ipg_CaseManagerId.Id, new ColumnSet(SystemUser.Fields.InternalEMailAddress)).ToEntity<SystemUser>();
            }

            if (emailtemplate == null)
            {
                throw new InvalidPluginExecutionException($"Email template with name {EMAILTEMPLATE}  does not exist!");
            }

            if (queue == null)
            {
                throw new InvalidPluginExecutionException($"Queue with name {QUEUEForEmailComunication}  does not exist!");
            }

            InstantiateTemplateResponse templateresponse = service.Execute(new InstantiateTemplateRequest()
            {
                ObjectId = salesOrder.Id,
                ObjectType = salesOrder.LogicalName,
                TemplateId = emailtemplate.Id
            }) as InstantiateTemplateResponse;

            var emailEnt = templateresponse.EntityCollection[0].ToEntity<Email>();
            emailEnt.From = new List<ActivityParty>() { new ActivityParty() { PartyId = queue?.ToEntityReference(), } };
            emailEnt.To = new List<ActivityParty>() { new ActivityParty() { AddressUsed = email } };
            emailEnt.RegardingObjectId = salesOrder.ToEntityReference();
            emailEnt.Description = emailEnt.Description.Replace("{CM email address}", cm?.InternalEMailAddress);
            emailEnt.Id = service.Create(emailEnt);

            if (attachment != null)
            {
                tracing.Trace("File will be attached to email");

                attachment.ObjectId = emailEnt.ToEntityReference();
                attachment.ObjectTypeCode = emailEnt.LogicalName;
                service.Create(attachment);
            }

            SendEmailRequest sendEmailreq = new SendEmailRequest
            {
                EmailId = emailEnt.Id,
                TrackingToken = "",
                IssueSend = true
            };

            SendEmailResponse sendEmailresp = (SendEmailResponse)service.Execute(sendEmailreq);
        }

        private Tuple<string, string> GetSubjectDescription(SalesOrder poEnt, Incident incident)
        {
            return new Tuple<string, string>($@"{poEnt.ipg_potypecodeEnum} Generated", $@"PO Number: {poEnt.Name}; Case Number: {incident.Title}.");
        }

        private void CreatePortalMessage(SalesOrder poEnt, Incident caseEnt, EntityReference fromUser, IOrganizationService service)
        {
            Tuple<string, string> info = GetSubjectDescription(poEnt, caseEnt);

            adx_portalcomment portalcomment = new adx_portalcomment()
            {
                RegardingObjectId = caseEnt.ToEntityReference(),
                ipg_POID = poEnt.ToEntityReference(),
                From = new List<ActivityParty>() { new ActivityParty() { PartyId = fromUser } },
                Subject = info.Item1,
                Description = info.Item2,
                adx_PortalCommentDirectionCodeEnum = adx_portalcomment_adx_PortalCommentDirectionCode.Outgoing

            };

            service.Create(portalcomment);
        }

        private Annotation GetNote(SalesOrder poEnt, EntityReference fromUser, string to, EntityReference objId)
        {
            return new Annotation()
            {
                NoteText = $@"PO {poEnt.Name} has been generated{(!string.IsNullOrEmpty(to) ? (" to " + to) : "")} by {fromUser.Name}",
                ObjectId = objId
            };
        }

        private void PostOperationHandlerManualCommunicatePO(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            SystemUser user = service.Retrieve(SystemUser.EntityLogicalName, context.InitiatingUserId, new ColumnSet("fullname")).ToEntity<SystemUser>();
            EntityReference userRef = user.ToEntityReference();
            userRef.Name = user.FullName;

            var orderId = context.InputParameters["OrderId"].ToString();

            if (orderId == null)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, "Order id can not be null.");
            }

            var po = service.Retrieve(SalesOrder.EntityLogicalName, new Guid(orderId),
                new ColumnSet(SalesOrder.Fields.ipg_potypecode, SalesOrder.Fields.ipg_CaseId, SalesOrder.Fields.ipg_Manufacturer_id, SalesOrder.Fields.ipg_documentid)).ToEntity<SalesOrder>();

            if (po.ipg_potypecode == null)
            {
                context.OutputParameters["Message"] = "PO Type is not specified.";
                return;
            }

            if (po.ipg_CaseId == null)
            {
                context.OutputParameters["Message"] = "Purchase Order has no related case.";
                return;
            }

            var incident = service.Retrieve(Incident.EntityLogicalName, po.ipg_CaseId.Id,
                new ColumnSet(Incident.Fields.ipg_CaseManagerId)).ToEntity<Incident>();

            var communicateTo = string.Empty;

            if (po.ipg_potypecode.Value == (int)ipg_PurchaseOrderTypes.MPO)
            {
                if (po.ipg_Manufacturer_id == null)
                {
                    context.OutputParameters["Message"] = "In order to communicate MPO Manufacturer is required.";
                    return;
                }

                if (incident.ipg_CaseManagerId == null)
                {
                    context.OutputParameters["Message"] = "In order to communicate MPO Case Manager is required.";
                    return;
                }

                communicateTo = service.Retrieve(Intake.Account.EntityLogicalName, po.ipg_Manufacturer_id.Id,
                new ColumnSet(Intake.Account.Fields.ipg_Ccmmunicateposto))?.ToEntity<Intake.Account>()?.ipg_Ccmmunicateposto;

                if (string.IsNullOrEmpty(communicateTo))
                {
                    context.OutputParameters["Message"] = $"Manufacturer email ({nameof(Intake.Account.Fields.ipg_Ccmmunicateposto)}) is required to communicate MPO.";
                    return;
                }
            }

            var isCommunicated = Communicate(po, incident, userRef, service, tracingService, localPluginContext.PluginExecutionContext, communicateTo);

            if (!isCommunicated)
            {
                context.OutputParameters["Message"] = "Failed to communicate PO.";
            }
            else
            {
                context.OutputParameters["Success"] = true;
                context.OutputParameters["Message"] = "PO communicated successfully.";
            }
        }

        private ActivityMimeAttachment GetPDFFileFromPO(SalesOrder salesOrder, IOrganizationService service, ITracingService tracing)
        {
            if (salesOrder.ipg_documentid != null)
            {
                var note = service.RetrieveMultiple(new QueryExpression(Annotation.EntityLogicalName)
                {
                    TopCount = 1
                    ,
                    ColumnSet = new ColumnSet(
                    Annotation.Fields.MimeType,
                    Annotation.Fields.Subject,
                    Annotation.Fields.DocumentBody,
                    Annotation.Fields.FileName),
                    Criteria = new FilterExpression()
                    {
                        Conditions = {
                        new ConditionExpression(Annotation.Fields.ObjectId, ConditionOperator.Equal, salesOrder.ipg_documentid.Id),
                    }
                    }
                }).Entities.FirstOrDefault()?.ToEntity<Annotation>();

                if (note != null)
                {
                    return new ActivityMimeAttachment()
                    {
                        MimeType = note.MimeType,
                        Subject = note.Subject,
                        Body = note.DocumentBody,
                        FileName = note.FileName
                    };
                }
            }
            return null;
        }
    }
}