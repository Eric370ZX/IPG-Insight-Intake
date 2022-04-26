using Bogus;
using System;
namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeContactAccount
    {
        public static Faker<ipg_contactsaccounts> Fake(this ipg_contactsaccounts self, Contact contact= null, Account account = null)
        {
            return new Faker<ipg_contactsaccounts>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_contactid, x => contact != null ? contact.ToEntityReference(): null)
                .RuleFor(x => x.ipg_accountid, x => account != null ? account.ToEntityReference(): null
                );
        }
    }
}
