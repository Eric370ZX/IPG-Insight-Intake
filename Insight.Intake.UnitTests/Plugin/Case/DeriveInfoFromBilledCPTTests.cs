using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Xunit;


namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class DeriveInfoFromBilledCPTTests: PluginTestsBase
    {
        [Fact]
        public void CPTCategoryDerivedFromBilledCPTOnCaseCreate()
        {
            var fakedContext = new XrmFakedContext();

            ipg_procedurename procedureName = new ipg_procedurename().Fake();
            ipg_cptcode cptcode = new ipg_cptcode().Fake().WithProcedureNameReference(procedureName.ToEntityReference());

            Incident incident = new Incident().Fake().WithBilledCptCode(cptcode);

            var listForInit = new List<Entity>() { cptcode, incident };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            //ACT
            fakedContext.ExecutePluginWith<DeriveInfoFromBilledCPT>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var createdIncident = crmContext.CreateQuery<Incident>().FirstOrDefault();

            Assert.Equal(incident.ipg_procedureid.Id, cptcode.ipg_procedurename.Id);
        }

        [Fact]
        public void CPTCategoryDerivedFromBilledCPTOnCaseUpdate()
        {
            var fakedContext = new XrmFakedContext();

            ipg_procedurename procedureName = new ipg_procedurename().Fake();
            ipg_cptcode newcptcode = new ipg_cptcode().Fake().WithProcedureNameReference(procedureName.ToEntityReference());

            Incident incident = new Incident().Fake().WithBilledCptCode(newcptcode);


            var listForInit = new List<Entity>() { new Incident() {Id = incident.Id }, newcptcode };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            //ACT
            fakedContext.ExecutePluginWith<DeriveInfoFromBilledCPT>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var createdIncident = crmContext.CreateQuery<Incident>().FirstOrDefault();

            Assert.Equal(incident.ipg_procedureid.Id, newcptcode.ipg_procedurename.Id);
        }

        [Fact]
        public void CPTCategoryDerivedFromBilledCPTOnCPTUpdate()
        {
            var fakedContext = new XrmFakedContext();

            ipg_procedurename procedureName = new ipg_procedurename().Fake();
            ipg_cptcode cptcode = new ipg_cptcode().Fake().WithProcedureNameReference(procedureName.ToEntityReference());

            Incident incident1 = new Incident().FakeActive().WithBilledCptCode(cptcode);
            Incident incident2 = new Incident().FakeActive().WithBilledCptCode(cptcode);

            var listForInit = new List<Entity>() { cptcode, incident1 , incident2 };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", cptcode } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", cptcode } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            //ACT
            fakedContext.ExecutePluginWith<DeriveInfoFromBilledCPT>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var UpdatedIncident1 = crmContext.CreateQuery<Incident>().Where(r => r.IncidentId == incident1.Id).FirstOrDefault();
            var UpdatedIncident2 = crmContext.CreateQuery<Incident>().Where(r => r.IncidentId == incident2.Id).FirstOrDefault();

            Assert.Equal(UpdatedIncident1.ipg_procedureid.Id, cptcode.ipg_procedurename.Id);
            Assert.Equal(UpdatedIncident2.ipg_procedureid.Id, cptcode.ipg_procedurename.Id);
        }
    }
}
