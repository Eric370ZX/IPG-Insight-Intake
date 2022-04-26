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
 * @namespace Intake.CasePartDetail.Manage
 */
var Intake;
(function (Intake) {
    var CasePartDetail;
    (function (CasePartDetail) {
        var Manage;
        (function (Manage) {
            var OptionSetOption = /** @class */ (function () {
                function OptionSetOption() {
                }
                return OptionSetOption;
            }());
            var _formattedPropertySuffix = '@OData.Community.Display.V1.FormattedValue';
            var _poTypes = [
                { value: 923720000, text: 'TPO' },
                { value: 923720001, text: 'ZPO' },
                { value: 923720002, text: 'CPA' },
                { value: 923720004, text: 'MPO' }
            ];
            //controls
            var _partsFormSelector = '#partsForm';
            var _actualPartIdDataKey = 'actualPartId';
            var _productIdSelector = '[name="productId"]';
            var _partNumberSelector = '[name="partNumber"]';
            var _partDescriptionSelector = '[name="partDescription"]';
            var _mfgNameSelector = '[name="mfgName"]';
            var _boxQtySelector = '[name="boxQty"]';
            var _keywordSelector = '[name="keyword"]';
            var _priceBookSelector = '[name="priceBook"]';
            var _claimTierSelector = '[name="claimTier"]';
            var _qtyImplantedSelector = '[name="qtyImplanted"]';
            var _qtyWastedSelector = '[name="qtyWasted"]';
            var _serialNumberSelector = '[name="serialNumber"]';
            var _lotNumberSelector = '[name="lotNumber"]';
            var _poTypeSelector = '[name="poType"]';
            var _unitCostOverrideSelector = '[name="unitCostOverride"]';
            var _unitShippingSelector = '[name="unitShipping"]';
            var _unitTaxSelector = '[name="unitTax"]';
            var _msrpSelector = '[name="msrp"]';
            var _ipgDiscountPriceSelector = '[name="ipgDiscountPrice"]';
            var _aicSelector = '[name="aic"]';
            var _incidentId = null;
            function init() {
                _incidentId = Intake.Utility.GetDataParam();
                if (!_incidentId) {
                    parent.Xrm.Navigation.openErrorDialog({ message: 'Error! Could not find Case Primary Carrier and consequently check Unsupported parts' });
                }
                loadData();
                $("#searchInput").select2({
                    width: 300,
                    minimumInputLength: 3,
                    query: function (query) {
                        //todo: request D365, migrate to select2 v4 (v4 was offline at the time of writing this code)
                        var data = { results: [] }, i, j, s;
                        for (i = 1; i < 5; i++) {
                            s = "";
                            for (j = 0; j < i; j++) {
                                s = s + query.term;
                            }
                            data.results.push({ id: query.term + i, text: s });
                        }
                        query.callback(data);
                    }
                });
            }
            Manage.init = init;
            function loadData() {
                parent.Xrm.WebApi.retrieveRecord('incident', _incidentId, '?$select=title,ipg_patientfullname').then(function (incidentResult) {
                    $('#caseTitleHeader').text(incidentResult.title + ' - ' + incidentResult.ipg_patientfullname);
                }, function (error) {
                    parent.Xrm.Navigation.openErrorDialog({ message: error.message });
                });
                parent.Xrm.WebApi.retrieveMultipleRecords('ipg_casepartdetail', '?$expand=ipg_productid(' +
                    '$select=ipg_manufacturerpartnumber,name,_ipg_manufacturerid_value,ipg_boxquantity,ipg_ipgpartnumber,ipg_claimtier,ipg_msrp)' +
                    ("&$filter=ipg_caseid/incidentid eq '" + _incidentId + "'")).then(function (actualPartsResult) {
                    var actualPartTemplateHtml = $('#actualPart-template').html();
                    for (var i = 0; i < actualPartsResult.entities.length; i++) {
                        var actualPartTemplate = $(actualPartTemplateHtml).clone();
                        var actualPart = actualPartsResult.entities[i];
                        actualPartTemplate.data(_actualPartIdDataKey, actualPart.ipg_casepartdetailid);
                        //column1
                        actualPartTemplate.find(_partNumberSelector).val(actualPart.ipg_productid.ipg_manufacturerpartnumber); //TODO: probably we can use productnumber instead of ipg_manufacturerpartnumber
                        actualPartTemplate.find(_partDescriptionSelector).val(actualPart.ipg_productid.name);
                        actualPartTemplate.find(_mfgNameSelector).val(actualPart.ipg_productid['_ipg_manufacturerid_value' + _formattedPropertySuffix]);
                        actualPartTemplate.find(_boxQtySelector).val(actualPart.ipg_productid.ipg_boxquantity);
                        actualPartTemplate.find(_keywordSelector).val(actualPart.ipg_productid.ipg_ipgpartnumber);
                        actualPartTemplate.find(_priceBookSelector).val('TODO');
                        actualPartTemplate.find(_claimTierSelector).val(actualPart.ipg_productid['ipg_claimtier' + _formattedPropertySuffix]);
                        //column2
                        actualPartTemplate.find(_qtyImplantedSelector).val(actualPart.ipg_quantity);
                        actualPartTemplate.find(_qtyWastedSelector).val(actualPart.ipg_quantitywasted);
                        actualPartTemplate.find(_serialNumberSelector).val(actualPart.ipg_serialnumber);
                        actualPartTemplate.find(_lotNumberSelector).val(actualPart.ipg_lotnumber);
                        //column3
                        var poTypeSelect = actualPartTemplate.find(_poTypeSelector);
                        populateOptionSetSelect(poTypeSelect[0], _poTypes);
                        poTypeSelect.val(actualPart.ipg_potypecode);
                        actualPartTemplate.find(_unitCostOverrideSelector).val(actualPart.ipg_enteredunitcost);
                        actualPartTemplate.find(_unitShippingSelector).val(actualPart.ipg_enteredshipping);
                        actualPartTemplate.find(_unitTaxSelector).val(actualPart.ipg_enteredtax);
                        actualPartTemplate.find(_msrpSelector).val(actualPart.ipg_productid.ipg_msrp);
                        actualPartTemplate.find(_ipgDiscountPriceSelector).val(actualPart.ipg_costprice);
                        actualPartTemplate.find(_aicSelector).val(actualPart.ipg_mac);
                        $(_partsFormSelector).append(actualPartTemplate);
                    }
                }, function (error) {
                    parent.Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
            function populateOptionSetSelect(selectElement, options) {
                for (var _i = 0, options_1 = options; _i < options_1.length; _i++) {
                    var option = options_1[_i];
                    var newOption = document.createElement('option');
                    newOption.value = option.value.toString();
                    newOption.text = option.text;
                    selectElement.add(newOption);
                }
            }
            function saveData() {
                return __awaiter(this, void 0, void 0, function () {
                    var errorMessage;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                errorMessage = validateForm();
                                if (errorMessage) {
                                    parent.Xrm.Navigation.openErrorDialog({ message: errorMessage });
                                    return [2 /*return*/];
                                }
                                return [4 /*yield*/, deleteParts()];
                            case 1:
                                _a.sent();
                                return [4 /*yield*/, addOrUpdateParts()];
                            case 2:
                                _a.sent();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Manage.saveData = saveData;
            function deleteParts() {
                return __awaiter(this, void 0, void 0, function () {
                    var actualParts, actualPartsToDelete, uiParts, _loop_1, _i, actualParts_1, actualPart, _a, actualPartsToDelete_1, actualPartToDelete;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0: return [4 /*yield*/, parent.Xrm.WebApi.retrieveMultipleRecords('ipg_casepartdetail', '?$select=ipg_casepartdetailid' +
                                    ("&$filter=ipg_caseid/incidentid eq '" + _incidentId + "'")).then(function (actualPartsResult) {
                                    return actualPartsResult.entities;
                                })];
                            case 1:
                                actualParts = _b.sent();
                                actualPartsToDelete = [];
                                uiParts = $("[data-" + _actualPartIdDataKey + "]");
                                _loop_1 = function (actualPart) {
                                    var uiPart = uiParts.filter(function (i, el) { return $(el).data(_actualPartIdDataKey) == actualPart.ipg_casepartdetailid; });
                                    if (uiPart.length == 0) {
                                        actualPartsToDelete.push(actualPart);
                                    }
                                };
                                for (_i = 0, actualParts_1 = actualParts; _i < actualParts_1.length; _i++) {
                                    actualPart = actualParts_1[_i];
                                    _loop_1(actualPart);
                                }
                                _a = 0, actualPartsToDelete_1 = actualPartsToDelete;
                                _b.label = 2;
                            case 2:
                                if (!(_a < actualPartsToDelete_1.length)) return [3 /*break*/, 5];
                                actualPartToDelete = actualPartsToDelete_1[_a];
                                return [4 /*yield*/, parent.Xrm.WebApi.deleteRecord('ipg_casepartdetail', actualPartToDelete.ipg_casepartdetailid).then(function success() { }, function (error) {
                                        parent.Xrm.Navigation.openErrorDialog({ message: error.message });
                                    })];
                            case 3:
                                _b.sent();
                                _b.label = 4;
                            case 4:
                                _a++;
                                return [3 /*break*/, 2];
                            case 5: return [2 /*return*/];
                        }
                    });
                });
            }
            function addOrUpdateParts() {
                return __awaiter(this, void 0, void 0, function () {
                    var uiParts, _loop_2, i;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                uiParts = $(_partsFormSelector).find("[data-" + _actualPartIdDataKey + "]");
                                _loop_2 = function (i) {
                                    var uiActualPart, uiActualPartId, createModel, updateModel;
                                    return __generator(this, function (_a) {
                                        switch (_a.label) {
                                            case 0:
                                                uiActualPart = uiParts.eq(i);
                                                uiActualPartId = uiActualPart.data(_actualPartIdDataKey);
                                                if (!((uiActualPartId || '').trim() == '')) return [3 /*break*/, 2];
                                                createModel = buildCreateModel(uiActualPart);
                                                return [4 /*yield*/, parent.Xrm.WebApi.createRecord('ipg_casepartdetail', createModel).then(function success(creationResult) {
                                                        uiActualPart.data(_actualPartIdDataKey, creationResult.id);
                                                    }, function (error) {
                                                        parent.Xrm.Navigation.openErrorDialog({ message: error.message });
                                                    })];
                                            case 1:
                                                _a.sent();
                                                return [3 /*break*/, 4];
                                            case 2:
                                                updateModel = buildUpdateModel(uiActualPart);
                                                return [4 /*yield*/, parent.Xrm.WebApi.updateRecord('ipg_casepartdetail', uiActualPartId, updateModel).then(function success() { }, function (error) {
                                                        parent.Xrm.Navigation.openErrorDialog({ message: error.message });
                                                    })];
                                            case 3:
                                                _a.sent();
                                                _a.label = 4;
                                            case 4: return [2 /*return*/];
                                        }
                                    });
                                };
                                i = 0;
                                _a.label = 1;
                            case 1:
                                if (!(i < uiParts.length)) return [3 /*break*/, 4];
                                return [5 /*yield**/, _loop_2(i)];
                            case 2:
                                _a.sent();
                                _a.label = 3;
                            case 3:
                                i++;
                                return [3 /*break*/, 1];
                            case 4: return [2 /*return*/];
                        }
                    });
                });
            }
            function buildCreateModel(uiPart) {
                var model = buildUpdateModel(uiPart);
                model['ipg_caseid@odata.bind'] = "/incidents(" + _incidentId + ")";
                model['ipg_productid@odata.bind'] = "/products(" + uiPart.find(_productIdSelector).val() + ")";
                return model;
            }
            function buildUpdateModel(uiPart) {
                var quantityString = uiPart.find(_qtyImplantedSelector).val().toString().trim();
                var quantityWastedString = uiPart.find(_qtyWastedSelector).val().toString().trim();
                var poTypeCodeString = uiPart.find(_poTypeSelector).val().toString().trim();
                var unitCostOverrideString = uiPart.find(_unitCostOverrideSelector).val().toString().trim();
                var enteredShippingString = uiPart.find(_unitShippingSelector).val().toString().trim();
                var enteredTaxString = uiPart.find(_unitTaxSelector).val().toString().trim();
                var model = {
                    //column2
                    'ipg_quantity': quantityString != '' ? Number(quantityString) : null,
                    'ipg_quantitywasted': quantityWastedString != '' ? Number(quantityWastedString) : null,
                    'ipg_serialnumber': uiPart.find(_serialNumberSelector).val(),
                    'ipg_lotnumber': uiPart.find(_serialNumberSelector).val(),
                    //column3
                    'ipg_potypecode': poTypeCodeString != '' ? Number(poTypeCodeString) : null,
                    'ipg_enteredunitcost': unitCostOverrideString != '' ? Number(unitCostOverrideString) : null,
                    'ipg_enteredshipping': enteredShippingString != '' ? Number(enteredShippingString) : null,
                    'ipg_enteredtax': enteredTaxString != '' ? Number(enteredTaxString) : null
                };
                return model;
            }
            function remove(button) {
                if (confirm('Remove this part?')) {
                    button.closest('[data-actualPartId]').remove();
                }
            }
            Manage.remove = remove;
            function validateForm() {
                return '';
            }
            function advancedSearch(button) {
                var filterString = '';
                var partNumber = $('#advSearchPartNumber').val().toString().trim();
                if (partNumber) {
                    filterString += "ipg_manufacturerproductnumber eq '" + encodeURIComponent(partNumber) + "'";
                }
                var optionsString = '';
                if (filterString) {
                }
                parent.Xrm.WebApi.retrieveMultipleRecords('product', optionsString).then(function (productsResult) {
                    //todo: advanced search results
                }, function (error) {
                    parent.Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
            Manage.advancedSearch = advancedSearch;
        })(Manage = CasePartDetail.Manage || (CasePartDetail.Manage = {}));
    })(CasePartDetail = Intake.CasePartDetail || (Intake.CasePartDetail = {}));
})(Intake || (Intake = {}));
