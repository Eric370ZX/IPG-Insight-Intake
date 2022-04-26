/**
 * @namespace Intake.ClaimResponseLineRemark
 */
var Intake;
(function (Intake) {
    var ClaimResponseLineRemark;
    (function (ClaimResponseLineRemark) {
        /**
        * Called on RemarkCode Change event
        * @function Intake.ClaimResponseLineRemark.OnChangeRemarkCode
        * @returns {void}
      */
        function OnChangeRemarkCode(executionContext) {
            //debugger;
            var formContext = executionContext.getFormContext();
            var remarkCode = formContext.getAttribute("ipg_remarkcode");
            if (remarkCode != null) {
                var remarkCodeName = remarkCode.getValue()[0].name;
                var codeAttr = formContext.getAttribute("ipg_code");
                if (codeAttr != null) {
                    codeAttr.setValue(remarkCodeName);
                }
                var nameAttr = formContext.getAttribute("ipg_name");
                if (nameAttr != null) {
                    nameAttr.setValue(remarkCodeName);
                }
            }
        }
        ClaimResponseLineRemark.OnChangeRemarkCode = OnChangeRemarkCode;
    })(ClaimResponseLineRemark = Intake.ClaimResponseLineRemark || (Intake.ClaimResponseLineRemark = {}));
})(Intake || (Intake = {}));
