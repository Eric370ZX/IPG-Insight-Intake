namespace Intake.TaskType {
  const attributes = {
    assignToTeam: "ipg_assigntoteam",
    portal: "ipg_isportal"
  }
  const portalTeamId = "f6a56f2c-c41d-eb11-a813-00224806f885";

  export function OnLoad(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    let assignedToTeam = formContext.getAttribute(attributes.assignToTeam);
    assignedToTeam.addOnChange(() => {
      SetPortalByAssignedTeam(formContext);
    });
    assignedToTeam.fireOnChange();
  }

  function SetPortalByAssignedTeam(formContext: Xrm.FormContext) {
    let assignedToTeam = formContext.getAttribute(attributes.assignToTeam)?.getValue();
    let assignedToTeamId = assignedToTeam
        ? Intake.Utility.GetFormattedId(assignedToTeam[0]?.id)
        : null

    if (assignedToTeamId == portalTeamId) {
      formContext.getAttribute(attributes.portal).setValue(true);
    }
  }
}
