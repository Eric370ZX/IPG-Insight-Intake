/**
 * @namespace Intake.ClaimResponseBatch.Ribbon
 */
var Intake;
(function (Intake) {
    var ClaimResponseBatch;
    (function (ClaimResponseBatch) {
        var Ribbon;
        (function (Ribbon) {
            var isButtonEnabled = false;
            var BatchStatus_New = 1;
            /**
             * Enable \ Disable Delete button
             * @function Intake.ClaimResponseBatch.Ribbon.DeleteButtonEnabled
             * @returns {boolean}
            */
            function DeleteButtonEnabled(primaryControl) {
                var formContext = primaryControl;
                var statusCode = formContext.getAttribute("statuscode");
                var recordCount = formContext.getAttribute("ipg_recordcount");
                if (statusCode != null && statusCode.getValue() == BatchStatus_New && recordCount != null && recordCount.getValue() == 0) {
                    isButtonEnabled = true;
                }
                else {
                    isButtonEnabled = false;
                }
                return isButtonEnabled;
            }
            Ribbon.DeleteButtonEnabled = DeleteButtonEnabled;
            /**
             * Called on Archive button click
             * @function Intake.ClaimResponseBatch.Ribbon.OnArchiveClick
             * @returns {void}
            */
            function OnArchiveClick(primaryControl) {
                var formContext = primaryControl;
                if (confirm("Are you sure you want to archive the claim response batch?")) {
                    var data = {
                        statuscode: 427880003 //archive
                    };
                    Xrm.WebApi.updateRecord('ipg_claimresponsebatch', formContext.data.entity.getId(), data).then(function success(result) {
                        formContext.data.refresh(false);
                    }, function (error) {
                        console.log(error.message);
                    });
                }
            }
            Ribbon.OnArchiveClick = OnArchiveClick;
        })(Ribbon = ClaimResponseBatch.Ribbon || (ClaimResponseBatch.Ribbon = {}));
    })(ClaimResponseBatch = Intake.ClaimResponseBatch || (Intake.ClaimResponseBatch = {}));
})(Intake || (Intake = {}));
