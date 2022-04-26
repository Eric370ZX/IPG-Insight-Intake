using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using Insight.Intake.Extensions;

namespace Insight.Intake.Plugin.AssociatedCpt
{
    public class PopulatePrimaryAttributePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    var associatedCpt = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_associatedcpt>();

                    if (associatedCpt.LogicalName != ipg_associatedcpt.EntityLogicalName)
                    {
                        return;
                    }

                    if (context.MessageName == "Create" || context.MessageName == "Update")
                    {
                        Intake.Account relatedFacility = null;

                        Intake.Account relatedCarrier = null;
                        
                        ipg_cptcode relatedCptCode = null;

                        if (context.MessageName == "Create")
                        {
                            /*if (associatedCpt.ipg_FacilityId != null)
                            {
                                relatedFacility = organizationService.Retrieve(Intake.Account.EntityLogicalName, associatedCpt.ipg_FacilityId.Id, new ColumnSet("name")).ToEntity<Intake.Account>();
                            }*/

                            if (associatedCpt.ipg_CarrierId != null)
                            {
                                relatedCarrier = organizationService.Retrieve(Intake.Account.EntityLogicalName, associatedCpt.ipg_CarrierId.Id, new ColumnSet("name")).ToEntity<Intake.Account>();
                            }
                            
                            relatedCptCode = organizationService.Retrieve(ipg_cptcode.EntityLogicalName, associatedCpt.ipg_CPTCodeId.Id, new ColumnSet("ipg_name")).ToEntity<ipg_cptcode>();
                        }
                        else if (context.MessageName == "Update")
                        {
                            if (!context.PreEntityImages.Contains("Image") || !(context.PreEntityImages["Image"] is Entity))
                            {
                                tracingService.Trace("{0}: Pre-Image does not exist.", nameof(PopulatePrimaryAttributePlugin));
                                return;
                            }

                            var associatedCptPreImage = (context.PreEntityImages["Image"]).ToEntity<ipg_associatedcpt>();

                            var updatedAssociatedCpt = associatedCpt.MergeWithImage(associatedCptPreImage);

                            /*if (updatedAssociatedCpt.ipg_FacilityId != null)
                            {
                                relatedFacility = organizationService.Retrieve(Intake.Account.EntityLogicalName, updatedAssociatedCpt.ipg_FacilityId.Id, new ColumnSet("name")).ToEntity<Intake.Account>();
                            }*/

                            if (updatedAssociatedCpt.ipg_CarrierId != null)
                            {
                                relatedCarrier = organizationService.Retrieve(Intake.Account.EntityLogicalName, updatedAssociatedCpt.ipg_CarrierId.Id, new ColumnSet("name")).ToEntity<Intake.Account>();
                            }
                            
                            relatedCptCode = organizationService.Retrieve(ipg_cptcode.EntityLogicalName, updatedAssociatedCpt.ipg_CPTCodeId.Id, new ColumnSet("ipg_name")).ToEntity<ipg_cptcode>();
                        }

                        var nameMap = new List<Entity>
                        {
                            relatedFacility,
                            relatedCarrier,
                            relatedCptCode
                        };

                        associatedCpt.ipg_name = String.Join(" - ", nameMap.Where(x => x != null).Select(x => x is Intake.Account ? ((Intake.Account)x).Name : x is ipg_cptcode ? ((ipg_cptcode)x).ipg_name : ""));

                        context.InputParameters["Target"] = associatedCpt.ToEntity<Entity>();
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(PopulatePrimaryAttributePlugin)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace("{0}: {1}", nameof(PopulatePrimaryAttributePlugin), exception.ToString());
                throw;
            }
        }
    }
}
