using FakeXrmEasy;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class PopulateSoonesTasksOnCaseTest
    {
        [Fact]
        public void FirstMissingInformationCategoryTasksCreated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory missingInfoCategory = new ipg_taskcategory()
                .Fake(CaseSoonestTasksHelper.MissingInformationCategory)
                    .WithName("Missing Information")
                .Generate();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            Task target = new Task()
                .Fake()
                    .WithScheduledEnd(new DateTime(2020, 12, 01, 12, 00, 00))
                    .WithCase(incident.ToEntityReference())
                    .WithTaskCategory(missingInfoCategory.ToEntityReference())
                    .WithState(TaskState.Open)
                .Generate();

            var fakedEntities = new List<Entity>() { incident, missingInfoCategory, target };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<PopulateSoonestTasksOnCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                incident = context.IncidentSet.FirstOrDefault(x => x.Id == incident.Id);

                Assert.NotNull(incident.ipg_missinginformationcategorytaskid);
                Assert.Equal(target.Id, incident.ipg_missinginformationcategorytaskid.Id);
            }
        }

        [Fact]
        public void SecondMissingInformationCategoryTasksCreated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory missingInfoCategory = new ipg_taskcategory()
                .Fake(CaseSoonestTasksHelper.MissingInformationCategory)
                    .WithName("Missing Information")
                .Generate();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            Task task = new Task()
                .Fake()
                    .WithScheduledEnd(new DateTime(2020, 12, 01, 12, 00, 00))
                    .WithCase(incident.ToEntityReference())
                    .WithTaskCategory(missingInfoCategory.ToEntityReference())
                    .WithState(TaskState.Open)
                .Generate();

            incident.ipg_missinginformationcategorytaskid = task.ToEntityReference();

            Task target = new Task()
                .Fake()
                    .WithScheduledEnd(new DateTime(2020, 11, 01, 12, 00, 00))
                    .WithCase(incident.ToEntityReference())
                    .WithTaskCategory(missingInfoCategory.ToEntityReference())
                    .WithState(TaskState.Open)
                .Generate();

            var fakedEntities = new List<Entity>() { incident, missingInfoCategory, task, target };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<PopulateSoonestTasksOnCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                incident = context.IncidentSet.FirstOrDefault(x => x.Id == incident.Id);

                Assert.NotNull(incident.ipg_missinginformationcategorytaskid);
                Assert.Equal(target.Id, incident.ipg_missinginformationcategorytaskid.Id);
            }
        }

        [Fact]
        public void MissingInformationCategoryTasksUpdated()
        {
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory missingInfoCategory = new ipg_taskcategory()
                .Fake(CaseSoonestTasksHelper.MissingInformationCategory)
                    .WithName("Missing Information")
                .Generate();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            Task preImage = new Task()
                .Fake()
                    .WithScheduledEnd(new DateTime(2020, 12, 01, 12, 00, 00))
                    .WithCase(incident.ToEntityReference())
                    .WithTaskCategory(missingInfoCategory.ToEntityReference())
                    .WithState(TaskState.Open)
                .Generate();

            Task task = new Task()
                .Fake()
                    .WithScheduledEnd(new DateTime(2020, 12, 01, 12, 00, 00))
                    .WithCase(incident.ToEntityReference())
                    .WithTaskCategory(missingInfoCategory.ToEntityReference())
                    .WithState(TaskState.Completed)
                .Generate();

            incident.ipg_missinginformationcategorytaskid = task.ToEntityReference();

            Task target = new Task()
                .Fake(task.Id)
                    .WithState(TaskState.Completed)
                .Generate();

            var fakedEntities = new List<Entity>() { incident, missingInfoCategory, task };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection() { new KeyValuePair<string, Entity>("PreImage", preImage) }
            };

            fakedContext.ExecutePluginWith<PopulateSoonestTasksOnCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                incident = context.IncidentSet.FirstOrDefault(x => x.Id == incident.Id);

                Assert.Null(incident.ipg_missinginformationcategorytaskid);
            }
        }

        [Fact]
        public void MissingInformationCategoryTaskDeleted()
        {
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory missingInfoCategory = new ipg_taskcategory()
                .Fake(CaseSoonestTasksHelper.MissingInformationCategory)
                    .WithName("Missing Information")
                .Generate();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            Task preImage = new Task()
                .Fake()
                    .WithScheduledEnd(new DateTime(2020, 12, 01, 12, 00, 00))
                    .WithCase(incident.ToEntityReference())
                    .WithTaskCategory(missingInfoCategory.ToEntityReference())
                    .WithState(TaskState.Open)
                .Generate();

            Task task = new Task()
                .Fake()
                    .WithScheduledEnd(new DateTime(2020, 12, 01, 12, 00, 00))
                    .WithCase(incident.ToEntityReference())
                    .WithTaskCategory(missingInfoCategory.ToEntityReference())
                    .WithState(TaskState.Open)
                .Generate();

            incident.ipg_missinginformationcategorytaskid = task.ToEntityReference();

            Task task2 = new Task()
                .Fake()
                    .WithScheduledEnd(new DateTime(2020, 11, 01, 12, 00, 00))
                    .WithCase(incident.ToEntityReference())
                    .WithTaskCategory(missingInfoCategory.ToEntityReference())
                    .WithState(TaskState.Open)
                .Generate();

            var fakedEntities = new List<Entity>() { incident, missingInfoCategory, task, task2 };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", task.ToEntityReference() } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Delete",
                Stage = 40,
                PrimaryEntityName = task.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection() { new KeyValuePair<string, Entity>("PreImage", preImage) }
            };

            fakedContext.ExecutePluginWith<PopulateSoonestTasksOnCase>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                incident = context.IncidentSet.FirstOrDefault(x => x.Id == incident.Id);

                Assert.NotNull(incident.ipg_missinginformationcategorytaskid);
                Assert.Equal(task2.Id, incident.ipg_missinginformationcategorytaskid.Id);
            }
        }
    }
}
