using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Insight.Intake.Plugin.ClaimResponseHeader;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.UnitTests.Plugin.ClaimResponse
{
    public class UpdateClaimIdTest : PluginTestsBase
    {
        [Fact]
        public void CheckClaimId()
        {
            var fakedContext = new XrmFakedContext();
            ServiceProvider = ServiceProviderMock.Object;
            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_claimresponseheader header = new ipg_claimresponseheader().Fake().WithClaimResponseBatchReference(batch);
            header.ipg_PostStatus = "error";

            Invoice invoice = new Invoice().Fake();
            invoice.Name = "testName";
            header.ipg_ClaimNumber = invoice.Name;

            var listForInit = new List<Entity> { batch, header, invoice };
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", header } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 20,
                PrimaryEntityName = ipg_claimresponseheader.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpdateClaimId>(pluginContext);
            Assert.Equal(invoice.Id, header.ipg_ClaimId.Id);
            Assert.Equal("new", header.ipg_PostStatus);
        }
    }
}
