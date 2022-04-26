using FakeXrmEasy;
using Insight.Intake.Plugin.PortalComment;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;
using System;

namespace Insight.Intake.UnitTests.Plugin.PortalComment
{
    public class SetMessageStatusTests : PluginTestsBase
    {
        [Fact]
        public static void SetPortalCreatedByFieldWithoutPortalOwner()
        {

            var fakedContext = new XrmFakedContext();
            adx_portalcomment portalComment = new adx_portalcomment().Fake();
            portalComment.StateCode = adx_portalcommentState.Open;
            
            var initList = new List<Entity> { portalComment };
            fakedContext.Initialize(initList);

            var inputParams = new ParameterCollection { { "Target", portalComment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = adx_portalcomment.EntityLogicalName,
                InputParameters = inputParams,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<SetMessageStatus>(pluginContext);

            var updatedComment = (adx_portalcomment)fakedContext.GetOrganizationService().Retrieve(adx_portalcomment.EntityLogicalName,
               portalComment.Id,
               new Microsoft.Xrm.Sdk.Query.ColumnSet(adx_portalcomment.Fields.StatusCode));

            Assert.Equal((int)adx_portalcomment_StatusCode.Unread, updatedComment.StatusCode.Value);
        }
    }
}
