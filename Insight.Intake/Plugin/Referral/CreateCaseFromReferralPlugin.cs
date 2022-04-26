using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Referral
{
    public class CreateCasePlugin : PluginBase
    {
        private IOrganizationService service;
        public CreateCasePlugin() : base(typeof(CreateCasePlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGReferralCreateCase", ipg_referral.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localContext)
        {
            var context = localContext.PluginExecutionContext;
            var tracingService = localContext.TracingService;
            service = localContext.OrganizationService;
            var referralRef = localContext.TargetRef();

            tracingService.Trace("Retrieving Referral");
            var referral = service.Retrieve(referralRef.LogicalName, referralRef.Id, new ColumnSet(true)).ToEntity<ipg_referral>();

            tracingService.Trace("Retrieving Carrier");
            var carrier = referral.ipg_CarrierId != null ?
                service.Retrieve(referral.ipg_CarrierId.LogicalName, referral.ipg_CarrierId.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType))
                .ToEntity<Intake.Account>() : null;

            tracingService.Trace("Retrieving Patient");
            var contact = referral.ipg_PatientId != null ? service.Retrieve(referral.ipg_PatientId.LogicalName, referral.ipg_PatientId.Id, new ColumnSet(Intake.Contact.Fields.Suffix)).ToEntity<Intake.Contact>() : null;

            tracingService.Trace("Creating Incident");
            var incident = new Incident()
            {
                ipg_ReferralId = referral.ToEntityReference(),
                ipg_referral_date = referral.CreatedOn,
                ipg_ReferralNumber = referral.ipg_name,
                ipg_IntakeMethod = referral.ipg_Origin,
                ipg_MemberIdNumber = referral.ipg_memberidnumber,
                ipg_SecondaryMemberIdNumber = referral.ipg_memberidnumber2,
                ipg_AutoClaimNumber = referral.ipg_autoclaimnumber,
                ipg_decisiondate = DateTime.UtcNow,
                ipg_referralcreatedate = referral.CreatedOn,

                Title = referral.ipg_referralcasenumber,
                ipg_CarrierId = referral.ipg_CarrierId,
                ipg_SecondaryCarrierId = referral.ipg_carriername2id,
                ipg_FacilityId = referral.ipg_FacilityId,
                ipg_FacilityMRN = referral.ipg_FacilityMRN,

                ipg_ReferralType = referral.ipg_referraltype,
                OwnerId = referral.OwnerId,

                ipg_SurgeryDate = referral.ipg_SurgeryDate,
                ipg_ActualDOS = referral.ipg_actualdos,
                ipg_procedureid = referral.ipg_ProcedureNameId,
                ipg_PhysicianId = referral.ipg_PhysicianId,
                ipg_PV2Description = referral.ipg_PV2Description,
                ipg_primarycarriergroupidnumber = referral.ipg_groupidnumber,
                ipg_SecondaryCarrierGroupIdNumber = referral.ipg_groupidnumber2,

                ipg_PatientId = referral.ipg_PatientId,
                ipg_patientsuffix = contact?.Suffix,
                CustomerId = referral.ipg_PatientId,
                ipg_PatientFirstName = referral.ipg_PatientFirstName,
                ipg_PatientMiddleName = referral.ipg_PatientMiddleName,
                ipg_PatientLastName = referral.ipg_PatientLastName,
                ipg_PatientDateofBirth = referral.ipg_PatientDateofBirth,
                ipg_PatientGender = referral.ipg_gender,
                ipg_PatientAddress = referral.ipg_PatientAddress,
                ipg_PatientCity = referral.ipg_PatientCity,
                ipg_PatientState = referral.ipg_PatientState,
                ipg_CasePatientZipCodeId = referral.ipg_melissapatientzipcodeId,
                ipg_PatientHomePhone = referral.ipg_homephone,
                ipg_PatientCellPhone = referral.ipg_cellphone,
                ipg_PatientWorkPhone = referral.ipg_workphone,
                ipg_PatientEmail = referral.ipg_email,

                ipg_InsuredFirstName = referral.ipg_insuredfirstname,
                ipg_InsuredLastName = referral.ipg_insuredlastname,
                ipg_insuredaddress = referral.ipg_InsuredAddress,
                ipg_insuredcity = referral.ipg_InsuredCity,
                ipg_insuredstate = referral.ipg_InsuredState,
                ipg_caseinsuredzipcodeId = referral.ipg_melissazipcodereferralinsuredId,
                ipg_InsuredDateOfBirth = referral.ipg_insureddateofbirth,
                ipg_insuredphone = referral.ipg_InsuredPhone,

                ipg_PrimaryCPTCodeOrProcedureName = referral.ipg_PrimaryCPTCodeOrProcedureName,
                ipg_BilledCPTId = referral.ipg_billedcptid,

                ipg_CPTCodeId1 = referral.ipg_CPTCodeId1,
                ipg_CPTCodeId2 = referral.ipg_CPTCodeId2,
                ipg_CPTCodeId3 = referral.ipg_CPTCodeId3,
                ipg_CPTCodeId4 = referral.ipg_CPTCodeId4,
                ipg_CPTCodeId5 = referral.ipg_CPTCodeId5,
                ipg_CPTCodeId6 = referral.ipg_CPTCodeId6,


                ipg_DxCodeId1 = referral.ipg_DxCodeId1,
                ipg_DxCodeId2 = referral.ipg_DxCodeId2,
                ipg_DxCodeId3 = referral.ipg_DxCodeId3,
                ipg_DxCodeId4 = referral.ipg_dxcodeid4,
                ipg_DxCodeId5 = referral.ipg_dxcodeid5,
                ipg_DxCodeId6 = referral.ipg_dxcodeid6,

                ipg_EHRCaseId = referral.ipg_EHRCaseId,
                ipg_EHRDataSource = referral.ipg_EHRDataSource,
                ipg_EhrFileFormatVersion = referral.ipg_EHRFileFormatVersion,
                ipg_EHRProcedureName = referral.ipg_EHRProcedureName
            };

            if (carrier?.ipg_carrierplantypesEnum == Account_ipg_carrierplantypes.WorkersComp)
            {
                incident.ipg_deductible = new Money(0);
                incident.ipg_deductiblemet = new Money(0);
                incident.ipg_deductibleremaining = new Money(0);
                incident.ipg_payercoinsurance = 100;
                incident.ipg_patientcoinsurance = 0;
                incident.ipg_oopmax = new Money(0);
                incident.ipg_oopmet = new Money(0);
                incident.ipg_oopremaining = new Money(0);
                incident.ipg_primarycarriereffectivedate = new DateTime(referral.ipg_SurgeryDate.Value.Year, 1, 1);
                incident.ipg_SecondaryCarrierExpirationDate = new DateTime(9999, 12, 31);
            }

            if (context.InputParameters.Contains("CaseState"))
            {
                tracingService.Trace(((OptionSetValue)context.InputParameters["CaseState"]).Value.ToString());
                incident.ipg_StateCode = (OptionSetValue)context.InputParameters["CaseState"];
            }

            if (referral.ipg_referraltypeEnum == ipg_ReferralType.Retro)
            {
                incident.ipg_ActualDOS = incident.ipg_SurgeryDate;
            }

            incident.Id = service.Create(incident);

            var claimsMailingAddresses = GetClaimsMailingAddresses(incident.ToEntityReference(), referral.ipg_CarrierId, referral.ipg_carriername2id, tracingService);
            if (claimsMailingAddresses[0] != null || claimsMailingAddresses[1] != null)
            {
                var incidentUpdateClaimsMailingAddress = new Incident()
                {
                    Id = incident.Id,
                    ipg_PrimaryCarrierClaimsMailingAddress = claimsMailingAddresses[0],
                    ipg_SecondaryCarrierClaimsMailingAddress = claimsMailingAddresses[1]
                };
                service.Update(incidentUpdateClaimsMailingAddress);
            }

            tracingService.Trace("Copying Portal comments");
            var comments = RetrieveRelatedToReferralComments(referral.Id).Select(comment => comment.ToEntity<adx_portalcomment>());
            if (comments.Count() > 0)
            {
                foreach (var comment in comments)
                {
                    var caseComment = new Entity(adx_portalcomment.EntityLogicalName).ToEntity<adx_portalcomment>();

                    caseComment.PriorityCode = comment.PriorityCode;
                    caseComment.adx_PortalCommentDirectionCode = comment.adx_PortalCommentDirectionCode;
                    caseComment.StatusCode = comment.StatusCode;
                    caseComment.OwnerId = comment.OwnerId;

                    caseComment.ipg_from = comment.ipg_from;
                    caseComment.From = CreateActivityParties(comment.From);
                    caseComment.ipg_to = comment.ipg_to;
                    caseComment.To = CreateActivityParties(comment.To);
                    caseComment.ipg_Type = comment.ipg_Type;
                    caseComment.ipg_patientname = comment.ipg_patientname;
                    caseComment.ipg_facilitypatientid = comment.ipg_facilitypatientid;

                    caseComment.Subject = comment.Subject;
                    caseComment.Description = comment.Description;

                    caseComment.ipg_owningportaluserid = comment.ipg_owningportaluserid;
                    caseComment.ipg_POID = comment.ipg_POID;
                    caseComment.ipg_FacilityId = comment.ipg_FacilityId;
                    caseComment.RegardingObjectId = incident.ToEntityReference();

                    caseComment.ipg_markedread = comment.ipg_markedread;
                    caseComment.ipg_assigntome = comment.ipg_assigntome;

                    service.Create(caseComment);
                }
            }

            tracingService.Trace("Updating Referral");
            service.Update(new ipg_referral()
            {
                Id = referralRef.Id,
                ipg_casestatusEnum = ipg_CaseStatus.Closed,
                ipg_AssociatedCaseId = incident.ToEntityReference()
            });


            if (carrier?.ipg_CarrierTypeEnum == ipg_CarrierType.Auto)
            {
                tracingService.Trace("Creating Auto Benefit");
                var benefit = new ipg_benefit()
                {
                    ipg_CaseId = incident.ToEntityReference(),
                    ipg_CarrierId = carrier.ToEntityReference(),
                    ipg_MemberID = referral.ipg_memberidnumber,
                    ipg_BenefitTypeEnum = ipg_BenefitType.Auto
                };
                service.Create(benefit);
            }
            tracingService.Trace("Creating Auth");
            if (!string.IsNullOrEmpty(referral.ipg_facility_auth) && referral.ipg_CarrierId != null)
            {
                service.Create(new ipg_authorization()
                {
                    ipg_carrierid = referral.ipg_CarrierId,
                    ipg_incidentid = incident.ToEntityReference(),
                    ipg_facilityauthnumber = referral.ipg_facility_auth,
                    ipg_procedurenameid = referral.ipg_ProcedureNameId

                });
            }

            tracingService.Trace("Creating Auth2");
            if (!string.IsNullOrEmpty(referral.ipg_facility_auth2) && referral.ipg_carriername2id != null)
            {
                service.Create(new ipg_authorization()
                {
                    ipg_carrierid = referral.ipg_carriername2id,
                    ipg_incidentid = incident.ToEntityReference(),
                    ipg_facilityauthnumber = referral.ipg_facility_auth2
                });
            }

            CopyRefferalNotesToCase(service, tracingService, referral.Id, incident.Id);
            CopyNotes(service, tracingService, referral, incident);
            MoveOpenTasks(service, tracingService, referral, incident);

            localContext.PluginExecutionContext.OutputParameters["Case"] = incident.ToEntityReference();
        }

        private void MoveOpenTasks(IOrganizationService service, ITracingService tracingService, ipg_referral referral, Incident incident)
        {
            var openTasksQuery = new QueryExpression
            {
                EntityName = Task.EntityLogicalName,
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Task.Fields.StateCode,ConditionOperator.Equal,(int)TaskState.Open),
                        new ConditionExpression(Task.Fields.RegardingObjectId,ConditionOperator.Equal,referral.Id)
                    }
                }
            };
            var openTasks = service.RetrieveMultiple(openTasksQuery)
                .Entities
                .Select(p => p.ToEntity<Task>());

            foreach (var iRefTask in openTasks)
            {
                service.Update(new Task()
                {
                    Id = iRefTask.Id,
                    RegardingObjectId = incident.ToEntityReference()
                });
            }
        }

        private DataCollection<Entity> RetrieveRelatedToReferralComments(Guid referralId)
        {
            var query = new QueryExpression
            {
                EntityName = adx_portalcomment.EntityLogicalName,
                ColumnSet = new ColumnSet(adx_portalcomment.Fields.PriorityCode, adx_portalcomment.Fields.adx_PortalCommentDirectionCode,
                                          adx_portalcomment.Fields.StatusCode, adx_portalcomment.Fields.OwnerId, adx_portalcomment.Fields.ipg_from,
                                          adx_portalcomment.Fields.From, adx_portalcomment.Fields.ipg_to,
                                          adx_portalcomment.Fields.To, adx_portalcomment.Fields.ipg_Type, adx_portalcomment.Fields.ipg_patientname,
                                          adx_portalcomment.Fields.ipg_facilitypatientid, adx_portalcomment.Fields.Subject, adx_portalcomment.Fields.Description,
                                          adx_portalcomment.Fields.ipg_owningportaluserid, adx_portalcomment.Fields.ipg_POID,
                                          adx_portalcomment.Fields.ipg_FacilityId, adx_portalcomment.Fields.RegardingObjectId,
                                          adx_portalcomment.Fields.ipg_markedread, adx_portalcomment.Fields.ipg_assigntome),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                      new ConditionExpression
                      {
                         AttributeName = adx_portalcomment.Fields.RegardingObjectId,
                         Operator = ConditionOperator.Equal,
                         Values = { referralId }
                      }
                    }
                }
            };
            return service.RetrieveMultiple(query).Entities;
        }

        private IEnumerable<ActivityParty> CreateActivityParties(IEnumerable<ActivityParty> activityParties)
        {
            List<ActivityParty> newActivityParties = new List<ActivityParty>();
            foreach (var activity in activityParties)
            {
                newActivityParties.Add(new ActivityParty { PartyId = activity.PartyId });
            }
            return (IEnumerable<ActivityParty>)newActivityParties;
        }

        private void CopyRefferalNotesToCase(IOrganizationService organizationService, ITracingService tracingService, Guid referralId, Guid incidentId)
        {
            tracingService.Trace("Retrieving referral notes to copy");
            var notes = organizationService.RetrieveMultiple(
                new QueryExpression
                {
                    EntityName = Annotation.EntityLogicalName,
                    ColumnSet = new ColumnSet(Annotation.Fields.Subject,
                                              Annotation.Fields.ObjectId,
                                              Annotation.Fields.NoteText),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                          new ConditionExpression (Annotation.Fields.ObjectId, ConditionOperator.Equal, referralId),
                          new ConditionExpression (Annotation.Fields.Subject, ConditionOperator.Equal, Constants.AnnotationSubjects.ReferralNote)
                        }
                    }
                }).Entities.Cast<Annotation>();

            tracingService.Trace("Creating Referral Notes for Case");
            foreach (var note in notes)
            {
                var caseNote = new Annotation
                {
                    ObjectId = new EntityReference(Incident.EntityLogicalName, incidentId),
                    Subject = note.Subject,
                    NoteText = note.NoteText
                };
                organizationService.Create(caseNote);
            }
        }

        private void CopyNotes(IOrganizationService organizationService, ITracingService tracingService, ipg_referral referral, Incident incident)
        {
            var noteIds = referral.GetNotesToCopyToCase();

            if (noteIds.Any())
            {
                tracingService.Trace("Retrieving notes to copy");
                var notes = organizationService.RetrieveMultiple(
                    new QueryExpression
                    {
                        EntityName = Annotation.EntityLogicalName,
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                            {
                          new ConditionExpression (Annotation.Fields.Id, ConditionOperator.In, noteIds.ToArray())
                            }
                        }
                    }).Entities.Cast<Annotation>();

                tracingService.Trace("Creating Case notes");
                foreach (var note in notes)
                {
                    var caseNote = new Annotation
                    {
                        ObjectId = new EntityReference(Incident.EntityLogicalName, incident.Id),
                        Subject = note.Subject,
                        NoteText = note.NoteText
                    };
                    organizationService.Create(caseNote);
                }
            }
        }

        private EntityReference[] GetClaimsMailingAddresses(EntityReference caseRef, EntityReference primaryCarrierRef, EntityReference secondaryCarrierRef, ITracingService tracingService)
        {
            var result = new EntityReference[2];
            var carrierClaimsMailingAddresses = GetCarrierClaimsMailingAddress(primaryCarrierRef);

            if (carrierClaimsMailingAddresses.Count == 1)
            {
                result[0] = carrierClaimsMailingAddresses[0].ToEntityReference();
            }
            var primaryCarrierSeveralMailingAddresses = carrierClaimsMailingAddresses.Count > 1;

            carrierClaimsMailingAddresses = GetCarrierClaimsMailingAddress(secondaryCarrierRef);
            if (carrierClaimsMailingAddresses.Count == 1)
            {
                result[1] = carrierClaimsMailingAddresses[0].ToEntityReference();
            }
            var secondaryCarrierSeveralMailingAddresses = carrierClaimsMailingAddresses.Count > 1;

            if (primaryCarrierSeveralMailingAddresses || secondaryCarrierSeveralMailingAddresses)
            {
                var carrierName = string.Empty;
                if (primaryCarrierSeveralMailingAddresses)
                {
                    carrierName = service.Retrieve(primaryCarrierRef.LogicalName, primaryCarrierRef.Id, new ColumnSet(Intake.Account.Fields.Name)).ToEntity<Intake.Account>().Name;
                }
                if (secondaryCarrierSeveralMailingAddresses)
                {
                    carrierName = (string.IsNullOrEmpty(carrierName) ? string.Empty : ", ") + service.Retrieve(secondaryCarrierRef.LogicalName, secondaryCarrierRef.Id, new ColumnSet(Intake.Account.Fields.Name)).ToEntity<Intake.Account>().Name;
                }
                var taskManager = new TaskManager(service, tracingService);
                var taskType = taskManager.GetTaskTypeById(TaskManager.TaskTypeIds.SELECT_THE_CLAIMS_MAILING_ADDRESS);
                var task = new Task()
                {
                    RegardingObjectId = caseRef,
                    ipg_caseid = caseRef,
                    ipg_tasktypeid = taskType.ToEntityReference(),
                    Description = string.Format(taskType.ipg_description, carrierName)
                };
                service.Create(task);
            }

            return result;
        }

        private List<Entity> GetCarrierClaimsMailingAddress(EntityReference carrierRef)
        {
            if (carrierRef != null)
            {
                var query = new QueryExpression(ipg_carrierclaimsmailingaddress.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(false),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(ipg_carrierclaimsmailingaddress.Fields.StateCode, ConditionOperator.Equal, (int)ipg_carrierclaimsmailingaddressState.Active),
                            new ConditionExpression(ipg_carrierclaimsmailingaddress.Fields.ipg_addresscarrierclaimmailingId, ConditionOperator.Equal, carrierRef.Id)
                        }
                    }
                };
                var ec = service.RetrieveMultiple(query);
                return ec.Entities.ToList<Entity>();
            }
            return new List<Entity>();
        }
    }
}