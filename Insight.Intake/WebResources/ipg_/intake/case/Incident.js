/**
 * @namespace Intake.Case
 */
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
        var _insuranceAndBenefitsTabId = "InsuranceInformation";
        var _secondaryCarrierSectionId = "SecondaryCarrierSection";
        var _secondaryCarrierAuthSectionId = "SecondaryCarrierAuthorization";
        var CaseStages;
        (function (CaseStages) {
            CaseStages["Intake"] = "fd6ad6ed-19fa-40c0-8995-5b768c63725a";
            CaseStages["Authorizations"] = "6485894b-12ed-41b3-a0ce-07abc11fb302";
            CaseStages["CaseManagement"] = "406f25d6-2503-4d55-a464-a95a1c8aaf13";
            CaseStages["Billing"] = "8c6cfada-32e1-4700-8efc-3c991b06f1f4";
            CaseStages["CarrierCollections"] = "008063c7-2746-43ce-b1cb-dca6e7620c3b";
            CaseStages["PatientCollections"] = "2fd06a7e-c253-4d57-9118-4a5c5d6954b5";
            CaseStages["Finance"] = "f4359293-fb63-4688-87ca-8d15f360049f";
        })(CaseStages || (CaseStages = {}));
        ;
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
        var PaymentPlanType;
        (function (PaymentPlanType) {
            PaymentPlanType[PaymentPlanType["None"] = 427880000] = "None";
            PaymentPlanType[PaymentPlanType["AutoDraft"] = 427880001] = "AutoDraft";
            PaymentPlanType[PaymentPlanType["Manual"] = 427880002] = "Manual";
        })(PaymentPlanType || (PaymentPlanType = {}));
        var CaseHoldReasonsOptionSetValues;
        (function (CaseHoldReasonsOptionSetValues) {
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["CollectionsHold"] = 11] = "CollectionsHold";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["FacilityRecoveryAPInvoiceIssued"] = 427880009] = "FacilityRecoveryAPInvoiceIssued";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["FacilityRecoveryDebitPending"] = 427880004] = "FacilityRecoveryDebitPending";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["FacilityRecoveryLetterSent"] = 427880001] = "FacilityRecoveryLetterSent";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["FacilityRecoveryManufacturerCreditPending"] = 427880005] = "FacilityRecoveryManufacturerCreditPending";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["FacilityRecoveryResearchApproved"] = 427880000] = "FacilityRecoveryResearchApproved";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["FacilityRecoveryResearchPending"] = 427880002] = "FacilityRecoveryResearchPending";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["FeeScheduleHold"] = 7] = "FeeScheduleHold";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["IssueHealthPlan"] = 8] = "IssueHealthPlan";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["IssueRebuttal"] = 9] = "IssueRebuttal";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["Other"] = 1] = "Other";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["PatientBankruptcy"] = 3] = "PatientBankruptcy";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["PatientLitigation"] = 2] = "PatientLitigation";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["PatientSettlementPending"] = 4] = "PatientSettlementPending";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["PendingFacilityContract"] = 5] = "PendingFacilityContract";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["PostClaimCorrections"] = 10] = "PostClaimCorrections";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["QueuedForBilling"] = 6] = "QueuedForBilling";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["SettlementPending"] = 12] = "SettlementPending";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["ManagerReview"] = 427880006] = "ManagerReview";
            CaseHoldReasonsOptionSetValues[CaseHoldReasonsOptionSetValues["PendingCourtesyClaimDocuments"] = 427880003] = "PendingCourtesyClaimDocuments";
        })(CaseHoldReasonsOptionSetValues || (CaseHoldReasonsOptionSetValues = {}));
        ;
        var CaseHoldReasonsLabels;
        (function (CaseHoldReasonsLabels) {
            CaseHoldReasonsLabels["CollectionsHold"] = "Collections Hold";
            CaseHoldReasonsLabels["FacilityRecoveryAPInvoiceIssued"] = "Facility Recovery - AP Invoice Issued";
            CaseHoldReasonsLabels["FacilityRecoveryDebitPending"] = "Facility Recovery - Debit Pending";
            CaseHoldReasonsLabels["FacilityRecoveryLetterSent"] = "Facility Recovery - Letter Sent";
            CaseHoldReasonsLabels["FacilityRecoveryManufacturerCreditPending"] = "Facility Recovery - Manufacturer Credit Pending";
            CaseHoldReasonsLabels["FacilityRecoveryResearchApproved"] = "Facility Recovery - Research Approved";
            CaseHoldReasonsLabels["FacilityRecoveryResearchPending"] = "Facility Recovery - Research Pending";
            CaseHoldReasonsLabels["FeeScheduleHold"] = "Fee Schedule Hold";
            CaseHoldReasonsLabels["IssueHealthPlan"] = "Issue - Health Plan";
            CaseHoldReasonsLabels["IssueRebuttal"] = "Issue - Rebuttal";
            CaseHoldReasonsLabels["Other"] = "Other";
            CaseHoldReasonsLabels["PatientBankruptcy"] = "Patient bankruptcy";
            CaseHoldReasonsLabels["PatientLitigation"] = "Patient Litigation";
            CaseHoldReasonsLabels["PatientSettlementPending"] = "Patient Settlement Pending";
            CaseHoldReasonsLabels["PendingFacilityContract"] = "Pending Facility Contract";
            CaseHoldReasonsLabels["PostClaimCorrections"] = "Post-Claim Corrections";
            CaseHoldReasonsLabels["QueuedForBilling"] = "Queued for Billing";
            CaseHoldReasonsLabels["SettlementPending"] = "Settlement Pending";
            CaseHoldReasonsLabels["ManagerReview"] = "Manager Review";
            CaseHoldReasonsLabels["PendingCourtesyClaimDocuments"] = "Pending Courtesy Claim Documents";
        })(CaseHoldReasonsLabels || (CaseHoldReasonsLabels = {}));
        var CaseAttributes;
        (function (CaseAttributes) {
            CaseAttributes["CarrierCoinsurance"] = "ipg_payercoinsurance";
            CaseAttributes["PatientCoinsurance"] = "ipg_patientcoinsurance";
        })(CaseAttributes || (CaseAttributes = {}));
        ;
        var _formContext;
        function OnFormLoad(executionContext) {
            var _a, _b, _c, _d;
            return __awaiter(this, void 0, void 0, function () {
                var formContext, caseState, lockCase, isStep2, lifeSycle, lsStep, carrierAttribute, cptAttributeNames, fieldsToLock, lastGateRunAttr, _i, cptAttributeNames_1, attrName, attr, caseStatusAttr, relationToInsuredAttr, focusOnTab;
                return __generator(this, function (_e) {
                    switch (_e.label) {
                        case 0:
                            console.log("OnLoadEvent fired");
                            formContext = executionContext.getFormContext();
                            caseState = formContext.getAttribute("ipg_statecode");
                            lockCase = formContext.getAttribute("ipg_islocked");
                            ConfigureOwnerField(formContext);
                            formContext.data.addOnLoad(RefreshOnDataLoad);
                            _formContext = formContext;
                            formContext.getControl("ipg_patientcity").setDisabled(true);
                            formContext.getControl("ipg_patientstate").setDisabled(true);
                            (_a = formContext.getAttribute("ipg_paymentplantype")) === null || _a === void 0 ? void 0 : _a.addOnChange(function () { return SetPaymentPlanAmountAsRequired(formContext); });
                            isStep2 = formContext.data.attributes.get("p_isStep2");
                            if (isStep2) {
                                showPIFStep2Form(executionContext);
                            }
                            lifeSycle = formContext.getAttribute("ipg_lifecyclestepid");
                            if (!(lifeSycle && lifeSycle.getValue())) return [3 /*break*/, 3];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord(lifeSycle.getValue()[0].entityType, lifeSycle.getValue()[0].id, "?$select=_ipg_gateconfigurationid_value")];
                        case 1:
                            lsStep = _e.sent();
                            if (!(lsStep && lsStep["_ipg_gateconfigurationid_value"])) return [3 /*break*/, 3];
                            return [4 /*yield*/, showTabsByGate(formContext, { entityType: "ipg_gateconfiguration", id: lsStep["_ipg_gateconfigurationid_value"], name: "" })];
                        case 2:
                            _e.sent();
                            _e.label = 3;
                        case 3:
                            showWarningMessageIfCaseOnHold(formContext);
                            addCarriersPreSearch(formContext);
                            carrierAttribute = formContext.getAttribute("ipg_carrierid");
                            carrierAttribute.addOnChange(function () { OnChangePrimaryCarrier(formContext); });
                            carrierAttribute.fireOnChange();
                            if (lifeSycle) {
                                lifeSycle.addOnChange(function () { return ShowAllReceivedField(formContext); });
                            }
                            return [4 /*yield*/, setPartsTabVisibility(formContext)];
                        case 4:
                            _e.sent();
                            caseState.addOnChange(function (event) { return showActualParts(event.getFormContext()); });
                            showActualParts(formContext);
                            ConfigureHeader(formContext);
                            ShowAllReceivedField(formContext);
                            disableBenefitTypeIfNeeded(formContext);
                            showHidePrimaryCarrierFields(formContext);
                            ShowSecondaryCarrierSection(executionContext);
                            showHideCaseHoldReasonOptions(formContext);
                            IfCarrierIsAutoHideBenefitsExhausted(formContext);
                            showHideCaseHoldReason(formContext);
                            AddValidation(formContext);
                            SetRequiredField(formContext);
                            LockFieldOnNotCaseSummary(formContext);
                            setRequiredActualProcedureDate(formContext);
                            HideObsoleteOrDeletedFields(formContext);
                            SetBpfStageBasedOnCaseState(formContext);
                            cptAttributeNames = ["ipg_cptcodeid1",
                                "ipg_cptcodeid2",
                                "ipg_cptcodeid3",
                                "ipg_cptcodeid4",
                                "ipg_cptcodeid5",
                                "ipg_cptcodeid6"];
                            if (lockCase) {
                                fieldsToLock = ["ipg_patientid", "ipg_carrierid", "ipg_secondarycarrierid",
                                    "ipg_memberidnumber", "ipg_procedureid",
                                    "ipg_cptcodename1", "ipg_cptcodeid1", "ipg_cptcodeid2", "ipg_cptcodeid3",
                                    "ipg_cptcodeid4", "ipg_cptcodeid5", "ipg_cptcodeid6",
                                    "ipg_actualdos", "ipg_facilityid", "ipg_physicianid",
                                    "ipg_dxcodeid1", "ipg_dxcodeid2", "ipg_dxcodeid3",
                                    "ipg_dxcodeid4", "ipg_dxcodeid5", "ipg_dxcodeid6",
                                    "ipg_patientfirstname", "ipg_patientfullname", "ipg_patientlastname",
                                    "ipg_patientmiddlename", "ipg_insuredfirstname", "ipg_insuredlastname",
                                    "ipg_insuredmiddlename", "ipg_insureddateofbirth", "ipg_insuredgender",
                                    "ipg_relationtoinsured", "ipg_secondarycarrierrelationtoinsured",
                                    "ipg_patientgender", "ipg_patientdateofbirth", "ipg_surgerydate",
                                    "ipg_homeplancarrierid", "ipg_accurate_equipment_list_received",
                                    "ipg_isallreceiveddate1", "ipg_secondarymemberidnumber",
                                    "ipg_patientzipcodeidselector", "ownerid", "ipg_primarycarephysicianid", "ipg_casemanagerid",
                                    "ipg_facility_auth2", "ipg_autodateofincident", "ipg_autoadjustername",
                                    "ipg_nursecasemgrname", "ipg_nursecasemgrphone", "ipg_is_authorization_required", "ipg_autocarrier"];
                                showLockCaseNotification(lockCase.getValue());
                                setFieldsLock(fieldsToLock, lockCase.getValue());
                            }
                            setRequiredSurgicalPhysicianNameAttribute(formContext);
                            setReqiredPrimaryCPTAndPrimaryDX(formContext);
                            lastGateRunAttr = formContext.getAttribute("ipg_lastgaterunid");
                            lastGateRunAttr.addOnChange(function () {
                                setRequiredSurgicalPhysicianNameAttribute(formContext);
                                setReqiredPrimaryCPTAndPrimaryDX(formContext);
                            });
                            setRequiredBilledCPT(formContext);
                            for (_i = 0, cptAttributeNames_1 = cptAttributeNames; _i < cptAttributeNames_1.length; _i++) {
                                attrName = cptAttributeNames_1[_i];
                                attr = formContext.getAttribute(attrName);
                                attr.addOnChange(function () { setRequiredBilledCPT(formContext); });
                            }
                            addPhysicianLookupCustomView(formContext);
                            caseStatusAttr = formContext.getAttribute("ipg_casestatus");
                            caseStatusAttr === null || caseStatusAttr === void 0 ? void 0 : caseStatusAttr.addOnChange(function () {
                                lockAndSetFormNotificationIfClosed(formContext);
                                DisplayOwnerField(formContext);
                                setVisibilityForCaseAuthButtons(formContext);
                            });
                            caseStatusAttr === null || caseStatusAttr === void 0 ? void 0 : caseStatusAttr.fireOnChange();
                            (_b = formContext.getAttribute(CaseAttributes.CarrierCoinsurance)) === null || _b === void 0 ? void 0 : _b.addOnChange(function () { return OnCoinsuranceChange(formContext); });
                            (_c = formContext.getAttribute(CaseAttributes.PatientCoinsurance)) === null || _c === void 0 ? void 0 : _c.addOnChange(function () { return OnCoinsuranceChange(formContext); });
                            relationToInsuredAttr = formContext.getAttribute("ipg_relationtoinsured");
                            relationToInsuredAttr === null || relationToInsuredAttr === void 0 ? void 0 : relationToInsuredAttr.addOnChange(function () {
                                lockInsuredFields(relationToInsuredAttr, lockCase);
                            });
                            relationToInsuredAttr === null || relationToInsuredAttr === void 0 ? void 0 : relationToInsuredAttr.fireOnChange();
                            lockCase === null || lockCase === void 0 ? void 0 : lockCase.addOnChange(function () {
                                lockInsuredFields(relationToInsuredAttr, lockCase);
                            });
                            CalculateActualTotalResponsibility(executionContext);
                            CalculateTotalReceived(executionContext);
                            CalculateTotalAdjustments(executionContext);
                            CalculateTotalWriteOff(executionContext);
                            CalculateTotalBalance(executionContext);
                            focusOnTab = window.localStorage.getItem('focusOnTab');
                            if (focusOnTab != null) {
                                Xrm.Page.ui.tabs.get(focusOnTab).setFocus();
                                window.localStorage.removeItem('focusOnTab');
                                window.localStorage.removeItem('back');
                            }
                            (_d = formContext.getAttribute("ipg_actualdos")) === null || _d === void 0 ? void 0 : _d.fireOnChange();
                            return [2 /*return*/];
                    }
                });
            });
        }
        Case.OnFormLoad = OnFormLoad;
        function OnCoinsuranceChange(formContext) {
            var _a, _b;
            var carrierCoinsurance = (_a = formContext.getAttribute(CaseAttributes.CarrierCoinsurance)) === null || _a === void 0 ? void 0 : _a.getValue();
            var patientCoinsurance = (_b = formContext.getAttribute(CaseAttributes.PatientCoinsurance)) === null || _b === void 0 ? void 0 : _b.getValue();
            var carrierNotificationId = "CoinsuranceCarrierNotification";
            var patientNotificationId = "CoinsurancePatientNotification";
            var isCoinsuranceValid = function () { return carrierCoinsurance + patientCoinsurance <= 100; };
            var showSumValidationMessages = function () {
                formContext.getControl(CaseAttributes.CarrierCoinsurance).setNotification("The sum of Patient Coinsurance and Carrier Coinsurance can not be more than 100", carrierNotificationId);
                formContext.getControl(CaseAttributes.PatientCoinsurance).setNotification("The sum of Patient Coinsurance and Carrier Coinsurance can not be more than 100", patientNotificationId);
            };
            var hideSumValidationMessages = function () {
                formContext.getControl(CaseAttributes.CarrierCoinsurance).clearNotification(carrierNotificationId);
                formContext.getControl(CaseAttributes.PatientCoinsurance).clearNotification(patientNotificationId);
            };
            !isCoinsuranceValid() ? showSumValidationMessages() : hideSumValidationMessages();
        }
        function lockAndSetFormNotificationIfClosed(formContext) {
            var _a;
            var status = (_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (status == 923720001) {
                formContext.ui.setFormNotification("Case is Closed", "WARNING", "closed");
                formContext.ui.tabs.forEach(function (tab) {
                    tab.setVisible(true);
                });
                formContext.ui.controls.forEach(function (control) {
                    if (control && control.getDisabled && !control.getDisabled()) {
                        control.setDisabled(true);
                    }
                });
            }
            else {
                formContext.ui.clearFormNotification("closed");
            }
        }
        function addCarriersPreSearch(anyContext) {
            var _a, _b;
            var formContext = typeof (anyContext.getFormContext) == 'function' ? anyContext.getFormContext() : anyContext;
            (_a = formContext.getControl("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.addPreSearch(function () {
                addPrimaryCarrierFilter(formContext);
            });
            (_b = formContext.getControl("ipg_secondarycarrierid")) === null || _b === void 0 ? void 0 : _b.addPreSearch(function () {
                addSecondaryCarrierFilter(formContext);
            });
        }
        Case.addCarriersPreSearch = addCarriersPreSearch;
        function addPrimaryCarrierFilter(formContext) {
            var filter = "<filter type='and'>\n    <condition attribute='ipg_carriertype' operator='not-in'>\n      <value>" + 427880000 /* Auto */ + "</value>\n      <value>" + 923720006 /* Government */ + "</value>\n      <value>" + 427880003 /* IPA */ + "</value>\n    </condition>\n    <condition attribute=\"ipg_carrieraccepted\" operator=\"eq\" value=\"1\" />\n  </filter>";
            formContext.getControl("ipg_carrierid").addCustomFilter(filter, "account");
        }
        function addSecondaryCarrierFilter(formContext) {
            var filter = "<filter type='and'>\n    <condition attribute='ipg_carriertype' operator='not-in'>\n    <value>" + 427880003 /* IPA */ + "</value>\n    <value>" + 427880001 /* WorkersComp */ + "</value>\n    </condition>\n  </filter>";
            formContext.getControl("ipg_secondarycarrierid").addCustomFilter(filter, "account");
        }
        function OnChangePrimaryCarrier(anyContext) {
            var _a;
            var formContext = typeof (anyContext.getFormContext) == 'function' ? anyContext.getFormContext() : anyContext;
            var carrierVal = (_a = formContext.getAttribute("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (carrierVal && carrierVal.length > 0) {
                Xrm.WebApi.retrieveRecord(carrierVal[0].entityType, carrierVal[0].id, "?$select=ipg_carriertype")
                    .then(function (carrier) {
                    var _a, _b, _c, _d, _e, _f, _g;
                    if (carrier["ipg_carriertype"] == 427880001 /* WorkersComp */) //Workers Comp
                     {
                        if (IsSecondaryCarrierPopulated(formContext)) {
                            (_a = formContext.getAttribute("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.setValue(null);
                            Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "Please remove secondary carrier before set selected carrier as primary" });
                        }
                        else {
                            (_b = formContext.getAttribute("ipg_secondarycarrierid")) === null || _b === void 0 ? void 0 : _b.setValue(null);
                            (_c = formContext.getControl("ipg_secondarycarrierid")) === null || _c === void 0 ? void 0 : _c.setDisabled(true);
                            (_d = formContext.getAttribute("ipg_secondarymemberidnumber")) === null || _d === void 0 ? void 0 : _d.setValue("");
                            (_e = formContext.getControl("ipg_secondarymemberidnumber")) === null || _e === void 0 ? void 0 : _e.setDisabled(true);
                        }
                        DisplayAddCarrierButton(formContext, 'none');
                    }
                    else {
                        (_f = formContext.getControl("ipg_secondarycarrierid")) === null || _f === void 0 ? void 0 : _f.setDisabled(false);
                        (_g = formContext.getControl("ipg_secondarymemberidnumber")) === null || _g === void 0 ? void 0 : _g.setDisabled(false);
                        DisplayAddCarrierButton(formContext, 'inline');
                    }
                });
            }
        }
        function IsSecondaryCarrierPopulated(formContext) {
            var _a;
            return !!((_a = formContext.getAttribute("ipg_secondarycarrierid")) === null || _a === void 0 ? void 0 : _a.getValue());
        }
        function DisplayAddCarrierButton(context, mode) {
            var content = context
                .getControl("WebResource_carriersfilter")
                .getObject();
            if (content) {
                content.contentWindow.document.getElementById("addCarrierButton").style.display = mode;
            }
        }
        function LockFieldOnNotCaseSummary(formContext) {
            var _this = this;
            var modifielableFields = ["ipg_facilityid",
                "ipg_cptcodeid",
                "ipg_billedcptid",
                "ipg_patientfirstname",
                "ipg_patientmiddlename",
                "ipg_patientlastname",
                "ipg_patientsuffix",
                "ipg_patientdateofbirth",
                "ipg_patientgender",
                "ipg_patientemail",
                "ipg_patientaddress",
                "ipg_patientzipcodeid",
                "ipg_patienthomephone",
                "ipg_patientworkphone",
                "ipg_patientcellphone",
                "ipg_procedureid",
                "ipg_surgerydate",
                "ipg_actualdos",
                "ipg_facilityid",
                "ipg_physicianid",
                "ipg_casemgrnameid",
                "ipg_iscourtesyclaimcase1"];
            var onTabStateChange = function (eventContext) {
                var tab = eventContext.getEventSource();
                if (tab.getDisplayState() == 'expanded' && tab.getName() != "PatientProcedureDetails") {
                    tab.sections.forEach(function (section) {
                        section.controls.forEach(function (ctr) { return __awaiter(_this, void 0, void 0, function () {
                            var attrName_1;
                            var _a, _b;
                            return __generator(this, function (_c) {
                                switch (_c.label) {
                                    case 0:
                                        if (!((_a = ctr) === null || _a === void 0 ? void 0 : _a.getAttribute)) return [3 /*break*/, 3];
                                        attrName_1 = (_b = ctr.getAttribute()) === null || _b === void 0 ? void 0 : _b.getName();
                                        if (!(attrName_1 === "ipg_iscourtesyclaimcase1")) return [3 /*break*/, 2];
                                        return [4 /*yield*/, procesCourtesyCaseFieldAccess(formContext)];
                                    case 1:
                                        _c.sent();
                                        return [3 /*break*/, 3];
                                    case 2:
                                        if (modifielableFields.find(function (mf) { return mf == attrName_1; })) {
                                            ctr.setDisabled(true);
                                        }
                                        _c.label = 3;
                                    case 3: return [2 /*return*/];
                                }
                            });
                        }); });
                    });
                }
            };
            formContext.ui.tabs.forEach(function (tab) {
                tab.addTabStateChange(onTabStateChange);
            });
        }
        function HideObsoleteOrDeletedFields(formContext) {
            var obsoleteFields = ["ipg_cptcategorycode",
                "ipg_billedcptid",
                "ipg_casemanagerid",
                "ipg_cmassignedid",
                "ipg_facilitycimid",
                "ipg_facilitymddid",
                "ipg_carriernetwork"
            ];
            obsoleteFields.forEach(function (field) { var _a; return (_a = formContext.getControl(field)) === null || _a === void 0 ? void 0 : _a.setVisible(false); });
        }
        function DisplayOwnerField(formContext) {
            var ownerControl = formContext.getControl("ownerid");
            if (isCaseOpened(formContext)) {
                ownerControl === null || ownerControl === void 0 ? void 0 : ownerControl.setDisabled(false);
            }
            else {
                ownerControl === null || ownerControl === void 0 ? void 0 : ownerControl.setDisabled(true);
            }
        }
        function isCaseOpened(formContext) {
            var _a;
            return ((_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue()) == 923720000; // Opened
        }
        function SetRequiredField(formContext) {
            var requiredField = ["ipg_patientfirstname",
                "ipg_patientlastname",
                "ipg_patientdateofbirth",
                "ipg_patientgender",
                "ipg_patientaddress",
                "ipg_patientzipcodeid",
                "ipg_procedureid",
                "ipg_surgerydate",
                "ipg_facilityid",
                "ipg_physicianid",
                "ipg_casemgrnameid",
                "ownerid",
                "ipg_iscourtesyclaimcase"];
            requiredField.forEach(function (field) { var _a; return (_a = formContext.getAttribute(field)) === null || _a === void 0 ? void 0 : _a.setRequiredLevel('required'); });
        }
        function setDisabledField(formContext) {
            var disabledField = ["ipg_iscourtesyclaimcase"];
            disabledField.forEach(function (field) { var _a; return (_a = formContext.getControl(field)) === null || _a === void 0 ? void 0 : _a.setDisabled(true); });
        }
        function ConfigureHeader(formContext) {
            var actDOSattr = formContext.getAttribute("ipg_actualdos");
            actDOSattr === null || actDOSattr === void 0 ? void 0 : actDOSattr.addOnChange(SetVisabilityOfProcedureDate);
            actDOSattr === null || actDOSattr === void 0 ? void 0 : actDOSattr.fireOnChange();
            var firstname = formContext.getAttribute("ipg_patientfirstname");
            var mi = formContext.getAttribute("ipg_patientmiddlename");
            var lastname = formContext.getAttribute("ipg_patientlastname");
            firstname === null || firstname === void 0 ? void 0 : firstname.addOnChange(CalculatePatientStatementOnHeader);
            mi === null || mi === void 0 ? void 0 : mi.addOnChange(CalculatePatientStatementOnHeader);
            lastname === null || lastname === void 0 ? void 0 : lastname.addOnChange(CalculatePatientStatementOnHeader);
            firstname === null || firstname === void 0 ? void 0 : firstname.fireOnChange();
        }
        function setPartsTabVisibility(formContext) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var caseid, partsTab, estimatedParts;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            caseid = formContext.data.entity.getId();
                            partsTab = formContext.ui.tabs.get("PartsTab");
                            if (partsTab) {
                                partsTab.setVisible(true);
                            }
                            if (!caseid) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_estimatedcasepartdetail", "?$select=ipg_estimatedcasepartdetailid&$filter=_ipg_caseid_value eq " + caseid + " and ipg_purchaseorderid ne null")];
                        case 1:
                            estimatedParts = _b.sent();
                            if (estimatedParts.entities.length > 0) {
                                (_a = formContext.ui.tabs.get("Orders")) === null || _a === void 0 ? void 0 : _a.setVisible(true);
                            }
                            _b.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            });
        }
        function showHideCaseHoldReason(formContext) {
            var HoldAttr = formContext.getAttribute("ipg_casehold");
            HoldAttr === null || HoldAttr === void 0 ? void 0 : HoldAttr.addOnChange(function (eventContext) {
                var formContext = eventContext.getFormContext();
                var caseOnHoldAttr = formContext.getAttribute('ipg_casehold');
                var HoldReasonCtr = formContext.getControl("header_ipg_caseholdreason");
                if (caseOnHoldAttr === null || caseOnHoldAttr === void 0 ? void 0 : caseOnHoldAttr.getValue()) {
                    HoldReasonCtr === null || HoldReasonCtr === void 0 ? void 0 : HoldReasonCtr.setVisible(true);
                }
                else {
                    HoldReasonCtr === null || HoldReasonCtr === void 0 ? void 0 : HoldReasonCtr.setVisible(false);
                }
                showWarningMessageIfCaseOnHold(formContext);
            });
            HoldAttr === null || HoldAttr === void 0 ? void 0 : HoldAttr.fireOnChange();
        }
        function OnPlanTypeChange(executionContext) {
            var formContext = executionContext.getFormContext();
            if (formContext) {
                showHidePrimaryCarrierFields(formContext);
            }
        }
        Case.OnPlanTypeChange = OnPlanTypeChange;
        function ShowSecondaryCarrierSection(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            var isSecondaryCarrier = (_a = formContext.getAttribute("ipg_secondarycarrier")) === null || _a === void 0 ? void 0 : _a.getValue();
            setSectionVisible(formContext, _insuranceAndBenefitsTabId, _secondaryCarrierSectionId, isSecondaryCarrier);
            setSectionVisible(formContext, _insuranceAndBenefitsTabId, _secondaryCarrierAuthSectionId, isSecondaryCarrier);
        }
        Case.ShowSecondaryCarrierSection = ShowSecondaryCarrierSection;
        function SetPaymentPlanAmountAsRequired(formContext) {
            var paymentPlanType = formContext.getAttribute("ipg_paymentplantype");
            var paymentPlanAmount = formContext.getAttribute("ipg_paymentplanamount");
            if ((paymentPlanType === null || paymentPlanType === void 0 ? void 0 : paymentPlanType.getValue()) == PaymentPlanType.None) {
                paymentPlanAmount.setRequiredLevel("none");
            }
            else {
                paymentPlanAmount.setRequiredLevel("required");
            }
        }
        Case.SetPaymentPlanAmountAsRequired = SetPaymentPlanAmountAsRequired;
        function ShowAllReceivedField(formContext) {
            var lifecycleValue = formContext.getAttribute("ipg_lifecyclestepid")
                ? formContext.getAttribute("ipg_lifecyclestepid").getValue() : null;
            var lifecycleValueId = lifecycleValue ? lifecycleValue[0].id.replace(/[{}]/g, "").toLowerCase() : null;
            if (!lifecycleValueId) {
                return;
            }
            var settingName = 'Case.LifecycleStepsForAllReceivedField';
            Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", "?$select=ipg_value&$filter=ipg_name eq '" + settingName + "'")
                .then(function (result) {
                if (result.entities.length > 0 && result.entities[0].ipg_value) {
                    var lifecycleSteps = String(result.entities[0].ipg_value).split(";");
                    //formContext.getControl("ipg_isallreceived").setVisible(lifecycleSteps.indexOf(lifecycleValueId) !== -1);
                }
            }, function (error) {
                console.error(error.message);
            });
        }
        function OnSave(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            var caseStatus = (_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (caseStatus && caseStatus == 923720001) {
                if (executionContext.getEventArgs().getSaveMode() == 70 || executionContext.getEventArgs().getSaveMode() == 2) {
                    executionContext.getEventArgs().preventDefault();
                    return;
                }
                executionContext.getEventArgs().preventDefault();
                Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "You are not able to save closed case." }).then(function () {
                    return;
                });
            }
            var casePartDetailsControl = formContext.getControl("ActualParts");
            if (casePartDetailsControl) {
                casePartDetailsControl.refreshRibbon();
            }
            var estimatedPartsControl = formContext.getControl("EstimatedParts");
            if (estimatedPartsControl) {
                estimatedPartsControl.refreshRibbon();
            }
            var purchaseOrderDetailsControl = formContext.getControl("PurchaseOrders");
            if (purchaseOrderDetailsControl) {
                purchaseOrderDetailsControl.refreshRibbon();
            }
            setRequiredActualProcedureDate(formContext);
            SetRequiredField(formContext);
            setDisabledField(formContext);
        }
        Case.OnSave = OnSave;
        function AddValidation(formContext) {
            var _a, _b;
            (_a = formContext.getAttribute("ipg_facilitymrn")) === null || _a === void 0 ? void 0 : _a.addOnChange(AllowOnlyDigit(25));
            (_b = formContext.getAttribute("ipg_externalid")) === null || _b === void 0 ? void 0 : _b.addOnChange(AllowOnlyDigit());
        }
        function AllowOnlyDigit(length) {
            var pattern = new RegExp("^[\\d]" + (length ? "{1," + length + "}" : "*") + "$");
            return function (getEventSource) { return validateAttributeByRegExp(getEventSource.getEventSource(), pattern, "Should be numeric characters!"); };
        }
        function validateAttributeByRegExp(attr, expression, messages) {
            var currentValue = attr.getValue();
            var control = attr.controls.get()[0];
            var notifactionid = "validationError";
            if (!expression.test(currentValue)) {
                control.setNotification(messages, notifactionid);
            }
            else {
                control.clearNotification(notifactionid);
            }
        }
        function SetCasePartTabVisibility() {
            var formContext = Xrm.Page;
            //if Case currently in Authorization and procedure is scheduled in the future show parts tab to allow add TPO Part
            var lifecycleValue = formContext.getAttribute("ipg_lifecyclestepid") ? formContext.getAttribute("ipg_lifecyclestepid").getValue() : null;
            var DOSValue = formContext.getAttribute("ipg_surgerydate") ? formContext.getAttribute("ipg_surgerydate").getValue() : null;
            if (lifecycleValue && lifecycleValue.length > 0
                && lifecycleValue[0].name === "Authorization" && DOSValue && DOSValue.setHours(0, 0, 0, 0) > new Date().setHours(0, 0, 0, 0)) {
                var parttab = formContext.ui.tabs.get("PartsTab");
                if (parttab) {
                    parttab.setVisible(true);
                }
            }
        }
        function showPIFStep2Form(executionContext) {
            var formContext = executionContext.getFormContext();
            var visibleTabs = ["ReferralData", "PatientProcedureDetails", "InsuranceInformation", "Documents", "DebugView", "EventLog"];
            formContext.ui.tabs.forEach(function (tab) {
                if (visibleTabs.indexOf(tab.getName()) === -1) {
                    tab.setVisible(false);
                }
                if (tab.getName() === "PatientProcedureDetails") {
                    var purchaseOrderProductsSection = tab.sections.get("PurchaseOrderProducts");
                    if (purchaseOrderProductsSection) {
                        purchaseOrderProductsSection.setVisible(false);
                    }
                }
            });
        }
        function showTabsByGate(formContext, gateConfig) {
            return __awaiter(this, void 0, void 0, function () {
                var result, visibleTabs_1, eventLogTab;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord(gateConfig.entityType, gateConfig.id, "?$select=ipg_visibletabsoncaseform")];
                        case 1:
                            result = _a.sent();
                            if (result && result.ipg_visibletabsoncaseform) {
                                visibleTabs_1 = JSON.parse(result.ipg_visibletabsoncaseform);
                                formContext.ui.tabs.forEach(function (tab) {
                                    var tabIsVisible = visibleTabs_1.indexOf(tab.getName()) !== -1;
                                    if (tabIsVisible) {
                                        tab.setVisible(true);
                                    }
                                    else {
                                        tab.setVisible(false);
                                    }
                                });
                            }
                            else {
                                formContext.ui.tabs.forEach(function (tab) {
                                    tab.setVisible(true);
                                });
                            }
                            eventLogTab = formContext.ui.tabs.get("EventLog");
                            if (eventLogTab) {
                                eventLogTab.setVisible(true);
                            }
                            setPartsTabVisibility(formContext);
                            return [2 /*return*/];
                    }
                });
            });
        }
        Case.showTabsByGate = showTabsByGate;
        function disableBenefitTypeIfNeeded(formContext) {
            var stateCodeAttribute = formContext.getAttribute('ipg_statecode');
            if (stateCodeAttribute) {
                var intakeStateCodeValue = 923720000;
                var authorizationStateCodeValue = 923720001;
                var stateCodeValue = stateCodeAttribute.getValue();
                if (stateCodeValue && stateCodeValue != intakeStateCodeValue
                    && stateCodeValue != authorizationStateCodeValue) {
                    var useDmeControl = formContext.getControl('ipg_usedmebenefits');
                    if (useDmeControl) {
                        useDmeControl.setDisabled(true);
                    }
                    var individualFamilyControl = formContext.getControl('ipg_individualorfamilybenefits');
                    if (individualFamilyControl) {
                        individualFamilyControl.setDisabled(true);
                    }
                    var inOutNetworkControl = formContext.getControl('ipg_inoutnetwork');
                    if (inOutNetworkControl) {
                        inOutNetworkControl.setDisabled(true);
                    }
                }
            }
        }
        function showHidePrimaryCarrierFields(formContext) {
            var autoNoFaultPlanTypeValue = 923720008;
            var workersCompPlanTypeValue = 923720004;
            var planTypeAttribute = formContext.getAttribute('ipg_primarycarrierplantype');
            if (planTypeAttribute) {
                var planTypeValue = planTypeAttribute.getValue();
                var isAutoOrWorkersComp = planTypeValue == autoNoFaultPlanTypeValue
                    || planTypeValue == workersCompPlanTypeValue;
                var tabId = 'InsuranceInformation';
                var sectionId = 'PrimaryCarrierSection';
                //show these fields
                setControlVisible(formContext, tabId, sectionId, "ipg_autoclaimnumber", isAutoOrWorkersComp);
                setControlVisible(formContext, tabId, sectionId, "ipg_autodateofincident", isAutoOrWorkersComp);
                setControlVisible(formContext, tabId, sectionId, "ipg_autoadjustername", isAutoOrWorkersComp);
                setControlVisible(formContext, tabId, sectionId, "ipg_adjusterphone", isAutoOrWorkersComp);
                setControlVisible(formContext, tabId, sectionId, "ipg_billingfax", isAutoOrWorkersComp);
                //hide these fields
                setControlVisible(formContext, tabId, sectionId, "ipg_memberidnumber", isAutoOrWorkersComp == false);
                setControlVisible(formContext, tabId, sectionId, "ipg_primarycarriergroupidnumber", isAutoOrWorkersComp == false);
                setControlVisible(formContext, tabId, sectionId, "ipg_homeplancarrierid", isAutoOrWorkersComp == false);
                setControlVisible(formContext, tabId, sectionId, "ipg_ipapayerid", isAutoOrWorkersComp == false);
                setControlVisible(formContext, tabId, sectionId, "ipg_plansponsor", isAutoOrWorkersComp == false);
                setControlVisible(formContext, tabId, sectionId, "ipg_primarycarriereffectivedate", isAutoOrWorkersComp == false);
                setControlVisible(formContext, tabId, sectionId, "ipg_primarycarrierexpirationdate", isAutoOrWorkersComp == false);
            }
        }
        function setSectionVisible(formContext, tabId, sectionId, isVisible) {
            var tab = formContext.ui.tabs.get(tabId);
            if (tab && tab.sections && tab.sections.getLength() > 0) {
                var section = tab.sections.get(sectionId);
                if (section && section.setVisible) {
                    section.setVisible(isVisible);
                }
            }
        }
        function setControlVisible(formContext, tabId, sectionId, fieldName, isVisible) {
            var tabObj = formContext.ui.tabs.get(tabId);
            if (tabObj) {
                var sectionObj = tabObj.sections.get(sectionId);
                if (sectionObj) {
                    var attribute = formContext.getAttribute(fieldName);
                    if (attribute) {
                        var attributeControls = attribute.controls.get();
                        for (var _i = 0, attributeControls_1 = attributeControls; _i < attributeControls_1.length; _i++) {
                            var c = attributeControls_1[_i];
                            var sectionControl = sectionObj.controls.get(c.getName());
                            if (sectionControl) {
                                sectionControl.setVisible(isVisible);
                            }
                        }
                    }
                }
            }
        }
        function OnCaseStateChange(executionContext) {
            var formContext = executionContext.getFormContext();
            if (formContext) {
                showHideCaseHoldReasonOptions(formContext);
                setPartsTabVisibility(formContext);
                SetBpfStageBasedOnCaseState(formContext);
            }
        }
        Case.OnCaseStateChange = OnCaseStateChange;
        function showHideCaseHoldReasonOptions(formContext) {
            //Parent optionset value
            var parCaseStateCode = formContext.getAttribute('ipg_statecode');
            //Child optionset control/options
            var chCaseStateControl = formContext.getControl('ipg_caseholdreason');
            var chCaseStateControlOptions = chCaseStateControl.getAttribute().getOptions();
            if (parCaseStateCode) {
                var parCaseStateCodeValue = parCaseStateCode.getValue();
                if (parCaseStateCodeValue == 923720001) {
                    //Authorization
                    chCaseStateControl.clearOptions();
                    for (var i = 1; i < chCaseStateControlOptions.length; i++) {
                        if (chCaseStateControlOptions[i].value == 1 || chCaseStateControlOptions[i].value == 5) {
                            chCaseStateControl.addOption(chCaseStateControlOptions[i]);
                        }
                    }
                }
                else if (parCaseStateCodeValue == 923720002) {
                    //Case Management
                    chCaseStateControl.clearOptions();
                    for (var i = 1; i < chCaseStateControlOptions.length; i++) {
                        if (chCaseStateControlOptions[i].value == 1 || chCaseStateControlOptions[i].value == 6 || chCaseStateControlOptions[i].value == 7) {
                            chCaseStateControl.addOption(chCaseStateControlOptions[i]);
                        }
                    }
                }
                else if (parCaseStateCodeValue == 923720004) {
                    //Carrier Services
                    chCaseStateControl.clearOptions();
                    for (var i = 1; i < chCaseStateControlOptions.length; i++) {
                        if (chCaseStateControlOptions[i].value == 8 || chCaseStateControlOptions[i].value == 9 || chCaseStateControlOptions[i].value == 10 || chCaseStateControlOptions[i].value == 11) {
                            chCaseStateControl.addOption(chCaseStateControlOptions[i]);
                        }
                    }
                }
                else if (parCaseStateCodeValue == 923720005) {
                    //Patient Services
                    chCaseStateControl.clearOptions();
                    for (var i = 1; i < chCaseStateControlOptions.length; i++) {
                        if (chCaseStateControlOptions[i].value == 1 || chCaseStateControlOptions[i].value == 11 || chCaseStateControlOptions[i].value == 12) {
                            chCaseStateControl.addOption(chCaseStateControlOptions[i]);
                        }
                    }
                }
                else {
                    chCaseStateControl.clearOptions();
                }
            }
        }
        function IfCarrierIsAutoHideBenefitsExhausted(formContext) {
            var lkpCarrier = formContext.getAttribute("ipg_carrierid");
            if (lkpCarrier && lkpCarrier.getValue()) {
                var control_1 = formContext.getControl("ipg_medicalbenefitsexhausted");
                if (control_1) {
                    Xrm.WebApi.retrieveRecord("account", lkpCarrier.getValue()[0].id, "?$select=ipg_carriertype").then(function (result) {
                        if (result && result.ipg_carriertype) {
                            if (result.ipg_carriertype == 427880000) {
                                control_1.setVisible(true);
                            }
                            else {
                                control_1.setVisible(false);
                            }
                        }
                    });
                }
            }
        }
        function showLockCaseNotification(isLock) {
            var lockCaseNotificationId = "isLockedNotification";
            if (isLock) {
                _formContext.ui.setFormNotification("Case is Locked", "WARNING", lockCaseNotificationId);
            }
            else {
                _formContext.ui.clearFormNotification(lockCaseNotificationId);
            }
        }
        function setFieldsLock(fieldNames, isDisabled) {
            var _a, _b;
            if (!fieldNames)
                return;
            isDisabled = isDisabled !== null && isDisabled !== void 0 ? isDisabled : true;
            if (Array.isArray(fieldNames)) {
                fieldNames.forEach(function (fieldName) {
                    var _a, _b;
                    (_b = (_a = _formContext.getAttribute(fieldName)) === null || _a === void 0 ? void 0 : _a.controls) === null || _b === void 0 ? void 0 : _b.forEach(function (c) { return c.setDisabled(isDisabled); });
                });
            }
            else if (typeof fieldNames === "string") {
                (_b = (_a = _formContext.getAttribute(fieldNames)) === null || _a === void 0 ? void 0 : _a.controls) === null || _b === void 0 ? void 0 : _b.forEach(function (c) { return c.setDisabled(isDisabled); });
            }
        }
        function showProcedureDateInHeader(formContext) {
            var scheduledDateAttr = formContext.getAttribute("ipg_surgerydate");
            var headerScheduledDateControl = formContext.getControl("header_ipg_surgerydate");
            var actualDateAttr = formContext.getAttribute("ipg_actualdos");
            var headerActualDateControl = formContext.getControl("header_ipg_actualdos");
            if (scheduledDateAttr && actualDateAttr
                && headerScheduledDateControl && headerActualDateControl) {
                if (actualDateAttr.getValue()) {
                    headerActualDateControl.setVisible(true);
                    headerScheduledDateControl.setVisible(false);
                }
                else {
                    headerActualDateControl.setVisible(false);
                    headerScheduledDateControl.setVisible(true);
                }
            }
        }
        function setRequiredActualProcedureDate(formContext) {
            var actualDateAttr = formContext.getAttribute("ipg_actualdos");
            if (actualDateAttr) {
                Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=statecode eq 0 and ipg_caseid/incidentid eq " + formContext.data.entity.getId().replace(/{|}/g, ""))
                    .then(function success(result) {
                    if (result.entities.length > 0) {
                        actualDateAttr.setRequiredLevel("required");
                    }
                    else {
                        actualDateAttr.setRequiredLevel("none");
                    }
                }, function (error) {
                    console.log(error.message);
                });
            }
        }
        function callDeriveHomePlanAction(formContext) {
            var _a, _b, _c, _d, _e;
            var outputResult;
            var carrierid = (_a = formContext === null || formContext === void 0 ? void 0 : formContext.getAttribute("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (((_b = formContext === null || formContext === void 0 ? void 0 : formContext.getAttribute("ipg_memberidnumber")) === null || _b === void 0 ? void 0 : _b.getIsDirty()) || ((_c = formContext === null || formContext === void 0 ? void 0 : formContext.getAttribute("ipg_carrierid")) === null || _c === void 0 ? void 0 : _c.getIsDirty())) {
                var parameters = {
                    "Target": {
                        "incidentid": formContext.data.entity.getId(),
                        "@odata.type": "Microsoft.Dynamics.CRM." + formContext.data.entity.getEntityName()
                    },
                    "MemberIdNumber": (_d = formContext === null || formContext === void 0 ? void 0 : formContext.getAttribute("ipg_memberidnumber")) === null || _d === void 0 ? void 0 : _d.getValue(),
                    "CarrierRef": carrierid
                        ? {
                            "accountid": (_e = carrierid[0]) === null || _e === void 0 ? void 0 : _e.id,
                            "@odata.type": "Microsoft.Dynamics.CRM.account"
                        }
                        : null
                };
                callAction("ipg_IPGGatingDeriveHomePlanPlugin", parameters, false, function (results) {
                    if (!results.Succeeded && results.CaseNote)
                        outputResult = results.CaseNote;
                });
            }
            return outputResult;
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
        function SetVisabilityOfProcedureDate(executionContext) {
            var formContext = executionContext.getFormContext();
            var headerPrefix = "header_";
            var actDOSattr = formContext.getAttribute("ipg_actualdos");
            var actDOSheaderctr = formContext.getControl(headerPrefix + "ipg_actualdos");
            var scheduledDOSheaderctr = formContext.getControl(headerPrefix + "ipg_surgerydate");
            if (actDOSattr === null || actDOSattr === void 0 ? void 0 : actDOSattr.getValue()) {
                actDOSheaderctr === null || actDOSheaderctr === void 0 ? void 0 : actDOSheaderctr.setVisible(true);
                scheduledDOSheaderctr === null || scheduledDOSheaderctr === void 0 ? void 0 : scheduledDOSheaderctr.setVisible(false);
            }
            else {
                actDOSheaderctr === null || actDOSheaderctr === void 0 ? void 0 : actDOSheaderctr.setVisible(false);
                scheduledDOSheaderctr === null || scheduledDOSheaderctr === void 0 ? void 0 : scheduledDOSheaderctr.setVisible(true);
            }
        }
        function CalculatePatientStatementOnHeader(executionContext) {
            var _a, _b, _c;
            var formContext = executionContext.getFormContext();
            var calculatedField = formContext.getAttribute("ipg_patientnamecalculated");
            if (calculatedField) {
                calculatedField.setSubmitMode("never");
                var firstName = (_a = formContext.getAttribute("ipg_patientfirstname")) === null || _a === void 0 ? void 0 : _a.getValue();
                var mi = (_b = formContext.getAttribute("ipg_patientmiddlename")) === null || _b === void 0 ? void 0 : _b.getValue();
                var lastname = (_c = formContext.getAttribute("ipg_patientlastname")) === null || _c === void 0 ? void 0 : _c.getValue();
                var calculatedValue = "" + firstName + (mi ? ' ' + mi : '') + (lastname ? ' ' + lastname : '');
                if ((calculatedField === null || calculatedField === void 0 ? void 0 : calculatedField.getValue()) != calculatedValue) {
                    calculatedField === null || calculatedField === void 0 ? void 0 : calculatedField.setValue(calculatedValue);
                }
            }
        }
        function setRequiredSurgicalPhysicianNameAttribute(formContext) {
            //set required Surgical Physician Name attribute if next gate is "Gate 3"
            setRequiredFieldToPassGate(formContext, "ipg_physicianid", 3);
        }
        function setRequiredBilledCPT(formContext) {
            var billedCptAttr = formContext.getAttribute("ipg_billedcptid");
            var cptAttributeNames = ["ipg_cptcodeid1",
                "ipg_cptcodeid2",
                "ipg_cptcodeid3",
                "ipg_cptcodeid4",
                "ipg_cptcodeid5",
                "ipg_cptcodeid6"];
            if (!billedCptAttr)
                return;
            billedCptAttr.setRequiredLevel("none");
            for (var _i = 0, cptAttributeNames_2 = cptAttributeNames; _i < cptAttributeNames_2.length; _i++) {
                var attrName = cptAttributeNames_2[_i];
                var attr = formContext.getAttribute(attrName);
                if (attr) {
                    var cptValue = attr.getValue();
                    if (cptValue && cptValue.length > 0) {
                        billedCptAttr.setRequiredLevel("required");
                        return;
                    }
                }
            }
        }
        function setReqiredPrimaryCPTAndPrimaryDX(formContext) {
            //set required Primary CPT and Primary DX attributes if next gate is "Gate 6"
            setRequiredFieldToPassGate(formContext, "ipg_cptcodeid1", 6);
            setRequiredFieldToPassGate(formContext, "ipg_dxcodeid1", 6);
        }
        //common function to set field required to pass gates
        function setRequiredFieldToPassGate(formContext, attrName, gateNumber) {
            var gateConfigurationAttr = formContext.getAttribute("ipg_gateconfigurationid");
            if (gateConfigurationAttr && gateConfigurationAttr.getValue()) {
                var gateConfiguration = gateConfigurationAttr.getValue()[0];
                var attribute = formContext.getAttribute(attrName);
                if (attribute && gateConfiguration.name === ("Gate " + gateNumber)) {
                    attribute.setRequiredLevel("required");
                }
                else {
                    attribute.setRequiredLevel("none");
                }
            }
        }
        function addPhysicianLookupCustomView(formContext) {
            var _a;
            var physicianControl = formContext.getControl("ipg_physicianid");
            var primaryCarePhysicianControl = formContext.getControl("ipg_primarycarephysicianid");
            if (physicianControl) {
                var facilityRef = (_a = formContext.getAttribute("ipg_facilityid")) === null || _a === void 0 ? void 0 : _a.getValue();
                if (facilityRef && facilityRef.length > 0) {
                    var viewId = "00000000-0000-0000-00AA-000010001111";
                    var fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\">\n                          <entity name=\"contact\">\n                           <attribute name=\"fullname\" />\n                           <attribute name=\"contactid\" />\n                           <order attribute=\"fullname\" descending=\"false\" />\n                            <filter type=\"or\" >\n                              <condition attribute=\"ipg_currentfacilityid\" operator=\"eq\" value=\"" + facilityRef[0].id + "\" />\n                              <filter type=\"and\" >\n                                <condition entityname=\"facilityPhysician\" attribute=\"ipg_facilityid\" operator=\"eq\" value=\"" + facilityRef[0].id + "\" />\n                                <condition entityname=\"facilityPhysician\" attribute=\"ipg_status\" operator=\"eq\" value=\"1\" />\n                              </filter>\n                            </filter>\n                            <link-entity name=\"ipg_facilityphysician\" from=\"ipg_physicianid\" to=\"contactid\" link-type=\"outer\" alias=\"facilityPhysician\" />\n                          </entity>\n                        </fetch>";
                    var viewDisplayName = "Physicians";
                    var layoutXml = "<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'>\n      <row name='result' id='contactid'>\n      <cell name='fullname' width='300' />\n      </row>\n      </grid>";
                    physicianControl.addCustomView(viewId, 'contact', viewDisplayName, fetchXml, layoutXml, true);
                    primaryCarePhysicianControl === null || primaryCarePhysicianControl === void 0 ? void 0 : primaryCarePhysicianControl.addCustomView(viewId, 'contact', viewDisplayName, fetchXml, layoutXml, true);
                }
            }
        }
        /**
        * Called on Facility Name change
        * @function Intake.Case.OnFacilityLookupChange
        * @returns {void}
       */
        function OnFacilityLookupChange(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            (_a = formContext.getAttribute("ipg_physicianid")) === null || _a === void 0 ? void 0 : _a.setValue(null);
            addPhysicianLookupCustomView(formContext);
        }
        Case.OnFacilityLookupChange = OnFacilityLookupChange;
        function OnExpandCollectionTab(eventcontext) {
            if (eventcontext) {
                var tab = eventcontext.getEventSource();
                if (tab.getDisplayState() == 'expanded') {
                    var formContext = eventcontext.getFormContext();
                    var arSummary = formContext.getControl("WebResource_ARSummary");
                    if (arSummary) {
                        var obj = arSummary.getObject();
                        if (obj) {
                            var src = obj.src;
                            obj.src = "about:blank";
                            obj.src = src;
                        }
                    }
                }
            }
        }
        Case.OnExpandCollectionTab = OnExpandCollectionTab;
        function filterEventsLog(executionContext) {
            var formContext = executionContext.getFormContext();
            var gridContext = formContext.getControl("cases_event_log");
            if (gridContext) {
                var caseId = formContext.data.entity.getId().replace(/{|}/g, "");
                var filterXml = "<filter type='and'>" +
                    "<condition attribute='ipg_caseid' operator='eq' value='" + caseId + "'/>" +
                    "</filter>";
                gridContext.setFilterXml(filterXml);
                formContext.ui.controls.get("cases_event_log").refresh();
            }
        }
        Case.filterEventsLog = filterEventsLog;
        function lockInsuredFields(relationToInsuredAttr, lockCase) {
            var fieldsToLock = ["ipg_insuredfirstname", "ipg_insuredmiddlename", "ipg_insuredlastname",
                "ipg_insureddateofbirth", "ipg_insuredgender", "ipg_insuredaddress", "ipg_insuredzipcodeid",
                "ipg_insuredphone"];
            var relationToInsured = relationToInsuredAttr === null || relationToInsuredAttr === void 0 ? void 0 : relationToInsuredAttr.getValue();
            if (relationToInsured && relationToInsured == 100000000 && !(lockCase === null || lockCase === void 0 ? void 0 : lockCase.getValue())) {
                setFieldsLock(fieldsToLock, true);
            }
            else {
                setFieldsLock(fieldsToLock, false);
            }
        }
        function showWarningMessageIfCaseOnHold(formContext) {
            var _a;
            var caseHold = formContext.getAttribute("ipg_casehold");
            if (caseHold && caseHold.getValue()) {
                var caseHoldReason = (_a = formContext.getAttribute("ipg_caseholdreason")) === null || _a === void 0 ? void 0 : _a.getValue();
                var warningMessage = "Case on Hold - ";
                switch (caseHoldReason) {
                    case CaseHoldReasonsOptionSetValues.CollectionsHold:
                        warningMessage += CaseHoldReasonsLabels.CollectionsHold;
                        break;
                    case CaseHoldReasonsOptionSetValues.FacilityRecoveryAPInvoiceIssued:
                        warningMessage += CaseHoldReasonsLabels.FacilityRecoveryAPInvoiceIssued;
                        break;
                    case CaseHoldReasonsOptionSetValues.FacilityRecoveryDebitPending:
                        warningMessage += CaseHoldReasonsLabels.FacilityRecoveryDebitPending;
                        break;
                    case CaseHoldReasonsOptionSetValues.FacilityRecoveryLetterSent:
                        warningMessage += CaseHoldReasonsLabels.FacilityRecoveryLetterSent;
                        break;
                    case CaseHoldReasonsOptionSetValues.FacilityRecoveryManufacturerCreditPending:
                        warningMessage += CaseHoldReasonsLabels.FacilityRecoveryManufacturerCreditPending;
                        break;
                    case CaseHoldReasonsOptionSetValues.FacilityRecoveryResearchApproved:
                        warningMessage += CaseHoldReasonsLabels.FacilityRecoveryResearchApproved;
                        break;
                    case CaseHoldReasonsOptionSetValues.FacilityRecoveryResearchPending:
                        warningMessage += CaseHoldReasonsLabels.FacilityRecoveryResearchPending;
                        break;
                    case CaseHoldReasonsOptionSetValues.FeeScheduleHold:
                        warningMessage += CaseHoldReasonsLabels.FeeScheduleHold;
                        break;
                    case CaseHoldReasonsOptionSetValues.IssueHealthPlan:
                        warningMessage += CaseHoldReasonsLabels.IssueHealthPlan;
                        break;
                    case CaseHoldReasonsOptionSetValues.IssueRebuttal:
                        warningMessage += CaseHoldReasonsLabels.IssueRebuttal;
                        break;
                    case CaseHoldReasonsOptionSetValues.Other:
                        warningMessage += CaseHoldReasonsLabels.Other;
                        break;
                    case CaseHoldReasonsOptionSetValues.PatientBankruptcy:
                        warningMessage += CaseHoldReasonsLabels.PatientBankruptcy;
                        break;
                    case CaseHoldReasonsOptionSetValues.PatientLitigation:
                        warningMessage += CaseHoldReasonsLabels.PatientLitigation;
                        break;
                    case CaseHoldReasonsOptionSetValues.PatientSettlementPending:
                        warningMessage += CaseHoldReasonsLabels.PatientSettlementPending;
                        break;
                    case CaseHoldReasonsOptionSetValues.PendingFacilityContract:
                        warningMessage += CaseHoldReasonsLabels.PendingFacilityContract;
                        break;
                    case CaseHoldReasonsOptionSetValues.PostClaimCorrections:
                        warningMessage += CaseHoldReasonsLabels.PostClaimCorrections;
                        break;
                    case CaseHoldReasonsOptionSetValues.QueuedForBilling:
                        warningMessage += CaseHoldReasonsLabels.QueuedForBilling;
                        break;
                    case CaseHoldReasonsOptionSetValues.SettlementPending:
                        warningMessage += CaseHoldReasonsLabels.SettlementPending;
                        break;
                    case CaseHoldReasonsOptionSetValues.ManagerReview:
                        warningMessage += CaseHoldReasonsLabels.ManagerReview;
                        break;
                    case CaseHoldReasonsOptionSetValues.PendingCourtesyClaimDocuments:
                        warningMessage += CaseHoldReasonsLabels.PendingCourtesyClaimDocuments;
                        break;
                    default:
                        warningMessage = "Case on Hold.";
                        break;
                }
                formContext.ui.setFormNotification(warningMessage, "WARNING", "casehold");
            }
            else {
                formContext.ui.clearFormNotification("casehold");
            }
        }
        function OnCaseReasonsChange(executionContext) {
            var formContext = executionContext.getFormContext();
        }
        Case.OnCaseReasonsChange = OnCaseReasonsChange;
        function showActualParts(formContext) {
            var _a, _b;
            var caseState = (_a = formContext.getAttribute("ipg_statecode")) === null || _a === void 0 ? void 0 : _a.getValue();
            var actualPartsSections = (_b = formContext.ui.tabs.get("PartsTab")) === null || _b === void 0 ? void 0 : _b.sections.get("ActualPartSection");
            actualPartsSections === null || actualPartsSections === void 0 ? void 0 : actualPartsSections.setVisible(caseState !== CaseStates.Authorizations);
        }
        function setVisibilityForCaseAuthButtons(formContext) {
            var _a;
            var isVisible = ((_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue()) != 923720001;
            formContext.getControl("WebResource_authorizationbuttons").setVisible(isVisible);
            formContext.getControl("WebResource_ObtainAuthorizationsBtn").setVisible(isVisible);
        }
        function SetAllReceivedDateField(formContext) {
            var allRecievedControl = formContext.getControl("ipg_isallreceiveddate1");
            if (allRecievedControl) {
                var caseId = formContext.data.entity.getId().replace('{', '').replace('}', '');
                var fetchXml = "?fetchXml=<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                    "  <entity name='salesorder'>" +
                    "    <attribute name='salesorderid' />" +
                    "    <filter type='and'>" +
                    "      <condition attribute='ipg_caseid' operator='eq' uiname='yjyjuy' uitype='incident' value='" + caseId + "' />" +
                    "    </filter>" +
                    "    <link-entity name='ipg_casepartdetail' from='ipg_purchaseorderid' to='salesorderid' link-type='inner' alias='ae' />" +
                    "  </entity>" +
                    "</fetch>";
                Xrm.WebApi.retrieveMultipleRecords("salesorder", fetchXml).then(function (result) { return allRecievedControl.setDisabled(result.entities && result.entities.length > 0); }, function (error) { return console.log(error); });
            }
        }
        function ValidateAllReceivedDate(executionContext) {
            var formContext = executionContext.getFormContext();
            var dateReceived = formContext.getAttribute("ipg_isallreceiveddate").getValue();
            var currentDate = new Date();
            if (dateReceived && dateReceived.valueOf() - currentDate.valueOf() > 0) {
                var field = formContext.getAttribute("ipg_isallreceiveddate");
                if ((field != null) && (field.getValue() != null))
                    field.setValue(null);
                formContext.getControl("ipg_isallreceiveddate1").setNotification("Future date is not allowed", "dateInvalid");
            }
            else
                formContext.getControl("ipg_isallreceiveddate1").clearNotification("dateInvalid");
        }
        Case.ValidateAllReceivedDate = ValidateAllReceivedDate;
        function ConfigureOwnerField(formContext) {
            var ownerAttr = formContext.getAttribute("ownerid");
            if (ownerAttr != null) {
                ownerAttr.setSubmitMode("never");
                ownerAttr.addOnChange(onChangeOwner);
            }
        }
        function onChangeOwner(executionContext) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, incidentid, confirmed, ownerattr, ownerattrValue, incident, result, e_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = executionContext.getFormContext();
                            incidentid = formContext.data.entity.getId();
                            confirmed = true;
                            ownerattr = executionContext.getEventSource();
                            ownerattrValue = ownerattr.getValue();
                            if (!((ownerattrValue === null || ownerattrValue === void 0 ? void 0 : ownerattrValue.length) > 0)) return [3 /*break*/, 9];
                            return [4 /*yield*/, Xrm.WebApi.online.retrieveRecord("incident", incidentid, "?$select=_ownerid_value")];
                        case 1:
                            incident = _a.sent();
                            if (!(ownerattrValue[0].entityType == "systemuser" && incident._ownerid_value !== ownerattrValue[0].id.replace("{", "").replace("}", "").toLocaleLowerCase())) return [3 /*break*/, 3];
                            return [4 /*yield*/, Xrm.Navigation.openConfirmDialog({ text: "System is about to reassign all related open User Tasks to the User you assigned this Case to. Do you wish to proceed?" })];
                        case 2:
                            result = _a.sent();
                            Xrm.Utility.showProgressIndicator("");
                            confirmed = result.confirmed;
                            _a.label = 3;
                        case 3:
                            if (!confirmed) return [3 /*break*/, 8];
                            _a.label = 4;
                        case 4:
                            _a.trys.push([4, 6, , 7]);
                            return [4 /*yield*/, Xrm.WebApi.online.updateRecord("incident", incidentid, { "ownerid@odata.bind": "/" + ownerattrValue[0].entityType + "s(" + ownerattrValue[0].id.replace("{", "").replace("}", "") + ")" })];
                        case 5:
                            _a.sent();
                            return [3 /*break*/, 7];
                        case 6:
                            e_1 = _a.sent();
                            Xrm.Navigation.openErrorDialog({ message: "Case has not been ReAssigned. Please try later or contact System Administrator!" });
                            return [3 /*break*/, 7];
                        case 7: return [3 /*break*/, 9];
                        case 8:
                            ownerattr.setValue([{
                                    entityType: incident["_ownerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"],
                                    id: incident["_ownerid_value"],
                                    name: incident["_ownerid_value@OData.Community.Display.V1.FormattedValue"]
                                }]);
                            _a.label = 9;
                        case 9:
                            Xrm.Utility.closeProgressIndicator();
                            return [2 /*return*/];
                    }
                });
            });
        }
        function CalculateActualTotalResponsibility(executionContext) {
            var _a, _b, _c, _d;
            var formContext = executionContext.getFormContext();
            var carrierResp = (_a = formContext.getAttribute("ipg_actualcarrierresponsibility")) === null || _a === void 0 ? void 0 : _a.getValue();
            var secondaryCarrierResp = (_b = formContext.getAttribute("ipg_actualsecondarycarrierresponsibility")) === null || _b === void 0 ? void 0 : _b.getValue();
            var patientResp = (_c = formContext.getAttribute("ipg_actualmemberresponsibility")) === null || _c === void 0 ? void 0 : _c.getValue();
            (_d = formContext.getAttribute("ipg_actualtotalresponsibility")) === null || _d === void 0 ? void 0 : _d.setValue(carrierResp + secondaryCarrierResp + patientResp);
        }
        Case.CalculateActualTotalResponsibility = CalculateActualTotalResponsibility;
        function CalculateTotalReceived(executionContext) {
            var _a, _b, _c, _d;
            var formContext = executionContext.getFormContext();
            var receivedCarrier = (_a = formContext.getAttribute("ipg_totalreceivedfromcarrier")) === null || _a === void 0 ? void 0 : _a.getValue();
            var receivedSecondaryCarrier = (_b = formContext.getAttribute("ipg_totalreceivedfromsecondarycarrier")) === null || _b === void 0 ? void 0 : _b.getValue();
            var receivedPatient = (_c = formContext.getAttribute("ipg_totalreceivedfrompatient")) === null || _c === void 0 ? void 0 : _c.getValue();
            (_d = formContext.getAttribute("ipg_totalreceived")) === null || _d === void 0 ? void 0 : _d.setValue(receivedCarrier + receivedSecondaryCarrier + receivedPatient);
        }
        Case.CalculateTotalReceived = CalculateTotalReceived;
        function CalculateTotalAdjustments(executionContext) {
            var _a, _b, _c, _d;
            var formContext = executionContext.getFormContext();
            var carrierAdj = (_a = formContext.getAttribute("ipg_totalcarrierrespadjustments")) === null || _a === void 0 ? void 0 : _a.getValue();
            var secondaryCarrierAdj = (_b = formContext.getAttribute("ipg_totalsecondarycarrierrespadjustments")) === null || _b === void 0 ? void 0 : _b.getValue();
            var patientAdj = (_c = formContext.getAttribute("ipg_totalpatientrespadjustments")) === null || _c === void 0 ? void 0 : _c.getValue();
            (_d = formContext.getAttribute("ipg_totalrespadjustments")) === null || _d === void 0 ? void 0 : _d.setValue(carrierAdj + secondaryCarrierAdj + patientAdj);
        }
        Case.CalculateTotalAdjustments = CalculateTotalAdjustments;
        function CalculateTotalWriteOff(executionContext) {
            var _a, _b, _c, _d;
            var formContext = executionContext.getFormContext();
            var carrierWriteOff = (_a = formContext.getAttribute("ipg_totalcarrierwriteoff")) === null || _a === void 0 ? void 0 : _a.getValue();
            var secondaryCarrierWriteOff = (_b = formContext.getAttribute("ipg_totalsecondarycarrierwriteoff")) === null || _b === void 0 ? void 0 : _b.getValue();
            var patientWriteOff = (_c = formContext.getAttribute("ipg_totalpatientwriteoff")) === null || _c === void 0 ? void 0 : _c.getValue();
            (_d = formContext.getAttribute("ipg_totalwriteoff")) === null || _d === void 0 ? void 0 : _d.setValue(carrierWriteOff + secondaryCarrierWriteOff + patientWriteOff);
        }
        Case.CalculateTotalWriteOff = CalculateTotalWriteOff;
        function CalculateTotalBalance(executionContext) {
            var _a, _b, _c, _d;
            var formContext = executionContext.getFormContext();
            var carrierBalance = (_a = formContext.getAttribute("ipg_remainingcarrierbalance")) === null || _a === void 0 ? void 0 : _a.getValue();
            var secondaryCarrierBalance = (_b = formContext.getAttribute("ipg_remainingsecondarycarrierbalance")) === null || _b === void 0 ? void 0 : _b.getValue();
            var patientBalance = (_c = formContext.getAttribute("ipg_remainingpatientbalance")) === null || _c === void 0 ? void 0 : _c.getValue();
            (_d = formContext.getAttribute("ipg_casebalance")) === null || _d === void 0 ? void 0 : _d.setValue(carrierBalance + secondaryCarrierBalance + patientBalance);
        }
        Case.CalculateTotalBalance = CalculateTotalBalance;
        function SetBpfStageBasedOnCaseState(formContext) {
            var incidentId = formContext.data.entity.getId();
            var ipg_statecode = formContext.getAttribute("ipg_statecode").getValue();
            if (ipg_statecode == null)
                return;
            var stageId;
            switch (ipg_statecode) {
                case CaseStates.Intake:
                    stageId = CaseStages.Intake;
                    break;
                case CaseStates.Authorizations:
                    stageId = CaseStages.Authorizations;
                    break;
                case CaseStates.CaseManagement:
                    stageId = CaseStages.CaseManagement;
                    break;
                case CaseStates.Billing:
                    stageId = CaseStages.Billing;
                    break;
                case CaseStates.CarrierCollections:
                    stageId = CaseStages.CarrierCollections;
                    break;
                case CaseStates.PatientCollections:
                    stageId = CaseStages.PatientCollections;
                    break;
                case CaseStates.Finance:
                    stageId = CaseStages.Finance;
                    break;
            }
            if (stageId == null)
                return;
            var apiQuery = "/api/data/v9.1/ipg_ipgcasebpfmainflows?";
            apiQuery += "$select=_activestageid_value,businessprocessflowinstanceid";
            apiQuery += "&$filter=_bpf_incidentid_value eq " + incidentId;
            var req = new XMLHttpRequest();
            req.open("GET", Xrm.Page.context.getClientUrl() + apiQuery, false);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    req.onreadystatechange = null;
                    if (this.status === 200) {
                        var results = JSON.parse(this.response);
                        console.log(results);
                        for (var i = 0; i < results.value.length; i++) {
                            var _activestageid_value = results.value[i]["_activestageid_value"];
                            var businessprocessflowinstanceid = results.value[i]["businessprocessflowinstanceid"];
                            if (_activestageid_value != stageId) {
                                setMainBpfStage(businessprocessflowinstanceid, stageId);
                            }
                        }
                    }
                    else {
                        console.log(this.statusText);
                    }
                }
            };
            req.send();
        }
        Case.SetBpfStageBasedOnCaseState = SetBpfStageBasedOnCaseState;
        function setMainBpfStage(businessprocessflowinstanceid, proposeStageId) {
            var entity = {};
            entity["activestageid@odata.bind"] = "/processstages(" + proposeStageId + ")";
            var req = new XMLHttpRequest();
            req.open("PATCH", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/ipg_ipgcasebpfmainflows(" + businessprocessflowinstanceid + ")", false);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    req.onreadystatechange = null;
                    if (this.status === 204) {
                        console.log('Success. Stage is set');
                    }
                    else {
                        console.log(this.statusText);
                    }
                }
            };
            req.send(JSON.stringify(entity));
        }
        function procesCourtesyCaseFieldAccess(formContext) {
            var _a, _b, _c;
            return __awaiter(this, void 0, void 0, function () {
                var openedCase, formattedCaseId, anyCourtesyActualPartExists;
                return __generator(this, function (_d) {
                    switch (_d.label) {
                        case 0:
                            openedCase = isCaseOpened(formContext);
                            formattedCaseId = Intake.Utility.GetFormattedId(formContext.data.entity.getId());
                            return [4 /*yield*/, anyCountresyActualPart(formattedCaseId)];
                        case 1:
                            anyCourtesyActualPartExists = _d.sent();
                            if (anyCourtesyActualPartExists) {
                                (_a = formContext.getAttribute("ipg_iscourtesyclaimcase1")) === null || _a === void 0 ? void 0 : _a.setValue(true);
                                (_b = formContext.getControl("ipg_iscourtesyclaimcase1")) === null || _b === void 0 ? void 0 : _b.setDisabled(true);
                            }
                            else if (openedCase) {
                                (_c = formContext.getControl("ipg_iscourtesyclaimcase1")) === null || _c === void 0 ? void 0 : _c.setDisabled(false);
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        function anyCountresyActualPart(id) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$filter=_ipg_caseid_value eq " + id + " and  ipg_iscourtesyclaimplan eq true")];
                        case 1: return [2 /*return*/, (((_a = (_b.sent())) === null || _a === void 0 ? void 0 : _a.entities.length) > 0)];
                    }
                });
            });
        }
        function RefreshOnDataLoad(event) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var formContext, psSubgrid, webResourceObj, src;
                return __generator(this, function (_b) {
                    formContext = event.getFormContext();
                    SetAllReceivedDateField(formContext);
                    psSubgrid = formContext.getControl("StatementDocuments");
                    psSubgrid === null || psSubgrid === void 0 ? void 0 : psSubgrid.refreshRibbon();
                    webResourceObj = (_a = formContext.getControl("WebResource_ARSummary")) === null || _a === void 0 ? void 0 : _a.getObject();
                    if (webResourceObj) {
                        src = webResourceObj.src;
                        webResourceObj.src = "about:blank";
                        webResourceObj.src = src;
                    }
                    return [2 /*return*/];
                });
            });
        }
        function onActualDosChange(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            var actualDosValue = formContext.getAttribute("ipg_actualdos") ? formContext.getAttribute("ipg_actualdos").getValue() : null;
            (_a = formContext.getControl("ipg_surgerydate")) === null || _a === void 0 ? void 0 : _a.setDisabled(actualDosValue != null);
        }
        Case.onActualDosChange = onActualDosChange;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
