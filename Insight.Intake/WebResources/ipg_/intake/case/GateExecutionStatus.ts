namespace Intake.Case {
    let now =  new Date();
    let startDateTime = new Date(
        now.getFullYear()
        , now.getMonth()
        , now.getDate()
        , now.getHours()
        , now.getSeconds() -5).toISOString().split('.')[0]+"Z";

    var GateRuning = true;
    const Xrm =  window.parent.Xrm;
    interface WFTask {id:string, name:string, status:string, order:number};
    const data:{ caseId:string,  InitiatingUser:string} = JSON.parse(getUrlParameter("data"));
    var WFTasks: WFTask[] = [];

    async function RetrieveWFTask(gateConfigId:string)
    {
        var gateConfig = await Xrm.WebApi.online.retrieveRecord("ipg_gateconfiguration", gateConfigId, "?$select=_ipg_documentsvalidationdetailid_value");      
        if(gateConfig["_ipg_documentsvalidationdetailid_value"])
        {
            var gatetask:WFTask = {
                id: gateConfig["_ipg_documentsvalidationdetailid_value"]
                ,name: gateConfig["_ipg_documentsvalidationdetailid_value@OData.Community.Display.V1.FormattedValue"]
                ,status: "Started"
                ,order: -1
            };

            WFTasks.push(gatetask);
        }
        
        var gateconfigDetails = await Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateconfigurationdetail",`?$select=ipg_executionorder,ipg_name&$orderby=ipg_executionorder&$filter=_ipg_gateconfigurationid_value eq ${gateConfigId} and statecode eq 0`);

        gateconfigDetails.entities.forEach(configdetails => {
            WFTasks.push({name:configdetails["ipg_name"], order:configdetails["ipg_executionorder"], id:configdetails["ipg_gateconfigurationdetailid"], status:"Started" });
        });
    }

    async function GetLastLogDate()
    {
      var lastlog = await Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateexecutionlog", `?$select=createdon&$filter=_ipg_caseid_value eq ${data.caseId}&$orderby=createdon desc&$top=1`);

      if(lastlog.entities && lastlog.entities.length > 0)
      {
        startDateTime = lastlog.entities[0]["createdon"];
      }
    }
    
    async function RetrieveGateConfigFromCase(caseId:string):Promise<string>
    {
        var result = await Xrm.WebApi.online.retrieveMultipleRecords("incident", `?$select=incidentid&$expand=ipg_lifecyclestepid($select=_ipg_gateconfigurationid_value)&$filter=incidentid eq ${caseId} and ipg_lifecyclestepid/ipg_lifecyclestepid ne null`);

        return result.entities[0]["ipg_lifecyclestepid"] 
        && result.entities[0]["ipg_lifecyclestepid"]["_ipg_gateconfigurationid_value"];
    }

    function BuildTable()
    {
        var $tbody = $('#wftasktable > tbody');

        for(var i = WFTasks.length - 1; i > -1; i--)
        {
            $tbody.append(`<tr ${i != 0 ? "style='display:none'" : ''} id=${WFTasks[i].id}><td>${WFTasks[i].name}</td><td>${WFTasks[i].status}</td></tr>`);
        }
    }

    async function RetrieveSessionId(caseId:string):Promise<string>
    {
        var incident = await Xrm.WebApi.online.retrieveRecord("incident",caseId, "?$select=ipg_cr_gatesessionid");

        return incident["ipg_cr_gatesessionid_value"];
    }

    function ProceedGating() {
        var parameters = {
          "Target": {
            "incidentid": data.caseId,
            "@odata.type": "Microsoft.Dynamics.CRM.incident"
          },
          "InitiatingUser": {
            "systemuserid": data.InitiatingUser,
            "@odata.type": "Microsoft.Dynamics.CRM.systemuser"
          },
          IsManual: true
        };
        const mainGatingCallback = (results) => {
          if (results.Output != null && results.Output != "") {
            var alertStrings = { confirmButtonLabel: "OK", text: results.Output };
            var alertOptions = { height: 150, width: 300 };
            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
              function success(result) {
              },
              function (error) {
                console.log(error.message);
              });
          } else {
            Xrm.WebApi.retrieveRecord("incident", data.caseId, "?$select=ipg_casestatus,_ipg_gateconfigurationid_value,ipg_statecode").then(
              function success(result) {
                if ((<string>result["_ipg_gateconfigurationid_value@OData.Community.Display.V1.FormattedValue"]).startsWith("Gate 11") && (result["ipg_casestatus"] != 923720001)) {
                  let alertStrings = { confirmButtonLabel: "Ok", text: `Case was promoted to ${result["ipg_statecode@OData.Community.Display.V1.FormattedValue"]}`, title: "Case state" };
                  Xrm.Navigation.openAlertDialog(alertStrings);
                }
              },
              function (error) {
                Xrm.Utility.alertDialog(error.message, null);
              }
            );
          }
        };
        callAction("ipg_IPGGatingStartGateProcessing", parameters, true,
          async function (results) {
              GateRuning = false;
              const warningSeverityLevel: number = 923720001;
              Xrm.Utility.closeProgressIndicator();
              if (results.AllowReject) {
                const confirmText = results.SeverityLevel === warningSeverityLevel
                  ? "There is at least one business rule warning flagged. Would you like to promote the Case anyway?"
                  : "Gating was completed with error. Do you want to continue with closing actions or cancel the process and do required actions";
                const confirmStr: Xrm.Navigation.ConfirmStrings = { title: "Approval required", text: confirmText };
                const confirmReslt = await Xrm.Navigation.openConfirmDialog(confirmStr, null);
                if (confirmReslt.confirmed) {
                  var parametersConfirm = {
                    "Target": {
                      "incidentid": data.caseId,
                      "@odata.type": "Microsoft.Dynamics.CRM.incident"
                    }
                  };
                  Xrm.Utility.showProgressIndicator("Processing...");
                  callAction("ipg_IPGGatingGateProcessingPostAction", parametersConfirm, true, (resultsConfirm) => {
                      mainGatingCallback(resultsConfirm);
                  });
                }
              } else {
                mainGatingCallback(results)
              }
          });
      }

      function callAction(actionName, parameters, async, successCallback) {
        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/" + actionName, async);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
          if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
              successCallback(JSON.parse(this.response));
            } else {
              Xrm.Utility.closeProgressIndicator();
              alert(JSON.parse(this.response).error.message);
            }
          }
        };
        req.send(JSON.stringify(parameters));
      }

      function getUrlParameter(sParam) {
        let sPageURL = window.location.search.substring(1),
          sURLVariables = sPageURL.split('&'),
          sParameterName,
          i;
    
        for (i = 0; i < sURLVariables.length; i++) {
          sParameterName = sURLVariables[i].split('=');
    
          if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? "" : decodeURIComponent(sParameterName[1]);
          }
        }
      };

      async function MonitorTasks()
      {
        if(GateRuning)
        {
            var task = WFTasks.find(r=>r.status == "Started");
            if(task)
            {
            var log = await Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateexecutionlog", `?$select=ipg_succeeded&$top=1&$filter=_ipg_gateconfigurationdetailid_value eq ${task.id} and _ipg_caseid_value eq ${data.caseId} and createdon gt ${startDateTime}`)
            if(log.entities.length == 0)
            {
              setTimeout(MonitorTasks, 500);
            }
            else
            {
                task.status = `Completed (${log.entities[0]["ipg_succeeded"] ? "S" : "E"})`;
                const row = $(`#${task.id}`);
                $(row.children()[1]).text(task.status);

                var nexttask = WFTasks.find(r=>r.status == "Started");
                if(nexttask)
                {
                    $(`#${nexttask.id}`).css("display","table-row");
                }

                MonitorTasks();
            }
        }
        }
        else{
            var logs = await Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateexecutionlog", `?$select=ipg_succeeded,_ipg_gateconfigurationdetailid_value&$filter=_ipg_caseid_value eq ${data.caseId} and createdon gt ${startDateTime}`);
            WFTasks.filter(r => r.status == "Started").map(r => {
                 let log = logs.entities.find(l => l["_ipg_gateconfigurationdetailid_value"] == r.id );
                 if(log)
                 {let row = $(`#${r.id}`);
                 r.status = `Completed (${log["ipg_succeeded"] ? "S" : "E"})`
                 $(row.children()[1]).text(r.status);
                 row.css("display","table-row");}
            })
        }
}
    export async function Onload()
    {
        var GateConfigId = await RetrieveGateConfigFromCase(data.caseId);
        await RetrieveWFTask(GateConfigId);
        await GetLastLogDate();
        BuildTable();
        setTimeout(MonitorTasks);
        setTimeout(ProceedGating);
    }
}

$(() => Intake.Case.Onload());
  