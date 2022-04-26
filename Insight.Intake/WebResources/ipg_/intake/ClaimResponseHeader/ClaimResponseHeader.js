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
 * @namespace Intake.ClaimResponseHeader
 */
var Intake;
(function (Intake) {
    var ClaimResponseHeader;
    (function (ClaimResponseHeader) {
        /**
        * Called on Form Load event
        * @function Intake.ClaimResponseHeader.OnLoadForm
        * @returns {void}
        */
        var globalFormContext;
        var isCarrierBatch = false;
        function OnLoadForm(executionContext) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, isManualPaymentAttr, batchValue, isRefund, paymentType, batch, isManualBatch, tabObj;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = executionContext.getFormContext();
                            globalFormContext = formContext;
                            isManualPaymentAttr = formContext.getAttribute("ipg_ismanualpayment");
                            batchValue = formContext.getAttribute('ipg_claimresponsebatchid').getValue();
                            isRefund = false;
                            paymentType = formContext.getAttribute('ipg_paymenttype').getValue();
                            if (!(paymentType == 427880002)) return [3 /*break*/, 1];
                            isRefund = true;
                            return [3 /*break*/, 3];
                        case 1:
                            if (!!paymentType) return [3 /*break*/, 3];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord('ipg_claimresponsebatch', Intake.Utility.removeCurlyBraces(batchValue[0].id), '?$select=ipg_type,ipg_paymentdate,ipg_paymentfrom')];
                        case 2:
                            batch = _a.sent();
                            isRefund = batch.ipg_type;
                            isCarrierBatch = (batch.ipg_paymentfrom == 427880000);
                            if (isRefund) {
                                formContext.getAttribute('ipg_paymenttype').setValue(427880002);
                                formContext.getAttribute('ipg_receiveddate').setValue(new Date(batch.ipg_paymentdate));
                                formContext.getControl('ipg_amountpaid_new4').setDisabled(false);
                            }
                            _a.label = 3;
                        case 3:
                            if (formContext.ui.getFormType() == 1) {
                                isManualPaymentAttr.setValue(true);
                                formContext.getAttribute('ipg_poststatus').setValue('new');
                            }
                            isManualBatch = isManualPaymentAttr != null ? isManualPaymentAttr.getValue() : false;
                            tabObj = formContext.ui.tabs.get("CarrierManualPaymentTab");
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
                            return [2 /*return*/];
                    }
                });
            });
        }
        ClaimResponseHeader.OnLoadForm = OnLoadForm;
        /**
        * Called on CaseId Change event
        * @function Intake.ClaimResponseHeader.OnChangeCaseId
        * @returns {void}
      */
        function OnChangeCaseId(executionContext) {
            //debugger;
            var formContext = executionContext.getFormContext();
            var caseId = formContext.getAttribute("ipg_caseid");
            if (formContext.getAttribute('ipg_paymenttype').getValue() != 427880002) {
                if (caseId != null) {
                    formContext.getAttribute("ipg_casenumber").setValue(caseId.getValue()[0].name);
                    if (isCarrierBatch) {
                        Xrm.WebApi.retrieveMultipleRecords("invoice", "?$top=1&$select=ipg_active,name,invoiceid,statecode,_customerid_value&$filter=_ipg_caseid_value eq " + caseId.getValue()[0].id + " and statecode eq 0")
                            .then(function (invoices) {
                            if (invoices.entities.length == 0) {
                                Xrm.Utility.alertDialog("Selected Case has no active claim(s).", function () { return; });
                            }
                            else {
                                invoices.entities.forEach(function (invoice) {
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
                        Xrm.WebApi.retrieveRecord('incident', caseId.getValue()[0].id, '?$select=_ipg_patientid_value').then(function success(result) {
                            formContext.getAttribute('ipg_origref').setValue(result['_ipg_patientid_value@OData.Community.Display.V1.FormattedValue']);
                        }, function (error) {
                            Xrm.Utility.alertDialog(error.message, null);
                        });
                    }
                }
            }
            else {
                if (caseId.getValue()) {
                    formContext.getAttribute('ipg_casenumber').setValue(caseId.getValue()[0].name);
                    if (!formContext.getAttribute('ipg_refundtype').getValue()) {
                        Xrm.WebApi.retrieveRecord('incident', caseId.getValue()[0].id, '?$select=_ipg_carrierid_value').then(function success(result) {
                            var carrierName = result['_ipg_carrierid_value@OData.Community.Display.V1.FormattedValue'];
                            var carrier = {
                                id: result['_ipg_carrierid_value'],
                                name: carrierName,
                                entityType: result['_ipg_carrierid_value@Microsoft.Dynamics.CRM.lookuplogicalname']
                            };
                            formContext.getAttribute('ipg_carrierid').setValue([carrier]);
                            formContext.getAttribute('ipg_paidto').setValue(carrierName);
                        }, function (error) {
                            Xrm.Utility.alertDialog(error.message, null);
                        });
                    }
                    else {
                        Xrm.WebApi.retrieveRecord('incident', caseId.getValue()[0].id, '?$select=_ipg_patientid_value').then(function success(result) {
                            formContext.getAttribute('ipg_carrierid').setValue(null);
                            formContext.getAttribute('ipg_paidto').setValue(result['_ipg_patientid_value@OData.Community.Display.V1.FormattedValue']);
                        }, function (error) {
                            Xrm.Utility.alertDialog(error.message, null);
                        });
                    }
                }
                else {
                    formContext.getAttribute('ipg_carrierid').setValue(null);
                    formContext.getAttribute('ipg_paidto').setValue(null);
                    formContext.getAttribute('ipg_casenumber').setValue(null);
                }
            }
        }
        ClaimResponseHeader.OnChangeCaseId = OnChangeCaseId;
        /**
        * Called on form Save event
        * @function Intake.ClaimResponseHeader.OnFormSave
        * @returns {void}
      */
        function OnFormSave(executionContext) {
            //debugger;
            var formContext = executionContext.getFormContext();
            var caseId = formContext.getAttribute("ipg_caseid");
            var claimId = formContext.getAttribute("ipg_claimid");
            if (caseId == null || claimId == null || caseId.getValue() == null || claimId.getValue() == null) {
                Xrm.Utility.alertDialog("Case and Claim must be populated.", function () { return; });
                executionContext.getEventArgs().preventDefault();
            }
        }
        ClaimResponseHeader.OnFormSave = OnFormSave;
        /**
        * Called on Add Adjustment click
        * @function Intake.ClaimResponseHeader.AddAdjustment
        * @returns {void}
      */
        function AddAdjustment(primaryControl, selectedItems) {
            //debugger;
            var formContext = primaryControl;
            var claimLineId = selectedItems[0].Id;
            var claimLineName = selectedItems[0].Name;
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "ipg_claimresponselineadjustment";
            //entityFormOptions["formId"] = formId;
            entityFormOptions["useQuickCreateForm"] = true;
            // Set default values for the ModalDialog form
            var formParameters = {};
            formParameters["ipg_claimresponselineid"] = [{ entityType: "ipg_claimresponseline", id: claimLineId, name: claimLineName }];
            // Open the form.
            Xrm.Navigation.openForm(entityFormOptions, formParameters)
                .then(function (result) {
                if (result != null && result.savedEntityReference.length == 1) {
                    var gridContext = formContext.getControl("ManualCarrierPaymentLines");
                    if (gridContext != null) {
                        gridContext.refresh();
                    }
                    formContext.data.refresh(false);
                }
            });
        }
        ClaimResponseHeader.AddAdjustment = AddAdjustment;
        /**
        * Called on Add Remark click
        * @function Intake.ClaimResponseHeader.AddRemark
        * @returns {void}
      */
        function AddRemark(primaryControl, selectedItems) {
            //debugger;
            var formContext = primaryControl;
            var claimLineId = selectedItems[0].Id;
            var claimLineName = selectedItems[0].Name;
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "ipg_claimresponselineremark";
            //entityFormOptions["formId"] = formId;
            entityFormOptions["useQuickCreateForm"] = true;
            // Set default values for the ModalDialog form
            var formParameters = {};
            formParameters["ipg_claimresponselineid"] = [{ entityType: "ipg_claimresponseline", id: claimLineId, name: claimLineName }];
            // Open the form.
            Xrm.Navigation.openForm(entityFormOptions, formParameters)
                .then(function (result) {
                if (result != null && result.savedEntityReference.length == 1) {
                    var gridContext = formContext.getControl("ManualCarrierPaymentLines");
                    if (gridContext != null) {
                        gridContext.refresh();
                    }
                    formContext.data.refresh(false);
                }
            });
        }
        ClaimResponseHeader.AddRemark = AddRemark;
        /**
        * Called on ManualCarrierPaymentLines grid save
        * @function Intake.ClaimResponseHeader.OnSaveManualCarrierPaymentLinesGrid
        * @returns {void}
      */
        function OnSaveManualCarrierPaymentLinesGrid(executionContext) {
            setTimeout(function () {
                globalFormContext.data.refresh(false);
            }, 500);
        }
        ClaimResponseHeader.OnSaveManualCarrierPaymentLinesGrid = OnSaveManualCarrierPaymentLinesGrid;
        /**
        * Called on Claim Change event
        * @function Intake.ClaimResponseHeader.OnClaimChange
        * @returns {void}
        */
        function OnClaimChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var claimId = formContext.getAttribute("ipg_claimid").getValue();
            if (claimId) {
                formContext.getAttribute("ipg_claimnumber").setValue(claimId[0].name);
            }
        }
        ClaimResponseHeader.OnClaimChange = OnClaimChange;
    })(ClaimResponseHeader = Intake.ClaimResponseHeader || (Intake.ClaimResponseHeader = {}));
})(Intake || (Intake = {}));
