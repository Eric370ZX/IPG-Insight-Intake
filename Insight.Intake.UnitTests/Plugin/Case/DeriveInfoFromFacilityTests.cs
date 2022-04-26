using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Xunit;


namespace Insight.Intake.UnitTests.Plugin
{
    public class DeriveInfoFromFacilityTests: PluginTestsBase
    {
        [Fact]
        public void InfoDerivedFromFacilityOnCaseCreate()
        {
            var fakedContext = new XrmFakedContext();
            SystemUser MDD = new SystemUser().Fake();
            SystemUser CIM = new SystemUser().Fake();
            SystemUser CaseManager = new SystemUser().Fake();


            Intake.Account parent = new Intake.Account().Fake();
            Intake.Account Facility = new Intake.Account().Fake()
                .WithMemberOf(parent.ToEntityReference())
                .WithManagers(MDD, CIM, CaseManager);


            Incident incident = new Incident().FakeActive().WithFacilityReference(Facility);

            var listForInit = new List<Entity>() { MDD, CIM, CaseManager, parent, Facility, incident};
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
            fakedContext.ExecutePluginWith<DeriveInfoFromFacility>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var createdIncident = crmContext.CreateQuery<Incident>().FirstOrDefault();
            Assert.Equal(incident.ipg_facilitymemberofid, parent.ToEntityReference());
        }

        [Fact]
        public void InfoDerivedFromFacilityOnCaseUpdate()
        {
            var fakedContext = new XrmFakedContext();
            SystemUser MDD = new SystemUser().Fake();
            SystemUser CIM = new SystemUser().Fake();
            SystemUser CaseManager = new SystemUser().Fake();


            Intake.Account parent = new Intake.Account().Fake();
            Intake.Account Facility = new Intake.Account().Fake()
                .WithMemberOf(parent.ToEntityReference())
                .WithManagers(MDD, CIM, CaseManager);


            Incident incident = new Incident().FakeActive().WithFacilityReference(Facility);

            var listForInit = new List<Entity>() { MDD, CIM, CaseManager, parent, Facility, incident };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() {},
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            //ACT
            fakedContext.ExecutePluginWith<DeriveInfoFromFacility>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var createdIncident = crmContext.CreateQuery<Incident>().FirstOrDefault();
            Assert.Equal(incident.ipg_facilitymemberofid, parent.ToEntityReference());
        }

        [Fact]
        public void InfoDerivedFromFacilityOnFacilityUpdate()
        {
            var fakedContext = new XrmFakedContext();
            SystemUser MDD = new SystemUser().Fake();
            SystemUser CIM = new SystemUser().Fake();
            SystemUser CaseManager = new SystemUser().Fake();


            Intake.Account parent = new Intake.Account().Fake();
            Intake.Account Facility = new Intake.Account().Fake((int)Account_CustomerTypeCode.Facility)
                .WithMemberOf(parent.ToEntityReference())
                .WithManagers(MDD, CIM, CaseManager);


            Incident incident = new Incident().FakeActive().WithFacilityReference(Facility);

            var listForInit = new List<Entity>() { MDD, CIM, CaseManager, parent, Facility, incident };
            fakedContext.Initialize(listForInit);


            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Intake.Account.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", Facility } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", Facility } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            //ACT
            fakedContext.ExecutePluginWith<DeriveInfoFromFacility>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var createdIncident = crmContext.CreateQuery<Incident>().FirstOrDefault();
            Assert.Equal(createdIncident.ipg_facilitymemberofid, parent.ToEntityReference());
        }
    }
}
