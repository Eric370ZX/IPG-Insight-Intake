namespace Intake.Case
{
  export function GetProcedureName(executionContext)
  { 
    let formContext = executionContext.getFormContext();
    let procedureName = formContext.getAttribute("ipg_procedureid").getValue();

    if ((typeof localStorage.ProcdureId != 'undefined') && procedureName == null) {
      let object = new Array();
      object[0] = new Object();
      object[0].id = localStorage.ProcdureId;
      object[0].name = localStorage.ProcdureName;
      object[0].entityType = "ipg_procedurename";
      formContext.getAttribute("ipg_procedureid").setValue(object);
    }
    localStorage.removeItem("ProcdureName");
    localStorage.removeItem("ProcdureId");
  }
 

  
}


