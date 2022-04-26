/**
 * @namespace Intake.Referral
 */
namespace Intake.Referral {

  const pif1formId = '5650e49e-0ada-45b8-8aa7-2e40a5617c65';
  const pif2formId = '7266869e-f61a-45e5-84f9-c7c5446dbfc5';

  const enum CaseStages {
    Intake = "Intake",
    Authorizations = "Authorizations",
    CaseManagement = "Case Management",
    Billing = "Billing",
    CarrierCollections = "Carrier Collections",
    PatientCollections = "Patient Collections",
    Finance = "Finance"
  };

  const enum CaseStates {
    Intake = 923720000,
    Authorizations = 923720001,
    CaseManagement = 923720002,
    Billing = 923720003,
    CarrierCollections = 923720004,
    PatientCollections = 923720005,
    Finance = 923720006
  };

  const enum CarrierTypes {
    Auto = 427880000,
    Commercial, value = 427880002,
    DME = 427880004,
    Government = 923720006,
    IPA = 427880003,
    Other = 923720011,
    SelfPay = 427880005,
    WorkersComp = 427880001
  };

  const enum CaseStatuses {
    Closed = 923720001,
    Open = 923720000
  }

  /**
   * Called on load form
   * @function Intake.Referral.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    
    formContext.data.addOnLoad((event) => ChooseForm(event.getFormContext()));
    ChooseForm(formContext);
    CreateFormVisibility(formContext);
    IntakePIFStep2Controls(formContext);
    var statusAttr = formContext.getAttribute("ipg_casestatus");
    statusAttr?.addOnChange(() => {
      lockAndSetFormNotificationIfClosed(formContext);
      DisplayOwnerField(formContext);
    });
    statusAttr?.fireOnChange();

    ConfigureStep1Form(formContext);
    ConfigureStep2Form(formContext);

    SetFaciltyName(formContext);
    setCaseStatusDisplayedValue(formContext);
    showTimeLine(formContext);
   // SetPhysicianFromFacility(formContext);
    addCarriersPreSearch(formContext);

    var primaryCategory = formContext.getAttribute("ipg_primarycarriercategory");
    primaryCategory?.addOnChange(() => {
      formContext.getAttribute("ipg_carrierid").setValue(null);
    });
    
    var secondaryCategory = formContext.getAttribute("ipg_secondarycarriercategory");
    secondaryCategory.addOnChange(() => {
      formContext.getAttribute("ipg_carriername2id").setValue(null);
    });

    const ownerid = formContext.getAttribute("ownerid");
    ownerid?.setSubmitMode("never");
    ownerid?.addOnChange(onChangeOwner);

    var focusOnTab = window.localStorage.getItem('focusOnTab');
    if (focusOnTab != null) { 
      Xrm.Page.ui.tabs.get(focusOnTab).setFocus();
      window.localStorage.removeItem('focusOnTab');
      window.localStorage.removeItem('back');
    }
    lockAndSetFormNotificationIfClosed(formContext);
  }

  function ConfigureStep1Form(formContext: Xrm.FormContext){
    if (formContext.ui.formSelector.getCurrentItem().getId() == pif1formId) {
      var carrierAttr = formContext.getAttribute("ipg_carrierid");
      carrierAttr?.addOnChange(OnChangePrimaryCarrier);
      carrierAttr?.fireOnChange();
    }
  }

  function ConfigureStep2Form(formContext: Xrm.FormContext) {
    if (formContext.ui.formSelector.getCurrentItem().getId() == pif2formId) {
      var carrierAttr = formContext.getAttribute("ipg_carrierid");
      carrierAttr?.addOnChange(OnChangeCarrier);

      OnChangeCarrier(null, formContext);

      var carrierAttr = formContext.getAttribute("ipg_carriername2id");
      carrierAttr?.addOnChange(OnChangeSecondCarrier);
      carrierAttr?.fireOnChange();
    }
  }

  function SetFaciltyName(formContext: Xrm.FormContext) {
    if (formContext.ui.getFormType() == XrmEnum.FormType.Create) {
      var guid = localStorage.facilityid;

      if (guid && guid != "undefined") {
        var object = new Array();
        object[0] = new Object();
        object[0].id = guid;
        object[0].name = localStorage.facilityname;
        object[0].entityType = "account";
        formContext.getAttribute("ipg_facilityid").setValue(null);
        formContext.getAttribute("ipg_facilityid").setValue(object);
        localStorage.removeItem("facilityname");
        localStorage.removeItem("facilityid");
      }
    }
  }



  async function lockAndSetFormNotificationIfClosed(formContext: Xrm.FormContext) {
    var status = formContext.getAttribute("ipg_casestatus")?.getValue();
    if (status == 923720001) {
      formContext.ui.setFormNotification("Referral is Closed", "WARNING", "closed");
      EnableFieldsIfClosed(formContext);
      formContext.getControl("ipg_carriername2id").setDisabled(true);
      formContext.getControl("ipg_memberidnumber2").setDisabled(true);
      const docId = formContext.getAttribute("ipg_sourcedocumentid").getValue(); 
      await Xrm.WebApi.updateRecord("ipg_document", Utility.removeCurlyBraces(docId[0].id), {"_ipg_referralid_value": null} );
    }
    else {
      formContext.ui.clearFormNotification("closed");
    }
  }

  /**
   * Called on change patient last name
   * @function Intake.Referral.OnChangePatientLastName
   * @returns {void}
  */
  export function OnChangePatientLastName(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    addFiltersToPatientField(formContext);
  }

  /**
   * Called on change patient first name
   * @function Intake.Referral.OnChangePatientFirstName
   * @returns {void}
  */
  export function OnChangePatientFirstName(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    addFiltersToPatientField(formContext);
  }

  /**
   * Called on change patient middle name
   * @function Intake.Referral.OnChangePatientMiddleName
   * @returns {void}
  */
  export function OnChangePatientMiddleName(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    addFiltersToPatientField(formContext);
  }

  /**
   * Called on change patient dob
   * @function Intake.Referral.OnChangePatientDOB
   * @returns {void}
  */
  export function OnChangePatientDOB(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    addFiltersToPatientField(formContext);
  }

  //PIF Step 1 Form
  function OnChangePrimaryCarrier(executionContext: Xrm.Events.EventContext){
    var formContext = executionContext.getFormContext();
    var carrierVal = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_carrierid")?.getValue();
    if (carrierVal && carrierVal.length > 0) {
      Xrm.WebApi.retrieveRecord(carrierVal[0].entityType, carrierVal[0].id, "?$select=ipg_carriertype")
        .then(carrier => {
          if (carrier["ipg_carriertype"] == CarrierTypes.WorkersComp) //Workers Comp
          {
            formContext.getAttribute("ipg_carriername2id")?.setValue(null);
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_carriername2id")?.setDisabled(true);
            formContext.getAttribute("ipg_memberidnumber2")?.setValue("");
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber2")?.setDisabled(true);
          }
          else {
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_carriername2id")?.setDisabled(false);
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber2")?.setDisabled(false);
          }
        });
    }
  }

  function OnChangeCarrier(executionContext: Xrm.Events.EventContext, formContext?: Xrm.FormContext) {
    var formContext = formContext || executionContext.getFormContext();
    var carrierVal = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_carrierid")?.getValue();

    if (executionContext) {
      var dateofinjuryAttr = formContext.getAttribute("ipg_autodateofincident");
      var adjusterNameAttr = formContext.getAttribute("ipg_autoadjustername");
      var adjusterPhoneAttr = formContext.getAttribute("ipg_autoadjusterphone");
      var wcNurseCaseMgrName = formContext.getAttribute("ipg_wcnursecasemgrname");
      var wcNurseCaseMgrPhone = formContext.getAttribute("ipg_wcnursecasemgrphone");

      dateofinjuryAttr.setValue(null);
      adjusterNameAttr.setValue(null);
      adjusterPhoneAttr.setValue(null);
      wcNurseCaseMgrName.setValue(null);
      wcNurseCaseMgrPhone.setValue(null);
    }

    var additionalRefInfSection = formContext.ui.tabs.get("Referral")?.sections.get("Additional_Referral_Info");

    if (carrierVal && carrierVal.length > 0) {
      Xrm.WebApi.retrieveRecord(carrierVal[0].entityType, carrierVal[0].id, "?$select=ipg_carriertype")
        .then(carrier => {
          if (carrier["ipg_carriertype"] == 427880000 || carrier["ipg_carriertype"] == 427880001) //auto, Workers Comp
          {
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber")?.setVisible(true);//claim #
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber1")?.setVisible(false);//memberid
            additionalRefInfSection?.setVisible(true);
            formContext.getAttribute("ipg_carriername2id")?.setValue(null);
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_carriername2id")?.setDisabled(true);
            formContext.getAttribute("ipg_memberidnumber2")?.setValue("");
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber2")?.setDisabled(true);
            formContext.getAttribute("ipg_memberidnumber21")?.setValue("");
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber21")?.setDisabled(true);
          }
          else {
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber")?.setVisible(false);//claim #
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber1")?.setVisible(true);//memberid
            additionalRefInfSection?.setVisible(false);
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_carriername2id")?.setDisabled(false);
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber2")?.setDisabled(false);
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber21")?.setDisabled(false);
          }
        });
    }
    else {
      formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber")?.setVisible(false);//claim #
      formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber1")?.setVisible(true);//memberid
      additionalRefInfSection?.setVisible(false);
    }
  }

  function OnChangeSecondCarrier(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    var carrierVal = (executionContext.getEventSource() as Xrm.Attributes.LookupAttribute)?.getValue();

    if (carrierVal && carrierVal.length > 0) {
      Xrm.WebApi.retrieveRecord(carrierVal[0].entityType, carrierVal[0].id, "?$select=ipg_carriertype")
        .then(carrier => {
          if (carrier["ipg_carriertype"] == 427880000 || carrier["ipg_carriertype"] == 427880001) //auto, Workers Comp
          {
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber2")?.setVisible(true);//claim #
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber21")?.setVisible(false);//memberid
          }
          else {
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber2")?.setVisible(false);//claim #
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber21")?.setVisible(true);//memberid
          }
        });
    }
    else {
      formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber2")?.setVisible(false);//claim #
      formContext.getControl<Xrm.Controls.StandardControl>("ipg_memberidnumber21")?.setVisible(true);//memberid
    }
  }

  /**
   * Called on change patient last name
   * @function Intake.Referral.OnChangeSurgeryDate
   * @returns {void}
  */
  export function OnChangeSurgeryDate(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    const fieldName = "ipg_surgerydate";
    const surgerydate = formContext.getAttribute(fieldName).getValue();
    if (surgerydate < new Date().setMonth(-6)) {
      (<Xrm.Controls.DateControl>formContext.getControl(fieldName)).setNotification("Invalid Scheduled DOS entered - outside of Carrier’s Timely Filing range. Please correct and try again.", fieldName);
    }
    else {
      formContext.getControl<Xrm.Controls.StandardControl>(fieldName).clearNotification(fieldName);
    }
  }


  /**
  * Called on CaseStatus change (fires on form load too)
  * @function Intake.Referral.DisplayOwnerField
  * @returns {void}
  */
  export function DisplayOwnerField(formContext: Xrm.FormContext) {
    const ownerControl = formContext.getControl<Xrm.Controls.StandardControl>("ownerid");
    if (isReferralOpened(formContext)) {
      ownerControl?.setDisabled(false);
    } else {
      ownerControl?.setDisabled(true);
    }
  }

  /**
  * Called on change patient last name
  * @function Intake.Referral.DisableAllFieldsIfFinishGate
  * @returns {void}
  */
  export function EnableFieldsIfClosed(formContext: Xrm.FormContext) {
    const caseStatus = formContext.getAttribute("ipg_casestatus");
    if (caseStatus && caseStatus.getValue() === 923720001) { //Closed
      formContext.ui.controls.forEach((control: Xrm.Controls.StandardControl) => {
        if (control && control.getDisabled && !control.getDisabled()) {
          control.setDisabled(true);
        }
      });
    }
  }

  /**
   * add filters to patient field
   * @function Intake.Referral.OnChangePatientDOB
   * @returns {void}
  */
  function addFiltersToPatientField(formContext: Xrm.FormContext) {
    function addCustomLookupFilter(formContext: Xrm.FormContext) {
      const filters: Array<string> = [
        '<condition attribute="ipg_contacttypeidname" operator="like" value="%Patient%" />',
      ];
      if (patientLastNameValue) filters.push(`<condition attribute="lastname" operator="eq" value="${patientLastNameValue}" />`);
      if (patientMiddleNameValue) filters.push(`<condition attribute="middlename" operator="eq" value="${patientMiddleNameValue}" />`);
      if (patientFirstNameValue) filters.push(`<condition attribute="firstname" operator="eq" value="${patientFirstNameValue}" />`);
      if (patientDateOfBirthValue) {
        const dateString = (patientDateOfBirthValue.getFullYear() + '-' + ('0' + (patientDateOfBirthValue.getMonth() + 1)).slice(-2) + '-' + ('0' + patientDateOfBirthValue.getDate()).slice(-2));
        filters.push(`<condition attribute="birthdate" operator="eq" value="${dateString}" />`);
      }
      let filterXml = `<filter type="and">${filters.join('')}</filter>`;
      formContext.getControl<Xrm.Controls.LookupControl>("ipg_patientid").addCustomFilter(filterXml, "contact");
    }

    let patientLastNameValue = formContext.getAttribute("ipg_patientlastname").getValue();
    let patientMiddleNameValue = formContext.getAttribute("ipg_patientmiddlename").getValue();
    let patientFirstNameValue = formContext.getAttribute("ipg_patientfirstname").getValue();
    let patientDateOfBirthValue = formContext.getAttribute("ipg_patientdateofbirth").getValue();

    if (patientLastNameValue || patientMiddleNameValue || patientFirstNameValue || patientDateOfBirthValue) {
      formContext.getControl<Xrm.Controls.LookupControl>("ipg_patientid").addPreSearch(function () {
        addCustomLookupFilter(formContext);
      });
    }
  }

  /**
 * Called on saving form
 * @function Intake.Referral.OnSaveForm
 * @returns {void}
*/
  export let showWarningMessage = true;
  export function OnSaveForm(executionContext: Xrm.Events.SaveEventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    let saveEvent = executionContext.getEventArgs();

    //Prevent auto-save
    if (saveEvent.getSaveMode() == XrmEnum.SaveMode.AutoSave || saveEvent.getSaveMode() == XrmEnum.SaveMode.SaveAndClose) {
      saveEvent.preventDefault();
      return;
    }

    //Allow activation
    if (saveEvent.getSaveMode() == XrmEnum.SaveMode.Reactivate) {
      return;
    }

    var caseStatus = formContext.getAttribute("ipg_casestatus")?.getValue();
    var isCaseStatusDirty = formContext.getAttribute("ipg_casestatus")?.getIsDirty();
    var isCaseStatusDisplayedDirty = formContext.getAttribute("ipg_casestatusdisplayed")?.getIsDirty();
    if (caseStatus == CaseStatuses.Closed && !isCaseStatusDirty && !isCaseStatusDisplayedDirty) {
      Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "You are not able to save closed referral." }).then(() => {
        return;
      });
    }

    if (showWarningMessage && formContext.data.entity.getIsDirty()) {
      saveEvent.preventDefault();
      Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "Referral information has been saved.  Please click 'Submit' to create a referral" }).then(() => {
        showWarningMessage = false;
        formContext.data.save().then(() => {

        });
      })
        .then(() => {
          showWarningMessage = true;
        });
    }
  }

  /**
 * Chooses form depending on 'ipg_lifecyclestepid' field value
 * @function Intake.Referral.ChooseForm
 * @returns {void}
 */
  function ChooseForm(formContext: Xrm.FormContext) {
    let ipg_lifecyclestepid = formContext.getAttribute("ipg_lifecyclestepid");
    let currentFormId = formContext.ui.formSelector.getCurrentItem().getId();

    // Default Form
    let currentFormRequired = pif1formId;

    if (ipg_lifecyclestepid) {
      let ipg_lifecyclestepidValue = ipg_lifecyclestepid.getValue();
      if (ipg_lifecyclestepidValue && ipg_lifecyclestepidValue.length > 0) {
        let ipg_lifecyclestepName = ipg_lifecyclestepidValue[0].name.toLocaleLowerCase();

        interface lifecycleStepValues {
          name: string;
          formId: string;
        }

        var lifecycleStepForms: lifecycleStepValues[] = [
          { name: 'intake step 1', formId: pif1formId },
          { name: 'ehr refferal', formId: pif1formId },
        ];
        let lifecycleStepForm = lifecycleStepForms.find(x => x.name == ipg_lifecyclestepName);
        // Form for all other lifecycle steps
        currentFormRequired = pif2formId;

        if (!!lifecycleStepForm) {
          currentFormRequired = lifecycleStepForm.formId;
        }
      }
    }
    if (currentFormId != currentFormRequired) {
      let items = formContext.ui.formSelector.items.get();
      for (let i in items) {
        if (items[i].getId() == currentFormRequired) {
          var attributes = Xrm.Page.data.entity.attributes.get();
          if (attributes != null) {
            for (var a of attributes) {
              if (a.getIsDirty()) 
                a.setSubmitMode('never')
            }
          }
          items[i].navigate();
        }
      }
    }
  }

  /**
  * Changing form layout based on create form type
  * @function Intake.Referral.CreateFormVisibility
  * @returns {void}
  */
  function CreateFormVisibility(formContext: Xrm.FormContext) {
    let formType = formContext.ui.getFormType();
    //if create form
    if (formType == XrmEnum.FormType.Create) {
      formContext.ui.tabs.get("GatingLogTab")?.setVisible(false);
      formContext.ui.tabs.get("TasksTab")?.setVisible(false);
      formContext.ui.tabs.get("Documents")?.setVisible(false);
      formContext.ui.tabs.get("Dev")?.setVisible(false);
      (<any>formContext.ui).headerSection.setBodyVisible(false);
      (<any>formContext.ui).headerSection.setCommandBarVisible(true);
      formContext.ui.setFormNotification('Enter the initial Referral information and Submit for processing.', 'INFO', '');
    }
    else {
      formContext.ui.tabs.get("GatingLogTab")?.setVisible(false);
      formContext.ui.tabs.get("TasksTab")?.setVisible(true);
      formContext.ui.tabs.get("Documents")?.setVisible(true);
      (<any>formContext.ui).headerSection.setBodyVisible(true);
      (<any>formContext.ui).headerSection.setCommandBarVisible(true);
    }
  }

  /**
 * Called on load form (Intake.Referral.OnLoadForm)
 * @function Intake.Referral.IntakePIFStep2Controls
 * @returns {void}
 */
  function IntakePIFStep2Controls(formContext: Xrm.FormContext) {

    const fieldsToHideShow = ["ipg_patientaddress", "ipg_melissapatientcity", "ipg_melissapatientstate", "ipg_melissapatientzipcodeid", "ipg_homephone", "ipg_workphone", "ipg_cellphone", "ipg_email",
      "ipg_dxcodeid1", "ipg_carriername2id", "ipg_memberidnumber2", "ipg_groupidnumber2", "ipg_facility_auth2", "WebResource_Insurance2Information"];


    const homePhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_homephone");
    const workPhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_workphone");
    const cellPhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_cellphone");


    if (formContext.ui.getFormType() == XrmEnum.FormType.Create) {

      homePhone.setRequiredLevel("none");
      workPhone.setRequiredLevel("none");
      cellPhone.setRequiredLevel("none");

      fieldsToHideShow.forEach((field) => {
        const control = <Xrm.Controls.StandardControl>formContext.getControl(field);
        if (control && control.getVisible()) 
          control?.setVisible(false);
          
        const attribute = <Xrm.Attributes.Attribute>formContext.getAttribute(field);
        if (attribute && attribute.getRequiredLevel())
          attribute?.setRequiredLevel("none");
      });

      setSectionVisibility(formContext, "Referral", "Additional_Referral_Info", false);
    }
    else
      if (formContext.ui.getFormType() == XrmEnum.FormType.Update) {

        //check for case created

        fieldsToHideShow.forEach((field) => {
          const control = <Xrm.Controls.StandardControl>formContext.getControl(field);
          if (control && !control.getVisible()) {
            control?.setVisible(true);
          }
        });

        if (homePhone.getValue()) {
          workPhone.setRequiredLevel("none");
          cellPhone.setRequiredLevel("none");
        }
        else if (workPhone.getValue()) {
          homePhone.setRequiredLevel("none");
          cellPhone.setRequiredLevel("none");
        }
        else if (cellPhone.getValue()) {
          homePhone.setRequiredLevel("none");
          workPhone.setRequiredLevel("none");
        }

        setSectionVisibility(formContext, "Referral", "Additional_Referral_Info", true);
      }
  }

  function setSectionVisibility(formContext: Xrm.FormContext, tabId: string, sectionId: string, isVisible: boolean) {
    const tab = formContext.ui.tabs.get(tabId);
    if (tab && tab.sections && tab.sections.getLength() > 0) {
      const section = tab.sections.get(sectionId);

      if (section && section?.setVisible) {
        section?.setVisible(isVisible);
      }
    }
  }

  /**
  * Called on change on Home Phone
  * @function Intake.Referral.onChangeHomePhone
  * @returns {void}
  */
  export function onChangeHomePhone(executionContext: Xrm.Events.SaveEventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    const homePhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_homephone");
    const workPhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_workphone");
    const cellPhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_cellphone");

    if (homePhone.getValue()) {
      workPhone.setRequiredLevel("none");
      cellPhone.setRequiredLevel("none");
    }
    else {
      workPhone.setRequiredLevel("required");
      cellPhone.setRequiredLevel("required");
    }
  }

  /**
  * Called on change on Work Phone
  * @function Intake.Referral.onChangeWorkPhone
  * @returns {void}
  */
  export function onChangeWorkPhone(executionContext: Xrm.Events.SaveEventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    const homePhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_homephone");
    const workPhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_workphone");
    const cellPhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_cellphone");

    if (workPhone.getValue()) {
      homePhone.setRequiredLevel("none");
      cellPhone.setRequiredLevel("none");
    }
    else {
      homePhone.setRequiredLevel("required");
      cellPhone.setRequiredLevel("required");
    }
  }


  /**
   * Called on change on Cell Phone
   * @function Intake.Referral.onChangeCellPhone
   * @returns {void}
  */
  export function onChangeCellPhone(executionContext: Xrm.Events.SaveEventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    const homePhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_homephone");
    const workPhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_workphone");
    const cellPhone = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_cellphone");

    if (cellPhone.getValue()) {
      homePhone.setRequiredLevel("none");
      workPhone.setRequiredLevel("none");
    }
    else {
      homePhone.setRequiredLevel("required");
      workPhone.setRequiredLevel("required");
    }
  }

  /**
  * Called on Dx Code change
  * @function Intake.Referral.OnDxChange
  * @returns {void}
 */
  export function OnDxChange(executionContext) {
    var formContext = executionContext.getFormContext();
    var DxCodeName: string = executionContext.getEventSource().getName();
    var index: number = Number(DxCodeName.substr(DxCodeName.length - 1, 1));
    var DxCodeValue = formContext.getAttribute("ipg_dxcodeid" + index.toString()).getValue();
    formContext.getControl("ipg_dxcodeid" + index.toString()).clearNotification("ipg_dxcodeid" + index.toString() + "dx");

    if (DxCodeValue) {
      for (var i = 1; i <= 6; i++) {
        var currentDxCodeValue = formContext.getAttribute("ipg_dxcodeid" + i.toString()).getValue();

        if ((index != i) && (currentDxCodeValue) && (currentDxCodeValue[0].id == DxCodeValue[0].id)) {
          formContext.getControl("ipg_dxcodeid" + index.toString()).setNotification("This Dx code has been already selected", "ipg_dxcodeid" + index.toString() + "dx");
          formContext.getAttribute("ipg_dxcodeid" + index.toString()).setValue(null);
        }
      }
    }
  }

   

  export function OnPatientZipCodeChange(executionContext: Xrm.Events.EventContext) {  

    var formContext = executionContext.getFormContext();
    var PatientZipCode = formContext.getAttribute("ipg_melissapatientzipcodeid");
    if (PatientZipCode.getValue()) {
      var zipId = PatientZipCode.getValue()[0].id;
      var zipName = PatientZipCode.getValue()[0].name;
      formContext.getAttribute("ipg_displayzipcodename").setValue(zipName);
      formContext.getAttribute("ipg_displayzipcodeid").setValue(zipId);
      Xrm.WebApi.retrieveRecord("ipg_melissazipcode", Intake.Utility.removeCurlyBraces(PatientZipCode.getValue()[0].id), "?$select=ipg_city&$expand=ipg_stateid($select=ipg_name)").then(function success(result) {
        var City = result.ipg_city;
        var State = result.ipg_stateid.ipg_name;
        if (City)
          formContext.getAttribute("ipg_melissapatientcity").setValue(City);
        if (State)
          formContext.getAttribute("ipg_melissapatientstate").setValue(State);
      }, function (error) {
        Xrm.Utility.alertDialog(error.message, null);
      });
    }
    else {
      formContext.getAttribute("ipg_melissapatientstate").setValue(null);
      formContext.getAttribute("ipg_melissapatientcity").setValue(null);
    }
  }
  


  export function isReferralOpened(formContext: Xrm.FormContext): Boolean {
    return formContext.getAttribute("ipg_casestatus")?.getValue() == 923720000; // Opened
  }

  function showHideTabs(formContext: Xrm.FormContext) {
    let environment = Intake.Utility.GetEnvironment();
    if ((environment ?? '').toUpperCase() != 'DEV') {
      let tabToHide = formContext.ui.tabs.get('GatingLogTab');
      if (tabToHide) {
        tabToHide?.setVisible(false);
      }
    }
  }

  function addPhysicianLookupCustomView(formContext: Xrm.FormContext) {
    var physicianControl = <Xrm.Controls.LookupControl>formContext.getControl("ipg_physicianid");
    if (physicianControl) {
      var facilityRef = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_facilityid")?.getValue();
      if (facilityRef && facilityRef.length > 0) {
        var viewId = "00000000-0000-0000-00AA-000010001111";
        var fetchXml = `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">
      <entity name="contact">
        <attribute name="fullname" />
        <attribute name="contactid" />
        <order attribute="fullname" descending="false" />
        <link-entity name="ipg_facilityphysician" from="ipg_physicianid" to="contactid" link-type="inner" alias="ad">
          <filter type="and">
            <condition attribute="ipg_facilityid" operator="eq" value="${facilityRef[0].id}" />
            <condition attribute="ipg_status" operator="eq" value="1" />
          </filter>
        </link-entity>
      </entity>
    </fetch>`;
        var viewDisplayName = "Physicians";
        var layoutXml = `<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'>
      <row name='result' id='contactid'>
      <cell name='fullname' width='300' />
      </row>
      </grid>`;
        physicianControl.addCustomView(viewId, 'contact', viewDisplayName, fetchXml, layoutXml, true);
      }
    }
  }

  /**
  * Called on Facility Name change
  * @function Intake.Referral.OnFacilityLookupChange
  * @returns {void}
 */
  export function OnFacilityLookupChange(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
   // SetPhysicianFromFacility(formContext);
    formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_physicianid")?.setValue(null);
    addPhysicianLookupCustomView(formContext);
  }


  export function SetPhysicianFromFacility(formContext) {


    if (formContext.getAttribute("ipg_facilityid")?.getValue() == null) {
      formContext.getControl("ipg_physicianid")?.setVisible(false);
    }

    else {
      formContext.getControl("ipg_physicianid")?.setVisible(true);

    }
  }

  export function filterEventsLog(executionContext) {
    let formContext = executionContext.getFormContext();
    let gridContext = formContext.getControl("referrals_event_log");

    if (gridContext) {
      let referralId = formContext.data.entity.getId().replace(/{|}/g, "");

      var filterXml =
        "<filter type='and'>" +
        "<condition attribute='ipg_referralid' operator='eq' value='" + referralId + "'/>" +
        "</filter>";

      gridContext.setFilterXml(filterXml);
      formContext.ui.controls.get("referrals_event_log").refresh();
    }
  }

  export function OnCaseStatusChange(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    setCaseStatusDisplayedValue(formContext);
  }

  export function OnCaseStateChange(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    setCaseStatusDisplayedValue(formContext);
  }

  export function OnCaseReasonChange(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    setCaseStatusDisplayedValue(formContext);
  }

  function setCaseStatusDisplayedValue(formContext: Xrm.FormContext) {
    var status = "";
    var caseStatus = formContext.getAttribute("ipg_casestatus")?.getValue();
    var caseState = formContext.getAttribute("ipg_statecode")?.getValue();
    var caseReason = formContext.getAttribute("statuscode")?.getValue();
    if (caseStatus && caseStatus == 923720000) {
      status += "Open in ";
    }
    else if (caseStatus && caseStatus == 923720001) {
      status += "Closed in ";
    }
    if (caseState) {
      switch (caseState) {
        case CaseStates.Intake:
          status += CaseStages.Intake;
          break;
        case CaseStates.Authorizations:
          status += CaseStages.Authorizations;
          break;
        case CaseStates.CaseManagement:
          status += CaseStages.CaseManagement;
          break;
        case CaseStates.Billing:
          status += CaseStages.Billing;
          break;
        case CaseStates.CarrierCollections:
          status += CaseStages.CarrierCollections;
          break;
        case CaseStates.PatientCollections:
          status += CaseStages.PatientCollections;
          break;
        case CaseStates.Finance:
          status += CaseStages.Finance;
          break;
        default:
          break;
      }
    }
    if (caseReason && caseReason == 1) {
      status += " - Active";
    }
    else if (caseReason && caseReason == 2) {
      status += " - Closed";
    }
    formContext.getAttribute("ipg_casestatusdisplayed")?.setValue(status);
  }

  function showTimeLine(formContext: Xrm.FormContext) {
    const associatedCaseAttr = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_associatedcaseid");

    if (!(associatedCaseAttr && associatedCaseAttr.getValue()?.length > 0)) {
      formContext.ui.tabs.get("Timeline").setVisible(true);
    }
  }

  function addCarriersPreSearch(formContext: Xrm.FormContext){
    formContext.getControl<Xrm.Controls.LookupControl>("ipg_carrierid")?.addPreSearch(() => {
      addPrimaryCarrierFilter(formContext);
    });
    formContext.getControl<Xrm.Controls.LookupControl>("ipg_carriername2id")?.addPreSearch(() => {
      addSecondaryCarrierFilter(formContext);
    });
  }

  function addPrimaryCarrierFilter(formContext: Xrm.FormContext){
    var primaryCategory = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_primarycarriercategory")?.getValue();

    var filter = `<filter type='and'>
    <condition attribute='ipg_carriertype' operator='not-in'>
      <value>${CarrierTypes.Auto}</value>
      <value>${CarrierTypes.Government}</value>
      <value>${CarrierTypes.IPA}</value>
    </condition>
    <condition attribute="ipg_carrieraccepted" operator="eq" value="1" />
    ${primaryCategory ? `<condition attribute="ipg_portalparent" operator="eq" value="${primaryCategory}"/>` : ""}
  </filter>`;

    formContext.getControl<Xrm.Controls.LookupControl>("ipg_carrierid").addCustomFilter(filter, "account");
  }

  function addSecondaryCarrierFilter(formContext: Xrm.FormContext){
    var secondaryCategory = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_secondarycarriercategory")?.getValue();

    var filter = `<filter type='and'>
    <condition attribute='ipg_carriertype' operator='not-in'>
      <value>${CarrierTypes.IPA}</value>
      <value>${CarrierTypes.WorkersComp}</value>
    </condition>
    ${secondaryCategory ? `<condition attribute="ipg_portalparent" operator="eq" value="${secondaryCategory}"/>` : ""}
  </filter>`;

    formContext.getControl<Xrm.Controls.LookupControl>("ipg_carriername2id").addCustomFilter(filter, "account");
  }

  async function onChangeOwner(executionContext: Xrm.Events.EventContext)
  {
    const formContext = executionContext.getFormContext();
    const referralid = formContext.data.entity.getId();
    let confirmed = true;

    const ownerattr = (executionContext.getEventSource() as Xrm.Attributes.LookupAttribute);
    const ownerattrValue = ownerattr.getValue();

    if(ownerattrValue?.length > 0)
    {
      if(ownerattrValue[0].entityType == "systemuser")
      {
        var result =  await Xrm.Navigation.openConfirmDialog({text: "System is about to reassign all related open User Tasks to the User you assigned this Referral to. Do you wish to proceed?"});
        Xrm.Utility.showProgressIndicator("");
        confirmed = result.confirmed;   
      }

      if(confirmed)
      {
        try
        {
          await Xrm.WebApi.online.updateRecord("ipg_referral", referralid , {"ownerid@odata.bind": `/${ownerattrValue[0].entityType}s(${ownerattrValue[0].id.replace("{","").replace("}","")})`});
        }
        catch (e)
        {
          Xrm.Navigation.openErrorDialog({message:"Referral has not been ReAssigned. Please try later or contact System Administrator!"});
        }
      }
      else
      {

        var incident = await Xrm.WebApi.online.retrieveRecord("ipg_referral",referralid,"?$select=_ownerid_value");

        ownerattr.setValue([{
          entityType:incident["_ownerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"], 
          id:incident["_ownerid_value"], 
          name:incident["_ownerid_value@OData.Community.Display.V1.FormattedValue"] }]);
      }
    }

    Xrm.Utility.closeProgressIndicator();
  }
}
