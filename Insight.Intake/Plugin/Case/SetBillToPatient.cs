using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class SetBillToPatient : PluginBase
    {
        public SetBillToPatient() : base(typeof(SetBillToPatient))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationCreateHandler);
        }

        void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            var incident = localPluginContext.Target<Incident>();

            bool billToPatient = false;
            if (incident.ipg_CarrierId != null)
            {
                var account = service.Retrieve(incident.ipg_CarrierId.LogicalName, incident.ipg_CarrierId.Id, new ColumnSet(nameof(Intake.Account.ipg_StatementToPatient).ToLower()));
                billToPatient = account.GetAttributeValue<bool>(nameof(Intake.Account.ipg_StatementToPatient).ToLower());
            }

            if (incident.ipg_FacilityId != null)
            {
                var dos = incident.GetCaseDos();
                var queryExpression = new QueryExpression(Entitlement.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(Entitlement.Fields.ipg_Billtopatient),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(Entitlement.ipg_EntitlementType).ToLower(), ConditionOperator.Equal, (int)ipg_EntitlementTypes.FacilityCarrier),
                            new ConditionExpression(nameof(Entitlement.ipg_CarrierId).ToLower(), ConditionOperator.Equal,  incident.ipg_CarrierId.Id),
                            new ConditionExpression(nameof(Entitlement.ipg_FacilityId).ToLower(), ConditionOperator.Equal,  incident.ipg_FacilityId.Id),
                            new ConditionExpression(nameof(Entitlement.StartDate).ToLower(), ConditionOperator.LessEqual, dos),
                            new ConditionExpression(nameof(Entitlement.EndDate).ToLower(), ConditionOperator.GreaterEqual, dos)
                        }
                    }
                };
                EntityCollection entitlements = service.RetrieveMultiple(queryExpression);
                var facilityCarrier = entitlements.Entities.FirstOrDefault();
                if (facilityCarrier != null){
                    billToPatient = facilityCarrier.ToEntity<Entitlement>().ipg_Billtopatient.Value;
                }
            }

            incident.ipg_BillToPatient = new OptionSetValue((billToPatient)
                                                ? (int)ipg_TwoOptions.Yes : (int)ipg_TwoOptions.No);
        }

    }
}
