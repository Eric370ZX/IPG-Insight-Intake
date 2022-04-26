using Insight.Intake.Plugin.GatingV2.CommonWfTask;


namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class IsProcedureNameNotApproved : WFTaskBase
    {

        public override WFTaskResult Run(WFTaskContext ctx)
        {
            var tracingService = ctx.TraceService;
            var gatingResponse = new WFTaskResult(false);

            if (ctx.dbContext.Referral != null)
            { // validation for referral moved to method in CheckReferralForm class
                gatingResponse.Succeeded = true;
            }
            else
            {
                var procedure = ctx.dbContext.Case.ipg_procedureid_Entity;
                if (procedure != null)
                {
                    var caseNote = string.Format("Procedurev Name {0} is not approved", procedure.ipg_name);
                    if (procedure.StatusCodeEnum == ipg_procedurename_StatusCode.Active)
                    {
                        gatingResponse.Succeeded = true;
                    }
                    else
                    {
                        gatingResponse.Succeeded = false;
                        gatingResponse.CaseNote = caseNote;
                    }

                }
                else
                {
                    gatingResponse.Succeeded = true;
                }
            }
            return gatingResponse;
        }
    }
}