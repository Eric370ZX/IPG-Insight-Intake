using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeInformationTypeRequiredInformationRule
    {
        public static Faker<ipg_informationtyperequiredinformationrule> Fake(this ipg_informationtyperequiredinformationrule self)
        {
            return new Faker<ipg_informationtyperequiredinformationrule>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => x.Name.FullName());
        }

        public static Faker<ipg_informationtyperequiredinformationrule> WithDocumentType(this ipg_informationtyperequiredinformationrule self, ipg_documenttype documenttype)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return self.Fake()
                .RuleFor(x => x.ipg_DocumentTypeId, x => documenttype.ToEntityReference());
        }

    }
}
