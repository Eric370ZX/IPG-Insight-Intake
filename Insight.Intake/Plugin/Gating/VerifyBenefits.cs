using Insight.Intake.Plugin.Common.Benefits;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class VerifyBenefits : PluginBase
    {
        public VerifyBenefits() : base(typeof(VerifyBenefits))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingVerifyBenefits", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            localPluginContext.Trace($"{nameof(VerifyBenefits)} plugin started");

            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            EntityReference targetRef = context.InputParameters["Target"] as EntityReference;
            context.OutputParameters["Succeeded"] = false;
            context.OutputParameters["CaseNote"] = string.Empty;
            context.OutputParameters["PortalNote"] = string.Empty;

            if (targetRef != null)
            {
                localPluginContext.Trace($"Starting {nameof(BenefitVerifier)}");
                var incColumns = new[] { Intake.Incident.Fields.ipg_CarrierId, Incident.Fields.ipg_MemberIdNumber };
                var incident = localPluginContext.SystemOrganizationService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(incColumns))
                    .ToEntity<Intake.Incident>();

                bool succeeded = false;

                bool hasValidEBV = CheckExistingBenefits(service, incident.Id, incident.ipg_CarrierId.Id, incident.ipg_MemberIdNumber, ipg_BenefitSources.EBV, 3);
                if(hasValidEBV)
                {
                    succeeded = true;
                }
                else
                {
                    bool hasValidBVF = CheckExistingBenefits(service, incident.Id, incident.ipg_CarrierId.Id, incident.ipg_MemberIdNumber, ipg_BenefitSources.BVF, 30);
                    if (hasValidBVF)
                    {
                        succeeded = true;
                    }
                }

                context.OutputParameters["Succeeded"] = succeeded;
            }
        }
        
        private bool CheckExistingBenefits(IOrganizationService service, Guid incidentId, Guid carrierId, string memberId, ipg_BenefitSources benefitSource, int maxDaysAgo)
        {
            return service.RetrieveMultiple(new QueryExpression(ipg_benefit.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                            {
                                new ConditionExpression(ipg_benefit.Fields.ipg_BenefitSource, ConditionOperator.Equal, (int)benefitSource),
                                new ConditionExpression(ipg_benefit.Fields.ipg_CaseId, ConditionOperator.Equal, incidentId),
                                new ConditionExpression(ipg_benefit.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierId),
                                new ConditionExpression(ipg_benefit.Fields.ipg_MemberID, ConditionOperator.Equal, memberId),
                                new ConditionExpression(ipg_benefit.Fields.CreatedOn, ConditionOperator.OnOrAfter,  DateTime.Today.AddDays(-maxDaysAgo)),
                                new ConditionExpression(ipg_benefit.Fields.StateCode, ConditionOperator.Equal, (int)ipg_benefitState.Active)
                            }
                }
            }).Entities.Any();
        }
    }
}