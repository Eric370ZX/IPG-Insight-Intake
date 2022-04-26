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
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        /**
         * Called from the form of Case
         * @function Intake.Case.CallGeneratePurchaseOrdersForm
         * @returns {void}
         */
        function CallGeneratePurchaseOrdersForm() {
            var _a, _b;
            return __awaiter(this, void 0, void 0, function () {
                var entityId, casePartDetails, _i, _c, POtypeValue, POtypeName, CPACasePartDetails, CasePartDetailsDistinctByPreviousPO, _d, CasePartDetailsDistinctByPreviousPO_1, distinctCasePartDetail, POtypeNameCasePartDetails, CasePartDetailsDistinctByManufacturer, _e, CasePartDetailsDistinctByManufacturer_1, distinctCasePartDetail;
                return __generator(this, function (_f) {
                    switch (_f.label) {
                        case 0:
                            entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
                            casePartDetails = GetCasePartDetailsAndManufacturerbyCase(entityId);
                            if (!(casePartDetails.length > 0)) return [3 /*break*/, 12];
                            _i = 0, _c = Object.keys(POtypeEnum);
                            _f.label = 1;
                        case 1:
                            if (!(_i < _c.length)) return [3 /*break*/, 11];
                            POtypeValue = _c[_i];
                            console.log(POtypeEnum[POtypeValue]);
                            POtypeName = POtypeEnum[POtypeValue];
                            if (!(typeof (POtypeName) === "string")) return [3 /*break*/, 10];
                            if (!(POtypeName === "CPA")) return [3 /*break*/, 6];
                            CPACasePartDetails = casePartDetails.filter(function (x) { return x.ipg_potypecode.Value === POtypeEnum.CPA; });
                            CasePartDetailsDistinctByPreviousPO = CPACasePartDetails.filter(function (thing, i, arr) { return arr.findIndex(function (t) { var _a, _b; return ((_a = t.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId) === null || _a === void 0 ? void 0 : _a.SalesOrderId) === ((_b = thing.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId) === null || _b === void 0 ? void 0 : _b.SalesOrderId); }) === i; });
                            _d = 0, CasePartDetailsDistinctByPreviousPO_1 = CasePartDetailsDistinctByPreviousPO;
                            _f.label = 2;
                        case 2:
                            if (!(_d < CasePartDetailsDistinctByPreviousPO_1.length)) return [3 /*break*/, 5];
                            distinctCasePartDetail = CasePartDetailsDistinctByPreviousPO_1[_d];
                            return [4 /*yield*/, CallGeneratePurchaseOrdersAction(entityId, "", (_a = distinctCasePartDetail.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId) === null || _a === void 0 ? void 0 : _a.SalesOrderId, POtypeName, false)];
                        case 3:
                            _f.sent();
                            _f.label = 4;
                        case 4:
                            _d++;
                            return [3 /*break*/, 2];
                        case 5: return [3 /*break*/, 10];
                        case 6:
                            POtypeNameCasePartDetails = casePartDetails.filter(function (x) { return x.ipg_potypecode.Value === POtypeEnum[POtypeName]; });
                            CasePartDetailsDistinctByManufacturer = POtypeNameCasePartDetails.filter(function (thing, i, arr) { return arr.findIndex(function (t) { var _a, _b; return t.ipg_product_ipg_casepartdetail_productid.ipg_manufacturerid.Id === thing.ipg_product_ipg_casepartdetail_productid.ipg_manufacturerid.Id && ((_a = t.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId) === null || _a === void 0 ? void 0 : _a.SalesOrderId) === ((_b = thing.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId) === null || _b === void 0 ? void 0 : _b.SalesOrderId); }) === i; });
                            _e = 0, CasePartDetailsDistinctByManufacturer_1 = CasePartDetailsDistinctByManufacturer;
                            _f.label = 7;
                        case 7:
                            if (!(_e < CasePartDetailsDistinctByManufacturer_1.length)) return [3 /*break*/, 10];
                            distinctCasePartDetail = CasePartDetailsDistinctByManufacturer_1[_e];
                            return [4 /*yield*/, CallGeneratePurchaseOrdersAction(entityId, distinctCasePartDetail.ipg_product_ipg_casepartdetail_productid.ipg_manufacturerid.Id, (_b = distinctCasePartDetail.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId) === null || _b === void 0 ? void 0 : _b.SalesOrderId, POtypeName, false)];
                        case 8:
                            _f.sent();
                            _f.label = 9;
                        case 9:
                            _e++;
                            return [3 /*break*/, 7];
                        case 10:
                            _i++;
                            return [3 /*break*/, 1];
                        case 11: return [3 /*break*/, 13];
                        case 12:
                            alert("No CasePartDetails found for the case");
                            _f.label = 13;
                        case 13: return [2 /*return*/];
                    }
                });
            });
        }
        Case.CallGeneratePurchaseOrdersForm = CallGeneratePurchaseOrdersForm;
        function CallGeneratePurchaseOrdersFormTPO() {
            return __awaiter(this, void 0, void 0, function () {
                var entityId, estimatedCasePartDetails, _i, _a, POtypeValue, POtypeName, POtypeNameCasePartDetails, CasePartDetailsDistinctByManufacturer, _b, CasePartDetailsDistinctByManufacturer_2, distinctCasePartDetail;
                return __generator(this, function (_c) {
                    switch (_c.label) {
                        case 0:
                            entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
                            estimatedCasePartDetails = GetEstimatedCasePartDetailsAndManufacturerbyCase(entityId);
                            return [4 /*yield*/, ConvertNotTPOTOTPO(estimatedCasePartDetails)];
                        case 1:
                            _c.sent();
                            if (!(estimatedCasePartDetails.length > 0)) return [3 /*break*/, 8];
                            _i = 0, _a = Object.keys(POtypeEnum);
                            _c.label = 2;
                        case 2:
                            if (!(_i < _a.length)) return [3 /*break*/, 7];
                            POtypeValue = _a[_i];
                            console.log(POtypeEnum[POtypeValue]);
                            POtypeName = POtypeEnum[POtypeValue];
                            if (!(typeof (POtypeName) === "string")) return [3 /*break*/, 6];
                            if (!(POtypeName === "TPO")) return [3 /*break*/, 6];
                            POtypeNameCasePartDetails = estimatedCasePartDetails.filter(function (x) { return x.ipg_potypecode.Value === POtypeEnum[POtypeName]; });
                            CasePartDetailsDistinctByManufacturer = POtypeNameCasePartDetails.filter(function (thing, i, arr) { return arr.findIndex(function (t) { return t.ipg_product_ipg_estimatedcasepartdetail_productid.ipg_manufacturerid.Id === thing.ipg_product_ipg_estimatedcasepartdetail_productid.ipg_manufacturerid.Id; }) === i; });
                            _b = 0, CasePartDetailsDistinctByManufacturer_2 = CasePartDetailsDistinctByManufacturer;
                            _c.label = 3;
                        case 3:
                            if (!(_b < CasePartDetailsDistinctByManufacturer_2.length)) return [3 /*break*/, 6];
                            distinctCasePartDetail = CasePartDetailsDistinctByManufacturer_2[_b];
                            return [4 /*yield*/, CallGeneratePurchaseOrdersAction(entityId, distinctCasePartDetail.ipg_product_ipg_estimatedcasepartdetail_productid.ipg_manufacturerid.Id, "", POtypeName, true)];
                        case 4:
                            _c.sent();
                            _c.label = 5;
                        case 5:
                            _b++;
                            return [3 /*break*/, 3];
                        case 6:
                            _i++;
                            return [3 /*break*/, 2];
                        case 7: return [3 /*break*/, 9];
                        case 8:
                            alert("No Estimated Case Part Details found for the case");
                            _c.label = 9;
                        case 9: return [2 /*return*/];
                    }
                });
            });
        }
        Case.CallGeneratePurchaseOrdersFormTPO = CallGeneratePurchaseOrdersFormTPO;
        /**
         * Called from the form of Case
         * @function Intake.Case.CallGeneratePurchaseOrdersFormChooseClick
         * @returns {void}
         */
        function CallGeneratePurchaseOrdersFormChooseClick(commandProperties, POType) {
            var entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
            var manufacturerId = (POType == "CPA" ? "" : commandProperties.SourceControlId.split(".")[3]);
            CallGeneratePurchaseOrdersAction(entityId, manufacturerId, "", POType, false);
        }
        Case.CallGeneratePurchaseOrdersFormChooseClick = CallGeneratePurchaseOrdersFormChooseClick;
        function CallGeneratePurchaseOrdersFormPOClick(commandProperties) {
            var arr = commandProperties.SourceControlId.split(".");
            var menuXml = '<Menu Id="ipg.incident.Button.' + arr[arr.length - 1] + '.Menu">';
            menuXml += '<MenuSection Id="ipg.incident.Section2.Section" Sequence="5" DisplayMode="Menu16">';
            menuXml += '<Controls Id="ipg.incident.Section2.Section.Controls">';
            var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
            var arr2 = commandProperties.SourceControlId.split("_");
            var POtypeValue = arr2.length == 2 ? POtypeEnum[arr2[1]] : "";
            var casepartdetails = [];
            var products = [];
            var manufacturers = [];
            var entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
            var url = baseUrl
                + "ipg_casepartdetailSet?"
                + "$select=ipg_productid"
                + "&$filter=ipg_caseid/Id eq (guid'" + entityId + "')"
                + "and ipg_productid/Id ne null "
                + " and ipg_IsChanged eq true";
            if (POtypeValue)
                url += " and ipg_potypecode/Value eq " + POtypeValue;
            GetRecords(url, casepartdetails);
            for (var i = 0; i < casepartdetails.length; i++) {
                products.push(casepartdetails[i].ipg_productid.Id);
            }
            if (products.length) {
                url = baseUrl
                    + "ProductSet?"
                    + "$select=ipg_manufacturerid"
                    + "&$filter=";
                for (var i = 0; i < products.length; i++) {
                    if (i < products.length - 1)
                        url += "ProductId eq (guid'" + products[i] + "') or ";
                    else
                        url += "ProductId eq (guid'" + products[i] + "')";
                }
                GetRecords(url, manufacturers);
                manufacturers = manufacturers.filter(function (value, index, self) { return self.map(function (m) { return m.Id; }).indexOf(value.Id) === index; });
                for (var i = 0; i < manufacturers.length; i++) {
                    menuXml += '<Button Alt="' + manufacturers[i].ipg_manufacturerid.Name + '" Command="incident|NoRelationship|Form|ipg.incident.Commands.' + arr[arr.length - 1] + '" CommandValueId="" Id="ipg.incident.Buttons.' + manufacturers[i].ipg_manufacturerid.Id + '" LabelText="' + manufacturers[i].ipg_manufacturerid.Name + '" Sequence="10" ToolTipTitle="' + manufacturers[i].ipg_manufacturerid.Name + '" />';
                }
            }
            menuXml += "</Controls>";
            menuXml += "</MenuSection>";
            menuXml += "</Menu>";
            commandProperties["PopulationXML"] = menuXml;
        }
        Case.CallGeneratePurchaseOrdersFormPOClick = CallGeneratePurchaseOrdersFormPOClick;
        function IsPOButtonEnabled(POType) {
            if (POType == "TPO") {
                var entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
                var tpo = GetPObyCase(POType, entityId);
                if (tpo != null) {
                    return false;
                }
            }
            return true;
        }
        Case.IsPOButtonEnabled = IsPOButtonEnabled;
        var POtypeEnum;
        (function (POtypeEnum) {
            POtypeEnum[POtypeEnum["TPO"] = 923720000] = "TPO";
            POtypeEnum[POtypeEnum["ZPO"] = 923720001] = "ZPO";
            POtypeEnum[POtypeEnum["CPA"] = 923720002] = "CPA";
            POtypeEnum[POtypeEnum["MPO"] = 923720004] = "MPO";
        })(POtypeEnum || (POtypeEnum = {}));
        function GetCasePartDetailsAndManufacturerbyCase(caseId) {
            var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
            var url = baseUrl + ("ipg_casepartdetailSet?$select=ipg_potypecode,ipg_product_ipg_casepartdetail_productid/ipg_manufacturerid,ipg_salesorder_ipg_casepartdetail_PurchaseOrderId/SalesOrderId&$expand=ipg_product_ipg_casepartdetail_productid,ipg_salesorder_ipg_casepartdetail_PurchaseOrderId&$filter=ipg_caseid/Id eq (guid'" + caseId + "') and ipg_IsChanged eq true");
            var casePartDetails = [];
            GetRecords(url, casePartDetails);
            return casePartDetails;
        }
        function GetEstimatedCasePartDetailsAndManufacturerbyCase(caseId) {
            var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
            var url = baseUrl + ("ipg_estimatedcasepartdetailSet?$select=ipg_estimatedcasepartdetailId,ipg_potypecode,ipg_product_ipg_estimatedcasepartdetail_productid/ipg_manufacturerid&$expand=ipg_product_ipg_estimatedcasepartdetail_productid&$filter=ipg_caseid/Id eq (guid'" + caseId + "')");
            var casePartDetails = [];
            GetRecords(url, casePartDetails);
            return casePartDetails;
        }
        function GetPObyCase(poType, caseId) {
            var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
            var url = baseUrl + ("SalesOrderSet?$top=1&$select=SalesOrderId&$filter=ipg_potypecode/Value eq " + POtypeEnum[poType] + " and ipg_CaseId/Id eq (guid'" + caseId + "')");
            var salesOrders = [];
            GetRecords(url, salesOrders);
            if (salesOrders.length > 0) {
                return salesOrders[0];
            }
            return null;
        }
        function IsAddCasePartDetailButtonEnabled() {
            var entityName = Xrm.Page.data.entity.getEntityName();
            var caseId = Xrm.Page.data.entity.getId();
            if (entityName === "incident") {
                var tpo = GetPObyCase("TPO", caseId);
                if (tpo != null && !IsCaseOn6Gate(caseId)) {
                    return false;
                }
            }
            return true;
        }
        Case.IsAddCasePartDetailButtonEnabled = IsAddCasePartDetailButtonEnabled;
        function IsCaseOn6Gate(caseId) {
            var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
            var url = baseUrl + ("IncidentSet?$top=1&$select=IncidentId,ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid/ipg_gateconfigurationid&$expand=ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid&$filter=IncidentId eq (guid'" + caseId + "')");
            var cases = [];
            GetRecords(url, cases);
            if (cases.length > 0) {
                return cases[0].ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid
                    && cases[0].ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid.ipg_gateconfigurationid
                    && cases[0].ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid.ipg_gateconfigurationid.Name.toLowerCase() === "gate 6";
            }
            return false;
        }
        /**
        * Gets records using oData
        * @function Intake.Case.GetRecords
        * @returns {array}
        */
        function GetRecords(url, entities) {
            var xhr = new XMLHttpRequest();
            xhr.open("GET", encodeURI(url), false);
            xhr.setRequestHeader("OData-MaxVersion", "4.0");
            xhr.setRequestHeader("OData-Version", "4.0");
            xhr.setRequestHeader("Accept", "application/json");
            xhr.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            xhr.onreadystatechange = function () {
                if (this.readyState === 4) {
                    xhr.onreadystatechange = null;
                    switch (this.status) {
                        case 200: // Success with content returned in response body.
                        case 204: // Success with no content returned in response body.
                        case 304: // Success with Not Modified
                            var data = JSON.parse(this.responseText || "{}");
                            if (data && data.d && data.d.results) {
                                AddRecordsToArray(data.d.results, entities);
                                FetchRecordsCallBack(data.d, entities);
                            }
                            break;
                        default: // All other statuses are error cases.
                            var error = void 0;
                            try {
                                error = JSON.parse(xhr.response).error;
                            }
                            catch (e) {
                                error = new Error("Unexpected Error");
                            }
                            alert("Error :  has occurred during retrieval of the records ");
                            console.error(error);
                            break;
                    }
                }
            };
            xhr.send();
        }
        function AddRecordsToArray(records, entities) {
            for (var i = 0; i < records.length; i++) {
                entities.push(records[i]);
            }
        }
        function FetchRecordsCallBack(records, entities) {
            if (records.__next != null) {
                var url = records.__next;
                GetRecords(url, entities);
            }
        }
        function CallGeneratePurchaseOrdersAction(entityId, manufacturerId, previousOrderId, POType, estimated) {
            return __awaiter(this, void 0, void 0, function () {
                var _a, entity, parameters, data, myUrlWithParams;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            _a = entityId
                                && IsPOValidForGeneration(entityId, manufacturerId, POType, estimated);
                            if (!_a) return [3 /*break*/, 2];
                            return [4 /*yield*/, IsManufacturerValid(entityId, manufacturerId, POType)];
                        case 1:
                            _a = (_b.sent());
                            _b.label = 2;
                        case 2:
                            if (_a) {
                                entity = {
                                    id: entityId,
                                    entityType: "incident"
                                };
                                parameters = {
                                    entity: entity
                                };
                                data = {
                                    caseId: entityId,
                                    accountId: manufacturerId,
                                    salesOrderId: previousOrderId,
                                    POType: POType,
                                    estimated: estimated
                                };
                                myUrlWithParams = new URL(Xrm.Utility.getGlobalContext().getCurrentAppUrl());
                                myUrlWithParams.searchParams.append("pagetype", "webresource");
                                myUrlWithParams.searchParams.append("webresourceName", "ipg_/intake/incident/POPreview.html");
                                myUrlWithParams.searchParams.append("cmdbar", "true");
                                myUrlWithParams.searchParams.append("navbar", "off");
                                myUrlWithParams.searchParams.append("data", JSON.stringify(data));
                                myUrlWithParams.searchParams.append("newWindow", "true");
                                window.open(myUrlWithParams.toString(), "_blank");
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        function IsPOValidForGeneration(entityId, manufacturerId, POType, estimated) {
            var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
            var POs = [];
            var potypeCode = POtypeEnum[POType];
            if (potypeCode == POtypeEnum.TPO) {
                return true;
            }
            var casepartdetails = [];
            var url = baseUrl
                + "ipg_casepartdetailSet?"
                + "$select=ipg_productid,ipg_product_ipg_casepartdetail_productid/StateCode,ipg_product_ipg_casepartdetail_productid/ipg_status&$expand=ipg_product_ipg_casepartdetail_productid"
                + "&$filter=ipg_caseid/Id eq (guid'" + entityId + "')"
                + " and ipg_potypecode/Value eq " + potypeCode
                + " and ipg_productid/Id ne null"
                + " and ipg_IsChanged eq true"
                + " and statecode/Value eq 0";
            GetRecords(url, casepartdetails);
            if (!casepartdetails.some(function (ap) { return ap.ipg_product_ipg_casepartdetail_productid.ipg_status
                //check that part approved and active
                && ap.ipg_product_ipg_casepartdetail_productid.ipg_status.Value === 923720000
                && ap.ipg_product_ipg_casepartdetail_productid.StateCode.Value === 0; })) {
                //prevent the PO generation instead of -> alert(`${POType} doesn't have approved Parts!`);???
                return false;
            }
            return true;
        }
        function IsManufacturerValid(entityId, manufacturerId, POType) {
            return __awaiter(this, void 0, void 0, function () {
                var entity, manufacturerRef, parameters, parameterTypes_1, request, result, e_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!(POType != POtypeEnum[POtypeEnum.CPA] && manufacturerId && entityId)) return [3 /*break*/, 5];
                            entity = {
                                id: entityId,
                                entityType: "incident"
                            };
                            manufacturerRef = {
                                id: manufacturerId,
                                entityType: "account"
                            };
                            parameters = {
                                entity: entity,
                                ManufacturerId: manufacturerRef,
                                Validation: "manufacturer"
                            };
                            parameterTypes_1 = {
                                "entity": {
                                    "typeName": "mscrm.incident",
                                    "structuralProperty": 5
                                },
                                "ManufacturerId": {
                                    "typeName": "mscrm.account",
                                    "structuralProperty": 5
                                },
                                "Validation": {
                                    "typeName": "Edm.String",
                                    "structuralProperty": 1
                                }
                            };
                            request = {
                                entity: parameters.entity,
                                ManufacturerId: parameters.ManufacturerId,
                                Validation: parameters.Validation,
                                getMetadata: function () {
                                    return {
                                        boundParameter: "entity",
                                        parameterTypes: parameterTypes_1,
                                        operationType: 0,
                                        operationName: "ipg_IPGCaseActionsPreValidatePO"
                                    };
                                }
                            };
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 3, , 4]);
                            return [4 /*yield*/, Xrm.WebApi.online.execute(request)];
                        case 2:
                            result = _a.sent();
                            return [3 /*break*/, 4];
                        case 3:
                            e_1 = _a.sent();
                            console.log(e_1);
                            return [3 /*break*/, 4];
                        case 4: return [2 /*return*/, true];
                        case 5: return [2 /*return*/, true];
                    }
                });
            });
        }
        //enable rules
        function IsLifeCycleStep(primaryControl, lifecycleStepNames) {
            var formContext = primaryControl;
            var lifecycleStep = formContext.getAttribute("ipg_lifecyclestepid").getValue();
            if (lifecycleStep && lifecycleStep.length && lifecycleStepNames.indexOf(lifecycleStep[0].name) != -1) {
                return true;
            }
            return false;
        }
        Case.IsLifeCycleStep = IsLifeCycleStep;
        function IsScheduledDOSGreaterOrEqualsToday(primaryControl, selectedControl) {
            var formContext = primaryControl;
            var today = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
            var surgeryDate = formContext.getAttribute("ipg_surgerydate").getValue();
            if (surgeryDate >= today) {
                return true;
            }
            return false;
        }
        Case.IsScheduledDOSGreaterOrEqualsToday = IsScheduledDOSGreaterOrEqualsToday;
        function IsDOSGreaterToday(primaryControl) {
            var _a, _b, _c;
            var formContext = primaryControl;
            var today = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
            var dos = (_b = (_a = formContext.getAttribute("ipg_actualdos")) === null || _a === void 0 ? void 0 : _a.getValue()) !== null && _b !== void 0 ? _b : (_c = formContext.getAttribute("ipg_surgerydate")) === null || _c === void 0 ? void 0 : _c.getValue();
            if (dos > today) {
                return true;
            }
            return false;
        }
        Case.IsDOSGreaterToday = IsDOSGreaterToday;
        function IsActualDOSEmpty(primaryControl, selectedControl) {
            var formContext = primaryControl;
            var actualdos = formContext.getAttribute("ipg_actualdos").getValue();
            if (!actualdos) {
                return true;
            }
            return false;
        }
        Case.IsActualDOSEmpty = IsActualDOSEmpty;
        var countOfEstimatedParts = null;
        function HasCaseAtLeastOneEstimatedPart(primaryControl, selectedControl) {
            var formContext = primaryControl;
            var caseId = formContext.data.entity.getId().replace("{", "").replace("}", "");
            if (countOfEstimatedParts === null) {
                Xrm.WebApi.retrieveMultipleRecords("ipg_estimatedcasepartdetail", "?$select=ipg_estimatedcasepartdetailid&$filter=_ipg_caseid_value eq " + caseId).then(function (result) {
                    countOfEstimatedParts = result.entities.length;
                    selectedControl ? selectedControl.refreshRibbon() : formContext.ui.refreshRibbon();
                });
            }
            else if (countOfEstimatedParts) {
                return true;
            }
            return false;
        }
        Case.HasCaseAtLeastOneEstimatedPart = HasCaseAtLeastOneEstimatedPart;
        function HasCaseAtLeastOneTPOEstimatedPart(primaryControl) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var formContext, caseId, estimatedParts;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            formContext = primaryControl;
                            caseId = formContext.data.entity.getId().replace("{", "").replace("}", "");
                            if (!(countOfEstimatedParts === null)) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_estimatedcasepartdetail", "?$select=ipg_estimatedcasepartdetailid&$top=1&$filter=_ipg_caseid_value eq " + caseId + " and ipg_potypecode eq 923720000")];
                        case 1:
                            estimatedParts = _b.sent();
                            return [2 /*return*/, ((_a = estimatedParts === null || estimatedParts === void 0 ? void 0 : estimatedParts.entities) === null || _a === void 0 ? void 0 : _a.length) > 0];
                        case 2: return [2 /*return*/];
                    }
                });
            });
        }
        Case.HasCaseAtLeastOneTPOEstimatedPart = HasCaseAtLeastOneTPOEstimatedPart;
        var countOfActualParts = null;
        function HasCaseAtLeastOneActualPart(primaryControl, selectedControl) {
            var formContext = primaryControl;
            var caseId = formContext.data.entity.getId().replace("{", "").replace("}", "");
            if (countOfActualParts === null) {
                Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=_ipg_caseid_value eq " + caseId).then(function (result) {
                    countOfActualParts = result.entities.length;
                    selectedControl ? selectedControl.refreshRibbon() : formContext.ui.refreshRibbon();
                });
            }
            else if (countOfActualParts) {
                return true;
            }
            return false;
        }
        Case.HasCaseAtLeastOneActualPart = HasCaseAtLeastOneActualPart;
        var countOfOpenedTissueRequestForm = null;
        function HasOpenedTissueRequestForm(primaryControl, selectedControl) {
            var formContext = primaryControl;
            var caseId = formContext.data.entity.getId().replace("{", "").replace("}", "");
            if (countOfOpenedTissueRequestForm === null) {
                Xrm.WebApi.retrieveMultipleRecords("task", "?$select=activityid&$filter=ipg_tasktypecode eq 427880048 and  statecode eq 0 and  _regardingobjectid_value eq " + caseId).then(function (result) {
                    countOfOpenedTissueRequestForm = result.entities.length;
                    selectedControl ? selectedControl.refreshRibbon() : formContext.ui.refreshRibbon();
                });
            }
            else if (countOfOpenedTissueRequestForm) {
                return true;
            }
            return false;
        }
        Case.HasOpenedTissueRequestForm = HasOpenedTissueRequestForm;
        function IsAllReceived(entityId) {
            var allReceived = null;
            entityId = Utility.removeCurlyBraces(entityId);
            var req = new XMLHttpRequest();
            req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/incidents(" + entityId + ")?$select=ipg_isallreceived", false);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    var result = JSON.parse(this.response);
                    req.onreadystatechange = null;
                    if (this.status === 200) {
                        allReceived = result["ipg_isallreceived"];
                    }
                    else {
                        var errorMessage = result.error && result.error.message || "Error during checking is all received property";
                        Xrm.Utility.alertDialog(errorMessage, null);
                    }
                }
            };
            req.send();
            return allReceived;
        }
        Case.IsAllReceived = IsAllReceived;
        function IsActualDOSLessThanToday(primaryControl, selectedControl) {
            var formContext = primaryControl;
            var today = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
            var actualdos = formContext.getAttribute("ipg_actualdos").getValue();
            if (actualdos && actualdos < today) {
                return true;
            }
            return false;
        }
        Case.IsActualDOSLessThanToday = IsActualDOSLessThanToday;
        function ConvertNotTPOTOTPO(EstimatedParts) {
            return __awaiter(this, void 0, void 0, function () {
                var notTPOPArts, _i, notTPOPArts_1, notTPO;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            notTPOPArts = EstimatedParts.filter(function (part) { var _a; return ((_a = part.ipg_potypecode) === null || _a === void 0 ? void 0 : _a.Value) !== POtypeEnum.TPO; });
                            _i = 0, notTPOPArts_1 = notTPOPArts;
                            _a.label = 1;
                        case 1:
                            if (!(_i < notTPOPArts_1.length)) return [3 /*break*/, 4];
                            notTPO = notTPOPArts_1[_i];
                            return [4 /*yield*/, Xrm.WebApi.updateRecord("ipg_estimatedcasepartdetail", notTPO.ipg_estimatedcasepartdetailId, { "ipg_potypecode": Number(POtypeEnum.TPO) })];
                        case 2:
                            _a.sent();
                            notTPO.ipg_potypecode.Value = Number(POtypeEnum.TPO);
                            _a.label = 3;
                        case 3:
                            _i++;
                            return [3 /*break*/, 1];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        }
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
