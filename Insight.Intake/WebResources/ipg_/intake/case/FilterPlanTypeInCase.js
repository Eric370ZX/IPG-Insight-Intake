/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        /**
         * Called on load form
         * @function Intake.Case.OnLoad
         * @returns {void}
        */
        function OnLoad(executionContext) {
            var formContext = executionContext.getFormContext();
            SetCarrierName(executionContext);
            SetPhysician(formContext);
            SetNameFields(formContext);
        }
        Case.OnLoad = OnLoad;
        /**
        * Lookup carrier types and convert to plan types
        * @function Intake.Case.SetCarrierName
        * @returns {void}
        */
        function SetCarrierName(executionContext) {
            var formContext = executionContext.getFormContext();
            var optionText;
            var carrier = formContext.getAttribute("ipg_carrierid").getValue();
            var carrierId = carrier[0];
            if (carrierId.id) {
                Xrm.WebApi.online.retrieveRecord("account", Intake.Utility.removeCurlyBraces(carrierId.id), "?$select=ipg_carriertype").then(function success(result) {
                    var carriertype = result["ipg_carriertype@OData.Community.Display.V1.FormattedValue"];
                    switch (carriertype) {
                        case "Auto":
                            optionText = "Other";
                            break;
                        case "Workers Comp":
                            optionText = "Other";
                            break;
                        case "Government":
                            optionText = "Government";
                            break;
                        case "Null":
                            optionText = "Other";
                            break;
                        default:
                            optionText = "Null";
                    }
                    if (optionText == "Null") {
                        if (!window.parent.Xrm.Page.getAttribute("ipg_carrierid").getValue()) {
                        }
                        else {
                            Xrm.WebApi.retrieveMultipleRecords("ipg_plantypeebvrelationship", "?$select=ipg_optionsetvalue,ipg_planvalue,ipg_ebvvalue").then(function success(result) {
                                for (var i = 0; i < result.entities.length; i++) {
                                    if (result.entities[i]["ipg_ebvvalue"] == optionText) {
                                        var optionSetValue = result.entities[i]["ipg_optionsetvalue"];
                                        Xrm.Page.getAttribute("ipg_benefitplantypecode").setValue(optionSetValue);
                                    }
                                }
                            }, function (error) {
                                Xrm.Utility.alertDialog(error.message, null);
                            });
                        }
                    }
                    var options = Xrm.Page.getAttribute("ipg_benefitplantypecode").getOptions();
                    var ii;
                    if (optionText == "Null") {
                        optionText = "Other";
                    }
                    for (ii = 0; ii < options.length; ii++) {
                        if (options[ii].text == optionText)
                            Xrm.Page.getAttribute("ipg_benefitplantypecode").setValue(options[ii].value);
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message, null);
                });
            }
        }
        function SetNameFields(formContext) {
            var caseId = formContext.getAttribute("title").getValue();
            var gender = "";
            var currentReferralId = 'Referral(no. "+caseId+")';
            var dob;
            Xrm.WebApi.retrieveMultipleRecords("ipg_referral", "?$select=ipg_referralcasenumber,ipg_name,ipg_gender,ipg_patientfirstname,ipg_patientmiddlename,ipg_patientlastname,ipg_patientdateofbirth,ipg_referralid").then(function success(results) {
                if (results.entities.length != 0) {
                    for (var i = 0; i < results.entities.length; i++) {
                        if (caseId.includes(results.entities[i]["ipg_referralcasenumber"])) {
                            if (results.entities[i]["ipg_gender"] == "923720001") {
                                gender = "Male";
                            }
                            if (results.entities[i]["ipg_gender"] == "923720002") {
                                gender = "Female";
                            }
                            if (results.entities[i]["ipg_gender"] == "427880000") {
                                gender = "Other";
                            }
                            formContext.getAttribute("ipg_insuredfirstname").setValue(results.entities[i]["ipg_patientfirstname"]);
                            formContext.getAttribute("ipg_insuredmiddlename").setValue(results.entities[i]["ipg_patientmiddlename"]);
                            formContext.getAttribute("ipg_insuredlastname").setValue(results.entities[i]["ipg_patientlastname"]);
                            formContext.getAttribute("ipg_insuredgender").setValue(gender);
                            dob = new Date(results.entities[i]["ipg_patientdateofbirth@OData.Community.Display.V1.FormattedValue"]);
                            formContext.getAttribute("ipg_insureddateofbirth").setValue(dob);
                        }
                    }
                }
            }, function (error) {
            });
        }
        function SetPhysician(formContext) {
            var physician = formContext.getAttribute("ipg_physicianid").getValue();
            if (physician != null) {
                var id = physician[0].id;
                var name = physician[0].name;
                Xrm.WebApi.online.retrieveRecord("contact", Intake.Utility.removeCurlyBraces(id), "?$select=ipg_approved").then(function success(result) {
                    var isApproved = result["ipg_approved"];
                    if (isApproved == false) {
                        formContext.getAttribute("ipg_physicianid").setValue();
                    }
                }, function (error) {
                });
            }
        }
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
