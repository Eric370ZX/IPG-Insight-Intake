using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class CreateTissueRequestFormTaskTest
    {
        [Fact]
        public void TestCreate()
        {
            var fakedContext = new XrmFakedContext();

            ipg_globalsetting trfSettings = new ipg_globalsetting()
                .Fake(
                    "TissueRequestForm_CPTCodes",
                    "20610,20900,20924,21230,24341,24342,25337,27415,27422,27427,29805,29823,29826,29867,29870,29874,29876,29877,29879,29880,29881,29882,29886,29888,29889,30450,30465,65426,65710,65730,65750,65755,65756,66250,76000,0232T")
                .Generate();


            ipg_cptcode trfCptCode = new ipg_cptcode()
                .Fake()
                    .WithCode("20610")
                .Generate();

            ipg_taskcategory taskCategory = new ipg_taskcategory()
                .Fake()
                    .WithName("Missing Information")
                .Generate();

            ipg_tasktype taskType = new ipg_tasktype()
                .Fake()
                    .WithName("Missing Tissue Request Form")
                .Generate();

            Incident target = new Incident()
                .Fake()
                    .FakeWithCptCode(trfCptCode)
                .Generate();

            var fakedEntities = new List<Entity>() { trfCptCode, target, taskCategory, taskType, trfSettings };

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

            fakedContext.ExecutePluginWith<CreateTissueRequestFormTask>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                Task task = context.TaskSet.FirstOrDefault(x => 
                    x.ipg_caseid.Id == target.Id && x.ipg_taskcategoryid.Id == taskCategory.Id &&
                    x.ipg_tasktypeid.Id == taskType.Id);

                Assert.NotNull(task);
                Assert.Equal(TaskState.Open, task.StateCode.Value);
            }
        }

        [Fact]
        public void TestUpdate_TrfNeeded()
        {
            var fakedContext = new XrmFakedContext();

            ipg_globalsetting trfSettings = new ipg_globalsetting()
                .Fake(
                    "TissueRequestForm_CPTCodes",
                    "20610,20900,20924,21230,24341,24342,25337,27415,27422,27427,29805,29823,29826,29867,29870,29874,29876,29877,29879,29880,29881,29882,29886,29888,29889,30450,30465,65426,65710,65730,65750,65755,65756,66250,76000,0232T")
                .Generate();


            ipg_cptcode trfCptCode = new ipg_cptcode()
                .Fake()
                    .WithCode("20610")
                .Generate();

            ipg_cptcode notTrfCptCode = new ipg_cptcode()
                .Fake()
                    .WithCode("66666")
                .Generate();

            ipg_taskcategory taskCategory = new ipg_taskcategory()
                .Fake()
                    .WithName("Missing Information")
                .Generate();

            ipg_tasktype taskType = new ipg_tasktype()
                .Fake()
                    .WithName("Missing Tissue Request Form")
                .Generate();

            Incident incident = new Incident()
                .Fake()
                    .FakeWithCptCode(trfCptCode)
                .Generate();

            Incident target = new Incident()
                .Fake(incident.Id)
                    .FakeWithCptCode(trfCptCode)
                .Generate();

            var fakedEntities = new List<Entity>() { notTrfCptCode, trfCptCode, incident, taskCategory, taskType, trfSettings };

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
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CreateTissueRequestFormTask>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                Task task = context.TaskSet.FirstOrDefault(x =>
                    x.ipg_caseid.Id == target.Id && x.ipg_taskcategoryid.Id == taskCategory.Id &&
                    x.ipg_tasktypeid.Id == taskType.Id);

                Assert.NotNull(task);
                Assert.Equal(TaskState.Open, task.StateCode.Value);
            }
        }

        [Fact]
        public void TestUpdate_TrfNotNeeded()
        {
            var fakedContext = new XrmFakedContext();

            ipg_globalsetting trfSettings = new ipg_globalsetting()
                .Fake(
                    "TissueRequestForm_CPTCodes",
                    "20610,20900,20924,21230,24341,24342,25337,27415,27422,27427,29805,29823,29826,29867,29870,29874,29876,29877,29879,29880,29881,29882,29886,29888,29889,30450,30465,65426,65710,65730,65750,65755,65756,66250,76000,0232T")
                .Generate();

            ipg_cptcode trfCptCode = new ipg_cptcode()
                .Fake()
                    .WithCode("20610")
                .Generate();

            ipg_cptcode notTrfCptCode = new ipg_cptcode()
                .Fake()
                    .WithCode("66666")
                .Generate();

            ipg_taskcategory taskCategory = new ipg_taskcategory()
                .Fake()
                    .WithName("Missing Information")
                .Generate();

            ipg_tasktype taskType = new ipg_tasktype()
                .Fake()
                    .WithName("Missing Tissue Request Form")
                .Generate();

            Incident incident = new Incident()
                .Fake()
                    .FakeWithCptCode(notTrfCptCode)
                .Generate();

            Incident target = new Incident()
                .Fake(incident.Id)
                    .FakeWithCptCode(notTrfCptCode)
                .Generate();

            Task trfNeededTask = new Task()
                .FakeTissueRequestForm(incident.Id, taskCategory.ToEntityReference(), taskType.ToEntityReference())
                .Generate();

            var fakedEntities = new List<Entity>() { notTrfCptCode, trfCptCode, incident, taskCategory, taskType, trfSettings, trfNeededTask };

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
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CreateTissueRequestFormTask>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                Task task = context.TaskSet.FirstOrDefault(x => x.Id == trfNeededTask.Id);

                Assert.NotNull(task);
                Assert.Equal(TaskState.Completed, task.StateCode.Value);
            }
        }
    }
}
