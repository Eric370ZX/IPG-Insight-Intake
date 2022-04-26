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
 * @namespace Intake.Order
 */
var Intake;
(function (Intake) {
    var Order;
    (function (Order) {
        var _utility = Xrm.Utility;
        var _clientUrl = Xrm.Page.context.getClientUrl();
        var _webApiVersion = 'v9.1';
        var _entityname = 'salesorder';
        var customActions = {
            manualPOCommunication: "ipg_ManualPOCommunication"
        };
        var statecode;
        (function (statecode) {
            statecode[statecode["Closed"] = 3] = "Closed";
        })(statecode = Order.statecode || (Order.statecode = {}));
        var statuscode;
        (function (statuscode) {
            statuscode[statuscode["InvoiceReceived"] = 923720006] = "InvoiceReceived";
            statuscode[statuscode["InReview"] = 923720007] = "InReview";
            statuscode[statuscode["VerifiedForPayment"] = 923720008] = "VerifiedForPayment";
            statuscode[statuscode["Partial"] = 923720010] = "Partial";
        })(statuscode = Order.statuscode || (Order.statuscode = {}));
        var ipg_commstatus;
        (function (ipg_commstatus) {
            ipg_commstatus[ipg_commstatus["Override"] = 427880000] = "Override";
            ipg_commstatus[ipg_commstatus["ReadyToCommunicate"] = 923720003] = "ReadyToCommunicate";
        })(ipg_commstatus = Order.ipg_commstatus || (Order.ipg_commstatus = {}));
        var ipg_potypecode;
        (function (ipg_potypecode) {
            ipg_potypecode[ipg_potypecode["TPO"] = 923720000] = "TPO";
            ipg_potypecode[ipg_potypecode["ZPO"] = 923720001] = "ZPO";
            ipg_potypecode[ipg_potypecode["CPA"] = 923720002] = "CPA";
            ipg_potypecode[ipg_potypecode["MPO"] = 923720004] = "MPO";
        })(ipg_potypecode = Order.ipg_potypecode || (Order.ipg_potypecode = {}));
        /**
         * Called on load form
         * @function Intake.Order.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            RemoveUnusedPOTypes(formContext);
            manufacturerVisibility(executionContext);
            formContext.data.addOnLoad(function () { return LockOrder(formContext); });
            LockOrder(formContext);
        }
        Order.OnLoadForm = OnLoadForm;
        /**
          * Called on change Invoice Subtotal
          * @function Intake.Order.OnChangeInvoiceSubtotal
          * @returns {void}
        */
        function OnChangeInvoiceSubtotal(executionContext) {
            var formContext = executionContext.getFormContext();
            calculateInvoiceTotal(formContext);
        }
        Order.OnChangeInvoiceSubtotal = OnChangeInvoiceSubtotal;
        /**
          * Called on change Invoice Tax
          * @function Intake.Order.OnChangeInvoiceTax
          * @returns {void}
        */
        function OnChangeInvoiceTax(executionContext) {
            var formContext = executionContext.getFormContext();
            calculateInvoiceTotal(formContext);
        }
        Order.OnChangeInvoiceTax = OnChangeInvoiceTax;
        /**
          * Called on change Invoice Shipping
          * @function Intake.Order.OnChangeInvoiceShipping
          * @returns {void}
        */
        function OnChangeInvoiceShipping(executionContext) {
            var formContext = executionContext.getFormContext();
            calculateInvoiceTotal(formContext);
        }
        Order.OnChangeInvoiceShipping = OnChangeInvoiceShipping;
        /**
          * Called on change Additional Invoice Total
          * @function Intake.Order.OnChangeAdditionalInvoiceTotal
          * @returns {void}
        */
        function OnChangeAdditionalInvoiceTotal(executionContext) {
            var formContext = executionContext.getFormContext();
            calculateInvoiceTotal(formContext);
        }
        Order.OnChangeAdditionalInvoiceTotal = OnChangeAdditionalInvoiceTotal;
        /**
          * Called on change Invoice Total
          * @function Intake.Order.OnChangeInvoiceTotal
          * @returns {void}
        */
        function OnChangeInvoiceTotal(executionContext) {
            var formContext = executionContext.getFormContext();
            calculateInvoiceVariance(formContext);
        }
        Order.OnChangeInvoiceTotal = OnChangeInvoiceTotal;
        function OnChangeIsExcludedFromExceptionDashboard(executionContext) {
            var formContext = executionContext.getFormContext();
            changePOCommStatus(formContext);
        }
        Order.OnChangeIsExcludedFromExceptionDashboard = OnChangeIsExcludedFromExceptionDashboard;
        /**
          * calculate Invoice Total
          * @function Intake.Order.calculateInvoiceTotal
          * @returns {void}
        */
        function calculateInvoiceTotal(formContext) {
            var invoiceSubtotal = formContext.getAttribute("ipg_invoicesubtotal").getValue();
            var invoiceShipping = formContext.getAttribute("ipg_invoiceshipping").getValue();
            var invoiceTax = formContext.getAttribute("ipg_invoicetax").getValue();
            var invoiceAdditionalTotal = formContext.getAttribute("ipg_additionalinvoicetotal").getValue();
            var invoiceTotal = (isNaN(invoiceSubtotal) ? 0 : invoiceSubtotal)
                + (isNaN(invoiceShipping) ? 0 : invoiceShipping)
                + (isNaN(invoiceTax) ? 0 : invoiceTax)
                + (isNaN(invoiceAdditionalTotal) ? 0 : invoiceAdditionalTotal);
            formContext.getAttribute("ipg_invoicetotal").setValue(invoiceTotal);
            formContext.getAttribute("ipg_invoicetotal").fireOnChange();
        }
        /**
          * calculate Invoice Variance
          * @function Intake.Order.calculateInvoiceVariance
          * @returns {void}
        */
        function calculateInvoiceVariance(formContext) {
            var invoiceTotal = formContext.getAttribute("ipg_invoicetotal").getValue();
            var orderTotal = formContext.getAttribute("totalamount").getValue();
            var invoiceVariance = (isNaN(orderTotal) ? 0 : orderTotal) - (isNaN(invoiceTotal) ? 0 : invoiceTotal);
            formContext.getAttribute("ipg_invoicevariance").setValue(invoiceVariance);
        }
        /**
        * Removes unused PO types
        * @function Intake.Order.RemoveUnusedPOTypes
        * @returns {void}
        */
        function RemoveUnusedPOTypes(formContext) {
            var currentPOType = Number(formContext.getAttribute("ipg_potypecode").getValue());
            if (currentPOType != 923720003)
                formContext.getControl("ipg_potypecode").removeOption(923720003);
        }
        function CloseOrphans(entityIds, selectedControl) {
            if (selectedControl && entityIds && entityIds.length > 0) {
                _utility.showProgressIndicator(null);
                Promise.all(entityIds.map(function (id) {
                    return fetch(_clientUrl + "/api/data/" + _webApiVersion + "/" + _entityname + "s(" + id + ")", {
                        method: 'PATCH',
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json; charset=utf-8",
                            "OData-MaxVersion": "4.0",
                            "OData-Version": "4.0"
                        },
                        body: JSON.stringify({
                            statecode: 3,
                            statuscode: 923720013,
                            ipg_downloadedfromportal: false,
                        })
                    });
                }))
                    .then(function (r) {
                    console.log(r);
                    selectedControl.refresh();
                    _utility.closeProgressIndicator();
                }).catch(function (error) {
                    console.log(error);
                    selectedControl.refresh();
                    _utility.closeProgressIndicator();
                });
            }
        }
        Order.CloseOrphans = CloseOrphans;
        function isOrphaneView(selectedControl) {
            return selectedControl.getViewSelector
                && selectedControl.getViewSelector().getCurrentView().name.toLowerCase().indexOf('orphan') > -1;
        }
        Order.isOrphaneView = isOrphaneView;
        /**
          * Checks if PO is available for revision
          * @function Intake.Order.IsRevisionAvailable
          * @returns {boolean}
        */
        function IsRevisionAvailable(selectedItems) {
            return __awaiter(this, void 0, void 0, function () {
                var order;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord("salesorder", selectedItems[0].Id, "?$select=statecode,statuscode,ipg_isestimatedpo")];
                        case 1:
                            order = _a.sent();
                            return [2 /*return*/, (((order.statecode != 3) || (order.statuscode != 923720004)) && !order.ipg_isestimatedpo)];
                    }
                });
            });
        }
        Order.IsRevisionAvailable = IsRevisionAvailable;
        /**
          * Checks if PO is available for Void operation
          * @function Intake.Order.IsVoidAvailable
          * @returns {boolean}
        */
        function IsVoidAvailable(selectedItems) {
            return __awaiter(this, void 0, void 0, function () {
                var order;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord("salesorder", selectedItems[0].Id, "?$select=statecode,statuscode,ipg_isestimatedpo")];
                        case 1:
                            order = _a.sent();
                            return [2 /*return*/, ((order.statecode != 3) && !order.ipg_isestimatedpo)];
                    }
                });
            });
        }
        Order.IsVoidAvailable = IsVoidAvailable;
        function GetCustomerId(formContext) {
            var Id = formContext.getAttribute("ipg_customerlookupid").getValue();
            var name = formContext.getAttribute("ipg_customerlookupname").getValue();
            if (Id) {
                var object = new Array();
                object[0] = new Object();
                object[0].id = Id;
                object[0].name = name;
                object[0].entityType = "salesorder";
                formContext.getAttribute("customerid").setValue(null);
                formContext.getAttribute("customerid").setValue(object);
            }
        }
        Order.GetCustomerId = GetCustomerId;
        function SetCustomerId(primaryControl) {
            var formContext = primaryControl;
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "salesorder";
            var customerId = formContext.getAttribute("customerid").getValue();
            var formParameters = {};
            formParameters["ipg_customerlookupid"] = customerId[0].id;
            formParameters["ipg_customerlookupname"] = customerId[0].name;
            Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
                console.log(success);
            }, function (error) {
                console.log(error);
            });
        }
        Order.SetCustomerId = SetCustomerId;
        /**
          * Opens a revised PO form
          * @function Intake.Order.OpenPOForRevision
          * @returns {void}
        */
        function OpenPOForRevision(selectedItems) {
            if (confirm("The selected PO will be moved to the history section. Do you want to proceed?")) {
                Xrm.Utility.showProgressIndicator("Please wait");
                var request = {
                    "Record": {
                        "@odata.type": "Microsoft.Dynamics.CRM.salesorder",
                        "salesorderid": selectedItems[0].Id
                    },
                    getMetadata: function () {
                        return {
                            boundParamter: null,
                            parameterTypes: {
                                "Record": {
                                    "typeName": "mscrm.crmbaseentity",
                                    "structuralProperty": 5
                                }
                            },
                            operationType: 0,
                            operationName: "ipg_IPGIntakeActionsCloneRecord"
                        };
                    }
                };
                return Xrm.WebApi.online.execute(request)
                    .then(function (response) {
                    Xrm.Utility.closeProgressIndicator();
                    if (response.ok) {
                        var entity = { ipg_isrevised: true, ipg_revisiondate: new Date().toISOString() };
                        Xrm.WebApi.updateRecord("salesorder", selectedItems[0].Id, entity).then(function success(result) {
                            response.json().then(function (data) {
                                var entityFormOptions = { entityName: "salesorder", entityId: data.salesorderid };
                                Xrm.Navigation.openForm(entityFormOptions);
                            });
                        }, function (error) {
                            Xrm.Utility.alertDialog(error.message, null);
                        });
                    }
                    else {
                        Xrm.Navigation.openErrorDialog({ message: response.statusText });
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
        Order.OpenPOForRevision = OpenPOForRevision;
        /**
          * Voids PO
          * @function Intake.Order.VoidPO
          * @returns {void}
        */
        function VoidPO(primaryControl, selectedItems) {
            if (confirm("The selected PO will be voided. Do you want to proceed?")) {
                var formContext_1 = primaryControl;
                Xrm.Utility.showProgressIndicator("Please wait");
                var request = {
                    entity: {
                        entityType: "salesorder",
                        id: selectedItems[0].Id
                    },
                    getMetadata: function () {
                        return {
                            boundParameter: "entity",
                            parameterTypes: {
                                entity: {
                                    typeName: "mscrm.salesorder",
                                    structuralProperty: 5
                                }
                            },
                            operationType: 0,
                            operationName: "ipg_IPGIntakeActionsVoidPO"
                        };
                    }
                };
                return Xrm.WebApi.online.execute(request)
                    .then(function (response) {
                    Xrm.Utility.closeProgressIndicator();
                    if (response.ok) {
                        var gridControl = formContext_1.getControl("PurchaseOrders");
                        gridControl.refresh();
                    }
                    else {
                        Xrm.Navigation.openErrorDialog({ message: response.statusText });
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
        Order.VoidPO = VoidPO;
        function LockOrder(formContext) {
            var _a;
            var statusReasoncode = (_a = formContext.getAttribute("statuscode")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (statusReasoncode === statuscode.InReview ||
                statusReasoncode === statuscode.InvoiceReceived ||
                statusReasoncode === statuscode.Partial ||
                statusReasoncode === statuscode.VerifiedForPayment) {
                formContext.ui.controls.forEach(function (control) {
                    if (control && control.getDisabled && !control.getDisabled()) {
                        control.setDisabled(true);
                    }
                });
            }
        }
        function changePOCommStatus(formContext) {
            var _a;
            var ignore = (_a = formContext.getAttribute("ipg_isexcludedfromexceptionsdashboard")) === null || _a === void 0 ? void 0 : _a.getValue();
            var commStatus = formContext.getAttribute("ipg_commstatus");
            if (ignore) {
                commStatus === null || commStatus === void 0 ? void 0 : commStatus.setValue(ipg_commstatus.Override);
            }
            else {
                commStatus === null || commStatus === void 0 ? void 0 : commStatus.setValue(ipg_commstatus.ReadyToCommunicate);
            }
        }
        function manufacturerVisibility(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            var manufacturer = formContext.getControl("ipg_manufacturer_id");
            var poType = (_a = formContext.getAttribute("ipg_potypecode")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (manufacturer)
                manufacturer.setVisible(poType !== ipg_potypecode.CPA);
        }
        Order.manufacturerVisibility = manufacturerVisibility;
        function OnClickCommunicatePORibbon(primaryContext) {
            var _a;
            var formContext = primaryContext;
            Xrm.Utility.showProgressIndicator("Please wait");
            var manualPOComunicationRequest = {
                OrderId: (_a = formContext.data.entity.getId()) === null || _a === void 0 ? void 0 : _a.replace("{", "").replace("}", ""),
                getMetadata: function () {
                    return {
                        boundParameter: null,
                        parameterTypes: {
                            "OrderId": {
                                "typeName": "Edm.String",
                                "structuralProperty": 1
                            },
                        },
                        operationType: 0,
                        operationName: customActions.manualPOCommunication
                    };
                }
            };
            Xrm.WebApi.online.execute(manualPOComunicationRequest).then(function (result) {
                if (result.ok) {
                    result.json().then(function (data) {
                        if (data.Success) {
                            Xrm.Utility.closeProgressIndicator();
                            formContext.data.refresh(true).then(function () { return formContext.ui.refreshRibbon(); });
                            Xrm.Utility.alertDialog(data.Message, null);
                        }
                        else {
                            Xrm.Utility.closeProgressIndicator();
                            Xrm.Utility.alertDialog(data.Message, null);
                        }
                    });
                }
            }, function (error) {
                Xrm.Utility.closeProgressIndicator();
                Xrm.Utility.alertDialog(error.message, null);
            });
        }
        Order.OnClickCommunicatePORibbon = OnClickCommunicatePORibbon;
        function IsEnableCommunicatePORibbon(primaryControl) {
            var formContext = primaryControl;
            var commStatus = formContext.getAttribute("ipg_commstatus").getValue();
            return commStatus == ipg_commstatus.ReadyToCommunicate;
        }
        Order.IsEnableCommunicatePORibbon = IsEnableCommunicatePORibbon;
    })(Order = Intake.Order || (Intake.Order = {}));
})(Intake || (Intake = {}));
