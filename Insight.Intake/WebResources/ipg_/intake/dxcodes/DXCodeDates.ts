/**
 * @namespace Intake.DxCode
 */
namespace Intake.DxCode{

  export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() == 1) {
      SetDXCodeDates(formContext);
      ClearAndDisableDxCodeField(formContext, false);
    }
    else{
      ClearAndDisableDxCodeField(formContext, true);
    }
  }

  function SetDXCodeDates(formContext: Xrm.FormContext) {
    let effectiveDate = new Date();
    let expirationDate = new Date('12/31/9999');
    formContext.getAttribute("ipg_effectivedate")?.setValue(effectiveDate);
    formContext.getAttribute("ipg_expirationdate")?.setValue(expirationDate);
  }

  function ClearAndDisableDxCodeField(formContext: Xrm.FormContext, isDisabled: boolean){
    if (isDisabled){
      formContext.getControl("ipg_dxcode")?.setDisabled(isDisabled);
    }
    else {
    //  formContext.getAttribute("ipg_dxcode")?.setValue(null);
      formContext.getControl("ipg_dxcode")?.setDisabled(isDisabled);
    }
  }
} 
