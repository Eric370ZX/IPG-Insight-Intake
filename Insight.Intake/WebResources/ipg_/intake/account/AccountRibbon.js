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
 * @namespace Intake.Account
 */
var Intake;
(function (Intake) {
    var Account;
    (function (Account) {
        /**
         * Open new account form
         * @function Intake.Account.NewAccountForm
         * @returns {void}
         */
        function NewAccountForm(PrimaryControl) {
            var viewSelector = PrimaryControl.getViewSelector();
            var viewName = viewSelector.getCurrentView().name.toLocaleLowerCase();
            var customertypecode;
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "account";
            var formParameters = {};
            var accountTypes = [
                { name: 'carrier', value: 923720001, formId: 'BF7A66A7-F2ED-417D-821F-9F064E348EAD' },
                { name: 'manufacturer', value: 923720002, formId: '42DEB6BE-3ADB-4BBE-BCD2-90B46ED81AC8' },
                { name: 'facilit', value: 923720000, formId: '550FB40D-0D41-4728-989B-121EACCA2BE6' },
                { name: 'distributor', value: 923720003, formId: '25B4ED80-3C95-4D29-8120-6384A813BA12' }
            ];
            accountTypes.forEach(function (element1) {
                if (viewName.indexOf(element1.name) >= 0) {
                    var viewWithMultipleAccountTypes = false;
                    accountTypes.forEach(function (element2) {
                        if ((element1.name != element2.name) && (viewName.indexOf(element2.name) >= 0))
                            viewWithMultipleAccountTypes = true;
                    });
                    if (!viewWithMultipleAccountTypes) {
                        customertypecode = element1.value;
                        entityFormOptions["formId"] = element1.formId;
                    }
                }
            });
            if (customertypecode) {
                formParameters["customertypecode"] = customertypecode;
            }
            Xrm.Navigation.openForm(entityFormOptions, formParameters);
        }
        Account.NewAccountForm = NewAccountForm;
        /**
         * Filter distributors
         * @function Intake.Account.FilterDistributors
         * @returns {void}
         */
        function FilterDistributors(PrimaryControl, selectedEntityTypeName, selectedControl, firstPrimaryItemId) {
            var relationshipName = "ipg_manufacturer_distributor";
            if (selectedControl.getRelationship().name == relationshipName) {
                var options = {
                    allowMultiSelect: false,
                    defaultEntityType: "account",
                    entityTypes: ["account"],
                    defaultViewId: '1465144F-A620-E911-A979-000D3A37062B',
                    disableMru: true
                    //filters: [{ entityLogicalName: "account", filterXml: encodeURIComponent("<filter type='and'><condition attribute='customertypecode' operator='eq' value='923720003' /></filter>")}]
                };
                Xrm.Utility.lookupObjects(options);
            }
            //else
            //XrmCore.Commands.AddFromSubGrid.addExistingFromSubGridAssociated(selectedEntityTypeName, selectedControl);
        }
        Account.FilterDistributors = FilterDistributors;
        /**
         * Enable rule for Add existing button
         * @function Intake.Account.HasAssociatedFacilities
         * @returns {bool}
         */
        function HasAssociatedFacilities(firstPrimaryItemId, selectedControl, singleAssociationAbbrs) {
            return __awaiter(this, void 0, void 0, function () {
                var document, docTypesAbbr, relatedEntitiesCount;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", firstPrimaryItemId, "?$select=_ipg_facilityid_value&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)")];
                        case 1:
                            document = _a.sent();
                            docTypesAbbr = singleAssociationAbbrs.split(',');
                            relatedEntitiesCount = selectedControl.getGrid().getTotalRecordCount();
                            if ((document["_ipg_facilityid_value"] != null || relatedEntitiesCount > 0) && docTypesAbbr.find(function (abbr) { var _a; return abbr === ((_a = document.ipg_DocumentTypeId) === null || _a === void 0 ? void 0 : _a.ipg_documenttypeabbreviation); })) {
                                return [2 /*return*/, false];
                            }
                            return [2 /*return*/, true];
                    }
                });
            });
        }
        Account.HasAssociatedFacilities = HasAssociatedFacilities;
        /**
         * Enable rule for Add existing button
         * @function Intake.Account.HasAssociatedCarriers
         * @returns {bool}
         */
        function HasAssociatedCarriers(firstPrimaryItemId, selectedControl, multipleAssociationAbbrs) {
            return __awaiter(this, void 0, void 0, function () {
                var document, docTypesAbbr, relatedEntitiesCount;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", firstPrimaryItemId, "?$select=_ipg_carrierid_value&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)")];
                        case 1:
                            document = _a.sent();
                            docTypesAbbr = multipleAssociationAbbrs.split(',');
                            relatedEntitiesCount = selectedControl.getGrid().getTotalRecordCount();
                            if ((document["_ipg_carrierid_value"] != null || relatedEntitiesCount > 0) && !docTypesAbbr.find(function (abbr) { var _a; return abbr === ((_a = document.ipg_DocumentTypeId) === null || _a === void 0 ? void 0 : _a.ipg_documenttypeabbreviation); })) {
                                return [2 /*return*/, false];
                            }
                            return [2 /*return*/, true];
                    }
                });
            });
        }
        Account.HasAssociatedCarriers = HasAssociatedCarriers;
        /**
         * Enable rule for Add existing button
         * @function Intake.Account.HasAssociatedManufacturers
         * @returns {bool}
         */
        function HasAssociatedManufacturers(firstPrimaryItemId, selectedControl, singleAssociationAbbrs) {
            return __awaiter(this, void 0, void 0, function () {
                var document, docTypesAbbr, relatedEntitiesCount;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveRecord("ipg_document", firstPrimaryItemId, "?$select=_ipg_ipg_manufacturerid_value&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)")];
                        case 1:
                            document = _a.sent();
                            docTypesAbbr = singleAssociationAbbrs.split(',');
                            relatedEntitiesCount = selectedControl.getGrid().getTotalRecordCount();
                            if ((document["_ipg_ipg_manufacturerid_value"] != null || relatedEntitiesCount > 0) && docTypesAbbr.find(function (abbr) { var _a; return abbr === ((_a = document.ipg_DocumentTypeId) === null || _a === void 0 ? void 0 : _a.ipg_documenttypeabbreviation); })) {
                                return [2 /*return*/, false];
                            }
                            return [2 /*return*/, true];
                    }
                });
            });
        }
        Account.HasAssociatedManufacturers = HasAssociatedManufacturers;
        /**
         * Enable rule for show button on related tab
         * @function Intake.Account.ShowOnRelatedTab
         * @returns {bool}
         */
        function ShowOnRelatedTab(selectedControl, tabName) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/, selectedControl["_controlName"] === tabName];
                });
            });
        }
        Account.ShowOnRelatedTab = ShowOnRelatedTab;
        /**
         * Action for Add Existing Facility, Carrier, Manufacturer button
         * @function Intake.Account.AddExistingAccount
         * @returns {void}
         */
        function AddExistingAccount(firstPrimaryItemId, selectedControl) {
            return __awaiter(this, void 0, void 0, function () {
                var tabName, customerTypeCode, options, reference, requests_1, e_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            tabName = selectedControl["_controlName"];
                            switch (tabName) {
                                case "Facilities":
                                    customerTypeCode = 923720000 /* Facility */;
                                    break;
                                case "Carriers":
                                    customerTypeCode = 923720001 /* Carrier */;
                                    break;
                                case "Manufacturers":
                                    customerTypeCode = 923720002 /* Manufacturer */;
                                    break;
                            }
                            options = {
                                allowMultiSelect: true,
                                defaultEntityType: "account",
                                entityTypes: ["account"],
                                disableMru: true,
                                filters: [{
                                        filterXml: "<filter type='or'><condition attribute='customertypecode' operator='eq' value='" + customerTypeCode + "' /></filter>",
                                        entityLogicalName: "account"
                                    }]
                            };
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 3, , 4]);
                            return [4 /*yield*/, Xrm.Utility.lookupObjects(options)];
                        case 2:
                            reference = _a.sent();
                            if (reference && reference.length) {
                                Xrm.Utility.showProgressIndicator("Reassigning a document");
                                try {
                                    requests_1 = [];
                                    reference.forEach(function (acc) {
                                        if (acc.entityType === "account") {
                                            var accountId = Intake.Utility.removeCurlyBraces(acc.id);
                                            var documentId = Intake.Utility.removeCurlyBraces(firstPrimaryItemId);
                                            var associateRequest = {
                                                getMetadata: function () { return ({
                                                    boundParameter: null,
                                                    parameterTypes: {},
                                                    operationType: 2,
                                                    operationName: "Associate",
                                                }); },
                                                relationship: "ipg_ipg_document_account",
                                                target: {
                                                    entityType: "account",
                                                    id: accountId,
                                                },
                                                relatedEntities: [
                                                    {
                                                        entityType: "ipg_document",
                                                        id: documentId,
                                                    },
                                                ]
                                            };
                                            requests_1.push(associateRequest);
                                        }
                                    });
                                    Xrm.WebApi.online.executeMultiple(requests_1).then(function (success) {
                                        var alertStrings = {
                                            confirmButtonLabel: "Ok",
                                            text: "Adding process completed successfully",
                                            title: "Success",
                                        };
                                        var alertOptions = { height: 120, width: 260 };
                                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                                            selectedControl.refresh();
                                        });
                                    }, function (error) {
                                        console.log(error.message);
                                    });
                                }
                                catch (e) {
                                    Xrm.Navigation.openErrorDialog({
                                        message: e.Message || e.message || "Error during adding process",
                                    });
                                }
                                Xrm.Utility.closeProgressIndicator();
                            }
                            return [3 /*break*/, 4];
                        case 3:
                            e_1 = _a.sent();
                            Xrm.Navigation.openErrorDialog({
                                message: e_1.Message || e_1.message || "Error during adding process",
                            });
                            return [3 /*break*/, 4];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        }
        Account.AddExistingAccount = AddExistingAccount;
    })(Account = Intake.Account || (Intake.Account = {}));
})(Intake || (Intake = {}));
