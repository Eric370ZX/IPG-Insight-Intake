using System;
using Bogus;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeAnnotation
    {
        public static Faker<Annotation> Fake(this Annotation self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            return new Faker<Annotation>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.NoteText, x => x.Random.String())
                .RuleFor(x => x.Subject, x => x.Random.String());
        }

        public static Faker<Annotation> WithDocument(this Faker<Annotation> self, string fileName = null, string documentBody = null)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            self.RuleFor(x => x.IsDocument, true)
                .RuleFor(x => x.FileName, x => fileName ?? x.Random.String())
                .RuleFor(x => x.FileSize, x => x.Random.Int())
                .RuleFor(x => x.DocumentBody, x => documentBody ?? x.Random.String());

            return self;
        }

        public static Faker<Annotation> WithObjectReference(this Faker<Annotation> self, Entity objectEnt)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (objectEnt == null) throw new ArgumentNullException(nameof(objectEnt));

            self.RuleFor(
                x => x.ObjectId,
                x => objectEnt.ToEntityReference())
                .RuleFor(
                x => x.ObjectTypeCode,
                x => objectEnt.LogicalName);

            return self;
        }
    }
}