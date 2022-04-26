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
 * @namespace Intake.Incident.Ribbon
 */
var Intake;
(function (Intake) {
    var Incident;
    (function (Incident) {
        var Ribbon;
        (function (Ribbon) {
            var CaseStates;
            (function (CaseStates) {
                CaseStates[CaseStates["Intake"] = 923720000] = "Intake";
                CaseStates[CaseStates["Authorizations"] = 923720001] = "Authorizations";
                CaseStates[CaseStates["CaseManagement"] = 923720002] = "CaseManagement";
                CaseStates[CaseStates["Billing"] = 923720003] = "Billing";
                CaseStates[CaseStates["CarrierCollections"] = 923720004] = "CarrierCollections";
                CaseStates[CaseStates["PatientCollections"] = 923720005] = "PatientCollections";
                CaseStates[CaseStates["Finance"] = 923720006] = "Finance";
            })(CaseStates || (CaseStates = {}));
            ;
            var caseManager = function (form) {
                var caseHoldField = form.getAttribute("ipg_casehold");
                var caseStateField = form.getAttribute("statecode");
                var setCaseHold = function (value) {
                    caseHoldField.setValue(value);
                    caseHoldField.setSubmitMode("always");
                    form.data.save().then(function () {
                        if (!value) {
                            form.ui.clearFormNotification("casehold");
                        }
                        form.ui.refreshRibbon();
                    }, function (error) {
                        caseHoldField.setValue(caseHoldField.getInitialValue());
                        form.ui.refreshRibbon();
                        Xrm.Utility.alertDialog("Error while saving the case: " + error.message, null);
                    });
                };
                var isOnHold = function () { return caseHoldField && caseHoldField.getValue() === true; };
                var isOpen = function () { return caseStateField && caseStateField.getValue() === 0; };
                var enableCaseHold = function () { return isOpen() && !isOnHold(); };
                var enableRemoveHold = function () { return isOpen() && isOnHold(); };
                var caseHold = function () {
                    if (!caseHoldField) {
                        return;
                    }
                    setCaseHold(true);
                };
                var removeHold = function () {
                    if (!caseHoldField) {
                        return;
                    }
                    setCaseHold(false);
                };
                return {
                    EnableCaseHold: enableCaseHold,
                    EnableRemoveHold: enableRemoveHold,
                    CaseHold: caseHold,
                    RemoveHold: removeHold,
                };
            };
            function caseHold(formContext, action) {
                formContext.data.refresh(true).then(function (p) {
                    var data = { caseId: formContext.data.entity.getId(), action: action };
                    var storageKey = "holdCase-" + data.caseId;
                    localStorage.setItem(storageKey, "false");
                    window.addEventListener("storage", function (ev) {
                        if (ev.key === storageKey && ev.newValue === "true") {
                            formContext.data.refresh(false).then(function (p) {
                                formContext.ui.refreshRibbon();
                                var caseHoldField = formContext.getAttribute("ipg_casehold");
                                caseHoldField.fireOnChange();
                                if (!caseHoldField.getValue()) {
                                    formContext.ui.clearFormNotification("casehold");
                                }
                            });
                        }
                    });
                    Xrm.Navigation.openWebResource("ipg_/intake/incident/holdCase.html", null, JSON.stringify(data));
                });
            }
            function CaseHold(formContext) {
                caseHold(formContext, "hold");
            }
            Ribbon.CaseHold = CaseHold;
            function RemoveHold(formContext) {
                caseHold(formContext, "unhold");
            }
            Ribbon.RemoveHold = RemoveHold;
            function ChangeHoldReason(formContext) {
                caseHold(formContext, "changehold");
            }
            Ribbon.ChangeHoldReason = ChangeHoldReason;
            function EnableCaseHold(formContext) {
                return caseManager(formContext).EnableCaseHold();
            }
            Ribbon.EnableCaseHold = EnableCaseHold;
            function EnableRemoveHold(formContext) {
                return caseManager(formContext).EnableRemoveHold();
            }
            Ribbon.EnableRemoveHold = EnableRemoveHold;
            function CloseCase(formContext) {
                formContext.data.refresh(true).then(function (p) {
                    var data = formContext.data.entity.getId();
                    var storageKey = "closeCase-" + data;
                    localStorage.setItem(storageKey, "false");
                    window.addEventListener("storage", function (ev) {
                        if (ev.key === storageKey && ev.newValue === "true") {
                            formContext.data.refresh(false);
                        }
                    });
                    var windowsOption = { openInNewWindow: false, height: 400, width: 400 };
                    Xrm.Navigation.openWebResource("ipg_/intake/incident/CloseCase.html", null, data);
                });
            }
            Ribbon.CloseCase = CloseCase;
            function CloseCaseValidation(formContext) {
                var _this = this;
                return new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var ipg_casestatus, isClosed, isCasemanagementTeam;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                ipg_casestatus = formContext.getAttribute("ipg_casestatus");
                                isClosed = ipg_casestatus.getValue() === 923720001;
                                if (isClosed) {
                                    resolve(false);
                                    return [2 /*return*/];
                                }
                                return [4 /*yield*/, IsTeamMember("Case Management")];
                            case 1:
                                isCasemanagementTeam = _a.sent();
                                resolve(isCasemanagementTeam);
                                return [2 /*return*/];
                        }
                    });
                }); });
            }
            Ribbon.CloseCaseValidation = CloseCaseValidation;
            function IsTeamMember(teamName) {
                var _this = this;
                return new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var userTeams, isTeamMember, ex_1;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                _a.trys.push([0, 2, , 3]);
                                return [4 /*yield*/, GetuserTeams()];
                            case 1:
                                userTeams = _a.sent();
                                isTeamMember = userTeams.filter(function (p) { return p == teamName; }).length > 0;
                                resolve(isTeamMember);
                                return [3 /*break*/, 3];
                            case 2:
                                ex_1 = _a.sent();
                                reject(ex_1.message);
                                return [3 /*break*/, 3];
                            case 3: return [2 /*return*/];
                        }
                    });
                }); });
            }
            function GetuserTeams() {
                return __awaiter(this, void 0, void 0, function () {
                    var fetchXml, teams;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\">\n              <entity name=\"team\">\n                <attribute name=\"name\" />\n                <order attribute=\"name\" descending=\"false\" />\n                <link-entity name=\"teammembership\" from=\"teamid\" to=\"teamid\" visible=\"false\" intersect=\"true\">\n                  <link-entity name=\"systemuser\" from=\"systemuserid\" to=\"systemuserid\" alias=\"ai\">\n                    <filter type=\"and\">\n                      <condition attribute=\"systemuserid\" operator=\"eq-userid\" />\n                    </filter>\n                  </link-entity>\n                </link-entity>\n              </entity>\n            </fetch>";
                                return [4 /*yield*/, Xrm.WebApi.online.retrieveMultipleRecords("team", "?fetchXml=" + fetchXml)];
                            case 1:
                                teams = _a.sent();
                                return [2 /*return*/, teams.entities.map(function (p) { return p.name; })];
                        }
                    });
                });
            }
            function BVF(form) {
                var formParams = {
                    "ipg_parentcaseid": form.data.entity.getId(),
                    "ipg_parentcaseidname": form.data.entity.getPrimaryAttributeValue(),
                    "ipg_parentcaseidtype": form.data.entity.getEntityName(),
                };
                Xrm.Utility.openEntityForm("ipg_benefitsverificationform", null, formParams);
            }
            Ribbon.BVF = BVF;
            function BVFValidation(form) {
                var result = true;
                if (!form.data.entity.getId())
                    result = false;
                return result;
            }
            Ribbon.BVFValidation = BVFValidation;
            /**
             * Called on Submit button click
             * @function Intake.Incident.Ribbon.OnSubmitClick
             * @returns {void}
            */
            function OnSubmitClick(primaryControl) {
                var _a;
                var formContext = primaryControl;
                var caseStatus = (_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue();
                if (caseStatus && caseStatus == 923720001) {
                    Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "You are not able to submit closed case." }).then(function () {
                        return;
                    });
                }
                else {
                    Xrm.Utility.showProgressIndicator("Processing...");
                    formContext.data.save().then(function () {
                        return __awaiter(this, void 0, void 0, function () {
                            var formData, _a;
                            return __generator(this, function (_b) {
                                switch (_b.label) {
                                    case 0:
                                        formData = { Id: formContext.data.entity.getId(), EntityName: formContext.data.entity.getEntityName(), UserId: Xrm.Utility.getGlobalContext().userSettings.userId };
                                        return [4 /*yield*/, ConfirmSubmit(formData.Id)];
                                    case 1:
                                        _a = (_b.sent());
                                        if (!_a) return [3 /*break*/, 3];
                                        return [4 /*yield*/, ConfirmSubmitGate11(formData.Id)];
                                    case 2:
                                        _a = (_b.sent());
                                        _b.label = 3;
                                    case 3:
                                        if (_a) {
                                            ProceedGating(formData, formContext);
                                            //MVP real time processing gating
                                            //ProceedGatingWithIncrementalView(formData, formContext);
                                        }
                                        else {
                                            Xrm.Utility.closeProgressIndicator();
                                        }
                                        return [2 /*return*/];
                                }
                            });
                        });
                    }, function (error) {
                        Xrm.Utility.closeProgressIndicator();
                        console.log(error.message);
                    });
                }
            }
            Ribbon.OnSubmitClick = OnSubmitClick;
            function ConfirmSubmit(caseId) {
                return __awaiter(this, void 0, void 0, function () {
                    var fetchXml, result, _a;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                fetchXml = "<fetch top=\"1\" >\n  <entity name=\"ipg_gateprocessingrule\" >\n    <attribute name=\"ipg_gateprocessingruleid\" />\n    <filter>\n      <condition entityname=\"nexLfStep\" attribute=\"ipg_gateconfigurationidname\" operator=\"eq\" value=\"Gate 9\" />\n      <condition entityname=\"LfStep\" attribute=\"ipg_gateconfigurationidname\" operator=\"eq\" value=\"Gate 8\" />\n    </filter>\n    <link-entity name=\"ipg_lifecyclestep\" from=\"ipg_lifecyclestepid\" to=\"ipg_lifecyclestepid\" link-type=\"inner\" alias=\"LfStep\" >\n      <link-entity name=\"incident\" from=\"ipg_lifecyclestepid\" to=\"ipg_lifecyclestepid\" >\n        <filter>\n          <condition attribute=\"incidentid\" operator=\"eq\" value=\"" + caseId + "\"  />\n        </filter>\n      </link-entity>\n    </link-entity>\n    <link-entity name=\"ipg_lifecyclestep\" from=\"ipg_lifecyclestepid\" to=\"ipg_nextlifecyclestepid\" link-type=\"inner\" alias=\"nexLfStep\" />\n  </entity>\n</fetch>";
                                return [4 /*yield*/, Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateprocessingrule", "?fetchXml=" + fetchXml)];
                            case 1:
                                result = _b.sent();
                                if (!(result.entities && result.entities.length > 0)) return [3 /*break*/, 4];
                                return [4 /*yield*/, Xrm.Navigation.openConfirmDialog({
                                        title: "Confirmation",
                                        text: "Please note that by clicking OK you are promoting this case to Billing.  If this in error, click ‘Cancel’ to return to the case."
                                    })];
                            case 2: return [4 /*yield*/, (_b.sent()).confirmed];
                            case 3:
                                _a = _b.sent();
                                return [3 /*break*/, 5];
                            case 4:
                                _a = true;
                                _b.label = 5;
                            case 5: 
                            //if case about go to Billing - show confirm dialog
                            return [2 /*return*/, _a];
                        }
                    });
                });
            }
            function ConfirmSubmitGate11(caseId) {
                return __awaiter(this, void 0, void 0, function () {
                    var result, _a;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=_ipg_lifecyclestepid_value")];
                            case 1:
                                result = _b.sent();
                                if (!(result["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"] && result["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"].startsWith("Claim Remittance"))) return [3 /*break*/, 4];
                                return [4 /*yield*/, Xrm.Navigation.openConfirmDialog({
                                        title: "Confirmation",
                                        text: "Are you sure you want to promote the Case?"
                                    })];
                            case 2: return [4 /*yield*/, (_b.sent()).confirmed];
                            case 3:
                                _a = _b.sent();
                                return [3 /*break*/, 5];
                            case 4:
                                _a = true;
                                _b.label = 5;
                            case 5: return [2 /*return*/, _a];
                        }
                    });
                });
            }
            function ProceedGatingWithIncrementalView(formData, formContext) {
                var data = {
                    caseId: formData.Id,
                    InitiatingUser: formData.UserId
                };
                var pageInput = {
                    pageType: "webresource",
                    webresourceName: "ipg_/intake/case/GateExecutionStatus.html",
                    data: JSON.stringify(data)
                };
                var navigationOptions = {
                    target: 2,
                    width: 650,
                    height: 400,
                    position: 1,
                };
                var refreshForm = function () {
                    formContext.data.refresh(false);
                    formContext.ui.refreshRibbon();
                    Xrm.Utility.closeProgressIndicator();
                };
                Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(refreshForm, refreshForm);
            }
            function ProceedGating(formData, formContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var gatingVersion, parameters, mainGatingCallback;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", "?$select=ipg_value&$filter=ipg_name eq 'GatingVersion'")];
                            case 1:
                                gatingVersion = _a.sent();
                                parameters = {
                                    "Target": {
                                        "incidentid": formData.Id,
                                        "@odata.type": "Microsoft.Dynamics.CRM." + formData.EntityName
                                    },
                                    "InitiatingUser": {
                                        "systemuserid": formData.UserId,
                                        "@odata.type": "Microsoft.Dynamics.CRM.systemuser"
                                    },
                                    IsManual: true
                                };
                                mainGatingCallback = function (results) {
                                    if (results.Output != null && results.Output != "") {
                                        var alertStrings = { confirmButtonLabel: "OK", text: results.Output };
                                        Xrm.Navigation.openAlertDialog(alertStrings, { height: 400, width: 600 }).then(function success(result) {
                                        }, function (error) {
                                            console.log(error.message);
                                        });
                                    }
                                    else {
                                        Xrm.WebApi.retrieveRecord("incident", formData.Id, "?$select=ipg_casestatus,_ipg_gateconfigurationid_value,ipg_statecode").then(function success(result) {
                                            if (result["_ipg_gateconfigurationid_value@OData.Community.Display.V1.FormattedValue"].startsWith("Gate 11") && (result["ipg_casestatus"] != 923720001)) {
                                                var alertStrings_1 = { confirmButtonLabel: "Ok", text: "Case was promoted to " + result["ipg_statecode@OData.Community.Display.V1.FormattedValue"], title: "Case state" };
                                                Xrm.Navigation.openAlertDialog(alertStrings_1);
                                            }
                                            if (result && result["_ipg_gateconfigurationid_value"]) {
                                                Intake.Case.showTabsByGate(formContext, { entityType: "ipg_gateconfiguration", id: result["_ipg_gateconfigurationid_value"], name: "" });
                                            }
                                        }, function (error) {
                                            Xrm.Utility.alertDialog(error.message, null);
                                        });
                                    }
                                };
                                callAction("ipg_IPGGatingStartGateProcessing", parameters, true, function (results) {
                                    var _this = this;
                                    formContext.data.refresh(true).then(function () { return __awaiter(_this, void 0, void 0, function () {
                                        var warningSeverityLevel, alertStrings, _a, confirmStr, confirmResult, alertStr, nextLifeCycleStep, confirmText, confirmStr, confirmReslt;
                                        return __generator(this, function (_b) {
                                            switch (_b.label) {
                                                case 0:
                                                    warningSeverityLevel = 923720001;
                                                    Xrm.Utility.closeProgressIndicator();
                                                    if (!(gatingVersion.entities[0].ipg_value == "2")) return [3 /*break*/, 9];
                                                    if (!results.SeverityLevel) {
                                                        alertStrings = { confirmButtonLabel: "OK", text: results.Output };
                                                        Xrm.Navigation.openAlertDialog(alertStrings, { height: 400, width: 600 }).then(function success(result) {
                                                        }, function (error) {
                                                            console.log(error.message);
                                                        });
                                                    }
                                                    _a = results.SeverityLevel;
                                                    switch (_a) {
                                                        case 923720003: return [3 /*break*/, 1];
                                                        case 923720002: return [3 /*break*/, 3];
                                                        case 923720001: return [3 /*break*/, 4];
                                                        case 923720000: return [3 /*break*/, 6];
                                                    }
                                                    return [3 /*break*/, 8];
                                                case 1:
                                                    confirmStr = { title: "Critical", text: "At least one critical issue found. Do you wish to proceed with closing this Case?", confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
                                                    return [4 /*yield*/, Xrm.Navigation.openConfirmDialog(confirmStr, { height: 400, width: 600 })];
                                                case 2:
                                                    confirmResult = _b.sent();
                                                    if (confirmResult.confirmed) {
                                                        gatePostAction(formContext, formData, mainGatingCallback);
                                                    }
                                                    return [3 /*break*/, 8];
                                                case 3:
                                                    alertStr = { title: "Error", text: "The following errors were found. Case cannot proceed to the next step until these errors are corrected and Case is reprocessed through the 'Submit' action. \n" + results.Output, confirmButtonLabel: "Close" };
                                                    Xrm.Navigation.openAlertDialog(alertStr, { height: 400, width: 600 }).then(function () {
                                                        gatePostAction(formContext, formData, mainGatingCallback);
                                                    });
                                                    return [3 /*break*/, 8];
                                                case 4: return [4 /*yield*/, getNextLifeCycleStep(formContext)];
                                                case 5:
                                                    nextLifeCycleStep = _b.sent();
                                                    alertStr = { title: "Warning", text: "The following warnings were found. Case promoted to " + nextLifeCycleStep + " step. \n" + results.Output, confirmButtonLabel: "Close" };
                                                    Xrm.Navigation.openAlertDialog(alertStr, { height: 400, width: 600 }).then(function () {
                                                        gatePostAction(formContext, formData, mainGatingCallback);
                                                    });
                                                    return [3 /*break*/, 8];
                                                case 6: return [4 /*yield*/, getNextLifeCycleStep(formContext)];
                                                case 7:
                                                    nextLifeCycleStep = _b.sent();
                                                    alertStr = { title: "Success", text: "No issues found. Case promoted to " + nextLifeCycleStep, confirmButtonLabel: "Close" };
                                                    Xrm.Navigation.openAlertDialog(alertStr, { height: 400, width: 600 }).then(function () {
                                                        mainGatingCallback({});
                                                    });
                                                    return [3 /*break*/, 8];
                                                case 8: return [3 /*break*/, 12];
                                                case 9:
                                                    if (!results.AllowReject) return [3 /*break*/, 11];
                                                    confirmText = results.SeverityLevel === warningSeverityLevel
                                                        ? "There is at least one business rule warning flagged. Would you like to promote the Case anyway?"
                                                        : "Gating was completed with error. Do you want to continue with closing actions or cancel the process and do required actions";
                                                    confirmStr = { title: "Approval required", text: confirmText };
                                                    return [4 /*yield*/, Xrm.Navigation.openConfirmDialog(confirmStr, null)];
                                                case 10:
                                                    confirmReslt = _b.sent();
                                                    if (confirmReslt.confirmed) {
                                                        gatePostAction(formContext, formData, mainGatingCallback);
                                                    }
                                                    return [3 /*break*/, 12];
                                                case 11:
                                                    mainGatingCallback(results);
                                                    _b.label = 12;
                                                case 12: return [2 /*return*/];
                                            }
                                        });
                                    }); });
                                });
                                return [2 /*return*/];
                        }
                    });
                });
            }
            function OpenAssignDialog(context, selectedRecordsIds) {
                var primatyContext = typeof (context === null || context === void 0 ? void 0 : context.getFormContext) == "function"
                    ? context.getFormContext()
                    : context;
                var data = { selectedRecordsIds: selectedRecordsIds };
                var pageInput = {
                    pageType: "webresource",
                    webresourceName: "ipg_/intake/case/assignCaseModal.html",
                    data: JSON.stringify(data)
                };
                var navigationOptions = {
                    target: 2,
                    width: 550,
                    height: 350,
                    position: 1,
                };
                Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(function (success) { var _a, _b; return ((_b = (_a = primatyContext) === null || _a === void 0 ? void 0 : _a.data) !== null && _b !== void 0 ? _b : primatyContext).refresh(); }, function (error) { var _a, _b; return ((_b = (_a = primatyContext) === null || _a === void 0 ? void 0 : _a.data) !== null && _b !== void 0 ? _b : primatyContext).refresh(); });
            }
            Ribbon.OpenAssignDialog = OpenAssignDialog;
            /**
              * call Custom action
              * @function Intake.Incident.Ribbon.callAction
              * @returns {void}
            */
            function callAction(actionName, parameters, async, successCallback) {
                var req = new XMLHttpRequest();
                req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + actionName, async);
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
             * enable rule for Generate PO
             * @function Intake.Incident.Ribbon.hasActualParts
             * @returns {boolean}
             */
            function hasActualParts(primaryControl, caseId) {
                return __awaiter(this, void 0, void 0, function () {
                    var res;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_casepartdetail', "?$filter=_ipg_caseid_value eq " + caseId.replace("{", "").replace("}", "").toLowerCase())];
                            case 1:
                                res = _a.sent();
                                if (res.entities.length > 0)
                                    return [2 /*return*/, true];
                                return [2 /*return*/, false];
                        }
                    });
                });
            }
            Ribbon.hasActualParts = hasActualParts;
            /**
            * enable rule for Generate PO
            * @function Intake.Incident.Ribbon.caseIsOpened
            * @returns {boolean}
            */
            function caseIsOpened(primaryControl, caseId) {
                return __awaiter(this, void 0, void 0, function () {
                    var caseStatus;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, (Xrm.WebApi.retrieveRecord('incident', "" + caseId.replace("{", "").replace("}", "").toLowerCase(), '?$select=ipg_casestatus'))];
                            case 1:
                                caseStatus = _a.sent();
                                if (caseStatus["ipg_casestatus"] == 923720000)
                                    return [2 /*return*/, true];
                                else
                                    return [2 /*return*/, false];
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.caseIsOpened = caseIsOpened;
            /**
             * Called on PDL button click
             * @function Intake.Incident.Ribbon.OnPDLClick
             * @returns {void}
            */
            function OnPDLClick(primaryControl) {
                var formContext = primaryControl;
                var ProformParameters = {
                    caseId: formContext.data.entity.getId()
                };
                var webResourceName = "ipg_/intake/incident/PDL.html";
                var customParameters = "params=" + JSON.stringify(ProformParameters);
                Xrm.Utility.openWebResource(webResourceName, customParameters);
            }
            Ribbon.OnPDLClick = OnPDLClick;
            /**
           * enable rule for PIF Step 2
           * @function Intake.Incident.Ribbon.isPIFStep2Form
           * @returns {boolean}
           */
            function isPIFStep2Form(primaryControl) {
                var lockCase = primaryControl.getAttribute("ipg_islocked");
                if (lockCase === null || lockCase === void 0 ? void 0 : lockCase.getValue())
                    return false;
                var isStep2 = Xrm.Utility.getGlobalContext().getQueryStringParameters().p_isStep2;
                if (isStep2) {
                    return false;
                }
                return true;
            }
            Ribbon.isPIFStep2Form = isPIFStep2Form;
            function SubmitButtonEnable(primaryControl) {
                var lockCase = primaryControl.getAttribute("ipg_islocked");
                return !(lockCase === null || lockCase === void 0 ? void 0 : lockCase.getValue());
            }
            Ribbon.SubmitButtonEnable = SubmitButtonEnable;
            function ShowLegacyAuditInformation(primaryControl) {
                var env = Intake.Utility.GetEnvironment();
                var envSuffix;
                if (env) {
                    envSuffix = '-' + env;
                }
                else {
                    envSuffix = '';
                }
                var formContext = primaryControl;
                var incidentId = formContext.data.entity.getId();
                incidentId = incidentId.replace(/[{}]/g, "");
                Xrm.Navigation.openUrl("https://insight-auditinfo" + envSuffix + ".azurewebsites.net/patientprocedureauditinfo/index/" + incidentId);
            }
            Ribbon.ShowLegacyAuditInformation = ShowLegacyAuditInformation;
            /**
             * Called on Add Patient Payment button click
             * @function Intake.Incident.Ribbon.OnAddPatientPaymentClick
             * @returns {void}
            */
            function OnAddPatientPaymentClick(primaryControl) {
                //This function is depriciated
                //It was used in Case Ribbon (AddPatientPayment command)
                var formContext = primaryControl;
                var caseId = formContext.data.entity.getId();
                if (!caseId)
                    return;
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_payment";
                entityFormOptions["formId"] = "05a46600-f1c0-450e-9846-73e3e82829eb";
                Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title").then(function (incident) {
                    var formParameters = {};
                    formParameters["ipg_caseid"] = caseId;
                    formParameters["ipg_caseidname"] = incident.title;
                    formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
                    Xrm.Navigation.openForm(entityFormOptions, formParameters);
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
            Ribbon.OnAddPatientPaymentClick = OnAddPatientPaymentClick;
            /**
             * Called on Add Adjustment button click
             * @function Intake.Incident.Ribbon.OnAddAdjustmentClick
             * @returns {void}
            */
            function OnAddAdjustmentClick(primaryControl) {
                var formContext = primaryControl;
                var caseId = formContext.data.entity.getId();
                if (!caseId)
                    return;
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_adjustment";
                Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
                    var formParameters = {};
                    formParameters["ipg_caseid"] = caseId;
                    formParameters["ipg_caseidname"] = incident.title;
                    formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
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
            function OnRetrieveBenefitsClick(primaryControl) {
                var formContext = primaryControl;
                var caseId = formContext.data.entity.getId();
                if (!caseId)
                    return;
                var memberid = formContext.getAttribute("ipg_memberidnumber").getValue();
                if (!memberid) {
                    var alertStrings = { confirmButtonLabel: "OK", text: "Member Id is empty!" };
                    var alertOptions = { height: 150, width: 300 };
                    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                    return;
                }
                Xrm.Utility.showProgressIndicator("Processing...");
                var arguments = { IsUserGenerated: true };
                Intake.Utility.CallActionProcess("incidents", Intake.Utility.removeCurlyBraces(caseId), "ipg_IPGCaseActionsVerifyBenefits", arguments).then(function () {
                    formContext.data.refresh(false);
                    Xrm.Utility.closeProgressIndicator();
                });
            }
            Ribbon.OnRetrieveBenefitsClick = OnRetrieveBenefitsClick;
            /**
             * Called on Write-Off Small Balance button click
             * @function Intake.Incident.Ribbon.OnWriteOffSmallBalanceClick
             * @returns {void}
            */
            function OnWriteOffSmallBalanceClick(primaryControl) {
                var formContext = primaryControl;
                var caseId = formContext.data.entity.getId();
                if (!caseId)
                    return;
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_adjustment";
                Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
                    var formParameters = {};
                    formParameters["ipg_caseid"] = caseId;
                    formParameters["ipg_caseidname"] = incident.title;
                    formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
                    formParameters["ipg_percent"] = 100;
                    formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
                    formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
                    formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
                    formParameters["ipg_casebalance"] = incident.ipg_casebalance;
                    formParameters["ipg_applyto"] = 427880002;
                    formParameters["ipg_adjustmenttype"] = 427880000;
                    formParameters["ipg_reason"] = 427880007;
                    Xrm.Navigation.openForm(entityFormOptions, formParameters);
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
            Ribbon.OnWriteOffSmallBalanceClick = OnWriteOffSmallBalanceClick;
            /**
             * Called on Patient Discount button click
             * @function Intake.Incident.Ribbon.OnPatientDiscountClick
             * @returns {void}
            */
            function OnPatientDiscountClick(primaryControl) {
                var formContext = primaryControl;
                var caseId = formContext.data.entity.getId();
                if (!caseId)
                    return;
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_adjustment";
                Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
                    var formParameters = {};
                    formParameters["ipg_caseid"] = caseId;
                    formParameters["ipg_caseidname"] = incident.title;
                    formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
                    formParameters["ipg_percent"] = 100;
                    formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
                    formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
                    formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
                    formParameters["ipg_casebalance"] = incident.ipg_casebalance;
                    formParameters["ipg_applyto"] = 427880002;
                    formParameters["ipg_adjustmenttype"] = 427880000;
                    formParameters["ipg_reason"] = 427880004;
                    formParameters["ipg_percent"] = 0;
                    Xrm.Navigation.openForm(entityFormOptions, formParameters);
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
            Ribbon.OnPatientDiscountClick = OnPatientDiscountClick;
            /**
           * Called on Write-Off Small Balance button click
           * @function Intake.Incident.Ribbon.OnWriteOffSmallBalanceClick
           * @returns {void}
          */
            function OnWriteOffPatientBalanceClick(primaryControl) {
                var formContext = primaryControl;
                var caseId = formContext.data.entity.getId();
                if (!caseId)
                    return;
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_adjustment";
                Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
                    var formParameters = {};
                    formParameters["ipg_caseid"] = caseId;
                    formParameters["ipg_caseidname"] = incident.title;
                    formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
                    formParameters["ipg_percent"] = 100;
                    formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
                    formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
                    formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
                    formParameters["ipg_casebalance"] = incident.ipg_casebalance;
                    formParameters["ipg_applyto"] = 427880000;
                    formParameters["ipg_adjustmenttype"] = 427880000;
                    formParameters["ipg_reason"] = 427880020;
                    Xrm.Navigation.openForm(entityFormOptions, formParameters);
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
            Ribbon.OnWriteOffPatientBalanceClick = OnWriteOffPatientBalanceClick;
            function OpenCalcRev(primaryControl) {
                //debugger;
                //get CalcRev application environment suffix
                var env = Intake.Utility.GetEnvironment();
                var envSuffix;
                if (env) {
                    envSuffix = '-' + env;
                }
                else {
                    envSuffix = '';
                }
                //get incidentId
                var incidentId = primaryControl.data.entity.getId();
                incidentId = incidentId.replace(/[{}]/g, ""); //delete curly brackets
                var windowSizeAndPosition = Intake.Utility.GetCalcRevWindowSizeAndPosition();
                //finally open the window
                window.open("https://insight-calcrev" + envSuffix + ".azurewebsites.net/Case/CalcRev/" + incidentId, 'calcRevWindow', windowSizeAndPosition);
            }
            Ribbon.OpenCalcRev = OpenCalcRev;
            function OpenEstimatedCalcRev(primaryControl) {
                //debugger;
                //get CalcRev application environment suffix
                var env = Intake.Utility.GetEnvironment();
                var envSuffix;
                if (env) {
                    envSuffix = '-' + env;
                }
                else {
                    envSuffix = '';
                }
                //get incidentId
                var incidentId = primaryControl.data.entity.getId();
                incidentId = incidentId.replace(/[{}]/g, ""); //delete curly brackets
                var windowSizeAndPosition = Intake.Utility.GetCalcRevWindowSizeAndPosition();
                //finally open the window
                window.open("https://insight-calcrev" + envSuffix + ".azurewebsites.net/Case/Estimate#/" + incidentId, 'calcRevWindow', windowSizeAndPosition);
            }
            Ribbon.OpenEstimatedCalcRev = OpenEstimatedCalcRev;
            function ShouldEnableCalcRevButton(primaryControl) {
                var lockCase = primaryControl.getAttribute("ipg_islocked");
                if (lockCase === null || lockCase === void 0 ? void 0 : lockCase.getValue()) {
                    return false;
                }
                else {
                    return lifeCycleStepForCalcRevButton(primaryControl);
                }
            }
            Ribbon.ShouldEnableCalcRevButton = ShouldEnableCalcRevButton;
            function isCaseOnHoldView(selectedControl) {
                return selectedControl.getViewSelector
                    && selectedControl.getViewSelector().getCurrentView().name.toLowerCase().indexOf('hold') > -1;
            }
            Ribbon.isCaseOnHoldView = isCaseOnHoldView;
            function ReleaseHold(entityIds, selectedControl) {
                if (selectedControl && entityIds && entityIds.length > 0) {
                    var data = { caseId: entityIds, action: "unhold" };
                    Xrm.Navigation.openWebResource("ipg_/intake/incident/holdCase.html", null, JSON.stringify(data));
                    window.addEventListener("storage", function (ev) {
                        if (ev.key === "RefreshHoldCaseRibbon" && ev.newValue === "true") {
                            selectedControl.refresh();
                            localStorage.removeItem("RefreshHoldCaseRibbon");
                        }
                    });
                }
            }
            Ribbon.ReleaseHold = ReleaseHold;
            function IsCaseClosedById(entityTypeName, entityId) {
                return __awaiter(this, void 0, void 0, function () {
                    var incident;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                if (!(entityTypeName === 'incident')) return [3 /*break*/, 2];
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord(entityTypeName, entityId, '?$select=ipg_casestatus')];
                            case 1:
                                incident = _a.sent();
                                return [2 /*return*/, !(incident.ipg_casestatus && incident.ipg_casestatus == 923720001)];
                            case 2: return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.IsCaseClosedById = IsCaseClosedById;
            function gatePostAction(formContext, formData, mainGatingCallback) {
                if (mainGatingCallback === void 0) { mainGatingCallback = undefined; }
                var parametersConfirm = {
                    "Target": {
                        "incidentid": formData.Id,
                        "@odata.type": "Microsoft.Dynamics.CRM." + formData.EntityName
                    }
                };
                Xrm.Utility.showProgressIndicator("Processing...");
                callAction("ipg_IPGGatingGateProcessingPostAction", parametersConfirm, true, function (resultsConfirm) {
                    formContext.data.refresh(true).then(function () {
                        formContext.ui.refreshRibbon();
                        Xrm.Utility.closeProgressIndicator();
                        typeof mainGatingCallback === "function" && mainGatingCallback(resultsConfirm);
                    });
                });
            }
            function IsCaseLocked(entityTypeName, entityId) {
                return __awaiter(this, void 0, void 0, function () {
                    var incident;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                if (!(entityTypeName === 'incident')) return [3 /*break*/, 2];
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord(entityTypeName, entityId, '?$select=ipg_islocked')];
                            case 1:
                                incident = _a.sent();
                                return [2 /*return*/, !((incident === null || incident === void 0 ? void 0 : incident.ipg_islocked) == true)];
                            case 2: return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.IsCaseLocked = IsCaseLocked;
            function OpenCase(formContext) {
                var _a, _b;
                return __awaiter(this, void 0, void 0, function () {
                    var ipg_IPGCaseOpenRequest, e_1;
                    return __generator(this, function (_c) {
                        switch (_c.label) {
                            case 0:
                                Xrm.Utility.showProgressIndicator("");
                                if (!(((_a = formContext === null || formContext === void 0 ? void 0 : formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue()) === 923720001)) return [3 /*break*/, 5];
                                ipg_IPGCaseOpenRequest = {
                                    entity: { entityType: "incident", id: formContext.data.entity.getId() },
                                    getMetadata: function () {
                                        return {
                                            boundParameter: "entity",
                                            parameterTypes: {
                                                "entity": {
                                                    "typeName": "mscrm.incident",
                                                    "structuralProperty": 5
                                                }
                                            },
                                            operationType: 0,
                                            operationName: "ipg_IPGCaseOpen"
                                        };
                                    }
                                };
                                _c.label = 1;
                            case 1:
                                _c.trys.push([1, 4, , 5]);
                                return [4 /*yield*/, Xrm.WebApi.online.execute(ipg_IPGCaseOpenRequest)];
                            case 2:
                                _c.sent();
                                return [4 /*yield*/, formContext.data.refresh(false)];
                            case 3:
                                _c.sent();
                                formContext.ui.refreshRibbon();
                                return [3 /*break*/, 5];
                            case 4:
                                e_1 = _c.sent();
                                Xrm.Navigation.openErrorDialog({ message: (_b = e_1.message) !== null && _b !== void 0 ? _b : "There is error. Please try later or Contact System Administrator!" });
                                return [3 /*break*/, 5];
                            case 5:
                                Xrm.Utility.closeProgressIndicator();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.OpenCase = OpenCase;
            function getNextLifeCycleStep(formContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var entity;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(formContext.data.entity.getId()), "?$select=_ipg_lifecyclestepid_value")];
                            case 1:
                                entity = _a.sent();
                                return [2 /*return*/, entity["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"]];
                        }
                    });
                });
            }
            function GenerateClaimOverride(primaryControl, caseId) {
                var _a;
                return __awaiter(this, void 0, void 0, function () {
                    var entityFormOptions, carrierLookup, carrier, claimType, formParameters, caseLookupValue, claimGenerationOverrides, claims, recentClaim;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                entityFormOptions = {
                                    entityName: "ipg_claimgenerationoverride",
                                    useQuickCreateForm: true
                                };
                                carrierLookup = (_a = primaryControl.getAttribute("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.getValue();
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord(carrierLookup[0].entityType, carrierLookup[0].id, "?$select=ipg_claimtype")];
                            case 1:
                                carrier = _b.sent();
                                claimType = carrier.ipg_claimtype;
                                formParameters = {};
                                caseLookupValue = new Array();
                                caseLookupValue[0] = new Object();
                                caseLookupValue[0].id = caseId;
                                caseLookupValue[0].name = primaryControl.getAttribute("title").getValue();
                                caseLookupValue[0].entityType = "incident";
                                formParameters["ipg_caseid"] = caseLookupValue;
                                formParameters["ipg_carrierid"] = carrierLookup;
                                formParameters["ipg_claimformtype"] = claimType;
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_claimgenerationoverride", "?$select=ipg_claimtogenerate,ipg_claimtoreplace,ipg_claimicn,ipg_claimnotes&$filter=_ipg_caseid_value eq " + caseId)];
                            case 2:
                                claimGenerationOverrides = _b.sent();
                                if (!(claimGenerationOverrides && claimGenerationOverrides.entities.length > 0)) return [3 /*break*/, 3];
                                formParameters["ipg_claimtogenerate"] = claimGenerationOverrides.entities[0].ipg_claimtogenerate;
                                formParameters["ipg_claimtoreplace"] = claimGenerationOverrides.entities[0].ipg_claimtoreplace;
                                formParameters["ipg_claimicn"] = claimGenerationOverrides.entities[0].ipg_claimicn;
                                formParameters["ipg_claimnotes"] = claimGenerationOverrides.entities[0].ipg_claimnotes;
                                return [3 /*break*/, 5];
                            case 3: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("invoice", "?$select=name,ipg_icn&$filter=_ipg_caseid_value eq " + caseId + " and _customerid_value eq " + carrierLookup[0].id)];
                            case 4:
                                claims = _b.sent();
                                if (claims && claims.entities.length == 1) {
                                    recentClaim = claims.entities[0];
                                }
                                else if (claims.entities.length > 1) {
                                    recentClaim = claims.entities[0];
                                    claims.entities.forEach(function (element) {
                                        if (element.name.substr(element.name.length - 1) > recentClaim.name.substr(recentClaim.name.length - 1)) {
                                            recentClaim = element;
                                        }
                                    });
                                }
                                else {
                                    throw Error("There is no claims with current Carrier.");
                                }
                                formParameters["ipg_claimtoreplace"] = recentClaim.name;
                                formParameters["ipg_claimicn"] = recentClaim.ipg_icn;
                                _b.label = 5;
                            case 5:
                                Xrm.Navigation.openForm(entityFormOptions, formParameters);
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.GenerateClaimOverride = GenerateClaimOverride;
            function IfCaseHasAssociatedClaim(caseId) {
                return __awaiter(this, void 0, void 0, function () {
                    var claims;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("invoice", "?$select=invoiceid&$filter=_ipg_caseid_value eq " + caseId)];
                            case 1:
                                claims = _a.sent();
                                return [2 /*return*/, claims && claims.entities.length > 0];
                        }
                    });
                });
            }
            Ribbon.IfCaseHasAssociatedClaim = IfCaseHasAssociatedClaim;
            function IsLifeCycleStep(primaryControl, lifeCycleStepString) {
                var _a;
                if (lifeCycleStepString && lifeCycleStepString.length > 0) {
                    var lifeCycleStepLookup = (_a = primaryControl.getAttribute("ipg_lifecyclestepid")) === null || _a === void 0 ? void 0 : _a.getValue();
                    if (lifeCycleStepLookup && lifeCycleStepLookup.length > 0) {
                        return lifeCycleStepLookup[0].name === lifeCycleStepString;
                    }
                }
            }
            Ribbon.IsLifeCycleStep = IsLifeCycleStep;
            function showCalcRev(primaryControl) {
                return lifeCycleStepForCalcRevButton(primaryControl);
            }
            Ribbon.showCalcRev = showCalcRev;
            function lifeCycleStepForCalcRevButton(primaryControl) {
                return (IsLifeCycleStep(primaryControl, "Calculate Revenue") || IsLifeCycleStep(primaryControl, "Generate PO"));
            }
            function AddExistingOrder(selectedItems, primaryControl, selectedControl) {
                var _a;
                return __awaiter(this, void 0, void 0, function () {
                    var partEntityName, orderAttributeSchemaName, parts, caseId, _i, selectedItems_1, partItem, part, alertStrings, alertOptions, lookupOptions;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                partEntityName = selectedItems[0].TypeName;
                                orderAttributeSchemaName = "ipg_PurchaseOrderId";
                                if (partEntityName === "ipg_estimatedcasepartdetail") {
                                    orderAttributeSchemaName = "ipg_purchaseorderid";
                                }
                                parts = [];
                                caseId = primaryControl.data.entity.getId().replace('{', '').replace('}', '').toLowerCase();
                                _i = 0, selectedItems_1 = selectedItems;
                                _b.label = 1;
                            case 1:
                                if (!(_i < selectedItems_1.length)) return [3 /*break*/, 4];
                                partItem = selectedItems_1[_i];
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord(partItem.TypeName, partItem.Id, "?$select=_ipg_purchaseorderid_value&$expand=" + orderAttributeSchemaName + "($select=statuscode,_ipg_caseid_value)")];
                            case 2:
                                part = _b.sent();
                                if (part._ipg_purchaseorderid_value == null || ((_a = part[orderAttributeSchemaName]) === null || _a === void 0 ? void 0 : _a._ipg_caseid_value) !== caseId || part[orderAttributeSchemaName].statuscode === 923720011 /*Voided*/) {
                                    parts.push(partItem);
                                    console.log(part);
                                }
                                _b.label = 3;
                            case 3:
                                _i++;
                                return [3 /*break*/, 1];
                            case 4:
                                if (parts.length == 0) {
                                    alertStrings = { confirmButtonLabel: "OK", text: "Part cannot be added to a PO since it is already on another PO on the Case.", title: "Action Cancelled" };
                                    alertOptions = { height: 120, width: 260 };
                                    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                                    return [2 /*return*/];
                                }
                                lookupOptions = {
                                    defaultEntityType: "salesorder",
                                    entityTypes: ["salesorder"],
                                    allowMultiSelect: false,
                                    disableMru: true,
                                    filters: [{ filterXml: '<filter type="and"><condition attribute="ipg_caseid" operator="eq" uitype="incident" value="' + caseId + '" /></filter>', entityLogicalName: "salesorder" }]
                                };
                                Xrm.Utility.lookupObjects(lookupOptions).then(function (orders) {
                                    var data;
                                    if (partEntityName === "ipg_estimatedcasepartdetail") {
                                        data =
                                            {
                                                "ipg_purchaseorderid@odata.bind": "/salesorders(" + orders[0].id.replace('{', '').replace('}', '') + ")"
                                            };
                                    }
                                    else {
                                        data =
                                            {
                                                "ipg_PurchaseOrderId@odata.bind": "/salesorders(" + orders[0].id.replace('{', '').replace('}', '') + ")",
                                                "ipg_ischanged": false,
                                                "ipg_islocked": true
                                            };
                                    }
                                    for (var _i = 0, parts_1 = parts; _i < parts_1.length; _i++) {
                                        var p = parts_1[_i];
                                        Xrm.WebApi.updateRecord(p.TypeName, p.Id, data).then(function success(result) {
                                            var alertStrings = { confirmButtonLabel: "OK", text: "Associated successfully", title: "Success" };
                                            var alertOptions = { height: 120, width: 260 };
                                            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                                            selectedControl.refresh();
                                            selectedControl.refreshRibbon(true);
                                        }, function (error) {
                                            console.log(error.message);
                                        });
                                    }
                                }, function (error) { console.log(error); });
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.AddExistingOrder = AddExistingOrder;
            function RefreshGrid(selectedControl) {
                selectedControl.refresh();
            }
            Ribbon.RefreshGrid = RefreshGrid;
            function UnlockCase(primaryControl) {
                var pageInput = {
                    pageType: 'webresource',
                    webresourceName: 'ipg_/intake/incident/UnlockCase.html',
                    data: primaryControl.data.entity.getId().replace(/[{}]/g, "")
                };
                var navigationOptions = {
                    target: 2,
                    height: 450,
                    width: 600,
                    position: 1,
                };
                Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(function () {
                    primaryControl.data.refresh(false);
                    primaryControl.ui.refreshRibbon();
                });
            }
            Ribbon.UnlockCase = UnlockCase;
            function EnableUnlockCase(primaryControl) {
                var _a;
                return __awaiter(this, void 0, void 0, function () {
                    var caseid, userId, isAdmin1role, incident;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                caseid = primaryControl.data.entity.getId();
                                userId = Xrm.Utility.getGlobalContext().userSettings.userId.replace(/[{}]/g, "").toLowerCase();
                                isAdmin1role = Xrm.Utility.getGlobalContext().userSettings.roles.get(function (r) { var _a; return ((_a = r.name) === null || _a === void 0 ? void 0 : _a.toLowerCase()) == 'admin1'; }).length > 0;
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("incident", "?$select=ipg_islocked&$expand=ipg_FacilityId($select=_ipg_facilitycasemgrid_value)&$filter=(incidentid eq " + caseid + ")")];
                            case 1:
                                incident = _b.sent();
                                if (incident.entities[0].ipg_islocked
                                    && (isAdmin1role
                                        || ((_a = incident.entities[0].ipg_FacilityId) === null || _a === void 0 ? void 0 : _a._ipg_facilitycasemgrid_value) && incident.entities[0].ipg_FacilityId._ipg_facilitycasemgrid_value.replace(/[{}]/g, "").toLowerCase() == userId)) {
                                    return [2 /*return*/, true];
                                }
                                else {
                                    return [2 /*return*/, false];
                                }
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.EnableUnlockCase = EnableUnlockCase;
        })(Ribbon = Incident.Ribbon || (Incident.Ribbon = {}));
    })(Incident = Intake.Incident || (Intake.Incident = {}));
})(Intake || (Intake = {}));
