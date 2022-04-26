/**
 * @namespace Intake.Case.InsuranceTab
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
        var InsuranceTab;
        (function (InsuranceTab) {
            var LOADING_MESSAGE = 'Loading...';
            var TAB_ID = 'InsuranceBenefitsAuth';
            var CARRIER1_SECTION_ID = 'Carrier1Section';
            var CARRIER2_SECTION_ID = 'Carrier2Section';
            var CarrierTypes;
            (function (CarrierTypes) {
                CarrierTypes[CarrierTypes["Auto"] = 427880000] = "Auto";
                CarrierTypes[CarrierTypes["Commercial"] = 427880002] = "Commercial";
                CarrierTypes[CarrierTypes["DME"] = 427880004] = "DME";
                CarrierTypes[CarrierTypes["Government"] = 923720006] = "Government";
                CarrierTypes[CarrierTypes["IPA"] = 427880003] = "IPA";
                CarrierTypes[CarrierTypes["Other"] = 923720011] = "Other";
                CarrierTypes[CarrierTypes["SelfPay"] = 427880005] = "SelfPay";
                CarrierTypes[CarrierTypes["WorkersComp"] = 427880001] = "WorkersComp";
            })(CarrierTypes || (CarrierTypes = {}));
            var BenefitTypes;
            (function (BenefitTypes) {
                BenefitTypes[BenefitTypes["GeneralHealth"] = 427880059] = "GeneralHealth";
                BenefitTypes[BenefitTypes["Auto"] = 427880000] = "Auto";
                BenefitTypes[BenefitTypes["WorkersComp"] = 427880001] = "WorkersComp";
                BenefitTypes[BenefitTypes["DurableMedicalEquipment"] = 427880040] = "DurableMedicalEquipment";
            })(BenefitTypes || (BenefitTypes = {}));
            var benefitFieldNames = {
                carrierId: ['ipg_carrierid', 'ipg_secondarycarrierid'],
                memberId: ['ipg_memberidnumber', 'ipg_secondarymemberidnumber'],
                benefitType: ['ipg_benefittypecode', 'ipg_carrier2benefittypecode'],
                inOutNetwork: ['ipg_inoutnetwork', 'ipg_carrier2isinoutnetwork'],
                coverageLevelDeductible: ['ipg_coverageleveldeductible', 'ipg_carrier2coverageleveldeductible'],
                coverageLevelOop: ['ipg_coverageleveloop', 'ipg_carrier2coverageleveloop'],
                coverageEffectiveDate: ['ipg_primarycarriereffectivedate', 'ipg_secondarycarriereffectivedate'],
                coverageExpirationDate: ['ipg_primarycarrierexpirationdate', 'ipg_secondarycarrierexpirationdate'],
                deductible: ['ipg_deductible', 'ipg_carrier2deductibledisplay'],
                deductibleMet: ['ipg_deductiblemet', 'ipg_carrier2deductiblemetdisplay'],
                deductibleRemaining: ['ipg_deductibleremainingdisplay', 'ipg_carrier2deductibleremainingdisplay'],
                payerCoinsurance: ['ipg_payercoinsurance', 'ipg_carrier2carriercoinsurancedisplay'],
                patientCoinsurance: ['ipg_patientcoinsurance', 'ipg_carrier2patientcoinsurancedisplay'],
                oopMax: ['ipg_oopmax', 'ipg_carrier2oopmaxdisplay'],
                oopMet: ['ipg_oopmet', 'ipg_carrier2oopmetdisplay'],
                oopRemaining: ['ipg_oopremainingdisplay', 'ipg_carrier2oopremainingdisplay']
            };
            function OnFormLoad(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext, carrierTypes;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                setCarrierButtonsContext(formContext);
                                return [4 /*yield*/, getCarrierTypes(formContext)];
                            case 1:
                                carrierTypes = _a.sent();
                                setupFieldsByCarrierType(formContext, carrierTypes[0]); //todo: secondary carrier
                                filterBenefitTypes(formContext, carrierTypes);
                                setupBenefitFields(formContext);
                                retrieveAndDisplayBenefits(formContext);
                                setInsuredGender(formContext); //todo: secondary carrier
                                SetBVFNames(formContext); //todo: this might be wrong
                                return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnFormLoad = OnFormLoad;
            function OnFormSave(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext, displayedCarrierIndex, carrierType;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                displayedCarrierIndex = getDisplayedCarrierIndex(formContext);
                                if (displayedCarrierIndex == null) {
                                    throw new Error('Could not determine displayed carrier');
                                }
                                return [4 /*yield*/, getCarrierType(formContext, benefitFieldNames.carrierId[displayedCarrierIndex])];
                            case 1:
                                carrierType = _a.sent();
                                if (displayedCarrierIndex == 0) {
                                    setupFieldsByCarrierType(formContext, carrierType); //todo: secondary carrier
                                }
                                filterCarrierBenefitTypes(formContext, displayedCarrierIndex, carrierType);
                                retrieveAndDisplayCarrierBenefits(formContext, displayedCarrierIndex);
                                return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnFormSave = OnFormSave;
            //primary carrier
            function OnBenefitTypeChange(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, , 3, 4]);
                                return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 0)];
                            case 2:
                                _a.sent();
                                return [3 /*break*/, 4];
                            case 3:
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 4: return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnBenefitTypeChange = OnBenefitTypeChange;
            function OnInOutNetworkChange(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, , 3, 4]);
                                return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 0)];
                            case 2:
                                _a.sent();
                                return [3 /*break*/, 4];
                            case 3:
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 4: return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnInOutNetworkChange = OnInOutNetworkChange;
            function OnDeductibleCoverageLevelChange(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, , 3, 4]);
                                return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 0)];
                            case 2:
                                _a.sent();
                                return [3 /*break*/, 4];
                            case 3:
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 4: return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnDeductibleCoverageLevelChange = OnDeductibleCoverageLevelChange;
            function OnOOPCoverageLevelChange(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, , 3, 4]);
                                return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 0)];
                            case 2:
                                _a.sent();
                                return [3 /*break*/, 4];
                            case 3:
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 4: return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnOOPCoverageLevelChange = OnOOPCoverageLevelChange;
            //secondary carrier
            function OnSecondaryBenefitTypeChange(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, , 3, 4]);
                                return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 1)];
                            case 2:
                                _a.sent();
                                return [3 /*break*/, 4];
                            case 3:
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 4: return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnSecondaryBenefitTypeChange = OnSecondaryBenefitTypeChange;
            function OnSecondaryInOutNetworkChange(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, , 3, 4]);
                                return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 1)];
                            case 2:
                                _a.sent();
                                return [3 /*break*/, 4];
                            case 3:
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 4: return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnSecondaryInOutNetworkChange = OnSecondaryInOutNetworkChange;
            function OnSecondaryDeductibleCoverageLevelChange(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, , 3, 4]);
                                return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 1)];
                            case 2:
                                _a.sent();
                                return [3 /*break*/, 4];
                            case 3:
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 4: return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnSecondaryDeductibleCoverageLevelChange = OnSecondaryDeductibleCoverageLevelChange;
            function OnSecondaryOOPCoverageLevelChange(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                Xrm.Utility.showProgressIndicator(LOADING_MESSAGE);
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, , 3, 4]);
                                return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 1)];
                            case 2:
                                _a.sent();
                                return [3 /*break*/, 4];
                            case 3:
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 4: return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.OnSecondaryOOPCoverageLevelChange = OnSecondaryOOPCoverageLevelChange;
            function SetBVFNames(formContext) {
                var memberId = formContext.getAttribute("ipg_memberidnumber").getValue();
                var i;
                if (memberId != null) {
                    Xrm.WebApi.retrieveMultipleRecords("ipg_benefitsverificationform", "?$select=ipg_memberidnumber,ipg_primarycontact,ipg_dateofinjury,ipg_deductible,ipg_deductiblemet,ipg_carriercoinsurance,ipg_patientcoinsurance,ipg_oopmax,ipg_oopmaxmet&$filter=ipg_memberidnumber eq '" + memberId + "'").then(function success(results) {
                        if (results.entities.length) {
                            formContext.getAttribute("ipg_primarycontactcode").setValue(results.entities[0]["ipg_primarycontact"]);
                            formContext.getAttribute("ipg_deductible").setValue(results.entities[0]["ipg_deductible"]);
                            formContext.getAttribute("ipg_deductiblemet").setValue(results.entities[0]["ipg_deductiblemet"]);
                            formContext.getAttribute("ipg_deductiblemet").setValue(results.entities[0]["ipg_deductibleremainingdisplay"]);
                            formContext.getAttribute("ipg_payercoinsurance").setValue(results.entities[0]["ipg_carriercoinsurance"]);
                            formContext.getAttribute("ipg_patientcoinsurance").setValue(results.entities[0]["ipg_patientcoinsurance"]);
                            formContext.getAttribute("ipg_oopmax").setValue(results.entities[0]["ipg_oopmax"]);
                            formContext.getAttribute("ipg_oopremainingdisplay").setValue(results.entities[0]["ipg_oopmaxmet"]);
                            formContext.getAttribute("ipg_autodateofincident").setValue(new Date(results.entities[0]["ipg_dateofinjury"]));
                        }
                    }, function (error) {
                        console.log(error.message);
                    });
                }
            }
            InsuranceTab.SetBVFNames = SetBVFNames;
            function setInsuredGender(formContext) {
                var optionGenderValue = Xrm.Page.getAttribute("ipg_patientgender").getValue();
                if (optionGenderValue != null) {
                    if (optionGenderValue == 923720000) {
                        optionGenderValue = 427880000;
                    }
                    formContext.getAttribute("ipg_insuredgender").setValue(optionGenderValue);
                }
            }
            InsuranceTab.setInsuredGender = setInsuredGender;
            function setupFieldsByCarrierType(formContext, primaryCarrierType) {
                return __awaiter(this, void 0, void 0, function () {
                    var isPrimaryMemberIdDme, memberIdAttribute, memberId;
                    return __generator(this, function (_a) {
                        isPrimaryMemberIdDme = false;
                        memberIdAttribute = formContext.getAttribute('ipg_memberidnumber');
                        if (memberIdAttribute) {
                            memberId = memberIdAttribute.getValue();
                            isPrimaryMemberIdDme = isDmeMemberId(memberId);
                        }
                        setControlsState(formContext, primaryCarrierType, isPrimaryMemberIdDme);
                        setDefaultValues(formContext, primaryCarrierType, isPrimaryMemberIdDme);
                        return [2 /*return*/];
                    });
                });
            }
            function setupAdjusterAndNurseFields(formContext, primaryCarrierType) {
                var showAdjuster = false;
                var showNurse = false;
                if (primaryCarrierType == CarrierTypes.Auto) {
                    showAdjuster = true;
                    showNurse = false;
                }
                else if (primaryCarrierType == CarrierTypes.WorkersComp) {
                    var primaryContactCodeAttribute = formContext.getAttribute('ipg_primarycontactcode');
                    if (primaryContactCodeAttribute) {
                        var primaryContactCodeValue = primaryContactCodeAttribute.getValue();
                        if (primaryContactCodeValue) {
                            if (primaryContactCodeValue == 427880000 /*Adjuster*/) {
                                showAdjuster = true;
                                showNurse = false;
                            }
                            else if (primaryContactCodeValue == 427880001 /*Nurse*/) {
                                showAdjuster = false;
                                showNurse = true;
                            }
                            else {
                                throw new Error('Unexpected Primary Contact Code value: ' + primaryContactCodeValue);
                            }
                        }
                    }
                }
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_autoadjustername", showAdjuster);
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_adjusterphone", showAdjuster);
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_nursecasemgrname", showNurse);
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_nursecasemgrphone", showNurse);
            }
            function setControlLabel(formContext, tabId, sectionId, fieldName, label) {
                var control = findControl(formContext, tabId, sectionId, fieldName);
                if (control) {
                    control.setLabel(label);
                }
            }
            function setControlDisabled(formContext, tabId, sectionId, fieldName, disable) {
                var control = findControl(formContext, tabId, sectionId, fieldName);
                if (control) {
                    control.setDisabled(disable);
                }
            }
            function setControlVisible(formContext, tabId, sectionId, fieldName, visible) {
                var control = findControl(formContext, tabId, sectionId, fieldName);
                if (control) {
                    control.setVisible(visible);
                }
            }
            function findControl(formContext, tabId, sectionId, fieldName) {
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
                                    return sectionControl;
                                }
                            }
                        }
                    }
                }
                return null;
            }
            function setControlsState(formContext, primaryCarrierType, isPrimaryMemberIdDme) {
                var isAutoOrWc = primaryCarrierType == CarrierTypes.Auto || primaryCarrierType == CarrierTypes.WorkersComp;
                //column1
                setControlLabel(formContext, TAB_ID, CARRIER1_SECTION_ID, 'ipg_memberidnumber', isAutoOrWc ? 'Claim #' : 'Member ID');
                //column2
                setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_benefittypecode", isAutoOrWc || isPrimaryMemberIdDme);
                setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_inoutnetwork", isAutoOrWc);
                setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_benefitplantypecode", isAutoOrWc);
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_plandescription", primaryCarrierType != CarrierTypes.WorkersComp);
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_plansponsor", primaryCarrierType != CarrierTypes.WorkersComp);
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_autobenefitsexhausted", primaryCarrierType == CarrierTypes.Auto);
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_billingfax", isAutoOrWc);
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_primarycontactcode", primaryCarrierType == CarrierTypes.WorkersComp);
                setupAdjusterAndNurseFields(formContext, primaryCarrierType);
                setControlVisible(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_autodateofincident", isAutoOrWc); //todo: create DateOfInjury field and use for both Auto and Wc
                //column3
                setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_coverageleveldeductible", isAutoOrWc);
                setControlDisabled(formContext, TAB_ID, CARRIER1_SECTION_ID, "ipg_coverageleveloop", isAutoOrWc);
            }
            function setDefaultValues(formContext, primaryCarrierType, isPrimaryMemberIdDme) {
                if (primaryCarrierType == CarrierTypes.Auto || primaryCarrierType == CarrierTypes.WorkersComp) {
                    //column1
                    formContext.getAttribute('ipg_relationtoinsured').setValue(100000000); //Self
                    formContext.getAttribute('ipg_insuredfirstname').setValue(formContext.getAttribute('ipg_patientfirstname').getValue());
                    formContext.getAttribute('ipg_insuredmiddlename').setValue(formContext.getAttribute('ipg_patientmiddlename').getValue());
                    formContext.getAttribute('ipg_insuredlastname').setValue(formContext.getAttribute('ipg_patientlastname').getValue());
                    formContext.getAttribute('ipg_insureddateofbirth').setValue(formContext.getAttribute('ipg_patientdateofbirth').getValue());
                    formContext.getAttribute('ipg_insuredgender').setValue(formContext.getAttribute('ipg_patientgender').getValue());
                    //column2
                    if (primaryCarrierType == CarrierTypes.Auto) {
                        formContext.getAttribute('ipg_benefittypecode').setValue(427880000); //Auto
                    }
                    else if (primaryCarrierType == CarrierTypes.WorkersComp) {
                        formContext.getAttribute('ipg_benefittypecode').setValue(427880001); //WC
                    }
                    formContext.getAttribute('ipg_benefitplantypecode').setValue(427880004); //Other
                    formContext.getAttribute('ipg_inoutnetwork').setValue(true); //INN
                    //column3
                    formContext.getAttribute('ipg_coverageleveldeductible').setValue(427880001); //Individual
                    formContext.getAttribute('ipg_coverageleveloop').setValue(427880001); //Individual
                    if (primaryCarrierType == CarrierTypes.Auto) {
                        formContext.getAttribute('ipg_is_authorization_required').setValue(false); //No
                    }
                    else if (primaryCarrierType == CarrierTypes.WorkersComp) {
                        formContext.getAttribute('ipg_is_authorization_required').setValue(true); //Yes
                    }
                }
            }
            function filterBenefitTypes(formContext, carrierTypes) {
                return __awaiter(this, void 0, void 0, function () {
                    var secondaryCarrrierId;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, filterCarrierBenefitTypes(formContext, 0, carrierTypes[0])];
                            case 1:
                                _a.sent();
                                secondaryCarrrierId = getCarrierIdByIndex(formContext, 1);
                                if (!secondaryCarrrierId) return [3 /*break*/, 3];
                                return [4 /*yield*/, filterCarrierBenefitTypes(formContext, 1, carrierTypes[1])];
                            case 2:
                                _a.sent();
                                _a.label = 3;
                            case 3: return [2 /*return*/];
                        }
                    });
                });
            }
            function filterCarrierBenefitTypes(formContext, carrierIndex, carrierType) {
                return __awaiter(this, void 0, void 0, function () {
                    var carrierIdAttribute, memberIdAttribute, benefitTypeAttribute, benefitTypeControl, incidentid, carrierIdValue, carrierId, memberId_1, benefits_1;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                carrierIdAttribute = formContext.getAttribute(benefitFieldNames.carrierId[carrierIndex]);
                                memberIdAttribute = formContext.getAttribute(benefitFieldNames.memberId[carrierIndex]);
                                benefitTypeAttribute = formContext.getAttribute(benefitFieldNames.benefitType[carrierIndex]);
                                benefitTypeControl = null;
                                if (benefitTypeAttribute && benefitTypeAttribute.controls.getLength()) {
                                    benefitTypeControl = benefitTypeAttribute.controls.getByIndex(0);
                                }
                                if (!(carrierIdAttribute && memberIdAttribute && benefitTypeControl)) return [3 /*break*/, 2];
                                incidentid = formContext.data.entity.getId().replace('{', '').replace('}', '');
                                carrierIdValue = carrierIdAttribute.getValue();
                                carrierId = null;
                                if (carrierIdValue && carrierIdValue.length) {
                                    carrierId = carrierIdValue[0].id.replace('{', '').replace('}', '');
                                }
                                memberId_1 = memberIdAttribute.getValue();
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_benefit', "?$select=ipg_benefittype&$filter=ipg_CaseId/incidentid eq " + incidentid + " and ipg_CarrierId/accountid eq " + carrierId + " and ipg_memberid eq '" + encodeURIComponent(memberId_1) + "'")];
                            case 1:
                                benefits_1 = _a.sent();
                                benefitTypeAttribute.getOptions().forEach(function (option) {
                                    if (option.value == BenefitTypes.GeneralHealth) {
                                        return; //do not filter out General Health benefit type
                                    }
                                    if (carrierType == CarrierTypes.Auto && option.value == BenefitTypes.Auto) {
                                        return;
                                    }
                                    if (carrierType == CarrierTypes.WorkersComp && option.value == BenefitTypes.WorkersComp) {
                                        return;
                                    }
                                    if (isDmeMemberId(memberId_1) && option.value == BenefitTypes.DurableMedicalEquipment) {
                                        return;
                                    }
                                    var benefitExists = benefits_1.entities.some(function (b) { return b.ipg_benefittype == option.value; });
                                    if (!benefitExists) {
                                        benefitTypeControl.removeOption(option.value);
                                    }
                                });
                                _a.label = 2;
                            case 2: return [2 /*return*/];
                        }
                    });
                });
            }
            function setupBenefitFields(formContext) {
                setupCarrierBenefitFields(formContext, 0);
                setupCarrierBenefitFields(formContext, 1);
            }
            function setupCarrierBenefitFields(formContext, carrierIndex) {
                formContext.getAttribute(benefitFieldNames.coverageEffectiveDate[carrierIndex]).setSubmitMode('never');
                formContext.getAttribute(benefitFieldNames.coverageExpirationDate[carrierIndex]).setSubmitMode('never');
                formContext.getAttribute(benefitFieldNames.deductible[carrierIndex]).setSubmitMode('never');
                formContext.getAttribute(benefitFieldNames.deductibleMet[carrierIndex]).setSubmitMode('never');
                formContext.getAttribute(benefitFieldNames.deductibleRemaining[carrierIndex]).setSubmitMode('never');
                formContext.getAttribute(benefitFieldNames.payerCoinsurance[carrierIndex]).setSubmitMode('never');
                formContext.getAttribute(benefitFieldNames.patientCoinsurance[carrierIndex]).setSubmitMode('never');
                formContext.getAttribute(benefitFieldNames.oopMax[carrierIndex]).setSubmitMode('never');
                formContext.getAttribute(benefitFieldNames.oopMet[carrierIndex]).setSubmitMode('never');
                formContext.getAttribute(benefitFieldNames.oopRemaining[carrierIndex]).setSubmitMode('never');
            }
            function retrieveAndDisplayBenefits(formContext) {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 0)];
                            case 1:
                                _a.sent();
                                return [4 /*yield*/, retrieveAndDisplayCarrierBenefits(formContext, 1)];
                            case 2:
                                _a.sent();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            function retrieveAndDisplayCarrierBenefits(formContext, carrierIndex) {
                return __awaiter(this, void 0, void 0, function () {
                    var coverageStart, coverageEnd, deductibleMax, deductibleMet, deductibleRemaining, payerCoinsurance, patientCoinsurance, oopMax, oopMet, oopRemaining, carrierIdAttribute, memberIdAttribute, benefitTypeAttribute, inOutNetworkAttribute, deductibleCoverageLevelAttribute, oopCoverageLevelAttribute, incidentid, carrierIdValue, carrierId, memberIdValue, benefitTypeValue, inOutNetworkValue, deductibleCoverageLevelValue_1, oopCoverageLevelValue_1, benefits_2, deductibleBenefit, oopBenefit;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                coverageStart = null, coverageEnd = null;
                                carrierIdAttribute = formContext.getAttribute(benefitFieldNames.carrierId[carrierIndex]);
                                memberIdAttribute = formContext.getAttribute(benefitFieldNames.memberId[carrierIndex]);
                                benefitTypeAttribute = formContext.getAttribute(benefitFieldNames.benefitType[carrierIndex]);
                                inOutNetworkAttribute = formContext.getAttribute(benefitFieldNames.inOutNetwork[carrierIndex]);
                                deductibleCoverageLevelAttribute = formContext.getAttribute(benefitFieldNames.coverageLevelDeductible[carrierIndex]);
                                oopCoverageLevelAttribute = formContext.getAttribute(benefitFieldNames.coverageLevelOop[carrierIndex]);
                                if (!(carrierIdAttribute && memberIdAttribute && benefitTypeAttribute && inOutNetworkAttribute && deductibleCoverageLevelAttribute && oopCoverageLevelAttribute)) return [3 /*break*/, 2];
                                incidentid = formContext.data.entity.getId().replace('{', '').replace('}', '');
                                carrierIdValue = carrierIdAttribute.getValue();
                                carrierId = null;
                                if (carrierIdValue && carrierIdValue.length) {
                                    carrierId = carrierIdValue[0].id.replace('{', '').replace('}', '');
                                }
                                memberIdValue = memberIdAttribute.getValue();
                                benefitTypeValue = benefitTypeAttribute.getValue();
                                inOutNetworkValue = inOutNetworkAttribute.getValue();
                                deductibleCoverageLevelValue_1 = deductibleCoverageLevelAttribute.getValue();
                                oopCoverageLevelValue_1 = oopCoverageLevelAttribute.getValue();
                                if (!(carrierId && memberIdValue && benefitTypeValue && inOutNetworkValue != null && (deductibleCoverageLevelValue_1 || oopCoverageLevelValue_1))) return [3 /*break*/, 2];
                                benefits_2 = null;
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_benefit', "?$top=1&$filter=ipg_CaseId/incidentid eq " + incidentid + " and ipg_CarrierId/accountid eq " + carrierId + " and ipg_memberid eq '" + encodeURIComponent(memberIdValue) + "'"
                                        + (" and ipg_benefittype eq " + benefitTypeValue + " and ipg_inoutnetwork eq " + inOutNetworkValue))
                                        .then(function (res) {
                                        benefits_2 = res.entities;
                                    })];
                            case 1:
                                _a.sent();
                                if (deductibleCoverageLevelValue_1) {
                                    deductibleBenefit = benefits_2.find(function (b) { return b.ipg_coveragelevel == deductibleCoverageLevelValue_1; });
                                    if (deductibleBenefit) {
                                        if (deductibleBenefit.ipg_eligibilitystartdate) {
                                            coverageStart = new Date(deductibleBenefit.ipg_eligibilitystartdate);
                                        }
                                        if (deductibleBenefit.ipg_eligibilityenddate) {
                                            coverageEnd = new Date(deductibleBenefit.ipg_eligibilityenddate);
                                        }
                                        deductibleMax = deductibleBenefit.ipg_deductible;
                                        deductibleMet = deductibleBenefit.ipg_deductiblemet;
                                        deductibleRemaining = deductibleBenefit.ipg_deductibleremainingcalculated;
                                        payerCoinsurance = deductibleBenefit.ipg_carriercoinsurance;
                                        patientCoinsurance = deductibleBenefit.ipg_membercoinsurance;
                                    }
                                }
                                if (oopCoverageLevelValue_1) {
                                    oopBenefit = benefits_2.find(function (b) { return b.ipg_coveragelevel == oopCoverageLevelValue_1; });
                                    if (oopBenefit) {
                                        oopMax = oopBenefit.ipg_memberoopmax;
                                        oopMet = oopBenefit.ipg_memberoopmet;
                                        oopRemaining = oopBenefit.ipg_memberoopremainingcalculated;
                                    }
                                }
                                _a.label = 2;
                            case 2:
                                //always update displayed benefit values. If benefit is found, display values. If benefit could not be found, clear values
                                formContext.getAttribute(benefitFieldNames.coverageEffectiveDate[carrierIndex]).setValue(coverageStart);
                                formContext.getAttribute(benefitFieldNames.coverageExpirationDate[carrierIndex]).setValue(coverageEnd);
                                formContext.getAttribute(benefitFieldNames.deductible[carrierIndex]).setValue(deductibleMax);
                                formContext.getAttribute(benefitFieldNames.deductibleMet[carrierIndex]).setValue(deductibleMet);
                                formContext.getAttribute(benefitFieldNames.deductibleRemaining[carrierIndex]).setValue(deductibleRemaining);
                                formContext.getAttribute(benefitFieldNames.payerCoinsurance[carrierIndex]).setValue(payerCoinsurance);
                                formContext.getAttribute(benefitFieldNames.patientCoinsurance[carrierIndex]).setValue(patientCoinsurance);
                                formContext.getAttribute(benefitFieldNames.oopMax[carrierIndex]).setValue(oopMax);
                                formContext.getAttribute(benefitFieldNames.oopMet[carrierIndex]).setValue(oopMet);
                                formContext.getAttribute(benefitFieldNames.oopRemaining[carrierIndex]).setValue(oopRemaining);
                                return [2 /*return*/];
                        }
                    });
                });
            }
            InsuranceTab.retrieveAndDisplayCarrierBenefits = retrieveAndDisplayCarrierBenefits;
            function getDisplayedCarrierIndex(formContext) {
                var tabObj = formContext.ui.tabs.get(TAB_ID);
                if (tabObj) {
                    var carrier1Section = tabObj.sections.get(CARRIER1_SECTION_ID);
                    if (carrier1Section) {
                        if (carrier1Section.getVisible()) {
                            return 0;
                        }
                        else {
                            var carrier2Section = tabObj.sections.get(CARRIER2_SECTION_ID);
                            if (carrier2Section === null || carrier2Section === void 0 ? void 0 : carrier2Section.getVisible()) {
                                return 1;
                            }
                        }
                    }
                }
                return null;
            }
            function getCarrierIdByIndex(formContext, carrierIndex) {
                return getLookupId(formContext, benefitFieldNames.carrierId[carrierIndex]);
            }
            function getLookupId(formContext, lookupFieldName) {
                var lookupAttribute = formContext.getAttribute(lookupFieldName);
                if (lookupAttribute) {
                    var lookupValue = lookupAttribute.getValue();
                    if (lookupValue && lookupValue.length && lookupValue[0].id) {
                        return lookupValue[0].id;
                    }
                }
                return null;
            }
            function getCarrierTypes(formContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var carrierTypes, _a, _b, carrier2Id, _c, _d;
                    return __generator(this, function (_e) {
                        switch (_e.label) {
                            case 0:
                                carrierTypes = new Array();
                                _a = carrierTypes;
                                _b = 0;
                                return [4 /*yield*/, getCarrierType(formContext, benefitFieldNames.carrierId[0])];
                            case 1:
                                _a[_b] = _e.sent();
                                carrier2Id = getLookupId(formContext, benefitFieldNames.carrierId[0]);
                                if (!carrier2Id) return [3 /*break*/, 3];
                                _c = carrierTypes;
                                _d = 1;
                                return [4 /*yield*/, getCarrierType(formContext, benefitFieldNames.carrierId[1])];
                            case 2:
                                _c[_d] = _e.sent();
                                _e.label = 3;
                            case 3: return [2 /*return*/, carrierTypes];
                        }
                    });
                });
            }
            function getCarrierType(formContext, carrierFieldName) {
                return __awaiter(this, void 0, void 0, function () {
                    var carrierAttribute, carrierValue, carrierResult;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                carrierAttribute = formContext.getAttribute(carrierFieldName);
                                if (!carrierAttribute) return [3 /*break*/, 2];
                                carrierValue = carrierAttribute.getValue();
                                if (!(carrierValue && carrierValue.length && carrierValue[0].id)) return [3 /*break*/, 2];
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", carrierValue[0].id, "?$select=ipg_carriertype")];
                            case 1:
                                carrierResult = _a.sent();
                                if (carrierResult && carrierResult.ipg_carriertype) {
                                    return [2 /*return*/, carrierResult.ipg_carriertype];
                                }
                                _a.label = 2;
                            case 2: return [2 /*return*/, null];
                        }
                    });
                });
            }
            function setCarrierButtonsContext(formContext) {
                {
                    var carrierButtonsControl = formContext.getControl('WebResource_carrierbuttons');
                    if (carrierButtonsControl != null && carrierButtonsControl != undefined) {
                        carrierButtonsControl.getContentWindow().then(function (contentWindow) {
                            contentWindow.Intake.CarrierButtons.setRefreshBenefitsFunctionReference(function () { retrieveAndDisplayCarrierBenefits(formContext, 0); });
                        });
                    }
                }
                {
                    var carrierButtons2Control = formContext.getControl('WebResource_carrierbuttons2');
                    if (carrierButtons2Control != null && carrierButtons2Control != undefined) {
                        carrierButtons2Control.getContentWindow().then(function (contentWindow) {
                            contentWindow.Intake.CarrierButtons.setRefreshBenefitsFunctionReference(function () { retrieveAndDisplayCarrierBenefits(formContext, 1); });
                        });
                    }
                }
            }
            function isDmeMemberId(memberId) {
                return (memberId || '').toUpperCase().startsWith('JQU');
            }
        })(InsuranceTab = Case.InsuranceTab || (Case.InsuranceTab = {}));
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
