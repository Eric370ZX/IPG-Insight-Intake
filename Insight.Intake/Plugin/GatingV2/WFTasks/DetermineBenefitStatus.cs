using Insight.Intake.Extensions;
using Insight.Intake.Plugin.GatingV2.CommonWfTask;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class DetermineBenefitStatus : WFTaskBase
    {
        public override WFTaskResult Run(WFTaskContext ctx)
        {
            var service = ctx.CrmService;
            var tracingService = ctx.TraceService;

            var gatingResponse = new WFTaskResult(false);
            Entity target = ctx.dbContext.Referral != null
                ? ctx.dbContext.Referral.ToEntity<Entity>()
                : ctx.dbContext.Case.ToEntity<Entity>();
            var targetRef = target.ToEntityReference();
            Intake.Account carrier = ctx.dbContext.Referral != null
                ? ctx.dbContext.Referral.ipg_CarrierId_Entity
                : ctx.dbContext.Case.ipg_CarrierId_Entity;

            var dos = target.GetCaseDos();
            if (carrier == null || dos == null)
            {
                UpdateTarget(service, target.ToEntityReference(), false);
                return gatingResponse;
            }
            var caseNote = string.Format("Carrier {0}  is not a contracted carrier. Please verify benefits.", carrier.Name);
            gatingResponse.CaseNote = caseNote;
            var carrierNetwork = carrier.ipg_carriernetworkid_Entity;
            if (carrierNetwork == null)
            {
                UpdateTarget(service, target.ToEntityReference(), false);
                return gatingResponse;
            }

            if (!carrierNetwork.ipg_EffectiveDate.HasValue || !carrierNetwork.ipg_ExpirationDate.HasValue)
            {
                UpdateTarget(service, targetRef, false);
                return gatingResponse;
            }

            if (dos > carrierNetwork.ipg_EffectiveDate.Value && dos < carrierNetwork.ipg_ExpirationDate.Value)
            {
                UpdateTarget(service, targetRef, true);
                caseNote = string.Format("Carrier {0} is a contracted carrier; {1} benefits will be displayed in the case.", carrier.Name, carrierNetwork.ipg_name);
                gatingResponse.Succeeded = true;
                gatingResponse.CaseNote = caseNote;
                return gatingResponse;
            }

            UpdateTarget(service, targetRef, false);
            return gatingResponse;
        }


        private void UpdateTarget(IOrganizationService service, EntityReference targetRef, bool inNetwork)
        {
            Entity targetToUpdate = new Entity(targetRef.LogicalName);
            targetToUpdate.Id = targetRef.Id;
            targetToUpdate["ipg_inoutnetwork"] = inNetwork;
            service.Update(targetToUpdate);
        }
    }
}