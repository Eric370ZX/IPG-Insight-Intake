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
 * @namespace Intake.Contact
 */
var Intake;
(function (Intake) {
    var Contact;
    (function (Contact) {
        var Document = /** @class */ (function () {
            function Document() {
            }
            return Document;
        }());
        if (typeof ($) === 'undefined') {
            $ = window.parent.$;
        }
        var GRID_ID = '#documentsGrid';
        var Xrm = window.parent.Xrm;
        //for DocumentRibbon script
        window.Xrm = Xrm;
        var PATIENT_ID = Xrm.Page.data.entity.getId();
        var DOCS_CASE_FETCH = "<fetch>\n  <entity name=\"ipg_document\" >\n    <attribute name=\"createdon\" />\n    <attribute name=\"ipg_documentid\" />\n    <attribute name=\"ipg_documenttypeid\" />\n    <attribute name=\"ipg_name\" />\n    <filter type=\"or\" >\n      <condition entityname=\"referral\" attribute=\"ipg_patientid\" operator=\"eq\" value=\"" + PATIENT_ID + "\" />\n    </filter>\n    <link-entity name=\"ipg_referral\" from=\"ipg_referralid\" to=\"ipg_referralid\" link-type=\"inner\" alias=\"referral\" />\n  </entity>\n</fetch>";
        var DOCS_REFERRALS_FETCH = "<fetch>\n  <entity name=\"ipg_document\" >\n    <attribute name=\"createdon\" />\n    <attribute name=\"ipg_documentid\" />\n    <attribute name=\"ipg_documenttypeid\" />\n    <attribute name=\"ipg_name\" />\n    <filter type=\"or\" >\n      <condition entityname=\"case\" attribute=\"ipg_patientid\" operator=\"eq\" value=\"" + PATIENT_ID + "\" />\n    </filter>\n    <link-entity name=\"incident\" from=\"incidentid\" to=\"ipg_caseid\" link-type=\"inner\" alias=\"case\" />\n  </entity>\n</fetch>";
        var docLinkFormatter = function (cellvalue, options, row) {
            var link = '';
            if (cellvalue) {
                link = '<a href="#" onclick="Intake.Contact.OpenDocumentForm(\'' + row.ipg_documentid + '\')">' +
                    cellvalue +
                    '</a>';
            }
            return link;
        };
        var dateTimeFormatter = function (cellvalue, options, row) {
            return new Date(cellvalue).toLocaleString();
        };
        function OpenDocumentForm(docId) {
            Xrm.Navigation.openForm({ entityName: "ipg_document", entityId: docId, openInNewWindow: true });
        }
        Contact.OpenDocumentForm = OpenDocumentForm;
        function GetRowsFromResponse(response, rows) {
            for (var i = 0; i < response.entities.length; i++) {
                var newDoc = {
                    ipg_documentid: response.entities[i].ipg_documentid,
                    ipg_name: response.entities[i].ipg_name,
                    ipg_documenttypename: response.entities[i]["_ipg_documenttypeid_value@OData.Community.Display.V1.FormattedValue"] || "",
                    createdon: response.entities[i].createdon
                };
                if (!rows.find(function (r) { return r.ipg_documentid === newDoc.ipg_documentid; })) {
                    rows.push(newDoc);
                }
            }
            return rows;
        }
        //Fetch Data From CRM
        function GetDataForGrid() {
            return __awaiter(this, void 0, void 0, function () {
                var rows, result, error_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            rows = [];
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 3, 4, 5]);
                            return [4 /*yield*/, Promise.all([Xrm.WebApi.retrieveMultipleRecords('ipg_document', "?fetchXml=" + DOCS_CASE_FETCH),
                                    Xrm.WebApi.retrieveMultipleRecords('ipg_document', "?fetchXml=" + DOCS_REFERRALS_FETCH)])];
                        case 2:
                            result = _a.sent();
                            result.map(function (r) { return GetRowsFromResponse(r, rows); });
                            return [3 /*break*/, 5];
                        case 3:
                            error_1 = _a.sent();
                            console.log(error_1);
                            return [3 /*break*/, 5];
                        case 4: return [2 /*return*/, rows];
                        case 5: return [2 /*return*/];
                    }
                });
            });
        }
        //Grid Initialization
        function initGrid() {
            return __awaiter(this, void 0, void 0, function () {
                var _a, _b, _c;
                return __generator(this, function (_d) {
                    switch (_d.label) {
                        case 0:
                            _b = (_a = $(GRID_ID)).jqGrid;
                            _c = {
                                datatype: 'local',
                                height: 'auto'
                            };
                            return [4 /*yield*/, GetDataForGrid()];
                        case 1:
                            _b.apply(_a, [(_c.data = _d.sent(),
                                    _c.autowidth = true,
                                    _c.rowNum = '100',
                                    _c.resizable = true,
                                    _c.hidegrid = false,
                                    _c.reload = true,
                                    _c.colNames = ['Row ID', 'Name', 'Type', 'Date'],
                                    _c.colModel = [
                                        { name: 'ipg_documentid', index: 'ipg_documentid', key: true, hidden: true },
                                        {
                                            name: 'ipg_name', index: 'ipg_name', width: 100,
                                            formatter: docLinkFormatter
                                        },
                                        { name: 'ipg_documenttypename', index: 'ipg_documenttypename', width: 100 },
                                        {
                                            name: 'createdon', index: 'createdon', width: 75, formatter: dateTimeFormatter
                                        }
                                    ],
                                    //when user sort rows new data fetched from CRM
                                    _c.onSortCol = function (index, columnIndex, sortOrder) {
                                        return __awaiter(this, void 0, void 0, function () {
                                            return __generator(this, function (_a) {
                                                switch (_a.label) {
                                                    case 0: return [4 /*yield*/, LoadData()];
                                                    case 1:
                                                        _a.sent();
                                                        return [2 /*return*/];
                                                }
                                            });
                                        });
                                    },
                                    _c.caption = 'Documents',
                                    _c.sortname = 'createdon',
                                    _c.sortorder = "desc",
                                    _c.pgbuttons = false,
                                    _c.viewrecords = false,
                                    _c.pgtext = "",
                                    _c.pginput = false,
                                    _c.multiselect = true,
                                    _c.toppager = true,
                                    _c.pagerpos = "right",
                                    _c.beforeSelectRow = function (rowid, e) {
                                        jQuery(GRID_ID).jqGrid('resetSelection');
                                        return true;
                                    },
                                    _c)]).navGrid(GRID_ID + "_toppager", { refresh: false, search: false, edit: false, view: false, del: false, add: false, position: "left" })
                                .navButtonAdd(GRID_ID + "_toppager", {
                                caption: "Preview Document",
                                buttonicon: "ui-icon-script",
                                onClickButton: function () {
                                    var selRowIds = jQuery(GRID_ID).jqGrid('getGridParam', 'selarrrow');
                                    if (!selRowIds || !selRowIds.length) {
                                        Xrm.Navigation.openErrorDialog({ message: "Please select document" });
                                        return;
                                    }
                                    else {
                                        Intake.Document.OpenDocumentPreview(Xrm.Page, selRowIds[0]);
                                    }
                                }
                            });
                            //Hide select all checkbox
                            $(GRID_ID + "_cb").remove();
                            return [2 /*return*/];
                    }
                });
            });
        }
        Contact.initGrid = initGrid;
        //Refresh Grid
        function LoadData() {
            return __awaiter(this, void 0, void 0, function () {
                var JQGrid, docs;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            JQGrid = jQuery(GRID_ID);
                            return [4 /*yield*/, GetDataForGrid()];
                        case 1:
                            docs = _a.sent();
                            JQGrid.jqGrid('setGridParam', { data: docs }).trigger('reloadGrid');
                            return [2 /*return*/];
                    }
                });
            });
        }
    })(Contact = Intake.Contact || (Intake.Contact = {}));
})(Intake || (Intake = {}));
