using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Bogus;
using Insight.Intake.Data;
using Insight.Intake.Extensions;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeIncident
    {
        public static Faker<Incident> Fake(this Incident self, IncidentState isActive = IncidentState.Active)
        {
            return new Faker<Incident>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.Title, x => x.Random.Int(10000, 99999).ToString())
                .RuleFor(x => x.TicketNumber, x => x.Random.Int(10000, 99999).ToString())
                .RuleFor(x => x.ipg_TodaysDate, x => DateTime.Now)
                .RuleFor(x => x.StateCode, x => isActive)
                .RuleFor(x => x.StatusCode, x => new OptionSetValue((int)Incident_StatusCode.New));
        }


        public static Faker<Incident> FakeActive(this Incident self)
        {
            return new Faker<Incident>()
                .RuleFor(x => x.StateCode, IncidentState.Active)
                .RuleFor(x => x.ipg_CaseStatus, new OptionSetValue((int)ipg_CaseStatus.Open))
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.Title, x => x.Random.Int(10000, 99999).ToString())
                .RuleFor(x => x.TicketNumber, x => x.Random.Int(10000, 99999).ToString())
                .RuleFor(x => x.ipg_TodaysDate, x => DateTime.Now);
        }
        public static Faker<Incident> Fake(this Incident self, Guid caseId)
        {
            return self.Fake().RuleFor(x => x.Id, x => caseId);
        }


        public static Faker<Incident> WithPatientCarrierBalance(this Faker<Incident> self, Decimal? patientBalance, Decimal? carrierBalance)
        {
            return self.RuleFor(x => x.ipg_RemainingPatientBalance, x => patientBalance.HasValue ? new Money(patientBalance.Value) : null)
                        .RuleFor(x => x.ipg_RemainingCarrierBalance, x => carrierBalance.HasValue ? new Money(carrierBalance.Value) : null);
        }

        public static Faker<Incident> WithDosHasBeenModified(this Faker<Incident> self, bool dosHasBeenModified)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_dosHasBeenModified, x => dosHasBeenModified);

            return self;
        }

        public static Faker<Incident> WithReferralTypeHasBeenModified(this Faker<Incident> self, bool referralTypeHasBeenModified)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_referraltypehasbeenmodified, x => referralTypeHasBeenModified);

            return self;
        }

        public static Faker<Incident> WithScheduledDos(this Faker<Incident> self, DateTime scheduledDos)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_SurgeryDate, x => scheduledDos);

            return self;
        }

        public static Faker<Incident> WithLastStatementType(this Faker<Incident> self, ipg_statementgenerationeventconfiguration statement)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_LastStatementType, x => statement.ToEntityReference());

            return self;
        }
        public static Faker<Incident> WithCreatedBy(this Faker<Incident> self, SystemUser user)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.CreatedBy, user.ToEntityReference());

            return self;
        }

        public static Faker<Incident> WithReferralType(this Faker<Incident> self, int referralType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_ReferralType, new OptionSetValue(referralType));

            return self;
        }

        public static Faker<Incident> WithGateConfigurationReference(this Faker<Incident> self, ipg_gateconfiguration gateConfiguration)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (gateConfiguration == null) throw new ArgumentNullException(nameof(gateConfiguration));

            self.RuleFor(x => x.ipg_gateconfigurationid, x => gateConfiguration.ToEntityReference());

            return self;
        }

        public static Faker<Incident> WithActualDos(this Faker<Incident> self, DateTime actualDos)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_ActualDOS, x => actualDos);
            self.RuleFor(x => x.ipg_SurgeryDate, x => actualDos);


            return self;
        }

        public static Faker<Incident> WithReferral(this Faker<Incident> self, EntityReference referralRef)
        {
            return self.RuleFor(x => x.ipg_ReferralId, x => referralRef);
        }

        public static Faker<Incident> WithIsAllReceivedDate(this Faker<Incident> self, DateTime allReceivedDate)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_isallreceiveddate, x => allReceivedDate);

            return self;
        }

        public static Faker<Incident> WithIsAllReceived(this Faker<Incident> self, bool isAllReceived)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_isallreceived, x => isAllReceived);

            return self;
        }

        public static Faker<Incident> WithIsAllReceivedBy(this Faker<Incident> self, EntityReference systemUserRef)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (systemUserRef == null) throw new ArgumentNullException(nameof(systemUserRef));

            self.RuleFor(x => x.ipg_allreceivedby, x => systemUserRef);

            return self;
        }

        public static Faker<Incident> WithOrigin(this Faker<Incident> self, Incident_CaseOriginCode origin)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.CaseOriginCode, x => new OptionSetValue((int)origin));

            return self;
        }

        public static Faker<Incident> WithProcedureNameReference(this Faker<Incident> self, ipg_procedurename procedureName)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (procedureName == null) throw new ArgumentNullException(nameof(procedureName));

            self.RuleFor(x => x.ipg_procedureid, x => procedureName.ToEntityReference());

            return self;
        }

        public static Faker<Incident> WithPatientReference(this Faker<Incident> self, Contact patient)
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

        public static Faker<Incident> WithPatientFullName(this Faker<Incident> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_PatientFirstName, x => x.Name.FirstName())
                .RuleFor(x => x.ipg_PatientLastName, x => x.Name.LastName());

            return self;
        }

        public static Faker<Incident> WithPatientProcedure(this Faker<Incident> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_ProcedureName, x => x.Random.String());

            return self;
        }

        public static Faker<Incident> WithCarrierReference(this Faker<Incident> self, Account carrier, bool isPrimaryCarrier = true)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (carrier == null) throw new ArgumentNullException(nameof(carrier));
            var carrierEntityRef = new EntityReference
            {
                Id = carrier.Id,
                LogicalName = carrier.LogicalName
            };
            if (isPrimaryCarrier)
            {
                self.RuleFor(
                x => x.ipg_CarrierId,
                x => new EntityReference
                {
                    Id = carrier.Id,
                    LogicalName = carrier.LogicalName
                })
                .RuleFor(x => x.ipg_primarycarriergroupidnumber, x => "12345");
            }
            else
            {
                self.RuleFor(
                     x => x.ipg_SecondaryCarrierId,
                     x => new EntityReference
                     {
                         Id = carrier.Id,
                         LogicalName = carrier.LogicalName
                     })
                     .RuleFor(x => x.ipg_SecondaryCarrierGroupIdNumber, x => "12345");
            }

            return self;
        }

        public static Faker<Incident> WithPrimaryCarrierReference(this Faker<Incident> self, Account carrier)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (carrier == null) throw new ArgumentNullException(nameof(carrier));

            self.RuleFor(
                x => x.ipg_CarrierId,
                x => new EntityReference
                {
                    Id = carrier.Id,
                    LogicalName = carrier.LogicalName,
                    Name = carrier.Name
                }
            )
            .RuleFor(x => x.ipg_primarycarriergroupidnumber, x => "12345");

            return self;
        }

        

        public static Faker<Incident> WithSecondaryCarrierReference(this Faker<Incident> self, Account carrier)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (carrier == null) throw new ArgumentNullException(nameof(carrier));

            self.RuleFor(
                x => x.ipg_SecondaryCarrierId,
                x => new EntityReference
                {
                    Id = carrier.Id,
                    LogicalName = carrier.LogicalName,
                    Name = carrier.Name
                }
            )
            .RuleFor(x => x.ipg_SecondaryCarrierGroupIdNumber, x => "12345");

            return self;
        }

        public static Faker<Incident> WithPrimaryClaimAddress(this Faker<Incident> self, ipg_carrierclaimsmailingaddress claimAddress)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (claimAddress == null) throw new ArgumentNullException(nameof(claimAddress));

            self.RuleFor(
                x => x.ipg_PrimaryCarrierClaimsMailingAddress,
                x => new EntityReference
                {
                    Id = claimAddress.Id,
                    LogicalName = claimAddress.LogicalName,
                }
            );

            return self;
        }

        public static Faker<Incident> WithSecondaryClaimAddress(this Faker<Incident> self, ipg_carrierclaimsmailingaddress claimAddress)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (claimAddress == null) throw new ArgumentNullException(nameof(claimAddress));

            self.RuleFor(
                x => x.ipg_SecondaryCarrierClaimsMailingAddress,
                x => new EntityReference
                {
                    Id = claimAddress.Id,
                    LogicalName = claimAddress.LogicalName,
                }
            );

            return self;
        }


        public static Faker<Incident> WithFacilityReference(this Faker<Incident> self, Account facility)
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

            return self;
        }

        public static Faker<Incident> WithDeviceStageOptionSet(this Faker<Incident> self, int deviceStage)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_DeviceStage, x => new OptionSetValue(deviceStage));

            return self;
        }

        public static Faker<Incident> FakeWithPatientDemographics(this Faker<Incident> self, Contact patient)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (patient == null) throw new ArgumentNullException(nameof(patient));

            self.RuleFor(x => x.ipg_PatientFirstName, patient.FirstName)
                .RuleFor(x => x.ipg_PatientLastName, patient.LastName)
                .RuleFor(x => x.ipg_PatientDateofBirth, patient.BirthDate)
                .RuleFor(x => x.ipg_PatientAddress, patient.Address1_Line1)
                .RuleFor(x => x.ipg_PatientCity, patient.Address1_City)
                .RuleFor(x => x.ipg_PatientState, patient.Address1_StateOrProvince)
                .RuleFor(x => x.ipg_PatientZip, patient.Address1_PostalCode)
                .RuleFor(x => x.ipg_PatientZipCodeId, new EntityReference(ipg_zipcode.EntityLogicalName, Guid.NewGuid()));

            return self;
        }

        public static Faker<Incident> FakeWithDxCode(this Faker<Incident> self, ipg_dxcode dxCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (dxCode == null) throw new ArgumentNullException(nameof(dxCode));

            self.RuleFor(x => x.ipg_DxCodeId1, new EntityReference
            {
                Id = dxCode.Id,
                LogicalName = ipg_dxcode.EntityLogicalName
            });

            return self;
        }

        public static Faker<Incident> FakeWithCptCode(this Faker<Incident> self, ipg_cptcode cptCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (cptCode == null) throw new ArgumentNullException(nameof(cptCode));


            self.RuleFor(x => x.ipg_CPTCodeId1, new EntityReference
            {
                Id = cptCode.Id,
                LogicalName = ipg_cptcode.EntityLogicalName
            });

            return self;
        }

        public static Faker<Incident> FakeWithPhysician(this Faker<Incident> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_PhysicianId, new EntityReference
            {
                Id = Guid.NewGuid(),
                LogicalName = Contact.EntityLogicalName
            });

            return self;
        }

        public static Faker<Incident> WithPhysician(this Faker<Incident> self, Contact physician)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_PhysicianId, new EntityReference
            {
                Id = physician.Id,
                LogicalName = Contact.EntityLogicalName
            });

            return self;
        }

        public static Faker<Incident> WithInsuredPatient(this Faker<Incident> self, new_RelationtoInsured relationToInsured, string patientPrefix = "", string insuredPrefix = "")
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.RuleFor(x => x.ipg_MemberIdNumber, x => "12345")
                .RuleFor(x => x.ipg_RelationToInsured, x => relationToInsured.ToOptionSetValue())
                .RuleFor(x => x.ipg_InsuredFirstName, x => insuredPrefix + "FirstName")
                .RuleFor(x => x.ipg_InsuredLastName, x => insuredPrefix + "LastName")
                .RuleFor(x => x.ipg_InsuredMiddleName, x => insuredPrefix + "MiddleName")
                .RuleFor(x => x.ipg_InsuredDateOfBirth, x => new DateTime(1961, 1, 25))
                .RuleFor(x => x.ipg_InsuredGender, x => new OptionSetValue((int)Genders.Male))
                .RuleFor(x => x.ipg_insuredzip, x => "12345")
                .RuleFor(x => x.ipg_insuredphone, x => "1111111")
                .RuleFor(x => x.ipg_insuredaddress, x => insuredPrefix + "Address")
                .RuleFor(x => x.ipg_insuredcity, x => insuredPrefix + "City")
                .RuleFor(x => x.ipg_insuredsid, x => "ins12345")
                .RuleFor(x => x.ipg_insuredstate, x => "CA")
                .RuleFor(x => x.ipg_PatientFirstName, x => patientPrefix + "FirstName")
                .RuleFor(x => x.ipg_PatientLastName, x => patientPrefix + "LastName")
                .RuleFor(x => x.ipg_PatientMiddleName, x => patientPrefix + "MiddleName")
                .RuleFor(x => x.ipg_PatientDateofBirth, x => new DateTime(1961, 1, 25))
                .RuleFor(x => x.ipg_PatientGender, x => new OptionSetValue((int)Genders.Male))
                .RuleFor(x => x.ipg_PatientAddress, x => patientPrefix + "Address")
                .RuleFor(x => x.ipg_PatientCellPhone, x => "2222222")
                .RuleFor(x => x.ipg_PatientCity, x => patientPrefix + "City")
                .RuleFor(x => x.ipg_PatientEmail, x => "patient@email.com")
                .RuleFor(x => x.ipg_PatientHomePhone, x => "22222222")
                .RuleFor(x => x.ipg_PatientState, x => "CA")
                .RuleFor(x => x.ipg_PatientZip, x => "12345")
                .RuleFor(x => x.ipg_policyid, x => "pol111111")
                ;

            return self;
        }

        public static Faker<Incident> WithPatientZipCode(this Faker<Incident> self, ipg_melissazipcode zip)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.RuleFor(x => x.ipg_CasePatientZipCodeId, x => zip.ToEntityReference());
            return self;
        }

        public static Faker<Incident> WithInsuredZipCode(this Faker<Incident> self, ipg_melissazipcode zip)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.RuleFor(x => x.ipg_caseinsuredzipcodeId, x => zip.ToEntityReference());
            return self;
        }


        public static Faker<Incident> WithSecondaryInsuredZipCode(this Faker<Incident> self, ipg_melissazipcode zip)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.RuleFor(x => x.ipg_melissacsecondaryZipCodeId, x => zip.ToEntityReference());
            return self;
        }


        public static Faker<Incident> FakeWithSecondaryInsured(this Faker<Incident> self, int relationToInsured)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_secondaryinsuredfirstname, x => "SecFirstName")
                .RuleFor(x => x.ipg_secondaryinsuredlastname, x => "SecLastName")
                .RuleFor(x => x.ipg_secondaryinsuredmiddlename, x => "S MiddleName")
                .RuleFor(x => x.ipg_secondaryinsureddateofbirth, x => new DateTime(1970, 10, 15))
                .RuleFor(x => x.ipg_secondaryinsuredgender, x => new OptionSetValue((int)Genders.Male))
                .RuleFor(x => x.ipg_secondaryinsuredemployer, x => "Sec Employer")
                .RuleFor(x => x.ipg_SecondaryCarrierRelationToInsured, x => new OptionSetValue(relationToInsured))
                .RuleFor(x => x.ipg_policyid2, x => "pol222222")
                ;

            return self;
        }

        public static Incident FakeInsuredPatient(this Incident self, Account carrier)
        {
            return new Incident
            {
                Id = Guid.NewGuid(),
                ipg_CarrierId = new EntityReference
                {
                    Id = carrier.Id,
                    LogicalName = Account.EntityLogicalName
                },
                ipg_MemberIdNumber = "12345",
                ipg_PatientFirstName = "FirstName1",
                ipg_PatientLastName = "LastName1",
                ipg_PatientMiddleName = "MiddleName1",
                ipg_PatientDateofBirth = new DateTime(1960, 1, 25),
                ipg_PatientGender = new OptionSetValue(FakeOptionSetMetadata.Gender().OptionSet.Options.First(m => m.Label.UserLocalizedLabel.Label == "Male").Value.Value),
                ipg_SurgeryDate = new DateTime(2018, 9, 20),
                ipg_EBVResult = new OptionSetValue((int)ipg_EBVResults.ELIGIBLE)
            };
        }

        public static Incident FakeDependentPatient(this Incident self, Account carrier)
        {
            return new Incident
            {
                ipg_CarrierId = new EntityReference
                {
                    Id = carrier.Id,
                    LogicalName = Account.EntityLogicalName
                },
                ipg_MemberIdNumber = "12345",
                ipg_RelationToInsured = new OptionSetValue((int)new_RelationtoInsured.Child),
                ipg_InsuredFirstName = "FirstName1",
                ipg_InsuredLastName = "LastName1",
                ipg_InsuredMiddleName = "MiddleName1",
                ipg_InsuredDateOfBirth = new DateTime(1960, 1, 25),
                ipg_InsuredGender = new OptionSetValue(FakeOptionSetMetadata.Gender().OptionSet.Options.First(m => m.Label.UserLocalizedLabel.Label == "Male").Value.Value),
                ipg_PatientFirstName = "FirstName2",
                ipg_PatientLastName = "LastName2",
                ipg_PatientMiddleName = "MiddleName2",
                ipg_PatientDateofBirth = new DateTime(1961, 2, 26),
                ipg_PatientGender = new OptionSetValue(FakeOptionSetMetadata.Gender().OptionSet.Options.First(m => m.Label.UserLocalizedLabel.Label == "Female").Value.Value),
                ipg_SurgeryDate = new DateTime(2018, 9, 20)
            };
        }



        public static Incident FakeIncidentWithPrimaryCarrierSecondaryCarrier(this Incident self, Account carrier, Account sCarrier, Contact contact)
        {
            return new Incident
            {
                Id = Guid.Empty.Equals(self.Id) ? Guid.NewGuid() : self.Id,
                ipg_CarrierId = new EntityReference
                {
                    Id = carrier?.Id ?? Guid.Empty,
                    LogicalName = Account.EntityLogicalName,
                    Name = carrier.Name
                },
                ipg_SecondaryCarrierId = new EntityReference
                {
                    Id = sCarrier?.Id ?? Guid.Empty,
                    LogicalName = Account.EntityLogicalName,
                    Name = carrier.Name
                },
                ipg_MemberIdNumber = contact.ipg_PrimaryMemberId,
                ipg_SecondaryMemberIdNumber = contact.ipg_SecondaryMemberId,
                ipg_SurgeryDate = new DateTime(2018, 10, 12)
            };
        }

        public static Faker<Incident> FakeWithClaimDetails(this Faker<Incident> self, bool profitApproved, bool approvalRequired, bool promotedForCollection)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.RuleFor(x => x.ipg_primarycarrierplantype, new OptionSetValue((int)ipg_CarrierType.Commercial))
                .RuleFor(x => x.ipg_billedcpt, x => "123")
                .RuleFor(x => x.ipg_carrierauthno, x => x.Random.String(8))
                .RuleFor(x => x.ipg_benefitplan, false)
                .RuleFor(x => x.Description, x => "case description")
                .RuleFor(x => x.ipg_pos, x => new OptionSetValue((int)ipg_facilitytype._11PhysicianOffice))
                .RuleFor(x => x.ipg_profitapproved, x => profitApproved)
                .RuleFor(x => x.ipg_approvalrequired, x => approvalRequired)
                .RuleFor(x => x.ipg_ispromotedforcollection, x => promotedForCollection)
                .RuleFor(x => x.StatusCode, x => new OptionSetValue((int)Incident_StatusCode.InProgress))
                ;

            return self;
        }

        public static Faker<Incident> WithAutoCarrierReference(this Faker<Incident> self, Account autoCarrier)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (autoCarrier == null) throw new ArgumentNullException(nameof(autoCarrier));
            var carrierEntityRef = new EntityReference
            {
                Id = autoCarrier.Id,
                LogicalName = autoCarrier.LogicalName
            };

            self.RuleFor(x => x.ipg_AutoCarrierId, x => carrierEntityRef);

            return self;
        }

        public static Faker<Incident> WithAutoCarrier(this Faker<Incident> self, bool? autoCarrier)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_AutoCarrier, x => autoCarrier);

            return self;
        }

        public static Faker<Incident> WithBenefitsExhausted(this Faker<Incident> self, bool? benefitsExhausted)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_medicalbenefitsexhausted, x => benefitsExhausted);

            return self;
        }

        public static Faker<Incident> WithOwner(this Faker<Incident> self, Entity owner)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            self.RuleFor(x => x.OwnerId, x => owner.ToEntityReference());

            return self;
        }

        public static Faker<Incident> WithAssignedToTeam(this Faker<Incident> self, Team team)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (team == null) throw new ArgumentNullException(nameof(team));

            self.RuleFor(x => x.ipg_assignedtoteamid, x => team.ToEntityReference());
            return self;
        }

        public static Faker<Incident> WithLFStep(this Faker<Incident> self, ipg_lifecyclestep lfstep)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (lfstep == null) throw new ArgumentNullException(nameof(lfstep));

            self.RuleFor(x => x.ipg_lifecyclestepid, x => lfstep.ToEntityReference());

            return self;
        }

        public static Faker<Incident> WithCaseStatusDisplayed(this Faker<Incident> self, ipg_casestatusdisplayed caseStatus)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (caseStatus == null) throw new ArgumentNullException(nameof(caseStatus));

            self.RuleFor(x => x.ipg_casestatusdisplayedid, x => caseStatus.ToEntityReference());

            return self;
        }

        public static Faker<Incident> WithMissingInformationCategoryTask(this Faker<Incident> self, EntityReference task)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (task == null) throw new ArgumentNullException(nameof(task));

            self.RuleFor(x => x.ipg_missinginformationcategorytaskid, x => task);

            return self;
        }

        public static Faker<Incident> WithBilledCptCode(this Faker<Incident> self, ipg_cptcode cptCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (cptCode == null) throw new ArgumentNullException(nameof(cptCode));


            self.RuleFor(x => x.ipg_BilledCPTId, new EntityReference
            {
                Id = cptCode.Id,
                LogicalName = ipg_cptcode.EntityLogicalName
            });

            return self;
        }

        public static Faker<Incident> WithCaseStatus(this Faker<Incident> self, int caseStatus)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_CaseStatus, x => new OptionSetValue(caseStatus));

            return self;
        }

        public static Faker<Incident> WithCoverageLevels(this Faker<Incident> self, ipg_BenefitCoverageLevels deductibleCoverageLevel,
            ipg_BenefitCoverageLevels oopCoverageLevel)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_CoverageLevelDeductibleEnum, x => deductibleCoverageLevel);
            self.RuleFor(x => x.ipg_CoverageLevelDeductible, x => new OptionSetValue((int)deductibleCoverageLevel));

            self.RuleFor(x => x.ipg_CoverageLevelOOPEnum, x => oopCoverageLevel);
            self.RuleFor(x => x.ipg_CoverageLevelOOP, x => new OptionSetValue((int)oopCoverageLevel));


            return self;
        }

        public static Faker<Incident> WithBenefitType(this Faker<Incident> self, ipg_BenefitType benefitType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_BenefitTypeCodeEnum, x => benefitType);
            self.RuleFor(x => x.ipg_BenefitTypeCode, x => new OptionSetValue((int)benefitType));

            return self;
        }

        public static Faker<Incident> WithSecondaryBenefitType(this Faker<Incident> self, ipg_BenefitType benefitType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_Carrier2BenefitTypeCode, x => new OptionSetValue((int)benefitType));

            return self;
        }

        public static Faker<Incident> WithInOutNetwork(this Faker<Incident> self, bool inOutNetwork)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_inoutnetwork, x => inOutNetwork);

            return self;
        }

        public static Faker<Incident> WithState(this Faker<Incident> self, int stateCode)
        {
            return self.RuleFor(x => x.ipg_StateCode, x => new OptionSetValue(stateCode));
        }

        public static Faker<Incident> WithState(this Faker<Incident> self, ipg_CaseStateCodes stateCode)
        {
            return self.RuleFor(x => x.ipg_StateCode, x => stateCode.ToOptionSetValue());
        }

        public static Faker<Incident> WithInOutNetwork2(this Faker<Incident> self, bool inOutNetwork)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_Carrier2IsInOutNetwork, x => inOutNetwork);

            return self;
        }

        public static Faker<Incident> WithMemberId(this Faker<Incident> self, string memberId)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_MemberIdNumber, x => memberId);

            return self;
        }

        public static Faker<Incident> WithSecondaryMemberId(this Faker<Incident> self, string memberId)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_SecondaryMemberIdNumber, x => memberId);

            return self;
        }

        public static Faker<Incident> WithCMAssigned(this Faker<Incident> self, EntityReference caseManager)
        {
            return self.RuleFor(x => x.ipg_cmassignedid, x => caseManager);
        }

        public static Faker<Incident> WithContactPatient(this Faker<Incident> self, bool contact)
        {
            return self.RuleFor(x => x.ipg_DoNotContactPatient, x => !contact);
        }

        public static Faker<Incident> WithPaymentPlanType(this Faker<Incident> self, int paymentPlanType)
        {
            return self.RuleFor(x => x.ipg_PaymentPlanType, x => new OptionSetValue(paymentPlanType));
        }

        public static Faker<Incident> WithAddress(this Faker<Incident> self, ipg_melissazipcode zipCode, string state, string city, string address)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_CasePatientZipCodeId, x => zipCode.ToEntityReference())
                .RuleFor(x => x.ipg_PatientState, x => state)
                .RuleFor(x => x.ipg_PatientCity, x => city)
                .RuleFor(x => x.ipg_PatientAddress, x => address)
                ;
            return self;
        }

        public static Faker<Incident> WithEhrAddress(this Faker<Incident> self, string zipCode, string state, string city, string address)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_EhrZipCode, x => zipCode)
                .RuleFor(x => x.ipg_EhrState, x => state)
                .RuleFor(x => x.ipg_EhrCity, x => city)
                .RuleFor(x => x.ipg_EhrAddress, x => address)
                ;
            return self;
        }

        public static Faker<Incident> WithLock(this Faker<Incident> self, bool islocked)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_islocked, x => islocked);
            return self;
        }
        
    }
}