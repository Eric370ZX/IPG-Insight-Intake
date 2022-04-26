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
 * @namespace Intake.CasePartDetail
 */
var Intake;
(function (Intake) {
    var CasePartDetail;
    (function (CasePartDetail) {
        var order_statecode;
        (function (order_statecode) {
            order_statecode[order_statecode["Closed"] = 3] = "Closed";
        })(order_statecode = CasePartDetail.order_statecode || (CasePartDetail.order_statecode = {}));
        var order_statuscode;
        (function (order_statuscode) {
            order_statuscode[order_statuscode["InvoiceReceived"] = 923720006] = "InvoiceReceived";
            order_statuscode[order_statuscode["InReview"] = 923720007] = "InReview";
            order_statuscode[order_statuscode["VerifiedForPayment"] = 923720008] = "VerifiedForPayment";
            order_statuscode[order_statuscode["Partial"] = 923720010] = "Partial";
        })(order_statuscode = CasePartDetail.order_statuscode || (CasePartDetail.order_statuscode = {}));
        /**
         * Called on load quick create form
         * @function Intake.CasePartDetail.OnLoadQuickCreateForm
         * @returns {void}
        */
        function OnLoadQuickCreateForm(executionContext) {
            var formContext = executionContext.getFormContext();
            RunEBVOnCreate(formContext);
            if (formContext.ui.getFormType() === 1) {
                formContext.getAttribute("ipg_quantity").setValue(1);
                formContext.getAttribute("ipg_quantity").setRequiredLevel("required");
            }
            SetAvailablePOTypes(formContext);
            var potypeAttr = formContext.getAttribute("ipg_potypecode");
            potypeAttr === null || potypeAttr === void 0 ? void 0 : potypeAttr.addOnChange(OnPoTypeChange);
            potypeAttr === null || potypeAttr === void 0 ? void 0 : potypeAttr.fireOnChange();
        }
        CasePartDetail.OnLoadQuickCreateForm = OnLoadQuickCreateForm;
        /**
         * Called on load form
         * @function Intake.CasePartDetail.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            RunEBVOnCreate(formContext);
            formContext.getAttribute("ipg_quantity").setRequiredLevel("required");
            SetAvailablePOTypes(formContext);
            var potypeAttr = formContext.getAttribute("ipg_potypecode");
            potypeAttr === null || potypeAttr === void 0 ? void 0 : potypeAttr.addOnChange(OnPoTypeChange);
            potypeAttr === null || potypeAttr === void 0 ? void 0 : potypeAttr.fireOnChange();
            ShowNotificationIfEnteredUnitCostGreaterThanMSRP(formContext);
            SetPOTypeIfConvertedPart(formContext);
            LockActualPart(formContext);
            formContext.getAttribute("ipg_quantity").fireOnChange();
        }
        CasePartDetail.OnLoadForm = OnLoadForm;
        /**
          * Called on change EnteredUnitCost
          * @function Intake.CasePartDetail.OnChangeEnteredUnitCost
          * @returns {void}
        */
        function OnChangeEnteredUnitCost(executionContext) {
            var formContext = executionContext.getFormContext();
            ShowNotificationIfEnteredUnitCostGreaterThanMSRP(formContext);
        }
        CasePartDetail.OnChangeEnteredUnitCost = OnChangeEnteredUnitCost;
        /**
          * Called on change Quantity Implanted
          * @function Intake.CasePartDetail.OnChangeQuantityImplanted
          * @returns {void}
        */
        function OnChangeQuantityImplanted(executionContext) {
            var formContext = executionContext.getFormContext();
            checkMaxValueOnPart(formContext);
        }
        CasePartDetail.OnChangeQuantityImplanted = OnChangeQuantityImplanted;
        /**
          * Called on change Part Override
          * @function Intake.CasePartDetail.OnChangePartOverride
          * @returns {void}
        */
        function OnChangePartOverride(executionContext) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext;
                return __generator(this, function (_a) {
                    formContext = executionContext.getFormContext();
                    CheckPartStatus(formContext);
                    CheckPartSupported(formContext);
                    CheckPartQuantity(formContext);
                    if (defaultPOTypeOptions) {
                        ResetPOTypeOptions(formContext);
                    }
                    SetAvailablePOTypes(formContext);
                    //await ShowNewPriceListMessage(formContext);
                    IsPartUsed(formContext);
                    return [2 /*return*/];
                });
            });
        }
        CasePartDetail.OnChangePartOverride = OnChangePartOverride;
        /**
          * Called on Form Save event
          * @function Intake.CasePartDetail.OnSaveForm
          * @returns {void}
        */
        function OnSaveForm(executionContext) {
            prohibitUnsupportedParts(executionContext);
        }
        CasePartDetail.OnSaveForm = OnSaveForm;
        /**
           * Called on change Case
          * @function Intake.CasePartDetail.OnChangeCase
          * @returns {void}
        */
        function OnChangeCase(executionContext) {
            var formContext = executionContext.getFormContext();
            if (defaultPOTypeOptions) {
                ResetPOTypeOptions(formContext);
            }
            SetAvailablePOTypes(formContext);
        }
        CasePartDetail.OnChangeCase = OnChangeCase;
        /**
          * Called on Form Save event. Checks if the chosen part is unsupported by the carrier or not
          * based on ipg_unsupportedpart table
          * @function prohibitUnsupportedParts
          * @returns {void}
        */
        var skipUnsupportedPartCheck = false;
        function prohibitUnsupportedParts(executionContext) {
            //debugger;
            if (skipUnsupportedPartCheck) { //we need this check because formContext.data.entity.save triggers this handler again
                skipUnsupportedPartCheck = false;
                return;
            }
            //disable saving before the async calls below because preventDefault does not work from async calls
            var saveEventArgs = executionContext.getEventArgs();
            saveEventArgs.preventDefault();
            var formContext = executionContext.getFormContext();
            var caseLookupAttributeValue = formContext.getAttribute("ipg_caseid").getValue();
            Xrm.WebApi.retrieveRecord(caseLookupAttributeValue[0].entityType, caseLookupAttributeValue[0].id, "?$select=incidentid&$expand=ipg_CarrierId($select=accountid),ipg_HomePlanCarrierId($select=accountid)").then(function (caseEntity) {
                if (!caseEntity.ipg_CarrierId || !caseEntity.ipg_CarrierId.accountid) {
                    Xrm.Navigation.openErrorDialog({ message: 'Error! Could not find Case Primary Carrier and consequently check Unsupported parts' });
                    return;
                }
                var productLookupAttributeValue = formContext.getAttribute("ipg_productid").getValue();
                var todayDateFormatted = Intake.Utility.FormatODataDate(new Date() /*today*/);
                //build filter expression
                var filter = "?$filter=ipg_ProductId/productid eq " + Intake.Utility.removeCurlyBraces(productLookupAttributeValue[0].id) +
                    (" and ipg_effectivedate le " + todayDateFormatted + " and statecode eq 0 and (ipg_CarrierId/accountid eq " + caseEntity.ipg_CarrierId.accountid);
                if (caseEntity.ipg_HomePlanCarrierId && caseEntity.ipg_HomePlanCarrierId.accountid) {
                    filter += " or ipg_CarrierId/accountid eq " + caseEntity.ipg_HomePlanCarrierId.accountid;
                }
                filter += ')';
                //check if unsupported parts exist
                Xrm.WebApi.retrieveMultipleRecords('ipg_unsupportedpart', filter + '&$select=ipg_unsupportedpartid').then(function (unsupportedParts) {
                    var notificationId = 'unsupportedPartError';
                    if (unsupportedParts.entities.length) {
                        formContext.ui.setFormNotification('This part is unsupported by the carrier (primary or home plan)', 'ERROR', notificationId);
                    }
                    else {
                        formContext.ui.clearFormNotification(notificationId);
                        skipUnsupportedPartCheck = true;
                        formContext.data.entity.save();
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ message: error.message });
            });
        }
        /**
          * Calculate Total value
          * @function Intake.CasePartDetail.CalculateTotal
          * @returns {void}
        */
        function CalculateTotal(executionContext) {
            var formContext = executionContext.getFormContext();
            var quantityAttributeValue = formContext.getAttribute("ipg_quantity").getValue();
            var enteredUnitCostAttributeValue = formContext.getAttribute("ipg_enteredunitcost").getValue();
            var enteredTaxAttributeValue = formContext.getAttribute("ipg_enteredtax").getValue();
            var enteredShippingAttributeValue = formContext.getAttribute("ipg_enteredshipping").getValue();
            var total = ((enteredUnitCostAttributeValue || 0) + (enteredTaxAttributeValue || 0) + (enteredShippingAttributeValue || 0)) * (quantityAttributeValue || 0);
            formContext.getAttribute("ipg_totaljscalculated").setValue(total);
        }
        CasePartDetail.CalculateTotal = CalculateTotal;
        /**
          * show notification if EnteredUnitCost greater than MSRP more than 10%
          * @function Intake.CasePartDetail.showNotificationIfEnteredUnitCostGreaterThanMSRP
          * @returns {void}
        */
        function ShowNotificationIfEnteredUnitCostGreaterThanMSRP(formContext) {
            var partLookupAttributeValue = formContext.getAttribute("ipg_productid").getValue();
            if (partLookupAttributeValue) {
                Xrm.WebApi.retrieveRecord(partLookupAttributeValue[0].entityType, partLookupAttributeValue[0].id, "?$select=ipg_msrp").then(function (result) {
                    if (result.ipg_msrp) {
                        var msrp = result.ipg_msrp;
                        var enteredUnitCostAttributeValue = formContext.getAttribute("ipg_enteredunitcost").getValue();
                        if (msrp * 1.1 < enteredUnitCostAttributeValue) {
                            Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "The Entered Unit Cost is greater more than 10%!" });
                        }
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
        /**
          * check max quantity restriction of product
          * @function Intake.CasePartDetail.checkMaxValueOnPart
          * @returns {void}
        */
        function checkMaxValueOnPart(formContext) {
            var partLookupAttributeValue = formContext.getAttribute("ipg_productid").getValue();
            var quantityAttributeValue = formContext.getAttribute("ipg_quantity").getValue();
            if (partLookupAttributeValue) {
                Xrm.WebApi.retrieveRecord(partLookupAttributeValue[0].entityType, partLookupAttributeValue[0].id, "?$select=ipg_enforcemaxquantity,ipg_maxquantity").then(function (product) {
                    if (product.ipg_maxquantity && quantityAttributeValue > product.ipg_maxquantity) {
                        var alertStrings = {
                            confirmButtonLabel: "Ok",
                            text: "Quantity Implanted is exceeding the Max Quantity Per Case value. Max value is: " + product.ipg_maxquantity,
                            title: "Warning"
                        };
                        var alertOptions = { height: 120, width: 260 };
                        if (product.ipg_enforcemaxquantity) {
                            formContext.getAttribute('ipg_quantity').setValue(0);
                            alertStrings.title = "Error";
                        }
                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
        /**
        * Shows warning if part approval status is pending
        * @function Intake.CasePartDetail.CheckPartStatus
        * @returns {void}
        */
        function CheckPartStatus(formContext) {
            var product = formContext.getAttribute("ipg_productid").getValue();
            if (product) {
                Xrm.WebApi.retrieveRecord("product", product[0].id, "?$select=name,ipg_status").then(function success(result) {
                    //when form on Create stage Product status pending - disable PO type and don't show notification
                    if (formContext.ui.getFormType() == 1 && result.ipg_status == 923720001) {
                        formContext.getAttribute("ipg_potypecode").setRequiredLevel("none");
                    }
                    else {
                        if (result.ipg_status == 923720001)
                            formContext.getControl("ipg_productid").setNotification("The part " + result.name + " hasn't been approved yet", "PartNotification");
                        else
                            formContext.getControl("ipg_productid").clearNotification("PartNotification");
                    }
                });
            }
            else
                Xrm.Page.ui.clearFormNotification("PartNotification");
        }
        /**
        * Shows warning if a part is not supported by Carrier
        * @function Intake.CasePartDetail.CheckPartSupported
        * @returns {void}
        */
        function CheckPartSupported(formContext) {
            var product = formContext.getAttribute("ipg_productid").getValue();
            var caseid = formContext.getAttribute("ipg_caseid").getValue();
            formContext.getControl('ipg_productid').clearNotification("unsupportedpart");
            Xrm.WebApi.retrieveRecord("incident", caseid[0].id, "?$select=_ipg_carrierid_value").then(function (result) {
                var carrier = result._ipg_carrierid_value;
                if (product && carrier) {
                    var query = "?$select=ipg_unsupportedpartid&$filter=_ipg_productid_value eq '" + Intake.Utility.removeCurlyBraces(product[0].id) + "' and _ipg_carrierid_value eq '" + carrier + "'";
                    Xrm.WebApi.retrieveMultipleRecords("ipg_unsupportedpart", query)
                        .then(function (result) {
                        if (result.entities.length > 0) {
                            Xrm.WebApi.retrieveRecord("account", carrier, "?$select=name").then(function (result) {
                                formContext.getControl('ipg_productid').setNotification("This part is not supported by carrier " + result.name, "unsupportedpart");
                            }, function (error) {
                                Xrm.Navigation.openErrorDialog({ message: error.message });
                            });
                        }
                    }, function (error) {
                        Xrm.Navigation.openErrorDialog({ message: error.message });
                    });
                }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ message: error.message });
            });
        }
        /**
        * set available PO Types by Surgery Date and DTM Member of Facility
        * @function Intake.CasePartDetail.SetAvailablePOTypes
        * @returns {void}
        */
        var defaultPOTypeOptions = null;
        function SetAvailablePOTypes(formContext) {
            var caseValue = formContext.getAttribute("ipg_caseid").getValue();
            var productValue = formContext.getAttribute("ipg_productid").getValue();
            var poTypepPickList = formContext.getControl("ipg_potypecode");
            if (poTypepPickList)
                defaultPOTypeOptions = poTypepPickList.getOptions();
            if (!productValue && poTypepPickList) {
                poTypepPickList.clearOptions();
                return;
            }
            if (poTypepPickList) {
                var currentPOType = Number(formContext.getAttribute("ipg_potypecode").getValue());
                if (currentPOType != 923720003)
                    poTypepPickList.removeOption(923720003);
            }
            if (caseValue && caseValue.length && caseValue[0] && caseValue[0].id) {
                Xrm.WebApi.retrieveRecord(caseValue[0].entityType, caseValue[0].id, "?$select=ipg_surgerydate&$expand=ipg_FacilityId($select=ipg_dtmmember,ipg_cpaonlyfacility)")
                    .then(function (caseResult) {
                    Xrm.WebApi.retrieveRecord(productValue[0].entityType, productValue[0].id, "?$select=ipg_boxquantity")
                        .then(function (productResult) {
                        var excludedOptions = null;
                        if (caseResult) {
                            var dateNow = new Date();
                            var currentDate = new Date(dateNow.getFullYear(), dateNow.getMonth(), dateNow.getDate());
                            if (caseResult.ipg_surgerydate && new Date(caseResult.ipg_surgerydate) < currentDate) {
                                if (caseResult.ipg_FacilityId && caseResult.ipg_FacilityId.ipg_cpaonlyfacility) {
                                    excludedOptions = [923720000, 923720001, 923720003, 923720004];
                                }
                                else if (productResult && productResult.ipg_boxquantity > 1) {
                                    excludedOptions = [923720000, 923720001, 923720004];
                                }
                                else {
                                    if (caseResult.ipg_FacilityId && caseResult.ipg_FacilityId.ipg_dtmmember) {
                                        excludedOptions = [923720000];
                                    }
                                    else {
                                        excludedOptions = [923720000, 923720001, 923720003, 923720004];
                                    }
                                }
                            }
                            else {
                                excludedOptions = [923720001, 923720002, 923720003, 923720004];
                            }
                            if (poTypepPickList && excludedOptions) {
                                var oldvalue = poTypepPickList.getAttribute().getValue();
                                excludedOptions.forEach(function (item) {
                                    item != oldvalue && poTypepPickList.removeOption(item);
                                });
                            }
                        }
                    });
                });
            }
        }
        function OnPoTypeChange(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            var poTypepPickListVal = (_a = formContext.getAttribute("ipg_potypecode")) === null || _a === void 0 ? void 0 : _a.getValue();
            var controlledFields = ["ipg_enteredunitcost", "ipg_enteredtax", "ipg_enteredshipping"];
            var disable = !poTypepPickListVal || [923720000, 923720004, 923720001].indexOf(poTypepPickListVal) > -1; /**TPO,MPO,ZPO */
            controlledFields.forEach(function (fieldname) { var _a; return (_a = formContext.getControl(fieldname)) === null || _a === void 0 ? void 0 : _a.setDisabled(disable); });
        }
        /**
        * reset PO Type Options
        * @function Intake.CasePartDetail.ResetPOTypeOptions
        * @returns {void}
        */
        function ResetPOTypeOptions(formContext) {
            var poTypepPickList = formContext.getControl("ipg_potypecode");
            poTypepPickList.clearOptions();
            defaultPOTypeOptions.forEach(function (item) {
                poTypepPickList.addOption(item);
            });
        }
        /**
        * Checks if max quantity limit reached
        * @function Intake.CasePartDetail.CheckPartQuantity
        * @returns {void}
        */
        function CheckPartQuantity(formContext) {
            var product = formContext.getAttribute("ipg_productid").getValue();
            var caseid = formContext.getAttribute("ipg_caseid").getValue();
            formContext.getControl('ipg_productid').clearNotification("quantity");
            formContext.ui.clearFormNotification("quantityAllowed");
            if (product) {
                Xrm.WebApi.retrieveRecord("product", product[0].id, "?$select=ipg_enforcemaxquantity,ipg_maxquantity").then(function (result) {
                    if ((result.ipg_enforcemaxquantity) && ((result.ipg_maxquantity == 0) || (result.ipg_maxquantity == null)))
                        formContext.getControl('ipg_productid').setNotification("You can't add this part because max quanity per case is set to 0", "quantity");
                    else if ((result.ipg_enforcemaxquantity) && (result.ipg_maxquantity > 0)) {
                        var maxQuantity_1 = result.ipg_maxquantity;
                        Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=ipg_caseid/incidentid eq '" + caseid[0].id + "' and ipg_productid/productid eq " + product[0].id).then(function (result) {
                            if (result.entities.length >= maxQuantity_1)
                                formContext.getControl('ipg_productid').setNotification("The maximum quantity of this part per case was reached", "quantity");
                        }, function (error) {
                            Xrm.Navigation.openErrorDialog({ message: error.message });
                        });
                    }
                    else if ((!result.ipg_enforcemaxquantity) && (result.ipg_maxquantity > 0)) {
                        var maxQuantity_2 = result.ipg_maxquantity;
                        Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=ipg_caseid/incidentid eq '" + caseid[0].id + "' and ipg_productid/productid eq " + product[0].id).then(function (result) {
                            if (result.entities.length >= maxQuantity_2)
                                formContext.ui.setFormNotification("The maximum quantity of this part per case was reached, but you can add this part into the case", "WARNING", "quantityAllowed");
                        }, function (error) {
                            Xrm.Navigation.openErrorDialog({ message: error.message });
                        });
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
        //Commented out because we cannot determine manufacturers for Price List https://eti-ipg.atlassian.net/browse/CPI-3627.
        //This code can be used later when we associate Price List with manufacturers
        ///**
        //* Shows warning if there is a new approved price list
        //* @function Intake.CasePartDetail.ShowNewPriceListMessage
        //* @returns {void}
        //*/
        //async function ShowNewPriceListMessage(formContext: Xrm.FormContext) {
        //  //debugger;
        //  formContext.ui.clearFormNotification("NewPriceList");
        //  let productIdValue = formContext.getAttribute("ipg_productid").getValue();
        //  if (!productIdValue || !productIdValue.length) {
        //    return;
        //  }
        //  //get Global Setting
        //  const settingName: string = 'NEW_PRICE_LIST_MESSAGE_DAYS';
        //  let numberOfDays: number|null = await Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", `?$select=ipg_value&$filter=ipg_name eq '${settingName}'`)
        //    .then((result: Xrm.RetrieveMultipleResult): number|null => {
        //      if (result.entities.length > 0 && result.entities[0].ipg_value) {
        //        return Number(result.entities[0].ipg_value);
        //      }
        //      else {
        //        Xrm.Navigation.openErrorDialog({ message: `${settingName} global setting does not exist or empty` });
        //      }
        //    },
        //      function (error) {
        //        console.log(error.message);
        //      }
        //  );
        //  if (numberOfDays == null || numberOfDays == NaN) {
        //    Xrm.Navigation.openErrorDialog({ message: `Could not parse a number from ${settingName} global setting` });
        //    return;
        //  }
        //  //get Facility
        //  let caseid = formContext.getAttribute("ipg_caseid").getValue();
        //  let facilityId: string = await Xrm.WebApi.retrieveRecord("incident", caseid[0].id, "?$select=_ipg_facilityid_value").then(function (result) {
        //    return result._ipg_facilityid_value;
        //  }, function (error) {
        //    Xrm.Navigation.openErrorDialog({ message: error.message });
        //  });
        //  if (!facilityId) {
        //    return;
        //  }
        //  //get recently approved price lists
        //  //todo: get Manufacturer from the part
        //  //todo: get manufacturers related the price list (how?)
        //  //todo: filter by Manufacturer
        //  const priceListAbbreviation = 'PRL';
        //  const approvedReviewStatusValue: number = 427880001;
        //  let priceLists = await Xrm.WebApi.retrieveMultipleRecords("ipg_document",
        //    `?$filter=(ipg_ipg_document_account/any(f:f/accountid eq '${facilityId}'))` +
        //      ` and ipg_DocumentTypeId/ipg_documenttypeabbreviation eq '${priceListAbbreviation}'` +
        //      ` and ipg_reviewstatus eq ${approvedReviewStatusValue} and Microsoft.Dynamics.CRM.LastXDays(PropertyName=@p1,PropertyValue=@p2)&@p1='ipg_reviewdate'&@p2=${numberOfDays}` +
        //    '&$select=ipg_documentid')
        //    .then((result: Xrm.RetrieveMultipleResult) => {
        //      return result;
        //    },
        //    function (error) {
        //      console.log(error.message);
        //    }
        //  );
        //  if (priceLists.entities.length > 0) {
        //    formContext.ui.setFormNotification('A new Price List is now in effect', 'WARNING', "NewPriceList");
        //  }
        //}
        /**
        * Runs auto EBV Request from system
        * @function Intake.CasePartDetail.RunEBVOnCreate
        * @returns {void}
        */
        function RunEBVOnCreate(formContext) {
            var formType = formContext.ui.getFormType();
            if (formType === 1 /* Create */ || formType === 5 /* QuickCreate */) {
                var arguments_1 = { IsUserGenerated: false, CarrierNumber: 1 };
                var caseIdField = formContext.getAttribute("ipg_caseid");
                var caseIdValue = caseIdField && caseIdField.getValue();
                if ((caseIdValue === null || caseIdValue === void 0 ? void 0 : caseIdValue.length) > 0 && caseIdValue[0].id) {
                    Intake.Utility.CallActionProcessAsync("incidents", Intake.Utility.removeCurlyBraces(caseIdValue[0].id), "ipg_IPGCaseActionsVerifyBenefits", arguments_1);
                }
            }
        }
        function SetPOTypeIfConvertedPart(formContext) {
            var casePartDetailID = Xrm.Page.data.entity.getId();
            if (casePartDetailID) {
                Xrm.WebApi.online.retrieveRecord("ipg_casepartdetail", Intake.Utility.removeCurlyBraces(casePartDetailID), "?$select=ipg_convertedfromestimated").then(function success(result) {
                    var ipg_convertedfromestimated = result["ipg_convertedfromestimated"];
                    if (ipg_convertedfromestimated) {
                        if (formContext.getControl("ipg_potypecode")) {
                            formContext.getControl("ipg_potypecode").setDisabled(true);
                        }
                    }
                }, function (error) {
                    Xrm.Navigation.openAlertDialog({ text: error.message });
                });
            }
        }
        CasePartDetail.SetPOTypeIfConvertedPart = SetPOTypeIfConvertedPart;
        /**
        * Shows warning if a part is already used in a case
        * @function Intake.CasePartDetail.IsPartUsed
        * @returns {void}
        */
        function IsPartUsed(formContext) {
            var product = formContext.getAttribute("ipg_productid").getValue();
            var incident = formContext.getAttribute("ipg_caseid").getValue();
            formContext.getControl('ipg_productid').clearNotification(IsPartUsed.name);
            if (product && incident) {
                Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=_ipg_caseid_value eq " + Intake.Utility.removeCurlyBraces(incident[0].id) + " and  _ipg_productid_value eq " + Intake.Utility.removeCurlyBraces(product[0].id) + " and statecode eq 0").then(function success(results) {
                    if (results.entities.length) {
                        formContext.getControl("ipg_productid").setNotification("The part is already used in this case", IsPartUsed.name);
                    }
                }, function (error) {
                    Xrm.Navigation.openAlertDialog({ text: error.message });
                });
            }
        }
        function LockActualPart(formContext) {
            return __awaiter(this, void 0, void 0, function () {
                var isPartLocked;
                return __generator(this, function (_a) {
                    isPartLocked = formContext.getAttribute("ipg_islocked").getValue();
                    if (isPartLocked) {
                        formContext.ui.controls.forEach(function (control) {
                            if (control && control.getDisabled && !control.getDisabled()) {
                                control.setDisabled(true);
                            }
                        });
                    }
                    return [2 /*return*/];
                });
            });
        }
    })(CasePartDetail = Intake.CasePartDetail || (Intake.CasePartDetail = {}));
})(Intake || (Intake = {}));
