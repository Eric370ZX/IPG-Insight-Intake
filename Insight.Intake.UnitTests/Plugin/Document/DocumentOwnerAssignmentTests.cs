using FakeXrmEasy;
using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class DocumentOwnerAssignmentTests : PluginTestsBase
    {
        private static Team fakedDocAdminTeam;
        private static SystemUser fakedCaseMng;
        private static SystemUser fakedSystemUser;
        private static Incident fakedCaseWithCaseMng;
        private static Incident fakedCaseWithoutCaseMng;
        private static ipg_referral fakedReferralWithCaseMng;
        private static ipg_referral fakedReferralWithoutCaseMng;
        private static Intake.Account fakedFacilityWithCaseMng;
        private static Intake.Account fakedFacilityWithoutCaseMng;
        private static ipg_documenttype fakedPortalGenericDocType;
        private static ipg_documenttype fakedFaxDocType;
        private static ipg_documenttype fakedEmailDocType;
        private static ipg_documenttype fakedOtherDocType;

        static DocumentOwnerAssignmentTests()
        {
            #region Arrange
            fakedCaseMng = new SystemUser().Fake();

            fakedSystemUser = new SystemUser().Fake();

            fakedDocAdminTeam = new Team().Fake("Document Admin");

            fakedFacilityWithCaseMng = new Intake.Account().Fake();
            fakedFacilityWithCaseMng.ipg_FacilityCaseMgrId = fakedCaseMng.ToEntityReference();

            fakedFacilityWithoutCaseMng = new Intake.Account().Fake();
            fakedFacilityWithoutCaseMng.ipg_FacilityCaseMgrId = null;

            fakedCaseWithCaseMng = new Incident().Fake();
            fakedCaseWithCaseMng.ipg_FacilityId = fakedFacilityWithCaseMng.ToEntityReference();

            fakedCaseWithoutCaseMng = new Incident().Fake();
            fakedCaseWithoutCaseMng.ipg_FacilityId = fakedFacilityWithoutCaseMng.ToEntityReference();

            fakedReferralWithCaseMng = new ipg_referral().Fake();
            fakedReferralWithCaseMng.ipg_FacilityId = fakedFacilityWithCaseMng.ToEntityReference();

            fakedReferralWithoutCaseMng = new ipg_referral().Fake();
            fakedReferralWithoutCaseMng.ipg_FacilityId = fakedFacilityWithoutCaseMng.ToEntityReference();

            fakedPortalGenericDocType = new ipg_documenttype().Fake("Portal Generic Document");
            fakedFaxDocType = new ipg_documenttype().Fake("Fax");
            fakedEmailDocType = new ipg_documenttype().Fake("Email");
            fakedOtherDocType = new ipg_documenttype().Fake("Other");
        }

        public static IEnumerable<object[]> GetDocuments()
        {
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Portal Doc with Case and Case Mng.",
                    ipg_CaseId = fakedCaseWithCaseMng.ToEntityReference(),
                    ipg_Source = ipg_DocumentSourceCode.Portal.ToOptionSetValue(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Fax type Doc with Case and with Case Mng.",
                    ipg_CaseId = fakedCaseWithCaseMng.ToEntityReference(),
                    ipg_DocumentTypeId = fakedFaxDocType.ToEntityReference(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Portal Doc with Case and without Case Mng.",
                    ipg_CaseId = fakedCaseWithoutCaseMng.ToEntityReference(),
                    ipg_Source = ipg_DocumentSourceCode.Portal.ToOptionSetValue(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Other type Doc with Case and without Case Mng.",
                    ipg_CaseId = fakedCaseWithoutCaseMng.ToEntityReference(),
                    ipg_DocumentTypeId = fakedOtherDocType.ToEntityReference(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Portal Doc with Referral and with Case Mng.",
                    ipg_ReferralId = fakedReferralWithCaseMng.ToEntityReference(),
                    ipg_Source = ipg_DocumentSourceCode.Portal.ToOptionSetValue(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Other type Doc with Referral and with Case Mng.",
                    ipg_ReferralId = fakedReferralWithCaseMng.ToEntityReference(),
                    ipg_DocumentTypeId = fakedOtherDocType.ToEntityReference(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Portal Doc with Referral and without Case Mng.",
                    ipg_ReferralId = fakedReferralWithoutCaseMng.ToEntityReference(),
                    ipg_Source = ipg_DocumentSourceCode.Portal.ToOptionSetValue(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Email type Doc with Referral and without Case Mng.",
                    ipg_ReferralId = fakedReferralWithoutCaseMng.ToEntityReference(),
                    ipg_DocumentTypeId = fakedEmailDocType.ToEntityReference(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Portal Doc with Facility and with Case Mng.",
                    ipg_FacilityId = fakedFacilityWithCaseMng.ToEntityReference(),
                    ipg_Source = ipg_DocumentSourceCode.Portal.ToOptionSetValue(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Other type Doc with Facility and with Case Mng.",
                    ipg_FacilityId = fakedFacilityWithCaseMng.ToEntityReference(),
                    ipg_DocumentTypeId = fakedOtherDocType.ToEntityReference(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = "Portal Doc with Facility and without Case Mng.",
                    ipg_FacilityId = fakedFacilityWithoutCaseMng.ToEntityReference(),
                    ipg_Source = ipg_DocumentSourceCode.Portal.ToOptionSetValue(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
            yield return new object[]
            {
                new ipg_document() {
                    Id = Guid.NewGuid(),
                    ipg_name = "Other type Doc with Facility and without Case Mng.",
                    ipg_FacilityId = fakedFacilityWithoutCaseMng.ToEntityReference(),
                    ipg_DocumentTypeId = fakedOtherDocType.ToEntityReference(),
                    OwnerId = fakedSystemUser.ToEntityReference()
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetDocuments), MemberType = typeof(DocumentOwnerAssignmentTests))]
        public void CreateDocument_ShouldAssignPropperUser(ipg_document fakedDoc)
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            var listForInit = new List<Entity>() { fakedDocAdminTeam, fakedCaseMng, fakedSystemUser, fakedPortalGenericDocType, fakedFaxDocType, fakedEmailDocType, fakedOtherDocType, fakedFacilityWithCaseMng,
                fakedFacilityWithoutCaseMng, fakedCaseWithoutCaseMng, fakedCaseWithCaseMng, fakedReferralWithCaseMng, fakedReferralWithoutCaseMng };

            fakedContext.Initialize(listForInit);
            fakedService.Create(fakedDoc);

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", fakedDoc } },
            };
            #endregion

            #region Act
            fakedContext.ExecutePluginWith<DocumentOwnerAssignment>(fakedPluginContext);
            var obtainedDoc = fakedService.Retrieve(ipg_document.EntityLogicalName, fakedDoc.Id, new ColumnSet(ipg_document.Fields.ipg_name, ipg_document.Fields.OwnerId))?.ToEntity<ipg_document>();
            #endregion

            #region Assert
            Assert.NotNull(obtainedDoc.OwnerId);

            switch (obtainedDoc.ipg_name)
            {
                case "Portal Doc with Facility and with Case Mng.":
                case "Portal Doc with Referral and with Case Mng.":
                case "Portal Doc with Case and Case Mng.":
                    Assert.Equal(fakedCaseMng.Id, obtainedDoc.OwnerId.Id);
                    break;
                case "Portal Doc with Facility and without Case Mng.":
                case "Portal Doc with Referral and without Case Mng.":
                case "Portal Doc with Case and without Case Mng.":
                case "Fax type Doc with Case and with Case Mng.":
                case "Email type Doc with Referral and without Case Mng.":
                    Assert.Equal(fakedDocAdminTeam.Id, obtainedDoc.OwnerId.Id);
                    break;
                case "Other type Doc with Facility and with Case Mng.":
                case "Other type Doc with Facility and without Case Mng.":
                case "Other type Doc with Referral and with Case Mng.":
                case "Other type Doc with Case and without Case Mng.":
                default:
                    Assert.Equal(fakedSystemUser.Id, obtainedDoc.OwnerId.Id);
                    break;
            }
            #endregion
        }
    }
}
