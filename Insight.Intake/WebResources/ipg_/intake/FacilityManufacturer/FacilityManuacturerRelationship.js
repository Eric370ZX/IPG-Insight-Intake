var Intake;
(function (Intake) {
    var FacilityManufacturerRelationship;
    (function (FacilityManufacturerRelationship) {
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            addFiltersToDistributorContactField(formContext);
            CreateTask(formContext);
            GetManufacturerName(formContext);
            RequestFacilityAccount(formContext);
        }
        FacilityManufacturerRelationship.OnLoadForm = OnLoadForm;
        /**
         * Called on change manufacturer
         * @function Intake.FacilityManufacturerRelationship.OnChangeManufacturer
         * @returns {void}
        */
        function OnChangeManufacturer(executionContext) {
            var formContext = executionContext.getFormContext();
            changeFacilityManufacturerRelationshipName(formContext);
            addFiltersToDistributorContactField(formContext);
        }
        FacilityManufacturerRelationship.OnChangeManufacturer = OnChangeManufacturer;
        /**
         * Called on change facility
         * @function Intake.FacilityManufacturerRelationship.OnChangeFacility
         * @returns {void}
        */
        function OnChangeFacility(executionContext) {
            var formContext = executionContext.getFormContext();
            changeFacilityManufacturerRelationshipName(formContext);
        }
        FacilityManufacturerRelationship.OnChangeFacility = OnChangeFacility;
        function changeFacilityManufacturerRelationshipName(formContext) {
            var facilityAttr = formContext.getAttribute("ipg_facilityid").getValue();
            var manufacturerAttr = formContext.getAttribute("ipg_manufacturerid").getValue();
            if (!facilityAttr || !manufacturerAttr)
                return;
            var facilityName = facilityAttr[0].name;
            var manufacturerName = manufacturerAttr[0].name;
            formContext.getAttribute("ipg_name").setValue(facilityName + " - " + manufacturerName);
        }
        function CreateTask(formContext) {
            var currentDate = new Date();
            var day = currentDate.getDate();
            var month = currentDate.getMonth() + 1;
            var year = currentDate.getFullYear();
            var currentMDYDate = month + "/" + day + "/" + year;
            var userSettings = Xrm.Utility.getGlobalContext().userSettings; // userSettings is an object with user information.
            var current_User_Id = userSettings.userId; // The user's unique id
            //let  current_User_Name = userSettings.userName;
            var current_User_Name = 'Vlad Vasiljevic';
            var current_User = userSettings.userName; // The user's unique id
            var manufacturerObject = Xrm.Page.getAttribute("ipg_manufacturerid").getValue();
            if (formContext.ui.getFormType() != 1) // 1 is for create
             {
                var Maanufacturerid = manufacturerObject[0].id;
                var manufacturerName = manufacturerObject[0].name;
                var object = new Array();
                object[0] = new Object();
                object[0].entityType = "account";
                object[0].name = manufacturerName;
                object[0].id = Maanufacturerid;
                formContext.getAttribute("ipg_manufacturerid").setValue(object);
            }
            if (manufacturerObject != null) {
                var facilityObject = Xrm.Page.getAttribute("ipg_facilityid").getValue();
                var facilityName = facilityObject[0].name;
                var facilityId = facilityObject[0].id;
                var id = manufacturerObject[0].id;
                var manufacturerName_1 = manufacturerObject[0].name;
                var description_1 = "Part missing for Manufacturer " + manufacturerName_1 + " does not have a relationship with Facility " + facilityName + ". Please create this association to resolve this task. ";
                var guid = void 0;
                guid = facilityId.replace("{", "").replace("}", "").toLowerCase();
                Xrm.WebApi.retrieveMultipleRecords("account", "?$select=ipg_manufactureraccountnumber,ipg_manufacturerisfacilityacctrequired,ipg_active,adx_createdbyusername &$filter=accountid  eq  ' " + guid + " '  and  name ne  ' " + current_User_Name + " '  and  ipg_active eq true  and  ipg_manufacturerisfacilityacctrequired eq true").then(function success(acctResult) {
                    // if (acctResult.entities.length)
                    if (acctResult.entities.length) {
                        if (confirm("Manufacturer Account Number is required for this facility.  Do you want to save anyways?")) {
                            Xrm.WebApi.online.retrieveMultipleRecords("task", "?$select=subject,activityid,activityid&$filter=subject eq 'Resolve Missing Facility Manufacturer Relationship'").then(function success(taskResults) {
                                if (taskResults.entities.length == 0) {
                                    var entity = {};
                                    entity["subject"] = "Resolve Missing Facility Manufacturer Relationship";
                                    //  entity["description"] = "Part missing for Manufacturer " + manufacturerName + " does not have a relationship with Facility " + facilityName + ". Please create this association to resolve this task. ";
                                    entity["description"] = description_1;
                                    entity["scheduledstart"] = currentMDYDate;
                                    Xrm.WebApi.createRecord("task", entity).then(function success(result) {
                                        UpdateStatusFacilityManufacturer(formContext);
                                    }, function (error) {
                                        Xrm.Navigation.openErrorDialog(error);
                                    });
                                }
                            }, function (error) {
                                Xrm.Navigation.openErrorDialog(error);
                            });
                        }
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog(error);
                });
            }
        }
        FacilityManufacturerRelationship.CreateTask = CreateTask;
        function UpdateStatusFacilityManufacturer(formContext) {
            Xrm.WebApi.online.retrieveMultipleRecords("task", "?$select=subject&$filter=subject eq 'Resolve Missing Facility Manufacturer Relationship'").then(function success(taskResults) {
                if (taskResults.entities.length) {
                    var updateStatus = taskResults.entities[0]["activityid"];
                    var entity = {};
                    entity["statecode"] = 1;
                    Xrm.WebApi.updateRecord("task", updateStatus, entity).then(function success(result) {
                    }, function (error) {
                        Xrm.Navigation.openErrorDialog(error);
                    });
                }
            }, function (error) {
                Xrm.Navigation.openErrorDialog(error);
            });
        }
        FacilityManufacturerRelationship.UpdateStatusFacilityManufacturer = UpdateStatusFacilityManufacturer;
        /**
         * add filters to distributor contact field
         * @function Intake.FacilityManufacturerRelationship.addFiltersToDistributorContactField
         * @returns {void}
        */
        function addFiltersToDistributorContactField(formContext) {
            function addCustomLookupFilter(formContext) {
                var filters = [
                    '<condition attribute="ipg_contacttypeidname" operator="like" value="%Distributor%" />',
                ];
                var filterXml = "<filter type=\"and\">" + filters.join('') + "</filter>";
                formContext.getControl("ipg_distributorcontact").addCustomFilter(filterXml, "contact");
            }
        }
        FacilityManufacturerRelationship.addFiltersToDistributorContactField = addFiltersToDistributorContactField;
        function RequestFacilityAccount(formContext) {
            if (localStorage.accountId) {
                Xrm.WebApi.retrieveRecord("account", localStorage.accountId, "?$select=ipg_manufacturerisfacilityacctrequired").then(function success(results) {
                    var _a;
                    if (results.ipg_manufacturerisfacilityacctrequired == true) {
                        (_a = formContext.getAttribute("ipg_manufactureraccountnumber")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required");
                    }
                }, function (error) {
                    console.log(error.message);
                });
            }
        }
        FacilityManufacturerRelationship.RequestFacilityAccount = RequestFacilityAccount;
        function GetManufacturerName(formContext) {
            var _a, _b, _c;
            if (formContext.ui.getFormType() == 1) // 1 is for create
             {
                formContext.getControl("ipg_active").setVisible(false);
                (_a = formContext.getAttribute("ipg_manufacturerrep")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required");
                (_b = formContext.getAttribute("ipg_manufacturerrepphone")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("required");
                (_c = formContext.getAttribute("ipg_manufacturerrepemailmask")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("required");
            }
            if (formContext.getAttribute("ipg_manufacturerid").getValue() == null && localStorage.accountId != null) {
                var accountIdNumber = localStorage.accountId;
                var accountName = localStorage.accountName;
                var object = new Array();
                object[0] = new Object();
                object[0].entityType = "account";
                object[0].name = accountName;
                object[0].id = accountIdNumber;
                if (localStorage.ManufacturerView === "true") {
                    formContext.getAttribute("ipg_manufacturerid").setValue(null);
                    formContext.getAttribute("ipg_manufacturerid").setValue(object);
                    localStorage.removeItem("ManufacturerView");
                }
                if (localStorage.FacilityView === "true") {
                    formContext.getAttribute("ipg_facilityid").setValue(null);
                    formContext.getAttribute("ipg_facilityid").setValue(object);
                    localStorage.removeItem("FacilityView");
                }
            }
            localStorage.removeItem("accountid");
            localStorage.removeItem("accountName");
        }
        FacilityManufacturerRelationship.GetManufacturerName = GetManufacturerName;
    })(FacilityManufacturerRelationship = Intake.FacilityManufacturerRelationship || (Intake.FacilityManufacturerRelationship = {}));
})(Intake || (Intake = {}));
