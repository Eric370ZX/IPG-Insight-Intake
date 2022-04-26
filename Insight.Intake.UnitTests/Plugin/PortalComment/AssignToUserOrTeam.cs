using FakeXrmEasy;
using Insight.Intake.Plugin.PortalComment;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.PortalComment
{
    public class PortalComment : PluginTestsBase
    {
        [Fact]
        public static void AssignToUserOrTeamTest()
        {
            var fakedContext = new XrmFakedContext();

            var owner = new SystemUser().Fake();
            adx_portalcomment portalComment = new adx_portalcomment().Fake().WithOwner(owner);

            var initList = new List<Entity> { owner, portalComment };
            fakedContext.Initialize(initList);

            var inputParams = new ParameterCollection { { "Target", portalComment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = adx_portalcomment.EntityLogicalName,
                InputParameters = inputParams,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<AssignToUserOrTeam>(pluginContext);
            var updatedComment = (adx_portalcomment)fakedContext.GetOrganizationService().Retrieve(adx_portalcomment.EntityLogicalName,
                portalComment.Id,
                new Microsoft.Xrm.Sdk.Query.ColumnSet(adx_portalcomment.Fields.To));
            Assert.NotNull(updatedComment.To);
        }
    }
}
