using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Plugin.Managers;

namespace Insight.Intake.Plugin.ClaimResponse
{
    public class CalculatePatientResponsibility : PluginBase
    {

        public CalculatePatientResponsibility() : base(typeof(CalculatePatientResponsibility))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_claimlineitem.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_claimlineitem.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var target = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimlineitem>();
                var claimRef = target.ipg_claimid;
                if (context.MessageName == MessageNames.Update)
                {
                    claimRef = service.Retrieve(target.LogicalName, target.Id, new ColumnSet(nameof(ipg_claimlineitem.ipg_claimid).ToLower())).ToEntity<ipg_claimlineitem>().ipg_claimid;
                }
                decimal allowed = (target.ipg_allowed == null ? 0 : target.ipg_allowed.Value);

                var queryExpression = new QueryExpression(ipg_claimlineitem.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimlineitem.ipg_allowed).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimlineitem.ipg_claimid).ToLower(), ConditionOperator.Equal, claimRef.Id)
                        }
                    }
                };
                EntityCollection claimLineItems = service.RetrieveMultiple(queryExpression);
                decimal totalAllowed = claimLineItems.Entities.Sum(claimLineItem => (claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_allowed).ToLower()) == null ? 0 : claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_allowed).ToLower()).Value));

                Invoice invoice = service.Retrieve(Invoice.EntityLogicalName, claimRef.Id, new ColumnSet(nameof(Invoice.ipg_caseid).ToLower())).ToEntity<Invoice>();
                
                double memberConisurance = 0;
                decimal unmetdeductible = 0;
                var caseManager = new CaseManager(service, localPluginContext.TracingService, invoice.ipg_caseid);
                var benefit = caseManager.GetBenefit(service);
                if (benefit != null)
                {
                    memberConisurance = benefit.GetAttributeValue<double?>(nameof(ipg_benefit.ipg_MemberCoinsurance).ToLower()) ?? 0;
                    decimal deductible = (benefit.GetAttributeValue<Money>(nameof(ipg_benefit.ipg_Deductible).ToLower()) == null ? 0 : benefit.GetAttributeValue<Money>(nameof(ipg_benefit.ipg_Deductible).ToLower()).Value);
                    decimal deductibleMet = (benefit.GetAttributeValue<Money>(nameof(ipg_benefit.ipg_DeductibleMet).ToLower()) == null ? 0 : benefit.GetAttributeValue<Money>(nameof(ipg_benefit.ipg_DeductibleMet).ToLower()).Value);
                    unmetdeductible = deductible - deductibleMet;
                }

                if (totalAllowed != 0)
                {
                    target.ipg_PatientResponsibility = new Money((allowed - ((allowed / totalAllowed) * unmetdeductible)) * (decimal)memberConisurance / 100);
                }
                else
                {
                    target.ipg_PatientResponsibility = new Money(0);
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}