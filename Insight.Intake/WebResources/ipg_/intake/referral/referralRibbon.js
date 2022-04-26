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
* @namespace Intake.Referral.Ribbon
*/
var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        var Ribbon;
        (function (Ribbon) {
            var CaseStatusesEnum = {
                Close: 923720001,
                Open: 923720000,
            };
            var CaseReasonsEnum = {
                Rejected: 923720002,
            };
            var ipg_CaseOutcomeCodes = {
                Gate1Fail: 427880000,
                Gate2Fail: 427880002,
                GateEhrFail: 427880020
            };
            var StructualPropertyTypes = {
                Unknown: 0,
                PrimitiveType: 1,
                ComplexType: 2,
                EnumerationType: 3,
                Collection: 4,
                EntityType: 5,
            };
            /**
            * Enable rule for Reopen Referral button on referral form
            * @function Intake.Referral.Ribbon.IsEnabledAddExistingButton
            */
            function IsEnabledAddExistingButton(firstPrimaryItemId, primaryEntityTypeName, selectedControl, allowedMultipleAssociationDocTypes) {
                var _a;
                return __awaiter(this, void 0, void 0, function () {
                    var document, relatedEntitiesCount, docTypesAbbr, error_1;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                if (!(primaryEntityTypeName === "ipg_document")) return [3 /*break*/, 4];
                                if (!firstPrimaryItemId.replace("{", "").replace("}", "")) return [3 /*break*/, 4];
                                _b.label = 1;
                            case 1:
                                _b.trys.push([1, 3, , 4]);
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", firstPrimaryItemId, "?$select=_ipg_referralid_value,_ipg_caseid_value,ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation, ipg_name)")];
                            case 2:
                                document = _b.sent();
                                relatedEntitiesCount = selectedControl.getGrid().getTotalRecordCount();
                                docTypesAbbr = allowedMultipleAssociationDocTypes.split(',');
                                if (((_a = document.ipg_DocumentTypeId) === null || _a === void 0 ? void 0 : _a.ipg_documenttypeabbreviation) === 'PIF') {
                                    return [2 /*return*/, false];
                                }
                                return [2 /*return*/, isDocumentTypeAllowslMultipleCaseOrReferralAssociation(document, relatedEntitiesCount, docTypesAbbr)];
                            case 3:
                                error_1 = _b.sent();
                                Xrm.Navigation.openAlertDialog(error_1 === null || error_1 === void 0 ? void 0 : error_1.message);
                                return [3 /*break*/, 4];
                            case 4: return [2 /*return*/, true];
                        }
                    });
                });
            }
            Ribbon.IsEnabledAddExistingButton = IsEnabledAddExistingButton;
            function isDocumentTypeAllowslMultipleCaseOrReferralAssociation(result, relatedEntitiesCount, docTypesAbbr) {
                var _a, _b;
                return docTypesAbbr.find(function (abbr) { var _a; return abbr === ((_a = result.ipg_DocumentTypeId) === null || _a === void 0 ? void 0 : _a.ipg_documenttypeabbreviation); })
                    || (result._ipg_referralid_value == null && result._ipg_caseid_value == null && relatedEntitiesCount === 0
                        && ((_a = result.ipg_DocumentTypeId) === null || _a === void 0 ? void 0 : _a.ipg_documenttypeabbreviation) != "FAX" && ((_b = result.ipg_DocumentTypeId) === null || _b === void 0 ? void 0 : _b.ipg_name) != "User Uploaded Generic Document");
            }
            /**
            * Enable rule for Reopen Referral button on referral form
            * @function Intake.Referral.Ribbon.ShowReopenReferralButtonOnForm
            */
            function ShowReopenReferralButtonOnForm(primaryControl) {
                return false; //this button is deprecated
            }
            Ribbon.ShowReopenReferralButtonOnForm = ShowReopenReferralButtonOnForm;
            /**
            * Enable rule for Reopen Referral button
            * @function Intake.Referral.Ribbon.ShowReopenReferralButton
            */
            function ShowReopenReferralButton() {
                return false; //this button is deprecated
            }
            Ribbon.ShowReopenReferralButton = ShowReopenReferralButton;
            /**
            * Enable rule for Reopen Referral button on referral form
            * @function Intake.Referral.Ribbon.IsEnabledReopenRefferalButton
            */
            function IsEnabledReopenRefferalButton(primaryControl) {
                return false; //this button is deprecated
            }
            Ribbon.IsEnabledReopenRefferalButton = IsEnabledReopenRefferalButton;
            /**
            * Enable rule for Reopen Referral button
            * @function Intake.Referral.Ribbon.EnableReopenRefferalButton
            */
            function EnableReopenRefferalButton(referralId) {
                return false; //this button is deprecated
            }
            Ribbon.EnableReopenRefferalButton = EnableReopenRefferalButton;
            /**
            * Called on Reopen Referral button click on referral form
            * @function Intake.Referral.Ribbon.OnReopenReferralClickOnForm
            * @returns {void}
            */
            function OnReopenReferralClickOnForm(primaryControl) {
                var formContext = primaryControl;
                var referralId = formContext.data.entity.getId();
                OnReopenReferralClick(referralId);
            }
            Ribbon.OnReopenReferralClickOnForm = OnReopenReferralClickOnForm;
            /**
            * Called on Reopen Referral button click
            * @function Intake.Referral.Ribbon.OnReopenReferralClick
            * @returns {void}
            */
            function OnReopenReferralClick(selectedReferralId) {
                Xrm.WebApi.retrieveRecord("ipg_referral", selectedReferralId.replace("{", "").replace("}", "").toLowerCase(), "?$select=_ipg_associatedcaseid_value, ipg_casestatus")
                    .then(function success(referral) {
                    if (referral) {
                        var referralStatus = referral === null || referral === void 0 ? void 0 : referral.ipg_casestatus;
                        var assosiatedCase = referral === null || referral === void 0 ? void 0 : referral._ipg_associatedcaseid_value;
                        if (referralStatus === CaseStatusesEnum.Close &&
                            assosiatedCase != null) {
                            Xrm.Navigation.openAlertDialog({
                                text: "Referral cannot be re-open",
                            });
                            return;
                        }
                        return referral;
                    }
                })
                    .then(function (referral) {
                    if (referral) {
                        var reqObject = {
                            Referral: {
                                "@odata.type": "Microsoft.Dynamics.CRM.ipg_referral",
                                ipg_referralid: referral.ipg_referralid,
                            },
                            getMetadata: function () {
                                return {
                                    boundParameter: null,
                                    operationType: 0,
                                    operationName: "ipg_IPGReferralReopenReferral",
                                    parameterTypes: {
                                        Referral: {
                                            typeName: "mscrm.ipg_referral",
                                            structuralProperty: 5,
                                        },
                                    },
                                };
                            },
                        };
                        Xrm.Utility.showProgressIndicator("Intake");
                        Xrm.WebApi.online.execute(reqObject).then(function (response) {
                            Xrm.Utility.closeProgressIndicator();
                            if (response.ok) {
                                var entityFormOptions = {};
                                entityFormOptions["formId"] = PifStep2Form;
                                entityFormOptions["entityName"] = "ipg_referral";
                                entityFormOptions["entityId"] = referral.ipg_referralid;
                                Xrm.Navigation.openForm(entityFormOptions);
                            }
                        }, function () {
                            Xrm.Utility.closeProgressIndicator();
                        });
                    }
                });
            }
            Ribbon.OnReopenReferralClick = OnReopenReferralClick;
            /**
            * Called on Submit button click
            * @function Intake.Referral.Ribbon.OnSubmitClick
            * @returns {void}
            */
            function OnSubmitClick(primaryControl) {
                var _a;
                var formContext = primaryControl;
                var caseStatus = (_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue();
                if (caseStatus && caseStatus == 923720001) {
                    Xrm.Navigation.openAlertDialog({
                        confirmButtonLabel: "Ok",
                        text: "You are not able to submit closed referral.",
                    }).then(function () {
                        return;
                    });
                }
                else {
                    saveAndStartGating(formContext);
                }
            }
            Ribbon.OnSubmitClick = OnSubmitClick;
            function saveAndStartGating(formContext) {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        Xrm.Utility.showProgressIndicator("Processing...");
                        Intake.Referral.showWarningMessage = false;
                        formContext.data.save().then(function () {
                            return __awaiter(this, void 0, void 0, function () {
                                var referralid, parameters, mainGatingCallback;
                                return __generator(this, function (_a) {
                                    referralid = formContext.data.entity.getId();
                                    parameters = {
                                        Target: {
                                            ipg_referralid: formContext.data.entity.getId(),
                                            "@odata.type": "Microsoft.Dynamics.CRM." +
                                                formContext.data.entity.getEntityName(),
                                        },
                                        InitiatingUser: {
                                            systemuserid: Xrm.Utility.getGlobalContext().userSettings.userId,
                                            "@odata.type": "Microsoft.Dynamics.CRM.systemuser",
                                        },
                                        IsManual: true,
                                    };
                                    mainGatingCallback = function (results) {
                                        Intake.Referral.showWarningMessage = true;
                                        if (results.Output != null && results.Output != "") {
                                            var alertStrings = {
                                                confirmButtonLabel: "Close Referral",
                                                cancelButtonLabel: "Cancel",
                                                text: results.Output,
                                                title: "Errors",
                                            };
                                            var alertOptions = { height: 150, width: 300 };
                                            Xrm.Navigation.openConfirmDialog(alertStrings, alertOptions).then(function success(result) {
                                                if (result.confirmed) {
                                                    Xrm.WebApi.updateRecord("ipg_referral", referralid, {
                                                        statecode: 1,
                                                        statuscode: 2,
                                                    }).then(function () {
                                                        formContext.data.refresh(false).then(function (rslt) {
                                                            checkIfRejected(formContext);
                                                        });
                                                    });
                                                }
                                            }, function (error) {
                                                console.log(error.message);
                                            });
                                        }
                                        else {
                                            var caseLookupValue_1 = formContext
                                                .getAttribute("ipg_associatedcaseid")
                                                .getValue();
                                            var formParameters_1 = {};
                                            formParameters_1["p_isStep2"] = "1";
                                            formContext.data.save().then(function () {
                                                if (caseLookupValue_1 && caseLookupValue_1.length) {
                                                    Xrm.Navigation.openForm({
                                                        entityName: caseLookupValue_1[0].entityType,
                                                        entityId: caseLookupValue_1[0].id,
                                                    }, formParameters_1);
                                                }
                                                else {
                                                    Xrm.Navigation.openForm({
                                                        entityId: formContext.data.entity.getId(),
                                                        entityName: formContext.data.entity.getEntityName(),
                                                    });
                                                }
                                            }, function (error) {
                                                console.log(error.message);
                                            });
                                        }
                                    };
                                    callAction("ipg_IPGGatingStartGateProcessing", parameters, true, function (results) {
                                        var _this = this;
                                        formContext.data.refresh(true).then(function () { return __awaiter(_this, void 0, void 0, function () {
                                            var alertStrings, _a, confirmStr, confirmResult, alertStr;
                                            return __generator(this, function (_b) {
                                                switch (_b.label) {
                                                    case 0:
                                                        Xrm.Utility.closeProgressIndicator();
                                                        if (!results.SeverityLevel) {
                                                            alertStrings = {
                                                                confirmButtonLabel: "OK",
                                                                text: results.Output,
                                                            };
                                                            Xrm.Navigation.openAlertDialog(alertStrings, {
                                                                height: 400,
                                                                width: 600,
                                                            }).then(function success(result) { }, function (error) {
                                                                console.log(error.message);
                                                            });
                                                        }
                                                        _a = results.SeverityLevel;
                                                        switch (_a) {
                                                            case 923720003: return [3 /*break*/, 1];
                                                            case 923720002: return [3 /*break*/, 3];
                                                            case 923720001: return [3 /*break*/, 4];
                                                            case 923720000: return [3 /*break*/, 5];
                                                        }
                                                        return [3 /*break*/, 6];
                                                    case 1:
                                                        confirmStr = {
                                                            title: "Alert!",
                                                            text: "The following critical issues have occurred. This Referral cannot be considered with the current information entered. Select \u2018Close Referral\u2019 to proceed with the closing of this Referral or select \u2018Continue\u2019 to return and continue making changes to this Referral and resubmit for consideration.\n" + results.Output,
                                                            confirmButtonLabel: "Close Referral",
                                                            cancelButtonLabel: "Continue",
                                                        };
                                                        return [4 /*yield*/, Xrm.Navigation.openConfirmDialog(confirmStr, {
                                                                height: 400,
                                                                width: 600,
                                                            })];
                                                    case 2:
                                                        confirmResult = _b.sent();
                                                        if (confirmResult.confirmed) {
                                                            gatePostAction(formContext, mainGatingCallback);
                                                        }
                                                        return [3 /*break*/, 6];
                                                    case 3:
                                                        alertStr = {
                                                            title: "Error",
                                                            text: "The following errors were found. Case cannot proceed to the next step until these errors are corrected and Referral is reprocessed through the 'Submit' action. \n" + results.Output,
                                                            confirmButtonLabel: "Close",
                                                        };
                                                        Xrm.Navigation.openAlertDialog(alertStr, {
                                                            height: 400,
                                                            width: 600,
                                                        }).then(function () {
                                                            gatePostAction(formContext, mainGatingCallback);
                                                        });
                                                        return [3 /*break*/, 6];
                                                    case 4:
                                                        Xrm.WebApi.retrieveRecord("ipg_referral", Intake.Utility.removeCurlyBraces(formContext.data.entity.getId()), "?$select=_ipg_lifecyclestepid_value").then(function success(referral) {
                                                            if (referral) {
                                                                alertStr = {
                                                                    title: "Warning",
                                                                    text: "The following warnings were found. Referral promoted to " + referral["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"] + " step. \n" + results.Output,
                                                                    confirmButtonLabel: "Close",
                                                                };
                                                                Xrm.Navigation.openAlertDialog(alertStr, {
                                                                    height: 400,
                                                                    width: 600,
                                                                }).then(function () {
                                                                    gatePostAction(formContext, mainGatingCallback);
                                                                });
                                                            }
                                                        });
                                                        return [3 /*break*/, 6];
                                                    case 5:
                                                        Xrm.WebApi.retrieveRecord("ipg_referral", Intake.Utility.removeCurlyBraces(formContext.data.entity.getId()), "?$select=_ipg_lifecyclestepid_value").then(function success(referral) {
                                                            if (referral) {
                                                                alertStr = {
                                                                    title: "Success",
                                                                    text: "No issues found. Referral promoted to " + referral["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"],
                                                                    confirmButtonLabel: "Close",
                                                                };
                                                                Xrm.Navigation.openAlertDialog(alertStr, {
                                                                    height: 400,
                                                                    width: 600,
                                                                }).then(function () {
                                                                    mainGatingCallback({});
                                                                });
                                                            }
                                                        });
                                                        return [3 /*break*/, 6];
                                                    case 6: return [2 /*return*/];
                                                }
                                            });
                                        }); });
                                    });
                                    return [2 /*return*/];
                                });
                            });
                        }, function (error) {
                            Xrm.Utility.closeProgressIndicator();
                            Intake.Referral.showWarningMessage = true;
                            console.log(error.message);
                        });
                        return [2 /*return*/];
                    });
                });
            }
            /**
            * call Custom action
            * @function Intake.Referral.Ribbon.callAction
            * @returns {void}
            */
            function callAction(actionName, parameters, async, successCallback) {
                var req = new XMLHttpRequest();
                req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() +
                    "/api/data/v9.1/" +
                    actionName, async);
                req.setRequestHeader("OData-MaxVersion", "4.0");
                req.setRequestHeader("OData-Version", "4.0");
                req.setRequestHeader("Accept", "application/json");
                req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                req.onreadystatechange = function () {
                    if (this.readyState === 4) {
                        req.onreadystatechange = null;
                        if (this.status === 200 || this.status === 204) {
                            if (this.response == "")
                                successCallback();
                            else
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
            function checkIfRejected(formCtx) {
                var _a;
                if (((_a = formCtx.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue()) == 923720001) {
                    //Closed
                    Xrm.Navigation.navigateTo({
                        pageType: "entitylist",
                        entityName: "ipg_document",
                    });
                }
            }
            /**
            * Do not show Submit button if this referral has an associated case already
            * @param primaryControl
            */
            function isShowSubmitButton(primaryControl) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext, referralId;
                    return __generator(this, function (_a) {
                        formContext = primaryControl;
                        if (formContext.ui.getFormType() == 1) {
                            return [2 /*return*/, true];
                        }
                        referralId = formContext.data.entity.getId();
                        if (referralId) {
                            return [2 /*return*/, Xrm.WebApi.retrieveRecord("ipg_referral", referralId, "?$select=_ipg_associatedcaseid_value").then(function success(result) {
                                    if (result._ipg_associatedcaseid_value) {
                                        return false;
                                    }
                                    else {
                                        return true;
                                    }
                                }, function (error) {
                                    Xrm.Navigation.openAlertDialog(error.message);
                                })];
                        }
                        return [2 /*return*/, false];
                    });
                });
            }
            Ribbon.isShowSubmitButton = isShowSubmitButton;
            var PifStep2Form = "7266869e-f61a-45e5-84f9-c7c5446dbfc5"; //PIF Intake Form - Step 2 form GUID
            function isShowCancelButton(primaryControl) {
                var formId = primaryControl.ui.formSelector.getCurrentItem().getId();
                return formId !== PifStep2Form;
            }
            Ribbon.isShowCancelButton = isShowCancelButton;
            function isReferralClosedAndHasAssociatedCase(primaryControl) {
                var _a, _b;
                var formContext = primaryControl;
                if (formContext.data.entity.getEntityName() == "ipg_referral") {
                    var caseStatus = (_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue();
                    var associatedCase = (_b = formContext
                        .getAttribute("ipg_associatedcaseid")) === null || _b === void 0 ? void 0 : _b.getValue();
                    if (associatedCase && caseStatus && caseStatus == 923720001) {
                        return false;
                    }
                }
                return true;
            }
            Ribbon.isReferralClosedAndHasAssociatedCase = isReferralClosedAndHasAssociatedCase;
            /**
            * Show EHR CPT Override button button if this referral has been rejected at EHR Gate
            * @param primaryControl
            */
            function isEnableEhrCptOverrideButton(primaryControl) {
                var formContext = primaryControl;
                //case status = closed
                //no associated case
                //gate outcome = EHRFail
                var caseStatusAttribute = formContext.getAttribute('ipg_casestatus');
                var associatedCaseAttribute = formContext.getAttribute('ipg_associatedcaseid');
                var caseOutcomeAttribute = formContext.getAttribute('ipg_caseoutcome');
                if (caseStatusAttribute && associatedCaseAttribute && caseOutcomeAttribute) {
                    var caseStatusValue = caseStatusAttribute.getValue();
                    var associatedCaseValue = associatedCaseAttribute.getValue();
                    var caseOutcomeValue = caseOutcomeAttribute.getValue();
                    if (caseStatusValue == CaseStatusesEnum.Close
                        && (!associatedCaseValue || associatedCaseValue.length == 0)
                        && caseOutcomeValue == ipg_CaseOutcomeCodes.GateEhrFail) {
                        return true;
                    }
                }
                return false;
            }
            Ribbon.isEnableEhrCptOverrideButton = isEnableEhrCptOverrideButton;
            function OnOverrideEhrCptClick(primaryControl) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext, referralId, reqObject, overrideResult, caseStatusAttribute;
                    var _this = this;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = primaryControl;
                                referralId = formContext.data.entity.getId().replace('{', '').replace('}', '');
                                reqObject = {
                                    entity: {
                                        entityType: "ipg_referral",
                                        id: referralId,
                                    },
                                    getMetadata: function () {
                                        return {
                                            boundParameter: "entity",
                                            operationType: 0,
                                            operationName: "ipg_IPGReferralActionsEhrCptOverride",
                                            parameterTypes: {
                                                entity: {
                                                    typeName: "mscrm.ipg_referral",
                                                    structuralProperty: StructualPropertyTypes.EntityType,
                                                },
                                            },
                                        };
                                    },
                                };
                                Xrm.Utility.showProgressIndicator("Please wait...");
                                return [4 /*yield*/, Xrm.WebApi.online.execute(reqObject).then(function (response) { return __awaiter(_this, void 0, void 0, function () {
                                        return __generator(this, function (_a) {
                                            //debugger;
                                            Xrm.Utility.closeProgressIndicator();
                                            if (response.ok) {
                                                return [2 /*return*/, response.json()];
                                            }
                                            return [2 /*return*/];
                                        });
                                    }); }, function () {
                                        Xrm.Utility.closeProgressIndicator();
                                    })];
                            case 1:
                                overrideResult = _a.sent();
                                if (!overrideResult.IsSuccess) return [3 /*break*/, 3];
                                caseStatusAttribute = formContext.getAttribute('ipg_casestatus');
                                if (caseStatusAttribute) {
                                    caseStatusAttribute.setValue(CaseStatusesEnum.Open); //set Open status because Closed referral cannot be saved
                                }
                                return [4 /*yield*/, saveAndStartGating(formContext)];
                            case 2:
                                _a.sent();
                                _a.label = 3;
                            case 3: return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.OnOverrideEhrCptClick = OnOverrideEhrCptClick;
            function gatePostAction(formContext, mainGatingCallback) {
                if (mainGatingCallback === void 0) { mainGatingCallback = undefined; }
                var parametersConfirm = {
                    Target: {
                        ipg_referralid: formContext.data.entity.getId(),
                        "@odata.type": "Microsoft.Dynamics.CRM." + formContext.data.entity.getEntityName(),
                    },
                };
                Xrm.Utility.showProgressIndicator("Processing...");
                callAction("ipg_IPGGatingGateProcessingPostAction", parametersConfirm, true, function (resultsConfirm) {
                    formContext.data.refresh(true).then(function () {
                        formContext.getControl("Timeline") &&
                            formContext.getControl("Timeline").refresh();
                        Xrm.Utility.closeProgressIndicator();
                        typeof mainGatingCallback === "function" &&
                            mainGatingCallback(resultsConfirm);
                    });
                });
            }
        })(Ribbon = Referral.Ribbon || (Referral.Ribbon = {}));
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
