/**
 * @namespace Intake.Contact
 */
namespace Intake.Contact {
  enum AccountReviewStatus {
    Approved = 427880001
  }

  export function isPortalContactForm(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    return formContext.ui.formSelector.getCurrentItem().getLabel() === "Portal Contact";
  }



  export async function Deactivate(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let contactId = formContext.data.entity.getId().replace('{', '').replace('}', '');
    let result = await getContactAccountRecords(contactId);
    for (var i = 0; i < result.entities.length; i++) {
      Xrm.WebApi.deleteRecord("ipg_contactsaccounts", result.entities[i].ipg_contactsaccountsid);
    }
    Xrm.WebApi.updateRecord("contact", contactId,{
      adx_identity_logonenabled: false,
      ipg_portalnotificationpreference: 427880001,
      adx_identity_lockoutenddate: new Date().toLocaleDateString('yyyy-MM-dd')
    }).then(
      function success() {
        window["isPortalContactEnabled"] = '';
        formContext.data.refresh(true);
        formContext.ui.refreshRibbon(true);
      },
      function (error) {
        console.log(error.message);
      }
    );
  }

  async function getContactAccountRecords(contactId: string) {
    const fetch = `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false" >
                       <entity name="ipg_contactsaccounts" >
                           <attribute name="ipg_contactsaccountsid" />
                           <filter type="and" >
                               <condition attribute="ipg_contactid" operator="eq" uiname="" uitype="contact" value="${contactId}" />
                           </filter>
                       </entity>
                   </fetch>`;
    return Xrm.WebApi.retrieveMultipleRecords("ipg_contactsaccounts", `?fetchXml=${fetch}`);
  }

  let isPortalContactEnabled = false;
  export function IsPortalContactEnabled(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let contactid = formContext.data.entity.getId().replace('{', '').replace('}', '');
    if (window["isPortalContactEnabled"])
      return isPortalContactEnabled;

    Xrm.WebApi.retrieveRecord("contact", contactid, "?$select=adx_identity_logonenabled").then(
      function success(result) {
        window["isPortalContactEnabled"] = true;
        isPortalContactEnabled = (result.adx_identity_logonenabled);
        if (isPortalContactEnabled) {
          formContext.ui.refreshRibbon(true);
        }
      }
    );

    return false;
  }

  /**
   * Open new contact form
   * @function Intake.Contact.NewContactForm
   * @returns {void}
   */
  export function NewContactForm(PrimaryControl) {
    let viewName: string = "carrier";
    if (PrimaryControl) { //PrimaryControl equals null in classic UI
      var viewSelector = PrimaryControl.getViewSelector();
      viewName = viewSelector.getCurrentView().name.toLocaleLowerCase();
    }

    var ipg_contacttypeid;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "contact";
    var formParameters = {};

    var defaultFordId = '1FED44D1-AE68-4A41-BD2B-F13ACAC4ACFA';
    interface contactTypesValues {
      name: string;
      value: string;
      formId: string;
    }
    var contactTypes: contactTypesValues[] = [
      { name: 'carrier', value: '5e9050c9-515e-46ec-9b95-65b8858876b0', formId: defaultFordId },
      { name: 'distributor', value: '0b0e5fff-afe1-4da5-9c68-68842c8ca5b2', formId: defaultFordId },
      { name: 'facilit', value: 'b6d48511-adbf-4ddf-8885-43d06460c63a', formId: defaultFordId }, // not 'facilitY'!!!
      { name: 'health plan network', value: '47724e16-c175-473f-91e3-f33d1514fca3', formId: '807AD6E4-30D2-4AD3-8CC4-0CD5E8EC1A9F' },
      { name: 'manufacturer', value: '8b383208-32b6-42b6-81bc-a0cbfed34fae', formId: '85ED629F-ED77-406A-BBF2-8013FA618F25' },
      { name: 'patient', value: '3ed7420f-e687-47ec-8c44-a675305137bd', formId: 'A1F66726-D064-430F-83D9-A40B2551E0F8' },
      { name: 'physician', value: '2b5407df-b88a-4bf4-8e6f-ab174bc13c78', formId: '12b0030d-bd5b-4c99-ad0d-fb7eab63426a' }
    ];

    contactTypes.forEach(function (element1) {
      if (viewName.indexOf(element1.name) >= 0) {
        var viewWithMultipleContactTypes: boolean = false;
        contactTypes.forEach(function (element2) {
          if ((element1.name != element2.name) && (viewName.indexOf(element2.name) >= 0))
            viewWithMultipleContactTypes = true;
        });
        if (!viewWithMultipleContactTypes) {
          ipg_contacttypeid = element1.value;
          entityFormOptions["formId"] = element1.formId;
        }
      }
    });

    //if (ipg_contacttypeid) {
    //  formParameters["ipg_contacttypeid"] = ipg_contacttypeid;
    //}
    Xrm.Navigation.openForm(entityFormOptions, formParameters);
  }

  /**
   * enable rule for 'New' dropdown for creating Carrier's contact
   * @function Intake.Contact.contactFormIsCarrier
   * @returns {boolean}
   */
  export function contactFormIsCarrier(primaryControl): boolean {
    let formContext: Xrm.FormContext = primaryControl;
    var formName = formContext.ui.formSelector.getCurrentItem().getLabel();
    if (formName === "Carrier") {
      return true;
    }
    return false;
  }

  /**
   * enable rule for 'Create Invitation' ribbon`s button
   * @function Intake.Contact.AccountStatusIsEnabled
   * @returns {boolean}
   */
  export async function AccountStatusIsEnabled(selectedRecordId: string) {
    const accountStatus = (
      await Xrm.WebApi.online.retrieveRecord(
        "contact"
        , selectedRecordId.toLocaleLowerCase().replace("{", "").replace("}", "")
        , "?$select=adx_identity_logonenabled"
      )
    )?.adx_identity_logonenabled;

    return accountStatus;
  }

  /**
  * enable rule for 'Create Invitation' ribbon`s button
  * @function Intake.Contact.AccountReviewStatusIsApproved
  * @returns {boolean}
  */
  export async function AccountReviewStatusIsApproved(selectedRecordId: string) {
    const accountReviewStatus = (
      await Xrm.WebApi.online.retrieveRecord(
        "contact"
        , selectedRecordId.toLocaleLowerCase().replace("{", "").replace("}", "")
        , "?$select=ipg_facility_user_status_typecode"
      )
    )?.ipg_facility_user_status_typecode;

    return accountReviewStatus == AccountReviewStatus.Approved;
  }

  /**
   * show Quick Create Carrier Form with predefined types of contact
   * @function Intake.Contact.NewCarrierContactSubgrid
   * @returns {void}
   */
  export function NewCarrierContactSubgrid(primaryControl, contactSubTypeName) {
    function openQuickCreateCarrierForm(carrierContactType, contactCarrierSubType) {
      let entityFormOptions = {};
      entityFormOptions["entityName"] = "contact";

      entityFormOptions["useQuickCreateForm"] = true;
      entityFormOptions["formId"] = "B587448B-DF14-45D6-9F2D-E73F5822E6E2";

      let formParameters = {};
      formParameters["ipg_relatedcarreiraccountid"] = formContext.data.entity.getId();
      formParameters["ipg_relatedcarreiraccountidname"] = formContext.getAttribute("name").getValue();
      formParameters["ipg_relatedcarreiraccountidentityType"] = "account";

      if (carrierContactType) {
        formParameters["ipg_contacttypeid"] = carrierContactType.ipg_contacttypeid;
        formParameters["ipg_contacttypeidname"] = carrierContactType.ipg_name;
        formParameters["ipg_contacttypeidentityType"] = "ipg_contacttype";
      }

      if (contactSubTypeName) {
        formParameters["ipg_contactsubtypeid"] = contactCarrierSubType.ipg_contactsubtypeid;
        formParameters["ipg_contactsubtypeidname"] = contactCarrierSubType.ipg_name;
        formParameters["ipg_contactsubtypeidentityType"] = "ipg_contactsubtype";
      }
      //formParameters["ipg_contactrolecode"] = contactRole;

      Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function (success) {
          console.log(success);
        },
        function (error) {
          console.log(error);
        });
    }

    let formContext: Xrm.FormContext = primaryControl;
    Xrm.Utility.showProgressIndicator("Loading...");
    Xrm.WebApi.retrieveMultipleRecords("ipg_contacttype", "?$select=ipg_name&$filter=contains(ipg_name,'Carrier')")
      .then((result) => {
        if (result && result.entities && result.entities.length) {
          let carrierContactType = result.entities[0]; //ipg_contacttypeid
          Xrm.WebApi.retrieveMultipleRecords("ipg_contactsubtype", "?$select=ipg_name&$filter=contains(ipg_name,'" + contactSubTypeName + "') and _ipg_parentcontacttype_value eq '" + carrierContactType.ipg_contacttypeid + "'")
            .then((result) => {
              if (result && result.entities && result.entities.length) {
                let contactCarrierSubType = result.entities[0];
                Xrm.Utility.closeProgressIndicator();
                openQuickCreateCarrierForm(carrierContactType, contactCarrierSubType);
              }
              else {
                Xrm.Utility.closeProgressIndicator();
                Xrm.Navigation.openAlertDialog({ text: "There is no '" + contactSubTypeName + "' contact subtype record!" });
              }
            }, (error) => {
              Xrm.Utility.closeProgressIndicator();
              console.log(error);
            });
        }
        else {
          Xrm.Utility.closeProgressIndicator();
          Xrm.Navigation.openAlertDialog({ text: "There is no 'Carrier' contact type record!" });
        }
        //var contactSubType = Xrm.WebApi.retrieveMultipleRecords("ipg_contactsubtype"); Claim Escalation
      }, (error) => {
        Xrm.Utility.closeProgressIndicator();
        console.log(error);
      });
  }

  export async function ChangePassword(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    const confirmString: Xrm.Navigation.ConfirmStrings = {
      text: "Do you wish to generate a temporary password for this user?",
      confirmButtonLabel: "Confirm",
      cancelButtonLabel: "Cancel"
    }
    const confirmDialogResult = await Xrm.Navigation.openConfirmDialog(confirmString);
    if (confirmDialogResult.confirmed) {
      let tempPassword = GenerateNewPassword();
      let contactId = formContext.data.entity.getId().replace("{", "").replace("}", "");
      StartChangePasswordWorkflow(contactId, tempPassword);
    }
  }

  function GenerateNewPassword() {
    var chars = "0123456789abcdefghijklmnopqrstuvwxyz!@$%^*.,()ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    var passwordLength = 12;
    var password = "!*temp*!";
    for (var i = 0; i < passwordLength; i++) {
      var randomNumber = Math.floor(Math.random() * chars.length);
      password += chars.substring(randomNumber, randomNumber + 1);
    }
    return password;
  }

  function StartChangePasswordWorkflow(contactId, tempPassword) {
    Xrm.Utility.showProgressIndicator("Processing...");

    let entity = {
      id: contactId,
      entityType: "contact"
    };
  
    let parameters = {
      entity: entity,
      NewPassword: `${tempPassword}`,
    };

    let parameterTypes = {
      "entity": {
        "typeName": "mscrm.contact",
        "structuralProperty": 5
      },
      "NewPassword": {
        "typeName": "Edm.String",
        "structuralProperty": 1
      }
    }

    let request = {
      entity: parameters.entity,
      NewPassword: parameters.NewPassword,
      getMetadata: function () {
        return {
          boundParameter: "entity",
          parameterTypes: parameterTypes,
          operationType: 0,
          operationName: "ipg_IPGIntakeContactWorkflowsChangeContactPassword"
        };
      }
    };

    Xrm.WebApi.online.execute(request)
      .then(function (response) {
        Xrm.Utility.closeProgressIndicator();
        if (response.ok) {
          Xrm.Navigation.openAlertDialog({text: "New temporary password generated succesfully!"});
        }
        else {
          Xrm.Navigation.openErrorDialog({ message: response.statusText });
        }
      }, function (error) {
        Xrm.Utility.closeProgressIndicator();
        Xrm.Navigation.openErrorDialog({ message: error.message });
      })
  }

  export async function Impersonate(primaryControl) {
    Xrm.Utility.showProgressIndicator("Processing...");
    let formContext: Xrm.FormContext = primaryControl;

    let entity = {
      id: formContext.data.entity.getId(),
      entityType: "contact"
    };

    let request = {
      entity: entity,
      getMetadata: function () {
        return {
          boundParameter: "entity",
          parameterTypes: {
            "entity": {
              "typeName": "mscrm.contact",
              "structuralProperty": 5
            }},
          operationType: 0,
          operationName: "ipg_IPGIntakeContactActionsImpersonate"
        };
      }
    };

    try
    {
      await Xrm.WebApi.online.execute(request);
      Xrm.Utility.closeProgressIndicator();
    }
    catch(e)
    {
      Xrm.Utility.closeProgressIndicator();
      Xrm.Navigation.openErrorDialog({ message: e.message });
    }
  }

  export async function ImpersonateEnablade(primaryControl):Promise<boolean> {
    let formContext: Xrm.FormContext = primaryControl;

    if(!IsCurrentUserSysAdmin() || formContext.ui.getFormType() == XrmEnum.FormType.Create)
    {
      return false;
    }

    let contactid = formContext.data.entity.getId();

    var contact = await Xrm.WebApi.retrieveRecord("contact", contactid, "?$select=adx_identity_securitystamp,ipg_backupjson")
    return contact.adx_identity_securitystamp != null;
  }
  export async function RevertBack(primaryControl) {
    Xrm.Utility.showProgressIndicator("Processing...");
    let formContext: Xrm.FormContext = primaryControl;

    let entity = {
      id: formContext.data.entity.getId(),
      entityType: "contact"
    };

    let request = {
      entity: entity,
      getMetadata: function () {
        return {
          boundParameter: "entity",
          parameterTypes: {
            "entity": {
              "typeName": "mscrm.contact",
              "structuralProperty": 5
            }},
          operationType: 0,
          operationName: "ipg_IPGIntakeContactActionsRevertBackImpersonation"
        };
      }
    };

    try
    {
      await Xrm.WebApi.online.execute(request);
      Xrm.Utility.closeProgressIndicator();
    }
    catch(e)
    {
      Xrm.Utility.closeProgressIndicator();
      Xrm.Navigation.openErrorDialog({ message: e.message });
    }
  }
  export async function RevertBackEnabled(primaryControl):Promise<boolean> {
    let formContext: Xrm.FormContext = primaryControl;
    if(!IsCurrentUserSysAdmin() || formContext.ui.getFormType() == XrmEnum.FormType.Create)
    {
      return false;
    }

    let contactid = formContext.data.entity.getId();

    var contact = await Xrm.WebApi.retrieveRecord("contact", contactid, "?$select=adx_identity_securitystamp,ipg_backupjson");
    return contact.adx_identity_securitystamp != null && contact.ipg_backupjson != null;
  }

  function IsCurrentUserSysAdmin():boolean
  {
    return Xrm.Utility.getGlobalContext().userSettings.roles.get(lk => lk.name && lk.name.toLowerCase().trim() == "system administrator").length > 0;
  }

  /**
 * Show buttons only on related Form
 * @function Intake.Contact.ShowOnlyOnRelatedForm
 * @returns {bool}
 */
  export async function ShowOnlyOnRelatedForm(primaryControl){    
    let formContext: Xrm.FormContext = primaryControl;      
    const formsName: string[] = ["Physician", "Portal Contact"];
    let formName = formContext.ui.formSelector.getCurrentItem().getLabel();    
    return formsName.indexOf(formName) !== -1; //formsName.includes(formName);
  }
  
  /**
 * Hide buttons only on related Form
 * @function Intake.Contact.HideOnlyOnRelatedForm
 * @returns {bool}
 */
  export async function HideOnlyOnRelatedForm(primaryControl){   
    let formContext: Xrm.FormContext = primaryControl; 
    let formName: string = "Physician";    
    return formContext.ui.formSelector.getCurrentItem().getLabel() != formName;
  }

}