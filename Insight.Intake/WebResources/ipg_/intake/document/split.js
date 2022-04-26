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
 * @namespace Intake.Document.Split
 */
var Intake;
(function (Intake) {
    var Document;
    (function (Document) {
        var Split;
        (function (Split) {
            var DocumentCategoryTypeName = "Patient Procedure";
            var docId;
            function onSelectReferralOrCaseClick() {
                parent.Xrm.Utility.lookupObjects({
                    entityTypes: ["incident", "ipg_referral"],
                }).then(onReferralOrCaseSelected, function () {
                    parent.Xrm.Navigation.openErrorDialog({ message: "An error occured" });
                });
            }
            Split.onSelectReferralOrCaseClick = onSelectReferralOrCaseClick;
            function InitPage() {
                return __awaiter(this, void 0, void 0, function () {
                    var documentTypes;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                docId = getDocIdParameter();
                                if (!docId) {
                                    parent.Xrm.Navigation.openErrorDialog({
                                        message: "Document ID parameter is required",
                                    });
                                    return [2 /*return*/];
                                }
                                return [4 /*yield*/, populateDocumentInfo()];
                            case 1:
                                _a.sent();
                                documentTypes = [];
                                parent.Xrm.Utility.showProgressIndicator(null);
                                return [4 /*yield*/, parent.Xrm.WebApi.retrieveMultipleRecords("ipg_documenttype", "?$filter=ipg_DocumentCategoryTypeId/ipg_name eq '" + encodeURIComponent(DocumentCategoryTypeName) + "' and statecode eq 0&$orderby=ipg_name").then(function (result) {
                                        parent.Xrm.Utility.closeProgressIndicator();
                                        for (var i = 0; i < result.entities.length; i++) {
                                            documentTypes.push({
                                                value: result.entities[i].ipg_documenttypeid,
                                                text: result.entities[i].ipg_name,
                                            });
                                        }
                                        populateDocumentTypes(documentTypes);
                                    })];
                            case 2:
                                _a.sent();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Split.InitPage = InitPage;
            function onPreviewClick() {
                if (!docId) {
                    return;
                }
                Intake.Document.previewById(docId);
            }
            Split.onPreviewClick = onPreviewClick;
            function onProcessClick() {
                splitDocument(false, false);
            }
            Split.onProcessClick = onProcessClick;
            function onSplitAndInitiateClick() {
                splitDocument(false, true);
            }
            Split.onSplitAndInitiateClick = onSplitAndInitiateClick;
            function onCancelClick() {
                parent.Xrm.Utility.confirmDialog("Do you want to close this page?", function () { return window.close(); }, null);
            }
            Split.onCancelClick = onCancelClick;
            function populateDocumentInfo() {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        parent.Xrm.Utility.showProgressIndicator(null);
                        return [2 /*return*/, parent.Xrm.WebApi.retrieveRecord("ipg_document", docId, "?$select=ipg_documentid,ipg_name&$expand=ipg_ReferralId($select=ipg_referralid,ipg_name),ipg_CaseId($select=incidentid,title)").then(function (doc) {
                                parent.Xrm.Utility.closeProgressIndicator();
                                //populate the document title hyperlink
                                var docTitleHyperlink = (document.getElementById("documentTitleHyperlink"));
                                docTitleHyperlink.innerText = doc.ipg_name;
                                docTitleHyperlink.onclick = function () {
                                    parent.Xrm.Navigation.openForm({
                                        entityName: "ipg_document",
                                        entityId: doc.ipg_documentid,
                                        openInNewWindow: true,
                                    });
                                };
                                //populate the case control from the document
                                if (doc.ipg_CaseId) {
                                    selectReferralOrCase(null, doc.ipg_CaseId.incidentid, doc.ipg_CaseId.title);
                                }
                                else if (doc.ipg_ReferralId) {
                                    selectReferralOrCase(doc.ipg_ReferralId.ipg_referralid, null, doc.ipg_ReferralId.ipg_name);
                                }
                            })];
                    });
                });
            }
            function onReferralOrCaseSelected(selectedEntities) {
                if (selectedEntities && selectedEntities.length) {
                    if (selectedEntities[0].entityType == "ipg_referral") {
                        selectReferralOrCase(selectedEntities[0].id, null, selectedEntities[0].name);
                    }
                    else if (selectedEntities[0].entityType == "incident") {
                        selectReferralOrCase(null, selectedEntities[0].id, selectedEntities[0].name);
                    }
                }
            }
            function splitDocument(isContinue, initiateReferral) {
                return __awaiter(this, void 0, void 0, function () {
                    var pifCount, postObject, i, range, docTypeSelectItem, docTypeId, selectedOptionIndex, description, referralId, caseId, globalContext, clientUrl;
                    return __generator(this, function (_a) {
                        if (!docId) {
                            return [2 /*return*/];
                        }
                        pifCount = 0;
                        postObject = {};
                        for (i = 0; i < 10; i++) {
                            range = (document.getElementById("rangeInput" + i)).value.trim();
                            if (range) {
                                docTypeSelectItem = (document.getElementById("docTypeSelect" + i));
                                docTypeId = docTypeSelectItem.value;
                                selectedOptionIndex = docTypeSelectItem.selectedIndex;
                                if (docTypeSelectItem.options[selectedOptionIndex].innerHTML ===
                                    "Patient Information Form")
                                    pifCount++;
                                if (!docTypeId) {
                                    parent.Xrm.Navigation.openAlertDialog({
                                        text: "Please select a document type for this range: " + range,
                                    });
                                    return [2 /*return*/];
                                }
                                description = (document.getElementById("description" + i)).value.trim();
                                postObject["Range" + i] = range;
                                postObject["DocTypeId" + i] = {
                                    "@data.type": "mscrm.ipg_documenttype",
                                    ipg_documenttypeid: docTypeId,
                                };
                                postObject["Description" + i] = description;
                            }
                            else {
                                if (i == 0) {
                                    parent.Xrm.Navigation.openAlertDialog({
                                        text: "Please enter the range #1",
                                    });
                                    return [2 /*return*/];
                                }
                            }
                        }
                        referralId = (document.getElementById("referralIdInput")).value;
                        if (referralId) {
                            postObject["ReferralId"] = {
                                "@data.type": "Microsoft.Dynamics.CRM.ipg_referral",
                                ipg_referralid: referralId,
                            };
                        }
                        caseId = (document.getElementById("caseIdInput")).value;
                        if (caseId) {
                            postObject["CaseId"] = {
                                "@data.type": "Microsoft.Dynamics.CRM.incident",
                                incidentid: caseId,
                            };
                        }
                        globalContext = parent.Xrm.Utility.getGlobalContext();
                        clientUrl = globalContext.getClientUrl();
                        if (initiateReferral) {
                            if (pifCount > 1) {
                                parent.Xrm.Navigation.openAlertDialog({
                                    text: "Referral can be initiated only from one PIF Document.",
                                });
                                return [2 /*return*/];
                            }
                            else if (pifCount === 0) {
                                parent.Xrm.Navigation.openAlertDialog({
                                    text: "Referral can be initiated only from PIF Document.",
                                });
                                return [2 /*return*/];
                            }
                        }
                        parent.Xrm.Utility.showProgressIndicator(null);
                        parent.$.ajax({
                            method: "POST",
                            url: clientUrl +
                                "/api/data/v9.0/ipg_documents(" +
                                docId +
                                ")/Microsoft.Dynamics.CRM.ipg_IPGDocumentActionsSplit",
                            contentType: "application/json; charset=utf-8",
                            data: JSON.stringify(postObject),
                            dataType: "json",
                            success: function (response) {
                                return __awaiter(this, void 0, void 0, function () {
                                    return __generator(this, function (_a) {
                                        parent.Xrm.Utility.closeProgressIndicator();
                                        parent.Xrm.Navigation.openAlertDialog({
                                            text: "Document split has finished",
                                        });
                                        if (isContinue && response.NewDocumentId) {
                                            parent.Xrm.Navigation.openWebResource("ipg_/intake/document/split.html", {
                                                width: window.innerWidth,
                                                height: window.innerHeight,
                                                openInNewWindow: false,
                                            }, response.NewDocumentId);
                                            window.close();
                                        }
                                        else if (initiateReferral) {
                                            parent.Xrm.Utility.showProgressIndicator(null);
                                            parent.Xrm.WebApi.retrieveMultipleRecords("ipg_document", "?$select=ipg_documentid&$filter=ipg_OriginalDocumentId/ipg_documentid eq " +
                                                docId).then(function success(docIds) {
                                                var idsArray = docIds.entities.map(function (d) {
                                                    return d.ipg_documentid;
                                                });
                                                parent.Xrm.Utility.closeProgressIndicator();
                                                Intake.Core.Document.InitiateReferral(idsArray);
                                            }, function (error) {
                                                parent.Xrm.Utility.closeProgressIndicator();
                                                parent.Xrm.Navigation.openAlertDialog({
                                                    text: "Cannot retrieve documents.",
                                                });
                                                return;
                                            });
                                        }
                                        else {
                                            if (parent && parent.opener) {
                                                parent.opener.postMessage("REFRESH_DOCUMENTS_GRID");
                                            }
                                            window.close();
                                        }
                                        if (parent && parent.opener) {
                                            parent.opener.NavigateToDocumentsView();
                                        }
                                        return [2 /*return*/];
                                    });
                                });
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                parent.Xrm.Navigation.openErrorDialog({
                                    message: "Document split has failed",
                                }); //todo: clear validation messages from server side
                                parent.Xrm.Utility.closeProgressIndicator();
                            },
                        });
                        return [2 /*return*/];
                    });
                });
            }
            function populateDocumentTypes(documentTypes) {
                filterDocumentTypes(documentTypes);
                for (var i = 0; i < 10; i++) {
                    var selectElement = (document.getElementById("docTypeSelect" + i));
                    for (var j = 0; j < documentTypes.length; j++) {
                        var newOption = document.createElement("option");
                        newOption.value = documentTypes[j].value;
                        newOption.text = documentTypes[j].text;
                        selectElement.add(newOption);
                    }
                }
            }
            function filterDocumentTypes(documentTypes) {
                var moveElements = function (arr, fromIndex, toIndex) {
                    var element = arr[fromIndex];
                    arr.splice(fromIndex, 1);
                    arr.splice(toIndex, 0, element);
                };
                documentTypesOrder.reverse().forEach(function (orderType) {
                    var index = documentTypes.findIndex(function (docType) { return docType.text == orderType; });
                    if (index != -1)
                        moveElements(documentTypes, index, 0);
                });
            }
            function selectReferralOrCase(referralId, caseId, name) {
                var referralIdInput = (document.getElementById("referralIdInput"));
                referralIdInput.value = referralId;
                var caseIdInput = (document.getElementById("caseIdInput"));
                caseIdInput.value = caseId;
                var referralOrCaseNameInput = (document.getElementById("referralOrCaseNameInput"));
                referralOrCaseNameInput.value = name;
            }
            function getDocIdParameter() {
                var docId;
                var dataParam = Intake.Utility.GetDataParam();
                if (dataParam) {
                    var dataParamDecoded = decodeURIComponent(dataParam);
                    docId = Intake.Utility.removeCurlyBraces(dataParamDecoded);
                }
                if (!docId) {
                    parent.Xrm.Navigation.openErrorDialog({
                        message: "Error. Document ID parameter is required",
                    });
                    return null;
                }
                return docId;
            }
            var ValueAndText = /** @class */ (function () {
                function ValueAndText() {
                }
                return ValueAndText;
            }());
            var documentTypesOrder = [
                "Patient Information Form",
                "Implant Charge Sheet",
                "Manufacturer Invoice",
            ];
        })(Split = Document.Split || (Document.Split = {}));
    })(Document = Intake.Document || (Intake.Document = {}));
})(Intake || (Intake = {}));
