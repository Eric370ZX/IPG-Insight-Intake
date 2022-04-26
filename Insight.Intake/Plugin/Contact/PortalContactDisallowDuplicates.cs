using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Contact
{
    public class PortalContactDisallowDuplicates : PluginBase
    {
        public PortalContactDisallowDuplicates() : base(typeof(PortalContactDisallowDuplicates))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Intake.Contact.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Intake.Contact.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localContext)
        {
            var contactEntity = localContext.Target<Intake.Contact>();

            if (IsUsernameAlreadyExist(localContext, contactEntity))
            {
                throw new InvalidPluginExecutionException("Contact with such 'Username' already exists");
            }
        }
        private bool IsUsernameAlreadyExist(LocalPluginContext localContext, Intake.Contact contactEntity)
        {
            if(string.IsNullOrWhiteSpace(contactEntity.adx_identity_username))
            {
                return false;
            }
            
            using (var orgContext = new OrganizationServiceContext(localContext.OrganizationService))
            {
                var existingContacts = (from c in orgContext.CreateQuery<Intake.Contact>()
                                       where c.adx_identity_username == contactEntity.adx_identity_username
                                       select new { c.Id }).Take(1).ToList();

                return existingContacts.Any();
            }
        }
    }
}
