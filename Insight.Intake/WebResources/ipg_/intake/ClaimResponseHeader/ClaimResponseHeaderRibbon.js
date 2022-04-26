/**
 * @namespace Intake.ClaimResponseHeader.Ribbon
 */
var Intake;
(function (Intake) {
    var ClaimResponseHeader;
    (function (ClaimResponseHeader) {
        var Ribbon;
        (function (Ribbon) {
            var isButtonEnabled = false;
            var BatchStatus_New = 1;
            var BatchStatus_InProgress = 427880001;
            /**
             * Enable \ Disable Subgrid Add New button
             * @function Intake.ClaimResponseHeader.Ribbon.SubgridAddButtonEnabled
             * @returns {boolean}
            */
            function SubgridAddButtonEnabled(primaryControl) {
                var formContext = primaryControl;
                var isManualBatch = formContext.getAttribute("ipg_ismanualbatch");
                if (isManualBatch != null && isManualBatch.getValue() == true) {
                    var statusCode = formContext.getAttribute("statuscode");
                    if (statusCode != null && (statusCode.getValue() == BatchStatus_New || statusCode.getValue() == BatchStatus_InProgress)) {
                        isButtonEnabled = true;
                    }
                    else {
                        isButtonEnabled = false;
                    }
                }
                return isButtonEnabled;
            }
            Ribbon.SubgridAddButtonEnabled = SubgridAddButtonEnabled;
            /**
             * Apply button OnClick
             * @function Intake.ClaimResponseHeader.Ribbon.OnClickApply
             * @returns {void}
            */
            function OnClickApply(primaryControl) {
                var formContext = primaryControl;
                var claimResponseHeaders = [];
                claimResponseHeaders.push({ Id: formContext.data.entity.getId() });
                Xrm.Utility.showProgressIndicator("Processing...");
                formContext.data.save().then(function () {
                    Xrm.Utility.closeProgressIndicator();
                    Intake.Claim.Ribbon.OnPostClick(claimResponseHeaders);
                }, function (error) {
                    var _a;
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openAlertDialog({ text: (_a = error === null || error === void 0 ? void 0 : error.message) !== null && _a !== void 0 ? _a : error });
                });
            }
            Ribbon.OnClickApply = OnClickApply;
            /**
             * Cancel button OnClick
             * @function Intake.ClaimResponseHeader.Ribbon.OnClickCancel
             * @returns {void}
            */
            function OnClickCancel(primaryControl) {
                var formContext = primaryControl;
                var claimResponseHeaderId = formContext.data.entity.getId();
                var claimResponseBatchAttr = formContext.getAttribute("ipg_claimresponsebatchid");
                Xrm.Navigation.openConfirmDialog({ title: "Cancel Payment", text: "Do you want to Cancel this payment and return to the Batch form?" }, { height: 300, width: 400 })
                    .then(function (success) {
                    if (success.confirmed) {
                        if (claimResponseBatchAttr != null) {
                            var claimResponseBatchId_1 = claimResponseBatchAttr.getValue()[0].id;
                            Xrm.WebApi.deleteRecord("ipg_claimresponseheader", claimResponseHeaderId)
                                .then(function (result) {
                                if (claimResponseBatchAttr != null) {
                                    var entityFormOptions = {};
                                    entityFormOptions["entityName"] = "ipg_claimresponsebatch";
                                    entityFormOptions["entityId"] = claimResponseBatchId_1;
                                    Xrm.Navigation.openForm(entityFormOptions);
                                }
                            }, function (error) {
                                console.log(error);
                            });
                        }
                        else {
                            console.log("Batch not found.");
                        }
                    }
                });
            }
            Ribbon.OnClickCancel = OnClickCancel;
            /**
             * Determinr if Payment is manual or not
             * @function Intake.ClaimResponseHeader.Ribbon.IsManualPayment
             * @returns {boolean}
            */
            function IsManualPayment(primaryControl) {
                var formContext = primaryControl;
                var isManualPayment = formContext.getAttribute("ipg_ismanualpayment");
                if (isManualPayment != null && isManualPayment.getValue() == true) {
                    return true;
                }
                else {
                    return false;
                }
            }
            Ribbon.IsManualPayment = IsManualPayment;
        })(Ribbon = ClaimResponseHeader.Ribbon || (ClaimResponseHeader.Ribbon = {}));
    })(ClaimResponseHeader = Intake.ClaimResponseHeader || (Intake.ClaimResponseHeader = {}));
})(Intake || (Intake = {}));
