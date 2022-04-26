using Insight.Intake.Plugin.GatingV2.CommonWfTask;

namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class IsAutoCarrier : WFTaskBase
    {
        public override WFTaskResult Run(WFTaskContext ctx)
        {
            Intake.Account carrier = null;
            Intake.Account secondaryCarrier = null;
            if (ctx.dbContext.Case != null)
            {
                carrier = ctx.dbContext.Case.ipg_CarrierId_Entity;
                secondaryCarrier = ctx.dbContext.Case.ipg_SecondaryCarrierId_Entity;
            }
            else if (ctx.dbContext.Referral != null)
            {
                carrier = ctx.dbContext.Referral.ipg_CarrierId_Entity;
            }
            else
            {
                return new WFTaskResult(true);
            }
            if (carrier != null)
            {
                var result = HandleCarrier(carrier);
                if (result != null)
                {
                    return result;
                }
            }
            if (secondaryCarrier != null)
            {
                var result = HandleCarrier(secondaryCarrier);
                if (result != null)
                {
                    return result;
                }
            }

            return new WFTaskResult(true);

        }
        private WFTaskResult HandleCarrier(Intake.Account carrier)
        {
            if (carrier.ipg_CarrierType?.Value == (int)ipg_CarrierType.Auto)
            {
                return new WFTaskResult(false, "Referral requires special processing")
                {
                    TaskSubject = "Check for Auto Carrier.",
                    TaskDescripton = $"Auto Carrier, {carrier.Name}, is present on the Case. Confirm if benefits are exhausted, if IPG can consider this Case and in what position the Auto Carrier belongs."
                };
            }
            return null;
        }
    }
}
