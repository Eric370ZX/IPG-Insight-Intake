using FakeXrmEasy;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Common.Interfaces;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System;
using Xunit;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class EhrCarrierMappingTaskTests
    {
        [Fact]
        public void CompletesMapping()
        {
            //ARRANGE

            var fakedContext = new XrmFakedContext();

            var globalSetting = new ipg_globalsetting().Fake(Constants.Settings.EHRResubmitURL, "https://azure.com?key=123")
                .Generate();
            var facility = new Intake.Account().Fake((int)Account_CustomerTypeCode.Facility).Generate();
            var carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier).Generate();
            var mapping = new ipg_ehrcarriermap().Fake()
                .WithFacility(facility)
                .WithCarrier(carrier)
                .WithStatus(ipg_EHRCarrierMappingStatuses.Pending)
                .WithTake(false)
                .Generate();
            var taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.UNKNOWN_FACILITY_CARRIER_COMBINATION_FROM_EHR).Generate();
            var task = new Task().Fake()
                .WithState(TaskState.Completed)
                .Generate();
            var taskPostImage = new Task().Fake()
                .WithTypeRef(taskType.ToEntityReference())
                .WithRegarding(mapping.ToEntityReference())
                .Generate();
            
            fakedContext.Initialize(new Entity[] { globalSetting, facility, carrier, mapping, taskType, task });

            var inputParameters = new ParameterCollection { { "Target", task } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", taskPostImage } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            var httpClientServiceMock = new Mock<IHttpClientService>();

            //ACT
            fakedContext.ExecutePluginWith(pluginContext, new EhrCarrierMappingTasks(httpClientServiceMock.Object));

            //ASSERT

            httpClientServiceMock.Verify(s => s.PostAsync(globalSetting.ipg_value + "&CarrierMappingId=" + mapping.ipg_ehrcarriermapId));

            var organizationService = fakedContext.GetOrganizationService();
            var updatedMapping = organizationService.Retrieve(ipg_ehrcarriermap.EntityLogicalName, mapping.ipg_ehrcarriermapId.Value, new ColumnSet(true)).ToEntity<ipg_ehrcarriermap>();
            Assert.Equal(ipg_EHRCarrierMappingStatuses.Complete, updatedMapping.ipg_StatusEnum);
            Assert.True(updatedMapping.ipg_take);
        }

        [Fact]
        public void Does_Not_Complete_Mapping_If_Task_Is_Not_Complete()
        {
            //ARRANGE

            var fakedContext = new XrmFakedContext();

            var mapping = new ipg_ehrcarriermap().Fake()
                .WithStatus(ipg_EHRCarrierMappingStatuses.Pending)
                .Generate();
            var task = new Task().Fake()
                .WithState(TaskState.Open)
                .Generate();

            fakedContext.Initialize(new Entity[] {   mapping, task });

            var inputParameters = new ParameterCollection { { "Target", task } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            var httpClientServiceMock = new Mock<IHttpClientService>();

            //ACT
            fakedContext.ExecutePluginWith(pluginContext, new EhrCarrierMappingTasks(httpClientServiceMock.Object));

            //ASSERT

            httpClientServiceMock.Verify(s => s.PostAsync(It.IsAny<string>()), Times.Never);

            var organizationService = fakedContext.GetOrganizationService();
            var updatedMapping = organizationService.Retrieve(ipg_ehrcarriermap.EntityLogicalName, mapping.ipg_ehrcarriermapId.Value, new ColumnSet(true)).ToEntity<ipg_ehrcarriermap>();
            Assert.Equal(ipg_EHRCarrierMappingStatuses.Pending, updatedMapping.ipg_StatusEnum);
        }

        [Fact]
        public void Does_Not_Complete_Mapping_If_Wrong_Task_Type()
        {
            //ARRANGE

            var fakedContext = new XrmFakedContext();

            var mapping = new ipg_ehrcarriermap().Fake()
                .WithStatus(ipg_EHRCarrierMappingStatuses.Pending)
                .WithTake(false)
                .Generate();
            var taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.USE_DME_BENEFITS_IF_JQUPRFIX).Generate();
            var task = new Task().Fake()
                .WithState(TaskState.Completed)
                .Generate();
            var taskPostImage = new Task().Fake()
                .WithTypeRef(taskType.ToEntityReference())
                .WithRegarding(mapping.ToEntityReference())
                .WithState(TaskState.Completed)
                .Generate();

            fakedContext.Initialize(new Entity[] {  mapping, taskType, task });

            var inputParameters = new ParameterCollection { { "Target", task } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", taskPostImage } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            var httpClientServiceMock = new Mock<IHttpClientService>();


            //ACT
            fakedContext.ExecutePluginWith(pluginContext, new EhrCarrierMappingTasks(httpClientServiceMock.Object));
            
            
            //ASSERT
            httpClientServiceMock.Verify(s => s.PostAsync(It.IsAny<string>()), Times.Never);

            var organizationService = fakedContext.GetOrganizationService();
            var updatedMapping = organizationService.Retrieve(ipg_ehrcarriermap.EntityLogicalName, mapping.ipg_ehrcarriermapId.Value, new ColumnSet(true)).ToEntity<ipg_ehrcarriermap>();
            Assert.Equal(ipg_EHRCarrierMappingStatuses.Pending, updatedMapping.ipg_StatusEnum);
        }

        [Fact]
        public void Throws_If_Carrier_Is_Not_Set()
        {
            //ARRANGE

            var fakedContext = new XrmFakedContext();

            var facility = new Intake.Account().Fake((int)Account_CustomerTypeCode.Facility).Generate();
            var mapping = new ipg_ehrcarriermap().Fake()
                .WithFacility(facility)
                .WithStatus(ipg_EHRCarrierMappingStatuses.Pending)
                .WithTake(false)
                .Generate();
            var taskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.UNKNOWN_FACILITY_CARRIER_COMBINATION_FROM_EHR).Generate();
            var task = new Task().Fake()
                .WithState(TaskState.Completed)
                .Generate();
            var taskPostImage = new Task().Fake()
                .WithTypeRef(taskType.ToEntityReference())
                .WithRegarding(mapping.ToEntityReference())
                .Generate();

            fakedContext.Initialize(new Entity[] {  facility, mapping, taskType, task });

            var inputParameters = new ParameterCollection { { "Target", task } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", taskPostImage } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            var httpClientServiceMock = new Mock<IHttpClientService>();


            //ACT

            Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith(pluginContext, new EhrCarrierMappingTasks(httpClientServiceMock.Object)));
            
        }
    }
}