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
 * @namespace Intake.EHRStaging
 */
var Intake;
(function (Intake) {
    var EHRStaging;
    (function (EHRStaging) {
        /**
         * Resubmits selected EHRStaging records to be processed again.
         * @function Intake.EHRStaging.Resubmit
         * @returns {void}
         */
        function Resubmit(selectedItemIds, primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            //debugger;
                            if (!selectedItemIds || !selectedItemIds.length) {
                                alert('EHRStaging record ID is required');
                                return [2 /*return*/];
                            }
                            return [4 /*yield*/, DoActionWithProgressIndicator('EHRResubmitURL', selectedItemIds, primaryControl)];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                    }
                });
            });
        }
        EHRStaging.Resubmit = Resubmit;
        function OkPurge(selectedItemIds, primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!selectedItemIds || !selectedItemIds.length) {
                                alert('EHRStaging record ID is required');
                                return [2 /*return*/];
                            }
                            return [4 /*yield*/, DoActionWithProgressIndicator('EHROkPurgeURL', selectedItemIds, primaryControl)];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                    }
                });
            });
        }
        EHRStaging.OkPurge = OkPurge;
        function isRejectedView(primaryControl) {
            return primaryControl.getViewSelector
                && primaryControl.getViewSelector().getCurrentView().name.toLowerCase().indexOf('rejected') > -1;
        }
        EHRStaging.isRejectedView = isRejectedView;
        function AreResubmittableRecords(selectedItemIds, primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var filterValues, fetchXml, retrievalResult;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            //debugger;
                            if (!selectedItemIds || !selectedItemIds.length) {
                                return [2 /*return*/, false];
                            }
                            filterValues = "";
                            selectedItemIds.forEach(function (id) {
                                filterValues += "\n<value>" + id + "</value>";
                            });
                            fetchXml = '?fetchXml=' + encodeURIComponent("<fetch>\n  <entity name=\"ipg_ehrstaging\">\n    <attribute name=\"ipg_ehrstagingid\" />\n    <attribute name=\"ipg_status\" />\n    <filter>\n      <condition attribute=\"ipg_ehrstagingid\" operator=\"in\">" +
                                filterValues +
                                "</condition>\n    </filter>\n  </entity>\n</fetch>");
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords('ipg_ehrstaging', fetchXml)];
                        case 1:
                            retrievalResult = _a.sent();
                            return [2 /*return*/, retrievalResult.entities.every(function (e) { return e.ipg_status == 'ERROR'; })];
                    }
                });
            });
        }
        EHRStaging.AreResubmittableRecords = AreResubmittableRecords;
        function DoActionWithProgressIndicator(urlSettingName, selectedItemIds, primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var error_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            Xrm.Utility.showProgressIndicator("Processing...");
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 3, , 4]);
                            return [4 /*yield*/, DoAction(urlSettingName, selectedItemIds, primaryControl)];
                        case 2:
                            _a.sent();
                            return [3 /*break*/, 4];
                        case 3:
                            error_1 = _a.sent();
                            Xrm.Utility.closeProgressIndicator();
                            Xrm.Navigation.openErrorDialog({ message: error_1.message || error_1.responseText });
                            return [3 /*break*/, 4];
                        case 4:
                            Xrm.Utility.closeProgressIndicator();
                            return [2 /*return*/];
                    }
                });
            });
        }
        function DoAction(urlSettingName, selectedItemIds, primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var retrievalResult, uri;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", "?$filter=ipg_name eq '" + urlSettingName + "'")];
                        case 1:
                            retrievalResult = _a.sent();
                            if (retrievalResult.entities.length == 0) {
                                Xrm.Navigation.openErrorDialog({ message: urlSettingName + " global setting is required" });
                            }
                            uri = retrievalResult.entities[0].ipg_value + '&EHRIds=' + selectedItemIds.join(',');
                            return [4 /*yield*/, fetch(uri, {
                                    method: 'POST',
                                    headers: {
                                        'Accept': 'application/json',
                                        'Content-Type': 'application/json; charset=utf-8'
                                    },
                                })
                                    .then(function (response) {
                                    if (response.status == 200) {
                                        primaryControl.refresh();
                                    }
                                })];
                        case 2:
                            _a.sent();
                            return [2 /*return*/];
                    }
                });
            });
        }
    })(EHRStaging = Intake.EHRStaging || (Intake.EHRStaging = {}));
})(Intake || (Intake = {}));
