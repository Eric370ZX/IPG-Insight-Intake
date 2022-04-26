/**
 * @namespace Intake.Incident
 */
 namespace Intake.Incident.UnlockCase {
    let Xrm = window.Xrm || window.top.Xrm;
    let incidentId: string;
  
    export async function InitUnlockCasePage() {
      incidentId = (new URLSearchParams(window.location.search)).get("data");
      if (!incidentId) {
        Xrm.Navigation.openErrorDialog({ message: 'Error. Incident ID parameter is required' });
        return null;
      }
    }
  
    export async function UnlockCase() {
        var reasonSelectEl= document.getElementById("reason") as HTMLSelectElement;
        var notesEl = document.getElementById("notes") as HTMLTextAreaElement;

        if (!reasonSelectEl.value || reasonSelectEl.value == "0" || reasonSelectEl.innerText == "") {
            Xrm.Navigation.openAlertDialog({text:'Please select a Reason'});
            return;
        }

        const target: { entityType:string,  id: string } = {
            entityType: "incident",
            id: incidentId
          };    
          const parameters: { Target: any, Reason: string, Notes?: string} = {
            Target: target,
            Reason: reasonSelectEl.innerText,
            Notes:notesEl.innerText
          }; 
      
          var ipg_IPGCaseActionsUnlockCase = {
            entity: parameters.Target,
            Reason: parameters.Reason,
            Notes: parameters.Notes,
            getMetadata: function () {
              return {
                boundParameter: "entity",
                parameterTypes: {
                  entity: {
                    "typeName": "mscrm.incident",
                    "structuralProperty": 5
                  },
                  "Reason": {
                    "typeName": "Edm.String",
                    "structuralProperty": 1
                  },
                  "Notes": {
                    "typeName": "Edm.String",
                    "structuralProperty": 1
                  }
                },
                operationType: 0,
                operationName: "ipg_IPGCaseActionsUnlockCase"
              };
            }
        };
          
        Xrm.Utility.showProgressIndicator(null);
        try
        {
            await Xrm.WebApi.online.execute(ipg_IPGCaseActionsUnlockCase);
            
            Xrm.Utility.closeProgressIndicator();
            window.close();
        }
        catch
        {
            Xrm.Utility.closeProgressIndicator();
            Xrm.Navigation.openAlertDialog({text:'There is error. If repeat contact System Administrator.'});
        }
    }
}
  
  document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("confirmBtn").addEventListener('click',Intake.Incident.UnlockCase.UnlockCase, false);
    document.getElementById("cancelBtn").addEventListener('click', () => window.close(), false);
    Intake.Incident.UnlockCase.InitUnlockCasePage();
  });
  