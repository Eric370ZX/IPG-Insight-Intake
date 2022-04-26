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
    var Authorization;
    (function (Authorization) {
        function OnLoad(executionContext) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext;
                return __generator(this, function (_a) {
                    formContext = executionContext.getFormContext();
                    addCarrierLookupCustomView(formContext);
                    IsAuthRequiredChanged(executionContext);
                    disableAllFields(executionContext);
                    checkIfDefault(formContext);
                    setDefaultValuesByCarrierPlanType(formContext);
                    return [2 /*return*/];
                });
            });
        }
        Authorization.OnLoad = OnLoad;
        function checkIfDefault(form) {
            return __awaiter(this, void 0, void 0, function () {
                var incidentId, existingAuthRecords, systemCreatedRecords;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!form.getAttribute("ipg_incidentid").getValue()) {
                                return [2 /*return*/];
                            }
                            incidentId = form.getAttribute("ipg_incidentid").getValue()[0].id;
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_authorization", "?$select=ipg_facilityauthnumber,_createdby_value&$filter=_ipg_incidentid_value eq " + incidentId)];
                        case 1:
                            existingAuthRecords = _a.sent();
                            if (existingAuthRecords.entities.length > 0) {
                                systemCreatedRecords = existingAuthRecords
                                    .entities
                                    .filter(function (p) { return p["_createdby_value@OData.Community.Display.V1.FormattedValue"].toLowerCase() == "system"; });
                                // if (systemCreatedRecords.length > 0) {
                                // const authNumber = systemCreatedRecords[0]["ipg_facilityauthnumber"];
                                //  form.getAttribute("ipg_facilityauthnumber")?.setValue(authNumber);
                                // }
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        function IsAuthRequiredChanged(executionContext) {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o;
            var formContext = executionContext.getFormContext();
            var isAuthRquired = (_a = formContext.getAttribute("ipg_isauthrequired")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (isAuthRquired) {
                (_b = formContext.getAttribute("ipg_callreference")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("required");
                (_c = formContext.getAttribute("ipg_csrame")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("required");
                (_d = formContext.getAttribute("ipg_csrphone")) === null || _d === void 0 ? void 0 : _d.setRequiredLevel("required");
                (_e = formContext.getAttribute("ipg_ipgauthnumber")) === null || _e === void 0 ? void 0 : _e.setRequiredLevel("required");
                (_f = formContext.getAttribute("ipg_autheffectivedate")) === null || _f === void 0 ? void 0 : _f.setRequiredLevel("required");
                (_g = formContext.getAttribute("ipg_authexpirationdate")) === null || _g === void 0 ? void 0 : _g.setRequiredLevel("required");
            }
            else {
                (_h = formContext.getAttribute("ipg_callreference")) === null || _h === void 0 ? void 0 : _h.setRequiredLevel("none");
                (_j = formContext.getAttribute("ipg_csrame")) === null || _j === void 0 ? void 0 : _j.setRequiredLevel("none");
                (_k = formContext.getAttribute("ipg_csrphone")) === null || _k === void 0 ? void 0 : _k.setRequiredLevel("none");
                (_l = formContext.getAttribute("ipg_ipgauthnumber")) === null || _l === void 0 ? void 0 : _l.setRequiredLevel("none");
                (_m = formContext.getAttribute("ipg_autheffectivedate")) === null || _m === void 0 ? void 0 : _m.setRequiredLevel("none");
                (_o = formContext.getAttribute("ipg_authexpirationdate")) === null || _o === void 0 ? void 0 : _o.setRequiredLevel("none");
            }
        }
        Authorization.IsAuthRequiredChanged = IsAuthRequiredChanged;
        function IsSameAsFacilityChanged(executionContext) {
            var _a, _b, _c, _d, _e;
            var formContext = executionContext.getFormContext();
            var isAuthSameAsFacility = (_a = formContext.getAttribute("ipg_isauthsameasfacility")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (isAuthSameAsFacility) {
                (_b = formContext.getControl("ipg_ipgauthnumber")) === null || _b === void 0 ? void 0 : _b.setDisabled(true);
                (_c = formContext.getAttribute("ipg_ipgauthnumber")) === null || _c === void 0 ? void 0 : _c.setValue((_d = formContext.getAttribute("ipg_facilityauthnumber")) === null || _d === void 0 ? void 0 : _d.getValue());
            }
            else {
                (_e = formContext.getControl("ipg_ipgauthnumber")) === null || _e === void 0 ? void 0 : _e.setDisabled(false);
            }
        }
        Authorization.IsSameAsFacilityChanged = IsSameAsFacilityChanged;
        function FacilityAuthChanged(executionContext) {
            var _a, _b, _c;
            var formContext = executionContext.getFormContext();
            var isAuthSameAsFacility = (_a = formContext.getAttribute("ipg_isauthsameasfacility")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (isAuthSameAsFacility) {
                (_b = formContext.getAttribute("ipg_ipgauthnumber")) === null || _b === void 0 ? void 0 : _b.setValue((_c = formContext.getAttribute("ipg_facilityauthnumber")) === null || _c === void 0 ? void 0 : _c.getValue());
            }
        }
        Authorization.FacilityAuthChanged = FacilityAuthChanged;
        function disableAllFields(executionContext) {
            var formContext = executionContext.getFormContext();
            if (formContext.ui.getFormType() == 2 /* Update */) {
                formContext.ui.controls.forEach(function (control) {
                    if (control && control.getDisabled && !control.getDisabled()) {
                        control.setDisabled(true);
                    }
                });
            }
        }
        Authorization.disableAllFields = disableAllFields;
        function addCarrierLookupCustomView(formContext) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var carrierControl, incidentRef, incident, viewId, fetchXml, viewDisplayName, layoutXml;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            carrierControl = formContext.getControl("ipg_carrierid");
                            if (!carrierControl) return [3 /*break*/, 2];
                            incidentRef = (_a = formContext.getAttribute("ipg_incidentid")) === null || _a === void 0 ? void 0 : _a.getValue();
                            if (!(incidentRef && incidentRef.length > 0)) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", incidentRef[0].id.replace('{', '').replace('}', ''), "?$select=_ipg_carrierid_value,_ipg_secondarycarrierid_value")];
                        case 1:
                            incident = _b.sent();
                            viewId = "00000000-0000-0000-00AA-000010001121";
                            fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">\n  <entity name=\"account\">\n    <attribute name=\"name\" />\n    <attribute name=\"accountid\" />\n    <order attribute=\"name\" descending=\"false\" />\n    <filter type=\"and\">\n      <filter type=\"or\">";
                            if (incident["_ipg_carrierid_value"]) {
                                fetchXml += "\n<condition attribute=\"accountid\" operator=\"eq\" value=\"" + incident["_ipg_carrierid_value"] + "\" />";
                            }
                            if (incident["_ipg_secondarycarrierid_value"]) {
                                fetchXml += "\n<condition attribute=\"accountid\" operator=\"eq\" uiname=\"Test\" uitype=\"account\" value=\"" + incident["_ipg_secondarycarrierid_value"] + "\" />";
                            }
                            fetchXml += "\n</filter>\n      </filter>\n    </entity>\n  </fetch>";
                            viewDisplayName = "Carriers";
                            layoutXml = "<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'>\n      <row name='result' id='accountid'>\n      <cell name='name' width='300' />\n      </row>\n      </grid>";
                            carrierControl.addCustomView(viewId, 'account', viewDisplayName, fetchXml, layoutXml, true);
                            _b.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            });
        }
        function setDefaultValuesByCarrierPlanType(formContext) {
            var _a, _b, _c;
            return __awaiter(this, void 0, void 0, function () {
                var carrierRef, incidentRef, carrier, incident, dosString, dos;
                return __generator(this, function (_d) {
                    switch (_d.label) {
                        case 0:
                            carrierRef = (_a = formContext.getAttribute("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.getValue();
                            incidentRef = (_b = formContext.getAttribute("ipg_incidentid")) === null || _b === void 0 ? void 0 : _b.getValue();
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", carrierRef[0].id, "?$select=ipg_carriertype")];
                        case 1:
                            carrier = _d.sent();
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", incidentRef[0].id, "?$select=ipg_surgerydate,ipg_actualdos")];
                        case 2:
                            incident = _d.sent();
                            dosString = (_c = incident["ipg_actualdos"]) !== null && _c !== void 0 ? _c : incident["ipg_surgerydate"];
                            dos = new Date(dosString);
                            if (carrier.ipg_carriertype == 427880001) {
                                formContext.getAttribute("ipg_autheffectivedate").setValue(new Date("1-1-" + dos.getFullYear()));
                                formContext.getAttribute("ipg_authexpirationdate").setValue(new Date("12-31-9999"));
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
    })(Authorization = Intake.Authorization || (Intake.Authorization = {}));
})(Intake || (Intake = {}));
