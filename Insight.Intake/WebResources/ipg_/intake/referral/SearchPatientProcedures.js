/**
 * @namespace Intake.Referral
 */
var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        var patientLookupKey = 'ipg_patientid';
        var referralGridKey = 'Referrals';
        var caseGridKey = 'Cases';
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Referral.SearchPatientsProcedures
         * @returns {void}
         */
        function SearchPatientsProcedures() {
            var patientLookupAttribute = Xrm.Page.getAttribute(patientLookupKey);
            var referralGridControl = Xrm.Page.getControl(referralGridKey);
            var caseGridControl = Xrm.Page.getControl(caseGridKey);
            function filterSubGrids() {
                var patientLookupValue = patientLookupAttribute.getValue();
                var referralGridFetchXml = '<fetch><entity name="ipg_referral"><filter type="and"><condition attribute="ipg_referralid" operator="null" /></filter></entity></fetch>';
                var caseGridFetchXml = '<fetch><entity name="incident"><filter type="and"><condition attribute="incidentid" operator="null" /></filter></entity></fetch>';
                // Update filter based on Patient Lookup value.
                if (patientLookupValue && patientLookupValue.length) {
                    var patientId = patientLookupValue[0].id;
                    referralGridFetchXml = "<fetch><entity name=\"ipg_referral\"><filter type=\"and\"><condition attribute=\"ipg_patientid\" operator=\"eq\" value=\"" + patientId + "\" /></filter></entity></fetch>";
                    caseGridFetchXml = "<fetch><entity name=\"incident\"><filter type=\"and\"><condition attribute=\"customerid\" operator=\"eq\" value=\"" + patientId + "\" /></filter></entity></fetch>";
                }
                // Change filter of the Referral Grid.
                referralGridControl.setFilterXml(referralGridFetchXml);
                referralGridControl.refresh();
                // Change filter of the Case Grid.
                caseGridControl.setFilterXml(caseGridFetchXml);
                caseGridControl.refresh();
            }
            if (patientLookupAttribute) {
                // Filter grids after init.
                filterSubGrids();
                // Add change listener to Patient Lookup Attribute.
                patientLookupAttribute.addOnChange(filterSubGrids);
            }
        }
        Referral.SearchPatientsProcedures = SearchPatientsProcedures;
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
