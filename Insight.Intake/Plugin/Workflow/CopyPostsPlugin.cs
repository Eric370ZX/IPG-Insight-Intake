using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Linq;
using Insight.Intake.Helpers;

namespace Insight.Intake.Plugin.Workflow
{
    public class CopyPostsPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);

                if (context.MessageName == Constants.ActionNames.IntakeActionsCopyPosts)
                {
                    if (context.InputParameters.Contains("SourceReference") && context.InputParameters.Contains("TargetReference"))
                    {
                        var sourceReference = (EntityReference)context.InputParameters["SourceReference"];

                        var targetReference = (EntityReference)context.InputParameters["TargetReference"];

                        var postsQuery = new QueryExpression
                        {
                            EntityName = Post.EntityLogicalName,
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression(LogicalOperator.And),
                            PageInfo = new PagingInfo
                            {
                                ReturnTotalRecordCount = true,
                            },
                        };
                        
                        postsQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, sourceReference.Id);

                        var posts = organizationService.RetrieveMultiple(postsQuery);

                        foreach (var post in posts.Entities.Cast<Post>())
                        {
                            #region Create Post
                            var newPost = new Post
                            {
                                RegardingObjectId = new EntityReference(targetReference.LogicalName, targetReference.Id),
                                Source = post.Source,
                                Type = post.Type,
                                Text = post.Text,
                                TimeZoneRuleVersionNumber = post.TimeZoneRuleVersionNumber,
                                UTCConversionTimeZoneCode = post.UTCConversionTimeZoneCode,
                            };
                            
                            newPost.Attributes.Add("createdby", post.CreatedBy);
                            
                            newPost.Attributes.Add("createdon", post.CreatedOn);
                            
                            newPost.Attributes.Add("modifiedby", post.ModifiedBy);
                            
                            newPost.Attributes.Add("modifiedon", post.ModifiedOn);

                            var newPostId = organizationService.Create(newPost);
                            #endregion

                            #region Create Post Comments
                            var postCommentsQuery = new QueryExpression
                            {
                                EntityName = PostComment.EntityLogicalName,
                                ColumnSet = new ColumnSet(true),
                                Criteria = new FilterExpression(LogicalOperator.And),
                                PageInfo = new PagingInfo
                                {
                                    ReturnTotalRecordCount = true,
                                },
                            };
                            
                            postCommentsQuery.Criteria.AddCondition("postid", ConditionOperator.Equal, post.Id);

                            var postComments = organizationService.RetrieveMultiple(postCommentsQuery);

                            foreach (var postComment in postComments.Entities.Cast<PostComment>())
                            {
                                var newPostComment = new PostComment
                                {
                                    PostId = new EntityReference(Post.EntityLogicalName, newPostId),
                                    Text = postComment.Text,
                                    TimeZoneRuleVersionNumber = postComment.TimeZoneRuleVersionNumber,
                                    UTCConversionTimeZoneCode = postComment.UTCConversionTimeZoneCode,
                                };
                                
                                newPostComment.Attributes.Add("createdby", postComment.CreatedBy);
                                
                                newPostComment.Attributes.Add("createdon", postComment.CreatedOn);

                                organizationService.Create(newPostComment);
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(CopyPostsPlugin)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace("{0}: {1}", nameof(CopyPostsPlugin), exception.ToString());
                throw;
            }
        }
    }
}