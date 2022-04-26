using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Insight.Intake.Plugin.Note
{
    public class ModifyNotesView : PluginBase
    {
        const string role = "UserNotes";
        const string filterOutSystemNotesLink =
            @"<link-entity name='systemuser' from='systemuserid' to='createdby'>
                  <attribute name='fullname' />
                  <filter type='or'>
                    <filter>
                        <condition attribute='applicationid' operator='null' />
                        <condition attribute='fullname' operator='neq' value='SYSTEM'/>
                    </filter>
                    <condition attribute='fullname' operator='eq' value='EHR Application'/>
                  </filter>
                </link-entity>";

        public ModifyNotesView() : base(typeof(ModifyNotesView))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.RetrieveMultiple, Annotation.EntityLogicalName, PreOperationhandler);
        }

        private void PreOperationhandler(LocalPluginContext obj)
        {
            var context = obj.PluginExecutionContext;
            var service = obj.OrganizationService;
            var tracing = obj.TracingService;

            FetchExpression fetchXml = context.InputParameters["Query"] as FetchExpression;
         

            if (fetchXml != null && context.Depth == 1)
            {
                XDocument parsedQuery = XDocument.Parse(fetchXml.Query);

                var filteredByObjectAttrCondtion = parsedQuery.Descendants("condition").Where(e => e.Attribute("attribute")?.Value == "objectid").ToList();
                var firsFtilteredByObjectAttrCondtion = filteredByObjectAttrCondtion.FirstOrDefault();
                var linkEntities = parsedQuery.Descendants("link-entity").ToList();
                var linkedUser = linkEntities.Where(e => e.Attribute("name")?.Value == SystemUser.EntityLogicalName
                && e.Attribute("to")?.Value == Annotation.Fields.ModifiedBy
                && e.Attribute("link-type")?.Value == "outer").FirstOrDefault();

                var entityElement = parsedQuery.Descendants("entity").FirstOrDefault();
                
                entityElement.Add(XElement.Parse(filterOutSystemNotesLink));

                if (filteredByObjectAttrCondtion.Count == 1
                    && firsFtilteredByObjectAttrCondtion.Attribute("operator").Value == "eq"
                    && linkEntities.Count == 1
                    && linkedUser != null
                    && linkedUser.Elements().Count() == 3)
                {
                    var incidentGuid = new Guid(firsFtilteredByObjectAttrCondtion.Attribute("value").Value);

                    if (IsUserHaveRole(context.InitiatingUserId, service, tracing) && IsIncidentGuid(incidentGuid, service, tracing))
                    {
                        firsFtilteredByObjectAttrCondtion.Remove();

                        entityElement.Add(XElement.Parse("<link-entity name='ipg_payment' from='ipg_paymentid' to='objectid' link-type='outer' alias='payment'/>"));
                        entityElement.Add(XElement.Parse("<link-entity name='task' from='activityid' to='objectid' link-type='outer' alias='task'/>"));
                        entityElement.Add(XElement.Parse("<link-entity name='ipg_adjustment' from='ipg_adjustmentid' to='objectid' link-type='outer' alias='adjustment'/>"));

                        entityElement.Add(XElement.Parse(@"<filter type='or'>
                                          <condition attribute='objectid' operator='eq' value='objid'/>
                                          <condition entityname='payment' attribute='ipg_caseid' operator='eq' value='objid' />
                                          <condition entityname='task' attribute='ipg_caseid' operator='eq' value='objid' />
                                          <condition entityname='adjustment' attribute='ipg_caseid' operator='eq' value='objid' />
                                        </filter>".Replace("objid", incidentGuid.ToString())));

                        fetchXml.Query = parsedQuery.ToString();
                    }
                }
            }
        }

        private bool IsIncidentGuid(Guid? guid, IOrganizationService service, ITracingService tracing)
        {
            return guid.HasValue && service.RetrieveMultiple(new QueryExpression(Incident.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(false),
                TopCount = 1,
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Incident.PrimaryIdAttribute, ConditionOperator.Equal, guid.Value)
                    }
                }
            }).Entities.Count > 0;
        }

        private bool IsUserHaveRole(Guid initiatingUserId, IOrganizationService service, ITracingService tracing)
        {

            var query = new QueryExpression(SystemUserRoles.EntityLogicalName)
            {
                NoLock = true,
                TopCount = 1,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(SystemUserRoles.Fields.SystemUserId, ConditionOperator.Equal, initiatingUserId)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(SystemUserRoles.EntityLogicalName, Role.EntityLogicalName, SystemUserRoles.Fields.RoleId, Role.PrimaryIdAttribute, JoinOperator.Inner)
                    {
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression(Role.PrimaryNameAttribute, ConditionOperator.Equal, role)
                            }
                        }
                    }
                }
            };

            return service.RetrieveMultiple(query).Entities.Count > 0;
        }
    }
}
