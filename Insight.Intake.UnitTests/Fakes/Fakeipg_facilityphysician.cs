using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class Fakeipg_facilityphysician
    {
        public static Faker<ipg_facilityphysician> Fake(this ipg_facilityphysician self)
        {
            return new Faker<ipg_facilityphysician>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => x.Name.FullName()); ;
        }
    }
}
