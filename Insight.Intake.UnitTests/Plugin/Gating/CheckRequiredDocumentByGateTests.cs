using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckRequiredDocumentByGateTests : PluginTestsBase
    {
        [Fact]
        public void CheckRequiredDocumentByGateTests_DocumentExists()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            ipg_documenttype docType = new ipg_documenttype().Fake("ICS");
            docType.ipg_DocumentTypeAbbreviation = "ICS";
            ipg_document document = new ipg_document()
                .Fake()
                .WithDocumentTypeReference(docType)
                .WithCaseReference(caseEntity);
            document.StatusCode = new OptionSetValue((int)ipg_document_StatusCode.Active);
            document.ipg_ReviewStatus = new OptionSetValue((int)ipg_document_ipg_ReviewStatus.Approved);

            var listForInit = new List<Entity>() { caseEntity, docType, document };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "DocumentType", docType.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckRequiredDocumentByGate",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckRequiredDocumentByGate>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
            Assert.Equal(1, pluginContext.OutputParameters["CodeOutput"]);
        }

        [Fact]
        public void CheckRequiredDocumentByGateTests_DocumentDoesNotExist()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            ipg_documenttype docType = new ipg_documenttype().Fake("ICS");
            docType.ipg_DocumentTypeAbbreviation = "ICS";

            var listForInit = new List<Entity>() { caseEntity, docType };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "DocumentType", docType.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckRequiredDocumentByGate",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckRequiredDocumentByGate>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
            Assert.Equal(1, pluginContext.OutputParameters["CodeOutput"]);
        }
    }
}
