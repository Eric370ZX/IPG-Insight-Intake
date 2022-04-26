using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeTaskType
    {
        public static Faker<ipg_tasktype> Fake(this ipg_tasktype self)
        {
            return new Faker<ipg_tasktype>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_tasktype> WithName(this Faker<ipg_tasktype> self, string name)
        {
            return self
                .RuleFor(x => x.ipg_name, x => name);
        }

        public static Faker<ipg_tasktype> WithStartDate(this Faker<ipg_tasktype> self, int startdate)
        {
            return self
                .RuleFor(x => x.ipg_startdate, x => startdate);
        }

        public static Faker<ipg_tasktype> WithDueDate(this Faker<ipg_tasktype> self, int dueDate)
        {
            return self
                .RuleFor(x => x.ipg_duedate, x => dueDate);
        }

        public static Faker<ipg_tasktype> WithAssignToteam(this Faker<ipg_tasktype> self, EntityReference teamRef)
        {
            return self
                .RuleFor(x => x.ipg_assigntoteam, x => teamRef);

        }
        public static Faker<ipg_tasktype> WithCategory(this Faker<ipg_tasktype> self, ipg_taskcategory taskcategory)
        {
            return self
                .RuleFor(x => x.ipg_taskcategoryid, x => taskcategory.ToEntityReference());
        }

        public static Faker<ipg_tasktype> WithTypeId(this Faker<ipg_tasktype> self, TaskTypeIds typeId)
        {
            return self
                .RuleFor(x => x.ipg_typeid, x => (int)typeId);
        }

        public static Faker<ipg_tasktype> WithDescription(this Faker<ipg_tasktype> self, string description)
        {
            return self.RuleFor(x => x.ipg_description, x => description);
        }
    }
}
