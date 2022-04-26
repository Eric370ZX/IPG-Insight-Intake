var Insight;
(function (Insight) {
    var Intake;
    (function (Intake) {
        var TaskTypeConfiguration;
        (function (TaskTypeConfiguration) {
            var taskTypeConfigId = "ipg_id";
            function OnLoadForm(executionContext) {
                var formContext = executionContext.getFormContext();
                if (formContext.ui.getFormType() !== 1) {
                    formContext.getControl(taskTypeConfigId).setDisabled(true);
                }
            }
            TaskTypeConfiguration.OnLoadForm = OnLoadForm;
        })(TaskTypeConfiguration = Intake.TaskTypeConfiguration || (Intake.TaskTypeConfiguration = {}));
    })(Intake = Insight.Intake || (Insight.Intake = {}));
})(Insight || (Insight = {}));
