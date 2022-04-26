/**
 * @namespace Intake.Contact
 */
var Intake;
(function (Intake) {
    var Contact;
    (function (Contact) {
        /**
        * Called on Form Load event
        * @function Intake.Contact.OnLoadForm
        * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var _a, _b, _c;
            var formContext = executionContext.getFormContext();
            SetFieldsNoneRequired(formContext);
            ChooseForm(formContext);
            ConfigurePortalContact(formContext);
            // AddUsernamePopulationFromEmail(formContext);
            SetNotificationFrequencyByPreference(formContext);
            OnReviewStatusChange(formContext);
            (_a = formContext.getAttribute('ipg_facility_user_status_typecode')) === null || _a === void 0 ? void 0 : _a.addOnChange(function () {
                OnReviewStatusChange(formContext);
            });
            OnRejectedReasonChange(formContext);
            (_b = formContext.getAttribute('ipg_rejectionreason')) === null || _b === void 0 ? void 0 : _b.addOnChange(function () {
                OnRejectedReasonChange(formContext);
            });
            isZipCodeRequired(formContext);
            (_c = formContext.getAttribute('address1_line1')) === null || _c === void 0 ? void 0 : _c.addOnChange(function () {
                isZipCodeRequired(formContext);
            });
            if (formContext.ui.formSelector.getCurrentItem().getLabel() === "Physician") {
                disableFormSelector(formContext);
            }
        }
        Contact.OnLoadForm = OnLoadForm;
        function SetNotificationFrequencyByPreference(formContext) {
            var notificationPreferenceAttr = formContext.getAttribute("ipg_portalnotificationpreference");
            var notificationFrequencyAttr = formContext.getAttribute("ipg_portalemailalertfrequency");
            var notificationFrequencyCtrl = formContext.getControl("ipg_portalemailalertfrequency");
            if (notificationPreferenceAttr) {
                notificationPreferenceAttr.addOnChange(function () {
                    if (notificationPreferenceAttr.getValue() === 427880001 /* OptOut */) {
                        notificationFrequencyAttr.setValue(427880000 /* Never */);
                        notificationFrequencyCtrl.setDisabled(true);
                    }
                    else {
                        notificationFrequencyAttr.setValue(427880003 /* OnceWeekly */);
                        notificationFrequencyCtrl.setDisabled(false);
                    }
                });
                notificationPreferenceAttr.fireOnChange();
            }
        }
        function AddUsernamePopulationFromEmail(formContaxt) {
            var emailAttribute = formContaxt.getAttribute("emailaddress1");
            emailAttribute.addOnChange(function () {
                formContaxt.getAttribute("adx_identity_username").setValue(emailAttribute.getValue());
            });
            emailAttribute.fireOnChange();
        }
        function SetFieldsNoneRequired(formContext) {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z;
            if (formContext.ui.formSelector.getCurrentItem().getLabel() === "Physician") {
                (_a = formContext.getAttribute("ipg_contacttypeid")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("none");
                (_b = formContext.getAttribute("middlename")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("none");
                (_c = formContext.getAttribute("birthdate")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("none");
                (_d = formContext.getAttribute("address1_telephone2")) === null || _d === void 0 ? void 0 : _d.setRequiredLevel("none");
                (_e = formContext.getAttribute("mobilephone")) === null || _e === void 0 ? void 0 : _e.setRequiredLevel("none");
                (_f = formContext.getAttribute("telephone1")) === null || _f === void 0 ? void 0 : _f.setRequiredLevel("none");
                (_g = formContext.getAttribute("address1_fax")) === null || _g === void 0 ? void 0 : _g.setRequiredLevel("none");
                (_h = formContext.getAttribute("emailaddress1")) === null || _h === void 0 ? void 0 : _h.setRequiredLevel("none");
                (_j = formContext.getAttribute("ipg_zipcodeid")) === null || _j === void 0 ? void 0 : _j.setRequiredLevel("none");
                (_k = formContext.getAttribute("address1_line1")) === null || _k === void 0 ? void 0 : _k.setRequiredLevel("none");
                (_l = formContext.getAttribute("address1_city")) === null || _l === void 0 ? void 0 : _l.setRequiredLevel("none");
                (_m = formContext.getAttribute("ipg_stateid")) === null || _m === void 0 ? void 0 : _m.setRequiredLevel("none");
                (_o = formContext.getAttribute("address1_country")) === null || _o === void 0 ? void 0 : _o.setRequiredLevel("none");
                (_p = formContext.getAttribute("statecode")) === null || _p === void 0 ? void 0 : _p.setRequiredLevel("none");
                (_q = formContext.getAttribute("parentcustomerid")) === null || _q === void 0 ? void 0 : _q.setRequiredLevel("none");
                (_r = formContext.getAttribute("ipg_physiciangrouptaxid")) === null || _r === void 0 ? void 0 : _r.setRequiredLevel("none");
                (_s = formContext.getAttribute("ipg_physiciangroupinstruction")) === null || _s === void 0 ? void 0 : _s.setRequiredLevel("none");
                (_t = formContext.getAttribute("ipg_physiciangroupnp")) === null || _t === void 0 ? void 0 : _t.setRequiredLevel("none");
                (_u = formContext.getAttribute("ipg_physiciantaxnumbe")) === null || _u === void 0 ? void 0 : _u.setRequiredLevel("none");
                (_v = formContext.getAttribute("ipg_physicianspecialinstructions")) === null || _v === void 0 ? void 0 : _v.setRequiredLevel("none");
                (_w = formContext.getAttribute("ipg_physiciannpi")) === null || _w === void 0 ? void 0 : _w.setRequiredLevel("none");
                (_x = formContext.getAttribute("ipg_physiciantaxonomycode")) === null || _x === void 0 ? void 0 : _x.setRequiredLevel("none");
                (_y = formContext.getAttribute("ipg_physiciannpistate")) === null || _y === void 0 ? void 0 : _y.setRequiredLevel("none");
                (_z = formContext.getAttribute("ipg_physicianrequestedbyid")) === null || _z === void 0 ? void 0 : _z.setRequiredLevel("none");
            }
        }
        function ConfigurePortalContact(formContex) {
            var _a, _b, _c, _d;
            if (formContex.ui.formSelector.getCurrentItem().getLabel() == "Portal Contact") {
                if (formContex.ui.getFormType() === 1 /* Create */) {
                    (_a = formContex.getControl("ipg_facilities")) === null || _a === void 0 ? void 0 : _a.setVisible(true);
                    (_b = formContex.getAttribute("ipg_facilities")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("required");
                }
                else {
                    ["ipg_facility_user_status_typecode",
                        "adx_identity_lastsuccessfullogin",
                        "ipg_lastpasswordmodifydate",
                        "adx_identity_lockoutenddate",
                        "adx_identity_logonenabled"].forEach(function (fieldName) {
                        var _a;
                        (_a = formContex.getControl(fieldName)) === null || _a === void 0 ? void 0 : _a.setVisible(true);
                    });
                    (_c = formContex.getAttribute("ipg_facilities")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("none");
                    (_d = formContex.getControl("ipg_facilities")) === null || _d === void 0 ? void 0 : _d.setVisible(false);
                }
                var notificationPreference_1 = formContex.getAttribute("ipg_portalnotificationpreference");
                notificationPreference_1.addOnChange(function () {
                    if (notificationPreference_1.getValue() === 427880001 /* OptOut */) {
                        formContex.getAttribute("ipg_portalemailalertfrequency").setValue(427880000 /* Never */);
                        formContex.getControl("ipg_portalemailalertfrequency").setDisabled(true);
                    }
                    else {
                        formContex.getAttribute("ipg_portalemailalertfrequency").setValue(427880003 /* OnceWeekly */);
                        formContex.getControl("ipg_portalemailalertfrequency").setDisabled(false);
                    }
                });
                notificationPreference_1.fireOnChange();
            }
        }
        /**
          * Called on Form Save event
          * @function Intake.Contact.OnSaveForm
          * @returns {void}
        */
        function OnSaveForm(executionContext) {
            var _a;
            var formContext = executionContext.getFormContext();
            if (formContext.ui.formSelector.getCurrentItem().getLabel() === "Portal Contact") {
                var isDirty = (_a = formContext.getAttribute("ipg_lastpasswordmodifydate")) === null || _a === void 0 ? void 0 : _a.getIsDirty();
                if (isDirty) {
                    ChangePasswordExpirationDate(formContext);
                }
            }
            if (CheckNPI(formContext)) {
                var saveEventArgs = executionContext.getEventArgs();
                saveEventArgs.preventDefault();
            }
        }
        Contact.OnSaveForm = OnSaveForm;
        /**
        * Change password expiration date on modifying 'Last Password Modify Date' field
        * @function Intake.Contact.ChangePasswordExpirationDate
        * @returns {void}
        */
        function ChangePasswordExpirationDate(formContext) {
            var _a, _b;
            var lastPasswordModifyDate = (_a = formContext.getAttribute("ipg_lastpasswordmodifydate")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (lastPasswordModifyDate) {
                var newExpirationDate = new Date(lastPasswordModifyDate);
                newExpirationDate.setDate(newExpirationDate.getDate() + 90);
                (_b = formContext.getAttribute("ipg_passwordexpirationdate")) === null || _b === void 0 ? void 0 : _b.setValue(newExpirationDate);
            }
        }
        /**
          * Called on change Physician NPI
          * @function Intake.Contact.OnChangeNPI
          * @returns {void}
        */
        function OnChangeNPI(executionContext) {
            var formContext = executionContext.getFormContext();
            CheckNPI(formContext);
        }
        Contact.OnChangeNPI = OnChangeNPI;
        /**
        * Shows warning if NPI exists in database
        * @function Intake.Contact.CheckNPI
        * @returns {void}
        */
        function CheckNPI(formContext) {
            var _a, _b;
            var NPIfield = 'ipg_physiciannpi';
            (_a = formContext.getControl(NPIfield)) === null || _a === void 0 ? void 0 : _a.clearNotification(NPIfield);
            var npi = (_b = formContext.getAttribute(NPIfield)) === null || _b === void 0 ? void 0 : _b.getValue();
            if ((npi) && (npi.length == 10)) {
                var query = "?$select=fullname&$filter=" + NPIfield + " eq '" + npi + "'";
                var GUID = formContext.data.entity.getId();
                if (GUID) {
                    GUID = GUID.slice(1, -1);
                    query = query + " and contactid ne " + GUID;
                }
                Xrm.WebApi.retrieveMultipleRecords('contact', query).then(function (result) {
                    if (result.entities.length) {
                        if (result.entities.length == 1)
                            formContext.getControl(NPIfield).setNotification('This NPI is used by another physician: ' + result.entities[0].fullname, NPIfield);
                        else {
                            var fullnames = "";
                            for (var _i = 0, _a = result.entities; _i < _a.length; _i++) {
                                var entity = _a[_i];
                                if (fullnames)
                                    fullnames += ", " + entity.fullname;
                                else
                                    fullnames = entity.fullname;
                            }
                            formContext.getControl(NPIfield).setNotification('This NPI is used by other physicians: ' + fullnames, NPIfield);
                        }
                        return true;
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
            return false;
        }
        /**
        * Called on load quick create carrier form
        * @function Intake.Contact.onLoadQuickCreateCarrierContactForm
        * @returns {void}
        */
        function onLoadQuickCreateCarrierContactForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var params = executionContext.getContext().getQueryStringParameters();
            fillFieldsByParams(formContext, params);
            setFieldsRequirementLevelAndVisibilityByType(formContext);
        }
        Contact.onLoadQuickCreateCarrierContactForm = onLoadQuickCreateCarrierContactForm;
        /**
        * fill fields on form by params
        * @function Intake.Contact.fillFieldsByParams
        * @returns {void}
        */
        function fillFieldsByParams(formContext, params) {
            var attributes = formContext.getAttribute();
            attributes.forEach(function (attribute) {
                if (attribute.getName() in params) {
                    if (attribute.getAttributeType() === "lookup") {
                        var lookupArr = Array();
                        lookupArr[0] = new Object();
                        lookupArr[0].id = params[attribute.getName()];
                        lookupArr[0].name = params[attribute.getName() + "name"];
                        lookupArr[0].entityType = params[attribute.getName() + "entityType"];
                        attribute.setValue(lookupArr);
                    }
                    else {
                        attribute.setValue(params[attribute.getName()]);
                    }
                }
            });
        }
        /**
      * Chooses form depending on 'Relation Type' field value
      * @function Intake.Contact.ChooseForm
      * @returns {void}
      */
        function ChooseForm(formContext) {
            var contactType = formContext.getAttribute("ipg_contacttypeid");
            if (contactType) {
                var contactTypeValue = contactType.getValue();
                if (contactTypeValue && contactTypeValue.length > 0) {
                    var contactTypeName_1 = contactTypeValue[0].name.toLocaleLowerCase();
                    var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
                    var contactTypes = [
                        { name: 'physician', formId: '12b0030d-bd5b-4c99-ad0d-fb7eab63426a'.toLocaleLowerCase() },
                        { name: 'carrier', formId: '807AD6E4-30D2-4AD3-8CC4-0CD5E8EC1A9F'.toLocaleLowerCase() },
                        { name: 'distributor', formId: 'A5733E8F-912A-46CE-B1FE-CBC68B4D3359'.toLocaleLowerCase() },
                        { name: 'facility', formId: '7EDAB598-14BD-4BE0-976F-8DC72F61D485'.toLocaleLowerCase() },
                        { name: 'manufacturer', formId: '85ED629F-ED77-406A-BBF2-8013FA618F25'.toLocaleLowerCase() },
                        { name: 'patient', formId: 'A1F66726-D064-430F-83D9-A40B2551E0F8'.toLocaleLowerCase() },
                        { name: 'health plan network', formId: 'A1F66726-D064-430F-83D9-A40B2551E0F8'.toLocaleLowerCase() },
                    ];
                    var currentContact = contactTypes.find(function (x) { return x.name == contactTypeName_1; });
                    if (currentFormId != currentContact.formId) {
                        var items = formContext.ui.formSelector.items.get();
                        for (var i in items) {
                            if (items[i].getId() == currentContact.formId)
                                items[i].navigate();
                        }
                    }
                }
            }
        }
        /**
        * set level of requirement and visibility of fields by contact's subType
        * @function Intake.Contact.setFieldsRequirementLevelAndVisibilityByType
        * @returns {void}
        */
        function setFieldsRequirementLevelAndVisibilityByType(formContext) {
            var contactSubTypeLookupValue = formContext.getAttribute("ipg_contactsubtypeid").getValue();
            if (contactSubTypeLookupValue && contactSubTypeLookupValue.length) {
                if (contactSubTypeLookupValue[0].name === "Claims Inquiries" || contactSubTypeLookupValue[0].name === "Pre-Certification") {
                    formContext.getAttribute("telephone1").setRequiredLevel("required");
                    formContext.getAttribute("lastname").setRequiredLevel("none");
                    formContext.getAttribute("firstname").setRequiredLevel("none");
                    formContext.getControl("mobilephone").setVisible(false);
                    formContext.getControl("lastname").setVisible(false);
                    formContext.getControl("firstname").setVisible(false);
                    formContext.getControl("middlename").setVisible(false);
                }
                else if (contactSubTypeLookupValue[0].name === "Sales") {
                    formContext.getControl("ipg_mddname_userid").setVisible(true);
                }
            }
        }
        /**
          * Called on change DOB
          * @function Intake.Contact.OnChangeDOB
          * @returns {void}
          */
        function OnChangeDOB(executionContext) {
            var formContext = executionContext.getFormContext();
            var DOBField = "birthdate";
            var birthdate = formContext.getAttribute(DOBField).getValue();
            formContext.getControl(DOBField).clearNotification(DOBField);
            if ((birthdate) && (birthdate > new Date())) {
                formContext.getControl(DOBField).setNotification("Impossible to create a contact with future Date of Birth", DOBField);
            }
        }
        Contact.OnChangeDOB = OnChangeDOB;
        /**
          * Called on change DOB
          * @function Intake.Contact.onSubGridDocumentRecordSelect
          * @returns {void}
        */
        function onSubGridDocumentRecordSelect(executionContext) {
            var formContext = executionContext.getFormContext();
            var currentEntity = formContext.data.entity;
            currentEntity.attributes.forEach(function (attribute, i) {
                attribute.controls.get(0).setDisabled(true);
            });
        }
        Contact.onSubGridDocumentRecordSelect = onSubGridDocumentRecordSelect;
        /**
          * Called on on Sub Grid FacilityPhysician Record Selected
          * @function Intake.Contact.onSubGridFacilityPhysicianRecordSelect
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
        Contact.onSubGridFacilityPhysicianRecordSelect = onSubGridFacilityPhysicianRecordSelect;
        function GetLookUpName(executionContext) {
            var formContext = executionContext.getFormContext();
            var contactObject = formContext.getAttribute("ipg_manufacturercontractaddress").getValue();
            if (contactObject != null) {
                var contactId = contactObject[0];
                var guid = contactId.id.replace("{", "").replace("}", "");
                Xrm.WebApi.retrieveRecord("contact", guid, "?$select=firstname,lastname,accountrolecode,telephone1,emailaddress1,ipg_contacttype").then(function success(results) {
                    formContext.getAttribute("firstname").setValue(results["firstname"]);
                    formContext.getAttribute("lastname").setValue(results["lastname"]);
                    formContext.getAttribute("telephone1").setValue(results["telephone1"]);
                    formContext.getAttribute("accountrolecode").setValue(results["accountrolecode"]);
                    formContext.getAttribute("emailaddress1").setValue(results["emailaddress1"]);
                    formContext.getAttribute("ipg_contacttype").setValue(results["ipg_contacttype"]);
                }, function (error) {
                });
            }
        }
        Contact.GetLookUpName = GetLookUpName;
        function ManufacturerContactType(executionContext) {
            var formContext = executionContext.getFormContext();
            var primaryContactType;
            var manId;
            var manufacturer = formContext.getAttribute("ipg_manufacturername").getValue();
            var contractType = formContext.getAttribute("ipg_contacttype").getValue();
            if (contractType == 427880000) {
                primaryContactType = "Primary";
                var i;
                if (manufacturer) {
                    Xrm.WebApi.retrieveMultipleRecords("contact", "?$select=ipg_contacttype&$filter=ipg_manufacturername eq '" + manufacturer + "'").then(function success(results) {
                        for (i = 0; i < results.entities.length; i++) {
                            manId = results.entities[i]["contactid"];
                            if (results.entities[i]["ipg_contacttype@OData.Community.Display.V1.FormattedValue"] == primaryContactType) {
                                formContext.getControl("ipg_contacttype").setNotification("Only one Primary contact type can exist, select Other.", manId);
                            }
                        }
                    }, function (error) {
                    });
                }
            }
            formContext.getControl("ipg_contacttype").clearNotification(manId);
        }
        Contact.ManufacturerContactType = ManufacturerContactType;
        function OnReviewStatusChange(formContext) {
            var _a, _b, _c;
            var reviewStatus = formContext.getAttribute("ipg_facility_user_status_typecode");
            var npi = formContext.getAttribute("ipg_physiciannpi");
            var taxonomy = formContext.getAttribute("ipg_physiciantaxonomycode");
            var active = formContext.getAttribute("ipg_active");
            var rejectionreason = formContext.getAttribute("ipg_rejectionreason");
            if ((reviewStatus === null || reviewStatus === void 0 ? void 0 : reviewStatus.getValue()) == 427880001) { // Approved      
                npi === null || npi === void 0 ? void 0 : npi.setRequiredLevel("required");
                taxonomy === null || taxonomy === void 0 ? void 0 : taxonomy.setRequiredLevel("required");
                rejectionreason === null || rejectionreason === void 0 ? void 0 : rejectionreason.setRequiredLevel("none");
                (_a = formContext.getControl("ipg_rejectionreason")) === null || _a === void 0 ? void 0 : _a.setVisible(false);
            }
            if ((reviewStatus === null || reviewStatus === void 0 ? void 0 : reviewStatus.getValue()) == 427880000) { // Pending Review
                npi === null || npi === void 0 ? void 0 : npi.setRequiredLevel("required");
                taxonomy === null || taxonomy === void 0 ? void 0 : taxonomy.setRequiredLevel("required");
                rejectionreason === null || rejectionreason === void 0 ? void 0 : rejectionreason.setRequiredLevel("none");
                (_b = formContext.getControl("ipg_rejectionreason")) === null || _b === void 0 ? void 0 : _b.setVisible(false);
            }
            if ((reviewStatus === null || reviewStatus === void 0 ? void 0 : reviewStatus.getValue()) == 427880002) { // Rejected
                npi === null || npi === void 0 ? void 0 : npi.setRequiredLevel("none");
                taxonomy === null || taxonomy === void 0 ? void 0 : taxonomy.setRequiredLevel("none");
                active === null || active === void 0 ? void 0 : active.setValue(false);
                rejectionreason === null || rejectionreason === void 0 ? void 0 : rejectionreason.setRequiredLevel("required");
                (_c = formContext.getControl("ipg_rejectionreason")) === null || _c === void 0 ? void 0 : _c.setVisible(true);
            }
            OnRejectedReasonChange(formContext);
        }
        function OnRejectedReasonChange(formContext) {
            var _a, _b, _c;
            var reviewStatus = formContext.getAttribute("ipg_facility_user_status_typecode");
            var rejectionreason = formContext.getAttribute("ipg_rejectionreason");
            var relatedphysician = formContext.getAttribute("ipg_relatedphysician");
            if ((reviewStatus === null || reviewStatus === void 0 ? void 0 : reviewStatus.getValue()) != 427880002) { // Rejected
                relatedphysician === null || relatedphysician === void 0 ? void 0 : relatedphysician.setRequiredLevel("none");
                (_a = formContext.getControl("ipg_relatedphysician")) === null || _a === void 0 ? void 0 : _a.setVisible(false);
            }
            if ((rejectionreason === null || rejectionreason === void 0 ? void 0 : rejectionreason.getValue()) == 427880000 && (reviewStatus === null || reviewStatus === void 0 ? void 0 : reviewStatus.getValue()) == 427880002) { // Duplicate      
                relatedphysician === null || relatedphysician === void 0 ? void 0 : relatedphysician.setRequiredLevel("required");
                (_b = formContext.getControl("ipg_relatedphysician")) === null || _b === void 0 ? void 0 : _b.setVisible(true);
            }
            if ((rejectionreason === null || rejectionreason === void 0 ? void 0 : rejectionreason.getValue()) == 427880001) { // Unable to Verify
                relatedphysician === null || relatedphysician === void 0 ? void 0 : relatedphysician.setRequiredLevel("none");
                (_c = formContext.getControl("ipg_relatedphysician")) === null || _c === void 0 ? void 0 : _c.setVisible(false);
            }
        }
        function disableFormSelector(formContext) {
            formContext.ui.formSelector.items.forEach(function (formItem) { return formItem.setVisible(false); });
        }
        function isZipCodeRequired(formContext) {
            var address = formContext.getAttribute("address1_line1");
            var zipcode = formContext.getAttribute("ipg_zipcodeid");
            if ((address === null || address === void 0 ? void 0 : address.getValue()) == null || (address === null || address === void 0 ? void 0 : address.getValue()) === "") {
                zipcode === null || zipcode === void 0 ? void 0 : zipcode.setRequiredLevel("none");
            }
            else {
                zipcode === null || zipcode === void 0 ? void 0 : zipcode.setRequiredLevel("required");
            }
        }
    })(Contact = Intake.Contact || (Intake.Contact = {}));
})(Intake || (Intake = {}));
