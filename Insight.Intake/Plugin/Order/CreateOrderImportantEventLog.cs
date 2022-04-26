using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Order
{
    public class CreateOrderImportantEventLog : PluginBase
    {
        public CreateOrderImportantEventLog() : base(typeof(CreateOrderImportantEventLog))
        {
            RegisterEvent(PipelineStages.PostOperation, "Create", SalesOrder.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var context = localPluginContext.PluginExecutionContext;
            var logId = Guid.Empty;
            var salesOrderEntity = localPluginContext.Target<SalesOrder>();
            var caseRef = salesOrderEntity.ipg_CaseId;

            if (caseRef == null || salesOrderEntity.ipg_potypecode == null || salesOrderEntity.Name == null)
                return;

            var caseEntity = service.Retrieve(caseRef.LogicalName, caseRef.Id, new ColumnSet(Incident.Fields.Title));
            var importantEventManager = new ImportantEventManager(service);

            if (salesOrderEntity.StatusCode.Value == (int)SalesOrder_StatusCode.Generated)
            {
                logId = importantEventManager.CreateImportantEventLog(caseEntity, context.InitiatingUserId, Constants.EventIds.ET7, new[] { salesOrderEntity.Name.ToString() });
                importantEventManager.SetCaseOrReferralPortalHeader(caseEntity, Constants.EventIds.ET7);
            }

            if (salesOrderEntity.StatusCode.Value == (int)SalesOrder_StatusCode.CommtoFacility
                || salesOrderEntity.StatusCode.Value == (int)SalesOrder_StatusCode.CommtoMFG)
            {
                string source = string.Empty;
                switch (salesOrderEntity.ipg_potypecode.Value)
                {
                    case (int)ipg_PurchaseOrderTypes.CPA:
                    case (int)ipg_PurchaseOrderTypes.TPO:
                    case (int)ipg_PurchaseOrderTypes.ZPO:
                        source = "Facility";
                        break;
                    case (int)ipg_PurchaseOrderTypes.MPO:
                        source = "Manufacturer";
                        break;
                    default:
                        break;
                }
                logId = importantEventManager.CreateImportantEventLog(caseEntity, context.UserId, Constants.EventIds.ET8, new[] { salesOrderEntity.Name.ToString(), source });
                importantEventManager.SetCaseOrReferralPortalHeader(caseEntity, Constants.EventIds.ET8);
            }
            if (logId != Guid.Empty)
                context.OutputParameters["Success"] = true;
        }
    }
}