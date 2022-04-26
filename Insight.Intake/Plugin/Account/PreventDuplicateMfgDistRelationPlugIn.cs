using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Account
{
    public class PreventDuplicateMfgDistRelationPlugIn : PluginBase
    {
        public PreventDuplicateMfgDistRelationPlugIn() : base(typeof(PreventDuplicateMfgDistRelationPlugIn))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, "ipg_distributor_manufacturer_relationship", PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, "ipg_distributor_manufacturer_relationship", PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity distManufacturerRelationship = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_distributor_manufacturer_relationship>();

                if (context.MessageName == MessageNames.Create)
                {
                    if (!distManufacturerRelationship.Contains("ipg_manufacturerid") || !distManufacturerRelationship.Contains("ipg_distributorid"))
                        throw new InvalidPluginExecutionException("Please fill all required fields.");

                    EntityReference manufacturerRef = distManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_manufacturerid");

                    EntityReference distRef = distManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_distributorid");

                    if (DuplicatesFound(service, distManufacturerRelationship.Id, manufacturerRef.Id, distRef.Id))
                        throw new InvalidPluginExecutionException("Distributor/Manufacturer relationship already exists.");
                }

                else
                {
                    Entity retrievedDistManufacturerRelationship = service.Retrieve(ipg_distributor_manufacturer_relationship.EntityLogicalName, distManufacturerRelationship.Id, new ColumnSet(nameof(ipg_distributor_manufacturer_relationship.ipg_DistributorId).ToLower(),
                                                                nameof(ipg_distributor_manufacturer_relationship.ipg_ManufacturerId).ToLower())).ToEntity<ipg_distributor_manufacturer_relationship>();

                    EntityReference manufacturerRef = distManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_manufacturerid") ?? retrievedDistManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_manufacturerid");

                    EntityReference distRef = distManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_distributorid") ?? retrievedDistManufacturerRelationship.GetAttributeValue<EntityReference>("ipg_distributorid");

                    if (DuplicatesFound(service, distManufacturerRelationship.Id, manufacturerRef.Id, distRef.Id))
                        throw new InvalidPluginExecutionException("Distributor/Manufacturer relationship already exists.");
                }
               
            }
        }

        private bool DuplicatesFound(IOrganizationService service, Guid relationshipId, Guid manufacturerId, Guid distId)
        {
            QueryExpression query = new QueryExpression("ipg_distributor_manufacturer_relationship")

            {
                ColumnSet = new ColumnSet(false)
            };

            query.Criteria.AddCondition("ipg_distributor_manufacturer_relationshipid", ConditionOperator.NotEqual, relationshipId);
            query.Criteria.AddCondition("ipg_manufacturerid", ConditionOperator.Equal, manufacturerId);
            query.Criteria.AddCondition("ipg_distributorid", ConditionOperator.Equal, distId);

            return service.RetrieveMultiple(query).Entities.Count > 0 ? true : false;
        }

    }
}
