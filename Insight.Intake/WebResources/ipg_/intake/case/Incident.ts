/**
 * @namespace Intake.Case
 */

namespace Intake.Case {
  const _insuranceAndBenefitsTabId: string = "InsuranceInformation";

  const _secondaryCarrierSectionId: string = "SecondaryCarrierSection";
  const _secondaryCarrierAuthSectionId: string = "SecondaryCarrierAuthorization";

  enum CaseStages {
    Intake = "fd6ad6ed-19fa-40c0-8995-5b768c63725a",
    Authorizations = "6485894b-12ed-41b3-a0ce-07abc11fb302",
    CaseManagement = "406f25d6-2503-4d55-a464-a95a1c8aaf13",
    Billing = "8c6cfada-32e1-4700-8efc-3c991b06f1f4",
    CarrierCollections = "008063c7-2746-43ce-b1cb-dca6e7620c3b",
    PatientCollections = "2fd06a7e-c253-4d57-9118-4a5c5d6954b5",
    Finance = "f4359293-fb63-4688-87ca-8d15f360049f"
  };

  enum CaseStates {
    Intake = 923720000,
    Authorizations = 923720001,
    CaseManagement = 923720002,
    Billing = 923720003,
    CarrierCollections = 923720004,
    PatientCollections = 923720005,
    Finance = 923720006
  };

  enum PaymentPlanType {
    None = 427880000,
    AutoDraft = 427880001,
    Manual = 427880002
  }

  enum CaseHoldReasonsOptionSetValues {
    CollectionsHold = 11,
    FacilityRecoveryAPInvoiceIssued = 427880009,
    FacilityRecoveryDebitPending = 427880004,
    FacilityRecoveryLetterSent = 427880001,
    FacilityRecoveryManufacturerCreditPending = 427880005,
    FacilityRecoveryResearchApproved = 427880000,
    FacilityRecoveryResearchPending = 427880002,
    FeeScheduleHold = 7,
    IssueHealthPlan = 8,
    IssueRebuttal = 9,
    Other = 1,
    PatientBankruptcy = 3,
    PatientLitigation = 2,
    PatientSettlementPending = 4,
    PendingFacilityContract = 5,
    PostClaimCorrections = 10,
    QueuedForBilling = 6,
    SettlementPending = 12,
    ManagerReview = 427880006,
    PendingCourtesyClaimDocuments = 427880003
  };

  enum CaseHoldReasonsLabels {
    CollectionsHold = "Collections Hold",
    FacilityRecoveryAPInvoiceIssued = "Facility Recovery - AP Invoice Issued",
    FacilityRecoveryDebitPending = "Facility Recovery - Debit Pending",
    FacilityRecoveryLetterSent = "Facility Recovery - Letter Sent",
    FacilityRecoveryManufacturerCreditPending = "Facility Recovery - Manufacturer Credit Pending",
    FacilityRecoveryResearchApproved = "Facility Recovery - Research Approved",
    FacilityRecoveryResearchPending = "Facility Recovery - Research Pending",
    FeeScheduleHold = "Fee Schedule Hold",
    IssueHealthPlan = "Issue - Health Plan",
    IssueRebuttal = "Issue - Rebuttal",
    Other = "Other",
    PatientBankruptcy = "Patient bankruptcy",
    PatientLitigation = "Patient Litigation",
    PatientSettlementPending = "Patient Settlement Pending",
    PendingFacilityContract = "Pending Facility Contract",
    PostClaimCorrections = "Post-Claim Corrections",
    QueuedForBilling = "Queued for Billing",
    SettlementPending = "Settlement Pending",
    ManagerReview = "Manager Review",
    PendingCourtesyClaimDocuments = "Pending Courtesy Claim Documents"
  }
  enum CaseAttributes {
    CarrierCoinsurance = "ipg_payercoinsurance",
    PatientCoinsurance = "ipg_patientcoinsurance"
  }

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

  let _formContext: Xrm.FormContext;

  export async function OnFormLoad(executionContext: Xrm.Events.SaveEventContext) {
    console.log("OnLoadEvent fired");
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    const caseState = formContext.getAttribute("ipg_statecode");
    const lockCase = formContext.getAttribute("ipg_islocked");
    
    ConfigureOwnerField(formContext);
    
    formContext.data.addOnLoad(RefreshOnDataLoad);

    _formContext = formContext;
    formContext.getControl("ipg_patientcity").setDisabled(true);
    formContext.getControl("ipg_patientstate").setDisabled(true);

    formContext.getAttribute("ipg_paymentplantype")?.addOnChange(() => SetPaymentPlanAmountAsRequired(formContext));

    var isStep2 = formContext.data.attributes.get("p_isStep2");
    if (isStep2) {
      showPIFStep2Form(executionContext);
    }

    let lifeSycle = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_lifecyclestepid");
    if (lifeSycle && lifeSycle.getValue()) {
      let lsStep = await Xrm.WebApi.retrieveRecord(lifeSycle.getValue()[0].entityType, lifeSycle.getValue()[0].id, "?$select=_ipg_gateconfigurationid_value");
        if (lsStep && lsStep["_ipg_gateconfigurationid_value"]) {
         await showTabsByGate (formContext, { entityType: "ipg_gateconfiguration", id: lsStep["_ipg_gateconfigurationid_value"], name: "" });
        }
    }

    showWarningMessageIfCaseOnHold(formContext);
    
    addCarriersPreSearch(formContext);
    const carrierAttribute = formContext.getAttribute("ipg_carrierid"); 
    carrierAttribute.addOnChange(()=> {OnChangePrimaryCarrier(formContext)});
    carrierAttribute.fireOnChange();

    if (lifeSycle) {
      lifeSycle.addOnChange(() => ShowAllReceivedField(formContext));
    }

    await setPartsTabVisibility(formContext);
    caseState.addOnChange((event:Xrm.Events.EventContext) => showActualParts(event.getFormContext()));
    showActualParts(formContext);

    ConfigureHeader(formContext);
    ShowAllReceivedField(formContext);
    disableBenefitTypeIfNeeded(formContext);
    showHidePrimaryCarrierFields(formContext);
    ShowSecondaryCarrierSection(executionContext);
    showHideCaseHoldReasonOptions(formContext);
    IfCarrierIsAutoHideBenefitsExhausted(formContext);
    showHideCaseHoldReason(formContext);
    AddValidation(formContext);
    SetRequiredField(formContext);
    LockFieldOnNotCaseSummary(formContext);
    setRequiredActualProcedureDate(formContext);
    HideObsoleteOrDeletedFields(formContext);
    SetBpfStageBasedOnCaseState(formContext);

    const cptAttributeNames =
    ["ipg_cptcodeid1"
      , "ipg_cptcodeid2"
      , "ipg_cptcodeid3"
      , "ipg_cptcodeid4"
      , "ipg_cptcodeid5"
      , "ipg_cptcodeid6"];
  
    if (lockCase) {
      const fieldsToLock = ["ipg_patientid","ipg_carrierid", "ipg_secondarycarrierid", 
        "ipg_memberidnumber", "ipg_procedureid",
        "ipg_cptcodename1", "ipg_cptcodeid1", "ipg_cptcodeid2", "ipg_cptcodeid3",
        "ipg_cptcodeid4", "ipg_cptcodeid5", "ipg_cptcodeid6",
        "ipg_actualdos", "ipg_facilityid", "ipg_physicianid",
        "ipg_dxcodeid1", "ipg_dxcodeid2", "ipg_dxcodeid3",
        "ipg_dxcodeid4", "ipg_dxcodeid5", "ipg_dxcodeid6",
        "ipg_patientfirstname", "ipg_patientfullname", "ipg_patientlastname",
        "ipg_patientmiddlename", "ipg_insuredfirstname", "ipg_insuredlastname",
        "ipg_insuredmiddlename", "ipg_insureddateofbirth", "ipg_insuredgender",
        "ipg_relationtoinsured", "ipg_secondarycarrierrelationtoinsured",
        "ipg_patientgender", "ipg_patientdateofbirth", "ipg_surgerydate",
        "ipg_homeplancarrierid", "ipg_accurate_equipment_list_received",
        "ipg_isallreceiveddate1", "ipg_secondarymemberidnumber",
        "ipg_patientzipcodeidselector", "ownerid", "ipg_primarycarephysicianid", "ipg_casemanagerid",
        "ipg_facility_auth2", "ipg_autodateofincident", "ipg_autoadjustername",
        "ipg_nursecasemgrname", "ipg_nursecasemgrphone", "ipg_is_authorization_required", "ipg_autocarrier"];

      showLockCaseNotification(lockCase.getValue());
      setFieldsLock(fieldsToLock, lockCase.getValue());
    }

    setRequiredSurgicalPhysicianNameAttribute(formContext);
    setReqiredPrimaryCPTAndPrimaryDX(formContext);
    let lastGateRunAttr = formContext.getAttribute("ipg_lastgaterunid");
    lastGateRunAttr.addOnChange(() => {
      setRequiredSurgicalPhysicianNameAttribute(formContext);
      setReqiredPrimaryCPTAndPrimaryDX(formContext);
    });

    setRequiredBilledCPT(formContext);

    for (let attrName of cptAttributeNames) {
      let attr = formContext.getAttribute(attrName);
      attr.addOnChange(() => {setRequiredBilledCPT(formContext);});
    }

    addPhysicianLookupCustomView(formContext);
    var caseStatusAttr = formContext.getAttribute("ipg_casestatus");
    caseStatusAttr?.addOnChange(() => {
      lockAndSetFormNotificationIfClosed(formContext);
      DisplayOwnerField(formContext);
      setVisibilityForCaseAuthButtons(formContext);
    });
    caseStatusAttr?.fireOnChange();

    formContext.getAttribute(CaseAttributes.CarrierCoinsurance)?.addOnChange(()=> OnCoinsuranceChange(formContext));
    formContext.getAttribute(CaseAttributes.PatientCoinsurance)?.addOnChange(()=> OnCoinsuranceChange(formContext));

    let relationToInsuredAttr = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_relationtoinsured");
    relationToInsuredAttr?.addOnChange(() => {
      lockInsuredFields(relationToInsuredAttr,lockCase);
    });
    relationToInsuredAttr?.fireOnChange();

    lockCase?.addOnChange(() => {
      lockInsuredFields(relationToInsuredAttr,lockCase);
    });

    CalculateActualTotalResponsibility(executionContext);
    CalculateTotalReceived(executionContext);
    CalculateTotalAdjustments(executionContext);
    CalculateTotalWriteOff(executionContext);
    CalculateTotalBalance(executionContext);

    var focusOnTab = window.localStorage.getItem('focusOnTab');
    if (focusOnTab != null) { 
      Xrm.Page.ui.tabs.get(focusOnTab).setFocus();
      window.localStorage.removeItem('focusOnTab');
      window.localStorage.removeItem('back');
    }

    formContext.getAttribute("ipg_actualdos")?.fireOnChange();
    
  }
  function OnCoinsuranceChange(formContext: Xrm.FormContext){
    let carrierCoinsurance = formContext.getAttribute(CaseAttributes.CarrierCoinsurance)?.getValue();
    let patientCoinsurance = formContext.getAttribute(CaseAttributes.PatientCoinsurance)?.getValue();
    
    const carrierNotificationId = "CoinsuranceCarrierNotification";
    const patientNotificationId = "CoinsurancePatientNotification";

    const isCoinsuranceValid = () => carrierCoinsurance + patientCoinsurance <= 100 ;
    
    const showSumValidationMessages = () =>{
      formContext.getControl(CaseAttributes.CarrierCoinsurance).setNotification("The sum of Patient Coinsurance and Carrier Coinsurance can not be more than 100", carrierNotificationId);
      formContext.getControl(CaseAttributes.PatientCoinsurance).setNotification("The sum of Patient Coinsurance and Carrier Coinsurance can not be more than 100", patientNotificationId);
    }

    const hideSumValidationMessages = () =>{
      formContext.getControl(CaseAttributes.CarrierCoinsurance).clearNotification(carrierNotificationId);
      formContext.getControl(CaseAttributes.PatientCoinsurance).clearNotification(patientNotificationId);
    }

    !isCoinsuranceValid() ? showSumValidationMessages(): hideSumValidationMessages();
  }
  function lockAndSetFormNotificationIfClosed(formContext: Xrm.FormContext) {
    var status = formContext.getAttribute("ipg_casestatus")?.getValue();
    if (status == 923720001) {
      formContext.ui.setFormNotification("Case is Closed", "WARNING", "closed");
      formContext.ui.tabs.forEach((tab) => {
        tab.setVisible(true);
      });
      formContext.ui.controls.forEach((control: Xrm.Controls.StandardControl) => {
        if (control && control.getDisabled && !control.getDisabled()) {
          control.setDisabled(true);
        }
      });
    }
    else {
      formContext.ui.clearFormNotification("closed");
    }
  }
  export function addCarriersPreSearch(anyContext){
    const formContext: Xrm.FormContext = typeof(anyContext.getFormContext) == 'function'? anyContext.getFormContext() : anyContext
    
    formContext.getControl("ipg_carrierid")
    ?.addPreSearch(() => {
      addPrimaryCarrierFilter(formContext);
    });
    formContext.getControl("ipg_secondarycarrierid")
    ?.addPreSearch(() => {
      addSecondaryCarrierFilter(formContext);
    });
  }

  function addPrimaryCarrierFilter(formContext: Xrm.FormContext){
    var filter = `<filter type='and'>
    <condition attribute='ipg_carriertype' operator='not-in'>
      <value>${CarrierTypes.Auto}</value>
      <value>${CarrierTypes.Government}</value>
      <value>${CarrierTypes.IPA}</value>
    </condition>
    <condition attribute="ipg_carrieraccepted" operator="eq" value="1" />
  </filter>`;
    formContext.getControl<Xrm.Controls.LookupControl>("ipg_carrierid").addCustomFilter(filter, "account");
  }

  function addSecondaryCarrierFilter(formContext: Xrm.FormContext){
    var filter = `<filter type='and'>
    <condition attribute='ipg_carriertype' operator='not-in'>
    <value>${CarrierTypes.IPA}</value>
    <value>${CarrierTypes.WorkersComp}</value>
    </condition>
  </filter>`;
    formContext.getControl<Xrm.Controls.LookupControl>("ipg_secondarycarrierid").addCustomFilter(filter, "account");
  }
  function OnChangePrimaryCarrier(anyContext){
    const formContext: Xrm.FormContext = typeof(anyContext.getFormContext) == 'function'? anyContext.getFormContext() : anyContext
    var carrierVal = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_carrierid")?.getValue();
    if (carrierVal && carrierVal.length > 0) {
      Xrm.WebApi.retrieveRecord(carrierVal[0].entityType, carrierVal[0].id, "?$select=ipg_carriertype")
        .then(carrier => {
          if (carrier["ipg_carriertype"] == CarrierTypes.WorkersComp) //Workers Comp
          {
            if(IsSecondaryCarrierPopulated(formContext)){
              formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_carrierid")?.setValue(null);
              Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "Please remove secondary carrier before set selected carrier as primary" });
            }
            else{
              formContext.getAttribute("ipg_secondarycarrierid")?.setValue(null);
              formContext.getControl<Xrm.Controls.StandardControl>("ipg_secondarycarrierid")?.setDisabled(true);
              formContext.getAttribute("ipg_secondarymemberidnumber")?.setValue("");
              formContext.getControl<Xrm.Controls.StandardControl>("ipg_secondarymemberidnumber")?.setDisabled(true);
            }
            DisplayAddCarrierButton(formContext,'none');
          }
          else {
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_secondarycarrierid")?.setDisabled(false);
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_secondarymemberidnumber")?.setDisabled(false);
            DisplayAddCarrierButton(formContext,'inline');
          }
        });
    }
  }
  function IsSecondaryCarrierPopulated(formContext: Xrm.FormContext){
    return !!formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_secondarycarrierid")?.getValue();
  }

  function DisplayAddCarrierButton(context: Xrm.FormContext, mode: string) {
    let content = context
      .getControl<Xrm.Controls.IframeControl>("WebResource_carriersfilter")
      .getObject();
    if (content) {
      content.contentWindow.document.getElementById("addCarrierButton").style.display = mode;
    }
  }
  function LockFieldOnNotCaseSummary(formContext: Xrm.FormContext) {
    var modifielableFields =
      ["ipg_facilityid"
        , "ipg_cptcodeid"
        , "ipg_billedcptid"
        , "ipg_patientfirstname"
        , "ipg_patientmiddlename"
        , "ipg_patientlastname"
        , "ipg_patientsuffix"
        , "ipg_patientdateofbirth"
        , "ipg_patientgender"
        , "ipg_patientemail"
        , "ipg_patientaddress"
        , "ipg_patientzipcodeid"
        , "ipg_patienthomephone"
        , "ipg_patientworkphone"
        , "ipg_patientcellphone"
        , "ipg_procedureid"
        , "ipg_surgerydate"
        , "ipg_actualdos"
        , "ipg_facilityid"
        , "ipg_physicianid"
        , "ipg_casemgrnameid"
        , "ipg_iscourtesyclaimcase1"];

    var onTabStateChange = (eventContext: Xrm.Events.EventContext) => {
      const tab = (<any>eventContext.getEventSource() as Xrm.Controls.Tab);

      if (tab.getDisplayState() == 'expanded' && tab.getName() != "PatientProcedureDetails") {
        tab.sections.forEach(section => {
          section.controls.forEach(async ctr => {
            if((ctr  as any)?.getAttribute)
            {
            const attrName = (ctr  as Xrm.Controls.StandardControl).getAttribute()?.getName();
            if (attrName === "ipg_iscourtesyclaimcase1"){
              await procesCourtesyCaseFieldAccess(formContext)
            }else if (modifielableFields.find(mf => mf == attrName)) {
              ctr.setDisabled(true);
            }
          }
          });
        })
      }
    }

    formContext.ui.tabs.forEach(tab => {
      tab.addTabStateChange(onTabStateChange);

    })
  }

  function HideObsoleteOrDeletedFields(formContext: Xrm.FormContext) {
    let obsoleteFields = ["ipg_cptcategorycode"
      , "ipg_billedcptid"
      , "ipg_casemanagerid"
      , "ipg_cmassignedid"
      , "ipg_facilitycimid"
      , "ipg_facilitymddid"
      ,"ipg_carriernetwork"
    ]

    obsoleteFields.forEach(field => formContext.getControl(field)?.setVisible(false));
  }

  function DisplayOwnerField(formContext: Xrm.FormContext) {
    const ownerControl = formContext.getControl("ownerid");
    if (isCaseOpened(formContext)) {
      ownerControl?.setDisabled(false);
    } else {
      ownerControl?.setDisabled(true);
    }
  }
  function isCaseOpened(formContext: Xrm.FormContext): Boolean {
    return formContext.getAttribute("ipg_casestatus")?.getValue() == 923720000; // Opened
  }
  function SetRequiredField(formContext: Xrm.FormContext) {
    let requiredField = ["ipg_patientfirstname"
      , "ipg_patientlastname"
      , "ipg_patientdateofbirth"
      , "ipg_patientgender"
      , "ipg_patientaddress"
      , "ipg_patientzipcodeid"
      , "ipg_procedureid"
      , "ipg_surgerydate"
      , "ipg_facilityid"
      , "ipg_physicianid"
      , "ipg_casemgrnameid"
      , "ownerid"
      , "ipg_iscourtesyclaimcase"]

    requiredField.forEach(field => formContext.getAttribute(field)?.setRequiredLevel('required'));
  }

  function setDisabledField(formContext: Xrm.FormContext) {
    let disabledField = ["ipg_iscourtesyclaimcase"]

    disabledField.forEach(field => formContext.getControl(field)?.setDisabled(true));
  }

  function ConfigureHeader(formContext: Xrm.FormContext) {
    const actDOSattr = formContext.getAttribute("ipg_actualdos");
    actDOSattr?.addOnChange(SetVisabilityOfProcedureDate);
    actDOSattr?.fireOnChange();

    const firstname = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_patientfirstname");
    const mi = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_patientmiddlename");
    const lastname = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_patientlastname");

    firstname?.addOnChange(CalculatePatientStatementOnHeader);
    mi?.addOnChange(CalculatePatientStatementOnHeader);
    lastname?.addOnChange(CalculatePatientStatementOnHeader);
    firstname?.fireOnChange();
  }

  async function setPartsTabVisibility(formContext: Xrm.FormContext) {
    const caseid = formContext.data.entity.getId();
      let partsTab = formContext.ui.tabs.get("PartsTab");
      if (partsTab) {
          partsTab.setVisible(true);
        }
        if(caseid)
        {
          const estimatedParts = await Xrm.WebApi.retrieveMultipleRecords("ipg_estimatedcasepartdetail",`?$select=ipg_estimatedcasepartdetailid&$filter=_ipg_caseid_value eq ${caseid} and ipg_purchaseorderid ne null`);
          
          if(estimatedParts.entities.length > 0)
          {
            formContext.ui.tabs.get("Orders")?.setVisible(true);
          }
        }
      }  

  function showHideCaseHoldReason(formContext: Xrm.FormContext) {
    var HoldAttr = formContext.getAttribute<Xrm.Attributes.BooleanAttribute>("ipg_casehold");
    HoldAttr?.addOnChange(eventContext => {
      const formContext = eventContext.getFormContext();
      var caseOnHoldAttr = formContext.getAttribute<Xrm.Attributes.BooleanAttribute>('ipg_casehold');
      var HoldReasonCtr = formContext.getControl<Xrm.Controls.OptionSetControl>("header_ipg_caseholdreason");
      if (caseOnHoldAttr?.getValue()) {
        HoldReasonCtr?.setVisible(true);
      }
      else {
        HoldReasonCtr?.setVisible(false);
      }
      showWarningMessageIfCaseOnHold(formContext);
    });

    HoldAttr?.fireOnChange();
  }

  export function OnPlanTypeChange(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    if (formContext) {
      showHidePrimaryCarrierFields(formContext);
    }
  }

  export function ShowSecondaryCarrierSection(executionContext: Xrm.Events.EventContext) {
    const formContext: Xrm.FormContext = executionContext.getFormContext();

    const isSecondaryCarrier = formContext.getAttribute("ipg_secondarycarrier")?.getValue();

    setSectionVisible(formContext, _insuranceAndBenefitsTabId, _secondaryCarrierSectionId, isSecondaryCarrier);
    setSectionVisible(formContext, _insuranceAndBenefitsTabId, _secondaryCarrierAuthSectionId, isSecondaryCarrier);
  }

  export function SetPaymentPlanAmountAsRequired(formContext: Xrm.FormContext) {
    const paymentPlanType = formContext.getAttribute("ipg_paymentplantype");
    const paymentPlanAmount = formContext.getAttribute("ipg_paymentplanamount");

    if (paymentPlanType?.getValue() == PaymentPlanType.None) {
      paymentPlanAmount.setRequiredLevel("none");
    }
    else {
      paymentPlanAmount.setRequiredLevel("required");
    }
  }

  function ShowAllReceivedField(formContext: Xrm.FormContext) {
    const lifecycleValue = formContext.getAttribute("ipg_lifecyclestepid")
      ? formContext.getAttribute("ipg_lifecyclestepid").getValue() : null;
    const lifecycleValueId = lifecycleValue ? lifecycleValue[0].id.replace(/[{}]/g, "").toLowerCase() : null;
    if (!lifecycleValueId) {
      return;
    }
    const settingName: string = 'Case.LifecycleStepsForAllReceivedField';
    Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting",
      `?$select=ipg_value&$filter=ipg_name eq '${settingName}'`)
      .then((result: Xrm.RetrieveMultipleResult) => {
        if (result.entities.length > 0 && result.entities[0].ipg_value) {
          var lifecycleSteps = String(result.entities[0].ipg_value).split(";");
          //formContext.getControl("ipg_isallreceived").setVisible(lifecycleSteps.indexOf(lifecycleValueId) !== -1);
        }
      },
        function (error) {
          console.error(error.message);
        }
      );
  }

  export function OnSave(executionContext: Xrm.Events.SaveEventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    var caseStatus = formContext.getAttribute("ipg_casestatus")?.getValue();
    if (caseStatus && caseStatus == 923720001) {
      if (executionContext.getEventArgs().getSaveMode() == 70 || executionContext.getEventArgs().getSaveMode() == 2) {
        executionContext.getEventArgs().preventDefault();
        return;
      }
      executionContext.getEventArgs().preventDefault();
      Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "You are not able to save closed case." }).then(() => {
        return;
      });
    }

    let casePartDetailsControl = formContext.getControl("ActualParts");
    if (casePartDetailsControl) {
      casePartDetailsControl.refreshRibbon();
    }

    let estimatedPartsControl = formContext.getControl("EstimatedParts");
    if (estimatedPartsControl) {
      estimatedPartsControl.refreshRibbon();
    }

    let purchaseOrderDetailsControl = formContext.getControl("PurchaseOrders");
    if (purchaseOrderDetailsControl) {
      purchaseOrderDetailsControl.refreshRibbon();
    }

    setRequiredActualProcedureDate(formContext);
    SetRequiredField(formContext);
    setDisabledField(formContext);
  }

  function AddValidation(formContext: Xrm.FormContext) {
    formContext.getAttribute("ipg_facilitymrn")?.addOnChange(AllowOnlyDigit(25));
    formContext.getAttribute("ipg_externalid")?.addOnChange(AllowOnlyDigit());
  }
  function AllowOnlyDigit(length?: number) {
    var pattern = new RegExp(`^[\\d]${length ? `{1,${length}}` : "*"}$`);
    return (getEventSource: Xrm.Events.EventContext) => validateAttributeByRegExp(getEventSource.getEventSource() as Xrm.Attributes.StringAttribute, pattern, "Should be numeric characters!");
  }

  function validateAttributeByRegExp(attr: Xrm.Attributes.StringAttribute, expression: RegExp, messages: string) {
    const currentValue = attr.getValue();
    const control = attr.controls.get()[0];
    const notifactionid = `validationError`;
    if (!expression.test(currentValue)) {
      control.setNotification(messages, notifactionid);
    } else {
      control.clearNotification(notifactionid);
    }
  }
  function SetCasePartTabVisibility() {
    let formContext = Xrm.Page;
    //if Case currently in Authorization and procedure is scheduled in the future show parts tab to allow add TPO Part

    let lifecycleValue = formContext.getAttribute("ipg_lifecyclestepid") ? formContext.getAttribute("ipg_lifecyclestepid").getValue() : null;
    let DOSValue = formContext.getAttribute("ipg_surgerydate") ? formContext.getAttribute("ipg_surgerydate").getValue() : null;

    if (lifecycleValue && lifecycleValue.length > 0
      && lifecycleValue[0].name === "Authorization" && DOSValue && DOSValue.setHours(0, 0, 0, 0) > new Date().setHours(0, 0, 0, 0)) {
      let parttab = formContext.ui.tabs.get("PartsTab");
      if (parttab) {
        parttab.setVisible(true);
      }
    }
  }
  function showPIFStep2Form(executionContext: Xrm.Events.SaveEventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    let visibleTabs = ["ReferralData", "PatientProcedureDetails", "InsuranceInformation", "Documents", "DebugView", "EventLog"];

    formContext.ui.tabs.forEach(function (tab) {
      if (visibleTabs.indexOf(tab.getName()) === -1) {
        tab.setVisible(false);
      }
      if (tab.getName() === "PatientProcedureDetails") {
        let purchaseOrderProductsSection = tab.sections.get("PurchaseOrderProducts");
        if (purchaseOrderProductsSection) {
          purchaseOrderProductsSection.setVisible(false);
        }
      }
    });
  }

  export async function showTabsByGate(formContext: Xrm.FormContext, gateConfig: Xrm.LookupValue) {
    const result = await Xrm.WebApi.retrieveRecord(gateConfig.entityType, gateConfig.id, "?$select=ipg_visibletabsoncaseform");
        if (result && result.ipg_visibletabsoncaseform) {
          let visibleTabs = JSON.parse(result.ipg_visibletabsoncaseform);
          formContext.ui.tabs.forEach(function (tab) {
            let tabIsVisible = visibleTabs.indexOf(tab.getName()) !== -1;
            if (tabIsVisible) {
              tab.setVisible(true);
            }
            else {
              tab.setVisible(false);
            }
          });
        }
        else {
          formContext.ui.tabs.forEach(function (tab) {
            tab.setVisible(true);
          });
        }

        let eventLogTab = formContext.ui.tabs.get("EventLog");
        if (eventLogTab) {
          eventLogTab.setVisible(true);
        }
        setPartsTabVisibility(formContext);
      }

  function disableBenefitTypeIfNeeded(formContext: Xrm.FormContext): void {

    let stateCodeAttribute: Xrm.Attributes.Attribute = formContext.getAttribute('ipg_statecode');
    if (stateCodeAttribute) {
      const intakeStateCodeValue: Number = 923720000;
      const authorizationStateCodeValue: Number = 923720001;

      let stateCodeValue: Number = stateCodeAttribute.getValue();
      if (stateCodeValue && stateCodeValue != intakeStateCodeValue
        && stateCodeValue != authorizationStateCodeValue) {

        let useDmeControl: Xrm.Controls.Control = formContext.getControl('ipg_usedmebenefits');
        if (useDmeControl) {
          useDmeControl.setDisabled(true);
        }
        let individualFamilyControl: Xrm.Controls.Control = formContext.getControl('ipg_individualorfamilybenefits');
        if (individualFamilyControl) {
          individualFamilyControl.setDisabled(true);
        }
        let inOutNetworkControl: Xrm.Controls.Control = formContext.getControl('ipg_inoutnetwork');
        if (inOutNetworkControl) {
          inOutNetworkControl.setDisabled(true);
        }
      }
    }
  }

  function showHidePrimaryCarrierFields(formContext: Xrm.FormContext) {

    const autoNoFaultPlanTypeValue: number = 923720008;
    const workersCompPlanTypeValue: number = 923720004;

    let planTypeAttribute: Xrm.Attributes.Attribute = formContext.getAttribute('ipg_primarycarrierplantype');
    if (planTypeAttribute) {
      let planTypeValue: number = <number>planTypeAttribute.getValue();
      let isAutoOrWorkersComp: boolean = planTypeValue == autoNoFaultPlanTypeValue
        || planTypeValue == workersCompPlanTypeValue;

      const tabId = 'InsuranceInformation';
      const sectionId = 'PrimaryCarrierSection';

      //show these fields
      setControlVisible(formContext, tabId, sectionId, "ipg_autoclaimnumber", isAutoOrWorkersComp);
      setControlVisible(formContext, tabId, sectionId, "ipg_autodateofincident", isAutoOrWorkersComp);
      setControlVisible(formContext, tabId, sectionId, "ipg_autoadjustername", isAutoOrWorkersComp);
      setControlVisible(formContext, tabId, sectionId, "ipg_adjusterphone", isAutoOrWorkersComp);
      setControlVisible(formContext, tabId, sectionId, "ipg_billingfax", isAutoOrWorkersComp);

      //hide these fields
      setControlVisible(formContext, tabId, sectionId, "ipg_memberidnumber", isAutoOrWorkersComp == false);
      setControlVisible(formContext, tabId, sectionId, "ipg_primarycarriergroupidnumber", isAutoOrWorkersComp == false);
      setControlVisible(formContext, tabId, sectionId, "ipg_homeplancarrierid", isAutoOrWorkersComp == false);
      setControlVisible(formContext, tabId, sectionId, "ipg_ipapayerid", isAutoOrWorkersComp == false);
      setControlVisible(formContext, tabId, sectionId, "ipg_plansponsor", isAutoOrWorkersComp == false);
      setControlVisible(formContext, tabId, sectionId, "ipg_primarycarriereffectivedate", isAutoOrWorkersComp == false);
      setControlVisible(formContext, tabId, sectionId, "ipg_primarycarrierexpirationdate", isAutoOrWorkersComp == false);
    }
  }

  function setSectionVisible(formContext: Xrm.FormContext, tabId: string, sectionId: string, isVisible: boolean) {
    const tab = formContext.ui.tabs.get(tabId);
    if (tab && tab.sections && tab.sections.getLength() > 0) {
      const section = tab.sections.get(sectionId);

      if (section && section.setVisible) {
        section.setVisible(isVisible);
      }
    }
  }

  function setControlVisible(formContext: Xrm.FormContext, tabId: string, sectionId: string, fieldName: string, isVisible: boolean) {
    var tabObj = formContext.ui.tabs.get(tabId);
    if (tabObj) {
      var sectionObj = tabObj.sections.get(sectionId);
      if (sectionObj) {
        let attribute = formContext.getAttribute(fieldName);
        if (attribute) {
          let attributeControls = attribute.controls.get();
          for (const c of attributeControls) {
            let sectionControl = sectionObj.controls.get(c.getName());
            if (sectionControl) {
              sectionControl.setVisible(isVisible);
            }
          }
        }
      }
    }
  }

  export function OnCaseStateChange(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    if (formContext) {
      showHideCaseHoldReasonOptions(formContext);
      setPartsTabVisibility(formContext);
      SetBpfStageBasedOnCaseState(formContext);
    }
  }

  function showHideCaseHoldReasonOptions(formContext: Xrm.FormContext) {
    //Parent optionset value
    let parCaseStateCode: Xrm.Attributes.Attribute = formContext.getAttribute('ipg_statecode');


    //Child optionset control/options
    let chCaseStateControl: Xrm.Controls.OptionSetControl = formContext.getControl('ipg_caseholdreason');
    let chCaseStateControlOptions: Xrm.OptionSetValue[] = chCaseStateControl.getAttribute().getOptions();


    if (parCaseStateCode) {
      let parCaseStateCodeValue: Number = parCaseStateCode.getValue();

      if (parCaseStateCodeValue == 923720001) {
        //Authorization
        chCaseStateControl.clearOptions();

        for (var i = 1; i < chCaseStateControlOptions.length; i++) {
          if (chCaseStateControlOptions[i].value == 1 || chCaseStateControlOptions[i].value == 5) {
            chCaseStateControl.addOption(chCaseStateControlOptions[i]);
          }
        }
      }
      else
        if (parCaseStateCodeValue == 923720002) {
          //Case Management
          chCaseStateControl.clearOptions();

          for (var i = 1; i < chCaseStateControlOptions.length; i++) {
            if (chCaseStateControlOptions[i].value == 1 || chCaseStateControlOptions[i].value == 6 || chCaseStateControlOptions[i].value == 7) {
              chCaseStateControl.addOption(chCaseStateControlOptions[i]);
            }
          }
        }
        else
          if (parCaseStateCodeValue == 923720004) {
            //Carrier Services
            chCaseStateControl.clearOptions();

            for (var i = 1; i < chCaseStateControlOptions.length; i++) {
              if (chCaseStateControlOptions[i].value == 8 || chCaseStateControlOptions[i].value == 9 || chCaseStateControlOptions[i].value == 10 || chCaseStateControlOptions[i].value == 11) {
                chCaseStateControl.addOption(chCaseStateControlOptions[i]);
              }
            }
          }
          else
            if (parCaseStateCodeValue == 923720005) {
              //Patient Services
              chCaseStateControl.clearOptions();

              for (var i = 1; i < chCaseStateControlOptions.length; i++) {
                if (chCaseStateControlOptions[i].value == 1 || chCaseStateControlOptions[i].value == 11 || chCaseStateControlOptions[i].value == 12) {
                  chCaseStateControl.addOption(chCaseStateControlOptions[i]);
                }
              }
            }
            else {
              chCaseStateControl.clearOptions();
            }
    }
  }





  function IfCarrierIsAutoHideBenefitsExhausted(formContext: Xrm.FormContext) {
    let lkpCarrier = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_carrierid");
    if (lkpCarrier && lkpCarrier.getValue()) {
      let control = formContext.getControl("ipg_medicalbenefitsexhausted");
      if (control) {
        Xrm.WebApi.retrieveRecord("account", lkpCarrier.getValue()[0].id, "?$select=ipg_carriertype").then((result) => {
          if (result && result.ipg_carriertype) {
            if (result.ipg_carriertype == 427880000) {
              control.setVisible(true);
            }
            else {
              control.setVisible(false);
            }
          }
        });
      }
    }
  }

  function showLockCaseNotification(isLock: boolean) {
    const lockCaseNotificationId = "isLockedNotification";
    if (isLock) {
      _formContext.ui.setFormNotification("Case is Locked", "WARNING", lockCaseNotificationId);
    }
    else {
      _formContext.ui.clearFormNotification(lockCaseNotificationId);
    }
  }

  function setFieldsLock(fieldNames: string[] | string, isDisabled?: boolean) {
    if (!fieldNames) return;
    isDisabled = isDisabled ?? true;

    if (Array.isArray(fieldNames)) {
      fieldNames.forEach((fieldName: string) => {
        _formContext.getAttribute(fieldName)
        ?.controls?.forEach(c =>c.setDisabled(isDisabled));
      });
    }
    else if (typeof fieldNames === "string")
    {
      _formContext.getAttribute(fieldNames)?.controls?.forEach(c=>c.setDisabled(isDisabled));
    }
  }

  function showProcedureDateInHeader(formContext: Xrm.FormContext) {
    let scheduledDateAttr = formContext.getAttribute("ipg_surgerydate");
    let headerScheduledDateControl = formContext.getControl("header_ipg_surgerydate");
    let actualDateAttr = formContext.getAttribute("ipg_actualdos");
    let headerActualDateControl = formContext.getControl("header_ipg_actualdos");
    if (scheduledDateAttr && actualDateAttr
      && headerScheduledDateControl && headerActualDateControl) {
      if (actualDateAttr.getValue()) {
        headerActualDateControl.setVisible(true);
        headerScheduledDateControl.setVisible(false);
      }
      else {
        headerActualDateControl.setVisible(false);
        headerScheduledDateControl.setVisible(true);
      }
    }
  }

  function setRequiredActualProcedureDate(formContext: Xrm.FormContext) {
    let actualDateAttr = formContext.getAttribute("ipg_actualdos");
    if (actualDateAttr) {
      Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail",
        "?$select=ipg_casepartdetailid&$filter=statecode eq 0 and ipg_caseid/incidentid eq " + formContext.data.entity.getId().replace(/{|}/g, ""))
        .then(
          function success(result) {
            if (result.entities.length > 0) {
              actualDateAttr.setRequiredLevel("required");
            }
            else {
              actualDateAttr.setRequiredLevel("none");
            }
          },
          function (error) {
            console.log(error.message);
          }
        );
    }
  }

  function callDeriveHomePlanAction(formContext: Xrm.FormContext): string | null | undefined {
    let outputResult: string | null | undefined;
    const carrierid = formContext?.getAttribute("ipg_carrierid")?.getValue();
    if (formContext?.getAttribute("ipg_memberidnumber")?.getIsDirty() ||
      formContext?.getAttribute("ipg_carrierid")?.getIsDirty()) {
      var parameters = {
        "Target": {
          "incidentid": formContext.data.entity.getId(),
          "@odata.type": "Microsoft.Dynamics.CRM." + formContext.data.entity.getEntityName()
        },
        "MemberIdNumber": formContext?.getAttribute("ipg_memberidnumber")?.getValue(),
        "CarrierRef": carrierid
          ? {
            "accountid": carrierid[0]?.id,
            "@odata.type": "Microsoft.Dynamics.CRM.account"
            }
          : null
      };
      callAction("ipg_IPGGatingDeriveHomePlanPlugin", parameters, false,
        function (results) {
          if (!results.Succeeded && results.CaseNote)
            outputResult = results.CaseNote;
        });
    }
    return outputResult;
  }

  function callAction(actionName, parameters, async, successCallback) {
    var req = new XMLHttpRequest();
    req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/" + actionName, async);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {
      if (this.readyState === 4) {
        req.onreadystatechange = null;
        if (this.status === 200) {
          successCallback(JSON.parse(this.response));
        } else {
          Xrm.Utility.closeProgressIndicator();
          alert(JSON.parse(this.response).error.message);
        }
      }
    };
    req.send(JSON.stringify(parameters));
  }

  function SetVisabilityOfProcedureDate(executionContext: Xrm.Events.EventContext) {
    const formContext: Xrm.FormContext = executionContext.getFormContext();
    const headerPrefix = "header_";
    const actDOSattr = formContext.getAttribute("ipg_actualdos");
    const actDOSheaderctr = formContext.getControl(headerPrefix + "ipg_actualdos");
    const scheduledDOSheaderctr = formContext.getControl(headerPrefix + "ipg_surgerydate");

    if (actDOSattr?.getValue()) {
      actDOSheaderctr?.setVisible(true);
      scheduledDOSheaderctr?.setVisible(false);
    }
    else {
      actDOSheaderctr?.setVisible(false);
      scheduledDOSheaderctr?.setVisible(true);
    }
  }

  function CalculatePatientStatementOnHeader(executionContext: Xrm.Events.EventContext) {
    const formContext: Xrm.FormContext = executionContext.getFormContext();
    const calculatedField = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_patientnamecalculated");

    if (calculatedField) {
      calculatedField.setSubmitMode("never");

      const firstName = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_patientfirstname")?.getValue();
      const mi = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_patientmiddlename")?.getValue()
      const lastname = formContext.getAttribute<Xrm.Attributes.StringAttribute>("ipg_patientlastname")?.getValue();

      const calculatedValue = `${firstName}${mi ? ' ' + mi : ''}${lastname ? ' ' + lastname : ''}`;

      if (calculatedField?.getValue() != calculatedValue) {
        calculatedField?.setValue(calculatedValue);
      }
    }
  }

  function setRequiredSurgicalPhysicianNameAttribute(formContext: Xrm.FormContext) {
    //set required Surgical Physician Name attribute if next gate is "Gate 3"
    setRequiredFieldToPassGate(formContext, "ipg_physicianid", 3);
  }

  function setRequiredBilledCPT(formContext: Xrm.FormContext) {
    let billedCptAttr = formContext.getAttribute("ipg_billedcptid");
    let cptAttributeNames =
    ["ipg_cptcodeid1"
      , "ipg_cptcodeid2"
      , "ipg_cptcodeid3"
      , "ipg_cptcodeid4"
      , "ipg_cptcodeid5"
      , "ipg_cptcodeid6"];
    
    if (!billedCptAttr) return;
    billedCptAttr.setRequiredLevel("none");
    for (let attrName of cptAttributeNames) {
      let attr = formContext.getAttribute(attrName);
      if (attr) {
        let cptValue = attr.getValue();
        if (cptValue && cptValue.length > 0) {
          billedCptAttr.setRequiredLevel("required");
          return;
        }
      }
    }
  }

  function setReqiredPrimaryCPTAndPrimaryDX(formContext: Xrm.FormContext) {
    //set required Primary CPT and Primary DX attributes if next gate is "Gate 6"
    setRequiredFieldToPassGate(formContext, "ipg_cptcodeid1", 6);
    setRequiredFieldToPassGate(formContext, "ipg_dxcodeid1", 6);
  }
  //common function to set field required to pass gates
  function setRequiredFieldToPassGate(formContext: Xrm.FormContext, attrName: string, gateNumber: number) {
    let gateConfigurationAttr = formContext.getAttribute("ipg_gateconfigurationid");
    if (gateConfigurationAttr && gateConfigurationAttr.getValue()) {
      let gateConfiguration = gateConfigurationAttr.getValue()[0];
      let attribute = formContext.getAttribute(attrName);
      if (attribute && gateConfiguration.name === ("Gate " + gateNumber)) {
        attribute.setRequiredLevel("required");
      }
      else {
        attribute.setRequiredLevel("none");
      }
    }
  }

  function addPhysicianLookupCustomView(formContext: Xrm.FormContext) {
    var physicianControl = <Xrm.Controls.LookupControl>formContext.getControl("ipg_physicianid");
    var primaryCarePhysicianControl = <Xrm.Controls.LookupControl>formContext.getControl("ipg_primarycarephysicianid");
    if (physicianControl) {
      var facilityRef = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_facilityid")?.getValue();
      if (facilityRef && facilityRef.length > 0) {
        var viewId = "00000000-0000-0000-00AA-000010001111";
        var fetchXml = `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">
                          <entity name="contact">
                           <attribute name="fullname" />
                           <attribute name="contactid" />
                           <order attribute="fullname" descending="false" />
                            <filter type="or" >
                              <condition attribute="ipg_currentfacilityid" operator="eq" value="${facilityRef[0].id}" />
                              <filter type="and" >
                                <condition entityname="facilityPhysician" attribute="ipg_facilityid" operator="eq" value="${facilityRef[0].id}" />
                                <condition entityname="facilityPhysician" attribute="ipg_status" operator="eq" value="1" />
                              </filter>
                            </filter>
                            <link-entity name="ipg_facilityphysician" from="ipg_physicianid" to="contactid" link-type="outer" alias="facilityPhysician" />
                          </entity>
                        </fetch>`;
        var viewDisplayName = "Physicians";
        var layoutXml = `<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'>
      <row name='result' id='contactid'>
      <cell name='fullname' width='300' />
      </row>
      </grid>`;
        physicianControl.addCustomView(viewId, 'contact', viewDisplayName, fetchXml, layoutXml, true);
        primaryCarePhysicianControl?.addCustomView(viewId, 'contact', viewDisplayName, fetchXml, layoutXml, true);
      }
    }
  }

  /**
  * Called on Facility Name change
  * @function Intake.Case.OnFacilityLookupChange
  * @returns {void}
 */
  export function OnFacilityLookupChange(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_physicianid")?.setValue(null);
    addPhysicianLookupCustomView(formContext);
  }

  export function OnExpandCollectionTab(eventcontext: Xrm.Events.StageChangeEventContext) {
    if (eventcontext) {
      const tab = (<any>eventcontext.getEventSource() as Xrm.Controls.Tab);

      if (tab.getDisplayState() == 'expanded') {
        const formContext = eventcontext.getFormContext();
        const arSummary = <any>formContext.getControl("WebResource_ARSummary");

        if (arSummary) {
          var obj = arSummary.getObject();
          if (obj) {
            let src = obj.src;
            obj.src = "about:blank";
            obj.src = src;
          }
        }
      }
    }
  }

  export function filterEventsLog(executionContext) {
    let formContext = executionContext.getFormContext();
    let gridContext = formContext.getControl("cases_event_log");

    if (gridContext) {
      let caseId = formContext.data.entity.getId().replace(/{|}/g, "");

      var filterXml =
        "<filter type='and'>" +
        "<condition attribute='ipg_caseid' operator='eq' value='" + caseId + "'/>" +
        "</filter>";

      gridContext.setFilterXml(filterXml);
      formContext.ui.controls.get("cases_event_log").refresh();
    }
  }

  function lockInsuredFields(relationToInsuredAttr: Xrm.Attributes.OptionSetAttribute, lockCase: Xrm.Attributes.Attribute<any>) {
    let fieldsToLock = ["ipg_insuredfirstname", "ipg_insuredmiddlename", "ipg_insuredlastname",
      "ipg_insureddateofbirth", "ipg_insuredgender", "ipg_insuredaddress", "ipg_insuredzipcodeid",
      "ipg_insuredphone"];
    let relationToInsured = relationToInsuredAttr?.getValue();
    if (relationToInsured && relationToInsured == 100000000 && !lockCase?.getValue()) {
      setFieldsLock(fieldsToLock, true);  
    }
    else {
      setFieldsLock(fieldsToLock, false);
    }
  }

  function showWarningMessageIfCaseOnHold(formContext: Xrm.FormContext){
    const caseHold = formContext.getAttribute<Xrm.Attributes.BooleanAttribute>("ipg_casehold");
    if (caseHold && caseHold.getValue()) {
      let caseHoldReason = formContext.getAttribute("ipg_caseholdreason")?.getValue();
      let warningMessage = "Case on Hold - ";
      switch (caseHoldReason) {
        case CaseHoldReasonsOptionSetValues.CollectionsHold:
          warningMessage += CaseHoldReasonsLabels.CollectionsHold;
          break;
        case CaseHoldReasonsOptionSetValues.FacilityRecoveryAPInvoiceIssued:
          warningMessage += CaseHoldReasonsLabels.FacilityRecoveryAPInvoiceIssued;
          break;
        case CaseHoldReasonsOptionSetValues.FacilityRecoveryDebitPending:
          warningMessage += CaseHoldReasonsLabels.FacilityRecoveryDebitPending;
          break;
        case CaseHoldReasonsOptionSetValues.FacilityRecoveryLetterSent:
          warningMessage += CaseHoldReasonsLabels.FacilityRecoveryLetterSent
          break;
        case CaseHoldReasonsOptionSetValues.FacilityRecoveryManufacturerCreditPending:
          warningMessage += CaseHoldReasonsLabels.FacilityRecoveryManufacturerCreditPending;
          break;
        case CaseHoldReasonsOptionSetValues.FacilityRecoveryResearchApproved:
          warningMessage += CaseHoldReasonsLabels.FacilityRecoveryResearchApproved;
          break;
        case CaseHoldReasonsOptionSetValues.FacilityRecoveryResearchPending:
          warningMessage += CaseHoldReasonsLabels.FacilityRecoveryResearchPending;
          break;
        case CaseHoldReasonsOptionSetValues.FeeScheduleHold:
          warningMessage += CaseHoldReasonsLabels.FeeScheduleHold;
          break;
        case CaseHoldReasonsOptionSetValues.IssueHealthPlan:
          warningMessage += CaseHoldReasonsLabels.IssueHealthPlan;
          break;
        case CaseHoldReasonsOptionSetValues.IssueRebuttal:
          warningMessage += CaseHoldReasonsLabels.IssueRebuttal;
          break;
        case CaseHoldReasonsOptionSetValues.Other:
          warningMessage +=  CaseHoldReasonsLabels.Other;
          break;
        case CaseHoldReasonsOptionSetValues.PatientBankruptcy:
          warningMessage += CaseHoldReasonsLabels.PatientBankruptcy;
          break;
        case CaseHoldReasonsOptionSetValues.PatientLitigation:
          warningMessage += CaseHoldReasonsLabels.PatientLitigation;
          break;
        case CaseHoldReasonsOptionSetValues.PatientSettlementPending:
          warningMessage += CaseHoldReasonsLabels.PatientSettlementPending;
          break;
        case CaseHoldReasonsOptionSetValues.PendingFacilityContract:
          warningMessage += CaseHoldReasonsLabels.PendingFacilityContract;
          break;
        case CaseHoldReasonsOptionSetValues.PostClaimCorrections:
          warningMessage += CaseHoldReasonsLabels.PostClaimCorrections;
          break;
        case CaseHoldReasonsOptionSetValues.QueuedForBilling:
          warningMessage += CaseHoldReasonsLabels.QueuedForBilling;
          break;
        case CaseHoldReasonsOptionSetValues.SettlementPending:
          warningMessage += CaseHoldReasonsLabels.SettlementPending;
          break;
        case CaseHoldReasonsOptionSetValues.ManagerReview:
          warningMessage += CaseHoldReasonsLabels.ManagerReview;
          break;
        case CaseHoldReasonsOptionSetValues.PendingCourtesyClaimDocuments:
          warningMessage += CaseHoldReasonsLabels.PendingCourtesyClaimDocuments;
          break;
        default:
          warningMessage = "Case on Hold."
          break;
      }
      formContext.ui.setFormNotification(warningMessage, "WARNING", "casehold");
    } 
    else {
      formContext.ui.clearFormNotification("casehold");
    }
  }

  export function OnCaseReasonsChange(executionContext: Xrm.Events.EventContext){
    var formContext = executionContext.getFormContext();
  }

  function showActualParts(formContext: Xrm.FormContext)
  {
    var caseState = formContext.getAttribute("ipg_statecode")?.getValue();
    
    let actualPartsSections = formContext.ui.tabs.get("PartsTab")?.sections.get("ActualPartSection");
    actualPartsSections?.setVisible(caseState !== CaseStates.Authorizations);
  }

  function setVisibilityForCaseAuthButtons(formContext: Xrm.FormContext){
    var isVisible = formContext.getAttribute("ipg_casestatus")?.getValue() != 923720001;
    formContext.getControl("WebResource_authorizationbuttons").setVisible(isVisible);
    formContext.getControl("WebResource_ObtainAuthorizationsBtn").setVisible(isVisible);
  }

  
  function SetAllReceivedDateField(formContext: Xrm.FormContext) {
    const allRecievedControl = formContext.getControl("ipg_isallreceiveddate1");
    if (allRecievedControl) {
        const caseId = formContext.data.entity.getId().replace('{', '').replace('}', '');
        var fetchXml = "?fetchXml=<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>"+
            "  <entity name='salesorder'>"+
            "    <attribute name='salesorderid' />"+
            "    <filter type='and'>"+
            "      <condition attribute='ipg_caseid' operator='eq' uiname='yjyjuy' uitype='incident' value='"+caseId+"' />"+
            "    </filter>"+
            "    <link-entity name='ipg_casepartdetail' from='ipg_purchaseorderid' to='salesorderid' link-type='inner' alias='ae' />"+
            "  </entity>"+
            "</fetch>";

        Xrm.WebApi.retrieveMultipleRecords("salesorder", fetchXml).then(result => allRecievedControl.setDisabled(result.entities && result.entities.length > 0),
        error => console.log(error));
    }
  }

  export function ValidateAllReceivedDate(executionContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    var dateReceived = formContext.getAttribute("ipg_isallreceiveddate").getValue();
    var currentDate = new Date();

    if (dateReceived && dateReceived.valueOf() - currentDate.valueOf() > 0) {
        var field = formContext.getAttribute("ipg_isallreceiveddate");
        if ((field != null) && (field.getValue() != null))
            field.setValue(null);
        formContext.getControl("ipg_isallreceiveddate1").setNotification("Future date is not allowed", "dateInvalid");
    }
    else
        formContext.getControl("ipg_isallreceiveddate1").clearNotification("dateInvalid"); 
  }

  function ConfigureOwnerField(formContext: Xrm.FormContext)
  {
    var ownerAttr = formContext.getAttribute("ownerid");

    if(ownerAttr != null)
    {
      ownerAttr.setSubmitMode("never");
      ownerAttr.addOnChange(onChangeOwner);
    }
  }

  async function onChangeOwner(executionContext: Xrm.Events.EventContext)
  {
    const formContext = executionContext.getFormContext();
    const incidentid = formContext.data.entity.getId();
    let confirmed = true;

    const ownerattr = (executionContext.getEventSource() as Xrm.Attributes.LookupAttribute);
    const ownerattrValue = ownerattr.getValue();

    if(ownerattrValue?.length > 0)
    {
      var incident = await Xrm.WebApi.online.retrieveRecord("incident",incidentid,"?$select=_ownerid_value");

      if(ownerattrValue[0].entityType == "systemuser" && incident._ownerid_value !== ownerattrValue[0].id.replace("{","").replace("}","").toLocaleLowerCase())
      {
        var result =  await Xrm.Navigation.openConfirmDialog({text: "System is about to reassign all related open User Tasks to the User you assigned this Case to. Do you wish to proceed?"});
        Xrm.Utility.showProgressIndicator("");
        confirmed = result.confirmed;   
      }

      if(confirmed)
      {
        try
        {
          await Xrm.WebApi.online.updateRecord("incident", incidentid , {"ownerid@odata.bind": `/${ownerattrValue[0].entityType}s(${ownerattrValue[0].id.replace("{","").replace("}","")})`});
        }
        catch (e)
        {
          Xrm.Navigation.openErrorDialog({message:"Case has not been ReAssigned. Please try later or contact System Administrator!"});
        }
      }
      else
      {
        ownerattr.setValue([{
          entityType:incident["_ownerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"], 
          id:incident["_ownerid_value"], 
          name:incident["_ownerid_value@OData.Community.Display.V1.FormattedValue"] }]);
      }
    }

    Xrm.Utility.closeProgressIndicator();
  }

  export function CalculateActualTotalResponsibility(executionContext: Xrm.Events.EventContext){
    var formContext = executionContext.getFormContext();
    var carrierResp = formContext.getAttribute("ipg_actualcarrierresponsibility")?.getValue();
    var secondaryCarrierResp = formContext.getAttribute("ipg_actualsecondarycarrierresponsibility")?.getValue();
    var patientResp = formContext.getAttribute("ipg_actualmemberresponsibility")?.getValue();
    formContext.getAttribute("ipg_actualtotalresponsibility")?.setValue(carrierResp + secondaryCarrierResp + patientResp);
  }

  export function CalculateTotalReceived(executionContext: Xrm.Events.EventContext){
    var formContext = executionContext.getFormContext();
    var receivedCarrier = formContext.getAttribute("ipg_totalreceivedfromcarrier")?.getValue();
    var receivedSecondaryCarrier = formContext.getAttribute("ipg_totalreceivedfromsecondarycarrier")?.getValue();
    var receivedPatient = formContext.getAttribute("ipg_totalreceivedfrompatient")?.getValue();
    formContext.getAttribute("ipg_totalreceived")?.setValue(receivedCarrier + receivedSecondaryCarrier + receivedPatient);
  }

  export function CalculateTotalAdjustments(executionContext: Xrm.Events.EventContext){
    var formContext = executionContext.getFormContext();
    var carrierAdj = formContext.getAttribute("ipg_totalcarrierrespadjustments")?.getValue();
    var secondaryCarrierAdj = formContext.getAttribute("ipg_totalsecondarycarrierrespadjustments")?.getValue();
    var patientAdj = formContext.getAttribute("ipg_totalpatientrespadjustments")?.getValue();
    formContext.getAttribute("ipg_totalrespadjustments")?.setValue(carrierAdj + secondaryCarrierAdj + patientAdj);
  }
  
  export function CalculateTotalWriteOff(executionContext: Xrm.Events.EventContext){
    var formContext = executionContext.getFormContext();
    var carrierWriteOff = formContext.getAttribute("ipg_totalcarrierwriteoff")?.getValue();
    var secondaryCarrierWriteOff = formContext.getAttribute("ipg_totalsecondarycarrierwriteoff")?.getValue();
    var patientWriteOff = formContext.getAttribute("ipg_totalpatientwriteoff")?.getValue();
    formContext.getAttribute("ipg_totalwriteoff")?.setValue(carrierWriteOff + secondaryCarrierWriteOff + patientWriteOff);
  }

  export function CalculateTotalBalance(executionContext: Xrm.Events.EventContext){
    var formContext = executionContext.getFormContext();
    var carrierBalance = formContext.getAttribute("ipg_remainingcarrierbalance")?.getValue();
    var secondaryCarrierBalance = formContext.getAttribute("ipg_remainingsecondarycarrierbalance")?.getValue();
    var patientBalance = formContext.getAttribute("ipg_remainingpatientbalance")?.getValue();
    formContext.getAttribute("ipg_casebalance")?.setValue(carrierBalance + secondaryCarrierBalance + patientBalance);
  }

  export function SetBpfStageBasedOnCaseState(formContext: Xrm.FormContext) {
    var incidentId = formContext.data.entity.getId();
    var ipg_statecode =  formContext.getAttribute("ipg_statecode").getValue();
    
    if (ipg_statecode == null ) return;
    var stageId;
    switch (ipg_statecode) {
      case CaseStates.Intake:
        stageId = CaseStages.Intake;
        break;
      case CaseStates.Authorizations:
        stageId = CaseStages.Authorizations;
        break;
      case CaseStates.CaseManagement:
        stageId = CaseStages.CaseManagement;
        break;
      case CaseStates.Billing:
        stageId = CaseStages.Billing;
        break;
      case CaseStates.CarrierCollections:
        stageId = CaseStages.CarrierCollections;
        break;
      case CaseStates.PatientCollections:
        stageId = CaseStages.PatientCollections;
        break;
      case CaseStates.Finance:
        stageId = CaseStages.Finance;
        break;
    }

    if (stageId == null) return;

    var apiQuery = "/api/data/v9.1/ipg_ipgcasebpfmainflows?";
    apiQuery += "$select=_activestageid_value,businessprocessflowinstanceid";
    apiQuery += "&$filter=_bpf_incidentid_value eq " + incidentId;

    var req = new XMLHttpRequest();
    req.open("GET", Xrm.Page.context.getClientUrl() + apiQuery, false);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                var results = JSON.parse(this.response);
                console.log(results)
                for (var i = 0; i < results.value.length; i++) {
                    var _activestageid_value = results.value[i]["_activestageid_value"];
                    var businessprocessflowinstanceid = results.value[i]["businessprocessflowinstanceid"];
                    if (_activestageid_value != stageId) {
                        setMainBpfStage(businessprocessflowinstanceid, stageId);
                    }
                }
            }
            else {
                console.log(this.statusText);
            }
        }
    };
    req.send();
}

function setMainBpfStage (businessprocessflowinstanceid, proposeStageId) {
    var entity = {};
    entity["activestageid@odata.bind"] = "/processstages(" + proposeStageId + ")";
    var req = new XMLHttpRequest();
    req.open("PATCH", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/ipg_ipgcasebpfmainflows(" + businessprocessflowinstanceid + ")", false);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 204) {
                console.log('Success. Stage is set');
            } else {
                console.log(this.statusText);
            }
        }
    };
    req.send(JSON.stringify(entity));
    }
    async function procesCourtesyCaseFieldAccess(formContext) {
      const openedCase = isCaseOpened(formContext);
      const formattedCaseId = Intake.Utility.GetFormattedId(formContext.data.entity.getId());
      const anyCourtesyActualPartExists = await anyCountresyActualPart(formattedCaseId);

      if (anyCourtesyActualPartExists) {
        formContext.getAttribute("ipg_iscourtesyclaimcase1")?.setValue(true);
        formContext.getControl("ipg_iscourtesyclaimcase1")?.setDisabled(true);
      } else if(openedCase) {
        formContext.getControl("ipg_iscourtesyclaimcase1")?.setDisabled(false);
      }
    }
    async function anyCountresyActualPart(id) {
      return (
        (
          await Xrm.WebApi.retrieveMultipleRecords(
            "ipg_casepartdetail"
            , `?$filter=_ipg_caseid_value eq ${id} and  ipg_iscourtesyclaimplan eq true`)
        )?.entities.length > 0
      );
    }

    async function RefreshOnDataLoad(event:Xrm.Events.EventContext)
    {
      const formContext = event.getFormContext();
      SetAllReceivedDateField(formContext);
      //refresh SubGrid and WebResources on Save
      var psSubgrid = formContext.getControl<Xrm.Controls.GridControl>("StatementDocuments");
      psSubgrid?.refreshRibbon();

      var webResourceObj = formContext.getControl<Xrm.Controls.FramedControl>("WebResource_ARSummary")?.getObject();
      if(webResourceObj)
      {
        var src = webResourceObj.src;
        webResourceObj.src = "about:blank";
        webResourceObj.src = src;
      }
    }

  export function onActualDosChange(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    let actualDosValue = formContext.getAttribute("ipg_actualdos") ? formContext.getAttribute("ipg_actualdos").getValue() : null;
    formContext.getControl("ipg_surgerydate")?.setDisabled(actualDosValue != null);
  }
} 
