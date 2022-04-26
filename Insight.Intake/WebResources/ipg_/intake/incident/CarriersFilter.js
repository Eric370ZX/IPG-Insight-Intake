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
    var CarriersFilter;
    (function (CarriersFilter) {
        var TAB_ID = 'InsuranceBenefitsAuth';
        var CARRIER1_SECTION_ID = 'Carrier1Section';
        var CARRIER1_AUTH_SECTION_ID = 'Carrier1AuthSection';
        var CARRIER2_SECTION_ID = 'Carrier2Section';
        var CARRIER2_AUTH_SECTION_ID = 'Carrier2AuthSection';
        var carrierNameElement;
        var carrier1Radio;
        var carrier1Label;
        var carrier2Radio;
        var carrier2Label;
        var addCarrierButton;
        var currentCarrierValue;
        var Xrm = Xrm || window.parent.Xrm;
        function init() {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            getControls();
                            currentCarrierValue = getSelectedCarrierValue();
                            return [4 /*yield*/, refreshView()];
                        case 1:
                            _a.sent();
                            addEventListeners();
                            return [2 /*return*/];
                    }
                });
            });
        }
        CarriersFilter.init = init;
        /*
         * This event handler may not be needed because users do not loose changes when switch displayed carriers
         */
        function onCarrierClick(event) {
            var returnValue = true;
            var newCarrierValue = getSelectedCarrierValue();
            if (newCarrierValue != currentCarrierValue) {
                var currentSectionId_1 = getCurrentSectionId();
                var isDirty_1 = isDirtySection(currentSectionId_1);
                if (isDirty_1) {
                    returnValue = false;
                    Xrm.Page.data.refresh(false).then(function () {
                        isDirty_1 = isDirtySection(currentSectionId_1);
                        if (!isDirty_1) {
                            setSelectedCarrierValue(newCarrierValue);
                            onCarrierChanged();
                        }
                    });
                }
            }
            return returnValue;
        }
        CarriersFilter.onCarrierClick = onCarrierClick;
        function onCarrierChanged() {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            currentCarrierValue = getSelectedCarrierValue();
                            return [4 /*yield*/, refreshView()];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                    }
                });
            });
        }
        CarriersFilter.onCarrierChanged = onCarrierChanged;
        function onAddCarrierClick() {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    Xrm.Navigation.openForm({
                        entityName: 'ipg_casecarrieraddingparameters',
                        useQuickCreateForm: true
                    }, {
                        ipg_caseid: Xrm.Page.data.entity.getId().replace('{', '').replace('}', '')
                    }).then(function (success) {
                        return __awaiter(this, void 0, void 0, function () {
                            var newCaseCarrierId;
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0:
                                        if (!(success.savedEntityReference && success.savedEntityReference.length)) return [3 /*break*/, 2];
                                        newCaseCarrierId = success.savedEntityReference[0].id.replace('{', '').replace('}', '');
                                        return [4 /*yield*/, Xrm.WebApi.deleteRecord('ipg_casecarrieraddingparameters', newCaseCarrierId)];
                                    case 1:
                                        _a.sent();
                                        if (Xrm.Page.data.refresh) {
                                            Xrm.Page.data.refresh(true);
                                        }
                                        _a.label = 2;
                                    case 2: return [2 /*return*/];
                                }
                            });
                        });
                    }, function (error) {
                        Xrm.Navigation.openErrorDialog({ message: error.message });
                    });
                    return [2 /*return*/];
                });
            });
        }
        CarriersFilter.onAddCarrierClick = onAddCarrierClick;
        function getControls() {
            carrierNameElement = document.getElementById('carrierNameElement');
            carrier1Radio = document.getElementById('carrier1Radio');
            carrier2Radio = document.getElementById('carrier2Radio');
            carrier1Label = document.getElementById('carrier1Label');
            carrier2Label = document.getElementById('carrier2Label');
            addCarrierButton = document.getElementById('addCarrierButton');
        }
        function getSelectedCarrierValue() {
            if (carrier1Radio.checked) {
                return carrier1Radio.value;
            }
            if (carrier2Radio.checked) {
                return carrier2Radio.value;
            }
        }
        function setSelectedCarrierValue(carrierValue) {
            if (carrierValue == 'carrier1') {
                carrier1Radio.checked = true;
            }
            if (carrierValue == 'carrier2') {
                carrier2Radio.checked = true;
            }
        }
        function addEventListeners() {
            var carrier1Attribute = Xrm.Page.getAttribute('ipg_carrierid');
            if (carrier1Attribute) {
                carrier1Attribute.addOnChange(refreshView); //todo: replace with Form OnSave because AddCarrier button should not be available until the form is saved
            }
            var carrier2Attribute = Xrm.Page.getAttribute('ipg_secondarycarrierid');
            if (carrier2Attribute) {
                carrier2Attribute.addOnChange(refreshView); //todo: replace with Form OnSave because AddCarrier button should not be available until the form is saved
            }
            var isLockedAttribute = Xrm.Page.getAttribute('ipg_islocked');
            if (isLockedAttribute) {
                isLockedAttribute.addOnChange(refreshView);
            }
        }
        function getCurrentSectionId() {
            if (carrier1Radio.value == currentCarrierValue) {
                return CARRIER1_SECTION_ID;
            }
            if (carrier2Radio.value == currentCarrierValue) {
                return CARRIER2_SECTION_ID;
            }
        }
        function refreshView() {
            return __awaiter(this, void 0, void 0, function () {
                var carrier1Name, isCarrier1Wc, carrier1Attribute, carrier1Value, carrierResult, carrier2Name, carrier2Attribute, carrier2Value, isCaseLocked, isLockedAttribute;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            carrier1Name = '';
                            isCarrier1Wc = false;
                            carrier1Attribute = Xrm.Page.getAttribute('ipg_carrierid');
                            if (!carrier1Attribute) return [3 /*break*/, 3];
                            carrier1Value = carrier1Attribute.getValue();
                            if (!(carrier1Value && carrier1Value.length)) return [3 /*break*/, 2];
                            carrier1Name = carrier1Value[0].name;
                            carrier1Label.innerText = carrier1Name + ' (Primary)';
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", carrier1Value[0].id, "?$select=ipg_carriertype")];
                        case 1:
                            carrierResult = _a.sent();
                            if (carrierResult && carrierResult.ipg_carriertype == 427880001 /*WC*/) {
                                isCarrier1Wc = true;
                            }
                            return [3 /*break*/, 3];
                        case 2:
                            carrier1Name = '';
                            carrier1Label.innerText = '';
                            _a.label = 3;
                        case 3:
                            carrier2Name = '';
                            carrier2Attribute = Xrm.Page.getAttribute('ipg_secondarycarrierid');
                            if (carrier2Attribute) {
                                carrier2Value = carrier2Attribute.getValue();
                                if (carrier2Value && carrier2Value.length) {
                                    carrier2Name = carrier2Value[0].name;
                                    carrier2Label.innerText = carrier2Name + ' (Secondary)';
                                    carrier2Radio.style.display = 'inline';
                                    carrier2Label.style.display = 'inline';
                                }
                                else {
                                    carrier2Name = '';
                                    carrier2Label.innerText = '';
                                    carrier2Radio.style.display = 'none';
                                    carrier2Label.style.display = 'none';
                                }
                            }
                            if (carrier2Radio.checked) {
                                carrierNameElement.innerText = carrier2Name;
                                showCarrier2();
                            }
                            else {
                                carrierNameElement.innerText = carrier1Name;
                                showCarrier1();
                            }
                            isCaseLocked = false;
                            isLockedAttribute = Xrm.Page.getAttribute('ipg_islocked');
                            if (isLockedAttribute) {
                                isCaseLocked = isLockedAttribute.getValue();
                            }
                            addCarrierButton.style.display = (carrier2Name || isCarrier1Wc || isCaseLocked) ? 'none' : 'inline';
                            return [2 /*return*/];
                    }
                });
            });
        }
        function isDirtySection(sectionId) {
            var section = Xrm.Page.ui.tabs.get(TAB_ID).sections.get(sectionId);
            var isDirty = false;
            for (var i = 0; i < section.controls.getLength(); i++) {
                var control = section.controls.getByIndex(i);
                if (control.getAttribute) {
                    var attribute = control.getAttribute();
                    if (attribute && attribute.getIsDirty()) {
                        isDirty = true;
                        break;
                    }
                }
            }
            return isDirty;
        }
        function showCarrier1() {
            Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER2_SECTION_ID).setVisible(false);
            Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER2_AUTH_SECTION_ID).setVisible(false);
            Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER1_SECTION_ID).setVisible(true);
            Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER1_AUTH_SECTION_ID).setVisible(true);
        }
        function showCarrier2() {
            Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER1_SECTION_ID).setVisible(false);
            Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER1_AUTH_SECTION_ID).setVisible(false);
            Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER2_SECTION_ID).setVisible(true);
            Xrm.Page.ui.tabs.get(TAB_ID).sections.get(CARRIER2_AUTH_SECTION_ID).setVisible(true);
        }
    })(CarriersFilter = Intake.CarriersFilter || (Intake.CarriersFilter = {}));
})(Intake || (Intake = {}));
