/**
 * @namespace Intake.Account
 */
var Intake;
(function (Intake) {
    var Account;
    (function (Account) {
        var associatedCptCodesGridKey = 'AssociatedCPTCodes';
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Account.FilterCPTGrid
         * @returns {void}
         */
        function FilterCPTGrid() {
            var blockedCptCodesGridControl = Xrm.Page.getControl(associatedCptCodesGridKey);
            function filter() {
                var accountId = Xrm.Page.data.entity.getId();
                var blockedCptCodesGridFetchXml = "<fetch><entity name=\"ipg_associatedcpt\"><filter type=\"or\"><condition attribute=\"ipg_carrierid\" operator=\"eq\" value=\"" + accountId + "\" /><condition attribute=\"ipg_facilityid\" operator=\"eq\" value=\"" + accountId + "\" /></filter></entity></fetch>";
                blockedCptCodesGridControl.setFilterXml(blockedCptCodesGridFetchXml);
                blockedCptCodesGridControl.refresh();
            }
            filter();
        }
        Account.FilterCPTGrid = FilterCPTGrid;
    })(Account = Intake.Account || (Intake.Account = {}));
})(Intake || (Intake = {}));
