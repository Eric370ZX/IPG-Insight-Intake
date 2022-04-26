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
 * @namespace Intake.benefitsVerificationForm
 */
var Intake;
(function (Intake) {
    var benefitsVerificationForm;
    (function (benefitsVerificationForm) {
        var Form;
        (function (Form) {
            var FormTypes;
            (function (FormTypes) {
                FormTypes["GeneralHealth"] = "8BBB1658-323B-4BCC-81EA-6E68FB6C4AA1";
                FormTypes["Auto"] = "d1560309-5c0d-49d6-bddc-64495e8fc285";
                FormTypes["WC"] = "b304dbab-c574-4cc2-87a9-87f123f5dfb0";
                FormTypes["DME"] = "782375fa-3b65-4251-8e2a-897c40864220";
            })(FormTypes = Form.FormTypes || (Form.FormTypes = {}));
            var CarrierTypes;
            (function (CarrierTypes) {
                CarrierTypes[CarrierTypes["Auto"] = 427880000] = "Auto";
                CarrierTypes[CarrierTypes["Commercial"] = 427880002] = "Commercial";
                CarrierTypes[CarrierTypes["DME"] = 427880004] = "DME";
                CarrierTypes[CarrierTypes["Government"] = 923720006] = "Government";
                CarrierTypes[CarrierTypes["IPA"] = 427880003] = "IPA";
                CarrierTypes[CarrierTypes["Other"] = 923720011] = "Other";
                CarrierTypes[CarrierTypes["SelfP0ay"] = 427880005] = "SelfP0ay";
                CarrierTypes[CarrierTypes["WorkersComp"] = 427880001] = "WorkersComp";
            })(CarrierTypes = Form.CarrierTypes || (Form.CarrierTypes = {}));
            var BvfStateCodes;
            (function (BvfStateCodes) {
                BvfStateCodes[BvfStateCodes["Active"] = 0] = "Active";
                BvfStateCodes[BvfStateCodes["Inactive"] = 1] = "Inactive";
            })(BvfStateCodes || (BvfStateCodes = {}));
            Form.attributes = {
                case: {
                    insuredaddress: "ipg_insuredaddress",
                    insuredcity: "ipg_insuredcity",
                    insuredstate: "ipg_insuredstate",
                    insuredzipcodeid: "ipg_insuredzipcodeid",
                    insuredphone: "ipg_insuredphone",
                    relationtoinsuredcode: "ipg_relationtoinsuredcode"
                },
                patient: {
                    patientaddress: "ipg_patientaddress",
                    patientcity: "ipg_patientcity",
                    patientstate: "ipg_patientstate",
                    patientzipcodeid: "ipg_patientzipcodeid",
                    patienthomephone: "ipg_patienthomephone"
                },
                optionSets: {
                    relationtoinsure: {
                        self: 100000000
                    }
                }
            };
            Form.webResources = {
                sameAsPatientCheckbox: "WebResource_sameAsPatientCheckbox"
            };
            var InitLoader;
            (function (InitLoader) {
                InitLoader.exportFields = [
                    //{ sourceField: "ipg_patientfirstname", targetField: "ipg_patientfirstname" },
                    //{ sourceField: "ipg_patientlastname", targetField: "ipg_patientlastname" },
                    //{ sourceField: "ipg_patientmiddlename", targetField: "ipg_patientmiddlename" },
                    //{ sourceField: "ipg_patientdateofbirth", targetField: "ipg_patientdateofbirth" },
                    //{ sourceField: "ipg_facilityid", targetField: "ipg_facilityid" },
                    //{ sourceField: "ipg_procedureid", targetField: "ipg_procedureid" },
                    //{ sourceField: "ipg_cptcodeid1", targetField: "ipg_cptcodeid1" },
                    //{ sourceField: "ipg_cptcodeid2", targetField: "ipg_cptcodeid2" },
                    //{ sourceField: "ipg_cptcodeid3", targetField: "ipg_cptcodeid3" },
                    //{ sourceField: "ipg_cptcodeid4", targetField: "ipg_cptcodeid4" },
                    //{ sourceField: "ipg_cptcodeid5", targetField: "ipg_cptcodeid5" },
                    //{ sourceField: "ipg_cptcodeid6", targetField: "ipg_cptcodeid6" },
                    //{ sourceField: "ipg_physicianid", targetField: "ipg_physicianid" },
                    //{ sourceField: "ipg_patientid", targetField: "ipg_patientid" },
                    { sourceField: "ipg_homeplancarrierid", targetField: "ipg_homeplancarrierid" },
                    { sourceField: "ipg_primarycarriergroupidnumber", targetField: "ipg_primarycarriergroupidnumber" },
                    { sourceField: "ipg_ipacarrierid", targetField: "ipg_ipacarrierid" },
                    //{ sourceField: "ipg_primarycarrierplantype", targetField: "ipg_primarycarrierplantype" },
                    { sourceField: "title", targetField: "ipg_caseid" },
                    //{ sourceField: "ipg_carrierid", targetField: "ipg_carrierid" },
                    { sourceField: "ipg_memberidnumber", targetField: "ipg_memberidnumber" },
                    //{ sourceField: "ipg_autocarrierid", targetField: "ipg_autocarrierid" },
                    { sourceField: "ipg_billingfax", targetField: "ipg_billingfax" },
                    { sourceField: "ipg_autoclaimnumber", targetField: "ipg_claim" },
                    { sourceField: "ipg_nursecasemgrname", targetField: "ipg_nursecasemgrname" },
                    { sourceField: "ipg_nursecasemgrphone", targetField: "ipg_nursecasemgrphone" },
                    { sourceField: "ipg_autoadjustername", targetField: "ipg_adjustername" },
                    { sourceField: "ipg_adjusterphone", targetField: "ipg_adjusterphone" },
                    { sourceField: "ipg_csrname", targetField: "ipg_csrname" },
                    { sourceField: "ipg_csrphone", targetField: "ipg_csrphone" },
                    { sourceField: "ipg_callreference", targetField: "ipg_callreference" },
                    { sourceField: "ipg_plandescription", targetField: "ipg_plandescription" },
                    { sourceField: "ipg_benefitsnotes", targetField: "ipg_benefitnotesmultiplelines" },
                    { sourceField: "ipg_deductible", targetField: "ipg_deductible" },
                    { sourceField: "ipg_deductiblemet", targetField: "ipg_deductiblemet" },
                    { sourceField: "ipg_deductibleremaining", targetField: "ipg_deductibleremainingcalc" },
                    { sourceField: "ipg_payercoinsurance", targetField: "ipg_carriercoinsurance" },
                    { sourceField: "ipg_patientcoinsurance", targetField: "ipg_patientcoinsurance" },
                    { sourceField: "ipg_oopmax", targetField: "ipg_oopmax" },
                    { sourceField: "ipg_oopmet", targetField: "ipg_oopmaxmet" },
                    { sourceField: "ipg_oopremaining", targetField: "ipg_oopmaxremainingcalc" },
                    { sourceField: "ipg_primarycarrierexpirationdate", targetField: "ipg_coverageexpirationdate" },
                    { sourceField: "ipg_benefitplantypecode", targetField: "ipg_plantypecode" },
                ];
                InitLoader.exportInsuredFields = [
                    { sourceField: Form.attributes.case.insuredaddress, targetField: Form.attributes.case.insuredaddress },
                    { sourceField: Form.attributes.case.insuredcity, targetField: Form.attributes.case.insuredcity },
                    { sourceField: Form.attributes.case.insuredstate, targetField: Form.attributes.case.insuredstate },
                    { sourceField: Form.attributes.case.insuredzipcodeid, targetField: Form.attributes.case.insuredzipcodeid },
                    { sourceField: Form.attributes.case.insuredphone, targetField: Form.attributes.case.insuredphone },
                ];
                InitLoader.exportPatientFields = [
                    { sourceField: Form.attributes.patient.patientaddress, targetField: Form.attributes.case.insuredaddress },
                    { sourceField: Form.attributes.patient.patientcity, targetField: Form.attributes.case.insuredcity },
                    { sourceField: Form.attributes.patient.patientstate, targetField: Form.attributes.case.insuredstate },
                    { sourceField: Form.attributes.patient.patientzipcodeid, targetField: Form.attributes.case.insuredzipcodeid },
                    { sourceField: Form.attributes.patient.patienthomephone, targetField: Form.attributes.case.insuredphone },
                ];
                InitLoader.autoBenefitsFields = [
                    'ipg_dateofinjury',
                    'ipg_autobenefitsexhausted',
                    'ipg_facilityautoexhaustletteronfile',
                    'ipg_claimopenandreviewingmedical',
                    'ipg_pipavailable',
                    'ipg_pipremaining',
                    'ipg_medpayavailable',
                    'ipg_medpayremaining',
                    'ipg_csrphone',
                    'ipg_csrname',
                    'ipg_callreference',
                    'ipg_benefitnotesmultiplelines',
                    'ipg_coverageeffectivedate',
                    'ipg_coverageexpirationdate',
                    'ipg_deductible',
                    'ipg_deductiblemet',
                    'ipg_carriercoinsurance',
                    'ipg_patientcoinsurance',
                    'ipg_oopmax',
                    'ipg_oopmaxmet'
                ];
            })(InitLoader || (InitLoader = {}));
            var _defaultAttributeMappings = {
                ipg_deductible: 0.00,
                ipg_deductiblemet: 0.00,
                ipg_deductibleremainingcalc: 0.00,
                ipg_carriercoinsurance: 100,
                ipg_patientcoinsurance: 0.00,
                ipg_oopmax: 0.00,
                ipg_oopmaxmet: 0.00,
                ipg_oopmaxremainingcalc: 0.00,
                ipg_coverageeffectivedate: new Date(new Date().getFullYear(), new Date("9999-01-01").getMonth(), new Date("9999-01-01").getDate()),
                ipg_coverageexpirationdate: new Date(9999, new Date("9999-12-31").getMonth(), new Date("9999-12-31").getDate()),
                ipg_inn_or_oon_code: 427880000 // IIN
            };
            function OnLoad(executionContext) {
                var _a, _b, _c;
                return __awaiter(this, void 0, void 0, function () {
                    var formContext, currentFormId, err_1;
                    return __generator(this, function (_d) {
                        switch (_d.label) {
                            case 0:
                                formContext = executionContext.getFormContext();
                                currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
                                if (currentFormId == "b304dbab-c574-4cc2-87a9-87f123f5dfb0") {
                                    (_a = formContext.getAttribute("ipg_benefitnotesmultiplelines")) === null || _a === void 0 ? void 0 : _a.setValue("Submit a copy of the Invoice with the Claim"); //Default notes
                                }
                                changeFieldPropertiesByRelation(formContext);
                                setCSRFieldsRequired(formContext);
                                setRequiredFieldsDependingOnBVF(formContext);
                                return [4 /*yield*/, filterCarrierBenefitTypes(formContext)];
                            case 1:
                                _d.sent();
                                setTimeout(function () { passContextToWebResource(formContext); }, 2000);
                                _d.label = 2;
                            case 2:
                                _d.trys.push([2, 7, 8, 9]);
                                Xrm.Utility.showProgressIndicator("Loading data");
                                return [4 /*yield*/, initPopulateData(formContext)];
                            case 3:
                                _d.sent();
                                return [4 /*yield*/, setBenefitFieldsFromCarrier(formContext)];
                            case 4:
                                _d.sent();
                                blockFields(formContext);
                                if (!(formContext.ui.getFormType() == 1 /* Create */)) return [3 /*break*/, 6];
                                if (!(formContext.ui.formSelector.getCurrentItem().getId() == FormTypes.Auto)) return [3 /*break*/, 6];
                                return [4 /*yield*/, PopulateAutoBenefitsFromLastBvf(formContext)];
                            case 5:
                                _d.sent();
                                _d.label = 6;
                            case 6:
                                (_b = formContext.getAttribute(Form.attributes.case.relationtoinsuredcode)) === null || _b === void 0 ? void 0 : _b.addOnChange(function () {
                                    initPopulateData(formContext);
                                    changeFieldPropertiesByRelation(formContext);
                                    passContextToWebResource(formContext);
                                });
                                return [3 /*break*/, 9];
                            case 7:
                                err_1 = _d.sent();
                                Xrm.Navigation.openErrorDialog({ message: (_c = err_1.message) !== null && _c !== void 0 ? _c : err_1 });
                                return [3 /*break*/, 9];
                            case 8:
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 9: return [2 /*return*/];
                        }
                    });
                });
            }
            Form.OnLoad = OnLoad;
            function SameAsPatientChange(formContext, sameAsPatient) {
                return __awaiter(this, void 0, void 0, function () {
                    var parentCaseField, parentCase, updatedExportFieldsArray;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                if (!sameAsPatient) return [3 /*break*/, 2];
                                parentCaseField = formContext.getAttribute("ipg_parentcaseid");
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", parentCaseField.getValue()[0].id)];
                            case 1:
                                parentCase = _a.sent();
                                updatedExportFieldsArray = InitLoader.exportPatientFields;
                                updateFields(updatedExportFieldsArray, parentCase, formContext);
                                return [3 /*break*/, 3];
                            case 2:
                                clearIntakeFields(formContext);
                                _a.label = 3;
                            case 3: return [2 /*return*/];
                        }
                    });
                });
            }
            Form.SameAsPatientChange = SameAsPatientChange;
            function clearIntakeFields(formContext) {
                var _a, _b, _c, _d, _e;
                (_a = formContext.getAttribute(Form.attributes.case.insuredaddress)) === null || _a === void 0 ? void 0 : _a.setValue(null);
                (_b = formContext.getAttribute(Form.attributes.case.insuredcity)) === null || _b === void 0 ? void 0 : _b.setValue(null);
                (_c = formContext.getAttribute(Form.attributes.case.insuredstate)) === null || _c === void 0 ? void 0 : _c.setValue(null);
                (_d = formContext.getAttribute(Form.attributes.case.insuredzipcodeid)) === null || _d === void 0 ? void 0 : _d.setValue(null);
                (_e = formContext.getAttribute(Form.attributes.case.insuredphone)) === null || _e === void 0 ? void 0 : _e.setValue(null);
            }
            Form.clearIntakeFields = clearIntakeFields;
            function OnSave(executionContext) {
                //CloneBVF(executionContext); Clone functionality is deprecated
            }
            Form.OnSave = OnSave;
            function changeFieldPropertiesByRelation(formContext) {
                var _a;
                var relationValue = (_a = formContext.getAttribute(Form.attributes.case.relationtoinsuredcode)) === null || _a === void 0 ? void 0 : _a.getValue();
                var isSelfStatus = relationValue == Form.attributes.optionSets.relationtoinsure.self;
                displayInsureFields(formContext, !isSelfStatus);
                setInsureFieldsRequiredStatus(formContext, !isSelfStatus);
            }
            function setInsureFieldsRequiredStatus(formContext, isRequired) {
                var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k;
                if (isRequired) {
                    (_a = formContext.getAttribute(Form.attributes.case.insuredaddress)) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required");
                    (_b = formContext.getAttribute(Form.attributes.case.insuredcity)) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("required");
                    (_c = formContext.getAttribute(Form.attributes.case.insuredstate)) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("required");
                    (_d = formContext.getAttribute(Form.attributes.case.insuredzipcodeid)) === null || _d === void 0 ? void 0 : _d.setRequiredLevel("required");
                    (_e = formContext.getAttribute(Form.attributes.case.insuredphone)) === null || _e === void 0 ? void 0 : _e.setRequiredLevel("required");
                }
                else {
                    (_f = formContext.getAttribute(Form.attributes.case.insuredaddress)) === null || _f === void 0 ? void 0 : _f.setRequiredLevel("none");
                    (_g = formContext.getAttribute(Form.attributes.case.insuredcity)) === null || _g === void 0 ? void 0 : _g.setRequiredLevel("none");
                    (_h = formContext.getAttribute(Form.attributes.case.insuredstate)) === null || _h === void 0 ? void 0 : _h.setRequiredLevel("none");
                    (_j = formContext.getAttribute(Form.attributes.case.insuredzipcodeid)) === null || _j === void 0 ? void 0 : _j.setRequiredLevel("none");
                    (_k = formContext.getAttribute(Form.attributes.case.insuredphone)) === null || _k === void 0 ? void 0 : _k.setRequiredLevel("none");
                }
            }
            function displayInsureFields(formContext, isVisible) {
                var _a, _b, _c, _d, _e, _f;
                (_a = formContext.getControl(Form.attributes.case.insuredaddress)) === null || _a === void 0 ? void 0 : _a.setVisible(isVisible);
                (_b = formContext.getControl(Form.attributes.case.insuredcity)) === null || _b === void 0 ? void 0 : _b.setVisible(isVisible);
                (_c = formContext.getControl(Form.attributes.case.insuredstate)) === null || _c === void 0 ? void 0 : _c.setVisible(isVisible);
                (_d = formContext.getControl(Form.attributes.case.insuredzipcodeid)) === null || _d === void 0 ? void 0 : _d.setVisible(isVisible);
                (_e = formContext.getControl(Form.attributes.case.insuredphone)) === null || _e === void 0 ? void 0 : _e.setVisible(isVisible);
                (_f = formContext.getControl(Form.webResources.sameAsPatientCheckbox)) === null || _f === void 0 ? void 0 : _f.setVisible(isVisible);
            }
            // This should be added to the forms OnLoad event
            function passContextToWebResource(formContext) {
                var webResouceControl = formContext.getControl(Form.webResources.sameAsPatientCheckbox);
                if (webResouceControl) {
                    webResouceControl.getContentWindow()
                        .then(function (contentWindow) {
                        //call our function to pass Xrm and formContext
                        contentWindow.setClientApiContext(Xrm, formContext);
                    });
                }
            }
            function initPopulateData(formContext) {
                var _this = this;
                return new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var parentCaseField, parentCase, err_2;
                    var _a, _b, _c, _d, _e, _f;
                    return __generator(this, function (_g) {
                        switch (_g.label) {
                            case 0:
                                parentCaseField = formContext.getAttribute("ipg_parentcaseid");
                                if (!parentCaseField || !parentCaseField.getValue()) {
                                    setDefaultGenericValues(formContext);
                                    resolve();
                                }
                                _g.label = 1;
                            case 1:
                                _g.trys.push([1, 5, , 6]);
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", parentCaseField.getValue()[0].id)];
                            case 2:
                                parentCase = _g.sent();
                                if (((_a = formContext.getAttribute(Form.attributes.case.relationtoinsuredcode)) === null || _a === void 0 ? void 0 : _a.getValue()) == Form.attributes.optionSets.relationtoinsure.self) {
                                    updateFields(InitLoader.exportInsuredFields, parentCase, formContext);
                                }
                                updateFields(InitLoader.exportFields, parentCase, formContext);
                                setDefaultGenericValues(formContext);
                                if (parentCase.ipg_actualdos) {
                                    (_b = formContext.getAttribute("ipg_surgerydate")) === null || _b === void 0 ? void 0 : _b.setValue(new Date(parentCase.ipg_actualdos));
                                }
                                else if (parentCase.ipg_surgerydate) {
                                    (_c = formContext.getAttribute("ipg_surgerydate")) === null || _c === void 0 ? void 0 : _c.setValue(new Date(parentCase.ipg_surgerydate));
                                }
                                if (parentCase.ipg_medicalbenefitsexhausted == true)
                                    (_d = formContext.getAttribute("ipg_autobenefitsexhausted")) === null || _d === void 0 ? void 0 : _d.setValue(1);
                                else
                                    (_e = formContext.getAttribute("ipg_autobenefitsexhausted")) === null || _e === void 0 ? void 0 : _e.setValue(0);
                                if (!(((_f = formContext.getAttribute("ipg_formtype")) === null || _f === void 0 ? void 0 : _f.getValue()) == 427880003)) return [3 /*break*/, 4];
                                return [4 /*yield*/, PopulateFieldsWithLastDmeBenefitRecordValues(formContext, parentCase)];
                            case 3:
                                _g.sent();
                                _g.label = 4;
                            case 4:
                                resolve();
                                return [3 /*break*/, 6];
                            case 5:
                                err_2 = _g.sent();
                                reject(err_2.message);
                                return [3 /*break*/, 6];
                            case 6: return [2 /*return*/];
                        }
                    });
                }); });
            }
            function blockFields(form) {
                //const unblockIfEmptyFields = ["ipg_carrierid", "ipg_homeplancarrierid", "ipg_primarycarriergroupidnumber", "ipg_memberidnumber","ipg_primarycarrierplantype", "ipg_autocarrierid"];
                var unblockIfEmptyFields = ["ipg_carrierid", "ipg_memberidnumber", "ipg_autocarrierid"];
                unblockIfEmptyFields.forEach(function (field) {
                    var trgAttr = form.getAttribute(field);
                    if (trgAttr && trgAttr.getValue()) {
                        trgAttr.controls.forEach(function (p) { return p.setDisabled(true); });
                    }
                });
            }
            function setBenefitFieldsFromCarrier(formContext) {
                var _this = this;
                return new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var carrierField, carrier, claimFormTypeAttribute, err_3;
                    var _this = this;
                    var _a;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                carrierField = formContext.getAttribute("ipg_carrierid");
                                if (!carrierField || !carrierField.getValue()) {
                                    resolve();
                                }
                                _b.label = 1;
                            case 1:
                                _b.trys.push([1, 3, , 4]);
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", carrierField.getValue()[0].id)];
                            case 2:
                                carrier = _b.sent();
                                if (formContext.ui.getFormType() == 2 /* Update */) {
                                    switch (carrier["ipg_carriertype"]) {
                                        case CarrierTypes.WorkersComp:
                                            ChangeForm(formContext, FormTypes.WC);
                                            break;
                                        case CarrierTypes.Auto:
                                            ChangeForm(formContext, FormTypes.Auto);
                                            break;
                                        case CarrierTypes.DME:
                                            ChangeForm(formContext, FormTypes.DME);
                                            break;
                                        default:
                                            ChangeForm(formContext, FormTypes.GeneralHealth);
                                            break;
                                    }
                                }
                                if (formContext.ui.getFormType() == 1 /* Create */) {
                                    formContext
                                        .getAttribute("ipg_billingcity")
                                        .setValue(carrier["ipg_billingaddress1_city"]);
                                    formContext
                                        .getAttribute("ipg_billingaddress")
                                        .setValue(carrier["address1_line2"]);
                                    if (carrier["_ipg_billingzipcodeid_value"]) {
                                        formContext.getAttribute("ipg_billingzipcodeid").setValue([
                                            {
                                                id: carrier["_ipg_billingzipcodeid_value"],
                                                name: carrier["_ipg_billingzipcodeid_value@OData.Community.Display.V1.FormattedValue"],
                                                entityType: carrier["_ipg_billingzipcodeid_value@Microsoft.Dynamics.CRM.lookuplogicalname"],
                                            },
                                        ]);
                                    }
                                    if (carrier["_ipg_billingstateid_value"]) {
                                        formContext.getAttribute("ipg_billingstateid").setValue([
                                            {
                                                id: carrier["_ipg_billingstateid_value"],
                                                name: carrier["_ipg_billingstateid_value@OData.Community.Display.V1.FormattedValue"],
                                                entityType: carrier["_ipg_billingstateid_value@Microsoft.Dynamics.CRM.lookuplogicalname"],
                                            },
                                        ]);
                                    }
                                    if (((_a = formContext.getAttribute("ipg_formtype")) === null || _a === void 0 ? void 0 : _a.getValue()) != 427880003) {
                                        if (carrier.ipg_carriertype === 427880006) {
                                            //Worker Comp BVF
                                            formContext.getAttribute("ipg_formtype").setValue(427880001); //worker comp BVF
                                            if (!formContext
                                                .getAttribute("ipg_benefitnotesmultiplelines")
                                                .getValue())
                                                formContext
                                                    .getAttribute("ipg_benefitnotesmultiplelines")
                                                    .setValue("Submit a copy of the Invoice with the Claim.");
                                        }
                                        else if (carrier.ipg_carriertype == 427880000) {
                                            //Auto Benefit BVF
                                            formContext.getAttribute("ipg_formtype").setValue(427880000); //Auto BVF
                                            sleep(1000).then(function () { return __awaiter(_this, void 0, void 0, function () {
                                                var parentCaseField, incident;
                                                return __generator(this, function (_a) {
                                                    switch (_a.label) {
                                                        case 0:
                                                            parentCaseField = formContext.getAttribute("header_ipg_parentcaseid");
                                                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", parentCaseField.getValue()[0].id, "?$select=ipg_facilityexhaustletteronfile")];
                                                        case 1:
                                                            incident = _a.sent();
                                                            if (incident.ipg_facilityexhaustletteronfile == true)
                                                                formContext
                                                                    .getAttribute("ipg_facilityautoexhaustletteronfile")
                                                                    .setValue(1);
                                                            else
                                                                formContext
                                                                    .getAttribute("ipg_facilityautoexhaustletteronfile")
                                                                    .setValue(0);
                                                            return [2 /*return*/];
                                                    }
                                                });
                                            }); });
                                        }
                                        else {
                                            formContext.getAttribute("ipg_formtype").setValue(427880002); //General Health
                                        }
                                    }
                                    formContext.getAttribute("ipg_source").setValue(427880000); //BVF
                                    if (carrier.ipg_claimtype) {
                                        claimFormTypeAttribute = formContext.getAttribute("ipg_claimformtypenew");
                                        claimFormTypeAttribute.setValue(carrier.ipg_claimtype);
                                    }
                                }
                                resolve();
                                return [3 /*break*/, 4];
                            case 3:
                                err_3 = _b.sent();
                                reject(err_3.message);
                                return [3 /*break*/, 4];
                            case 4: return [2 /*return*/];
                        }
                    });
                }); });
            }
            //Register the function on onSave event on all benefit forms
            function CloneBVF(executionContext) {
                var formContext = executionContext.getFormContext();
                var entityName = formContext.data.entity.getEntityName();
                var currentForm = formContext.ui.formSelector.getCurrentItem();
                if (formContext.ui.getFormType() == 2 /* Update */) {
                    var recordId = formContext.data.entity.getId();
                    var workflowId = "74b2ab0a-9687-4361-ae41-59656877ac07";
                    executeWorkflow(recordId, workflowId, executionContext.getContext().getClientUrl());
                }
            }
            Form.CloneBVF = CloneBVF;
            function setDefaultGenericValues(formContext) {
                for (var attr in _defaultAttributeMappings) {
                    if (formContext.getAttribute(attr) && !formContext.getAttribute(attr).getValue())
                        formContext.getAttribute(attr).setValue(_defaultAttributeMappings[attr]);
                }
            }
            Form.setDefaultGenericValues = setDefaultGenericValues;
            function executeWorkflow(recordId, workflowId, clientUrl) {
                var functionName = "executeWorkflow >>";
                var query = "";
                try {
                    //Define the query to execute the action
                    query = "workflows(" + workflowId.replace("}", "").replace("{", "") + ")/Microsoft.Dynamics.CRM.ExecuteWorkflow";
                    var data = {
                        "EntityId": recordId
                    };
                    var req = new XMLHttpRequest();
                    req.open("POST", clientUrl + "/api/data/v9.1/" + query, false);
                    req.setRequestHeader("Accept", "application/json");
                    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                    req.setRequestHeader("OData-MaxVersion", "4.0");
                    req.setRequestHeader("OData-Version", "4.0");
                    req.onreadystatechange = function () {
                        if (this.readyState == 4 /* complete */) {
                            req.onreadystatechange = null;
                            if (this.status == 200 || this.status == 204) {
                                //success callback this returns null since no return value available.
                                //var result = JSON.parse(this.response);
                                var alertStrings2 = { confirmButtonLabel: "Ok", text: "BVF record is cloned.", title: "Clone Record" };
                                var alertOptions2 = { height: 120, width: 260 };
                                Xrm.Navigation.openAlertDialog(alertStrings2, alertOptions2).then(function (success) { }, function (error) { });
                            }
                            else {
                                //error callback
                                var error = JSON.parse(this.response).error;
                                var alertStrings1 = { confirmButtonLabel: "Ok", text: error, title: "Error Cloning Record" };
                                var alertOptions1 = { height: 120, width: 260 };
                                Xrm.Navigation.openAlertDialog(alertStrings1, alertOptions1).then(function (success) { }, function (error) { });
                            }
                        }
                    };
                    req.send(JSON.stringify(data));
                }
                catch (e) {
                    var alertStrings = { confirmButtonLabel: "Ok", text: e, title: "Error Cloning Record" };
                    var alertOptions = { height: 120, width: 260 };
                    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) { }, function (error) { });
                }
            }
            function sleep(ms) {
                return new Promise(function (resolve) { return setTimeout(resolve, ms); });
            }
            function ChangeForm(formContext, formId) {
                var currentForm = formContext.ui.formSelector.getCurrentItem();
                var availableForms = formContext.ui.formSelector.items.get();
                if (currentForm.getId() != formId) {
                    for (var i in availableForms) {
                        var form = availableForms[i];
                        // try to find a form based on the name
                        if (form.getId() == formId) {
                            form.navigate();
                            return true;
                        }
                    }
                }
            }
            function CloseForm(formContext) {
                formContext.ui.close();
                //window.history.back();
            }
            Form.CloseForm = CloseForm;
            function DOIValueOnChange(executionContext) {
                var formContext = executionContext.getFormContext();
                if (formContext) {
                    var doiField = formContext.getAttribute("ipg_dateofinjury").getValue();
                    var todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
                    if (doiField >= todayDate) {
                        var alertStrings = { confirmButtonLabel: "Ok", text: "DOI must be prior to today’s date", title: "DOI Alert" };
                        var alertOptions = { height: 120, width: 260 };
                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                            formContext.getAttribute("ipg_dateofinjury").setValue(null);
                            console.log("Alert dialog closed");
                        }, function (error) {
                            console.log(error.message);
                        });
                    }
                }
            }
            Form.DOIValueOnChange = DOIValueOnChange;
            function PopulateAutoBenefitsFromLastBvf(formContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var incidentId, carrierId, lastBvfResult, lastBvf, _i, _a, fieldName;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                incidentId = getLookupFirstId(formContext, 'ipg_parentcaseid');
                                carrierId = getLookupFirstId(formContext, 'ipg_autocarrierid');
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_benefitsverificationform", "?$top=1&$filter=_ipg_parentcaseid_value eq " + incidentId + " and _ipg_carrierid_value eq " + carrierId + " and statecode eq " + BvfStateCodes.Active + "&$orderby=createdon desc")];
                            case 1:
                                lastBvfResult = _b.sent();
                                if (lastBvfResult.entities.length) {
                                    lastBvf = lastBvfResult.entities[0];
                                    for (_i = 0, _a = InitLoader.autoBenefitsFields; _i < _a.length; _i++) {
                                        fieldName = _a[_i];
                                        setFormFieldFromEntity(formContext, lastBvf, fieldName, fieldName);
                                    }
                                }
                                return [2 /*return*/];
                        }
                    });
                });
            }
        })(Form = benefitsVerificationForm.Form || (benefitsVerificationForm.Form = {}));
        function setCSRFieldsRequired(formContext) {
            var _a, _b, _c, _d, _e, _f;
            var formtypeid = formContext.ui.formSelector.getCurrentItem().getId();
            if (formtypeid == Form.FormTypes.GeneralHealth) {
                (_a = formContext.getAttribute("ipg_csrname")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required");
                (_b = formContext.getAttribute("ipg_csrphone")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("none");
                (_c = formContext.getAttribute("ipg_callreference")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("required");
            }
            else {
                (_d = formContext.getAttribute("ipg_csrname")) === null || _d === void 0 ? void 0 : _d.setRequiredLevel("none");
                (_e = formContext.getAttribute("ipg_csrphone")) === null || _e === void 0 ? void 0 : _e.setRequiredLevel("none");
                (_f = formContext.getAttribute("ipg_callreference")) === null || _f === void 0 ? void 0 : _f.setRequiredLevel("none");
            }
        }
        function setRequiredFieldsDependingOnBVF(formContext) {
            var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
            if (currentFormId == Form.FormTypes.GeneralHealth) {
                var requiredfield = [
                    "ipg_insuredname",
                    "ipg_insuredgendercode",
                    "ipg_relationtoinsuredcode",
                    "ipg_benefittypecode",
                    "ipg_inn_or_oon_code",
                    "ipg_plantypecode",
                    "ipg_coverageexpirationdate"
                ];
                requiredfield.forEach(function (att) { var _a; return (_a = formContext.getAttribute(att)) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required"); });
            }
            else if (currentFormId == Form.FormTypes.DME) {
                var requiredfields = [
                    "ipg_inn_or_oon_code",
                    "ipg_plantypecode",
                    "ipg_coverageexpirationdate",
                    "ipg_deductibletypecode",
                    "ipg_oopmaxtypecode",
                    "ipg_csrname",
                    "ipg_csrphone",
                    "ipg_callreference"
                ];
                requiredfields.forEach(function (att) { var _a; return (_a = formContext.getAttribute(att)) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required"); });
            }
        }
        function filterCarrierBenefitTypes(formContext) {
            return __awaiter(this, void 0, void 0, function () {
                var benefitTypeAttribute, benefitTypeControl_1, serviceTypes_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            benefitTypeAttribute = formContext.getAttribute('ipg_benefittypecode');
                            if (!benefitTypeAttribute) return [3 /*break*/, 2];
                            if (!benefitTypeAttribute.controls.getLength()) return [3 /*break*/, 2];
                            benefitTypeControl_1 = benefitTypeAttribute.controls.get(0);
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_gwservicetypecode', '?$filter=ipg_displayonbvf eq true and statecode eq 0')];
                        case 1:
                            serviceTypes_1 = _a.sent();
                            benefitTypeAttribute.getOptions().forEach(function (option) {
                                var isServiceTypeFound = serviceTypes_1.entities.some(function (b) { return b.ipg_benefittypecode == option.value; });
                                if (!isServiceTypeFound) {
                                    benefitTypeControl_1.removeOption(option.value);
                                }
                            });
                            _a.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            });
        }
        function updateFields(fields, parentCase, formContext) {
            fields.forEach(function (val) {
                setFormFieldFromEntity(formContext, parentCase, val.sourceField, val.targetField);
            });
        }
        function PopulateFieldsWithLastDmeBenefitRecordValues(formContext, parentCase) {
            return __awaiter(this, void 0, void 0, function () {
                var dmeBenefits, lastDmeBenefit;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_benefit", "?$select=ipg_inoutnetwork,ipg_plansponsor,ipg_deductible,ipg_deductiblemet,ipg_carriercoinsurance,ipg_membercoinsurance,ipg_memberoopmax,ipg_memberoopmet,ipg_eligibilitystartdate,ipg_eligibilityenddate"
                                + "&$filter=_ipg_caseid_value eq " + parentCase.incidentid + " and ipg_benefittype eq 427880040&$orderby=createdon desc")];
                        case 1:
                            dmeBenefits = _a.sent();
                            if (dmeBenefits.entities.length > 0) {
                                lastDmeBenefit = dmeBenefits.entities[0];
                                formContext.getAttribute("ipg_inn_or_oon_code").setValue(lastDmeBenefit["ipg_inoutnetwork"] ? 427880000 : 427880001);
                                formContext.getAttribute("ipg_plansponsor").setValue(lastDmeBenefit["ipg_plansponsor"]);
                                formContext.getAttribute("ipg_coverageeffectivedate").setValue(new Date(lastDmeBenefit["ipg_eligibilitystartdate"]));
                                formContext.getAttribute("ipg_coverageexpirationdate").setValue(new Date(lastDmeBenefit["ipg_eligibilityenddate"]));
                                formContext.getAttribute("ipg_deductible").setValue(lastDmeBenefit["ipg_deductible"]);
                                formContext.getAttribute("ipg_deductiblemet").setValue(lastDmeBenefit["ipg_deductiblemet"]);
                                formContext.getAttribute("ipg_carriercoinsurance").setValue(lastDmeBenefit["ipg_carriercoinsurance"]);
                                formContext.getAttribute("ipg_patientcoinsurance").setValue(lastDmeBenefit["ipg_membercoinsurance"]);
                                formContext.getAttribute("ipg_oopmax").setValue(lastDmeBenefit["ipg_memberoopmax"]);
                                formContext.getAttribute("ipg_oopmaxmet").setValue(lastDmeBenefit["ipg_memberoopmet"]);
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        function getLookupFirstId(formContext, attributeName) {
            var _a;
            var value = (_a = formContext.getAttribute(attributeName)) === null || _a === void 0 ? void 0 : _a.getValue();
            if (value) {
                return value[0].id;
            }
            return null;
        }
        function setFormFieldFromEntity(formContext, entity, sourceFieldName, targetFieldName) {
            if (entity[sourceFieldName] === undefined && entity["_" + sourceFieldName + "_value"] === undefined) {
                return;
            }
            var trgAttr = formContext.getAttribute(targetFieldName);
            if (trgAttr) {
                switch (trgAttr.getAttributeType()) {
                    case "string":
                    case "optionset":
                    case "money":
                    case "memo":
                        trgAttr.setValue(entity[sourceFieldName]);
                        break;
                    case "datetime":
                        trgAttr.setValue(new Date(entity[sourceFieldName]));
                        break;
                    case "lookup":
                        trgAttr.setValue([{
                                id: entity["_" + sourceFieldName + "_value"],
                                name: entity["_" + sourceFieldName + "_value@OData.Community.Display.V1.FormattedValue"],
                                entityType: entity["_" + sourceFieldName + "_value@Microsoft.Dynamics.CRM.lookuplogicalname"],
                            }]);
                        break;
                }
            }
        }
    })(benefitsVerificationForm = Intake.benefitsVerificationForm || (Intake.benefitsVerificationForm = {}));
})(Intake || (Intake = {}));
