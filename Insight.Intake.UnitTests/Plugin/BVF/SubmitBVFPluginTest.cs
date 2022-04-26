using FakeXrmEasy;
using Insight.Intake.Plugin.BVF;
using Insight.Intake.UnitTests.Fakes;
using System.Collections.Generic;
using Xunit;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;
using Moq;
using Insight.Intake.Plugin.Common.Interfaces;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.UnitTests.Plugin.BVF
{
    public class SubmitBVFPluginTest: PluginTestsBase
    {
        const string REASON = "BVF Completed successfully";

        [Fact]
        public void RecreatesIndividualBenefit()
        {
            TestBenefitRecreation(ipg_BenefitCoverageLevels.Individual);
        }

        [Fact]
        public void RecreatesFamilyBenefit()
        {
            TestBenefitRecreation(ipg_BenefitCoverageLevels.Family);
        }

        [Fact]
        public void CheckThatTaskCompleted()
        {
            //ARRANGE
            
            var fakedContext = new XrmFakedContext();

            Intake.Incident incident = new Incident().Fake();
            Intake.Task task = new Task().Fake().WithType(ipg_TaskType1.ManualBVF).WithRegarding(incident.ToEntityReference());
            var carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier).Generate();
            Intake.ipg_benefitsverificationform bvf = new ipg_benefitsverificationform().Fake()
                .WithFormType(ipg_benefitsverificationform_ipg_formtype.Auto)
                .WithCase(incident.ToEntityReference())
                .WithCarrierReference(carrier.ToEntityReference())
                .WithMemberId();

            var listForInit = new List<Entity> { incident, task };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", bvf } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = bvf.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            var caseBenefitSwitcherMock = new Mock<ICaseBenefitSwitcher>();

            //ACT
            fakedContext.ExecutePluginWith(pluginContext, new SubmitBVFPlugin(caseBenefitSwitcherMock.Object));

            //ASSERT

            caseBenefitSwitcherMock.Verify(s => s.UpdateInOutNetwork(incident.Id, carrier.Id));

            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());
            var completedTask = GetCompletedBVFTask(crmContext, incident.ToEntityReference());
            Assert.True(completedTask?.Id == task.Id, "Task not completed"); 
        }

        [Fact]
        public void CheckThatTaskCreatedAsCompleted()
        {
            //ARRANGE
            
            var fakedContext = new XrmFakedContext();

            Intake.Incident incident = new Incident().Fake();
            var carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier).Generate();
            Intake.ipg_benefitsverificationform bvf = new ipg_benefitsverificationform().Fake()
                .WithFormType(ipg_benefitsverificationform_ipg_formtype.Auto)
                .WithCase(incident.ToEntityReference())
                .WithCarrierReference(carrier.ToEntityReference())
                .WithMemberId();

            var listForInit = new List<Entity> { incident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", bvf } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = bvf.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            var caseBenefitSwitcherMock = new Mock<ICaseBenefitSwitcher>();

            //ACT
            fakedContext.ExecutePluginWith(pluginContext, new SubmitBVFPlugin(caseBenefitSwitcherMock.Object));

            //ASSERT

            caseBenefitSwitcherMock.Verify(s => s.UpdateInOutNetwork(incident.Id, carrier.Id));

            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());
            var completedTask = GetCompletedBVFTask(crmContext, incident.ToEntityReference());
            Assert.True(completedTask != null, "Task not completed");
        }

        private Task GetCompletedBVFTask(OrganizationServiceContext crmContext, EntityReference caseRef)
        {
            return (from task in crmContext.CreateQuery<Task>()
                           where task.RegardingObjectId.Equals(caseRef)
                                 && task.ipg_tasktypecodeEnum == ipg_TaskType1.ManualBVF
                                 && task.StatusCode.Value == (int)Task_StatusCode.Resolved
                                 && task.StateCode.Value == TaskState.Completed
                                 && task.ipg_closurenote == REASON
                    select task).FirstOrDefault();
        }

        private void TestBenefitRecreation(ipg_BenefitCoverageLevels coverageLevel)
        {
            //ARRANGE

            var incident = new Incident().Fake().WithMemberId("XYZ12345").Generate();
            var carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier).Generate();
            var bvf = new ipg_benefitsverificationform().Fake()
                .WithFormType(ipg_benefitsverificationform_ipg_formtype.GeneralHealth)
                .WithCase(incident.ToEntityReference())
                .WithCarrierReference(carrier.ToEntityReference())
                .WithMemberId(incident.ipg_MemberIdNumber)
                .WithBenefit(ipg_BenefitType.HealthBenefitPlanCoverage, ipg_inn_or_oon.INN, DateTime.Now.AddDays(-200), DateTime.Now.AddDays(200), coverageLevel, coverageLevel)
                .Generate();
            var oldBenefit = new ipg_benefit().Fake()
                .WithCaseReference(incident)
                .WithCarrierReference(carrier)
                .WithMemberId(incident.ipg_MemberIdNumber)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithInOutNetwork(true)
                .WithCoverageLevel(coverageLevel)
                .WithIndividualBenefits()
                .Generate();

            var listForInit = new List<Entity> { incident, carrier, bvf, oldBenefit };
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", bvf } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = bvf.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            var caseBenefitSwitcherMock = new Mock<ICaseBenefitSwitcher>();

            //ACT
            fakedContext.ExecutePluginWith(pluginContext, new SubmitBVFPlugin(caseBenefitSwitcherMock.Object));

            //ASSERT

            var orgService = fakedContext.GetOrganizationService();

            var benefits = GetBenefits(orgService, incident);
            Assert.Single(benefits);
            var newBenefit = benefits.FirstOrDefault();
            Assert.NotEqual(oldBenefit.Id, newBenefit.Id);
            VerifyBenefit(bvf, newBenefit);
        }

        private IEnumerable<ipg_benefit> GetBenefits(IOrganizationService orgService, Incident incident)
        {
            return orgService.RetrieveMultiple(new QueryExpression
            {
                EntityName = ipg_benefit.EntityLogicalName,
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_benefit.Fields.ipg_CaseId, ConditionOperator.Equal, incident.Id),
                        new ConditionExpression(ipg_benefit.Fields.StateCode, ConditionOperator.Equal, (int)ipg_benefitState.Active)
                    }
                }

            }).Entities.Select(e => e.ToEntity<ipg_benefit>());
        }

        private void VerifyBenefit(ipg_benefitsverificationform bvf, ipg_benefit benefit)
        {
            Assert.Equal(bvf.ipg_BenefitTypeCodeEnum, benefit.ipg_BenefitTypeEnum);
            Assert.True(benefit.ipg_InOutNetwork);
            Assert.Equal(bvf.ipg_coverageeffectivedate, benefit.ipg_EligibilityStartDate);
            Assert.Equal(bvf.ipg_coverageexpirationdate, benefit.ipg_EligibilityEndDate);
            Assert.Equal(bvf.ipg_deductible, benefit.ipg_Deductible);
            Assert.Equal(bvf.ipg_deductiblemet, benefit.ipg_DeductibleMet);
            Assert.Equal(bvf.ipg_oopmax, benefit.ipg_MemberOOPMax);
            Assert.Equal(bvf.ipg_oopmaxmet, benefit.ipg_MemberOOPMet);
        }
    }
}
