using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Extensions
{
    public static class PluginExecutionContextExtenstions
    {
        public static T GetTarget<T>(this IPluginExecutionContext context)  where T : Entity
        {
            if (context.InputParameters.ContainsKey("Target") && context.InputParameters["Target"] is Entity entityRecord)
            {
                return entityRecord.ToEntity<T>();
            }

            throw new InvalidPluginExecutionException("Target parameter doesn't exist in PluginContext");
        }
    }
}
