using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Workflow
{
    public class CloneRecordPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);

                if (context.MessageName == Constants.ActionNames.IntakeActionsCloneRecord)
                {
                    if (context.InputParameters.Contains("Record"))
                    {
                        var recordReference = (EntityReference)context.InputParameters["Record"];
                        var record = organizationService.Retrieve(recordReference.LogicalName, recordReference.Id, new ColumnSet(true));
                       
                        EntityReference newRecordRef = null;
                        if (record.LogicalName == ipg_document.EntityLogicalName) {
                            var cloneHelper = new CloneEntityHelper(organizationService);
                            newRecordRef = cloneHelper.CloneDocument(record);
                        }                      

                        context.OutputParameters["ClonedRecord"] = newRecordRef;
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(CloneRecordPlugin)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace("{0}: {1}", nameof(CloneRecordPlugin), exception.ToString());
                throw;
            }
        }
    }
}
