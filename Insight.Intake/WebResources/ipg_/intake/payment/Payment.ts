/**
 * @namespace Intake.Payment
 */
namespace Intake.Payment {

  let globalFormContext: Xrm.FormContext;

  /**
  * Shows warning if Expiration Date is less than Effective Date
  * @function Intake.Payment.ClaimOnChange
  * @returns {void}
  */
  export function ClaimOnChange(executionContext) {
    var formContext = executionContext.getFormContext();
    //formContext.getControl("RemitDetailsForm").refresh();
    var claim = formContext.getAttribute("ipg_claim").getValue();
    var header = formContext.getAttribute("ipg_claimresponseheader").getValue();
    if ((claim) && (header)) {
      var parameters = {
        "Claim": {
          "invoiceid": claim[0].id,
          "@odata.type": "Microsoft.Dynamics.CRM.invoice"
        },
        "Header": {
          "ipg_claimresponseheaderid": header[0].id,
          "@odata.type": "Microsoft.Dynamics.CRM.ipg_claimresponseheader"
        }
      };
      callAction("ipg_IPGIntakeActionProcessClaimLineItems", parameters, true,
        function (results) {
            formContext.ui.quickForms._getByName("RemitDetailsForm").refresh();
        });
    }
  }

  /**
    * call Custom action
    * @function Intake.Referral.Ribbon.callAction
    * @returns {void}
  */
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

  /**
   * Called on load patient payment form
   * @function Intake.Payment.OnLoadPatientPaymentForm
   * @returns {void}
  */
  export function OnLoadPatientPaymentForm(executionContext) {
    let formContext = executionContext.getFormContext();
    OnLoadForm(formContext);
    if (formContext.ui.getFormType() == 2)
    {
      let fieldsArray : string[] = ["ipg_paymentdate", "ipg_memberpaid_new", "ipg_checknumber", "ipg_notes"];
      fieldsArray.forEach(function (value) {
        formContext.getControl(value).setDisabled(true);
      });
    }
    AdjustmentOnLoad(formContext);
  }

  /**
   * Called on load carrier payment form
   * @function Intake.Payment.OnLoadCarrierPaymentForm
   * @returns {void}
  */
  export async function OnLoadCarrierPaymentForm(executionContext) {
    let formContext = executionContext.getFormContext();
    OnLoadForm(formContext);
    let refund = false;
    let headerValue = formContext.getAttribute('ipg_claimresponseheader').getValue();
    if (headerValue) {
      let header = await Xrm.WebApi.retrieveRecord('ipg_claimresponseheader', Intake.Utility.removeCurlyBraces(headerValue[0].id), '?$select=ipg_paymenttype');
      refund = (header.ipg_paymenttype == 427880002)
    }
    if (formContext.ui.getFormType() == 1) {
      formContext.ui.tabs.get("MainTab").sections.get("RemitDetails").setVisible(refund);
      formContext.ui.tabs.get("MainTab").sections.get("RemitDetailsPreliminary").setVisible(!refund);
      if (!refund) {
        formContext.getAttribute("ipg_claim").fireOnChange();
      }
    }
    else {
      formContext.ui.tabs.get("MainTab").sections.get("RemitDetails").setVisible(!refund);
      formContext.ui.tabs.get("MainTab").sections.get("RemitDetailsPreliminary").setVisible(false);
      if (!refund) {
        formContext.getControl("ipg_claim").setDisabled(true);
      }
    }
    if (refund) {
      formContext.getControl("ipg_claim").setVisible(false);
      formContext.getControl("ipg_totalinsurancepaid").setVisible(true);
      formContext.ui.tabs.get("MainTab").sections.get("AdjustmentSection").setVisible(false);
    }
    else {
      formContext.getControl("ipg_totalinsurancepaid").setVisible(false);
    }
    AdjustmentOnLoad(formContext);
  }

  let amountToApply = 0;
  let initialCarrierBalance = 0;
  let initialSecondaryCarrierBalance = 0;
  let initialPatientBalance = 0;
  let initialBalance = 0;
  let payerTypes: Xrm.OptionSetValue[];
  let activeFilter = "";

  function AdjustmentOnLoad(formContext) {
    SetVisibility(formContext);
    formContext.getControl("ipg_adjustmenttype").removeOption(427880003);
    payerTypes = formContext.getControl("ipg_to").getOptions();
    if (formContext.ui.getFormType() == 1) {

      let caseId = formContext.getAttribute("ipg_caseid").getValue();
      if (caseId) {
        Xrm.WebApi.retrieveRecord("incident", caseId[0].id, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
          initialCarrierBalance = incident.ipg_remainingcarrierbalance;
          initialSecondaryCarrierBalance = incident.ipg_remainingsecondarycarrierbalance;
          initialPatientBalance = incident.ipg_remainingpatientbalance;
          initialBalance = incident.ipg_casebalance;

          var claimResponseHeader = formContext.getAttribute("ipg_claimresponseheader").getValue()[0].id;
          Xrm.WebApi.retrieveRecord("ipg_claimresponseheader", claimResponseHeader, "?$select=ipg_amountpaid_new,_ipg_claimresponsebatchid_value").then(
            function success(result) {
              var ipg_amountpaid = result["ipg_amountpaid_new"];
              var _ipg_claimresponsebatchid_value = result["_ipg_claimresponsebatchid_value"];
              Xrm.WebApi.retrieveRecord("ipg_claimresponsebatch", _ipg_claimresponsebatchid_value, "?$select=ipg_paymentfrom").then(
                function success(result) {
                  var ipg_paymentfrom = result["ipg_paymentfrom"];
                  if (ipg_paymentfrom == 427880000) {
                    var claim = formContext.getAttribute("ipg_claim").getValue();
                    if (claim) {
                      var claimId = claim[0].id;
                      Xrm.WebApi.retrieveRecord("invoice", claimId, "?$select=_customerid_value").then(
                        function success(result) {
                          var _customerid_value = result["_customerid_value"];
                          Xrm.WebApi.retrieveRecord("incident", caseId[0].id, "?$select=_ipg_carrierid_value,_ipg_secondarycarrierid_value").then(
                            function success(result) {
                              var _ipg_carrierid_value = result["_ipg_carrierid_value"];
                              var _ipg_secondarycarrierid_value = result["_ipg_secondarycarrierid_value"];
                              if (_customerid_value == _ipg_carrierid_value) {
                                initialCarrierBalance -= ipg_amountpaid;
                                initialBalance -= ipg_amountpaid;
                              }
                              else if (_customerid_value == _ipg_secondarycarrierid_value) {
                                initialSecondaryCarrierBalance -= ipg_amountpaid;
                                initialBalance -= ipg_amountpaid;
                              }
                              Calculate(formContext);
                            },
                            function (error) {
                              Xrm.Utility.alertDialog(error.message, null);
                            }
                          );
                        },
                        function (error) {
                          Xrm.Utility.alertDialog(error.message, null);
                        }
                      );
                    }
                  }
                  else {
                    initialPatientBalance -= ipg_amountpaid;
                    initialBalance -= ipg_amountpaid;
                    Calculate(formContext);
                  }
                },
                function (error) {
                  Xrm.Utility.alertDialog(error.message, null);
                }
              );
            },
            function (error) {
              Xrm.Utility.alertDialog(error, null);
            }
          );
        }, function (error) {
          Xrm.Navigation.openErrorDialog({ message: error.message });
        });
      }
      
      resetReasons(formContext);
      formContext.getAttribute("ipg_percent").setValue(100);
    }
    else {
      var arr: string[] = ["ipg_applyto", "ipg_adjustmenttype", "ipg_amounttype", "ipg_percent", "ipg_amount", "ipg_reason", "ipg_from", "ipg_to"];
      for (var control of arr) {
        if (formContext.getControl(control)) {
          formContext.getControl(control).setDisabled(true);
        }
      }
    }
  }

  function Calculate(formContext) {

    if (formContext.ui.getFormType() == 1) {
      let applyTo = formContext.getAttribute("ipg_applyto").getValue();
      let adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
      let amountType = formContext.getAttribute("ipg_amounttype").getValue();
      let from = formContext.getAttribute("ipg_from").getValue();
      let to = formContext.getAttribute("ipg_to").getValue();

      amountToApply = 0;
      SetInitialValues(formContext);

      let initialAmount = 0;
      if ((adjustmentType != 427880000) && (adjustmentType != 427880002) && (adjustmentType != 427880004)) {
        SetInitialValues(formContext);
      }
      else if (adjustmentType == 427880004) {
        SetInitialValues(formContext);
        switch (from) {
          case 427880000: //primary carrier
            initialAmount = initialCarrierBalance;
            break;
          case 427880001: //secondary carrier
            initialAmount = initialSecondaryCarrierBalance;
            break;
          case 427880002: //patient
            initialAmount = initialPatientBalance;
            break;
          default:
            initialAmount = 0;
        }
        //amountToApply = Math.min(initialAmount, amount);
        if (initialAmount > 0) {
          if (!amountType) {
            amountToApply = initialAmount * formContext.getAttribute("ipg_percent").getValue() / 100;
          }
          else {
            amountToApply = Math.min(initialAmount, formContext.getAttribute("ipg_amount").getValue());
          }
          if ((from == 427880000) && (to == 427880001)) {
            formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance - amountToApply);
            formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance + amountToApply);
          }
          else if ((from == 427880000) && (to == 427880002)) {
            formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance - amountToApply);
            formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance + amountToApply);
          }
          else if ((from == 427880001) && (to == 427880000)) {
            formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance + amountToApply);
            formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance - amountToApply);
          }
          else if ((from == 427880001) && (to == 427880002)) {
            formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance - amountToApply);
            formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance + amountToApply);
          }
          else if ((from == 427880002) && (to == 427880000)) {
            formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance + amountToApply);
            formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance - amountToApply);
          }
          else if ((from == 427880002) && (to == 427880001)) {
            formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance + amountToApply);
            formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance - amountToApply);
          }
        }
        else {
          SetInitialValues(formContext);
        }
      }
      else {
        switch (applyTo) {
          case 427880000: //primary carrier
            initialAmount = initialCarrierBalance;
            break;
          case 427880001: //secondary carrier
            initialAmount = initialSecondaryCarrierBalance;
            break;
          case 427880002: //patient
            initialAmount = initialPatientBalance;
            break;
          default:
            initialAmount = 0;
        }
        if (initialAmount > 0) {
          if (!amountType) {
            amountToApply = initialAmount * formContext.getAttribute("ipg_percent").getValue() / 100;
          }
          else {
            amountToApply = Math.min(initialAmount, formContext.getAttribute("ipg_amount").getValue());
          }
          switch (applyTo) {
            case 427880000: //primary carrier
              formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance - amountToApply);
              break;
            case 427880001: //secondary carrier
              formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance - amountToApply);
              break;
            case 427880002: //patient
              formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance - amountToApply);
              break;
          }
          formContext.getAttribute("ipg_casebalance").setValue(initialBalance - amountToApply);
        }
        else {
          SetInitialValues(formContext);
        }
      }

      formContext.getAttribute("ipg_amounttoapply").setValue(amountToApply);

      let webResource = formContext.getControl("WebResource_ARSummary");
      let htmlObject = webResource.getObject();
      if (htmlObject) {
        let src = htmlObject.src;
        let aboutBlank = "about:blank";
        htmlObject.src = aboutBlank;
        htmlObject.src = src;
      }

    }

  }

  function SetInitialValues(formContext) {
    formContext.getAttribute("ipg_amounttoapply").setValue(0);
    formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance);
    formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance);
    formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance);
    formContext.getAttribute("ipg_casebalance").setValue(initialBalance);
  }

  let removedReasons: object[] = [];
  function resetReasons(formContext: Xrm.FormContext) {
    let fieldName = "ipg_reason";
    for (var i = 0; i < formContext.getAttribute(fieldName).getOptions().length; i++) {
      var value = formContext.getAttribute(fieldName).getOptions()[i].value;
      formContext.getControl(fieldName).removeOption(Number(value));
      removedReasons.push(formContext.getAttribute(fieldName).getOptions()[i]);
    }
  }

  /**
   * Called on Apply To field change
   * @function Intake.Payment.OnApplyToChange
   * @returns {void}
  */
  export function OnApplyToChange(executionContext) {
    let formContext = executionContext.getFormContext();
    Calculate(formContext);
    FillReasons(formContext);
    SetDefaultReason(formContext);
  }

  function FillReasons(formContext: Xrm.FormContext) {

    let fieldName = "ipg_reason";
    let reasonsControl: Xrm.Controls.Control = formContext.getControl(fieldName);
    reasonsControl.clearOptions();

    let applyTo = formContext.getAttribute("ipg_applyto").getValue();
    let adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
    if ((applyTo) && ((adjustmentType == 427880000) || (adjustmentType == 427880002))) {

      if (adjustmentType == 427880000) {
        let reasons: number[][] = [[427880021, 427880024, 427880023, 427880025, 427880026, 427880000, 427880020, 427880002, 427880001, 427880003, 427880008, 427880009],
        [427880022, 427880019, 427880020, 427880004, 427880005, 427880007]];
        reasons[(applyTo == 427880000) || (applyTo == 427880001) ? 0 : 1].forEach(reason => {
          reasonsControl.addOption(removedReasons.find(r => r["value"] == reason));
        });
      }
      else if (adjustmentType == 427880002) {
        let reasons: number[] = [427880006];
        reasons.forEach(reason => {
          reasonsControl.addOption(removedReasons.find(r => r["value"] == reason));
        });
      }
    }
    else if (adjustmentType == 427880004) {
      let reasons: number[] = [427880010];
      reasons.forEach(reason => {
        reasonsControl.addOption(removedReasons.find(r => r["value"] == reason));
      });
    }
  }

  function SetDefaultReason(formContext) {
    let defaultReason = null;
    let adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
    switch (adjustmentType) {
      case 427880000:
        let applyTo = formContext.getAttribute("ipg_applyto").getValue();
        if ((applyTo == 427880000) || (applyTo == 427880001)) {
          defaultReason = 427880021;
        }
        else if (applyTo == 427880002) {
          defaultReason = 427880019;
        }
        break;
      case 427880002:
        defaultReason = 427880006;
        break;
      case 427880004:
        defaultReason = 427880010;
        break;
    }
    formContext.getAttribute("ipg_reason").setValue(defaultReason);
  }

  /**
   * @function Intake.Payment.Recalculate
   * @returns {void}
  */
  export function Recalculate(executionContext) {
    let formContext = executionContext.getFormContext();
    Calculate(formContext);
    FillReasons(formContext);
  }

  /**
   * Called on Adjustment Type field change
   * @function Intake.Payment.OnAdjustmentTypeChange
   * @returns {void}
  */
  export function OnAdjustmentTypeChange(executionContext) {
    let formContext = executionContext.getFormContext();
    SetVisibility(formContext);
    Calculate(formContext);
    FillReasons(formContext);
    FilterFromToFields(formContext);
    SetDefaultReason(formContext);
  }

  function FilterFromToFields(formContext) {
    let adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
    let caseId = formContext.getAttribute("ipg_caseid").getValue();
    if (caseId && (adjustmentType == 427880004)) {
      Xrm.WebApi.retrieveRecord("incident", caseId[0].id, "?$select=ipg_secondarycarrier,ipg_secondarycarrierstatus").then(
        function success(result) {
          let index = payerTypes.findIndex(x => x.value == 427880001);
          if ((index >= 0) && (!result["ipg_secondarycarrier"] || !result["ipg_secondarycarrierstatus"])) {
            payerTypes.splice(index, 1);
            let fieldNames: string[] = ["ipg_from", "ipg_to"];
            fieldNames.forEach(fieldName => {
              let control: Xrm.Controls.Control = formContext.getControl(fieldName);
              control.clearOptions();
              payerTypes.forEach(payerType => {
                control.addOption(payerType);
              });
            });
          }
        },
        function (error) {
          Xrm.Utility.alertDialog(error.message, null);
        }
      );
    }
  }

  /**
   * Called on Amount Type field change
   * @function Intake.Payment.OnAmountTypeChange
   * @returns {void}
  */
  export function OnAmountTypeChange(executionContext) {
    let formContext = executionContext.getFormContext();
    Calculate(formContext)
  }

  /**
   * Called on Percent field change
   * @function Intake.Payment.OnPercentChange
   * @returns {void}
  */
  export function OnPercentChange() {
    let formContext = globalFormContext;
    Calculate(formContext);
  }
  Xrm.Page.OnPercentChange = OnPercentChange;

  /**
   * Called on Amount field change
   * @function Intake.Payment.OnAmountChange
   * @returns {void}
  */
  export function OnAmountChange() {
    let formContext = globalFormContext;
    let amountField = "ipg_amount";
    formContext.getControl(amountField).clearNotification(amountField);
    Calculate(formContext);
    let amountToApply = formContext.getAttribute("ipg_amounttoapply").getValue();
    let amount = formContext.getAttribute(amountField).getValue();
    if (amount > amountToApply) {
      formContext.getControl(amountField).setNotification("Amount specified to write-off is greater than remaining balance.  Please revise the amount and try again.", amountField);
      formContext.getAttribute(amountField).setValue(amountToApply);
    }
  }
  Xrm.Page.OnAmountChange = OnAmountChange;

  /**
   * Called on From field change
   * @function Intake.Payment.OnFromChange
   * @returns {void}
  */
  export function OnFromChange(executionContext) {
    let formContext = executionContext.getFormContext();
    let fieldName = "ipg_to";
    let toControl: Xrm.Controls.Control = formContext.getControl(fieldName);
    toControl.clearOptions();

    let from = formContext.getAttribute("ipg_from").getValue();
    if (from) {
      payerTypes.forEach(payerType => {
        if (from != payerType.value) {
          toControl.addOption(payerType);
        }
      });
    }
    else {
      payerTypes.forEach(payerType => {
        toControl.addOption(payerType);
      });
    }
    Calculate(formContext);
  }

  /**
   * Called on To field change
   * @function Intake.Payment.OnToChange
   * @returns {void}
  */
  export function OnToChange(executionContext) {
    let formContext = executionContext.getFormContext();
    let fieldName = "ipg_to";
    formContext.getControl(fieldName).clearNotification(fieldName);
    if (formContext.getAttribute(fieldName).getValue() == 427880002) //patient
    {
      let caseId = formContext.getAttribute("ipg_caseid").getValue();
      if (caseId[0].id) {
        Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId[0].id), "?$select=ipg_billtopatient").then(
          function success(result) {
            if (result["ipg_billtopatient"] == 923720000) {
              formContext.getControl(fieldName).setNotification("Bill to Patient is set to NO on the case. Therefore, balances cannot be transferred to the patient. If this patient should be billed by IPG, then set the Bill to Patient on the case to YES and repost the balance transfer transaction", fieldName);
            }
          },
          function (error) {
            Xrm.Utility.alertDialog(error.message, null);
          }
        );
      }
    }
    Calculate(formContext);
  }

  /**
   * Called on Apply Adjustment change
   * @function Intake.Payment.OnApplyAdjustmentChange
   * @returns {void}
  */
  export function OnApplyAdjustmentChange(executionContext) {
    let formContext = executionContext.getFormContext();
    let fieldName = "ipg_to";
    formContext.getControl(fieldName).clearNotification(fieldName);
    SetVisibility(formContext);
  }

  function SetVisibility(formContext: Xrm.FormContext) {
    let applyAdjustment = formContext.getAttribute("ipg_applyadjustment").getValue();
    let adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
    formContext.getControl("WebResource_RadioButtonList").setVisible(applyAdjustment && (adjustmentType != 427880003));
  }

  /**
 * Called on saving form
 * @function Intake.Payment.OnSaveForm
 * @returns {void}
*/
  export function OnSaveForm(executionContext) {
    //OOB bug. Resolution: https://community.dynamics.com/365/unified-interface/f/unified-interface-forum/377366/dialog-box-message-displaying-when-using-formcontext-data-save-then-or-formcontext-data-save/1021229#1021229
    let formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() == 1) {
      formContext.data.entity.addOnSave(function () {
      });
    }
  }

  async function OnLoadForm(formContext) {
    globalFormContext = formContext;
    let headerValue = formContext.getAttribute('ipg_claimresponseheader').getValue();
    if (headerValue) {
      let header = await Xrm.WebApi.retrieveRecord('ipg_claimresponseheader', Intake.Utility.removeCurlyBraces(headerValue[0].id), '?$select=_ipg_claimresponsebatchid_value');
      if (header._ipg_claimresponsebatchid_value) {
        let batch = await Xrm.WebApi.retrieveRecord('ipg_claimresponsebatch', header._ipg_claimresponsebatchid_value, '?$select=ipg_paymentfrom');
        let isCarrierPayment = (batch.ipg_paymentfrom == 427880000);
        let carrierFormId = '2708FC53-BBD4-4D52-B91E-56C5524B97F4'.toLocaleLowerCase();
        let patientFormId = '05A46600-F1C0-450E-9846-73E3E82829EB'.toLocaleLowerCase();
        let currentFormId = (<string>formContext.ui.formSelector.getCurrentItem().getId()).toLocaleLowerCase();
        if (isCarrierPayment && (carrierFormId != currentFormId)) {
          let items = formContext.ui.formSelector.items.get();
          for (let i in items) {
            if (items[i].getId() == carrierFormId) {
              items[i].navigate();
            }
          }
        }
        else if (!isCarrierPayment && (patientFormId != currentFormId)) {
          let items = formContext.ui.formSelector.items.get();
          for (let i in items) {
            if (items[i].getId() == patientFormId) {
              items[i].navigate();
            }
          }
        }
      }
    }
    FilterTasks(formContext);
  }

  function FilterTasks(formContext) {
    const tasksControl: Xrm.Page.GridControl = Xrm.Page.getControl("Tasks");
    const caseId = formContext.getAttribute("ipg_caseid").getValue();
    if (caseId) {
      const fetxtXml = `<fetch><entity name="task"><filter type="and"><condition attribute="regardingobjectid" operator="eq" value="${Intake.Utility.removeCurlyBraces(caseId[0].id)}" uitype="incident" uiname="${caseId[0].name}" /><condition attribute="ipg_tasktypecode" value="427880065" operator="eq"/></filter></entity></fetch>`;
      tasksControl.setFilterXml(fetxtXml);
      tasksControl.refresh();
    }
  }

}
