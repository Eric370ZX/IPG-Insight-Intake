using System;
using Bogus;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeDocument
    {
        public static Faker<ipg_document> Fake(this ipg_document self, Guid? guid = null)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            return new Faker<ipg_document>()
                .RuleFor(x => x.Id, x => guid ?? Guid.NewGuid())
                .RuleFor(x => x.StateCode, x =>  ipg_documentState.Active)
                .RuleFor(x => x.ipg_isactivepatientstatement, x => true);
        }

        public static Faker<ipg_document> WithStatusCode(this Faker<ipg_document> self, ipg_documenttype_StatusCode statusCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            self.RuleFor(
                x => x.StatusCode,
                x => new OptionSetValue((int)statusCode)
            );

            return self;
        }

        public static Faker<ipg_document> WithOriginalDocDate(this Faker<ipg_document> self, DateTime date)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_originaldocdate, x => date);

            return self;
        }

        public static Faker<ipg_document> WithCreatedOnDate(this Faker<ipg_document> self, DateTime date)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.CreatedOn, x => date);

            return self;
        }

        public static Faker<ipg_document> WithDocumentTypeReference(this Faker<ipg_document> self, ipg_documenttype documentType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (documentType == null) throw new ArgumentNullException(nameof(documentType));

            self.RuleFor(
                x => x.ipg_DocumentTypeId,
                x => new EntityReference
                {
                    Id = documentType.Id,
                    LogicalName = documentType.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_document> WithDocumentCategoryReference(this Faker<ipg_document> self, ipg_documentcategorytype documentCategory)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (documentCategory == null) throw new ArgumentNullException(nameof(documentCategory));

            self.RuleFor(
                x => x.ipg_documenttypecategoryid,
                x => new EntityReference
                {
                    Id = documentCategory.Id,
                    LogicalName = documentCategory.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_document> WithReferralReference(this Faker<ipg_document> self, ipg_referral referral)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            
            if (referral == null) throw new ArgumentNullException(nameof(referral));

            self.RuleFor(
                x => x.ipg_ReferralId, 
                x => new EntityReference
                {
                    Id = referral.Id,
                    LogicalName = referral.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_document> WithCaseReference(this Faker<ipg_document> self, Incident caseEntity) {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (caseEntity == null) throw new ArgumentNullException(nameof(caseEntity));

            self.RuleFor(
                x => x.ipg_CaseId,
                x => new EntityReference
                {
                    Id = caseEntity.Id,
                    LogicalName = caseEntity.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_document> WithOwnerReference(this Faker<ipg_document> self, EntityReference ownerReference)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (ownerReference == null) throw new ArgumentNullException(nameof(ownerReference));

            self.RuleFor(
                x => x.OwnerId,
                x => ownerReference
            );

            return self;
        }

        public static Faker<ipg_document> WithOriginatingTaskReference(this Faker<ipg_document> self, Task task)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (task == null) throw new ArgumentNullException(nameof(task));

            self.RuleFor(
                x => x.ipg_originatingtaskid,
                x => task.ToEntityReference()
            );

            return self;
        }

        public static Faker<ipg_document> WithCarrierReference(this Faker<ipg_document> self, Account carrierEntity)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (carrierEntity == null) throw new ArgumentNullException(nameof(carrierEntity));

            self.RuleFor(
                x => x.ipg_carrierid,
                x => new EntityReference
                {
                    Id = carrierEntity.Id,
                    LogicalName = carrierEntity.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_document> WithMainDocument(this Faker<ipg_document> self, ipg_document pifDocument)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (pifDocument == null) throw new ArgumentNullException(nameof(pifDocument));

            self.RuleFor(x => x.ipg_maindocument, x => pifDocument.ToEntityReference());

            return self;
        }

        public static Faker<ipg_document>  WithFacilityReference(this Faker<ipg_document> self, Account FacilityEntity)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (FacilityEntity == null) throw new ArgumentNullException(nameof(FacilityEntity));

            self.RuleFor(
                x => x.ipg_FacilityId,
                x => new EntityReference
                {
                    Id = FacilityEntity.Id,
                    LogicalName = FacilityEntity.LogicalName
                }
            );

            return self;
        }

        public static Faker<ipg_document> WithRevision(this Faker<ipg_document> self, int version)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_Revision, version
            );

            return self;
        }

        public static Faker<ipg_document> WithName(this Faker<ipg_document> self, string name)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_name, name
            );

            return self;
        }

        public static Faker<ipg_document> WithSource(this Faker<ipg_document> self, ipg_DocumentSourceCode source)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_Source, new OptionSetValue((int)source)
            );

            return self;
        }

        public static Faker<ipg_document> WithFileName(this Faker<ipg_document> self, string fileName)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_FileName, x => fileName
            );

            return self;
        }

        public static Faker<ipg_document> WithDocumentBody(this Faker<ipg_document> self, string documentBody)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_documentbody, x => documentBody ?? x.Random.String());

            return self;
        }

        public static Faker<ipg_document> WithReviewStatus(this Faker<ipg_document> self, ipg_document_ipg_ReviewStatus reviewStatus)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_ReviewStatus, x => new OptionSetValue((int)reviewStatus)
            );

            return self;
        }

        public static Faker<ipg_document> WithDateAddedToFacility(this Faker<ipg_document> self,  DateTime addedDate)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_DateAddedToFacility, x => addedDate
            );

            return self;
        }
    }
}