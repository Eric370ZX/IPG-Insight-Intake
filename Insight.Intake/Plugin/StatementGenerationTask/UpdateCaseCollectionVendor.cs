using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.StatementGenerationTask
{
    public class UpdateCaseCollectionVendor : PluginBase
    {
        public UpdateCaseCollectionVendor() : base(typeof(UpdateCaseCollectionVendor))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_statementgenerationtask.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);

            var postImage = localPluginContext.PostImage<ipg_statementgenerationtask>();

            var document = postImage.ipg_DocumentId != null
                ? service.Retrieve(ipg_document.EntityLogicalName, postImage.ipg_DocumentId.Id, new ColumnSet(ipg_document.Fields.ipg_documenttypecategoryid)).ToEntity<ipg_document>()
                : null;

            var docCategory = document?.ipg_documenttypecategoryid != null
               ? service.Retrieve(ipg_documentcategorytype.EntityLogicalName, document.ipg_documenttypecategoryid.Id, new ColumnSet(ipg_documentcategorytype.Fields.ipg_name)).ToEntity<ipg_documentcategorytype>()
               : null;

            var incident = postImage.ipg_caseid != null
                ? service.Retrieve(Incident.EntityLogicalName, postImage.ipg_caseid.Id, new ColumnSet (Incident.Fields.ipg_CollectionVendor)).ToEntity<Incident>()
                : null;

            var eventConfiguration = postImage.ipg_eventid != null
                ? service.Retrieve(ipg_statementgenerationeventconfiguration.EntityLogicalName, postImage.ipg_eventid.Id, new ColumnSet(ipg_statementgenerationeventconfiguration.Fields.ipg_name)).ToEntity<ipg_statementgenerationeventconfiguration>()
                : null;

            if (incident != null
                && eventConfiguration != null
                && eventConfiguration.ipg_name != null)
            {
                Incident_ipg_CollectionVendor collectionVendor;
                if (eventConfiguration.ipg_name == "S1 Generated" || eventConfiguration.ipg_name == "S3 Generated")
                {
                    collectionVendor = Incident_ipg_CollectionVendor.SCG;
                }
                else
                {
                    collectionVendor = Incident_ipg_CollectionVendor.IPG;
                }
                UpdateCollectionVendor(incident, service, collectionVendor);
            }
        }

        private void UpdateCollectionVendor(Incident incident, IOrganizationService service, Incident_ipg_CollectionVendor collectionVendor)
        {
            if (incident.ipg_CollectionVendor?.Value != (int)collectionVendor)
            {
                service.Update(new Incident()
                {
                    Id = incident.Id,
                    ipg_CollectionVendor = new OptionSetValue((int)collectionVendor)
                });
            }
        }
    }
}
