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
    var CaseAuthorization;
    (function (CaseAuthorization) {
        var _a;
        var CarrierType;
        (function (CarrierType) {
            CarrierType[CarrierType["Carrier1"] = 1] = "Carrier1";
            CarrierType[CarrierType["Carrier2"] = 2] = "Carrier2";
            //Tertiary = 2 not needed anymore
        })(CarrierType || (CarrierType = {}));
        var IncidentCarrierTypeMapping = (_a = {},
            _a[CarrierType.Carrier1] = "ipg_carrierid",
            _a[CarrierType.Carrier2] = "ipg_secondarycarrierid",
            _a);
        var _incidentAuthFields = [
            "title", "_ipg_carrierid_value", "_ipg_procedureid_value",
            "_ipg_homeplancarrierid_value", "_ipg_secondarycarrierid_value",
            "_ipg_autocarrierid_value", "ipg_is_authorization_required"
        ];
        var _authorizationRecentFields = [
            "ipg_facilityauthnumber", "_createdby_value",
            "ipg_ipgauthnumber"
        ];
        var Xrm = Xrm || parent.Xrm;
        function Init() {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var isLocked;
                return __generator(this, function (_b) {
                    isLocked = (_a = Xrm.Page.getAttribute("ipg_islocked")) === null || _a === void 0 ? void 0 : _a.getValue();
                    //@ts-ignore
                    document.getElementById('obtainAuthorization').disabled = isLocked;
                    return [2 /*return*/];
                });
            });
        }
        CaseAuthorization.Init = Init;
        function ObtainCarrierAuthDefault() {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var params;
                return __generator(this, function (_b) {
                    params = Intake.Utility.GetWindowJsonParameters();
                    ObtainCarrierAuth(params.recordId, (_a = params.carrierType) !== null && _a !== void 0 ? _a : 1);
                    return [2 /*return*/];
                });
            });
        }
        CaseAuthorization.ObtainCarrierAuthDefault = ObtainCarrierAuthDefault;
        function ObtainCarrierAuth(incidentId, carrierType) {
            return __awaiter(this, void 0, void 0, function () {
                var Xrm, incident, incidentRef, carrierId, carrierName, carrierRef, procedureNameRef, formParameters, recentAuth;
                var _a;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            Xrm = Xrm || parent.Xrm;
                            Xrm.Utility.showProgressIndicator("Preparing the form");
                            return [4 /*yield*/, GetCaseDataForAuth(incidentId)];
                        case 1:
                            incident = _b.sent();
                            incidentRef = {
                                entityType: "incident",
                                id: incidentId,
                                name: incident.title
                            };
                            carrierId = incident["_" + IncidentCarrierTypeMapping[carrierType] + "_value"];
                            carrierName = incident["_" + IncidentCarrierTypeMapping[carrierType] + "_value@OData.Community.Display.V1.FormattedValue"];
                            carrierRef = {
                                entityType: "account",
                                id: carrierId,
                                name: carrierName
                            };
                            procedureNameRef = {
                                entityType: "ipg_procedurename",
                                id: incident["_ipg_procedureid_value"],
                                name: incident["_ipg_procedureid_value@OData.Community.Display.V1.FormattedValue"]
                            };
                            formParameters = (_a = {},
                                _a["ipg_carrierid"] = carrierRef,
                                _a["ipg_procedurenameid"] = procedureNameRef,
                                _a["ipg_isauthrequired"] = incident["ipg_is_authorization_required"],
                                _a);
                            return [4 /*yield*/, GetRecentAuthRecord(incidentId, carrierId)];
                        case 2:
                            recentAuth = _b.sent();
                            if (recentAuth) {
                                formParameters["ipg_facilityauthnumber"] = recentAuth.ipg_facilityauthnumber;
                                if (recentAuth["_createdby_value@OData.Community.Display.V1.FormattedValue"].toLowerCase() != 'system') {
                                    formParameters["ipg_facilityauthnumber"] = recentAuth.ipg_facilityauthnumber;
                                }
                            }
                            else {
                                if (incident.ipg_incident_ipg_referral_AssociatedCaseId &&
                                    incident.ipg_incident_ipg_referral_AssociatedCaseId.length > 0) {
                                    formParameters["ipg_facilityauthnumber"] = incident.ipg_incident_ipg_referral_AssociatedCaseId[0].ipg_facility_auth;
                                }
                            }
                            Xrm.Navigation.navigateTo({
                                pageType: "entityrecord",
                                entityName: "ipg_authorization",
                                formId: "42b37e40-4ed6-402c-a87f-737cd05825ba",
                                createFromEntity: incidentRef,
                                data: formParameters
                            }, {
                                width: window.screen.width * 0.75,
                                height: window.screen.height * 0.75,
                                position: 1,
                                target: 2 //dialog
                            }).then(function (res) {
                                setTimeout(function (_) {
                                    //debugger;
                                    if (res.savedEntityReference && res.savedEntityReference.length) {
                                        //new authorization has have been associated in CaseRecentAuthorization plugin and will be displayed after saving the case
                                        Xrm.Page.data.refresh(true);
                                    }
                                }, 250);
                            }, function (error) {
                                console.log(error);
                            });
                            Xrm.Utility.closeProgressIndicator();
                            return [2 /*return*/];
                    }
                });
            });
        }
        CaseAuthorization.ObtainCarrierAuth = ObtainCarrierAuth;
        function GetCaseDataForAuth(incidentId) {
            return __awaiter(this, void 0, void 0, function () {
                var select, expand, url, response;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            incidentId = incidentId.replace("{", "").replace("}", "");
                            select = "$select=" + _incidentAuthFields.join(",");
                            expand = "$expand=ipg_incident_ipg_referral_AssociatedCaseId($select=ipg_facility_auth)";
                            url = "/api/data/v9.1/incidents(" + incidentId + ")?" + select + "&" + expand;
                            return [4 /*yield*/, fetch(url, {
                                    method: "GET",
                                    headers: {
                                        "Prefer": 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"'
                                    }
                                })];
                        case 1:
                            response = _a.sent();
                            return [4 /*yield*/, response.json()];
                        case 2: return [2 /*return*/, _a.sent()];
                    }
                });
            });
        }
        function GetRecentAuthRecord(incidentId, carrierId) {
            return __awaiter(this, void 0, void 0, function () {
                var filter, orderby, top, select, url, response, records;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            incidentId = incidentId.replace("{", "").replace("}", "");
                            carrierId = carrierId.replace("{", "").replace("}", "");
                            filter = "$filter=_ipg_incidentid_value eq '" + incidentId + "' and _ipg_carrierid_value eq '" + carrierId + "' and statecode eq 0";
                            orderby = "$orderby=createdon desc";
                            top = "$top=1";
                            select = "$select=" + _authorizationRecentFields.join(",");
                            url = "/api/data/v9.1/ipg_authorizations?" + filter + "&" + orderby + "&" + top + "&" + select;
                            return [4 /*yield*/, fetch(url, {
                                    method: "GET",
                                    headers: {
                                        "Prefer": 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"'
                                    }
                                })];
                        case 1:
                            response = _a.sent();
                            return [4 /*yield*/, response.json()];
                        case 2:
                            records = _a.sent();
                            if (records && records.value && records.value.length > 0) {
                                return [2 /*return*/, records.value[0]];
                            }
                            return [2 /*return*/, null];
                    }
                });
            });
        }
    })(CaseAuthorization = Intake.CaseAuthorization || (Intake.CaseAuthorization = {}));
})(Intake || (Intake = {}));
