using System;

namespace Insight.Intake.Models
{
    public class LifecycleStepsConstants
    {
        public static readonly Guid AddPartsGate6 = new Guid("8055a135-3cd5-e911-a9b8-000d3a367d35");
        public static readonly Guid AuthorizationGate3 = new Guid("20a244cb-d3c1-e911-a983-000d3a37043b");
        public static readonly Guid CalculateRevenueGate7 = new Guid("8255a135-3cd5-e911-a9b8-000d3a367d35");
        public static readonly Guid ClaimRemittanceGate11 = new Guid("8a55a135-3cd5-e911-a9b8-000d3a367d35");
        public static readonly Guid DOSTransitionGate4 = new Guid("2f80a9da-d3c1-e911-a983-000d3a37043b");
        public static readonly Guid EHRRefferalGateEHR = new Guid("1f5d0ed1-1136-ea11-a813-000d3a33fc30");
        public static readonly Guid GeneratePOGate8 = new Guid("8455a135-3cd5-e911-a9b8-000d3a367d35");
        public static readonly Guid IntakeStep1Gate1 = new Guid("d52dccb6-d3c1-e911-a983-000d3a37043b");
        public static readonly Guid IntakeStep2Gate2 = new Guid("871532c4-d3c1-e911-a983-000d3a37043b");
        public static readonly Guid PatientPaymentGate11 = new Guid("8c55a135-3cd5-e911-a9b8-000d3a367d35");
        public static readonly Guid SubmitClaimGate9 = new Guid("8655a135-3cd5-e911-a9b8-000d3a367d35");
        public static readonly Guid ZirmedResponseGate10 = new Guid("8855a135-3cd5-e911-a9b8-000d3a367d35");
    }

    public class WorkflowTasksConstants
    {
        public static readonly Guid AddPartsGate6 = new Guid("8055a135-3cd5-e911-a9b8-000d3a367d35");
        public static readonly Guid AuthorizationGate3 = new Guid("20a244cb-d3c1-e911-a983-000d3a37043b");
        public static readonly Guid CalculateRevenueGate7 = new Guid("8255a135-3cd5-e911-a9b8-000d3a367d35");
        public static readonly Guid PreauthorizationHCPCS = new Guid("3e234070-11da-eb11-bacb-000d3a58c907");
    }
    public class WorkflowTaskGroupsConstants
    {
        public static readonly Guid ReviewAuthorizationRequirements = new Guid("ebf16551-a481-eb11-a812-000d3a31bbd2");
    }

    public class GlobalSettingConstants
    {
        public static readonly string DefaultProcedureIdSettingName = "NoCptDefaultProcedureId";
    }
}