namespace Intake.Incident.HoldCase {
  interface HoldCaseResponse {
    Output: string,
    Succeeded: boolean
  }
  interface HoldCaseRequest {
    caseId: string,
    holdReason: number | undefined,
    holdNote: string,
    isOnHold: boolean
  }

  const Xrm = window.parent.Xrm;
  const holdReasons: Xrm.OptionSetValue[] = [];
  const inputData = JSON.parse(getUrlParameter("data"));

  export async function init() {
    Xrm.Utility.showProgressIndicator("Opening...");

    if (inputData.action == "unhold") {
      $("#lblHoldReason").hide();
      $("#holdReason").hide();
      $("#lblHeader").text("Remove hold");
    }
    else if(inputData.action == "changehold")
    {
      $("#lblHeader").text("Change hold Reason");
      $("#holdNote").hide();
      $("#holdNote").hide();
    }

    if (!Array.isArray(inputData.caseId)){
      const caseId = inputData.caseId as string;
      const caseTarget = await Xrm.WebApi.retrieveRecord("incident", caseId as string, `?$select=ipg_casehold,ipg_statecode`);

      if(inputData.action != "unhold")
      {
        caseTarget.ipg_statecode && await CrmManager.LoadOptionsByCaseStateCodeAsync(caseTarget.ipg_statecode, holdReasons);
        
        if(holdReasons.length == 0)
        {
          Xrm.Navigation.openErrorDialog({message:"Case can not be put on Hold because of current state!"}).then(()=>window.close());
        }

        holdReasons.forEach(p => $("#holdReason").prepend(`<option value="${p.value}">${p.text}</option>`));
            
        $("#holdReason option:first-child").prop("selected", "selected");
            
        if ($("#holdReason option").length == 0) {
          $("#okBtn").attr("disabled", "disabled");
        }
      }     
    }
    
    Xrm.Utility.closeProgressIndicator();
  }

  async function SendRequest(request: HoldCaseRequest, onSuccess: (data: HoldCaseResponse) => void, onError: (message: string) => void) {
    const target: { incidentid: string,"@odata.type":string  } = {
      incidentid: request.caseId,
      "@odata.type":"Microsoft.Dynamics.CRM.incident"
    };

    const parameterTypes =  {
      "Target": {
        "typeName": "mscrm.incident",
        "structuralProperty": 5
      },
      "IsOnHold": {
        "typeName": "Edm.Boolean",
        "structuralProperty": 1
      },
      "HoldNote": {
        "typeName": "Edm.String",
        "structuralProperty": 1
      },
      "HoldReason": {
        "typeName": "Edm.String",
        "structuralProperty": 1
      }

    };

    const ipg_IPGCaseActionsHoldCaseRequest = {
      Target: target,
      IsOnHold: request.isOnHold
    };
  
    if(request.holdReason)
    {
      ipg_IPGCaseActionsHoldCaseRequest["HoldReason"] = request.holdReason;
    }
    
    if(request.holdNote)
    {
      ipg_IPGCaseActionsHoldCaseRequest["HoldNote"] = request.holdNote;
    }

    ipg_IPGCaseActionsHoldCaseRequest["getMetadata"] = function () {
      return {
        boundParameter: null,
        parameterTypes: parameterTypes,
        operationType: 0,
        operationName: "ipg_IPGCaseActionsHoldCase"
      };
    }

    try {
      const result = await Xrm.WebApi.online.execute(ipg_IPGCaseActionsHoldCaseRequest);
      
      if (result.ok) 
      {
        onSuccess(await result.json());
      }
    }
    catch (error) {
      onError(error.message);
    }
  }

  export async function HoldCase() {
    const requestData = inputData;
    const isHold = requestData.action as string === "hold" || requestData.action == "changehold";
    const holdReason = $("#holdReason").val();
    const holdNotes = $("#holdNote").val();
    
    if ((isHold && !holdReason) || requestData.action != "changehold" && !holdNotes) {
      Xrm.Navigation.openErrorDialog({message:"You need to populate mandatory fields"});
      return;
    }

    Xrm.Utility.showProgressIndicator("Processing...");

    const ProcessCase = async (caseId: string) => {
      const request: HoldCaseRequest = {
        caseId: caseId,
        holdNote: holdNotes as string,
        holdReason: holdReason as number,
        isOnHold: isHold
      };

      const successCallback = (data: HoldCaseResponse) => {
        Xrm.Utility.closeProgressIndicator();
        const storageKey = `holdCase-${request.caseId}`;
        localStorage.setItem(storageKey, "true");
        if(data.Succeeded)
        {
          Xrm.Navigation.openAlertDialog({text:"Request was processed successfully"}).then(() => {
            window.parent.close()
          });
        }
        else
        {
          Xrm.Navigation.openErrorDialog({message:`Error while puting case on hold: ${data.Output}`}).then(() => {
            window.parent.close();
          });
        }
      }

      await SendRequest(request, successCallback,
        (message) => {
          Xrm.Utility.closeProgressIndicator();
          Xrm.Navigation.openErrorDialog({message: message}).always;
        });
    };

    if (Array.isArray(requestData.caseId)) {
      const caseIds = requestData.caseId as Array<string>;

      for (var i = 0; i < caseIds.length; i++) {
        await ProcessCase(caseIds[i]);
      }

      localStorage.setItem('RefreshHoldCaseRibbon', "true");
    }
    else {
      await ProcessCase(requestData.caseId as string);
    }
  }
  export function Cancel() {
    window.parent.close();
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
  namespace CrmManager {
    export async function LoadOptionsByCaseStateCodeAsync(casestatecode:string, targetList: Xrm.OptionSetValue[]): Promise<void> {
        const data  = await Xrm.WebApi.retrieveMultipleRecords('ipg_caseholdconfiguration'
        ,`?$select=ipg_caseholdreason&$filter=ipg_casestate eq ${casestatecode} and statecode eq 0`);
          
        for (let i = 0; i < data.entities.length; i++) {
            targetList.push({
              text: data.entities[i]['ipg_caseholdreason@OData.Community.Display.V1.FormattedValue'],
              value: data.entities[i]['ipg_caseholdreason']
            });
          };
    }
}
}

$(() => {
  Intake.Incident.HoldCase.init();
  $("#okBtn").on('click', Intake.Incident.HoldCase.HoldCase);
  $("#cancelBtn").on('click', Intake.Incident.HoldCase.Cancel);
});
