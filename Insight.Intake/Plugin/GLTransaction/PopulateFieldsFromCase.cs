using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.GLTransaction
{
    public class PopulateFieldsFromCase : PluginBase
    {
        public PopulateFieldsFromCase() : base(typeof(PopulateFieldsFromCase))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_GLTransaction.EntityLogicalName, SetFieldsFromCase);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_GLTransaction.EntityLogicalName, SetFieldsFromCase);
        }

        private void SetFieldsFromCase(LocalPluginContext context)
        {
            var gLTransaction = context.Target<ipg_GLTransaction>();

            var incident = gLTransaction.ipg_IncidentId != null
                ? GetIncidentFromTransaction(context.OrganizationService, gLTransaction.ipg_IncidentId.Id)
                : null;
            gLTransaction.ipg_NetworkType = incident?.ipg_carriernetwork?.Name;
        }

        private Incident GetIncidentFromTransaction(IOrganizationService context, Guid caseId)
        {
            var orgContext = new OrganizationServiceContext(context);
            return orgContext.CreateQuery<Incident>().
                FirstOrDefault(incident => incident.Id == caseId && incident.ipg_carriernetwork != null);
        }
    }
}
