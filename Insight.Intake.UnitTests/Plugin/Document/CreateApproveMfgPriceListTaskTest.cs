using FakeXrmEasy;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class CreateApproveMfgPriceListTaskTest : PluginTestsBase
    {
        private static readonly string MfgPriceListApproverGlobalSettingName = "MFG_PRICE_LIST_APPROVER_TEAM_NAME";
        private static readonly string MfgPriceListApproverTeamName = "VP of Ops";
        private static readonly int ApproveMfgPriceListTaskTypeValue = 427880053;

        [Fact]
        public void Creates_ApproveMfgPriceList_task()
        {
            //SETUP

            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("Price list", "PRL");
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithReviewStatus(ipg_document_ipg_ReviewStatus.PendingReview);
            ipg_globalsetting globalSetting = new ipg_globalsetting().Fake()
                .WithName(MfgPriceListApproverGlobalSettingName)
                .WithValue(MfgPriceListApproverTeamName);
            Team team = new Team() { Id = System.Guid.NewGuid(), Name = MfgPriceListApproverTeamName };

            var listForInit = new List<Entity> { documentType, document, globalSetting, team };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };


            //EXECUTE

            fakedContext.ExecutePluginWith<CreateApproveMfgPriceListTask>(pluginContext);


            //ASSERT

            var query = new QueryExpression(Task.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(true)
            };
            var orgService = fakedContext.GetOrganizationService();
            var result = orgService.RetrieveMultiple(query).Entities;
            Assert.Single(result);
            Assert.Equal(ApproveMfgPriceListTaskTypeValue, ((Task)result[0]).ipg_tasktypecode.Value);
            Assert.Equal(team.Id, ((Task)result[0]).OwnerId.Id);
            Assert.Equal(document.Id, ((Task)result[0]).RegardingObjectId.Id);
        }

    }
}
