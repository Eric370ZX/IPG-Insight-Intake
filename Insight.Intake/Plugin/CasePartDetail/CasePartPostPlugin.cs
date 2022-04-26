using System;
using System.Linq;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.CasePartDetail
{
    public class CasePartPostPlugin : PluginBase
    {
        public CasePartPostPlugin() : base(typeof(CasePartPostPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_casepartdetail.EntityLogicalName, PostOperationHandlerOnCreate);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_casepartdetail.EntityLogicalName, PostOperationHandlerOnUpdate);
        }

        private void PostOperationHandlerOnUpdate(LocalPluginContext context)
        {
            var crmService = context.OrganizationService;
            var tracingService = context.TracingService;
            var target = context.PostImage<ipg_casepartdetail>();

            CheckFacilityManufacturerRelationShip(target, crmService, tracingService);
            CheckMSRPPDiscountTreshHold(target, crmService, tracingService);
        }

        private void PostOperationHandlerOnCreate(LocalPluginContext context)
        {
            var crmService = context.OrganizationService;
            var tracingService = context.TracingService;
            var target = context.Target<ipg_casepartdetail>();

            if (target.ipg_productid != null && target.ipg_caseid != null)
            {              
                var taskManager = new TaskManager(crmService, tracingService, null, Guid.Empty);

                var product = crmService.Retrieve(target.ipg_productid.LogicalName, target.ipg_productid.Id, new ColumnSet(Intake.Product.Fields.ipg_status
                    , Intake.Product.Fields.ipg_manufacturerid
                    , Intake.Product.Fields.ipg_manufacturerpartnumber)).ToEntity<Intake.Product>();

                if (product.ipg_statusEnum == Product_ipg_status.Pending && product.ipg_manufacturerid != null)
                {
                    var caseEnt = crmService.Retrieve(target.ipg_caseid.LogicalName, target.ipg_caseid.Id, new ColumnSet(false)).ToEntity<Incident>();
                    product.ipg_manufacturerid.Name = product.ipg_manufacturerid.Name ?? crmService.Retrieve(product.ipg_manufacturerid.LogicalName, product.ipg_manufacturerid.Id, new ColumnSet(Intake.Account.PrimaryNameAttribute))
                        .ToEntity<Intake.Account>().Name;


                    var type = taskManager.GetTaskTypeByTaskTypeId(TaskManager.TaskTypeIds.NEW_PART_REQUEST, new ColumnSet(ipg_tasktype.Fields.ipg_description));
                    
                    if(type != null)
                    {
                        taskManager.CreateTask(product.ipg_manufacturerid, type.ToEntityReference(), new Task()
                        {
                            ipg_caseid = caseEnt.ToEntityReference(),
                            Description = (type?.ipg_description ?? "").Replace("<Part #>", product.ipg_manufacturerpartnumber).Replace("<Manufacturer>", product.ipg_manufacturerid.Name)
                        });
                    }
                }
            }

            CheckFacilityManufacturerRelationShip(target, crmService, tracingService);
            CheckMSRPPDiscountTreshHold(target, crmService, tracingService);
        }

        private void CheckFacilityManufacturerRelationShip(ipg_casepartdetail actualpart, IOrganizationService crmService, ITracingService tracingService)
        {
            if (actualpart.ipg_potypecodeEnum == ipg_PurchaseOrderTypes.ZPO
                || actualpart.ipg_potypecodeEnum == ipg_PurchaseOrderTypes.TPO
                || actualpart.ipg_potypecodeEnum == ipg_PurchaseOrderTypes.MPO)
            {

                EntityReference facilitymfgRelationShip = null;
                tracingService.Trace("Start Checking Facility Manufacturer RelationShip");

                if (actualpart.ipg_caseid == null)
                {
                    tracingService.Trace($"Ref for Case on Actual Part with id: {actualpart.Id} is empty.");
                    return;
                }

                if (actualpart.ipg_productid == null)
                {
                    tracingService.Trace($"Ref for Product on Actual Part with id: {actualpart.Id} is empty.");
                    return;
                }

                var product = crmService.Retrieve(actualpart.ipg_productid.LogicalName, actualpart.ipg_productid.Id, new ColumnSet(Intake.Product.Fields.ipg_manufacturerid)).ToEntity<Intake.Product>();
                var incident = crmService.Retrieve(actualpart.ipg_caseid.LogicalName, actualpart.ipg_caseid.Id, new ColumnSet(Incident.Fields.ipg_FacilityId)).ToEntity<Incident>();

                tracingService.Trace($"productid {product?.Id}; incidetntid {incident?.Id}");

                if (incident.ipg_FacilityId != null && product.ipg_manufacturerid != null)
                {
                    tracingService.Trace($"Looking for relationship with facility {incident.ipg_FacilityId.Id} and MFG id {product.ipg_manufacturerid.Id}");

                    facilitymfgRelationShip = crmService.RetrieveMultiple(new QueryExpression(ipg_facilitymanufacturerrelationship.EntityLogicalName)
                    {
                        ColumnSet = new ColumnSet(true),
                        TopCount = 1,
                        Criteria = new FilterExpression()
                        {
                            Conditions = {
                                new ConditionExpression(ipg_facilitymanufacturerrelationship.Fields.ipg_ManufacturerId, ConditionOperator.Equal, product.ipg_manufacturerid.Id),
                                new ConditionExpression(ipg_facilitymanufacturerrelationship.Fields.ipg_FacilityId, ConditionOperator.Equal, incident.ipg_FacilityId.Id)
                        }
                        }
                    }).Entities.FirstOrDefault()?.ToEntityReference();
                }

                if (facilitymfgRelationShip == null)
                {
                    tracingService.Trace($"relationship not found for facility {incident.ipg_FacilityId?.Id} and MFG id {product.ipg_manufacturerid?.Id}");

                    var taskManager = new TaskManager(crmService, tracingService, null, Guid.Empty);

                    taskManager.CreateTask(actualpart.ToEntityReference(), TaskManager.TaskTypeIds.MISSING_FACILITY_MFG, new Task() { ipg_caseid = actualpart.ipg_caseid });
                }
            }
        }

        private void CheckMSRPPDiscountTreshHold(ipg_casepartdetail actualpart, IOrganizationService crmService, ITracingService tracingService)
        {
            if (actualpart.ipg_potypecodeEnum == ipg_PurchaseOrderTypes.CPA && actualpart.ipg_isapprovedoverrideprice != true)
            {
                var product = crmService.Retrieve(actualpart.ipg_productid.LogicalName, actualpart.ipg_productid.Id, new ColumnSet(Intake.Product.Fields.ipg_manufacturerid, Intake.Product.Fields.ipg_msrp, Intake.Product.Fields.ipg_manufacturerdiscountpricemoney)).ToEntity<Intake.Product>();
                var mfg = crmService.Retrieve(product.ipg_manufacturerid.LogicalName, product.ipg_manufacturerid.Id, new ColumnSet(Intake.Account.Fields.ipg_ManufacturerIsParticipating)).ToEntity<Intake.Account>();

                if (mfg.ipg_ManufacturerIsParticipating != true || mfg.ipg_ManufacturerIsParticipating == true && (product.ipg_manufacturerdiscountpricemoney?.Value ?? 0) == 0
                    && (actualpart.ipg_enteredunitcost?.Value ?? 0) > ((product.ipg_msrp?.Value ?? 0) * 0.2M + (product.ipg_msrp?.Value ?? 0))
                    || (product.ipg_manufacturerdiscountpricemoney?.Value ?? 0) > 0 && (actualpart.ipg_enteredunitcost?.Value ?? 0) > product.ipg_manufacturerdiscountpricemoney?.Value)
                {
                    var taskManager = new TaskManager(crmService, tracingService, null, Guid.Empty);

                    var type = taskManager.GetTaskTypeByTaskTypeId(TaskManager.TaskTypeIds.UNIT_COST_OVERRIDE_TRESHHOLD_EXCEEDED, new ColumnSet(ipg_tasktype.Fields.ipg_description));
                    
                    taskManager.CreateTask(actualpart.ToEntityReference(), type.ToEntityReference(), new Task() 
                    { 
                        ipg_caseid = actualpart.ipg_caseid,
                        Description = type?.ipg_description.Replace("<MSRP threshold of 20% or the Discount Price of 0%>", (product.ipg_manufacturerdiscountpricemoney?.Value ?? 0) == 0 ? "MSRP threshold of 20%" : "Discount Price of 0%")
                    });
                }
            }
        }
    }
}
