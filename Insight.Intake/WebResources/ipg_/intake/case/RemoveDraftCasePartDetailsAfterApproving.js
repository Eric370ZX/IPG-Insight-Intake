/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        /**
         * Called on saving case
         * @function Intake.Case.RemoveDraftCasePartDetailsAfterApproving
         * @returns {void}
        */
        function RemoveDraftCasePartDetailsAfterApproving(context) {
            var saveEvent = context.getEventArgs();
            if (saveEvent.getSaveMode() == 70) {
                saveEvent.preventDefault();
                return;
            }
            var partViewIsChanged = Xrm.Page.getAttribute("ipg_partviewischanged");
            if (partViewIsChanged.getValue()) {
                saveEvent.preventDefault();
                var entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
                Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=ipg_caseid/incidentid eq " + entityId + " and statuscode eq 1").then(function success(result) {
                    if (result && result.entities.length) {
                        var promises = [];
                        for (var _i = 0, _a = result.entities; _i < _a.length; _i++) {
                            var part = _a[_i];
                            promises.push(Xrm.WebApi.deleteRecord("ipg_casepartdetail", part.ipg_casepartdetailid));
                        }
                        Promise.all(promises).then(function () {
                            partViewIsChanged.setValue(false);
                            Xrm.Page.data.save();
                        });
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
        Case.RemoveDraftCasePartDetailsAfterApproving = RemoveDraftCasePartDetailsAfterApproving;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
