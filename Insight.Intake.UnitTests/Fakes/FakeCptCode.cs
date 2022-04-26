using System;
using Bogus;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeCptCode
    {
        public static Faker<ipg_cptcode> Fake(this ipg_cptcode self)
        {
            return new Faker<ipg_cptcode>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_CPTName, x => x.Random.String())
                .RuleFor(x => x.ipg_cptcode1, x => x.Random.String())
                .RuleFor(x => x.ipg_name, x => x.Random.Number(1000, 10000).ToString())
                .RuleFor(x => x.ipg_EffectiveDate, x => x.Date.Past(1))
                .RuleFor(x => x.ipg_ExpirationDate, x => x.Date.Future(1));
        }

        public static Faker<ipg_cptcode> WithImplantUsedOptionSetValue(this Faker<ipg_cptcode> self, int optionSetValue)
        {
            self.RuleFor(x => x.ipg_ImplantUsed, x => new OptionSetValue(optionSetValue));
            
            return self;
        }

        public static Faker<ipg_cptcode> WithSupportedCptValue(this Faker<ipg_cptcode> self, bool supportedCpt)
        {
            self.RuleFor(x => x.ipg_supportedCPT, x => supportedCpt);

            return self;
        }

        public static Faker<ipg_cptcode> WithStatusCode(this Faker<ipg_cptcode> self, int statusCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.StatusCode, x => new OptionSetValue(statusCode));
            return self;
        }

        public static Faker<ipg_cptcode> WithExpirationDate(this Faker<ipg_cptcode> self, DateTime date)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_ExpirationDate, x => date);
            return self;
        }

        public static Faker<ipg_cptcode> WithEffectiveDate(this Faker<ipg_cptcode> self, DateTime date)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_EffectiveDate, x => date);
            return self;
        }

        public static Faker<ipg_cptcode> WithCode(this Faker<ipg_cptcode> self, string code)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_cptcode1, x => code);
            return self;
        }

        public static Faker<ipg_cptcode> WithProcedureNameReference(this Faker<ipg_cptcode> self, EntityReference procedureNameReference)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_procedurename, x => procedureNameReference);
            return self;
        }

        public static Faker<ipg_cptcode> WithGroupReportingCode(this Faker<ipg_cptcode> self, OptionSetValue grReportingCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_cptroupreporting, x => grReportingCode);
            return self;
        }
    }
}