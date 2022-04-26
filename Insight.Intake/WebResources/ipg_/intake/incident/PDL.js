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
        if (typeof ($) === 'undefined') {
            $ = window.parent.$;
        }
        var data = window.location.search.split('=');
        var params = decodeURIComponent(data[1]);
        var keyValue = params.split('=');
        var parameters = JSON.parse(keyValue[1]);
        /**
        * Fill PDL
        * @function Intake.Incident.FillPDL
        * @returns {void}
        */
        function FillPDL() {
            FillIpgAddress();
            var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
            var caseRecord = [];
            var url = baseUrl
                + "IncidentSet?"
                + "$select=ipg_PatientFirstName,ipg_PatientLastName,ipg_SurgeryDate,Title,ipg_PhysicianId,ipg_Facility,ipg_CPTCodeId1,ipg_DxCodeId1"
                + "&$filter=IncidentId eq (guid'" + parameters.caseId + "')";
            GetRecords(url, caseRecord);
            if (caseRecord.length > 0) {
                $('#patient').html(caseRecord[0].ipg_PatientFirstName + ' ' + caseRecord[0].ipg_PatientLastName);
                $('#surgeryDate').html(formatDate(ToDateTime(caseRecord[0].ipg_SurgeryDate)));
                $('#title').html(caseRecord[0].Title);
                $('#physician').html(caseRecord[0].ipg_PhysicianId.Name);
                $('#facility').html(caseRecord[0].ipg_Facility);
                $('#cpt').html(caseRecord[0].ipg_CPTCodeId1.Name);
                $('#dx').html(caseRecord[0].ipg_DxCodeId1.Name);
            }
            var parts = [];
            var sum = 0;
            url = baseUrl
                + "SalesOrderDetailSet?"
                + "$select=ProductId,Quantity,ExtendedAmount"
                + "&$filter=ipg_caseid/Id eq (guid'" + parameters.caseId + "')";
            GetRecords(url, parts);
            parts.forEach(function (part) {
                var partRecord = [];
                url = baseUrl
                    + "ProductSet?"
                    + "$select=ipg_HCPCSCodeId,ipg_manufacturerpartnumber"
                    + "&$filter=ProductId eq (guid'" + part.ProductId.Id + "')";
                GetRecords(url, partRecord);
                var HCPCS = "";
                var partNumber = "";
                if (partRecord.length > 0) {
                    if (partRecord[0].ipg_HCPCSCodeId.Name)
                        HCPCS = partRecord[0].ipg_HCPCSCodeId.Name;
                    if (partRecord[0].ipg_manufacturerpartnumber)
                        partNumber = partRecord[0].ipg_manufacturerpartnumber;
                }
                var markup = "<tr><td width='15%'>" + HCPCS + "</td><td width='10%' style='text-align:center'>" + parseFloat(part.Quantity) + "</td><td width='45%'>" + part.ProductId.Name + "</td><td width='15%'>" + partNumber + "</td><td width='15%'  style='text-align:right'>$" + parseFloat(part.ExtendedAmount.Value).toLocaleString('en-us', { minimumFractionDigits: 2 }) + "</td></tr>";
                sum += parseFloat(part.ExtendedAmount.Value);
                $("#parts").append(markup);
            });
            var markup = "<tr><td colspan='4' style='text-align:right'>Total Billed</td><td style='text-align:right'>$" + sum.toLocaleString('en-us', { minimumFractionDigits: 2 }) + "</td></tr>";
            $("#parts").append(markup);
            $('#currentdate').html(formatDate(new Date()));
        }
        Incident.FillPDL = FillPDL;
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
        function FillIpgAddress() {
            return __awaiter(this, void 0, void 0, function () {
                var ipgAddress;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Intake.Utility.GetIPGAddress(parent.Xrm.WebApi)];
                        case 1:
                            ipgAddress = _a.sent();
                            $("#IPGAddress").html(ipgAddress.street.replace('-', '*') + " * " + ipgAddress.city + ", " + ipgAddress.state + " " + ipgAddress.zip);
                            return [2 /*return*/];
                    }
                });
            });
        }
    })(Incident = Intake.Incident || (Intake.Incident = {}));
})(Intake || (Intake = {}));
