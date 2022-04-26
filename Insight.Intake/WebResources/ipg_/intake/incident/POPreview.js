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
    var Incident;
    (function (Incident) {
        var WebApi = parent.Xrm.WebApi;
        var notRequired = 'Not Required';
        var Facility = /** @class */ (function () {
            function Facility() {
            }
            return Facility;
        }());
        ;
        var Contact = /** @class */ (function () {
            function Contact() {
            }
            return Contact;
        }());
        var Header = /** @class */ (function () {
            function Header() {
                this.title = '';
                this.manufacturer = { name: '', number: '', mfgRep: '', suppresshipaa: false, email: '' };
                this.physician = { name: '' };
                this.facility = { name: '', stateZip: '', shipingAddress: '', equipmentListRcv: '', FacilityMdd: '', contact: { name: '', phone: '', email: '' } };
                this.patient = { name: '', firstName: '', lastName: '' };
                this.ipgAddress = { zip: '', state: '', city: '', street: '' };
            }
            ;
            return Header;
        }());
        if (typeof ($) === 'undefined') {
            $ = window.parent.$;
        }
        var data = new URLSearchParams(window.location.search).get("data");
        var parameters = JSON.parse(decodeURIComponent(data));
        function RefreshParent() {
            if (window.top.opener && window.top.opener.Xrm) {
                var xrm = window.top.opener.Xrm;
                xrm.Page.ui.refreshRibbon();
                var PatientProcedureDetailsTab = xrm.Page.ui.tabs.get("PatientProcedureDetails");
                if (PatientProcedureDetailsTab) {
                    var purchaseOrderProductsSection = PatientProcedureDetailsTab.sections.get("PurchaseOrderProducts");
                    if (purchaseOrderProductsSection) {
                        purchaseOrderProductsSection.setVisible(true);
                    }
                }
            }
        }
        /**
        * PO Preview
        * @function Intake.Incident.POPreview
        * @returns {void}
        */
        function POPreview() {
            return __awaiter(this, void 0, void 0, function () {
                var baseUrl, headerData, POtypeValue, dataLines, modelNumbers_1, mfgRep;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            Xrm.Utility.showProgressIndicator("");
                            caseId = parameters.caseId;
                            manufacturerId = parameters.accountId;
                            salesOrderId = parameters.salesOrderId;
                            POType = parameters.POType;
                            estimated = parameters.estimated;
                            baseUrl = "/XRMServices/2011/OrganizationData.svc/";
                            return [4 /*yield*/, GetHeader(baseUrl, caseId, manufacturerId, POType)];
                        case 1:
                            headerData = _a.sent();
                            $('#GeneratePO').prop('value', 'Generate PO ' + POType);
                            POtypeValue = "";
                            switch (POType) {
                                case "TPO":
                                    GenerateZPO_TPOHeader(headerData);
                                    POtypeValue = "923720000";
                                    break;
                                case "ZPO":
                                    GenerateZPO_TPOHeader(headerData);
                                    POtypeValue = "923720001";
                                    break;
                                case "CPA":
                                    GenerateMPOHeader(headerData);
                                    POtypeValue = "923720002";
                                    break;
                                case "MPO":
                                    $("#communicateToRow").css("display", "table-row");
                                    GenerateMPOHeader(headerData);
                                    POtypeValue = "923720004";
                                    break;
                            }
                            dataLines = GetDataLines(baseUrl, caseId, manufacturerId, salesOrderId, POtypeValue, estimated);
                            if (dataLines && dataLines.parts && dataLines.products) {
                                modelNumbers_1 = '';
                                dataLines.parts.forEach(function (part) {
                                    modelNumbers_1 += part.ipg_manufacturerpartnumber && modelNumbers_1.indexOf(part.ipg_manufacturerpartnumber) < 0 ? "".concat((modelNumbers_1.length > 0 ? ', ' : '')).concat(part.ipg_manufacturerpartnumber) : '';
                                    var quantity = 0;
                                    for (var j = 0; j < dataLines.products.length; j++) {
                                        if (dataLines.products[j].key == part.ProductId)
                                            quantity += parseFloat(dataLines.products[j].value);
                                    }
                                    var markup = "<tr><td style='text-align:center'>" + ((part.Description) ? part.Description : '') + "</td><td style='text-align:center'>" + part.ipg_manufacturerpartnumber + "</td><td style='text-align:center'></td><td style='text-align:center'>" + quantity.toLocaleString('en-us', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + "</td></tr>";
                                    $("#parts").append(markup);
                                });
                                mfgRep = $('#manufacturerRep');
                                if (mfgRep) {
                                    mfgRep.html(modelNumbers_1);
                                }
                            }
                            Xrm.Utility.closeProgressIndicator();
                            return [2 /*return*/];
                    }
                });
            });
        }
        Incident.POPreview = POPreview;
        /**
        * type cast OData DateTime type to CRM DateTime
        * @function Intake.Incident.ToDateTime
        * @returns {Date}
        */
        function ToDateTime(dt) {
            dt = dt.replace("/Date(", "");
            dt = dt.replace(") /", "");
            return new Date(parseInt(dt, 10));
        }
        /**
        * type cast DateTime to string
        * @function Intake.Incident.ToDateTime
        * @returns {string}
        */
        function formatDate(date) {
            var d = new Date(date), month = '' + (d.getMonth() + 1), day = '' + d.getDate(), year = d.getFullYear();
            if (month.length < 2)
                month = '0' + month;
            if (day.length < 2)
                day = '0' + day;
            return [month, day, year].join('/');
        }
        /**
        * Gets records using oData
        * @function Intake.Incident.GetRecords
        * @returns {array}
        */
        function GetRecords(url, entities) {
            $.ajax({
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: url,
                async: false,
                beforeSend: function (XMLHttpRequest) {
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                },
                success: function (data, textStatus, XmlHttpRequest) {
                    if (data && data.d != null && data.d.results != null) {
                        AddRecordsToArray(data.d.results, entities);
                        FetchRecordsCallBack(data.d, entities);
                    }
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    alert("Error :  has occurred during retrieval of the records ");
                    console.log(XmlHttpRequest.responseText);
                }
            });
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
        ;
        function IsValidEmail() {
            var emailStr = $("#emailaddress").val().toString();
            if (emailStr) {
                var emails = emailStr.split(';');
                return emails.every(checkEmail);
            }
            else {
                return false;
            }
        }
        function checkEmail(email) {
            if (email.trim()) {
                return validateEmail(email.trim());
            }
            else {
                return true;
            }
        }
        function validateEmail(email) {
            var re = /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
            return re.test(email);
        }
        /**
        * Generate PO
        * @function Intake.Incident.POPreview
        * @returns {void}
        */
        function GeneratePO() {
            if (POType == "MPO") {
                if (!IsValidEmail() && $("#communicate-po-checkbox").prop("checked")) {
                    Xrm.Navigation.openConfirmDialog({ text: "Manufacturer email address is missing or invalid!" }).then(function (success) {
                        if (success.confirmed) {
                            GeneratePOContinue();
                        }
                    });
                }
                else {
                    GeneratePOContinue();
                }
            }
            else if (POType == "TPO") {
                var CPTcodes = ["29881", "65426", "29888", "65755", "67255", "65756", "19342", "66180", "29877", "65710", "29882", "65755", "11970", "29870"];
                Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title&$expand=ipg_CPTCodeId1($select=ipg_cptcode),ipg_CPTCodeId2($select=ipg_cptcode),ipg_CPTCodeId3($select=ipg_cptcode),ipg_CPTCodeId4($select=ipg_cptcode),ipg_CPTCodeId5($select=ipg_cptcode),ipg_CPTCodeId6($select=ipg_cptcode)").then(function success(result) {
                    if (result["ipg_CPTCodeId1"] && CPTcodes.find(function (x) { return x == result["ipg_CPTCodeId1"]["ipg_cptcode"]; })) {
                        Xrm.WebApi.retrieveMultipleRecords("ipg_document", "?$select=ipg_documentid&$filter=_ipg_caseid_value eq " + caseId + " and  _ipg_documenttypeid_value eq 305A4E30-3AEA-7905-37DE-50537821D7AE and statecode eq 0")
                            .then(function (result) {
                            if (result.entities.length == 0) {
                                var alertStrings = { confirmButtonLabel: "OK", text: "Please add Tissue Request Form the case. Tissue Request Form is required document for TPO." };
                                var alertOptions = { height: 150, width: 300 };
                                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function success(result) {
                                }, function (error) {
                                    console.log(error.message);
                                });
                            }
                            else
                                GeneratePOContinue();
                        }, function (error) {
                            Xrm.Navigation.openErrorDialog({ message: error.message });
                        });
                    }
                    else
                        GeneratePOContinue();
                });
            }
            else
                GeneratePOContinue();
        }
        Incident.GeneratePO = GeneratePO;
        function GeneratePOContinue() {
            var _a;
            var email = $("#emailaddress").length > 0 && $("#emailaddress")[0].value;
            var communicatePo = (_a = document.getElementById("communicate-po-checkbox")) === null || _a === void 0 ? void 0 : _a.checked;
            var parameters = {
                entity: {
                    id: caseId,
                    entityType: "incident"
                },
                manufacturer: {
                    id: manufacturerId,
                    entityType: "account"
                },
                previousPO: {
                    id: salesOrderId,
                    entityType: "salesorder"
                },
                POType: POType,
                EstimatedPO: estimated
            };
            var parameterTypes = {
                "entity": {
                    "typeName": "mscrm.incident",
                    "structuralProperty": 5
                },
                "POType": {
                    "typeName": "Edm.String",
                    "structuralProperty": 1
                },
                "CommunicatePo": {
                    "typeName": "Edm.boolean",
                    "structuralProperty": 1
                },
                "previousPO": {
                    "typeName": "mscrm.salesorder",
                    "structuralProperty": 5
                },
                "manufacturer": {
                    "typeName": "mscrm.account",
                    "structuralProperty": 5
                },
                "EstimatedPO": {
                    "typeName": "Edm.boolean",
                    "structuralProperty": 1
                }
            };
            var request = {
                entity: parameters.entity,
                previousPO: parameters.previousPO,
                manufacturer: parameters.manufacturer,
                POType: parameters.POType,
                EstimatedPO: parameters.EstimatedPO,
                CommunicatePo: communicatePo,
                getMetadata: function () {
                    return {
                        boundParameter: "entity",
                        parameterTypes: parameterTypes,
                        operationType: 0,
                        operationName: "ipg_IPGIntakeCaseActionsGeneratePO"
                    };
                }
            };
            IsValidEmail();
            var confirmStrings = {
                text: "The system will attempt to generate POs for Manufacturers\n based on the rules defined for the Carrier, \n Manufacturer and Facility. Continue?",
                title: ""
            };
            var confirmOptions = { height: 200, width: 450 };
            Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(function (success) {
                if (success.confirmed) {
                    Xrm.Utility.showProgressIndicator(null);
                    Xrm.WebApi.online.execute(request)
                        .then(function (response) {
                        if (response.ok) {
                            return response.json().then(function (responseData) {
                                if (responseData.Success && responseData.poId) {
                                    var afterGenTypes_1 = {
                                        "entity": {
                                            "typeName": "mscrm.salesorder",
                                            "structuralProperty": 5
                                        },
                                        "CommunicatePo": {
                                            "typeName": "Edm.boolean",
                                            "structuralProperty": 1
                                        },
                                    };
                                    var afterGenRequest = {
                                        entity: {
                                            id: responseData.poId.salesorderid,
                                            entityType: "salesorder"
                                        },
                                        CommunicatePo: communicatePo,
                                        getMetadata: function () {
                                            return {
                                                boundParameter: "entity",
                                                parameterTypes: afterGenTypes_1,
                                                operationType: 0,
                                                operationName: "ipg_IPGIntakeOrderActionsAfterGeneration"
                                            };
                                        }
                                    };
                                    if (POType == "MPO" && email) {
                                        afterGenTypes_1["CommunicateTo"] = {
                                            "typeName": "Edm.String",
                                            "structuralProperty": 1
                                        };
                                        afterGenRequest["CommunicateTo"] = email;
                                    }
                                    Xrm.WebApi.online.execute(afterGenRequest)
                                        .then(function (response) {
                                        RefreshParent();
                                        Xrm.Utility.closeProgressIndicator();
                                        Xrm.Navigation.openForm({ 'entityName': 'salesorder', 'entityId': responseData.poId.salesorderid });
                                    })
                                        .catch(function (error) {
                                        console.log(JSON.stringify(error));
                                        RefreshParent();
                                        Xrm.Utility.closeProgressIndicator();
                                        Xrm.Navigation.openForm({ 'entityName': 'salesorder', 'entityId': responseData.poId.salesorderid, cmdbar: true, navBar: 'on', openInNewWindow: false });
                                    });
                                }
                                else {
                                    Xrm.Utility.closeProgressIndicator();
                                    var alertStrings = { confirmButtonLabel: "OK", text: "All Parts has quantity of 0.\n Previous POs have been voided!" };
                                    Xrm.Navigation.openAlertDialog(alertStrings);
                                }
                            });
                        }
                        else {
                            Xrm.Utility.closeProgressIndicator();
                            Xrm.Navigation.openErrorDialog({ message: response.statusText });
                        }
                    }, function (error) {
                        Xrm.Navigation.openErrorDialog({ message: error.message });
                        Xrm.Utility.closeProgressIndicator();
                    });
                }
            });
        }
        Incident.GeneratePOContinue = GeneratePOContinue;
        /**
        * Generate Header for ZPO
        * @function Intake.Incident.GenerateZPOHeader
        * @returns {void}
        */
        function GenerateZPO_TPOHeader(data) {
            $('#zpoManufacturerName').html(data.manufacturer.name);
            $('#zpofacilityName').html(data.facility.name);
            $('#zpocontactName').html(data.facility.contact.name);
            $('#zposhippingAddress').html(data.facility.shipingAddress);
            $('#zpofacilityStateZip').html(data.facility.stateZip);
            $('#zpoPhone').html(data.facility.contact.phone);
            $('#zpoEmail').html(data.facility.contact.email);
            $('#zpoIPGAccountName').html("IPG ACCOUNT with " + data.manufacturer.name);
            $('#zpoIPGAccountNumber').html(data.manufacturer.number);
            $("#IPGAddress").html(data.ipgAddress.street);
            $("#IPGCityStateZip").html("".concat(data.ipgAddress.city, ", ").concat(data.ipgAddress.state, " ").concat(data.ipgAddress.zip));
            $("#zpoHeader").css("display", "table");
        }
        /**
        * Generate Header for MPO
        * @function Intake.Incident.GenerateMPOHeader
        * @returns {void}
        */
        function GenerateMPOHeader(data) {
            $('#manufacturerRep').html(data.manufacturer.mfgRep);
            $('#IPGRep').html(data.facility.FacilityMdd);
            $('#equipmentListRcv').html(data.facility.equipmentListRcv);
            $('#manufacturer').html(data.manufacturer.name);
            $('#accountNumber').html(data.manufacturer.number);
            $('#title').html(data.title);
            $('#physician').html(data.physician.name);
            $('#facility').html(data.facility.name);
            $('#facilityStateZip').html(data.facility.stateZip);
            $('#patient').html(data.patient.name);
            $('#datePO').html(data.poDate);
            $('#surgeryDate').html(data.surgeryDate);
            $('#procedure').html('Procedure: ' + data.procedureName);
            $('#emailaddress').val(data.manufacturer.email);
            $("#mpoHeader").css("display", "table");
        }
        /**
        * Get information for PO Header
        * @function Intake.Incident.GetHeader
        * @returns Promise<Header>
        */
        function GetHeader(baseUrl, caseId, manufacturerId, poType) {
            return __awaiter(this, void 0, void 0, function () {
                var header, _case, facility, manufacturer, _a;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            header = new Header();
                            header.poDate = formatDate(new Date());
                            _case = GetCase(baseUrl, caseId);
                            return [4 /*yield*/, GetFacility(baseUrl, _case.facility.Id)];
                        case 1:
                            facility = _b.sent();
                            return [4 /*yield*/, GetManufacturer(baseUrl, manufacturerId, _case.facility.Id)];
                        case 2:
                            manufacturer = _b.sent();
                            if (facility && !facility.contact) {
                                facility.contact = header.facility.contact;
                            }
                            header.surgeryDate = _case.surgeryDate || header.surgeryDate;
                            header.procedureName = _case.procedureName || header.procedureName;
                            header.physician.name = _case.physician.name || header.physician.name;
                            header.title = _case.title || header.title;
                            header.facility.name = facility && facility.name || header.facility.name;
                            header.facility.stateZip = facility && facility.stateZip || header.facility.stateZip;
                            header.facility.shipingAddress = facility && facility.shipingAddress || header.facility.shipingAddress;
                            header.facility.contact.name = facility && facility.contact && facility.contact.name || header.facility.contact.name;
                            header.facility.contact.email = facility && facility.contact && facility.contact.email || header.facility.contact.email;
                            header.facility.contact.phone = facility && facility.contact && facility.contact.phone || header.facility.contact.phone;
                            header.facility.equipmentListRcv = facility && facility.equipmentListRcv || header.facility.equipmentListRcv;
                            header.manufacturer.name = manufacturer && manufacturer.name || header.manufacturer.name;
                            header.manufacturer.email = manufacturer && manufacturer.email || header.manufacturer.email;
                            header.manufacturer.mfgRep = manufacturer && manufacturer.mfgRep || header.manufacturer.mfgRep;
                            header.manufacturer.suppresshipaa = manufacturer && manufacturer.suppresshipaa || header.manufacturer.suppresshipaa;
                            if (poType != "CPA") {
                                if (header.manufacturer.suppresshipaa) {
                                    header.patient.name = "";
                                }
                                else if (_case.patient) {
                                    header.patient.name = (_case.patient.lastName.slice(0, 3) + '_' + _case.patient.firstName.slice(0, 3)).toUpperCase();
                                }
                                else {
                                    header.patient.name = (header.patient.lastName.slice(0, 3) + '_' + header.patient.firstName.slice(0, 3)).toUpperCase();
                                }
                                header.manufacturer.number = manufacturer && manufacturer.number || header.manufacturer.number;
                            }
                            else {
                                header.patient.name = _case.patient && _case.patient.name || header.patient.name;
                                header.manufacturer.number = notRequired;
                            }
                            _a = header;
                            return [4 /*yield*/, Intake.Utility.GetIPGAddress(WebApi)];
                        case 3:
                            _a.ipgAddress = (_b.sent()) || header.ipgAddress;
                            return [2 /*return*/, header];
                    }
                });
            });
        }
        /**
       * Gets Manufacturer Info
       * @function Intake.Incident.GetManufacturer
       * @returns {name: string, number: string}
       */
        function GetManufacturer(baseUrl, manufacturerId, facilityId) {
            return __awaiter(this, void 0, void 0, function () {
                var manufacturerRecord, url, mfg, manufacturer, query, manufacturers, facilityManufactureRelationShip;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!manufacturerId) return [3 /*break*/, 2];
                            manufacturerRecord = [];
                            url = baseUrl
                                + "AccountSet?"
                                + "$select=Name,ipg_manufactureraccountnumber, ipg_manufacturerprimaryemail,ipg_manufacturerprimaryoptions, ipg_ManufacturerIsFacilityAcctRequired, ipg_ParentAccound, ipg_suppresshipaa, ipg_Ccmmunicateposto"
                                + "&$filter=AccountId eq (guid'" + manufacturerId + "')";
                            GetRecords(url, manufacturerRecord);
                            if (!(manufacturerRecord.length > 0)) return [3 /*break*/, 2];
                            mfg = manufacturerRecord[0];
                            manufacturer = {
                                name: mfg.Name,
                                number: '',
                                email: mfg.ipg_Ccmmunicateposto,
                                singlesource: mfg.ipg_manufacturerprimaryoptions,
                                suppresshipaa: mfg.ipg_suppresshipaa,
                                mfgRep: ''
                            };
                            while (mfg && mfg.ipg_ParentAccound && mfg.ipg_ParentAccound.Id) {
                                query = baseUrl
                                    + "AccountSet?"
                                    + "$select= ipg_ManufacturerIsFacilityAcctRequired"
                                    + "&$filter=AccountId eq (guid'" + mfg.ipg_ParentAccound.Id + "')";
                                manufacturers = [];
                                GetRecords(query, manufacturers);
                                mfg = manufacturers[0];
                            }
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_facilitymanufacturerrelationship", "?$top=1&$select=ipg_manufactureraccountnumber,ipg_manufacturerrepemail&$filter=_ipg_facilityid_value eq ".concat(facilityId, " and _ipg_manufacturerid_value eq ").concat(manufacturerId))];
                        case 1:
                            facilityManufactureRelationShip = _a.sent();
                            //manufacturer.email = manufacturer.singlesource ? manufacturer.email : (facilityManufactureRelationShip.entities.length > 0 && facilityManufactureRelationShip.entities[0].ipg_manufacturerrepemail);
                            if (!mfg.ipg_ManufacturerIsFacilityAcctRequired) {
                                manufacturer.number = notRequired;
                            }
                            else {
                                manufacturer.number = facilityManufactureRelationShip.entities.length > 0 && facilityManufactureRelationShip.entities[0].ipg_manufactureraccountnumber || '';
                            }
                            return [2 /*return*/, manufacturer];
                        case 2: return [2 /*return*/, null];
                    }
                });
            });
        }
        function GetMfgModelNumbers(baseUrl, manufacturerId) {
            if (manufacturerId) {
                var manufacturerPartRecord = [];
                var url = "".concat(baseUrl, "ProductSet?$select=ipg_manufacturerpartnumber&$filter=ipg_manufacturerid/Id eq (guid'").concat(manufacturerId, "')");
                GetRecords(url, manufacturerPartRecord);
                var result_1 = '';
                manufacturerPartRecord.forEach(function (p) { return result_1 += p.ipg_manufacturerpartnumber ? ", ".concat(p.ipg_manufacturerpartnumber) : ''; });
                return result_1;
            }
            return null;
        }
        /**
        * Gets Case Info
        * @function Intake.Incident.GetCase
        * @returns any
        */
        function GetCase(baseUrl, caseId) {
            var caseRecord = [];
            var url = baseUrl
                + "IncidentSet?"
                + "$select=Title,ipg_PhysicianId,ipg_FacilityId,ipg_PatientFirstName,ipg_PatientLastName,ipg_SurgeryDate,ipg_procedureid,ipg_accurate_equipment_list_received"
                + "&$filter=IncidentId eq (guid'" + caseId + "')";
            GetRecords(url, caseRecord);
            if (caseRecord.length > 0) {
                var _case = {};
                if (caseRecord[0].Title) {
                    _case.title = caseRecord[0].Title;
                }
                if (caseRecord[0].ipg_PhysicianId) {
                    _case.physician = {};
                    _case.physician.name = caseRecord[0].ipg_PhysicianId.Name;
                    _case.physician.Id = caseRecord[0].ipg_PhysicianId.Id;
                }
                if (caseRecord[0].ipg_FacilityId) {
                    _case.facility = {};
                    _case.facility.name = caseRecord[0].ipg_FacilityId.Name;
                    _case.facility.Id = caseRecord[0].ipg_FacilityId.Id;
                }
                if (caseRecord[0].ipg_PatientFirstName) {
                    _case.patient = {};
                    _case.patient.firstName = caseRecord[0].ipg_PatientFirstName || '';
                }
                if (caseRecord[0].ipg_PatientLastName) {
                    _case.patient.lastName = caseRecord[0].ipg_PatientLastName || '';
                }
                _case.patient.name = caseRecord[0].ipg_PatientFirstName + ' ' + caseRecord[0].ipg_PatientLastName;
                if (caseRecord[0].ipg_SurgeryDate) {
                    _case.surgeryDate = formatDate(ToDateTime(caseRecord[0].ipg_SurgeryDate));
                }
                if (caseRecord[0].ipg_procedureid) {
                    _case.procedureName = caseRecord[0].ipg_procedureid.Name;
                }
                if (caseRecord[0].ipg_accurate_equipment_list_received) {
                    _case.equipmentListRcv = formatDate(ToDateTime(caseRecord[0].ipg_accurate_equipment_list_received));
                }
                return _case;
            }
            return null;
        }
        /**
       * Gets Facility Info
       * @function Intake.Incident.GetFacility
       * @returns Promise<Facility>
       */
        function GetFacility(baseUrl, facilityId) {
            return __awaiter(this, void 0, void 0, function () {
                var facilityRecord, Geturl, query, facility;
                return __generator(this, function (_a) {
                    if (facilityId) {
                        facilityRecord = [];
                        Geturl = function (id) { return baseUrl
                            + "AccountSet?"
                            + "$select=Name,Address1_Line1,Address1_City,Address1_PostalCode,Address1_Telephone1,EMailAddress1,ipg_StateId,ParentAccountId,ipg_FacilityMddId,Address1_PrimaryContactName"
                            + "&$filter=AccountId eq (guid'" + id + "')"; };
                        GetRecords(Geturl(facilityId), facilityRecord);
                        if (facilityRecord.length > 0) {
                            if (facilityRecord[0].ParentAccountId && facilityRecord[0].ParentAccountId.Id) {
                                query = Geturl(facilityRecord[0].ParentAccountId.Id);
                                facilityRecord = [];
                                GetRecords(query, facilityRecord);
                            }
                            facility = new Facility();
                            facility.contact = new Contact();
                            facility.stateZip = "";
                            if (facilityRecord[0].ipg_FacilityMddId) {
                                facility.FacilityMdd = facilityRecord[0].ipg_FacilityMddId.Name;
                            }
                            if (facilityRecord[0].Address1_City) {
                                facility.stateZip = facilityRecord[0].Address1_City;
                            }
                            if (facilityRecord[0].ipg_StateId && facilityRecord[0].ipg_StateId.Name) {
                                facility.stateZip += (facility.stateZip ? ", " : "") + facilityRecord[0].ipg_StateId.Name;
                            }
                            if (facilityRecord[0].Address1_PostalCode) {
                                facility.stateZip += (facility.stateZip ? ", " : "") + facilityRecord[0].Address1_PostalCode;
                            }
                            if (facilityRecord[0].Address1_Line1) {
                                facility.shipingAddress = facilityRecord[0].Address1_Line1;
                            }
                            if (facilityRecord[0].Name) {
                                facility.name = facilityRecord[0].Name;
                            }
                            if (facilityRecord[0].Address1_Telephone1) {
                                facility.contact.phone = facilityRecord[0].Address1_Telephone1;
                            }
                            if (facilityRecord[0].EMailAddress1) {
                                facility.contact.email = facilityRecord[0].EMailAddress1;
                            }
                            if (facilityRecord[0].Address1_PrimaryContactName) {
                                facility.contact.name = facilityRecord[0].Address1_PrimaryContactName;
                            }
                            //facility.contact = await GetMatManager(facilityId);
                            return [2 /*return*/, facility];
                        }
                    }
                    return [2 /*return*/, null];
                });
            });
        }
        /**
       * Gets Contact with role MatManager
       * @function Intake.Incident.GetMatManager
       * @returns Promise<{ name: string, phone: string, email: string }>
       */
        function GetMatManager(accountId) {
            return __awaiter(this, void 0, void 0, function () {
                var materialCode, contactRecord, result, e_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            materialCode = 923720004;
                            if (!accountId) return [3 /*break*/, 5];
                            contactRecord = { name: null, phone: null, email: null };
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 3, , 4]);
                            return [4 /*yield*/, WebApi.retrieveMultipleRecords('ipg_contactsaccounts', '?$top=1&$select=ipg_contactsaccountsid&$expand=ipg_contactid($select=fullname,emailaddress1,telephone1)'
                                    + "&$filter=ipg_contactrolecode eq '".concat(materialCode, "' and _ipg_accountid_value eq ").concat(accountId, " and ipg_contactid / contactid ne null"))];
                        case 2:
                            result = _a.sent();
                            if (result.entities && result.entities.length >= 1) {
                                contactRecord.name = result.entities[0].ipg_contactid.fullname;
                                contactRecord.phone = result.entities[0].ipg_contactid.telephone1;
                                contactRecord.email = result.entities[0].ipg_contactid.emailaddress1;
                            }
                            return [3 /*break*/, 4];
                        case 3:
                            e_1 = _a.sent();
                            alert("Error :  has occurred during retrieval of the records ");
                            console.log(e_1.error);
                            return [3 /*break*/, 4];
                        case 4: return [2 /*return*/, contactRecord];
                        case 5: return [2 /*return*/, null];
                    }
                });
            });
        }
        /**
      * Gets Data to display Lines
      * @function Intake.Incident.GetDataLines
      * @returns any
      */
        function GetDataLines(baseUrl, caseId, manufacturerId, salesOrderId, poTypeValue, estimated) {
            if (estimated === void 0) { estimated = false; }
            var url = baseUrl
                + "ipg_casepartdetailSet?"
                + "$select=ipg_productid,ipg_quantity"
                + "&$filter=ipg_caseid/Id eq (guid'" + caseId + "') and ipg_potypecode/Value eq " + poTypeValue
                + " and ipg_IsChanged eq true";
            if (salesOrderId) {
                url += " and ipg_PurchaseOrderId/Id eq (guid'" + salesOrderId + "')";
            }
            else {
                url += " and ipg_PurchaseOrderId eq null";
            }
            var casepartdetails = [];
            var products = [];
            if (poTypeValue === "923720000" && estimated) {
                url = baseUrl
                    + "ipg_estimatedcasepartdetailSet?"
                    + "$select=ipg_productid,ipg_quantity"
                    + "&$filter=ipg_caseid/Id eq (guid'" + caseId + "') and ipg_potypecode/Value eq " + poTypeValue;
            }
            GetRecords(url, casepartdetails);
            for (var i = 0; i < casepartdetails.length; i++) {
                //products.push(casepartdetails[i].ipg_productid.Id)
                var flag = true;
                for (var j = 0; j < products.length; j++) {
                    if (products[j].key == casepartdetails[i].ipg_productid.Id) {
                        flag = false;
                        products[j].value += casepartdetails[i].ipg_quantity;
                    }
                }
                if (flag)
                    products.push({ key: casepartdetails[i].ipg_productid.Id, value: casepartdetails[i].ipg_quantity });
            }
            if (products.length) {
                url = baseUrl
                    + "ProductSet?"
                    + "$select=ipg_manufacturerpartnumber,Description,ProductId";
                if (manufacturerId)
                    url += "&$filter=ipg_manufacturerid/Id eq (guid'" + manufacturerId + "')" + (POType != "TPO" ? " and ipg_status/Value eq 923720000" : "") + " and (";
                else
                    url += "&$filter=(";
                for (var i = 0; i < products.length; i++) {
                    if (i < products.length - 1)
                        url += "ProductId eq (guid'" + products[i].key + "') or ";
                    else
                        url += "ProductId eq (guid'" + products[i].key + "'))";
                }
                var parts = [];
                GetRecords(url, parts);
                return {
                    parts: parts,
                    products: products
                };
            }
            return null;
        }
    })(Incident = Intake.Incident || (Intake.Incident = {}));
})(Intake || (Intake = {}));
