using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Common;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class CaseIsClosed : PluginBase
    {
        public CaseIsClosed() : base(typeof(CaseIsClosed))
        {
            RegisterEvent(PipelineStages.PreValidation, "Update", Incident.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, "Update", Incident.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, "Update", Incident.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            var targetCase = localPluginContext.Target<Incident>();
            if (targetCase.ipg_CaseStatus?.Value ==(int) ipg_CaseStatus.Closed)
            { 
                if(localPluginContext.PluginExecutionContext.Stage== (int)PluginStage.PreOperation)
                {
                    var preImage = localPluginContext.PreImage<Incident>();
                    if (preImage?.ipg_ClosedDate == null)
                    {
                        targetCase.ipg_ClosedDate = DateTime.Now;
                    }
                    SetCaseReason(localPluginContext, service, targetCase);
                }
                if (localPluginContext.PluginExecutionContext.Stage == (int)PluginStage.PreValidation)
                {
                    var caseManager = new CaseManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetCase.ToEntityReference());
                    caseManager.CloseOutstandingTasks();
                    caseManager.ClosePSTasks();
                }
            }
        }

        private void SetCaseReason(LocalPluginContext localPluginContext, IOrganizationService service, Incident targetCase)
        {
            var incident = service.Retrieve(targetCase.LogicalName, targetCase.Id, new ColumnSet(nameof(Incident.ipg_gateconfigurationid).ToLower(), nameof(Incident.ipg_RemainingPatientBalance).ToLower())).ToEntity<Incident>();
            if ((incident.ipg_gateconfigurationid != null) && incident.ipg_gateconfigurationid.Name.Contains("11"))
            {
                if (incident.ipg_RemainingPatientBalance == null || incident.ipg_RemainingPatientBalance?.Value == 0)
                {
                    var crmContext = new OrganizationServiceContext(localPluginContext.OrganizationService);
                    string reason = "";

                    var mostRecentPayment = (from payment in crmContext.CreateQuery<ipg_payment>()
                                             where payment.ipg_CaseId.Id == targetCase.Id
                                             orderby payment.CreatedOn descending
                                             select payment).FirstOrDefault();

                    var mostRecentAdjustment = (from adjustment in crmContext.CreateQuery<ipg_adjustment>()
                                                where adjustment.ipg_CaseId.Id == targetCase.Id
                                                orderby adjustment.CreatedOn descending
                                                select adjustment).FirstOrDefault();

                    if ((mostRecentPayment == null && mostRecentAdjustment != null) || (mostRecentPayment != null && mostRecentAdjustment != null && mostRecentPayment.CreatedOn <= mostRecentAdjustment.CreatedOn))
                    {
                        if(mostRecentAdjustment.ipg_Reason != null)
                        {
                            reason = D365Helpers.GetOptionSetValueLabel(mostRecentAdjustment.LogicalName, nameof(ipg_adjustment.ipg_Reason).ToLower(), mostRecentAdjustment.ipg_Reason.Value, service);
                            if(!reason.StartsWith(@"W/O"))
                            {
                                reason = "Case Closed for No Balance - " + reason;
                            }
                        }
                    }
                    else if ((mostRecentPayment != null && mostRecentAdjustment == null) || (mostRecentPayment != null && mostRecentAdjustment != null && mostRecentPayment.CreatedOn > mostRecentAdjustment.CreatedOn))
                    {
                        reason = "Case Closed for No Balance - Payment Received";
                    }

                    if(!String.IsNullOrWhiteSpace(reason))
                    {
                        reason = reason.Replace(" ", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
                        targetCase.ipg_Reasons = new OptionSetValue((int)Enum.Parse(typeof(ipg_CaseReasons), reason));
                    }
                }
            }
        }
    }
}
