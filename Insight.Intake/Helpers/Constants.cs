using System;

namespace Insight.Intake.Helpers
{
    public static class Constants
    {
        public static class ActionNames
        {
            public const string CaseActionsDeterminePrimaryCarrier = "ipg_IPGCaseActionsDeterminePrimaryCarrier";
            public const string IntakeActionsCopyPosts = "ipg_IPGIntakeActionsCopyPosts";
            public const string IntakeActionsCopyDocuments = "ipg_IPGIntakeActionsCopyDocuments";
            public const string IntakeActionsCloneRecord = "ipg_IPGIntakeActionsCloneRecord";
            public const string IntakeActionsCheckCPTCodes = "ipg_IPGIntakeActionsCheckCPTCodes";
            public const string GatingStartGateProcessing = "ipg_IPGGatingStartGateProcessing";
            public const string CaseActionsCreateClaim = "ipg_IPGCaseActionsCreateClaim";
            public const string CaseCloseCase = "ipg_IPGCaseCloseCase";
        }

        public static class Settings
        {
            public const string GenerateClaimLogicApp = "GenerateClaimLogicApp";
            public const string EHRResubmitURL = "EHRResubmitURL";
            public const string PortalGenericDocumentSettings = "PortalGenericDocumentSettings";
            public const string Gate11 = "Gate 11";
            public const string TissueRequestForm_CPTCodes = "TissueRequestForm_CPTCodes";
            public const string MfgPriceListApprover = "MFG_PRICE_LIST_APPROVER_TEAM_NAME";
            public const string CMS1500_PDF_TEMPLATE = "CMS1500_PDF_TEMPLATE";
            public const string UB04_PDF_TEMPLATE = "UB04_PDF_TEMPLATE";
            public const string Collection_LF_Step_Id = "Lifecyclestep.Collections";
            public const string PS_Service_LF_Step_Id = "Lifecyclestep.PatientService";
        }

        public static class TeamGuids
        {
            public static readonly Guid CaseManagement = new Guid("EB250319-B41D-E911-A979-000D3A370E23");
            public static readonly Guid CarrierServices = new Guid("6FE6456E-8EF9-E911-A813-000D3A33F7CA");
        }

        public static class TeamNames
        {
            public const string ImplementationMgr = "Implementation Mgr";
            public const string DocumentAdmin = "Document Admin";
            public const string CaseManagement = "Case Management";
            public const string PatientServices = "Patient Services";
        }

        public static class GateNames
        {
            public const string Gate1 = "Gate 1";
            public const string Gate2 = "Gate 2";
            public const string Gate3 = "Gate 3";
            public const string Gate9 = "Gate 9";
            public const string Gate11 = "Gate 11";
        }

        public static class GateConfigurationGuids
        {
            public static readonly Guid Gate2Config = new Guid("3c407929-aa4c-e911-a982-000d3a37062b");
            public static readonly Guid EhrGateId = new Guid("1c3f4e0e-0236-ea11-a813-000d3a33fc30");
        }

        public static class LifecycleStepGuids
        {
            public static readonly Guid IntakeStep2LifecycleStepId = new Guid("871532c4-d3c1-e911-a983-000d3a37043b");
            public static readonly Guid EhrLifecycleStepId = new Guid("1f5d0ed1-1136-ea11-a813-000d3a33fc30");
        }

        public static class CaseStatusDisplayedGuids
        {
            public static readonly Guid CaseStatusDisplayedId = new Guid("fbf272d4-42ac-e911-a987-000d3a370909");
            public static readonly Guid BillingInProgress = new Guid("911e6b83-43ac-e911-a987-000d3a370909");
            public static readonly Guid CarrierCollectionsInProgress = new Guid("72cc1396-43ac-e911-a987-000d3a370909");
        }

        public static class RoleGuids
        {
            public static readonly Guid AdminRole = new Guid("627090FF-40A3-4053-8790-584EDC5BE201");
        }

        public static class TaskCategory
        {
            public static readonly Guid Administrative = new Guid("ec85a524-dd11-eb11-a813-000d3a3156c1");
        }

        public static class TaskCategoryNames
        {
            public const string CaseProcessing = "Case Processing";
            public const string MissingInformation = "Missing Information";
            public const string SLA = "SLA";
            public const string CarrierCollections = "Carrier Collections";
            public const string PatientCollections = "Patient Collections";
            public const string PatientOutreach = "Patient Outreach";
            public const string CarrierOutreach = "Carrier Outreach";
        }

        public static class TaskSubCategoryNames
        {
            public const string NoClaimHolds = "No Claim Holds";
            public const string SLAMet = "SLA Met";
            public const string SLANotMet = "SLA Not Met";
        }

        public static class TaskTypeNames
        {
            public const string RequestToCompleteCaseMgmtWork = "Request to Complete Case Mgmt. Work (Pool)";
            public const string FacilityRecoveryResearchPending = "Facility Recovery - Research Pending";
            public const string InstitutionalClaimsReadyToPrint = "Institutional Claims Ready to Print";
            public const string ProfessionalClaimsReadyToPrint = "Professional Claims Ready to Print";
            public const string SLAPayProvider = "SLA - Pay Provider";
            public const string SLAGeneratePO = "SLA - Generate PO";
            public const string PotentialDuplicatePatient = "Potential Duplicate Patient";
            public const string NewPortalUserRequest = "New Portal User Request";
            public const string HomePlanCouldNotBeDetermined = "Home Plan Could Not Be Determined";
            public const string InactiveHomePlan = "Inactive Home Plan";
           
            public const string OutgoingCallRequiredLevel1 = "Outgoing Call Required. Level 1";
        }

        public static class TaskReason
        {
            public static readonly Guid CaseIsClosed = new Guid("1fdc8e79-a8dd-eb11-bacb-002248044f51");
        }

        public static class TaskReasonNames 
        {
            public static string Pending_Payment_from_Carrier = "Pending Payment from Carrier";
        }

        public static class Workflows
        {
            public static Guid VerifyBenefitsAsyncId => new Guid("8b204983-82cf-46b9-8ccc-4759b8b4506e");
        }

        public static class EventIds
        {
            public const string ET1 = "ET1";
            public const string ET4 = "ET4";
            public const string ET5 = "ET5";
            public const string ET6 = "ET6";
            public const string ET7 = "ET7";
            public const string ET8 = "ET8";
            public const string ET9 = "ET9";
            public const string ET10 = "ET10";
            public const string ET11 = "ET11";
            public const string ET12 = "ET12";
            public const string ET25 = "ET25";
            public const string ET26 = "ET26";
            public const string ET27 = "ET27";
        }

        public static class DocumentTypeAbbreviations
        {
            public const string PostProcedurePacket = "PPP";
            public const string PatientStatementDocType = "PST";
            public const string PIF = "PIF";
            public const string BVF = "BVF";
            public const string MFG_INV = "MFG INV";
            public const string MfgPriceListDocType = "PRL";
            public const string EbvDocumentType = "EBV";
        }

        public static class DocumentTypeGuids
        {
            public static readonly Guid PortalGenericDocument = new Guid("2a8e6b4f-06fd-14bf-ad19-4ea2327a4f76");
        }

        public static class DocumentTypeNames
        {
            public const string CMS1500_UB04 = "CMS1500/UB04";
        }

        public static class DocumentCategioryNames
        {
            public const string PatientStatement = "Patient Statement";
        }

        public static class ActivityTypeNames
        {
            public const string ExceptionApproved = "Exception Approved";
        }

        public static class FeeScheduleGuids
        {
            public static readonly Guid IPG = new Guid("494439d4-6939-4bc0-b82c-52fc38656b40");
        }

        public static class UserNames
        {
            public const string ImportUserName = "Import Process";
        }

        public static class SecurityRoleNames
        {
            public const string SYS_ADMIN = "System Administrator";
        }

        public static class TaskStatusReasonNames
        {
            public const string CaseUnlocked = "Case Unlocked";
        }

        public static class AnnotationSubjects
        {
            public const string ReferralNote = "Referral Note";
        }
    }
}