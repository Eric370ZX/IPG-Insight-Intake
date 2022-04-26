using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class HoldCaseActionPluginTests : PluginTestsBase
    {
        [Fact]
        public void Returns_true_when_case_on_hold_with_created_hold_task()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create case and hold task configurations, bind them
            Incident incident = new Incident()
                .Fake()
                .WithState((int)ipg_CaseStateCodes.CarrierServices);

            ipg_taskcategory taskCategory = new ipg_taskcategory()
                .Fake()
                .WithName("Carrier Services");
            ipg_tasktype taskType = new ipg_tasktype()
                .Fake()
                .WithCategory(taskCategory)
                .WithName("Request for Manager Review")
                .WithTypeId(TaskTypeIds.REQUEST_FOR_MANAGER_REVIEW)
                .WithDescription("Review this Case for {0} to determine resolution.");
            ipg_caseholdconfiguration caseHoldConfiguration = new ipg_caseholdconfiguration()
                .Fake()
                .WithTaskId("1823")
                .WithCaseState(ipg_CaseStateCodes.CarrierServices)
                .WithHoldReason(ipg_Caseholdreason.ManagerReview);

            var listForInit = new List<Entity> { incident, taskCategory, taskType, caseHoldConfiguration };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGCaseActionsHoldCase",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident.ToEntityReference() }, { "HoldReason", new OptionSetValue(427880006) },
                                                              { "IsOnHold", true }, { "HoldNote", "test" } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<HoldCaseActionPlugin>(pluginContext);
            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();

            var holdTask = fakedService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, incident.Id)
                    }
                }
            }).Entities.FirstOrDefault()?.ToEntity<Task>();
            var updatedIncident = fakedService.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.Equal(true, updatedIncident.ipg_casehold);
            Assert.Equal((int)ipg_Caseholdreason.ManagerReview, updatedIncident.ipg_caseholdreason.Value);
            #endregion
        }

        [Fact]
        public void Returns_true_when_case_unhold()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create case
            Incident incident = new Incident()
                .Fake()
                .WithState((int)ipg_CaseStateCodes.CarrierServices);

            var listForInit = new List<Entity> { incident };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGCaseActionsHoldCase",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident.ToEntityReference() }, { "HoldReason", new OptionSetValue(427880006) },
                                                              { "IsOnHold", false }, { "HoldNote", "test" } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<HoldCaseActionPlugin>(pluginContext);
            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();

            var updatedIncident = fakedService.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.Equal(false, updatedIncident.ipg_casehold);
            #endregion
        }
    }
}
