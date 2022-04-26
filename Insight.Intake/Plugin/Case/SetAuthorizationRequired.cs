using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Case
{
    public class SetAuthorizationRequired : PluginBase
    {
        public SetAuthorizationRequired() : base(typeof(SetAuthorizationRequired))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localContext)
        {
            var incident = localContext.Target<Incident>();

            Intake.Account mainCarrier = null;
            if (incident.ipg_CarrierId != null)
            {
                mainCarrier = localContext.SystemOrganizationService
                    .Retrieve(incident.ipg_CarrierId.LogicalName, incident.ipg_CarrierId.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType))
                    .ToEntity<Intake.Account>();
            }

            Intake.Account secondaryCarrier = null;
            if (incident.ipg_SecondaryCarrierId != null)
            {
                secondaryCarrier = localContext.SystemOrganizationService
                    .Retrieve(incident.ipg_SecondaryCarrierId.LogicalName, incident.ipg_SecondaryCarrierId.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType))
                    .ToEntity<Intake.Account>();
            }

            var isPrimaryCarrierAuto = mainCarrier?.ipg_CarrierTypeEnum == ipg_CarrierType.Auto;
            var isSecondaryCarrierAuto = secondaryCarrier?.ipg_CarrierTypeEnum == ipg_CarrierType.Auto;
            if (isPrimaryCarrierAuto || isSecondaryCarrierAuto)
            {
                incident.ipg_is_authorization_required = false;
            }
        }
    }
}
