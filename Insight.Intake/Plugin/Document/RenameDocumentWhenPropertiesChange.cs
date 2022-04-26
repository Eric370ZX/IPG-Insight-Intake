using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Document
{
    public class RenameDocumentWhenPropertiesChange : PluginBase
    {
        const string dateFormat = "MMddyyy";
        const string priceListManufacturerDocumentTypeId = "0b77781a-106b-e911-a983-000d3a37062b";

        public RenameDocumentWhenPropertiesChange() : base(typeof(RenameDocumentWhenPropertiesChange))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_document.EntityLogicalName, PreOperationCreateOrUpdateHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_document.EntityLogicalName, PreOperationCreateOrUpdateHandler);
        }

        private void PreOperationCreateOrUpdateHandler(LocalPluginContext localPluginContext)
        {

            ChangeFileNameBasedOnSourceOnPreCreate(localPluginContext);

            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            tracingService.Trace($"{typeof(RenameDocumentWhenPropertiesChange)} plugin started");

            tracingService.Trace("Getting Target input parameter (Document entity)");

            var document = localPluginContext.Target<ipg_document>();

            tracingService.Trace($"Contains caseid in attributes: {document.Attributes.Contains(ipg_document.Fields.ipg_CaseId)}");

            if (context.MessageName == MessageNames.Create)
            {
                DetermineRightRelationShip(document, service, tracingService);
                GetRevision(document, service, tracingService);
            }

            tracingService.Trace("Check if the doc attributes contain required fields to generate DocumentName");
            if (document.Attributes.Contains(ipg_document.Fields.ipg_CaseId)
                || document.Attributes.Contains(ipg_document.Fields.ipg_DocumentTypeId)
                || document.Attributes.Contains(ipg_document.Fields.ipg_Revision)
                || document.Attributes.Contains(ipg_document.Fields.ipg_ReferralId)
                || document.Attributes.Contains(ipg_document.Fields.ipg_carrierid)
                || document.Attributes.Contains(ipg_document.Fields.ipg_FacilityId)
                || document.Attributes.Contains(ipg_document.Fields.ipg_documenttypecategoryid))
            {
                EntityReference caseId = document.ipg_CaseId;
                EntityReference docTypeId = document.ipg_DocumentTypeId;
                int? revision = document.ipg_Revision;

                EntityReference referralId = document.ipg_ReferralId;
                EntityReference carrierId = document.ipg_carrierid;
                EntityReference facilityId = document.ipg_FacilityId;
                EntityReference manufacturerId = document.ipg_ipg_manufacturerid;
                EntityReference documentTypeCategoryId = document.ipg_documenttypecategoryid;
                DateTime? createdOn = document.CreatedOn;



                if (localPluginContext.PluginExecutionContext.MessageName == MessageNames.Update)
                {
                    ipg_document docEntityFromServer = GetDocument(service, document.Id, tracingService);

                    caseId = document.Attributes.Contains(ipg_document.Fields.ipg_CaseId) ? caseId : docEntityFromServer.ipg_CaseId;
                    docTypeId = document.Attributes.Contains(ipg_document.Fields.ipg_DocumentTypeId) ? docTypeId : docEntityFromServer.ipg_DocumentTypeId;
                    revision = document.Attributes.Contains(ipg_document.Fields.ipg_Revision) ? revision : docEntityFromServer.ipg_Revision;

                    referralId = document.Attributes.Contains(ipg_document.Fields.ipg_ReferralId) ? referralId : docEntityFromServer.ipg_ReferralId;
                    carrierId = document.Attributes.Contains(ipg_document.Fields.ipg_carrierid) ? carrierId : docEntityFromServer.ipg_carrierid;
                    facilityId = document.Attributes.Contains(ipg_document.Fields.ipg_FacilityId) ? facilityId : docEntityFromServer.ipg_FacilityId;
                    manufacturerId = document.Attributes.Contains(ipg_document.Fields.ipg_ipg_manufacturerid) ? manufacturerId : docEntityFromServer.ipg_ipg_manufacturerid;
                    documentTypeCategoryId = document.Attributes.Contains(ipg_document.Fields.ipg_documenttypecategoryid) ? documentTypeCategoryId : docEntityFromServer.ipg_documenttypecategoryid;

                    createdOn = docEntityFromServer.CreatedOn;
                }
                else
                {
                    FormNameOfManufacturerDocument(localPluginContext);
                }

                createdOn = createdOn ?? new DateTime();
                tracingService.Trace("Checking if Case attached");

                if (docTypeId != null && (caseId != null || referralId != null || carrierId != null || facilityId != null))
                {
                    tracingService.Trace("Retrieving Doc Type with id = " + docTypeId.Id);
                    var docType = service.Retrieve(ipg_documenttype.EntityLogicalName,
                                                   docTypeId.Id,
                                                   new ColumnSet(ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation
                                                   , ipg_documenttype.Fields.ipg_DocumentCategoryTypeId))
                                                   .ToEntity<ipg_documenttype>();

                    //Fill documentTypeCategory on the Document from ipg_documenttype if null 
                    if (documentTypeCategoryId == null && docType.ipg_DocumentCategoryTypeId != null)
                    {
                        tracingService.Trace("Doc Category Type is empty will be filled by Document Category from Doc Type");
                        document.ipg_documenttypecategoryid = docType.ipg_DocumentCategoryTypeId;
                    }

                    if (!string.IsNullOrEmpty(docType?.ipg_DocumentTypeAbbreviation))
                    {

                        documentTypeCategoryId = documentTypeCategoryId ?? docType.ipg_DocumentCategoryTypeId;

                        var docTypeCategoryName = documentTypeCategoryId == null ? "" : documentTypeCategoryId.Name
                            ?? service.Retrieve(documentTypeCategoryId.LogicalName, documentTypeCategoryId.Id, new ColumnSet(ipg_documenttype.Fields.ipg_name)).GetAttributeValue<string>(ipg_documenttype.Fields.ipg_name);

                        tracingService.Trace($"docTypeCategoryName - {docTypeCategoryName}");

                        string preFix = "";
                        string createdOnString = createdOn.Value.ToString(dateFormat);
                        bool useRevision = true;

                        tracingService.Trace($"createdOnString - {createdOnString}");

                        if (carrierId != null || docTypeCategoryName == "Carrier")
                        {
                            tracingService.Trace($"Try Get Prefix for Carrier");
                            if (carrierId == null)
                            {
                                tracingService.Trace($"Carrier is empty!");
                            }
                            else
                            {
                                tracingService.Trace($"Get Prefix Fo Carrier {carrierId.Id}");
                                preFix = GetAccountName(service, carrierId);
                            }
                        }
                        else if (facilityId != null && docTypeCategoryName == "Facility")
                        {
                            tracingService.Trace($"Try Get Prefix for Facility");
                            if (facilityId == null)
                            {
                                tracingService.Trace($"Facility is empty!");
                            }
                            else
                            {
                                tracingService.Trace($"Get Prefix Fo Facility {facilityId.Id}");
                                preFix = GetAccountName(service, facilityId);
                            }
                        }
                        else if (manufacturerId != null || docTypeCategoryName == "Manufacturer")
                        {
                            tracingService.Trace($"Try Get Prefix for Manufacturer");
                            if (manufacturerId == null)
                            {
                                tracingService.Trace($"Manufacturer is empty!");
                            }
                            else
                            {
                                tracingService.Trace($"Get Prefix Fo Facility {manufacturerId.Id}");
                                preFix = GetAccountName(service, manufacturerId);
                            }
                        }
                        else if (docTypeCategoryName == "Patient Statement")
                        {
                            tracingService.Trace($"Try Get Prefix for Patient Statement");
                            if (referralId == null && caseId == null)
                            {
                                tracingService.Trace($"Referral or Case is empty!");
                            }
                            else
                            {
                                tracingService.Trace($"Get Prefix For Patient Statement CaseId: {caseId?.Id}");
                                preFix = caseId != null ? GetCaseId(service, caseId) : "";
                                useRevision = false;
                            }
                        }
                        else if ((referralId != null && caseId != null) || docTypeCategoryName == "Patient Procedure")
                        {
                            tracingService.Trace($"Try Get Prefix for Patient Procedure");
                            if (referralId == null && caseId == null)
                            {
                                tracingService.Trace($"Referral or Case is empty!");
                            }
                            else
                            {
                                tracingService.Trace($"Get Prefix For Patient Procedure CaseId: {caseId?.Id}; ReferralId: {referralId?.Id}");
                                preFix = caseId != null ? GetCaseId(service, caseId) : referralId != null ? GetReferralId(service, referralId) : "";
                            }
                        }
                        else
                        {
                            tracingService.Trace($"Try Get Prefix by default for Patient Procedure");
                            if (referralId == null && caseId == null)
                            {
                                tracingService.Trace($"Referral or Case is empty!");
                            }
                            else
                            {
                                tracingService.Trace($"Get default Prefix From Patient/Referral CaseId: {caseId?.Id}; ReferralId: {referralId?.Id}");
                                preFix = caseId != null ? GetCaseId(service, caseId) : referralId != null ? GetReferralId(service, referralId) : "";
                            }
                        }

                        tracingService.Trace("Generating and setting a new DocumentName");

                        document.ipg_name = $"{(string.IsNullOrEmpty(preFix) ? "" : $"{preFix}_")}{docType.ipg_DocumentTypeAbbreviation}_{createdOnString}{(useRevision && revision.HasValue ? $".{revision}" : "")}";

                        tracingService.Trace($"New Document Name: {document.ipg_name}");
                    }
                }
            }
            tracingService.Trace($"{typeof(RenameDocumentWhenPropertiesChange)} plugin finished");
        }

        private void FormNameOfManufacturerDocument(LocalPluginContext localPluginContext)
        {
            var context = new OrganizationServiceContext(localPluginContext.OrganizationService);

            var doc = localPluginContext.Target<ipg_document>();

            if (doc.ipg_DocumentTypeId?.Id.ToString() != priceListManufacturerDocumentTypeId)
                return;

            var manufacturerName = doc?.ipg_ipg_manufacturerid?.Name == null
                ? doc.ipg_ipg_manufacturerid.Id != null
                    ? context.CreateQuery<Intake.Account>().FirstOrDefault(acc => acc.Id == doc.ipg_ipg_manufacturerid.Id).Name
                    : null
                : doc.ipg_ipg_manufacturerid.Name;

            var docType = doc?.ipg_DocumentTypeId?.Name == null
                ? doc?.ipg_DocumentTypeId?.Id != null
                    ? context.CreateQuery<ipg_documenttype>().FirstOrDefault(type => type.Id == doc.ipg_DocumentTypeId.Id).ipg_DocumentTypeAbbreviation
                    : null
                : doc.ipg_ipg_manufacturerid.Name;

            var dateUploaded = doc.CreatedOn.Value.ToString(dateFormat);
            var version = doc.VersionNumber.HasValue ? doc.VersionNumber.Value : 1;

            if (manufacturerName != null && docType != null && dateUploaded != null)
            {
                doc.ipg_name = $"{manufacturerName}_{docType}_{dateUploaded}.{version}";
            }
        }
        private void DetermineRightRelationShip(ipg_document document, IOrganizationService service, ITracingService tracingService)
        {
            if (document.ipg_FacilityId != null && document.ipg_FacilityId.Id == document.ipg_carrierid?.Id && document.ipg_FacilityId.Id == document.ipg_ipg_manufacturerid?.Id)
            {
                tracingService.Trace($"All relationship for account the same, Determine which correct.");

                var parentAccount = service.Retrieve(document.ipg_FacilityId.LogicalName, document.ipg_FacilityId.Id, new ColumnSet(Intake.Account.Fields.CustomerTypeCode, Intake.Account.Fields.Name)).ToEntity<Intake.Account>();

                tracingService.Trace($"Parent Account with id {document.ipg_FacilityId.Id} is {parentAccount.CustomerTypeCodeEnum}");

                switch (parentAccount.CustomerTypeCodeEnum)
                {

                    case Account_CustomerTypeCode.Facility:
                        document.ipg_carrierid = document.ipg_ipg_manufacturerid = null;
                        document.ipg_FacilityId.Name = parentAccount.Name;
                        break;
                    case Account_CustomerTypeCode.Carrier:
                        document.ipg_FacilityId = document.ipg_ipg_manufacturerid = null;
                        document.ipg_carrierid.Name = parentAccount.Name;
                        break;
                    case Account_CustomerTypeCode.Manufacturer:
                        document.ipg_FacilityId = document.ipg_carrierid = null;
                        document.ipg_ipg_manufacturerid.Name = parentAccount.Name;
                        break;
                }
            }
        }

        private void GetRevision(ipg_document document, IOrganizationService service, ITracingService tracingService)
        {
            tracingService.Trace("Determining Revision Version");

            string parentAttr = "";
            EntityReference ragardingRecord = null;

            if (document.ipg_ReferralId != null)
            {
                ragardingRecord = document.ipg_ReferralId;
                parentAttr = ipg_document.Fields.ipg_ReferralId;
            }
            else if (document.ipg_CaseId != null)
            {
                ragardingRecord = document.ipg_CaseId;
                parentAttr = ipg_document.Fields.ipg_CaseId;
            }
            else if (document.ipg_FacilityId != null)
            {
                ragardingRecord = document.ipg_FacilityId;
                parentAttr = ipg_document.Fields.ipg_FacilityId;
            }
            else if (document.ipg_ipg_manufacturerid != null)
            {
                ragardingRecord = document.ipg_ipg_manufacturerid;
                parentAttr = ipg_document.Fields.ipg_ipg_manufacturerid;
            }
            else if (document.ipg_carrierid != null)
            {
                ragardingRecord = document.ipg_carrierid;
                parentAttr = ipg_document.Fields.ipg_carrierid;
            }
            else if (document.ipg_patientid != null)
            {
                ragardingRecord = document.ipg_patientid;
                parentAttr = ipg_document.Fields.ipg_patientid;
            }

            if (ragardingRecord == null)
            {
                tracingService.Trace("Document Created from document Processing it will have Revision 1.");
            }
            else if (document.ipg_DocumentTypeId != null)
            {
                tracingService.Trace($"Document Created from {ragardingRecord.KeyAttributes}({ragardingRecord.LogicalName} with id {ragardingRecord.Id}) details.");

                tracingService.Trace($"Looking for previous revision of Document with type {document.ipg_DocumentTypeId?.Id}");

                var previousDoc = service.RetrieveMultiple(new QueryExpression(document.LogicalName)
                {
                    TopCount = 1,
                    ColumnSet = new ColumnSet(ipg_document.Fields.ipg_Revision),
                    Orders = { new OrderExpression(ipg_document.Fields.ipg_Revision, OrderType.Descending) },
                    Criteria = new FilterExpression()
                    {
                        Conditions = {
                            new ConditionExpression(ipg_document.Fields.ipg_DocumentTypeId, ConditionOperator.Equal, document.ipg_DocumentTypeId.Id),
                            new ConditionExpression(parentAttr, ConditionOperator.Equal, ragardingRecord.Id)
                        }
                    }
                }).Entities.FirstOrDefault()?.ToEntity<ipg_document>();

                if (previousDoc != null)
                {
                    tracingService.Trace($"previous Doc was found {previousDoc.Id}, revision {previousDoc.ipg_Revision}");

                    document.ipg_Revision = (previousDoc.ipg_Revision ?? 0) + 1;
                    document.ipg_previousdocumentid = previousDoc.ToEntityReference();

                }
            }

            document.ipg_Revision = document.ipg_Revision ?? 1;
        }

        private string GetAccountName(IOrganizationService crmService, EntityReference account)
        {
            if (string.IsNullOrEmpty(account.Name))
            {
                account.Name = crmService.Retrieve(account.LogicalName, account.Id, new ColumnSet(Intake.Account.Fields.Name)).GetAttributeValue<string>(Intake.Account.Fields.Name);
            }

            return account.Name;
        }

        private string GetCaseId(IOrganizationService crmService, EntityReference incidentRef)
        {
            if (incidentRef?.LogicalName != Incident.EntityLogicalName) throw new ArgumentException(nameof(incidentRef));

            var caseEnt = crmService.Retrieve(incidentRef.LogicalName, incidentRef.Id, new ColumnSet(nameof(Incident.Title).ToLower(),
                                                            nameof(Incident.TicketNumber).ToLower())).ToEntity<Incident>();

            return caseEnt.Title ?? caseEnt.TicketNumber;
        }

        private string GetReferralId(IOrganizationService crmService, EntityReference referralRef)
        {
            if (referralRef?.LogicalName != ipg_referral.EntityLogicalName) throw new ArgumentException(nameof(referralRef));

            return crmService.Retrieve(referralRef.LogicalName, referralRef.Id, new ColumnSet(nameof(ipg_referral.ipg_referralcasenumber)
                .ToLower())).ToEntity<ipg_referral>().ipg_referralcasenumber;
        }

        private ipg_document GetDocument(IOrganizationService crmService, Guid? documentId, ITracingService tracingService)
        {
            tracingService.Trace($"ChangeFileNameBasedOnAssignedToOnPreCreate started");
            tracingService.Trace("Retrieving Document with id=" + documentId.Value);

            return crmService.Retrieve(ipg_document.EntityLogicalName,
                                                           documentId.Value,
                                                            new ColumnSet(ipg_document.Fields.ipg_CaseId,
                                                                          ipg_document.Fields.ipg_DocumentTypeId,
                                                                          ipg_document.Fields.ipg_Revision,
                                                                          ipg_document.Fields.ipg_carrierid,
                                                                          ipg_document.Fields.ipg_ReferralId,
                                                                          ipg_document.Fields.ipg_FacilityId,
                                                                          ipg_document.Fields.CreatedOn,
                                                                          ipg_document.Fields.ipg_documenttypecategoryid,
                                                                          ipg_document.Fields.OwnerId))
                                                      .ToEntity<ipg_document>();
        }

        private void ChangeFileNameBasedOnSourceOnPreCreate(LocalPluginContext localPluginContext)
        {
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace("Build file name Assigned To + fileName");
            var document = localPluginContext.Target<ipg_document>();
            string prefixOwner = "";
            string documentName = "";
            SystemUser Owner = new SystemUser();

            if ((localPluginContext.PluginExecutionContext.MessageName == MessageNames.Create) && (document.OwnerId?.Id ?? new Guid()) != new Guid())
            {
                tracingService.Trace("On Create Start");
                var service = localPluginContext.OrganizationService;
                try { Owner = service.Retrieve(SystemUser.EntityLogicalName, document.OwnerId.Id, new ColumnSet(SystemUser.Fields.FullName)).ToEntity<SystemUser>(); } catch { }
                documentName = document.ipg_FileName;

                try { prefixOwner = Owner.FullName; } catch { prefixOwner = ""; }

                tracingService.Trace("Filename Value on create: " + documentName);
                tracingService.Trace("Prefix Value on create: " + prefixOwner);
                tracingService.Trace("On Create Start END");
            }
            else if (localPluginContext.PluginExecutionContext.MessageName == MessageNames.Update)
            {
                var service = localPluginContext.OrganizationService;
                tracingService.Trace("On Update Start");
                var documentEntity = service.Retrieve(ipg_document.EntityLogicalName, localPluginContext.Target<ipg_document>().Id, new ColumnSet(true)).ToEntity<ipg_document>();
                tracingService.Trace("On Update OwnerId:" + documentEntity.OwnerId.Id);
                try { Owner = service.Retrieve(SystemUser.EntityLogicalName, documentEntity.OwnerId.Id, new ColumnSet(SystemUser.Fields.FullName)).ToEntity<SystemUser>(); } catch { }
                try { prefixOwner = Owner.FullName; } catch { prefixOwner = ""; }
                documentName = documentEntity.ipg_FileName;

                tracingService.Trace("FileName Value on Update: " + documentName);
                tracingService.Trace("Prefix Value on Update: " + prefixOwner);
                tracingService.Trace("On Update END");
            }

            tracingService.Trace($"Pre-Condition Step - Prefix - {prefixOwner}");
            tracingService.Trace($"Pre-Condition Step - FileName - {documentName}");

            if (!String.IsNullOrEmpty(prefixOwner) && !String.IsNullOrEmpty(documentName))
            {
                tracingService.Trace($"Condition - Prefix - {prefixOwner}");
                tracingService.Trace($"Condition - FileName - {documentName}");

                if (!documentName.StartsWith(prefixOwner))
                {
                    document.ipg_FileName = $"{prefixOwner}-{documentName}";
                }
            }

            tracingService.Trace($"ChangeFileNameBasedOnAssignedToOnPreCreate completed");
        }

    }
}