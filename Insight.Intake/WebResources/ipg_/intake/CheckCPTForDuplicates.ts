/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {

  /**
   * Called on CPT change
   * @function Intake.Utility.OnCPTChange
   * @returns {void}
  */
  export function OnCPTChange(executionContext) {
    var formContext = executionContext.getFormContext();
    var CPTName: string = executionContext.getEventSource().getName();
    var index: number = Number(CPTName.substr(CPTName.length - 1, 1));
    var CPTValue = formContext.getAttribute("ipg_cptcodeid" + index.toString()).getValue();
    formContext.getControl("ipg_cptcodeid" + index.toString()).clearNotification("ipg_cptcodeid" + index.toString() + "cpt");
    if (CPTValue) {
      for (var i = 1; i <= 6; i++) {
        var currentCPTValue = formContext.getAttribute("ipg_cptcodeid" + i.toString()).getValue();
        if ((index != i) && (currentCPTValue) && (currentCPTValue[0].id == CPTValue[0].id)) {
          formContext.getControl("ipg_cptcodeid" + index.toString()).setNotification("This CPT code has been already selected", "ipg_cptcodeid" + index.toString() + "cpt");
          formContext.getAttribute("ipg_cptcodeid" + index.toString()).setValue(null);
        }
        else {
          formContext.getControl("ipg_cptcodeid" + index.toString()).clearNotification();
        }

      }
    }
    SetCPTProcedureName(executionContext);
  }
  function SetCPTProcedureName(executionContext) {
   
    var procedureObject;
    var formContext = executionContext.getFormContext();
    if (localStorage.ProcdureId) {
      localStorage.removeItem("ProcdureName");
      localStorage.removeItem("ProcdureId");
    }
    procedureObject = formContext.getAttribute("ipg_cptcodeid1").getValue();
    if (procedureObject) {
      localStorage.ProcdureName = procedureObject[0].name;
      localStorage.ProcdureId = procedureObject[0].id;
    }


  }

}
