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
 * @namespace Intake.Payment.Ribbon
 */
var Intake;
(function (Intake) {
    var Payment;
    (function (Payment) {
        var Ribbon;
        (function (Ribbon) {
            /**
             * Called on 'Add Manual Payment' button clickn
             * @function Intake.Payment.Ribbon.AddManualPayment
             * @returns {void}
            */
            function AddManualPayment(primaryControl) {
                var formContext = primaryControl;
                var formParameters = {
                    incidentId: formContext.data.entity.getId()
                };
                var webResourceName = "ipg_/intake/claim/PaymentProcessing.html";
                var customParameters = encodeURIComponent("params=" + JSON.stringify(formParameters));
                Xrm.Navigation.openWebResource(webResourceName, null, customParameters);
            }
            Ribbon.AddManualPayment = AddManualPayment;
            /**
             * Called on Additional Details button click
             * @function Intake.Payment.Ribbon.OnAdditionalDetailsClick
             * @returns {void}
            */
            function OnAdditionalDetailsClick(firstSelectedItemId, primaryControl) {
                var formContext = primaryControl;
                if (firstSelectedItemId == null)
                    return;
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_payment";
                entityFormOptions["entityId"] = firstSelectedItemId.replace(/[{}]/g, "");
                entityFormOptions["formId"] = "4c6e5b46-79fd-4f21-acb7-ca1269d5a1b2";
                Xrm.Navigation.openForm(entityFormOptions);
            }
            Ribbon.OnAdditionalDetailsClick = OnAdditionalDetailsClick;
            /**
             * Called on Add Adjustment button click
             * @function Intake.Payment.Ribbon.OnAddAdjustmentClick
             * @returns {void}
            */
            function OnAddAdjustmentClick(primaryControl) {
                var formContext = primaryControl;
                var caseId = formContext.getAttribute("ipg_caseid").getValue();
                if (!caseId) {
                    return;
                }
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_adjustment";
                Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId[0].id), "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
                    var formParameters = {};
                    formParameters["ipg_caseid"] = Intake.Utility.removeCurlyBraces(caseId[0].id);
                    formParameters["ipg_caseidname"] = incident.title;
                    formParameters["ipg_caseidtype"] = caseId[0].entityType;
                    formParameters["ipg_percent"] = 100;
                    formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
                    formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
                    formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
                    formParameters["ipg_casebalance"] = incident.ipg_casebalance;
                    Xrm.Navigation.openForm(entityFormOptions, formParameters);
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
            Ribbon.OnAddAdjustmentClick = OnAddAdjustmentClick;
            /**
             * Called on Cancel button click
             * @function Intake.Payment.Ribbon.OnCancelClick
             * @returns {void}
            */
            function OnCancelClick(primaryControl) {
                window.history.back();
            }
            Ribbon.OnCancelClick = OnCancelClick;
            /**
             * Called on Save button click
             * @function Intake.Payment.Ribbon.OnSaveClick
             * @returns {void}
            */
            function OnSaveClick(formContext) {
                formContext.data.save().then(function (success) {
                    callValidatePaymentAction(formContext);
                    checkAdjustmentTask(formContext);
                }, function (error) {
                    console.log(error.message);
                });
            }
            Ribbon.OnSaveClick = OnSaveClick;
            function callValidatePaymentAction(formContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var target, parameters, ipg_IPGPaymentPreValidatePayment;
                    return __generator(this, function (_a) {
                        target = {
                            ipg_paymentid: Intake.Utility.removeCurlyBraces(formContext.data.entity.getId())
                        };
                        target["@odata.type"] = "Microsoft.Dynamics.CRM.ipg_payment";
                        parameters = {
                            Target: target,
                        };
                        ipg_IPGPaymentPreValidatePayment = {
                            Target: parameters.Target,
                            getMetadata: function () {
                                return {
                                    boundParameter: null,
                                    parameterTypes: {
                                        "Target": {
                                            "typeName": "mscrm.ipg_payment",
                                            "structuralProperty": 5
                                        }
                                    },
                                    operationType: 0,
                                    operationName: "ipg_IPGPaymentPreValidatePayment"
                                };
                            }
                        };
                        Xrm.WebApi.online.execute(ipg_IPGPaymentPreValidatePayment).then(function success(result) {
                            return __awaiter(this, void 0, void 0, function () {
                                var response;
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0:
                                            if (!result.ok) return [3 /*break*/, 2];
                                            return [4 /*yield*/, result.json()];
                                        case 1:
                                            response = _a.sent();
                                            if (response.Message) {
                                                Xrm.Utility.alertDialog(response.Message, null);
                                            }
                                            _a.label = 2;
                                        case 2: return [2 /*return*/];
                                    }
                                });
                            });
                        }, function (error) {
                            Xrm.Utility.alertDialog(error.message, null);
                        });
                        return [2 /*return*/];
                    });
                });
            }
            function checkAdjustmentTask(formContext) {
                var caseId = formContext.getAttribute("ipg_caseid").getValue();
                if (caseId) {
                    Xrm.WebApi.online.retrieveMultipleRecords("task", "?$select=activityid,subject&$filter=_regardingobjectid_value eq " + Intake.Utility.removeCurlyBraces(caseId[0].id) + " and ipg_tasktypecode eq 427880065 and statecode eq 0").then(function success(results) {
                        if (results.entities.length) {
                            if (confirm("The task \"" + results.entities[0]["subject"] + "\" is assigned to the case " + caseId[0].name + ". Do you want to open it?")) {
                                var entityFormOptions = {};
                                entityFormOptions["entityName"] = "task";
                                entityFormOptions["entityId"] = results.entities[0]["activityid"];
                                Xrm.Navigation.openForm(entityFormOptions);
                            }
                        }
                    }, function (error) {
                        Xrm.Utility.alertDialog(error.message, null);
                    });
                }
            }
        })(Ribbon = Payment.Ribbon || (Payment.Ribbon = {}));
    })(Payment = Intake.Payment || (Intake.Payment = {}));
})(Intake || (Intake = {}));
