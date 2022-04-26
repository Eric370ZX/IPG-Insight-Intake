using FakeXrmEasy;
using Insight.Intake.Plugin.Note;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Microsoft.Xrm.Sdk.Messages;
using System.Xml.Linq;

namespace Insight.Intake.UnitTests.Plugin.Note
{
    public class ModifyNotesViewTest : PluginTestsBase
    {
        [Fact]
        public void FetchQueryModifiedTest()
        {
            SystemUser user = new SystemUser().Fake("InitiateUser");
            Role role = new Role().Fake("UserNotes");
            Incident incident = new Incident().Fake();
            var fakedContext = new XrmFakedContext();

            fakedContext.Initialize(new List<Entity>() { user, role, incident });

            fakedContext.AddRelationship("systemuserroles_association", new XrmFakedRelationship
            {
                IntersectEntity = SystemUserRoles.EntityLogicalName,
                Entity1LogicalName = SystemUser.EntityLogicalName,
                Entity1Attribute = SystemUser.PrimaryIdAttribute,
                Entity2LogicalName = Role.EntityLogicalName,
                Entity2Attribute = Role.PrimaryIdAttribute
            });

            var crmService = fakedContext.GetOrganizationService();

            var request = new AssociateRequest()
            {
                Target = user.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                    {
                        role.ToEntityReference()
                    },
                Relationship = new Relationship("systemuserroles_association")
            };

            crmService.Execute(request);

            ParameterCollection input = new ParameterCollection() {
                    {"Query", new FetchExpression($@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' returntotalrecordcount='false' page='1' count='10' no-lock='false'>
                                                    <entity name='annotation'>
                                                    <attribute name='annotationid'/>
                                                    <attribute name='subject'/>
                                                    <attribute name='notetext'/>
                                                    <attribute name='filename'/>
                                                    <attribute name='filesize'/>
                                                    <attribute name='isdocument'/>
                                                    <attribute name='createdby'/>
                                                    <attribute name='createdon'/>
                                                    <attribute name='modifiedby'/>
                                                    <attribute name='modifiedon'/>
                                                    <attribute name='mimetype'/>
                                                    <order attribute='modifiedon' descending='true'/>
                                                    <order attribute='annotationid' descending='false'/>
                                                    <link-entity name='systemuser' from='systemuserid' to='modifiedby' alias='systemuser' link-type='outer'>
                                                    <attribute name='entityimage_url'/>
                                                    <attribute name='systemuserid'/>
                                                    <attribute name='fullname'/>
                                                    </link-entity><filter type='and'>
                                                    <filter type='and'>
                                                    <condition attribute='objectid' operator='eq' value='{incident.Id}'/></filter>
                                                    </filter>
                                                    </entity></fetch>") }
                };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.RetrieveMultiple,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Annotation.EntityLogicalName,
                Depth = 1,
                InitiatingUserId = user.Id,
                InputParameters = input
            };

            fakedContext.ExecutePluginWith<ModifyNotesView>(pluginContext);
            var queryStr = (input["Query"] as FetchExpression).Query;
            XDocument parsedQuery = XDocument.Parse(queryStr);
            
            Assert.Single(parsedQuery.Descendants("link-entity").Where(e => e.Attribute("name")?.Value == "ipg_payment"));
            Assert.Single(parsedQuery.Descendants("link-entity").Where(e => e.Attribute("name")?.Value == "task"));
            Assert.Single(parsedQuery.Descendants("link-entity").Where(e => e.Attribute("name")?.Value == "ipg_adjustment"));
            Assert.Equal(2, parsedQuery.Descendants("link-entity").Where(e => e.Attribute("name")?.Value == "systemuser").Count());
            
        }
        public void FetchQueryNotModifiedTest()
        {
            SystemUser user = new SystemUser().Fake("InitiateUser");
            Incident incident = new Incident().Fake();
            var fakedContext = new XrmFakedContext();

            fakedContext.Initialize(new List<Entity>() { user, incident });

            ParameterCollection input = new ParameterCollection() {
                    {"Query", new FetchExpression($@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' returntotalrecordcount='false' page='1' count='10' no-lock='false'>
                                                    <entity name='annotation'>
                                                    <attribute name='annotationid'/>
                                                    <attribute name='subject'/>
                                                    <attribute name='notetext'/>
                                                    <attribute name='filename'/>
                                                    <attribute name='filesize'/>
                                                    <attribute name='isdocument'/>
                                                    <attribute name='createdby'/>
                                                    <attribute name='createdon'/>
                                                    <attribute name='modifiedby'/>
                                                    <attribute name='modifiedon'/>
                                                    <attribute name='mimetype'/>
                                                    <order attribute='modifiedon' descending='true'/>
                                                    <order attribute='annotationid' descending='false'/>
                                                    <link-entity name='systemuser' from='systemuserid' to='modifiedby' alias='systemuser' link-type='outer'>
                                                    <attribute name='entityimage_url'/>
                                                    <attribute name='systemuserid'/>
                                                    <attribute name='fullname'/>
                                                    </link-entity><filter type='and'>
                                                    <filter type='and'>
                                                    <condition attribute='objectid' operator='eq' value='{incident.Id}'/></filter>
                                                    </filter>
                                                    </entity></fetch>") }
                };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.RetrieveMultiple,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Annotation.EntityLogicalName,
                Depth = 1,
                InitiatingUserId = user.Id,
                InputParameters = input
            };

            fakedContext.ExecutePluginWith<ModifyNotesView>(pluginContext);
            var queryStr = (input["Query"] as FetchExpression).Query;
            XDocument parsedQuery = XDocument.Parse(queryStr);

            Assert.Empty(parsedQuery.Descendants("link-entity").Where(e => e.Attribute("name")?.Value == "ipg_payment"));
            Assert.Empty(parsedQuery.Descendants("link-entity").Where(e => e.Attribute("name")?.Value == "task"));
            Assert.Empty(parsedQuery.Descendants("link-entity").Where(e => e.Attribute("name")?.Value == "ipg_adjustment"));
            Assert.Single(parsedQuery.Descendants("link-entity").Where(e => e.Attribute("name")?.Value == "systemuser"));

        }
    }
}
