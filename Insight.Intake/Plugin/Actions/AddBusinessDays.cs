using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using Insight.Intake.Helpers;

namespace Insight.Intake.Plugin.Actions
{
    public class AddBusinessDays : PluginBase
    {
        public AddBusinessDays() : base(typeof(AddBusinessDays))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeActionsAddBusinessDays", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            DateTime originalDate = DateTime.Now;
            if (context.InputParameters.Contains("StartDate") && context.InputParameters["StartDate"] is DateTime)
            {
                originalDate = (DateTime)context.InputParameters["StartDate"];
            }

            int businessDaysToAdd = 0;
            if (context.InputParameters.Contains("BusinessDaysToAdd") && context.InputParameters["BusinessDaysToAdd"] is int)
            {
                businessDaysToAdd =  (int)context.InputParameters["BusinessDaysToAdd"];
            }

            Entity calendar = null;
            if (context.InputParameters.Contains("ClosuresCalendar") && context.InputParameters["ClosuresCalendar"] is EntityReference)
            {
                EntityReference holidaySchedule = (EntityReference)context.InputParameters["ClosuresCalendar"];
                if (holidaySchedule != null)
                {
                    calendar = localPluginContext.OrganizationService.Retrieve("calendar", holidaySchedule.Id, new ColumnSet(true));
                }
            }

            DateTime tempDate = originalDate;

            if (businessDaysToAdd > 0)
            {
                while (businessDaysToAdd > 0)
                {
                    tempDate = tempDate.AddDays(1);

                    if (tempDate.IsBusinessDay(calendar))
                    {
                        // Only decrease the days to add if the day we've just added counts as a business day
                        businessDaysToAdd--;
                    }
                }
            }
            else if (businessDaysToAdd < 0)
            {
                while (businessDaysToAdd < 0)
                {
                    tempDate = tempDate.AddDays(-1);

                    if (tempDate.IsBusinessDay(calendar))
                    {
                        // Only increase the days to add if the day we've just added counts as a business day
                        businessDaysToAdd++;
                    }
                }
            }
            context.OutputParameters["ResultDate"] = tempDate;
        }
    }
}
