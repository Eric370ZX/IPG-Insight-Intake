using System;
using Bogus;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeContact
    {
        public static Faker<Contact> Fake(this Contact self, int customerTypeCode = (int)Contact_CustomerTypeCode.DefaultValue)
        {
            return new Faker<Contact>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.FirstName, x => x.Name.FindName())
                .RuleFor(x => x.LastName, x => x.Name.LastName())
                .RuleFor(x => x.Address1_Line1, x => "Test Street Address")
                .RuleFor(x => x.Address1_City, x => "Test City")
                .RuleFor(x => x.Address1_StateOrProvince, x => "Test State")
                .RuleFor(x => x.Address1_PostalCode, x => "123456")
                .RuleFor(x => x.Telephone1, x => "1234567890")
                .RuleFor(x => x.Telephone2, x => "1234567890")
                .RuleFor(x => x.Telephone3, x => "1234567890")
                .RuleFor(x => x.BirthDate, new DateTime())
                .RuleFor(x => x.FamilyStatusCode, x => new OptionSetValue((int)Contact_FamilyStatusCode.Single))
                .RuleFor(x => x.ipg_EmployerName, x => "Employer Name")
                .RuleFor(x => x.ipg_EmploymentStatus, x => new OptionSetValue((int)Contact_ipg_EmploymentStatus.Employed))
                .RuleFor(x => x.CustomerTypeCode, x => new OptionSetValue(customerTypeCode))
                .RuleFor(x => x.ipg_BusinessExt, x => "1234566")
                .RuleFor(x => x.ipg_PhysicianNPI, x => "123456789");
        }

        public static Contact FakeContactWithMemberId(this Contact self, string primaryMemberId = "AAK12345", string secondaryMemberid = "AAP12345")
        {
            var contact = new Contact().Fake();
            contact.RuleFor(x => x.ipg_PrimaryMemberId, x => primaryMemberId)
                .RuleFor(x => x.ipg_SecondaryMemberId, x => secondaryMemberid)
                .RuleFor(x => x.Id, x => Guid.Empty.Equals(self.Id)?Guid.NewGuid():self.Id);
            return contact;
        }

        public static Contact FakeWithPhysicianDetails(this Contact self, string physicianNpi, string taxonomyCode)
        {
            var contact = new Contact().Fake();
            contact.RuleFor(x => x.ipg_PhysicianNPI, x => physicianNpi)
                   .RuleFor(x => x.ipg_PhysicianTaxonomyCode, x => taxonomyCode)
                   ;

            return contact;
        }

        public static Contact FakeWithContactExtension(this Contact self)
        {
            var contact = new Contact().Fake();
            contact.RuleFor(x => x.ipg_BusinessExt, x => self.ipg_BusinessExt); 
            
            return contact;
        }

        public static Contact FakeWithMissingNPI(this Contact self, string missingNpi)
        {
          
            if(missingNpi =="")
            {
                self.ipg_PhysicianNPI = "";
            }
            var contact = new Contact().Fake();
            contact.RuleFor(x => x.ipg_PhysicianNPI, x => self.ipg_PhysicianNPI);

            return contact;
        }

        public static Contact FakeWithNPI(this Contact self)
        { 
            var contact = new Contact().Fake();
            contact.RuleFor(x => x.ipg_PhysicianNPI, x => self.ipg_PhysicianNPI);

            return contact;
        }

        public static Contact FakeWithDuplicateNPI(this Contact self, string npi)
        {
            var contact = new Contact().Fake();
            contact.RuleFor(x => x.ipg_PhysicianNPI, x => self.ipg_PhysicianNPI);

            if(npi.Equals(npi))
            {
                self.ipg_PhysicianNPI = "duplicate";
            }
            return contact;
        }

        public static Contact FakeWithFacilityUserStatus(this Contact self, ipg_facility_user_request_status_type facilityUserStatus)
        {
            var contact = new Contact().Fake();
            contact.RuleFor(x => x.ipg_facility_user_status_typecode, x => new OptionSetValue((int)facilityUserStatus))
            .RuleFor(x => x.ipg_facility_user_status_typecodeEnum, x => facilityUserStatus);

            return contact;
        }

        public static Faker<Contact> FakePortalContact(this Contact self)
        {
            return new Contact().Fake().RuleFor(x => x.adx_identity_securitystamp, x => "test");
        }

        public static Faker<Contact> WithEmail(this Faker<Contact> self, string email)
        {
            return self.RuleFor(x => x.EMailAddress1, x => email);
        }

    }
}