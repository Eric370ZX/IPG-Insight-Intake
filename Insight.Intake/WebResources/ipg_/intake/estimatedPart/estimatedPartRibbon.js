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
 * @namespace Intake.EstimaredPart.Ribbon
 */
var Intake;
(function (Intake) {
    var EstimaredPart;
    (function (EstimaredPart) {
        var Ribbon;
        (function (Ribbon) {
            function OnClcikConvertToActualPart(primaryControl, selectedItems) {
                var _a, _b;
                return __awaiter(this, void 0, void 0, function () {
                    var converted, formContext, caseId, arguments, estimatedParts, i, item, entity, actualPart, e_1, customGrid_1, src, aboutBlank;
                    return __generator(this, function (_c) {
                        switch (_c.label) {
                            case 0:
                                converted = 0;
                                Xrm.Utility.showProgressIndicator("");
                                formContext = primaryControl;
                                caseId = formContext.data.entity
                                    .getId()
                                    .replace("{", "")
                                    .replace("}", "");
                                arguments = { IsUserGenerated: false, CarrierNumber: 1 };
                                Intake.Utility.CallActionProcessAsync("incidents", Intake.Utility.removeCurlyBraces(caseId), "ipg_IPGCaseActionsVerifyBenefits", arguments);
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_estimatedcasepartdetail", "?$filter=_ipg_caseid_value eq " + caseId + "\n      and _ipg_actualpartid_value eq null and ipg_ranumber eq null")];
                            case 1:
                                estimatedParts = _c.sent();
                                if (!(estimatedParts &&
                                    estimatedParts.entities &&
                                    estimatedParts.entities.length)) return [3 /*break*/, 9];
                                i = 0;
                                _c.label = 2;
                            case 2:
                                if (!(i < estimatedParts.entities.length)) return [3 /*break*/, 8];
                                item = estimatedParts.entities[i];
                                if (!(selectedItems.indexOf(item.ipg_estimatedcasepartdetailid) != -1)) return [3 /*break*/, 7];
                                _c.label = 3;
                            case 3:
                                _c.trys.push([3, 6, , 7]);
                                entity = {};
                                entity["ipg_potypecode"] = item.ipg_potypecode;
                                entity["ipg_caseid@odata.bind"] = "/incidents(" + item._ipg_caseid_value + ")";
                                entity["ipg_productid@odata.bind"] = "/products(" + item._ipg_productid_value + ")";
                                entity["ipg_quantity"] = item.ipg_quantity;
                                entity["ipg_enteredunitcost"] = item.ipg_unitcost;
                                entity["ipg_enteredshipping"] = item.ipg_unitshipping;
                                entity["ipg_enteredtax"] = item.ipg_unittax;
                                if (item._ipg_uomid_value) {
                                    entity["ipg_uomid@odata.bind"] = "/uoms(" + item._ipg_uomid_value + ")";
                                }
                                return [4 /*yield*/, Xrm.WebApi.createRecord("ipg_casepartdetail", entity)];
                            case 4:
                                actualPart = _c.sent();
                                return [4 /*yield*/, Xrm.WebApi.updateRecord("ipg_estimatedcasepartdetail", item.ipg_estimatedcasepartdetailid, {
                                        "ipg_actualpartid@odata.bind": "/ipg_casepartdetails(" + actualPart.id + ")",
                                    })];
                            case 5:
                                _c.sent();
                                converted++;
                                return [3 /*break*/, 7];
                            case 6:
                                e_1 = _c.sent();
                                return [3 /*break*/, 7];
                            case 7:
                                i++;
                                return [3 /*break*/, 2];
                            case 8:
                                customGrid_1 = formContext.getControl("WebResource_CaseActualPartsGrid");
                                if (customGrid_1) {
                                    src = customGrid_1.getSrc();
                                    aboutBlank = "about:blank";
                                    customGrid_1.setSrc(aboutBlank);
                                    setTimeout(function () {
                                        customGrid_1.setSrc(src);
                                    }, 1000);
                                }
                                (_a = formContext.getControl("ActualParts")) === null || _a === void 0 ? void 0 : _a.refresh();
                                (_b = formContext.getControl("EstimatedParts")) === null || _b === void 0 ? void 0 : _b.refresh();
                                _c.label = 9;
                            case 9:
                                Xrm.Utility.closeProgressIndicator();
                                Xrm.Navigation.openAlertDialog({
                                    text: "Conversion of Estimated Parts to Actual Parts process is complete. " + converted + " of " + selectedItems.length + " successfully converted.",
                                });
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.OnClcikConvertToActualPart = OnClcikConvertToActualPart;
        })(Ribbon = EstimaredPart.Ribbon || (EstimaredPart.Ribbon = {}));
    })(EstimaredPart = Intake.EstimaredPart || (Intake.EstimaredPart = {}));
})(Intake || (Intake = {}));
