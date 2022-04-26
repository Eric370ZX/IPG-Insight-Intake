/**
 * @namespace Intake.Contact
 */
namespace Intake.Contact {
  const enum FormType {
    Undefined = 0,
    Create = 1,
    Update = 2,
    ReadOnly = 3,
    Disabled = 4,
    BulkEdit = 6,
    QuickCreate = 5,
    ReadOptimized = 11
  }
  const enum NotificationFrequency {
    Never = 427880000,
    OnceDaily = 427880001,
    OnceWeekly = 427880003
  }
  const enum NotificationPreference {
    OptIn = 427880000,
    OptOut = 427880001
  }
  /**
  * Called on Form Load event
  * @function Intake.Contact.OnLoadForm
  * @returns {void}
  */
  export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
    let formContext = executionContext.getFormContext();

    SetFieldsNoneRequired(formContext);
    ChooseForm(formContext);
    ConfigurePortalContact(formContext);
   // AddUsernamePopulationFromEmail(formContext);
    SetNotificationFrequencyByPreference(formContext);

    OnReviewStatusChange(formContext);
    formContext.getAttribute('ipg_facility_user_status_typecode')?.addOnChange(() => {
      OnReviewStatusChange(formContext);
    });  

    OnRejectedReasonChange(formContext);
    formContext.getAttribute('ipg_rejectionreason')?.addOnChange(() => {
      OnRejectedReasonChange(formContext);
    });  

    isZipCodeRequired(formContext);
    formContext.getAttribute('address1_line1')?.addOnChange(() => {
      isZipCodeRequired(formContext);
    });     

    if(formContext.ui.formSelector.getCurrentItem().getLabel() === "Physician") {
      disableFormSelector(formContext);      
    }        

  }

  function SetNotificationFrequencyByPreference(formContext: Xrm.FormContext) {
    const notificationPreferenceAttr = formContext.getAttribute("ipg_portalnotificationpreference");
    const notificationFrequencyAttr = formContext.getAttribute("ipg_portalemailalertfrequency");
    const notificationFrequencyCtrl = formContext.getControl("ipg_portalemailalertfrequency");
    if (notificationPreferenceAttr) {
      notificationPreferenceAttr.addOnChange(
        () => {
          if (notificationPreferenceAttr.getValue() === NotificationPreference.OptOut) {
            notificationFrequencyAttr.setValue(NotificationFrequency.Never);
            notificationFrequencyCtrl.setDisabled(true);
          } else {
            notificationFrequencyAttr.setValue(NotificationFrequency.OnceWeekly);
            notificationFrequencyCtrl.setDisabled(false);
          }
        });
      notificationPreferenceAttr.fireOnChange();
    }
  }
  function AddUsernamePopulationFromEmail(formContaxt: Xrm.FormContext) {
    const emailAttribute = formContaxt.getAttribute("emailaddress1");
    emailAttribute.addOnChange(
      () => {
        formContaxt.getAttribute("adx_identity_username").setValue(emailAttribute.getValue());
      })
    emailAttribute.fireOnChange();
  }
  function SetFieldsNoneRequired(formContext: Xrm.FormContext) {
    if (formContext.ui.formSelector.getCurrentItem().getLabel() === "Physician") {
      formContext.getAttribute("ipg_contacttypeid")?.setRequiredLevel("none");
      formContext.getAttribute("middlename")?.setRequiredLevel("none");
      formContext.getAttribute("birthdate")?.setRequiredLevel("none");
      formContext.getAttribute("address1_telephone2")?.setRequiredLevel("none");
      formContext.getAttribute("mobilephone")?.setRequiredLevel("none");
      formContext.getAttribute("telephone1")?.setRequiredLevel("none");
      formContext.getAttribute("address1_fax")?.setRequiredLevel("none");
      formContext.getAttribute("emailaddress1")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_zipcodeid")?.setRequiredLevel("none");
      formContext.getAttribute("address1_line1")?.setRequiredLevel("none");
      formContext.getAttribute("address1_city")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_stateid")?.setRequiredLevel("none");
      formContext.getAttribute("address1_country")?.setRequiredLevel("none");
      formContext.getAttribute("statecode")?.setRequiredLevel("none");
      formContext.getAttribute("parentcustomerid")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_physiciangrouptaxid")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_physiciangroupinstruction")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_physiciangroupnp")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_physiciantaxnumbe")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_physicianspecialinstructions")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_physiciannpi")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_physiciantaxonomycode")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_physiciannpistate")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_physicianrequestedbyid")?.setRequiredLevel("none");
    }
  }
  function ConfigurePortalContact(formContex: Xrm.FormContext) {
    if (formContex.ui.formSelector.getCurrentItem().getLabel() == "Portal Contact") {
      if (formContex.ui.getFormType() === XrmEnum.FormType.Create) {
        formContex.getControl("ipg_facilities")?.setVisible(true);
        formContex.getAttribute("ipg_facilities")?.setRequiredLevel("required");
      }
      else {
        ["ipg_facility_user_status_typecode"
          , "adx_identity_lastsuccessfullogin"
          , "ipg_lastpasswordmodifydate"
          , "adx_identity_lockoutenddate"
          , "adx_identity_logonenabled"].forEach(fieldName => {
            formContex.getControl(fieldName)?.setVisible(true);
          });

        formContex.getAttribute("ipg_facilities")?.setRequiredLevel("none");
        formContex.getControl("ipg_facilities")?.setVisible(false);

      }
      const notificationPreference = formContex.getAttribute("ipg_portalnotificationpreference");

      notificationPreference.addOnChange(() => {
        if (notificationPreference.getValue() === NotificationPreference.OptOut) {
          formContex.getAttribute("ipg_portalemailalertfrequency").setValue(NotificationFrequency.Never);
          formContex.getControl("ipg_portalemailalertfrequency").setDisabled(true);
        }
        else {
          formContex.getAttribute("ipg_portalemailalertfrequency").setValue(NotificationFrequency.OnceWeekly);
          formContex.getControl("ipg_portalemailalertfrequency").setDisabled(false);
        }
      });
      notificationPreference.fireOnChange();
    }
  }
  /**
    * Called on Form Save event
    * @function Intake.Contact.OnSaveForm
    * @returns {void}
  */
  export function OnSaveForm(executionContext: Xrm.Events.SaveEventContext) {
    let formContext = executionContext.getFormContext();
    if (formContext.ui.formSelector.getCurrentItem().getLabel() === "Portal Contact") {
      let isDirty = formContext.getAttribute("ipg_lastpasswordmodifydate")?.getIsDirty();
      if (isDirty) {
        ChangePasswordExpirationDate(formContext);
      }
    }
    if (CheckNPI(formContext)) {
      let saveEventArgs: Xrm.Events.SaveEventArguments = executionContext.getEventArgs();
      saveEventArgs.preventDefault();
    }
    
  }

  /**
  * Change password expiration date on modifying 'Last Password Modify Date' field
  * @function Intake.Contact.ChangePasswordExpirationDate
  * @returns {void}
  */
  function ChangePasswordExpirationDate(formContext) {
    let lastPasswordModifyDate = formContext.getAttribute("ipg_lastpasswordmodifydate")?.getValue();
    if (lastPasswordModifyDate) {
      let newExpirationDate = new Date(lastPasswordModifyDate);
      newExpirationDate.setDate(newExpirationDate.getDate() + 90);
      formContext.getAttribute("ipg_passwordexpirationdate")?.setValue(newExpirationDate);
    }
  }

  /**
    * Called on change Physician NPI
    * @function Intake.Contact.OnChangeNPI
    * @returns {void}
  */
  export function OnChangeNPI(executionContext) {
    let formContext = executionContext.getFormContext();
    CheckNPI(formContext);
  }


  /**
  * Shows warning if NPI exists in database
  * @function Intake.Contact.CheckNPI
  * @returns {void}
  */
  function CheckNPI(formContext): boolean {
    const NPIfield: string = 'ipg_physiciannpi';
    formContext.getControl(NPIfield)?.clearNotification(NPIfield);
    let npi: string = formContext.getAttribute(NPIfield)?.getValue();
    if ((npi) && (npi.length == 10)) {
      let query = "?$select=fullname&$filter=" + NPIfield + " eq '" + npi + "'";
      let GUID = formContext.data.entity.getId();
      if (GUID) {
        GUID = GUID.slice(1, -1);
        query = query + " and contactid ne " + GUID;
      }
      Xrm.WebApi.retrieveMultipleRecords('contact', query).then(function (result) {
        if (result.entities.length) {
          if (result.entities.length == 1)
            formContext.getControl(NPIfield).setNotification('This NPI is used by another physician: ' + result.entities[0].fullname, NPIfield);
          else {
            let fullnames: string = "";
            for (let entity of result.entities) {
              if (fullnames)
                fullnames += ", " + entity.fullname;
              else
                fullnames = entity.fullname;
            }
            formContext.getControl(NPIfield).setNotification('This NPI is used by other physicians: ' + fullnames, NPIfield);
          }
          return true;
        }
      }, function (error) {
        Xrm.Navigation.openErrorDialog({ message: error.message });
      });
    }
    return false;
  }

  /**
  * Called on load quick create carrier form
  * @function Intake.Contact.onLoadQuickCreateCarrierContactForm
  * @returns {void}
  */
  export function onLoadQuickCreateCarrierContactForm(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    let params = executionContext.getContext().getQueryStringParameters();
    fillFieldsByParams(formContext, params);
    setFieldsRequirementLevelAndVisibilityByType(formContext);
  }


  /**
  * fill fields on form by params
  * @function Intake.Contact.fillFieldsByParams
  * @returns {void}
  */
  function fillFieldsByParams(formContext: Xrm.FormContext, params) {
    let attributes = formContext.getAttribute();
    attributes.forEach((attribute) => {
      if (attribute.getName() in params) {
        if (attribute.getAttributeType() === "lookup") {
          let lookupArr = Array();
          lookupArr[0] = new Object();
          lookupArr[0].id = params[attribute.getName()];
          lookupArr[0].name = params[attribute.getName() + "name"];
          lookupArr[0].entityType = params[attribute.getName() + "entityType"];
          attribute.setValue(lookupArr);
        }
        else {
          attribute.setValue(params[attribute.getName()]);
        }
      }
    });
  }

  /**
* Chooses form depending on 'Relation Type' field value
* @function Intake.Contact.ChooseForm
* @returns {void}
*/
  function ChooseForm(formContext) {
    let contactType = formContext.getAttribute("ipg_contacttypeid");
    if (contactType) {
      let contactTypeValue = contactType.getValue();
      if (contactTypeValue && contactTypeValue.length > 0) {
        let contactTypeName = contactTypeValue[0].name.toLocaleLowerCase();
        let currentFormId = formContext.ui.formSelector.getCurrentItem().getId();

        interface contactTypeValues {
          name: string;
          formId: string;
        }
        var contactTypes: contactTypeValues[] = [
          { name: 'physician', formId: '12b0030d-bd5b-4c99-ad0d-fb7eab63426a'.toLocaleLowerCase() },
          { name: 'carrier', formId: '807AD6E4-30D2-4AD3-8CC4-0CD5E8EC1A9F'.toLocaleLowerCase() },
          { name: 'distributor', formId: 'A5733E8F-912A-46CE-B1FE-CBC68B4D3359'.toLocaleLowerCase() },
          { name: 'facility', formId: '7EDAB598-14BD-4BE0-976F-8DC72F61D485'.toLocaleLowerCase() },
          { name: 'manufacturer', formId: '85ED629F-ED77-406A-BBF2-8013FA618F25'.toLocaleLowerCase() },
          { name: 'patient', formId: 'A1F66726-D064-430F-83D9-A40B2551E0F8'.toLocaleLowerCase() },
          { name: 'health plan network', formId: 'A1F66726-D064-430F-83D9-A40B2551E0F8'.toLocaleLowerCase() },
        ];
        let currentContact = contactTypes.find(x => x.name == contactTypeName);
        if (currentFormId != currentContact.formId) {
          let items = formContext.ui.formSelector.items.get();
          for (let i in items) {
            if (items[i].getId() == currentContact.formId)
              items[i].navigate();
          }
        }
      }
    }
  }


  /**
  * set level of requirement and visibility of fields by contact's subType
  * @function Intake.Contact.setFieldsRequirementLevelAndVisibilityByType
  * @returns {void}
  */
  function setFieldsRequirementLevelAndVisibilityByType(formContext: Xrm.FormContext) {
    let contactSubTypeLookupValue = formContext.getAttribute("ipg_contactsubtypeid").getValue();
    if (contactSubTypeLookupValue && contactSubTypeLookupValue.length) {
      if (contactSubTypeLookupValue[0].name === "Claims Inquiries" || contactSubTypeLookupValue[0].name === "Pre-Certification") {
        formContext.getAttribute("telephone1").setRequiredLevel("required");
        formContext.getAttribute("lastname").setRequiredLevel("none");
        formContext.getAttribute("firstname").setRequiredLevel("none");
        formContext.getControl("mobilephone").setVisible(false);
        formContext.getControl("lastname").setVisible(false);
        formContext.getControl("firstname").setVisible(false);
        formContext.getControl("middlename").setVisible(false);
      }
      else if (contactSubTypeLookupValue[0].name === "Sales") {
        formContext.getControl("ipg_mddname_userid").setVisible(true);
      }
    }
  }

  /**
    * Called on change DOB
    * @function Intake.Contact.OnChangeDOB
    * @returns {void}
    */
  export function OnChangeDOB(executionContext) {
    let formContext = executionContext.getFormContext();
    let DOBField = "birthdate";
    let birthdate = formContext.getAttribute(DOBField).getValue();
    formContext.getControl(DOBField).clearNotification(DOBField);
    if ((birthdate) && (birthdate > new Date())) {
      formContext.getControl(DOBField).setNotification("Impossible to create a contact with future Date of Birth", DOBField);
    }
  }



  /**
    * Called on change DOB
    * @function Intake.Contact.onSubGridDocumentRecordSelect
    * @returns {void}
  */
  export function onSubGridDocumentRecordSelect(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    var currentEntity = formContext.data.entity;
    currentEntity.attributes.forEach(function (attribute, i) {
      attribute.controls.get(0).setDisabled(true);
    });
  }

  /**
    * Called on on Sub Grid FacilityPhysician Record Selected
    * @function Intake.Contact.onSubGridFacilityPhysicianRecordSelect
    * @returns {void}
  */
  export function onSubGridFacilityPhysicianRecordSelect(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    var currentEntity = formContext.data.entity;
    currentEntity.attributes.forEach(function (attribute, i) {
      if (attribute.getName() !== "ipg_status") {
        attribute.controls.get(0).setDisabled(true);
      }
    });
  }

  

  export function GetLookUpName(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    let contactObject = formContext.getAttribute("ipg_manufacturercontractaddress").getValue();

    if (contactObject != null) {
      let contactId = contactObject[0];
      let guid = contactId.id.replace("{", "").replace("}", "");
      Xrm.WebApi.retrieveRecord("contact", guid, "?$select=firstname,lastname,accountrolecode,telephone1,emailaddress1,ipg_contacttype").then(function success(results) {
        formContext.getAttribute("firstname").setValue(results["firstname"]);
        formContext.getAttribute("lastname").setValue(results["lastname"]);
        formContext.getAttribute("telephone1").setValue(results["telephone1"]);
        formContext.getAttribute("accountrolecode").setValue(results["accountrolecode"]);
        formContext.getAttribute("emailaddress1").setValue(results["emailaddress1"]);
        formContext.getAttribute("ipg_contacttype").setValue(results["ipg_contacttype"]);

      },
        function (error) {
        });
    }
  }


  export function ManufacturerContactType(executionContext) {

    var formContext = executionContext.getFormContext();
    var primaryContactType;
    var manId;
    var manufacturer = formContext.getAttribute("ipg_manufacturername").getValue();
    var contractType = formContext.getAttribute("ipg_contacttype").getValue(); 
    if (contractType == 427880000) {
      primaryContactType = "Primary";
      var i;
      if (manufacturer) {
        Xrm.WebApi.retrieveMultipleRecords("contact", "?$select=ipg_contacttype&$filter=ipg_manufacturername eq '" + manufacturer + "'").then(function success(results) {
          for (i = 0; i < results.entities.length; i++) {
            manId = results.entities[i]["contactid"];

            if (results.entities[i]["ipg_contacttype@OData.Community.Display.V1.FormattedValue"] == primaryContactType) {              
              formContext.getControl("ipg_contacttype").setNotification("Only one Primary contact type can exist, select Other.", manId);
            }
          }
        }, function (error) {
        });
      }
    }
    formContext.getControl("ipg_contacttype").clearNotification(manId);
  } 
  
  function OnReviewStatusChange(formContext: Xrm.FormContext) {
    const reviewStatus = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_facility_user_status_typecode");       
    const npi = formContext.getAttribute("ipg_physiciannpi");  
    const taxonomy = formContext.getAttribute("ipg_physiciantaxonomycode");     
    const active = formContext.getAttribute("ipg_active");
    const rejectionreason = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_rejectionreason");
    if(reviewStatus?.getValue() == 427880001) { // Approved      
      npi?.setRequiredLevel("required");   
      taxonomy?.setRequiredLevel("required");  
      rejectionreason?.setRequiredLevel("none"); 
      formContext.getControl("ipg_rejectionreason")?.setVisible(false);
    } 
    if(reviewStatus?.getValue() == 427880000) { // Pending Review
      npi?.setRequiredLevel("required");
      taxonomy?.setRequiredLevel("required"); 
      rejectionreason?.setRequiredLevel("none");     
      formContext.getControl("ipg_rejectionreason")?.setVisible(false);       
    } 
    if(reviewStatus?.getValue() == 427880002) { // Rejected
      npi?.setRequiredLevel("none"); 
      taxonomy?.setRequiredLevel("none"); 
      active?.setValue(false);
      rejectionreason?.setRequiredLevel("required"); 
      formContext.getControl("ipg_rejectionreason")?.setVisible(true);
    }
    OnRejectedReasonChange(formContext);
  }
  
  function OnRejectedReasonChange(formContext: Xrm.FormContext) {
    const reviewStatus = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_facility_user_status_typecode");
    const rejectionreason = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_rejectionreason");
    const relatedphysician = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_relatedphysician");    
    if(reviewStatus?.getValue() != 427880002) { // Rejected
      relatedphysician?.setRequiredLevel("none");   
      formContext.getControl("ipg_relatedphysician")?.setVisible(false);    
    }      
    if(rejectionreason?.getValue() == 427880000 && reviewStatus?.getValue() == 427880002) { // Duplicate      
      relatedphysician?.setRequiredLevel("required");   
      formContext.getControl("ipg_relatedphysician")?.setVisible(true);   
    } 
    if(rejectionreason?.getValue() == 427880001) { // Unable to Verify
      relatedphysician?.setRequiredLevel("none");   
      formContext.getControl("ipg_relatedphysician")?.setVisible(false);    
    }  
  }

  function disableFormSelector(formContext){
    formContext.ui.formSelector.items.forEach(formItem => formItem.setVisible(false));
  }
  
  function isZipCodeRequired(formContext: Xrm.FormContext) {
    let address = formContext.getAttribute("address1_line1");       
    let zipcode = formContext.getAttribute("ipg_zipcodeid");  
    if(address?.getValue() == null || address?.getValue() === "") {      
      zipcode?.setRequiredLevel("none");        
    } else { 
      zipcode?.setRequiredLevel("required");             
    }         
  }  

}