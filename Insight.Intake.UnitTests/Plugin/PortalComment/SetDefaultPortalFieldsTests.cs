using FakeXrmEasy;
using Insight.Intake.Plugin.PortalComment;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;
using System;

namespace Insight.Intake.UnitTests.Plugin.PortalComment
{
    public class SetDefaultPortalFieldsTests : PluginTestsBase
    {
        [Fact]
        public static void SetPortalCreatedByFieldWithoutPortalOwner()
        {
          
            var fakedContext = new XrmFakedContext();
            var owner = new SystemUser().Fake();
            adx_portalcomment portalComment = new adx_portalcomment().Fake().WithOwner(owner);
            portalComment.adx_PortalCommentDirectionCode = new OptionSetValue();
            portalComment.adx_PortalCommentDirectionCode.Value = (int)adx_portalcomment_adx_PortalCommentDirectionCode.Outgoing;

            var initList = new List<Entity> { owner, portalComment };
            fakedContext.Initialize(initList);

            var inputParams = new ParameterCollection { { "Target", portalComment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = adx_portalcomment.EntityLogicalName,
                InputParameters = inputParams,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<SetDefaultPortalFields>(pluginContext);
            var updatedComment = portalComment;
            Assert.NotNull(updatedComment.ipg_portalcreatedby);
            Assert.Equal("IPG", updatedComment.ipg_portalcreatedby);
        }

        [Fact]
        public static void SetPortalCreatedByFieldWithPortalOwner()
        {

            var fakedContext = new XrmFakedContext();
            var owner = new SystemUser().Fake();
            var owningUser = new Contact().Fake();
            adx_portalcomment portalComment = new adx_portalcomment().Fake().WithOwner(owner).WithPortalOwner(owningUser);
            portalComment.ipg_owningportaluserid.Name = "Test User";
            portalComment.adx_PortalCommentDirectionCode = new OptionSetValue();
            portalComment.adx_PortalCommentDirectionCode.Value = (int)adx_portalcomment_adx_PortalCommentDirectionCode.Incoming;

            var initList = new List<Entity> { owner, portalComment, owningUser };
            fakedContext.Initialize(initList);

            var inputParams = new ParameterCollection { { "Target", portalComment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = adx_portalcomment.EntityLogicalName,
                InputParameters = inputParams,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<SetDefaultPortalFields>(pluginContext);
            var updatedComment = portalComment;
            Assert.NotNull(updatedComment.ipg_portalcreatedby);
            Assert.Equal("Test User", updatedComment.ipg_portalcreatedby);
        }

        [Fact]
        public static void SetToFromField()
        {
            var fakedContext = new XrmFakedContext();

            var owner = new SystemUser().Fake();
            adx_portalcomment portalComment = new adx_portalcomment().Fake().WithOwner(owner);
            var optionSet = new OptionSetValue();
            optionSet.Value = new Random().Next(1, 2);
            portalComment.adx_PortalCommentDirectionCode = optionSet;


            var initList = new List<Entity> { owner, portalComment };
            fakedContext.Initialize(initList);

            var inputParams = new ParameterCollection { { "Target", portalComment } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = adx_portalcomment.EntityLogicalName,
                InputParameters = inputParams,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<SetDefaultPortalFields>(pluginContext);
            var updatedComment = portalComment;
            Assert.NotNull(updatedComment.ipg_tofrom);
            if (updatedComment.adx_PortalCommentDirectionCode.Value == 1)
            {
                Assert.Equal("To IPG", updatedComment.ipg_tofrom);
            }
            if (updatedComment.adx_PortalCommentDirectionCode.Value == 2)
            {
                Assert.Equal("From IPG", updatedComment.ipg_tofrom);
            }
        }
    }
}
