/**
 * @namespace Intake.ClaimResponseBatch
 */
namespace Intake.ClaimResponseBatch {

  let disabledFields: string[] = [];
  /**
  * Called on Form Load event
  * @function Intake.ClaimResponseBatch.OnLoadForm
  * @returns {void}
*/
  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();
    let isManualBatchAttr = formContext.getAttribute("ipg_ismanualbatch");

    var controls = formContext.ui.controls.get();
    for (var i in controls) {
      var control = controls[i];
      if (control.getDisabled()) {
        disabledFields.push(control._controlName);
      }
    }

    if (formContext.ui.getFormType() == 1) {
      setBatchType(formContext);
      if (isManualBatchAttr != null) {
        isManualBatchAttr.setValue(true);
      }

      let recordCountAttr = formContext.getAttribute("ipg_recordcount");
      if (recordCountAttr != null) {
        recordCountAttr.setValue(0);
      }

      let appliedAmountAttr = formContext.getAttribute("ipg_appliedamount_new");
      if (appliedAmountAttr != null) {
        appliedAmountAttr.setValue(0);
      }
    }
    else {
      chooseForm(formContext);
      if (formContext.ui.getFormType() == 2) {
        SetAvailability(formContext);
      }
    }

    let isManualBatch = isManualBatchAttr != null ? isManualBatchAttr.getValue() : false;

    let tabObj = formContext.ui.tabs.get("GeneralTab");
    if (tabObj != null) {
      let manualBatchHeaderSection = tabObj.sections.get("ManualBatchHeaderSection");
      if (manualBatchHeaderSection != null) {
        manualBatchHeaderSection.setVisible(isManualBatch);
      }

      let manualBaatchPaymentsSection = tabObj.sections.get("ManualBatchPaymentsSection");
      if (manualBaatchPaymentsSection != null) {
        manualBaatchPaymentsSection.setVisible(isManualBatch);
      }

      let automaticBaatchSubgridsSection = tabObj.sections.get("AutomaticBatchSubgridsSection");
      if (automaticBaatchSubgridsSection != null) {
        automaticBaatchSubgridsSection.setVisible(!isManualBatch);
      }
    }

    let bankDateAttr = formContext.getAttribute("ipg_paymentdate");
    if (bankDateAttr != null && !bankDateAttr.getValue()) {
      bankDateAttr.setValue(new Date());
    }

  }

  /**
  * Called on Bank Date Change event
  * @function Intake.ClaimResponseBatch.OnChangeBankDate
  * @returns {void}
*/
  export function OnChangeBankDate(executionContext) {
    let formContext = executionContext.getFormContext();
    let bankDateAttr = formContext.getAttribute("ipg_paymentdate");

    if (bankDateAttr != null) {
      let bankDate = bankDateAttr.getValue();
      if (bankDate > Date.now()) {
        Xrm.Utility.alertDialog("Bank date can't be set in the future", () => {
          bankDateAttr.setValue(new Date());
        });
      }
    }
  }

  export function SetAvailability(formContext) {
    let userId = Intake.Utility.removeCurlyBraces(Xrm.Utility.getGlobalContext().getUserId());
    let ownerId: Xrm.Attributes.Attribute = formContext.getAttribute("ownerid").getValue();
    var controls = formContext.ui.controls.get();
    if (!ownerId) {
      for (var i in controls) {
        var control = controls[i];
        if (!control.getDisabled() && (control._controlName != "ownerid")) {
          control.setDisabled(true);
        }
      }
    }
    else if (Intake.Utility.removeCurlyBraces(ownerId[0].id) != userId) {
      for (var i in controls) {
        var control = controls[i];
        if (!control.getDisabled()) {
          control.setDisabled(true);
        }
      }
    }
    else {
      for (var i in controls) {
        var control = controls[i];
        if (control.getDisabled() && (!disabledFields.find(f => f == control._controlName))) {
          control.setDisabled(false);
        }
      }
    }
  }

  export function OnOwnerChange(executionContext) {
    let formContext = executionContext.getFormContext();
    SetAvailability(formContext);
  }

  /**
  * Called on Claim Response Type change event
  * @function Intake.ClaimResponseBatch.onTypeChange
  * @returns {void}
*/
  export function onTypeChange(executionContext) {
    let formContext = executionContext.getFormContext();
    chooseForm(formContext);
  }

  let batchTypes: object[] = [
    { name: 'payment', value: false, formId: 'F4BC4EC3-1EC2-4939-8019-0A0F6169C9A1'.toLocaleLowerCase() },
    { name: 'refund', value: true, formId: 'EC1CA242-DAE9-4EC8-BF96-D2C1449CD8D1'.toLocaleLowerCase() }
  ];

  function setBatchType(formContext) {
    let currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
    formContext.getAttribute('ipg_type').setValue(batchTypes.find(x => x['formId'] == currentFormId)['value']);
  }

  function chooseForm(formContext) {
    let batchType = formContext.getAttribute("ipg_type").getValue();
    let currentFormId = formContext.ui.formSelector.getCurrentItem().getId();

    let currentBatchType = batchTypes.find(x => x['value'] == batchType);
    if (currentBatchType != undefined) {
      if (currentFormId != currentBatchType['formId']) {
        var items = formContext.ui.formSelector.items.get();
        for (var i in items) {
          if (items[i].getId() == currentBatchType['formId']) {
            doNotSubmitChangedAttributes();
            items[i].navigate();
          }
        }
      }
    }
  }

  function doNotSubmitChangedAttributes(): void {
    let attributes: Xrm.Attributes.Attribute[] = Xrm.Page.data.entity.attributes.get();
    for (let attr of attributes) {
      if (attr.getIsDirty()) {
        attr.setSubmitMode("never");
      }
    }
  }

}
