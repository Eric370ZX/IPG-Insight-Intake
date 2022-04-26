using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.Common
{
    public class PreAuthGatingManager
    {
        private readonly IOrganizationService crmService;
        private readonly ITracingService tracingService;
        private readonly EntityReference _caseRef;
        public PreAuthGatingManager(IOrganizationService organizationService, ITracingService tracingService, EntityReference caseRef)
        {
            this.crmService = organizationService;
            this.tracingService = tracingService;
            this._caseRef = caseRef;
        }
        public GatingResponse ValidateWithCPT(IEnumerable<Task> existingTasks, Func<IEnumerable<ipg_cptcode>,GatingResponse> validate)
        {
            if (existingTasks.Any(p => p.StateCode != TaskState.Open))
            {
                return new GatingResponse(true);
            }
            var cptCodesFields = new[] { "ipg_cptcodeid1", "ipg_cptcodeid2", "ipg_cptcodeid3", "ipg_cptcodeid4", "ipg_cptcodeid5", "ipg_cptcodeid6" };
            var sourceCase = crmService
                .Retrieve(_caseRef.LogicalName, _caseRef.Id, new ColumnSet(cptCodesFields));
            Func<EntityReference, ipg_cptcode> getCpt = (EntityReference cptRef) => cptRef != null ? crmService.Retrieve(cptRef.LogicalName, cptRef.Id, new ColumnSet(true)).ToEntity<ipg_cptcode>() : null;
            var cptCodes = cptCodesFields
                .Select(p => getCpt(sourceCase.GetAttributeValue<EntityReference>(p)))
                .Where(p => p != null);
            return validate(cptCodes);
        }

        public GatingResponse SimpleValidate(IEnumerable<Task> existingTasks, Func<GatingResponse> validate)
        {
            if (existingTasks.Any(p => p.StateCode != TaskState.Open))
            {
                return new GatingResponse(true);
            }
            return validate();
        }
    }
}
