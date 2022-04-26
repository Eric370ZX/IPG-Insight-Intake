using FakeXrmEasy;
using Insight.Intake.Plugin.Payment;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Payment
{
    public class UpdateClaimResponseHeaderStatusTest : PluginTestsBase
    {
        [Fact]
        public void CreatePayment()
        {

            var fakedContext = new XrmFakedContext();

            decimal amountPaid = 100;
            ipg_claimresponsebatch batch = new ipg_claimresponsebatch().Fake();
            Incident incident = new Incident().Fake();
            ipg_claimresponseheader header = new ipg_claimresponseheader()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseBatchId = batch.ToEntityReference(),
                ipg_AmountPaid_new = new Money(amountPaid),
                ipg_PostStatus = "new"
            };
            ipg_payment payment = new ipg_payment()
            {
                Id = Guid.NewGuid(),
                ipg_ClaimResponseHeader = header.ToEntityReference(),
                ipg_CaseId = incident.ToEntityReference()
            };

            fakedContext.Initialize(new List<Entity> { batch, header, payment, incident });

            var inputParameters = new ParameterCollection { { "Target", payment } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_payment.EntityLogicalName,
                InputParameters = inputParameters
            };

            fakedContext.ExecutePluginWith<UpdateClaimResponseHeaderStatus>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            ipg_claimresponsebatch batchFaked = fakedService.Retrieve(batch.LogicalName, batch.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_appliedamount_new).ToLower())).ToEntity<ipg_claimresponsebatch>();
            Assert.Equal(batchFaked.ipg_appliedamount_new.Value, amountPaid);

        }

    }
}
