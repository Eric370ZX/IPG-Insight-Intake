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
    var Case;
    (function (Case) {
        var now = new Date();
        var startDateTime = new Date(now.getFullYear(), now.getMonth(), now.getDate(), now.getHours(), now.getSeconds() - 5).toISOString().split('.')[0] + "Z";
        var GateRuning = true;
        var Xrm = window.parent.Xrm;
        ;
        var data = JSON.parse(getUrlParameter("data"));
        var WFTasks = [];
        function RetrieveWFTask(gateConfigId) {
            return __awaiter(this, void 0, void 0, function () {
                var gateConfig, gatetask, gateconfigDetails;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.online.retrieveRecord("ipg_gateconfiguration", gateConfigId, "?$select=_ipg_documentsvalidationdetailid_value")];
                        case 1:
                            gateConfig = _a.sent();
                            if (gateConfig["_ipg_documentsvalidationdetailid_value"]) {
                                gatetask = {
                                    id: gateConfig["_ipg_documentsvalidationdetailid_value"],
                                    name: gateConfig["_ipg_documentsvalidationdetailid_value@OData.Community.Display.V1.FormattedValue"],
                                    status: "Started",
                                    order: -1
                                };
                                WFTasks.push(gatetask);
                            }
                            return [4 /*yield*/, Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateconfigurationdetail", "?$select=ipg_executionorder,ipg_name&$orderby=ipg_executionorder&$filter=_ipg_gateconfigurationid_value eq " + gateConfigId + " and statecode eq 0")];
                        case 2:
                            gateconfigDetails = _a.sent();
                            gateconfigDetails.entities.forEach(function (configdetails) {
                                WFTasks.push({ name: configdetails["ipg_name"], order: configdetails["ipg_executionorder"], id: configdetails["ipg_gateconfigurationdetailid"], status: "Started" });
                            });
                            return [2 /*return*/];
                    }
                });
            });
        }
        function GetLastLogDate() {
            return __awaiter(this, void 0, void 0, function () {
                var lastlog;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateexecutionlog", "?$select=createdon&$filter=_ipg_caseid_value eq " + data.caseId + "&$orderby=createdon desc&$top=1")];
                        case 1:
                            lastlog = _a.sent();
                            if (lastlog.entities && lastlog.entities.length > 0) {
                                startDateTime = lastlog.entities[0]["createdon"];
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        function RetrieveGateConfigFromCase(caseId) {
            return __awaiter(this, void 0, void 0, function () {
                var result;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.online.retrieveMultipleRecords("incident", "?$select=incidentid&$expand=ipg_lifecyclestepid($select=_ipg_gateconfigurationid_value)&$filter=incidentid eq " + caseId + " and ipg_lifecyclestepid/ipg_lifecyclestepid ne null")];
                        case 1:
                            result = _a.sent();
                            return [2 /*return*/, result.entities[0]["ipg_lifecyclestepid"]
                                    && result.entities[0]["ipg_lifecyclestepid"]["_ipg_gateconfigurationid_value"]];
                    }
                });
            });
        }
        function BuildTable() {
            var $tbody = $('#wftasktable > tbody');
            for (var i = WFTasks.length - 1; i > -1; i--) {
                $tbody.append("<tr " + (i != 0 ? "style='display:none'" : '') + " id=" + WFTasks[i].id + "><td>" + WFTasks[i].name + "</td><td>" + WFTasks[i].status + "</td></tr>");
            }
        }
        function RetrieveSessionId(caseId) {
            return __awaiter(this, void 0, void 0, function () {
                var incident;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.online.retrieveRecord("incident", caseId, "?$select=ipg_cr_gatesessionid")];
                        case 1:
                            incident = _a.sent();
                            return [2 /*return*/, incident["ipg_cr_gatesessionid_value"]];
                    }
                });
            });
        }
        function ProceedGating() {
            var parameters = {
                "Target": {
                    "incidentid": data.caseId,
                    "@odata.type": "Microsoft.Dynamics.CRM.incident"
                },
                "InitiatingUser": {
                    "systemuserid": data.InitiatingUser,
                    "@odata.type": "Microsoft.Dynamics.CRM.systemuser"
                },
                IsManual: true
            };
            var mainGatingCallback = function (results) {
                if (results.Output != null && results.Output != "") {
                    var alertStrings = { confirmButtonLabel: "OK", text: results.Output };
                    var alertOptions = { height: 150, width: 300 };
                    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function success(result) {
                    }, function (error) {
                        console.log(error.message);
                    });
                }
                else {
                    Xrm.WebApi.retrieveRecord("incident", data.caseId, "?$select=ipg_casestatus,_ipg_gateconfigurationid_value,ipg_statecode").then(function success(result) {
                        if (result["_ipg_gateconfigurationid_value@OData.Community.Display.V1.FormattedValue"].startsWith("Gate 11") && (result["ipg_casestatus"] != 923720001)) {
                            var alertStrings_1 = { confirmButtonLabel: "Ok", text: "Case was promoted to " + result["ipg_statecode@OData.Community.Display.V1.FormattedValue"], title: "Case state" };
                            Xrm.Navigation.openAlertDialog(alertStrings_1);
                        }
                    }, function (error) {
                        Xrm.Utility.alertDialog(error.message, null);
                    });
                }
            };
            callAction("ipg_IPGGatingStartGateProcessing", parameters, true, function (results) {
                return __awaiter(this, void 0, void 0, function () {
                    var warningSeverityLevel, confirmText, confirmStr, confirmReslt, parametersConfirm;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                GateRuning = false;
                                warningSeverityLevel = 923720001;
                                Xrm.Utility.closeProgressIndicator();
                                if (!results.AllowReject) return [3 /*break*/, 2];
                                confirmText = results.SeverityLevel === warningSeverityLevel
                                    ? "There is at least one business rule warning flagged. Would you like to promote the Case anyway?"
                                    : "Gating was completed with error. Do you want to continue with closing actions or cancel the process and do required actions";
                                confirmStr = { title: "Approval required", text: confirmText };
                                return [4 /*yield*/, Xrm.Navigation.openConfirmDialog(confirmStr, null)];
                            case 1:
                                confirmReslt = _a.sent();
                                if (confirmReslt.confirmed) {
                                    parametersConfirm = {
                                        "Target": {
                                            "incidentid": data.caseId,
                                            "@odata.type": "Microsoft.Dynamics.CRM.incident"
                                        }
                                    };
                                    Xrm.Utility.showProgressIndicator("Processing...");
                                    callAction("ipg_IPGGatingGateProcessingPostAction", parametersConfirm, true, function (resultsConfirm) {
                                        mainGatingCallback(resultsConfirm);
                                    });
                                }
                                return [3 /*break*/, 3];
                            case 2:
                                mainGatingCallback(results);
                                _a.label = 3;
                            case 3: return [2 /*return*/];
                        }
                    });
                });
            });
        }
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
        function MonitorTasks() {
            return __awaiter(this, void 0, void 0, function () {
                var task, log, row, nexttask, logs;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!GateRuning) return [3 /*break*/, 3];
                            task = WFTasks.find(function (r) { return r.status == "Started"; });
                            if (!task) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateexecutionlog", "?$select=ipg_succeeded&$top=1&$filter=_ipg_gateconfigurationdetailid_value eq " + task.id + " and _ipg_caseid_value eq " + data.caseId + " and createdon gt " + startDateTime)];
                        case 1:
                            log = _a.sent();
                            if (log.entities.length == 0) {
                                setTimeout(MonitorTasks, 500);
                            }
                            else {
                                task.status = "Completed (" + (log.entities[0]["ipg_succeeded"] ? "S" : "E") + ")";
                                row = $("#" + task.id);
                                $(row.children()[1]).text(task.status);
                                nexttask = WFTasks.find(function (r) { return r.status == "Started"; });
                                if (nexttask) {
                                    $("#" + nexttask.id).css("display", "table-row");
                                }
                                MonitorTasks();
                            }
                            _a.label = 2;
                        case 2: return [3 /*break*/, 5];
                        case 3: return [4 /*yield*/, Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateexecutionlog", "?$select=ipg_succeeded,_ipg_gateconfigurationdetailid_value&$filter=_ipg_caseid_value eq " + data.caseId + " and createdon gt " + startDateTime)];
                        case 4:
                            logs = _a.sent();
                            WFTasks.filter(function (r) { return r.status == "Started"; }).map(function (r) {
                                var log = logs.entities.find(function (l) { return l["_ipg_gateconfigurationdetailid_value"] == r.id; });
                                if (log) {
                                    var row = $("#" + r.id);
                                    r.status = "Completed (" + (log["ipg_succeeded"] ? "S" : "E") + ")";
                                    $(row.children()[1]).text(r.status);
                                    row.css("display", "table-row");
                                }
                            });
                            _a.label = 5;
                        case 5: return [2 /*return*/];
                    }
                });
            });
        }
        function Onload() {
            return __awaiter(this, void 0, void 0, function () {
                var GateConfigId;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, RetrieveGateConfigFromCase(data.caseId)];
                        case 1:
                            GateConfigId = _a.sent();
                            return [4 /*yield*/, RetrieveWFTask(GateConfigId)];
                        case 2:
                            _a.sent();
                            return [4 /*yield*/, GetLastLogDate()];
                        case 3:
                            _a.sent();
                            BuildTable();
                            setTimeout(MonitorTasks);
                            setTimeout(ProceedGating);
                            return [2 /*return*/];
                    }
                });
            });
        }
        Case.Onload = Onload;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
$(function () { return Intake.Case.Onload(); });
