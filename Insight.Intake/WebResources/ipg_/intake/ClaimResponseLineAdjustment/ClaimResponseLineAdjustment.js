/**
 * @namespace Intake.ClaimResponseLineAdjustment
 */
var Intake;
(function (Intake) {
    var ClaimResponseLineAdjustment;
    (function (ClaimResponseLineAdjustment) {
        /**
        * Called on ClaimStatus Change event
        * @function Intake.ClaimResponseLineAdjustment.OnChangeClaimStatus
        * @returns {void}
      */
        function OnChangeClaimStatus(executionContext) {
            //debugger;
            var formContext = executionContext.getFormContext();
            var claimStatus = formContext.getAttribute("ipg_claimstatus");
            if (claimStatus != null) {
                var claimStatusName = claimStatus.getValue()[0].name;
                if (claimStatusName.includes(" - ")) {
                    var adjustmentCode = claimStatusName.substring(0, claimStatusName.indexOf(" - "));
                    var adjustmentCodeAttr = formContext.getAttribute("ipg_code");
                    if (adjustmentCodeAttr != null) {
                        adjustmentCodeAttr.setValue(adjustmentCode);
                    }
                }
            }
        }
        ClaimResponseLineAdjustment.OnChangeClaimStatus = OnChangeClaimStatus;
    })(ClaimResponseLineAdjustment = Intake.ClaimResponseLineAdjustment || (Intake.ClaimResponseLineAdjustment = {}));
})(Intake || (Intake = {}));
