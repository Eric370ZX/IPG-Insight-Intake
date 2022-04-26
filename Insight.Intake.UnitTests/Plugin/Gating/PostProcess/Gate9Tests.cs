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
    public class Gate9Tests
    {
        [Fact]
        public void Gate9Tests_ResultSuccessfull_CreatesPromotedToCollectionsTask_And_CancellAllPSTask()
        {
            var fakedContext = new XrmFakedContext();
            ipg_globalsetting settings = new ipg_globalsetting().Fake("Carrier Services team", "Carrier Services");
            Team carrierteam = new Team().Fake("Carrier Services");
            Intake.Account carrier = new Intake.Account().FakeCarrierForEBV();
            Incident caseEntity = new Incident().Fake().WithPrimaryCarrierReference(carrier);
            ipg_statementgenerationeventconfiguration taskconfig1 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CaseApproved);
            ipg_statementgenerationeventconfiguration taskconfig2 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.PromotedToCollection1);
            ipg_statementgenerationeventconfiguration taskconfig3 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.PromotedToCollection2);

            ipg_documentcategorytype docategory = new ipg_documentcategorytype().Fake(DocumentCategioryNames.PatientStatement);
            ipg_documenttype P1Doc = new ipg_documenttype().Fake("Patient Statement P1").WithDocumentCategory(docategory);
            ipg_documenttype A2Doc = new ipg_documenttype().Fake("Patient Statement A2").WithDocumentCategory(docategory);
            ipg_document Psdoc1 = new ipg_document().Fake().WithDocumentCategoryReference(docategory).WithDocumentTypeReference(P1Doc).WithCaseReference(caseEntity);
            ipg_document Psdoc2 = new ipg_document().Fake().WithDocumentCategoryReference(docategory).WithDocumentTypeReference(A2Doc).WithCaseReference(caseEntity);

            ipg_statementgenerationtask AcceptedCaseTask = new ipg_statementgenerationtask().Fake().WithCaseReference(caseEntity).WithStatementEventConfig(taskconfig1);
            var listForInit = new List<Entity>() { P1Doc, A2Doc, caseEntity, taskconfig1, taskconfig2, taskconfig3, AcceptedCaseTask, carrier, carrierteam, settings, docategory, Psdoc1, Psdoc2 };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "MinimumSeverityLevel", (int)ipg_SeverityLevel.Warning } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate9",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Gate9>(pluginContext);

            var service = fakedContext.GetOrganizationService();
            var context = new CrmServiceContext(service);

            var PSTask = (from pstasks in context.ipg_statementgenerationtaskSet
                          join configevent in context.ipg_statementgenerationeventconfigurationSet on pstasks.ipg_eventid.Id equals configevent.ipg_statementgenerationeventconfigurationId
                          where pstasks.ipg_caseid.Id == caseEntity.Id
                          select pstasks).FirstOrDefault();

            var psDoc = (from doc in context.ipg_documentSet
                             where doc.ipg_CaseId.Id == caseEntity.Id && doc.ipg_documenttypecategoryid.Id == docategory.Id
                             select doc).ToList();

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.NotNull(PSTask);
            Assert.Single(psDoc.Where(d => d.ipg_isactivepatientstatement == false));
        }

        [Fact]
        public void Gate9Tests_ResultError_Not_CreatesPromotedToCollectionsTask_And_NotCancellAllPSTask()
        {
            var fakedContext = new XrmFakedContext();

            ipg_globalsetting settings = new ipg_globalsetting().Fake("Carrier Services team", "Carrier Services");
            Team carrierteam = new Team().Fake("Carrier Services");
            Intake.Account carrier = new Intake.Account().FakeCarrierForEBV();
            Incident caseEntity = new Incident().Fake().WithPrimaryCarrierReference(carrier);
            ipg_statementgenerationeventconfiguration taskconfig1 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CaseApproved);
            ipg_statementgenerationeventconfiguration taskconfig2 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.PromotedToCollection1);
            ipg_statementgenerationeventconfiguration taskconfig3 = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.PromotedToCollection2);

            ipg_documentcategorytype docategory = new ipg_documentcategorytype().Fake(DocumentCategioryNames.PatientStatement);
            ipg_document Psdoc1 = new ipg_document().Fake().WithDocumentCategoryReference(docategory).WithCaseReference(caseEntity);
            ipg_document Psdoc2 = new ipg_document().Fake().WithDocumentCategoryReference(docategory).WithCaseReference(caseEntity);

            ipg_statementgenerationtask AcceptedCaseTask = new ipg_statementgenerationtask().Fake().WithCaseReference(caseEntity).WithStatementEventConfig(taskconfig1);
            var listForInit = new List<Entity>() { caseEntity, taskconfig1, taskconfig2, taskconfig3, AcceptedCaseTask, carrier, carrierteam, settings, docategory, Psdoc1, Psdoc2 };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "MinimumSeverityLevel", (int)ipg_SeverityLevel.Error } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate9",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Gate9>(pluginContext);

            var service = fakedContext.GetOrganizationService();
            var context = new CrmServiceContext(service);

            var PSTask = (from pstasks in context.ipg_statementgenerationtaskSet
                          join configevent in context.ipg_statementgenerationeventconfigurationSet on pstasks.ipg_eventid.Id equals configevent.ipg_statementgenerationeventconfigurationId
                          where pstasks.ipg_caseid.Id == caseEntity.Id
                          select pstasks).FirstOrDefault();

            var psDoc = (from doc in context.ipg_documentSet
                             where doc.ipg_CaseId.Id == caseEntity.Id && doc.ipg_documenttypecategoryid.Id == docategory.Id
                             select doc).ToList();

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.NotNull(PSTask);
            Assert.True(psDoc.All(d => d.StateCode == ipg_documentState.Active));
        }
    }
}
