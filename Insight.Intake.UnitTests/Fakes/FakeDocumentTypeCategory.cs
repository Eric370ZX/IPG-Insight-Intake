using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeDocumentTypeCategory
    {
        public static Faker<ipg_documentcategorytype> Fake(this ipg_documentcategorytype self, string Name)
        {
            return new Faker<ipg_documentcategorytype>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => Name);
        }
    }
}
