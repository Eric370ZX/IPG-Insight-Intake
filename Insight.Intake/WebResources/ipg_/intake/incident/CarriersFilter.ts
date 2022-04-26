namespace Intake.CarriersFilter {

  const TAB_ID = 'InsuranceBenefitsAuth';
  const CARRIER1_SECTION_ID = 'Carrier1Section';
  const CARRIER1_AUTH_SECTION_ID = 'Carrier1AuthSection';
  const CARRIER2_SECTION_ID = 'Carrier2Section';
  const CARRIER2_AUTH_SECTION_ID = 'Carrier2AuthSection';

  var carrierNameElement: HTMLSpanElement;
  var carrier1Radio: HTMLInputElement;
  var carrier1Label: HTMLLabelElement;
  var carrier2Radio: HTMLInputElement;
  var carrier2Label: HTMLLabelElement;
  var addCarrierButton: HTMLButtonElement;

  var currentCarrierValue: string;

  var Xrm: Xrm.XrmStatic = Xrm || window.parent.Xrm;

  export async function init(): Promise<any> {
    getControls();

    currentCarrierValue = getSelectedCarrierValue();

    await refreshView();

    addEventListeners();
  }

  /*
   * This event handler may not be needed because users do not loose changes when switch displayed carriers
   */
  export function onCarrierClick(event: any): boolean {
    let returnValue: boolean = true;

    let newCarrierValue: string = getSelectedCarrierValue();
    if (newCarrierValue != currentCarrierValue) {
      let currentSectionId = getCurrentSectionId();
      let isDirty: boolean = isDirtySection(currentSectionId);
      if (isDirty) {
        returnValue = false;
        
        Xrm.Page.data.refresh(false).then(function () {
          isDirty = isDirtySection(currentSectionId);
          if (! isDirty) {
            setSelectedCarrierValue(newCarrierValue);
            onCarrierChanged();
          }
        });
      }
    }

    return returnValue;
  }

  export async function onCarrierChanged(): Promise<any> {
    currentCarrierValue = getSelectedCarrierValue();

    await refreshView();
  }

  export async function onAddCarrierClick(): Promise<any> {
    Xrm.Navigation.openForm({
      entityName: 'ipg_casecarrieraddingparameters',
      useQuickCreateForm: true
    },
    {
      ipg_caseid: Xrm.Page.data.entity.getId().replace('{', '').replace('}', '')
    }).then(
      async function (success) {
        //debugger;
        if (success.savedEntityReference && success.savedEntityReference.length) {
          let newCaseCarrierId: string = success.savedEntityReference[0].id.replace('{', '').replace('}', '');
          await Xrm.WebApi.deleteRecord('ipg_casecarrieraddingparameters', newCaseCarrierId);

          if (Xrm.Page.data.refresh) {
            Xrm.Page.data.refresh(true);
          }
        }
      },
      function (error) {
        Xrm.Navigation.openErrorDialog({ message: error.message })
      });

  }

  function getControls() {
    carrierNameElement = <HTMLSpanElement>document.getElementById('carrierNameElement');
    carrier1Radio = <HTMLInputElement>document.getElementById('carrier1Radio');
    carrier2Radio = <HTMLInputElement>document.getElementById('carrier2Radio');
    carrier1Label = <HTMLLabelElement>document.getElementById('carrier1Label');
    carrier2Label = <HTMLLabelElement>document.getElementById('carrier2Label');
    addCarrierButton = <HTMLButtonElement>document.getElementById('addCarrierButton');
  }

  function getSelectedCarrierValue(): string {
    if (carrier1Radio.checked) {
      return carrier1Radio.value;
    }
    if(carrier2Radio.checked) {
      return carrier2Radio.value;
    }
  }

  function setSelectedCarrierValue(carrierValue) {
    if (carrierValue == 'carrier1') {
      carrier1Radio.checked = true;
    }
    if (carrierValue == 'carrier2') {
      carrier2Radio.checked = true;
    }
  }

  function addEventListeners() {
    let carrier1Attribute = Xrm.Page.getAttribute('ipg_carrierid');
    if (carrier1Attribute) {
      carrier1Attribute.addOnChange(refreshView); //todo: replace with Form OnSave because AddCarrier button should not be available until the form is saved
    }

    let carrier2Attribute = Xrm.Page.getAttribute('ipg_secondarycarrierid');
    if (carrier2Attribute) {
      carrier2Attribute.addOnChange(refreshView); //todo: replace with Form OnSave because AddCarrier button should not be available until the form is saved
    }

    let isLockedAttribute = Xrm.Page.getAttribute('ipg_islocked');
    if (isLockedAttribute) {
      isLockedAttribute.addOnChange(refreshView);
    }
  }

  function getCurrentSectionId() {
    if (carrier1Radio.value == currentCarrierValue) {
      return CARRIER1_SECTION_ID;
    }
    if (carrier2Radio.value == currentCarrierValue) {
      return CARRIER2_SECTION_ID;
    }
  }

  async function refreshView() {
    let carrier1Name: string = '';
    let isCarrier1Wc: boolean = false;
    let carrier1Attribute = Xrm.Page.getAttribute('ipg_carrierid');
    if (carrier1Attribute) {
      let carrier1Value = carrier1Attribute.getValue();
      if (carrier1Value && carrier1Value.length) {
        carrier1Name = carrier1Value[0].name;
        carrier1Label.innerText = carrier1Name + ' (Primary)';

        let carrierResult = await Xrm.WebApi.retrieveRecord("account", carrier1Value[0].id, `?$select=ipg_carriertype`); 
        if (carrierResult && carrierResult.ipg_carriertype == 427880001 /*WC*/) {  
          isCarrier1Wc = true;
        }
      }
      else {
        carrier1Name = '';
        carrier1Label.innerText = '';
      }
    }

    let carrier2Name: string = '';
    let carrier2Attribute = Xrm.Page.getAttribute('ipg_secondarycarrierid');
    if (carrier2Attribute) {
      let carrier2Value = carrier2Attribute.getValue();
      if (carrier2Value && carrier2Value.length) {
        carrier2Name = carrier2Value[0].name;
        carrier2Label.innerText = carrier2Name + ' (Secondary)';
        carrier2Radio.style.display = 'inline';
        carrier2Label.style.display = 'inline';
      }
      else {
        carrier2Name = '';
        carrier2Label.innerText = '';
        carrier2Radio.style.display = 'none';
        carrier2Label.style.display = 'none';
      }
    }

    if (carrier2Radio.checked) {
      carrierNameElement.innerText = carrier2Name;
      showCarrier2();
    }
    else {
      carrierNameElement.innerText = carrier1Name;
      showCarrier1();
    }

    let isCaseLocked: boolean = false;
    let isLockedAttribute = Xrm.Page.getAttribute('ipg_islocked');
    if (isLockedAttribute) {
      isCaseLocked = isLockedAttribute.getValue();
    }

    addCarrierButton.style.display = (carrier2Name || isCarrier1Wc || isCaseLocked) ? 'none' : 'inline';
  }

  function isDirtySection(sectionId: string): boolean {
    let section = Xrm.Page.ui.tabs.get(TAB_ID).sections.get(sectionId);

    let isDirty: boolean = false;
    for (let i = 0; i < section.controls.getLength(); i++) {
      let control: Xrm.Controls.Control = section.controls.getByIndex(i);
      if ((<any>control).getAttribute) {
        let attribute: Xrm.Attributes.Attribute = (<any>control).getAttribute();
        if (attribute && attribute.getIsDirty()) {
          isDirty = true;
          break;
        }
      }
    }

    return isDirty;
  }

  function showCarrier1() {
    Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER2_SECTION_ID).setVisible(false);
    Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER2_AUTH_SECTION_ID).setVisible(false);
    Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER1_SECTION_ID).setVisible(true);
    Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER1_AUTH_SECTION_ID).setVisible(true);
  }

  function showCarrier2() {
    Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER1_SECTION_ID).setVisible(false);
    Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER1_AUTH_SECTION_ID).setVisible(false);
    Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER2_SECTION_ID).setVisible(true);
    Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER2_AUTH_SECTION_ID).setVisible(true);
  }

}
