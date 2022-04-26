/**
 * @namespace Intake.Adjustment
 */
namespace Intake.Adjustment {

  let amountToApply = 0;
  let initialCarrierBalance = 0;
  let initialSecondaryCarrierBalance = 0;
  let initialPatientBalance = 0;
  let initialBalance = 0;
  let payerTypes: Xrm.OptionSetValue[];
  let activeFilter = "";
  let globalFormContext: Xrm.FormContext;

  /**
   * Called on Form Load
   * @function Intake.Adjustment.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();
    globalFormContext = formContext;
    payerTypes = formContext.getControl("ipg_to").getOptions();
    if (formContext.ui.getFormType() == 1) {

      initialCarrierBalance = formContext.getAttribute("ipg_remainingcarrierbalance").getValue();
      initialSecondaryCarrierBalance = formContext.getAttribute("ipg_remainingsecondarycarrierbalance").getValue();
      initialPatientBalance = formContext.getAttribute("ipg_remainingpatientbalance").getValue();
      initialBalance = formContext.getAttribute("ipg_casebalance").getValue();
      let caseId = formContext.getAttribute("ipg_caseid").getValue();
      if (caseId) {
        Xrm.WebApi.retrieveRecord("incident", caseId[0].id, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
          initialCarrierBalance = incident.ipg_remainingcarrierbalance;
          initialSecondaryCarrierBalance = incident.ipg_remainingsecondarycarrierbalance;
          initialPatientBalance = incident.ipg_remainingpatientbalance;
          initialBalance = incident.ipg_casebalance;
        }, function (error) {
          Xrm.Navigation.openErrorDialog({ message: error.message });
        });
        CheckIfCaseIsClosed(formContext, caseId[0].id);
      }

      resetReasons(formContext);
      if (formContext.getAttribute("ipg_adjustmenttype").getValue()) {
        Calculate(formContext);
        FillReasons(formContext);
        var arr: string[] = ["ipg_applyto", "ipg_adjustmenttype"];
        if (formContext.getAttribute("ipg_reverse").getValue()) {
          arr.push("ipg_amounttype");
          arr.push("ipg_amount");
          arr.push("ipg_payment");
          arr.push("ipg_tocase");
          arr.push("ipg_from");
          arr.push("ipg_to");
          arr.push("ipg_transferofpaymenttype");
        }
        for (var control of arr) {
          if (formContext.getControl(control)) {
            formContext.getControl(control).setDisabled(true);
          }
        }
      }
      else {
        formContext.getAttribute("ipg_percent").setValue(100);
      }
    }
    else {
      var arr: string[] = ["ipg_applyto", "ipg_adjustmenttype", "ipg_amounttype", "ipg_percent", "ipg_amount", "ipg_reason", "ipg_payment", "ipg_tocase", "ipg_from", "ipg_to"];
      for (var control of arr) {
        if (formContext.getControl(control)) {
          formContext.getControl(control).setDisabled(true);
        }
      }
    }
    SetCaseLabel(formContext);
  }

  /**
   * Called on Apply To field change
   * @function Intake.Adjustment.OnApplyToChange
   * @returns {void}
  */
  export function OnApplyToChange(executionContext) {
    let formContext = executionContext.getFormContext();
    Calculate(formContext);
    FillReasons(formContext);
    SetDefaultReason(formContext);
  }

  /**
   * @function Intake.Adjustment.Recalculate
   * @returns {void}
  */
  export function Recalculate(executionContext) {
    let formContext = executionContext.getFormContext();
    Calculate(formContext);
    FillReasons(formContext);
  }

  /**
   * Called on Adjustment Type field change
   * @function Intake.Adjustment.OnAdjustmentTypeChange
   * @returns {void}
  */
  export function OnAdjustmentTypeChange(executionContext) {
    let formContext = executionContext.getFormContext();
    SetVisibility(formContext);
    Calculate(formContext);
    FillReasons(formContext);
    SetCaseLabel(formContext);
    FilterFromToFields(formContext);
    SetDefaultReason(formContext);
  }

  /**
   * Called on Amount Type field change
   * @function Intake.Adjustment.OnAmountTypeChange
   * @returns {void}
  */
  export function OnAmountTypeChange(executionContext) {
    let formContext = executionContext.getFormContext();
    Calculate(formContext)
  }

  /**
   * Called on Percent field change
   * @function Intake.Adjustment.OnPercentChange
   * @returns {void}
  */
  export function OnPercentChange() {
    let formContext = globalFormContext;
    Calculate(formContext);
  }
  Xrm.Page.OnPercentChange = OnPercentChange;

  /**
   * Called on Amount field change
   * @function Intake.Adjustment.OnAmountChange
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

  function Calculate(formContext) {

    if (formContext.ui.getFormType() == 1) {
      let applyTo = formContext.getAttribute("ipg_applyto").getValue();
      let adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
      let amountType = formContext.getAttribute("ipg_amounttype").getValue();
      let reverse = formContext.getAttribute("ipg_reverse").getValue();
      let payment = formContext.getAttribute("ipg_payment").getValue();
      let transferOfPaymentType = formContext.getAttribute("ipg_transferofpaymenttype").getValue();
      let from = formContext.getAttribute("ipg_from").getValue();
      let to = formContext.getAttribute("ipg_to").getValue();

      amountToApply = 0;

      let initialAmount = 0;
      if ((adjustmentType != 427880000) && (adjustmentType != 427880002) && (adjustmentType != 427880003) && (adjustmentType != 427880004)) {
        SetInitialValues(formContext);
      }
      else if (adjustmentType == 427880003) {
        SetInitialValues(formContext);
        if (payment) {
          Xrm.WebApi.retrieveRecord("ipg_payment", Intake.Utility.removeCurlyBraces(payment[0].id), "?$select=ipg_interestpayment,ipg_memberpaid_new,ipg_totalinsurancepaid").then(
            function success(result) {
              var ipg_interestpayment = result["ipg_interestpayment"];
              var ipg_memberpaid = result["ipg_memberpaid_new"];
              var ipg_totalinsurancepaid = result["ipg_totalinsurancepaid"];
              amountToApply = ipg_interestpayment + ipg_memberpaid + ipg_totalinsurancepaid;

              if (transferOfPaymentType) {
                if ((from == 427880000) && (to == 427880001)) {
                  formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance + ipg_totalinsurancepaid + ipg_interestpayment);
                  formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance - ipg_totalinsurancepaid - ipg_interestpayment);
                }
                else if ((from == 427880000) && (to == 427880002)) {
                  formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance + ipg_totalinsurancepaid + ipg_interestpayment);
                  formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance - ipg_totalinsurancepaid - ipg_interestpayment);
                }
                else if ((from == 427880001) && (to == 427880000)) {
                  formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance - ipg_totalinsurancepaid - ipg_interestpayment);
                  formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance + ipg_totalinsurancepaid + ipg_interestpayment);
                }
                else if ((from == 427880001) && (to == 427880002)) {
                  formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance + ipg_totalinsurancepaid + ipg_interestpayment);
                  formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance - ipg_totalinsurancepaid - ipg_interestpayment);
                }
                else if ((from == 427880002) && (to == 427880000)) {
                  formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance - ipg_memberpaid);
                  formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance + ipg_memberpaid);
                }
                else if ((from == 427880002) && (to == 427880001)) {
                  formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance - ipg_memberpaid);
                  formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance + ipg_memberpaid);
                }
              }
              else {
                formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance + ipg_totalinsurancepaid + ipg_interestpayment);
                formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance + ipg_memberpaid);
                formContext.getAttribute("ipg_casebalance").setValue(initialBalance + ipg_totalinsurancepaid + ipg_interestpayment + ipg_memberpaid);
              }

              formContext.getAttribute("ipg_amounttoapply").setValue(amountToApply);
            },
            function (error) {
              Xrm.Utility.alertDialog(error.message, null);
            }
          );
        }
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
          if (reverse) {
            amountToApply = -amountToApply;
          }
          //let amount = formContext.getAttribute("ipg_amount").getValue();
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
          if (reverse) {
            amountToApply = -amountToApply;
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
    else if (adjustmentType == 427880003) {
      let reasons: number[] = [427880012];
      reasons.forEach(reason => {
        reasonsControl.addOption(removedReasons.find(r => r["value"] == reason));
      });
    }
    else if (adjustmentType == 427880004) {
      let reasons: number[] = [427880010];
      reasons.forEach(reason => {
        reasonsControl.addOption(removedReasons.find(r => r["value"] == reason));
      });
    }
  }

  /**
   * Called on Transfer of Payment Type field change
   * @function Intake.Adjustment.OnTransferOfPaymentTypeChange
   * @returns {void}
  */
  export function OnTransferOfPaymentTypeChange(executionContext) {
    let formContext = executionContext.getFormContext();
    formContext.getAttribute("ipg_payment").setValue(null);
    let transferOfPaymentType = formContext.getAttribute("ipg_transferofpaymenttype").getValue();
    if (!transferOfPaymentType) {
      AddPresearchToPaymentsContintue(formContext, "");
    }
    SetCaseLabel(formContext);
    Calculate(formContext);
  }

  function SetCaseLabel(formContext: Xrm.FormContext) {
    formContext.getControl("ipg_caseid").setLabel(((formContext.getAttribute("ipg_adjustmenttype").getValue() == 427880003) && (!formContext.getAttribute("ipg_transferofpaymenttype").getValue())) ? "From Case" : "Case");
  }

  /**
   * Called on To Case field change
   * @function Intake.Adjustment.OnToCaseChange
   * @returns {void}
  */
  export function OnToCaseChange(executionContext) {
    let formContext = executionContext.getFormContext();
    let toCaseField = "ipg_tocase";
    formContext.getControl(toCaseField).clearNotification(toCaseField);
    let toCase = formContext.getAttribute(toCaseField).getValue();
    let caseId = formContext.getAttribute("ipg_caseid").getValue();
    if ((toCase) && (caseId) && (toCase[0].id == caseId[0].id)) {
      formContext.getControl(toCaseField).setNotification("You can't transfer payment to the same case", toCaseField);
      formContext.getAttribute(toCaseField).setValue(null)
    }
  }

  /**
   * Called on From field change
   * @function Intake.Adjustment.OnFromChange
   * @returns {void}
  */
  export function OnFromChange(executionContext) {
    let formContext = executionContext.getFormContext();
    formContext.getAttribute("ipg_payment").setValue(null);
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
    AddPresearchToPayments(formContext);
  }

  /**
   * Called on To field change
   * @function Intake.Adjustment.OnToChange
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

  function AddPresearchToPayments(formContext) {
    let from = formContext.getAttribute("ipg_from").getValue();
    if ((from == 427880000) || (from == 427880001)) {
      let caseId = formContext.getAttribute("ipg_caseid").getValue();
      if (caseId[0].id) {
        Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId[0].id), "?$select=_ipg_carrierid_value,_ipg_secondarycarrierid_value").then(
          function success(result) {
            Xrm.WebApi.retrieveMultipleRecords("invoice", "?$select=invoiceid,name&$filter=_ipg_carrierid_value eq " + Intake.Utility.removeCurlyBraces(result[from == 427880000 ? "_ipg_carrierid_value" : "_ipg_secondarycarrierid_value"]) + " and _ipg_caseid_value eq " + Intake.Utility.removeCurlyBraces(caseId[0].id)).then(
              function success(results) {
                let paymentFilter = "<filter type='and'><condition attribute='ipg_claim' operator='in'>";
                let conditions = "";
                for (var i = 0; i < results.entities.length; i++) {
                  conditions += "<value uitype='invoice' uiname='" + results.entities[i]["name"] + "'>{" + results.entities[i]["invoiceid"] + "}</value>"
                }
                if (conditions) {
                  paymentFilter += conditions + "</condition></filter>";
                }
                else {
                  paymentFilter = "<filter type='and'>" +
                    "<condition attribute='ipg_caseid' value='" + caseId[0].id + "' uitype='incident' uiname='" + caseId[0].name + "' operator='ne'/>" +
                    "<condition attribute='ipg_caseid' value='" + caseId[0].id + "' uitype='incident' uiname='" + caseId[0].name + "' operator='eq'/>" +
                    "</filter>";
                }
                AddPresearchToPaymentsContintue(formContext, paymentFilter);
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
    else if (from == 427880002) {
      let paymentFilter = "<filter type='and'><condition attribute='ipg_memberpaid_new' operator='ne' value='0'/></filter>";
      AddPresearchToPaymentsContintue(formContext, paymentFilter);
    }

  }

  function AddPresearchToPaymentsContintue(formContext, paymentFilter) {
    formContext.getControl("ipg_payment").removePreSearch(function () {
      FilterPayments(formContext, activeFilter);
    });
    formContext.getControl("ipg_payment").addPreSearch(function () {
      FilterPayments(formContext, paymentFilter);
    });
    activeFilter = paymentFilter;
  }

  function FilterPayments(formContext, paymentFilter) {
    if ((activeFilter == paymentFilter) && (paymentFilter != "")) {
      formContext.getControl("ipg_payment").addCustomFilter(paymentFilter, "ipg_payment");
    }
  }

  /**
   * Called on Payment field change
   * @function Intake.Adjustment.OnPaymentChange
   * @returns {void}
  */
  export function OnPaymentChange(executionContext) {
    let formContext = executionContext.getFormContext();
    Calculate(formContext);
  }

  function FilterFromToFields(formContext) {
    let adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
    let caseId = formContext.getAttribute("ipg_caseid").getValue();
    if (caseId && (adjustmentType == 427880004)) {
      Xrm.WebApi.retrieveRecord("incident", caseId[0].id, "?$select=_ipg_secondarycarrierid_value").then(
        function success(result) {
          let index = payerTypes.findIndex(x => x.value == 427880001);
          if ((index >= 0) && !result["_ipg_secondarycarrierid_value"]) {
            let fieldNames: string[] = ["ipg_from", "ipg_to"];
            fieldNames.forEach(fieldName => {
              let control: Xrm.Controls.Control = formContext.getControl(fieldName);
              control.clearOptions();
              payerTypes.forEach(payerType => {
                if (payerType.value != 427880001) {
                  control.addOption(payerType);
                }
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

  function SetDefaultReason(formContext) {
    let defaultReason = null;
    let reasonField = "ipg_reason";
    let adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
    switch (adjustmentType) {
      case 427880000:
        let applyTo = formContext.getAttribute("ipg_applyto").getValue();
        if ((applyTo == 427880000) || (applyTo == 427880001)) {
          defaultReason = 427880021;
        }
        else if (applyTo == 427880002) {
          defaultReason = 427880022;
        }
        break;
      case 427880002:
        defaultReason = 427880006;
        break;
      case 427880003:
        defaultReason = 427880012;
        break;
      case 427880004:
        defaultReason = 427880010;
        break;
    }
    if (formContext.getControl(reasonField).getOptions().find(e => e.value == defaultReason)) {
      formContext.getAttribute(reasonField).setValue(defaultReason);
    }
    else {
      formContext.getAttribute(reasonField).setValue(null);
    }
  }

  function SetVisibility(formContext: Xrm.FormContext) {
    let adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
    formContext.getControl("WebResource_RadioButtonList").setVisible(adjustmentType != 427880003);
  }

  function CheckIfCaseIsClosed(formContext, caseId: string) {
    Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=ipg_casestatus").then(function (incident) {
      if (incident.ipg_casestatus == 923720001) {
        formContext.getControl("ipg_caseid").setNotification("This case is currently closed. Therefore, payment or adjustment transactions cannot be applied at this time. Please reopen the case to proceed.", "ipg_caseid");
      }
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });
  }

  /**
  * Called on form Save event
  * @function Intake.Adjustment.OnFormSave
  * @returns {void}
*/
  export function OnFormSave(executionContext) {
    let formContext = executionContext.getFormContext();
    let amountToApply = formContext.getAttribute("ipg_amounttoapply").getValue();
    if (amountToApply == 0) {
      Xrm.Utility.alertDialog("You can't apply a zero adjustment.", () => { return; })
      executionContext.getEventArgs().preventDefault();
    }
  }

}
