using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using System.Xml.Linq;

namespace Insight.Intake.Plugin.ImportantEventsLog
{
    public class RetrieveMultipleImportantEventsLogFilter : PluginBase
    {
        public RetrieveMultipleImportantEventsLogFilter() : base(typeof(RetrieveMultipleImportantEventsLogFilter))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.RetrieveMultiple, ipg_importanteventslog.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            if (context.InputParameters.Contains("Query"))
            {
                if (context.InputParameters["Query"] is FetchExpression)
                {
                    var fetchExpressionQuery = context.InputParameters["Query"] as FetchExpression;
                    var queryVisitor = new Helpers.QueryVisitor();
                    var query = service.ConvertFetchXmlToQueryExpression(fetchExpressionQuery.Query);
                    query.Accept(queryVisitor);

                    if (queryVisitor.CaseId == null && queryVisitor.ReferralId == null)
                    {
                        return;
                    }

                    XDocument fetchXmlDoc = XDocument.Parse(fetchExpressionQuery.Query);
                    var entityElement = fetchXmlDoc.Descendants("entity").FirstOrDefault();
                    var linkEntityElement = fetchXmlDoc.Descendants("link-entity").FirstOrDefault();
                    if (linkEntityElement != null)
                    {
                        var linkEntityName = linkEntityElement.Attributes("name").FirstOrDefault().Value;
                        if (linkEntityName == ipg_importanteventconfig.EntityLogicalName)
                        {
                            var filterElements = entityElement.Descendants("filter");

                            var stateCodeConditions = from c in filterElements.Descendants("condition")
                                                      where c.Attribute("attribute").Value.Equals(ipg_importanteventconfig.Fields.ipg_visibility)
                                                      select c;

                            if (stateCodeConditions.Count() > 0)
                            {
                                stateCodeConditions.ToList().ForEach(x => x.Remove());
                            }
                        }
                    }

                    entityElement.Add(
                           new XElement("link-entity",
                               new XAttribute("name", ipg_importanteventconfig.EntityLogicalName),
                               new XAttribute("from", ipg_importanteventconfig.Fields.ipg_name),
                               new XAttribute("to", ipg_importanteventslog.Fields.ipg_configId),
                               new XAttribute("link-type", "inner"),
                           new XElement("filter",
                           new XElement("condition",
                               new XAttribute("attribute", ipg_importanteventconfig.Fields.ipg_visibility),
                               new XAttribute("operator", "eq"),
                               new XAttribute("value", 0)
                           ))));

                    fetchExpressionQuery.Query = fetchXmlDoc.ToString();
                }
            }  
        }
    }
}
