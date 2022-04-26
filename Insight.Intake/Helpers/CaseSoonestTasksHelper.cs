using Insight.Intake.Models;
using System;

namespace Insight.Intake.Helpers
{
    public static class CaseSoonestTasksHelper
    {
        public static readonly Guid MissingInformationCategory = new Guid("7647BC90-4DFE-EA11-A815-000D3A3156C1");

        public static readonly Guid MedicalPolicyReviewCategory = new Guid("BDA2B1D7-7B01-EB11-A813-00224806F885");

        public static CaseSoonestTaskMapping[] FieldMappings = new CaseSoonestTaskMapping[]
        {
            new CaseSoonestTaskMapping(
                Task.Fields.ipg_taskcategoryid, MissingInformationCategory,
                Incident.Fields.ipg_missinginformationcategorytaskid),
            new CaseSoonestTaskMapping(
                Task.Fields.ipg_taskcategoryid, MedicalPolicyReviewCategory,
                Incident.Fields.ipg_medicalpolicyreviewcategorytaskid)
        };
    };
}