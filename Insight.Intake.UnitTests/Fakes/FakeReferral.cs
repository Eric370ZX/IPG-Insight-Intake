using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Bogus;
using Insight.Intake.Extensions;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeReferral
    {
        public static Faker<ipg_referral> Fake(this ipg_referral self, Guid referralId)
        {
            return self.Fake().RuleFor(x => x.Id, x => referralId);
        }

        public static Faker<ipg_referral> WithLFstep(this Faker<ipg_referral> self, ipg_lifecyclestep lfstep)
        {
            return self.RuleFor(x => x.ipg_lifecyclestepid, lfstep.ToEntityReference())
                .RuleFor(x => x.ipg_gateconfigurationid, lfstep.ipg_gateconfigurationid);
        }

        public static Faker<ipg_referral> Fake(this ipg_referral self)
        {
            return new Faker<ipg_referral>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_PatientFirstName, x => x.Name.FindName())
                .RuleFor(x => x.ipg_PatientMiddleName, x => x.Name.Prefix())
                .RuleFor(x => x.ipg_PatientLastName, x => x.Name.LastName())
                .RuleFor(x => x.ipg_PatientDateofBirth, x => x.Date.Past(30))
                .RuleFor(x => x.ipg_SurgeryDate, x => x.Date.Future(2));
        }
        public static Faker<ipg_referral> WithPortalUser(this Faker<ipg_referral> self, Guid? guid=null)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                 x => x.ipg_portaluserid,
                    x => new EntityReference
                    {
                        Id = guid ?? new Guid(),
                        LogicalName = Contact.EntityLogicalName
                    }
            );

            return self;
        }

        public static Faker<ipg_referral> WithOwner(this Faker<ipg_referral> self, SystemUser systemUser )
        {
            return self.RuleFor(x => x.OwnerId, x => systemUser.ToEntityReference());
        }

        public static Faker<ipg_referral> WithAssignedToTeam(this Faker<ipg_referral> self, Team team)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (team == null) throw new ArgumentNullException(nameof(team));

            self.RuleFor(x => x.ipg_assignedtoteamid, x => team.ToEntityReference());
            return self;
        }

        public static Faker<ipg_referral> WithCptCodeReferences(this Faker<ipg_referral> self, IList<ipg_cptcode> cptCodes)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (cptCodes == null) throw new ArgumentNullException(nameof(cptCodes));

            if (cptCodes.Count > 6)
            {
                cptCodes = cptCodes.Take(6).ToList();
            }

            for (var index = 0; index < cptCodes.Count; index++)
            {
                var cptCode = cptCodes[index];
                var reference = new EntityReference
                {
                    Id = cptCode.Id,
                    LogicalName = cptCode.LogicalName,
                    Name = cptCode.ipg_name
                };

                switch (index)
                {
                    case 0: self.RuleFor(x => x.ipg_CPTCodeId1, x => reference); break;
                    case 1: self.RuleFor(x => x.ipg_CPTCodeId2, x => reference); break;
                    case 2: self.RuleFor(x => x.ipg_CPTCodeId3, x => reference); break;
                    case 3: self.RuleFor(x => x.ipg_CPTCodeId4, x => reference); break;
                    case 4: self.RuleFor(x => x.ipg_CPTCodeId5, x => reference); break;
                    case 5: self.RuleFor(x => x.ipg_CPTCodeId6, x => reference); break;
                }
            }

            return self;
        }

        public static Faker<ipg_referral> WithProcedureName(this Faker<ipg_referral> self, ipg_procedurename procedureName)
        {
            return self.RuleFor(x => x.ipg_ProcedureNameId, x => procedureName.ToEntityReference());
        }
        public static Faker<ipg_referral> WithFacilityReference(this Faker<ipg_referral> self, Account facility)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (facility == null) throw new ArgumentNullException(nameof(facility));

            self.RuleFor(
                x => x.ipg_FacilityId,
                x => new EntityReference
                {
                    Id = facility.Id,
                    LogicalName = facility.LogicalName
                }
            );

            self.RuleFor(x => x.ipg_Facility, x => facility.Name);

            return self;
        }

        public static Faker<ipg_referral> WithSourceDocument(this Faker<ipg_referral> self, ipg_document document)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (document == null) throw new ArgumentNullException(nameof(document));

            self.RuleFor(x => x.ipg_SourceDocumentId, x => document.ToEntityReference());

            return self;
        }

        public static Faker<Invoice> WithCaseReference(this Faker<Invoice> self, Incident caseRef)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (caseRef == null)
            {
                throw new ArgumentNullException(nameof(caseRef));
            }

            self.RuleFor(
                x => x.ipg_caseid,
                x => new EntityReference
                {
                    Id = caseRef.Id,
                    LogicalName = caseRef.LogicalName
                }
            );

            //self.RuleFor(x => x.ipg_caseid, x => caseRef.Name);

            return self;
        }

        public static Faker<ipg_referral> WithCaseReference(this Faker<ipg_referral> self, Incident caseRef)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (caseRef == null)
            {
                throw new ArgumentNullException(nameof(caseRef));
            }

            self.RuleFor(
                x => x.ipg_AssociatedCaseId,
                x => new EntityReference
                {
                    Id = caseRef.Id,
                    LogicalName = caseRef.LogicalName
                }
            );

            return self;
        }
        public static Faker<ipg_referral> WithCarrierReference(this Faker<ipg_referral> self, Account carrier)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (carrier == null) throw new ArgumentNullException(nameof(carrier));

            self.RuleFor(
                x => x.ipg_CarrierId,
                x => new EntityReference
                {
                    Id = carrier.Id,
                    LogicalName = carrier.LogicalName
                }
            );

            self.RuleFor(x => x.ipg_Carrier, x => carrier.Name);

            return self;
        }

        public static Faker<ipg_referral> WithPatientReference(this Faker<ipg_referral> self, Contact patient)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (patient == null) throw new ArgumentNullException(nameof(patient));

            self.RuleFor(
                x => x.ipg_PatientId,
                x => new EntityReference
                {
                    Id = patient.Id,
                    LogicalName = patient.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_referral> WithDisplayedStatus(this Faker<ipg_referral> self, ipg_casestatusdisplayed status)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (status == null) throw new ArgumentNullException(nameof(status));

            self.RuleFor(
                x => x.ipg_casestatusdisplayedid,
                x => new EntityReference
                {
                    Id = status.Id,
                    LogicalName = status.LogicalName,
                    Name = status.ipg_name
                }
            );

            return self;
        }

        public static Faker<ipg_referral> WithGateConfigurationReference(this Faker<ipg_referral> self, ipg_gateconfiguration gateConfiguration)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (gateConfiguration == null) throw new ArgumentNullException(nameof(gateConfiguration));

            self.RuleFor(
                x => x.ipg_gateconfigurationid,
                x => new EntityReference
                {
                    Id = gateConfiguration.Id,
                    LogicalName = gateConfiguration.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_referral> WithCaseStatusDisplayed(this Faker<ipg_referral> self, ipg_casestatusdisplayed caseStatus)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (caseStatus == null) throw new ArgumentNullException(nameof(caseStatus));

            self.RuleFor(x => x.ipg_casestatusdisplayedid, x => caseStatus.ToEntityReference());

            return self;
        }

        public static Faker<ipg_referral> WithSurgeryDate(this Faker<ipg_referral> self, DateTime date)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_SurgeryDate, x => date);
            return self;
        }

        public static Faker<ipg_referral> WithReferralNumber(this Faker<ipg_referral> self, string referralNumber)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_referralcasenumber, x => referralNumber);
            return self;
        }

        public static Faker<ipg_referral> WithOrigin(this Faker<ipg_referral> self, Incident_CaseOriginCode originCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_Origin, x => originCode.ToOptionSetValue());

            return self;
        }

        public static Faker<ipg_referral> WithReferralType(this Faker<ipg_referral> self, ipg_ReferralType type)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_referraltype, x => type.ToOptionSetValue());

            return self;
        }

        public static Faker<ipg_referral> WithCaseStatus(this Faker<ipg_referral> self, ipg_CaseStatus status)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_casestatus, x => status.ToOptionSetValue());

            return self;
        }

        public static Faker<ipg_referral> WithCreatedOn(this Faker<ipg_referral> self, DateTime date)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.CreatedOn, x => date);
            self.RuleFor(x => x.OverriddenCreatedOn, x => date);

            return self;
        }

        public static Faker<ipg_referral> WithProcedureNameReference(this Faker<ipg_referral> self, EntityReference procedureNameReference)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_ProcedureNameId, x => procedureNameReference);
            return self;
        }

        public static Faker<ipg_referral> WithOutcomeCode(this Faker<ipg_referral> self, ipg_OutcomeCodes outcomeCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_OutcomeCodeEnum, x => outcomeCode);
            return self;
        }

        public static Faker<ipg_referral> WithStateCode(this Faker<ipg_referral> self, ipg_referralState stateCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.StateCode, x => stateCode);
            return self;
        }

        public static Faker<ipg_referral> WithCaseOutcome(this Faker<ipg_referral> self, ipg_CaseOutcomeCodes caseOutcomeCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_caseoutcomeEnum, x => caseOutcomeCode);
            return self;
        }

        public static Faker<ipg_referral> WithStateCode(this Faker<ipg_referral> self, ipg_CaseStateCodes stateCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_statecodeEnum, x => stateCode);
            return self;
        }

        public static Faker<ipg_referral> WithAddress(this Faker<ipg_referral> self, ipg_melissazipcode zipCode, string state, string city, string address)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_melissapatientzipcodeId, x => zipCode.ToEntityReference())
                .RuleFor(x => x.ipg_melissapatientstate, x => state)
                .RuleFor(x => x.ipg_melissapatientcity, x => city)
                .RuleFor(x => x.ipg_PatientAddress, x => address)
                ;
            return self;
        }

        public static Faker<ipg_referral> WithEhrAddress(this Faker<ipg_referral> self, string zipCode, string state, string city, string address)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_EhrZipCode, x => zipCode)
                .RuleFor(x => x.ipg_EhrState, x => state)
                .RuleFor(x => x.ipg_EhrCity, x => city)
                .RuleFor(x => x.ipg_EhrAddress, x => address)
                ;
            return self;
        }

    }
}