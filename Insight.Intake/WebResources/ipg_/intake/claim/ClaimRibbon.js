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
 * @namespace Intake.Claim.Ribbon
 */
var Intake;
(function (Intake) {
    var Claim;
    (function (Claim) {
        var Ribbon;
        (function (Ribbon) {
            if (typeof ($) === 'undefined') {
                $ = window.parent.$;
            }
            var CaseStatesEnum = {
                CarrierCollections: 923720004,
                PatientCollections: 923720005
            };
            var ipg_claimreplacement = {
                New: 427880000,
                Replacement: 427880001
            };
            /**
             * Enables\Disables Post button
             * @function Intake.Claim.Ribbon.PostButtonEnabled
             * @returns {boolean}
            */
            var isButtonEnabled = false;
            function PostButtonEnabled(primaryControl) {
                var formContext = primaryControl;
                if (window["isAsyncOperationCompleted"])
                    return isButtonEnabled;
                Xrm.WebApi.retrieveRecord("ipg_claimresponsebatch", formContext.data.entity.getId(), "?$select=_ownerid_value,modifiedon").then(function success(result) {
                    window["isAsyncOperationCompleted"] = true;
                    isButtonEnabled = (result._ownerid_value.toUpperCase() == Intake.Utility.removeCurlyBraces(Xrm.Utility.getGlobalContext().userSettings.userId).toUpperCase());
                    if (isButtonEnabled) {
                        formContext.ui.refreshRibbon(true);
                    }
                });
                return false;
            }
            Ribbon.PostButtonEnabled = PostButtonEnabled;
            /**
             * Runs on modified on event
             * @function Intake.Claim.Ribbon.OnModifiedOnChange
             * @returns {void}
            */
            function OnModifiedOnChange(primaryControl) {
                var formContext = primaryControl;
                window["isAsyncOperationCompleted"] = false;
            }
            Ribbon.OnModifiedOnChange = OnModifiedOnChange;
            /**
             * Called on Post button click
             * @function Intake.Claim.Ribbon.OnPostClick
             * @returns {void}
            */
            function OnPostClick(selectedItems) {
                /*let formParameters =
                {
                  claimResponseHeaderId: selectedItems[0].Id
                };
                let webResourceName = "ipg_/intake/claim/PaymentProcessing.html";
                var customParameters = encodeURIComponent("params=" + JSON.stringify(formParameters));
                Xrm.Navigation.openWebResource(webResourceName, null, customParameters);*/
                var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
                var claimResponseHeaderRecord = [];
                var url = baseUrl
                    + "ipg_claimresponseheaderSet?"
                    + "$select=ipg_PaymentNotes,ipg_CaseNumber,ipg_CorrectedCaseNumber,ipg_ClaimNumber,ipg_ClaimResponseBatchId,ipg_AmountPaid_new,ipg_Interest_new,ipg_PaymentType,ipg_RefundType"
                    + "&$filter=ipg_claimresponseheaderId eq (guid'" + selectedItems[0].Id + "')";
                GetRecords(url, claimResponseHeaderRecord);
                if (claimResponseHeaderRecord.length > 0) {
                    var caseNumber = (claimResponseHeaderRecord[0].ipg_CorrectedCaseNumber) ? claimResponseHeaderRecord[0].ipg_CorrectedCaseNumber : claimResponseHeaderRecord[0].ipg_CaseNumber;
                    if (caseNumber) {
                        var caseRecord = [];
                        url = baseUrl
                            + "IncidentSet?"
                            + "$select=IncidentId,ipg_CaseStatus"
                            + "&$filter=Title eq ('" + caseNumber + "')";
                        GetRecords(url, caseRecord);
                        if (caseRecord.length > 0) {
                            if (caseRecord[0].ipg_CaseStatus.Value == 923720001) //closed
                             {
                                Xrm.Navigation.openAlertDialog({ text: "This case is currently closed. Therefore, payment or adjustment transactions cannot be applied at this time. Please reopen the case to proceed." });
                                return;
                            }
                            var entityFormOptions = {};
                            entityFormOptions["entityName"] = "ipg_payment";
                            entityFormOptions["formId"] = "2708FC53-BBD4-4D52-B91E-56C5524B97F4";
                            var formParameters = {};
                            formParameters["ipg_caseid"] = caseRecord[0].IncidentId;
                            formParameters["ipg_caseidname"] = caseNumber;
                            formParameters["ipg_claimresponseheader"] = selectedItems[0].Id;
                            formParameters["ipg_claimresponseheadername"] = selectedItems[0].Name;
                            if ((claimResponseHeaderRecord[0].ipg_PaymentType.Value == 427880002) && (!claimResponseHeaderRecord[0].ipg_RefundType)) {
                                formParameters["ipg_totalinsurancepaid"] = -claimResponseHeaderRecord[0].ipg_AmountPaid_new.Value;
                            }
                            else if ((claimResponseHeaderRecord[0].ipg_PaymentType.Value == 427880002) && (claimResponseHeaderRecord[0].ipg_RefundType)) {
                                formParameters["ipg_memberpaid_new"] = -claimResponseHeaderRecord[0].ipg_AmountPaid_new.Value;
                            }
                            else {
                                formParameters["ipg_memberpaid_new"] = claimResponseHeaderRecord[0].ipg_AmountPaid_new.Value;
                            }
                            formParameters["ipg_notes"] = claimResponseHeaderRecord[0].ipg_PaymentNotes;
                            var claimRecord = [];
                            url = baseUrl
                                + "InvoiceSet?"
                                + "$select=Name,InvoiceId"
                                + "&$filter=ipg_caseid/Id eq (guid'" + caseRecord[0].IncidentId + "') and (Name eq '" + claimResponseHeaderRecord[0].ipg_ClaimNumber + "')";
                            GetRecords(url, claimRecord);
                            if (claimRecord.length > 0) {
                                formParameters["ipg_claim"] = claimRecord[0].InvoiceId;
                                formParameters["ipg_claimname"] = claimRecord[0].Name;
                                formParameters["ipg_claimtoggle"] = true;
                            }
                            var claimResponseBatchRecord = [];
                            url = baseUrl
                                + "ipg_claimresponsebatchSet?"
                                + "$select=ipg_PaymentFrom,ipg_IsManualBatch"
                                + "&$filter=ipg_claimresponsebatchId eq (guid'" + claimResponseHeaderRecord[0].ipg_ClaimResponseBatchId.Id + "')";
                            GetRecords(url, claimResponseBatchRecord);
                            if ((claimResponseBatchRecord.length > 0) && (claimResponseBatchRecord[0].ipg_PaymentFrom.Value == 427880001) || //patient payment
                                (claimResponseHeaderRecord[0].ipg_PaymentType.Value == 427880002) && claimResponseHeaderRecord[0].ipg_RefundType) { //patient refund
                                entityFormOptions["formId"] = "05A46600-F1C0-450E-9846-73E3E82829EB";
                            }
                            else {
                                formParameters["ipg_interestpayment"] = claimResponseHeaderRecord[0].ipg_Interest_new.Value;
                            }
                            formParameters["ipg_ismanualpayment"] = (claimResponseBatchRecord.length > 0) && (claimResponseBatchRecord[0].ipg_IsManualBatch);
                            Xrm.Navigation.openForm(entityFormOptions, formParameters);
                        }
                    }
                }
            }
            Ribbon.OnPostClick = OnPostClick;
            /**
             * Called on Review button click
             * @function Intake.Claim.Ribbon.OnReviewClick
             * @returns {void}
            */
            function OnReviewClick(primaryControl, selectedItems) {
                var formContext = primaryControl;
                var data = {
                    "ipg_poststatus": "review"
                };
                Xrm.WebApi.updateRecord("ipg_claimresponseheader", selectedItems[0].Id, data).then(function success(result) {
                    window["previousSelectedItemId"] = '';
                    formContext.getControl("ClaimResponseHeaders").refresh();
                    formContext.getControl("PaymentsSubgrid").refresh();
                    formContext.ui.refreshRibbon(true);
                    CreateTask(formContext, selectedItems[0].Id);
                }, function (error) {
                    console.log(error.message);
                });
            }
            Ribbon.OnReviewClick = OnReviewClick;
            /**
             * Create Task when header is in review status
             * @function Intake.Claim.Ribbon.CreateTask
             * @returns {void}
            */
            function CreateTask(formContext, headerId) {
                Xrm.WebApi.online.retrieveMultipleRecords("team", "?$select=teamid&$filter=name eq 'Patient Collectors'").then(function success(results) {
                    if (results.entities.length) {
                        var teamid = results.entities[0]["teamid"];
                        var entity = {};
                        entity["subject"] = formContext.getAttribute("ipg_paymentsource").getValue() + " Batch Requires Review";
                        entity["prioritycode"] = 1;
                        //entity["description"] = "";
                        entity["regardingobjectid_ipg_claimresponseheader@odata.bind"] = "/ipg_claimresponseheaders(" + headerId + ")";
                        entity["scheduledstart"] = Date.now;
                        entity["ownerid@odata.bind"] = "/teams(" + teamid + ")";
                        Xrm.WebApi.createRecord("task", entity).then(function success(result) {
                        }, function (error) {
                            Xrm.Navigation.openErrorDialog(error);
                        });
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog(error);
                });
            }
            /**
             * Called on Undo Review button click
             * @function Intake.Claim.Ribbon.OnUndoReviewClick
             * @returns {void}
            */
            function OnUndoReviewClick(primaryControl, selectedItems) {
                var formContext = primaryControl;
                var data = {
                    "ipg_poststatus": "new"
                };
                Xrm.WebApi.updateRecord("ipg_claimresponseheader", selectedItems[0].Id, data).then(function success(result) {
                    window["previousSelectedItemId"] = '';
                    formContext.getControl("ClaimResponseHeaders").refresh();
                    formContext.getControl("PaymentsSubgrid").refresh();
                    formContext.ui.refreshRibbon(true);
                }, function (error) {
                    console.log(error.message);
                });
            }
            Ribbon.OnUndoReviewClick = OnUndoReviewClick;
            /**
             * Called on Corr Case button click
             * @function Intake.Claim.Ribbon.OnCorrCaseClick
             * @returns {void}
            */
            function OnCorrCaseClick(primaryControl, selectedItems) {
                var formContext = primaryControl;
                var options = {
                    allowMultiSelect: false,
                    defaultEntityType: 'incident',
                    entityTypes: ['incident'],
                    disableMru: true
                };
                Xrm.Utility.lookupObjects(options).then(function (newCase) {
                    if (newCase && newCase.length) {
                        var entity = {};
                        entity['ipg_CaseId@odata.bind'] = "/incidents(" + newCase[0].id.replace('{', '').replace('}', '') + ")";
                        entity['ipg_correctedcasenumber'] = newCase[0].name;
                        Xrm.WebApi.updateRecord('ipg_claimresponseheader', selectedItems[0].Id, entity).then(function success(result) {
                            window["previousSelectedItemId"] = '';
                            formContext.getControl('ClaimResponseHeaders').refresh();
                            formContext.getControl('PaymentsSubgrid').refresh();
                        }, function (error) {
                            Xrm.Navigation.openErrorDialog(error);
                        });
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
            Ribbon.OnCorrCaseClick = OnCorrCaseClick;
            /**
             * Called on Completed button click
             * @function Intake.Claim.Ribbon.OnCompletedClick
             * @returns {void}
            */
            function OnCompletedClick(primaryControl) {
                var formContext = primaryControl;
                var data = {
                    "statuscode": 427880000
                };
                Xrm.WebApi.updateRecord("ipg_claimresponsebatch", formContext.data.entity.getId(), data).then(function success(result) {
                    formContext.data.refresh(true);
                    formContext.ui.refreshRibbon();
                }, function (error) {
                    console.log(error.message);
                });
            }
            Ribbon.OnCompletedClick = OnCompletedClick;
            /**
             * Enables\Disables Completed button
             * @function Intake.Claim.Ribbon.CompletedButtonEnabled
             * @returns {boolean}
            */
            var isCompletedButtonEnabled = false;
            function CompletedButtonEnabled(primaryControl) {
                var formContext = primaryControl;
                if (window["isAsyncOperationCompletedButtonEnabled"])
                    return isCompletedButtonEnabled;
                var query = "?$select=ipg_amountpaid_new,ipg_claimresponseheaderid&$expand=ipg_ClaimResponseBatchId($select=ipg_claimresponsebatchid,ipg_isfull835,statuscode)&$filter=ipg_poststatus ne 'posted' and ipg_ClaimResponseBatchId/ipg_claimresponsebatchid eq " + formContext.data.entity.getId().slice(1, -1);
                Xrm.WebApi.retrieveMultipleRecords("ipg_claimresponseheader", query)
                    .then(function (result) {
                    window["isAsyncOperationCompletedButtonEnabled"] = true;
                    var numberHeaders = 0;
                    for (var _i = 0, _a = result.entities; _i < _a.length; _i++) {
                        var entity = _a[_i];
                        if ((entity.ipg_ClaimResponseBatchId.statuscode == 427880000) || (entity.ipg_amountpaid_new && entity.ipg_amountpaid_new.Value) || (entity.ipg_ClaimResponseBatchId.ipg_isfull835))
                            numberHeaders++;
                    }
                    //isCompletedButtonEnabled = (result.entities.length == 0)
                    isCompletedButtonEnabled = (numberHeaders == 0);
                    if (isCompletedButtonEnabled) {
                        formContext.ui.refreshRibbon();
                    }
                }, function (error) {
                    console.log(error.message);
                });
                return false;
            }
            Ribbon.CompletedButtonEnabled = CompletedButtonEnabled;
            /**
             * Checks if selected record in Review Status
             * @function Intake.Claim.Ribbon.IsSelectedRecordInReviewStatus
             * @returns {boolean}
            */
            var selectedRecordInReviewStatus = false;
            //let previousSelectedItemId;
            function IsSelectedRecordInReviewStatus(selectedEntityTypeName, firstSelectedItemId, primaryControl) {
                var formContext = primaryControl;
                var stateCodeAttr = 'ipg_poststatus';
                if (firstSelectedItemId == null)
                    return;
                if (window["previousSelectedItemId"] !== firstSelectedItemId) {
                    window["previousSelectedItemId"] = firstSelectedItemId;
                    Xrm.WebApi.retrieveRecord(selectedEntityTypeName, firstSelectedItemId.replace(/[{}]/g, ""), "?$select=" + stateCodeAttr).then(function success(result) {
                        selectedRecordInReviewStatus = (result[stateCodeAttr] == 'review');
                        formContext.ui.refreshRibbon(true);
                    }, function (error) {
                        console.log(error.message);
                    });
                }
                else {
                    return !selectedRecordInReviewStatus;
                }
            }
            Ribbon.IsSelectedRecordInReviewStatus = IsSelectedRecordInReviewStatus;
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
            /**
             * Called on Void Claim button click
             * @function Intake.Claim.Ribbon.OnVoidClaimClick
             * @returns {void}
            */
            function OnVoidClaimClick(primaryControl) {
                var formContext = primaryControl;
                var reason = prompt('Please enter a reason');
                if (reason) {
                    if (confirm("Are you sure you want to Void Claim " + formContext.getAttribute('name').getValue() + "?")) {
                        var data = {
                            'ipg_active': false,
                            'ipg_voidreason': reason,
                            'ipg_status': 427880003,
                            'ipg_reason': 427880008 //voided by IPG
                        };
                        Xrm.WebApi.updateRecord('invoice', formContext.data.entity.getId(), data).then(function success(result) {
                            formContext.getAttribute('ipg_voidreason').setValue(reason);
                            formContext.getControl().forEach(function (control) {
                                control.setDisabled(true);
                            });
                        }, function (error) {
                            console.log(error.message);
                        });
                    }
                }
            }
            Ribbon.OnVoidClaimClick = OnVoidClaimClick;
            /**
             * Called on Delete button click
             * @function Intake.Claim.Ribbon.OnDeleteClick
             * @returns {void}
            */
            function OnDeleteClick(primaryControl, selectedItems) {
                if (confirm("Are you sure to delete the claim response header?")) {
                    Xrm.WebApi.online.deleteRecord("ipg_claimresponseheader", Intake.Utility.removeCurlyBraces(selectedItems[0].Id)).then(function success(result) {
                        var formContext = primaryControl;
                        window["previousSelectedItemId"] = '';
                        formContext.getControl("ClaimResponseHeaders").refresh();
                        formContext.getControl("PaymentsSubgrid").refresh();
                        formContext.ui.refreshRibbon(true);
                    }, function (error) {
                        console.log(error.message);
                    });
                }
            }
            Ribbon.OnDeleteClick = OnDeleteClick;
            /**
             * Enables\Disables Delete button
             * @function Intake.Claim.Ribbon.DeleteButtonEnabled
             * @returns {boolean}
            */
            var isDeleteButtonEnabled = false;
            var isCheckManualBatchOperationCompleted = false;
            function DeleteButtonEnabled(primaryControl) {
                var formContext = primaryControl;
                if (isCheckManualBatchOperationCompleted) {
                    isCheckManualBatchOperationCompleted = false;
                    return isDeleteButtonEnabled;
                }
                Xrm.WebApi.retrieveRecord("ipg_claimresponsebatch", formContext.data.entity.getId(), "?$select=ipg_ismanualbatch").then(function success(result) {
                    isCheckManualBatchOperationCompleted = true;
                    isDeleteButtonEnabled = result.ipg_ismanualbatch;
                    if (isDeleteButtonEnabled) {
                        formContext.ui.refreshRibbon(true);
                    }
                });
                return false;
            }
            Ribbon.DeleteButtonEnabled = DeleteButtonEnabled;
            /**
            * Called on GeneratePaperClaim button click
            * @function Intake.Claim.Ribbon.OnGeneratePaperClaimClick
            * @returns {void}
           */
            function OnGeneratePaperClaimClick(primaryControl) {
                var isPrimaryClaim;
                var carrierId;
                var primaryCarrierBalance = primaryControl.getAttribute("ipg_actualcarrierresponsibility").getValue();
                var secondaryCarrierBalance = primaryControl.getAttribute("ipg_actualsecondarycarrierresponsibility").getValue();
                if (primaryCarrierBalance > 0) {
                    isPrimaryClaim = true;
                    carrierId = primaryControl.getAttribute("ipg_carrierid");
                }
                else if (secondaryCarrierBalance > 0) {
                    isPrimaryClaim = false;
                    carrierId = primaryControl.getAttribute("ipg_secondarycarrierid");
                }
                else {
                    console.log("Claim cannot be generated since there is no balance on Primary or Secondary Carrier");
                    Xrm.Navigation.openAlertDialog({ text: "Claim cannot be generated since there is no balance on Primary or Secondary Carrier" });
                    return;
                }
                if (carrierId == null) {
                    console.log("Carrier Name not Selected");
                    Xrm.Navigation.openAlertDialog({ text: "Carrier Name not Selected" });
                    return;
                }
                else {
                    carrierId = carrierId.getValue()[0].id.replace("{", "").replace("}", "");
                }
                var fetch = "<fetch top='1' distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>\n                      <entity name='invoice' >\n                        <attribute name='name'/>\n                        <attribute name='customerid'/>\n                        <attribute name='invoiceid'/>\n                        <attribute name='createdon'/>\n                        <attribute name='ipg_icn'/>\n                        <order descending='true' attribute='createdon' />\n                        <link-entity name='account' alias='carrier' link-type='inner' to='customerid' from='accountid'>\n                           <attribute name='ipg_claimtype'/>\n                           <filter type='and'>\n                             <condition attribute='accountid' value='" + carrierId + "' operator='eq'/>\n                           </filter>\n                        </link-entity>\n                      </entity>\n                   </fetch>";
                Xrm.WebApi.retrieveMultipleRecords("invoice", "?fetchXml=" + fetch).then(function success(invoices) {
                    return __awaiter(this, void 0, void 0, function () {
                        var entityType_1, claimGenerationParametersId_1, entityFormOptions, formParameters, caseId_1, _a, _b, _c;
                        return __generator(this, function (_d) {
                            switch (_d.label) {
                                case 0:
                                    if (!(invoices != null && invoices.entities.length > 0)) return [3 /*break*/, 2];
                                    entityFormOptions = {};
                                    entityFormOptions["entityName"] = "invoice";
                                    entityFormOptions["useQuickCreateForm"] = true;
                                    entityFormOptions["formId"] = "7DA8C0DE-5AA3-4F61-8784-7DF56431132A";
                                    formParameters = {};
                                    caseId_1 = primaryControl.data.entity.getId();
                                    formParameters["ipg_caseid"] = primaryControl.data.entity.getEntityReference();
                                    formParameters["ipg_isprimaryorsecondaryclaim"] = isPrimaryClaim;
                                    _a = formParameters;
                                    _b = "customerid";
                                    _c = {
                                        entityType: "account"
                                    };
                                    return [4 /*yield*/, Xrm.WebApi.retrieveRecord('account', carrierId, "?$select=name")];
                                case 1:
                                    _a[_b] = (_c.name = (_d.sent()).name,
                                        _c.id = carrierId,
                                        _c);
                                    formParameters["ipg_claimtypecode"] = invoices.entities[0]["carrier.ipg_claimtype"];
                                    formParameters["name"] = invoices.entities[0].name;
                                    formParameters["ipg_icn"] = invoices.entities[0].ipg_icn;
                                    // Open the form.
                                    Xrm.Navigation.openForm(entityFormOptions, formParameters)
                                        .then(function (result) {
                                        if (result != null && result.savedEntityReference.length == 1) {
                                            entityType_1 = result.savedEntityReference[0].entityType;
                                            claimGenerationParametersId_1 = result.savedEntityReference[0].id;
                                            return Xrm.WebApi.retrieveRecord(entityType_1, claimGenerationParametersId_1);
                                        }
                                    })
                                        .then(function (response) {
                                        if (response) {
                                            var target = { entityType: "incident", id: caseId_1 };
                                            var reqObject = {
                                                entity: target,
                                                IsPrimaryOrSecondaryClaim: response.ipg_isprimaryorsecondaryclaim,
                                                GenerateClaimFlag: true,
                                                GeneratePdfFlag: true,
                                                Icn: response.ipg_icn != null ? response.ipg_icn : "",
                                                Box32: response.ipg_box32 != null ? response.ipg_box32.toString() : "",
                                                Reason: response.ipg_reason != null ? response.ipg_reason : "",
                                                IsReplacementClaim: response.ipg_claimreplacement === ipg_claimreplacement.Replacement,
                                                ManualClaim: true,
                                                getMetadata: function () {
                                                    return {
                                                        boundParameter: "entity",
                                                        operationType: 0,
                                                        operationName: "ipg_IPGCaseActionsCreateClaim",
                                                        parameterTypes: {
                                                            "entity": {
                                                                typeName: "mscrm.incident",
                                                                structuralProperty: 5
                                                            }, "IsPrimaryOrSecondaryClaim": {
                                                                typeName: "Edm.Boolean",
                                                                structuralProperty: 1
                                                            }, "GenerateClaimFlag": {
                                                                typeName: "Edm.Boolean",
                                                                structuralProperty: 1
                                                            }, "GeneratePdfFlag": {
                                                                typeName: "Edm.Boolean",
                                                                structuralProperty: 1
                                                            }, "Icn": {
                                                                typeName: "Edm.String",
                                                                structuralProperty: 1
                                                            }, "Box32": {
                                                                typeName: "Edm.String",
                                                                structuralProperty: 1
                                                            }, "Reason": {
                                                                typeName: "Edm.String",
                                                                structuralProperty: 1
                                                            }, "IsReplacementClaim": {
                                                                typeName: "Edm.Boolean",
                                                                structuralProperty: 1
                                                            }, "ManualClaim": {
                                                                typeName: "Edm.Boolean",
                                                                structuralProperty: 1
                                                            }
                                                        }
                                                    };
                                                }
                                            };
                                            Xrm.Utility.showProgressIndicator("Generating Claim...");
                                            Xrm.WebApi.online.execute(reqObject)
                                                .then(function (response) {
                                                Xrm.Utility.closeProgressIndicator();
                                                if (response.ok) {
                                                    Xrm.WebApi.deleteRecord(entityType_1, claimGenerationParametersId_1);
                                                    return response.json();
                                                }
                                                else {
                                                    Xrm.Navigation.openAlertDialog({ text: response.statusText });
                                                    return;
                                                }
                                            }, function (error) {
                                                Xrm.Utility.closeProgressIndicator();
                                                console.log(error.message);
                                                Xrm.Navigation.openAlertDialog({ text: error.message });
                                            })
                                                .then(function (result) {
                                                if (result.HasErrors == true) {
                                                    Xrm.Navigation.openAlertDialog({ text: result.Message });
                                                }
                                                else {
                                                    console.log(result.PdfFileBase64);
                                                    var blob = b64toBlob(result.PdfFileBase64, "application/pdf");
                                                    var blobUrl = URL.createObjectURL(blob);
                                                    window.open(blobUrl, "_blank");
                                                }
                                            });
                                        }
                                    }),
                                        (function (error) {
                                            console.log(error);
                                            Xrm.Navigation.openAlertDialog({ text: error.message });
                                        });
                                    _d.label = 2;
                                case 2: return [2 /*return*/];
                            }
                        });
                    });
                }, function (error) {
                    console.log(error.message);
                });
            }
            Ribbon.OnGeneratePaperClaimClick = OnGeneratePaperClaimClick;
            function b64toBlob(b64Data, contentType, sliceSize) {
                if (sliceSize === void 0) { sliceSize = 512; }
                var byteCharacters = atob(b64Data);
                var byteArrays = [];
                for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
                    var slice = byteCharacters.slice(offset, offset + sliceSize);
                    var byteNumbers = new Array(slice.length);
                    for (var i = 0; i < slice.length; i++) {
                        byteNumbers[i] = slice.charCodeAt(i);
                    }
                    var byteArray = new Uint8Array(byteNumbers);
                    byteArrays.push(byteArray);
                }
                var blob = new Blob(byteArrays, { type: contentType });
                return blob;
            }
            /**
               * @function Intake.Claim.Ribbon.onClaimToGenerateFieldChange
             * @returns {void}
            */
            function onClaimToGenerateFieldChange(executionContext) {
                var formContext = executionContext.getFormContext();
                var displayControl = formContext.getAttribute("ipg_claimreplacement").getValue() === ipg_claimreplacement.Replacement;
                formContext.getControl("name").setVisible(displayControl);
                formContext.getControl("ipg_icn").setVisible(displayControl);
            }
            Ribbon.onClaimToGenerateFieldChange = onClaimToGenerateFieldChange;
            /**
             * Enables\Disables Generate Paper Claim button
             * @function Intake.Claim.Ribbon.showGeneratePaperClaimButton
             * @returns {boolean}
            */
            var isGeneratePaperClaimButtonEnabled = false;
            function ShowGeneratePaperClaimButton(primaryControl) {
                return __awaiter(this, void 0, void 0, function () {
                    var formContext, caseState, isCaseLocked, caseId;
                    return __generator(this, function (_a) {
                        formContext = primaryControl;
                        if (window["isAsyncOperationCompleted"])
                            return [2 /*return*/, isGeneratePaperClaimButtonEnabled];
                        caseState = formContext.getAttribute("ipg_statecode").getValue();
                        isCaseLocked = formContext.getAttribute("ipg_islocked").getValue();
                        caseId = formContext.data.entity.getId().replace("{", "").replace("}", "").toLowerCase();
                        Xrm.WebApi.retrieveMultipleRecords("invoice", "?$select=invoiceid&$filter=_ipg_caseid_value eq " + caseId).then(function success(results) {
                            if (results.entities.length > 0) {
                                window["isAsyncOperationCompleted"] = true;
                                isGeneratePaperClaimButtonEnabled = (caseState == CaseStatesEnum.CarrierCollections || caseState == CaseStatesEnum.PatientCollections) && isCaseLocked;
                                if (isGeneratePaperClaimButtonEnabled) {
                                    formContext.ui.refreshRibbon(true);
                                }
                            }
                        }, function (error) {
                            console.log(error.message);
                        });
                        return [2 /*return*/];
                    });
                });
            }
            Ribbon.ShowGeneratePaperClaimButton = ShowGeneratePaperClaimButton;
            /**
             * Called on Unable To Post button click
             * @function Intake.Claim.Ribbon.onUnableToPostClick
             * @returns {void}
            */
            function onUnableToPostClick(primaryControl, selectedItems) {
                var formContext = primaryControl;
                var data = {
                    ipg_poststatus: "unable to post"
                };
                Xrm.WebApi.updateRecord("ipg_claimresponseheader", selectedItems[0].Id, data).then(function success(result) {
                    window["previousSelectedItemId"] = '';
                    formContext.getControl("ClaimResponseHeaders").refresh();
                    formContext.getControl("PaymentsSubgrid").refresh();
                    formContext.ui.refreshRibbon(true);
                    var notetext = prompt("Enter a note");
                    if (notetext) {
                        var entity = {
                            subject: "Unable to post",
                            notetext: notetext,
                            "objectid_ipg_claimresponseheader@odata.bind": "/ipg_claimresponseheaders(" + selectedItems[0].Id + ")"
                        };
                        Xrm.WebApi.createRecord("annotation", entity).then(function success(result) {
                        }, function (error) {
                            Xrm.Navigation.openErrorDialog(error);
                        });
                    }
                }, function (error) {
                    console.log(error.message);
                });
            }
            Ribbon.onUnableToPostClick = onUnableToPostClick;
            /**
             * Called on Enable To Post button click
             * @function Intake.Claim.Ribbon.onEnableToPostClick
             * @returns {void}
            */
            function onEnableToPostClick(primaryControl, selectedItems) {
                var formContext = primaryControl;
                var data = {
                    ipg_poststatus: "new"
                };
                Xrm.WebApi.updateRecord("ipg_claimresponseheader", selectedItems[0].Id, data).then(function success(result) {
                    window["previousSelectedItemId"] = '';
                    formContext.getControl("ClaimResponseHeaders").refresh();
                    formContext.getControl("PaymentsSubgrid").refresh();
                    formContext.ui.refreshRibbon(true);
                }, function (error) {
                    console.log(error.message);
                });
            }
            Ribbon.onEnableToPostClick = onEnableToPostClick;
        })(Ribbon = Claim.Ribbon || (Claim.Ribbon = {}));
    })(Claim = Intake.Claim || (Intake.Claim = {}));
})(Intake || (Intake = {}));
