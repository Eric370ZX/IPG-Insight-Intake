using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Plugin.ClaimResponseHeader;

namespace Insight.Intake.UnitTests.Plugin.ClaimResponse
{
    public class UpdateClaimResponseBatchAmountsTest : PluginTestsBase
    {
        [Fact]
        public void CheckBatchAmount()
        {
            var fakedContext = new XrmFakedContext();
            ServiceProvider = ServiceProviderMock.Object;
            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_claimresponseheader header1 = new ipg_claimresponseheader().Fake().WithClaimResponseBatchReference(batch);
            ipg_claimresponseheader header2 = new ipg_claimresponseheader().Fake().WithClaimResponseBatchReference(batch);
            header1.ipg_AmountPaid_new = new Money(100);
            header1.ipg_PaymentType = new OptionSetValue((int)ipg_ClaimResponseHeaderType.Refund);
            header2.ipg_AmountPaid_new = new Money(200);
            header2.ipg_PaymentType = new OptionSetValue((int)ipg_ClaimResponseHeaderType.Refund);

            Incident case1 = new Incident().Fake();
            Incident case2 = new Incident().Fake();
            header1.ipg_CaseId = case1.ToEntityReference();
            header2.ipg_CaseId = case1.ToEntityReference();

            var listForInit = new List<Entity> { batch, header1, header2, case1, case2 };
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", header1 } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = ipg_claimresponseheader.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpdateClaimResponseBatchAmounts>(pluginContext);
            var batchRef = (fakedContext.GetOrganizationService().Retrieve(batch.LogicalName, batch.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_TotalAmount_new).ToLower())).ToEntity<ipg_claimresponsebatch>());
            Assert.Equal(header1.ipg_AmountPaid_new.Value + header2.ipg_AmountPaid_new.Value, batchRef.ipg_TotalAmount_new.Value);
        }
    }
}
