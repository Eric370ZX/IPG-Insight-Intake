/**
 * @namespace Intake.Case.InsuranceTab
 */

namespace Intake.Case.InsuranceTab {

  const LOADING_MESSAGE = 'Loading...';
  const TAB_ID = 'InsuranceBenefitsAuth';
  const CARRIER1_SECTION_ID = 'Carrier1Section';
  const CARRIER2_SECTION_ID = 'Carrier2Section';

  enum CarrierTypes {
    Auto = 427880000,
    Commercial = 427880002,
    DME = 427880004,
    Government = 923720006,
    IPA = 427880003,
    Other = 923720011,
    SelfPay = 427880005,
    WorkersComp = 427880001
  }

  enum BenefitTypes {
    GeneralHealth = 427880059,
    Auto = 427880000,
    WorkersComp = 427880001,
    DurableMedicalEquipment = 427880040
  }

  var benefitFieldNames = {
    carrierId: ['ipg_carrierid', 'ipg_secondarycarrierid'],
    memberId: ['ipg_memberidnumber', 'ipg_secondarymemberidnumber'],
    benefitType: ['ipg_benefittypecode', 'ipg_carrier2benefittypecode'],
    inOutNetwork: ['ipg_inoutnetwork', 'ipg_carrier2isinoutnetwork'],
    coverageLevelDeductible: ['ipg_coverageleveldeductible', 'ipg_carrier2coverageleveldeductible'],
    coverageLevelOop: ['ipg_coverageleveloop', 'ipg_carrier2coverageleveloop'],
    coverageEffectiveDate: ['ipg_primarycarriereffectivedate', 'ipg_secondarycarriereffectivedate'],
    coverageExpirationDate: ['ipg_primarycarrierexpirationdate', 'ipg_secondarycarrierexpirationdate'],
    deductible: ['ipg_deductible', 'ipg_carrier2deductibledisplay'],
    deductibleMet: ['ipg_deductiblemet', 'ipg_carrier2deductiblemetdisplay'],
    deductibleRemaining: ['ipg_deductibleremainingdisplay', 'ipg_carrier2deductibleremainingdisplay'],
    payerCoinsurance: ['ipg_payercoinsurance', 'ipg_carrier2carriercoinsurancedisplay'],
    patientCoinsurance: ['ipg_patientcoinsurance', 'ipg_carrier2patientcoinsurancedisplay'],
    oopMax: ['ipg_oopmax', 'ipg_carrier2oopmaxdisplay'],
    oopMet: ['ipg_oopmet', 'ipg_carrier2oopmetdisplay'],
    oopRemaining: ['ipg_oopremainingdisplay', 'ipg_carrier2oopremainingdisplay']
  };

  export async function OnFormLoad(executionContext: Xrm.Events.SaveEventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    setCarrierButtonsContext(formContext);

    let carrierTypes = await getCarrierTypes(formContext);

    setupFieldsByCarrierType(formContext, carrierTypes[0]); //todo: secondary carrier
    filterBenefitTypes(formContext, carrierTypes);
    setupBenefitFields(formContext);
    retrieveAndDisplayBenefits(formContext);
    setInsuredGender(formContext); //todo: secondary carrier
    SetBVFNames(formContext); //todo: this might be wrong
  }

  export async function OnFormSave(executionContext: Xrm.Events.SaveEventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    let displayedCarrierIndex = getDisplayedCarrierIndex(formContext);
    if (displayedCarrierIndex == null) {
      throw new Error('Could not determine displayed carrier')
    }

    let carrierType = await getCarrierType(formContext, benefitFieldNames.carrierId[displayedCarrierIndex]);

    if (displayedCarrierIndex == 0) {
      setupFieldsByCarrierType(formContext, carrierType); //todo: secondary carrier
    }
    filterCarrierBenefitTypes(formContext, displayedCarrierIndex, carrierType);
    retrieveAndDisplayCarrierBenefits(formContext, displayedCarrierIndex);
  }


  //primary carrier

  export async function OnBenefitTypeChange(executionContext: Xrm.Events.SaveEventContext) {
    let formContext = executionContext.getFormContext();

    Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
    try {
      await retrieveAndDisplayCarrierBenefits(formContext, 0);
    }
    finally {
      Xrm.Utility.closeProgressIndicator();
    }
  }

  export async function OnInOutNetworkChange(executionContext: Xrm.Events.SaveEventContext) {
    let formContext = executionContext.getFormContext();

    Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
    try {
      await retrieveAndDisplayCarrierBenefits(formContext, 0);
    }
    finally {
      Xrm.Utility.closeProgressIndicator();
    }
  }

  export async function OnDeductibleCoverageLevelChange(executionContext: Xrm.Events.SaveEventContext) {
    let formContext = executionContext.getFormContext();

    Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
    try {
      await retrieveAndDisplayCarrierBenefits(formContext, 0);
    }
    finally {
      Xrm.Utility.closeProgressIndicator();
    }
  }

  export async function OnOOPCoverageLevelChange(executionContext: Xrm.Events.SaveEventContext) {
    let formContext = executionContext.getFormContext();

    Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
    try {
      await retrieveAndDisplayCarrierBenefits(formContext, 0);
    }
    finally {
      Xrm.Utility.closeProgressIndicator();
    }
  }


  //secondary carrier

  export async function OnSecondaryBenefitTypeChange(executionContext: Xrm.Events.SaveEventContext) {
    let formContext = executionContext.getFormContext();

    Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
    try {
      await retrieveAndDisplayCarrierBenefits(formContext, 1);
    }
    finally {
      Xrm.Utility.closeProgressIndicator();
    }
  }

  export async function OnSecondaryInOutNetworkChange(executionContext: Xrm.Events.SaveEventContext) {
    let formContext = executionContext.getFormContext();

    Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
    try {
      await retrieveAndDisplayCarrierBenefits(formContext, 1);
    }
    finally {
      Xrm.Utility.closeProgressIndicator();
    }
  }

  export async function OnSecondaryDeductibleCoverageLevelChange(executionContext: Xrm.Events.SaveEventContext) {
    let formContext = executionContext.getFormContext();

    Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
    try {
      await retrieveAndDisplayCarrierBenefits(formContext, 1);
    }
    finally {
      Xrm.Utility.closeProgressIndicator();
    }
  }

  export async function OnSecondaryOOPCoverageLevelChange(executionContext: Xrm.Events.SaveEventContext) {
    let formContext = executionContext.getFormContext();

    Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
    try {
      await retrieveAndDisplayCarrierBenefits(formContext, 1);
    }
    finally {
      Xrm.Utility.closeProgressIndicator();
    }
  }

  export function SetBVFNames(formContext) {

    let memberId = formContext.getAttribute("ipg_memberidnumber").getValue();
    let i;
    if (memberId != null) {
      Xrm.WebApi.retrieveMultipleRecords("ipg_benefitsverificationform", `?$select=ipg_memberidnumber,ipg_primarycontact,ipg_dateofinjury,ipg_deductible,ipg_deductiblemet,ipg_carriercoinsurance,ipg_patientcoinsurance,ipg_oopmax,ipg_oopmaxmet&$filter=ipg_memberidnumber eq '${memberId}'`).then(function success(results) {
        if (results.entities.length) {
          formContext.getAttribute("ipg_primarycontactcode").setValue(results.entities[0]["ipg_primarycontact"]);
          formContext.getAttribute("ipg_deductible").setValue(results.entities[0]["ipg_deductible"]);
          formContext.getAttribute("ipg_deductiblemet").setValue(results.entities[0]["ipg_deductiblemet"]);
          formContext.getAttribute("ipg_deductiblemet").setValue(results.entities[0]["ipg_deductibleremainingdisplay"]);
          formContext.getAttribute("ipg_payercoinsurance").setValue(results.entities[0]["ipg_carriercoinsurance"]);
          formContext.getAttribute("ipg_patientcoinsurance").setValue(results.entities[0]["ipg_patientcoinsurance"]);
          formContext.getAttribute("ipg_oopmax").setValue(results.entities[0]["ipg_oopmax"]);
          formContext.getAttribute("ipg_oopremainingdisplay").setValue(results.entities[0]["ipg_oopmaxmet"]);
          formContext.getAttribute("ipg_autodateofincident").setValue(new Date(results.entities[0]["ipg_dateofinjury"]));
        }
      }, function (error) {
        console.log(error.message);
      });

    }
  }

  export function setInsuredGender(formContext) {
    let optionGenderValue = Xrm.Page.getAttribute("ipg_patientgender").getValue();
    if (optionGenderValue != null) {
      if (optionGenderValue == 923720000) {
        optionGenderValue = 427880000;
      }
      formContext.getAttribute("ipg_insuredgender").setValue(optionGenderValue);
    }
  }


  async function setupFieldsByCarrierType(formContext: Xrm.FormContext, primaryCarrierType: CarrierTypes | null) {
    let isPrimaryMemberIdDme: boolean = false;
    let memberIdAttribute = formContext.getAttribute('ipg_memberidnumber');
    if (memberIdAttribute) {
      let memberId: string = memberIdAttribute.getValue();
      isPrimaryMemberIdDme = isDmeMemberId(memberId);
    }

    setControlsState(formContext, primaryCarrierType, isPrimaryMemberIdDme);

    setDefaultValues(formContext, primaryCarrierType, isPrimaryMemberIdDme);
  }

  function setupAdjusterAndNurseFields(formContext: Xrm.FormContext, primaryCarrierType: CarrierTypes) {
    let showAdjuster: boolean = false;
    let showNurse: boolean = false;

    if (primaryCarrierType == CarrierTypes.Auto) {
      showAdjuster = true;
      showNurse = false;
    }
    else if (primaryCarrierType == CarrierTypes.WorkersComp) {
      let primaryContactCodeAttribute = formContext.getAttribute('ipg_primarycontactcode');
      if (primaryContactCodeAttribute) {
        let primaryContactCodeValue = primaryContactCodeAttribute.getValue();
        if (primaryContactCodeValue) {
          if (primaryContactCodeValue == 427880000 /*Adjuster*/) {
            showAdjuster = true;
            showNurse = false;
          }
          else if (primaryContactCodeValue == 427880001 /*Nurse*/) {
            showAdjuster = false;
            showNurse = true;
          }
          else {
            throw new Error('Unexpected Primary Contact Code value: ' + primaryContactCodeValue);
          }
        }
      }
    }

    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_autoadjustername", showAdjuster);
    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_adjusterphone", showAdjuster);
    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_nursecasemgrname", showNurse);
    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_nursecasemgrphone", showNurse);
  }

  function setControlLabel(formContext: Xrm.FormContext, tabId: string, sectionId: string, fieldName: string, label: string) {
    let control = findControl(formContext, tabId, sectionId, fieldName);
    if (control) {
      control.setLabel(label);
    }
  }

  function setControlDisabled(formContext: Xrm.FormContext, tabId: string, sectionId: string, fieldName: string, disable: boolean) {
    let control = findControl(formContext, tabId, sectionId, fieldName);
    if (control) {
      control.setDisabled(disable);
    }
  }

  function setControlVisible(formContext: Xrm.FormContext, tabId: string, sectionId: string, fieldName: string, visible: boolean) {
    let control = findControl(formContext, tabId, sectionId, fieldName);
    if (control) {
      control.setVisible(visible);
    }
  }

  function findControl(formContext: Xrm.FormContext, tabId: string, sectionId: string, fieldName: string): Xrm.Controls.Control {
    let tabObj = formContext.ui.tabs.get(tabId);
    if (tabObj) {
      let sectionObj = tabObj.sections.get(sectionId);
      if (sectionObj) {
        let attribute = formContext.getAttribute(fieldName);
        if (attribute) {
          let attributeControls = attribute.controls.get();
          for (const c of attributeControls) {
            let sectionControl = sectionObj.controls.get(c.getName());
            if (sectionControl) {
              return sectionControl;
            }
          }
        }
      }
    }

    return null;
  }

  function setControlsState(formContext: Xrm.FormContext, primaryCarrierType: CarrierTypes | null, isPrimaryMemberIdDme: boolean) {
    let isAutoOrWc: boolean = primaryCarrierType == CarrierTypes.Auto || primaryCarrierType == CarrierTypes.WorkersComp;

    //column1
    setControlLabel(formContext, TAB_ID, CARRIER1_SECTION_ID, 'ipg_memberidnumber', isAutoOrWc ? 'Claim #' : 'Member ID');

    //column2
    setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_benefittypecode", isAutoOrWc || isPrimaryMemberIdDme);
    setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_inoutnetwork", isAutoOrWc);
    setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_benefitplantypecode", isAutoOrWc);
    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_plandescription", primaryCarrierType != CarrierTypes.WorkersComp);
    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_plansponsor", primaryCarrierType != CarrierTypes.WorkersComp);
    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_autobenefitsexhausted", primaryCarrierType == CarrierTypes.Auto);
    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_billingfax", isAutoOrWc);
    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_primarycontactcode", primaryCarrierType == CarrierTypes.WorkersComp);
    setupAdjusterAndNurseFields(formContext, primaryCarrierType);
    setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_autodateofincident", isAutoOrWc); //todo: create DateOfInjury field and use for both Auto and Wc

    //column3
    setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_coverageleveldeductible", isAutoOrWc)
    setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_coverageleveloop", isAutoOrWc)
  }

  function setDefaultValues(formContext: Xrm.FormContext, primaryCarrierType: CarrierTypes, isPrimaryMemberIdDme: boolean) {
    if (primaryCarrierType == CarrierTypes.Auto || primaryCarrierType == CarrierTypes.WorkersComp) {
      //column1
      formContext.getAttribute('ipg_relationtoinsured').setValue(100000000); //Self
      formContext.getAttribute('ipg_insuredfirstname').setValue(formContext.getAttribute('ipg_patientfirstname').getValue());
      formContext.getAttribute('ipg_insuredmiddlename').setValue(formContext.getAttribute('ipg_patientmiddlename').getValue());
      formContext.getAttribute('ipg_insuredlastname').setValue(formContext.getAttribute('ipg_patientlastname').getValue());
      formContext.getAttribute('ipg_insureddateofbirth').setValue(formContext.getAttribute('ipg_patientdateofbirth').getValue());
      formContext.getAttribute('ipg_insuredgender').setValue(formContext.getAttribute('ipg_patientgender').getValue());

      //column2
      if (primaryCarrierType == CarrierTypes.Auto) {
        formContext.getAttribute('ipg_benefittypecode').setValue(427880000); //Auto
      }
      else if (primaryCarrierType == CarrierTypes.WorkersComp) {
        formContext.getAttribute('ipg_benefittypecode').setValue(427880001); //WC
      }
      formContext.getAttribute('ipg_benefitplantypecode').setValue(427880004); //Other
      formContext.getAttribute('ipg_inoutnetwork').setValue(true); //INN

      //column3
      formContext.getAttribute('ipg_coverageleveldeductible').setValue(427880001); //Individual
      formContext.getAttribute('ipg_coverageleveloop').setValue(427880001); //Individual
      if (primaryCarrierType == CarrierTypes.Auto) {
        formContext.getAttribute('ipg_is_authorization_required').setValue(false); //No
      }
      else if (primaryCarrierType == CarrierTypes.WorkersComp) {
        formContext.getAttribute('ipg_is_authorization_required').setValue(true); //Yes
      }
    }
  }

  async function filterBenefitTypes(formContext: Xrm.FormContext, carrierTypes: Array<CarrierTypes | null>) {
    await filterCarrierBenefitTypes(formContext, 0, carrierTypes[0]);

    let secondaryCarrrierId = getCarrierIdByIndex(formContext, 1);
    if (secondaryCarrrierId) {
      await filterCarrierBenefitTypes(formContext, 1, carrierTypes[1]);
    }
  }


  async function filterCarrierBenefitTypes(formContext: Xrm.FormContext, carrierIndex: number, carrierType: CarrierTypes | null) {
    let carrierIdAttribute = formContext.getAttribute(benefitFieldNames.carrierId[carrierIndex]);
    let memberIdAttribute = formContext.getAttribute(benefitFieldNames.memberId[carrierIndex]);
    let benefitTypeAttribute = formContext.getAttribute(benefitFieldNames.benefitType[carrierIndex]);
    let benefitTypeControl: Xrm.Controls.Control = null;
    if (benefitTypeAttribute && benefitTypeAttribute.controls.getLength()) {
      benefitTypeControl = benefitTypeAttribute.controls.getByIndex(0);
    }

    if (carrierIdAttribute && memberIdAttribute && benefitTypeControl) {
      let incidentid: string = formContext.data.entity.getId().replace('{', '').replace('}', '');
      let carrierIdValue: Xrm.LookupValue[] = carrierIdAttribute.getValue();
      let carrierId: string = null;
      if (carrierIdValue && carrierIdValue.length) {
        carrierId = carrierIdValue[0].id.replace('{', '').replace('}', '');
      }
      let memberId: string = memberIdAttribute.getValue();

      let benefits = await Xrm.WebApi.retrieveMultipleRecords('ipg_benefit', `?$select=ipg_benefittype&$filter=ipg_CaseId/incidentid eq ${incidentid} and ipg_CarrierId/accountid eq ${carrierId} and ipg_memberid eq '${encodeURIComponent(memberId)}'`);

      benefitTypeAttribute.getOptions().forEach(function (option) {
        if (option.value == BenefitTypes.GeneralHealth) {
          return; //do not filter out General Health benefit type
        }
        if (carrierType == CarrierTypes.Auto && option.value == BenefitTypes.Auto) {
          return;
        }
        if (carrierType == CarrierTypes.WorkersComp && option.value == BenefitTypes.WorkersComp) {
          return;
        }
        if (isDmeMemberId(memberId) && option.value == BenefitTypes.DurableMedicalEquipment) {
          return;
        }

        let benefitExists: boolean = benefits.entities.some(b => b.ipg_benefittype == option.value);

        if (!benefitExists) {
          benefitTypeControl.removeOption(option.value);
        }
      });
    }
  }

  function setupBenefitFields(formContext: Xrm.FormContext) {
    setupCarrierBenefitFields(formContext, 0);
    setupCarrierBenefitFields(formContext, 1);
  }

  function setupCarrierBenefitFields(formContext: Xrm.FormContext, carrierIndex: number) {
    formContext.getAttribute(benefitFieldNames.coverageEffectiveDate[carrierIndex]).setSubmitMode('never');
    formContext.getAttribute(benefitFieldNames.coverageExpirationDate[carrierIndex]).setSubmitMode('never');
    formContext.getAttribute(benefitFieldNames.deductible[carrierIndex]).setSubmitMode('never');
    formContext.getAttribute(benefitFieldNames.deductibleMet[carrierIndex]).setSubmitMode('never');
    formContext.getAttribute(benefitFieldNames.deductibleRemaining[carrierIndex]).setSubmitMode('never');
    formContext.getAttribute(benefitFieldNames.payerCoinsurance[carrierIndex]).setSubmitMode('never');
    formContext.getAttribute(benefitFieldNames.patientCoinsurance[carrierIndex]).setSubmitMode('never');
    formContext.getAttribute(benefitFieldNames.oopMax[carrierIndex]).setSubmitMode('never');
    formContext.getAttribute(benefitFieldNames.oopMet[carrierIndex]).setSubmitMode('never');
    formContext.getAttribute(benefitFieldNames.oopRemaining[carrierIndex]).setSubmitMode('never');
  }

  async function retrieveAndDisplayBenefits(formContext: Xrm.FormContext) {
    await retrieveAndDisplayCarrierBenefits(formContext, 0);
    await retrieveAndDisplayCarrierBenefits(formContext, 1);
  }

  export async function retrieveAndDisplayCarrierBenefits(formContext: Xrm.FormContext, carrierIndex: number) {
    //debugger;

    //variables to be displayed at the end
    let coverageStart: Date | null = null, coverageEnd: Date | null = null;
    let deductibleMax: number | null,
      deductibleMet: number | null,
      deductibleRemaining: number | null,
      payerCoinsurance: number | null,
      patientCoinsurance: number | null,
      oopMax: number | null,
      oopMet: number | null,
      oopRemaining: number | null;

    let carrierIdAttribute = formContext.getAttribute(benefitFieldNames.carrierId[carrierIndex]);
    let memberIdAttribute = formContext.getAttribute(benefitFieldNames.memberId[carrierIndex]);
    let benefitTypeAttribute = formContext.getAttribute(benefitFieldNames.benefitType[carrierIndex]);
    let inOutNetworkAttribute = formContext.getAttribute(benefitFieldNames.inOutNetwork[carrierIndex]);
    let deductibleCoverageLevelAttribute = formContext.getAttribute(benefitFieldNames.coverageLevelDeductible[carrierIndex]);
    let oopCoverageLevelAttribute = formContext.getAttribute(benefitFieldNames.coverageLevelOop[carrierIndex]);

    if (carrierIdAttribute && memberIdAttribute && benefitTypeAttribute && inOutNetworkAttribute && deductibleCoverageLevelAttribute && oopCoverageLevelAttribute) {
      let incidentid = formContext.data.entity.getId().replace('{', '').replace('}', '');

      let carrierIdValue: Xrm.LookupValue[] = carrierIdAttribute.getValue();
      let carrierId: string = null;
      if (carrierIdValue && carrierIdValue.length) {
        carrierId = carrierIdValue[0].id.replace('{', '').replace('}', '');
      }

      let memberIdValue: string = memberIdAttribute.getValue();
      let benefitTypeValue: number = benefitTypeAttribute.getValue();
      let inOutNetworkValue: number = inOutNetworkAttribute.getValue();
      let deductibleCoverageLevelValue: number = deductibleCoverageLevelAttribute.getValue();
      let oopCoverageLevelValue: number = oopCoverageLevelAttribute.getValue();

      if (carrierId && memberIdValue && benefitTypeValue && inOutNetworkValue != null && (deductibleCoverageLevelValue || oopCoverageLevelValue)) {
        let benefits: any[] = null;
        await Xrm.WebApi.retrieveMultipleRecords('ipg_benefit', `?$top=1&$filter=ipg_CaseId/incidentid eq ${incidentid} and ipg_CarrierId/accountid eq ${carrierId} and ipg_memberid eq '${encodeURIComponent(memberIdValue)}'`
          + ` and ipg_benefittype eq ${benefitTypeValue} and ipg_inoutnetwork eq ${inOutNetworkValue}`)
          .then(function (res) {
            benefits = res.entities;
          });

        if (deductibleCoverageLevelValue) {
          let deductibleBenefit = benefits.find(b => b.ipg_coveragelevel == deductibleCoverageLevelValue);
          if (deductibleBenefit) {
            if (deductibleBenefit.ipg_eligibilitystartdate) {
              coverageStart = new Date(deductibleBenefit.ipg_eligibilitystartdate);
            }
            if (deductibleBenefit.ipg_eligibilityenddate) {
              coverageEnd = new Date(deductibleBenefit.ipg_eligibilityenddate);
            }
            deductibleMax = deductibleBenefit.ipg_deductible;
            deductibleMet = deductibleBenefit.ipg_deductiblemet;
            deductibleRemaining = deductibleBenefit.ipg_deductibleremainingcalculated;
            payerCoinsurance = deductibleBenefit.ipg_carriercoinsurance;
            patientCoinsurance = deductibleBenefit.ipg_membercoinsurance;
          }
        }

        if (oopCoverageLevelValue) {
          let oopBenefit = benefits.find(b => b.ipg_coveragelevel == oopCoverageLevelValue);
          if (oopBenefit) {
            oopMax = oopBenefit.ipg_memberoopmax;
            oopMet = oopBenefit.ipg_memberoopmet;
            oopRemaining = oopBenefit.ipg_memberoopremainingcalculated;
          }
        }
      }
    }

    //always update displayed benefit values. If benefit is found, display values. If benefit could not be found, clear values
    formContext.getAttribute(benefitFieldNames.coverageEffectiveDate[carrierIndex]).setValue(coverageStart);
    formContext.getAttribute(benefitFieldNames.coverageExpirationDate[carrierIndex]).setValue(coverageEnd);
    formContext.getAttribute(benefitFieldNames.deductible[carrierIndex]).setValue(deductibleMax);
    formContext.getAttribute(benefitFieldNames.deductibleMet[carrierIndex]).setValue(deductibleMet);
    formContext.getAttribute(benefitFieldNames.deductibleRemaining[carrierIndex]).setValue(deductibleRemaining);
    formContext.getAttribute(benefitFieldNames.payerCoinsurance[carrierIndex]).setValue(payerCoinsurance);
    formContext.getAttribute(benefitFieldNames.patientCoinsurance[carrierIndex]).setValue(patientCoinsurance);
    formContext.getAttribute(benefitFieldNames.oopMax[carrierIndex]).setValue(oopMax);
    formContext.getAttribute(benefitFieldNames.oopMet[carrierIndex]).setValue(oopMet);
    formContext.getAttribute(benefitFieldNames.oopRemaining[carrierIndex]).setValue(oopRemaining);
  }

  function getDisplayedCarrierIndex(formContext: Xrm.FormContext): number {
    let tabObj = formContext.ui.tabs.get(TAB_ID);
    if (tabObj) {
      let carrier1Section = tabObj.sections.get(CARRIER1_SECTION_ID);
      if (carrier1Section) {
        if (carrier1Section.getVisible()) {
          return 0;
        }
        else {
          let carrier2Section = tabObj.sections.get(CARRIER2_SECTION_ID);
          if (carrier2Section?.getVisible()) {
            return 1;
          }
        }
      }
    }

    return null;
  }

  function getCarrierIdByIndex(formContext: Xrm.FormContext, carrierIndex: number): string {
    return getLookupId(formContext, benefitFieldNames.carrierId[carrierIndex]);
  }

  function getLookupId(formContext: Xrm.FormContext, lookupFieldName: string): string {
    let lookupAttribute = formContext.getAttribute(lookupFieldName);
    if (lookupAttribute) {
      let lookupValue = lookupAttribute.getValue();
      if (lookupValue && lookupValue.length && lookupValue[0].id) {
        return lookupValue[0].id;
      }
    }

    return null;
  }

  async function getCarrierTypes(formContext: Xrm.FormContext): Promise<Array<CarrierTypes | null>> {
    let carrierTypes: Array<CarrierTypes | null> = new Array<CarrierTypes | null>();
    carrierTypes[0] = await getCarrierType(formContext, benefitFieldNames.carrierId[0]);

    let carrier2Id = getLookupId(formContext, benefitFieldNames.carrierId[0]);
    if (carrier2Id) {
      carrierTypes[1] = await getCarrierType(formContext, benefitFieldNames.carrierId[1]);
    }

    return carrierTypes;
  }

  async function getCarrierType(formContext: Xrm.FormContext, carrierFieldName: string): Promise<CarrierTypes | null> {
    let carrierAttribute = formContext.getAttribute(carrierFieldName);
    if (carrierAttribute) {
      let carrierValue = carrierAttribute.getValue();
      if (carrierValue && carrierValue.length && carrierValue[0].id) {
        let carrierResult = await Xrm.WebApi.retrieveRecord("account", carrierValue[0].id, `?$select=ipg_carriertype`);
        if (carrierResult && carrierResult.ipg_carriertype) {
          return carrierResult.ipg_carriertype;
        }
      }
    }

    return null;
  }

  function setCarrierButtonsContext(formContext: Xrm.FormContext) {
    {
      let carrierButtonsControl: Xrm.Controls.FramedControl = formContext.getControl('WebResource_carrierbuttons');
      if (carrierButtonsControl != null && carrierButtonsControl != undefined) {
        carrierButtonsControl.getContentWindow().then(
          function (contentWindow: any) {
            contentWindow.Intake.CarrierButtons.setRefreshBenefitsFunctionReference(function () { retrieveAndDisplayCarrierBenefits(formContext, 0) });
          }
        )
      }
    }
    {
      let carrierButtons2Control: Xrm.Controls.FramedControl = formContext.getControl('WebResource_carrierbuttons2');
      if (carrierButtons2Control != null && carrierButtons2Control != undefined) {
        carrierButtons2Control.getContentWindow().then(
          function (contentWindow: any) {
            contentWindow.Intake.CarrierButtons.setRefreshBenefitsFunctionReference(function () { retrieveAndDisplayCarrierBenefits(formContext, 1) });
          }
        )
      }
    }
  }

  function isDmeMemberId(memberId: string) {
    return (memberId || '').toUpperCase().startsWith('JQU')
  }
}
