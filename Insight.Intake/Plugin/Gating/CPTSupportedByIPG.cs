using Insight.Intake.Data;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Insight.Intake.Plugin.Gating
{
    /// <summary>
    /// Deprecated. logic is moved to CptCodesValidation
    /// </summary>
    [Obsolete]
    public class CPTSupportedByIPG : PluginBase
    {
        private readonly string[] cptCodes =
{
            "ipg_cptcodeid1",
            "ipg_cptcodeid2",
            "ipg_cptcodeid3",
            "ipg_cptcodeid4",
            "ipg_cptcodeid5",
            "ipg_cptcodeid6"
        };

        public CPTSupportedByIPG() : base(typeof(CPTSupportedByIPG))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCPTSupportedByIPG", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            EntityReference targetRef = context.InputParameters["Target"] as EntityReference;
            var gateManager = new GateManager(service, tracingService, targetRef);

            context.OutputParameters["Succeeded"] = false;
            context.OutputParameters["CaseNote"] = string.Empty;
            context.OutputParameters["PortalNote"] = string.Empty;

            if (targetRef != null)
            {
                Entity target = service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(
                    "ipg_surgerydate",
                    "ipg_cptcodeid1",
                    "ipg_cptcodeid2",
                    "ipg_cptcodeid3",
                    "ipg_cptcodeid4",
                    "ipg_cptcodeid5",
                    "ipg_cptcodeid6",
                    "ipg_facilityid"));

                if (!target.Contains("ipg_cptcodeid1") && !target.Contains("ipg_cptcodeid2") && !target.Contains("ipg_cptcodeid3") && !target.Contains("ipg_cptcodeid4") && !target.Contains("ipg_cptcodeid5") && !target.Contains("ipg_cptcodeid6"))
                {
                    context.OutputParameters["Succeeded"] = true;
                    return;
                }
                else
                {
                    DateTime dos = target.GetAttributeValue<DateTime>("ipg_surgerydate");

                    bool supportedCPTExists = false;
                    var notSupportedCpts = new List<string>();
                    foreach (var cptCode in cptCodes)
                    {
                        if (target.Contains(cptCode))
                        {
                            var cptRef = target.GetAttributeValue<EntityReference>(cptCode);
                            if (CptSupported(service, cptRef))
                            {
                                supportedCPTExists = true;
                                break;
                            }
                            else
                            {
                                notSupportedCpts.Add(cptRef.Name);
                            }
                        }
                    }

                    if (!supportedCPTExists)
                    {
                        var caseNote = string.Join(",", notSupportedCpts.Select(x => string.Format("Billable CPT {0} contains and implant that is not supported by IPG. Case rejected and communication sent to facility. ", x)));
                        context.OutputParameters["CaseNote"] = caseNote;
                        var cim = gateManager.GetFacilityCIM(target.GetAttributeValue<EntityReference>("ipg_facilityid").Id);
                        var portalNote = string.Format(@"IPG is unable to approve this case because the billable CPT, {0}, does not include a supported implant. If the physician uses a supported implant, please notify {1} for reconsideration of this case.", string.Join(",", notSupportedCpts), cim?.FullName);
                        context.OutputParameters["PortalNote"] = portalNote;
                    }
                    else
                    {
                        context.OutputParameters["Succeeded"] = true;
                    }
                }
            }
        }

        private bool CptSupported(IOrganizationService service, EntityReference cptRef)
        {
            var cptCode = service.Retrieve(ipg_cptcode.EntityLogicalName, cptRef.Id, 
                new ColumnSet(LogicalNameof<ipg_cptcode>.Property(x => x.ipg_ImplantUsed))).ToEntity<ipg_cptcode>();

            if(cptCode != null)
            {
                if (cptCode.ipg_ImplantUsed?.Value != ImplantUsedOptionSet.No)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
