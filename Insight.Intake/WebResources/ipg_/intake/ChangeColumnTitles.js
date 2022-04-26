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
 * @namespace Intake
 */
var Intake;
(function (Intake) {
    var xmlPath = "../WebResources/ipg_/intake/ViewSettings.xml";
    var ViewSettingsXml = "";
    if (typeof ($) === 'undefined') {
        $ = window.parent.$;
    }
    $.ajax({
        type: "GET",
        url: xmlPath,
        dataType: "xml",
        success: function (xml) {
            ViewSettingsXml = xml;
        }
    });
    function ChangeColumnTitle(rowData, userLCID) {
        var divs = parent.document.querySelectorAll("div.wj-cells");
        divs.forEach(function (div) {
            var viewName = div.getAttribute("aria-label");
            if (viewName) {
                ReadXML(viewName);
            }
        });
    }
    Intake.ChangeColumnTitle = ChangeColumnTitle;
    function parseXML(xml, viewName) {
        $(xml).find("View").each(function () {
            if ($(this).attr("Name") == viewName) {
                var entity = $(this).attr("Entity");
                $(this).find("Field").each(function () {
                    var fieldName = $(this).attr("Name");
                    var title = $(this).attr("Title");
                    var divs = parent.document.querySelectorAll('[id$="' + fieldName + '"]');
                    divs.forEach(function (div) {
                        var data_lp_id = div.getAttribute("data-lp-id");
                        var rightEntity = true;
                        if (data_lp_id) {
                            if (data_lp_id.indexOf(entity) == -1)
                                rightEntity = false;
                        }
                        if (rightEntity) {
                            div.setAttribute("title", title);
                            var divElements = div.getElementsByTagName("*");
                            for (var i = 0; i < divElements.length; i++) {
                                if ((divElements[i].className == "headerCaption") ||
                                    (divElements[i].className == "grid-header-text") ||
                                    divElements[i].className.includes("headerText-")) {
                                    divElements[i].innerHTML = title;
                                    break;
                                }
                            }
                        }
                    });
                });
            }
        });
    }
    function ReadXML(viewName) {
        try {
            console.log("Change columns in" + viewName);
            if (ViewSettingsXml) {
                parseXML(ViewSettingsXml, viewName);
            }
            else {
                $.ajax({
                    type: "GET",
                    url: xmlPath,
                    dataType: "xml",
                    success: function (xml) {
                        ViewSettingsXml = xml;
                        parseXML(ViewSettingsXml, viewName);
                    }
                });
            }
        }
        catch (e) {
            alert("Error while reading XML; Description – " + e.description);
        }
    }
    function GetFirstFieldName(viewName) {
        return __awaiter(this, void 0, void 0, function () {
            var result;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!ViewSettingsXml) return [3 /*break*/, 1];
                        return [2 /*return*/, $($(ViewSettingsXml).find("View[Name='" + viewName + "']").children()[0]).attr("Name")];
                    case 1: return [4 /*yield*/, fetch("../WebResources/ipg_/intake/ViewSettings.xml")];
                    case 2:
                        result = _a.sent();
                        return [4 /*yield*/, result.text()];
                    case 3:
                        ViewSettingsXml = _a.sent();
                        return [2 /*return*/, $($(ViewSettingsXml).find("View[Name='" + viewName + "']").children()[0]).attr("Name")];
                }
            });
        });
    }
    function ChangeColumnTittleOnLoadGrid(executionContext, gridName) {
        return __awaiter(this, void 0, void 0, function () {
            var formContext, gridControl, entityName, controlName, trycount, checkIfGridRendered_1;
            var _this = this;
            return __generator(this, function (_a) {
                formContext = executionContext.getFormContext();
                gridControl = formContext.getControl(gridName);
                entityName = gridControl.getEntityName();
                controlName = gridControl.getName();
                trycount = 0;
                if (gridControl) {
                    checkIfGridRendered_1 = function () { return __awaiter(_this, void 0, void 0, function () {
                        var currentView, firstfieldName;
                        var _a, _b;
                        return __generator(this, function (_c) {
                            switch (_c.label) {
                                case 0:
                                    currentView = gridControl.getViewSelector().getCurrentView();
                                    return [4 /*yield*/, GetFirstFieldName(currentView.name)];
                                case 1:
                                    firstfieldName = _c.sent();
                                    if (((_a = parent.document.querySelectorAll("[id$='" + firstfieldName + "'][data-lp-id*='" + entityName + "']")) === null || _a === void 0 ? void 0 : _a.length) > 0
                                        || ((_b = parent.document.querySelectorAll("[id$='" + firstfieldName + "'][data-lp-id*='" + controlName + "']")) === null || _b === void 0 ? void 0 : _b.length) > 0) {
                                        ReadXML(currentView.name);
                                        trycount = 0;
                                    }
                                    else if (trycount < 10) {
                                        trycount++;
                                        setTimeout(checkIfGridRendered_1, 100);
                                    }
                                    else {
                                        trycount = 0;
                                    }
                                    return [2 /*return*/];
                            }
                        });
                    }); };
                    gridControl.addOnLoad(function () { return setTimeout(checkIfGridRendered_1, 100); });
                }
                return [2 /*return*/];
            });
        });
    }
    Intake.ChangeColumnTittleOnLoadGrid = ChangeColumnTittleOnLoadGrid;
})(Intake || (Intake = {}));
