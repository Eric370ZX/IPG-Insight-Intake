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
 * @namespace Intake.Referral
 */
var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        var pif1formId = '5650e49e-0ada-45b8-8aa7-2e40a5617c65';
        var pif2formId = '7266869e-f61a-45e5-84f9-c7c5446dbfc5';
        ;
        ;
        ;
        /**
         * Called on load form
         * @function Intake.Referral.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            formContext.data.addOnLoad(function (event) { return ChooseForm(event.getFormContext()); });
            ChooseForm(formContext);
            CreateFormVisibility(formContext);
            IntakePIFStep2Controls(formContext);
            var statusAttr = formContext.getAttribute("ipg_casestatus");
            statusAttr === null || statusAttr === void 0 ? void 0 : statusAttr.addOnChange(function () {
                lockAndSetFormNotificationIfClosed(formContext);
                DisplayOwnerField(formContext);
            });
            statusAttr === null || statusAttr === void 0 ? void 0 : statusAttr.fireOnChange();
            ConfigureStep1Form(formContext);
            ConfigureStep2Form(formContext);
            SetFaciltyName(formContext);
            setCaseStatusDisplayedValue(formContext);
            showTimeLine(formContext);
            // SetPhysicianFromFacility(formContext);
            addCarriersPreSearch(formContext);
            var primaryCategory = formContext.getAttribute("ipg_primarycarriercategory");
            primaryCategory === null || primaryCategory === void 0 ? void 0 : primaryCategory.addOnChange(function () {
                formContext.getAttribute("ipg_carrierid").setValue(null);
            });
            var secondaryCategory = formContext.getAttribute("ipg_secondarycarriercategory");
            secondaryCategory.addOnChange(function () {
                formContext.getAttribute("ipg_carriername2id").setValue(null);
            });
            var ownerid = formContext.getAttribute("ownerid");
            ownerid === null || ownerid === void 0 ? void 0 : ownerid.setSubmitMode("never");
            ownerid === null || ownerid === void 0 ? void 0 : ownerid.addOnChange(onChangeOwner);
            var focusOnTab = window.localStorage.getItem('focusOnTab');
            if (focusOnTab != null) {
                Xrm.Page.ui.tabs.get(focusOnTab).setFocus();
                window.localStorage.removeItem('focusOnTab');
                window.localStorage.removeItem('back');
            }
            lockAndSetFormNotificationIfClosed(formContext);
        }
        Referral.OnLoadForm = OnLoadForm;
        function ConfigureStep1Form(formContext) {
            if (formContext.ui.formSelector.getCurrentItem().getId() == pif1formId) {
                var carrierAttr = formContext.getAttribute("ipg_carrierid");
                carrierAttr === null || carrierAttr === void 0 ? void 0 : carrierAttr.addOnChange(OnChangePrimaryCarrier);
                carrierAttr === null || carrierAttr === void 0 ? void 0 : carrierAttr.fireOnChange();
            }
        }
        function ConfigureStep2Form(formContext) {
            if (formContext.ui.formSelector.getCurrentItem().getId() == pif2formId) {
                var carrierAttr = formContext.getAttribute("ipg_carrierid");
                carrierAttr === null || carrierAttr === void 0 ? void 0 : carrierAttr.addOnChange(OnChangeCarrier);
                OnChangeCarrier(null, formContext);
                var carrierAttr = formContext.getAttribute("ipg_carriername2id");
                carrierAttr === null || carrierAttr === void 0 ? void 0 : carrierAttr.addOnChange(OnChangeSecondCarrier);
                carrierAttr === null || carrierAttr === void 0 ? void 0 : carrierAttr.fireOnChange();
            }
        }
        function SetFaciltyName(formContext) {
            if (formContext.ui.getFormType() == 1 /* Create */) {
                var guid = localStorage.facilityid;
                if (guid && guid != "undefined") {
                    var object = new Array();
                    object[0] = new Object();
                    object[0].id = guid;
                    object[0].name = localStorage.facilityname;
                    object[0].entityType = "account";
                    formContext.getAttribute("ipg_facilityid").setValue(null);
                    formContext.getAttribute("ipg_facilityid").setValue(object);
                    localStorage.removeItem("facilityname");
                    localStorage.removeItem("facilityid");
                }
            }
        }
        function lockAndSetFormNotificationIfClosed(formContext) {
            var _a;
            return __awaiter(this, void 0, void 0, function () {
                var status, docId;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            status = (_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue();
                            if (!(status == 923720001)) return [3 /*break*/, 2];
                            formContext.ui.setFormNotification("Referral is Closed", "WARNING", "closed");
                            EnableFieldsIfClosed(formContext);
                            formContext.getControl("ipg_carriername2id").setDisabled(true);
                            formContext.getControl("ipg_memberidnumber2").setDisabled(true);
                            docId = formContext.getAttribute("ipg_sourcedocumentid").getValue();
                            return [4 /*yield*/, Xrm.WebApi.updateRecord("ipg_document", Intake.Utility.removeCurlyBraces(docId[0].id), { "_ipg_referralid_value": null })];
                        case 1:
                            _b.sent();
                            return [3 /*break*/, 3];
                        case 2:
                            formContext.ui.clearFormNotification("closed");
                            _b.label = 3;
                        case 3: return [2 /*return*/];
                    }
                });
            });
        }
        /**
         * Called on change patient last name
         * @function Intake.Referral.OnChangePatientLastName
         * @returns {void}
        */
        function OnChangePatientLastName(executionContext) {
            var formContext = executionContext.getFormContext();
            addFiltersToPatientField(formContext);
        }
        Referral.OnChangePatientLastName = OnChangePatientLastName;
        /**
         * Called on change patient first name
         * @function Intake.Referral.OnChangePatientFirstName
         * @returns {void}
        */
        function OnChangePatientFirstName(executionContext) {
            var formContext = executionContext.getFormContext();
            addFiltersToPatientField(formContext);
        }
        Referral.OnChangePatientFirstName = OnChangePatientFirstName;
        /**
         * Called on change patient middle name
         * @function Intake.Referral.OnChangePatientMiddleName
         * @returns {void}
        */
        function OnChangePatientMiddleName(executionContext) {
            var formContext = executionContext.getFormContext();
            addFiltersToPatientField(formContext);
        }
        Referral.OnChangePatientMiddleName = OnChangePatientMiddleName;
        /**
         * Called on change patient dob
         * @function Intake.Referral.OnChangePatientDOB
         * @returns {void}
        */
        function OnChangePatientDOB(executionContext) {
            var formContext = executionContext.getFormContext();
            addFiltersToPatientField(formContext);
        }
        Referral.OnChangePatientDOB = OnChangePatientDOB;
        //PIF Step 1 Form
        function OnChangePrimaryCarrier(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            var carrierVal = (_a = formContext.getAttribute("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (carrierVal && carrierVal.length > 0) {
                Xrm.WebApi.retrieveRecord(carrierVal[0].entityType, carrierVal[0].id, "?$select=ipg_carriertype")
                    .then(function (carrier) {
                    var _a, _b, _c, _d, _e, _f;
                    if (carrier["ipg_carriertype"] == 427880001 /* WorkersComp */) //Workers Comp
                     {
                        (_a = formContext.getAttribute("ipg_carriername2id")) === null || _a === void 0 ? void 0 : _a.setValue(null);
                        (_b = formContext.getControl("ipg_carriername2id")) === null || _b === void 0 ? void 0 : _b.setDisabled(true);
                        (_c = formContext.getAttribute("ipg_memberidnumber2")) === null || _c === void 0 ? void 0 : _c.setValue("");
                        (_d = formContext.getControl("ipg_memberidnumber2")) === null || _d === void 0 ? void 0 : _d.setDisabled(true);
                    }
                    else {
                        (_e = formContext.getControl("ipg_carriername2id")) === null || _e === void 0 ? void 0 : _e.setDisabled(false);
                        (_f = formContext.getControl("ipg_memberidnumber2")) === null || _f === void 0 ? void 0 : _f.setDisabled(false);
                    }
                });
            }
        }
        function OnChangeCarrier(executionContext, formContext) {
            var _a, _b, _c, _d;
            var formContext = formContext || executionContext.getFormContext();
            var carrierVal = (_a = formContext.getAttribute("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (executionContext) {
                var dateofinjuryAttr = formContext.getAttribute("ipg_autodateofincident");
                var adjusterNameAttr = formContext.getAttribute("ipg_autoadjustername");
                var adjusterPhoneAttr = formContext.getAttribute("ipg_autoadjusterphone");
                var wcNurseCaseMgrName = formContext.getAttribute("ipg_wcnursecasemgrname");
                var wcNurseCaseMgrPhone = formContext.getAttribute("ipg_wcnursecasemgrphone");
                dateofinjuryAttr.setValue(null);
                adjusterNameAttr.setValue(null);
                adjusterPhoneAttr.setValue(null);
                wcNurseCaseMgrName.setValue(null);
                wcNurseCaseMgrPhone.setValue(null);
            }
            var additionalRefInfSection = (_b = formContext.ui.tabs.get("Referral")) === null || _b === void 0 ? void 0 : _b.sections.get("Additional_Referral_Info");
            if (carrierVal && carrierVal.length > 0) {
                Xrm.WebApi.retrieveRecord(carrierVal[0].entityType, carrierVal[0].id, "?$select=ipg_carriertype")
                    .then(function (carrier) {
                    var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o;
                    if (carrier["ipg_carriertype"] == 427880000 || carrier["ipg_carriertype"] == 427880001) //auto, Workers Comp
                     {
                        (_a = formContext.getControl("ipg_memberidnumber")) === null || _a === void 0 ? void 0 : _a.setVisible(true); //claim #
                        (_b = formContext.getControl("ipg_memberidnumber1")) === null || _b === void 0 ? void 0 : _b.setVisible(false); //memberid
                        additionalRefInfSection === null || additionalRefInfSection === void 0 ? void 0 : additionalRefInfSection.setVisible(true);
                        (_c = formContext.getAttribute("ipg_carriername2id")) === null || _c === void 0 ? void 0 : _c.setValue(null);
                        (_d = formContext.getControl("ipg_carriername2id")) === null || _d === void 0 ? void 0 : _d.setDisabled(true);
                        (_e = formContext.getAttribute("ipg_memberidnumber2")) === null || _e === void 0 ? void 0 : _e.setValue("");
                        (_f = formContext.getControl("ipg_memberidnumber2")) === null || _f === void 0 ? void 0 : _f.setDisabled(true);
                        (_g = formContext.getAttribute("ipg_memberidnumber21")) === null || _g === void 0 ? void 0 : _g.setValue("");
                        (_h = formContext.getControl("ipg_memberidnumber21")) === null || _h === void 0 ? void 0 : _h.setDisabled(true);
                    }
                    else {
                        (_j = formContext.getControl("ipg_memberidnumber")) === null || _j === void 0 ? void 0 : _j.setVisible(false); //claim #
                        (_k = formContext.getControl("ipg_memberidnumber1")) === null || _k === void 0 ? void 0 : _k.setVisible(true); //memberid
                        additionalRefInfSection === null || additionalRefInfSection === void 0 ? void 0 : additionalRefInfSection.setVisible(false);
                        (_l = formContext.getControl("ipg_carriername2id")) === null || _l === void 0 ? void 0 : _l.setDisabled(false);
                        (_m = formContext.getControl("ipg_memberidnumber2")) === null || _m === void 0 ? void 0 : _m.setDisabled(false);
                        (_o = formContext.getControl("ipg_memberidnumber21")) === null || _o === void 0 ? void 0 : _o.setDisabled(false);
                    }
                });
            }
            else {
                (_c = formContext.getControl("ipg_memberidnumber")) === null || _c === void 0 ? void 0 : _c.setVisible(false); //claim #
                (_d = formContext.getControl("ipg_memberidnumber1")) === null || _d === void 0 ? void 0 : _d.setVisible(true); //memberid
                additionalRefInfSection === null || additionalRefInfSection === void 0 ? void 0 : additionalRefInfSection.setVisible(false);
            }
        }
        function OnChangeSecondCarrier(executionContext) {
            var _a, _b, _c;
            var formContext = executionContext.getFormContext();
            var carrierVal = (_a = executionContext.getEventSource()) === null || _a === void 0 ? void 0 : _a.getValue();
            if (carrierVal && carrierVal.length > 0) {
                Xrm.WebApi.retrieveRecord(carrierVal[0].entityType, carrierVal[0].id, "?$select=ipg_carriertype")
                    .then(function (carrier) {
                    var _a, _b, _c, _d;
                    if (carrier["ipg_carriertype"] == 427880000 || carrier["ipg_carriertype"] == 427880001) //auto, Workers Comp
                     {
                        (_a = formContext.getControl("ipg_memberidnumber2")) === null || _a === void 0 ? void 0 : _a.setVisible(true); //claim #
                        (_b = formContext.getControl("ipg_memberidnumber21")) === null || _b === void 0 ? void 0 : _b.setVisible(false); //memberid
                    }
                    else {
                        (_c = formContext.getControl("ipg_memberidnumber2")) === null || _c === void 0 ? void 0 : _c.setVisible(false); //claim #
                        (_d = formContext.getControl("ipg_memberidnumber21")) === null || _d === void 0 ? void 0 : _d.setVisible(true); //memberid
                    }
                });
            }
            else {
                (_b = formContext.getControl("ipg_memberidnumber2")) === null || _b === void 0 ? void 0 : _b.setVisible(false); //claim #
                (_c = formContext.getControl("ipg_memberidnumber21")) === null || _c === void 0 ? void 0 : _c.setVisible(true); //memberid
            }
        }
        /**
         * Called on change patient last name
         * @function Intake.Referral.OnChangeSurgeryDate
         * @returns {void}
        */
        function OnChangeSurgeryDate(executionContext) {
            var formContext = executionContext.getFormContext();
            var fieldName = "ipg_surgerydate";
            var surgerydate = formContext.getAttribute(fieldName).getValue();
            if (surgerydate < new Date().setMonth(-6)) {
                formContext.getControl(fieldName).setNotification("Invalid Scheduled DOS entered - outside of Carrier’s Timely Filing range. Please correct and try again.", fieldName);
            }
            else {
                formContext.getControl(fieldName).clearNotification(fieldName);
            }
        }
        Referral.OnChangeSurgeryDate = OnChangeSurgeryDate;
        /**
        * Called on CaseStatus change (fires on form load too)
        * @function Intake.Referral.DisplayOwnerField
        * @returns {void}
        */
        function DisplayOwnerField(formContext) {
            var ownerControl = formContext.getControl("ownerid");
            if (isReferralOpened(formContext)) {
                ownerControl === null || ownerControl === void 0 ? void 0 : ownerControl.setDisabled(false);
            }
            else {
                ownerControl === null || ownerControl === void 0 ? void 0 : ownerControl.setDisabled(true);
            }
        }
        Referral.DisplayOwnerField = DisplayOwnerField;
        /**
        * Called on change patient last name
        * @function Intake.Referral.DisableAllFieldsIfFinishGate
        * @returns {void}
        */
        function EnableFieldsIfClosed(formContext) {
            var caseStatus = formContext.getAttribute("ipg_casestatus");
            if (caseStatus && caseStatus.getValue() === 923720001) { //Closed
                formContext.ui.controls.forEach(function (control) {
                    if (control && control.getDisabled && !control.getDisabled()) {
                        control.setDisabled(true);
                    }
                });
            }
        }
        Referral.EnableFieldsIfClosed = EnableFieldsIfClosed;
        /**
         * add filters to patient field
         * @function Intake.Referral.OnChangePatientDOB
         * @returns {void}
        */
        function addFiltersToPatientField(formContext) {
            function addCustomLookupFilter(formContext) {
                var filters = [
                    '<condition attribute="ipg_contacttypeidname" operator="like" value="%Patient%" />',
                ];
                if (patientLastNameValue)
                    filters.push("<condition attribute=\"lastname\" operator=\"eq\" value=\"" + patientLastNameValue + "\" />");
                if (patientMiddleNameValue)
                    filters.push("<condition attribute=\"middlename\" operator=\"eq\" value=\"" + patientMiddleNameValue + "\" />");
                if (patientFirstNameValue)
                    filters.push("<condition attribute=\"firstname\" operator=\"eq\" value=\"" + patientFirstNameValue + "\" />");
                if (patientDateOfBirthValue) {
                    var dateString = (patientDateOfBirthValue.getFullYear() + '-' + ('0' + (patientDateOfBirthValue.getMonth() + 1)).slice(-2) + '-' + ('0' + patientDateOfBirthValue.getDate()).slice(-2));
                    filters.push("<condition attribute=\"birthdate\" operator=\"eq\" value=\"" + dateString + "\" />");
                }
                var filterXml = "<filter type=\"and\">" + filters.join('') + "</filter>";
                formContext.getControl("ipg_patientid").addCustomFilter(filterXml, "contact");
            }
            var patientLastNameValue = formContext.getAttribute("ipg_patientlastname").getValue();
            var patientMiddleNameValue = formContext.getAttribute("ipg_patientmiddlename").getValue();
            var patientFirstNameValue = formContext.getAttribute("ipg_patientfirstname").getValue();
            var patientDateOfBirthValue = formContext.getAttribute("ipg_patientdateofbirth").getValue();
            if (patientLastNameValue || patientMiddleNameValue || patientFirstNameValue || patientDateOfBirthValue) {
                formContext.getControl("ipg_patientid").addPreSearch(function () {
                    addCustomLookupFilter(formContext);
                });
            }
        }
        /**
       * Called on saving form
       * @function Intake.Referral.OnSaveForm
       * @returns {void}
      */
        Referral.showWarningMessage = true;
        function OnSaveForm(executionContext) {
            var _a, _b, _c;
            var formContext = executionContext.getFormContext();
            var saveEvent = executionContext.getEventArgs();
            //Prevent auto-save
            if (saveEvent.getSaveMode() == 70 /* AutoSave */ || saveEvent.getSaveMode() == 2 /* SaveAndClose */) {
                saveEvent.preventDefault();
                return;
            }
            //Allow activation
            if (saveEvent.getSaveMode() == 6 /* Reactivate */) {
                return;
            }
            var caseStatus = (_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue();
            var isCaseStatusDirty = (_b = formContext.getAttribute("ipg_casestatus")) === null || _b === void 0 ? void 0 : _b.getIsDirty();
            var isCaseStatusDisplayedDirty = (_c = formContext.getAttribute("ipg_casestatusdisplayed")) === null || _c === void 0 ? void 0 : _c.getIsDirty();
            if (caseStatus == 923720001 /* Closed */ && !isCaseStatusDirty && !isCaseStatusDisplayedDirty) {
                Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "You are not able to save closed referral." }).then(function () {
                    return;
                });
            }
            if (Referral.showWarningMessage && formContext.data.entity.getIsDirty()) {
                saveEvent.preventDefault();
                Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "Referral information has been saved.  Please click 'Submit' to create a referral" }).then(function () {
                    Referral.showWarningMessage = false;
                    formContext.data.save().then(function () {
                    });
                })
                    .then(function () {
                    Referral.showWarningMessage = true;
                });
            }
        }
        Referral.OnSaveForm = OnSaveForm;
        /**
       * Chooses form depending on 'ipg_lifecyclestepid' field value
       * @function Intake.Referral.ChooseForm
       * @returns {void}
       */
        function ChooseForm(formContext) {
            var ipg_lifecyclestepid = formContext.getAttribute("ipg_lifecyclestepid");
            var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
            // Default Form
            var currentFormRequired = pif1formId;
            if (ipg_lifecyclestepid) {
                var ipg_lifecyclestepidValue = ipg_lifecyclestepid.getValue();
                if (ipg_lifecyclestepidValue && ipg_lifecyclestepidValue.length > 0) {
                    var ipg_lifecyclestepName_1 = ipg_lifecyclestepidValue[0].name.toLocaleLowerCase();
                    var lifecycleStepForms = [
                        { name: 'intake step 1', formId: pif1formId },
                        { name: 'ehr refferal', formId: pif1formId },
                    ];
                    var lifecycleStepForm = lifecycleStepForms.find(function (x) { return x.name == ipg_lifecyclestepName_1; });
                    // Form for all other lifecycle steps
                    currentFormRequired = pif2formId;
                    if (!!lifecycleStepForm) {
                        currentFormRequired = lifecycleStepForm.formId;
                    }
                }
            }
            if (currentFormId != currentFormRequired) {
                var items = formContext.ui.formSelector.items.get();
                for (var i in items) {
                    if (items[i].getId() == currentFormRequired) {
                        var attributes = Xrm.Page.data.entity.attributes.get();
                        if (attributes != null) {
                            for (var _i = 0, attributes_1 = attributes; _i < attributes_1.length; _i++) {
                                var a = attributes_1[_i];
                                if (a.getIsDirty())
                                    a.setSubmitMode('never');
                            }
                        }
                        items[i].navigate();
                    }
                }
            }
        }
        /**
        * Changing form layout based on create form type
        * @function Intake.Referral.CreateFormVisibility
        * @returns {void}
        */
        function CreateFormVisibility(formContext) {
            var _a, _b, _c, _d, _e, _f, _g;
            var formType = formContext.ui.getFormType();
            //if create form
            if (formType == 1 /* Create */) {
                (_a = formContext.ui.tabs.get("GatingLogTab")) === null || _a === void 0 ? void 0 : _a.setVisible(false);
                (_b = formContext.ui.tabs.get("TasksTab")) === null || _b === void 0 ? void 0 : _b.setVisible(false);
                (_c = formContext.ui.tabs.get("Documents")) === null || _c === void 0 ? void 0 : _c.setVisible(false);
                (_d = formContext.ui.tabs.get("Dev")) === null || _d === void 0 ? void 0 : _d.setVisible(false);
                formContext.ui.headerSection.setBodyVisible(false);
                formContext.ui.headerSection.setCommandBarVisible(true);
                formContext.ui.setFormNotification('Enter the initial Referral information and Submit for processing.', 'INFO', '');
            }
            else {
                (_e = formContext.ui.tabs.get("GatingLogTab")) === null || _e === void 0 ? void 0 : _e.setVisible(false);
                (_f = formContext.ui.tabs.get("TasksTab")) === null || _f === void 0 ? void 0 : _f.setVisible(true);
                (_g = formContext.ui.tabs.get("Documents")) === null || _g === void 0 ? void 0 : _g.setVisible(true);
                formContext.ui.headerSection.setBodyVisible(true);
                formContext.ui.headerSection.setCommandBarVisible(true);
            }
        }
        /**
       * Called on load form (Intake.Referral.OnLoadForm)
       * @function Intake.Referral.IntakePIFStep2Controls
       * @returns {void}
       */
        function IntakePIFStep2Controls(formContext) {
            var fieldsToHideShow = ["ipg_patientaddress", "ipg_melissapatientcity", "ipg_melissapatientstate", "ipg_melissapatientzipcodeid", "ipg_homephone", "ipg_workphone", "ipg_cellphone", "ipg_email",
                "ipg_dxcodeid1", "ipg_carriername2id", "ipg_memberidnumber2", "ipg_groupidnumber2", "ipg_facility_auth2", "WebResource_Insurance2Information"];
            var homePhone = formContext.getAttribute("ipg_homephone");
            var workPhone = formContext.getAttribute("ipg_workphone");
            var cellPhone = formContext.getAttribute("ipg_cellphone");
            if (formContext.ui.getFormType() == 1 /* Create */) {
                homePhone.setRequiredLevel("none");
                workPhone.setRequiredLevel("none");
                cellPhone.setRequiredLevel("none");
                fieldsToHideShow.forEach(function (field) {
                    var control = formContext.getControl(field);
                    if (control && control.getVisible())
                        control === null || control === void 0 ? void 0 : control.setVisible(false);
                    var attribute = formContext.getAttribute(field);
                    if (attribute && attribute.getRequiredLevel())
                        attribute === null || attribute === void 0 ? void 0 : attribute.setRequiredLevel("none");
                });
                setSectionVisibility(formContext, "Referral", "Additional_Referral_Info", false);
            }
            else if (formContext.ui.getFormType() == 2 /* Update */) {
                //check for case created
                fieldsToHideShow.forEach(function (field) {
                    var control = formContext.getControl(field);
                    if (control && !control.getVisible()) {
                        control === null || control === void 0 ? void 0 : control.setVisible(true);
                    }
                });
                if (homePhone.getValue()) {
                    workPhone.setRequiredLevel("none");
                    cellPhone.setRequiredLevel("none");
                }
                else if (workPhone.getValue()) {
                    homePhone.setRequiredLevel("none");
                    cellPhone.setRequiredLevel("none");
                }
                else if (cellPhone.getValue()) {
                    homePhone.setRequiredLevel("none");
                    workPhone.setRequiredLevel("none");
                }
                setSectionVisibility(formContext, "Referral", "Additional_Referral_Info", true);
            }
        }
        function setSectionVisibility(formContext, tabId, sectionId, isVisible) {
            var tab = formContext.ui.tabs.get(tabId);
            if (tab && tab.sections && tab.sections.getLength() > 0) {
                var section = tab.sections.get(sectionId);
                if (section && (section === null || section === void 0 ? void 0 : section.setVisible)) {
                    section === null || section === void 0 ? void 0 : section.setVisible(isVisible);
                }
            }
        }
        /**
        * Called on change on Home Phone
        * @function Intake.Referral.onChangeHomePhone
        * @returns {void}
        */
        function onChangeHomePhone(executionContext) {
            var formContext = executionContext.getFormContext();
            var homePhone = formContext.getAttribute("ipg_homephone");
            var workPhone = formContext.getAttribute("ipg_workphone");
            var cellPhone = formContext.getAttribute("ipg_cellphone");
            if (homePhone.getValue()) {
                workPhone.setRequiredLevel("none");
                cellPhone.setRequiredLevel("none");
            }
            else {
                workPhone.setRequiredLevel("required");
                cellPhone.setRequiredLevel("required");
            }
        }
        Referral.onChangeHomePhone = onChangeHomePhone;
        /**
        * Called on change on Work Phone
        * @function Intake.Referral.onChangeWorkPhone
        * @returns {void}
        */
        function onChangeWorkPhone(executionContext) {
            var formContext = executionContext.getFormContext();
            var homePhone = formContext.getAttribute("ipg_homephone");
            var workPhone = formContext.getAttribute("ipg_workphone");
            var cellPhone = formContext.getAttribute("ipg_cellphone");
            if (workPhone.getValue()) {
                homePhone.setRequiredLevel("none");
                cellPhone.setRequiredLevel("none");
            }
            else {
                homePhone.setRequiredLevel("required");
                cellPhone.setRequiredLevel("required");
            }
        }
        Referral.onChangeWorkPhone = onChangeWorkPhone;
        /**
         * Called on change on Cell Phone
         * @function Intake.Referral.onChangeCellPhone
         * @returns {void}
        */
        function onChangeCellPhone(executionContext) {
            var formContext = executionContext.getFormContext();
            var homePhone = formContext.getAttribute("ipg_homephone");
            var workPhone = formContext.getAttribute("ipg_workphone");
            var cellPhone = formContext.getAttribute("ipg_cellphone");
            if (cellPhone.getValue()) {
                homePhone.setRequiredLevel("none");
                workPhone.setRequiredLevel("none");
            }
            else {
                homePhone.setRequiredLevel("required");
                workPhone.setRequiredLevel("required");
            }
        }
        Referral.onChangeCellPhone = onChangeCellPhone;
        /**
        * Called on Dx Code change
        * @function Intake.Referral.OnDxChange
        * @returns {void}
       */
        function OnDxChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var DxCodeName = executionContext.getEventSource().getName();
            var index = Number(DxCodeName.substr(DxCodeName.length - 1, 1));
            var DxCodeValue = formContext.getAttribute("ipg_dxcodeid" + index.toString()).getValue();
            formContext.getControl("ipg_dxcodeid" + index.toString()).clearNotification("ipg_dxcodeid" + index.toString() + "dx");
            if (DxCodeValue) {
                for (var i = 1; i <= 6; i++) {
                    var currentDxCodeValue = formContext.getAttribute("ipg_dxcodeid" + i.toString()).getValue();
                    if ((index != i) && (currentDxCodeValue) && (currentDxCodeValue[0].id == DxCodeValue[0].id)) {
                        formContext.getControl("ipg_dxcodeid" + index.toString()).setNotification("This Dx code has been already selected", "ipg_dxcodeid" + index.toString() + "dx");
                        formContext.getAttribute("ipg_dxcodeid" + index.toString()).setValue(null);
                    }
                }
            }
        }
        Referral.OnDxChange = OnDxChange;
        function OnPatientZipCodeChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var PatientZipCode = formContext.getAttribute("ipg_melissapatientzipcodeid");
            if (PatientZipCode.getValue()) {
                var zipId = PatientZipCode.getValue()[0].id;
                var zipName = PatientZipCode.getValue()[0].name;
                formContext.getAttribute("ipg_displayzipcodename").setValue(zipName);
                formContext.getAttribute("ipg_displayzipcodeid").setValue(zipId);
                Xrm.WebApi.retrieveRecord("ipg_melissazipcode", Intake.Utility.removeCurlyBraces(PatientZipCode.getValue()[0].id), "?$select=ipg_city&$expand=ipg_stateid($select=ipg_name)").then(function success(result) {
                    var City = result.ipg_city;
                    var State = result.ipg_stateid.ipg_name;
                    if (City)
                        formContext.getAttribute("ipg_melissapatientcity").setValue(City);
                    if (State)
                        formContext.getAttribute("ipg_melissapatientstate").setValue(State);
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message, null);
                });
            }
            else {
                formContext.getAttribute("ipg_melissapatientstate").setValue(null);
                formContext.getAttribute("ipg_melissapatientcity").setValue(null);
            }
        }
        Referral.OnPatientZipCodeChange = OnPatientZipCodeChange;
        function isReferralOpened(formContext) {
            var _a;
            return ((_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue()) == 923720000; // Opened
        }
        Referral.isReferralOpened = isReferralOpened;
        function showHideTabs(formContext) {
            var environment = Intake.Utility.GetEnvironment();
            if ((environment !== null && environment !== void 0 ? environment : '').toUpperCase() != 'DEV') {
                var tabToHide = formContext.ui.tabs.get('GatingLogTab');
                if (tabToHide) {
                    tabToHide === null || tabToHide === void 0 ? void 0 : tabToHide.setVisible(false);
                }
            }
        }
        function addPhysicianLookupCustomView(formContext) {
            var _a;
            var physicianControl = formContext.getControl("ipg_physicianid");
            if (physicianControl) {
                var facilityRef = (_a = formContext.getAttribute("ipg_facilityid")) === null || _a === void 0 ? void 0 : _a.getValue();
                if (facilityRef && facilityRef.length > 0) {
                    var viewId = "00000000-0000-0000-00AA-000010001111";
                    var fetchXml = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\">\n      <entity name=\"contact\">\n        <attribute name=\"fullname\" />\n        <attribute name=\"contactid\" />\n        <order attribute=\"fullname\" descending=\"false\" />\n        <link-entity name=\"ipg_facilityphysician\" from=\"ipg_physicianid\" to=\"contactid\" link-type=\"inner\" alias=\"ad\">\n          <filter type=\"and\">\n            <condition attribute=\"ipg_facilityid\" operator=\"eq\" value=\"" + facilityRef[0].id + "\" />\n            <condition attribute=\"ipg_status\" operator=\"eq\" value=\"1\" />\n          </filter>\n        </link-entity>\n      </entity>\n    </fetch>";
                    var viewDisplayName = "Physicians";
                    var layoutXml = "<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'>\n      <row name='result' id='contactid'>\n      <cell name='fullname' width='300' />\n      </row>\n      </grid>";
                    physicianControl.addCustomView(viewId, 'contact', viewDisplayName, fetchXml, layoutXml, true);
                }
            }
        }
        /**
        * Called on Facility Name change
        * @function Intake.Referral.OnFacilityLookupChange
        * @returns {void}
       */
        function OnFacilityLookupChange(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            // SetPhysicianFromFacility(formContext);
            (_a = formContext.getAttribute("ipg_physicianid")) === null || _a === void 0 ? void 0 : _a.setValue(null);
            addPhysicianLookupCustomView(formContext);
        }
        Referral.OnFacilityLookupChange = OnFacilityLookupChange;
        function SetPhysicianFromFacility(formContext) {
            var _a, _b, _c;
            if (((_a = formContext.getAttribute("ipg_facilityid")) === null || _a === void 0 ? void 0 : _a.getValue()) == null) {
                (_b = formContext.getControl("ipg_physicianid")) === null || _b === void 0 ? void 0 : _b.setVisible(false);
            }
            else {
                (_c = formContext.getControl("ipg_physicianid")) === null || _c === void 0 ? void 0 : _c.setVisible(true);
            }
        }
        Referral.SetPhysicianFromFacility = SetPhysicianFromFacility;
        function filterEventsLog(executionContext) {
            var formContext = executionContext.getFormContext();
            var gridContext = formContext.getControl("referrals_event_log");
            if (gridContext) {
                var referralId = formContext.data.entity.getId().replace(/{|}/g, "");
                var filterXml = "<filter type='and'>" +
                    "<condition attribute='ipg_referralid' operator='eq' value='" + referralId + "'/>" +
                    "</filter>";
                gridContext.setFilterXml(filterXml);
                formContext.ui.controls.get("referrals_event_log").refresh();
            }
        }
        Referral.filterEventsLog = filterEventsLog;
        function OnCaseStatusChange(executionContext) {
            var formContext = executionContext.getFormContext();
            setCaseStatusDisplayedValue(formContext);
        }
        Referral.OnCaseStatusChange = OnCaseStatusChange;
        function OnCaseStateChange(executionContext) {
            var formContext = executionContext.getFormContext();
            setCaseStatusDisplayedValue(formContext);
        }
        Referral.OnCaseStateChange = OnCaseStateChange;
        function OnCaseReasonChange(executionContext) {
            var formContext = executionContext.getFormContext();
            setCaseStatusDisplayedValue(formContext);
        }
        Referral.OnCaseReasonChange = OnCaseReasonChange;
        function setCaseStatusDisplayedValue(formContext) {
            var _a, _b, _c, _d;
            var status = "";
            var caseStatus = (_a = formContext.getAttribute("ipg_casestatus")) === null || _a === void 0 ? void 0 : _a.getValue();
            var caseState = (_b = formContext.getAttribute("ipg_statecode")) === null || _b === void 0 ? void 0 : _b.getValue();
            var caseReason = (_c = formContext.getAttribute("statuscode")) === null || _c === void 0 ? void 0 : _c.getValue();
            if (caseStatus && caseStatus == 923720000) {
                status += "Open in ";
            }
            else if (caseStatus && caseStatus == 923720001) {
                status += "Closed in ";
            }
            if (caseState) {
                switch (caseState) {
                    case 923720000 /* Intake */:
                        status += "Intake" /* Intake */;
                        break;
                    case 923720001 /* Authorizations */:
                        status += "Authorizations" /* Authorizations */;
                        break;
                    case 923720002 /* CaseManagement */:
                        status += "Case Management" /* CaseManagement */;
                        break;
                    case 923720003 /* Billing */:
                        status += "Billing" /* Billing */;
                        break;
                    case 923720004 /* CarrierCollections */:
                        status += "Carrier Collections" /* CarrierCollections */;
                        break;
                    case 923720005 /* PatientCollections */:
                        status += "Patient Collections" /* PatientCollections */;
                        break;
                    case 923720006 /* Finance */:
                        status += "Finance" /* Finance */;
                        break;
                    default:
                        break;
                }
            }
            if (caseReason && caseReason == 1) {
                status += " - Active";
            }
            else if (caseReason && caseReason == 2) {
                status += " - Closed";
            }
            (_d = formContext.getAttribute("ipg_casestatusdisplayed")) === null || _d === void 0 ? void 0 : _d.setValue(status);
        }
        function showTimeLine(formContext) {
            var _a;
            var associatedCaseAttr = formContext.getAttribute("ipg_associatedcaseid");
            if (!(associatedCaseAttr && ((_a = associatedCaseAttr.getValue()) === null || _a === void 0 ? void 0 : _a.length) > 0)) {
                formContext.ui.tabs.get("Timeline").setVisible(true);
            }
        }
        function addCarriersPreSearch(formContext) {
            var _a, _b;
            (_a = formContext.getControl("ipg_carrierid")) === null || _a === void 0 ? void 0 : _a.addPreSearch(function () {
                addPrimaryCarrierFilter(formContext);
            });
            (_b = formContext.getControl("ipg_carriername2id")) === null || _b === void 0 ? void 0 : _b.addPreSearch(function () {
                addSecondaryCarrierFilter(formContext);
            });
        }
        function addPrimaryCarrierFilter(formContext) {
            var _a;
            var primaryCategory = (_a = formContext.getAttribute("ipg_primarycarriercategory")) === null || _a === void 0 ? void 0 : _a.getValue();
            var filter = "<filter type='and'>\n    <condition attribute='ipg_carriertype' operator='not-in'>\n      <value>" + 427880000 /* Auto */ + "</value>\n      <value>" + 923720006 /* Government */ + "</value>\n      <value>" + 427880003 /* IPA */ + "</value>\n    </condition>\n    <condition attribute=\"ipg_carrieraccepted\" operator=\"eq\" value=\"1\" />\n    " + (primaryCategory ? "<condition attribute=\"ipg_portalparent\" operator=\"eq\" value=\"" + primaryCategory + "\"/>" : "") + "\n  </filter>";
            formContext.getControl("ipg_carrierid").addCustomFilter(filter, "account");
        }
        function addSecondaryCarrierFilter(formContext) {
            var _a;
            var secondaryCategory = (_a = formContext.getAttribute("ipg_secondarycarriercategory")) === null || _a === void 0 ? void 0 : _a.getValue();
            var filter = "<filter type='and'>\n    <condition attribute='ipg_carriertype' operator='not-in'>\n      <value>" + 427880003 /* IPA */ + "</value>\n      <value>" + 427880001 /* WorkersComp */ + "</value>\n    </condition>\n    " + (secondaryCategory ? "<condition attribute=\"ipg_portalparent\" operator=\"eq\" value=\"" + secondaryCategory + "\"/>" : "") + "\n  </filter>";
            formContext.getControl("ipg_carriername2id").addCustomFilter(filter, "account");
        }
        function onChangeOwner(executionContext) {
            return __awaiter(this, void 0, void 0, function () {
                var formContext, referralid, confirmed, ownerattr, ownerattrValue, result, e_1, incident;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            formContext = executionContext.getFormContext();
                            referralid = formContext.data.entity.getId();
                            confirmed = true;
                            ownerattr = executionContext.getEventSource();
                            ownerattrValue = ownerattr.getValue();
                            if (!((ownerattrValue === null || ownerattrValue === void 0 ? void 0 : ownerattrValue.length) > 0)) return [3 /*break*/, 9];
                            if (!(ownerattrValue[0].entityType == "systemuser")) return [3 /*break*/, 2];
                            return [4 /*yield*/, Xrm.Navigation.openConfirmDialog({ text: "System is about to reassign all related open User Tasks to the User you assigned this Referral to. Do you wish to proceed?" })];
                        case 1:
                            result = _a.sent();
                            Xrm.Utility.showProgressIndicator("");
                            confirmed = result.confirmed;
                            _a.label = 2;
                        case 2:
                            if (!confirmed) return [3 /*break*/, 7];
                            _a.label = 3;
                        case 3:
                            _a.trys.push([3, 5, , 6]);
                            return [4 /*yield*/, Xrm.WebApi.online.updateRecord("ipg_referral", referralid, { "ownerid@odata.bind": "/" + ownerattrValue[0].entityType + "s(" + ownerattrValue[0].id.replace("{", "").replace("}", "") + ")" })];
                        case 4:
                            _a.sent();
                            return [3 /*break*/, 6];
                        case 5:
                            e_1 = _a.sent();
                            Xrm.Navigation.openErrorDialog({ message: "Referral has not been ReAssigned. Please try later or contact System Administrator!" });
                            return [3 /*break*/, 6];
                        case 6: return [3 /*break*/, 9];
                        case 7: return [4 /*yield*/, Xrm.WebApi.online.retrieveRecord("ipg_referral", referralid, "?$select=_ownerid_value")];
                        case 8:
                            incident = _a.sent();
                            ownerattr.setValue([{
                                    entityType: incident["_ownerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"],
                                    id: incident["_ownerid_value"],
                                    name: incident["_ownerid_value@OData.Community.Display.V1.FormattedValue"]
                                }]);
                            _a.label = 9;
                        case 9:
                            Xrm.Utility.closeProgressIndicator();
                            return [2 /*return*/];
                    }
                });
            });
        }
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
