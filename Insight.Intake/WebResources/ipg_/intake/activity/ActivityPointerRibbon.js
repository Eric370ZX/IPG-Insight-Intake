/**
 * @namespace Intake.ActivityPointer
 *
 */
var Intake;
(function (Intake) {
    var ActivityPointer;
    (function (ActivityPointer) {
        function OpenRescheduleTaskDialog(selectedItemIds) {
            var data = {
                taskId: selectedItemIds[0],
                additionalTasks: ""
            };
            if (selectedItemIds.length > 1) {
                data.additionalTasks = selectedItemIds.slice(1, selectedItemIds.length).toString();
            }
            Xrm.Navigation.openWebResource("ipg_/intake/task/rescheduleTask.html", { width: 800, height: 670, openInNewWindow: true }, JSON.stringify(data));
        }
        ActivityPointer.OpenRescheduleTaskDialog = OpenRescheduleTaskDialog;
    })(ActivityPointer = Intake.ActivityPointer || (Intake.ActivityPointer = {}));
})(Intake || (Intake = {}));
