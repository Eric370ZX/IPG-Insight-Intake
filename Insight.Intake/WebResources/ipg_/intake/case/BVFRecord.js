var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        function SetBVFNames(executionContext) {
            var formContext = executionContext.getFormContext();
            var memberId = formContext.getAttribute("ipg_memberidnumber").getValue();
            if (memberId != null) {
                Xrm.WebApi.retrieveMultipleRecords("ipg_benefitsverificationform", "?$select=ipg_memberidnumber,ipg_primarycontact,ipg_dateofinjury," +
                    "ipg_deductible, ipg_deductiblemet, ipg_carriercoinsurance, ipg_patientcoinsurance, ipg_oopmax, ipg_oopmaxmet & " +
                    "$filter=ipg_memberidnumber eq '" + memberId + "'").then(function success(results) {
                    if (results.entities.length) {
                        formContext.getAttribute("ipg_primarycontactcode").setValue(results.entities[0]["ipg_primarycontact"]);
                        formContext.getAttribute("ipg_deductible").setValue(results.entities[0]["ipg_deductible"]);
                        formContext.getAttribute("ipg_deductiblemet").setValue(results.entities[0]["ipg_deductiblemet"]);
                        formContext.getAttribute("ipg_deductiblemet").setValue(results.entities[0]["ipg_deductibleremainingdisplay"]);
                        formContext.getAttribute("ipg_payercoinsurance").setValue(results.entities[0]["ipg_carriercoinsurance"]);
                        formContext.getAttribute("ipg_patientcoinsurance").setValue(results.entities[0]["ipg_patientcoinsurance"]);
                        formContext.getAttribute("ipg_oopmax").setValue(results.entities[0]["ipg_oopmax"]);
                        formContext.getAttribute("ipg_oopremainingdisplay").setValue(results.entities[0]["ipg_oopmaxmet"]);
                        formContext.getAttribute("ipg_autodateofincident").setValue(new Date(results.entities[0]["ipg_dateofinjury"]));
                    }
                }, function (error) {
                    console.log(error.message);
                });
            }
        }
        Case.SetBVFNames = SetBVFNames;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
