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
    public class CheckValidatedAcceptedClaimTests : PluginTestsBase
    {
        private readonly string messageName = "ipg_IPGGatingCheckValidatedAcceptedClaim";
        [Fact]
        public void CaseValidatedAcceptedClaim_NoClaims_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            Invoice submInvoice = new Invoice().Fake()
                .WithCaseReference(caseEntity);
            submInvoice.ipg_Status = new OptionSetValue((int)Insight.Intake.ipg_ClaimStatus.Submitted);
            submInvoice.StateCode = InvoiceState.Active;
            submInvoice.ipg_caseid = caseEntity.ToEntityReference();            

            var listForInit = new List<Entity>() { caseEntity, submInvoice };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckValidatedAcceptedClaim>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CaseValidatedAcceptedClaim_HasClaims_returnsError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            Invoice submInvoice = new Invoice().Fake()
                .WithCaseReference(caseEntity);
            submInvoice.ipg_Status = new OptionSetValue((int)Insight.Intake.ipg_ClaimStatus.Validated);
            submInvoice.ipg_Reason = new OptionSetValue((int)Insight.Intake.ipg_ClaimReason.RejectedbyIntermediary);
            submInvoice.StateCode = InvoiceState.Active;
            submInvoice.ipg_caseid = caseEntity.ToEntityReference();

            var listForInit = new List<Entity>() { caseEntity, submInvoice };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckValidatedAcceptedClaim>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.False(pluginContext.OutputParameters["Succeeded"] as bool?==true);
        }
    }
}
