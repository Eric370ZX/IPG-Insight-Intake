using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using static Insight.Intake.Helpers.Constants;
using System.Text;
using System.Runtime.Serialization.Json;
using Insight.Intake.Models;
using System.IO;
using Microsoft.Xrm.Sdk.Messages;
using System;

namespace Insight.Intake.Plugin.Managers
{
    public class PortalContactManager
    {
        private IOrganizationService _service;
        private ITracingService _tracing;
        public PortalContactManager(IOrganizationService service, ITracingService tracing)
        {
            _service = service;
            _tracing = tracing;
        }

        public List<Intake.Account> GetActiveFacilitiesNotConnectedWithContact(EntityReference contactREf)
        {
            _tracing.Trace($"{nameof(GetActiveFacilitiesNotConnectedWithContact)} Started");

            var facilities = _service.RetrieveMultiple(new QueryExpression(Intake.Account.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression
                {
                    Conditions = {
                        new ConditionExpression(Intake.Account.Fields.CustomerTypeCode, ConditionOperator.Equal, (int)Account_CustomerTypeCode.Facility),
                        new ConditionExpression(Intake.Account.Fields.StateCode, ConditionOperator.Equal, (int)AccountState.Active),
                        new ConditionExpression(Intake.Account.Fields.ipg_active, ConditionOperator.Equal, true),
                        new ConditionExpression(ipg_contactsaccounts.EntityLogicalName, ipg_contactsaccounts.PrimaryIdAttribute, ConditionOperator.Null)
                    }
                },
                LinkEntities = {
                new LinkEntity(Intake.Account.EntityLogicalName,
                ipg_contactsaccounts.EntityLogicalName,
                Intake.Account.PrimaryIdAttribute, ipg_contactsaccounts.Fields.ipg_accountid, JoinOperator.LeftOuter)
                {
                    EntityAlias = ipg_contactsaccounts.EntityLogicalName,
                    LinkCriteria = new FilterExpression()
                    {
                        Conditions = {new ConditionExpression(ipg_contactsaccounts.Fields.ipg_contactid, ConditionOperator.Equal, contactREf.Id) }
                    }
                }
                }
            }).Entities.Select(f => f.ToEntity<Intake.Account>()).ToList();

            _tracing.Trace($"{nameof(GetActiveFacilitiesNotConnectedWithContact)} Done");

            return facilities;
        }

        public void Impersonate(EntityReference fromContact, EntityReference toContact)
        {
            var newRelationShip = GetContactAccountsByContact(fromContact);

            DeleteAllContactAccounts(toContact);

            foreach (var item in newRelationShip)
            {
                _service.Create(new ipg_contactsaccounts() { ipg_contactid = toContact, ipg_accountid = item.ipg_accountid, ipg_contactrolecodeEnum = item.ipg_contactrolecodeEnum });
            }

            var webRoles = GetWebRoles(fromContact).Select(x => x.Id);

            DisassociateContactWithOldWebRoles(toContact);
            AssociateContactWithNewWebRoles(toContact, webRoles);
        }

        public void RestoreFacilityRelationShipAndWebRoles(Intake.Contact contact)
        {
            PortalContactBackUpJson backup = null;
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(contact.ipg_backupjson)))
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(PortalContactBackUpJson));
                backup = (PortalContactBackUpJson)deserializer.ReadObject(ms);
            }

            DeleteAllContactAccounts(contact.ToEntityReference());
            CreateContactAccountsFromBackUp(contact, backup.facilities);

            DisassociateContactWithOldWebRoles(contact.ToEntityReference());
            AssociateContactWithNewWebRoles(contact.ToEntityReference(), backup.WebRoles, true);

            _service.Update(new Intake.Contact() { Id = contact.Id, ipg_backupjson = null });
        }

        private void CreateContactAccountsFromBackUp(Intake.Contact contact, List<ContactAccountInfo> facilities)
        {
            var accountsInSystem = _service.RetrieveMultiple(new QueryExpression(Intake.Account.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Intake.Account.PrimaryIdAttribute)
                ,
                Criteria = new FilterExpression()
                {
                    Conditions = { new ConditionExpression(Intake.Account.PrimaryIdAttribute, ConditionOperator.In, facilities.Select(f => f.id).ToArray()) }
                }
            }).Entities.Join(facilities, f => f.Id, f => f.id, (f1, f2) => f2);

            foreach (var item in accountsInSystem)
            {
                CreateAdminContactRelationShipWithFacility(contact.ToEntityReference(), new EntityReference(Intake.Account.EntityLogicalName, item.id), item.Roles);
            }
        }

        private void DeleteAllContactAccounts(EntityReference contact)
        {
            var contactAccounts = GetContactAccounts(contact);

            foreach (var item in contactAccounts)
            {
                _service.Delete(item.LogicalName, item.Id);
            }
        }

        private void AssociateContactWithNewWebRoles(EntityReference contact, IEnumerable<Guid> webRoles,bool checkWebRolesInSystem = false)
        {
            var webRolesInSystem = checkWebRolesInSystem ? _service.RetrieveMultiple(new QueryExpression(Adx_webrole.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Adx_webrole.PrimaryIdAttribute),
                Criteria = new FilterExpression()
                {
                    Conditions = { new ConditionExpression(Adx_webrole.PrimaryIdAttribute, ConditionOperator.In, webRoles.ToArray()) }
                }
            }).Entities.Select(x => x.ToEntityReference()) : webRoles.Select(x=> new EntityReference(Adx_webrole.EntityLogicalName, x));

            AssociateRequest AssociateRequest = new AssociateRequest();
            AssociateRequest.Relationship = new Relationship(adx_webrole_contact.EntityLogicalName);
            AssociateRequest.Target = contact;

            AssociateRequest.RelatedEntities = new EntityReferenceCollection(webRolesInSystem.ToList());
            _service.Execute(AssociateRequest);
        }

        public void DisassociateContactWithOldWebRoles(EntityReference contact)
        {
            DisassociateRequest disassociateRequest = new DisassociateRequest();
            disassociateRequest.Relationship = new Relationship(adx_webrole_contact.EntityLogicalName);
            disassociateRequest.Target = contact;

            var contactWebRoles = GetWebRoles(contact);

            disassociateRequest.RelatedEntities = new EntityReferenceCollection(contactWebRoles);
            _service.Execute(disassociateRequest);
        }

        public List<ipg_contactsaccounts> GetContactAccountsByContact(EntityReference contact)
        {
            return _service.RetrieveMultiple(new QueryExpression(ipg_contactsaccounts.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_contactsaccounts.Fields.ipg_accountid, ipg_contactsaccounts.Fields.ipg_contactrolecode),
                Criteria = new FilterExpression()
                {
                    Conditions = { new ConditionExpression(ipg_contactsaccounts.Fields.ipg_contactid, ConditionOperator.Equal, contact.Id) }
                }
            }).Entities.Select(x => x.ToEntity<ipg_contactsaccounts>()).ToList();
        }

        public void BackUpFacilityRelationShipAndWebRoles(EntityReference contact)
        {
            List<ipg_contactsaccounts> contactAccounts = GetContactAccounts(contact);
            List<Guid> webRolesIds = GetWebRoles(contact).Select(x=>x.Id).ToList();

            var backup = new PortalContactBackUpJson() {
                facilities = new List<ContactAccountInfo>(),
                WebRoles = webRolesIds
            };

            foreach (var item in contactAccounts)
            {
                backup.facilities.Add(new ContactAccountInfo() { id = item.ipg_accountid.Id, Roles = item.ipg_contactrolecodeEnum.ToList() });
            }

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(PortalContactBackUpJson));
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, backup);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj);
            string json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();

            _service.Update(new Intake.Contact() { Id = contact.Id, ipg_backupjson = json });
        }

        public  List<ipg_contactsaccounts> GetContactAccounts(EntityReference sysuserPortalContact)
        {
           return _service.RetrieveMultiple(new QueryExpression(ipg_contactsaccounts.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_contactsaccounts.Fields.ipg_accountid, ipg_contactsaccounts.Fields.ipg_contactrolecode),
                Criteria = new FilterExpression()
                {
                    Conditions = { new ConditionExpression(ipg_contactsaccounts.Fields.ipg_contactid, ConditionOperator.Equal, sysuserPortalContact.Id),
                    new ConditionExpression(ipg_contactsaccounts.Fields.ipg_accountid, ConditionOperator.NotNull)}
                }
            }).Entities.Select(e => e.ToEntity<ipg_contactsaccounts>()).ToList();
        }

        public List<EntityReference> GetWebRoles(EntityReference contact)
        {
            return _service.RetrieveMultiple(new QueryExpression(adx_webrole_contact.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(adx_webrole_contact.Fields.adx_webroleid),
                Criteria = new FilterExpression()
                {
                    Conditions = { new ConditionExpression(adx_webrole_contact.Fields.contactid, ConditionOperator.Equal, contact.Id) }
                }
            }).Entities.Select(e => new EntityReference(Adx_webrole.EntityLogicalName, e.ToEntity<adx_webrole_contact>().adx_webroleid.Value)).ToList();
        }


        public bool IsActiveFacility(Intake.Account account)
        {
            return account.CustomerTypeCodeEnum == Account_CustomerTypeCode.Facility
                 && account.StateCode == AccountState.Active
                 && account.ipg_active == true;
        }

        public bool IsSysAdmineRole(List<EntityReference> roles)
        {
            _tracing.Trace($"{nameof(IsSysAdmineRole)} Started");
            return _service.RetrieveMultiple(new QueryExpression(Role.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions = { new ConditionExpression(Role.PrimaryNameAttribute, ConditionOperator.Equal, SecurityRoleNames.SYS_ADMIN) }
                }
            }).Entities.Any();
        }

        public List<Intake.Account> GetActiveFacilities(IOrganizationService _service)
        {
            _tracing.Trace($"{nameof(GetActiveFacilities)} Started");

            var facilities = _service.RetrieveMultiple(new QueryExpression(Intake.Account.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression
                {
                    Conditions = {
                        new ConditionExpression(Intake.Account.Fields.CustomerTypeCode, ConditionOperator.Equal, (int)Account_CustomerTypeCode.Facility),
                        new ConditionExpression(Intake.Account.Fields.StateCode, ConditionOperator.Equal, (int)AccountState.Active),
                        new ConditionExpression(Intake.Account.Fields.ipg_active, ConditionOperator.Equal, true)
                    }
                }
            }).Entities.Select(f => f.ToEntity<Intake.Account>()).ToList();

            _tracing.Trace($"{nameof(GetActiveFacilities)} Done");

            return facilities;
        }
        public Intake.Contact GetContactFromSysAdminUser(SystemUser SysUser)
        {
            _tracing.Trace($"{nameof(GetContactFromSysAdminUser)} Started");

            var contact = _service.RetrieveMultiple(new QueryExpression(Intake.Contact.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                TopCount = 1,
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Intake.Contact.Fields.adx_identity_securitystamp, ConditionOperator.NotNull),
                        new ConditionExpression(Intake.Contact.Fields.EMailAddress1, ConditionOperator.Equal, SysUser.InternalEMailAddress)
                    }
                }
            }).Entities.Select(cont => cont.ToEntity<Intake.Contact>()).FirstOrDefault();

            _tracing.Trace($"{nameof(GetContactFromSysAdminUser)} Done");

            return contact;
        }

        public List<Intake.Contact> GetPortalContactsFromSysAdminUsers()
        {
            _tracing.Trace($"{nameof(GetPortalContactsFromSysAdminUsers)} Started");

            var adminSysUsers = _service.RetrieveMultiple(new QueryExpression(SystemUser.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(SystemUser.Fields.InternalEMailAddress),
                Criteria = new FilterExpression()
                {
                    Conditions = { new ConditionExpression(SystemUser.Fields.InternalEMailAddress, ConditionOperator.NotNull)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(SystemUser.EntityLogicalName, SystemUserRoles.EntityLogicalName, SystemUser.PrimaryIdAttribute, SystemUserRoles.Fields.SystemUserId, JoinOperator.Inner)
                    {
                        LinkEntities =
                        {
                            new LinkEntity(SystemUserRoles.EntityLogicalName, Role.EntityLogicalName,  SystemUserRoles.Fields.RoleId, Role.PrimaryIdAttribute, JoinOperator.Inner)
                            {
                                LinkCriteria = new FilterExpression()
                                {
                                    Conditions = {new ConditionExpression(Role.PrimaryNameAttribute, ConditionOperator.Equal, SecurityRoleNames.SYS_ADMIN) }
                                }
                            }
                        }
                    }
                }
            }).Entities.Select(c => c.ToEntity<SystemUser>());

            var contacts = _service.RetrieveMultiple(new QueryExpression(Intake.Contact.EntityLogicalName)
            {   ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Intake.Contact.Fields.adx_identity_securitystamp, ConditionOperator.NotNull),
                        new ConditionExpression(Intake.Contact.Fields.EMailAddress1, ConditionOperator.In, adminSysUsers.Select(su =>su.InternalEMailAddress).ToArray())
                    }
                }
            }).Entities.Select(cont => cont.ToEntity<Intake.Contact>()).ToList();

            _tracing.Trace($"{nameof(GetPortalContactsFromSysAdminUsers)} Done");

            return contacts;
        }

        public void CreateAdminContactRelationShipWithFacility(EntityReference contact, EntityReference facility, List<ipg_contactsaccounts_ipg_ContactRoleCode> roles = null)
        {
            _tracing.Trace($"{nameof(CreateAdminContactRelationShipWithFacility)} Started");

            roles = roles ?? new List<ipg_contactsaccounts_ipg_ContactRoleCode>() { ipg_contactsaccounts_ipg_ContactRoleCode.Administrator };

            _service.Create(new ipg_contactsaccounts()
            {
                ipg_accountid = facility,
                ipg_contactid = contact,
                ipg_contactrolecodeEnum = new List<ipg_contactsaccounts_ipg_ContactRoleCode>() { ipg_contactsaccounts_ipg_ContactRoleCode.Administrator }
            });

            _tracing.Trace($"{nameof(CreateAdminContactRelationShipWithFacility)} Done");
        }

        public bool IsSysUserHaveAdminRole(SystemUser SysUser)
        {
            _tracing.Trace($"{nameof(IsSysUserHaveAdminRole)} Started");

            return SysUser.GetAttributeValue<AliasedValue>($"{SystemUserRoles.EntityLogicalName}.{SystemUserRoles.PrimaryIdAttribute}") != null;
        }

        public SystemUser GetSysUserWithAdminSecurityRole(EntityReference SysUser)
        {
            _tracing.Trace($"{nameof(IsSysUserHaveAdminRole)} Started");

            return _service.RetrieveMultiple(new QueryExpression(SystemUser.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(SystemUser.Fields.InternalEMailAddress),
                Criteria = new FilterExpression()
                {
                    Conditions = {new ConditionExpression(SystemUser.PrimaryIdAttribute, ConditionOperator.Equal, SysUser.Id) }
                },
                LinkEntities =
                {
                    new LinkEntity(SystemUser.EntityLogicalName, SystemUserRoles.EntityLogicalName, SystemUser.PrimaryIdAttribute, SystemUserRoles.Fields.SystemUserId, JoinOperator.LeftOuter)
                    {
                        EntityAlias = SystemUserRoles.EntityLogicalName,
                        Columns = new ColumnSet(SystemUserRoles.PrimaryIdAttribute),
                        LinkEntities =
                        {
                            new LinkEntity(SystemUserRoles.EntityLogicalName, Role.EntityLogicalName,SystemUserRoles.Fields.RoleId, Role.PrimaryIdAttribute, JoinOperator.LeftOuter)
                            {
                                LinkCriteria = new FilterExpression()
                                {
                                    Conditions = {new ConditionExpression(Role.PrimaryNameAttribute, ConditionOperator.Equal, SecurityRoleNames.SYS_ADMIN) }
                                }
                            }
                        }
                    }
                }
            }).Entities.Select(c => c.ToEntity<SystemUser>()).FirstOrDefault();
        }

    }
}
