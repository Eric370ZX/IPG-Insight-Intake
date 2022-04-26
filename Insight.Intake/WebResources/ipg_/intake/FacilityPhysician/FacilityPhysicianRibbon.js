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
 * @namespace Intake.FacilityPhysician.Ribbon
 */
var Intake;
(function (Intake) {
    var FacilityPhysician;
    (function (FacilityPhysician) {
        var Ribbon;
        (function (Ribbon) {
            // just fast format for ids to not use Utility.js in ribbon file
            var format = function (id) {
                return id.replace("{", "").replace("}", "").toLowerCase();
            };
            /**
            * Show buttons only on related Tab
            * @function Intake.FacilityPhysician.Ribbon.ShowOnlyOnRelatedTab
            * @returns {bool}
            */
            function ShowOnlyOnRelatedTab(selectedControl, tabName) {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        return [2 /*return*/, selectedControl["_controlName"] === tabName];
                    });
                });
            }
            Ribbon.ShowOnlyOnRelatedTab = ShowOnlyOnRelatedTab;
            /**
            * Hide buttons only on related Tab
            * @function Intake.FacilityPhysician.Ribbon.HideOnlyOnRelatedTab
            * @returns {bool}
            */
            function HideOnlyOnRelatedTab(selectedControl, tabName) {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        return [2 /*return*/, selectedControl["_controlName"] != tabName];
                    });
                });
            }
            Ribbon.HideOnlyOnRelatedTab = HideOnlyOnRelatedTab;
            /**
             * Is All Selected Records Are Active
             * @function Intake.FacilityPhysician.Ribbon.IsAllSelectedRecordsAreActive
             * @returns {bool}
             */
            function IsAllSelectedRecordsAreActive(selectedIds) {
                return __awaiter(this, void 0, void 0, function () {
                    var fetchXml, records;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                if (!(selectedIds && selectedIds.length > 0)) return [3 /*break*/, 2];
                                fetchXml = generateFetchXmlToRetrieveFacilityPhysiciansByIds(selectedIds);
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_facilityphysician", fetchXml)];
                            case 1:
                                records = _a.sent();
                                if (records && records.entities.length > 0) {
                                    return [2 /*return*/, records.entities.every(function (r) {
                                            return r.ipg_status;
                                        })];
                                }
                                _a.label = 2;
                            case 2: return [2 /*return*/, false];
                        }
                    });
                });
            }
            Ribbon.IsAllSelectedRecordsAreActive = IsAllSelectedRecordsAreActive;
            /**
             * Is All Selected Records Are Inactive
             * @function Intake.FacilityPhysician.Ribbon.IsAllSelectedRecordsAreInactive
             * @returns {bool}
             */
            function IsAllSelectedRecordsAreInactive(selectedIds) {
                return __awaiter(this, void 0, void 0, function () {
                    var fetchXml, records;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                if (!(selectedIds && selectedIds.length > 0)) return [3 /*break*/, 2];
                                fetchXml = generateFetchXmlToRetrieveFacilityPhysiciansByIds(selectedIds);
                                return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_facilityphysician", fetchXml)];
                            case 1:
                                records = _a.sent();
                                if (records && records.entities.length > 0) {
                                    return [2 /*return*/, records.entities.every(function (r) {
                                            return !r.ipg_status;
                                        })];
                                }
                                _a.label = 2;
                            case 2: return [2 /*return*/, false];
                        }
                    });
                });
            }
            Ribbon.IsAllSelectedRecordsAreInactive = IsAllSelectedRecordsAreInactive;
            function generateFetchXmlToRetrieveFacilityPhysiciansByIds(recordIds) {
                var filterValues = "";
                recordIds.forEach(function (id) {
                    filterValues += "\n<value>" + id + "</value>";
                });
                var fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">\n                        <entity name=\"ipg_facilityphysician\">\n                        <attribute name=\"ipg_facilityphysicianid\" />\n                        <attribute name=\"ipg_status\" />\n                        <filter type=\"and\">\n                            <condition attribute=\"ipg_facilityphysicianid\" operator=\"in\">" +
                    filterValues +
                    "\n                            </condition>\n                        </filter>\n                        </entity>\n                    </fetch>";
                fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
                return fetchXml;
            }
            /**
             * Set Active Flag
             * @function Intake.FacilityPhysician.Ribbon.setActiveFlag
             * @returns {void}
             */
            function setActiveFlag(primaryControl, selectedIds, isActive) {
                var Sdk = {
                    UpdateRequest: function (entityTypeName, id, payload) {
                        this.etn = entityTypeName;
                        this.id = id;
                        this.payload = payload;
                        this.getMetadata = function () {
                            return {
                                boundParameter: null,
                                parameterTypes: {},
                                operationType: 2,
                                operationName: "Update",
                            };
                        };
                    },
                };
                var requests = [];
                var newActiveFlag = {
                    "ipg_status": isActive
                };
                selectedIds.forEach(function (recordId) {
                    requests.push(new Sdk.UpdateRequest("ipg_facilityphysician", recordId, newActiveFlag));
                });
                Xrm.WebApi.online.executeMultiple(requests).then(function (success) {
                    var alertStrings = {
                        confirmButtonLabel: "Ok",
                        text: "The records are updated successfully",
                        title: "Success",
                    };
                    var alertOptions = { height: 120, width: 260 };
                    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                        primaryControl.refresh(true);
                    });
                }, function (error) {
                    console.log(error.message);
                });
            }
            Ribbon.setActiveFlag = setActiveFlag;
        })(Ribbon = FacilityPhysician.Ribbon || (FacilityPhysician.Ribbon = {}));
    })(FacilityPhysician = Intake.FacilityPhysician || (Intake.FacilityPhysician = {}));
})(Intake || (Intake = {}));
