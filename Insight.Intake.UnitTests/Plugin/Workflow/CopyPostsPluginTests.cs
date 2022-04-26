using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Data;
using Insight.Intake.Plugin.Workflow;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Workflow
{
    public class CopyPostsPluginTests : PluginTestsBase
    {
        [Fact]
        public void CheckWhatPostsCopiedCorrectly()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            ipg_referral referral = new ipg_referral().Fake();

            Incident incident = new Incident().Fake();

            var numberOfPosts = new Random().Next(5, 10);

            IList<Post> posts = new Post().Fake()
                .WithRegardingObjectReference(referral)
                .WithSourceOptionSetValue(PostSourceCode.AutoPost)
                .WithTypeOptionSetValue(PostTypeCode.CheckIn)
                .Generate(numberOfPosts);

            IList<Post> createdPosts = new List<Post>();

            OrganizationServiceMock.WithCreateCrud<Post>().Callback<Entity>(x => createdPosts.Add(x.ToEntity<Post>()));
            
            OrganizationServiceMock.WithRetrieveMultipleCrud(Post.EntityLogicalName, posts.Cast<Entity>().ToList());

            OrganizationServiceMock.WithRetrieveMultipleCrud(PostComment.EntityLogicalName, new List<Entity>());
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGIntakeActionsCopyPosts");

            var request = new ipg_IPGIntakeActionsCopyPostsRequest
            {
                SourceReference = new EntityReference(ipg_referral.EntityLogicalName, referral.Id),
                TargetReference = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion
            
            #region Execute plugin
            var plugin = new CopyPostsPlugin();

            plugin.Execute(ServiceProvider);

            foreach (var post in posts)
            {
                var createdPost = createdPosts.FirstOrDefault(x => x.Text == post.Text);
                
                Assert.NotNull(createdPost);
                
                Assert.Equal(incident.LogicalName, createdPost.RegardingObjectId.LogicalName);
                
                Assert.Equal(incident.Id, createdPost.RegardingObjectId.Id);
            }
            #endregion
        }
        
        [Fact]
        public void CheckWhatPostsWithCommentsCopiedCorrectly()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            ipg_referral referral = new ipg_referral().Fake();

            Incident incident = new Incident().Fake();

            var numberOfPostComments = new Random().Next(5, 10);

            Post post = new Post().Fake()
                .WithRegardingObjectReference(referral)
                .WithSourceOptionSetValue(PostSourceCode.AutoPost)
                .WithTypeOptionSetValue(PostTypeCode.CheckIn);

            IList<PostComment> postComments = new PostComment().Fake()
                .WithPostReference(post)
                .Generate(numberOfPostComments);

            IList<Post> createdPosts = new List<Post>();

            IList<PostComment> createdComments = new List<PostComment>();

            OrganizationServiceMock.WithCreateCrud<Post>().Callback<Entity>(x => createdPosts.Add(x.ToEntity<Post>()));

            OrganizationServiceMock.WithCreateCrud<PostComment>().Callback<Entity>(x => createdComments.Add(x.ToEntity<PostComment>()));
            
            OrganizationServiceMock.WithRetrieveMultipleCrud(Post.EntityLogicalName, new List<Entity> { post });

            OrganizationServiceMock.WithRetrieveMultipleCrud(PostComment.EntityLogicalName, postComments.Cast<Entity>().ToList());
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGIntakeActionsCopyPosts");

            var request = new ipg_IPGIntakeActionsCopyPostsRequest
            {
                SourceReference = new EntityReference(ipg_referral.EntityLogicalName, referral.Id),
                TargetReference = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion
            
            #region Execute plugin
            var plugin = new CopyPostsPlugin();

            plugin.Execute(ServiceProvider);

            var createdPost = createdPosts.FirstOrDefault(x => x.Text == post.Text);
                
            Assert.NotNull(createdPost);

            foreach (var postComment in postComments)
            {
                var createdComment = createdComments.FirstOrDefault(x => x.Text == postComment.Text);
                
                Assert.NotNull(createdComment);
                
                Assert.Equal(createdPost.LogicalName, createdComment.PostId.LogicalName);
                
                Assert.Equal(createdPost.Id, createdComment.PostId.Id);
            }
            #endregion
        }
    }
}