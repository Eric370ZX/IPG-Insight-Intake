using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class OpenCaseAction : PluginBase
    {
        public OpenCaseAction() : base(typeof(OpenCaseAction))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGCaseOpen", Intake.Incident.EntityLogicalName, ProcessOpenCase);
        }

        private void ProcessOpenCase(LocalPluginContext context)
        {
            var tracing = context.TracingService;
            var service = context.OrganizationService;
       
            var incidentRef = context.TargetRef();

            tracing.Trace($"Case about to be reopened ${incidentRef.Id}");

            var incidentEnt = service.Retrieve(incidentRef.LogicalName, incidentRef.Id, new ColumnSet(Incident.Fields.ipg_lifecyclestepid
                , Incident.Fields.ipg_CaseStatus)).ToEntity<Intake.Incident>();

            tracing.Trace($"Check that Case Closed");

            if (incidentEnt.ipg_CaseStatusEnum == ipg_CaseStatus.Closed)
            {
                if (incidentEnt.ipg_lifecyclestepid == null)
                {
                    throw new InvalidPluginExecutionException("Case does not have Life Cycle Step!");
                }

                var reopenConfig = service.RetrieveMultiple(new QueryExpression(ipg_gateprocessingrule.EntityLogicalName)
                {
                    TopCount = 1,
                    ColumnSet = new ColumnSet(ipg_gateprocessingrule.Fields.ipg_nextlifecyclestepid
                    , ipg_gateprocessingrule.Fields.ipg_providerstatus
                    , ipg_gateprocessingrule.Fields.ipg_casestate
                    , ipg_gateprocessingrule.Fields.ipg_casestatus
                    , ipg_gateprocessingrule.Fields.ipg_casestatusdisplayedid),

                    Criteria = new FilterExpression()
                    {
                        Conditions =
                        {
                            new ConditionExpression(ipg_gateprocessingrule.Fields.StateCode, ConditionOperator.Equal, (int)ipg_gateprocessingruleState.Active)
                            , new ConditionExpression(ipg_gateprocessingrule.Fields.ipg_lifecyclestepid, ConditionOperator.Equal, incidentEnt.ipg_lifecyclestepid.Id)
                            , new ConditionExpression(ipg_gateprocessingrule.Fields.ipg_actioncode, ConditionOperator.Equal, (int)ipg_gateprocessingrule_ipg_actioncode.ReOpen)
                        }
                    }
                }).Entities.FirstOrDefault()?.ToEntity<ipg_gateprocessingrule>();

                var incidentUpdate = new Incident()
                {
                    Id = incidentEnt.Id,
                    ipg_CaseStatusEnum = ipg_CaseStatus.Open,
                    StatusCodeEnum = Incident_StatusCode.InProgress,
                    StateCode = IncidentState.Active
                };

                if (reopenConfig != null)
                {

                    tracing.Trace($"Gate Processing Rule ({reopenConfig.Id}) found for this Case");
                    var nextGateConfig = reopenConfig.ipg_nextlifecyclestepid != null ?
                        service.Retrieve(reopenConfig.ipg_nextlifecyclestepid.LogicalName, reopenConfig.ipg_nextlifecyclestepid.Id, new ColumnSet(ipg_lifecyclestep.Fields.ipg_gateconfigurationid))
                        .ToEntity<ipg_lifecyclestep>().ipg_gateconfigurationid
                        : null;
                    incidentEnt.ipg_gateconfigurationid = nextGateConfig;
                    incidentUpdate.ipg_lifecyclestepid = reopenConfig.ipg_nextlifecyclestepid;
                    incidentUpdate.ipg_StateCode = reopenConfig.ipg_casestate;
                    incidentUpdate.ipg_CaseStatus = reopenConfig.ipg_casestatus;
                    incidentUpdate.ipg_casestatusdisplayedid = reopenConfig.ipg_casestatusdisplayedid;
                    incidentUpdate.ipg_providerstatus = reopenConfig.ipg_providerstatus;
                }
                else
                {
                    tracing.Trace($"Gate Processing Rule not  found for this Case with Life Cycle Step {incidentEnt.ipg_lifecyclestepid?.Id}");
                    tracing.Trace($"Case will be Opened without Change to Life Cycle Step, Case Status, Case State, Case Status Displayed, Provider Status");
                }

                tracing.Trace($"Update Case");

                service.Update(incidentUpdate);

                var retrievedCase = service.Retrieve(incidentRef.LogicalName, incidentRef.Id, new ColumnSet(true)).ToEntity<Incident>();
            }
            else
            {
                throw new InvalidPluginExecutionException("Case not closed!");
            }
        }
    }
}
