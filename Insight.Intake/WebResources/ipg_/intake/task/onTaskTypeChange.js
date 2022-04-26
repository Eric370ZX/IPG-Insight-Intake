/**
 * @namespace Intake.Task
 *
 */
var Intake;
(function (Intake) {
    var Task;
    (function (Task) {
        function onChangeTaskType(executionContext) {
            var formContext = executionContext.getFormContext();
            var taskTypeAttribute = formContext.getAttribute("ipg_tasktypecode");
            if (taskTypeAttribute) {
                if (taskTypeAttribute.getValue() != null) {
                    if (taskTypeAttribute.getValue() == "427880059") {
                        formContext.getControl("ipg_documenttype").setVisible(true);
                        formContext.getControl("ownerid").setVisible(false);
                        formContext.getControl("ipg_owner").setVisible(true);
                        formContext.getAttribute("ipg_owner").setRequiredLevel("required");
                    }
                    else {
                        formContext.getControl("ipg_documenttype").setVisible(false);
                        formContext.getControl("ownerid").setVisible(true);
                        formContext.getControl("ipg_owner").setVisible(false);
                        formContext.getAttribute("ipg_owner").setValue(null);
                        formContext.getAttribute("ipg_owner").setRequiredLevel("none");
                    }
                }
            }
        }
        Task.onChangeTaskType = onChangeTaskType;
    })(Task = Intake.Task || (Intake.Task = {}));
})(Intake || (Intake = {}));
