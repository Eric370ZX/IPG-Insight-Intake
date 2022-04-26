using FakeXrmEasy;
using Insight.Intake.Plugin.Gating.PostProcess;
using Insight.Intake.Repositories;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.UnitTests.Plugin.Gating.PostProcess
{
    public class Gate11Tests
    {
        [Fact]
        public void Gate11Tests_CarrierPaymentStatementGenerationTaskCreated()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            ipg_statementgenerationeventconfiguration taskconfigCarrierPayment = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment);
            ipg_statementgenerationeventconfiguration taskconfigA2 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.A2Generated);

            ipg_statementgenerationtask statementgenerationtask = new ipg_statementgenerationtask().Fake().WithCaseReference(caseEntity);
            var listForInit = new List<Entity>() { caseEntity, taskconfigCarrierPayment, taskconfigA2, statementgenerationtask };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "MinimumSeverityLevel", (int)ipg_SeverityLevel.Warning } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate11",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Gate11>(pluginContext);

            var service = fakedContext.GetOrganizationService();
            var context = new CrmServiceContext(service);

            var PSTask = (from pstasks in context.ipg_statementgenerationtaskSet
                          join configevent in context.ipg_statementgenerationeventconfigurationSet on pstasks.ipg_eventid.Id equals configevent.ipg_statementgenerationeventconfigurationId
                          where pstasks.ipg_caseid.Id == caseEntity.Id && configevent.ipg_name == PSEvents.CarrierPayment
                          select pstasks).FirstOrDefault();
            var statementgenerationtaskUpdated = service.Retrieve(statementgenerationtask.LogicalName, statementgenerationtask.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true)).ToEntity<ipg_statementgenerationtask>();
            //Assert
            Assert.NotNull(PSTask);

            Assert.Equal(ipg_statementgenerationtaskState.Inactive, statementgenerationtaskUpdated.StateCode);
            Assert.Equal(ipg_statementgenerationtask_StatusCode.Canceled, statementgenerationtaskUpdated.StatusCodeEnum);
        }

        [Fact]
        public void Gate11Tests_CarrierPaymentStatementGenerationTaskNotCreated_IfA2DocActive()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            ipg_statementgenerationeventconfiguration taskconfigCarrierPayment = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment);
            ipg_statementgenerationeventconfiguration taskconfigA2 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.A2Generated);

            ipg_statementgenerationtask statementgenerationtask = new ipg_statementgenerationtask().Fake().WithCaseReference(caseEntity);
            ipg_documentcategorytype docategory = new ipg_documentcategorytype().Fake(DocumentCategioryNames.PatientStatement);
            ipg_documenttype A2Doc = new ipg_documenttype().Fake("Patient Statement A2").WithDocumentCategory(docategory);
            ipg_document Psdoc = new ipg_document().Fake().WithDocumentCategoryReference(docategory).WithDocumentTypeReference(A2Doc).WithCaseReference(caseEntity);

            var listForInit = new List<Entity>() { docategory, A2Doc, Psdoc, caseEntity, taskconfigCarrierPayment, taskconfigA2, statementgenerationtask };

         

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "MinimumSeverityLevel", (int)ipg_SeverityLevel.Warning } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate11",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Gate11>(pluginContext);

            var service = fakedContext.GetOrganizationService();
            var context = new CrmServiceContext(service);

            var PSTask = (from pstasks in context.ipg_statementgenerationtaskSet
                          join configevent in context.ipg_statementgenerationeventconfigurationSet on pstasks.ipg_eventid.Id equals configevent.ipg_statementgenerationeventconfigurationId
                          where pstasks.ipg_caseid.Id == caseEntity.Id && configevent.ipg_name == PSEvents.CarrierPayment
                          select pstasks).FirstOrDefault();
            var statementgenerationtaskUpdated = service.Retrieve(statementgenerationtask.LogicalName, statementgenerationtask.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true)).ToEntity<ipg_statementgenerationtask>();
            //Assert
            Assert.Null(PSTask);

            Assert.NotEqual(ipg_statementgenerationtaskState.Inactive, statementgenerationtaskUpdated.StateCode);
            Assert.NotEqual(ipg_statementgenerationtask_StatusCode.Canceled, statementgenerationtaskUpdated.StatusCodeEnum);
        }

        [Fact]
        public void Gate11Tests_CarrierPaymentStatementGenerationTaskNotCreated_IfCarrierPaymentEventScheduled()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            ipg_statementgenerationeventconfiguration taskconfigCarrierPayment = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment);
            ipg_statementgenerationeventconfiguration taskconfigA2 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.A2Generated);

            ipg_statementgenerationtask statementgenerationtask = new ipg_statementgenerationtask().Fake().WithCaseReference(caseEntity).WithStatementEventConfig(taskconfigCarrierPayment);

            var listForInit = new List<Entity>() {caseEntity, taskconfigCarrierPayment, taskconfigA2, statementgenerationtask };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "MinimumSeverityLevel", (int)ipg_SeverityLevel.Warning } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate11",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Gate11>(pluginContext);

            var service = fakedContext.GetOrganizationService();
            var context = new CrmServiceContext(service);

            var PSTask = (from pstasks in context.ipg_statementgenerationtaskSet
                          join configevent in context.ipg_statementgenerationeventconfigurationSet on pstasks.ipg_eventid.Id equals configevent.ipg_statementgenerationeventconfigurationId
                          where pstasks.ipg_caseid.Id == caseEntity.Id 
                          && configevent.ipg_name == PSEvents.CarrierPayment 
                          select pstasks);
            var statementgenerationtaskUpdated = PSTask.FirstOrDefault();
            
            //Assert
            Assert.Single(PSTask);
            Assert.Equal(statementgenerationtask.Id, statementgenerationtaskUpdated.Id);
            Assert.NotEqual(ipg_statementgenerationtaskState.Inactive, statementgenerationtaskUpdated.StateCode);
            Assert.NotEqual(ipg_statementgenerationtask_StatusCode.Canceled, statementgenerationtaskUpdated.StatusCodeEnum);
        }
    }
}
