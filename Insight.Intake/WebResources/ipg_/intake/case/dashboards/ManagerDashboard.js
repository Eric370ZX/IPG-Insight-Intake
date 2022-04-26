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
var Insight;
(function (Insight) {
    var Intake;
    (function (Intake) {
        var redCircle = "ipg_/intake/img/red-circle-32px.png";
        var yellowCircle = "ipg_/intake/img/yellow-circle-32px.png";
        var greenCircle = "ipg_/intake/img/green-circle-32px.png";
        var greenCheckMark = "ipg_/intake/img/check-mark-green-32px.png";
        var redCancel = "ipg_/intake/img/red-cancel-32px.png";
        function ShowSlaThresholdIcon(rowData, userLcid) {
            return __awaiter(this, void 0, void 0, function () {
                var rowObject, incidentId, imgLink, filter, select, orderBy, top_1, url, response, records;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            rowObject = JSON.parse(rowData);
                            incidentId = rowObject.RowId.replace("{", "").replace("}", "");
                            imgLink = "";
                            if (!(rowObject.ipg_slathresholddayscalculated_Value != undefined &&
                                rowObject.ipg_slathresholddayscalculated_Value != null)) return [3 /*break*/, 1];
                            if (rowObject.ipg_slathresholddayscalculated_Value <= 0) {
                                imgLink = redCircle;
                            }
                            else if (rowObject.ipg_slathresholddayscalculated_Value == 1) {
                                imgLink = yellowCircle;
                            }
                            else if (rowObject.ipg_slathresholddayscalculated_Value > 1) {
                                imgLink = greenCircle;
                            }
                            return [3 /*break*/, 4];
                        case 1:
                            filter = "$filter=_ipg_caseid_value eq '" + incidentId + "' and ipg_slatypecode ne null and statecode eq 1";
                            select = "$select=actualend,scheduledend";
                            orderBy = "$orderby=scheduledend desc";
                            top_1 = "$top=1";
                            url = "/api/data/v9.1/tasks?" + filter + "&" + select + "&" + orderBy + "&" + top_1;
                            return [4 /*yield*/, fetch(url)];
                        case 2:
                            response = _a.sent();
                            return [4 /*yield*/, response.json()];
                        case 3:
                            records = _a.sent();
                            if (records.value && records.value.length > 0) {
                                if (records.value[0].actualend > records.value[0].scheduledend) {
                                    imgLink = redCancel;
                                }
                                else {
                                    imgLink = greenCheckMark;
                                }
                            }
                            _a.label = 4;
                        case 4: return [2 /*return*/, [imgLink]];
                    }
                });
            });
        }
        Intake.ShowSlaThresholdIcon = ShowSlaThresholdIcon;
    })(Intake = Insight.Intake || (Insight.Intake = {}));
})(Insight || (Insight = {}));
