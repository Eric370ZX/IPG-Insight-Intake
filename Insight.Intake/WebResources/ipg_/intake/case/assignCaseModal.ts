namespace Intake.Case.Assign {
  const selectedRecordLabel: string =
    "You have selected <&N> record(-s). Please indicate the User and/or Team to which these records should be assigned.";
  const confirmtring:string = "System is about to reassign all related open User Tasks to the User you assigned this Case to. Do you wish to proceed?";
  const assignTypes = {
    me: "Me",
    userOrTeam: "User or team"
  };
  let selectedRecordsIds: string[];
  let Xrm: Xrm.XrmStatic;

  export async function InitData() {
    try {
      selectedRecordsIds = JSON.parse(GetUrlParameter("data"))
        .selectedRecordsIds as string[];
      if (!Array.isArray(selectedRecordsIds)) {
        selectedRecordsIds = new Array<string>(
          (selectedRecordsIds as string)
            .replace("{", "")
            .replace("}", "")
            .toLocaleLowerCase()
        );
      }
      Xrm = window.parent.Xrm;
      SetSelectedRecordsLabel(selectedRecordsIds.length);
      await FillTeams();
    } catch (error) {
      debugger;
      console.log(error);
    }
  }
  function SetSelectedRecordsLabel(count) {
    document.getElementById("selected-records-label").innerText =
      selectedRecordLabel.replace("<&N>", count.toString());
  }
  function RetrieveAllTeamRecords() {
    var teamFetchXML =
      "<?xml version='1.0'?>" +
      "<fetch distinct='false' mapping='logical' outputformat='xmlplatform' version='1.0'>" +
      "<entity name='team'>" +
      "</entity>" +
      "</fetch>";

    teamFetchXML = "?fetchXml=" + encodeURIComponent(teamFetchXML);
    return Xrm.WebApi.retrieveMultipleRecords("team", teamFetchXML) as any;
  }
  async function FillTeams() {
    const teams = await RetrieveAllTeamRecords();
    let select = document.getElementById("assign-to-team-select") as any;

    teams.entities.forEach((team) => {
      select.options[select.options.length] = new Option(
        team.name,
        team.teamid
      );
    });
  }

  function IsMeAssignTypeSelected(): boolean {
    let select = document.getElementById("assign-to-type-select") as any;
    let selected: string = select.options[select.selectedIndex].text;
    if (selected === assignTypes.userOrTeam) {
      return false;
    }
    return true;
  }

  async function GetTeamsByLoggedUser() {
    let loggedUserId = Xrm.Utility.getGlobalContext()
      .userSettings.userId.toLowerCase()
      .replace("{", "")
      .replace("}", "");

    let teamFetchXML = `<?xml version='1.0'?>
     <fetch distinct='true' mapping='logical' outputformat='xmlplatform' version='1.0'>
      <entity name='team'>
      <attribute name='name'/>
      <attribute name='teamid'/>
        <link-entity name='teammembership' intersect='true' visible='false' to='teamid' from='teamid'>
          <link-entity name='systemuser' to='systemuserid' from='systemuserid' alias='ab'>
            <filter type='and'>
              <condition attribute='systemuserid' value='${loggedUserId}' operator='eq'/>
            </filter>
          </link-entity>
        </link-entity>
      </entity>
     </fetch>`;

    teamFetchXML = "?fetchXml=" + encodeURIComponent(teamFetchXML);
    return Xrm.WebApi.retrieveMultipleRecords("team", teamFetchXML) as any;
  }
  async function PopulateAssignedToTeamByDefault() {
    let teams = (await GetTeamsByLoggedUser()).entities;
    if (teams.length == 1) {
      let select = document.getElementById("assign-to-team-select") as any;

      select.options[0] = new Option(teams[0].name, teams[0].teamid);
    } else return;
  }

  export function OnAssignToTypeChanged() {
    let isMeAssignTypeSelected = IsMeAssignTypeSelected();
    if (!isMeAssignTypeSelected) {
      PopulateAssignedToTeamByDefault();
    }
    (document.getElementById("assign-to-team-select") as any).disabled =
      isMeAssignTypeSelected;
    (document.getElementById("assign-to-user-select") as any).disabled =
      isMeAssignTypeSelected;

    if (isMeAssignTypeSelected) {
      ClearSelectById("assign-to-team-select");
      ClearSelectById("assign-to-user-select");
    }
  }

  function GetUrlParameter(sParam) {
    let sPageURL = window.location.search.substring(1),
      sURLVariables = sPageURL.split("&"),
      sParameterName,
      i;

    for (i = 0; i < sURLVariables.length; i++) {
      sParameterName = sURLVariables[i].split("=");

      if (sParameterName[0] === sParam) {
        return sParameterName[1] === undefined
          ? ""
          : decodeURIComponent(sParameterName[1]);
      }
    }
  }
  export function ProcessAssign() {
    if (IsInputDataValid()) AssignCasesRequest();
    else OnError("Validation failed", "Please select any team ");
  }
  export function CloseDialog() {
    window.close();
  }
  function GetUsersByTeam(teamId: string) {
    let usersByTeamFetch: string = `<?xml version='1.0'?>
    <fetch distinct='true' mapping='logical' version='1.0'>
      <entity name='systemuser'>
        <attribute name='fullname'/>
        <attribute name='systemuserid'/>
          <filter type='and'>
            <condition attribute='isdisabled' value='0' operator='eq'/>
          </filter>
        <link-entity name='teammembership' intersect='true' visible='false' to='systemuserid' from='systemuserid'>
          <link-entity name='team' to='teamid' from='teamid'>
            <filter type='and'>
              <condition attribute='teamid' value='${teamId}' uitype='team' operator='eq'/>
            </filter>
          </link-entity>
        </link-entity>
      </entity>
    </fetch>`;

    usersByTeamFetch = "?fetchXml=" + encodeURIComponent(usersByTeamFetch);
    return Xrm.WebApi.retrieveMultipleRecords(
      "systemuser",
      usersByTeamFetch
    ) as any;
  }
  export async function FillUsersBySelectedTeam() {
    let users = await GetUsersByTeam(
      (document.getElementById("assign-to-team-select") as any).value
    );
    let select = document.getElementById("assign-to-user-select") as any;

    users.entities.forEach((user) => {
      select.options[select.options.length] = new Option(
        user.fullname,
        user.systemuserid
      );
    });
  }
  export function ClearSelectById(id: string) {
    ($(`#${id}`) as any)
      .find("option")
      .remove()
      .end()
      .append('<option selected value="0"></option>');
  }

  function DoAssignToUser() {
    return (
      (document.getElementById("assign-to-user-select") as any).value != "0"
    );
  }
  async function CanCaseBeUpdated(id) {
    const caseRecord = await Xrm.WebApi.online.retrieveRecord(
      "incident",
      id,
      "?$select=title"
    );
    return caseRecord.title != null && caseRecord.title !== "";
  }
  function OnError(title: string, text: string) {
    var alertStrings = {
      confirmButtonLabel: "Ok",
      text: text,
      title: title,
    };
    var alertOptions = { height: 120, width: 260 };
    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (
      success
    ) {
      Xrm.Utility.closeProgressIndicator();
      window.close();
    });
  }
  function IsInputDataValid() {
    return IsMeAssignTypeSelected()
      ? true
      : (document.getElementById("assign-to-team-select") as any).value != "0";
  }
  async function AssignCasesRequest() {
    Xrm.Utility.showProgressIndicator("Processing...");
    
    const assignToTeam = (document.getElementById("assign-to-team-select") as any).value;
    const assignToUser = (document.getElementById("assign-to-user-select") as any).value;

    let isUserSelected = DoAssignToUser() || IsMeAssignTypeSelected();
    let selectedUserOrTeamId = IsMeAssignTypeSelected()
      ? Xrm.Utility.getGlobalContext()
          .userSettings.userId.toLowerCase()
          .replace("{", "")
          .replace("}", "")
      : DoAssignToUser()
      ? assignToUser
      : assignToTeam;
    let hasError: boolean;

    var entity = {};

    if(assignToTeam)
    {
      entity["ipg_assignedtoteamid@odata.bind"] = `/teams(${assignToTeam})`;
    }

    if (isUserSelected) 
    {
      var result =  await Xrm.Navigation.openConfirmDialog({text: confirmtring});
      
      if(!result.confirmed)
      {
        Xrm.Utility.closeProgressIndicator();
        return;
      }

      entity["ownerid@odata.bind"] = `/systemusers(${selectedUserOrTeamId})`;
    } 
    else 
    {
      entity["ownerid@odata.bind"] = `/teams(${selectedUserOrTeamId})`;
    }

    for (let i = 0; i < selectedRecordsIds.length; i++) {
      const isCasesValid = await CanCaseBeUpdated(selectedRecordsIds[i]);
      if (hasError || !isCasesValid) {
        OnError("Error", "Selected record(-s) can not be updated");
        break;
      }

      Xrm.WebApi.online
        .updateRecord("incident", selectedRecordsIds[i], entity)
        .then(
          function success(result) {
            if (i == selectedRecordsIds.length - 1) {
              var alertStrings = {
                confirmButtonLabel: "Ok",
                text: "Records were seccessfuly updated",
                title: "Success",
              };
              var alertOptions = { height: 120, width: 260 };
              Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                function (success) {
                  Xrm.Utility.closeProgressIndicator();
                  window.close();
                }
              );
            }
          },
          function (error) {
            Xrm.Utility.closeProgressIndicator();
            Xrm.Navigation.openAlertDialog(error.message);
            hasError = true;
          }
        );
    }
  }
}
$(() => {
  Intake.Case.Assign.InitData();
  $("#assign-to-type-select")
    .on("change", () => {
      Intake.Case.Assign.OnAssignToTypeChanged();
    })
    .trigger("change");
  $("#okBtn").on("click", () => {
    Intake.Case.Assign.ProcessAssign();
  });
  $("#cancelBtn").on("click", () => {
    Intake.Case.Assign.CloseDialog();
  });
  $("#assign-to-team-select").on("change", () => {
    Intake.Case.Assign.ClearSelectById("assign-to-user-select");
    Intake.Case.Assign.FillUsersBySelectedTeam();
  });
});
