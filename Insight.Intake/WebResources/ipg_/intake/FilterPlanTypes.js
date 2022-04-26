/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        /**
         * Called on load form
         * @function Intake.Utility.OnLoad
         * @returns {void}
        */
        function OnLoad(executionContext) {
            var formContext = executionContext.getFormContext();
            RemoveUnusedPlanTypes(formContext);
            SetEffectiveDate(formContext);
        }
        Utility.OnLoad = OnLoad;
        function SetEffectiveDate(executionContext) {
            var formContext = executionContext._entityName ? executionContext : executionContext.getFormContext();
            var todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
            var expirationDate = new Date("12/31/9999");
            var effectiveDateAttr = formContext.getAttribute("ipg_effectivedate");
            if (effectiveDateAttr && effectiveDateAttr.getValue() == null) {
                formContext.getAttribute("ipg_effectivedate").setValue(todayDate);
                formContext.getAttribute("ipg_expirationdate").setValue(expirationDate);
            }
        }
        Utility.SetEffectiveDate = SetEffectiveDate;
        /**
        * Removes unused plan types
        * @function Intake.Utility.RemoveUnusedPlanTypes
        * @returns {void}
        */
        function RemoveUnusedPlanTypes(formContext) {
            // field names in Case, Account and Health Plan Network entities
            var fields = ["ipg_primarycarrierplantype", "ipg_carriersupportedplantypes", "ipg_plantype", "ipg_excludedplantypemultiselect"];
            fields.forEach(function (fieldName) {
                var attribute = formContext.getAttribute(fieldName);
                if (attribute)
                    RemovePlanTypes(formContext, attribute);
            });
        }
        /**
        * Gets disabled plan types
        * @function Intake.Utility.RemovePlanTypes
        * @returns {void}
        */
        function RemovePlanTypes(formContext, attribute) {
            var query = "?$select=ipg_disabledplantypes&$filter=ipg_name eq 'Disabled plan types'";
            Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", query)
                .then(function (result) {
                if (result.entities.length > 0) {
                    var disabledPlanTypes = result.entities[0].ipg_disabledplantypes;
                    if (disabledPlanTypes) {
                        var currentPlanType = attribute.getValue();
                        var array = disabledPlanTypes.split(",");
                        array.forEach(function (value) {
                            if (currentPlanType != value)
                                formContext.getControl(attribute._attributeName).removeOption(Number(value));
                        });
                    }
                }
            }, function (error) {
                console.log(error.message);
            });
        }
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
