using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Authorization
{
    /// <summary>
    /// Recent Authorization is set from JS now
    /// </summary>
    public class CaseRecentAuthorization : PluginBase
    {
        public CaseRecentAuthorization() : base(typeof(CaseRecentAuthorization))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                MessageNames.Create,
                ipg_authorization.EntityLogicalName,
                OnCreate);
            RegisterEvent(
                PipelineStages.PostOperation,
                MessageNames.Update,
                Incident.EntityLogicalName,
                OnCaseUpdate);
        }

        private void OnCreate(LocalPluginContext pluginContext)
        {
            ipg_authorization target = pluginContext.PluginExecutionContext.GetTarget<ipg_authorization>();

            if (target.ipg_incidentid != null)
            {
                Incident incident = pluginContext.SystemOrganizationService.Retrieve<Incident>(
                    target.ipg_incidentid.Id, 
                    new ColumnSet(Incident.Fields.ipg_CarrierId, Incident.Fields.ipg_SecondaryCarrierId));

                if (target.ipg_carrierid != null)
                {
                    if (incident.ipg_CarrierId != null && incident.ipg_CarrierId.Id == target.ipg_carrierid.Id)
                    {
                        pluginContext.OrganizationService.Update(new Incident()
                        {
                            Id = target.ipg_incidentid.Id,
                            ipg_AuthorizationId = target.ToEntityReference()
                        });
                    }
                    else if (incident.ipg_SecondaryCarrierId != null && incident.ipg_SecondaryCarrierId.Id == target.ipg_carrierid.Id)
                    {
                        pluginContext.OrganizationService.Update(new Incident()
                        {
                            Id = target.ipg_incidentid.Id,
                            ipg_secondaryauthorizationid = target.ToEntityReference()
                        });
                    }
                }

                if (target.Contains(ipg_authorization.Fields.ipg_isauthrequired) && target.ipg_isauthrequired != null)
                {
                    pluginContext.OrganizationService.Update(new Incident()
                    {
                        Id = target.ipg_incidentid.Id,
                        ipg_is_authorization_required = target.ipg_isauthrequired
                    });
                }
            }
        }

        private void OnCaseUpdate(LocalPluginContext pluginContext)
        {
            Incident target = pluginContext.Target<Incident>();
            var service = pluginContext.OrganizationService;
            var caseupdate = new Incident() { Id = target.Id };
            var shouldupdate = false;
            
            if (target.ipg_CarrierId != null)
            {
                var authrecord = service.RetrieveMultiple(new QueryExpression(ipg_authorization.EntityLogicalName)
                {
                    TopCount = 1,
                    ColumnSet = new ColumnSet(false),
                    Orders = {new OrderExpression(ipg_authorization.Fields.CreatedOn, OrderType.Descending) },
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression(ipg_authorization.Fields.ipg_carrierid, ConditionOperator.Equal, target.ipg_CarrierId .Id),
                            new ConditionExpression(ipg_authorization.Fields.ipg_incidentid, ConditionOperator.Equal, target.Id),
                            new ConditionExpression(ipg_authorization.Fields.StateCode, ConditionOperator.Equal, (int)ipg_authorizationState.Active),
                            new ConditionExpression(ipg_authorization.Fields.ipg_facilityauthnumber, ConditionOperator.NotNull),
                        }
                    }
                }).Entities.FirstOrDefault();

                shouldupdate = true;
                caseupdate.ipg_AuthorizationId = authrecord != null ? authrecord.ToEntityReference() : null;
            }

            if(target.ipg_SecondaryCarrierId != null)
            {
                var authrecord = service.RetrieveMultiple(new QueryExpression(ipg_authorization.EntityLogicalName)
                {
                    TopCount = 1,
                    ColumnSet = new ColumnSet(false),
                    Orders = { new OrderExpression(ipg_authorization.Fields.CreatedOn, OrderType.Descending) },
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression(ipg_authorization.Fields.ipg_carrierid, ConditionOperator.Equal, target.ipg_SecondaryCarrierId .Id),
                            new ConditionExpression(ipg_authorization.Fields.ipg_incidentid, ConditionOperator.Equal, target.Id),
                            new ConditionExpression(ipg_authorization.Fields.StateCode, ConditionOperator.Equal, (int)ipg_authorizationState.Active),
                            new ConditionExpression(ipg_authorization.Fields.ipg_facilityauthnumber, ConditionOperator.NotNull),
                        }
                    }
                }).Entities.FirstOrDefault();

                shouldupdate = true;
                caseupdate.ipg_secondaryauthorizationid = authrecord != null ? authrecord.ToEntityReference() : null;
            }

            if(shouldupdate)
            {
                service.Update(caseupdate);
            }
        }
    }
}
