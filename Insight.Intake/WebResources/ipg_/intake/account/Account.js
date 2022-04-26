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
        var workersCompPlanTypeValue = 923720004;
        var dirtyFields = ['ipg_statementtopatient', 'address1_country', 'address2_line3', 'ipg_claimname', 'ipg_pricelistdate']; //these fields are set business rules
        var facilityFormId = "550FB40D-0D41-4728-989B-121EACCA2BE6";
        var carrierFormId = "BF7A66A7-F2ED-417D-821F-9F064E348EAD";
        var FormIds = {
            facility: "550FB40D-0D41-4728-989B-121EACCA2BE6",
            carrier: "BF7A66A7-F2ED-417D-821F-9F064E348EAD"
        };
        var AccountStatusCodes;
        (function (AccountStatusCodes) {
            AccountStatusCodes[AccountStatusCodes["Active"] = 1] = "Active";
            AccountStatusCodes[AccountStatusCodes["Inactive"] = 2] = "Inactive";
        })(AccountStatusCodes || (AccountStatusCodes = {}));
        ;
        var AccountStatuses;
        (function (AccountStatuses) {
            AccountStatuses[AccountStatuses["Active"] = 427880000] = "Active";
            AccountStatuses[AccountStatuses["Inactive"] = 427880001] = "Inactive";
        })(AccountStatuses || (AccountStatuses = {}));
        ;
        /**
         * Called on load form
         * @function Intake.Account.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            formContext.getControl("customertypecode").setVisible(false);
            HideRelationTypeField(formContext);
            ChooseForm(formContext);
            hideWorkersCompOrOtherPlanTypes(formContext);
            //updateBillPatient(formContext);   
            setCarrierAccepted(formContext);
            var formName = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();
            sessionStorage.setItem('passVariables', formName);
            //setDefaultManufacturerPaymentThreshold(formContext);
            if (setDefaultManufacturerPaymentThreshold.length != 0) {
                setDefaultManufacturerPaymentThreshold(formContext);
            }
            OnChangeType();
            setSubmitModeForDirtyFields(formContext);
            SetRemittanceAddress(formContext);
            disableFormSelector(formContext);
            DirectBill(formContext);
        }
        Account.OnLoadForm = OnLoadForm;
        /**
        * Called on save form
        * @function Intake.Account.OnSaveForm
        * @returns {void}
       */
        function OnSaveForm(executionContext) {
            var _a, _b;
            var formContext = executionContext.getFormContext();
            var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
            if (currentFormId.toLocaleLowerCase() === facilityFormId.toLocaleLowerCase()) {
                var statusCode = (_a = formContext.getAttribute("statuscode")) === null || _a === void 0 ? void 0 : _a.getValue();
                var status_1 = (_b = formContext.getAttribute("ipg_status")) === null || _b === void 0 ? void 0 : _b.getValue();
                if (statusCode != null && statusCode === AccountStatusCodes.Active && status_1 != null && status_1 === AccountStatuses.Active) {
                    formContext.getAttribute("ipg_active").setValue(true);
                }
                else if (statusCode != null && statusCode === AccountStatusCodes.Inactive && status_1 != null && status_1 === AccountStatuses.Inactive) {
                    formContext.getAttribute("ipg_active").setValue(false);
                }
            }
            if (formContext.ui.getFormType() == 1) { // Create form
                setRelationshipType(formContext);
            }
        }
        Account.OnSaveForm = OnSaveForm;
        function SetBSAFile(executionContext) {
            var _a, _b, _c;
            var formContext = executionContext.getFormContext();
            if (formContext.getAttribute("ipg_facilitysignedbsa").getValue() == 923720000) {
                (_a = formContext.getAttribute("ipg_bsanotrequiredreason")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("none");
            }
            else if (formContext.getAttribute("ipg_facilitysignedbsa").getValue() == 923720001) {
                (_b = formContext.getAttribute("ipg_bsanotrequiredreason")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("required");
            }
            else {
                (_c = formContext.getAttribute("ipg_bsanotrequiredreason")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("none");
            }
        }
        Account.SetBSAFile = SetBSAFile;
        /**
        * Hides 'Relation Type' field if it's filled
        * @function Intake.Account.HideRelationTypeField
        * @returns {void}
        */
        function HideRelationTypeField(formContext) {
            var customertypecode = formContext.getAttribute("customertypecode");
            if (customertypecode) {
                if (customertypecode.getValue() != null)
                    formContext.getControl("customertypecode").setVisible(false);
            }
        }
        /**
        * Displays the selected  Set CPT Description
        * @function Intake.Account.SetCPTDescription
        * @returns {void}
        */
        function DisplayCarrierCPTName(formContext) {
            // ipg_carriercptname
            var carrierName = formContext.getAttribute("ipg_facilitycptname").getValue();
            if (carrierName == null) {
                formContext.getAttribute("ipg_carriercptname").setVisible(true);
            }
            else {
                formContext.getAttribute("ipg_carriercptname").setVisible(false);
            }
        }
        function SetCPTDescription(formContext) {
            var cptDescription;
            var cptCodeObject = Xrm.Page.getAttribute("ipg_cptcode").getValue();
            if (cptCodeObject != null) {
                var cptCodeId;
                cptCodeId = cptCodeObject[0];
                var cptId;
                //cptId= Object.fromEntries(
                //  Object.entries(cptCodeId).map(([key, value]) => [key, value])
                //);
                var epirationdate = new Date('12/31/9999');
                var todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
                var guid = cptId.id.replace("{", "").replace("}", "");
                Xrm.WebApi.retrieveRecord("ipg_cptcode", guid, "?$select=ipg_cptcode,ipg_description,ipg_cptname").then(function success(results) {
                    Xrm.Page.getAttribute("ipg_facilitycptname").setValue(results["ipg_cptname"] + " - " + results["ipg_cptcode"]);
                    Xrm.Page.getAttribute("ipg_cptdescription").setValue(results["ipg_description"]);
                    Xrm.Page.getAttribute("ipg_effectivedate").setValue(todayDate);
                    Xrm.Page.getAttribute("ipg_expirationdate").setValue(epirationdate);
                }, function (error) {
                });
            }
        }
        // Account.SetCPTDescription = SetCPTDescription;
        /**
        * Choose form depending on 'Relation Type' field value
        * @function Intake.Account.ChooseForm
        * @returns {void}
        */
        function ChooseForm(formContext) {
            var customertypecode = formContext.getAttribute("customertypecode");
            if (customertypecode) {
                var relationType = customertypecode.getValue();
                if ((formContext.ui.getFormType() == 2) && (relationType != null)) {
                    var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
                    var accountTypes = [
                        { name: 'carrier', value: 923720001, formId: 'BF7A66A7-F2ED-417D-821F-9F064E348EAD'.toLocaleLowerCase() },
                        { name: 'manufacturer', value: 923720002, formId: '42DEB6BE-3ADB-4BBE-BCD2-90B46ED81AC8'.toLocaleLowerCase() },
                        { name: 'facility', value: 923720000, formId: 'e3ed4465-f4d0-4aa7-8f16-07bf6dd60d13'.toLocaleLowerCase() },
                        { name: 'distributor', value: 923720003, formId: '25B4ED80-3C95-4D29-8120-6384A813BA12'.toLocaleLowerCase() }
                    ];
                    var currentAccountType = accountTypes.find(function (x) { return x.value == relationType; });
                    if (currentAccountType != undefined) {
                        if (currentFormId != currentAccountType.formId) {
                            var items = formContext.ui.formSelector.items.get();
                            for (var i in items) {
                                if (items[i].getId() == currentAccountType.formId) {
                                    //prevent the dialog about unsaved changes
                                    DoNotSubmitChangedAttributes();
                                    items[i].navigate();
                                }
                            }
                        }
                    }
                }
            }
        }
        /*
         * Changes Submit Mode for all dirty attributes. It can be used to prevent the dialog about unsaved changes
         */
        function DoNotSubmitChangedAttributes() {
            var attributes = Xrm.Page.data.entity.attributes.get();
            for (var _i = 0, attributes_1 = attributes; _i < attributes_1.length; _i++) {
                var attr = attributes_1[_i];
                if (attr.getIsDirty()) {
                    attr.setSubmitMode("never");
                }
            }
        }
        /**
         * Called on change 'Pay to address' field
         * @function Intake.Account.OnChangePayToAddress
         * @returns {void}
        */
        function OnChangePayToAddress(executionContext) {
            var formContext = executionContext.getFormContext();
            var claimspaytoaddress = formContext.getAttribute("ipg_claimspaytoaddress").getValue();
            if (claimspaytoaddress == 923720001) {
                formContext.getAttribute("address2_line3").setValue(null);
                formContext.getAttribute("ipg_claimspaytopostalcode").setValue(null);
                formContext.getAttribute("ipg_claimspaytopostalcode").fireOnChange();
            }
            else {
                Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", "?$select=ipg_value&$filter=ipg_name eq 'IPG Address'").then(function success(result) {
                    if (result.entities.length > 0) {
                        var ipgAddress = result.entities[0].ipg_value;
                        var addressArray = ipgAddress.split(';');
                        var zip;
                        var street;
                        addressArray.forEach(function (item) {
                            if (item.startsWith("ZIP")) {
                                zip = item.substring(4);
                            }
                            else if (item.startsWith("Street")) {
                                street = item.substring(8);
                            }
                        });
                        if (street) {
                            formContext.getAttribute("address2_line3").setValue(street);
                        }
                        if (zip) {
                            Xrm.WebApi.retrieveMultipleRecords("ipg_zipcode", "?$select=ipg_zipcodeid&$filter=ipg_name eq '" + zip + "'").then(function success(results) {
                                if (results.entities.length > 0) {
                                    formContext.getAttribute("ipg_claimspaytopostalcode").setValue([{ entityType: "ipg_zipcode", id: results.entities[0]["ipg_zipcodeid"], name: zip }]);
                                    formContext.getAttribute("ipg_claimspaytopostalcode").fireOnChange();
                                }
                            }, function (error) {
                                console.log(error.message);
                            });
                        }
                    }
                }, function (error) {
                    console.log(error.message);
                });
            }
        }
        Account.OnChangePayToAddress = OnChangePayToAddress;
        function OnChangeType(executionContext) {
            if (executionContext === void 0) { executionContext = null; }
            var fieldCodeAttribute;
            var formContext;
            if (executionContext != null) {
                fieldCodeAttribute = executionContext.getEventSource();
                formContext = executionContext.getFormContext();
            }
            else {
                formContext = Xrm.Page;
                fieldCodeAttribute = formContext.getAttribute("ipg_type");
            }
        }
        Account.OnChangeType = OnChangeType;
        /*
         * Called on change of account.ipg_carriersupportedplantypes
         * @function Intake.Account.OnCarrierSupportedPlanTypesChange
         * @returns {void}
         */
        function OnCarrierSupportedPlanTypesChange(executionContext) {
            var formContext = executionContext.getFormContext();
            hideWorkersCompOrOtherPlanTypes(formContext);
            var selectedPlanTypes = formContext.getAttribute("ipg_carriersupportedplantypes").getValue();
            if (selectedPlanTypes == null || selectedPlanTypes.length == 0) {
                resetPlanTypeOptions(formContext);
            }
            updateBillPatient(formContext);
        }
        Account.OnCarrierSupportedPlanTypesChange = OnCarrierSupportedPlanTypesChange;
        function hideWorkersCompOrOtherPlanTypes(formContext) {
            var planTypesControl = formContext.getControl("ipg_carriersupportedplantypes");
            if (planTypesControl) {
                var controlPlanTypeOptions = planTypesControl.getOptions();
                var selectedPlanTypes = formContext.getAttribute("ipg_carriersupportedplantypes").getValue();
                if (selectedPlanTypes && selectedPlanTypes.length > 0) {
                    if (selectedPlanTypes.indexOf(workersCompPlanTypeValue) == -1) {
                        //remove Workers Comp option
                        if (optionsContainValue(controlPlanTypeOptions, workersCompPlanTypeValue)) {
                            planTypesControl.removeOption(workersCompPlanTypeValue);
                        }
                    }
                    else {
                        //remove all non Workers Comp options
                        for (var i = 0; i < controlPlanTypeOptions.length; i++) {
                            if (controlPlanTypeOptions[i].value && controlPlanTypeOptions[i].value != workersCompPlanTypeValue) {
                                planTypesControl.removeOption(controlPlanTypeOptions[i].value);
                            }
                        }
                    }
                }
            }
        }
        function updateBillPatient(formContext) {
            if (formContext.getAttribute("ipg_carriersupportedplantypes") && formContext.getAttribute("ipg_statementtopatient")) {
                var selectedPlanTypes = formContext.getAttribute("ipg_carriersupportedplantypes").getValue();
                formContext.getAttribute("ipg_statementtopatient").setValue(selectedPlanTypes == null || selectedPlanTypes.indexOf(workersCompPlanTypeValue) == -1);
            }
        }
        function optionsContainValue(array, optionValue) {
            for (var i = 0; i < array.length; i++) {
                if (array[i].value == optionValue) {
                    return true;
                }
            }
            return false;
        }
        function resetPlanTypeOptions(formContext) {
            //get all options
            var allPlanTypeOptions = formContext.getAttribute("ipg_carriersupportedplantypes").getOptions();
            //get disabled options and add only enabled
            var query = "?$select=ipg_disabledplantypes&$filter=ipg_name eq 'Disabled plan types'";
            Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", query)
                .then(function (result) {
                var disabledPlanTypes = [];
                if (result.entities.length > 0) {
                    var disabledPlanTypesString = result.entities[0].ipg_disabledplantypes;
                    if (disabledPlanTypesString) {
                        disabledPlanTypes = disabledPlanTypesString.split(",");
                    }
                }
                var planTypesControl = formContext.getControl("ipg_carriersupportedplantypes");
                planTypesControl.clearOptions();
                for (var i = 0; i < allPlanTypeOptions.length; i++) {
                    if (disabledPlanTypes.indexOf(allPlanTypeOptions[i].value.toString()) == -1) {
                        planTypesControl.addOption(allPlanTypeOptions[i]);
                    }
                }
            }, function (error) {
                console.log(error.message);
            });
        }
        function ShowLegacyCarrierAuditInformation(primaryControl) {
            var env = Intake.Utility.GetEnvironment();
            var envSuffix;
            if (env) {
                envSuffix = '-' + env;
            }
            else {
                envSuffix = '';
            }
            var formContext = primaryControl;
            var incidentId = formContext.data.entity.getId();
            incidentId = incidentId.replace(/[{}]/g, "");
            Xrm.Navigation.openUrl("https://insight-auditinfo" + envSuffix + ".azurewebsites.net/carrierauditinfo/index/" + incidentId);
        }
        Account.ShowLegacyCarrierAuditInformation = ShowLegacyCarrierAuditInformation;
        function getDocuments(facilityId, type) {
            return __awaiter(this, void 0, void 0, function () {
                var fetch;
                return __generator(this, function (_a) {
                    fetch = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\n                  <entity name='ipg_document'>\n                    <all-attributes/>\n                    <order attribute='ipg_name' descending='false' />\n                    <filter type='and'>\n                      <condition attribute='ipg_facilityid' operator='eq' value='" + facilityId + "' />\n                      <condition attribute='statecode' operator='eq' value='0' />\n                    </filter>\n                    <link-entity name='ipg_documenttype' from='ipg_documenttypeid' to='ipg_documenttypeid' link-type='inner' alias='aa'>\n                      <filter type='and'>\n                        <condition attribute='ipg_documenttypeabbreviation' operator='eq' value='" + type + "' />\n                      </filter>\n                    </link-entity>\n                  </entity>\n                </fetch>";
                    return [2 /*return*/, Xrm.WebApi.retrieveMultipleRecords("ipg_document", "?fetchXml=" + fetch)];
                });
            });
        }
        function setCarrierAccepted(form) {
            return __awaiter(this, void 0, void 0, function () {
                var customertypecode, userTeams, isCarrierAdminTeam_1, hasAdminRole_1, carrierAccepted;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            customertypecode = form.getAttribute("customertypecode");
                            if (!(customertypecode && customertypecode.getValue() == 923720001)) return [3 /*break*/, 3];
                            return [4 /*yield*/, GetuserTeams()];
                        case 1:
                            userTeams = _a.sent();
                            isCarrierAdminTeam_1 = userTeams.filter(function (p) { return p == "Carrier Services"; }).length > 0;
                            return [4 /*yield*/, checkUserRole("System Administrator")];
                        case 2:
                            hasAdminRole_1 = _a.sent();
                            carrierAccepted = form.getAttribute("ipg_carrieraccepted");
                            carrierAccepted.setRequiredLevel(isCarrierAdminTeam_1 || hasAdminRole_1 ? "required" : "none");
                            carrierAccepted.controls.forEach(function (ctrl) { return ctrl.setDisabled(!(isCarrierAdminTeam_1 || hasAdminRole_1)); });
                            _a.label = 3;
                        case 3: return [2 /*return*/];
                    }
                });
            });
        }
        function GetuserTeams() {
            return __awaiter(this, void 0, void 0, function () {
                var fetchXml, teams;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\">\n              <entity name=\"team\">\n                <attribute name=\"name\" />\n                <order attribute=\"name\" descending=\"false\" />\n                <link-entity name=\"teammembership\" from=\"teamid\" to=\"teamid\" visible=\"false\" intersect=\"true\">\n                  <link-entity name=\"systemuser\" from=\"systemuserid\" to=\"systemuserid\" alias=\"ai\">\n                    <filter type=\"and\">\n                      <condition attribute=\"systemuserid\" operator=\"eq-userid\" />\n                    </filter>\n                  </link-entity>\n                </link-entity>\n              </entity>\n            </fetch>";
                            return [4 /*yield*/, Xrm.WebApi.online.retrieveMultipleRecords("team", "?fetchXml=" + fetchXml)];
                        case 1:
                            teams = _a.sent();
                            return [2 /*return*/, teams.entities.map(function (p) { return p.name; })];
                    }
                });
            });
        }
        function checkUserRole(roleName) {
            return __awaiter(this, void 0, void 0, function () {
                var personalRoles, result, i, targetUserRole;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            personalRoles = Xrm.Utility.getGlobalContext().getUserRoles();
                            result = false;
                            i = 0;
                            _a.label = 1;
                        case 1:
                            if (!(i < personalRoles.length)) return [3 /*break*/, 4];
                            return [4 /*yield*/, GetRoleName(personalRoles[i])];
                        case 2:
                            targetUserRole = _a.sent();
                            if (roleName.toLowerCase() == targetUserRole.toLowerCase()) {
                                result = true;
                                return [3 /*break*/, 4];
                            }
                            _a.label = 3;
                        case 3:
                            i++;
                            return [3 /*break*/, 1];
                        case 4: return [2 /*return*/, result];
                    }
                });
            });
        }
        function GetRoleName(roleId) {
            return __awaiter(this, void 0, void 0, function () {
                var roleNames;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("role", "?$select=name&$filter=roleid eq " + roleId)];
                        case 1:
                            roleNames = _a.sent();
                            if (roleNames.entities.length == 0) {
                                return [2 /*return*/, ""];
                            }
                            return [2 /*return*/, roleNames.entities[0].name];
                    }
                });
            });
        }
        function setDefaultManufacturerPaymentThreshold(form) {
            if (form.getAttribute("customertypecode").getValue() != null) {
                //  if (form.getAttribute("customertypecode").setValue() != null) {
                var customertypecode = form.getAttribute("customertypecode");
                if (((form.ui.getFormType() == 1)) && customertypecode && customertypecode.getValue() == 923720002) { //Manufacturer
                    if (form.getAttribute("ipg_manufacturerpaymentthreshold") != null) {
                        form.getAttribute("ipg_manufacturerpaymentthreshold").setValue(0);
                    }
                }
            }
        }
        //function setDefaultManufacturerPaymentThreshold(form: Xrm.FormContext) {
        //  const customertypecode = form.getAttribute<Xrm.Attributes.OptionSetAttribute>("customertypecode");
        //  if (((form.ui.getFormType() == 1)) && customertypecode && customertypecode.getValue() == 923720002) {//Manufacturer
        //    form.getAttribute("ipg_manufacturerpaymentthreshold").setValue(0);
        //  }
        //}
        function setSubmitModeForDirtyFields(formContext) {
            if (formContext.ui.getFormType() != 1) {
                dirtyFields.forEach(function (field) {
                    var attr = formContext.getAttribute(field);
                    if (attr && attr.getIsDirty()) {
                        attr.setSubmitMode('never');
                    }
                });
            }
        }
        /**
         * Called on dirty field change
         * @function Intake.Account.onDirtyFieldChange
         * @returns {void}
        */
        function onDirtyFieldChange(executionContext) {
            var formContext = executionContext.getFormContext();
            if (formContext.ui.getFormType() != 1) {
                var field = executionContext.getEventSource()._attributeName;
                var attr = formContext.getAttribute(field);
                if (!attr.getIsDirty()) {
                    attr.setSubmitMode('dirty');
                }
            }
        }
        Account.onDirtyFieldChange = onDirtyFieldChange;
        /**
         * Called on on Sub Grid FacilityPhysician Record Selected
         * @function Intake.Account.onSubGridFacilityPhysicianRecordSelect
         * @returns {void}
        */
        function onSubGridFacilityPhysicianRecordSelect(executionContext) {
            var formContext = executionContext.getFormContext();
            var currentEntity = formContext.data.entity;
            currentEntity.attributes.forEach(function (attribute, i) {
                if (attribute.getName() !== "ipg_status") {
                    attribute.controls.get(0).setDisabled(true);
                }
            });
        }
        Account.onSubGridFacilityPhysicianRecordSelect = onSubGridFacilityPhysicianRecordSelect;
        function setRelationshipType(formContext) {
            var _a;
            var formId = formContext.ui.formSelector.getCurrentItem().getId();
            if (formId === FormIds.carrier) {
                (_a = formContext.getAttribute("customertypecode")) === null || _a === void 0 ? void 0 : _a.setValue(923720001); // Carrier
            }
        }
        function SetRemittanceAddress(primaryControl) {
            return __awaiter(this, void 0, void 0, function () {
                var address, formContext, payToName, payToAddress, payToCity, payToStateId, payToZipId, payToPhone, state, zip;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, Intake.Utility.GetRemittanceAddress(Xrm.WebApi)];
                        case 1:
                            address = _a.sent();
                            formContext = primaryControl;
                            payToName = formContext.getAttribute("ipg_paytoname");
                            payToAddress = formContext.getAttribute("ipg_paytoaddress");
                            payToCity = formContext.getAttribute("ipg_paytocity");
                            payToStateId = formContext.getAttribute("ipg_paytostateid");
                            payToZipId = formContext.getAttribute("ipg_paytozipid");
                            payToPhone = formContext.getAttribute("ipg_paytophone");
                            if (payToName && address.name) {
                                payToName.setValue(address.name);
                            }
                            if (payToAddress && address.address) {
                                payToAddress.setValue(address.address);
                            }
                            if (payToCity && address.city) {
                                payToCity.setValue(address.city);
                            }
                            if (!(payToStateId && address.state)) return [3 /*break*/, 3];
                            return [4 /*yield*/, Intake.Utility.GetState(Xrm.WebApi, address.state)];
                        case 2:
                            state = _a.sent();
                            if (state !== null && state.id && state.stateName) {
                                payToStateId.setValue([{
                                        id: state.id.replace("{", "").replace("}", ""),
                                        entityType: "ipg_state",
                                        name: state.stateName
                                    }]);
                            }
                            _a.label = 3;
                        case 3:
                            if (!(payToZipId && address.zip)) return [3 /*break*/, 5];
                            return [4 /*yield*/, Intake.Utility.GetZip(Xrm.WebApi, address.zip)];
                        case 4:
                            zip = _a.sent();
                            if (zip !== null && zip.id && zip.zipName) {
                                payToZipId.setValue([{
                                        id: zip.id.replace("{", "").replace("}", ""),
                                        entityType: "ipg_zipcode",
                                        name: zip.zipName
                                    }]);
                            }
                            _a.label = 5;
                        case 5:
                            if (payToPhone && address.phone) {
                                payToPhone.setValue(address.phone);
                            }
                            return [2 /*return*/];
                    }
                });
            });
        }
        Account.SetRemittanceAddress = SetRemittanceAddress;
        function disableFormSelector(formContext) {
            formContext.ui.formSelector.items.forEach(function (formItem) { return formItem.setVisible(false); });
        }
        function DirectBill(formContext) {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j;
            if (((_a = formContext.getAttribute("ipg_manufacturerisparticipating")) === null || _a === void 0 ? void 0 : _a.getValue()) == true) {
                (_b = formContext.getAttribute("address1_line2")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("required");
                (_c = formContext.getAttribute("address2_city")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("required");
                (_d = formContext.getAttribute("address2_stateorprovince")) === null || _d === void 0 ? void 0 : _d.setRequiredLevel("required");
                (_e = formContext.getAttribute("ipg_melissaaccountzipcodeid")) === null || _e === void 0 ? void 0 : _e.setRequiredLevel("required");
            }
            else {
                (_f = formContext.getAttribute("address1_line2")) === null || _f === void 0 ? void 0 : _f.setRequiredLevel("none");
                (_g = formContext.getAttribute("address2_city")) === null || _g === void 0 ? void 0 : _g.setRequiredLevel("none");
                (_h = formContext.getAttribute("address2_stateorprovince")) === null || _h === void 0 ? void 0 : _h.setRequiredLevel("none");
                (_j = formContext.getAttribute("ipg_melissaaccountzipcodeid")) === null || _j === void 0 ? void 0 : _j.setRequiredLevel("none");
            }
        }
        Account.DirectBill = DirectBill;
    })(Account = Intake.Account || (Intake.Account = {}));
})(Intake || (Intake = {}));
