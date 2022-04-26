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
 * @namespace Intake.ContactsAccount.Ribbon
 */
var Intake;
(function (Intake) {
    var ContactsAccount;
    (function (ContactsAccount) {
        var Ribbon;
        (function (Ribbon) {
            /**
             * Called on New button click on subgrid dropdown
             * @function Intake.ContactsAccount.Ribbon.OnNewClick
             * @returns {void}
             */
            function OnNewClick(primaryControl, contactRole) {
                var formContext = primaryControl;
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_contactsaccounts";
                entityFormOptions["useQuickCreateForm"] = true;
                var formParameters = {};
                if (formContext.data.entity.getEntityName() == "account") {
                    formParameters["ipg_accountid"] = formContext.data.entity.getId();
                    formParameters["ipg_accountidname"] =
                        formContext.getAttribute("name") &&
                            formContext.getAttribute("name").getValue();
                }
                else if (formContext.data.entity.getEntityName() == "contact") {
                    formParameters["ipg_contactid"] = formContext.data.entity.getId();
                    formParameters["ipg_contactidname"] =
                        formContext.getAttribute("fullname") &&
                            formContext.getAttribute("fullname").getValue();
                }
                formParameters["ipg_contactrolecode"] = "[" + contactRole + "]";
                Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
                    console.log(success);
                }, function (error) {
                    console.log(error);
                });
            }
            Ribbon.OnNewClick = OnNewClick;
            /**
             * Opens Contacts Account Quick Form
             * @function Intake.ContactsAccount.Ribbon.UpsertContactAccount
             * @returns {void}
             */
            function UpsertContactAccount(primaryControl, selectedRecordId) {
                return __awaiter(this, void 0, void 0, function () {
                    var entityFormOptions, formParameters, contact;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                entityFormOptions = {
                                    formId: "96960917-c807-4b68-9376-91e353de0062",
                                    entityName: "ipg_contactsaccounts",
                                    useQuickCreateForm: true,
                                };
                                formParameters = {};
                                if (!selectedRecordId) return [3 /*break*/, 2];
                                return [4 /*yield*/, retriveContactAccountById(selectedRecordId)];
                            case 1:
                                contact = _a.sent();
                                formParameters = {
                                    ipg_contactid: contact._ipg_contactid_value,
                                    ipg_contactname: contact.ipg_contactname,
                                    ipg_communicationpreference: contact.ipg_communicationpreference,
                                    ipg_email: contact.ipg_email,
                                    ipg_fax: contact.ipg_fax,
                                    ipg_isprimarycontact: contact.ipg_isprimarycontact,
                                    ipg_mainphone: contact.ipg_mainphone,
                                    ipg_otherphone: contact.ipg_otherphone,
                                    ipg_contactrolecode: contact.ipg_contactrolecode.split(',').map(Number)
                                };
                                _a.label = 2;
                            case 2:
                                formParameters["ipg_accountid"] = primaryControl.data.entity.getId();
                                return [4 /*yield*/, Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function () {
                                        var _a;
                                        (_a = primaryControl === null || primaryControl === void 0 ? void 0 : primaryControl.data) === null || _a === void 0 ? void 0 : _a.refresh();
                                    })];
                            case 3:
                                _a.sent();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.UpsertContactAccount = UpsertContactAccount;
            function retriveContactAccountById(id) {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, Xrm.WebApi.online.retrieveRecord("ipg_contactsaccounts", id, "?$select=_ipg_contactid_value,ipg_contactname,ipg_communicationpreference,ipg_email,ipg_fax,ipg_isprimarycontact,ipg_mainphone,ipg_otherphone,ipg_contactrolecode")];
                            case 1: return [2 /*return*/, _a.sent()];
                        }
                    });
                });
            }
            /**
            * Opens Portal Contact Form
            * @function Intake.ContactsAccount.Ribbon.EditPortalFacilityUser
            * @returns {void}
            */
            //TODO : rename
            function EditPortalFacilityUser(selectedRecordId) {
                return __awaiter(this, void 0, void 0, function () {
                    var formOptions, _a;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                _a = {};
                                return [4 /*yield*/, retriveContactAccountById(selectedRecordId)];
                            case 1:
                                formOptions = (_a.entityId = (_b.sent())._ipg_contactid_value,
                                    _a.formId = "ab10241c-37d5-4b20-9327-804338978643",
                                    _a.entityName = "contact",
                                    _a);
                                Xrm.Navigation.openForm(formOptions);
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.EditPortalFacilityUser = EditPortalFacilityUser;
            function Disassosiate(selectedIds) {
                return __awaiter(this, void 0, void 0, function () {
                    var confirmString, confirmDialogResult;
                    var _this = this;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                confirmString = {
                                    text: "Do you wish to proceed with disassociating the selected Portal Users from this Facility?",
                                    cancelButtonLabel: "Cancel",
                                    confirmButtonLabel: "Continue"
                                };
                                return [4 /*yield*/, Xrm.Navigation.openConfirmDialog(confirmString)];
                            case 1:
                                confirmDialogResult = _a.sent();
                                if (confirmDialogResult.confirmed) {
                                    selectedIds.forEach(function (selectedRecordId) { return __awaiter(_this, void 0, void 0, function () { return __generator(this, function (_a) {
                                        switch (_a.label) {
                                            case 0: return [4 /*yield*/, Xrm.WebApi.deleteRecord("ipg_contactsaccounts", selectedRecordId)];
                                            case 1: return [2 /*return*/, _a.sent()];
                                        }
                                    }); }); });
                                }
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.Disassosiate = Disassosiate;
            /**
             * Opens AssosiateFacility
             * @function Intake.ContactsAccount.Ribbon.AssosiateWithNewFacility
             * @returns {void}
             */
            function AssosiateWithNewFacility(primaryControl) {
                return __awaiter(this, void 0, void 0, function () {
                    var formOptions, formParameters;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                formOptions = {
                                    formId: "3d62166dde-dc3e-4671-aa0c-de52f3f278b9",
                                    entityName: "ipg_contactsaccounts",
                                    useQuickCreateForm: true
                                };
                                formParameters = primaryControl.data.entity.getEntityName() === 'contact'
                                    ? { ipg_contactid: format(primaryControl.data.entity.getId()) }
                                    : { ipg_accountid: format(primaryControl.data.entity.getId()) };
                                return [4 /*yield*/, Xrm.Navigation.openForm(formOptions, formParameters)];
                            case 1:
                                _a.sent();
                                return [2 /*return*/];
                        }
                    });
                });
            }
            Ribbon.AssosiateWithNewFacility = AssosiateWithNewFacility;
            // just fast format for ids to not use Utility.js in ribbon file
            var format = function (id) {
                return id.replace("{", "").replace("}", "").toLowerCase();
            };
            /**
            * Show buttons only on related Tab
            * @function Intake.ContactsAccount.Ribbon.ShowOnRelatedTab
            * @returns {void}
            */
            function ShowOnlyOnRelatedTab(selectedControl, tabName) {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        return [2 /*return*/, selectedControl["_controlName"] === tabName];
                    });
                });
            }
            Ribbon.ShowOnlyOnRelatedTab = ShowOnlyOnRelatedTab;
            /**
            * Hide buttons only on related Tab
            * @function Intake.ContactsAccount.Ribbon.HideOnRelatedTab
            * @returns {void}
            */
            function HideOnlyOnRelatedTab(selectedControl, tabName) {
                return __awaiter(this, void 0, void 0, function () {
                    return __generator(this, function (_a) {
                        return [2 /*return*/, selectedControl["_controlName"] != tabName];
                    });
                });
            }
            Ribbon.HideOnlyOnRelatedTab = HideOnlyOnRelatedTab;
        })(Ribbon = ContactsAccount.Ribbon || (ContactsAccount.Ribbon = {}));
    })(ContactsAccount = Intake.ContactsAccount || (Intake.ContactsAccount = {}));
})(Intake || (Intake = {}));
