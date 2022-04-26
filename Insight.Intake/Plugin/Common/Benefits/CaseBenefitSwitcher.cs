using Insight.Intake.Plugin.Common.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Reflection;

namespace Insight.Intake.Plugin.Common.Benefits
{
    public class CaseBenefitSwitcher : ICaseBenefitSwitcher
    {
        private readonly IOrganizationService _organizationService;
        private readonly ITracingService _tracingService;


        public CaseBenefitSwitcher(IOrganizationService organizationService, ITracingService tracingService)
        {
            _organizationService = organizationService;
            _tracingService = tracingService;
        }

        /// <summary>
        /// If MemberId starts with JQU and DME benefits exist, set Benefit Type = DME
        /// </summary>
        /// <param name="incidentId"></param>
        /// <param name="carrierId"></param>
        /// <param name="memberId"></param>
        public void SetDMEBenefitTypeIfNeeded(Guid incidentId, CarrierNumbers carrierNumber)
        {
            _tracingService.Trace($"Starting {MethodBase.GetCurrentMethod().Name}");

            _tracingService.Trace("Retrieving Incident");
            var incident = _organizationService.Retrieve(Incident.EntityLogicalName, incidentId, new ColumnSet(
                Incident.Fields.ipg_CarrierId,
                Incident.Fields.ipg_BenefitTypeCode,
                Incident.Fields.ipg_MemberIdNumber,
                Incident.Fields.ipg_SecondaryCarrierId,
                Incident.Fields.ipg_Carrier2BenefitTypeCode,
                Incident.Fields.ipg_SecondaryMemberIdNumber
                )).ToEntity<Incident>();

            Guid? carrierId;
            string memberId;
            ipg_BenefitType? benefitType;
            switch (carrierNumber)
            {
                case CarrierNumbers.First:
                    carrierId = incident.ipg_CarrierId?.Id;
                    memberId = incident.ipg_MemberIdNumber;
                    benefitType = incident.ipg_BenefitTypeCodeEnum;
                    break;
                case CarrierNumbers.Second:
                    carrierId = incident.ipg_SecondaryCarrierId?.Id;
                    memberId = incident.ipg_SecondaryMemberIdNumber;
                    benefitType = incident.ipg_Carrier2BenefitTypeCodeEnum;
                    break;
                default:
                    throw new Exception("Unexpected carrier number: " + carrierNumber);
            }
            if (carrierId.HasValue == false)
            {
                throw new Exception("Carrier is required");
            }

            if (benefitType == ipg_BenefitType.DurableMedicalEquipment_DME)
            {
                _tracingService.Trace("Already DME benefit type. Exit");
                return;
            }

            if ((memberId ?? "").ToUpper().StartsWith("JQU") == false)
            {
                _tracingService.Trace("Not JQU member id. Exit");
                return;
            }

            using (var crmServiceContext = new CrmServiceContext(_organizationService))
            {
                _tracingService.Trace("Find existing DME benefits");
                var firstDmeBenefit = crmServiceContext.ipg_benefitSet.FirstOrDefault(b =>
                    b.ipg_CaseId != null && b.ipg_CaseId.Id == incidentId
                    && b.ipg_CarrierId != null && b.ipg_CarrierId.Id == carrierId
                    && b.ipg_MemberID == memberId
                    && b.ipg_BenefitTypeEnum == ipg_BenefitType.DurableMedicalEquipment_DME
                    && b.StateCode == ipg_benefitState.Active
                    );

                if(firstDmeBenefit != null)
                {
                    var incidentUpdate = new Incident { 
                        Id = incident.Id
                    };

                    switch (carrierNumber)
                    {
                        case CarrierNumbers.First:
                            incidentUpdate.ipg_BenefitTypeCodeEnum = ipg_BenefitType.DurableMedicalEquipment_DME;
                            break;
                        case CarrierNumbers.Second:
                            incidentUpdate.ipg_Carrier2BenefitTypeCodeEnum = ipg_BenefitType.DurableMedicalEquipment_DME;
                            break;
                        default:
                            throw new InvalidPluginExecutionException("Could not find the carrier on the case");
                    }

                    _tracingService.Trace("Updating Incident (setting DME)");
                    crmServiceContext.Attach(incidentUpdate);
                    crmServiceContext.UpdateObject(incidentUpdate);
                    crmServiceContext.SaveChanges();
                }
            }
        }

        public void UpdateInOutNetwork(Guid incidentId, Guid carrierId)
        {
            _tracingService.Trace($"Starting {MethodBase.GetCurrentMethod().Name}");

            _tracingService.Trace("Retrieving Incident");
            var incident = _organizationService.Retrieve(Incident.EntityLogicalName, incidentId, new ColumnSet(
                Incident.Fields.ipg_ActualDOS,
                Incident.Fields.ipg_SurgeryDate,
                Incident.Fields.ipg_FacilityId,
                Incident.Fields.ipg_CarrierId,
                Incident.Fields.ipg_SecondaryCarrierId
                )).ToEntity<Incident>();

            _tracingService.Trace("Determine DOS");
            DateTime? dos = incident.ipg_ActualDOS ?? incident.ipg_SurgeryDate;

            if (dos.HasValue == false)
            {
                _tracingService.Trace("No Incident DOS. Exit");
                return;
            }

            _tracingService.Trace("Checking if Facility and Carrier are populated");
            if (incident.ipg_FacilityId == null)
            {
                _tracingService.Trace("No Facility. Exit");
                return;
            }

            using (var crmServiceContext = new CrmServiceContext(_organizationService))
            {
                _tracingService.Trace("Searching for a Facility-Carrier entitlement");
                var contract = crmServiceContext.EntitlementSet.FirstOrDefault(ent =>
                    ent.ipg_EntitlementTypeEnum == ipg_EntitlementTypes.FacilityCarrier
                    //&& ent.StateCode == EntitlementState.Active uncomment if needed. On 4/24/2021 Facility-Carrier contract were drafts
                    && ent.ipg_FacilityId != null && ent.ipg_FacilityId.Id == incident.ipg_FacilityId.Id
                    && ent.ipg_CarrierId != null && ent.ipg_CarrierId.Id == carrierId
                    && ent.ipg_contract_statusEnum == Entitlement_ipg_contract_status.Live
                    && ent.StartDate != null && ent.StartDate.Value <= dos.Value && (ent.EndDate == null || ent.EndDate.Value >= dos.Value)
                    );

                _tracingService.Trace("Checking if a contract is found and INN/OON is set");
                if (contract != null && contract.ipg_carrier_network_statusEnum.HasValue)
                {
                    var incidentUpdate = new Incident { Id = incident.Id };
                    bool newInOutNetwork;
                    switch (contract.ipg_carrier_network_statusEnum.Value)
                    {
                        case Entitlement_ipg_carrier_network_status.INN:
                            newInOutNetwork = true;
                            break;
                        case Entitlement_ipg_carrier_network_status.OON:
                            newInOutNetwork = false;
                            break;
                        default:

                            throw new Exception("Unexpected IPG carrier network status: " + contract.ipg_carrier_network_statusEnum.Value);
                    }

                    if (incident.ipg_CarrierId?.Id == carrierId)
                    {
                        incidentUpdate.ipg_inoutnetwork = newInOutNetwork;
                    }
                    else if (incident.ipg_SecondaryCarrierId?.Id == carrierId)
                    {
                        incidentUpdate.ipg_Carrier2IsInOutNetwork = newInOutNetwork;
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Could not find the carrier on the case");
                    }

                    _tracingService.Trace("Updating Incident");
                    crmServiceContext.Attach(incidentUpdate);
                    crmServiceContext.UpdateObject(incidentUpdate);
                    crmServiceContext.SaveChanges();
                }
            }
        }
    }
}
