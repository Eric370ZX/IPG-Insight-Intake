/**
 * @namespace Intake.Adjustment
 */
var Intake;
(function (Intake) {
    var Adjustment;
    (function (Adjustment) {
        var amountToApply = 0;
        var initialCarrierBalance = 0;
        var initialSecondaryCarrierBalance = 0;
        var initialPatientBalance = 0;
        var initialBalance = 0;
        var payerTypes;
        var activeFilter = "";
        var globalFormContext;
        /**
         * Called on Form Load
         * @function Intake.Adjustment.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            globalFormContext = formContext;
            payerTypes = formContext.getControl("ipg_to").getOptions();
            if (formContext.ui.getFormType() == 1) {
                initialCarrierBalance = formContext.getAttribute("ipg_remainingcarrierbalance").getValue();
                initialSecondaryCarrierBalance = formContext.getAttribute("ipg_remainingsecondarycarrierbalance").getValue();
                initialPatientBalance = formContext.getAttribute("ipg_remainingpatientbalance").getValue();
                initialBalance = formContext.getAttribute("ipg_casebalance").getValue();
                var caseId = formContext.getAttribute("ipg_caseid").getValue();
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
                    var arr = ["ipg_applyto", "ipg_adjustmenttype"];
                    if (formContext.getAttribute("ipg_reverse").getValue()) {
                        arr.push("ipg_amounttype");
                        arr.push("ipg_amount");
                        arr.push("ipg_payment");
                        arr.push("ipg_tocase");
                        arr.push("ipg_from");
                        arr.push("ipg_to");
                        arr.push("ipg_transferofpaymenttype");
                    }
                    for (var _i = 0, arr_1 = arr; _i < arr_1.length; _i++) {
                        var control = arr_1[_i];
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
                var arr = ["ipg_applyto", "ipg_adjustmenttype", "ipg_amounttype", "ipg_percent", "ipg_amount", "ipg_reason", "ipg_payment", "ipg_tocase", "ipg_from", "ipg_to"];
                for (var _a = 0, arr_2 = arr; _a < arr_2.length; _a++) {
                    var control = arr_2[_a];
                    if (formContext.getControl(control)) {
                        formContext.getControl(control).setDisabled(true);
                    }
                }
            }
            SetCaseLabel(formContext);
        }
        Adjustment.OnLoadForm = OnLoadForm;
        /**
         * Called on Apply To field change
         * @function Intake.Adjustment.OnApplyToChange
         * @returns {void}
        */
        function OnApplyToChange(executionContext) {
            var formContext = executionContext.getFormContext();
            Calculate(formContext);
            FillReasons(formContext);
            SetDefaultReason(formContext);
        }
        Adjustment.OnApplyToChange = OnApplyToChange;
        /**
         * @function Intake.Adjustment.Recalculate
         * @returns {void}
        */
        function Recalculate(executionContext) {
            var formContext = executionContext.getFormContext();
            Calculate(formContext);
            FillReasons(formContext);
        }
        Adjustment.Recalculate = Recalculate;
        /**
         * Called on Adjustment Type field change
         * @function Intake.Adjustment.OnAdjustmentTypeChange
         * @returns {void}
        */
        function OnAdjustmentTypeChange(executionContext) {
            var formContext = executionContext.getFormContext();
            SetVisibility(formContext);
            Calculate(formContext);
            FillReasons(formContext);
            SetCaseLabel(formContext);
            FilterFromToFields(formContext);
            SetDefaultReason(formContext);
        }
        Adjustment.OnAdjustmentTypeChange = OnAdjustmentTypeChange;
        /**
         * Called on Amount Type field change
         * @function Intake.Adjustment.OnAmountTypeChange
         * @returns {void}
        */
        function OnAmountTypeChange(executionContext) {
            var formContext = executionContext.getFormContext();
            Calculate(formContext);
        }
        Adjustment.OnAmountTypeChange = OnAmountTypeChange;
        /**
         * Called on Percent field change
         * @function Intake.Adjustment.OnPercentChange
         * @returns {void}
        */
        function OnPercentChange() {
            var formContext = globalFormContext;
            Calculate(formContext);
        }
        Adjustment.OnPercentChange = OnPercentChange;
        Xrm.Page.OnPercentChange = OnPercentChange;
        /**
         * Called on Amount field change
         * @function Intake.Adjustment.OnAmountChange
         * @returns {void}
        */
        function OnAmountChange() {
            var formContext = globalFormContext;
            var amountField = "ipg_amount";
            formContext.getControl(amountField).clearNotification(amountField);
            Calculate(formContext);
            var amountToApply = formContext.getAttribute("ipg_amounttoapply").getValue();
            var amount = formContext.getAttribute(amountField).getValue();
            if (amount > amountToApply) {
                formContext.getControl(amountField).setNotification("Amount specified to write-off is greater than remaining balance.  Please revise the amount and try again.", amountField);
                formContext.getAttribute(amountField).setValue(amountToApply);
            }
        }
        Adjustment.OnAmountChange = OnAmountChange;
        Xrm.Page.OnAmountChange = OnAmountChange;
        function Calculate(formContext) {
            if (formContext.ui.getFormType() == 1) {
                var applyTo = formContext.getAttribute("ipg_applyto").getValue();
                var adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
                var amountType = formContext.getAttribute("ipg_amounttype").getValue();
                var reverse = formContext.getAttribute("ipg_reverse").getValue();
                var payment = formContext.getAttribute("ipg_payment").getValue();
                var transferOfPaymentType_1 = formContext.getAttribute("ipg_transferofpaymenttype").getValue();
                var from_1 = formContext.getAttribute("ipg_from").getValue();
                var to_1 = formContext.getAttribute("ipg_to").getValue();
                amountToApply = 0;
                var initialAmount = 0;
                if ((adjustmentType != 427880000) && (adjustmentType != 427880002) && (adjustmentType != 427880003) && (adjustmentType != 427880004)) {
                    SetInitialValues(formContext);
                }
                else if (adjustmentType == 427880003) {
                    SetInitialValues(formContext);
                    if (payment) {
                        Xrm.WebApi.retrieveRecord("ipg_payment", Intake.Utility.removeCurlyBraces(payment[0].id), "?$select=ipg_interestpayment,ipg_memberpaid_new,ipg_totalinsurancepaid").then(function success(result) {
                            var ipg_interestpayment = result["ipg_interestpayment"];
                            var ipg_memberpaid = result["ipg_memberpaid_new"];
                            var ipg_totalinsurancepaid = result["ipg_totalinsurancepaid"];
                            amountToApply = ipg_interestpayment + ipg_memberpaid + ipg_totalinsurancepaid;
                            if (transferOfPaymentType_1) {
                                if ((from_1 == 427880000) && (to_1 == 427880001)) {
                                    formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance + ipg_totalinsurancepaid + ipg_interestpayment);
                                    formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance - ipg_totalinsurancepaid - ipg_interestpayment);
                                }
                                else if ((from_1 == 427880000) && (to_1 == 427880002)) {
                                    formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance + ipg_totalinsurancepaid + ipg_interestpayment);
                                    formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance - ipg_totalinsurancepaid - ipg_interestpayment);
                                }
                                else if ((from_1 == 427880001) && (to_1 == 427880000)) {
                                    formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance - ipg_totalinsurancepaid - ipg_interestpayment);
                                    formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance + ipg_totalinsurancepaid + ipg_interestpayment);
                                }
                                else if ((from_1 == 427880001) && (to_1 == 427880002)) {
                                    formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance + ipg_totalinsurancepaid + ipg_interestpayment);
                                    formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance - ipg_totalinsurancepaid - ipg_interestpayment);
                                }
                                else if ((from_1 == 427880002) && (to_1 == 427880000)) {
                                    formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance - ipg_memberpaid);
                                    formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance + ipg_memberpaid);
                                }
                                else if ((from_1 == 427880002) && (to_1 == 427880001)) {
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
                        }, function (error) {
                            Xrm.Utility.alertDialog(error.message, null);
                        });
                    }
                }
                else if (adjustmentType == 427880004) {
                    SetInitialValues(formContext);
                    switch (from_1) {
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
                        if ((from_1 == 427880000) && (to_1 == 427880001)) {
                            formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance - amountToApply);
                            formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance + amountToApply);
                        }
                        else if ((from_1 == 427880000) && (to_1 == 427880002)) {
                            formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance - amountToApply);
                            formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance + amountToApply);
                        }
                        else if ((from_1 == 427880001) && (to_1 == 427880000)) {
                            formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance + amountToApply);
                            formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance - amountToApply);
                        }
                        else if ((from_1 == 427880001) && (to_1 == 427880002)) {
                            formContext.getAttribute("ipg_remainingsecondarycarrierbalance").setValue(initialSecondaryCarrierBalance - amountToApply);
                            formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance + amountToApply);
                        }
                        else if ((from_1 == 427880002) && (to_1 == 427880000)) {
                            formContext.getAttribute("ipg_remainingcarrierbalance").setValue(initialCarrierBalance + amountToApply);
                            formContext.getAttribute("ipg_remainingpatientbalance").setValue(initialPatientBalance - amountToApply);
                        }
                        else if ((from_1 == 427880002) && (to_1 == 427880001)) {
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
                var webResource = formContext.getControl("WebResource_ARSummary");
                var htmlObject = webResource.getObject();
                if (htmlObject) {
                    var src = htmlObject.src;
                    var aboutBlank = "about:blank";
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
        var removedReasons = [];
        function resetReasons(formContext) {
            var fieldName = "ipg_reason";
            for (var i = 0; i < formContext.getAttribute(fieldName).getOptions().length; i++) {
                var value = formContext.getAttribute(fieldName).getOptions()[i].value;
                formContext.getControl(fieldName).removeOption(Number(value));
                removedReasons.push(formContext.getAttribute(fieldName).getOptions()[i]);
            }
        }
        function FillReasons(formContext) {
            var fieldName = "ipg_reason";
            var reasonsControl = formContext.getControl(fieldName);
            reasonsControl.clearOptions();
            var applyTo = formContext.getAttribute("ipg_applyto").getValue();
            var adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
            if ((applyTo) && ((adjustmentType == 427880000) || (adjustmentType == 427880002))) {
                if (adjustmentType == 427880000) {
                    var reasons = [[427880021, 427880024, 427880023, 427880025, 427880026, 427880000, 427880020, 427880002, 427880001, 427880003, 427880008, 427880009],
                        [427880022, 427880019, 427880020, 427880004, 427880005, 427880007]];
                    reasons[(applyTo == 427880000) || (applyTo == 427880001) ? 0 : 1].forEach(function (reason) {
                        reasonsControl.addOption(removedReasons.find(function (r) { return r["value"] == reason; }));
                    });
                }
                else if (adjustmentType == 427880002) {
                    var reasons = [427880006];
                    reasons.forEach(function (reason) {
                        reasonsControl.addOption(removedReasons.find(function (r) { return r["value"] == reason; }));
                    });
                }
            }
            else if (adjustmentType == 427880003) {
                var reasons = [427880012];
                reasons.forEach(function (reason) {
                    reasonsControl.addOption(removedReasons.find(function (r) { return r["value"] == reason; }));
                });
            }
            else if (adjustmentType == 427880004) {
                var reasons = [427880010];
                reasons.forEach(function (reason) {
                    reasonsControl.addOption(removedReasons.find(function (r) { return r["value"] == reason; }));
                });
            }
        }
        /**
         * Called on Transfer of Payment Type field change
         * @function Intake.Adjustment.OnTransferOfPaymentTypeChange
         * @returns {void}
        */
        function OnTransferOfPaymentTypeChange(executionContext) {
            var formContext = executionContext.getFormContext();
            formContext.getAttribute("ipg_payment").setValue(null);
            var transferOfPaymentType = formContext.getAttribute("ipg_transferofpaymenttype").getValue();
            if (!transferOfPaymentType) {
                AddPresearchToPaymentsContintue(formContext, "");
            }
            SetCaseLabel(formContext);
            Calculate(formContext);
        }
        Adjustment.OnTransferOfPaymentTypeChange = OnTransferOfPaymentTypeChange;
        function SetCaseLabel(formContext) {
            formContext.getControl("ipg_caseid").setLabel(((formContext.getAttribute("ipg_adjustmenttype").getValue() == 427880003) && (!formContext.getAttribute("ipg_transferofpaymenttype").getValue())) ? "From Case" : "Case");
        }
        /**
         * Called on To Case field change
         * @function Intake.Adjustment.OnToCaseChange
         * @returns {void}
        */
        function OnToCaseChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var toCaseField = "ipg_tocase";
            formContext.getControl(toCaseField).clearNotification(toCaseField);
            var toCase = formContext.getAttribute(toCaseField).getValue();
            var caseId = formContext.getAttribute("ipg_caseid").getValue();
            if ((toCase) && (caseId) && (toCase[0].id == caseId[0].id)) {
                formContext.getControl(toCaseField).setNotification("You can't transfer payment to the same case", toCaseField);
                formContext.getAttribute(toCaseField).setValue(null);
            }
        }
        Adjustment.OnToCaseChange = OnToCaseChange;
        /**
         * Called on From field change
         * @function Intake.Adjustment.OnFromChange
         * @returns {void}
        */
        function OnFromChange(executionContext) {
            var formContext = executionContext.getFormContext();
            formContext.getAttribute("ipg_payment").setValue(null);
            var fieldName = "ipg_to";
            var toControl = formContext.getControl(fieldName);
            toControl.clearOptions();
            var from = formContext.getAttribute("ipg_from").getValue();
            if (from) {
                payerTypes.forEach(function (payerType) {
                    if (from != payerType.value) {
                        toControl.addOption(payerType);
                    }
                });
            }
            else {
                payerTypes.forEach(function (payerType) {
                    toControl.addOption(payerType);
                });
            }
            Calculate(formContext);
            AddPresearchToPayments(formContext);
        }
        Adjustment.OnFromChange = OnFromChange;
        /**
         * Called on To field change
         * @function Intake.Adjustment.OnToChange
         * @returns {void}
        */
        function OnToChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var fieldName = "ipg_to";
            formContext.getControl(fieldName).clearNotification(fieldName);
            if (formContext.getAttribute(fieldName).getValue() == 427880002) //patient
             {
                var caseId = formContext.getAttribute("ipg_caseid").getValue();
                if (caseId[0].id) {
                    Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId[0].id), "?$select=ipg_billtopatient").then(function success(result) {
                        if (result["ipg_billtopatient"] == 923720000) {
                            formContext.getControl(fieldName).setNotification("Bill to Patient is set to NO on the case. Therefore, balances cannot be transferred to the patient. If this patient should be billed by IPG, then set the Bill to Patient on the case to YES and repost the balance transfer transaction", fieldName);
                        }
                    }, function (error) {
                        Xrm.Utility.alertDialog(error.message, null);
                    });
                }
            }
            Calculate(formContext);
        }
        Adjustment.OnToChange = OnToChange;
        function AddPresearchToPayments(formContext) {
            var from = formContext.getAttribute("ipg_from").getValue();
            if ((from == 427880000) || (from == 427880001)) {
                var caseId_1 = formContext.getAttribute("ipg_caseid").getValue();
                if (caseId_1[0].id) {
                    Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId_1[0].id), "?$select=_ipg_carrierid_value,_ipg_secondarycarrierid_value").then(function success(result) {
                        Xrm.WebApi.retrieveMultipleRecords("invoice", "?$select=invoiceid,name&$filter=_ipg_carrierid_value eq " + Intake.Utility.removeCurlyBraces(result[from == 427880000 ? "_ipg_carrierid_value" : "_ipg_secondarycarrierid_value"]) + " and _ipg_caseid_value eq " + Intake.Utility.removeCurlyBraces(caseId_1[0].id)).then(function success(results) {
                            var paymentFilter = "<filter type='and'><condition attribute='ipg_claim' operator='in'>";
                            var conditions = "";
                            for (var i = 0; i < results.entities.length; i++) {
                                conditions += "<value uitype='invoice' uiname='" + results.entities[i]["name"] + "'>{" + results.entities[i]["invoiceid"] + "}</value>";
                            }
                            if (conditions) {
                                paymentFilter += conditions + "</condition></filter>";
                            }
                            else {
                                paymentFilter = "<filter type='and'>" +
                                    "<condition attribute='ipg_caseid' value='" + caseId_1[0].id + "' uitype='incident' uiname='" + caseId_1[0].name + "' operator='ne'/>" +
                                    "<condition attribute='ipg_caseid' value='" + caseId_1[0].id + "' uitype='incident' uiname='" + caseId_1[0].name + "' operator='eq'/>" +
                                    "</filter>";
                            }
                            AddPresearchToPaymentsContintue(formContext, paymentFilter);
                        }, function (error) {
                            Xrm.Utility.alertDialog(error.message, null);
                        });
                    }, function (error) {
                        Xrm.Utility.alertDialog(error.message, null);
                    });
                }
            }
            else if (from == 427880002) {
                var paymentFilter = "<filter type='and'><condition attribute='ipg_memberpaid_new' operator='ne' value='0'/></filter>";
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
        function OnPaymentChange(executionContext) {
            var formContext = executionContext.getFormContext();
            Calculate(formContext);
        }
        Adjustment.OnPaymentChange = OnPaymentChange;
        function FilterFromToFields(formContext) {
            var adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
            var caseId = formContext.getAttribute("ipg_caseid").getValue();
            if (caseId && (adjustmentType == 427880004)) {
                Xrm.WebApi.retrieveRecord("incident", caseId[0].id, "?$select=_ipg_secondarycarrierid_value").then(function success(result) {
                    var index = payerTypes.findIndex(function (x) { return x.value == 427880001; });
                    if ((index >= 0) && !result["_ipg_secondarycarrierid_value"]) {
                        var fieldNames = ["ipg_from", "ipg_to"];
                        fieldNames.forEach(function (fieldName) {
                            var control = formContext.getControl(fieldName);
                            control.clearOptions();
                            payerTypes.forEach(function (payerType) {
                                if (payerType.value != 427880001) {
                                    control.addOption(payerType);
                                }
                            });
                        });
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message, null);
                });
            }
        }
        function SetDefaultReason(formContext) {
            var defaultReason = null;
            var reasonField = "ipg_reason";
            var adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
            switch (adjustmentType) {
                case 427880000:
                    var applyTo = formContext.getAttribute("ipg_applyto").getValue();
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
            if (formContext.getControl(reasonField).getOptions().find(function (e) { return e.value == defaultReason; })) {
                formContext.getAttribute(reasonField).setValue(defaultReason);
            }
            else {
                formContext.getAttribute(reasonField).setValue(null);
            }
        }
        function SetVisibility(formContext) {
            var adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
            formContext.getControl("WebResource_RadioButtonList").setVisible(adjustmentType != 427880003);
        }
        function CheckIfCaseIsClosed(formContext, caseId) {
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
        function OnFormSave(executionContext) {
            var formContext = executionContext.getFormContext();
            var amountToApply = formContext.getAttribute("ipg_amounttoapply").getValue();
            if (amountToApply == 0) {
                Xrm.Utility.alertDialog("You can't apply a zero adjustment.", function () { return; });
                executionContext.getEventArgs().preventDefault();
            }
        }
        Adjustment.OnFormSave = OnFormSave;
    })(Adjustment = Intake.Adjustment || (Intake.Adjustment = {}));
})(Intake || (Intake = {}));
