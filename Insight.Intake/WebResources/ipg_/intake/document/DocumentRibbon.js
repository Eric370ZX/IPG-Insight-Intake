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
        var ReviewStatuses;
        (function (ReviewStatuses) {
            ReviewStatuses[ReviewStatuses["Used"] = 427880006] = "Used";
        })(ReviewStatuses || (ReviewStatuses = {}));
        /**
         * Opens Document Split HTML web resource.
         * @function Intake.Document.OpenDocumentSplit
         * @returns {void}
         */
        function OpenDocumentSplit(primaryControl) {
            var firstSelectedItemId;
            if (primaryControl.getGrid) {
                var gridControl_1 = primaryControl;
                var grid = gridControl_1.getGrid();
                var selectedRows = grid.getSelectedRows();
                if (selectedRows && selectedRows.getLength()) {
                    firstSelectedItemId = selectedRows.getByIndex(0).data.entity.getId();
                }
                window.NavigateToDocumentsView = NavigateToDocumentsView;
                window.parent.addEventListener("message", function (event) {
                    if (event.data == "REFRESH_DOCUMENTS_GRID" && gridControl_1) {
                        gridControl_1.refresh();
                    }
                }, false);
            }
            else {
                var formContext_1 = primaryControl;
                firstSelectedItemId = formContext_1.data.entity.getId();
                window.parent.addEventListener("message", function (event) {
                    if (event.data == "REFRESH_DOCUMENTS_GRID") {
                        formContext_1.data.refresh(true);
                    }
                }, false);
            }
            if (!firstSelectedItemId) {
                alert("Document ID is required to split the document");
                return;
            }
            Xrm.WebApi.retrieveRecord("ipg_document", firstSelectedItemId, "?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)").then(function success(doc) {
                var myUrlWithParams = new URL(Xrm.Utility.getGlobalContext().getCurrentAppUrl());
                myUrlWithParams.searchParams.append("pagetype", "webresource");
                myUrlWithParams.searchParams.append("webresourceName", "ipg_/intake/document/split.html");
                myUrlWithParams.searchParams.append("cmdbar", "true");
                myUrlWithParams.searchParams.append("navbar", "off");
                myUrlWithParams.searchParams.append("data", firstSelectedItemId);
                myUrlWithParams.searchParams.append("newWindow", "true");
                window.open(myUrlWithParams.toString(), "_blank", "height=700,width=800");
            }, function (error) {
                alert("Could not retrieve ipg_document: " + error.message);
            });
        }
        Document.OpenDocumentSplit = OpenDocumentSplit;
        function NavigateToDocumentsView() {
            var pageInput = {
                entityName: "ipg_document",
                pageType: "entitylist"
            };
            var navigationOptions = {
                target: 1
            };
            Xrm.Navigation.navigateTo(pageInput, navigationOptions);
        }
        /**
         * enable rule for check selected document review status
         * @function Intake.Document.isEnableReviewStatus
         * @returns {boolean}
         */
        function isEnableReviewStatus(selectedRecordId) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var status;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.online.retrieveRecord("ipg_document", selectedRecordId.toLocaleLowerCase().replace("{", "").replace("}", ""), "?$select=ipg_reviewstatus")];
                        case 1:
                            status = (_a = (_b.sent())) === null || _a === void 0 ? void 0 : _a.ipg_reviewstatus;
                            return [2 /*return*/, status != ReviewStatuses.Used];
                    }
                });
            });
        }
        Document.isEnableReviewStatus = isEnableReviewStatus;
        /**
           * enable rule for Initiate Referral button
           * @function Intake.Document.hasNotMoreThanTwoReferrals
           * @returns {boolean}
           */
        function hasNotMoreThanTwoReferrals(firstPrimaryItemId) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var document, referrals;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            if (!firstPrimaryItemId.replace("{", "").replace("}", "")) return [3 /*break*/, 3];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", firstPrimaryItemId, "?$select=_ipg_referralid_value,ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation, ipg_name)")];
                        case 1:
                            document = _b.sent();
                            if (!(((_a = document.ipg_DocumentTypeId) === null || _a === void 0 ? void 0 : _a.ipg_documenttypeabbreviation) === 'PIF')) return [3 /*break*/, 3];
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_referral", "?$select=ipg_referralid&$filter=_ipg_sourcedocumentid_value eq " + Intake.Utility.removeCurlyBraces(document.ipg_documentid))];
                        case 2:
                            referrals = _b.sent();
                            if (referrals.entities.length > 1) {
                                return [2 /*return*/, false];
                            }
                            else {
                                return [2 /*return*/, true];
                            }
                            _b.label = 3;
                        case 3: return [2 /*return*/];
                    }
                });
            });
        }
        Document.hasNotMoreThanTwoReferrals = hasNotMoreThanTwoReferrals;
        /**
         * enable rule for buttons by document type's name
         * @function Intake.Document.isEnableByDocumentTypeName
         * @returns {boolean}
         */
        function isEnableByDocumentTypeName(primaryControl, documentTypeNames, enableIfNoDocType) {
            var formContext = primaryControl;
            var documentType = null;
            var referral = null;
            var fileExists = false;
            var documentTypeNamesArray = null;
            if (primaryControl.getGrid) {
                var grid = primaryControl.getGrid();
                var selectedRows = grid.getSelectedRows();
                if (selectedRows && selectedRows.getLength()) {
                    documentTypeNamesArray = selectedRows.getAll().map(function (d) {
                        return d.getAttribute("ipg_documenttypeid").getValue()[0].name;
                    });
                    if (documentTypeNamesArray.filter(function (d) {
                        return d === "Patient Information Form";
                    }).length > 1) {
                        return false;
                    }
                    fileExists = true;
                }
            }
            else {
                var entityType = formContext.data.entity.getEntityName();
                if (entityType == "ipg_document") {
                    var attribute = formContext.getAttribute("ipg_documenttypeid");
                    referral = formContext.getAttribute("ipg_referralid").getValue();
                    if (attribute && attribute.getIsDirty() == false) {
                        documentTypeNamesArray = attribute.getValue().map(function (a) {
                            return a.name;
                        });
                    }
                }
                else {
                    throw new Error("Unexpected entityType: " + entityType);
                }
                fileExists = !!formContext.data.entity.getId();
            }
            if (enableIfNoDocType &&
                fileExists &&
                (!documentTypeNamesArray || !documentTypeNamesArray.length)) {
                return true;
            }
            if (documentTypeNames == "Patient Information Form") {
                if ((!referral || !referral.length) &&
                    documentTypeNamesArray != null &&
                    documentTypeNamesArray.length &&
                    documentTypeNamesArray.indexOf(documentTypeNames) >= 0) {
                    return true;
                }
            }
            else {
                var checker = function (arr, target) {
                    return target.every(function (v) {
                        return arr.includes(v);
                    });
                };
                if (documentTypeNamesArray != null &&
                    documentTypeNamesArray.length &&
                    checker(documentTypeNames.split("|"), documentTypeNamesArray)) {
                    return true;
                }
            }
            return false;
        }
        Document.isEnableByDocumentTypeName = isEnableByDocumentTypeName;
        function isEnableByNumberOfPages(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var grid, selectedRows, documentId, document, numberOfPages;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!primaryControl.getGrid) return [3 /*break*/, 3];
                            grid = primaryControl.getGrid();
                            selectedRows = grid.getSelectedRows();
                            if (!(selectedRows && selectedRows.getLength())) return [3 /*break*/, 2];
                            documentId = selectedRows.get(0).getData().getEntity().getId();
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", documentId, "?$select=ipg_numberofpages")];
                        case 1:
                            document = _a.sent();
                            return [2 /*return*/, document.ipg_numberofpages > 1];
                        case 2: return [3 /*break*/, 4];
                        case 3:
                            numberOfPages = primaryControl
                                .getAttribute("ipg_numberofpages")
                                .getValue();
                            if (numberOfPages) {
                                return [2 /*return*/, numberOfPages > 1];
                            }
                            _a.label = 4;
                        case 4: return [2 /*return*/, false];
                    }
                });
            });
        }
        Document.isEnableByNumberOfPages = isEnableByNumberOfPages;
        /**
         * Open Document Preview HTML web resource.
         * @function Intake.Document.OpenDocumentPreview
         * @returns {void}
         */
        function OpenDocumentPreview(primaryControl, firstSelectedRecordId) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, currentDocId, doc, env, docAppEnvSuffix_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = primaryControl;
                            currentDocId = null;
                            if (firstSelectedRecordId) {
                                currentDocId = firstSelectedRecordId.replace("{", "").replace("}", "");
                            }
                            else {
                                currentDocId = formContext.data.entity.getId();
                            }
                            if (!currentDocId) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", currentDocId, "?$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)&$select=_ipg_ebvresponseid_value")];
                        case 1:
                            doc = _a.sent();
                            if (doc && doc.ipg_DocumentTypeId.ipg_documenttypeabbreviation == "EBV") {
                                //preview EBV doc
                                if (doc._ipg_ebvresponseid_value) {
                                    Xrm.Navigation.openWebResource("ipg_/intake/EBVResponse/ViewDetails.html", { width: 1000, height: 800, openInNewWindow: true }, doc._ipg_ebvresponseid_value);
                                }
                                else {
                                    Xrm.Navigation.openAlertDialog({ text: "EBV Response is not set" });
                                }
                                return [2 /*return*/];
                            }
                            env = void 0;
                            if (location.host.indexOf("-dev") >= 0) {
                                env = "dev";
                            }
                            else if (location.host.indexOf("-qa") >= 0) {
                                env = "qa";
                            }
                            else if (location.host.indexOf("-prd") >= 0) {
                                env = "prd";
                            }
                            else {
                                env = "";
                            }
                            if (env) {
                                docAppEnvSuffix_1 = "-" + env;
                            }
                            else {
                                docAppEnvSuffix_1 = "";
                            }
                            Xrm.WebApi.retrieveMultipleRecords("annotation", "?$filter=_objectid_value eq '" + currentDocId + "'").then(function (result) {
                                var height = 600;
                                var width = 800;
                                if (result && result.entities.length) {
                                    var annotationId = result.entities[0].annotationid;
                                    Xrm.Navigation.openUrl("https://insight-documents" + docAppEnvSuffix_1 + ".azurewebsites.net/documents/" + annotationId, { height: height, width: width });
                                }
                                else {
                                    Xrm.Navigation.openUrl("https://insight-documents" + docAppEnvSuffix_1 + ".azurewebsites.net/legacydocuments/" + currentDocId, { height: height, width: width });
                                }
                            }, function (error) {
                                Xrm.Navigation.openErrorDialog({ message: error.message });
                            });
                            _a.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            });
        }
        Document.OpenDocumentPreview = OpenDocumentPreview;
        /**
         * Opens "Merge Documents" HTML web resource.
         * @function Intake.Document.OpenDocumentMerge
         * @returns {void}
         */
        function OpenDocumentMerge(primaryControl, selectedRecordIds) {
            return __awaiter(this, void 0, void 0, function () {
                var incidentId, inputParams;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            incidentId = primaryControl.data.entity.getId();
                            incidentId = incidentId.substring(1, incidentId.length - 1);
                            inputParams = {
                                caseId: incidentId,
                                docIds: selectedRecordIds
                            };
                            window.parentControl = primaryControl;
                            return [4 /*yield*/, CheckPPPAvailability(incidentId)];
                        case 1:
                            if (_a.sent()) {
                                Xrm.Navigation.openWebResource("ipg_/intake/document/merge.html", { width: 800, height: 670, openInNewWindow: true }, "params=" + JSON.stringify(inputParams));
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        Document.OpenDocumentMerge = OpenDocumentMerge;
        /**
         * Open lookup form with "FacilityDocument" view by default
         * @function Intake.Document.AddExistingFacilityDocuments
         * @returns {void}
         */
        function AddExistingFacilityDocuments(primaryControl) {
            var formContext = primaryControl;
            var options = {
                allowMultiSelect: false,
                defaultEntityType: "ipg_document",
                entityTypes: ["ipg_document"],
                defaultViewId: "98485AE2-3E8C-E911-A97E-000D3A37043B",
                disableMru: true,
            };
            Xrm.Utility.lookupObjects(options).then(function (documnet) {
                if (documnet && documnet.length) {
                    var account = formContext.data.entity.getEntityReference();
                    associateRecords("ipg_ipg_document_account", { EntityListName: "accounts", Id: account.id }, { EntityListName: "ipg_documents", Id: documnet[0].id }).then(function (response) {
                        formContext.data.refresh(true);
                    });
                }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ message: error.message });
            });
        }
        Document.AddExistingFacilityDocuments = AddExistingFacilityDocuments;
        /**
         * enable rule for "Add Facility Document" button
         * @function Intake.Document.addFacilityDocumentIsVisible
         * @returns {boolean}
         */
        function addFacilityDocumentIsVisible(primaryControl) {
            var formContext = primaryControl;
            var accountType = null;
            if (formContext.data.entity.getEntityName() == "account") {
                accountType = formContext.getAttribute("customertypecode").getValue();
                if (accountType && accountType === 923720000) {
                    //facility
                    return true;
                }
            }
            return false;
        }
        Document.addFacilityDocumentIsVisible = addFacilityDocumentIsVisible;
        /**
         * call Web Api for creating a relationship between entities
         * @function Intake.Document.AssociateRecords
         * @returns {Intake.Utility.HttpRequest}
         */
        function associateRecords(relationshipName, targetEntity, sourceEntity) {
            var globalContext = Xrm.Utility.getGlobalContext();
            var clientUrl = globalContext.getClientUrl();
            var requestOptions = {
                path: clientUrl + "/api/data/v9.0/" + targetEntity.EntityListName + "(" + Intake.Utility.removeCurlyBraces(targetEntity.Id) + ")/" + relationshipName + "/$ref",
                body: {
                    "@odata.id": clientUrl +
                        ("/api/data/v9.0/" + sourceEntity.EntityListName + "(" + Intake.Utility.removeCurlyBraces(sourceEntity.Id) + ")"),
                },
                headers: {
                    "OData-MaxVersion": "4.0",
                    "OData-Version": "4.0",
                    Accept: "application/json",
                    "Content-Type": "application/json",
                },
            };
            return Intake.Utility.HttpRequest.post(requestOptions)
                .then(function (response) { return response; })
                .catch(function (response) {
                return Xrm.Navigation.openErrorDialog({
                    message: response.error.message,
                }).then(function () { return null; });
            });
        }
        /**
         * call Web Api for creating a relationship between entities
         * @function Intake.Document.AssociateRecords
         * @returns {Intake.Utility.HttpRequest}
         */
        function disassociateRecords(relationshipName, targetEntity, sourceEntity) {
            var globalContext = Xrm.Utility.getGlobalContext();
            var clientUrl = globalContext.getClientUrl();
            var requestOptions = {
                path: clientUrl + "/api/data/v9.0/" + targetEntity.EntityListName + "(" + Intake.Utility.removeCurlyBraces(targetEntity.Id) + ")/" + relationshipName + "(" + Intake.Utility.removeCurlyBraces(sourceEntity.Id) + ")/$ref",
                headers: {
                    "OData-MaxVersion": "4.0",
                    "OData-Version": "4.0",
                    Accept: "application/json",
                    "Content-Type": "application/json",
                },
            };
            return Intake.Utility.HttpRequest.delete(requestOptions)
                .then(function (response) { return response; })
                .catch(function (response) {
                return Xrm.Navigation.openErrorDialog({
                    message: response.error.message,
                }).then(function () { return null; });
            });
        }
        /**
         * Associate case part detail and document
         * @function Intake.Document.AssociateDocumentToCasePartDetail
         * @returns {Intake.Utility.HttpRequest}
         */
        function associateDocumentToCasePartDetail(primaryControl, selectedItemsReferences) {
            var formContext = primaryControl;
            var orderProductsControl = formContext.getControl("PurchaseOrderDetails");
            if (orderProductsControl) {
                if (orderProductsControl.getGrid().getSelectedRows().getLength()) {
                    var selectedOrderProduct = orderProductsControl
                        .getGrid()
                        .getSelectedRows()
                        .getByIndex(0);
                    var product_1 = selectedOrderProduct
                        .getAttribute("productid")
                        .getValue();
                    var caseRef_1 = formContext
                        .getAttribute("ipg_caseid")
                        .getValue();
                    if (product_1 && product_1.length && caseRef_1 && caseRef_1.length) {
                        Xrm.Utility.showProgressIndicator("Please wait");
                        CallCloneDocumentAction(selectedItemsReferences[0].Id).then(function (result1) {
                            if (result1.ipg_documentid) {
                                var associateInvoiceToCase = associateRecords("ipg_incident_ipg_document_CaseId", {
                                    EntityListName: "incidents",
                                    Id: caseRef_1[0].id.replace("{", "").replace("}", ""),
                                }, {
                                    EntityListName: "ipg_documents",
                                    Id: result1.ipg_documentid,
                                });
                                var associateInvoiceToCasePartDetail = Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=ipg_productid/productid eq " +
                                    product_1[0].id +
                                    " and ipg_caseid/incidentid eq " +
                                    caseRef_1[0].id).then(function (result) {
                                    if (result && result.entities.length) {
                                        associateRecords("ipg_ipg_casepartdetail_ipg_document", {
                                            EntityListName: "ipg_casepartdetails",
                                            Id: result.entities[0].ipg_casepartdetailid,
                                        }, {
                                            EntityListName: "ipg_documents",
                                            Id: result1.ipg_documentid,
                                        });
                                    }
                                });
                                Promise.all([
                                    associateInvoiceToCase,
                                    associateInvoiceToCasePartDetail,
                                ])
                                    .then(function (results) {
                                    Xrm.Utility.alertDialog("The invoice was associated to the Case Part Detail.", function () {
                                        formContext
                                            .getControl("PurchaseOrderDetails")
                                            .refresh();
                                        Xrm.Utility.closeProgressIndicator();
                                    });
                                })
                                    .catch(function (error) {
                                    Xrm.Utility.alertDialog(error.Message, null);
                                    Xrm.Utility.closeProgressIndicator();
                                });
                            }
                        });
                    }
                }
            }
        }
        Document.associateDocumentToCasePartDetail = associateDocumentToCasePartDetail;
        function CallCloneDocumentAction(entityId) {
            if (entityId) {
                var request = {
                    Record: {
                        "@odata.type": "Microsoft.Dynamics.CRM.ipg_document",
                        ipg_documentid: entityId,
                    },
                    getMetadata: function () {
                        return {
                            boundParamter: null,
                            parameterTypes: {
                                Record: {
                                    typeName: "mscrm.ipg_document",
                                    structuralProperty: 5,
                                },
                            },
                            operationType: 0,
                            operationName: "ipg_IPGIntakeActionsCloneRecord",
                        };
                    },
                };
                return Xrm.WebApi.online.execute(request).then(function (response) {
                    Xrm.Utility.closeProgressIndicator();
                    if (response.ok) {
                        return response.json();
                    }
                    else {
                        Xrm.Navigation.openErrorDialog({ message: response.statusText });
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
        /**
         * Action to reassign a document from one case to another
         * @function Intake.Document.executeReassingAction
         */
        function executeReassingAction(incidentId, documentId) {
            return __awaiter(this, void 0, void 0, function () {
                var request;
                return __generator(this, function (_a) {
                    if (incidentId && documentId) {
                        request = {
                            entity: {
                                id: documentId,
                                entityType: "ipg_document",
                            },
                            CaseRef: {
                                id: incidentId,
                                entityType: "incident",
                            },
                            getMetadata: function () {
                                return {
                                    boundParameter: "entity",
                                    parameterTypes: {
                                        entity: {
                                            typeName: "mscrm.ipg_document",
                                            structuralProperty: 5,
                                        },
                                        CaseRef: {
                                            typeName: "mscrm.incident",
                                            structuralProperty: 5,
                                        },
                                    },
                                    operationType: 0,
                                    operationName: "ipg_IPGDocumentActionsReassignToCase",
                                };
                            },
                        };
                        return [2 /*return*/, Xrm.WebApi.online.execute(request)];
                    }
                    return [2 /*return*/];
                });
            });
        }
        Document.executeReassingAction = executeReassingAction;
        /**
         * enable rule for "Associate To Actual Part" button
         * @function Intake.Document.associateToActualPartIsVisible
         * @returns {boolean}
         */
        function associateToActualPartIsVisible(selectedControl) {
            if (selectedControl.name == "Invoices") {
                return true;
            }
            return false;
        }
        Document.associateToActualPartIsVisible = associateToActualPartIsVisible;
        function CheckPPPAvailability(caseId) {
            return __awaiter(this, void 0, void 0, function () {
                var documents, e_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            _a.trys.push([0, 2, , 3]);
                            return [4 /*yield*/, parent.Xrm.WebApi.retrieveMultipleRecords("ipg_document", "?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)&$filter=_ipg_caseid_value eq " + Intake.Utility.removeCurlyBraces(caseId) + " and ipg_DocumentTypeId/ipg_documenttypeid ne null and statecode eq 0")];
                        case 1:
                            documents = _a.sent();
                            if (!documents || documents.entities.length == 0) {
                                parent.Xrm.Navigation.openErrorDialog({
                                    message: "Case does not have documents!",
                                });
                                return [2 /*return*/, false];
                            }
                            return [3 /*break*/, 3];
                        case 2:
                            e_1 = _a.sent();
                            parent.Xrm.Navigation.openErrorDialog(e_1);
                            return [2 /*return*/, false];
                        case 3: return [2 /*return*/, true];
                    }
                });
            });
        }
        Document.CheckPPPAvailability = CheckPPPAvailability;
        Document.ChargeSheet = "ICS";
        Document.Invoice = "MFG INV";
        function GetCpaPOByCase(caseId) {
            return __awaiter(this, void 0, void 0, function () {
                var cpaTypeCode, result;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            cpaTypeCode = 923720002;
                            return [4 /*yield*/, parent.Xrm.WebApi.retrieveMultipleRecords("salesorder", "?$top=1&$filter=_ipg_caseid_value eq " + Intake.Utility.removeCurlyBraces(caseId) + " and ipg_potypecode eq " + cpaTypeCode + "  and statecode eq 0")];
                        case 1:
                            result = _a.sent();
                            if (result && result.entities.length > 0) {
                                return [2 /*return*/, true];
                            }
                            else {
                                return [2 /*return*/, false];
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        Document.GetCpaPOByCase = GetCpaPOByCase;
        /**
         * command for "Reassign another Case" button
         * @function Intake.Document.MoveDocumentToAnotherCase
         * @returns {boolean}
         */
        function MoveDocumentToAnotherCase(primaryControl, selectedItemIds, selectedControl) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var formContext, options, incident, i, doc, reassignedIncidentId, result, Case_1, ownerType, data, e_2, alertStrings, alertOptions, e_3;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            formContext = primaryControl;
                            options = {
                                allowMultiSelect: false,
                                defaultEntityType: "incident",
                                entityTypes: ["incident"],
                                disableMru: true,
                            };
                            _b.label = 1;
                        case 1:
                            _b.trys.push([1, 14, , 15]);
                            return [4 /*yield*/, Xrm.Utility.lookupObjects(options)];
                        case 2:
                            incident = _b.sent();
                            if (!(incident && incident.length)) return [3 /*break*/, 13];
                            Xrm.Utility.showProgressIndicator("Reassigning a document");
                            i = 0;
                            _b.label = 3;
                        case 3:
                            if (!(i < selectedItemIds.length)) return [3 /*break*/, 12];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", selectedItemIds[i], "?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)")];
                        case 4:
                            doc = _b.sent();
                            if (!(!doc.ipg_DocumentTypeId ||
                                doc.ipg_DocumentTypeId.ipg_documenttypeabbreviation !== "PIF")) return [3 /*break*/, 10];
                            _b.label = 5;
                        case 5:
                            _b.trys.push([5, 8, , 9]);
                            reassignedIncidentId = incident[0].id;
                            return [4 /*yield*/, Intake.Document.executeReassingAction(reassignedIncidentId, doc.ipg_documentid)];
                        case 6:
                            result = _b.sent();
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", reassignedIncidentId, "?$select=_ownerid_value")];
                        case 7:
                            Case_1 = _b.sent();
                            ownerType = Case_1["_ownerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                            data = {
                                "ownerid@odata.bind": "/" + ownerType + "s(" + Case_1["_ownerid_value"] + ")",
                            };
                            Xrm.WebApi.updateRecord("ipg_document", doc.ipg_documentid, data).then(function success(result) { }, function (error) {
                                console.log(error.message);
                            });
                            selectedControl.refresh();
                            (_a = formContext === null || formContext === void 0 ? void 0 : formContext.getControl("CommonDocuments")) === null || _a === void 0 ? void 0 : _a.refresh();
                            return [3 /*break*/, 9];
                        case 8:
                            e_2 = _b.sent();
                            Xrm.Navigation.openErrorDialog({
                                message: e_2.Message ||
                                    e_2.message ||
                                    "Error during reassinging a document",
                            });
                            return [3 /*break*/, 9];
                        case 9: return [3 /*break*/, 11];
                        case 10:
                            alertStrings = {
                                confirmButtonLabel: "Ok",
                                text: "Document type must not be PIF document. Skipped",
                            };
                            alertOptions = {
                                height: 120,
                                width: 260,
                            };
                            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                            _b.label = 11;
                        case 11:
                            i++;
                            return [3 /*break*/, 3];
                        case 12:
                            Xrm.Utility.closeProgressIndicator();
                            _b.label = 13;
                        case 13: return [3 /*break*/, 15];
                        case 14:
                            e_3 = _b.sent();
                            Xrm.Navigation.openErrorDialog({
                                message: e_3.Message || e_3.message || "Error during reassinging a document",
                            });
                            return [3 /*break*/, 15];
                        case 15: return [2 /*return*/];
                    }
                });
            });
        }
        Document.MoveDocumentToAnotherCase = MoveDocumentToAnotherCase;
        /**
         * command for "RemoveDocument" button
         * @function Intake.Document.MoveDocumentToAnotherCase
         * @returns {boolean}
         */
        function RemoveDocument(primaryControl, selectedItemIds, selectedControl) {
            var formContext = primaryControl;
            var options = {
                allowMultiSelect: false,
                defaultEntityType: "incident",
                entityTypes: ["incident"],
                disableMru: true,
            };
            try {
                var _loop_1 = function (i) {
                    Xrm.WebApi.retrieveRecord("ipg_document", selectedItemIds[i], "?$select=ipg_documentid,_ipg_caseid_value&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)").then(function success(doc) {
                        if (!doc.ipg_DocumentTypeId ||
                            !doc._ipg_caseid_value ||
                            doc.ipg_DocumentTypeId.ipg_documenttypeabbreviation !== "PIF") {
                            disassociateRecords("ipg_incident_ipg_document_CaseId", {
                                EntityListName: "incidents",
                                Id: doc._ipg_caseid_value.replace("{", "").replace("}", ""),
                            }, {
                                EntityListName: "ipg_documents",
                                Id: selectedItemIds[i].replace("{", "").replace("}", ""),
                            }).then(function (response) { });
                        }
                        else {
                            alert("Document type must not be PIF document. Skipped");
                        }
                    }, function (error) {
                        console.log("Could not retrieve ipg_document: " + error.message);
                    });
                };
                for (var i = 0; i < selectedItemIds.length; i++) {
                    _loop_1(i);
                }
                //wait before grid refresh
                setTimeout(function () {
                    selectedControl.refresh();
                }, 1000);
            }
            catch (error) {
                Xrm.Navigation.openErrorDialog({ message: error.message });
            }
        }
        Document.RemoveDocument = RemoveDocument;
        /**
         * enable rule for "Reassign another Case" button
         * @function Intake.Document.IsCaseDocumentGrid
         * @returns {boolean}
         */
        function IsCaseDocumentGrid(primaryControl, selectedControl) {
            if (selectedControl.getName() === "Documents_Attached") {
                return true;
            }
            return false;
        }
        Document.IsCaseDocumentGrid = IsCaseDocumentGrid;
        /**
         * enable rule for not displaying button on Patient Statements subgrid
         * @function Intake.Document.IsStatementDocumentGrid
         * @returns {boolean}
         */
        function IsStatementDocumentGrid(control) {
            var cpntrolName = control.getName();
            return cpntrolName === "PatientStatements" || cpntrolName == "StatementDocuments";
        }
        Document.IsStatementDocumentGrid = IsStatementDocumentGrid;
        /**
         * Opens "EBV Response" form. This function can be called for EBV documents from Grid or Form.
         * @function Intake.Document.OpenDocumentMerge
         * @returns {void}
         */
        function OpenEbvResponse(selectedControl) {
            return __awaiter(this, void 0, void 0, function () {
                var firstSelectedItemId, gridControl, grid, selectedRows, formContext;
                return __generator(this, function (_a) {
                    if (selectedControl.getGrid) {
                        gridControl = selectedControl;
                        grid = gridControl.getGrid();
                        selectedRows = grid.getSelectedRows();
                        if (selectedRows && selectedRows.getLength()) {
                            firstSelectedItemId = selectedRows.getByIndex(0).data.entity.getId();
                        }
                    }
                    else {
                        formContext = selectedControl;
                        firstSelectedItemId = formContext.data.entity.getId();
                    }
                    if (!firstSelectedItemId) {
                        alert("Document ID is required to open EBV Response details");
                        return [2 /*return*/];
                    }
                    Xrm.WebApi.retrieveRecord("ipg_document", firstSelectedItemId).then(function (doc) {
                        if (doc._ipg_ebvresponseid_value) {
                            Xrm.Navigation.openForm({
                                entityName: "ipg_ebvresponse",
                                entityId: doc._ipg_ebvresponseid_value,
                                openInNewWindow: true,
                            });
                        }
                        else {
                            Xrm.Utility.alertDialog("No EBV Response", null);
                        }
                    }, function (error) {
                        console.log(error);
                        Xrm.Utility.alertDialog("Could not retrieve Document", null);
                    });
                    return [2 /*return*/];
                });
            });
        }
        Document.OpenEbvResponse = OpenEbvResponse;
        /**
         * Open a new document form
         * @function Intake.Document.NewDocument
         * @returns {void}
         */
        function NewDocument() {
            Xrm.WebApi.retrieveMultipleRecords("ipg_documenttype", "?$select=ipg_documenttypeid,ipg_name&$filter=ipg_name eq 'User Uploaded Generic Document'").then(function success(results) {
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_document";
                var formParameters = {};
                if (results.entities.length) {
                    formParameters["ipg_documenttypeid"] =
                        results.entities[0]["ipg_documenttypeid"];
                    formParameters["ipg_documenttypeidname"] =
                        results.entities[0]["ipg_name"];
                    formParameters["ipg_documenttypeidtype"] = "ipg_documenttype";
                    //formParameters["ipg_filterdocumenttypes"] = false;
                }
                Xrm.Navigation.openForm(entityFormOptions, formParameters);
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ message: error.message });
            });
        }
        Document.NewDocument = NewDocument;
        /**
         * Called on 'AddDocumentFromAnotherCase' button click
         * @function Intake.Document.AddDocumentFromAnotherCase
         * @returns {void}
         */
        function AddDocumentFromAnotherCase(formContext) {
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "ipg_documentrecordcopy";
            entityFormOptions["useQuickCreateForm"] = true;
            entityFormOptions["formId"] = "f5f24590-0c2e-4c51-9ee4-dba509aebc46";
            var formParameters = {};
            formParameters["ipg_caseidentityType"] = "incident";
            Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
                console.log(success);
                var documentCopyRecordId = null;
                documentCopyRecordId = Intake.Utility.removeCurlyBraces(success.savedEntityReference[0].id);
                Xrm.WebApi.retrieveRecord("ipg_documentrecordcopy", documentCopyRecordId, "?$select=_ipg_caseid_value,_ipg_documentid_value").then(function success(result) {
                    var _ipg_caseid_value = result["_ipg_caseid_value"];
                    var _ipg_documentid_value = result["_ipg_documentid_value"];
                    var association = {
                        "@odata.id": formContext.context.getClientUrl() +
                            ("/api/data/v9.1/ipg_documents(" + _ipg_documentid_value + ")"),
                    };
                    var req = new XMLHttpRequest();
                    req.open("POST", formContext.context.getClientUrl() +
                        ("/api/data/v9.1/incidents(" + _ipg_caseid_value + ")/ipg_incident_ipg_document/$ref"), true);
                    req.setRequestHeader("Accept", "application/json");
                    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                    req.setRequestHeader("OData-MaxVersion", "4.0");
                    req.setRequestHeader("OData-Version", "4.0");
                    req.onreadystatechange = function () {
                        if (this.readyState === 4) {
                            req.onreadystatechange = null;
                            if (this.status === 204 || this.status === 1223) {
                                Xrm.Page.getControl("CommonDocuments").refresh();
                            }
                            // else
                            //   {
                            //      Xrm.Utility.alertDialog(this.statusText, null);
                            //    }
                        }
                    };
                    req.send(JSON.stringify(association));
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message, null);
                });
            }, function (error) {
                console.log(error);
            });
        }
        Document.AddDocumentFromAnotherCase = AddDocumentFromAnotherCase;
        /**
         * enable rule for "Reassign another Referral" button
         * @function Intake.Document.IsReferralDocumentGrid
         * @returns {boolean}
         */
        function IsReferralDocumentGrid(primaryControl, selectedControl) {
            if (selectedControl.getName() === "sgDocumentsForReferral") {
                return true;
            }
            return false;
        }
        Document.IsReferralDocumentGrid = IsReferralDocumentGrid;
        function IsReferral(primaryControl) {
            var formContext = primaryControl;
            var referral = formContext
                .getAttribute("ipg_referralid")
                .getValue();
            if (referral === null) {
                return false;
            }
            return true;
        }
        Document.IsReferral = IsReferral;
        function IsCase(primaryControl) {
            var formContext = primaryControl;
            var Case = formContext
                .getAttribute("ipg_caseid")
                .getValue();
            if (Case === null) {
                return false;
            }
            return true;
        }
        Document.IsCase = IsCase;
        /**
         * command for "Reassign another Case" button
         * @function Intake.Document.MoveDocumentToAnotherReferral
         * @returns {boolean}
         */
        function MoveDocumentToAnotherReferral(primaryControl, selectedItemIds, selectedControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, options, referral, i, doc, result, e_4, alertStrings, alertOptions, e_5;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = primaryControl;
                            options = {
                                allowMultiSelect: false,
                                defaultEntityType: "ipg_referral",
                                entityTypes: ["ipg_referral"],
                                disableMru: true,
                            };
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 13, , 14]);
                            return [4 /*yield*/, Xrm.Utility.lookupObjects(options)];
                        case 2:
                            referral = _a.sent();
                            if (!(referral && referral.length)) return [3 /*break*/, 12];
                            Xrm.Utility.showProgressIndicator("Reassigning a document");
                            i = 0;
                            _a.label = 3;
                        case 3:
                            if (!(i < selectedItemIds.length)) return [3 /*break*/, 11];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", selectedItemIds[i], "?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)")];
                        case 4:
                            doc = _a.sent();
                            if (!(!doc.ipg_DocumentTypeId ||
                                doc.ipg_DocumentTypeId.ipg_documenttypeabbreviation !== "PIF")) return [3 /*break*/, 9];
                            _a.label = 5;
                        case 5:
                            _a.trys.push([5, 7, , 8]);
                            return [4 /*yield*/, Intake.Document.executeReassingToReferralAction(referral[0].id, doc.ipg_documentid)];
                        case 6:
                            result = _a.sent();
                            selectedControl.refresh();
                            return [3 /*break*/, 8];
                        case 7:
                            e_4 = _a.sent();
                            Xrm.Navigation.openErrorDialog({
                                message: e_4.Message ||
                                    e_4.message ||
                                    "Error during reassinging a document",
                            });
                            return [3 /*break*/, 8];
                        case 8: return [3 /*break*/, 10];
                        case 9:
                            alertStrings = {
                                confirmButtonLabel: "Ok",
                                text: "Document type must not be PIF document. Skipped",
                            };
                            alertOptions = {
                                height: 120,
                                width: 260,
                            };
                            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                            _a.label = 10;
                        case 10:
                            i++;
                            return [3 /*break*/, 3];
                        case 11:
                            Xrm.Utility.closeProgressIndicator();
                            _a.label = 12;
                        case 12: return [3 /*break*/, 14];
                        case 13:
                            e_5 = _a.sent();
                            Xrm.Navigation.openErrorDialog({
                                message: e_5.Message || e_5.message || "Error during reassinging a document",
                            });
                            return [3 /*break*/, 14];
                        case 14: return [2 /*return*/];
                    }
                });
            });
        }
        Document.MoveDocumentToAnotherReferral = MoveDocumentToAnotherReferral;
        /**
         * Action to reassign a document from one case to another
         * @function Intake.Document.executeReassingAction
         */
        function executeReassingToReferralAction(incidentId, documentId) {
            return __awaiter(this, void 0, void 0, function () {
                var request;
                return __generator(this, function (_a) {
                    if (incidentId && documentId) {
                        request = {
                            entity: {
                                id: documentId,
                                entityType: "ipg_document",
                            },
                            ReferralRef: {
                                id: incidentId,
                                entityType: "ipg_referral",
                            },
                            getMetadata: function () {
                                return {
                                    boundParameter: "entity",
                                    parameterTypes: {
                                        entity: {
                                            typeName: "mscrm.ipg_document",
                                            structuralProperty: 5,
                                        },
                                        ReferralRef: {
                                            typeName: "mscrm.ipg_referral",
                                            structuralProperty: 5,
                                        },
                                    },
                                    operationType: 0,
                                    operationName: "ipg_IPGDocumentActionsReassignToReferral",
                                };
                            },
                        };
                        return [2 /*return*/, Xrm.WebApi.online.execute(request)];
                    }
                    return [2 /*return*/];
                });
            });
        }
        Document.executeReassingToReferralAction = executeReassingToReferralAction;
        /**
         * command for "Reassign another Case" button
         * @function Intake.Document.ReassignDocumentToAnotherCase
         * @returns {boolean}
         */
        function ReassignDocumentToAnotherCase(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, options, incident, result, reassignedIncidentId, res, Case_2, data, e_6, alertStrings, alertOptions, e_7;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = primaryControl;
                            options = {
                                allowMultiSelect: false,
                                defaultEntityType: "incident",
                                entityTypes: ["incident"],
                                disableMru: true,
                            };
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 12, , 13]);
                            return [4 /*yield*/, Xrm.Utility.lookupObjects(options)];
                        case 2:
                            incident = _a.sent();
                            if (!(incident && incident.length)) return [3 /*break*/, 11];
                            Xrm.Utility.showProgressIndicator("Reassigning a document");
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", formContext.data.entity.getId(), "?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)")];
                        case 3:
                            result = _a.sent();
                            if (!(!result.ipg_DocumentTypeId ||
                                result.ipg_DocumentTypeId.ipg_documenttypeabbreviation !== "PIF")) return [3 /*break*/, 9];
                            _a.label = 4;
                        case 4:
                            _a.trys.push([4, 7, , 8]);
                            reassignedIncidentId = incident[0].id;
                            return [4 /*yield*/, Intake.Document.executeReassingAction(reassignedIncidentId, result.ipg_documentid)];
                        case 5:
                            res = _a.sent();
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("incident", reassignedIncidentId, "?$select=_ownerid_value")];
                        case 6:
                            Case_2 = _a.sent();
                            data = {
                                "ownerid@odata.bind": "/systemusers(" + Case_2["_ownerid_value"] + ")",
                            };
                            Xrm.WebApi.updateRecord("ipg_document", result.ipg_documentid, data).then(function success(result) {
                                formContext.data.refresh(true);
                            }, function (error) {
                                console.log(error.message);
                            });
                            return [3 /*break*/, 8];
                        case 7:
                            e_6 = _a.sent();
                            Xrm.Navigation.openErrorDialog({
                                message: e_6.Message || e_6.message || "Error during reassinging a document",
                            });
                            return [3 /*break*/, 8];
                        case 8: return [3 /*break*/, 10];
                        case 9:
                            alertStrings = {
                                confirmButtonLabel: "Ok",
                                text: "Document type must not be PIF document. Skipped",
                            };
                            alertOptions = {
                                height: 120,
                                width: 260,
                            };
                            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                            _a.label = 10;
                        case 10:
                            Xrm.Utility.closeProgressIndicator();
                            _a.label = 11;
                        case 11: return [3 /*break*/, 13];
                        case 12:
                            e_7 = _a.sent();
                            Xrm.Navigation.openErrorDialog({
                                message: e_7.Message || e_7.message || "Error during reassinging a document",
                            });
                            return [3 /*break*/, 13];
                        case 13: return [2 /*return*/];
                    }
                });
            });
        }
        Document.ReassignDocumentToAnotherCase = ReassignDocumentToAnotherCase;
        function IsAllDocumentsActive(selectedDocumentsIds) {
            return __awaiter(this, void 0, void 0, function () {
                var fetchXML, docs;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!(selectedDocumentsIds && selectedDocumentsIds.length > 0)) return [3 /*break*/, 2];
                            fetchXML = generateFetchXmlToRetrieveDocumentsByIds(selectedDocumentsIds);
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_document", fetchXML)];
                        case 1:
                            docs = _a.sent();
                            if (docs && docs.entities.length > 0) {
                                return [2 /*return*/, docs.entities.every(function (d) {
                                        return d.statecode === 0;
                                    })];
                            }
                            _a.label = 2;
                        case 2: return [2 /*return*/, false];
                    }
                });
            });
        }
        Document.IsAllDocumentsActive = IsAllDocumentsActive;
        function generateFetchXmlToRetrieveDocumentsByIds(documentIds) {
            var filterValues = "";
            documentIds.forEach(function (id) {
                filterValues += "\n<value>" + id + "</value>";
            });
            var fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">\n                  <entity name=\"ipg_document\">\n                    <attribute name=\"ipg_documentid\" />\n                    <attribute name=\"ipg_name\" />\n                    <attribute name=\"statecode\" />\n                    <attribute name=\"ipg_caseid\" />\n                    <attribute name=\"ipg_referralid\" />\n                    <filter type=\"and\">\n                      <condition attribute=\"ipg_documentid\" operator=\"in\">" +
                filterValues +
                "\n                      </condition>\n                    </filter>\n                    <link-entity name=\"ipg_documenttype\" from=\"ipg_documenttypeid\" to=\"ipg_documenttypeid\" visible=\"false\" link-type=\"outer\" alias=\"ipg_documenttypeid\">\n                        <attribute name=\"ipg_documenttypeid\" />\n                        <attribute name=\"ipg_documenttypeabbreviation\" />\n                        <attribute name=\"ipg_name\" />\n                      </link-entity>\n                  </entity>\n                </fetch>";
            fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
            return fetchXml;
        }
        function AttachDocuments(primaryControl, selectedDocumentsIds) {
            return __awaiter(this, void 0, void 0, function () {
                var urlParameters, Sdk, options, reference, incidentId, referralId, requests, newDocData_1, fetchXml, docs, newDocData_2, e_8, e_9;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            urlParameters = getUrlParameters();
                            if (!(urlParameters && (urlParameters['etn'] === 'incident' || urlParameters['etn'] === 'ipg_referral'))) return [3 /*break*/, 2];
                            return [4 /*yield*/, AttachToCurrentCaseOrReferral(primaryControl, selectedDocumentsIds, urlParameters)];
                        case 1:
                            _a.sent();
                            $("button[data-id='dialogCloseIconButton']", parent.document).click();
                            return [2 /*return*/];
                        case 2:
                            Sdk = {
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
                            options = {
                                allowMultiSelect: false,
                                defaultEntityType: "incident",
                                entityTypes: ["incident", "ipg_referral"],
                                disableMru: true,
                            };
                            _a.label = 3;
                        case 3:
                            _a.trys.push([3, 12, , 13]);
                            return [4 /*yield*/, Xrm.Utility.lookupObjects(options)];
                        case 4:
                            reference = _a.sent();
                            if (!(reference && reference.length)) return [3 /*break*/, 11];
                            Xrm.Utility.showProgressIndicator("Reassigning a document");
                            _a.label = 5;
                        case 5:
                            _a.trys.push([5, 9, , 10]);
                            requests = [];
                            if (!(reference[0].entityType === "incident")) return [3 /*break*/, 7];
                            incidentId = reference[0].id.replace(/[{|}]/g, "");
                            newDocData_1 = {
                                "ipg_CaseId@odata.bind": "/incidents(" + incidentId + ")",
                            };
                            fetchXml = generateFetchXmlToRetrieveDocumentsByIds(selectedDocumentsIds);
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_document", fetchXml)];
                        case 6:
                            docs = _a.sent();
                            if (docs && docs.entities.length > 0) {
                                docs.entities.forEach(function (doc) {
                                    if (doc["ipg_documenttypeid.ipg_documenttypeabbreviation"] ===
                                        "MFG INV") {
                                        var associateRequest = {
                                            getMetadata: function () { return ({
                                                boundParameter: null,
                                                parameterTypes: {},
                                                operationType: 2,
                                                operationName: "Associate",
                                            }); },
                                            relationship: "ipg_incident_ipg_document",
                                            target: {
                                                entityType: "incident",
                                                id: incidentId,
                                            },
                                            relatedEntities: [
                                                {
                                                    entityType: "ipg_document",
                                                    id: doc.ipg_documentid,
                                                },
                                            ],
                                        };
                                        requests.push(associateRequest);
                                    }
                                    else {
                                        requests.push(new Sdk.UpdateRequest("ipg_document", doc.ipg_documentid, newDocData_1));
                                    }
                                });
                            }
                            return [3 /*break*/, 8];
                        case 7:
                            if (reference[0].entityType === "ipg_referral") {
                                referralId = reference[0].id.replace(/[{|}]/g, "");
                                newDocData_2 = {
                                    "ipg_ReferralId@odata.bind": "/ipg_referrals(" + referralId + ")",
                                };
                                selectedDocumentsIds.forEach(function (docId) {
                                    requests.push(new Sdk.UpdateRequest("ipg_document", docId, newDocData_2));
                                });
                            }
                            _a.label = 8;
                        case 8:
                            Xrm.WebApi.online.executeMultiple(requests).then(function (success) {
                                var alertStrings = {
                                    confirmButtonLabel: "Ok",
                                    text: "The documents are attached successfully",
                                    title: "Success",
                                };
                                var alertOptions = { height: 120, width: 260 };
                                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                                    primaryControl.refresh(true);
                                });
                            }, function (error) {
                                console.log(error.message);
                            });
                            return [3 /*break*/, 10];
                        case 9:
                            e_8 = _a.sent();
                            Xrm.Navigation.openErrorDialog({
                                message: e_8.Message || e_8.message || "Error during reassinging a document",
                            });
                            return [3 /*break*/, 10];
                        case 10:
                            Xrm.Utility.closeProgressIndicator();
                            _a.label = 11;
                        case 11: return [3 /*break*/, 13];
                        case 12:
                            e_9 = _a.sent();
                            Xrm.Navigation.openErrorDialog({
                                message: e_9.Message || e_9.message || "Error during reassinging a document",
                            });
                            return [3 /*break*/, 13];
                        case 13: return [2 /*return*/];
                    }
                });
            });
        }
        Document.AttachDocuments = AttachDocuments;
        function IfDocumentsHaveNoRelatedReferralAndCase(selectedDocumentsIds) {
            return __awaiter(this, void 0, void 0, function () {
                var fetchXML, docs;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!(selectedDocumentsIds && selectedDocumentsIds.length > 0)) return [3 /*break*/, 2];
                            fetchXML = generateFetchXmlToRetrieveDocumentsByIds(selectedDocumentsIds);
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_document", fetchXML)];
                        case 1:
                            docs = _a.sent();
                            if (docs && docs.entities.length > 0) {
                                return [2 /*return*/, docs.entities.some(function (d) {
                                        return ((d._ipg_referralid_value == null && d._ipg_caseid_value == null) ||
                                            d["ipg_documenttypeid.ipg_documenttypeabbreviation"] === "MFG INV");
                                    })];
                            }
                            _a.label = 2;
                        case 2: return [2 /*return*/, false];
                    }
                });
            });
        }
        Document.IfDocumentsHaveNoRelatedReferralAndCase = IfDocumentsHaveNoRelatedReferralAndCase;
        function SoftDeleteDocument(primaryControl) {
            var formContext = primaryControl;
            var confirmStrings = {
                text: "Select 'OK' to deactivate this document or 'Cancel' to cancel and return to previous screen. \nNote: system will retain this document as an inactive document for 180 days.",
                title: "Confirm Deletion",
            };
            var confirmOptions = { height: 200, width: 450 };
            Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(function (success) {
                if (success.confirmed) {
                    formContext.getAttribute("statecode").setValue(1);
                    formContext.getAttribute("ipg_reviewstatus").setValue(427880004); // Unable to Process
                    formContext.data.save();
                }
            });
        }
        Document.SoftDeleteDocument = SoftDeleteDocument;
        function SoftDeleteMultiple(primaryControl, selectedRecordIds) {
            var formContext = primaryControl;
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
            var confirmStrings = {
                text: "Select 'OK' to deactivate these documents or 'Cancel' to cancel and return to previous screen. \nNote: system will retain these documents as an inactive documents for 180 days.",
                title: "Confirm Deletion",
            };
            var confirmOptions = { height: 200, width: 450 };
            Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(function (success) {
                if (success.confirmed) {
                    var newDocData_3 = {
                        "statecode": "1",
                        "ipg_reviewstatus": "427880004",
                    };
                    var requests = [];
                    selectedRecordIds.forEach(function (docId) {
                        requests.push(new Sdk.UpdateRequest("ipg_document", docId, newDocData_3));
                    });
                    Xrm.WebApi.online.executeMultiple(requests).then(function (success) {
                        var alertStrings = {
                            confirmButtonLabel: "Ok",
                            text: "The documents are deactivated",
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
            });
        }
        Document.SoftDeleteMultiple = SoftDeleteMultiple;
        function IfUserHasTeam(teamName) {
            return __awaiter(this, void 0, void 0, function () {
                var currentUserId, teams;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            currentUserId = Xrm.Utility.getGlobalContext().userSettings.userId.replace(/[{|}]/g, "");
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("team", "?$select=name&$expand=teammembership_association($filter=systemuserid eq " +
                                    currentUserId +
                                    ")")];
                        case 1:
                            teams = _a.sent();
                            if (teams && teams.entities.length > 0) {
                                return [2 /*return*/, teams.entities.some(function (team) {
                                        return team.name == "IT Admin";
                                    })];
                            }
                            return [2 /*return*/, false];
                    }
                });
            });
        }
        Document.IfUserHasTeam = IfUserHasTeam;
        function OpenGeneratePatientStatementPage(selectedControl, primaryControl) {
            var pageInput = {
                pageType: 'webresource',
                webresourceName: 'ipg_/intake/incident/GeneratePatientStatement.html',
                data: primaryControl.data.entity.getId().replace(/[{}]/g, "")
            };
            var navigationOptions = {
                target: 2,
                height: 300,
                width: 600,
                position: 1,
            };
            Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(function () {
                selectedControl.refresh();
                selectedControl.refreshRibbon();
            });
        }
        Document.OpenGeneratePatientStatementPage = OpenGeneratePatientStatementPage;
        function EnableGeneratePSButton(selectedControl, primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/, IsStatementDocumentGrid(selectedControl)];
                });
            });
        }
        Document.EnableGeneratePSButton = EnableGeneratePSButton;
        function CloseForm(primaryControl) {
            primaryControl.ui.close();
        }
        Document.CloseForm = CloseForm;
        function AttachFromComputer(primaryControl) {
            var _a, _b, _c;
            var formContext = primaryControl;
            var regardingRef = {
                entityType: formContext.data.entity.getEntityName(),
                id: formContext.data.entity.getId()
            };
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "ipg_document";
            entityFormOptions["createFromEntity"] = regardingRef;
            var formParameters = {};
            if (((_a = formContext.data.entity.attributes.get("customertypecode")) === null || _a === void 0 ? void 0 : _a.getValue()) === 923720000 /* Facility */) {
                formParameters["ipg_documenttypecategoryid"] = { entityType: 'ipg_documentcategorytype', id: "" + "a31d69a3-2877-5ba0-298f-48a0a2dccf10" /* Facility */, name: 'Facility' };
            }
            else if (((_b = formContext.data.entity.attributes.get("customertypecode")) === null || _b === void 0 ? void 0 : _b.getValue()) === 923720001 /* Carrier */) {
                formParameters["ipg_documenttypecategoryid"] = { entityType: 'ipg_documentcategorytype', id: "" + "1c70ac42-b871-7557-d20e-48a0a21b8237" /* Carrier */, name: 'Carrier' };
            }
            else if (((_c = formContext.data.entity.attributes.get("customertypecode")) === null || _c === void 0 ? void 0 : _c.getValue()) === 923720002 /* Manufacturer */) {
                formParameters["ipg_documenttypecategoryid"] = { entityType: 'ipg_documentcategorytype', id: "" + "ccd48636-a66a-e911-a97b-000d3a370868" /* Manufacturer */, name: 'Manufacturer' };
            }
            window.localStorage.setItem('back', 'true');
            Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
            }, function (error) {
                console.log(error);
            });
        }
        Document.AttachFromComputer = AttachFromComputer;
        function AttachFromDocProcessing(primaryControl, selectedDocumentsIds, params) {
            Xrm.Navigation.navigateTo({ pageType: "entitylist", entityName: "ipg_document" }, { target: 2, position: 1, width: { value: 95, unit: "%" } }).then(function success(success) {
            }, function error(error) {
                console.log(error);
            });
        }
        Document.AttachFromDocProcessing = AttachFromDocProcessing;
        function AttachToCurrentCaseOrReferral(primaryControl, selectedDocumentsIds, params) {
            return __awaiter(this, void 0, void 0, function () {
                var Sdk, newDocData, fetchXml, docs;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            Sdk = {
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
                            if (params['id'] == null || params['undefined']) {
                                console.log('Unable to get Case/Referral Id');
                                return [2 /*return*/];
                            }
                            if (params['etn'] === 'incident') {
                                newDocData = {
                                    "ipg_CaseId@odata.bind": "/incidents(" + params['id'] + ")",
                                    "ipg_reviewstatus": "427880001" // Approved
                                };
                            }
                            else if (params['etn'] === 'ipg_referral') {
                                newDocData = {
                                    "ipg_ReferralId@odata.bind": "/incidents(" + params['id'] + ")",
                                    "ipg_reviewstatus": "427880001" // Approved
                                };
                            }
                            fetchXml = generateFetchXmlToRetrieveDocumentsByIds(selectedDocumentsIds);
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_document", fetchXml)];
                        case 1:
                            docs = _a.sent();
                            if (docs && docs.entities.length > 0) {
                                docs.entities.forEach(function (doc) {
                                    if ((doc["_ipg_caseid_value"] != null || doc["_ipg_referralid_value"] != null)
                                        && doc["ipg_documenttypeid.ipg_documenttypeabbreviation"] != "MFG INV") {
                                        var alertStrings = {
                                            confirmButtonLabel: "Ok",
                                            text: "Document Type '" + doc["ipg_documenttypeid.ipg_name"] + "' is already associated to another Referral or Case and cannot be associated to more than one record",
                                            title: "Action Cancelled",
                                        };
                                        var alertOptions = { height: 120, width: 260 };
                                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                                        }, function (error) {
                                            console.log(error.message);
                                        });
                                    }
                                    else {
                                        Xrm.WebApi.online.execute(new Sdk.UpdateRequest("ipg_document", doc.ipg_documentid, newDocData)).then(function (success) {
                                            var alertStrings = {
                                                confirmButtonLabel: "Ok",
                                                text: "Attached successfully",
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
                                });
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        function getUrlParameters() {
            var queryString = parent.location.search.substring(1);
            var params = {};
            var queryStringParts = queryString.split("&");
            for (var i = 0; i < queryStringParts.length; i++) {
                var pieces = queryStringParts[i].split("=");
                params[pieces[0].toLowerCase()] = pieces.length === 1 ? null : decodeURIComponent(pieces[1]);
            }
            return params;
        }
        function IsCreateForm(primaryControl) {
            var formContext = primaryControl;
            if (formContext.ui.getFormType() === 1 /*Create*/) {
                return false;
            }
            return true;
        }
        Document.IsCreateForm = IsCreateForm;
        function MarkApproved(primaryControl, selectedDocumentsIds) {
            var gridControl = primaryControl;
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
            var newDocData = { "ipg_reviewstatus": "427880001" /*Approved*/ };
            selectedDocumentsIds.forEach(function (docId) {
                Xrm.WebApi.online.execute(new Sdk.UpdateRequest("ipg_document", docId, newDocData)).then(function (success) {
                    gridControl.refresh();
                }, function (error) {
                    console.log(error.message);
                });
            });
        }
        Document.MarkApproved = MarkApproved;
    })(Document = Intake.Document || (Intake.Document = {}));
})(Intake || (Intake = {}));
