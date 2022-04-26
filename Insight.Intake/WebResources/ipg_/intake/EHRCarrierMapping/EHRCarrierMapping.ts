/**
* @namespace Intake.EHRCarrierMapping
*/
namespace Intake.EHRCarrierMapping {
   /**
    * Called on load form
    * @function Intake.EHRCarrierMapping.OnLoadForm
    * @returns {void}
   */
    export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
        let formContext: Xrm.FormContext = executionContext.getFormContext();
        if (formContext.ui.getFormType() == 1) { //create form type
            setDisabledFields(formContext, false);
        }

        let effectiveDateAttr = formContext.getAttribute("ipg_effectivedate");
        effectiveDateAttr.addOnChange(() => {
            compareEffectiveAndExpirationDates(formContext);
          });
        let expirationDateAttr = formContext.getAttribute("ipg_expirationdate");
        expirationDateAttr.addOnChange(() => {
            compareEffectiveAndExpirationDates(formContext);
          });
    }

    export function OnSave(executionContext: Xrm.Events.EventContext){
        let formContext: Xrm.FormContext = executionContext.getFormContext();
        setDisabledFields(formContext, true);
    }

    function setDisabledFields(formContext:Xrm.FormContext, isDisabled: boolean) {

        const fieldNames = ["ipg_name", "ipg_carrierposition", "ipg_facilityid"];

        fieldNames.forEach((fieldName: string) => {
            formContext.getControl(fieldName)?.setDisabled(isDisabled);
        });
    }

    function compareEffectiveAndExpirationDates(formContext: Xrm.FormContext){
        let effectiveDate = formContext.getAttribute("ipg_effectivedate")?.getValue();
        let expirationDate = formContext.getAttribute("ipg_expirationdate")?.getValue();
        if (effectiveDate && expirationDate && effectiveDate > expirationDate){
            formContext.getControl("ipg_effectivedate")?.setNotification("Effective Date should be earlier than Expiration Date.", "date");
        }
        else {
            formContext.getControl("ipg_effectivedate")?.clearNotification("date");
        }
    }
}

