namespace Insight.Intake.TaskTypeConfiguration {

  const taskTypeConfigId = "ipg_id";

  export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
    let formContext = executionContext.getFormContext();

    if (formContext.ui.getFormType() !== 1) {
      formContext.getControl(taskTypeConfigId).setDisabled(true);
    }
  }
}
