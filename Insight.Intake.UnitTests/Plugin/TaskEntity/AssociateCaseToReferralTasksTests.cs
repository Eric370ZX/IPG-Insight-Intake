using FakeXrmEasy;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class AssociateCaseToReferralTasksTests : PluginTestsBase
    {
        [Fact]
        public void Returns_true_when_case_associates_to_referral_tasks()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create a fake case, tasks and referral. Bind the referral with the tasks and case.
            ipg_referral referral = new ipg_referral().Fake();
            Task task1 = new Task().Fake().WithReferral(referral.ToEntityReference());
            Task task2 = new Task().Fake().WithReferral(referral.ToEntityReference());
            Incident createdCase = new Incident().Fake().WithReferral(referral.ToEntityReference());

            var listForInit = new List<Entity> { referral, task1, task2, createdCase };

            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdCase } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<AssociateCaseToReferralTasks>(pluginContext);

            #endregion

            #region Asserts

            var fakedService = fakedContext.GetOrganizationService();

            var query = new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression( Task.Fields.ipg_caseid, ConditionOperator.Equal, createdCase.Id)
                    }
                }
            };
            //retrieve all related documents to Referral
            var relatedTasks = fakedService.RetrieveMultiple(query).Entities;

            Assert.True(relatedTasks.Count == 2);

            #endregion
        }
    }
}
