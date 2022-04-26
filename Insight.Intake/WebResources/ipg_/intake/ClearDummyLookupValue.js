/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        var dummyId = '00000000-0000-0000-0000-000000000000';
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Utility.ShowQuickViewFormIfRecordNotFound
         * @returns {void}
         */
        function ClearDummyLookupValue(lookupFieldKey) {
            var lookupFieldAttribute = Xrm.Page.getAttribute(lookupFieldKey);
            var lookupFieldValue = lookupFieldAttribute.getValue();
            if (lookupFieldValue && lookupFieldValue.length) {
                lookupFieldValue = lookupFieldValue.slice();
                for (var index = 0, length_1 = lookupFieldValue.length; index < length_1; index++) {
                    var value = lookupFieldValue[index];
                    if (value.id === dummyId || value.id === "{" + dummyId + "}") {
                        lookupFieldValue.splice(index, 1);
                    }
                }
                lookupFieldAttribute.setValue(lookupFieldValue);
            }
        }
        Utility.ClearDummyLookupValue = ClearDummyLookupValue;
        /**
         * @namespace Intake.Utility.ClearDummyLookupValue
         */
        (function (ClearDummyLookupValue) {
            /**
             * Called when the form is loaded. See D365 configuration for details.
             * @function Intake.Utility.ClearDummyLookupValue.FromJSON
             * @returns {void}
             */
            function FromJSON(jsonString) {
                var payload = JSON.parse(jsonString);
                payload.forEach(function (lookupFieldKey) { return ClearDummyLookupValue(lookupFieldKey); });
            }
            ClearDummyLookupValue.FromJSON = FromJSON;
        })(ClearDummyLookupValue = Utility.ClearDummyLookupValue || (Utility.ClearDummyLookupValue = {}));
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
