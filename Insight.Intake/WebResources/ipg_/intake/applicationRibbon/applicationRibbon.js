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
    var ApplicationRibbon;
    (function (ApplicationRibbon) {
        var _duplicateKeyPrefix = "ipg-crm-duplicate";
        var _duplicateSaveEvent = 666;
        var _dateOptions = { year: 'numeric', month: 'numeric', day: 'numeric' };
        var _stringComparisonRegex = /[^a-zA-Z0-9]/g;
        var _duplicateIgnoreAttributes = [
            "address1_addressid",
            "address2_addressid",
            "ipg_externalid",
            "ipg_zirmedid"
        ];
        var _isOnSaveValidationFired = false;
        function OpenDuplicateForm(primaryControl, recordId, entityName) {
            var formParameters = {};
            //Xrm.Page.getAttribute("ipg_effectivedate").setValue(null);
            //Xrm.Page.getAttribute("ipg_expirationdate").setValue(null);
            primaryControl.data.entity.attributes.forEach(function (x) {
                //if (primaryControl.getControl(x.getName()).getVisible()) {
                if (_duplicateIgnoreAttributes.indexOf(x.getName()) === -1)
                    formParameters[x.getName()] = x.getValue();
                //}
            });
            sessionStorage.setItem(_duplicateKeyPrefix + "-" + entityName, recordId);
            Xrm.Navigation.navigateTo({
                pageType: "entityrecord",
                entityName: entityName,
                data: formParameters
            }, {
                position: 1,
                target: 1
            });
        }
        ApplicationRibbon.OpenDuplicateForm = OpenDuplicateForm;
        function IsDuplicateEnabled(primaryControl, recordId, entityName) {
            var entities = Intake.Configs.DuplicateButton.GetEnabledEntities();
            return (entities === null || entities === void 0 ? void 0 : entities.indexOf(entityName)) > -1;
        }
        ApplicationRibbon.IsDuplicateEnabled = IsDuplicateEnabled;
        function SetOnSaveValidation(primaryControl, recordId, entityName) {
            if (_isOnSaveValidationFired)
                return false;
            _isOnSaveValidationFired = true;
            function onSaveDuplicateValidation(executionContext) {
                return __awaiter(this, void 0, void 0, function () {
                    var saveMode, originalRecordId, originalRecord, attributes, isCompleteMatch, _i, attributes_1, attr, alertSettings, alertOptions, saveOptions;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                saveMode = executionContext.getEventArgs().getSaveMode();
                                if (saveMode === _duplicateSaveEvent) {
                                    primaryControl.data.entity.removeOnSave(onSaveDuplicateValidation);
                                    return [2 /*return*/];
                                }
                                originalRecordId = sessionStorage.getItem(_duplicateKeyPrefix + "-" + entityName);
                                if (!originalRecordId) return [3 /*break*/, 2];
                                executionContext.getEventArgs().preventDefault();
                                return [4 /*yield*/, Xrm.WebApi.retrieveRecord(entityName, originalRecordId)];
                            case 1:
                                originalRecord = _a.sent();
                                attributes = primaryControl.data.entity.attributes.get();
                                isCompleteMatch = true;
                                for (_i = 0, attributes_1 = attributes; _i < attributes_1.length; _i++) {
                                    attr = attributes_1[_i];
                                    if (_duplicateIgnoreAttributes.indexOf(attr.getName()) > -1)
                                        continue;
                                    if (!IsFormAttributeEqualToValueFromObject(attr, originalRecord)) {
                                        isCompleteMatch = false;
                                        break;
                                    }
                                }
                                if (isCompleteMatch) {
                                    alertSettings = {
                                        confirmButtonLabel: "Ok",
                                        text: "In order to create a record via duplicate functionality you should change at least one field on the form",
                                        title: "Record duplication"
                                    };
                                    alertOptions = { height: 120, width: 260 };
                                    Xrm.Navigation.openAlertDialog(alertSettings, alertOptions);
                                }
                                else {
                                    primaryControl.data.entity.removeOnSave(onSaveDuplicateValidation);
                                    sessionStorage.removeItem(_duplicateKeyPrefix + "-" + entityName);
                                    saveOptions = {
                                        saveMode: _duplicateSaveEvent
                                    };
                                    primaryControl.data.save(saveOptions);
                                }
                                _a.label = 2;
                            case 2: return [2 /*return*/];
                        }
                    });
                });
            }
            primaryControl.data.entity.removeOnSave(onSaveDuplicateValidation);
            primaryControl.data.entity.addOnSave(onSaveDuplicateValidation);
            //This return is required because this function is used as enable rule
            return false;
        }
        ApplicationRibbon.SetOnSaveValidation = SetOnSaveValidation;
        function EmptyFunction() {
            return true;
        }
        ApplicationRibbon.EmptyFunction = EmptyFunction;
        function GoToNextGen() {
            //debugger;
            //get CalcRev application environment suffix
            var env = getEnvironment();
            var envSuffix;
            if (env) {
                envSuffix = '-' + env;
            }
            else {
                envSuffix = '';
            }
            location.href = "https://insight-calcrev" + envSuffix + ".azurewebsites.net";
        }
        ApplicationRibbon.GoToNextGen = GoToNextGen;
        function IsFormAttributeEqualToValueFromObject(attr, record) {
            var _a, _b, _c;
            switch (attr.getAttributeType()) {
                case "string":
                case "memo":
                    return ((_a = record[attr.getName()]) === null || _a === void 0 ? void 0 : _a.replace(_stringComparisonRegex, '').toLowerCase()) === ((_b = attr.getValue()) === null || _b === void 0 ? void 0 : _b.replace(_stringComparisonRegex, '').toLowerCase());
                case "multiselectoptionset": {
                    var value = attr.getValue();
                    var recordValue_1 = (_c = record[attr.getName()]) === null || _c === void 0 ? void 0 : _c.split(',');
                    if (value && recordValue_1)
                        return value.every(function (v) { return recordValue_1.indexOf(v.toString()) > -1; });
                    return !value && !recordValue_1;
                }
                case "datetime": {
                    var attrValue = attr.getValue();
                    var apiValue = record[attr.getName()];
                    if (attrValue && apiValue)
                        return attrValue.toLocaleDateString('en-US', _dateOptions) === new Date(apiValue).toLocaleDateString('en-US', _dateOptions);
                    return !attrValue && !apiValue;
                }
                case "lookup": {
                    var apiAttributeName = "_" + attr.getName() + "_value";
                    var lookupValue = attr.getValue();
                    if (lookupValue && lookupValue.length > 0) {
                        var id = lookupValue[0].id.replace("}", "").replace("{", "").toLowerCase();
                        return id === record[apiAttributeName];
                    }
                    return !lookupValue && !record[apiAttributeName];
                }
                default:
                    return record[attr.getName()] === attr.getValue();
            }
        }
        function getEnvironment() {
            if (location.host.indexOf('-dev') >= 0) {
                return 'dev';
            }
            else if (location.host.indexOf('-qa') >= 0) {
                return 'qa';
            }
            else if (location.host.indexOf('-prd') >= 0) {
                return 'prd';
            }
            else {
                return '';
            }
        }
    })(ApplicationRibbon = Intake.ApplicationRibbon || (Intake.ApplicationRibbon = {}));
})(Intake || (Intake = {}));
