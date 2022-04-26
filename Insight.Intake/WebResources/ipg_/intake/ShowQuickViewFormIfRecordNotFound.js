/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        var dummyId = '00000000-0000-0000-0000-000000000000';
        var controls = [];
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Utility.ShowQuickViewFormIfRecordNotFound
         * @returns {void}
         */
        function ShowQuickViewFormIfRecordNotFound(fieldKey, lookupFieldKey, quickViewFormKey, entityType) {
            var masterFieldAttribute = Xrm.Page.getAttribute(fieldKey);
            var lookupFieldAttribute = Xrm.Page.getAttribute(lookupFieldKey);
            function addDummyLookupValue() {
                var lookupValue = lookupFieldAttribute.getValue();
                if (!lookupValue || lookupValue.length === 0) {
                    lookupFieldAttribute.setValue([
                        {
                            id: dummyId,
                            name: 'Dummy Data',
                            entityType: entityType
                        }
                    ]);
                }
            }
            function wait() {
                return new Promise(function (resolve) { return setTimeout(resolve, 0); });
            }
            // Master fields change listener
            function changeListener() {
                addDummyLookupValue();
                // Wait until the business rule is executed.
                wait().then(function () {
                    controls.forEach(function (_a) {
                        var masterControl = _a[0], lookupControl = _a[1];
                        if (!masterControl.getVisible()) {
                            lookupControl.getAttribute().setValue(null);
                        }
                    });
                });
            }
            masterFieldAttribute.addOnChange(changeListener);
            // We should set the dummy data, if the master field contains data and
            // the related lookup field does not contains data.
            if (masterFieldAttribute.getValue()) {
                addDummyLookupValue();
            }
        }
        Utility.ShowQuickViewFormIfRecordNotFound = ShowQuickViewFormIfRecordNotFound;
        /**
         * @namespace Intake.Utility.ShowQuickViewFormIfRecordNotFound
         */
        (function (ShowQuickViewFormIfRecordNotFound) {
            /**
             * Called when the form is loaded. See D365 configuration for details.
             * @function Intake.Utility.ShowQuickViewFormIfRecordNotFound
             * @returns {void}
             */
            function FromJSON(jsonString) {
                var _this = this;
                var payload = JSON.parse(jsonString);
                payload.forEach(function (tuple) {
                    var fieldKey = tuple[0], lookupFieldKey = tuple[1];
                    var control = Xrm.Page.getControl(fieldKey);
                    var lookupControl = Xrm.Page.getControl(lookupFieldKey);
                    controls.push([control, lookupControl]);
                    ShowQuickViewFormIfRecordNotFound.apply(_this, tuple);
                });
            }
            ShowQuickViewFormIfRecordNotFound.FromJSON = FromJSON;
        })(ShowQuickViewFormIfRecordNotFound = Utility.ShowQuickViewFormIfRecordNotFound || (Utility.ShowQuickViewFormIfRecordNotFound = {}));
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
