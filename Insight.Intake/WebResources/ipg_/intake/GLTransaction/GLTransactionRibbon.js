/**
 * @namespace Intake.GLTransaction.Ribbon
 */
var Intake;
(function (Intake) {
    var GLTransaction;
    (function (GLTransaction) {
        var Ribbon;
        (function (Ribbon) {
            /**
             * Called on Additional Details button click
             * @function Intake.GLTransaction.Ribbon.OnAdditionalDetailsClick
             * @returns {void}
            */
            function OnAdditionalDetailsClick(firstSelectedItemId, primaryControl) {
                var formContext = primaryControl;
                if (firstSelectedItemId == null)
                    return;
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_gltransaction";
                entityFormOptions["entityId"] = firstSelectedItemId.replace(/[{}]/g, "");
                entityFormOptions["formId"] = "cfd50ddf-8d0d-4072-8a83-ab41bdba30c1";
                Xrm.Navigation.openForm(entityFormOptions);
            }
            Ribbon.OnAdditionalDetailsClick = OnAdditionalDetailsClick;
            /**
             * Called on New Adjustment button click
             * @function Intake.GLTransaction.Ribbon.onNewAdjustmentClick
             * @returns {void}
            */
            function onNewAdjustmentClick(formContext) {
                var entityFormOptions = {
                    entityName: "ipg_adjustment",
                    openInNewWindow: true
                };
                var formParameters = {
                    ipg_caseid: formContext.data.entity.getId().replace(/[{}]/g, ""),
                    ipg_caseidname: formContext.getAttribute("title").getValue()
                };
                Xrm.Navigation.openForm(entityFormOptions, formParameters);
            }
            Ribbon.onNewAdjustmentClick = onNewAdjustmentClick;
            /**
             * Called on New Adjustment button click
             * @function Intake.GLTransaction.Ribbon.onNewPaymentBatchClick
             * @returns {void}
            */
            function onNewPaymentBatchClick(formContext) {
                var entityFormOptions = {
                    entityName: "ipg_claimresponsebatch",
                    openInNewWindow: true
                };
                Xrm.Navigation.openForm(entityFormOptions);
            }
            Ribbon.onNewPaymentBatchClick = onNewPaymentBatchClick;
        })(Ribbon = GLTransaction.Ribbon || (GLTransaction.Ribbon = {}));
    })(GLTransaction = Intake.GLTransaction || (Intake.GLTransaction = {}));
})(Intake || (Intake = {}));
