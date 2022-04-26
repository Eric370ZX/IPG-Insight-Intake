/**
 * @namespace Intake.ClaimResponseHeader
 */
namespace Intake.ClaimResponseHeader {

  /**
  * Called on Form Load event
  * @function Intake.ClaimResponseHeader.OnLoadForm
  * @returns {void}
  */
  let globalFormContext: Xrm.FormContext;
  let isCarrierBatch: boolean = false;

  export async function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();
    globalFormContext = formContext;
    let isManualPaymentAttr = formContext.getAttribute("ipg_ismanualpayment");
    let batchValue = formContext.getAttribute('ipg_claimresponsebatchid').getValue();
    let isRefund = false;
    let paymentType = formContext.getAttribute('ipg_paymenttype').getValue();
    if (paymentType == 427880002) {
      isRefund = true;
    }
    else if (!paymentType) {
      let batch = await Xrm.WebApi.retrieveRecord('ipg_claimresponsebatch', Intake.Utility.removeCurlyBraces(batchValue[0].id), '?$select=ipg_type,ipg_paymentdate,ipg_paymentfrom');
      isRefund = batch.ipg_type;
      isCarrierBatch = (batch.ipg_paymentfrom == 427880000);
      if (isRefund) {
        formContext.getAttribute('ipg_paymenttype').setValue(427880002)
        formContext.getAttribute('ipg_receiveddate').setValue(new Date(batch.ipg_paymentdate));
        formContext.getControl('ipg_amountpaid_new4').setDisabled(false);
      }
    }
    if (formContext.ui.getFormType() == 1) {
      isManualPaymentAttr.setValue(true);
      formContext.getAttribute('ipg_poststatus').setValue('new');
    }

    let isManualBatch = isManualPaymentAttr != null ? isManualPaymentAttr.getValue() : false;
    let tabObj = formContext.ui.tabs.get("CarrierManualPaymentTab");
    if (tabObj != null) {
      tabObj.setVisible(isManualBatch && !isRefund && isCarrierBatch);
    }

    tabObj = formContext.ui.tabs.get("PatientManualPaymentTab");
    if (tabObj != null) {
      tabObj.setVisible(isManualBatch && !isRefund && !isCarrierBatch);
    }

    tabObj = formContext.ui.tabs.get("GeneralTab");
    if (tabObj != null) {
      tabObj.setVisible(!isManualBatch && !isRefund);
    }

    tabObj = formContext.ui.tabs.get("DevTab");
    if (tabObj != null) {
      tabObj.setVisible(!isManualBatch);
    }

    tabObj = formContext.ui.tabs.get("RefundTab");
    if (tabObj != null) {
      tabObj.setVisible(isRefund);
    }
  }

  /**
  * Called on CaseId Change event
  * @function Intake.ClaimResponseHeader.OnChangeCaseId
  * @returns {void}
*/
  export function OnChangeCaseId(executionContext) {
    //debugger;
    let formContext = executionContext.getFormContext();
    let caseId = formContext.getAttribute("ipg_caseid");

    if (formContext.getAttribute('ipg_paymenttype').getValue() != 427880002) {
      if (caseId != null) {
        formContext.getAttribute("ipg_casenumber").setValue(caseId.getValue()[0].name);
        if (isCarrierBatch) {
          Xrm.WebApi.retrieveMultipleRecords("invoice", `?$top=1&$select=ipg_active,name,invoiceid,statecode,_customerid_value&$filter=_ipg_caseid_value eq ${caseId.getValue()[0].id} and statecode eq 0`)
            .then((invoices) => {
              if (invoices.entities.length == 0) {
                Xrm.Utility.alertDialog("Selected Case has no active claim(s).", () => { return; });
              }
              else {
                invoices.entities.forEach((invoice) => {
                  if (invoice.statecode == 0) { //found active claim
                    formContext.getAttribute("ipg_claimid").setValue([{ entityType: "invoice", id: invoice.invoiceid, name: invoice.name }]);
                    formContext.getAttribute("ipg_claimid").fireOnChange();
                    formContext.getAttribute("ipg_claimnumber").setValue(invoice.name);
                    formContext.getAttribute("ipg_origref").setValue(invoice["_customerid_value@OData.Community.Display.V1.FormattedValue"]);
                  }
                });
              }
            });
        }
        else {
          Xrm.WebApi.retrieveRecord('incident', caseId.getValue()[0].id, '?$select=_ipg_patientid_value').then(
            function success(result) {
              formContext.getAttribute('ipg_origref').setValue(result['_ipg_patientid_value@OData.Community.Display.V1.FormattedValue']);
            },
            function (error) {
              Xrm.Utility.alertDialog(error.message, null);
            }
          );
        }
      }
    }
    else {
      if (caseId.getValue()) {
        formContext.getAttribute('ipg_casenumber').setValue(caseId.getValue()[0].name);
        if (!formContext.getAttribute('ipg_refundtype').getValue()) {
          Xrm.WebApi.retrieveRecord('incident', caseId.getValue()[0].id, '?$select=_ipg_carrierid_value').then(
            function success(result) {
              let carrierName = result['_ipg_carrierid_value@OData.Community.Display.V1.FormattedValue'];
              let carrier =
              {
                id: result['_ipg_carrierid_value'],
                name: carrierName,
                entityType: result['_ipg_carrierid_value@Microsoft.Dynamics.CRM.lookuplogicalname']
              };
              formContext.getAttribute('ipg_carrierid').setValue([carrier]);
              formContext.getAttribute('ipg_paidto').setValue(carrierName);
            },
            function (error) {
              Xrm.Utility.alertDialog(error.message, null);
            }
          );
        }
        else {
          Xrm.WebApi.retrieveRecord('incident', caseId.getValue()[0].id, '?$select=_ipg_patientid_value').then(
            function success(result) {
              formContext.getAttribute('ipg_carrierid').setValue(null);
              formContext.getAttribute('ipg_paidto').setValue(result['_ipg_patientid_value@OData.Community.Display.V1.FormattedValue']);
            },
            function (error) {
              Xrm.Utility.alertDialog(error.message, null);
            }
          );

        }
      }
      else {
        formContext.getAttribute('ipg_carrierid').setValue(null);
        formContext.getAttribute('ipg_paidto').setValue(null);
        formContext.getAttribute('ipg_casenumber').setValue(null);
      }
    }
  }

  /**
  * Called on form Save event
  * @function Intake.ClaimResponseHeader.OnFormSave
  * @returns {void}
*/
  export function OnFormSave(executionContext) {
    //debugger;
    let formContext = executionContext.getFormContext();
    let caseId = formContext.getAttribute("ipg_caseid");
    let claimId = formContext.getAttribute("ipg_claimid");

    if (caseId == null || claimId == null || caseId.getValue() == null || claimId.getValue() == null) {
      Xrm.Utility.alertDialog("Case and Claim must be populated.", () => { return; })
      executionContext.getEventArgs().preventDefault();
    }
  }

  /**
  * Called on Add Adjustment click
  * @function Intake.ClaimResponseHeader.AddAdjustment
  * @returns {void}
*/
  export function AddAdjustment(primaryControl, selectedItems) {
    //debugger;
    let formContext: Xrm.FormContext = primaryControl;
    let claimLineId = selectedItems[0].Id;
    let claimLineName = selectedItems[0].Name;
    let entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_claimresponselineadjustment";
    //entityFormOptions["formId"] = formId;
    entityFormOptions["useQuickCreateForm"] = true;

    // Set default values for the ModalDialog form
    let formParameters = {};
    formParameters["ipg_claimresponselineid"] = [{ entityType: "ipg_claimresponseline", id: claimLineId, name: claimLineName }];

    // Open the form.
    Xrm.Navigation.openForm(entityFormOptions, formParameters)
      .then((result) => {
        if (result != null && result.savedEntityReference.length == 1) {
          let gridContext = formContext.getControl("ManualCarrierPaymentLines");
          if (gridContext != null) {
            gridContext.refresh();
          }
          formContext.data.refresh(false);
        }
      });
  }

  /**
  * Called on Add Remark click
  * @function Intake.ClaimResponseHeader.AddRemark
  * @returns {void}
*/
  export function AddRemark(primaryControl, selectedItems) {
    //debugger;
    let formContext: Xrm.FormContext = primaryControl;
    let claimLineId = selectedItems[0].Id;
    let claimLineName = selectedItems[0].Name;
    let entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_claimresponselineremark";
    //entityFormOptions["formId"] = formId;
    entityFormOptions["useQuickCreateForm"] = true;

    // Set default values for the ModalDialog form
    let formParameters = {};
    formParameters["ipg_claimresponselineid"] = [{ entityType: "ipg_claimresponseline", id: claimLineId, name: claimLineName }];

    // Open the form.
    Xrm.Navigation.openForm(entityFormOptions, formParameters)
      .then((result) => {
        if (result != null && result.savedEntityReference.length == 1) {
          let gridContext = formContext.getControl("ManualCarrierPaymentLines");
          if (gridContext != null) {
            gridContext.refresh();
          }
          formContext.data.refresh(false);
        }
      });
  }

  /**
  * Called on ManualCarrierPaymentLines grid save
  * @function Intake.ClaimResponseHeader.OnSaveManualCarrierPaymentLinesGrid
  * @returns {void}
*/
  export function OnSaveManualCarrierPaymentLinesGrid(executionContext) {
    setTimeout(() => {
      globalFormContext.data.refresh(false)
    }, 500);
  }

  /**
  * Called on Claim Change event
  * @function Intake.ClaimResponseHeader.OnClaimChange
  * @returns {void}
  */
  export function OnClaimChange(executionContext) {
    let formContext = executionContext.getFormContext();
    let claimId = formContext.getAttribute("ipg_claimid").getValue();
    if (claimId) {
      formContext.getAttribute("ipg_claimnumber").setValue(claimId[0].name);
    }
  }

}
