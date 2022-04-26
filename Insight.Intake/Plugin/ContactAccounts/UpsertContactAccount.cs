using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.ContactAccount
{
    public class UpsertContactAccount : PluginBase
    {
        public UpsertContactAccount() : base(typeof(UpsertContactAccount))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_contactsaccounts.EntityLogicalName, PostOperationHeandler);
        }

        private void PostOperationHeandler(LocalPluginContext pluginContext)
        {
            pluginContext.Trace($"Start Upserting");
            UpsertContact(pluginContext);
        }
        private void UpsertContact(LocalPluginContext context)
        {
            var contactAccount = context.Target<ipg_contactsaccounts>();
            if (contactAccount.ipg_contactname == null)
                return;
            if (contactAccount.ipg_contactid?.Id == null)
            {
                InsertContact(context, contactAccount);
            }
            else UpdateContact(context, contactAccount);

            UpdatePrimaryContact(contactAccount, context.OrganizationService, context.TracingService);
        }
        private void InsertContact(LocalPluginContext context, ipg_contactsaccounts targetContactsAccount)
        {
            context.Trace("Start inserting new contact ...");
            var contactRef = new EntityReference
            (
                Intake.Contact.EntityLogicalName,
                context.OrganizationService.Create(MapContact(targetContactsAccount, context.TracingService))
            );
            context.OrganizationService.Update(new ipg_contactsaccounts() { Id= targetContactsAccount.Id, ipg_contactid = contactRef });
            context.Trace($"New contact added successfuly, Id:{contactRef.Id}");
        }
        private void UpdateContact(LocalPluginContext context, ipg_contactsaccounts targetContactAccount)
        {
            context.Trace($"Start Updating existing contact with Id: {targetContactAccount.ipg_contactid.Id}");
            var contactToUpdate = MapContact(targetContactAccount, context.TracingService);
            context.OrganizationService.Update(contactToUpdate);
            context.Trace($"Successfuly Updated");

            var contactRef = new EntityReference(Intake.Contact.EntityLogicalName, contactToUpdate.Id);
            DeleteExistingContactsAccountsByContactId(targetContactAccount, context);
            targetContactAccount.ipg_contactid = contactRef; // should use service.Update() ?   
        }
        private void DeleteExistingContactsAccountsByContactId(ipg_contactsaccounts targetContactAccount, LocalPluginContext context)
        {
            // should be oly one record to delete, but if there are duplicates they will be deleted too
            context.Trace("Start deleting existing contacts Account");
            new OrganizationServiceContext(context.OrganizationService).CreateQuery<ipg_contactsaccounts>()
                 .Where(contactsAccount => contactsAccount.ipg_contactid.Id == targetContactAccount.ipg_contactid.Id
                     && contactsAccount.Id != targetContactAccount.Id
                     && contactsAccount.ipg_accountid.Id == targetContactAccount.ipg_accountid.Id)
                 .ToList()
                 .ForEach(contactsAccount => context.OrganizationService.Delete(ipg_contactsaccounts.EntityLogicalName, contactsAccount.Id));
            context.Trace("Deleting existing contacts accounts finished successfuly");
        }
        private Intake.Contact MapContact(ipg_contactsaccounts contactsAccount, ITracingService tracingService)
        {
            tracingService.Trace("Start mapping contact");
            var mappedContact = new Intake.Contact()
            {
                Telephone1 = contactsAccount?.ipg_mainphone,
                Telephone2 = contactsAccount?.ipg_otherphone,
                EMailAddress1 = contactsAccount?.ipg_email,
                Fax = contactsAccount?.ipg_fax,
                FirstName = contactsAccount?.ipg_contactname.Split(' ').FirstOrDefault(),
                LastName = contactsAccount?.ipg_contactname.Split(' ').LastOrDefault()
            };
            if (contactsAccount.ipg_contactid != null) { mappedContact.Id = contactsAccount.ipg_contactid.Id; }

            tracingService.Trace("Contact mapped");
            return mappedContact;
        }
        private void UpdatePrimaryContact(ipg_contactsaccounts targetContactsAccount, IOrganizationService service, ITracingService tracingService)
        {
            tracingService.Trace($"Checking is record is Primary contact");
            if (targetContactsAccount.ipg_IsPrimaryContact.HasValue && targetContactsAccount.ipg_IsPrimaryContact.Value)
            {
                tracingService.Trace($"Record is Primary contact");
                tracingService.Trace($"Setting other records as non-pimary ...");
                new OrganizationServiceContext(service).CreateQuery<ipg_contactsaccounts>()
                    .Where(contactsAccount => contactsAccount.ipg_accountid.Id == targetContactsAccount.ipg_accountid.Id
                        && contactsAccount.ipg_IsPrimaryContact != null && contactsAccount.ipg_IsPrimaryContact.Value
                        && contactsAccount.Id != targetContactsAccount.Id)
                    .ToList()
                    .ForEach(contactsAccount =>
                    {
                        service.Update(new ipg_contactsaccounts
                        {
                            Id = contactsAccount.Id,
                            ipg_IsPrimaryContact = false
                        });
                    });
                tracingService.Trace($"Setting other records as non-pimary completed successfuly");
            }
            else { tracingService.Trace($"Record is NOT Primary contact"); }
        }
    }
}
