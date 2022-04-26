var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
/**
 * @namespace Intake.Payment
 */
var Intake;
(function (Intake) {
    var Payment;
    (function (Payment) {
        var globalFormContext;
        /**
        * Shows warning if Expiration Date is less than Effective Date
        * @function Intake.Payment.ClaimOnChange
        * @returns {void}
        */
        function ClaimOnChange(executionContext) {
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
                callAction("ipg_IPGIntakeActionProcessClaimLineItems", parameters, true, function (results) {
                    formContext.ui.quickForms._getByName("RemitDetailsForm").refresh();
                });
            }
        }
        Payment.ClaimOnChange = ClaimOnChange;
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
                    }
                    else {
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
        function OnLoadPatientPaymentForm(executionContext) {
            var formContext = executionContext.getFormContext();
            OnLoadForm(formContext);
            if (formContext.ui.getFormType() == 2) {
                var fieldsArray = ["ipg_paymentdate", "ipg_memberpaid_new", "ipg_checknumber", "ipg_notes"];
                fieldsArray.forEach(function (value) {
                    formContext.getControl(value).setDisabled(true);
                });
            }
            AdjustmentOnLoad(formContext);
        }
        Payment.OnLoadPatientPaymentForm = OnLoadPatientPaymentForm;
        /**
         * Called on load carrier payment form
         * @function Intake.Payment.OnLoadCarrierPaymentForm
         * @returns {void}
        */
        function OnLoadCarrierPaymentForm(executionContext) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, refund, headerValue, header;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = executionContext.getFormContext();
                            OnLoadForm(formContext);
                            refund = false;
                            headerValue = formContext.getAttribute('ipg_claimresponseheader').getValue();
                            if (!headerValue) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord('ipg_claimresponseheader', Intake.Utility.removeCurlyBraces(headerValue[0].id), '?$select=ipg_paymenttype')];
                        case 1:
                            header = _a.sent();
                            refund = (header.ipg_paymenttype == 427880002);
                            _a.label = 2;
                        case 2:
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
                            return [2 /*return*/];
                    }
                });
            });
        }
        Payment.OnLoadCarrierPaymentForm = OnLoadCarrierPaymentForm;
        var amountToApply = 0;
        var initialCarrierBalance = 0;
        var initialSecondaryCarrierBalance = 0;
        var initialPatientBalance = 0;
        var initialBalance = 0;
        var payerTypes;
        var activeFilter = "";
        function AdjustmentOnLoad(formContext) {
            SetVisibility(formContext);
            formContext.getControl("ipg_adjustmenttype").removeOption(427880003);
            payerTypes = formContext.getControl("ipg_to").getOptions();
            if (formContext.ui.getFormType() == 1) {
                var caseId_1 = formContext.getAttribute("ipg_caseid").getValue();
                if (caseId_1) {
                    Xrm.WebApi.retrieveRecord("incident", caseId_1[0].id, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
                        initialCarrierBalance = incident.ipg_remainingcarrierbalance;
                        initialSecondaryCarrierBalance = incident.ipg_remainingsecondarycarrierbalance;
                        initialPatientBalance = incident.ipg_remainingpatientbalance;
                        initialBalance = incident.ipg_casebalance;
                        var claimResponseHeader = formContext.getAttribute("ipg_claimresponseheader").getValue()[0].id;
                        Xrm.WebApi.retrieveRecord("ipg_claimresponseheader", claimResponseHeader, "?$select=ipg_amountpaid_new,_ipg_claimresponsebatchid_value").then(function success(result) {
                            var ipg_amountpaid = result["ipg_amountpaid_new"];
                            var _ipg_claimresponsebatchid_value = result["_ipg_claimresponsebatchid_value"];
                            Xrm.WebApi.retrieveRecord("ipg_claimresponsebatch", _ipg_claimresponsebatchid_value, "?$select=ipg_paymentfrom").then(function success(result) {
                                var ipg_paymentfrom = result["ipg_paymentfrom"];
                                if (ipg_paymentfrom == 427880000) {
                                    var claim = formContext.getAttribute("ipg_claim").getValue();
                                    if (claim) {
                                        var claimId = claim[0].id;
                                        Xrm.WebApi.retrieveRecord("invoice", claimId, "?$select=_customerid_value").then(function success(result) {
                                            var _customerid_value = result["_customerid_value"];
                                            Xrm.WebApi.retrieveRecord("incident", caseId_1[0].id, "?$select=_ipg_carrierid_value,_ipg_secondarycarrierid_value").then(function success(result) {
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
                                            }, function (error) {
                                                Xrm.Utility.alertDialog(error.message, null);
                                            });
                                        }, function (error) {
                                            Xrm.Utility.alertDialog(error.message, null);
                                        });
                                    }
                                }
                                else {
                                    initialPatientBalance -= ipg_amountpaid;
                                    initialBalance -= ipg_amountpaid;
                                    Calculate(formContext);
                                }
                            }, function (error) {
                                Xrm.Utility.alertDialog(error.message, null);
                            });
                        }, function (error) {
                            Xrm.Utility.alertDialog(error, null);
                        });
                    }, function (error) {
                        Xrm.Navigation.openErrorDialog({ message: error.message });
                    });
                }
                resetReasons(formContext);
                formContext.getAttribute("ipg_percent").setValue(100);
            }
            else {
                var arr = ["ipg_applyto", "ipg_adjustmenttype", "ipg_amounttype", "ipg_percent", "ipg_amount", "ipg_reason", "ipg_from", "ipg_to"];
                for (var _i = 0, arr_1 = arr; _i < arr_1.length; _i++) {
                    var control = arr_1[_i];
                    if (formContext.getControl(control)) {
                        formContext.getControl(control).setDisabled(true);
                    }
                }
            }
        }
        function Calculate(formContext) {
            if (formContext.ui.getFormType() == 1) {
                var applyTo = formContext.getAttribute("ipg_applyto").getValue();
                var adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
                var amountType = formContext.getAttribute("ipg_amounttype").getValue();
                var from = formContext.getAttribute("ipg_from").getValue();
                var to = formContext.getAttribute("ipg_to").getValue();
                amountToApply = 0;
                SetInitialValues(formContext);
                var initialAmount = 0;
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
        /**
         * Called on Apply To field change
         * @function Intake.Payment.OnApplyToChange
         * @returns {void}
        */
        function OnApplyToChange(executionContext) {
            var formContext = executionContext.getFormContext();
            Calculate(formContext);
            FillReasons(formContext);
            SetDefaultReason(formContext);
        }
        Payment.OnApplyToChange = OnApplyToChange;
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
            else if (adjustmentType == 427880004) {
                var reasons = [427880010];
                reasons.forEach(function (reason) {
                    reasonsControl.addOption(removedReasons.find(function (r) { return r["value"] == reason; }));
                });
            }
        }
        function SetDefaultReason(formContext) {
            var defaultReason = null;
            var adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
            switch (adjustmentType) {
                case 427880000:
                    var applyTo = formContext.getAttribute("ipg_applyto").getValue();
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
        function Recalculate(executionContext) {
            var formContext = executionContext.getFormContext();
            Calculate(formContext);
            FillReasons(formContext);
        }
        Payment.Recalculate = Recalculate;
        /**
         * Called on Adjustment Type field change
         * @function Intake.Payment.OnAdjustmentTypeChange
         * @returns {void}
        */
        function OnAdjustmentTypeChange(executionContext) {
            var formContext = executionContext.getFormContext();
            SetVisibility(formContext);
            Calculate(formContext);
            FillReasons(formContext);
            FilterFromToFields(formContext);
            SetDefaultReason(formContext);
        }
        Payment.OnAdjustmentTypeChange = OnAdjustmentTypeChange;
        function FilterFromToFields(formContext) {
            var adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
            var caseId = formContext.getAttribute("ipg_caseid").getValue();
            if (caseId && (adjustmentType == 427880004)) {
                Xrm.WebApi.retrieveRecord("incident", caseId[0].id, "?$select=ipg_secondarycarrier,ipg_secondarycarrierstatus").then(function success(result) {
                    var index = payerTypes.findIndex(function (x) { return x.value == 427880001; });
                    if ((index >= 0) && (!result["ipg_secondarycarrier"] || !result["ipg_secondarycarrierstatus"])) {
                        payerTypes.splice(index, 1);
                        var fieldNames = ["ipg_from", "ipg_to"];
                        fieldNames.forEach(function (fieldName) {
                            var control = formContext.getControl(fieldName);
                            control.clearOptions();
                            payerTypes.forEach(function (payerType) {
                                control.addOption(payerType);
                            });
                        });
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message, null);
                });
            }
        }
        /**
         * Called on Amount Type field change
         * @function Intake.Payment.OnAmountTypeChange
         * @returns {void}
        */
        function OnAmountTypeChange(executionContext) {
            var formContext = executionContext.getFormContext();
            Calculate(formContext);
        }
        Payment.OnAmountTypeChange = OnAmountTypeChange;
        /**
         * Called on Percent field change
         * @function Intake.Payment.OnPercentChange
         * @returns {void}
        */
        function OnPercentChange() {
            var formContext = globalFormContext;
            Calculate(formContext);
        }
        Payment.OnPercentChange = OnPercentChange;
        Xrm.Page.OnPercentChange = OnPercentChange;
        /**
         * Called on Amount field change
         * @function Intake.Payment.OnAmountChange
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
        Payment.OnAmountChange = OnAmountChange;
        Xrm.Page.OnAmountChange = OnAmountChange;
        /**
         * Called on From field change
         * @function Intake.Payment.OnFromChange
         * @returns {void}
        */
        function OnFromChange(executionContext) {
            var formContext = executionContext.getFormContext();
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
        }
        Payment.OnFromChange = OnFromChange;
        /**
         * Called on To field change
         * @function Intake.Payment.OnToChange
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
        Payment.OnToChange = OnToChange;
        /**
         * Called on Apply Adjustment change
         * @function Intake.Payment.OnApplyAdjustmentChange
         * @returns {void}
        */
        function OnApplyAdjustmentChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var fieldName = "ipg_to";
            formContext.getControl(fieldName).clearNotification(fieldName);
            SetVisibility(formContext);
        }
        Payment.OnApplyAdjustmentChange = OnApplyAdjustmentChange;
        function SetVisibility(formContext) {
            var applyAdjustment = formContext.getAttribute("ipg_applyadjustment").getValue();
            var adjustmentType = formContext.getAttribute("ipg_adjustmenttype").getValue();
            formContext.getControl("WebResource_RadioButtonList").setVisible(applyAdjustment && (adjustmentType != 427880003));
        }
        /**
       * Called on saving form
       * @function Intake.Payment.OnSaveForm
       * @returns {void}
      */
        function OnSaveForm(executionContext) {
            //OOB bug. Resolution: https://community.dynamics.com/365/unified-interface/f/unified-interface-forum/377366/dialog-box-message-displaying-when-using-formcontext-data-save-then-or-formcontext-data-save/1021229#1021229
            var formContext = executionContext.getFormContext();
            if (formContext.ui.getFormType() == 1) {
                formContext.data.entity.addOnSave(function () {
                });
            }
        }
        Payment.OnSaveForm = OnSaveForm;
        function OnLoadForm(formContext) {
            return __awaiter(this, void 0, void 0, function () {
                var headerValue, header, batch, isCarrierPayment, carrierFormId, patientFormId, currentFormId, items, i, items, i;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            globalFormContext = formContext;
                            headerValue = formContext.getAttribute('ipg_claimresponseheader').getValue();
                            if (!headerValue) return [3 /*break*/, 3];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord('ipg_claimresponseheader', Intake.Utility.removeCurlyBraces(headerValue[0].id), '?$select=_ipg_claimresponsebatchid_value')];
                        case 1:
                            header = _a.sent();
                            if (!header._ipg_claimresponsebatchid_value) return [3 /*break*/, 3];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord('ipg_claimresponsebatch', header._ipg_claimresponsebatchid_value, '?$select=ipg_paymentfrom')];
                        case 2:
                            batch = _a.sent();
                            isCarrierPayment = (batch.ipg_paymentfrom == 427880000);
                            carrierFormId = '2708FC53-BBD4-4D52-B91E-56C5524B97F4'.toLocaleLowerCase();
                            patientFormId = '05A46600-F1C0-450E-9846-73E3E82829EB'.toLocaleLowerCase();
                            currentFormId = formContext.ui.formSelector.getCurrentItem().getId().toLocaleLowerCase();
                            if (isCarrierPayment && (carrierFormId != currentFormId)) {
                                items = formContext.ui.formSelector.items.get();
                                for (i in items) {
                                    if (items[i].getId() == carrierFormId) {
                                        items[i].navigate();
                                    }
                                }
                            }
                            else if (!isCarrierPayment && (patientFormId != currentFormId)) {
                                items = formContext.ui.formSelector.items.get();
                                for (i in items) {
                                    if (items[i].getId() == patientFormId) {
                                        items[i].navigate();
                                    }
                                }
                            }
                            _a.label = 3;
                        case 3:
                            FilterTasks(formContext);
                            return [2 /*return*/];
                    }
                });
            });
        }
        function FilterTasks(formContext) {
            var tasksControl = Xrm.Page.getControl("Tasks");
            var caseId = formContext.getAttribute("ipg_caseid").getValue();
            if (caseId) {
                var fetxtXml = "<fetch><entity name=\"task\"><filter type=\"and\"><condition attribute=\"regardingobjectid\" operator=\"eq\" value=\"" + Intake.Utility.removeCurlyBraces(caseId[0].id) + "\" uitype=\"incident\" uiname=\"" + caseId[0].name + "\" /><condition attribute=\"ipg_tasktypecode\" value=\"427880065\" operator=\"eq\"/></filter></entity></fetch>";
                tasksControl.setFilterXml(fetxtXml);
                tasksControl.refresh();
            }
        }
    })(Payment = Intake.Payment || (Intake.Payment = {}));
})(Intake || (Intake = {}));
