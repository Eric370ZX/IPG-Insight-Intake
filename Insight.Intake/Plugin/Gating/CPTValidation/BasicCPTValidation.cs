using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.CPTValidation
{
    public abstract class BasicCPTValidation
    {
        protected Entity target;
        protected readonly GateManager gateManager;
        protected readonly IOrganizationService crmService;
        protected readonly string[] _cptCodes =
            {
            Incident.Fields.ipg_CPTCodeId1,
            Incident.Fields.ipg_CPTCodeId2,
            Incident.Fields.ipg_CPTCodeId3,
            Incident.Fields.ipg_CPTCodeId4,
            Incident.Fields.ipg_CPTCodeId5,
            Incident.Fields.ipg_CPTCodeId6
            };
        protected readonly string[] _caseRequiredColumns = {
            Incident.Fields.ipg_BilledCPTId,
            Incident.Fields.ipg_ActualDOS,
            Incident.Fields.ipg_SurgeryDate,
            Incident.Fields.ipg_CarrierId,
            Incident.Fields.ipg_FacilityId
        };
        protected List<EntityReference> GetNotEmptyCptReferences(Entity target) => 
            _cptCodes
                .Where(code => target.Contains(code) && target[code] != null)
                .Select(code => target.GetAttributeValue<EntityReference>(code))
                .ToList();
        public BasicCPTValidation(Entity target, GateManager gateManager, IOrganizationService crmService)
        {
            this.target = target;
            this.gateManager = gateManager;
            this.crmService = crmService;
        }
       
        public abstract CPTValidationResult Validate();
    }
}
