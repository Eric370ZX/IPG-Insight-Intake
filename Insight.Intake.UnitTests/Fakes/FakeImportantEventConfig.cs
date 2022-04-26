using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeImportantEventConfig
    {
        public static Faker<ipg_importanteventconfig> Fake(this ipg_importanteventconfig self, string eventId, string eventType, string eventDescription, string eventtrigger)
        {
            return new Faker<ipg_importanteventconfig>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => eventId)
                .RuleFor(x => x.ipg_eventtype, x => eventType)
                .RuleFor(x => x.ipg_eventdescription, x => eventDescription)
                .RuleFor(x => x.ipg_eventnote, x => eventtrigger);
        }
    }
}
