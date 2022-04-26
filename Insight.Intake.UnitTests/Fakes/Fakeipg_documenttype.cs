using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class Fakeipg_documenttype
    {
        public static Faker<ipg_documenttype> Fake(this ipg_documenttype self)
        {
            return new Faker<ipg_documenttype>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => x.Name.FullName()); ;
        }
    }
}
