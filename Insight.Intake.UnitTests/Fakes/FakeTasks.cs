using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeTasks
    {
        public static Task CreateTaskForDeriveHomePlan(this Task task,string subject, string description, Incident incident)
        {
            return new Task
            {
                Subject = subject,
                Id = Guid.NewGuid(),
                Description = description,
                RegardingObjectId = new EntityReference
                {
                    Id = incident.Id,
                    LogicalName = incident.LogicalName
                }
            };
        }
    }
}
