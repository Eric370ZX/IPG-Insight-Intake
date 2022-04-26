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
        var AccountReviewStatus;
        (function (AccountReviewStatus) {
            AccountReviewStatus[AccountReviewStatus["Approved"] = 427880001] = "Approved";
        })(AccountReviewStatus || (AccountReviewStatus = {}));
        function isPortalContactForm(primaryControl) {
            var formContext = primaryControl;
            return formContext.ui.formSelector.getCurrentItem().getLabel() === "Portal Contact";
        }
        Contact.isPortalContactForm = isPortalContactForm;
        function Deactivate(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, contactId, result, i;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = primaryControl;
                            contactId = formContext.data.entity.getId().replace('{', '').replace('}', '');
                            return [4 /*yield*/, getContactAccountRecords(contactId)];
                        case 1:
                            result = _a.sent();
                            for (i = 0; i < result.entities.length; i++) {
                                Xrm.WebApi.deleteRecord("ipg_contactsaccounts", result.entities[i].ipg_contactsaccountsid);
                            }
                            Xrm.WebApi.updateRecord("contact", contactId, {
                                adx_identity_logonenabled: false,
                                ipg_portalnotificationpreference: 427880001,
                                adx_identity_lockoutenddate: new Date().toLocaleDateString('yyyy-MM-dd')
                            }).then(function success() {
                                window["isPortalContactEnabled"] = '';
                                formContext.data.refresh(true);
                                formContext.ui.refreshRibbon(true);
                            }, function (error) {
                                console.log(error.message);
                            });
                            return [2 /*return*/];
                    }
                });
            });
        }
        Contact.Deactivate = Deactivate;
        function getContactAccountRecords(contactId) {
            return __awaiter(this, void 0, void 0, function () {
                var fetch;
                return __generator(this, function (_a) {
                    fetch = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\" >\n                       <entity name=\"ipg_contactsaccounts\" >\n                           <attribute name=\"ipg_contactsaccountsid\" />\n                           <filter type=\"and\" >\n                               <condition attribute=\"ipg_contactid\" operator=\"eq\" uiname=\"\" uitype=\"contact\" value=\"" + contactId + "\" />\n                           </filter>\n                       </entity>\n                   </fetch>";
                    return [2 /*return*/, Xrm.WebApi.retrieveMultipleRecords("ipg_contactsaccounts", "?fetchXml=" + fetch)];
                });
            });
        }
        var isPortalContactEnabled = false;
        function IsPortalContactEnabled(primaryControl) {
            var formContext = primaryControl;
            var contactid = formContext.data.entity.getId().replace('{', '').replace('}', '');
            if (window["isPortalContactEnabled"])
                return isPortalContactEnabled;
            Xrm.WebApi.retrieveRecord("contact", contactid, "?$select=adx_identity_logonenabled").then(function success(result) {
                window["isPortalContactEnabled"] = true;
                isPortalContactEnabled = (result.adx_identity_logonenabled);
                if (isPortalContactEnabled) {
                    formContext.ui.refreshRibbon(true);
                }
            });
            return false;
        }
        Contact.IsPortalContactEnabled = IsPortalContactEnabled;
        /**
         * Open new contact form
         * @function Intake.Contact.NewContactForm
         * @returns {void}
         */
        function NewContactForm(PrimaryControl) {
            var viewName = "carrier";
            if (PrimaryControl) { //PrimaryControl equals null in classic UI
                var viewSelector = PrimaryControl.getViewSelector();
                viewName = viewSelector.getCurrentView().name.toLocaleLowerCase();
            }
            var ipg_contacttypeid;
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "contact";
            var formParameters = {};
            var defaultFordId = '1FED44D1-AE68-4A41-BD2B-F13ACAC4ACFA';
            var contactTypes = [
                { name: 'carrier', value: '5e9050c9-515e-46ec-9b95-65b8858876b0', formId: defaultFordId },
                { name: 'distributor', value: '0b0e5fff-afe1-4da5-9c68-68842c8ca5b2', formId: defaultFordId },
                { name: 'facilit', value: 'b6d48511-adbf-4ddf-8885-43d06460c63a', formId: defaultFordId },
                { name: 'health plan network', value: '47724e16-c175-473f-91e3-f33d1514fca3', formId: '807AD6E4-30D2-4AD3-8CC4-0CD5E8EC1A9F' },
                { name: 'manufacturer', value: '8b383208-32b6-42b6-81bc-a0cbfed34fae', formId: '85ED629F-ED77-406A-BBF2-8013FA618F25' },
                { name: 'patient', value: '3ed7420f-e687-47ec-8c44-a675305137bd', formId: 'A1F66726-D064-430F-83D9-A40B2551E0F8' },
                { name: 'physician', value: '2b5407df-b88a-4bf4-8e6f-ab174bc13c78', formId: '12b0030d-bd5b-4c99-ad0d-fb7eab63426a' }
            ];
            contactTypes.forEach(function (element1) {
                if (viewName.indexOf(element1.name) >= 0) {
                    var viewWithMultipleContactTypes = false;
                    contactTypes.forEach(function (element2) {
                        if ((element1.name != element2.name) && (viewName.indexOf(element2.name) >= 0))
                            viewWithMultipleContactTypes = true;
                    });
                    if (!viewWithMultipleContactTypes) {
                        ipg_contacttypeid = element1.value;
                        entityFormOptions["formId"] = element1.formId;
                    }
                }
            });
            //if (ipg_contacttypeid) {
            //  formParameters["ipg_contacttypeid"] = ipg_contacttypeid;
            //}
            Xrm.Navigation.openForm(entityFormOptions, formParameters);
        }
        Contact.NewContactForm = NewContactForm;
        /**
         * enable rule for 'New' dropdown for creating Carrier's contact
         * @function Intake.Contact.contactFormIsCarrier
         * @returns {boolean}
         */
        function contactFormIsCarrier(primaryControl) {
            var formContext = primaryControl;
            var formName = formContext.ui.formSelector.getCurrentItem().getLabel();
            if (formName === "Carrier") {
                return true;
            }
            return false;
        }
        Contact.contactFormIsCarrier = contactFormIsCarrier;
        /**
         * enable rule for 'Create Invitation' ribbon`s button
         * @function Intake.Contact.AccountStatusIsEnabled
         * @returns {boolean}
         */
        function AccountStatusIsEnabled(selectedRecordId) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var accountStatus;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.online.retrieveRecord("contact", selectedRecordId.toLocaleLowerCase().replace("{", "").replace("}", ""), "?$select=adx_identity_logonenabled")];
                        case 1:
                            accountStatus = (_a = (_b.sent())) === null || _a === void 0 ? void 0 : _a.adx_identity_logonenabled;
                            return [2 /*return*/, accountStatus];
                    }
                });
            });
        }
        Contact.AccountStatusIsEnabled = AccountStatusIsEnabled;
        /**
        * enable rule for 'Create Invitation' ribbon`s button
        * @function Intake.Contact.AccountReviewStatusIsApproved
        * @returns {boolean}
        */
        function AccountReviewStatusIsApproved(selectedRecordId) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var accountReviewStatus;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.online.retrieveRecord("contact", selectedRecordId.toLocaleLowerCase().replace("{", "").replace("}", ""), "?$select=ipg_facility_user_status_typecode")];
                        case 1:
                            accountReviewStatus = (_a = (_b.sent())) === null || _a === void 0 ? void 0 : _a.ipg_facility_user_status_typecode;
                            return [2 /*return*/, accountReviewStatus == AccountReviewStatus.Approved];
                    }
                });
            });
        }
        Contact.AccountReviewStatusIsApproved = AccountReviewStatusIsApproved;
        /**
         * show Quick Create Carrier Form with predefined types of contact
         * @function Intake.Contact.NewCarrierContactSubgrid
         * @returns {void}
         */
        function NewCarrierContactSubgrid(primaryControl, contactSubTypeName) {
            function openQuickCreateCarrierForm(carrierContactType, contactCarrierSubType) {
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "contact";
                entityFormOptions["useQuickCreateForm"] = true;
                entityFormOptions["formId"] = "B587448B-DF14-45D6-9F2D-E73F5822E6E2";
                var formParameters = {};
                formParameters["ipg_relatedcarreiraccountid"] = formContext.data.entity.getId();
                formParameters["ipg_relatedcarreiraccountidname"] = formContext.getAttribute("name").getValue();
                formParameters["ipg_relatedcarreiraccountidentityType"] = "account";
                if (carrierContactType) {
                    formParameters["ipg_contacttypeid"] = carrierContactType.ipg_contacttypeid;
                    formParameters["ipg_contacttypeidname"] = carrierContactType.ipg_name;
                    formParameters["ipg_contacttypeidentityType"] = "ipg_contacttype";
                }
                if (contactSubTypeName) {
                    formParameters["ipg_contactsubtypeid"] = contactCarrierSubType.ipg_contactsubtypeid;
                    formParameters["ipg_contactsubtypeidname"] = contactCarrierSubType.ipg_name;
                    formParameters["ipg_contactsubtypeidentityType"] = "ipg_contactsubtype";
                }
                //formParameters["ipg_contactrolecode"] = contactRole;
                Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
                    console.log(success);
                }, function (error) {
                    console.log(error);
                });
            }
            var formContext = primaryControl;
            Xrm.Utility.showProgressIndicator("Loading...");
            Xrm.WebApi.retrieveMultipleRecords("ipg_contacttype", "?$select=ipg_name&$filter=contains(ipg_name,'Carrier')")
                .then(function (result) {
                if (result && result.entities && result.entities.length) {
                    var carrierContactType_1 = result.entities[0]; //ipg_contacttypeid
                    Xrm.WebApi.retrieveMultipleRecords("ipg_contactsubtype", "?$select=ipg_name&$filter=contains(ipg_name,'" + contactSubTypeName + "') and _ipg_parentcontacttype_value eq '" + carrierContactType_1.ipg_contacttypeid + "'")
                        .then(function (result) {
                        if (result && result.entities && result.entities.length) {
                            var contactCarrierSubType = result.entities[0];
                            Xrm.Utility.closeProgressIndicator();
                            openQuickCreateCarrierForm(carrierContactType_1, contactCarrierSubType);
                        }
                        else {
                            Xrm.Utility.closeProgressIndicator();
                            Xrm.Navigation.openAlertDialog({ text: "There is no '" + contactSubTypeName + "' contact subtype record!" });
                        }
                    }, function (error) {
                        Xrm.Utility.closeProgressIndicator();
                        console.log(error);
                    });
                }
                else {
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openAlertDialog({ text: "There is no 'Carrier' contact type record!" });
                }
                //var contactSubType = Xrm.WebApi.retrieveMultipleRecords("ipg_contactsubtype"); Claim Escalation
            }, function (error) {
                Xrm.Utility.closeProgressIndicator();
                console.log(error);
            });
        }
        Contact.NewCarrierContactSubgrid = NewCarrierContactSubgrid;
        function ChangePassword(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, confirmString, confirmDialogResult, tempPassword, contactId;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = primaryControl;
                            confirmString = {
                                text: "Do you wish to generate a temporary password for this user?",
                                confirmButtonLabel: "Confirm",
                                cancelButtonLabel: "Cancel"
                            };
                            return [4 /*yield*/, Xrm.Navigation.openConfirmDialog(confirmString)];
                        case 1:
                            confirmDialogResult = _a.sent();
                            if (confirmDialogResult.confirmed) {
                                tempPassword = GenerateNewPassword();
                                contactId = formContext.data.entity.getId().replace("{", "").replace("}", "");
                                StartChangePasswordWorkflow(contactId, tempPassword);
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        Contact.ChangePassword = ChangePassword;
        function GenerateNewPassword() {
            var chars = "0123456789abcdefghijklmnopqrstuvwxyz!@$%^*.,()ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var passwordLength = 12;
            var password = "!*temp*!";
            for (var i = 0; i < passwordLength; i++) {
                var randomNumber = Math.floor(Math.random() * chars.length);
                password += chars.substring(randomNumber, randomNumber + 1);
            }
            return password;
        }
        function StartChangePasswordWorkflow(contactId, tempPassword) {
            Xrm.Utility.showProgressIndicator("Processing...");
            var entity = {
                id: contactId,
                entityType: "contact"
            };
            var parameters = {
                entity: entity,
                NewPassword: "" + tempPassword,
            };
            var parameterTypes = {
                "entity": {
                    "typeName": "mscrm.contact",
                    "structuralProperty": 5
                },
                "NewPassword": {
                    "typeName": "Edm.String",
                    "structuralProperty": 1
                }
            };
            var request = {
                entity: parameters.entity,
                NewPassword: parameters.NewPassword,
                getMetadata: function () {
                    return {
                        boundParameter: "entity",
                        parameterTypes: parameterTypes,
                        operationType: 0,
                        operationName: "ipg_IPGIntakeContactWorkflowsChangeContactPassword"
                    };
                }
            };
            Xrm.WebApi.online.execute(request)
                .then(function (response) {
                Xrm.Utility.closeProgressIndicator();
                if (response.ok) {
                    Xrm.Navigation.openAlertDialog({ text: "New temporary password generated succesfully!" });
                }
                else {
                    Xrm.Navigation.openErrorDialog({ message: response.statusText });
                }
            }, function (error) {
                Xrm.Utility.closeProgressIndicator();
                Xrm.Navigation.openErrorDialog({ message: error.message });
            });
        }
        function Impersonate(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, entity, request, e_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            Xrm.Utility.showProgressIndicator("Processing...");
                            formContext = primaryControl;
                            entity = {
                                id: formContext.data.entity.getId(),
                                entityType: "contact"
                            };
                            request = {
                                entity: entity,
                                getMetadata: function () {
                                    return {
                                        boundParameter: "entity",
                                        parameterTypes: {
                                            "entity": {
                                                "typeName": "mscrm.contact",
                                                "structuralProperty": 5
                                            }
                                        },
                                        operationType: 0,
                                        operationName: "ipg_IPGIntakeContactActionsImpersonate"
                                    };
                                }
                            };
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 3, , 4]);
                            return [4 /*yield*/, Xrm.WebApi.online.execute(request)];
                        case 2:
                            _a.sent();
                            Xrm.Utility.closeProgressIndicator();
                            return [3 /*break*/, 4];
                        case 3:
                            e_1 = _a.sent();
                            Xrm.Utility.closeProgressIndicator();
                            Xrm.Navigation.openErrorDialog({ message: e_1.message });
                            return [3 /*break*/, 4];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        }
        Contact.Impersonate = Impersonate;
        function ImpersonateEnablade(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, contactid, contact;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = primaryControl;
                            if (!IsCurrentUserSysAdmin() || formContext.ui.getFormType() == 1 /* Create */) {
                                return [2 /*return*/, false];
                            }
                            contactid = formContext.data.entity.getId();
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("contact", contactid, "?$select=adx_identity_securitystamp,ipg_backupjson")];
                        case 1:
                            contact = _a.sent();
                            return [2 /*return*/, contact.adx_identity_securitystamp != null];
                    }
                });
            });
        }
        Contact.ImpersonateEnablade = ImpersonateEnablade;
        function RevertBack(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, entity, request, e_2;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            Xrm.Utility.showProgressIndicator("Processing...");
                            formContext = primaryControl;
                            entity = {
                                id: formContext.data.entity.getId(),
                                entityType: "contact"
                            };
                            request = {
                                entity: entity,
                                getMetadata: function () {
                                    return {
                                        boundParameter: "entity",
                                        parameterTypes: {
                                            "entity": {
                                                "typeName": "mscrm.contact",
                                                "structuralProperty": 5
                                            }
                                        },
                                        operationType: 0,
                                        operationName: "ipg_IPGIntakeContactActionsRevertBackImpersonation"
                                    };
                                }
                            };
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 3, , 4]);
                            return [4 /*yield*/, Xrm.WebApi.online.execute(request)];
                        case 2:
                            _a.sent();
                            Xrm.Utility.closeProgressIndicator();
                            return [3 /*break*/, 4];
                        case 3:
                            e_2 = _a.sent();
                            Xrm.Utility.closeProgressIndicator();
                            Xrm.Navigation.openErrorDialog({ message: e_2.message });
                            return [3 /*break*/, 4];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        }
        Contact.RevertBack = RevertBack;
        function RevertBackEnabled(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, contactid, contact;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = primaryControl;
                            if (!IsCurrentUserSysAdmin() || formContext.ui.getFormType() == 1 /* Create */) {
                                return [2 /*return*/, false];
                            }
                            contactid = formContext.data.entity.getId();
                            return [4 /*yield*/, Xrm.WebApi.retrieveRecord("contact", contactid, "?$select=adx_identity_securitystamp,ipg_backupjson")];
                        case 1:
                            contact = _a.sent();
                            return [2 /*return*/, contact.adx_identity_securitystamp != null && contact.ipg_backupjson != null];
                    }
                });
            });
        }
        Contact.RevertBackEnabled = RevertBackEnabled;
        function IsCurrentUserSysAdmin() {
            return Xrm.Utility.getGlobalContext().userSettings.roles.get(function (lk) { return lk.name && lk.name.toLowerCase().trim() == "system administrator"; }).length > 0;
        }
        /**
       * Show buttons only on related Form
       * @function Intake.Contact.ShowOnlyOnRelatedForm
       * @returns {bool}
       */
        function ShowOnlyOnRelatedForm(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, formsName, formName;
                return __generator(this, function (_a) {
                    formContext = primaryControl;
                    formsName = ["Physician", "Portal Contact"];
                    formName = formContext.ui.formSelector.getCurrentItem().getLabel();
                    return [2 /*return*/, formsName.indexOf(formName) !== -1]; //formsName.includes(formName);
                });
            });
        }
        Contact.ShowOnlyOnRelatedForm = ShowOnlyOnRelatedForm;
        /**
       * Hide buttons only on related Form
       * @function Intake.Contact.HideOnlyOnRelatedForm
       * @returns {bool}
       */
        function HideOnlyOnRelatedForm(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, formName;
                return __generator(this, function (_a) {
                    formContext = primaryControl;
                    formName = "Physician";
                    return [2 /*return*/, formContext.ui.formSelector.getCurrentItem().getLabel() != formName];
                });
            });
        }
        Contact.HideOnlyOnRelatedForm = HideOnlyOnRelatedForm;
    })(Contact = Intake.Contact || (Intake.Contact = {}));
})(Intake || (Intake = {}));
