using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Insight.Intake.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Case
{
    public class PreValidatePO : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                if (context.MessageName != "ipg_IPGCaseActionsPreValidatePO" || !context.InputParameters.Contains("Target"))
                {
                    return;
                }

                if (context.InputParameters.Contains("Validation")
                    && (context.InputParameters["Validation"] as string).ToLower() == "manufacturer")
                {
                    var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    var organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);

                    var mfg = context.InputParameters["ManufacturerId"] as EntityReference;
                    var incidentRef = context.InputParameters["Target"] as EntityReference;
                    var incident = organizationService.Retrieve(incidentRef.LogicalName, incidentRef.Id
                        , new ColumnSet(nameof(Incident.ipg_FacilityId).ToLower())).ToEntity<Incident>();

                    if (!context.InputParameters.Contains("ManufacturerId") || (context.InputParameters["ManufacturerId"] as EntityReference) == null)
                    {
                        context.OutputParameters["HasError"] = true;
                        context.OutputParameters["Message"] = "Check ManufacturerId";
                    }
                    else if (incident.ipg_FacilityId == null)
                    {
                        context.OutputParameters["HasError"] = true;
                        context.OutputParameters["Message"] = "Case doesn't have Facility!";
                    }
                    else if (!IsManufacturerNumberValid(organizationService, mfg, incident.ipg_FacilityId))
                    {
                        CreateRequiredFacilityNumbTask(organizationService, incidentRef, mfg);
                        context.OutputParameters["HasError"] = true;
                        context.OutputParameters["Message"] = $"Facility Account # for {mfg.Name} Manufacturer Does Not Exist!";
                    }
                }
                else
                {
                    var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    var organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);

                    var incidentRef = (EntityReference)context.InputParameters["Target"];

                    var incident = (Incident)organizationService.Retrieve(
                        incidentRef.LogicalName,
                        incidentRef.Id,
                        new ColumnSet(true)
                    );

                    var errorList = new List<string>();

                    var infoList = new List<string>();

                    ValidateIncident(incident, ref errorList);

                    ValidateIncidentParts(organizationService, incident, ref errorList);

                    ValidateRequiredDocuments(organizationService, incident, ref errorList);

                    ValidateManufacturer(organizationService, incident, ref errorList, ref infoList);

                    if (errorList.Count > 0 || infoList.Count > 0)
                    {
                        var hasError = errorList.Count > 0;

                        context.OutputParameters["HasError"] = hasError;

                        if (hasError)
                        {
                            context.OutputParameters["Message"] = string.Join(", ", errorList);
                        }
                        else
                        {
                            context.OutputParameters["Message"] = string.Join(", ", infoList);
                        }
                    }
                    else
                    {
                        context.OutputParameters["HasError"] = true;

                        context.OutputParameters["Message"] = "No mfg's eligible for PO generation at this time.";
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(PreValidatePO)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace("{0}: {1}", nameof(PreValidatePO), exception.ToString());
                throw;
            }
        }


        /// <summary>
        /// Validates incident information and populates error list.
        /// Validation rules:
        ///     1. Patient must be entered
        ///     2. Procedure type must be entered
        ///     3. Carrier must be entered
        ///     4. Facility must be entered
        ///     5. Stage must be entered
        /// </summary>
        /// <param name="incident">Incident record</param>
        /// <param name="errorList">List of errors</param>
        private void ValidateIncident(Incident incident, ref List<string> errorList)
        {
            if (string.IsNullOrEmpty(incident.ipg_PatientFirstName))
            {
                errorList.Add("Patient First Name");
            }

            if (string.IsNullOrEmpty(incident.ipg_PatientLastName))
            {
                errorList.Add("Patient Last Name");
            }

            if (string.IsNullOrEmpty(incident.ipg_ProcedureName))
            {
                errorList.Add("Procedure");
            }

            if (incident.ipg_CarrierId == null)
            {
                errorList.Add("Carrier");
            }

            if (incident.ipg_FacilityId == null)
            {
                errorList.Add("Facility");
            }

            if (incident.ipg_DeviceStage == null)
            {
                errorList.Add("Stage");
            }
        }

        /// <summary>
        /// Validates parts related to incident and populates error list.
        /// Validation rules:
        ///     1. If is TPO, then incident must contains at least one related part with following criteria.
        ///         1.1. Part type must be Biologics
        ///         1.2. Part inventory type must be Drop Ship
        ///     2. If is not TPO, then incident must contains at least one related part
        /// </summary>
        /// <param name="incident">Incident record</param>
        /// <param name="errorList">List of errors</param>
        private void ValidateIncidentParts(IOrganizationService organizationService, Incident incident, ref List<string> errorList)
        {
            var isTpoOnly = incident.ipg_ActualDOS == null || incident.ipg_ActualDOS < DateTime.MinValue;

            var incidentParts = new List<ipg_casepartdetail>();

            var incidentPartsQuery = new QueryExpression
            {
                EntityName = ipg_casepartdetail.EntityLogicalName,
                ColumnSet = new ColumnSet(true),
                PageInfo = new PagingInfo
                {
                    ReturnTotalRecordCount = true,
                },
            };

            #region Check what no parts being on the case
            incidentPartsQuery.Criteria = new FilterExpression(LogicalOperator.And);

            incidentPartsQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, incident.Id);

            if (isTpoOnly)
            {
                var incidentPartsRequest = organizationService.RetrieveMultiple(incidentPartsQuery);

                foreach (var incidentPart in incidentPartsRequest.Entities.Cast<ipg_casepartdetail>())
                {
                    if (incidentPart.ipg_productid == null)
                    {
                        continue;
                    }

                    var part = (Intake.Product)organizationService.Retrieve(
                        incidentPart.ipg_productid.LogicalName,
                        incidentPart.ipg_productid.Id,
                        new ColumnSet(true)
                    );

                    if ((part.ProductTypeCode.Value == ProductTypeOptionSet.Biologics || part.ProductTypeCode.Value == ProductInventoryTypeOptionSet.DropShip)
                        && incidentPart.ipg_payfacility == false)
                    {
                        incidentParts.Add(incidentPart);
                    }
                }
            }
            else
            {
                var incidentPartsRequest = organizationService.RetrieveMultiple(incidentPartsQuery);

                incidentParts.AddRange(incidentPartsRequest.Entities.Cast<ipg_casepartdetail>());
            }

            if (incidentParts.Count == 0)
            {
                errorList.Add("No Parts eligible for PO generation. Check Inventory Method, ReOrder Method, Part Type, Actual DOS, etc.");

                // Don't check parts again if parts do not exist.
                return;
            }
            #endregion

            #region Check what no parts being eligible for PO generation
            incidentPartsQuery.Criteria = new FilterExpression(LogicalOperator.And);

            incidentPartsQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, incident.Id);

            incidentPartsQuery.Criteria.AddCondition("ipg_purchaseorderid", ConditionOperator.Null);

            if (isTpoOnly)
            {
                var incidentPartsRequest = organizationService.RetrieveMultiple(incidentPartsQuery);

                foreach (var incidentPart in incidentPartsRequest.Entities.Cast<ipg_casepartdetail>())
                {
                    if (incidentPart.ipg_productid == null)
                    {
                        continue;
                    }

                    var part = (Intake.Product)organizationService.Retrieve(
                        incidentPart.ipg_productid.LogicalName,
                        incidentPart.ipg_productid.Id,
                        new ColumnSet(true)
                    );

                    if ((part.ProductTypeCode.Value == ProductTypeOptionSet.Biologics || part.ProductTypeCode.Value == ProductInventoryTypeOptionSet.DropShip)
                        && incidentPart.ipg_payfacility == false)
                    {
                        incidentParts.Add(incidentPart);
                    }
                }
            }
            else
            {
                var incidentPartsRequest = organizationService.RetrieveMultiple(incidentPartsQuery);

                incidentParts.AddRange(incidentPartsRequest.Entities.Cast<ipg_casepartdetail>());
            }
            #endregion
        }

        /// <summary>
        /// Validates required documents and populates error list.
        /// </summary>
        /// <param name="incident">Incident record</param>
        /// <param name="errorList">List of errors</param>
        private void ValidateRequiredDocuments(IOrganizationService organizationService, Incident incident, ref List<string> errorList)
        {
            var isTpoOnly = incident.ipg_ActualDOS == null || incident.ipg_ActualDOS < DateTime.MinValue;

            if (!isTpoOnly)
            {
                var requiredInformationQuery = new QueryExpression
                {
                    EntityName = ipg_requiredinformation.EntityLogicalName,
                    Criteria = new FilterExpression(LogicalOperator.And),
                    ColumnSet = new ColumnSet(true),
                    PageInfo = new PagingInfo
                    {
                        ReturnTotalRecordCount = true,
                    },
                };

                requiredInformationQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, incident.Id);

                var requiredInformationRequest = organizationService.RetrieveMultiple(requiredInformationQuery);

                foreach (var requiredInformation in requiredInformationRequest.Entities.Cast<ipg_requiredinformation>())
                {
                    var requiredDocumentsQuery = new QueryExpression
                    {
                        EntityName = ipg_document.EntityLogicalName,
                        Criteria = new FilterExpression(LogicalOperator.And),
                        ColumnSet = new ColumnSet(true),
                        PageInfo = new PagingInfo
                        {
                            ReturnTotalRecordCount = true,
                        },
                    };

                    requiredDocumentsQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, incident.Id);

                    requiredDocumentsQuery.Criteria.AddCondition("ipg_documenttypeid", ConditionOperator.Equal, requiredInformation.ipg_DocumentTypeId.Id);

                    var requiredDocumentsRequest = organizationService.RetrieveMultiple(requiredDocumentsQuery);

                    if (requiredDocumentsRequest.Entities.Count == 0)
                    {
                        var tmpError = "Required documents not available";

                        if (!errorList.Contains(tmpError))
                        {
                            errorList.Add(tmpError);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates facility manufacturer contracts and populates error list.
        /// </summary>
        /// <param name="incident">Incident record</param>
        /// <param name="errorList">List of error messages</param>
        /// <param name="infoList">List of info messages</param>
        private void ValidateManufacturer(IOrganizationService organizationService, Incident incident, ref List<string> errorList, ref List<string> infoList)
        {
            var isTpoOnly = incident.ipg_ActualDOS == null || incident.ipg_ActualDOS < DateTime.MinValue;

            if (errorList.Count > 0)
            {
                return;
            }

            var facility = (Intake.Account)organizationService.Retrieve(
                Intake.Account.EntityLogicalName,
                incident.ipg_FacilityId.Id,
                new ColumnSet(true)
            );

            var carrier = (Intake.Account)organizationService.Retrieve(
                Intake.Account.EntityLogicalName,
                incident.ipg_CarrierId.Id,
                new ColumnSet(true)
            );

            var incidentParts = new List<ipg_casepartdetail>();

            var incidentPartsQuery = new QueryExpression
            {
                EntityName = ipg_casepartdetail.EntityLogicalName,
                ColumnSet = new ColumnSet(true),
                PageInfo = new PagingInfo
                {
                    ReturnTotalRecordCount = true,
                },
            };

            if (isTpoOnly)
            {
                incidentPartsQuery.Criteria = new FilterExpression(LogicalOperator.And);

                incidentPartsQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, incident.Id);

                incidentPartsQuery.Criteria.AddCondition("ipg_purchaseorderid", ConditionOperator.Null);

                var incidentPartsRequest = organizationService.RetrieveMultiple(incidentPartsQuery);

                foreach (var incidentPart in incidentPartsRequest.Entities.Cast<ipg_casepartdetail>())
                {
                    if (incidentPart.ipg_manufacturerid == null)
                    {
                        continue;
                    }

                    var manufacturer = (Intake.Account)organizationService.Retrieve(
                        Intake.Account.EntityLogicalName,
                        incidentPart.ipg_manufacturerid.Id,
                        new ColumnSet(true)
                    );

                    // TPO
                    if ((facility.ipg_dtmmemberEnum == Account_ipg_dtmmember.Yes || carrier.ipg_RequireMfgPOOnly == true || manufacturer.ipg_ManufacturerIsParticipating == true)
                        && incidentPart.ipg_payfacility == false
                        && (incidentPart.ipg_facilityorders.Value == FacilityOrdersOptionSet.FacilityOrders || incidentPart.ipg_facilityorders.Value == FacilityOrdersOptionSet.TpoOrders))
                    {
                        incidentParts.Add(incidentPart);
                    }
                }
            }
            else
            {
                incidentPartsQuery.Criteria = new FilterExpression(LogicalOperator.And);

                incidentPartsQuery.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, incident.Id);

                incidentPartsQuery.Criteria.AddCondition("ipg_purchaseorderid", ConditionOperator.Null);

                var incidentPartsRequest = organizationService.RetrieveMultiple(incidentPartsQuery);

                foreach (var incidentPart in incidentPartsRequest.Entities.Cast<ipg_casepartdetail>())
                {
                    if (incidentPart.ipg_manufacturerid == null)
                    {
                        continue;
                    }

                    var manufacturer = (Intake.Account)organizationService.Retrieve(
                        Intake.Account.EntityLogicalName,
                        incidentPart.ipg_manufacturerid.Id,
                        new ColumnSet(true)
                    );

                    // TODO: Refactor
                    // This logic is taken from Legacy application. I decided to keep it in case we need to understand logical determination of PO type.  
                    // This particular logic will need to be removed in the future because as it stands right now it doesn't make sense.

                    // ZPO
                    if ((facility.ipg_dtmmemberEnum == Account_ipg_dtmmember.Yes || carrier.ipg_RequireMfgPOOnly == true || manufacturer.ipg_ManufacturerIsParticipating == true)
                        && incidentPart.ipg_payfacility == false
                        && incidentPart.ipg_facilityorders.Value == FacilityOrdersOptionSet.FacilityOrders)
                    {
                        incidentParts.Add(incidentPart);
                    }
                    // MPO
                    else if ((facility.ipg_dtmmemberEnum == Account_ipg_dtmmember.Yes || carrier.ipg_RequireMfgPOOnly == true || manufacturer.ipg_ManufacturerIsParticipating == true)
                             && incidentPart.ipg_payfacility == false
                             && incidentPart.ipg_facilityorders.Value != FacilityOrdersOptionSet.FacilityOrders)
                    {
                        incidentParts.Add(incidentPart);
                    }
                    // CPA
                    else
                    {
                        incidentParts.Add(incidentPart);
                    }
                }
            }

            var todayDate = DateTime.UtcNow;

            foreach (var incidentPart in incidentParts)
            {
                var manufacturer = (Intake.Account)organizationService.Retrieve(
                    Intake.Account.EntityLogicalName,
                    incidentPart.ipg_manufacturerid.Id,
                    new ColumnSet(true)
                );

                // Check that manufacturer account number exists, if the facility requires it.
                if (manufacturer.ipg_ManufacturerIsFacilityAcctRequired == true && string.IsNullOrEmpty(manufacturer.ipg_manufactureraccountnumber))
                {
                    errorList.Add($"{manufacturer.Name} missing required account number for facility");
                }
                else if (!isTpoOnly)
                {
                    var tmpName = $"{manufacturer.Name}";

                    if (!infoList.Contains(tmpName))
                    {
                        infoList.Add(tmpName);
                    }
                }
                else
                {
                    var tmpName = $"{manufacturer.Name} **<- TPO Only ** (No Actual DOS)";

                    if (!infoList.Contains(tmpName))
                    {
                        infoList.Add(tmpName);
                    }
                }

                // Only check contracted manufactures.
                if (facility.ipg_dtmmemberEnum == Account_ipg_dtmmember.Yes && todayDate >= manufacturer.ipg_ManufacturerEffectiveDate
                    && todayDate <= manufacturer.ipg_ManufacturerExpirationDate)
                {
                    var facilityManufacturerQuery = new QueryExpression
                    {
                        EntityName = ipg_facilitymanufacturerrelationship.EntityLogicalName,
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression(LogicalOperator.And),
                        PageInfo = new PagingInfo
                        {
                            ReturnTotalRecordCount = true,
                        },
                    };

                    facilityManufacturerQuery.Criteria.AddCondition("ipg_manufacturerid", ConditionOperator.Equal, manufacturer.Id);

                    facilityManufacturerQuery.Criteria.AddCondition("ipg_facilityid", ConditionOperator.Equal, facility.Id);

                    var facilityManufacturerRequest = organizationService.RetrieveMultiple(facilityManufacturerQuery);

                    if (facilityManufacturerRequest.Entities.Count == 0)
                    {
                        var tmpName = $"{manufacturer.Name} missing manufacturer definition for facility";

                        if (!errorList.Contains(tmpName))
                        {
                            errorList.Add(tmpName);
                        }
                    }
                }
            }
        }


        private Intake.Account GetManufacturer(IOrganizationService service, EntityReference manufacturerRef)
        {
            return manufacturerRef == null ? null : (Intake.Account)service.Retrieve(manufacturerRef.LogicalName, manufacturerRef.Id
                , new ColumnSet(nameof(Intake.Account.ipg_ManufacturerIsFacilityAcctRequired).ToLower()
                , nameof(Intake.Account.ipg_ParentAccound).ToLower()
                , nameof(Intake.Account.ipg_manufactureraccountnumber).ToLower()
                , nameof(Intake.Account.Name).ToLower()));
        }

        private bool IsManufacturerNumberValid(IOrganizationService service, EntityReference manufacturerRef, EntityReference facilityRef)
        {
            var crmContext = new OrganizationServiceContext(service);

            Intake.Account manufacturerTarget = GetManufacturer(service, manufacturerRef);
            manufacturerRef.Name = manufacturerTarget.Name;

            Intake.Account manufacturer = manufacturerTarget;

            var AccountNumber = (from facilityManufacturerRelationShip in crmContext.CreateQuery<ipg_facilitymanufacturerrelationship>()
                                 where facilityManufacturerRelationShip.ipg_ManufacturerId.Id == manufacturerRef.Id
                                 && facilityManufacturerRelationShip.ipg_FacilityId.Id == facilityRef.Id
                                 select facilityManufacturerRelationShip.ipg_ManufacturerAccountNumber).FirstOrDefault();

            while (manufacturer?.ipg_ParentAccound != null)
            {
                manufacturer = GetManufacturer(service, manufacturer.ipg_ParentAccound);
            }

            if (manufacturer.ipg_ManufacturerIsFacilityAcctRequired == true && string.IsNullOrEmpty(AccountNumber))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CreateRequiredFacilityNumbTask(IOrganizationService service, EntityReference incidentRef, EntityReference manufacturerRef)
        {
            var crmContext = new OrganizationServiceContext(service);

            var caseManagers = (from team in crmContext.CreateQuery<Team>()
                                where team.Name == "Case Management"
                                select new EntityReference(team.LogicalName, team.Id)).FirstOrDefault();

            var requiredFacilityNumbTask = new Task()
            {
                Subject = $"Required Facility Account # is Missing for {manufacturerRef.Name} Manufacturer",
                RegardingObjectId = incidentRef,
                ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.RequiredFacilityAccount),
                OwnerId = caseManagers,
            };

            if (caseManagers != null)
            {

                var existingrequiredFacilityNumbTask = (from task in crmContext.CreateQuery<Task>()
                                                        where task.RegardingObjectId.Id == incidentRef.Id
                                                        && task.ipg_tasktypecode.Value == (int)ipg_TaskType1.RequiredFacilityAccount
                                                        && task.StateCode.Value == TaskState.Open
                                                        select new Task() { Id = (Guid)task.ActivityId }).FirstOrDefault();

                if (existingrequiredFacilityNumbTask == null)
                {
                    var recordId = service.Create(requiredFacilityNumbTask);
                }
            }
        }
    }
}