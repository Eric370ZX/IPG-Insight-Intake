using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CaseHasSubmittedClaimsTests : PluginTestsBase
    {
        [Fact]
        public void CaseHasSubmittedClaims_HasClaims_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account().Fake();
            carrier.Name = "Some name";
            Incident caseEntity = new Incident().Fake()
                .WithCarrierReference(carrier,true);
            Invoice submInvoice = new Invoice().Fake()
                .WithCaseReference(caseEntity);
            submInvoice.ipg_Status = new OptionSetValue((int)Insight.Intake.ipg_ClaimStatus.Submitted);
            submInvoice.ipg_active = true;
            submInvoice.ipg_caseid = caseEntity.ToEntityReference();            

            var listForInit = new List<Entity>() { caseEntity, submInvoice, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCaseHasSubmittedClaims",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CaseHasSubmittedClaims>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CaseHasSubmittedClaims_NoClaims_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account().Fake();
            carrier.Name = "Some name";
            Incident caseEntity = new Incident().Fake()
                .WithCarrierReference(carrier,true);
            Invoice submInvoice = new Invoice().Fake()
                .WithCaseReference(caseEntity);
            submInvoice.ipg_Status = new OptionSetValue((int)Insight.Intake.ipg_ClaimStatus.Processed);
            submInvoice.StateCode = InvoiceState.Active;
            submInvoice.ipg_caseid = caseEntity.ToEntityReference();

            var listForInit = new List<Entity>() { caseEntity, submInvoice, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCaseHasSubmittedClaims",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CaseHasSubmittedClaims>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.False(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
