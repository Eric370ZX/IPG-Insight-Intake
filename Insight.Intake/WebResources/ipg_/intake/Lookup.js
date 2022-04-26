/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Utility.Lookup
         * @returns {void}
         */
        function Lookup(fieldKey, lookupFieldKey, entityType) {
            var attribute = Xrm.Page.getAttribute(fieldKey);
            var lookupAttribute = Xrm.Page.getAttribute(lookupFieldKey);
            var attributeName = attribute.getName();
            // Listener.
            function onChange() {
                // WORKAROUND: Line below should be changed when Microsoft added API for receive selected item from the control.
                var customControl = window.parent.Ipg.CustomControls.CCManager.getControl(attribute.getName());
                if (customControl.selectedValue) {
                    lookupAttribute.setValue([
                        {
                            id: customControl.selectedValue,
                            name: "Automatically Selected Value (From: " + attributeName + ")",
                            entityType: entityType
                        }
                    ]);
                }
                else {
                    lookupAttribute.setValue(null);
                }
                lookupAttribute.fireOnChange();
            }
            attribute.addOnChange(onChange);
        }
        Utility.Lookup = Lookup;
        /**
         * @namespace Intake.Utility.Lookup
         */
        (function (Lookup) {
            /**
             * Called when the form is loaded. See D365 configuration for details.
             * @function Intake.Utility.Lookup
             * @returns {void}
             */
            function FromJSON(jsonString) {
                var _this = this;
                var payload = JSON.parse(jsonString);
                payload.forEach(function (tuple) { return Lookup.apply(_this, tuple); });
            }
            Lookup.FromJSON = FromJSON;
        })(Lookup = Utility.Lookup || (Utility.Lookup = {}));
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
