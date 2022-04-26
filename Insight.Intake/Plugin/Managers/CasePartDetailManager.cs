using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using Insight.Intake.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Insight.Intake.Repositories;

namespace Insight.Intake.Plugin.Managers
{
    public class CasePartDetailManager
    {
        private static string[] _voidPartColumns = new string[]
        {
            ipg_casepartdetail.Fields.ipg_PurchaseOrderId, ipg_casepartdetail.Fields.ipg_potypecode, ipg_casepartdetail.Fields.ipg_caseid
        };

        private static string[] _removePartColumns = new string[]
        {
            ipg_casepartdetail.Fields.ipg_PurchaseOrderId
        };

        private static string[] _voidPartOrderColumns = new string[]
        {
            SalesOrder.Fields.StateCode, SalesOrder.Fields.StatusCode
        };

        private IOrganizationService _crmService;
        private ITracingService _tracingService;

        private TaskRepository _taskRepository;

        public CasePartDetailManager(IOrganizationService crmService, ITracingService tracingService)
        {
            _crmService = crmService;
            _tracingService = tracingService;

            _taskRepository = new TaskRepository(crmService);
        }

        public void VoidCasePartFromCase(Guid casePartDetailId)
        {
            if (casePartDetailId == null || casePartDetailId == Guid.Empty)
            {
                throw new ArgumentException("Missing case part detail ID");
            }

            ipg_casepartdetail partDetail = _crmService.Retrieve<ipg_casepartdetail>(casePartDetailId, new ColumnSet(_voidPartColumns));

            _crmService.Update(new ipg_casepartdetail()
            {
                Id = partDetail.Id,
                ipg_quantity = 0
            });
            
            _crmService.Execute(new SetStateRequest()
            {
                EntityMoniker = new EntityReference(ipg_casepartdetail.EntityLogicalName, casePartDetailId),
                State = ipg_casepartdetailState.Inactive.ToOptionSetValue(),
                Status = ipg_casepartdetail_StatusCode.Inactive.ToOptionSetValue()
            });

            if (partDetail.ipg_PurchaseOrderId != null)
            {
                SalesOrder order = _crmService.Retrieve<SalesOrder>(partDetail.ipg_PurchaseOrderId.Id, new ColumnSet(_voidPartOrderColumns));

                new OrderManager(_crmService)
                    .VoidOrder(partDetail.ipg_PurchaseOrderId.Id);

                if (partDetail.ipg_potypecode != null && partDetail.ipg_potypecode.Value == (int)ipg_PurchaseOrderTypes.CPA &&
                    order.StatusCode.Value == (int)SalesOrder_StatusCode.Paid)
                {
                    new TaskManager(_crmService, _tracingService, null, Guid.Empty)
                        .CreateGenerateDebitAgainstFutureCpaFacilityTask(partDetail.ipg_caseid.Id);
                }
            }
            
            var generateClaimTasks = _taskRepository.GetGenerateSubmitClaimOpenTasks(partDetail.ipg_caseid.Id, new ColumnSet(false));

            foreach (Entity task in generateClaimTasks)
            {
                _crmService.Execute(new SetStateRequest()
                {
                    EntityMoniker = task.ToEntityReference(),
                    State = TaskState.Canceled.ToOptionSetValue(),
                    Status = Task_StatusCode.PartChanges.ToOptionSetValue()
                });
            }
        }

        public void RemoveCasePartFromCase(Guid casePartDetailId)
        {
            if (casePartDetailId == null || casePartDetailId == Guid.Empty)
            {
                throw new ArgumentException("Missing case part detail ID");
            }

            ipg_casepartdetail partDetail = _crmService.Retrieve<ipg_casepartdetail>(casePartDetailId, new ColumnSet(_removePartColumns));

            if (partDetail.ipg_PurchaseOrderId != null)
            {
                throw new ArgumentException("You can't remove a part that become part of a PO");
            }

            _crmService.Delete(partDetail.LogicalName, partDetail.Id);
        }
    }
}
