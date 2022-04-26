/**
 * @namespace Intake.ContactsAccount.Ribbon
 */
namespace Intake.ContactsAccount.Ribbon {
  /**
   * Called on New button click on subgrid dropdown
   * @function Intake.ContactsAccount.Ribbon.OnNewClick
   * @returns {void}
   */
  export function OnNewClick(primaryControl, contactRole) {
    let formContext: Xrm.FormContext = primaryControl;
    let entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_contactsaccounts";
    entityFormOptions["useQuickCreateForm"] = true;

    let formParameters = {};
    if (formContext.data.entity.getEntityName() == "account") {
      formParameters["ipg_accountid"] = formContext.data.entity.getId();
      formParameters["ipg_accountidname"] =
        formContext.getAttribute("name") &&
        formContext.getAttribute("name").getValue();
    } else if (formContext.data.entity.getEntityName() == "contact") {
      formParameters["ipg_contactid"] = formContext.data.entity.getId();
      formParameters["ipg_contactidname"] =
        formContext.getAttribute("fullname") &&
        formContext.getAttribute("fullname").getValue();
    }

    formParameters["ipg_contactrolecode"] = `[${contactRole}]`;

    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
      function (success) {
        console.log(success);
      },
      function (error) {
        console.log(error);
      }
    );
  }
  /**
   * Opens Contacts Account Quick Form
   * @function Intake.ContactsAccount.Ribbon.UpsertContactAccount
   * @returns {void}
   */
  export async function UpsertContactAccount(
    primaryControl,
    selectedRecordId?
  ) {
    // add populating of client id if selected record (edit mode)
    let entityFormOptions: Xrm.Navigation.EntityFormOptions = {
      formId: "96960917-c807-4b68-9376-91e353de0062", // create/edit contacts account form
      entityName: "ipg_contactsaccounts",
      useQuickCreateForm: true,
    };
    let formParameters: Xrm.Utility.OpenParameters = {};
    if (selectedRecordId) {
      const contact = await retriveContactAccountById(selectedRecordId);
      formParameters = {
        ipg_contactid: contact._ipg_contactid_value,
        ipg_contactname: contact.ipg_contactname,
        ipg_communicationpreference: contact.ipg_communicationpreference,
        ipg_email: contact.ipg_email,
        ipg_fax: contact.ipg_fax,
        ipg_isprimarycontact: contact.ipg_isprimarycontact,
        ipg_mainphone: contact.ipg_mainphone,
        ipg_otherphone: contact.ipg_otherphone,
        ipg_contactrolecode: contact.ipg_contactrolecode.split(',').map(Number)

      };
    }
    formParameters["ipg_accountid"] = primaryControl.data.entity.getId();
    await Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
      () => {
        primaryControl?.data?.refresh();
      }
    );
  }
  async function retriveContactAccountById(id: string): Promise<any> {
    return await Xrm.WebApi.online.retrieveRecord(
      "ipg_contactsaccounts",
      id,
      "?$select=_ipg_contactid_value,ipg_contactname,ipg_communicationpreference,ipg_email,ipg_fax,ipg_isprimarycontact,ipg_mainphone,ipg_otherphone,ipg_contactrolecode"
    );
  }
  /**
  * Opens Portal Contact Form
  * @function Intake.ContactsAccount.Ribbon.EditPortalFacilityUser
  * @returns {void}
  */
  //TODO : rename
  export async function EditPortalFacilityUser(selectedRecordId: string) {
    const formOptions: Xrm.Navigation.EntityFormOptions = {
      entityId: (await retriveContactAccountById(selectedRecordId))._ipg_contactid_value,
      formId: "ab10241c-37d5-4b20-9327-804338978643",
      entityName: "contact",
    }
    Xrm.Navigation.openForm(formOptions);
  }
  export async function Disassosiate(selectedIds) {
    const confirmString: Xrm.Navigation.ConfirmStrings = {
      text: "Do you wish to proceed with disassociating the selected Portal Users from this Facility?",
      cancelButtonLabel: "Cancel",
      confirmButtonLabel: "Continue"
    }
    const confirmDialogResult = await Xrm.Navigation.openConfirmDialog(confirmString);
    if (confirmDialogResult.confirmed) {
      selectedIds.forEach(async selectedRecordId => await Xrm.WebApi.deleteRecord("ipg_contactsaccounts", selectedRecordId));
    }
  }

  /**
   * Opens AssosiateFacility
   * @function Intake.ContactsAccount.Ribbon.AssosiateWithNewFacility
   * @returns {void}
   */
  export async function AssosiateWithNewFacility(primaryControl: Xrm.FormContext) {
    const formOptions: Xrm.Navigation.EntityFormOptions = {
      formId: "3d62166dde-dc3e-4671-aa0c-de52f3f278b9", // portal contact form
      entityName: "ipg_contactsaccounts",
      useQuickCreateForm: true
    }
    const formParameters =
      primaryControl.data.entity.getEntityName() === 'contact'
        ? { ipg_contactid: format(primaryControl.data.entity.getId()) }
        : { ipg_accountid: format(primaryControl.data.entity.getId()) }

    await Xrm.Navigation.openForm(formOptions, formParameters);
  }

  // just fast format for ids to not use Utility.js in ribbon file
  const format: Function = (id: string): string=> {
    return id.replace("{", "").replace("}", "").toLowerCase()
  }

   /**
   * Show buttons only on related Tab
   * @function Intake.ContactsAccount.Ribbon.ShowOnRelatedTab
   * @returns {void}
   */
  export async function ShowOnlyOnRelatedTab(selectedControl: Xrm.Controls.GridControl, tabName: string){    
    return selectedControl["_controlName"] === tabName;
  }
  
   /**
   * Hide buttons only on related Tab
   * @function Intake.ContactsAccount.Ribbon.HideOnRelatedTab
   * @returns {void}
   */
    export async function HideOnlyOnRelatedTab(selectedControl: Xrm.Controls.GridControl, tabName: string){    
      return selectedControl["_controlName"] != tabName;
    }

}