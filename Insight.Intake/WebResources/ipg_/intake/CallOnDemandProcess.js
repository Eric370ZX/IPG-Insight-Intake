/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Utility.CallOnDemandProcess
         * @returns {void}
         */
        function CallOnDemandProcess(entityTypeCode, processId) {
            var globalContext = Xrm.Utility.getGlobalContext();
            var clientUrl = globalContext.getClientUrl();
            var entityId = Xrm.Page.data.entity.getId();
            var promise = Promise.resolve();
            var requestOptions = {
                path: clientUrl + "/api/data/v9.0/workflows(" + processId + ")/Microsoft.Dynamics.CRM.ExecuteWorkflow",
                body: {
                    EntityId: Intake.Utility.removeCurlyBraces(entityId),
                },
                headers: {
                    'OData-MaxVersion': '4.0',
                    'OData-Version': '4.0',
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
            };
            if (Xrm.Page.data.entity.getIsDirty()) {
                promise = promise.then(function () {
                    return Xrm.Navigation
                        .openConfirmDialog({
                        title: 'Unsaved Changes',
                        text: 'Your changes have not been saved. To stay on the page so that you can save your changes, click Cancel.',
                    });
                });
            }
            promise.then(function (payload) {
                if (payload && !payload.confirmed) {
                    return;
                }
                Xrm.Utility.showProgressIndicator(null);
                Intake.Utility.HttpRequest
                    .post(requestOptions)
                    .then(function () {
                    var closeProgressIndicatorAndRefreshRibbon = function () {
                        Xrm.Utility.closeProgressIndicator();
                        Xrm.Page.ui.refreshRibbon();
                    };
                    Xrm.Page.data.refresh(false)
                        .then(closeProgressIndicatorAndRefreshRibbon)
                        .catch(closeProgressIndicatorAndRefreshRibbon);
                })
                    .catch(function (response) {
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openErrorDialog({ message: response.error.message });
                });
            });
        }
        Utility.CallOnDemandProcess = CallOnDemandProcess;
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
