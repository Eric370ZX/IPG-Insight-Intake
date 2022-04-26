/**
 * @namespace Intake.ChargeCenter
 */
namespace Intake.ChargeCenter {
  /**
    * Called on FormLoad event
    * @function Intake.ChargeCenter.OnFormLoad
    * @returns {void}
  */
  export function OnFormLoad(executionContext: Xrm.Events.SaveEventContext) {
    disableOrEnableKeywordExceptionsField(executionContext);
  }

  /**
    * Called on Supported field Change event
    * @function Intake.ChargeCenter.OnSupportedChange
    * @returns {void}
  */
  export function OnSupportedChange(executionContext: Xrm.Events.SaveEventContext) {
    disableOrEnableKeywordExceptionsField(executionContext);
  }

  /**
    * Called on Form Save event
    * @function Intake.ChargeCenter.OnSaveForm
    * @returns {void}
  */
  export function OnSaveForm(executionContext: Xrm.Events.SaveEventContext) {
    validateRequiredFields(executionContext);
  }


  /**
    * Enables or disables KeywordExceptions field based on the value of Supported field
    * @returns {void}
  */
  function disableOrEnableKeywordExceptionsField(executionContext: Xrm.Events.SaveEventContext) {

    //I had to use this script instead of a business rule because on 4/5/2019 D365 does not support multi-select option set fields in business rules

    let formContext = executionContext.getFormContext();
    let isSupported: boolean = formContext.getAttribute("ipg_supported").getValue();
    if (isSupported) {
      formContext.getControl("ipg_keywordexceptions").setDisabled(true);
      formContext.getAttribute("ipg_keywordexceptions").setValue(null);
    }
    else {
      formContext.getControl("ipg_keywordexceptions").setDisabled(false);
    }
  }

  /**
    * At least one of these fields is required: Part Category, Part Name or HCPCS Code
    * @returns {boolean}
  */
  function validateRequiredFields(executionContext: Xrm.Events.SaveEventContext): boolean {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    let partCategoryAttributeValue: Xrm.LookupValue[] = formContext.getAttribute("ipg_partcategoryid").getValue();
    let partAttributeValue: Xrm.LookupValue[] = formContext.getAttribute("ipg_productid").getValue();
    let hcpcsCodeAttributeValue: Xrm.LookupValue[] = formContext.getAttribute("ipg_hcpcscodeid").getValue();

    const notificationId: string = 'requiredFieldValidation';
    if (!partCategoryAttributeValue && !partAttributeValue && !hcpcsCodeAttributeValue) {
      formContext.ui.setFormNotification('At least one of these fields is required: Part Category, Part Name or HCPCS Code', 'ERROR', notificationId);
      let saveEventArgs: Xrm.Events.SaveEventArguments = executionContext.getEventArgs();
      saveEventArgs.preventDefault();

      return false;
    }

    formContext.ui.clearFormNotification(notificationId);
    
    return true;
  }
}
