using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Order
{
    public class SetPOIssuedOrPOPaidOnCase : PluginBase
    {
        public SetPOIssuedOrPOPaidOnCase() : base(typeof(SetPOIssuedOrPOPaidOnCase))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, SalesOrder.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, SalesOrder.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            if (context.InputParameters["Target"] is Entity targetOrder)
            {
                var caseId = new Guid();
                var order = targetOrder.ToEntity<SalesOrder>();
                var poIssueDate = new DateTime?();
                if (context.MessageName == MessageNames.Create && targetOrder.Contains(SalesOrder.Fields.ipg_CaseId))
                {
                    caseId = order.ipg_CaseId.Id;
                    poIssueDate = order.ipg_po_issue_date;
                }
                if (context.MessageName == MessageNames.Update)
                {
                    var postImage = context.PostEntityImages.Contains("PostImage") ? localPluginContext.PostImage<Entity>() : null;
                    if (postImage != null && postImage.Contains(SalesOrder.Fields.ipg_CaseId))
                    {
                        caseId = postImage.GetAttributeValue<EntityReference>(SalesOrder.Fields.ipg_CaseId).Id;
                        if (postImage.GetAttributeValue<DateTime?>(SalesOrder.Fields.ipg_po_issue_date) != null)
                        {
                            poIssueDate = postImage.GetAttributeValue<DateTime>(SalesOrder.Fields.ipg_po_issue_date);
                        }
                    }
                }
                if (caseId != Guid.Empty)
                {
                    var incidentEntity = service.Retrieve(Incident.EntityLogicalName, caseId,
                                                          new ColumnSet(Incident.Fields.StateCode, Incident.Fields.ipg_portalalerts));
                    if (incidentEntity != null)
                    {
                        var incident = incidentEntity.ToEntity<Incident>();
                        if (string.IsNullOrEmpty(incident.ipg_portalalerts) || incident.ipg_portalalerts != "Missing Information")
                        {
                            if (poIssueDate != null)
                            {
                                incident.ipg_portalalerts = "PO Issued";
                            }
                            else if (IsAnyPaidOrdersOnCase(service, caseId))
                            {
                                incident.ipg_portalalerts = "PO Paid";
                            }
                            else
                            {
                                incident.ipg_portalalerts = null;
                            }
                            service.Update(incident);
                        }
                    }
                }
            }
        }



        private bool IsAnyPaidOrdersOnCase(IOrganizationService service, Guid caseId)
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='{Incident.EntityLogicalName}'>
                                <attribute name='{Incident.Fields.IncidentId}' />
                                <filter type='and'>
                                  <condition attribute='{Incident.Fields.IncidentId}' operator='eq' value='{caseId}' />
                                </filter>
                                <link-entity name='{SalesOrder.EntityLogicalName}' from='{SalesOrder.Fields.ipg_CaseId}' to='{Incident.Fields.Id}' link-type='inner' alias='{SalesOrder.EntityLogicalName}'>
                                  <link-entity name='{Invoice.EntityLogicalName}' from='{Invoice.Fields.SalesOrderId}' to='{SalesOrder.Fields.Id}' link-type='inner' alias='{Invoice.EntityLogicalName}'>
                                    <filter type='and'>
                                      <condition attribute='{Invoice.Fields.StateCode}' operator='eq' value='{(int)InvoiceState.Paid}' />
                                    </filter>
                                  </link-entity>
                                </link-entity>
                              </entity>
                            </fetch>";
            return service.RetrieveMultiple(new FetchExpression(fetch)).Entities.Any();
        }
    }
}

