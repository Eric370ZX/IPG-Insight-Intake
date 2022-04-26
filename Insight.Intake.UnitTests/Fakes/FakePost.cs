using System;
using System.Activities.Expressions;
using Bogus;
using Insight.Intake.Data;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakePost
    {
        public static Faker<Post> Fake(this Post self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            return new Faker<Post>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.Text, x => x.Random.String());
        }

        public static Faker<Post> WithRegardingObjectReference(this Faker<Post> self, Entity regardingObject)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            if (regardingObject == null) throw new ArgumentNullException(nameof(regardingObject));
            
            self.RuleFor(
                x => x.RegardingObjectId, 
                x => new EntityReference
                {
                    LogicalName = regardingObject.LogicalName,
                    Id = regardingObject.Id,
                }
            );
            
            return self;
        }

        public static Faker<Post> WithTypeOptionSetValue(this Faker<Post> self, PostTypeCode type)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            self.RuleFor(x => x.Type, x => new OptionSetValue((int)type));

            return self;
        }

        public static Faker<Post> WithSourceOptionSetValue(this Faker<Post> self, PostSourceCode source)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            self.RuleFor(x => x.Source, x => new OptionSetValue((int)source));
            
            return self;
        }
    }

    internal static class FakePostComment
    {
        public static Faker<PostComment> Fake(this PostComment self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            return new Faker<PostComment>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.Text, x => x.Random.String());
        }

        public static Faker<PostComment> WithPostReference(this Faker<PostComment> self, Post post)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.PostId,
                x => new EntityReference
                {
                    LogicalName = post.LogicalName,
                    Id = post.Id,
                }
            );

            return self;
        }
    }
}