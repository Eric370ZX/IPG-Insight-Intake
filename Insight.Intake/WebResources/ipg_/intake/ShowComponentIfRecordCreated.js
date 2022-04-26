/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        var ComponentType;
        (function (ComponentType) {
            ComponentType["Tab"] = "Tab";
            ComponentType["Section"] = "Section";
            ComponentType["Control"] = "Control";
        })(ComponentType = Utility.ComponentType || (Utility.ComponentType = {}));
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Utility.ShowComponentIfRecordCreated
         * @returns {void}
         */
        function ShowComponentIfRecordCreated(componentType) {
            var keys = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                keys[_i - 1] = arguments[_i];
            }
            var formType = Xrm.Page.ui.getFormType();
            var isRecordCreated = 2 /* Update */ === formType || 4 /* Disabled */ === formType || 3 /* ReadOnly */ === formType;
            var component;
            switch (componentType) {
                case ComponentType.Tab:
                    {
                        var tabKey = keys[0];
                        component = Xrm.Page.ui.tabs.get(tabKey);
                    }
                    break;
                case ComponentType.Section:
                    {
                        var tabKey = keys[0], sectionKey = keys[1];
                        var tab = Xrm.Page.ui.tabs.get(tabKey);
                        component = tab && tab.sections.get(sectionKey);
                    }
                    break;
                case ComponentType.Control:
                    {
                        var controlKey = keys[0];
                        component = Xrm.Page.getControl(controlKey);
                    }
                    break;
                default:
                    break;
            }
            if (component) {
                component.setVisible(isRecordCreated);
            }
        }
        Utility.ShowComponentIfRecordCreated = ShowComponentIfRecordCreated;
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
