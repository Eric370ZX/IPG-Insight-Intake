using Insight.Intake.Data;
using Insight.Intake.Helpers;
using Insight.Intake.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using Insight.Intake.Plugin.Managers;
using Insight.Intake.Repositories;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.Plugin.Common.Benefits
{
    /// <summary>
    /// It is important that this class is public
    /// because the nested class EBVResponse can be deserialized 
    /// only if it is public and his parents too
    /// </summary>
    public class BenefitVerifier: IDisposable
    {
        private readonly string ZirmedReceiverId = "ZIRMED";
        private readonly string ZirmedTestPayerCode = "66666";

        private IPluginExecutionContext _pluginContext;
        private IOrganizationService _organizationService;
        private ITracingService _tracingService;
        private OrganizationServiceContext _context;
        private TaskManager _taskManager;
        private TeamRepository _teamRepository;

        //config
        private bool _isTestMode = false;
        private string _ipgProviderName = string.Empty;
        private string _ipgProviderNpi = string.Empty;
        private string _ipgProviderTaxId = string.Empty;
        private string _ipgZirmedSenderId = string.Empty;
        private string _ipgLogicAppUrl = string.Empty;
        private string _ipgTaskTypeName = string.Empty;

        private static readonly int MaxBvfBenefitDays = 30;
        private static readonly int MaxEbvBenefitDays = 3;


        internal BenefitVerifier(IPluginExecutionContext context, IOrganizationService organizationService, ITracingService tracingService)
        {
            _pluginContext = context;
            _organizationService = organizationService;
            _tracingService = tracingService;
            _context = new OrganizationServiceContext(organizationService);
            _taskManager = new TaskManager(organizationService, tracingService);
            _teamRepository = new TeamRepository(organizationService);
        }

        public bool Verify(EntityReference incidentReference, bool isUserGenerated, CarrierNumbers carrierNumber)
        {
            _tracingService.Trace("Reading config settings");
            bool.TryParse(GetGlobalSettingValueByKey(_organizationService, "EBVIsTestMode"), out _isTestMode);
            _ipgProviderName = GetGlobalSettingValueByKey(_organizationService, "EBVProviderName");
            _ipgProviderNpi = GetGlobalSettingValueByKey(_organizationService, "EBVProviderNPI");
            _ipgProviderTaxId = GetGlobalSettingValueByKey(_organizationService, "EBVProviderTaxId");
            _ipgZirmedSenderId = GetGlobalSettingValueByKey(_organizationService, "EBVZirmedSenderId");
            _ipgLogicAppUrl = GetGlobalSettingValueByKey(_organizationService, "EBVLogicAppURL");

            _tracingService.Trace($"Retrieving Case id {incidentReference.Id}");
            var incident = (Incident)_organizationService.Retrieve(incidentReference.LogicalName, incidentReference.Id, new ColumnSet(
                   Incident.Fields.ipg_EBVResult,
                   Incident.Fields.ipg_LastEBVCheckDateTime,
                   Incident.Fields.ipg_PatientFirstName,
                   Incident.Fields.ipg_PatientLastName,
                   Incident.Fields.ipg_PatientMiddleName,
                   Incident.Fields.ipg_PatientDateofBirth,
                   Incident.Fields.ipg_PatientGender,
                   Incident.Fields.ipg_ActualDOS,
                   Incident.Fields.ipg_SurgeryDate,
                   Incident.Fields.ipg_PatientId,
                   Incident.Fields.ipg_LastBenefitInvalidationDate,
                   Incident.Fields.ipg_lifecyclestepid,

                   //primary carrier
                   Incident.Fields.ipg_CarrierId,
                   Incident.Fields.ipg_MemberIdNumber,
                   Incident.Fields.ipg_RelationToInsured,
                   Incident.Fields.ipg_InsuredFirstName,
                   Incident.Fields.ipg_InsuredLastName,
                   Incident.Fields.ipg_InsuredMiddleName,
                   Incident.Fields.ipg_InsuredDateOfBirth,
                   Incident.Fields.ipg_InsuredGender,
                   
                   //secondary carrier
                   Incident.Fields.ipg_SecondaryCarrierId,
                   Incident.Fields.ipg_SecondaryMemberIdNumber,
                   Incident.Fields.ipg_SecondaryCarrierRelationToInsured,
                   Incident.Fields.ipg_secondaryinsuredfirstname,
                   Incident.Fields.ipg_secondaryinsuredlastname,
                   Incident.Fields.ipg_secondaryinsuredmiddlename,
                   Incident.Fields.ipg_secondaryinsureddateofbirth,
                   Incident.Fields.ipg_secondaryinsuredgender
                ));

            _tracingService.Trace($"Determine a carrier for EBV");
            EntityReference carrierReference;
            string memberId;
            switch (carrierNumber)
            {
                case CarrierNumbers.First:
                    carrierReference = incident.ipg_CarrierId;
                    memberId = incident.ipg_MemberIdNumber;
                    break;
                case CarrierNumbers.Second:
                    carrierReference = incident.ipg_SecondaryCarrierId;
                    memberId = incident.ipg_SecondaryMemberIdNumber;
                    break;
                default:
                    throw new Exception("Unexpected Carrier Number: " + carrierNumber);
            }
            _tracingService.Trace($"Determined EBV carrier: " + carrierReference.Name);

            if (carrierReference == null)
            {
                _tracingService.Trace("Carrier is not set");
                return false;
            }

            var carrier = _organizationService.Retrieve(Intake.Account.EntityLogicalName, carrierReference.Id,
                new ColumnSet(
                    Intake.Account.Fields.Name,
                    Intake.Account.Fields.ipg_ZirMedID,
                    Intake.Account.Fields.ipg_EnableEBVZirMed
                )).ToEntity<Intake.Account>();
            if (carrier == null)
            {
                _tracingService.Trace("Could not find the Carrier");
                return false;
            }

            _tracingService.Trace($"Case Validation");
            string caseValidationError = ValidateCase(incident, carrierNumber, carrier);
            if (string.IsNullOrWhiteSpace(caseValidationError) == false)
            {
                _tracingService.Trace($"Case Validation Errors: {caseValidationError}");
                return false;
            }

            if(isUserGenerated == false)
            {
                if (carrier.ipg_EnableEBVZirMed == true)
                {
                    _tracingService.Trace($"Requesting existing EBV benefits");
                    bool recentEbvBenefitExists = CheckExistingBenefits(incident.Id, carrier.Id, memberId, ipg_BenefitSources.EBV, MaxEbvBenefitDays);
                    if (recentEbvBenefitExists)
                    {
                        _tracingService.Trace($"EBV benefit within {MaxEbvBenefitDays} days for case with id {incident.Id} already exists");
                        return true;
                    }

                    return ExecuteEBVRequest(incident, isUserGenerated, carrierNumber, carrier, memberId);
                }
                else
                {
                    return ExecuteManualBenefitProcess(incident, carrier.Id, memberId, $"Carrier {carrier.Name} is not EBV-enabled");
                }
            }
            else
            {
                if(carrier.ipg_EnableEBVZirMed != true)
                {
                    //this exception should not happen because EBV button is disabled for EBV-disabled carriers
                    throw new Exception($"Carrier {carrier.Name} is not EBV enabled");
                }

                return ExecuteEBVRequest(incident, isUserGenerated, carrierNumber, carrier, memberId);
            }
        }

        private string ValidateCase(Incident incident, CarrierNumbers carrierNumber, Intake.Account carrier)
        {
            if (string.IsNullOrWhiteSpace(carrier.Name))
            {
                return "Carrier Name is required";
            }
            if (string.IsNullOrWhiteSpace(carrier.ipg_ZirMedID))
            {
                return "Carrier ZirMed ID is required";
            }

            if (incident.ipg_SurgeryDate.HasValue == false)
            {
                return "Case Surgery Date is required";
            }

            string memberId;
            new_RelationtoInsured? relationToInsured;
            string insuredFirstName, insuredLastName;
            DateTime? insuredDob;
            EntityReference carrierReference;
            switch (carrierNumber)
            {
                case CarrierNumbers.First:
                    memberId = incident.ipg_MemberIdNumber;
                    relationToInsured = incident.ipg_RelationToInsuredEnum;
                    insuredFirstName = incident.ipg_InsuredFirstName;
                    insuredLastName = incident.ipg_InsuredLastName;
                    insuredDob = incident.ipg_InsuredDateOfBirth;
                    carrierReference = incident.ipg_CarrierId;
                    break;
                case CarrierNumbers.Second:
                    memberId = incident.ipg_SecondaryMemberIdNumber;
                    relationToInsured = incident.ipg_SecondaryCarrierRelationToInsuredEnum;
                    insuredFirstName = incident.ipg_secondaryinsuredfirstname;
                    insuredLastName = incident.ipg_secondaryinsuredlastname;
                    insuredDob = incident.ipg_secondaryinsureddateofbirth;
                    carrierReference = incident.ipg_SecondaryCarrierId;
                    break;
                default:
                    throw new NotSupportedException("Unexpected carrier number: " + carrierNumber);
            }

            if (string.IsNullOrWhiteSpace(memberId))
            {
                return "Member ID is required";
            }

            if (relationToInsured != new_RelationtoInsured.Self)
            {
                if (string.IsNullOrWhiteSpace(insuredFirstName))
                {
                    return "Insured First Name is required";
                }
                if (string.IsNullOrWhiteSpace(insuredLastName))
                {
                    return "Insured Last Name is required";
                }
                if (insuredDob.HasValue == false)
                {
                    return "Insured Date of Birth is required";
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(incident.ipg_PatientFirstName))
                {
                    return "Patient First Name is required";
                }
                if (string.IsNullOrWhiteSpace(incident.ipg_PatientLastName))
                {
                    return "Patient Last Name is required";
                }
                if (incident.ipg_PatientDateofBirth.HasValue == false)
                {
                    return "Patient Date of Birth is required";
                }
            }

            return null;
        }

        private bool CheckExistingBenefits(Guid incidentId, Guid carrierId, string memberId, ipg_BenefitSources benefitSource, int maxDaysAgo)
        {
            return _organizationService.RetrieveMultiple(new QueryExpression(ipg_benefit.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                            {
                                new ConditionExpression(ipg_benefit.Fields.ipg_BenefitSource, ConditionOperator.Equal, (int)benefitSource),
                                new ConditionExpression(ipg_benefit.Fields.ipg_CaseId, ConditionOperator.Equal, incidentId),
                                new ConditionExpression(ipg_benefit.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierId),
                                new ConditionExpression(ipg_benefit.Fields.ipg_MemberID, ConditionOperator.Equal, memberId),
                                new ConditionExpression(ipg_benefit.Fields.CreatedOn, ConditionOperator.OnOrAfter,  DateTime.Today.AddDays(-maxDaysAgo)),
                                new ConditionExpression(ipg_benefit.Fields.StateCode, ConditionOperator.Equal, (int)ipg_benefitState.Active)
                            }
                }
            }).Entities.Any();
        }

        private bool ExecuteEBVRequest(Incident incident, bool isUserGenerated, CarrierNumbers carrierNumber, Intake.Account carrier, string memberId)
        {
            var incidentReference = incident.ToEntityReference();

            EligibilityInquiry newEligibilityInquiry = GenerateEligibilityInquiry(incident, isUserGenerated, carrierNumber, carrier, memberId);

            string xml = SerializeObjectAsXML(newEligibilityInquiry);

            _tracingService.Trace($"Sending request to {_ipgLogicAppUrl}, EBVIsTestMode: {_isTestMode}, _ipgProviderName: {_ipgProviderName}, _ipgProviderNpi: {_ipgProviderNpi}," +
                $"_ipgProviderTaxId: {_ipgProviderTaxId}, _ipgZirmedSenderId: {_ipgZirmedSenderId}");
            var task = System.Threading.Tasks.Task.Run(() =>
            {
                return SendRequest(_ipgLogicAppUrl, xml);
            });
            bool isEbvRequestCompletedSuccessfully = task.Wait(TimeSpan.FromMinutes(1));
            _tracingService.Trace($"EBV executed for CaseId: {incidentReference.Id.ToString()}, Is completed successfully: {isEbvRequestCompletedSuccessfully.ToString()}, InquiryId: {task.Result.inquiryId}, ResponseId: {task.Result.responseId}, XML: {xml}");

            if (isEbvRequestCompletedSuccessfully)
            {
                _tracingService.Trace("Updating Case fields after EBV request");
                var attributesForUpdate = new Dictionary<string, object>() {
                            { nameof(Incident.ipg_LastEBVCheckDateTime).ToLower(),  DateTime.UtcNow },
                            { nameof(Incident.ipg_benefitdatasourcecode).ToLower(), new OptionSetValue((int)Incident_ipg_benefitdatasourcecode.EBV) },
                            { nameof(Incident.ipg_benefitsupdatedby).ToLower(), new EntityReference(SystemUser.EntityLogicalName, _pluginContext.InitiatingUserId) }
                        };
                UpdateRecordField(_organizationService, incidentReference.LogicalName, incidentReference.Id, attributesForUpdate);

                _tracingService.Trace("Invoking EBV callback"); //todo: replace the callback workflow with a plugin
                var callBackAction = new OrganizationRequest("ipg_IPGCaseActionsEBVCallback");
                callBackAction.Parameters.Add("Target", new EntityReference(incidentReference.LogicalName, incidentReference.Id));
                callBackAction.Parameters.Add("EBVInquiryId", new EntityReference("ipg_ebvinquiry", task.Result.inquiryId));
                callBackAction.Parameters.Add("EBVResponseId", new EntityReference("ipg_ebvresponse", task.Result.responseId));
                OrganizationResponse response = (OrganizationResponse)_organizationService.Execute(callBackAction);

                _tracingService.Trace("Updating EBV documents");
                CreateNewAndDeactivatePreviousEBVDocuments(incidentReference, task.Result.responseId);

                _tracingService.Trace("Validating benefits data");
                ValidateBenefitsData(task.Result.responseId, incidentReference, carrierNumber, ref memberId);

                var benefitImporter = new EbvBenefitImporter(_organizationService, _tracingService);
                benefitImporter.ImportEBVBenefitsToD365(incident.Id, carrierNumber, task.Result.responseId, _pluginContext.UserId);

                var caseBenefitSwitcher = new CaseBenefitSwitcher(_organizationService, _tracingService);
                caseBenefitSwitcher.SetDMEBenefitTypeIfNeeded(incident.Id, carrierNumber);
                caseBenefitSwitcher.UpdateInOutNetwork(incident.Id, carrier.Id);

                _tracingService.Trace("Retrieving EBV result");
                var incidentWithEbvResult = (Incident)_organizationService.Retrieve(incidentReference.LogicalName, incidentReference.Id, new ColumnSet(nameof(Incident.ipg_EBVResult).ToLower()));
                if (incidentWithEbvResult.ipg_EBVResult != null)
                {
                    if(incidentWithEbvResult.ipg_EBVResultEnum == ipg_EBVResults.SERVER_DOWN
                        || incidentWithEbvResult.ipg_EBVResultEnum == ipg_EBVResults.FAILED)
                    {
                        var ebvResponse = _organizationService.Retrieve(ipg_EBVResponse.EntityLogicalName, task.Result.responseId, new ColumnSet(ipg_EBVResponse.Fields.ipg_name))
                            .ToEntity<ipg_EBVResponse>();

                        ExecuteManualBenefitProcess(incident, carrier.Id, memberId, "EBV is enabled but failed for the following reason(s). Please initiate Manual Benefits and save to the Case." + Environment.NewLine
                                + "1. " + ebvResponse.ipg_name);
                    }
                    else
                    {
                        CloseManualBenefitsVerificationTasks(incident, carrier);
                    }

                    if(incidentWithEbvResult.ipg_EBVResultEnum == ipg_EBVResults.INACTIVE)
                    {
                        var taskTemplate = new Task()
                        {
                            ipg_carrierid = new EntityReference(Intake.Account.EntityLogicalName, carrier.Id)
                        };

                        if(incident.ipg_lifecyclestepid?.Id != LifecycleStepsConstants.AuthorizationGate3)
                        {
                            var caseManagersTeam = _teamRepository.GetByName(Constants.TeamNames.CaseManagement, new ColumnSet(Team.Fields.Id));
                            if(caseManagersTeam == null)
                            {
                                throw new Exception($"Could not find '{Constants.TeamNames.CaseManagement}' team");
                            }

                            taskTemplate.ipg_assignedtoteamid = new EntityReference(Team.EntityLogicalName, caseManagersTeam.Id);
                        }

                        _tracingService.Trace("Creating Inactive benefits task");
                        _taskManager.CreateTask(incidentReference, TaskTypeIds.INACTIVE_BENEFITS_TASK_TYPE_ID, taskTemplate);
                    }
                    
                    if (incidentWithEbvResult.ipg_EBVResultEnum == ipg_EBVResults.ELIGIBLE)
                    {
                        return true; //benefits have been verified
                    }
                }
            }
            else
            {
                ExecuteManualBenefitProcess(incident, carrier.Id, memberId, "EBV failed");
            }

            return false;
        }

        private EligibilityInquiry GenerateEligibilityInquiry(Incident incident, bool isUserGenerated, CarrierNumbers carrierNumber, Intake.Account carrier, string memberId)
        {
            _tracingService.Trace("Getting a new EBV Request ID");
            var newEbvInquiryId = _organizationService.Create(new ipg_EBVInquiryAutoNumber());
            var newEbvInquiry = _organizationService.Retrieve(ipg_EBVInquiryAutoNumber.EntityLogicalName, newEbvInquiryId, new ColumnSet(true)).ToEntity<ipg_EBVInquiryAutoNumber>();
            int requestId = int.Parse(newEbvInquiry.ipg_AutoNumber);

            new_RelationtoInsured? relationToInsured;
            string insuredFirstName, insuredLastName, insuredMiddleName;
            DateTime? insuredDob;
            switch (carrierNumber)
            {
                case CarrierNumbers.First:
                    relationToInsured = incident.ipg_RelationToInsuredEnum;
                    insuredFirstName = incident.ipg_InsuredFirstName;
                    insuredLastName = incident.ipg_InsuredLastName;
                    insuredMiddleName = incident.ipg_InsuredMiddleName;
                    insuredDob = incident.ipg_InsuredDateOfBirth;
                    break;
                case CarrierNumbers.Second:
                    relationToInsured = incident.ipg_SecondaryCarrierRelationToInsuredEnum;
                    insuredFirstName = incident.ipg_secondaryinsuredfirstname;
                    insuredLastName = incident.ipg_secondaryinsuredlastname;
                    insuredMiddleName = incident.ipg_secondaryinsuredmiddlename;
                    insuredDob = incident.ipg_secondaryinsureddateofbirth;
                    break;
                default:
                    throw new NotSupportedException("Unexpected carrier number: " + carrierNumber);
            }

            _tracingService.Trace("Creating EligibilityInquiry");
            var newEligibilityInquiry = new EligibilityInquiry
            {
                id = Guid.NewGuid().ToString(),
                RequestId = requestId,
                RequestedBy = isUserGenerated ? _pluginContext.InitiatingUserId.ToString() : _pluginContext.UserId.ToString(),
                CaseId = incident.Id.ToString(),
                CarrierId = carrier.Id.ToString(),
                PatientId = incident.ipg_PatientId?.Id.ToString(),
                IsUserGenerated = isUserGenerated,
                CreatedAt = DateTime.UtcNow,
                Status = EligibilityInquiry.Statuses.INPROCESS,

                SenderId = _ipgZirmedSenderId,
                ReceiverId = ZirmedReceiverId,

                PayerCode = _isTestMode ? ZirmedTestPayerCode : carrier.ipg_ZirMedID,
                PayerName = _isTestMode ? ZirmedTestPayerCode : carrier.Name,

                ProviderName = _ipgProviderName,
                ProviderNpi = _ipgProviderNpi,
                ProviderTaxId = _ipgProviderTaxId,

                MemberId = memberId, //we send only primary MemberId
                PolicyNumber = null, //[SB]: Policy ID and Insured Id should be the same field. For now, you can use member id number in D365

                RelationToInsured = ConvertRelationToInsuredTo270X12Format(relationToInsured),

                SubscriberFirstName = relationToInsured != new_RelationtoInsured.Self
                                        ? insuredFirstName
                                        : incident.ipg_PatientFirstName,
                SubscriberLastName = relationToInsured != new_RelationtoInsured.Self
                                        ? insuredLastName
                                        : incident.ipg_PatientLastName,
                SubscriberMiddleName = relationToInsured != new_RelationtoInsured.Self
                                        ? insuredMiddleName
                                        : incident.ipg_PatientMiddleName,
                SubscriberDOB = relationToInsured != new_RelationtoInsured.Self
                                        ? insuredDob
                                        : incident.ipg_PatientDateofBirth,
                SubscriberGender = relationToInsured != new_RelationtoInsured.Self
                                        ? ConvertGenderTo270X12Format((Genders?)incident.ipg_InsuredGender?.Value)
                                        : ConvertGenderTo270X12Format((Genders?)incident.ipg_PatientGender?.Value),

                DOS = incident.ipg_ActualDOS ?? incident.ipg_SurgeryDate
            };

            if (relationToInsured != new_RelationtoInsured.Self)
            {
                _tracingService.Trace("Setting EligibilityInquiry dependent properties");
                newEligibilityInquiry.DependentFirstName = incident.ipg_PatientFirstName;
                newEligibilityInquiry.DependentLastName = incident.ipg_PatientLastName;
                newEligibilityInquiry.DependentMiddleName = incident.ipg_PatientMiddleName;
                newEligibilityInquiry.DependentDOB = incident.ipg_PatientDateofBirth;
                newEligibilityInquiry.DependentGender = ConvertGenderTo270X12Format((Genders?)incident.ipg_PatientGender?.Value);
            }

            return newEligibilityInquiry;
        }

        private string SerializeObjectAsXML(EligibilityInquiry eligibilityInquiry)
        {
            var serializer = new XmlSerializer(typeof(EligibilityInquiry));
            using (StringWriter writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, eligibilityInquiry);
                return writer.ToString();
            }
        }

        private EBVResponse SendRequest(string url, string xml)
        {
            EBVResponse ebvResponse = new EBVResponse();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(xml);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            HttpWebResponse responsee = (HttpWebResponse)request.GetResponse();
            if (responsee.StatusCode == HttpStatusCode.OK || responsee.StatusCode == HttpStatusCode.Accepted)
            {
                Stream responseStream = responsee.GetResponseStream();

                using (MemoryStream DeSerializememoryStream = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(EBVResponse));
                    StreamWriter writer = new StreamWriter(DeSerializememoryStream);
                    writer.Write(new StreamReader(responseStream).ReadToEnd());
                    writer.Flush();

                    DeSerializememoryStream.Position = 0;
                    ebvResponse = (EBVResponse)serializer.ReadObject(DeSerializememoryStream);
                }
            }

            return ebvResponse;
        }

        private string ConvertRelationToInsuredTo270X12Format(new_RelationtoInsured? relationToInsured)
        {
            if (relationToInsured.HasValue)
            {
                switch (relationToInsured)
                {
                    case new_RelationtoInsured.Self:
                        return "S";

                    case new_RelationtoInsured.Child:
                        return "19";

                    case new_RelationtoInsured.Spouse:
                        return "01";

                    case new_RelationtoInsured.Other: //other adult
                        return "34";

                    default:
                        return "D"; //unspecified dependent
                }
            }


            return null;
        }

        private string ConvertGenderTo270X12Format(Genders? gender)
        {
            if (gender.HasValue)
            {
                switch (gender)
                {
                    case Genders.Male:
                        return "M";

                    case Genders.Female:
                        return "F";
                }
            }

            return null;
        }

        private void UpdateRecordField(IOrganizationService service, string entityLogicalName, Guid Id, Dictionary<string, object> attributes)
        {
            Entity entityToUpdate = new Entity(entityLogicalName);
            entityToUpdate.Id = Id;
            foreach (var attribute in attributes)
            {
                entityToUpdate[attribute.Key] = attribute.Value;
            }

            service.Update(entityToUpdate);
        }

        private Guid CreateNewAndDeactivatePreviousEBVDocuments(EntityReference caseReference, Guid ebvResponseId)
        {
            var ebvDocumentType = _context.CreateQuery<ipg_documenttype>().FirstOrDefault(dt => dt.ipg_DocumentTypeAbbreviation == Constants.DocumentTypeAbbreviations.EbvDocumentType);
            if (ebvDocumentType == null)
            {
                throw new Exception($"Could not find {Constants.DocumentTypeAbbreviations.EbvDocumentType} document type");
            }

            //deactivate existing active EBV documents
            var existingEbvDocuments = from doc in _context.CreateQuery<ipg_document>()
                                       where doc.ipg_CaseId.Id == caseReference.Id
                                            && doc.ipg_DocumentTypeId.Id == ebvDocumentType.Id
                                            && doc.StateCode == ipg_documentState.Active
                                       select doc;
            foreach (var doc in existingEbvDocuments)
            {
                var setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = doc.ToEntityReference(),
                    State = new OptionSetValue((int)ipg_documentState.Inactive),
                    Status = new OptionSetValue((int)ipg_document_StatusCode.Inactive)
                };
                _organizationService.Execute(setStateRequest);
            }

            //create a new EBV doc
            return this._organizationService.Create(new ipg_document
            {
                ipg_DocumentTypeId = ebvDocumentType.ToEntityReference(),
                ipg_CaseId = caseReference,
                ipg_EBVResponseId = new EntityReference(ipg_EBVResponse.EntityLogicalName, ebvResponseId)
            });
        }

        private void ValidateBenefitsData(Guid responseId, EntityReference incidentReference, CarrierNumbers carrierNumber, ref string memberId)
        {
            var ebvResponse = _organizationService.Retrieve(ipg_EBVResponse.EntityLogicalName, responseId, new ColumnSet(true))
                .ToEntity<ipg_EBVResponse>();
            var sourceIncident = _organizationService.Retrieve(Incident.EntityLogicalName, incidentReference.Id, new ColumnSet(
                nameof(Incident.OwnerId).ToLower().ToLower()))
                .ToEntity<Incident>();

            ipg_EBVSubscriber subscriber = null;
            ipg_EBVSubscriber dependant = null;
            if (ebvResponse.ipg_SubscriberId != null)
            {
                subscriber = _organizationService.Retrieve(ipg_EBVSubscriber.EntityLogicalName, ebvResponse.ipg_SubscriberId.Id, new ColumnSet(true))
                    .ToEntity<ipg_EBVSubscriber>();
            }
            if (subscriber == null)
            {
                return;
            }
                        
            if (ebvResponse.ipg_DependentId != null)
            {
                dependant = _organizationService.Retrieve(ipg_EBVSubscriber.EntityLogicalName, ebvResponse.ipg_DependentId.Id, new ColumnSet(true))
                    .ToEntity<ipg_EBVSubscriber>();
            }
            var ebvPatient = dependant != null ? dependant : subscriber;
            var casePatient = GetPatientByIncident(incidentReference.Id);
            
            if (ebvPatient.ipg_DOB?.Date != casePatient?.BirthDate?.Date)
            {
                var subject = "Update Patient Demographics based on EBV";
                var description = $@"Patient DOB from EBV request differs from patient DOB on the case. 
                 \nCase patient DOB is { casePatient?.BirthDate}. EBV patient DOB is {ebvPatient.ipg_DOB}";
                CreateUserTask(incidentReference, sourceIncident.OwnerId, subject, description);
            }
            if (ebvPatient.ipg_AddressLine1?.ToLower() != casePatient?.Address1_Line1?.ToLower())
            {
                var subject = "Update Patient Demographics based on EBV";
                var description = $@"Patient address line 1 from EBV request differs from patient address line 1 on the case. 
                 \nCase patient address line 1 is { casePatient?.Address1_Line1}. EBV patient address line 1 is {ebvPatient.ipg_AddressLine1}";
                CreateUserTask(incidentReference, sourceIncident.OwnerId, subject, description);
            }
            if(string.IsNullOrWhiteSpace(subscriber.ipg_MemberId) == false
                && subscriber.ipg_MemberId != sourceIncident.ipg_MemberIdNumber)
            {
                var incidentUpdate = new Incident {Id = incidentReference.Id };
                memberId = subscriber.ipg_MemberId;

                if(!string.IsNullOrEmpty(subscriber.ipg_PlanSponsor)){
                    incidentUpdate.ipg_PlanSponsor = subscriber.ipg_PlanSponsor;
                }
                
                switch (carrierNumber)
                {
                    case CarrierNumbers.First:
                        incidentUpdate.ipg_MemberIdNumber = memberId;
                        break;
                    case CarrierNumbers.Second:
                        incidentUpdate.ipg_SecondaryMemberIdNumber = memberId;
                        break;
                    default:
                        throw new Exception("Unexpected carrier number: " + carrierNumber);
                }
                _organizationService.Update(incidentUpdate);

                var annotation = new Annotation()
                {
                    Subject = "EBV Update",
                    NoteText = "Member ID Updated Based on EBV",
                    ObjectId = incidentReference
                };
                _organizationService.Create(annotation);
            }
        }

        private void CreateUserTask(EntityReference caseRef, EntityReference ownerId, string subject, string description, ipg_TaskType1? taskType = null, EntityReference taskTypeRef = null)
        {
            if (HasUserTask(subject, caseRef))
            {
                return;
            }
            var task = new Task()
            {
                RegardingObjectId = caseRef,
                Subject = subject,
                Description = description,
                OwnerId = ownerId,
                ipg_tasktypecodeEnum = taskType,
                ipg_tasktypeid = taskTypeRef
            };
            _organizationService.Create(task);
        }

        public bool HasUserTask(string subject, EntityReference caseRef)
        {
            var query = new QueryExpression("task");
            query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, caseRef.Id);
            query.Criteria.AddCondition("subject", ConditionOperator.Equal, subject);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)TaskState.Open);
            var tasks = _organizationService.RetrieveMultiple(query);
            return tasks.Entities.Any();
        }

        private void CreateManualEBVRequiredTask(EntityReference caseRef, string reason = "")
        {
            var taskType = _taskManager.GetTaskTypeById(TaskTypeIds.EBV_FAILED_TASK_TYPE_ID);

            var task = new Task()
            {
                RegardingObjectId = caseRef,
                Description = string.Format(taskType.ipg_description, reason),
                ipg_tasktypeid = taskType.ToEntityReference()
            };
            _organizationService.Create(task);
        }

        private Intake.Contact GetPatientByIncident(Guid incidentId)
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='contact'>
                            <attribute name='fullname' />
                            <attribute name='telephone1' />
                            <attribute name='contactid' />
                            <attribute name='lastname' />
                            <attribute name='firstname' />
                            <attribute name='birthdate' />
                            <order attribute='fullname' descending='false' />
                            <link-entity name='incident' from='ipg_patientid' to='contactid' link-type='inner' alias='ab'>
                              <filter type='and'>
                                <condition attribute='incidentid' operator='eq' value='{incidentId}' />
                              </filter>
                            </link-entity>
                          </entity>
                        </fetch>";
            var patients = _organizationService.RetrieveMultiple(new FetchExpression(fetch)).Entities;
            if (patients.Count == 0)
            {
                return null;
            }
            return patients.FirstOrDefault().ToEntity<Intake.Contact>();
        }

        private string GetGlobalSettingValueByKey(IOrganizationService service, string key)
        {
            QueryByAttribute query = new QueryByAttribute(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_globalsetting.ipg_value).ToLower())
            };
            query.AddAttributeValue(nameof(ipg_globalsetting.ipg_name).ToLower(), key);

            EntityCollection coll = service.RetrieveMultiple(query);

            if (coll.Entities.Count == 1)
                return coll.Entities[0].GetAttributeValue<string>(nameof(ipg_globalsetting.ipg_value).ToLower());

            return string.Empty;
        }

        private void CloseManualBenefitsVerificationTasks(Incident incident, Intake.Account carrier)
        {
            _tracingService.Trace("Start closing Manual Benefits Verification tasks");

            _tracingService.Trace("Creating Context");
            using (CrmServiceContext context = new CrmServiceContext(_organizationService))
            {
                _tracingService.Trace("Retrieving open tasks");
                var openTasks = (from task in context.TaskSet
                                 join taskType in context.ipg_tasktypeSet on task.ipg_tasktypeid.Id equals taskType.Id
                                 where taskType.ipg_typeid == (int)TaskTypeIds.EBV_FAILED_TASK_TYPE_ID
                                     && task.RegardingObjectId != null && task.RegardingObjectId.Id == incident.Id
                                     && task.ipg_carrierid != null && task.ipg_carrierid.Id == carrier.Id //memberId is not needed 
                                     && task.StateCode == TaskState.Open
                                 select task).ToList();

                _tracingService.Trace($"Completing {openTasks.Count} tasks");
                foreach (var openTask in openTasks)
                {
                    openTask.StateCode = TaskState.Completed;
                    openTask.StatusCodeEnum = Task_StatusCode.Resolved;
                    context.UpdateObject(openTask);
                }

                if (openTasks.Any())
                {
                    _tracingService.Trace("Saving context changes");
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Executes Manual BVF
        /// </summary>
        /// <param name="incident"></param>
        /// <param name="description"></param>
        /// <returns>If we already have benefit within 30 days returns true</returns>
        private bool ExecuteManualBenefitProcess(Incident incident, Guid carrierId, string memberId, string description = "Manual Benefits Verification Required")
        {
            if (IsBvfBenefitAttachedToTheCase(incident.Id, carrierId, memberId))
            {
                return true;
            }

            CreateManualEBVRequiredTask(incident.ToEntityReference(), description);

            return false;
        }

        private bool IsBvfBenefitAttachedToTheCase(Guid incidentId, Guid carrierId, string memberId)
        {
            _tracingService.Trace($"Requesting existing BVF benefits");
            bool recentBvfBenefitExists = CheckExistingBenefits(incidentId, carrierId, memberId, ipg_BenefitSources.BVF, MaxBvfBenefitDays);
            if (recentBvfBenefitExists)
            {
                _tracingService.Trace($"BVF benefit within {MaxBvfBenefitDays} days for case with id {incidentId} already exists");
            }

            return recentBvfBenefitExists;
        }

        public void Dispose()
        {
            if (this._context != null)
            {
                this._context.Dispose();
            }
        }

        public void CreateTaskManualBenefitVerificationRequired(EntityReference incidentReference, CarrierNumbers carrierNumber)
        {
            var incident = (Incident)_organizationService.Retrieve(incidentReference.LogicalName, incidentReference.Id, new ColumnSet(
                   Incident.Fields.ipg_CarrierId,
                   Incident.Fields.ipg_SecondaryCarrierId
                ));
            EntityReference carrierReference;
            switch (carrierNumber)
            {
                case CarrierNumbers.First:
                    carrierReference = incident.ipg_CarrierId;
                    break;
                case CarrierNumbers.Second:
                    carrierReference = incident.ipg_SecondaryCarrierId;
                    break;
                default:
                    throw new Exception("Unexpected Carrier Number: " + carrierNumber);
            }
            var carrier = _organizationService.Retrieve(Intake.Account.EntityLogicalName, carrierReference.Id, new ColumnSet(
                Intake.Account.Fields.Name,
                Intake.Account.Fields.ipg_EnableEBVZirMed
            )).ToEntity<Intake.Account>();
            if (carrier == null)
            {
                _tracingService.Trace("Could not find the Carrier");
                return;
            }
            if (carrier.ipg_EnableEBVZirMed != true)
            {
                var taskType = _taskManager.GetTaskTypeById(TaskTypeIds.MANUAL_BENEFIT_VERIFICATION_REQUIRED);
                var template = new Task() { Description = string.Format(taskType.ipg_description, carrier.Name) };
                _taskManager.CreateTask(incidentReference, taskType.ToEntityReference(), template);
            }
        }

        /// <summary>
        /// It is important that this class is public
        /// because it can be deserialized 
        /// only if it is public and his parents too
        /// </summary>
        [DataContract]
        public class EBVResponse
        {
            [DataMember]
            public Guid inquiryId { get; set; }
            [DataMember]
            public Guid responseId { get; set; }
        }
    }
}