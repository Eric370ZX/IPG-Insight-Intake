using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class POChangesAction: PluginBase
    {
        //Obsolute, User will Generate PO throoug Generate PO Button
        public POChangesAction() : base(typeof(POChangesAction))
        {
            //RegisterEvent(PipelineStages.PostOperation, "ipg_IPGPOChangesAction", Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localContext)
        {
            var crmService = localContext.OrganizationService;
            var tracingService = localContext.TracingService;
            var incidentRef = localContext.TargetRef();

            var actualPartsGroups = GetActualParts(incidentRef, crmService, tracingService);
            
            actualPartsGroups.ForEach(order => ProcessOrder(order, incidentRef, crmService, tracingService));
        }

        private void ProcessOrder(IGrouping<Guid, ipg_casepartdetail> order, EntityReference incidentRef, IOrganizationService crmService, ITracingService tracingService)
        {
            var potype = ((OptionSetValue)order.First().GetAttributeValue<AliasedValue>($"{SalesOrder.EntityLogicalName}.{SalesOrder.Fields.ipg_potypecode}")?.Value)?.Value;

            if(order.Any(part => part.ipg_potypecode?.Value == ((OptionSetValue)part.GetAttributeValue<AliasedValue>($"{SalesOrder.EntityLogicalName}.{SalesOrder.Fields.ipg_potypecode}")?.Value)?.Value))
            {
                crmService.Update(new SalesOrder()
                {
                    Id = order.Key,
                    StateCode = SalesOrderState.Active,
                    StatusCodeEnum = SalesOrder_StatusCode.Revised
                });
            }
            else
            {
                var fullfillrequest = new FulfillSalesOrderRequest()
                {
                    OrderClose = new OrderClose
                    {
                        SalesOrderId = new EntityReference
                        { LogicalName = SalesOrder.EntityLogicalName, Id = order.Key }
                    },
                    Status = new OptionSetValue((int)SalesOrder_StatusCode.Voided)
                };

                crmService.Execute(fullfillrequest);
            }

            var newPOS = order
                .Where(r => r.ipg_potypecode?.Value != potype)
                .GroupBy(r => new {
            potype = r.ipg_potypecodeEnum
            ,mfg = (EntityReference)r.GetAttributeValue<AliasedValue>($"{Intake.Product.EntityLogicalName}{Intake.Product.Fields.ipg_manufacturerid}")?.Value});

            foreach(var newpo  in newPOS)
            {
                newpo.ToList().ForEach(r => crmService.Update(new ipg_casepartdetail() { Id = r.Id, ipg_PurchaseOrderId = null }));

                var GeneratePO = new ipg_IPGIntakeCaseActionsGeneratePORequest()
                {
                    Target = incidentRef,
                    POType = newpo.Key.potype?.ToString(),
                    manufacturer = newpo.Key.mfg
                };

                crmService.Execute(GeneratePO);
            }       
        }

        private List<IGrouping<Guid, ipg_casepartdetail>> GetActualParts(EntityReference incidentRef, IOrganizationService crmService, ITracingService tracingService)
        {
            var actualpartsGroups = crmService.RetrieveMultiple(new QueryExpression(ipg_casepartdetail.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_casepartdetail.Fields.ipg_PurchaseOrderId
                , ipg_casepartdetail.Fields.ipg_potypecode
                , ipg_casepartdetail.Fields.ipg_manufacturerid
                , ipg_casepartdetail.Fields.ipg_casepartdetailId),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_casepartdetail.Fields.ipg_caseid, ConditionOperator.Equal, incidentRef.Id)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(ipg_casepartdetail.EntityLogicalName, SalesOrder.EntityLogicalName, ipg_casepartdetail.Fields.ipg_PurchaseOrderId, SalesOrder.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        EntityAlias = SalesOrder.EntityLogicalName,
                        Columns = new ColumnSet(SalesOrder.Fields.ipg_potypecode, SalesOrder.PrimaryIdAttribute),
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions = {new ConditionExpression(SalesOrder.Fields.StateCode, ConditionOperator.Equal, (int)SalesOrderState.Active)}
                        }
                    },
                     new LinkEntity(ipg_casepartdetail.EntityLogicalName, Intake.Product.EntityLogicalName, ipg_casepartdetail.Fields.ipg_productid, Intake.Product.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        EntityAlias = Intake.Product.EntityLogicalName,
                        Columns = new ColumnSet(Intake.Product.Fields.ipg_manufacturerid)
                    }
                }
            }).Entities
            .Select(r => r.ToEntity<ipg_casepartdetail>())
            .GroupBy(r => (Guid)r.GetAttributeValue<AliasedValue>($"{SalesOrder.EntityLogicalName}.{SalesOrder.PrimaryIdAttribute}").Value)
            .Where(r => r.Any(part => part.ipg_potypecode?.Value != ((OptionSetValue)part.GetAttributeValue<AliasedValue>($"{SalesOrder.EntityLogicalName}.{SalesOrder.Fields.ipg_potypecode}")?.Value)?.Value));

            return actualpartsGroups.ToList();
        }
    }
}
