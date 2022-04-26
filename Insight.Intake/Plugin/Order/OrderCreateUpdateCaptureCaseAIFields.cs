

using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Order
{
    public class OrderCreateUpdateCaptureCaseAIFields : PluginBase
    {

        public OrderCreateUpdateCaptureCaseAIFields() : base(typeof(OrderCreateUpdateCaptureCaseAIFields))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, SalesOrder.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, SalesOrder.EntityLogicalName, PreOperationHandler);
        }


        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);

            var salesOrder = (Entity)context.InputParameters["Target"];

            var ipg_CaseIdObject = localPluginContext.Target<SalesOrder>().ipg_CaseId;

            if (ipg_CaseIdObject != null)

            {

                var caseEntity = service.Retrieve(ipg_CaseIdObject.LogicalName, ipg_CaseIdObject.Id,
                new ColumnSet(Incident.Fields.Title, 
                              Incident.Fields.ipg_FacilityMRN, 
                              Incident.Fields.ipg_Facility, 
                              Incident.Fields.ipg_FacilityId, 
                              Incident.Fields.ipg_PatientFullName,
                              Incident.Fields.ipg_ActualDOS, 
                              Incident.Fields.ipg_SurgeryDate)).ToEntity<Incident>();

                if (caseEntity != null)
                {
                    salesOrder[SalesOrder.Fields.ipg_caseticketnumber] = caseEntity.Title;
                    salesOrder[SalesOrder.Fields.ipg_casefacilityname] = caseEntity.ipg_FacilityId?.Name;
                    salesOrder[SalesOrder.Fields.ipg_casefacilitymrn] = caseEntity.ipg_FacilityMRN;
                    salesOrder[SalesOrder.Fields.ipg_casepatientfullname] = caseEntity.ipg_PatientFullName;
                    salesOrder[SalesOrder.Fields.ipg_casesurgerydate] = caseEntity.ipg_ActualDOS.HasValue ? caseEntity.ipg_ActualDOS : caseEntity.ipg_SurgeryDate;
                }
            }

            Object ipg_apidObject = null;


            if (salesOrder.Contains(SalesOrder.Fields.ipg_APID))
            {

                ipg_apidObject = localPluginContext.Target<SalesOrder>().ipg_APID;
                var ipg_apidER = (EntityReference)ipg_apidObject;

                if (ipg_apidObject != null)
                {
                    var ipg_apidEntity = service.Retrieve(ipg_apidER.LogicalName, ipg_apidER.Id,
                        new ColumnSet(ipg_accountspayable.Fields.ipg_name, ipg_accountspayable.Fields.ipg_PaymentDate));
                    Object ipg_nameObject = null;


                    if (ipg_apidEntity.Contains(ipg_accountspayable.Fields.ipg_name) && ipg_apidEntity[ipg_accountspayable.Fields.ipg_name] != null && !string.IsNullOrEmpty(ipg_apidEntity[ipg_accountspayable.Fields.ipg_name].ToString()))
                        ipg_nameObject = ipg_apidEntity[ipg_accountspayable.Fields.ipg_name].ToString();

                    if (ipg_nameObject != null)
                    {
                        salesOrder[SalesOrder.Fields.ipg_claimchecknumber] = (string)ipg_nameObject;
                    }
                    Object ipg_paymentdateObject = null;

                    if (ipg_apidEntity.Contains(ipg_accountspayable.Fields.ipg_PaymentDate) && ipg_apidEntity[ipg_accountspayable.Fields.ipg_PaymentDate] != null && !string.IsNullOrEmpty(ipg_apidEntity[ipg_accountspayable.Fields.ipg_PaymentDate].ToString()))
                        ipg_paymentdateObject = ipg_apidEntity[ipg_accountspayable.Fields.ipg_PaymentDate].ToString();

                    if (ipg_paymentdateObject != null)
                    {
                        salesOrder[SalesOrder.Fields.ipg_claimcheckdate] = Convert.ToDateTime(ipg_paymentdateObject);
                    }
                }
            }
        }
    }
}

