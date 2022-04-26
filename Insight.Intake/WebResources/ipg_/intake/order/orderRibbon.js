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
    var Order;
    (function (Order) {
        var Ribbon;
        (function (Ribbon) {
            var statecode;
            (function (statecode) {
                statecode[statecode["Closed"] = 3] = "Closed";
            })(statecode = Ribbon.statecode || (Ribbon.statecode = {}));
            var statuscode;
            (function (statuscode) {
                statuscode[statuscode["InvoiceReceived"] = 923720006] = "InvoiceReceived";
                statuscode[statuscode["InReview"] = 923720007] = "InReview";
                statuscode[statuscode["VerifiedForPayment"] = 923720008] = "VerifiedForPayment";
                statuscode[statuscode["Partial"] = 923720010] = "Partial";
            })(statuscode = Ribbon.statuscode || (Ribbon.statuscode = {}));
            var _entityName = "salesorder";
            var _communicationExceptionViewId = "B2B608FF-9738-EB11-A813-00224802E4A2";
            var _communicationExceptionControlId = "PoCommunicationExceptions";
            function RemoveFromExceptionView(selectedControl, selectedRecordIds) {
                return __awaiter(this, void 0, void 0, function () {
                    var _i, selectedRecordIds_1, recordId;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                Xrm.Utility.showProgressIndicator("Removing from the view");
                                _i = 0, selectedRecordIds_1 = selectedRecordIds;
                                _a.label = 1;
                            case 1:
                                if (!(_i < selectedRecordIds_1.length)) return [3 /*break*/, 4];
                                recordId = selectedRecordIds_1[_i];
                                return [4 /*yield*/, Xrm.WebApi.updateRecord(_entityName, recordId, {
                                        ipg_isexcludedfromexceptionsdashboard: true
                                    })];
                            case 2:
                                _a.sent();
                                _a.label = 3;
                            case 3:
                                _i++;
                                return [3 /*break*/, 1];
                            case 4:
                                Xrm.Utility.closeProgressIndicator();
                                selectedControl.refresh();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.RemoveFromExceptionView = RemoveFromExceptionView;
            function IsRemoveFromExceptionViewEnabled(selected) {
                var _a, _b, _c, _d;
                if (((_a = selected.controlDescriptor) === null || _a === void 0 ? void 0 : _a.Id) === _communicationExceptionControlId &&
                    ((_d = (_c = (_b = selected.controlDescriptor) === null || _b === void 0 ? void 0 : _b.Parameters) === null || _c === void 0 ? void 0 : _c.ViewId) === null || _d === void 0 ? void 0 : _d.toLowerCase().includes(_communicationExceptionViewId.toLowerCase()))) {
                    return true;
                }
            }
            Ribbon.IsRemoveFromExceptionViewEnabled = IsRemoveFromExceptionViewEnabled;
            /**
            * enable rule for Generate PO
            * @function Intake.Order.Ribbon.IsLastCalcRevAfterLastActualPartDate
            * @returns {boolean}
            */
            function IsLastCalcRevAfterLastActualPartDate(primaryControl, caseId) {
                return __awaiter(this, void 0, void 0, function () {
                    var lastCalcRevDate, actualParts, latestPartDate;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                lastCalcRevDate = new Date(primaryControl.getAttribute("ipg_lastcalcrevon").getValue());
                                return [4 /*yield*/, (Xrm.WebApi.retrieveMultipleRecords('ipg_casepartdetail', "?$select=ipg_casepartdetailid,modifiedon&$filter=_ipg_caseid_value eq ".concat(caseId.replace("{", "").replace("}", "").toLowerCase())))];
                            case 1:
                                actualParts = _a.sent();
                                latestPartDate = new Date(actualParts.entities[0]["modifiedon"]);
                                actualParts.entities.forEach(function (part) {
                                    var partDate = new Date(part["modifiedon"]);
                                    if (partDate > latestPartDate)
                                        latestPartDate = partDate;
                                });
                                if (latestPartDate < lastCalcRevDate)
                                    return [2 /*return*/, true];
                                return [2 /*return*/, false];
                        }
                    });
                });
            }
            Ribbon.IsLastCalcRevAfterLastActualPartDate = IsLastCalcRevAfterLastActualPartDate;
            /**
            * enable rule for Generate PO
            * @function Intake.Order.Ribbon.IsAllActualPartsLocked
            * @returns {boolean}
            */
            function IsAllActualPartsLocked(caseId) {
                return __awaiter(this, void 0, void 0, function () {
                    var actualParts, counter, _i, actualParts_1, part;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, (Xrm.WebApi.retrieveMultipleRecords('ipg_casepartdetail', "?$select=ipg_casepartdetailid,_ipg_purchaseorderid_value&$filter=_ipg_caseid_value eq ".concat(caseId.replace("{", "").replace("}", "").toLowerCase())))];
                            case 1:
                                actualParts = (_a.sent()).entities;
                                counter = 0;
                                _i = 0, actualParts_1 = actualParts;
                                _a.label = 2;
                            case 2:
                                if (!(_i < actualParts_1.length)) return [3 /*break*/, 5];
                                part = actualParts_1[_i];
                                return [4 /*yield*/, GetAmountOfLockedParts(part)];
                            case 3:
                                if (_a.sent()) {
                                    counter++;
                                }
                                _a.label = 4;
                            case 4:
                                _i++;
                                return [3 /*break*/, 2];
                            case 5: return [2 /*return*/, counter != actualParts.length];
                        }
                    });
                });
            }
            Ribbon.IsAllActualPartsLocked = IsAllActualPartsLocked;
            function GetAmountOfLockedParts(part) {
                return __awaiter(this, void 0, void 0, function () {
                    var order;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                if (!(part["_ipg_purchaseorderid_value"] != null)) return [3 /*break*/, 2];
                                return [4 /*yield*/, (Xrm.WebApi.retrieveRecord('salesorder', "".concat(part["_ipg_purchaseorderid_value"]), "?$select=statecode,statuscode"))];
                            case 1:
                                order = _a.sent();
                                if (order['statecode'] === statecode.Closed ||
                                    order['statuscode'] === statuscode.InReview ||
                                    order['statuscode'] === statuscode.InvoiceReceived ||
                                    order['statuscode'] === statuscode.Partial ||
                                    order['statuscode'] === statuscode.VerifiedForPayment) {
                                    return [2 /*return*/, true];
                                }
                                _a.label = 2;
                            case 2: return [2 /*return*/, false];
                        }
                    });
                });
            }
            function DisplayIconTooltip(rowData, userLCID) {
                var _a;
                var str = JSON.parse(rowData);
                var coldata = str.ipg_potypecode;
                var location = "ipg_/intake/img/";
                var etimated = (_a = str.ipg_isestimatedpo_Value) === null || _a === void 0 ? void 0 : _a._val;
                var imgName = "";
                var tooltip = coldata;
                if (tooltip == "TPO" && etimated) {
                    tooltip = "Estimated" + tooltip;
                }
                switch (coldata) {
                    case 'TPO': //923720000: //TPO  
                        imgName = etimated ? "PO_ETicon.gif" : "PO_Ticon.gif";
                        break;
                    case 'ZPO': //ZPO
                        imgName = "PO_Zicon.gif";
                        break;
                    case 'CPA': //CPA
                        imgName = "PO_Cicon.gif";
                        break;
                    case 'MPO': //MPO
                        imgName = "PO_MPOicon.gif";
                        break;
                    default:
                        imgName = "";
                        tooltip = "";
                        break;
                }
                var resultarray = [imgName ? location + imgName : '', tooltip];
                return resultarray;
            }
            Ribbon.DisplayIconTooltip = DisplayIconTooltip;
            function UnLockPOs(POIds, gridControl) {
                return __awaiter(this, void 0, void 0, function () {
                    var i, e_1;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                if (!POIds || POIds.length == 0) {
                                    Xrm.Navigation.openAlertDialog({ text: "Please select at least 1 record!" });
                                    return [2 /*return*/];
                                }
                                Xrm.Utility.showProgressIndicator("");
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, 6, 7, 8]);
                                i = 0;
                                _a.label = 2;
                            case 2:
                                if (!(i < POIds.length)) return [3 /*break*/, 5];
                                return [4 /*yield*/, Xrm.WebApi.updateRecord(_entityName, POIds[i], {
                                        "ipg_commstatus": 923720003,
                                        "ipg_invoicestatus": 923720000,
                                        "statecode": 0,
                                        "statuscode": 923720015,
                                        "ipg_mysurgpropostatus": 427880001 //Open
                                    })];
                            case 3:
                                _a.sent();
                                _a.label = 4;
                            case 4:
                                i++;
                                return [3 /*break*/, 2];
                            case 5: return [3 /*break*/, 8];
                            case 6:
                                e_1 = _a.sent();
                                Xrm.Navigation.openAlertDialog({ text: "There was an error on unlock PO. Please try later or Contact System Administrator!" });
                                return [3 /*break*/, 8];
                            case 7:
                                gridControl.refresh();
                                Xrm.Utility.closeProgressIndicator();
                                return [7 /*endfinally*/];
                            case 8: return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.UnLockPOs = UnLockPOs;
        })(Ribbon = Order.Ribbon || (Order.Ribbon = {}));
    })(Order = Intake.Order || (Intake.Order = {}));
})(Intake || (Intake = {}));
