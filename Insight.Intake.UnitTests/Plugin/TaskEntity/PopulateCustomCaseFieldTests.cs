using FakeXrmEasy;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class PopulateCustomCaseFieldTests : PluginTestsBase
    {
        [Fact]
        public void PopulateCustomCaseField_OnCreateTaskRegardingObjectIsCase_returnCaseIdIsUpdated()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEnt = new Incident().Fake();
            var task = new Task()
            {
                Id = Guid.NewGuid(),
                RegardingObjectId = caseEnt.ToEntityReference()
            };
            fakedContext.Initialize(new List<Entity> { caseEnt });

            var inputParameters = new ParameterCollection { { "Target", task  } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<PopulateCustomCaseField>(pluginContext);

            var target = (Entity)pluginContext.InputParameters["Target"];
            Assert.True(target.Contains("ipg_caseid") && target.GetAttributeValue<EntityReference>("ipg_caseid").Id == caseEnt.Id);
        }

        [Fact]
        public void PopulateCustomCaseField_OnCreateTaskRegardingObjectIsNotCase_returnCaseIdIsUpdated()
        {
            var fakedContext = new XrmFakedContext();

            var accountRef = new EntityReference(Intake.Account.EntityLogicalName, Guid.NewGuid());
            var task = new Task()
            {
                Id = Guid.NewGuid(),
                RegardingObjectId = accountRef
            };
            fakedContext.Initialize(new List<Entity> { task });

            var inputParameters = new ParameterCollection { { "Target", task } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<PopulateCustomCaseField>(pluginContext);

            var target = (Entity)pluginContext.InputParameters["Target"];
            Assert.True(!target.Contains("ipg_caseid"));
        }

        [Fact]
        public void PopulateCustomCaseField_OnUpdateTaskRegardingObjectIsCase_returnCaseIdIsUpdated()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEnt = new Incident().Fake();
            var target = new Task()
            {
                Id = Guid.NewGuid()
            };

            var preImageEntity = new Task()
            {
                Id = Guid.NewGuid(),
                RegardingObjectId = caseEnt.ToEntityReference()
            };
            fakedContext.Initialize(new List<Entity> { target, caseEnt });

            var inputParameters = new ParameterCollection { { "Target", target } };
            var preImage = new EntityImageCollection { { "Image", preImageEntity } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = preImage
            };

            fakedContext.ExecutePluginWith<PopulateCustomCaseField>(pluginContext);

            var targetOutcome = (Entity)pluginContext.InputParameters["Target"];
            Assert.True(targetOutcome.Contains("ipg_caseid") && targetOutcome.GetAttributeValue<EntityReference>("ipg_caseid").Id == caseEnt.Id);
        }

        [Fact]
        public void PopulateFacilityField_OnCreateTaskRegardingObjectIsCase_returnCaseIdIsUpdated()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account facility = new Intake.Account().Fake();
            Incident caseEnt = new Incident().Fake().WithFacilityReference(facility);
            var target = new Task()
            {
                Id = Guid.NewGuid()
            };

            var preImageEntity = new Task()
            {
                Id = Guid.NewGuid(),
                RegardingObjectId = caseEnt.ToEntityReference()
            };
            fakedContext.Initialize(new List<Entity> { target, facility, caseEnt });

            var inputParameters = new ParameterCollection { { "Target", target } };
            var preImage = new EntityImageCollection { { "Image", preImageEntity } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = preImage
            };

            fakedContext.ExecutePluginWith<PopulateCustomCaseField>(pluginContext);

            var targetOutcome = (Entity)pluginContext.InputParameters["Target"];
            Assert.True(targetOutcome.Contains("ipg_caseid") && targetOutcome.GetAttributeValue<EntityReference>("ipg_caseid").Id == caseEnt.Id, "Case field on tasl not filled!");
            Assert.True(targetOutcome.Contains("ipg_facilityid") && targetOutcome.GetAttributeValue<EntityReference>("ipg_facilityid").Id == facility.Id, "Facility field on tasl not filled!");
        }


        [Fact]
        public void PopulateFacilityField_OnUpdateTaskRegardingObjectIsCase_returnCaseIdIsUpdated()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account facility = new Intake.Account().Fake();
            Incident caseEnt = new Incident().Fake().WithFacilityReference(facility);
            var target = new Task()
            {
                Id = Guid.NewGuid()
            };

            var preImageEntity = new Task()
            {
                Id = Guid.NewGuid(),
                RegardingObjectId = caseEnt.ToEntityReference()
            };
            fakedContext.Initialize(new List<Entity> { target, facility, caseEnt });

            var inputParameters = new ParameterCollection { { "Target", target } };
            var preImage = new EntityImageCollection { { "Image", preImageEntity } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = preImage
            };

            fakedContext.ExecutePluginWith<PopulateCustomCaseField>(pluginContext);

            var targetOutcome = (Entity)pluginContext.InputParameters["Target"];
            Assert.True(targetOutcome.Contains("ipg_caseid") && targetOutcome.GetAttributeValue<EntityReference>("ipg_caseid").Id == caseEnt.Id, "Case field on tasl not filled!");
            Assert.True(targetOutcome.Contains("ipg_facilityid") && targetOutcome.GetAttributeValue<EntityReference>("ipg_facilityid").Id == facility.Id, "Facility field on tasl not filled!");
        }

        [Fact]
        public void PopulateFacilityField_OnCreateTaskRegardingObjectIsNotCase_returnCaseIdIsUpdated()
        {
            var fakedContext = new XrmFakedContext();

            var accountRef = new EntityReference(Intake.Account.EntityLogicalName, Guid.NewGuid());
            var task = new Task()
            {
                Id = Guid.NewGuid(),
                RegardingObjectId = accountRef
            };
            fakedContext.Initialize(new List<Entity> { task });

            var inputParameters = new ParameterCollection { { "Target", task } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<PopulateCustomCaseField>(pluginContext);

            var target = (Entity)pluginContext.InputParameters["Target"];
            Assert.True(!target.Contains("ipg_caseid"));
            Assert.True(!target.Contains("i[g_facilityid"));
        }
    }
}
