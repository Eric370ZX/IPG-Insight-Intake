using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Linq;
using Insight.Intake.Helpers;

namespace Insight.Intake.Plugin.Workflow
{
    public class CopyDocumentsPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService) serviceProvider.GetService(typeof(ITracingService));

            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);

                if (context.MessageName == Constants.ActionNames.IntakeActionsCopyDocuments)
                {
                    if (context.InputParameters.Contains("SourceReference") && context.InputParameters.Contains("TargetReference"))
                    {
                        var source = (EntityReference)context.InputParameters["SourceReference"];

                        var target = (EntityReference)context.InputParameters["TargetReference"];

                        var documentsQuery = new QueryExpression
                        {
                            EntityName = ipg_document.EntityLogicalName,
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression(LogicalOperator.And),
                            PageInfo = new PagingInfo
                            {
                                ReturnTotalRecordCount = true
                            },
                        };
                        
                        documentsQuery.Criteria.AddCondition("ipg_referralid", ConditionOperator.Equal, source.Id);

                        documentsQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Null);

                        var documents = organizationService.RetrieveMultiple(documentsQuery);

                        if (documents.Entities.Any())
                        {
                            foreach (var document in documents.Entities.Cast<ipg_document>())
                            {
                                var updatedDocument = new Entity(document.LogicalName, document.Id).ToEntity<ipg_document>();
                                updatedDocument.ipg_CaseId = new EntityReference(target.LogicalName, target.Id);
                                organizationService.Update(updatedDocument);
                            }
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw faultException;
                //throw new InvalidPluginExecutionException($"An error occurred in {nameof(CopyDocumentsPlugin)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace("{0}: {1}", nameof(CopyDocumentsPlugin), exception.ToString());
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(CopyDocumentsPlugin)}.", exception);
            }
        }
    }
}