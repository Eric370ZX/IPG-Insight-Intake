using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Insight.Intake.Data;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class UpdateCaseBenefitTypeTests : PluginTestsBase
    {
        #region Auto and WC carriers

        [Fact]
        public void DoesNotUpdate_BenefitType_If_Irrelevant_CarrierType()
        {
            //ARRANGE
            var carrier = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .WithCarrierType(ipg_CarrierType.Commercial)
                .Generate();
            var incident = new Incident().Fake()
                .WithCarrierReference(carrier)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseBenefitType>(pluginContext);

            //ASSERT
            Assert.Equal(ipg_BenefitType.HealthBenefitPlanCoverage, incident.ipg_BenefitTypeCodeEnum);
        }

        [Fact]
        public void Sets_BenefitType_Auto_If_Auto_Carrier()
        {
            //ARRANGE
            var carrier = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .WithCarrierType(ipg_CarrierType.Auto)
                .Generate();
            var incident = new Incident().Fake()
                .WithCarrierReference(carrier)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseBenefitType>(pluginContext);

            //ASSERT
            Assert.Equal(ipg_BenefitType.Auto, incident.ipg_BenefitTypeCodeEnum);
        }

        [Fact]
        public void Sets_BenefitType_WC_If_WC_Carrier()
        {
            //ARRANGE
            var carrier = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .WithCarrierType(ipg_CarrierType.WorkersComp)
                .Generate();
            var incident = new Incident().Fake()
                .WithCarrierReference(carrier)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseBenefitType>(pluginContext);

            //ASSERT
            Assert.Equal(ipg_BenefitType.WorkersComp, incident.ipg_BenefitTypeCodeEnum);
        }

        #endregion

        #region DME benefit type

        [Fact]
        public void SetsDmeBenefitTypeIfJqu()
        {
            //ARRANGE
            const string memberId = "JQU123";
            var carrier = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .WithCarrierType(ipg_CarrierType.Commercial)
                .Generate();
            var incident = new Incident().Fake()
                    .WithCarrierReference(carrier)
                    .WithMemberId(memberId)
                    .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseBenefitType>(pluginContext);

            //ASSERT
            Assert.Equal(ipg_BenefitType.DurableMedicalEquipment_DME, incident.ipg_BenefitTypeCodeEnum);
        }

        [Fact]
        public void DoesNotSetDmeBenefitTypeIfNotJqu()
        {
            //ARRANGE
            const string memberId = "JQ123";
            var incident = new Incident().Fake()
                    .WithMemberId(memberId)
                    .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseBenefitType>(pluginContext);

            //ASSERT
            var incidentAfterUpdate = ((Entity)inputParameters["Target"]).ToEntity<Incident>();
            Assert.Equal(ipg_BenefitType.HealthBenefitPlanCoverage, incidentAfterUpdate.ipg_BenefitTypeCodeEnum);
        }

        [Fact]
        public void SetsDmeSecondaryBenefitTypeIfJqu()
        {
            //ARRANGE
            const string memberId = "JQU123";
            var carrier = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .WithCarrierType(ipg_CarrierType.Commercial)
                .Generate();
            var carrier2 = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .WithCarrierType(ipg_CarrierType.Commercial)
                .Generate();
            var incident = new Incident().Fake()
                .WithCarrierReference(carrier)
                .WithCarrierReference(carrier2, isPrimaryCarrier: false)
                .WithSecondaryBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithSecondaryMemberId(memberId)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, carrier2, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseBenefitType>(pluginContext);

            //ASSERT
            Assert.Equal(ipg_BenefitType.DurableMedicalEquipment_DME, incident.ipg_Carrier2BenefitTypeCodeEnum);
        }

        [Fact]
        public void CreatesDmeUserTaskBenefitTypeIfJqu()
        {
            //ARRANGE
            const string memberId = "JQU123";
            var incident = new Incident().Fake()
                    .WithMemberId(memberId)
                    .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                    .Generate();
            var taskType = new ipg_tasktype().Fake()
                    .WithTypeId(TaskTypeIds.USE_DME_BENEFITS_IF_JQUPRFIX)
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, taskType });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseBenefitType>(pluginContext);

            //ASSERT
            var organizationService = fakedContext.GetOrganizationService();
            var task = organizationService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            })
            .Entities.Select(e => e.ToEntity<Task>()).FirstOrDefault(t => 
                    t.RegardingObjectId.Id == incident.Id 
                    && t.ipg_tasktypeid.Id == taskType.Id
                    && t.StateCode == TaskState.Open);

            Assert.NotNull(task);
        }

        [Fact]
        public void DoesNotCreateDmeUserTaskIfAlreadyExists()
        {
            //ARRANGE
            const string memberId = "JQU123";
            var incident = new Incident().Fake()
                    .WithMemberId(memberId)
                    .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                    .Generate();
            var taskType = new ipg_tasktype().Fake()
                    .WithTypeId(TaskTypeIds.USE_DME_BENEFITS_IF_JQUPRFIX)
                    .Generate();
            var existingTask = new Task().Fake()
                .WithTypeRef(taskType.ToEntityReference())
                .WithRegarding(incident.ToEntityReference())
                .WithState(TaskState.Open)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, taskType, existingTask });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseBenefitType>(pluginContext);

            //ASSERT
            var organizationService = fakedContext.GetOrganizationService();
            var tasks = organizationService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            })
            .Entities.Select(e => e.ToEntity<Task>()).Where(t =>
                    t.RegardingObjectId.Id == incident.Id
                    && t.ipg_tasktypeid.Id == taskType.Id
                    && t.StateCode == TaskState.Open);

            Assert.Single(tasks);
            Assert.Equal(existingTask.Id, tasks.First().Id);
        }

        [Fact]
        public void DoesNotCreateDmeUserTaskIfNotJQU()
        {
            //ARRANGE
            const string memberId = "JQ123";
            var incident = new Incident().Fake()
                    .WithMemberId(memberId)
                    .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                    .Generate();
            var taskType = new ipg_tasktype().Fake()
                    .WithTypeId(TaskTypeIds.USE_DME_BENEFITS_IF_JQUPRFIX)
                    .Generate();
            
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, taskType });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseBenefitType>(pluginContext);

            //ASSERT
            var organizationService = fakedContext.GetOrganizationService();
            var tasks = organizationService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            })
            .Entities.Select(e => e.ToEntity<Task>()).Where(t =>
                    t.RegardingObjectId.Id == incident.Id
                    && t.ipg_tasktypeid.Id == taskType.Id
                    && t.StateCode == TaskState.Open);

            Assert.Empty(tasks);
        }

        #endregion
    }
}
