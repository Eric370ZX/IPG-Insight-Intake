using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class DeriveInfoFromFacility : PluginBase
    {
        private static string syncFields = $"{Intake.Account.Fields.ipg_FacilityMddId},{Intake.Account.Fields.ipg_FacilityCimId},{Intake.Account.Fields.ipg_FacilityCaseMgrId}";

        public DeriveInfoFromFacility():base(typeof(DeriveInfoFromFacility))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, DeriveOnCase);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Incident.EntityLogicalName, DeriveOnCase);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Intake.Account.EntityLogicalName, DeriveOnFacilityUpdate);
        }

        private void DeriveOnCase(LocalPluginContext localContext)
        {
            var target = localContext.Target<Incident>();
            var service = localContext.OrganizationService;

            if (target.Contains(Incident.Fields.ipg_FacilityId))
            {
                var facility = target.ipg_FacilityId != null ? service.Retrieve(target.ipg_FacilityId.LogicalName, target.ipg_FacilityId.Id
              , new ColumnSet(Intake.Account.Fields.ParentAccountId)
              ).ToEntity<Intake.Account>() : null;

                target.ipg_facilitymemberofid = facility?.ParentAccountId;
            }
        }

        private void DeriveOnFacilityUpdate(LocalPluginContext localContext)
        {
            var target = localContext.Target<Intake.Account>();
            var account = localContext.PostImage<Intake.Account>();

            if (account.CustomerTypeCodeEnum == Account_CustomerTypeCode.Facility && target.Attributes.Any(attr => syncFields.Contains(attr.Key)))
            {
                var updateCase = new Incident()
                {
                    ipg_facilitymemberofid = account.ParentAccountId
                };

                var incidents = localContext.OrganizationService.RetrieveMultiple(new QueryExpression(Incident.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(false),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression(Incident.Fields.StateCode, ConditionOperator.Equal, (int)IncidentState.Active),
                            new ConditionExpression(Incident.Fields.ipg_CaseStatus, ConditionOperator.Equal, (int)ipg_CaseStatus.Open),
                            new ConditionExpression(Incident.Fields.ipg_FacilityId, ConditionOperator.Equal, target.Id),
                        }
                    }
                }).Entities;

                foreach (var incident in incidents)
                {
                    updateCase.Id = incident.Id;
                    localContext.OrganizationService.Update(updateCase);
                }
            }
        }
    }
}
