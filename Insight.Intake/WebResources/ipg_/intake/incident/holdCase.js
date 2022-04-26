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
var Intake;
(function (Intake) {
    var Incident;
    (function (Incident) {
        var HoldCase;
        (function (HoldCase_1) {
            var Xrm = window.parent.Xrm;
            var holdReasons = [];
            var inputData = JSON.parse(getUrlParameter("data"));
            function init() {
                return __awaiter(this, void 0, void 0, function () {
                    var caseId, caseTarget, _a;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                Xrm.Utility.showProgressIndicator("Opening...");
                                if (inputData.action == "unhold") {
                                    $("#lblHoldReason").hide();
                                    $("#holdReason").hide();
                                    $("#lblHeader").text("Remove hold");
                                }
                                else if (inputData.action == "changehold") {
                                    $("#lblHeader").text("Change hold Reason");
                                    $("#holdNote").hide();
                                    $("#holdNote").hide();
                                }
                                if (!!Array.isArray(inputData.caseId)) return [3 /*break*/, 4];
                                caseId = inputData.caseId;
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=ipg_casehold,ipg_statecode")];
                            case 1:
                                caseTarget = _b.sent();
                                if (!(inputData.action != "unhold")) return [3 /*break*/, 4];
                                _a = caseTarget.ipg_statecode;
                                if (!_a) return [3 /*break*/, 3];
                                return [4 /*yield*/, CrmManager.LoadOptionsByCaseStateCodeAsync(caseTarget.ipg_statecode, holdReasons)];
                            case 2:
                                _a = (_b.sent());
                                _b.label = 3;
                            case 3:
                                _a;
                                if (holdReasons.length == 0) {
                                    Xrm.Navigation.openErrorDialog({ message: "Case can not be put on Hold because of current state!" }).then(function () { return window.close(); });
                                }
                                holdReasons.forEach(function (p) { return $("#holdReason").prepend("<option value=\"" + p.value + "\">" + p.text + "</option>"); });
                                $("#holdReason option:first-child").prop("selected", "selected");
                                if ($("#holdReason option").length == 0) {
                                    $("#okBtn").attr("disabled", "disabled");
                                }
                                _b.label = 4;
                            case 4:
                                Xrm.Utility.closeProgressIndicator();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            HoldCase_1.init = init;
            function SendRequest(request, onSuccess, onError) {
                return __awaiter(this, void 0, void 0, function () {
                    var target, parameterTypes, ipg_IPGCaseActionsHoldCaseRequest, result, _a, error_1;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                target = {
                                    incidentid: request.caseId,
                                    "@odata.type": "Microsoft.Dynamics.CRM.incident"
                                };
                                parameterTypes = {
                                    "Target": {
                                        "typeName": "mscrm.incident",
                                        "structuralProperty": 5
                                    },
                                    "IsOnHold": {
                                        "typeName": "Edm.Boolean",
                                        "structuralProperty": 1
                                    },
                                    "HoldNote": {
                                        "typeName": "Edm.String",
                                        "structuralProperty": 1
                                    },
                                    "HoldReason": {
                                        "typeName": "Edm.String",
                                        "structuralProperty": 1
                                    }
                                };
                                ipg_IPGCaseActionsHoldCaseRequest = {
                                    Target: target,
                                    IsOnHold: request.isOnHold
                                };
                                if (request.holdReason) {
                                    ipg_IPGCaseActionsHoldCaseRequest["HoldReason"] = request.holdReason;
                                }
                                if (request.holdNote) {
                                    ipg_IPGCaseActionsHoldCaseRequest["HoldNote"] = request.holdNote;
                                }
                                ipg_IPGCaseActionsHoldCaseRequest["getMetadata"] = function () {
                                    return {
                                        boundParameter: null,
                                        parameterTypes: parameterTypes,
                                        operationType: 0,
                                        operationName: "ipg_IPGCaseActionsHoldCase"
                                    };
                                };
                                _b.label = 1;
                            case 1:
                                _b.trys.push([1, 5, , 6]);
                                return [4 /*yield*/, Xrm.WebApi.online.execute(ipg_IPGCaseActionsHoldCaseRequest)];
                            case 2:
                                result = _b.sent();
                                if (!result.ok) return [3 /*break*/, 4];
                                _a = onSuccess;
                                return [4 /*yield*/, result.json()];
                            case 3:
                                _a.apply(void 0, [_b.sent()]);
                                _b.label = 4;
                            case 4: return [3 /*break*/, 6];
                            case 5:
                                error_1 = _b.sent();
                                onError(error_1.message);
                                return [3 /*break*/, 6];
                            case 6: return [2 /*return*/];
                        }
                    });
                });
            }
            function HoldCase() {
                return __awaiter(this, void 0, void 0, function () {
                    var requestData, isHold, holdReason, holdNotes, ProcessCase, caseIds, i;
                    var _this = this;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                requestData = inputData;
                                isHold = requestData.action === "hold" || requestData.action == "changehold";
                                holdReason = $("#holdReason").val();
                                holdNotes = $("#holdNote").val();
                                if ((isHold && !holdReason) || requestData.action != "changehold" && !holdNotes) {
                                    Xrm.Navigation.openErrorDialog({ message: "You need to populate mandatory fields" });
                                    return [2 /*return*/];
                                }
                                Xrm.Utility.showProgressIndicator("Processing...");
                                ProcessCase = function (caseId) { return __awaiter(_this, void 0, void 0, function () {
                                    var request, successCallback;
                                    return __generator(this, function (_a) {
                                        switch (_a.label) {
                                            case 0:
                                                request = {
                                                    caseId: caseId,
                                                    holdNote: holdNotes,
                                                    holdReason: holdReason,
                                                    isOnHold: isHold
                                                };
                                                successCallback = function (data) {
                                                    Xrm.Utility.closeProgressIndicator();
                                                    var storageKey = "holdCase-" + request.caseId;
                                                    localStorage.setItem(storageKey, "true");
                                                    if (data.Succeeded) {
                                                        Xrm.Navigation.openAlertDialog({ text: "Request was processed successfully" }).then(function () {
                                                            window.parent.close();
                                                        });
                                                    }
                                                    else {
                                                        Xrm.Navigation.openErrorDialog({ message: "Error while puting case on hold: " + data.Output }).then(function () {
                                                            window.parent.close();
                                                        });
                                                    }
                                                };
                                                return [4 /*yield*/, SendRequest(request, successCallback, function (message) {
                                                        Xrm.Utility.closeProgressIndicator();
                                                        Xrm.Navigation.openErrorDialog({ message: message }).always;
                                                    })];
                                            case 1:
                                                _a.sent();
                                                return [2 /*return*/];
                                        }
                                    });
                                }); };
                                if (!Array.isArray(requestData.caseId)) return [3 /*break*/, 5];
                                caseIds = requestData.caseId;
                                i = 0;
                                _a.label = 1;
                            case 1:
                                if (!(i < caseIds.length)) return [3 /*break*/, 4];
                                return [4 /*yield*/, ProcessCase(caseIds[i])];
                            case 2:
                                _a.sent();
                                _a.label = 3;
                            case 3:
                                i++;
                                return [3 /*break*/, 1];
                            case 4:
                                localStorage.setItem('RefreshHoldCaseRibbon', "true");
                                return [3 /*break*/, 7];
                            case 5: return [4 /*yield*/, ProcessCase(requestData.caseId)];
                            case 6:
                                _a.sent();
                                _a.label = 7;
                            case 7: return [2 /*return*/];
                        }
                    });
                });
            }
            HoldCase_1.HoldCase = HoldCase;
            function Cancel() {
                window.parent.close();
            }
            HoldCase_1.Cancel = Cancel;
            function getUrlParameter(sParam) {
                var sPageURL = window.location.search.substring(1), sURLVariables = sPageURL.split('&'), sParameterName, i;
                for (i = 0; i < sURLVariables.length; i++) {
                    sParameterName = sURLVariables[i].split('=');
                    if (sParameterName[0] === sParam) {
                        return sParameterName[1] === undefined ? "" : decodeURIComponent(sParameterName[1]);
                    }
                }
            }
            ;
            var CrmManager;
            (function (CrmManager) {
                function LoadOptionsByCaseStateCodeAsync(casestatecode, targetList) {
                    return __awaiter(this, void 0, void 0, function () {
                        var data, i;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_caseholdconfiguration', "?$select=ipg_caseholdreason&$filter=ipg_casestate eq " + casestatecode + " and statecode eq 0")];
                                case 1:
                                    data = _a.sent();
                                    for (i = 0; i < data.entities.length; i++) {
                                        targetList.push({
                                            text: data.entities[i]['ipg_caseholdreason@OData.Community.Display.V1.FormattedValue'],
                                            value: data.entities[i]['ipg_caseholdreason']
                                        });
                                    }
                                    ;
                                    return [2 /*return*/];
                            }
                        });
                    });
                }
                CrmManager.LoadOptionsByCaseStateCodeAsync = LoadOptionsByCaseStateCodeAsync;
            })(CrmManager || (CrmManager = {}));
        })(HoldCase = Incident.HoldCase || (Incident.HoldCase = {}));
    })(Incident = Intake.Incident || (Intake.Incident = {}));
})(Intake || (Intake = {}));
$(function () {
    Intake.Incident.HoldCase.init();
    $("#okBtn").on('click', Intake.Incident.HoldCase.HoldCase);
    $("#cancelBtn").on('click', Intake.Incident.HoldCase.Cancel);
});
