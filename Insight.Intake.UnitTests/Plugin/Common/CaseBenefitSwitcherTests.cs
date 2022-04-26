using FakeXrmEasy;
using Insight.Intake.Data;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Plugin.Common.Benefits;

namespace Insight.Intake.UnitTests.Plugin.Common
{
    public class CaseBenefitSwitcherTests : PluginTestsBase
    {
        #region DME

        [Fact]
        public void DoesNotUpdate_BenefitType_If_AlreadyDME()
        {
            //ARRANGE
            var carrier = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var incident = new Incident().Fake()
                .WithCarrierReference(carrier)
                .WithBenefitType(ipg_BenefitType.DurableMedicalEquipment_DME)
                .WithMemberId("XYZ123")
                .Generate();
            
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident });

            //ACT
            var caseBenefitSwitcher = new CaseBenefitSwitcher(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService());
            caseBenefitSwitcher.SetDMEBenefitTypeIfNeeded(incident.Id, CarrierNumbers.First);

            //ASSERT
            var updatedIncident = fakedContext.GetOrganizationService().Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(ipg_BenefitType.DurableMedicalEquipment_DME, updatedIncident.ipg_BenefitTypeCodeEnum);
        }

        [Fact]
        public void DoesNotUpdate_BenefitType_If_NotJQU()
        {
            //ARRANGE
            var carrier = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var incident = new Incident().Fake()
                .WithCarrierReference(carrier)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithMemberId("NOTJQU123")
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident });

            //ACT
            var caseBenefitSwitcher = new CaseBenefitSwitcher(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService());
            caseBenefitSwitcher.SetDMEBenefitTypeIfNeeded(incident.Id, CarrierNumbers.First);

            //ASSERT
            var updatedIncident = fakedContext.GetOrganizationService().Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(ipg_BenefitType.HealthBenefitPlanCoverage, updatedIncident.ipg_BenefitTypeCodeEnum);
        }

        [Fact]
        public void DoesNot_Set_DME_If_NoDME()
        {
            //ARRANGE
            string memberId = "JQU123";
            var carrier = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var incident = new Incident().Fake()
                .WithCarrierReference(carrier)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithMemberId(memberId)
                .Generate();
            var benefit = new ipg_benefit().Fake()
                .WithCaseReference(incident)
                .WithCarrierReference(carrier)
                .WithMemberId(memberId)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithInOutNetwork(true)
                .WithCoverageLevel(ipg_BenefitCoverageLevels.Individual)
                .WithIndividualBenefits(20, 10, 40, 20)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident, benefit });

            //ACT
            var caseBenefitSwitcher = new CaseBenefitSwitcher(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService());
            caseBenefitSwitcher.SetDMEBenefitTypeIfNeeded(incident.Id, CarrierNumbers.First);

            //ASSERT
            var updatedIncident = fakedContext.GetOrganizationService().Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(ipg_BenefitType.HealthBenefitPlanCoverage, updatedIncident.ipg_BenefitTypeCodeEnum);
        }

        [Fact]
        public void Sets_DME_If_DME_Benefits_Exist()
        {
            //ARRANGE
            string memberId = "JQU123";
            var carrier = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var incident = new Incident().Fake()
                .WithCarrierReference(carrier)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithMemberId(memberId)
                .Generate();
            var benefit = new ipg_benefit().Fake()
                .WithCaseReference(incident)
                .WithCarrierReference(carrier)
                .WithMemberId(memberId)
                .WithBenefitType(ipg_BenefitType.DurableMedicalEquipment_DME)
                .WithInOutNetwork(true)
                .WithCoverageLevel(ipg_BenefitCoverageLevels.Individual)
                .WithIndividualBenefits(20, 10, 40, 20)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident, benefit });

            //ACT
            var caseBenefitSwitcher = new CaseBenefitSwitcher(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService());
            caseBenefitSwitcher.SetDMEBenefitTypeIfNeeded(incident.Id, CarrierNumbers.First);

            //ASSERT
            var updatedIncident = fakedContext.GetOrganizationService().Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(ipg_BenefitType.DurableMedicalEquipment_DME, updatedIncident.ipg_BenefitTypeCodeEnum);
        }

        [Fact]
        public void Sets_Secondary_DME_If_DME_Benefits_Exist()
        {
            //ARRANGE
            var secondaryMemberId = "JQU123";
            var carrier1 = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var carrier2 = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var incident = new Incident().Fake()
                .WithCarrierReference(carrier1)
                .WithCarrierReference(carrier2, isPrimaryCarrier: false)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithSecondaryBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithMemberId("XYZ123")
                .WithSecondaryMemberId(secondaryMemberId)
                .Generate();
            var benefit = new ipg_benefit().Fake()
                .WithCaseReference(incident)
                .WithCarrierReference(carrier2)
                .WithMemberId(secondaryMemberId)
                .WithBenefitType(ipg_BenefitType.DurableMedicalEquipment_DME)
                .WithInOutNetwork(true)
                .WithCoverageLevel(ipg_BenefitCoverageLevels.Individual)
                .WithIndividualBenefits(20, 10, 40, 20)
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier1, carrier2, incident, benefit });

            //ACT
            var caseBenefitSwitcher = new CaseBenefitSwitcher(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService());
            caseBenefitSwitcher.SetDMEBenefitTypeIfNeeded(incident.Id, CarrierNumbers.Second);

            //ASSERT
            var updatedIncident = fakedContext.GetOrganizationService().Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(ipg_BenefitType.HealthBenefitPlanCoverage, updatedIncident.ipg_BenefitTypeCodeEnum);
            Assert.Equal(ipg_BenefitType.DurableMedicalEquipment_DME, updatedIncident.ipg_Carrier2BenefitTypeCodeEnum);
        }

        #endregion


        #region InOutNetwork

        [Fact]
        public void Changes_InOutNetwork_If_Contract_Exists()
        {
            //ARRANGE

            var facility = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Facility).Generate();
            var carrier = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var incident = new Incident().Fake()
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .WithScheduledDos(DateTime.Now)
                .WithInOutNetwork(false)
                .WithInOutNetwork2(false)
                .Generate();
            var entitlement = new Entitlement().Fake()
                .WithEntitlementType(new Microsoft.Xrm.Sdk.OptionSetValue((int)ipg_EntitlementTypes.FacilityCarrier))
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .WithContractStatus(new OptionSetValue((int)Entitlement_ipg_contract_status.Live))
                .WithEffectiveDateRange(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1))
                .WithIPGCarrierNetworkStatus(new Microsoft.Xrm.Sdk.OptionSetValue((int)Entitlement_ipg_carrier_network_status.INN))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, entitlement });

            //ACT

            var caseBenefitSwitcher = new CaseBenefitSwitcher(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService());
            caseBenefitSwitcher.UpdateInOutNetwork(incident.Id, carrier.Id);


            //ASSERT
            var updatedIncident = fakedContext.GetOrganizationService().Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.True(updatedIncident.ipg_inoutnetwork);
            Assert.False(updatedIncident.ipg_Carrier2IsInOutNetwork);
        }

        [Fact]
        public void Changes_InOutNetwork2_If_Contract_Exists()
        {
            //ARRANGE

            var facility = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Facility).Generate();
            var carrier = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var carrier2 = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var incident = new Incident().Fake()
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .WithCarrierReference(carrier2, isPrimaryCarrier: false)
                .WithScheduledDos(DateTime.Now)
                .WithInOutNetwork(false)
                .WithInOutNetwork2(false)
                .Generate();
            var entitlement = new Entitlement().Fake()
                .WithEntitlementType(new Microsoft.Xrm.Sdk.OptionSetValue((int)ipg_EntitlementTypes.FacilityCarrier))
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier2)
                .WithContractStatus(new OptionSetValue((int)Entitlement_ipg_contract_status.Live))
                .WithEffectiveDateRange(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1))
                .WithIPGCarrierNetworkStatus(new Microsoft.Xrm.Sdk.OptionSetValue((int)Entitlement_ipg_carrier_network_status.INN))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, entitlement });

            //ACT

            var caseBenefitSwitcher = new CaseBenefitSwitcher(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService());
            caseBenefitSwitcher.UpdateInOutNetwork(incident.Id, carrier2.Id);


            //ASSERT
            var updatedIncident = fakedContext.GetOrganizationService().Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.False(updatedIncident.ipg_inoutnetwork);
            Assert.True(updatedIncident.ipg_Carrier2IsInOutNetwork);
        }

        [Fact]
        public void Does_Not_Change_InOutNetwork_If_Contract_Not_Found()
        {
            //ARRANGE

            var facility = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Facility).Generate();
            var carrier = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var incident = new Incident().Fake()
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .WithScheduledDos(DateTime.Now)
                .WithInOutNetwork(false)
                .WithInOutNetwork2(false)
                .Generate();
            var entitlement = new Entitlement().Fake()
                .WithEntitlementType(new Microsoft.Xrm.Sdk.OptionSetValue((int)ipg_EntitlementTypes.FacilityCarrier))
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .WithContractStatus(new OptionSetValue((int)Entitlement_ipg_contract_status.Pending))
                .WithEffectiveDateRange(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1))
                .WithIPGCarrierNetworkStatus(new Microsoft.Xrm.Sdk.OptionSetValue((int)Entitlement_ipg_carrier_network_status.INN))
                .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, entitlement });

            //ACT

            var caseBenefitSwitcher = new CaseBenefitSwitcher(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService());
            caseBenefitSwitcher.UpdateInOutNetwork(incident.Id, carrier.Id);


            //ASSERT
            var updatedIncident = fakedContext.GetOrganizationService().Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            Assert.False(updatedIncident.ipg_inoutnetwork);
            Assert.False(updatedIncident.ipg_Carrier2IsInOutNetwork);
        }

        #endregion
    }
}
