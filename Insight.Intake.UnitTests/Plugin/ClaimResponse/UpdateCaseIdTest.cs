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
    public class UpdateCaseIdTest : PluginTestsBase
    {
        [Fact]
        public void CheckCaseId()
        {
            var fakedContext = new XrmFakedContext();
            ServiceProvider = ServiceProviderMock.Object;
            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            ipg_claimresponseheader header = new ipg_claimresponseheader().Fake().WithClaimResponseBatchReference(batch);
            header.ipg_PostStatus = "error";

            Incident incident = new Incident().Fake();
            incident.Title = "testTitle";
            header.ipg_CorrectedCaseNumber = incident.Title;

            var listForInit = new List<Entity> { batch, header, incident };
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

            fakedContext.ExecutePluginWith<UpdateCaseId>(pluginContext);
            Assert.Equal(incident.Id, header.ipg_CaseId.Id);
            Assert.Equal("new", header.ipg_PostStatus);
        }
    }
}
