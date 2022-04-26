using Insight.Intake.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.Managers
{
    public class OrderManager
    {
        private IOrganizationService _crmService;

        public OrderManager(IOrganizationService crmService)
        {
            _crmService = crmService;
        }

        public void VoidOrder(Guid orderId)
        {
            _crmService.Execute(new SetStateRequest()
            {
                EntityMoniker = new EntityReference(SalesOrder.EntityLogicalName, orderId),
                State = SalesOrderState.Fulfilled.ToOptionSetValue(),
                Status = SalesOrder_StatusCode.Voided.ToOptionSetValue()
            });
        }
    }
}
