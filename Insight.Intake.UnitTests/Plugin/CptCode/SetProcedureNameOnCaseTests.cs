using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy;
using Insight.Intake.Plugin.CptCode;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;
using Microsoft.Xrm.Sdk.Query;


namespace Insight.Intake.UnitTests.Plugin.CptCode
{
    public class SetProcedureNameOnCaseTests : PluginTestsBase
    {
        [Theory]
        [InlineData("Update")]
        public void CheckUpdateMultipleCases(string messageName)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            ipg_cptcode fakeCptCode = new ipg_cptcode().Fake();
            fakeCptCode.ipg_procedurename = new EntityReference(ipg_procedurename.EntityLogicalName, new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90"));

            Incident fakedCase0 = new Incident()
            { 
                IncidentId = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad89"),
                StateCode = 0,
                ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Open),
                ipg_BilledCPTId = fakeCptCode.ToEntityReference()       
            };
            Incident fakedCase1 = new Incident()
            {
                IncidentId = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad88"),
                StateCode = 0,
                ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Open),
                ipg_BilledCPTId = fakeCptCode.ToEntityReference()
            };
            Incident fakedCase2 = new Incident()
            {
                IncidentId = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad87"),
                StateCode = 0,
                ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Open),
                ipg_BilledCPTId = fakeCptCode.ToEntityReference()
            };

            var listForInit = new List<Entity>() { fakedCase0, fakedCase1, fakedCase2 };
            fakedContext.Initialize(listForInit);

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_cptcode.EntityLogicalName,
                PostEntityImages = new EntityImageCollection() { { "PostImage", fakeCptCode } }
            };

            // Act
            fakedContext.ExecutePluginWith<SetProcedureNameOnCase>(fakedPluginContext);
            var incident0 = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase0.Id, new ColumnSet(Incident.Fields.ipg_procedureid))?.ToEntity<Incident>();
            var incident1 = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase1.Id, new ColumnSet(Incident.Fields.ipg_procedureid))?.ToEntity<Incident>();
            var incident2 = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase2.Id, new ColumnSet(Incident.Fields.ipg_procedureid))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(incident0.ipg_procedureid.Id, new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90"));
            Assert.Equal(incident1.ipg_procedureid.Id, new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90"));
            Assert.Equal(incident2.ipg_procedureid.Id, new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90"));
        }

        [Theory]
        [InlineData("Update")]
        public void NoProcedureName(string messageName)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            ipg_cptcode fakeCptCode = new ipg_cptcode().Fake();
            fakeCptCode.ipg_procedurename = null;

            Incident fakedCase0 = new Incident()
            {
                IncidentId = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad89"),
                StateCode = 0,
                ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Open),
                ipg_BilledCPTId = fakeCptCode.ToEntityReference(),
                ipg_procedureid = new EntityReference(ipg_procedurename.EntityLogicalName, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"))
            };
            Incident fakedCase1 = new Incident()
            {
                IncidentId = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad88"),
                StateCode = 0,
                ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Open),
                ipg_BilledCPTId = fakeCptCode.ToEntityReference(),
                ipg_procedureid = new EntityReference(ipg_procedurename.EntityLogicalName, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"))
            };
            Incident fakedCase2 = new Incident()
            {
                IncidentId = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad87"),
                StateCode = 0,
                ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Open),
                ipg_BilledCPTId = fakeCptCode.ToEntityReference(),
                ipg_procedureid = new EntityReference(ipg_procedurename.EntityLogicalName, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"))
            };

            var listForInit = new List<Entity>() { fakedCase0, fakedCase1, fakedCase2 };
            fakedContext.Initialize(listForInit);

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_cptcode.EntityLogicalName,
                PostEntityImages = new EntityImageCollection() { { "PostImage", fakeCptCode } }
            };

            // Act
            fakedContext.ExecutePluginWith<SetProcedureNameOnCase>(fakedPluginContext);
            var incident0 = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase0.Id, new ColumnSet(Incident.Fields.ipg_procedureid))?.ToEntity<Incident>();
            var incident1 = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase1.Id, new ColumnSet(Incident.Fields.ipg_procedureid))?.ToEntity<Incident>();
            var incident2 = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase2.Id, new ColumnSet(Incident.Fields.ipg_procedureid))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(incident0.ipg_procedureid.Id, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"));
            Assert.Equal(incident1.ipg_procedureid.Id, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"));
            Assert.Equal(incident2.ipg_procedureid.Id, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"));
        }

        [Theory]
        [InlineData("Update")]
        public void NoBilledCPT(string messageName)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            ipg_cptcode fakeCptCode = new ipg_cptcode().Fake();
            fakeCptCode.ipg_procedurename = null;

            Incident fakedCase0 = new Incident()
            {
                IncidentId = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad89"),
                StateCode = 0,
                ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Open),
                ipg_BilledCPTId = null,
                ipg_procedureid = new EntityReference(ipg_procedurename.EntityLogicalName, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"))
            };
            Incident fakedCase1 = new Incident()
            {
                IncidentId = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad88"),
                StateCode = 0,
                ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Open),
                ipg_BilledCPTId = null,
                ipg_procedureid = new EntityReference(ipg_procedurename.EntityLogicalName, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"))
            };
            Incident fakedCase2 = new Incident()
            {
                IncidentId = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad87"),
                StateCode = 0,
                ipg_CaseStatus = new OptionSetValue((int)ipg_CaseStatus.Open),
                ipg_BilledCPTId = null,
                ipg_procedureid = new EntityReference(ipg_procedurename.EntityLogicalName, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"))
            };

            var listForInit = new List<Entity>() { fakedCase0, fakedCase1, fakedCase2 };
            fakedContext.Initialize(listForInit);

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_cptcode.EntityLogicalName,
                PostEntityImages = new EntityImageCollection() { { "PostImage", fakeCptCode } }
            };

            // Act
            fakedContext.ExecutePluginWith<SetProcedureNameOnCase>(fakedPluginContext);
            var incident0 = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase0.Id, new ColumnSet(Incident.Fields.ipg_procedureid))?.ToEntity<Incident>();
            var incident1 = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase1.Id, new ColumnSet(Incident.Fields.ipg_procedureid))?.ToEntity<Incident>();
            var incident2 = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase2.Id, new ColumnSet(Incident.Fields.ipg_procedureid))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(incident0.ipg_procedureid.Id, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"));
            Assert.Equal(incident1.ipg_procedureid.Id, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"));
            Assert.Equal(incident2.ipg_procedureid.Id, new Guid("e9117a91-ba0a-e411-b681-6c3be5a8ad89"));
        }
    }
}
