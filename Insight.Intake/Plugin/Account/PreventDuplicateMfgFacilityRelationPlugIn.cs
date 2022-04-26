using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Account
{
    public class PreventDuplicateMfgFacilityRelationPlugIn : PluginBase
    {
        public PreventDuplicateMfgFacilityRelationPlugIn() : base(typeof(PreventDuplicateMfgFacilityRelationPlugIn))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, "ipg_facilitymanufacturerrelationship", PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, "ipg_facilitymanufacturerrelationship", PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity facilityManufacturerRelationship = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_facilitymanufacturerrelationship>();

                if (context.MessageName == MessageNames.Create)
                {
                    if (!facilityManufacturerRelationship.Contains("ipg_manufacturerid") || !facilityManufacturerRelationship.Contains("ipg_facilityid"))
                        throw new InvalidPluginExecutionException("Please fill all required fields.");

                    EntityReference manufacturerRef = facilityManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_manufacturerid");

                    EntityReference facilityRef = facilityManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_facilityid");

                    if (DuplicatesFound(service, facilityManufacturerRelationship.Id, manufacturerRef.Id, facilityRef.Id))
                        throw new InvalidPluginExecutionException("Facility/Manufacturer relationship already exists.");
                }

                else
                {
                    Entity retrievedFacilityManufacturerRelationship = service.Retrieve(ipg_facilitymanufacturerrelationship.EntityLogicalName, facilityManufacturerRelationship.Id, new ColumnSet(nameof(ipg_facilitymanufacturerrelationship.ipg_FacilityId).ToLower(),
                                                                nameof(ipg_facilitymanufacturerrelationship.ipg_ManufacturerId).ToLower())).ToEntity<ipg_facilitymanufacturerrelationship>();

                    EntityReference manufacturerRef = facilityManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_manufacturerid") ?? retrievedFacilityManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_manufacturerid");

                    EntityReference facilityRef = facilityManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_facilityid") ?? retrievedFacilityManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_facilityid");

                    if (DuplicatesFound(service, facilityManufacturerRelationship.Id, manufacturerRef.Id, facilityRef.Id))
                        throw new InvalidPluginExecutionException("Facility/Manufacturer relationship already exists.");
                }

            }
        }


        private bool DuplicatesFound(IOrganizationService service, Guid relationshipId, Guid manufacturerId, Guid facilityId)
        {
            QueryExpression query = new QueryExpression("ipg_facilitymanufacturerrelationship")

                {
                    ColumnSet = new ColumnSet(false)
                };

            query.Criteria.AddCondition("ipg_facilitymanufacturerrelationshipid", ConditionOperator.NotEqual, relationshipId);
            query.Criteria.AddCondition("ipg_manufacturerid", ConditionOperator.Equal, manufacturerId);
            query.Criteria.AddCondition("ipg_facilityid", ConditionOperator.Equal, facilityId);

            return service.RetrieveMultiple(query).Entities.Count > 0 ? true : false;
        }


    }
}
