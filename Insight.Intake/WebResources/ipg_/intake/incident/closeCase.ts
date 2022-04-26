namespace Intake.Incident.CloseCase {
  let caseState: Xrm.OptionSetValue;
  const closeReasons: Xrm.OptionSetValue[] = [];
  const closeCategories: Xrm.OptionSetValue[] = [];
  const caseCloseMappings: ICaseCloseConfiguration[] = [];
  const incidentData: { ipg_statecode: number } = { ipg_statecode: -1 };
  export function init() {
    window.parent.Xrm.Utility.showProgressIndicator("Opening...");
    const initLoader = [
      loadOptions("ipg_caseclosurereason", closeReasons),
      loadOptions("ipg_caseclosurecategory", closeCategories),
      loadCaseClosureMapping(),
      loadCaseData(),
      checkIfCaseCanbeClosed(getUrlParameter("data") as string)
    ];
    Promise.all(initLoader).then((value) => {

      let displayMessage = `In order to close the case, ensure that there are no Actual Parts on the case, Revenue has been removed,
                              there are no POs, no outstanding Claims, no payments received and all AR balances are at zero.`;
      const msgs = value.filter(p => typeof p === "string" && !isEmptyOrSpaces(p)).map(p => `${p};\n`);
      if (msgs && msgs.length > 0) {
        displayMessage += `\nWarning messages:\n${msgs}`;
        alertDialog(displayMessage, "OK", () => {
          filterClosureReasons();
          window.parent.Xrm.Utility.closeProgressIndicator();
        }, null);
      }
      filterClosureReasons();
      window.parent.Xrm.Utility.closeProgressIndicator();
    }, (reason) => {
      alertDialog(`${reason}`, "ERROR", () => {
        window.parent.close();
      }, null);
    });

    $("#closeCategory").change(filterCloseReason);

  }
  export function ProcessCase() {
    const closeReason = $("#closeReason").val();
    const closeReasonDescr = $("#closeReason option:selected").text();
    const closeCaseConfig = caseCloseMappings
      .find(p => p.CaseState.value == caseState.value && p.ClosureReason.value == closeReason);
    const closedBy = closeCaseConfig.ClosedBy;
    const facilityСommunication = closeCaseConfig.FacilityCommunication;
    const eventLogEntry = closeCaseConfig.EventLogEntry;
    const closeNotes = $("#caseNote").val();
    const caseId = getUrlParameter("data");
    const onSuccess = () => {
      window.parent.Xrm.Utility.closeProgressIndicator();
      const storageKey = `closeCase-${caseId}`;
      localStorage.setItem(storageKey, "true");
      alertDialog("Case closed succesfully", "OK", () => {
        window.parent.close();
      }, null);
    };
    window.parent.Xrm.Utility.showProgressIndicator("Closing the case...");
    closeCaseRequest(caseId as string, closeReason as number, closeReasonDescr as string, closeNotes as string, closedBy,
      facilityСommunication, eventLogEntry, onSuccess, (message) => {
        window.parent.Xrm.Utility.closeProgressIndicator();
        alertDialog(`Error while closing case:${message}`, "ERROR", null, null);
      });
  }
  export function CloseCase() {
    window.parent.close();
  }
  function checkIfCaseCanbeClosed(caseid: string): Promise<any> {
    return new Promise<string>((resolve, reject) => {
      const target: { incidentid: string } = {
        incidentid: caseid
      };
      target["@odata.type"] = "Microsoft.Dynamics.CRM.incident";
      const parameters: { Target: any } = {
        Target: target,
      };

      var ipg_IPGCaseCloseCaseRequest = {
        Target: parameters.Target,

        getMetadata: function () {
          return {
            boundParameter: null,
            parameterTypes: {
              "Target": {
                "typeName": "mscrm.incident",
                "structuralProperty": 5
              }
            },
            operationType: 0,
            operationName: "ipg_IPGCaseCheckIfCaseCanBeClosed"
          };
        }
      };

      window.parent.Xrm.WebApi.online.execute(ipg_IPGCaseCloseCaseRequest).then(
        async function success(result) {
          if (result.ok) {
            const response = await result.json();
            if (response.Succeeded) {
              if (response.SeverityLevel == 923720000) {//info
                resolve("");
              } else if (response.SeverityLevel == 923720001) {//Warning 
                resolve(`Additional Activities are needed: \n${response.Output}`)
              }
            } else {
              reject(`Case can not be closed. ${response.Output}`);
            }
          }
        },
        function (error) {
          reject(error.message);
        }
      );
    });
  }
  function filterCloseReason() {
    const closeCategory = $("#closeCategory").val();
    //const closuresReasons
    const filteredData = caseCloseMappings
      .filter(p => p.CaseState != null && p.CaseState.value == caseState.value && p.ClosureCategory.value == closeCategory);

    populateOptionSet("closeReason", filteredData.map(p => p.ClosureReason).filter((value, index, self) => self.indexOf(value) === index));
  }
  function closeCaseRequest(caseid: string, closeReason: number, closeReasonDescr: string, closeNote: string,
    closedBy: string, facilityCommunication: string, eventLogEntry: string,
    onSuccess: () => void, onError: (message: string) => void) {
    const target: { incidentid: string } = {
      incidentid: caseid
    };
    target["@odata.type"] = "Microsoft.Dynamics.CRM.incident";
    const parameters: { Target: any, CloseReason: number, CloseNotes: string, CloseReasonDescr: string, ClosedBy: string, FacilityCommunication: string, EventLogEntry: string } = {
      Target: target,
      CloseReason: closeReason,
      CloseNotes: closeNote,
      CloseReasonDescr: closeReasonDescr,
      ClosedBy: closedBy,
      FacilityCommunication: facilityCommunication,
      EventLogEntry: eventLogEntry
    };

    var ipg_IPGCaseCloseCaseRequest = {
      Target: parameters.Target,
      CloseReason: parameters.CloseReason,
      CloseNotes: parameters.CloseNotes,
      CloseReasonDescr: parameters.CloseReasonDescr,
      ClosedBy: parameters.ClosedBy,
      FacilityCommunication: parameters.FacilityCommunication,
      EventLogEntry: parameters.EventLogEntry,
      getMetadata: function () {
        return {
          boundParameter: null,
          parameterTypes: {
            "Target": {
              "typeName": "mscrm.incident",
              "structuralProperty": 5
            },
            "CloseReason": {
              "typeName": "Edm.Int32",
              "structuralProperty": 1
            },
            "CloseReasonDescr": {
              "typeName": "Edm.String",
              "structuralProperty": 1
            },
            "CloseNotes": {
              "typeName": "Edm.String",
              "structuralProperty": 1
            },
            "ClosedBy": {
              "typeName": "Edm.String",
              "structuralProperty": 1
            },
            "FacilityCommunication": {
              "typeName": "Edm.String",
              "structuralProperty": 1
            },
            "EventLogEntry": {
              "typeName": "Edm.String",
              "structuralProperty": 1
            }
          },
          operationType: 0,
          operationName: "ipg_IPGCaseCloseCase"
        };
      }
    };

    window.parent.Xrm.WebApi.online.execute(ipg_IPGCaseCloseCaseRequest).then(
      function success(result) {
        if (result.ok) {
          onSuccess();
          //Success - No Return Data - Do Something
        }
      },
      function (error) {
        onError(error.message);
      }
    );
  }

  function filterClosureReasons() {
    caseState = caseCloseMappings
      .filter(p => (p.CaseState != null && p.CaseState.value == incidentData.ipg_statecode)).map(x => x.CaseState).filter((caseState, index, self) =>
        index === self.findIndex((t) => (
          t.text === caseState.text && t.value === caseState.value
        ))
      )[0];

    const filteredData = caseCloseMappings
      .filter(p => (p.CaseState != null && p.CaseState.value == caseState.value));

    const uniqueCloseCategoryNumbersSet = new Array<number>();
    const uniqueCloseCategorySet = new Array<Xrm.OptionSetValue>();
    const allCloseCategrories = filteredData
      .filter(p => (p.ClosureCategory.value != null && p.ClosureCategory.text != null)).map(p => p.ClosureCategory);
    if (allCloseCategrories == null || allCloseCategrories.length <= 0) {
      alertDialog("No close category has been found.", "ERROR", () => { window.parent.close(); }, null);
    }

    for (let i = 0; i < allCloseCategrories.length; i++) {
      if (uniqueCloseCategoryNumbersSet.indexOf(allCloseCategrories[i].value) == -1) {
        uniqueCloseCategoryNumbersSet.push(allCloseCategrories[i].value);
        uniqueCloseCategorySet.push(allCloseCategrories[i]);
      }
    }

   
    populateOptionSet("closeCategory", uniqueCloseCategorySet);
    
    populateOptionSet("closeReason", filteredData
      .map(p => p.ClosureReason)
      .filter((value, index, self) => self.indexOf(value) === index));//getting unique values

    $("#closeCategory").change();
  }

  function populateOptionSet(picklistName: string, data: Xrm.OptionSetValue[]) {
    $(`#${picklistName} option`).remove();
    data.forEach(p => $(`#${picklistName}`).prepend(`<option value="${p.value}">${p.text}</option>`));
    $(`#${picklistName} option:first-child`).prop("selected", "selected");
  }
  function loadCaseData(): Promise<void> {
    return new Promise<void>((resolve, reject) => {
      const caseId = getUrlParameter("data");
      window.parent.Xrm.WebApi.retrieveRecord("incident", caseId as string, `?$select=ipg_statecode`)
        .then(caseTarget => {
          incidentData.ipg_statecode = caseTarget.ipg_statecode;
          resolve();
        });
    });
  }
  function loadCaseClosureMapping(): Promise<void> {
    return new Promise<void>((resolve, reject) => {
      window.parent.Xrm.WebApi.online.retrieveMultipleRecords("ipg_casecloseconfiguration",
        "?$select=ipg_name,ipg_casecloseconfigurationid,ipg_caseclosurereason,ipg_caseclosuretype,ipg_caseclosurecategory,_ipg_casedisplaystatusid_value,ipg_casestate,ipg_closedby,ipg_facilitycommunication&$expand=ipg_casedisplaystatusid($select=ipg_name)&$filter=statecode eq 0")
        .then((results) => {
          for (var i = 0; i < results.entities.length; i++) {
            caseCloseMappings.push({
              CaseState: {
                value: results.entities[i]["ipg_casestate"] as number | undefined,
                text: results.entities[i]["ipg_casestate@OData.Community.Display.V1.FormattedValue"]
              },
              ClosureReason: {
                value: results.entities[i]["ipg_caseclosurereason"] as number | undefined,
                text: results.entities[i]["ipg_caseclosurereason@OData.Community.Display.V1.FormattedValue"]
              },
              ClosureCategory: {
                value: results.entities[i]["ipg_caseclosurecategory"] as number | undefined,
                text: results.entities[i]["ipg_caseclosurecategory@OData.Community.Display.V1.FormattedValue"]
              },
              EventLogEntry: results.entities[i]["ipg_name"],
              ClosedBy: results.entities[i]["ipg_closedby"],
              FacilityCommunication: results.entities[i]["ipg_facilitycommunication"],
            });
          }
          resolve();
        },
          error => {
            alertDialog(error.message, "ERROR", () => { reject() }, null);
          }
        );
    });
  }
  function loadOptions(pickListName: string, targetList: Xrm.OptionSetValue[]): Promise<void> {
    return new Promise<void>((resolve, reject) => {
      $.get(`/api/data/v9.0/GlobalOptionSetDefinitions(Name='${pickListName}')`, null, (data) => {
        for (let i = 0; i < data.Options.length; i++) {
          targetList.push({
            text: data.Options[i].Label.UserLocalizedLabel.Label,
            value: data.Options[i].Value
          });
        };
        resolve();
      }, "json");
    });
  }
  function getUrlParameter(sParam) {
    let sPageURL = window.location.search.substring(1),
      sURLVariables = sPageURL.split('&'),
      sParameterName,
      i;

    for (i = 0; i < sURLVariables.length; i++) {
      sParameterName = sURLVariables[i].split('=');

      if (sParameterName[0] === sParam) {
        return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
      }
    }
  };

  function alertDialog(alertText: string, alertTitle: string, onSuccess: () => void, onError: () => void) {
    let alertStrings = { confirmButtonLabel: "OK", text: alertText, title: alertTitle };
    let alertOptions = { height: 120, width: 260 };
    window.parent.Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
      function (success) {
        onSuccess();
      },
      function (error) {
        onError();
      }
    );
  }

  function isEmptyOrSpaces(str){
    return str === null || str.match(/^ *$/) !== null;
}
}
interface ICaseCloseConfiguration {
  ClosureReason: Xrm.OptionSetValue,
  ClosureCategory: Xrm.OptionSetValue,
  CaseState: Xrm.OptionSetValue,
  ClosedBy: string,
  FacilityCommunication: string,
  EventLogEntry: string
}
$(document).ready(() => {
  Intake.Incident.CloseCase.init();
  $("#okBtn").on("click", Intake.Incident.CloseCase.ProcessCase);
  $("#cancelBtn").on("click", Intake.Incident.CloseCase.CloseCase);
});