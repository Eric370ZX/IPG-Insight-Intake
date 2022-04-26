using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Text.RegularExpressions;
namespace Insight.Intake.Plugin.Managers
{
    public class ImportantEventManager
    {
        private IOrganizationService _crmService;

        public ImportantEventManager(IOrganizationService organizationService)
        {
            this._crmService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        }

        public Guid CreateImportantEventLog(Entity incident, Guid performedBy, string eventId, string[] eventDescriptionParam = null)
        {
            Guid logId = Guid.Empty;
            if (incident == null)
            {
                throw new ArgumentNullException(nameof(incident));
            }

            if (!string.IsNullOrEmpty(eventId))
            {
                var eventConfigEntity = RetrieveEventConfig(eventId);
                if (eventConfigEntity != null)
                {
                    var eventConfig = eventConfigEntity.ToEntity<ipg_importanteventconfig>();
                    var eventdDescription = SetEventDescriptionParameters(eventConfig.ipg_eventdescription, eventDescriptionParam);
                    var log = new ipg_importanteventslog()
                    {
                        ipg_name = eventdDescription,
                        ipg_configId = eventId,
                        ipg_activitydescription = eventdDescription,
                        ipg_datetimestamp = DateTime.Now
                    };
                    if (performedBy != null && performedBy != Guid.Empty)
                    {
                        log.ipg_performedby = new EntityReference(SystemUser.EntityLogicalName, performedBy);
                    }
                    var activityType = RetrieveActivityType(eventConfig.ipg_eventtype);
                    if (activityType != null)
                    {
                        log.ipg_activity = activityType.ToEntityReference();
                    }
                    if (incident.LogicalName == Incident.EntityLogicalName)
                    {
                        if (incident.ToEntity<Incident>().Id != null && incident.ToEntity<Incident>().Id != Guid.Empty)
                        {
                            log.ipg_caseid = incident.ToEntity<Incident>().Id.ToString();
                        }
                        if (!string.IsNullOrEmpty(incident.ToEntity<Incident>()?.Title))
                        {
                            log.ipg_casenumbertext = incident.ToEntity<Incident>().Title;
                        }
                    }
                    if (incident.LogicalName == ipg_referral.EntityLogicalName)
                    {
                        if (incident.ToEntity<ipg_referral>().Id != null && incident.ToEntity<ipg_referral>().Id != Guid.Empty)
                        {
                            log.ipg_referralid = incident.ToEntity<ipg_referral>().Id.ToString();
                        }
                        if (!string.IsNullOrEmpty(incident.ToEntity<ipg_referral>().ipg_referralcasenumber))
                        {
                            log.ipg_casenumbertext = incident.ToEntity<ipg_referral>().ipg_referralcasenumber;
                        }
                    }
                    logId = _crmService.Create(log);
                }
            }
            return logId;
        }

        public void SetCaseOrReferralPortalHeader(Entity entity, string eventId)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var eventConfig = RetrieveEventConfig(eventId)?.ToEntity<ipg_importanteventconfig>();

            if (eventConfig != null)
            {
                if (entity.LogicalName == Incident.EntityLogicalName)
                {
                    SetCasePortalHeader(entity, eventConfig, _crmService);
                }
                else if (entity.LogicalName == ipg_referral.EntityLogicalName)
                {
                    SetReferralPortalHeader(entity, eventConfig, _crmService);
                }
            }
        }

        private Entity RetrieveEventConfig(string eventId)
        {
            var query = new QueryExpression(ipg_importanteventconfig.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_importanteventconfig.Fields.ipg_name, ipg_importanteventconfig.Fields.ipg_eventtype,
                                          ipg_importanteventconfig.Fields.ipg_eventdescription, ipg_importanteventconfig.Fields.ipg_PortalReason),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression( ipg_importanteventconfig.Fields.ipg_name, ConditionOperator.Equal, eventId)
                    }
                }
            };
            return _crmService.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        private Entity RetrieveActivityType(string activityType)
        {
            var query = new QueryExpression(ipg_activitytype.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_activitytype.Fields.ipg_name),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression( ipg_activitytype.Fields.ipg_name, ConditionOperator.Equal, activityType)
                    }
                }
            };
            return _crmService.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        private string SetEventDescriptionParameters(string eventDescription, string[] eventDescriptionParam)
        {
            if (eventDescriptionParam != null)
            {
                Regex pattern = new Regex("<.*?>");
                foreach (var param in eventDescriptionParam)
                {
                    if (!String.IsNullOrEmpty(param))
                        eventDescription = pattern.Replace(eventDescription, param, 1);
                }
            }
            return eventDescription;
        }

        private void SetCasePortalHeader(Entity entity, ipg_importanteventconfig eventConfig, IOrganizationService crmService)
        {
            var incident = crmService.Retrieve(Incident.EntityLogicalName, entity.Id, new ColumnSet(Incident.Fields.ipg_facilitycimid, Incident.Fields.ipg_Reasons))?.ToEntity<Incident>();

            var caseToUpdate = new Incident();
            caseToUpdate.Id = entity.Id;

            switch (eventConfig.ipg_PortalReason)
            {
                case null:
                    return;
                case PortalHeaderPlaseholder.Blank:
                    caseToUpdate.ipg_portalheadermultiplelines = string.Empty;
                    break;
                default:
                    var mdmName = incident.ipg_facilitycimid?.Id != null
                        ? crmService.Retrieve(SystemUser.EntityLogicalName, incident.ipg_facilitycimid.Id, new ColumnSet(SystemUser.Fields.FullName))?.ToEntity<SystemUser>()?.FullName
                        : null;

                    var portalReason = incident.ipg_Reasons?.Value != null
                        ? incident.ipg_ReasonsEnum.Value.ToString() : null;

                    caseToUpdate.ipg_portalheadermultiplelines = eventConfig.ipg_PortalReason
                        .Replace(PortalHeaderPlaseholder.MDMName, mdmName)
                        .Replace(PortalHeaderPlaseholder.PortalReason, portalReason);
                    break;
            }

            try
            {
                crmService.Update(caseToUpdate);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, $"Failed to update Case Portal Header: {ex.Message}");
            }
        }

        private void SetReferralPortalHeader(Entity entity, ipg_importanteventconfig eventConfig, IOrganizationService crmService)
        {
            var referral = _crmService.Retrieve(ipg_referral.EntityLogicalName, entity.Id,
                        new ColumnSet(ipg_referral.Fields.ipg_FacilityId, ipg_referral.Fields.ipg_ReasonCode)).ToEntity<ipg_referral>();

            var referralToUpdate = new ipg_referral();
            referralToUpdate.Id = entity.Id;

            switch (eventConfig.ipg_PortalReason)
            {
                case null:
                    return;
                case PortalHeaderPlaseholder.Blank:
                    referralToUpdate.ipg_PortalHeader = string.Empty;
                    break;
                default:
                    var mdmName = Helpers.ReferralHelper.GetFacilityMDM(referral, crmService)?.FullName;

                    var portalReason = referral.ipg_ReasonCode?.Value != null
                        ? referral.ipg_ReasonCodeEnum.Value.ToString() : null;

                    referralToUpdate.ipg_PortalHeader = eventConfig.ipg_PortalReason
                        .Replace(PortalHeaderPlaseholder.MDMName, mdmName)
                        .Replace(PortalHeaderPlaseholder.PortalReason, portalReason);
                    break;
            }

            try
            {
                _crmService.Update(referralToUpdate);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, $"Failed to update Referral Portal Header: {ex.Message}");
            }
        }
    }

    internal class PortalHeaderPlaseholder
    {
        internal const string Blank = "<Blank>";
        internal const string PortalReason = "<Portal Event Reason>";
        internal const string MDMName = "<MDM Name>";
    }
}