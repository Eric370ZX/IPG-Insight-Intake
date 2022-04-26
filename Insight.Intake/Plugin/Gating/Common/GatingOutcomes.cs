namespace Insight.Intake.Plugin.Gating.Common
{
    public static class GatingOutcomes
    {
        public static string NoPatientGenderOnTheCase => "02bfa36b-1a3b-eb11-a813-00224806f885";
        public static string PrimaryDxMismatchToPatientGender => "c1cb733b-1d3b-eb11-a813-00224806f885";
        public static string CalcRevUpdateNotRequired => "f057356f-0460-eb11-a812-000d3a36d4df";
        public static string NewCalcRevRequired => "57c653a0-0460-eb11-a812-000d3a36d4df";
        public static string CriticalFieldsLastChangeDateIsEmpty => "dd2c161e-0560-eb11-a812-000d3a36d4df";
        public static string AutoCarrierFound => "dd2c161e-0560-eb11-a812-000d3a36d4df";
        public static string WcCarrierFound => "dd2c161e-0560-eb11-a812-000d3a36d4df";
    }
}