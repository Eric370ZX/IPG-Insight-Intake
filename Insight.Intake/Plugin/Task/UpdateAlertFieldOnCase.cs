using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class UpdateAlertFieldOnCase : PluginBase
    {
        public UpdateAlertFieldOnCase() : base(typeof(UpdateAlertFieldOnCase))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Task.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Task.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            if (context.InputParameters["Target"] is Entity targetTask)
            {
                var caseId = new Guid();
                if (context.MessageName == MessageNames.Create && targetTask.Contains(Task.Fields.ipg_caseid))
                {
                    caseId = targetTask.GetAttributeValue<EntityReference>(Task.Fields.ipg_caseid).Id;
                }
                if (context.MessageName == MessageNames.Update)
                {
                    var postImage = context.PostEntityImages.Contains("PostImage") ? localPluginContext.PostImage<Entity>() : null;
                    if (postImage != null && postImage.Contains(Task.Fields.ipg_caseid))
                    {
                        caseId = postImage.GetAttributeValue<EntityReference>(Task.Fields.ipg_caseid).Id;
                    }
                }
                if (caseId != Guid.Empty)
                {
                    var entities = GetCaseWithMissingTasks(service, caseId);
                    if (entities != null && entities.Count > 0)
                    {
                        Incident incident = new Entity(Incident.EntityLogicalName, caseId).ToEntity<Incident>();
                        if (IsCaseWithMissingInformation(entities))
                        {
                            incident.ipg_portalalerts = "Missing Information";
                        }
                        else if (entities.FirstOrDefault().ToEntity<Incident>().ipg_portalalerts == "Missing Information")
                        {
                            incident.ipg_portalalerts = null;
                        }
                        service.Update(incident);
                    }
                }
            }
        }

        private bool IsCaseWithMissingInformation(DataCollection<Entity> entities)
        {
            return entities.Any(incidentEntity =>
            {
                var taskTypeName = incidentEntity.GetAttributeValue<AliasedValue>($"{ipg_tasktype.EntityLogicalName}.{ipg_tasktype.Fields.ipg_name}")?.Value as string;
                if (taskTypeName != null)
                {
                    return taskTypeName.ToLower().Contains("missing");
                }
                return false;
            });
        }
        private DataCollection<Entity> GetCaseWithMissingTasks(IOrganizationService service, Guid caseId)
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' >
                                  <entity name='{Incident.EntityLogicalName}' >
                                      <attribute name='{Incident.Fields.Id}' />
                                      <attribute name='{Incident.Fields.ipg_portalalerts}' />
                                      <filter type='and'>
                                            <condition attribute='{Incident.Fields.Id}' operator='eq' value='{caseId}' />
                                      </filter>
                                      <link-entity name='{Task.EntityLogicalName}' from='{Task.Fields.ipg_caseid}' to='{Incident.Fields.Id}' link-type='inner' alias='{Task.EntityLogicalName}' >
                                              <filter type='and'>
                                                    <condition attribute='{Task.Fields.ipg_isvisibleonportal}' operator='eq' value='1' />
                                                    <condition attribute = '{Task.Fields.ipg_portalstatus}' operator= 'eq' value = '{(int)Task_ipg_portalstatus.Open}' />
                                              </filter>
                                              <link-entity name='{ipg_tasktype.EntityLogicalName}' from='{ipg_tasktype.Fields.ipg_tasktypeId}' to='{Task.Fields.ipg_tasktypeid}' link-type='inner' alias='{ipg_tasktype.EntityLogicalName}' >
                                                  <attribute name='{ipg_tasktype.Fields.ipg_name}' />
                                              </link-entity>
                                      </link-entity>
                                  </entity>
                              </fetch>";
            var incidents = service.RetrieveMultiple(new FetchExpression(fetch)).Entities;
            return incidents;
        }
    }
}

