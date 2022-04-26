using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Common.Benefits
{
    public class EbvBenefitImporter
    {
        const string InPlanNetworkCode = "Y";
        const string OutPlanNetworkCode = "N";
        const string OtherSourceOfDataCode = "W";

        const string IndividualCoverageLevelCode = "IND";
        const string FamilyCoverageLevelCode = "FAM";

        const string CoInsuranceCode = "A";
        const string DeductibleCode = "C";
        const string OOPCode = "G";

        const string TPQ_Year = "21";
        const string TPQ_ServiceYear = "22";
        const string TPQ_CalendarYear = "23";
        const string TPQ_YearToDate = "24";
        const string TPQ_Contract = "25";
        const string TPQ_Remaining = "29";

        private IOrganizationService _organizationService;
        private ITracingService _log;


        public EbvBenefitImporter(IOrganizationService organizationService, ITracingService tracingService)
        {
            _organizationService = organizationService;
            _log = tracingService;
        }


        public void ImportEBVBenefitsToD365(Guid incidentId, CarrierNumbers carrierNumber, Guid ebvResponseId, Guid currentUserId)
        {
            _log.Trace("Requesting Case");
            var incident = _organizationService.Retrieve(Incident.EntityLogicalName, incidentId, new ColumnSet(
                Incident.Fields.ipg_CarrierId,
                Incident.Fields.ipg_MemberIdNumber,
                Incident.Fields.ipg_SecondaryCarrierId,
                Incident.Fields.ipg_SecondaryMemberIdNumber
                )).ToEntity<Incident>();

            Guid? carrierId;
            string memberId;
            switch (carrierNumber)
            {
                case CarrierNumbers.First:
                    carrierId = incident.ipg_CarrierId?.Id;
                    memberId = incident.ipg_MemberIdNumber;
                    break;
                case CarrierNumbers.Second:
                    carrierId = incident.ipg_SecondaryCarrierId?.Id;
                    memberId = incident.ipg_SecondaryMemberIdNumber;
                    break;
                default:
                    throw new Exception("Unexpected carrier number: " + carrierNumber);
            }
            if(carrierId.HasValue == false)
            {
                throw new Exception("Carrier is required");
            }
            if(string.IsNullOrWhiteSpace(memberId))
            {
                throw new Exception("MemberId is required");
            }

            using(var organizationContext = new OrganizationServiceContext(_organizationService))
            {
                _log.Trace("Requesting existing benefits");
                var existingInsuredBenefits = (from benefit in organizationContext.CreateQuery<ipg_benefit>()
                                        where benefit.ipg_CaseId != null && benefit.ipg_CaseId.Id == incidentId
                                             && benefit.ipg_CarrierId != null && benefit.ipg_CarrierId.Id == carrierId
                                             && benefit.ipg_MemberID == memberId
                                             && benefit.StateCode == ipg_benefitState.Active
                                        select benefit).ToList();

                _log.Trace("Requesting EBV benefits to import into D365");
                var ebvBenefits = (from benefit in organizationContext.CreateQuery<ipg_EBVBenefit>()
                                   where benefit.ipg_ResponseId.Id == ebvResponseId
                                   select benefit).ToList();

                _log.Trace("Requesting service types");
                var gwServiceTypes = (from serviceType in organizationContext.CreateQuery<ipg_GWServiceTypeCode>()
                                            where serviceType.ipg_ImportBenefits == true && serviceType.ipg_BenefitTypeCode != null
                                            select serviceType).ToList();

                var benefitTypes = existingInsuredBenefits.Select(b => b.ipg_BenefitTypeEnum)
                    .Union(gwServiceTypes.Select(st => st.ipg_BenefitTypeCodeEnum))
                    .Where(bt => bt.HasValue)
                    .Select(bt => bt.Value)
                    .Distinct();

                //Update existing benefits. We cannot delete all existing benefits and import everything anew because we need to keep BVF coinsurance
                foreach (var benefitType in benefitTypes)
                {
                    string serviceTypeCode = gwServiceTypes.FirstOrDefault(st => st.ipg_BenefitTypeCodeEnum == benefitType)?.ipg_name;
                    if(string.IsNullOrWhiteSpace(serviceTypeCode))
                    {
                        continue;
                    }

                    ImportBenefits(organizationContext, incidentId, carrierId.Value, memberId, currentUserId, existingInsuredBenefits, ebvBenefits, serviceTypeCode, benefitType, inNetwork: true, coverageLevel: ipg_BenefitCoverageLevels.Individual);
                    ImportBenefits(organizationContext, incidentId, carrierId.Value, memberId, currentUserId, existingInsuredBenefits, ebvBenefits, serviceTypeCode, benefitType, inNetwork: true, coverageLevel: ipg_BenefitCoverageLevels.Family);
                    ImportBenefits(organizationContext, incidentId, carrierId.Value, memberId, currentUserId, existingInsuredBenefits, ebvBenefits, serviceTypeCode, benefitType, inNetwork: false, coverageLevel: ipg_BenefitCoverageLevels.Individual);
                    ImportBenefits(organizationContext, incidentId, carrierId.Value, memberId, currentUserId, existingInsuredBenefits, ebvBenefits, serviceTypeCode, benefitType, inNetwork: false, coverageLevel: ipg_BenefitCoverageLevels.Family);
                }

                _log.Trace("Saving benefits");
                organizationContext.SaveChanges();

                CopyGeneralBenefitsAsDmeIfNeeded(organizationContext, incidentId, carrierId.Value, memberId);
            }
        }

        private void ImportBenefits(OrganizationServiceContext organizationContext, Guid incidentId, Guid carrierId, string memberId, Guid currentUserId,
            IEnumerable<ipg_benefit> existingInsuredBenefits, IEnumerable<ipg_EBVBenefit> ebvBenefits, string serviceTypeCode,
            ipg_BenefitType benefitType, bool inNetwork, ipg_BenefitCoverageLevels coverageLevel)
        {
            var existingBenefit = existingInsuredBenefits.FirstOrDefault(b => b.ipg_BenefitTypeEnum == benefitType
                                    && b.ipg_InOutNetwork == inNetwork
                                    && b.ipg_CoverageLevelEnum == coverageLevel);

            IEnumerable<ipg_EBVBenefit> benefitsToImport = new List<ipg_EBVBenefit>();
            string networkCode = inNetwork ? InPlanNetworkCode : OutPlanNetworkCode;
            string coverageLevelCode;
            switch (coverageLevel)
            {
                case ipg_BenefitCoverageLevels.Individual:
                    coverageLevelCode = IndividualCoverageLevelCode;
                    break;
                case ipg_BenefitCoverageLevels.Family:
                    coverageLevelCode = FamilyCoverageLevelCode;
                    break;
                default:
                    throw new NotSupportedException("Unexpected individualOrFamily value: " + coverageLevel);
            }

            benefitsToImport = ebvBenefits.Where(b =>
                       b.ipg_ServiceType == serviceTypeCode
                    && b.ipg_InPlanNetwork == networkCode
                    && b.ipg_CoverageLevel == coverageLevelCode);

            if (existingBenefit == null)
            {
                if (benefitsToImport.Any())
                {
                    _log.Trace("Creating a new D365 benefit");
                    var newBenefit = new ipg_benefit()
                    {
                        ipg_CaseId = new EntityReference(ipg_benefit.EntityLogicalName, incidentId),
                        ipg_CarrierId = new EntityReference(Intake.Account.EntityLogicalName, carrierId),
                        ipg_MemberID = memberId,
                        ipg_BenefitTypeEnum = benefitType,
                        ipg_InOutNetwork = inNetwork,
                        ipg_CoverageLevelEnum = coverageLevel,
                        ipg_CoinsuranceBenefitSourceEnum = ipg_BenefitSources.EBV
                    };
                    SetD365BenefitValuesFromEbv(newBenefit, benefitsToImport, currentUserId);
                    organizationContext.AddObject(newBenefit);
                }
            }
            else
            {
                if (benefitsToImport.Any())
                {
                    _log.Trace("Updating D365 benefit");
                    SetD365BenefitValuesFromEbv(existingBenefit, benefitsToImport, currentUserId);
                    organizationContext.UpdateObject(existingBenefit);
                }
                else
                {
                    _log.Trace("Deleting D365 benefit");
                    organizationContext.DeleteObject(existingBenefit);
                }
            }
        }

        private void SetD365BenefitValuesFromEbv(ipg_benefit benefit, IEnumerable<ipg_EBVBenefit> benefitsToImport, Guid currentUserid)
        {
            if (benefit.ipg_CoinsuranceBenefitSourceEnum != ipg_BenefitSources.BVF)
            {
                //do not override BVF coinsurance

                benefit.ipg_CarrierCoinsurance = Convert.ToDouble(GetCoInsurance(benefitsToImport));
                benefit.ipg_MemberCoinsurance = 100 - benefit.ipg_CarrierCoinsurance;
                benefit.ipg_CoinsuranceBenefitSourceEnum = ipg_BenefitSources.EBV;
            }

            benefit.ipg_DeductibleMet = new Money(GetDeductibleMet(benefitsToImport));
            benefit.ipg_Deductible = new Money(GetDeductibleMax(benefitsToImport));

            benefit.ipg_MemberOOPMet = new Money(GetOOPMet(benefitsToImport));
            benefit.ipg_MemberOOPMax = new Money(GetOOPMax(benefitsToImport));

            benefit.ipg_BenefitSourceEnum = ipg_BenefitSources.EBV;
            benefit.ipg_ObtainedBy = new EntityReference(SystemUser.EntityLogicalName, currentUserid);
        }

        private decimal GetCoInsurance(IEnumerable<ipg_EBVBenefit> benefits)
        {
            var conInsuranceBenefit = benefits.Where(
                        b => b.ipg_Status == CoInsuranceCode)
                .FirstOrDefault();
            if (conInsuranceBenefit != null)
            {
                return (conInsuranceBenefit.ipg_Percentage ?? 0) * 100;
            }

            return 0;
        }

        private decimal GetDeductibleMet(IEnumerable<ipg_EBVBenefit> benefits)
        {
            var deductibleMetBenefits = benefits
                   .Where(b => b.ipg_Status == DeductibleCode
                               && (b.ipg_TimePeriodQualifier == TPQ_YearToDate));

            if (deductibleMetBenefits.Any())
            {
                return deductibleMetBenefits.Max(x => x.ipg_MonetaryAmount ?? 0);
            }

            return 0;
        }

        private decimal GetDeductibleMax(IEnumerable<ipg_EBVBenefit> benefits)
        {
            var deductibleMaxBenefits = benefits
                   .Where(b => b.ipg_Status == DeductibleCode
                               && (b.ipg_TimePeriodQualifier == TPQ_ServiceYear || b.ipg_TimePeriodQualifier == TPQ_CalendarYear || b.ipg_TimePeriodQualifier == TPQ_Contract));

            if (deductibleMaxBenefits.Any())
            {
                return deductibleMaxBenefits.Max(x => x.ipg_MonetaryAmount ?? 0);
            }

            return 0;
        }

        private decimal GetDeductibleRemaining(IEnumerable<ipg_EBVBenefit> benefits)
        {
            var deductibleRemainingBenefits = benefits
                   .Where(b => b.ipg_Status == DeductibleCode
                               && (b.ipg_TimePeriodQualifier == TPQ_Remaining));

            if (deductibleRemainingBenefits.Any())
            {
                return deductibleRemainingBenefits.Max(x => x.ipg_MonetaryAmount ?? 0);
            }

            return 0;
        }

        private decimal GetOOPMet(IEnumerable<ipg_EBVBenefit> benefits)
        {
            var oopMetBenefits = benefits
                   .Where(b => b.ipg_Status == OOPCode
                               && (b.ipg_TimePeriodQualifier == TPQ_YearToDate));

            if (oopMetBenefits.Any())
            {
                return oopMetBenefits.Max(x => x.ipg_MonetaryAmount ?? 0);
            }

            return 0;
        }

        private decimal GetOOPMax(IEnumerable<ipg_EBVBenefit> benefits)
        {
            var oopMaxBenefits = benefits
                   .Where(b => b.ipg_Status == OOPCode
                               && (b.ipg_TimePeriodQualifier == TPQ_ServiceYear || b.ipg_TimePeriodQualifier == TPQ_CalendarYear || b.ipg_TimePeriodQualifier == TPQ_Contract));

            if (oopMaxBenefits.Any())
            {
                return oopMaxBenefits.Max(x => x.ipg_MonetaryAmount ?? 0);
            }

            return 0;
        }

        
        private decimal GetOOPRemaining(IEnumerable<ipg_EBVBenefit> benefits)
        {
            var oopRemainingBenefits = benefits
                   .Where(b => b.ipg_Status == OOPCode
                               && (b.ipg_TimePeriodQualifier == TPQ_Remaining));

            if (oopRemainingBenefits.Any())
            {
                return oopRemainingBenefits.Max(x => x.ipg_MonetaryAmount ?? 0);
            }

            return 0;
        }

        /// <summary>
        /// If DME benefit type is selected on the case but we have not received DME via EBV,
        /// we should copy General Health (30) benefits as DME benefits.
        /// </summary>
        /// <param name="incidentId"></param>
        private void CopyGeneralBenefitsAsDmeIfNeeded(OrganizationServiceContext organizationServiceContext, Guid incidentId, Guid carrierId, string memberId)
        {
            _log.Trace("Copy general benefits as DME if needed");

            var incident = _organizationService.Retrieve(Incident.EntityLogicalName, incidentId, new ColumnSet(
                    Incident.Fields.ipg_BenefitTypeCode
                    )).ToEntity<Incident>();
            if (incident == null)
            {
                throw new InvalidPluginExecutionException("Could not find the incident");
            }
            if (incident.ipg_BenefitTypeCodeEnum == ipg_BenefitType.DurableMedicalEquipment_DME)
            {
                _log.Trace("Requesting DME benefits");
                var dmeBenefits = organizationServiceContext.CreateQuery<ipg_benefit>()
                    .Where(b =>
                        b.ipg_CaseId != null && b.ipg_CaseId.Id == incidentId
                        && b.ipg_CarrierId != null && b.ipg_CarrierId.Id == carrierId
                        && b.ipg_MemberID == memberId
                        && b.ipg_BenefitTypeEnum == ipg_BenefitType.DurableMedicalEquipment_DME
                        && b.StateCode == ipg_benefitState.Active)
                    .ToList();
                if(dmeBenefits.Any() == false)
                {
                    //DME benefits do not exist. Copy general benefits as DME
                    _log.Trace("Requesting general benefits");
                    var healthBenefits = organizationServiceContext.CreateQuery<ipg_benefit>()
                        .Where(b =>
                            b.ipg_CaseId != null && b.ipg_CaseId.Id == incidentId
                            && b.ipg_CarrierId != null && b.ipg_CarrierId.Id == carrierId
                            && b.ipg_MemberID == memberId
                            && b.ipg_BenefitTypeEnum == ipg_BenefitType.HealthBenefitPlanCoverage
                            && b.StateCode == ipg_benefitState.Active)
                        .ToList();

                    if(healthBenefits.Any())
                    {
                        _log.Trace("Copying general benefits as DME");
                        foreach (var healthBenefit in healthBenefits)
                        {
                            var newDmeBenefit = new ipg_benefit
                            {
                                ipg_CaseId = healthBenefit.ipg_CaseId,
                                ipg_CarrierId = healthBenefit.ipg_CarrierId,
                                ipg_MemberID = healthBenefit.ipg_MemberID,

                                ipg_BenefitTypeEnum = ipg_BenefitType.DurableMedicalEquipment_DME,
                                ipg_InOutNetwork = healthBenefit.ipg_InOutNetwork,
                                ipg_CoverageLevelEnum = healthBenefit.ipg_CoverageLevelEnum,
                                ipg_EligibilityStartDate = healthBenefit.ipg_EligibilityStartDate,
                                ipg_EligibilityEndDate = healthBenefit.ipg_EligibilityEndDate,
                                ipg_Deductible = healthBenefit.ipg_Deductible,
                                ipg_DeductibleMet = healthBenefit.ipg_DeductibleMet,
                                ipg_CarrierCoinsurance = healthBenefit.ipg_CarrierCoinsurance,
                                ipg_MemberCoinsurance = healthBenefit.ipg_MemberCoinsurance,
                                ipg_MemberOOPMax = healthBenefit.ipg_MemberOOPMax,
                                ipg_MemberOOPMet = healthBenefit.ipg_MemberOOPMet
                            };
                            organizationServiceContext.AddObject(newDmeBenefit);
                        }

                        _log.Trace("Saving changes");
                        organizationServiceContext.SaveChanges();
                    }
                }
            }
        }
    }
}
