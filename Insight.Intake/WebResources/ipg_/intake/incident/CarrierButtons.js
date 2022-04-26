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
    var CarrierButtons;
    (function (CarrierButtons) {
        var _a, _b, _c;
        var CarrierNumber;
        (function (CarrierNumber) {
            CarrierNumber[CarrierNumber["First"] = 1] = "First";
            CarrierNumber[CarrierNumber["Second"] = 2] = "Second";
            //Tertiary = 3
        })(CarrierNumber || (CarrierNumber = {}));
        var CarriersMapping = (_a = {},
            _a[CarrierNumber.First] = "ipg_carrierid",
            _a[CarrierNumber.Second] = "ipg_secondarycarrierid",
            _a);
        var CarrierCoinsuranceMapping = (_b = {},
            _b[CarrierNumber.First] = "ipg_payercoinsurance",
            _b[CarrierNumber.Second] = "ipg_patientcoinsurance",
            _b);
        var PatientCoinsuranceMapping = (_c = {},
            _c[CarrierNumber.First] = "ipg_carrier2carriercoinsurancedisplay",
            _c[CarrierNumber.Second] = "ipg_carrier2patientcoinsurancedisplay",
            _c);
        var Xrm = Xrm || parent.Xrm;
        var refreshBenefitsFunctionReference;
        function Init() {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var isLocked;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            isLocked = (_a = Xrm.Page.getAttribute("ipg_islocked")) === null || _a === void 0 ? void 0 : _a.getValue();
                            //@ts-ignore
                            document.getElementById('manualBenefitsButton').disabled = isLocked;
                            return [4 /*yield*/, SetEbvButtonAvailability()];
                        case 1:
                            _b.sent();
                            addEventHandlers();
                            return [2 /*return*/];
                    }
                });
            });
        }
        CarrierButtons.Init = Init;
        function SetEbvButtonAvailability() {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var disableEbv, isLocked, params, carrierIdAttributeName, carrierAttribute, carrierIdValue, carrierId, select, queryPath, response, carrier;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            disableEbv = false;
                            isLocked = (_a = Xrm.Page.getAttribute("ipg_islocked")) === null || _a === void 0 ? void 0 : _a.getValue();
                            if (!isLocked) return [3 /*break*/, 1];
                            disableEbv = true;
                            return [3 /*break*/, 4];
                        case 1:
                            params = Intake.Utility.GetWindowJsonParameters();
                            carrierIdAttributeName = CarriersMapping[params.carrierType];
                            carrierAttribute = Xrm.Page.getAttribute(carrierIdAttributeName);
                            if (!carrierAttribute) return [3 /*break*/, 4];
                            carrierIdValue = carrierAttribute.getValue();
                            if (!(carrierIdValue && carrierIdValue.length)) return [3 /*break*/, 4];
                            carrierId = carrierIdValue[0].id.replace("{", "").replace("}", "");
                            select = "$select=ipg_enableebvzirmed";
                            queryPath = "/api/data/v9.1/accounts(" + carrierId + ")?" + select;
                            return [4 /*yield*/, fetch(queryPath, {
                                    method: "GET"
                                })];
                        case 2:
                            response = _b.sent();
                            return [4 /*yield*/, response.json()];
                        case 3:
                            carrier = _b.sent();
                            if (!carrier.ipg_enableebvzirmed) {
                                disableEbv = true;
                            }
                            _b.label = 4;
                        case 4:
                            //@ts-ignore
                            document.getElementById('ebvButton').disabled = disableEbv;
                            return [2 /*return*/];
                    }
                });
            });
        }
        function VerifyBenefits() {
            return __awaiter(this, void 0, void 0, function () {
                var caseId, params, ebvCarrierNumber, memberId, memberidAttribute, alertStrings, alertOptions, postObject;
                return __generator(this, function (_a) {
                    caseId = Xrm.Page.data.entity.getId();
                    if (!caseId) {
                        throw new Error("Could not get Case ID");
                    }
                    caseId = caseId.replace('{', '').replace('}', '');
                    params = Intake.Utility.GetWindowJsonParameters();
                    if (params.carrierType == CarrierNumber.First) {
                        ebvCarrierNumber = 1;
                    }
                    else if (params.carrierType == CarrierNumber.Second) {
                        ebvCarrierNumber = 2;
                    }
                    else {
                        throw new Error("Unexpected carrier type");
                    }
                    memberId = null;
                    memberidAttribute = Xrm.Page.getAttribute("ipg_memberidnumber");
                    if (memberidAttribute) {
                        memberId = memberidAttribute.getValue();
                    }
                    if (!memberId) {
                        alertStrings = { confirmButtonLabel: "OK", text: "Member Id is empty!" };
                        alertOptions = { height: 150, width: 300 };
                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                        return [2 /*return*/];
                    }
                    postObject = { IsUserGenerated: true, CarrierNumber: ebvCarrierNumber };
                    Xrm.Utility.showProgressIndicator("Processing...");
                    parent.$.ajax({
                        method: "POST",
                        url: '/api/data/v9.0/incidents(' + caseId + ')/Microsoft.Dynamics.CRM.ipg_IPGCaseActionsVerifyBenefits',
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(postObject),
                        dataType: 'json',
                        success: function (response) {
                            return __awaiter(this, void 0, void 0, function () {
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0: return [4 /*yield*/, Xrm.Page.data.refresh(false)];
                                        case 1:
                                            _a.sent();
                                            if (refreshBenefitsFunctionReference) {
                                                refreshBenefitsFunctionReference();
                                            }
                                            Xrm.Utility.closeProgressIndicator();
                                            return [2 /*return*/];
                                    }
                                });
                            });
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            Xrm.Navigation.openErrorDialog({ message: "Benefits verification failed" });
                            Xrm.Utility.closeProgressIndicator();
                        }
                    });
                    return [2 /*return*/];
                });
            });
        }
        CarrierButtons.VerifyBenefits = VerifyBenefits;
        function OpenBvf() {
            return __awaiter(this, void 0, void 0, function () {
                var params, carrierIdAttributeName, carrierIdAttribute, carrierIdValue, carrierId, carrier, entityFormOptions, formParams, carrierCoinsuranceAttr, patientCoinsuranceAttr, otherCarrierAttributeName, otherCarrierAttribute, otherCarrierValue;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            params = Intake.Utility.GetWindowJsonParameters();
                            carrierIdAttributeName = CarriersMapping[params.carrierType];
                            carrierIdAttribute = Xrm.Page.getAttribute(carrierIdAttributeName);
                            if (!carrierIdAttribute) return [3 /*break*/, 2];
                            carrierIdValue = carrierIdAttribute.getValue();
                            if (!(carrierIdValue && carrierIdValue.length)) return [3 /*break*/, 2];
                            carrierId = carrierIdValue[0].id.toString().replace("{", "").replace("}", "");
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord('account', carrierId, '?$select=ipg_carriertype')];
                        case 1:
                            carrier = _a.sent();
                            if (carrier) {
                                entityFormOptions = {
                                    openInNewWindow: true,
                                    entityName: "ipg_benefitsverificationform",
                                    createFromEntity: {
                                        id: Xrm.Page.data.entity.getId(),
                                        name: Xrm.Page.data.entity.getPrimaryAttributeValue(),
                                        entityType: Xrm.Page.data.entity.getEntityName(),
                                    }
                                };
                                formParams = {};
                                carrierCoinsuranceAttr = Xrm.Page.getAttribute(CarrierCoinsuranceMapping[params.carrierType]);
                                if (carrierCoinsuranceAttr) {
                                    formParams["ipg_carriercoinsurance"] = carrierCoinsuranceAttr.getValue();
                                }
                                patientCoinsuranceAttr = Xrm.Page.getAttribute(PatientCoinsuranceMapping[params.carrierType]);
                                if (patientCoinsuranceAttr) {
                                    formParams["ipg_patientcoinsurance"] = patientCoinsuranceAttr.getValue();
                                }
                                if (carrier.ipg_carriertype == 427880000 /*Auto*/) {
                                    entityFormOptions.formId = "d1560309-5c0d-49d6-bddc-64495e8fc285"; //Auto Benefits Form
                                    formParams["ipg_autocarrierid"] = carrierIdValue[0].id;
                                    formParams["ipg_autocarrieridname"] = carrierIdValue[0].name;
                                    formParams["ipg_autocarrieridtype"] = carrierIdValue[0].entityType;
                                    formParams["ipg_carrierid"] = carrierIdValue[0].id;
                                    formParams["ipg_carrieridname"] = carrierIdValue[0].name;
                                    formParams["ipg_carrieridtype"] = carrierIdValue[0].entityType;
                                    otherCarrierAttributeName = void 0;
                                    if (params.carrierType == CarrierNumber.First) {
                                        otherCarrierAttributeName = CarriersMapping[CarrierNumber.Second];
                                    }
                                    else if (params.carrierType == CarrierNumber.Second) {
                                        otherCarrierAttributeName = CarriersMapping[CarrierNumber.First];
                                    }
                                    else {
                                        throw new Error("Unexpected carrier type: " + params.carrierType);
                                    }
                                    otherCarrierAttribute = Xrm.Page.getAttribute(otherCarrierAttributeName);
                                    if (otherCarrierAttribute) {
                                        otherCarrierValue = otherCarrierAttribute.getValue();
                                        if (otherCarrierValue && otherCarrierValue.length) {
                                            formParams["ipg_othercarriername1id"] = otherCarrierValue[0].id;
                                            formParams["ipg_othercarriername1idname"] = otherCarrierValue[0].name;
                                            formParams["ipg_othercarriername1idtype"] = otherCarrierValue[0].entityType;
                                        }
                                    }
                                }
                                else if (carrier.ipg_carriertype == 427880001 /*Workers Comp*/) {
                                    entityFormOptions.formId = "b304dbab-c574-4cc2-87a9-87f123f5dfb0"; //Workers Comp Form
                                    formParams["ipg_carrierid"] = carrierIdValue[0].id;
                                    formParams["ipg_carrieridname"] = carrierIdValue[0].name;
                                    formParams["ipg_carrieridtype"] = carrierIdValue[0].entityType;
                                }
                                else {
                                    entityFormOptions.formId = "8BBB1658-323B-4BCC-81EA-6E68FB6C4AA1"; //General Health BVF 
                                    formParams["ipg_carrierid"] = carrierIdValue[0].id;
                                    formParams["ipg_carrieridname"] = carrierIdValue[0].name;
                                    formParams["ipg_carrieridtype"] = carrierIdValue[0].entityType;
                                }
                                if (Xrm.Page.getAttribute("ipg_memberidnumber").getValue().toLowerCase().startsWith("jqu")) {
                                    entityFormOptions.formId = "782375fa-3b65-4251-8e2a-897c40864220"; //DME BVF
                                    formParams["ipg_formtype"] = 427880003; //DME form type
                                    formParams["ipg_benefittypecode"] = 427880040; //DME benefit type
                                }
                                Xrm.Navigation.openForm(entityFormOptions, formParams); //Default Information Form
                            }
                            _a.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            });
        }
        CarrierButtons.OpenBvf = OpenBvf;
        function setRefreshBenefitsFunctionReference(reference) {
            refreshBenefitsFunctionReference = reference;
        }
        CarrierButtons.setRefreshBenefitsFunctionReference = setRefreshBenefitsFunctionReference;
        function unload() {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    removeEventHandlers();
                    return [2 /*return*/];
                });
            });
        }
        CarrierButtons.unload = unload;
        function addEventHandlers() {
            var params = Intake.Utility.GetWindowJsonParameters();
            var carrierIdAttributeName = CarriersMapping[params.carrierType];
            var carrierAttribute = Xrm.Page.getAttribute(carrierIdAttributeName);
            if (carrierAttribute) {
                carrierAttribute.addOnChange(SetEbvButtonAvailability);
            }
        }
        function removeEventHandlers() {
            var params = Intake.Utility.GetWindowJsonParameters();
            var carrierIdAttributeName = CarriersMapping[params.carrierType];
            var carrierAttribute = Xrm.Page.getAttribute(carrierIdAttributeName);
            if (carrierAttribute) {
                carrierAttribute.removeOnChange(SetEbvButtonAvailability);
            }
        }
    })(CarrierButtons = Intake.CarrierButtons || (Intake.CarrierButtons = {}));
})(Intake || (Intake = {}));
