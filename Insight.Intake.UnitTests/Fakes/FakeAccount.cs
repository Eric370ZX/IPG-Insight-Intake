using Bogus;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeAccount
    {
        public static Faker<Account> Fake(this Account self)
        {
            return new Faker<Account>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.StateCode, x => AccountState.Active)
                .RuleFor(x => x.ipg_active, x => true)
                .RuleFor(x => x.AccountNumber, x => x.Random.Number(50, 100).ToString())
                .RuleFor(x => x.ipg_ZirMedID, x => x.Random.Number(50, 100).ToString())
                .RuleFor(x => x.ipg_EnableEBVZirMed, x => true);
        }

        public static Faker<Account> Fake(this Account self, Account_CustomerTypeCode customerTypeCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return self.Fake()
                .RuleFor(x => x.CustomerTypeCode, x => customerTypeCode.ToOptionSetValue());
        }

        public static Faker<Account> Fake(this Account self, int customerTypeCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return self.Fake()
                .RuleFor(x => x.CustomerTypeCode, x => new OptionSetValue(customerTypeCode));
        }

        public static Faker<Account> WithHomePlanNetworkOptionSetValue(this Faker<Account> self, int homePlanNetwork)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_HomePlanNetworkOptionSet, x => new OptionSetValue(homePlanNetwork));

            return self;
        }

        public static Faker<Account> WithCarrierAccepted(this Faker<Account> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_carrieraccepted, x => true);

            return self;
        }


        public static Faker<Account> WithParentMfg(this Faker<Account> self, EntityReference parent)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_ParentAccound, x => parent);

            return self;
        }

        public static Faker<Account> WithMemberOf(this Faker<Account> self, EntityReference parent)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ParentAccountId, x => parent);

            return self;
        }


        public static Account FakeCarrierForEBV(this Account self)
        {
            return new Account
            {
                Id = Guid.NewGuid(),
                Name = "Name1",
                ipg_ZirMedID = "EDIID1"
            };
        }

        public static Account FakeCarrierForDeriveHomePlan(this Account self, string name = "Name1")
        {
            return new Account
            {
                Id = Guid.NewGuid(),
                Name = name,
                ipg_HomePlanNetworkOptionSet = new OptionSetValue(FakeOptionSetMetadata.FakeHomeplanNetworkOptionset().OptionSet.Options.First(m => m.Label.UserLocalizedLabel.Label == "BCBS").Value.Value)
            };
        }

        public static Faker<Account> WithDTM(this Faker<Account> self, bool isDtm)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_dtmmember, x =>  new OptionSetValue(isDtm ? 1 : 0));

            return self;
        }

        public static Faker<Account> FakeCarrierForClaimGeneration(this Faker<Account> self, bool isPrimaryCarrier, ipg_claim_type claimType, ipg_CarrierType carrierType = ipg_CarrierType.WorkersComp)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.CustomerTypeCode, x => Account_CustomerTypeCode.Carrier.ToOptionSetValue())
                .RuleFor(x => x.ipg_CarrierType, x => carrierType.ToOptionSetValue())
                .RuleFor(x => x.ipg_CarrierSupportedPlanTypes, x => new OptionSetValueCollection() { ipg_CarrierPlanTypes1.WorkersComp.ToOptionSetValue() })
                .RuleFor(x => x.Name, isPrimaryCarrier ? "Primary Carrier" : "Secondary Carrier")
                .RuleFor(x => x.ipg_ClaimType, x => claimType.ToOptionSetValue())
                .RuleFor(x => x.ipg_ClaimsServicingProviderName, x => ipg_ClaimServicingProviderNames.Facility.ToOptionSetValue())
                .RuleFor(x => x.ipg_ClaimsServicingProviderAddress, x => ipg_ClaimServicingProviderAddresses.Facility.ToOptionSetValue())
                .RuleFor(x => x.ipg_ClaimsPayToName, x => ipg_ClaimPayToNames.IPGPayerAccount.ToOptionSetValue())
                .RuleFor(x => x.ipg_ClaimsPayToAddress, x => ipg_ClaimsPayToAddresses.IPG.ToOptionSetValue());

            return self;
        }

        public static Faker<Account> FakeFacilityForClaimGeneration(this Faker<Account> self, ipg_state state)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.Name, x => "Test Facility")
                .RuleFor(x => x.ipg_StateId, x => state.ToEntityReference())
                .RuleFor(x => x.CustomerTypeCode, x => Account_CustomerTypeCode.Facility.ToOptionSetValue());

            return self;
        }


        public static Faker<Account> WithManufacturerEffectiveDate(this Faker<Account> self, DateTime effectiveDate)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_ManufacturerEffectiveDate, x => effectiveDate);

            return self;
        }

        public static Faker<Account> WithManufacturerExpirationDate(this Faker<Account> self, DateTime expirationDate)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_ManufacturerExpirationDate, x => expirationDate);

            return self;
        }

        public static Account FakeAccountForClaims(this Account self, Account_CustomerTypeCode customerTypeCode, bool isPrimaryCarrier, ipg_claim_type claimType)
        {
            return new Account()
            {
                Id = Guid.NewGuid(),
                Name = customerTypeCode == Account_CustomerTypeCode.Facility ? "Test Facility" : isPrimaryCarrier ? "Primary Carrier" : "Secondary Carrier",
                ipg_ClaimName = isPrimaryCarrier ? "Test Primary Claim Name" : "Test Secondary Claim Name",
                ipg_ClaimType = claimType.ToOptionSetValue(),
                ipg_ConsolidateLikeCodesonClaim = false,
                ipg_UseAlternateHCPCS = false,
                ipg_usesecondaryhcpcs = false,
                Address1_City = "City1",
                Address1_Line1 = "Address1",
                Address1_PostalCode = "1111111",
                Address1_StateOrProvince = "State1",
                Address2_City = "City22",
                Address2_Line1 = "Address22",
                Address2_PostalCode = "2222222",
                Address2_StateOrProvince = "State22",
                ipg_UseClaimsAddressfieldsonclaims = true,
                ipg_UseClaimsAddressToFieldbutFacilityAddress = false,
                ipg_Ub04TypeOfBillCode = "0832",
                ipg_HealthPlanIDBox51 = "box51"
            };
        }

        public static Faker<Account> WithCarrierNetworkReference(this Faker<Account> self, ipg_carriernetwork carrierNetwork)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (carrierNetwork == null) throw new ArgumentNullException(nameof(carrierNetwork));

            self.RuleFor(
                x => x.ipg_carriernetworkid,
                x => new EntityReference
                {
                    Id = carrierNetwork.Id,
                    LogicalName = carrierNetwork.LogicalName
                }
            );

            return self;
        }

        public static Faker<Account> WithNetworkReference(this Faker<Account> self, ipg_carriernetwork carrierNetwork)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (carrierNetwork == null) throw new ArgumentNullException(nameof(carrierNetwork));

            self.RuleFor(
                x => x.ipg_carriernetworkid,
                x => new EntityReference
                {
                    Id = carrierNetwork.Id,
                    LogicalName = carrierNetwork.LogicalName
                }
            );

            return self;
        }

        public static Faker<Account> WithManagers(this Faker<Account> self, SystemUser MDD, SystemUser CIM, SystemUser casemgr)
        {
            self.RuleFor(x => x.ipg_FacilityMddId, MDD.ToEntityReference())
                .RuleFor(x => x.ipg_FacilityCimId, CIM.ToEntityReference())
                .RuleFor(x => x.ipg_FacilityCaseMgrId, casemgr.ToEntityReference());


            return self;
        }

        public static Faker<Account> WithCarrierType(this Faker<Account> self, ipg_CarrierType carrierType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_CarrierType,
                x => new OptionSetValue((int)carrierType)
            );

            return self;
        }

        public static Faker<Account> WithName(this Faker<Account> self, string carrierName)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.Name,
                x => carrierName
            );

            return self;
        }

        public static Faker<Account> WithEnrolledInEhr(this Faker<Account> self, Account_ipg_enrolledinehr enrolledInEhr)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_enrolledinehrEnum,
                x => enrolledInEhr
            );

            return self;
        }

    }
}