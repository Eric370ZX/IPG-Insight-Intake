/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        /**
          * Called on onchange event of name field. See D365 configuration for details.
          * @function Intake.Utility.CheckCPTForFormatting
          * @returns {void}
          */
        function CheckCPTForFormatting(executionContext) {
            //debugger;
            var regexp = new RegExp("^(\\d{4}([A-Za-z0-9]{1}))$");
            var formContext = executionContext.getFormContext();
            var cptCodeAttribute = formContext.getAttribute("ipg_cptcode");
            if (cptCodeAttribute) {
                var cptCodeControl = formContext.getControl("ipg_cptcode");
                if (cptCodeControl) {
                    if (!regexp.test(cptCodeAttribute.getValue())) {
                        cptCodeControl.setNotification("Incorrect CPT code format: must contain four numbers followed by one alphanumeric character", 'cptcode');
                    }
                    else {
                        cptCodeControl.clearNotification('cptcode');
                    }
                }
            }
        }
        Utility.CheckCPTForFormatting = CheckCPTForFormatting;
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
