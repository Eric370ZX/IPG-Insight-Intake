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
 * @namespace Intake.Document
 */
var Intake;
(function (Intake) {
    var Document;
    (function (Document) {
        //Custom formatter for the document name column
        var docLinkFormatter = function (cellvalue, options, row) {
            var link = '';
            if (cellvalue) {
                link = '<a href="#" onclick="Intake.Document.OpenDocumentForm(\'' + row.ipg_documentid + '\')">' +
                    cellvalue +
                    '</a>';
            }
            return link;
        };
        //Custom formatter for datetime columns
        var dateTimeFormatter = function (cellvalue, options, row) {
            return new Date(cellvalue).toLocaleString();
        };
        function OpenDocumentForm(docId) {
            parent.Xrm.Navigation.openForm({ entityName: "ipg_document", entityId: docId, openInNewWindow: true });
        }
        Document.OpenDocumentForm = OpenDocumentForm;
        function concatDocIds(docIds) {
            return docIds.join();
        }
        function InitMergePage() {
            return __awaiter(this, void 0, void 0, function () {
                var data, params, keyValue, _a, _b, _c, _d;
                return __generator(this, function (_e) {
                    switch (_e.label) {
                        case 0:
                            data = window.location.search.split('=');
                            params = decodeURIComponent(data[1]);
                            keyValue = params.split('=');
                            inputParams = JSON.parse(keyValue[1]);
                            $('#docGrid').jqGrid({
                                datatype: 'local',
                                height: '400px',
                                autowidth: true,
                                rowNum: '100',
                                resizable: true,
                                multiselect: true,
                                colNames: ['Row ID', 'Name', 'Type', 'Date',],
                                colModel: [
                                    { name: 'ipg_documentid', index: 'ipg_documentid', key: true, hidden: true },
                                    {
                                        name: 'ipg_name', index: 'ipg_name', width: 150,
                                        formatter: docLinkFormatter
                                    },
                                    { name: 'ipg_documenttypename', index: 'ipg_documenttypename', width: 150 },
                                    {
                                        name: 'modifiedon', index: 'modifiedon', width: 75, formatter: dateTimeFormatter
                                    },
                                ],
                                loadComplete: function (data) {
                                    if (data.records === 1) {
                                        $(this).jqGrid('setSelection', data.rows[0].ipg_documentid);
                                    }
                                },
                                caption: 'Assemble Documents',
                                sortname: 'Name',
                                sortorder: "asc",
                                viewrecords: true
                            });
                            jQuery("#docGrid").jqGrid('sortableRows');
                            _b = (_a = $('#docGrid')).jqGrid;
                            _c = ['setGridParam'];
                            _d = {};
                            return [4 /*yield*/, GetDocs()];
                        case 1:
                            _b.apply(_a, _c.concat([(_d.data = (_e.sent()).entities, _d)])).trigger('reloadGrid');
                            FillDocumentTypes();
                            return [2 /*return*/];
                    }
                });
            });
        }
        Document.InitMergePage = InitMergePage;
        function GetDocs() {
            return parent.Xrm.WebApi.retrieveMultipleRecords('ipg_document', "?$select=ipg_documentid,ipg_name,modifiedon " +
                "&$expand=ipg_DocumentTypeId($select=ipg_name,ipg_documenttypeabbreviation) " +
                "&$filter=" +
                ("" + Intake.Utility.FormatODataLogicIn('ipg_documentid', inputParams.docIds)));
        }
        function Merge() {
            return __awaiter(this, void 0, void 0, function () {
                var docGridVar, data, globalContext, clientUrl, docIdsString, postObject;
                return __generator(this, function (_a) {
                    docGridVar = $('#docGrid');
                    data = docGridVar.jqGrid('getGridParam', 'data');
                    globalContext = parent.Xrm.Utility.getGlobalContext();
                    clientUrl = globalContext.getClientUrl();
                    docIdsString = concatDocIds(inputParams.docIds);
                    postObject = {
                        CaseId: {
                            '@data.type': 'mscrm.incident',
                            'incidentid': inputParams.caseId
                        },
                        PackageName: document.getElementById('assemblyNameInput').value,
                        DocIds: docIdsString,
                        TypeId: $("#docTypeSelect").val(),
                        Description: $("#mergedDocumentNameInput").val()
                    };
                    parent.Xrm.Utility.showProgressIndicator(null);
                    parent.$.ajax({
                        method: "POST",
                        url: clientUrl + '/api/data/v9.0/ipg_IPGIntakeActionsMergeDocuments',
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(postObject),
                        dataType: 'json',
                        success: function (response) {
                            parent.Xrm.Utility.closeProgressIndicator();
                            Close();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            parent.Xrm.Utility.closeProgressIndicator();
                            parent.Xrm.Navigation.openErrorDialog({ message: "Could not merge the documents" }); //todo: clear validation messages from server side
                        }
                    });
                    return [2 /*return*/];
                });
            });
        }
        Document.Merge = Merge;
        function Close() {
            window.close();
            parent.opener.Xrm.Page.getControl("Documents_Attached").refresh();
        }
        Document.Close = Close;
        function deactivateDocs(docIds) {
            var batchId = 'batch_123456';
            var changeSetId = 'changeset_BBB456';
            var data = [];
            data.push('--' + batchId);
            data.push('Content-Type: multipart/mixed;boundary=' + changeSetId);
            for (var i = 0; i < docIds.length; i++) {
                //add a request
                data.push('');
                data.push('--' + changeSetId);
                data.push('Content-Type:application/http');
                data.push('Content-Transfer-Encoding:binary');
                data.push('Content-ID:' + (i + 1));
                data.push('');
                data.push('PATCH ' + parent.Xrm.Page.context.getClientUrl() + '/api/data/v9.0/ipg_documents(' + docIds[i] + ') HTTP/1.1');
                data.push('Content-Type:application/json;type=entry');
                data.push('');
                data.push('{ "statecode":1 }');
            }
            //end of changeset
            data.push('--' + changeSetId + '--');
            //end of batch
            data.push('--' + batchId + '--');
            var payload = data.join('\r\n');
            parent.Xrm.Utility.showProgressIndicator(null);
            $.ajax({
                type: 'POST',
                url: parent.Xrm.Page.context.getClientUrl() + '/api/data/v9.0/$batch',
                headers: {
                    'Content-Type': 'multipart/mixed;boundary=' + batchId,
                    'Accept': 'application/json',
                    'Odata-MaxVersion': '4.0',
                    'Odata-Version': '4.0'
                },
                data: payload,
                async: false,
                success: function (data, textStatus, xhr) {
                    parent.Xrm.Utility.closeProgressIndicator();
                    parent.Xrm.Navigation.openAlertDialog({ text: 'The documents have been deactivated' });
                },
                error: function (xhr, data, textStatus, errorThrown) {
                    parent.Xrm.Utility.closeProgressIndicator();
                    parent.Xrm.Navigation.openErrorDialog({ message: textStatus + " " + errorThrown });
                }
            });
        }
        function GetPossibleTypesForMegeDocument() {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, parent.Xrm.WebApi.retrieveMultipleRecords("ipg_documenttype", "?$select=ipg_documenttypeid,ipg_name&$filter=ipg_packet eq true")];
                        case 1: return [2 /*return*/, (_a.sent()).entities.sort(function (firstEl, secondEl) { return firstEl.ipg_name > secondEl.ipg_name ? 1 : -1; })];
                    }
                });
            });
        }
        function FillDocumentTypes() {
            return __awaiter(this, void 0, void 0, function () {
                var types, documentTypesSelect;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, GetPossibleTypesForMegeDocument()];
                        case 1:
                            types = _a.sent();
                            documentTypesSelect = document.getElementById("docTypeSelect");
                            types.forEach(function (type) {
                                var newOption = document.createElement("option");
                                newOption.value = type.ipg_documenttypeid;
                                newOption.text = type.ipg_name;
                                documentTypesSelect.add(newOption);
                            });
                            return [2 /*return*/];
                    }
                });
            });
        }
        var inputParams;
    })(Document = Intake.Document || (Intake.Document = {}));
})(Intake || (Intake = {}));
