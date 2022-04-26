namespace Intake.Case {

  /**
 * Called on load form
 * @function Intake.Case.HighlightBilledCPT
 * @returns {void}
*/
  export function HighlightBilledCPT(executionContext: Xrm.Events.EventContext) {
    let formContext = executionContext.getFormContext();
    //let cptCodeId1Control = formContext.getControl("ipg_cptcodeid1");
    //cptCodeId1Control.setLabel("Billed CPT");
    let incidentId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());

    Xrm.WebApi.retrieveRecord("incident", incidentId, "?$select=_ipg_cptcodeid1_value,_ipg_cptcodeid2_value,_ipg_cptcodeid3_value,_ipg_cptcodeid4_value,_ipg_cptcodeid5_value,_ipg_cptcodeid6_value,_ipg_billedcptid_value").then(function (incident) {
      console.log(incident);
      if (incident._ipg_billedcptid_value) {
        if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid1_value) {
          let control = formContext.getControl("ipg_cptcodeid1");
          control.setLabel("Billed CPT");
        }
        else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid2_value) {
          let control = formContext.getControl("ipg_cptcodeid2");
          control.setLabel("Billed CPT");
        }
        else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid3_value) {
          let control = formContext.getControl("ipg_cptcodeid3");
          control.setLabel("Billed CPT");
        }
        else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid4_value) {
          let control = formContext.getControl("ipg_cptcodeid4");
          control.setLabel("Billed CPT");
        }
        else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid5_value) {
          let control = formContext.getControl("ipg_cptcodeid5");
          control.setLabel("Billed CPT");
        }
        else if (incident._ipg_billedcptid_value == incident._ipg_cptcodeid6_value) {
          let control = formContext.getControl("ipg_cptcodeid6");
          control.setLabel("Billed CPT");
        }
      }
    }, function (error) {
      
    });
  }
}
