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
        if (typeof ($) === 'undefined') {
            $ = window.parent.$;
        }
        /**
         * Called on load form
         * @function Intake.Document.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var formType = formContext.ui.getFormType();
            if (formType !== 1 /* Create */) {
                if (formContext.getAttribute("ipg_name").getValue()) {
                    formContext.getControl("ipg_name").setDisabled(true);
                }
                if (formContext.getAttribute("ipg_source").getValue()) {
                    formContext.getControl("ipg_source").setDisabled(true);
                }
                if (formContext.getAttribute("createdon").getValue()) {
                    formContext.getControl("createdon").setDisabled(true);
                }
                formContext.getAttribute("ipg_documenttypecategoryid").fireOnChange();
                ShowHideDocumentCategoryTypes(formContext);
            }
            else {
                formContext.getControl('WebResource_DocumentFile').setVisible(false);
                var documentBody = formContext.getAttribute("ipg_documentbody");
                if (documentBody) {
                    documentBody.setRequiredLevel("required");
                    formContext.getControl("ipg_documentbody").setVisible(true);
                }
                formContext.getAttribute("ipg_source").setValue(923720002);
                formContext.ui.tabs.get("Document").sections.get("Document_section_4").setVisible(false);
                formContext.ui.tabs.get("Document").sections.get("Document_section_2").setVisible(false);
                formContext.getAttribute("ipg_filterdocumenttypes").setValue(false);
                if (formContext.getAttribute("ipg_facilityid").getValue() != null
                    || formContext.getAttribute("ipg_carrierid").getValue() != null
                    || formContext.getAttribute("ipg_ipg_manufacturerid").getValue() != null) {
                    formContext.getAttribute("ipg_documenttypecategoryid").fireOnChange();
                }
                else if (formContext.getAttribute("ipg_caseid").getValue() != null
                    || formContext.getAttribute("ipg_referralid").getValue() != null) {
                    function addCustomLookupFilter(formContext) {
                        var filters = [];
                        filters.push("<condition attribute=\"ipg_documentcategorytypeid\" operator=\"eq\" value=\"".concat("db8c208e-c02f-72f1-47db-48a0a28a0fe9" /* PatientProcedure */, "\" />"));
                        filters.push("<condition attribute=\"ipg_documentcategorytypeid\" operator=\"eq\" value=\"".concat("979a4281-dbeb-eb11-bacb-000d3a5aaa66" /* Claim */, "\" />"));
                        filters.push("<condition attribute=\"ipg_documentcategorytypeid\" operator=\"eq\" value=\"".concat("63187393-dbeb-eb11-bacb-000d3a5aaa66" /* PurchaseOrder */, "\" />"));
                        filters.push("<condition attribute=\"ipg_documentcategorytypeid\" operator=\"eq\" value=\"".concat("10464d75-dbeb-eb11-bacb-000d3a5aaa66" /* PatientStatement */, "\" />"));
                        var filterXml = "<filter type=\"or\">".concat(filters.join(''), "</filter>");
                        formContext.getControl("ipg_documenttypeid").addCustomFilter(filterXml, "ipg_documenttype");
                    }
                    var filterFunction = function () {
                        addCustomLookupFilter(formContext);
                    };
                    formContext.getControl("ipg_documenttypeid").addPreSearch(filterFunction);
                }
            }
            ChangeNameCreatedByModifiedBy(formContext);
            showHideEbvResponse(formContext);
            checkDocType(formContext);
            var documentTypeFilterConditions = "";
            if (formType !== 1 /* Create */) {
                documentTypeFilterConditions = "<condition attribute='ipg_name' value='%Generic%' operator='not-like'/>" +
                    "<condition attribute='ipg_name' value='%Fax%' operator='not-like'/>";
            }
            else {
                documentTypeFilterConditions = "<condition attribute='ipg_name' value='%Portal%' operator='not-like'/>" +
                    "<condition attribute='ipg_name' value='%Fax%' operator='not-like'/>";
            }
            formContext.getControl("ipg_documenttypeid").addPreSearch(function () {
                addDocumentTypeFilter(formContext, documentTypeFilterConditions);
            });
            function addDocumentTypeFilter(formContext, documentTypeFilter) {
                var filterXML = "<filter type='and'>".concat(documentTypeFilter, "</filter>");
                formContext.getControl("ipg_documenttypeid").addCustomFilter(filterXML, "ipg_documenttype");
            }
            if (formType !== 1 /* Create */) {
                OpenRelatedCaseOrReferral(executionContext);
                //ShowAccountsTabByCategory(formContext);
            }
        }
        Document.OnLoadForm = OnLoadForm;
        // Maybe it will be use in the future
        // /**
        //  * Show Accounts Tab By Category
        //  * @function Intake.Document.ShowAccountsTabByCategory
        //  * @returns {void}
        // */
        // function ShowAccountsTabByCategory(formContext: Xrm.FormContext) {
        //   let category = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
        //   if(category == null)
        //     return;
        //   let categoryName = category[0].name;
        //   formContext.ui.tabs.get(categoryName + "Tab")?.setVisible(true);        
        // }
        /**
         * on change document type
         * @function Intake.Document.OnChangeDocumentType
         * @returns {void}
        */
        function OnChangeDocumentType(executionContext) {
            var formContext = executionContext.getFormContext();
            var lookUp = executionContext.getEventSource();
            formContext.ui.refreshRibbon();
            var DocumentType = lookUp.getValue();
            if (DocumentType != null && DocumentType.length > 0) {
                Xrm.WebApi.retrieveRecord(DocumentType[0].entityType, DocumentType[0].id, "?$select=_ipg_documentcategorytypeid_value")
                    .then(function (response) {
                    if (response._ipg_documentcategorytypeid_value) {
                        var docCategory = formContext.getAttribute("ipg_documenttypecategoryid");
                        var lookUp_1 = {
                            id: response["_ipg_documentcategorytypeid_value"],
                            name: response["_ipg_documentcategorytypeid_value@OData.Community.Display.V1.FormattedValue"],
                            entityType: response["_ipg_documentcategorytypeid_value@Microsoft.Dynamics.CRM.lookuplogicalname"]
                        };
                        docCategory && docCategory.setValue([lookUp_1]);
                        if (formContext.ui.getFormType() !== 1 /* Create */) {
                            formContext.getAttribute("ipg_documenttypecategoryid").fireOnChange();
                        }
                        if (response["_ipg_documentcategorytypeid_value@OData.Community.Display.V1.FormattedValue"].toLowerCase() == "carrier") {
                            formContext.getControl("ipg_carrierid").setVisible(true);
                            formContext.getControl("ipg_facilityid").setVisible(false);
                            formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);
                            formContext.getAttribute("ipg_carrierid").setRequiredLevel("required");
                            formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
                            formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
                        }
                        else if (response["_ipg_documentcategorytypeid_value@OData.Community.Display.V1.FormattedValue"].toLowerCase() == "facility") {
                            formContext.getControl("ipg_carrierid").setVisible(false);
                            formContext.getControl("ipg_facilityid").setVisible(true);
                            formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);
                            formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
                            formContext.getAttribute("ipg_facilityid").setRequiredLevel("required");
                            formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
                        }
                        else if (response["_ipg_documentcategorytypeid_value@OData.Community.Display.V1.FormattedValue"].toLowerCase() == "manufacturer") {
                            formContext.getControl("ipg_carrierid").setVisible(false);
                            formContext.getControl("ipg_facilityid").setVisible(false);
                            formContext.getControl("ipg_ipg_manufacturerid").setVisible(true);
                            formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
                            formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
                            formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("required");
                        }
                        else {
                            formContext.getControl("ipg_carrierid").setVisible(false);
                            formContext.getControl("ipg_facilityid").setVisible(false);
                            formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);
                            formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
                            formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
                            formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
                        }
                    }
                })
                    .catch(function (error) {
                    console.log(error);
                });
            }
            showHideEbvResponse(formContext);
            checkDocType(formContext);
        }
        Document.OnChangeDocumentType = OnChangeDocumentType;
        /**
         * Called on save form
         * @function Intake.Document.OnSaveForm
         * @returns {void}
        */
        var preventSave = true;
        function OnSaveForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var docType = formContext.getAttribute("ipg_documenttypeid").getValue();
            var facility = formContext.getAttribute("ipg_facilityid").getValue();
            if (formContext.data.entity.getIsDirty() === false) {
                return;
            }
            //let saveEventArgs: Xrm.Events.SaveEventArguments = executionContext.getEventArgs();
            //saveEventArgs.preventDefault();
            checkForChangesAndReopenCase(executionContext);
            if (formContext.ui.getFormType() === 1
                && formContext.getAttribute("ipg_reviewstatus")) {
                formContext.getAttribute("ipg_reviewstatus").setValue(427880001);
            }
            if (docType) {
                var docName = docType[0].name;
                formContext.getAttribute("ipg_textdocumenttype").setValue(docName);
            }
            if (facility) {
                var facName = facility[0].name;
                formContext.getAttribute("ipg_textfacilityname").setValue(facName);
            }
        }
        Document.OnSaveForm = OnSaveForm;
        function SetDocumentCategory(formContext) {
            var _a;
            var carrier = (_a = formContext.getAttribute("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (carrier) {
                Xrm.WebApi.retrieveMultipleRecords("ipg_documentcategorytype", "?$select=ipg_name, ipg_documentcategorytypeid&$filter=ipg_name eq 'Carrier'").then(function success(results) {
                    if (results.entities.length > 0) {
                        var object = new Array();
                        object[0] = new Object();
                        object[0].id = results.entities[0]["ipg_documentcategorytypeid"];
                        object[0].name = results.entities[0]["ipg_name"];
                        object[0].entityType = "ipg_documentcategorytype";
                        var documentCategoryAttr = formContext.getAttribute("ipg_documenttypecategoryid");
                        documentCategoryAttr.setValue(object);
                        documentCategoryAttr.fireOnChange();
                        formContext.getControl("ipg_documenttypecategoryid").setDisabled(true);
                    }
                }, function (error) {
                });
            }
        }
        //function Intake.Document.refreshCrmForm
        function refreshCrmForm(executionContext) {
            var formContext = executionContext.getFormContext();
            setTimeout(function () {
                var windowOptions = {
                    openInNewWindow: false,
                    entityName: formContext.data.entity.getEntityName(),
                    entityId: formContext.data.entity.getId()
                };
                Xrm.Navigation.openForm(windowOptions);
            }, 1000);
        }
        Document.refreshCrmForm = refreshCrmForm;
        /**
         * on change document type category
         * @function Intake.Document.OnChangeDocumentCategoryType
         * @returns {void}
        */
        function OnChangeDocumentCategoryType(executionContext) {
            var formContext = executionContext.getFormContext();
            //formContext.getAttribute("ipg_documenttypeid").setValue(null);
            ShowHideDocumentCategoryTypes(formContext);
            addFiltersToDocumentType(formContext);
        }
        Document.OnChangeDocumentCategoryType = OnChangeDocumentCategoryType;
        /**
         * add filters to doucument type field
         * @function Intake.Referral.OnChangePatientDOB
         * @returns {void}
        */
        function addFiltersToDocumentType(formContext) {
            function addCustomLookupFilter(formContext) {
                var filters = [];
                var documentCategoryType = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
                if (documentCategoryType) {
                    filters.push("<condition attribute=\"ipg_documentcategorytypeid\" operator=\"eq\" value=\"".concat(Intake.Utility.removeCurlyBraces(documentCategoryType[0].id), "\" />"));
                }
                var filterXml = "<filter type=\"and\">".concat(filters.join(''), "</filter>");
                formContext.getControl("ipg_documenttypeid").addCustomFilter(filterXml, "ipg_documenttype");
            }
            var filterFunction = function () {
                addCustomLookupFilter(formContext);
            };
            var documentCategoryType = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
            if (documentCategoryType) {
                formContext.getControl("ipg_documenttypeid").addPreSearch(filterFunction);
            }
            else {
                formContext.getControl("ipg_documenttypeid").removePreSearch(filterFunction);
            }
        }
        function ChangeNameCreatedByModifiedBy(formContext) {
            var createdByAttr = formContext.getAttribute("createdby");
            var modifiedByAttr = formContext.getAttribute("modifiedby");
            var sourceAttr = formContext.getAttribute("ipg_source");
            var sourceVal = sourceAttr.getText();
            switch (sourceVal) {
                case "Portal":
                case "Fax":
                    var createdByVal = createdByAttr.getValue();
                    var modifiedByVal = modifiedByAttr.getValue();
                    createdByVal[0].name = sourceVal;
                    createdByAttr.setValue(createdByVal);
                    if (modifiedByVal[0].id === createdByVal[0].id)
                        modifiedByAttr.setValue(createdByVal);
                    break;
                default:
            }
        }
        function populateFileInfo() {
            $('#file-info').empty();
            var documentId = Intake.Utility.removeCurlyBraces(parent.Xrm.Page.data.entity.getId());
            if (documentId) {
                parent.Xrm.WebApi.retrieveMultipleRecords('annotation', "?$select=annotationid,filename&$filter=_objectid_value eq ".concat(documentId)).then(function success(results) {
                    if (results.entities.length) {
                        $('#file-info').append("<label id='".concat(results.entities[0]['annotationid'], "'><a href='#' class='attachment'>").concat(results.entities[0]['filename'], "</a></label>&nbsp;<button class='file-delete'>X</button>"));
                    }
                }, function (error) {
                    parent.Xrm.Utility.alertDialog(error.message, null);
                });
            }
        }
        Document.populateFileInfo = populateFileInfo;
        function onFileClick(e, node) {
            e.preventDefault();
            var annotationId = $(node).closest('label').attr('id');
            var URL = "".concat(parent.Xrm.Page.context.getClientUrl(), "/userdefined/edit.aspx?etc=5&id=").concat(annotationId);
            $.get(URL, function (data) {
                var WRPCTokenElement = $(data).find('[WRPCTokenUrl]');
                if (WRPCTokenElement) {
                    var WRPCTokenUrl = WRPCTokenElement.attr('WRPCTokenUrl');
                    if (WRPCTokenUrl) {
                        URL = "".concat(parent.Xrm.Page.context.getClientUrl(), "/Activities/Attachment/download.aspx?AttachmentType=5&AttachmentId=").concat(annotationId, "&IsNotesTabAttachment=undefined").concat(WRPCTokenUrl);
                        window.open(URL);
                    }
                }
                return false;
            });
        }
        Document.onFileClick = onFileClick;
        function onFileDelete(node) {
            if (confirm('Are you sure you want to delete this File?')) {
                var annotationId = $(node).parent().find('label').attr('id');
                parent.Xrm.WebApi.deleteRecord('annotation', annotationId).then(function success(result) {
                    parent.Xrm.Page.getAttribute('ipg_filename').setValue(null);
                    parent.Xrm.Page.getAttribute('ipg_name').setValue(null);
                    parent.Xrm.Page.getAttribute('ipg_documentbody').setValue(null);
                    parent.Xrm.Page.getControl('ipg_documentbody').setVisible(true);
                    parent.Xrm.Page.getControl('WebResource_DocumentFile').setVisible(false);
                }, function (error) {
                    parent.Xrm.Utility.alertDialog(error.message, null);
                });
            }
        }
        Document.onFileDelete = onFileDelete;
        function ShowHideDocumentCategoryTypes(formContext) {
            var documentCategoryType = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
            if (documentCategoryType) {
                if (documentCategoryType[0].name.toLowerCase() == "carrier") {
                    formContext.getControl("ipg_carrierid").setVisible(true);
                    formContext.getControl("ipg_facilityid").setVisible(false);
                    formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);
                    formContext.getAttribute("ipg_carrierid").setRequiredLevel("required");
                    formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
                    formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
                }
                else if (documentCategoryType[0].name.toLowerCase() == "facility") {
                    formContext.getControl("ipg_carrierid").setVisible(false);
                    formContext.getControl("ipg_facilityid").setVisible(true);
                    formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);
                    formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
                    formContext.getAttribute("ipg_facilityid").setRequiredLevel("required");
                    formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
                }
                else if (documentCategoryType[0].name.toLowerCase() == "manufacturer") {
                    formContext.getControl("ipg_carrierid").setVisible(false);
                    formContext.getControl("ipg_facilityid").setVisible(false);
                    formContext.getControl("ipg_ipg_manufacturerid").setVisible(true);
                    formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
                    formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
                    formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("required");
                }
                else {
                    formContext.getControl("ipg_carrierid").setVisible(false);
                    formContext.getControl("ipg_facilityid").setVisible(false);
                    formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);
                    formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
                    formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
                    formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
                }
            }
            else {
                formContext.getControl("ipg_carrierid").setVisible(false);
                formContext.getControl("ipg_facilityid").setVisible(false);
                formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);
            }
        }
        function showHideEbvResponse(formContext) {
            return __awaiter(this, void 0, void 0, function () {
                var docTypeAttribute, ebvResponseControl, visible, docTypeValue, docType;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            docTypeAttribute = formContext.getAttribute('ipg_documenttypeid');
                            ebvResponseControl = formContext.getControl("ipg_ebvresponseid");
                            if (!(docTypeAttribute && ebvResponseControl)) return [3 /*break*/, 3];
                            visible = false;
                            docTypeValue = docTypeAttribute.getValue();
                            if (!(docTypeValue && docTypeValue.length)) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord('ipg_documenttype', docTypeValue[0].id, '?$select=ipg_documenttypeabbreviation')];
                        case 1:
                            docType = _a.sent();
                            if (docType.ipg_documenttypeabbreviation == 'EBV') {
                                visible = true;
                            }
                            _a.label = 2;
                        case 2:
                            ebvResponseControl.setVisible(visible);
                            _a.label = 3;
                        case 3: return [2 /*return*/];
                    }
                });
            });
        }
        function checkForChangesAndReopenCase(executionContext) {
            var formContext = executionContext.getFormContext();
            if (formContext.getAttribute("ipg_caseid")) {
                var CaseId = formContext.getAttribute("ipg_caseid").getValue();
                if (CaseId) {
                    if (preventSave) {
                        executionContext.getEventArgs().preventDefault();
                    }
                    Xrm.WebApi.retrieveRecord('incident', CaseId[0].id, "?$select=ipg_casestatus")
                        .then(function (response) {
                        if (!preventSave) {
                            return;
                        }
                        preventSave = false;
                        var CaseStatus = response["ipg_casestatus@OData.Community.Display.V1.FormattedValue"];
                        if (CaseStatus == 'Closed') {
                            var confirmStrings = { text: "This case is currently closed. Do you want to reopen?", title: "Confirm", confirmButtonLabel: "Yes - Reopen", cancelButtonLabel: "No - Leave closed" };
                            Xrm.Navigation.openConfirmDialog(confirmStrings, null).then(function (success) {
                                if (success.confirmed) {
                                    console.log("Yes - Reopen");
                                    formContext.data.save();
                                }
                                else {
                                    console.log("No - Leave closed");
                                    formContext.data.save();
                                }
                            });
                        }
                        else {
                            formContext.data.save();
                        }
                    })
                        .catch(function (error) {
                        console.log(error);
                    });
                }
            }
        }
        function checkDocType(formContext) {
            return __awaiter(this, void 0, void 0, function () {
                var docTypeAttribute, docTypeValue, relatedCaseControl;
                return __generator(this, function (_a) {
                    docTypeAttribute = formContext.getAttribute('ipg_documenttypeid');
                    if (docTypeAttribute && docTypeAttribute.getValue() && docTypeAttribute.getValue().length) {
                        docTypeValue = docTypeAttribute.getValue();
                        relatedCaseControl = formContext.getControl("ipg_caseid");
                        if (docTypeValue[0].name === "User Uploaded Generic Document") {
                            if (relatedCaseControl) {
                                formContext.getControl("ipg_caseid").setDisabled(true);
                            }
                        }
                        else if (docTypeValue[0].name === "Fax") {
                            if (relatedCaseControl) {
                                formContext.getControl("ipg_caseid").setDisabled(true);
                            }
                        }
                        else {
                            if (relatedCaseControl) {
                                formContext.getControl("ipg_caseid").setDisabled(false);
                            }
                        }
                    }
                    return [2 /*return*/];
                });
            });
        }
        /**
         * Called on 'Case Source' field change
         * @function Intake.Document.OnCaseSourceChange
         * @returns {void}
        */
        function OnCaseSourceChange(executionContext) {
            var formContext = executionContext.getFormContext();
            formContext.ui.clearFormNotification("case");
            var incident = formContext.getAttribute("ipg_caseid").getValue();
            var caseSource = formContext.getAttribute("ipg_casesourceid").getValue();
            if (incident && caseSource) {
                if (incident[0].id == caseSource[0].id) {
                    formContext.ui.setFormNotification("Please, select a document from an another case", "WARNING", "case");
                }
            }
        }
        Document.OnCaseSourceChange = OnCaseSourceChange;
        function OpenRelatedCaseOrReferral(executionContext) {
            var formContext = executionContext.getFormContext();
            var formType = formContext.ui.getFormType();
            if (formType === 1 /* Create */) {
                return;
            }
            var back = window.localStorage.getItem('back');
            if (back !== 'true') {
                return;
            }
            var relatedCase = formContext.data.entity.attributes.get("ipg_caseid").getValue();
            var relatedReferral = formContext.data.entity.attributes.get("ipg_referralid").getValue();
            if (relatedCase != null && relatedCase.length > 0) {
                var regardingEntityName = relatedCase[0].entityType;
                var regardingId = relatedCase[0].id;
            }
            else if (relatedReferral != null && relatedReferral.length > 0) {
                var regardingEntityName = relatedReferral[0].entityType;
                var regardingId = relatedReferral[0].id;
            }
            else {
                return;
            }
            var entityFormOptions = {};
            entityFormOptions["entityName"] = regardingEntityName;
            entityFormOptions["entityId"] = regardingId;
            Xrm.Navigation.openForm(entityFormOptions).then(function (success) {
                console.log(success);
                window.localStorage.setItem('focusOnTab', 'Documents');
                if (formContext.data.entity.getEntityName() != 'ipg_document') {
                    window.localStorage.removeItem('back');
                }
            }, function (error) {
                console.log(error);
                window.localStorage.removeItem('back');
            });
        }
        Document.OpenRelatedCaseOrReferral = OpenRelatedCaseOrReferral;
        function SetReviewStatus(formContext) {
            var categoryType = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
            formContext.getAttribute("ipg_reviewstatus").setValue(null);
            if (categoryType) {
                var name_1 = categoryType[0].name;
                switch (name_1) {
                    case "Carrier":
                        formContext.getAttribute("ipg_reviewstatus").setValue(427880001);
                        break;
                    case "Facility":
                        formContext.getAttribute("ipg_reviewstatus").setValue(427880001);
                        break;
                    case "Manufacturer":
                        formContext.getAttribute("ipg_reviewstatus").setValue(427880001);
                        break;
                }
            }
        }
        Document.SetReviewStatus = SetReviewStatus;
    })(Document = Intake.Document || (Intake.Document = {}));
})(Intake || (Intake = {}));
