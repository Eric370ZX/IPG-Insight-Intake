using FakeXrmEasy;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    
    public class ReassignDocumentToReferralTests: PluginTestsBase
    {
        [Fact]
        public void CheckReassignDocumentWithOriginatingTask()
        {
            var fakedContext = new XrmFakedContext();
            fakedContext.AddRelationship( ipg_document.Fields.ipg_ipg_referral_ipg_document_ReferralId,
                new XrmFakedRelationship(ipg_referral.Fields.Id, ipg_document.Fields.ipg_ReferralId, ipg_referral.EntityLogicalName, ipg_document.EntityLogicalName));

            ipg_referral primaryRefferal = new ipg_referral()
                .Fake(Guid.NewGuid());

            ipg_referral secondaryreferral = new ipg_referral()
                .Fake(Guid.NewGuid());

            ipg_documenttype documentType = new ipg_documenttype()
                .Fake(Guid.NewGuid());

            Task originatingTask = new Task()
                .Fake()
                .WithRegarding(primaryRefferal.ToEntityReference());

            ipg_document document = new ipg_document()
                .Fake(Guid.NewGuid())
                .WithReferralReference(primaryRefferal)
                .WithDocumentTypeReference(documentType)
                .WithOwnerReference(new EntityReference("systemsuser", new Guid()))
                .WithOriginatingTaskReference(originatingTask);

            var fakedEntities = new Entity[] { primaryRefferal, secondaryreferral, documentType, originatingTask, document };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", document.ToEntityReference() }, { "ReferralRef", secondaryreferral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "IsSuccess", false }, { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGDocumentActionsReassignToReferral",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };

            fakedContext.ExecutePluginWith<ReassignDocumentToReferral>(pluginContext);

            originatingTask = fakedContext.GetOrganizationService()
                .Retrieve(originatingTask.LogicalName, originatingTask.Id, new ColumnSet(true))
                .ToEntity<Task>();

            document = fakedContext.GetOrganizationService()
                .Retrieve(document.LogicalName, document.Id, new ColumnSet(true))
                .ToEntity<ipg_document>();

            Assert.True(pluginContext.OutputParameters.Contains("IsSuccess") && pluginContext.OutputParameters["IsSuccess"] is bool success && success);

            Assert.True(originatingTask.RegardingObjectId.Id == secondaryreferral.Id, "Task is not reassigned");
            Assert.True(document.ipg_ReferralId.Id == secondaryreferral.Id, "Document is not reassigned");
        }

        [Fact]
        public void CheckReassignDocumentWithoutOriginatingTask()
        {
            var fakedContext = new XrmFakedContext();
            fakedContext.AddRelationship(ipg_document.Fields.ipg_ipg_referral_ipg_document_ReferralId,
                new XrmFakedRelationship(ipg_referral.Fields.Id, ipg_document.Fields.ipg_ReferralId, ipg_referral.EntityLogicalName, ipg_document.EntityLogicalName));

            ipg_referral primaryRefferal = new ipg_referral()
                .Fake(Guid.NewGuid());

            ipg_referral secondaryreferral = new ipg_referral()
                .Fake(Guid.NewGuid());

            ipg_documenttype documentType = new ipg_documenttype()
                .Fake(Guid.NewGuid());

            ipg_document document = new ipg_document()
                .Fake(Guid.NewGuid())
                .WithReferralReference(primaryRefferal)
                .WithDocumentTypeReference(documentType)
                .WithOwnerReference(new EntityReference("systemsuser", new Guid()));

            var fakedEntities = new Entity[] { primaryRefferal, secondaryreferral, documentType, document };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", document.ToEntityReference() }, { "ReferralRef", secondaryreferral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "IsSuccess", false }, { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGDocumentActionsReassignToReferral",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };

            fakedContext.ExecutePluginWith<ReassignDocumentToReferral>(pluginContext);

            document = fakedContext.GetOrganizationService()
                .Retrieve(document.LogicalName, document.Id, new ColumnSet(true))
                .ToEntity<ipg_document>();

            Task newTask = fakedContext.GetOrganizationService()
                .RetrieveMultiple(GetLatestTaskQuery())
                .Entities
                .FirstOrDefault().ToEntity<Task>();

            Assert.True(
                pluginContext.OutputParameters.Contains("IsSuccess") && pluginContext.OutputParameters["IsSuccess"] is bool success && success,
                pluginContext.OutputParameters["Message"].ToString());

            Assert.True(document.ipg_ReferralId.Id == secondaryreferral.Id, "Document is not reassigned");
            Assert.True(newTask.ipg_DocumentType.Id == documentType.Id, "Incorrect document type");
            Assert.True(newTask.StateCode.Value == TaskState.Completed, "Task should be completed");
            Assert.True(newTask.RegardingObjectId.Id == secondaryreferral.Id, "Task should be assigned to different referral");
        }

        private QueryExpression GetLatestTaskQuery()
        {
            return new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                TopCount = 1,
                Orders =
                {
                    new OrderExpression("createdon", OrderType.Descending)
                }
            };
        }
    }
}
