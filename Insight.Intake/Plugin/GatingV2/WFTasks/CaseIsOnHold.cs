using Insight.Intake.Plugin.GatingV2.CommonWfTask;

namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class CaseIsOnHold : WFTaskBase
    {
        public override WFTaskResult Run(WFTaskContext ctx)
        {
            return new WFTaskResult(ctx.dbContext.Case?.ipg_casehold != true);
        }
    }
}
