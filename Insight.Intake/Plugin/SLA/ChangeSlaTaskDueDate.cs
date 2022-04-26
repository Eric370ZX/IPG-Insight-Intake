using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using System;

namespace Insight.Intake.Plugin.SLA
{
    public class ChangeSlaTaskDueDate : PluginBase
    {
        public ChangeSlaTaskDueDate() : base(typeof(ChangeSlaTaskDueDate))
        {
            RegisterEvent(
                PipelineStages.PreOperation,
                MessageNames.Update,
                Task.EntityLogicalName,
                OnTaskUpdate);
        }

        private void OnTaskUpdate(LocalPluginContext pluginContext)
        {
            var target = pluginContext.Target<Task>();
            if (target.Contains(Task.Fields.ScheduledStart) && target.ScheduledStart != null)
            {
                var preImage = pluginContext.PreImage<Task>();
                if (preImage != null && preImage.Contains(Task.Fields.ipg_taskcategoryid) 
                    && preImage.ipg_taskcategoryid != null && preImage.ipg_taskcategoryid.Name == Constants.TaskCategoryNames.SLA)
                {
                    var taskManager = new TaskManager(pluginContext.OrganizationService,
                    pluginContext.TracingService,
                    null,
                    pluginContext.PluginExecutionContext.InitiatingUserId);

                    preImage.ScheduledStart = target.ScheduledStart;

                    var newTask = taskManager.ConfigureTaskByTaskType(preImage);
                    var time = DateTime.Now.TimeOfDay;
                    target.ScheduledStart = target.ScheduledStart.Value.Date + time;
                    target.ScheduledEnd = newTask.ScheduledEnd.Value.Date + time;
                }
            }
        }
    }
}