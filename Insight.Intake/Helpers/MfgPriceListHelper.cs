using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Helpers
{
    internal static class MfgPriceListHelper
    {
        private static readonly string MfgPriceListApproverGlobalSettingName = "MFG_PRICE_LIST_APPROVER_TEAM_NAME";

        public static Team GetMfgPriceListApproverTeamOrThrow(IOrganizationService organizationService, ITracingService tracingService)
        {
            var team = GetMfgPriceListApproverTeam(organizationService, tracingService);
            if (team == null)
            {
                throw new InvalidPluginExecutionException("Could not find the team approving Mfg Price List");
            }

            return team;
        }

        private static Team GetMfgPriceListApproverTeam(IOrganizationService organizationService, ITracingService tracingService)
        {
            tracingService.Trace($"Retrieving {MfgPriceListApproverGlobalSettingName} global setting");
            var globalSettings = organizationService.RetrieveMultiple(new QueryExpression(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_globalsetting.ipg_value)),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_globalsetting.ipg_name).ToLower(), ConditionOperator.Equal, MfgPriceListApproverGlobalSettingName)
                        }
                }
            });
            if (globalSettings.Entities.Count == 0)
            {
                throw new InvalidPluginExecutionException($"Could not find the global setting '{MfgPriceListApproverGlobalSettingName}'");
            }
            var globalSetting = globalSettings.Entities[0].ToEntity<ipg_globalsetting>();
            if (string.IsNullOrWhiteSpace(globalSetting.ipg_value))
            {
                throw new InvalidPluginExecutionException($"'{MfgPriceListApproverGlobalSettingName}' global setting value is empty");
            }

            tracingService.Trace($"Retrieving '{globalSetting.ipg_value}' team");
            var teams = organizationService.RetrieveMultiple(new QueryExpression(Team.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Team.Name).ToLower(), ConditionOperator.Equal, globalSetting.ipg_value)
                        }
                }
            });
            if (teams.Entities.Count == 0)
            {
                return null;
            }

            return teams.Entities[0].ToEntity<Team>();
        }
    }
}
