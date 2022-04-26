using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class ConfigureTask : PluginBase
    {
        private IOrganizationService service;
        public ConfigureTask() : base(typeof(ConfigureTask))
        {
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Create, Task.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Task.EntityLogicalName, SyncInformation);
        }

        private void PreOperationHandler(LocalPluginContext localContext)
        {
            var target = localContext.Target<Task>();
            service = localContext.OrganizationService;
            var taskManager = new TaskManager(service, localContext.TracingService, target.ToEntityReference(), localContext.PluginExecutionContext.InitiatingUserId);
            Invoice regardingClaim = null;
            if (target.ipg_caseid != null)
            {
                regardingClaim = RetrieveLastInvoiceOnCase(target.ipg_caseid.Id);
            }

            taskManager.ConfigureTaskByTaskType(target);

            if (regardingClaim?.ipg_ReasonEnum != ipg_ClaimReason.SubmittedPaper)
            {
                taskManager.CheckTaskForDuplicates(target);
            }
        }

        private void SyncInformation(LocalPluginContext localContext)
        {
            var target = localContext.Target<Task>();
            var taskManager = new TaskManager(localContext.OrganizationService, localContext.TracingService, target.ToEntityReference(), localContext.PluginExecutionContext.InitiatingUserId);

            taskManager.SyncStatuses(target);
        }

        private Invoice RetrieveLastInvoiceOnCase(Guid caseId)
        {
            var fetchXml = $@"<?xml version='1.0'?>
                                <fetch top='1' distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>        
                                    <entity name='invoice'>  
                                        <attribute name='ipg_reason'/> 
                                        <attribute name='createdon'/> 
                                        <order descending='true' attribute='createdon'/>
                                        <link-entity name='incident' alias='ah' link-type='inner' to='ipg_caseid' from='incidentid'>
                                            <attribute name='title'/>               
                                            <attribute name='incidentid'/>     
                                            <filter type='and'>
                                                <condition attribute='incidentid' value='{caseId}' operator='eq'/>
                                            </filter >   
                                        </link-entity>                                
                                    </entity>                                
                                </fetch>";
            return service.RetrieveMultiple(new FetchExpression(fetchXml)).Entities.FirstOrDefault()?.ToEntity<Invoice>();
        }
    }
}
