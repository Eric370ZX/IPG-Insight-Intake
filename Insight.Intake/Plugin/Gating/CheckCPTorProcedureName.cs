using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckCPTorProcedureName : PluginBase
    {

        public CheckCPTorProcedureName() : base(typeof(CheckCPTorProcedureName))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckCPTorProcedureName", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            EntityReference targetRef = context.InputParameters["Target"] as EntityReference;

            context.OutputParameters["Succeeded"] = false;
            context.OutputParameters["CaseNote"] = string.Empty;
            context.OutputParameters["PortalNote"] = string.Empty;

            if (targetRef != null)
            {
                var fields = new string[7];
                if(string.Equals(targetRef.LogicalName, ipg_referral.EntityLogicalName))
                {
                    fields = new string[7] { ipg_referral.Fields.ipg_CPTCodeId1,
                                            ipg_referral.Fields.ipg_CPTCodeId2,
                                            ipg_referral.Fields.ipg_CPTCodeId3,
                                            ipg_referral.Fields.ipg_CPTCodeId4,
                                            ipg_referral.Fields.ipg_CPTCodeId5,
                                            ipg_referral.Fields.ipg_CPTCodeId6,
                                            ipg_referral.Fields.ipg_ProcedureNameId };
                }
                else if (string.Equals(targetRef.LogicalName, Incident.EntityLogicalName))
                {
                    fields = new string[7] { Incident.Fields.ipg_CPTCodeId1,
                                            Incident.Fields.ipg_CPTCodeId2,
                                            Incident.Fields.ipg_CPTCodeId3,
                                            Incident.Fields.ipg_CPTCodeId4,
                                            Incident.Fields.ipg_CPTCodeId5,
                                            Incident.Fields.ipg_CPTCodeId6,
                                            Incident.Fields.ipg_procedureid };
                }
                Entity target = service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(fields));
                if (target.GetAttributeValue<EntityReference>(fields[0]) != null
                        || target.GetAttributeValue<EntityReference>(fields[1]) != null
                        || target.GetAttributeValue<EntityReference>(fields[2]) != null
                        || target.GetAttributeValue<EntityReference>(fields[3]) != null
                        || target.GetAttributeValue<EntityReference>(fields[4]) != null
                        || target.GetAttributeValue<EntityReference>(fields[5]) != null)
                {
                    context.OutputParameters["Succeeded"] = true;
                    context.OutputParameters["CodeOutput"] = 2;
                }
                else if (target.GetAttributeValue<EntityReference>(fields[6]) != null)
                {
                    context.OutputParameters["Succeeded"] = true;
                    context.OutputParameters["CodeOutput"] = 1;
                }
            }
        }
    }
}
