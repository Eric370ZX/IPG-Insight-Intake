using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.Plugin.Gating
{
    public class ValidateEhrPatientAddress : GatingPluginBase
    {
        public ValidateEhrPatientAddress() : base("ipg_IPGGatingValidateEhrPatientAddress") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var targetRef = ctx.TargetRef();

            if (targetRef.LogicalName != ipg_referral.EntityLogicalName
                && targetRef.LogicalName != Incident.EntityLogicalName)
            {
                return new GatingResponse(true);
            }

            using (var orgServiceContext = new OrganizationServiceContext(crmService))
            {
                var task = (from t in orgServiceContext.CreateQuery<Task>()
                                        join tt in orgServiceContext.CreateQuery<ipg_tasktype>() on t.ipg_tasktypeid.Id equals tt.Id
                                        where t.RegardingObjectId != null && t.RegardingObjectId.Id == targetRef.Id
                                            && t.StateCode == TaskState.Open
                                            && tt.ipg_typeid == (int)TaskTypeIds.MISSING_OR_INVALID_PATIENT_ADDRESS
                            select t).FirstOrDefault();
                if (task != null)
                {
                    string negativeResponseMessage;
                    if(string.IsNullOrWhiteSpace(task.ipg_metatag) == false)
                    {
                        negativeResponseMessage = task.ipg_metatag;
                    }
                    else
                    {
                        negativeResponseMessage = task.Description;
                    }

                    return new GatingResponse
                    {
                        Succeeded = false,
                        CustomMessage = negativeResponseMessage
                    };
                }
            }

            return new GatingResponse
            {
                Succeeded = true,
                CustomMessage = "Patient Address Values Provided and Valid"
            };
        }
    }
}
