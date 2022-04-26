using FakeXrmEasy;
using Insight.Intake.Plugin.Managers;
using Insight.Intake.Plugin.SLA;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.SLA
{
    public class CompleteDecisionCaseTest
    {
        [Fact]
        public void Close_task_when_referral_closed()
        {
            DateTime createdDate = new DateTime(2020, 09, 08, 12, 00, 00);
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Decision Retro Case")
                .WithTypeId(TaskManager.TaskTypeIds.SLA_Decision_Retro_Case)
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(3);

            ipg_referral retroReferral = new ipg_referral()
                .Fake(Guid.NewGuid())
                .WithReferralType(ipg_ReferralType.Retro)
                .WithCreatedOn(createdDate)
                .WithStateCode(ipg_referralState.Inactive)
                .Generate();

            Task slaRetroTask = new Task()
                .Fake()
                .WithSubject("SLA - Decision Retro Case")
                .WithRegarding(retroReferral.ToEntityReference())
                .WithTaskCategoryRef(taskCategory.ToEntityReference())
                .WithTypeRef(taskType.ToEntityReference())
                .WithScheduledEnd(createdDate.AddDays(3))
                .Generate();

            var fakedEntities = new List<Entity>() { retroReferral, slaRetroTask, taskCategory, taskType };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", retroReferral } };
            var preImage = new EntityImageCollection { { "PreImage", retroReferral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = retroReferral.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = preImage
            };

            fakedContext.ExecutePluginWith<CompleteDecisionCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var slaTask = context.TaskSet
                        .FirstOrDefault(t =>
                            t.RegardingObjectId.Id == retroReferral.Id && t.ipg_tasktypeid.Id == taskType.Id &&
                            t.StateCode.Value == TaskState.Completed);

                Assert.NotNull(slaTask);
                Assert.Equal(slaTask.ActualEnd <= slaTask.ScheduledEnd ? "SLA Met" : "SLA Not Met", slaTask.Subcategory);
                Assert.Equal(TaskState.Completed, slaTask.StateCode);
            }
        }

        [Fact]
        public void Close_task_when_case_closed()
        {
            DateTime createdDate = new DateTime(2020, 09, 08, 12, 00, 00);
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Decision Retro Case")
                .WithTypeId(TaskManager.TaskTypeIds.SLA_Decision_Retro_Case)
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(3);

            Incident incident = new Incident().Fake()
                .WithCaseStatus((int)ipg_CaseStatus.Closed)
                .WithReferralType((int)ipg_ReferralType.Retro);

            Task slaRetroTask = new Task()
                .Fake()
                .WithSubject("SLA - Decision Retro Case")
                .WithCase(incident.ToEntityReference())
                .WithRegarding(incident.ToEntityReference())
                .WithTaskCategoryRef(taskCategory.ToEntityReference())
                .WithTypeRef(taskType.ToEntityReference())
                .WithScheduledEnd(createdDate.AddDays(3))
                .Generate();

            var fakedEntities = new List<Entity>() { incident, slaRetroTask, taskCategory, taskType };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", incident } };
            var preImage = new EntityImageCollection { { "PreImage", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = incident.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = preImage
            };

            fakedContext.ExecutePluginWith<CompleteDecisionCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var slaTask = context.TaskSet
                        .FirstOrDefault(t =>
                            t.ipg_caseid.Id == incident.Id && t.ipg_tasktypeid.Id == taskType.Id &&
                            t.StateCode.Value == TaskState.Completed);

                Assert.NotNull(slaTask);
                Assert.Equal(slaTask.ActualEnd <= slaTask.ScheduledEnd ? "SLA Met" : "SLA Not Met", slaTask.Subcategory);
                Assert.Equal(TaskState.Completed, slaTask.StateCode);
            }
        }

        [Fact]
        public void Close_task_when_case_on_Case_Management_stage()
        {
            DateTime createdDate = new DateTime(2020, 09, 08, 12, 00, 00);
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Decision Retro Case")
                .WithTypeId(TaskManager.TaskTypeIds.SLA_Decision_Retro_Case)
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(3);

            Incident incident = new Incident().Fake()
                .WithState((int)ipg_CaseStateCodes.CaseManagement)
                .WithReferralType((int)ipg_ReferralType.Retro);

            Task slaRetroTask = new Task()
                .Fake()
                .WithSubject("SLA - Decision Retro Case")
                .WithCase(incident.ToEntityReference())
                .WithRegarding(incident.ToEntityReference())
                .WithTaskCategoryRef(taskCategory.ToEntityReference())
                .WithTypeRef(taskType.ToEntityReference())
                .WithScheduledEnd(createdDate.AddDays(3))
                .Generate();

            var fakedEntities = new List<Entity>() { incident, slaRetroTask, taskCategory, taskType };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", incident } };
            var preImage = new EntityImageCollection { { "PreImage", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = incident.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = preImage
            };

            fakedContext.ExecutePluginWith<CompleteDecisionCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var slaTask = context.TaskSet
                        .FirstOrDefault(t =>
                            t.ipg_caseid.Id == incident.Id && t.ipg_tasktypeid.Id == taskType.Id &&
                            t.StateCode.Value == TaskState.Completed);

                Assert.NotNull(slaTask);
                Assert.Equal(slaTask.ActualEnd <= slaTask.ScheduledEnd ? "SLA Met" : "SLA Not Met", slaTask.Subcategory);
                Assert.Equal(TaskState.Completed, slaTask.StateCode);
            }
        }
    }
}
