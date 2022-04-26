using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Repositories;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class InitPatientOutreachTaskOnStatementTest
    {
        [Fact]
        public void CheckTaskCreatedOnTaskStatementCreated()
        {
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();
            incident.ipg_DoNotContactPatient = false;
            ipg_documentcategorytype docCategory = new ipg_documentcategorytype().Fake("Patient Statement");
            ipg_documenttype docType = new ipg_documenttype().Fake("Patient Statement A2").WithDocumentCategory(docCategory);
            ipg_document A2Doc = new ipg_document().Fake().WithDocumentCategoryReference(docCategory).WithDocumentTypeReference(docType).WithCaseReference(incident);
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName("Patient Outreach");
            ipg_tasktype tasktype = new ipg_tasktype().Fake().WithName("Outgoing Call Required. <Level>").WithCategory(taskCategory);

            ipg_statementgenerationeventconfiguration configEvent = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.A2Generated);
            ipg_taskreasondetails taskReasonDetai = new ipg_taskreasondetails().Fake().WithTaskType(tasktype).WithOnStatementEvent(configEvent); 
            ipg_statementgenerationtask statementTask = new ipg_statementgenerationtask().Fake().WithCaseReference(incident).WithStatementEventConfig(configEvent);

            var listForInit = new List<Entity> { docType, docCategory, A2Doc, incident, tasktype, taskCategory, configEvent, taskReasonDetai };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", statementTask }
            };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
            };

            fakedContext.ExecutePluginWith<InitPatientOutreachTaskOnStatement>(pluginContext);

            using (CrmServiceContext context = new CrmServiceContext(fakedContext.GetOrganizationService()))
            {
                Task task = context.TaskSet.FirstOrDefault(x =>
                    x.RegardingObjectId.Id == incident.Id && x.ipg_tasktypeid.Id == tasktype.Id);

                Assert.NotNull(task);
                Assert.Equal(1, task.ipg_level);
            }
        }

        [Fact]
        public void CheckTaskCreatedOnContactpatientChange()
        {
            var fakedContext = new XrmFakedContext();


            Incident incident = new Incident().Fake().WithContactPatient(true).WithState(ipg_CaseStateCodes.PatientServices);
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName("Patient Outreach");
            ipg_tasktype tasktype = new ipg_tasktype().Fake().WithName("Outgoing Call Required. <Level>").WithCategory(taskCategory);

            ipg_statementgenerationeventconfiguration configEvent = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.A2Generated);
            ipg_taskreasondetails taskReasonDetai = new ipg_taskreasondetails().Fake().WithTaskType(tasktype).WithOnStatementEvent(configEvent);

            var listForInit = new List<Entity> { incident, tasktype, taskCategory, configEvent, taskReasonDetai };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", incident },
            };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection() { { "PreImage", new Incident() { Id = incident.Id, ipg_DoNotContactPatient = true } } },
            };

            fakedContext.ExecutePluginWith<InitPatientOutreachTaskOnStatement>(pluginContext);

            using (CrmServiceContext context = new CrmServiceContext(fakedContext.GetOrganizationService()))
            {
                Task task = context.TaskSet.FirstOrDefault(x =>
                    x.RegardingObjectId.Id == incident.Id && x.ipg_tasktypeid.Id == tasktype.Id);

                Assert.NotNull(task);
                Assert.Equal(1, task.ipg_level);
            }
        }

        [Fact]
        public void CheckTaskNotCreatedOnContactpatientChangeIfCaseNotInPatietnService()
        {
            var fakedContext = new XrmFakedContext();


            Incident incident = new Incident().Fake().WithContactPatient(true).WithState(ipg_CaseStateCodes.CaseManagement);
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName("Patient Outreach");
            ipg_tasktype tasktype = new ipg_tasktype().Fake().WithName("Outgoing Call Required. <Level>").WithCategory(taskCategory);

            ipg_statementgenerationeventconfiguration configEvent = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.A2Generated);
            ipg_taskreasondetails taskReasonDetai = new ipg_taskreasondetails().Fake().WithTaskType(tasktype).WithOnStatementEvent(configEvent);

            var listForInit = new List<Entity> { incident, tasktype, taskCategory, configEvent, taskReasonDetai };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", incident },
            };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection() { { "PreImage", new Incident() { Id = incident.Id, ipg_DoNotContactPatient = true } } },
            };

            fakedContext.ExecutePluginWith<InitPatientOutreachTaskOnStatement>(pluginContext);

            using (CrmServiceContext context = new CrmServiceContext(fakedContext.GetOrganizationService()))
            {
                Task task = context.TaskSet.FirstOrDefault(x =>
                    x.RegardingObjectId.Id == incident.Id && x.ipg_tasktypeid.Id == tasktype.Id);

                Assert.Null(task);
            }
        }

        [Fact]
        public void CheckTaskNotCreatedOnTaskStatementCreatedIfThereIsMorethenOneActiveA2PSDoc()
        {
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake().WithContactPatient(true);
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName("Patient Outreach");
            ipg_tasktype tasktype = new ipg_tasktype().Fake().WithName("Outgoing Call Required. <Level>").WithCategory(taskCategory);

            ipg_documentcategorytype category = new ipg_documentcategorytype().Fake("Patient Statement");
            ipg_documenttype type = new ipg_documenttype().Fake($"PST_{PSType.A2}").WithDocumentCategory(category);
            ipg_document A2doc = new ipg_document().Fake().WithDocumentCategoryReference(category).WithCaseReference(incident).WithDocumentTypeReference(type);
            ipg_document A2doc2 = new ipg_document().Fake().WithDocumentCategoryReference(category).WithCaseReference(incident).WithDocumentTypeReference(type);
            ipg_statementgenerationeventconfiguration configEvent = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.A2Generated);
            ipg_taskreasondetails taskReasonDetai = new ipg_taskreasondetails().Fake().WithTaskType(tasktype).WithOnStatementEvent(configEvent);
            ipg_statementgenerationtask statementTask = new ipg_statementgenerationtask().Fake().WithCaseReference(incident).WithStatementEventConfig(configEvent);

            var listForInit = new List<Entity> { A2doc2, category, type, A2doc, incident, tasktype, taskCategory, configEvent, taskReasonDetai };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", statementTask }
            };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
            };

            fakedContext.ExecutePluginWith<InitPatientOutreachTaskOnStatement>(pluginContext);

            using (CrmServiceContext context = new CrmServiceContext(fakedContext.GetOrganizationService()))
            {
                Task task = context.TaskSet.FirstOrDefault(x =>
                    x.RegardingObjectId.Id == incident.Id && x.ipg_tasktypeid.Id == tasktype.Id);

                Assert.Null(task);
            }
        }
    }
}
