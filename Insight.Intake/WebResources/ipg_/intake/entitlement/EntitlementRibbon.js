/**
 * @namespace Intake.Entitlement
 */
var Intake;
(function (Intake) {
    var Entitlement;
    (function (Entitlement) {
        /**
         * Open new entitlement form
         * @function Intake.Entitlement.NewEntitlementForm
         * @returns {void}
         */
        function NewEntitlementForm(PrimaryItemsId) {
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "entitlement";
            var formParameters = {};
            Xrm.WebApi.retrieveRecord("account", PrimaryItemsId[0], "?$select=customertypecode,name").then(function (account) {
                if (account.customertypecode == 923720000) {
                    formParameters["ipg_facilityid"] = PrimaryItemsId[0];
                    formParameters["ipg_facilityidname"] = account.name;
                }
                else if (account.customertypecode == 923720001) {
                    formParameters["ipg_carrierid"] = PrimaryItemsId[0];
                    formParameters["ipg_carrieridname"] = account.name;
                    formParameters["customerid"] = PrimaryItemsId[0];
                }
                Xrm.Navigation.openForm(entityFormOptions, formParameters);
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ message: error.message });
            });
        }
        Entitlement.NewEntitlementForm = NewEntitlementForm;
    })(Entitlement = Intake.Entitlement || (Intake.Entitlement = {}));
})(Intake || (Intake = {}));
