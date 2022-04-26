using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class GateProcessingPostAction : PluginBase
    {
        public GateProcessingPostAction() : base(typeof(GateProcessingPostAction))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingGateProcessingPostAction", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var _context = localPluginContext.PluginExecutionContext;
            var targetRef = _context.InputParameters["Target"] as EntityReference;
            var _service = localPluginContext.OrganizationService;
            var _tracingService = localPluginContext.TracingService;
            var target = _service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(true));
            EntityReference _caseRef = null ;
            if (target.LogicalName == Incident.EntityLogicalName)
            {
                _caseRef = target.ToEntityReference();
            }
            else if (target.LogicalName == ipg_referral.EntityLogicalName)
            {
                _caseRef = target.Contains("ipg_associatedcaseid") ? target.GetAttributeValue<EntityReference>("ipg_associatedcaseid") : null;
            }
            if (_caseRef == null && target.LogicalName == Incident.EntityLogicalName)
            {
                throw new InvalidPluginExecutionException("Unknown primary entity");
            }

            var caseGateExecutionId = GetCaseGateExecutionId(localPluginContext, target);
            var gateManagerPostAction = new GateManagerPostAction(target, _caseRef, _service, _tracingService, _context);
            gateManagerPostAction.StartCasePostGateExecution(caseGateExecutionId);
            gateManagerPostAction.RunPostActionDelayed();
            gateManagerPostAction.FinishCasePostGateExecution(caseGateExecutionId);
        }

        private Guid? GetCaseGateExecutionId(LocalPluginContext context, Entity target)
        {
            var crmContext = new OrganizationServiceContext(context.OrganizationService);
            var sessionId = target.GetAttributeValue<string>("ipg_cr_gatesessionid");
            var caseGateExecution = (from cge in crmContext.CreateQuery<ipg_casegateexecution>()
                                    where cge.ipg_SessionID.Equals(sessionId)
                                    select cge).FirstOrDefault();
            if(caseGateExecution != null)
            {
                return caseGateExecution.Id;
            }
            return null;
        }
    }
}
