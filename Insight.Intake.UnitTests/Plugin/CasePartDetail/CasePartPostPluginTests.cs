using FakeXrmEasy;
using Insight.Intake.Plugin.CasePartDetail;
using Insight.Intake.Plugin.Managers;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.CasePartDetail
{
    public class CasePartPostPluginTests: PluginTestsBase
    {

        [Fact]
        public void CheckCreatingShellPartGeneratesTask()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account manufacturer = new Intake.Account().Fake();
            Intake.Account facility = new Intake.Account().Fake();

            ipg_facilitymanufacturerrelationship facilitymfg = new ipg_facilitymanufacturerrelationship()
                .Fake()
                .WithFacilityReference(facility)
                .WithManufacturerReference(manufacturer);

            Intake.Product product = new Intake.Product().Fake()
                                        .RuleFor(p => p.ipg_manufacturerid, p => manufacturer.ToEntityReference())
                                        .RuleFor(p => p.ipg_statusEnum, p => Product_ipg_status.Pending);
            
            Intake.Incident incident = new Incident().Fake().WithFacilityReference(facility);

            Intake.ipg_casepartdetail casePart = new Intake.ipg_casepartdetail().Fake()
                                                        .WithProductReference(product)
                                                        .WithCaseReference(incident)
                                                        .RuleFor(cp=>cp.StatusCodeEnum, cp=> ipg_casepartdetail_StatusCode.Active);

            Team team = new Team() { Id = Guid.NewGuid(), Name = "Material manager" };
            ipg_tasktype taskType2 = new ipg_tasktype().Fake().WithTypeId(TaskManager.TaskTypeIds.NEW_PART_REQUEST);

            var listForInit = new List<Entity>() { product, casePart, incident, manufacturer, team, facility, facilitymfg, taskType2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", casePart } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_casepartdetail.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { },
                PreEntityImages = new EntityImageCollection() { }
            };

            fakedContext.ExecutePluginWith<CasePartPostPlugin>(pluginContext);

            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());

            var task = (from t in crmContext.CreateQuery<Task>()
                        where t.RegardingObjectId.Id == manufacturer.Id
                        select t).FirstOrDefault();

            Assert.NotNull(task);
            Assert.Equal(taskType2.ToEntityReference(), task.ipg_tasktypeid);
        }

        [Fact]
        public void CheckCreatingMissinFacilityMFGTask()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account manufacturer = new Intake.Account().Fake();
            Intake.Account facility = new Intake.Account().Fake();

            ipg_tasktype taskType = new ipg_tasktype().Fake().WithTypeId(TaskManager.TaskTypeIds.MISSING_FACILITY_MFG);
            ipg_tasktype taskType2 = new ipg_tasktype().Fake().WithTypeId(TaskManager.TaskTypeIds.NEW_PART_REQUEST);

            Intake.Product product = new Intake.Product().Fake()
                                        .RuleFor(p => p.ipg_manufacturerid, p => manufacturer.ToEntityReference())
                                        .RuleFor(p => p.ipg_statusEnum, p => Product_ipg_status.Pending);

            Intake.Incident incident = new Incident().Fake().WithFacilityReference(facility);

            Intake.ipg_casepartdetail casePart = new Intake.ipg_casepartdetail().Fake()
                                                        .WithProductReference(product)
                                                        .WithCaseReference(incident)
                                                        .WithPOType((int)ipg_PurchaseOrderTypes.MPO)
                                                        .RuleFor(cp => cp.StatusCodeEnum, cp => ipg_casepartdetail_StatusCode.Active);

            Team team = new Team() { Id = Guid.NewGuid(), Name = "Material manager" };

            var listForInit = new List<Entity>() { product, casePart, incident, manufacturer, team, facility, taskType, taskType2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", casePart } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_casepartdetail.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { },
                PreEntityImages = new EntityImageCollection() { }
            };

            fakedContext.ExecutePluginWith<CasePartPostPlugin>(pluginContext);

            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());

            var task = (from t in crmContext.CreateQuery<Task>()
                        where t.RegardingObjectId.Id == manufacturer.Id
                        select t).FirstOrDefault();
            
            var missignFacilityMFGTask = (from t in crmContext.CreateQuery<Task>()
                                          where t.ipg_tasktypeid.Id == taskType.Id
                                          select t).FirstOrDefault();

            Assert.NotNull(missignFacilityMFGTask);
        }
    }
}
