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
    public class DecisionCaseTest
    {
        [Fact]
        public void CheckDecisionStatCase()
        {
            DateTime createdDate = new DateTime(2020, 09, 08, 12, 00, 00);
            var fakedContext = new XrmFakedContext();

            ipg_referral statReferral = new ipg_referral()
                .Fake(Guid.NewGuid())
                .WithReferralType(ipg_ReferralType.Stat)
                .WithOutcomeCode(ipg_OutcomeCodes.Rejected)
                .WithCaseStatus(ipg_CaseStatus.Closed)
                .WithCreatedOn(createdDate);

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Decision Stat Case")
                .WithTypeId(TaskManager.TaskTypeIds.SLA_Decision_Stat_Case)
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(1);

            var fakedEntities = new List<Entity>() { statReferral, taskCategory, taskType };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", statReferral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = statReferral.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", new ipg_referral() { Id = statReferral.Id, ipg_casestatusEnum = ipg_CaseStatus.Open, ipg_referraltype = statReferral.ipg_referraltype } } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", statReferral } }
            };

            fakedContext.ExecutePluginWith<DecisionCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var task = context.TaskSet.FirstOrDefault(x => x.RegardingObjectId != null && x.RegardingObjectId.Id == statReferral.Id);

                Assert.Equal(taskType.Id, task.ipg_tasktypeid?.Id);
                Assert.Equal(TaskState.Open, task.StateCode);
            }
        }

        [Fact]
        public void CheckDecisionUrgentCase()
        {
            DateTime createdDate = new DateTime(2020, 09, 08, 12, 00, 00);
            var fakedContext = new XrmFakedContext();

            ipg_referral urgentReferral = new ipg_referral()
                .Fake(Guid.NewGuid())
                .WithReferralType(ipg_ReferralType.Urgent)
                .WithOutcomeCode(ipg_OutcomeCodes.Rejected)
                .WithCaseStatus(ipg_CaseStatus.Closed)
                .WithCreatedOn(createdDate);

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Decision Urgent Case")
                .WithTypeId(TaskManager.TaskTypeIds.SLA_Decision_Urgent_Case)
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(1);

            var fakedEntities = new List<Entity>() { urgentReferral, taskCategory, taskType };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", urgentReferral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = urgentReferral.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", new ipg_referral() { Id = urgentReferral.Id, ipg_casestatusEnum = ipg_CaseStatus.Open, ipg_referraltype = urgentReferral.ipg_referraltype } } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", urgentReferral } }
            };

            fakedContext.ExecutePluginWith<DecisionCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var task = context.TaskSet.FirstOrDefault(x => x.RegardingObjectId != null && x.RegardingObjectId.Id == urgentReferral.Id);

                Assert.Equal(taskType.Id, task.ipg_tasktypeid?.Id);
                Assert.Equal(TaskState.Open, task.StateCode);
            }
        }

        [Fact]
        public void CheckDecisionStandardCase()
        {
            DateTime createdDate = new DateTime(2020, 09, 08, 12, 00, 00);
            var fakedContext = new XrmFakedContext();

            ipg_referral standardReferral = new ipg_referral()
                .Fake(Guid.NewGuid())
                .WithReferralType(ipg_ReferralType.Standard)
                .WithOutcomeCode(ipg_OutcomeCodes.Rejected)
                .WithCaseStatus(ipg_CaseStatus.Closed)
                .WithCreatedOn(createdDate);

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Decision Standard Case")
                .WithTypeId(TaskManager.TaskTypeIds.SLA_Decision_Standard_Case)
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(3);

            var fakedEntities = new List<Entity>() { standardReferral, taskCategory, taskType };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", standardReferral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = standardReferral.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", new ipg_referral() { Id = standardReferral.Id, ipg_casestatusEnum = ipg_CaseStatus.Open, ipg_referraltype = standardReferral.ipg_referraltype } } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", standardReferral } }
            };

            fakedContext.ExecutePluginWith<DecisionCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var task = context.TaskSet.FirstOrDefault(x => x.RegardingObjectId != null && x.RegardingObjectId.Id == standardReferral.Id);

                Assert.Equal(taskType.Id, task.ipg_tasktypeid?.Id);
                Assert.Equal(TaskState.Open, task.StateCode);
            }
        }

        [Fact]
        public void CheckDecisionRetroCase()
        {
            DateTime createdDate = new DateTime(2020, 09, 08, 12, 00, 00);
            var fakedContext = new XrmFakedContext();

            ipg_referral retroReferral = new ipg_referral()
                .Fake(Guid.NewGuid())
                .WithReferralType(ipg_ReferralType.Retro)
                .WithCaseStatus(ipg_CaseStatus.Closed)
                .WithOutcomeCode(ipg_OutcomeCodes.Rejected)
                .WithCreatedOn(createdDate);

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Decision Retro Case")
                .WithTypeId(TaskManager.TaskTypeIds.SLA_Decision_Retro_Case)
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(1)
                .Generate();

            var fakedEntities = new List<Entity>() { retroReferral, taskCategory, taskType };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", retroReferral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = retroReferral.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", new ipg_referral() {Id = retroReferral.Id, ipg_casestatusEnum = ipg_CaseStatus.Open, ipg_referraltype = retroReferral.ipg_referraltype } } },
                PreEntityImages = new EntityImageCollection() { {"PreImage",retroReferral} }
            };

            fakedContext.ExecutePluginWith<DecisionCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var task = context.TaskSet.FirstOrDefault(x => x.RegardingObjectId != null && x.RegardingObjectId.Id == retroReferral.Id);

                Assert.Equal(taskType.Id, task.ipg_tasktypeid?.Id);
                Assert.Equal(TaskState.Open, task.StateCode);
            }
        }
    }
}
