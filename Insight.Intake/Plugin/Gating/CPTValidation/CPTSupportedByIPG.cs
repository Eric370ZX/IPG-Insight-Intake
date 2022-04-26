using Insight.Intake.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.CPTValidation
{
    public class CPTSupportedByIPG : BasicCPTValidation
    {
        public CPTSupportedByIPG(Entity target, GateManager gateManager, IOrganizationService crmService) : base(target, gateManager, crmService)
        {

        }
        public override CPTValidationResult Validate()
        {
            if (!_cptCodes.Any(code => target.Contains(code)))
            {
                return new CPTValidationResult()
                {
                    ValidCPTCodes = new List<Guid>(),
                    Output = "No CPT codes"
                };
            }
            else
            {
                DateTime dos = target.GetAttributeValue<DateTime>(Incident.Fields.ipg_SurgeryDate);

                var cptRreferenses = GetNotEmptyCptReferences(target);
                var supportedCpts = cptRreferenses.Where(cpt => !CptIsNotImplantUser(cpt.Id)).ToList();

                if (!supportedCpts.Any())
                {
                    var caseNote = string.Join(",", supportedCpts.Select(x => $"Billable CPT {x.Name} contains and implant that is not supported by IPG. Case rejected and communication sent to facility. "));
                    var cim = gateManager.GetFacilityCIM(target.GetAttributeValue<EntityReference>("ipg_facilityid").Id);
                    var portalNote = $"IPG is unable to approve this case because the billable CPT, {string.Join(",", supportedCpts.Select(x => x.Name))}, does not include a supported implant. If the physician uses a supported implant, please notify {cim?.FullName} for reconsideration of this case.";
                    return new CPTValidationResult
                    {
                        ValidCPTCodes = new List<Guid>(),
                        Output = portalNote
                    };
                }
                else
                {
                    return new CPTValidationResult
                    {
                        ValidCPTCodes = supportedCpts.Select(x => x.Id).ToList(),
                    };
                }
            }
        }

        private bool CptIsNotImplantUser(Guid cptId)
        {
            var cptCode = crmService.Retrieve(ipg_cptcode.EntityLogicalName, cptId,
                new ColumnSet(ipg_cptcode.Fields.ipg_ImplantUsed, ipg_cptcode.Fields.ipg_supportedCPT)).ToEntity<ipg_cptcode>();

            return (cptCode.ipg_ImplantUsed?.Value != ImplantUsedOptionSet.No);
        }
    }
}
