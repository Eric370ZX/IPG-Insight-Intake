using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Account
{
    public class SetDocumentAssociationDate : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                var organizationService = serviceProvider.GetOrganizationService(context.UserId);

                if (context.InputParameters.Contains("Target") == false)
                {
                    return;
                }

                string relationshipName = string.Empty;

                EntityReferenceCollection relatedEntities = null;

                EntityReference relatedEntity = null;

                if (context.MessageName == "Associate")
                {
                    if (context.InputParameters.Contains("Relationship"))
                    {
                        relationshipName = context.InputParameters["Relationship"].ToString();
                    }

                    if (relationshipName != "ipg_ipg_document_account.Referenced")
                    {
                        return;
                    }

                    if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                    {
                        relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;

                        relatedEntity = relatedEntities[0];

                        Entity targetDocument = new Entity(relatedEntity.LogicalName);
                        targetDocument.Id = relatedEntity.Id;
                        targetDocument["ipg_dateaddedtofacility"] = DateTime.Now.Date;
                        organizationService.Update(targetDocument);

                    }
                }
            }

            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {this.GetType().Name}.", faultException);
            }
        }
    }
}

