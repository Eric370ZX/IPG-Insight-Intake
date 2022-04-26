using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;

namespace Insight.Intake.Plugin.Document
{
    public class RetrieveDocumentsForDynamicView : PluginBase
    {
        public RetrieveDocumentsForDynamicView() : base(typeof(RetrieveDocumentsForDynamicView))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.RetrieveMultiple, ipg_document.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            try
            {
                QueryExpression currentQuery = context.InputParameters["Query"] as QueryExpression;
                if (currentQuery != null)
                {
                    Dictionary<string, object> requestParameters = currentQuery.Criteria.Conditions.Where(e => e.Operator == ConditionOperator.Equal).
                    ToDictionary(e => e.AttributeName, e => e.Values.First());

                    var orderParameters = string.Join("\n", currentQuery.Orders.Select(x => $"<order attribute='{x.AttributeName}' descending='{(x.OrderType == OrderType.Descending ? true : false).ToString().ToLower()}' />")); ;
                    
                    if (requestParameters.ContainsKey("ipg_linkedentitiesfilter"))
                    {
                        var objectConditions = PluginExtensions.deserializeObject<ObjectConditions>((string)requestParameters["ipg_linkedentitiesfilter"]);
                        SetNewQueryForInvoces(context, objectConditions, orderParameters);
                    }
                }

                FetchExpression fetchXml = context.InputParameters["Query"] as FetchExpression;
                if (fetchXml != null)
                {
                    XDocument parsedQuery = XDocument.Parse(fetchXml.Query);

                    Dictionary<string, string> requestParameters = parsedQuery
                    .Descendants("condition")
                    .Where(e =>
                    e.Attribute("attribute") != null &&
                    e.Attribute("operator") != null &&
                    e.Attribute("value") != null &&
                    string.Equals(e.Attribute("operator").Value, "eq", StringComparison.InvariantCultureIgnoreCase))
                    .GroupBy(e => e.Attribute("attribute").Value)
                    .Select(g => g.First())
                    .ToDictionary(e => e.Attribute("attribute").Value, e => e.Attribute("value").Value);

                    var orderParameters = string.Join("\n", parsedQuery.Descendants("order").Select(x => $"<order attribute='{x.Attribute("attribute").Value}' descending='{x.Attribute("descending").Value}' />"));

                    if (requestParameters.ContainsKey("ipg_linkedentitiesfilter"))
                    {
                        var objectConditions = PluginExtensions.deserializeObject<ObjectConditions>(requestParameters["ipg_linkedentitiesfilter"]);
                        SetNewQueryForInvoces(context, objectConditions, orderParameters);
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {this.GetType().Name}.", faultException);
            }
            catch (Exception exception)
            {
                localPluginContext.Trace($"{this.GetType().Name}: {exception}");
                throw;
            }
            //throw new Exception("test");
        }

        private void SetNewQueryForInvoces(IPluginExecutionContext context, ObjectConditions objectConditions, string orderParams)
        {
            if (objectConditions != null && objectConditions.ViewType == "Invoices" && objectConditions.FacilityId != null && objectConditions.ProductId != null)
            {
                var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                              <entity name='ipg_document'>
                                                <attribute name='ipg_documentid' />
                                                <attribute name='ipg_name' />
                                                <attribute name='createdon' />
                                                <attribute name='ipg_source' />
                                                <attribute name='ipg_documenttypeid' />
                                                {orderParams}
                                                <filter type='and'>
                                                  <condition attribute='ipg_documenttypeidname' operator='like' value='%Invoice%' />
                                                  <condition attribute='createdon' operator='last-x-months' value='6' />
                                                </filter>
                                                <link-entity name='incident' from='incidentid' to='ipg_caseid' link-type='inner' alias='ae'>
                                                  <filter type='and'>
                                                    <condition attribute='ipg_facilityid' operator='eq' value='{objectConditions.FacilityId}' />
                                                  </filter>
                                                  <link-entity name='ipg_casepartdetail' from='ipg_caseid' to='incidentid' link-type='inner' alias='af'>
                                                    <filter type='and'>
                                                      <condition attribute='ipg_productid' operator='eq' value='{objectConditions.ProductId}' />
                                                    </filter>
                                                  </link-entity>
                                                </link-entity>
                                              </entity>
                                            </fetch>";

                context.InputParameters["Query"] = new FetchExpression(fetch);
            }
        }
    }

    public class ObjectConditions {
        public string ViewType { get; set; }
        public Guid FacilityId { get; set; }
        public Guid ProductId { get; set; }
    }
}
