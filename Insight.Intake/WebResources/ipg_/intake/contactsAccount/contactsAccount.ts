/**
 * @namespace Intake.ContactsAccount
 */
namespace Intake.ContactsAccount {
  /**
   * Called on load form
   * @function Intake.ContactsAccount.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    setupVisibilityOfFieldsByContactRole(formContext);
  }

  /**
   * Called on load quick create form
   * @function Intake.ContactsAccount.OnLoadQuickCreateForm
   * @returns {void}
  */
  export function OnLoadQuickCreateForm(executionContext: Xrm.Events.EventContext) {
    const formContext: Xrm.FormContext = executionContext.getFormContext();
    setupVisibilityOfFieldsByContactRole(formContext);
    stVisibilityByRelation(formContext);
    restrictExistingRelations(formContext);    
  }
  function restrictExistingRelations(formContext: Xrm.FormContext) {
    const accountAttr = formContext.getAttribute("ipg_accountid");
    const contactAttr = formContext.getAttribute("ipg_contactid");

    const recordAlreadyExistsMessage = (entityName) => `This ${entityName} has already been assosiated with the selected ${entityName == "Contact" ? "Facility" : "Contact"}`;

    accountAttr.addOnChange(async () => {
      if (await relationExists(formContext)) {
        accountAttr.setValue(null);
        Xrm.Navigation.openAlertDialog({ text: recordAlreadyExistsMessage("Facility"), title: "Warning" });
      }
    })
    contactAttr.addOnChange(async () => {
      if (await relationExists(formContext)) {
        contactAttr.setValue(null);
        Xrm.Navigation.openAlertDialog({ text: recordAlreadyExistsMessage("Contact"), title: "Warning" });
      }
    })
  }
  async function relationExists(formContext: Xrm.FormContext) {
    const accountId = formContext.getAttribute("ipg_accountid")?.getValue();
    const contactId = formContext.getAttribute("ipg_contactid")?.getValue();

    return accountId && contactId
      ? (await Xrm.WebApi.retrieveMultipleRecords(
        "ipg_contactsaccounts"
        , `?$filter=_ipg_contactid_value eq ${Intake.Utility.GetFormattedId(contactId[0]?.id)} and _ipg_accountid_value eq ${Intake.Utility.GetFormattedId(accountId[0]?.id)}`))
        .entities
        .length > 0
      : false
  }
  /**
   * Called on load quick create/edit form
   * @function Intake.ContactsAccount.OnLoadUpsertForm
   * @returns {void}
  */
  export function OnLoadUpsertForm(executionContext: Xrm.Events.EventContext) {
    const formContext = executionContext.getFormContext();
    OnCommunicationPreferencesChange(formContext);
    formContext.getAttribute('ipg_communicationpreference').addOnChange(() => {
      OnCommunicationPreferencesChange(formContext);
    });    

    setLockedPrimaryContact(formContext);
  }

  /**
   * Called on Communication Preferences change in quick create/edit form
   * @function Intake.ContactsAccount.OnCommunicationPreferencesChange
   * @returns {void}
  */
  function OnCommunicationPreferencesChange(formContext: Xrm.FormContext) {
    const preference = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_communicationpreference");       
    const email = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_email");  
    const mainPhone = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_mainphone"); 
    if(preference.getValue() == 1) { // Email      
      email?.setRequiredLevel("required");   
      mainPhone?.setRequiredLevel("none")   
    } 
    if(preference.getValue() == 2) { // Main Phone
      mainPhone?.setRequiredLevel("required");
      email?.setRequiredLevel("none")      
    } 
    if(preference.getValue() == 3) { // Portal Comment
      email?.setRequiredLevel("none") 
      mainPhone?.setRequiredLevel("none") 
    }
  }

  /**
   * Set Locked Facility Primary Contact checkbox
   * @function Intake.ContactsAccount.setLockedPrimaryContact
   * @returns {void}
  */
   function setLockedPrimaryContact(formContext) {
    var isprimaryContact = formContext.getAttribute("ipg_isprimarycontact");
    if (isprimaryContact.getValue() == true) {   
        formContext.getControl("ipg_isprimarycontact").setDisabled(true);
    }
    else {
        formContext.getControl("ipg_isprimarycontact").setDisabled(false);
    }
  }

  /**
   * Called on change contact role
   * @function Intake.ContactsAccount.OnChangeContactRole
   * @returns {void}
  */
  export function OnChangeContactRole(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    setupVisibilityOfFieldsByContactRole(formContext);
  }

  /**
   * set visibility of fields by contact role
   * @function Intake.ContactsAccount.setupVisibilityOfFieldsByContactRole
   * @returns {void}
  */
  function setupVisibilityOfFieldsByContactRole(formContext: Xrm.FormContext) {
    let contactType = formContext.getAttribute("ipg_contactrolecode").getValue();
    if (contactType && contactType.indexOf(923720002) != -1) {
      formContext.getControl("ipg_rboname")?.setVisible(true);
    }
    else if (contactType && contactType.indexOf(923720006) != -1) {
      formContext.getControl("ipg_vendorid")?.setVisible(true);
      formContext.getControl("ipg_daystopay")?.setVisible(true);
      formContext.getAttribute("ipg_vendorid")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_daystopay")?.setRequiredLevel("required");
    }
    else {
      formContext.getControl("ipg_rboname")?.setVisible(false);
      formContext.getControl("ipg_vendorid")?.setVisible(false);
      formContext.getControl("ipg_daystopay")?.setVisible(false);
      formContext.getAttribute("ipg_vendorid")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_daystopay")?.setRequiredLevel("none");
    }
  }
  function stVisibilityByRelation(formContext: Xrm.FormContext) {
    if (formContext.getAttribute("ipg_contactid").getValue()) {
      formContext.getControl("ipg_contactid").setVisible(false);
    } else {
      formContext.getControl("ipg_accountid").setVisible(false);
    }
  }
}