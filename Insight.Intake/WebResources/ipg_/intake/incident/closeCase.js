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
        var CloseCase;
        (function (CloseCase_1) {
            var caseState;
            var closeReasons = [];
            var closeCategories = [];
            var caseCloseMappings = [];
            var incidentData = { ipg_statecode: -1 };
            function init() {
                window.parent.Xrm.Utility.showProgressIndicator("Opening...");
                var initLoader = [
                    loadOptions("ipg_caseclosurereason", closeReasons),
                    loadOptions("ipg_caseclosurecategory", closeCategories),
                    loadCaseClosureMapping(),
                    loadCaseData(),
                    checkIfCaseCanbeClosed(getUrlParameter("data"))
                ];
                Promise.all(initLoader).then(function (value) {
                    var displayMessage = "In order to close the case, ensure that there are no Actual Parts on the case, Revenue has been removed,\n                              there are no POs, no outstanding Claims, no payments received and all AR balances are at zero.";
                    var msgs = value.filter(function (p) { return typeof p === "string" && !isEmptyOrSpaces(p); }).map(function (p) { return p + ";\n"; });
                    if (msgs && msgs.length > 0) {
                        displayMessage += "\nWarning messages:\n" + msgs;
                        alertDialog(displayMessage, "OK", function () {
                            filterClosureReasons();
                            window.parent.Xrm.Utility.closeProgressIndicator();
                        }, null);
                    }
                    filterClosureReasons();
                    window.parent.Xrm.Utility.closeProgressIndicator();
                }, function (reason) {
                    alertDialog("" + reason, "ERROR", function () {
                        window.parent.close();
                    }, null);
                });
                $("#closeCategory").change(filterCloseReason);
            }
            CloseCase_1.init = init;
            function ProcessCase() {
                var closeReason = $("#closeReason").val();
                var closeReasonDescr = $("#closeReason option:selected").text();
                var closeCaseConfig = caseCloseMappings
                    .find(function (p) { return p.CaseState.value == caseState.value && p.ClosureReason.value == closeReason; });
                var closedBy = closeCaseConfig.ClosedBy;
                var facilityСommunication = closeCaseConfig.FacilityCommunication;
                var eventLogEntry = closeCaseConfig.EventLogEntry;
                var closeNotes = $("#caseNote").val();
                var caseId = getUrlParameter("data");
                var onSuccess = function () {
                    window.parent.Xrm.Utility.closeProgressIndicator();
                    var storageKey = "closeCase-" + caseId;
                    localStorage.setItem(storageKey, "true");
                    alertDialog("Case closed succesfully", "OK", function () {
                        window.parent.close();
                    }, null);
                };
                window.parent.Xrm.Utility.showProgressIndicator("Closing the case...");
                closeCaseRequest(caseId, closeReason, closeReasonDescr, closeNotes, closedBy, facilityСommunication, eventLogEntry, onSuccess, function (message) {
                    window.parent.Xrm.Utility.closeProgressIndicator();
                    alertDialog("Error while closing case:" + message, "ERROR", null, null);
                });
            }
            CloseCase_1.ProcessCase = ProcessCase;
            function CloseCase() {
                window.parent.close();
            }
            CloseCase_1.CloseCase = CloseCase;
            function checkIfCaseCanbeClosed(caseid) {
                return new Promise(function (resolve, reject) {
                    var target = {
                        incidentid: caseid
                    };
                    target["@odata.type"] = "Microsoft.Dynamics.CRM.incident";
                    var parameters = {
                        Target: target,
                    };
                    var ipg_IPGCaseCloseCaseRequest = {
                        Target: parameters.Target,
                        getMetadata: function () {
                            return {
                                boundParameter: null,
                                parameterTypes: {
                                    "Target": {
                                        "typeName": "mscrm.incident",
                                        "structuralProperty": 5
                                    }
                                },
                                operationType: 0,
                                operationName: "ipg_IPGCaseCheckIfCaseCanBeClosed"
                            };
                        }
                    };
                    window.parent.Xrm.WebApi.online.execute(ipg_IPGCaseCloseCaseRequest).then(function success(result) {
                        return __awaiter(this, void 0, void 0, function () {
                            var response;
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0:
                                        if (!result.ok) return [3 /*break*/, 2];
                                        return [4 /*yield*/, result.json()];
                                    case 1:
                                        response = _a.sent();
                                        if (response.Succeeded) {
                                            if (response.SeverityLevel == 923720000) { //info
                                                resolve("");
                                            }
                                            else if (response.SeverityLevel == 923720001) { //Warning 
                                                resolve("Additional Activities are needed: \n" + response.Output);
                                            }
                                        }
                                        else {
                                            reject("Case can not be closed. " + response.Output);
                                        }
                                        _a.label = 2;
                                    case 2: return [2 /*return*/];
                                }
                            });
                        });
                    }, function (error) {
                        reject(error.message);
                    });
                });
            }
            function filterCloseReason() {
                var closeCategory = $("#closeCategory").val();
                //const closuresReasons
                var filteredData = caseCloseMappings
                    .filter(function (p) { return p.CaseState != null && p.CaseState.value == caseState.value && p.ClosureCategory.value == closeCategory; });
                populateOptionSet("closeReason", filteredData.map(function (p) { return p.ClosureReason; }).filter(function (value, index, self) { return self.indexOf(value) === index; }));
            }
            function closeCaseRequest(caseid, closeReason, closeReasonDescr, closeNote, closedBy, facilityCommunication, eventLogEntry, onSuccess, onError) {
                var target = {
                    incidentid: caseid
                };
                target["@odata.type"] = "Microsoft.Dynamics.CRM.incident";
                var parameters = {
                    Target: target,
                    CloseReason: closeReason,
                    CloseNotes: closeNote,
                    CloseReasonDescr: closeReasonDescr,
                    ClosedBy: closedBy,
                    FacilityCommunication: facilityCommunication,
                    EventLogEntry: eventLogEntry
                };
                var ipg_IPGCaseCloseCaseRequest = {
                    Target: parameters.Target,
                    CloseReason: parameters.CloseReason,
                    CloseNotes: parameters.CloseNotes,
                    CloseReasonDescr: parameters.CloseReasonDescr,
                    ClosedBy: parameters.ClosedBy,
                    FacilityCommunication: parameters.FacilityCommunication,
                    EventLogEntry: parameters.EventLogEntry,
                    getMetadata: function () {
                        return {
                            boundParameter: null,
                            parameterTypes: {
                                "Target": {
                                    "typeName": "mscrm.incident",
                                    "structuralProperty": 5
                                },
                                "CloseReason": {
                                    "typeName": "Edm.Int32",
                                    "structuralProperty": 1
                                },
                                "CloseReasonDescr": {
                                    "typeName": "Edm.String",
                                    "structuralProperty": 1
                                },
                                "CloseNotes": {
                                    "typeName": "Edm.String",
                                    "structuralProperty": 1
                                },
                                "ClosedBy": {
                                    "typeName": "Edm.String",
                                    "structuralProperty": 1
                                },
                                "FacilityCommunication": {
                                    "typeName": "Edm.String",
                                    "structuralProperty": 1
                                },
                                "EventLogEntry": {
                                    "typeName": "Edm.String",
                                    "structuralProperty": 1
                                }
                            },
                            operationType: 0,
                            operationName: "ipg_IPGCaseCloseCase"
                        };
                    }
                };
                window.parent.Xrm.WebApi.online.execute(ipg_IPGCaseCloseCaseRequest).then(function success(result) {
                    if (result.ok) {
                        onSuccess();
                        //Success - No Return Data - Do Something
                    }
                }, function (error) {
                    onError(error.message);
                });
            }
            function filterClosureReasons() {
                caseState = caseCloseMappings
                    .filter(function (p) { return (p.CaseState != null && p.CaseState.value == incidentData.ipg_statecode); }).map(function (x) { return x.CaseState; }).filter(function (caseState, index, self) {
                    return index === self.findIndex(function (t) { return (t.text === caseState.text && t.value === caseState.value); });
                })[0];
                var filteredData = caseCloseMappings
                    .filter(function (p) { return (p.CaseState != null && p.CaseState.value == caseState.value); });
                var uniqueCloseCategoryNumbersSet = new Array();
                var uniqueCloseCategorySet = new Array();
                var allCloseCategrories = filteredData
                    .filter(function (p) { return (p.ClosureCategory.value != null && p.ClosureCategory.text != null); }).map(function (p) { return p.ClosureCategory; });
                if (allCloseCategrories == null || allCloseCategrories.length <= 0) {
                    alertDialog("No close category has been found.", "ERROR", function () { window.parent.close(); }, null);
                }
                for (var i = 0; i < allCloseCategrories.length; i++) {
                    if (uniqueCloseCategoryNumbersSet.indexOf(allCloseCategrories[i].value) == -1) {
                        uniqueCloseCategoryNumbersSet.push(allCloseCategrories[i].value);
                        uniqueCloseCategorySet.push(allCloseCategrories[i]);
                    }
                }
                populateOptionSet("closeCategory", uniqueCloseCategorySet);
                populateOptionSet("closeReason", filteredData
                    .map(function (p) { return p.ClosureReason; })
                    .filter(function (value, index, self) { return self.indexOf(value) === index; })); //getting unique values
                $("#closeCategory").change();
            }
            function populateOptionSet(picklistName, data) {
                $("#" + picklistName + " option").remove();
                data.forEach(function (p) { return $("#" + picklistName).prepend("<option value=\"" + p.value + "\">" + p.text + "</option>"); });
                $("#" + picklistName + " option:first-child").prop("selected", "selected");
            }
            function loadCaseData() {
                return new Promise(function (resolve, reject) {
                    var caseId = getUrlParameter("data");
                    window.parent.Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=ipg_statecode")
                        .then(function (caseTarget) {
                        incidentData.ipg_statecode = caseTarget.ipg_statecode;
                        resolve();
                    });
                });
            }
            function loadCaseClosureMapping() {
                return new Promise(function (resolve, reject) {
                    window.parent.Xrm.WebApi.online.retrieveMultipleRecords("ipg_casecloseconfiguration", "?$select=ipg_name,ipg_casecloseconfigurationid,ipg_caseclosurereason,ipg_caseclosuretype,ipg_caseclosurecategory,_ipg_casedisplaystatusid_value,ipg_casestate,ipg_closedby,ipg_facilitycommunication&$expand=ipg_casedisplaystatusid($select=ipg_name)&$filter=statecode eq 0")
                        .then(function (results) {
                        for (var i = 0; i < results.entities.length; i++) {
                            caseCloseMappings.push({
                                CaseState: {
                                    value: results.entities[i]["ipg_casestate"],
                                    text: results.entities[i]["ipg_casestate@OData.Community.Display.V1.FormattedValue"]
                                },
                                ClosureReason: {
                                    value: results.entities[i]["ipg_caseclosurereason"],
                                    text: results.entities[i]["ipg_caseclosurereason@OData.Community.Display.V1.FormattedValue"]
                                },
                                ClosureCategory: {
                                    value: results.entities[i]["ipg_caseclosurecategory"],
                                    text: results.entities[i]["ipg_caseclosurecategory@OData.Community.Display.V1.FormattedValue"]
                                },
                                EventLogEntry: results.entities[i]["ipg_name"],
                                ClosedBy: results.entities[i]["ipg_closedby"],
                                FacilityCommunication: results.entities[i]["ipg_facilitycommunication"],
                            });
                        }
                        resolve();
                    }, function (error) {
                        alertDialog(error.message, "ERROR", function () { reject(); }, null);
                    });
                });
            }
            function loadOptions(pickListName, targetList) {
                return new Promise(function (resolve, reject) {
                    $.get("/api/data/v9.0/GlobalOptionSetDefinitions(Name='" + pickListName + "')", null, function (data) {
                        for (var i = 0; i < data.Options.length; i++) {
                            targetList.push({
                                text: data.Options[i].Label.UserLocalizedLabel.Label,
                                value: data.Options[i].Value
                            });
                        }
                        ;
                        resolve();
                    }, "json");
                });
            }
            function getUrlParameter(sParam) {
                var sPageURL = window.location.search.substring(1), sURLVariables = sPageURL.split('&'), sParameterName, i;
                for (i = 0; i < sURLVariables.length; i++) {
                    sParameterName = sURLVariables[i].split('=');
                    if (sParameterName[0] === sParam) {
                        return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
                    }
                }
            }
            ;
            function alertDialog(alertText, alertTitle, onSuccess, onError) {
                var alertStrings = { confirmButtonLabel: "OK", text: alertText, title: alertTitle };
                var alertOptions = { height: 120, width: 260 };
                window.parent.Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                    onSuccess();
                }, function (error) {
                    onError();
                });
            }
            function isEmptyOrSpaces(str) {
                return str === null || str.match(/^ *$/) !== null;
            }
        })(CloseCase = Incident.CloseCase || (Incident.CloseCase = {}));
    })(Incident = Intake.Incident || (Intake.Incident = {}));
})(Intake || (Intake = {}));
$(document).ready(function () {
    Intake.Incident.CloseCase.init();
    $("#okBtn").on("click", Intake.Incident.CloseCase.ProcessCase);
    $("#cancelBtn").on("click", Intake.Incident.CloseCase.CloseCase);
});
