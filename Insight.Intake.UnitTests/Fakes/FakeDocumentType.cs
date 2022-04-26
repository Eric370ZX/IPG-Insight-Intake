using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeDocumentType
    {
        public static Faker<ipg_documenttype> Fake(this ipg_documenttype self, Guid caseId)
        {
            return self.Fake().RuleFor(x => x.Id, x => caseId);
        }

        public static Faker<ipg_documenttype> Fake(this ipg_documenttype self, string documentTypeName)
        {
            return new Faker<ipg_documenttype>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => documentTypeName);
        }

        public static Faker<ipg_documenttype> Fake(this ipg_documenttype self, string documentTypeName, string docTypeAbbreviation)
        {
            return new Faker<ipg_documenttype>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => documentTypeName)
                .RuleFor(x => x.ipg_DocumentTypeAbbreviation, x => docTypeAbbreviation);
        }

        public static Faker<ipg_documenttype> WithDocumentCategory(this Faker<ipg_documenttype> self, ipg_documentcategorytype documentcategory)
        {
            self.RuleFor(x => x.ipg_DocumentCategoryTypeId, x => documentcategory.ToEntityReference());
            return self;
        }
    }
}
