using Insight.Intake.Plugin.GatingV2.WFTasks;
using System;
using System.Collections.Generic;

namespace Insight.Intake.Plugin.GatingV2.CommonWfTask
{
    enum WfTaskType
    {
        WFCheckAutoCarrier,
        CaseIsOnHold,
        IsCPTSupportedByCarrier,
        IsCPTSupportedByFacilityCarrier,
        DetermineBenefitStatus,
        IsPatientDemographicsNotProvided,
        IsPatientInsuranceNotProvided,
        IsProcedureNameNotApproved,
        RequiredDocumentsByGatePIF,
        ValidateDxCode
    }

    public static class WfTaskFactory
    {
        private static readonly Dictionary<Guid, WfTaskType> wfTasks = new Dictionary<Guid, WfTaskType>() {
            {new Guid("a1716f36-8af2-ea11-a815-000d3a3156c1"), WfTaskType.WFCheckAutoCarrier },
            {new Guid("02275a7e-3248-eb11-a813-00224802e4a2"), WfTaskType.CaseIsOnHold },
            {new Guid("ddf1498b-3248-eb11-a813-00224802e4a2"), WfTaskType.IsCPTSupportedByCarrier },
            {new Guid("92964793-3248-eb11-a813-00224802e4a2"), WfTaskType.IsCPTSupportedByFacilityCarrier },
            {new Guid("8ce05c64-3248-eb11-a813-00224802e4a2"), WfTaskType.DetermineBenefitStatus },

            {new Guid("bb47509a-3248-eb11-a813-00224802e4a2"), WfTaskType.IsPatientDemographicsNotProvided },
            {new Guid("14fbe9ac-3248-eb11-a813-00224802e4a2"), WfTaskType.IsPatientInsuranceNotProvided },
            {new Guid("00aacab4-3248-eb11-a813-00224802e4a2"), WfTaskType.IsProcedureNameNotApproved },
            {new Guid("569fb6d0-3248-eb11-a813-00224802e4a2"), WfTaskType.RequiredDocumentsByGatePIF },
            {new Guid("c9df2ad7-3248-eb11-a813-00224802e4a2"), WfTaskType.ValidateDxCode },
        };
        public static WFTaskBase Build(Guid wfTaskId)
        {
            var wfTaskType = wfTasks[wfTaskId];
            switch (wfTaskType)
            {
                case WfTaskType.WFCheckAutoCarrier: return new IsAutoCarrier();
                case WfTaskType.CaseIsOnHold: return new CaseIsOnHold();
                case WfTaskType.IsCPTSupportedByCarrier: return new IsCPTSupportedByCarrier();
                case WfTaskType.IsCPTSupportedByFacilityCarrier: return new IsCPTSupportedByFacilityCarrier();
                case WfTaskType.DetermineBenefitStatus: return new DetermineBenefitStatus();

                case WfTaskType.IsPatientDemographicsNotProvided: return new IsPatientDemographicsNotProvided();
                case WfTaskType.IsPatientInsuranceNotProvided: return new IsPatientInsuranceNotProvided();
                case WfTaskType.IsProcedureNameNotApproved: return new IsProcedureNameNotApproved();
                case WfTaskType.RequiredDocumentsByGatePIF: return new RequiredDocumentsByGatePIF();
                case WfTaskType.ValidateDxCode: return new ValidateDxCode();
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
