using FakeXrmEasy;
using FakeXrmEasy.Extensions;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Insight.Intake.UnitTests.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Insight.Intake.Helpers.Constants;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class ValidateMissingInformationTest : PluginTestsBase
    {
        private readonly AttributeMetadata[] Attributes = new AttributeMetadata[] {
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_PatientLastName), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "Patient Last Name"} } },
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_PatientFirstName), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "Patient Last Name"} } },
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_PatientDateofBirth), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "Patient Date of Birth"} } },
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_PatientAddress), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "Patient Address"} } },
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_PatientCity), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "Patient City"} } },
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_PatientState), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "Patient State"} } },
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_PatientZipCodeId), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "Patient Zip"} } },
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_DxCodeId1), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "Dx Code Id 1"} } },
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_CPTCodeId1), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "CPT Code Id 1"} } },
                     new AttributeMetadata(){ SchemaName = nameof(Incident.ipg_PhysicianId), DisplayName = new Label(){ UserLocalizedLabel = new LocalizedLabel(){ Label = "Physician"} } }
                 };
        [Fact]
        public void ValidateNoTaskCreatedIfCaseHasNoMissingInformation()
        {
            //ARRANGE

            ServiceProvider = ServiceProviderMock.Object;
            Contact fakePatient = new Contact().Fake();
            ipg_cptcode fakeCpt = new ipg_cptcode().Fake();
            ipg_dxcode fakeDx = new ipg_dxcode().Fake();

            Incident fakeIncident = new Incident().Fake()
                                                  .WithPatientReference(fakePatient)
                                                  .FakeWithPatientDemographics(fakePatient)
                                                  .FakeWithCptCode(fakeCpt)
                                                  .FakeWithDxCode(fakeDx)
                                                  .FakeWithPhysician();
            fakeIncident.ipg_SurgeryDate = DateTime.Now;

            OrganizationServiceMock.WithRetrieveCrud(fakeIncident);
            OrganizationService = OrganizationServiceMock.Object;
            
            var inputTarget = new EntityReference(Incident.EntityLogicalName, fakeIncident.Id);
            var inputParameters = new ParameterCollection
            {
                {"Target", inputTarget}
            };

            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            var entityMetaData = new EntityMetadata();
            Utility.SetPropertyValue(entityMetaData,
                 Utility.GetPropertyName(() => entityMetaData.Attributes), Attributes);

            OrganizationServiceMock.Setup(x => x.Execute(
                    It.Is<RetrieveEntityRequest>(r => r.LogicalName == Incident.EntityLogicalName)))
                .Returns(new RetrieveEntityResponse()
                {
                    Results = new ParameterCollection
                        {
                            { "EntityMetadata", entityMetaData  }
                        }
                });

            Task taskCreated = null;

            OrganizationServiceMock.Setup(t =>
                t.Create(It.Is<Entity>(e => e.LogicalName.ToUpper() == "task".ToUpper()))) //only match an entity with a logical name of "task"
                .Returns(Guid.NewGuid()) //can return any guid here
                .Callback<Task>(s => taskCreated = s); //store the Create method invocation parameter for inspection later

            //ACT
            var plugin = new ValidateMissingInformation();
            plugin.Execute(ServiceProvider);

            //ASSERT
            Assert.Null(taskCreated);
        }

        [Fact]
        public void ValidateTaskCreatedIfCaseHasMissingInformation()
        {
            //ARRANGE

            var fakedContext = new XrmFakedContext();
            Contact fakePatient = new Contact().Fake();
            ipg_cptcode fakeCpt = new ipg_cptcode().Fake();
            ipg_taskcategory missingInfoCategory = new ipg_taskcategory().Fake().WithName(TaskCategoryNames.MissingInformation);
            ipg_tasktype requestPatientInfo = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.REQUEST_PATIENT_INFORMATION)
                .WithDescription(@"Please provide the following Patient information: 
                                    < missing field value 1, missing field value 2, missing field value 3, etc.> ");

            Incident fakeIncident = new Incident().Fake()
                                                  .WithPatientReference(fakePatient)
                                                  .FakeWithPatientDemographics(fakePatient)
                                                  .FakeWithCptCode(fakeCpt)
                                                  .WithActualDos(DateTime.Now)
                                                  //FakeWithDxCode("Test Dx Code")  ------DO NOT Provide DX Code in Case .
                                                  .FakeWithPhysician();
            fakedContext.Initialize(new List<Entity> { fakePatient, fakeCpt, missingInfoCategory, requestPatientInfo, fakeIncident });
            
            var entityMetadata = new EntityMetadata()
            {
                LogicalName = Incident.EntityLogicalName
            };
            var dxcode = new StringAttributeMetadata()
            {
                SchemaName = Incident.Fields.ipg_DxCodeId1,
                LogicalName = Incident.Fields.ipg_DxCodeId1,
                DisplayName = new Label() { UserLocalizedLabel = new LocalizedLabel() {Label = "Dx Code Id 1" } }
            };

            entityMetadata.SetAttributeCollection(new[] { dxcode });

            fakedContext.InitializeMetadata(entityMetadata);

            var request = new ipg_IPGIntakeActionValidateMissingInformationRequest() { Target = fakeIncident.ToEntityReference() };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = request.RequestName,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = request.Parameters,
            };

            //ACT
            fakedContext.ExecutePluginWith<ValidateMissingInformation>(pluginContext);

            var context = new CrmServiceContext(fakedContext.GetOrganizationService());
            var taskCreated = context.TaskSet.FirstOrDefault();

            //ASSERT
            Assert.NotNull(taskCreated);
            Assert.True(taskCreated.RegardingObjectId.Id == fakeIncident.Id);
            Assert.Contains("Dx Code Id 1", taskCreated.Description);
        }

    }
}
