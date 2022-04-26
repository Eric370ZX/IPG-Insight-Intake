using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class FacilityCarrierContractExists : PluginBase
    {
        private IOrganizationService _service;
        public FacilityCarrierContractExists() : base(typeof(FacilityCarrierContractExists))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingFacilityCarrierContractExists", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext ctx)
        {
            var target = ctx.TargetRef();
            var context = ctx.PluginExecutionContext;
            _service = ctx.OrganizationService;

            context.OutputParameters["Succeeded"] = false;
            context.OutputParameters["CaseNote"] = string.Empty;
            context.OutputParameters["PortalNote"] = string.Empty;

            if (target.LogicalName != Incident.EntityLogicalName)
            {
                var referral = _service.Retrieve(target.LogicalName, target.Id, new ColumnSet(
                    ipg_referral.Fields.ipg_FacilityId,
                    ipg_referral.Fields.ipg_CarrierId,
                    ipg_referral.Fields.OwnerId)).ToEntity<ipg_referral>();
                if (referral.ipg_CarrierId != null && referral.ipg_FacilityId != null)
                {
                    var carrierNetwork = FindCarrierNetwork(referral.ipg_CarrierId.Id, _service);
                    if (carrierNetwork?.ipg_name?.ToLower() == "other") {
                        context.OutputParameters["CodeOutput"] = 2;
                        context.OutputParameters["Succeeded"] = true;
                        return;
                    }
                    if (FindContract(referral.ipg_CarrierId, referral.ipg_FacilityId, referral.GetCaseDos()))
                    {
                        context.OutputParameters["CodeOutput"] = 1;
                        context.OutputParameters["Succeeded"] = true;
                        return;
                    }
                }
            }
            else
            {
                var incident = _service.Retrieve(target.LogicalName, target.Id, new ColumnSet(
                    Incident.Fields.ipg_FacilityId,
                    Incident.Fields.ipg_CarrierId,
                    Incident.Fields.ipg_ActualDOS,
                    Incident.Fields.ipg_SurgeryDate,
                    Incident.Fields.OwnerId)).ToEntity<Incident>();
                if (incident.ipg_CarrierId != null && incident.ipg_FacilityId != null)
                {
                    var carrierNetwork = FindCarrierNetwork(incident.ipg_CarrierId.Id, _service);
                    if (carrierNetwork?.ipg_name?.ToLower() == "other")
                    {
                        context.OutputParameters["CodeOutput"] = 2;
                        context.OutputParameters["Succeeded"] = true;
                        return;
                    }
                    if (FindContract(incident.ipg_CarrierId, incident.ipg_FacilityId, incident.GetCaseDos()))
                    {
                        context.OutputParameters["CodeOutput"] = 1;
                        context.OutputParameters["Succeeded"] = true;
                        return;
                    }
                }
            }

            //hard coded creation of task should be deleted from WF Task
            //CreateContractIssueTask(incident);
            context.OutputParameters["Succeeded"] = false;
        }

        private ipg_carriernetwork FindCarrierNetwork(Guid carrierId, IOrganizationService service)
        {
            var carrier = _service.Retrieve(Intake.Account.EntityLogicalName, carrierId, new ColumnSet(Intake.Account.Fields.ipg_carriernetworkid))
                          .ToEntity<Intake.Account>();
            if (carrier.ipg_carriernetworkid == null)
            {
                return null;
            }

            var carrierNetwork = service.Retrieve(carrier.ipg_carriernetworkid.LogicalName, carrier.ipg_carriernetworkid.Id, new ColumnSet(ipg_carriernetwork.Fields.ipg_name)).ToEntity<ipg_carriernetwork>();

            return carrierNetwork;
        }

        private bool FindContract(EntityReference carrierId, EntityReference facilityId, DateTime? dos)
        {
            var query = new QueryExpression
            {
                EntityName = Entitlement.EntityLogicalName,
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Entitlement.Fields.ipg_Active, ConditionOperator.Equal, true),
                        new ConditionExpression(Entitlement.Fields.ipg_EntitlementType, ConditionOperator.Equal, (int)ipg_EntitlementTypes.FacilityCarrier),
                        new ConditionExpression(Entitlement.Fields.ipg_FacilityId, ConditionOperator.Equal, facilityId.Id),
                        new ConditionExpression(Entitlement.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierId.Id)
                    }
                }
            };
            //commented accordign to the story https://eti-ipg.atlassian.net/browse/CPI-21000
            //if (dos != null)
            //{
            //    query.Criteria.AddCondition(Entitlement.Fields.StartDate, ConditionOperator.OnOrBefore, dos.Value);
            //    query.Criteria.AddCondition(Entitlement.Fields.EndDate, ConditionOperator.OnOrAfter, dos.Value);
            //}
            //else
            //{
            //    return false;
            //}
            return _service.RetrieveMultiple(query).Entities.Any();
        }

        private void CreateContractIssueTask(Incident incident)
        {
            var carrierName = GetAccountNameById(incident.ipg_CarrierId);
            var facilityName = GetAccountNameById(incident.ipg_FacilityId);
            var taskSubject = "Resolve Facility-Carrier Contract Issues";
            var taskSubcategory = $"{facilityName} - {carrierName}";
            if (!TaskAlreadyExisits(incident, taskSubcategory))
            {
                var task = new Task
                {
                    Subject = taskSubject,
                    Subcategory = taskSubcategory,
                    RegardingObjectId = incident.ToEntityReference(),
                    OwnerId = incident.OwnerId
                };
                _service.Create(task);
            }
        }

        private bool TaskAlreadyExisits(Incident incident, string subcategory)
        {
            var query = new QueryExpression
            {
                EntityName = Task.EntityLogicalName,
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Task.Fields.Subcategory, ConditionOperator.Equal, subcategory),
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, incident.ToEntityReference().Id),
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                    }
                }
            };
            return _service.RetrieveMultiple(query).Entities.Any();
        }

        private string GetAccountNameById(EntityReference accountRef)
        {
            if (accountRef == null)
            {
                return "--";
            }
            var account = _service.Retrieve(Intake.Account.EntityLogicalName, accountRef.Id, new ColumnSet(Intake.Account.Fields.Name)).ToEntity<Intake.Account>();
            return account.Name;
        }
    }
}
