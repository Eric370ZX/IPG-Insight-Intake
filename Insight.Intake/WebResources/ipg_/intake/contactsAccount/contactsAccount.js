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
 * @namespace Intake.ContactsAccount
 */
var Intake;
(function (Intake) {
    var ContactsAccount;
    (function (ContactsAccount) {
        /**
         * Called on load form
         * @function Intake.ContactsAccount.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            setupVisibilityOfFieldsByContactRole(formContext);
        }
        ContactsAccount.OnLoadForm = OnLoadForm;
        /**
         * Called on load quick create form
         * @function Intake.ContactsAccount.OnLoadQuickCreateForm
         * @returns {void}
        */
        function OnLoadQuickCreateForm(executionContext) {
            var formContext = executionContext.getFormContext();
            setupVisibilityOfFieldsByContactRole(formContext);
            stVisibilityByRelation(formContext);
            restrictExistingRelations(formContext);
        }
        ContactsAccount.OnLoadQuickCreateForm = OnLoadQuickCreateForm;
        function restrictExistingRelations(formContext) {
            var _this = this;
            var accountAttr = formContext.getAttribute("ipg_accountid");
            var contactAttr = formContext.getAttribute("ipg_contactid");
            var recordAlreadyExistsMessage = function (entityName) { return "This " + entityName + " has already been assosiated with the selected " + (entityName == "Contact" ? "Facility" : "Contact"); };
            accountAttr.addOnChange(function () { return __awaiter(_this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, relationExists(formContext)];
                        case 1:
                            if (_a.sent()) {
                                accountAttr.setValue(null);
                                Xrm.Navigation.openAlertDialog({ text: recordAlreadyExistsMessage("Facility"), title: "Warning" });
                            }
                            return [2 /*return*/];
                    }
                });
            }); });
            contactAttr.addOnChange(function () { return __awaiter(_this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, relationExists(formContext)];
                        case 1:
                            if (_a.sent()) {
                                contactAttr.setValue(null);
                                Xrm.Navigation.openAlertDialog({ text: recordAlreadyExistsMessage("Contact"), title: "Warning" });
                            }
                            return [2 /*return*/];
                    }
                });
            }); });
        }
        function relationExists(formContext) {
            var _a, _b, _c, _d;
            return __awaiter(this, void 0, void 0, function () {
                var accountId, contactId, _e;
                return __generator(this, function (_f) {
                    switch (_f.label) {
                        case 0:
                            accountId = (_a = formContext.getAttribute("ipg_accountid")) === null || _a === void 0 ? void 0 : _a.getValue();
                            contactId = (_b = formContext.getAttribute("ipg_contactid")) === null || _b === void 0 ? void 0 : _b.getValue();
                            if (!(accountId && contactId)) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("ipg_contactsaccounts", "?$filter=_ipg_contactid_value eq " + Intake.Utility.GetFormattedId((_c = contactId[0]) === null || _c === void 0 ? void 0 : _c.id) + " and _ipg_accountid_value eq " + Intake.Utility.GetFormattedId((_d = accountId[0]) === null || _d === void 0 ? void 0 : _d.id))];
                        case 1:
                            _e = (_f.sent())
                                .entities
                                .length > 0;
                            return [3 /*break*/, 3];
                        case 2:
                            _e = false;
                            _f.label = 3;
                        case 3: return [2 /*return*/, _e];
                    }
                });
            });
        }
        /**
         * Called on load quick create/edit form
         * @function Intake.ContactsAccount.OnLoadUpsertForm
         * @returns {void}
        */
        function OnLoadUpsertForm(executionContext) {
            var formContext = executionContext.getFormContext();
            OnCommunicationPreferencesChange(formContext);
            formContext.getAttribute('ipg_communicationpreference').addOnChange(function () {
                OnCommunicationPreferencesChange(formContext);
            });
            setLockedPrimaryContact(formContext);
        }
        ContactsAccount.OnLoadUpsertForm = OnLoadUpsertForm;
        /**
         * Called on Communication Preferences change in quick create/edit form
         * @function Intake.ContactsAccount.OnCommunicationPreferencesChange
         * @returns {void}
        */
        function OnCommunicationPreferencesChange(formContext) {
            var preference = formContext.getAttribute("ipg_communicationpreference");
            var email = formContext.getAttribute("ipg_email");
            var mainPhone = formContext.getAttribute("ipg_mainphone");
            if (preference.getValue() == 1) { // Email      
                email === null || email === void 0 ? void 0 : email.setRequiredLevel("required");
                mainPhone === null || mainPhone === void 0 ? void 0 : mainPhone.setRequiredLevel("none");
            }
            if (preference.getValue() == 2) { // Main Phone
                mainPhone === null || mainPhone === void 0 ? void 0 : mainPhone.setRequiredLevel("required");
                email === null || email === void 0 ? void 0 : email.setRequiredLevel("none");
            }
            if (preference.getValue() == 3) { // Portal Comment
                email === null || email === void 0 ? void 0 : email.setRequiredLevel("none");
                mainPhone === null || mainPhone === void 0 ? void 0 : mainPhone.setRequiredLevel("none");
            }
        }
        /**
         * Set Locked Facility Primary Contact checkbox
         * @function Intake.ContactsAccount.setLockedPrimaryContact
         * @returns {void}
        */
        function setLockedPrimaryContact(formContext) {
            var isprimaryContact = formContext.getAttribute("ipg_isprimarycontact");
            if (isprimaryContact.getValue() == true) {
                formContext.getControl("ipg_isprimarycontact").setDisabled(true);
            }
            else {
                formContext.getControl("ipg_isprimarycontact").setDisabled(false);
            }
        }
        /**
         * Called on change contact role
         * @function Intake.ContactsAccount.OnChangeContactRole
         * @returns {void}
        */
        function OnChangeContactRole(executionContext) {
            var formContext = executionContext.getFormContext();
            setupVisibilityOfFieldsByContactRole(formContext);
        }
        ContactsAccount.OnChangeContactRole = OnChangeContactRole;
        /**
         * set visibility of fields by contact role
         * @function Intake.ContactsAccount.setupVisibilityOfFieldsByContactRole
         * @returns {void}
        */
        function setupVisibilityOfFieldsByContactRole(formContext) {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k;
            var contactType = formContext.getAttribute("ipg_contactrolecode").getValue();
            if (contactType && contactType.indexOf(923720002) != -1) {
                (_a = formContext.getControl("ipg_rboname")) === null || _a === void 0 ? void 0 : _a.setVisible(true);
            }
            else if (contactType && contactType.indexOf(923720006) != -1) {
                (_b = formContext.getControl("ipg_vendorid")) === null || _b === void 0 ? void 0 : _b.setVisible(true);
                (_c = formContext.getControl("ipg_daystopay")) === null || _c === void 0 ? void 0 : _c.setVisible(true);
                (_d = formContext.getAttribute("ipg_vendorid")) === null || _d === void 0 ? void 0 : _d.setRequiredLevel("required");
                (_e = formContext.getAttribute("ipg_daystopay")) === null || _e === void 0 ? void 0 : _e.setRequiredLevel("required");
            }
            else {
                (_f = formContext.getControl("ipg_rboname")) === null || _f === void 0 ? void 0 : _f.setVisible(false);
                (_g = formContext.getControl("ipg_vendorid")) === null || _g === void 0 ? void 0 : _g.setVisible(false);
                (_h = formContext.getControl("ipg_daystopay")) === null || _h === void 0 ? void 0 : _h.setVisible(false);
                (_j = formContext.getAttribute("ipg_vendorid")) === null || _j === void 0 ? void 0 : _j.setRequiredLevel("none");
                (_k = formContext.getAttribute("ipg_daystopay")) === null || _k === void 0 ? void 0 : _k.setRequiredLevel("none");
            }
        }
        function stVisibilityByRelation(formContext) {
            if (formContext.getAttribute("ipg_contactid").getValue()) {
                formContext.getControl("ipg_contactid").setVisible(false);
            }
            else {
                formContext.getControl("ipg_accountid").setVisible(false);
            }
        }
    })(ContactsAccount = Intake.ContactsAccount || (Intake.ContactsAccount = {}));
})(Intake || (Intake = {}));
