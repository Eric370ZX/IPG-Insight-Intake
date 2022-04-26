using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeQueueItem
    {
        public static Faker<QueueItem> Fake(this QueueItem self)
        {
            return new Faker<QueueItem>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<QueueItem> WithQueue(this Faker<QueueItem> self, Queue queue)
        {
            self.RuleFor(x => x.QueueId, x => queue.ToEntityReference());
            return self;
        }

        public static Faker<QueueItem> WithItem(this Faker<QueueItem> self, Entity item)
        {
            self.RuleFor(x => x.ObjectId, x => item.ToEntityReference());
            return self;
        }

        public static Faker<QueueItem> WithWorker(this Faker<QueueItem> self, Entity worker)
        {
            self.RuleFor(x => x.WorkerId, x => worker.ToEntityReference());
            return self;
        }
    }
}
