/**
 * @namespace Intake.EstimatedPart
 */
 namespace Intake.EstimatedPart {

  /**
   * Called on load form
   * @function Intake.EstimatedPart.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
   
    OnChangeQuantityImplanted(formContext);    
  }

  /**
     * OnChange handler. 
     * @function Intake.EstimatedPart.OnChangeHandler
     * @returns {void}
     */
   function OnChangeQuantityImplanted(formContext: Xrm.FormContext){
    const quantityImplantedAttribute = formContext.getAttribute("ipg_quantity"); 
    quantityImplantedAttribute.addOnChange(()=> {checkMaxValueOnPart(formContext)});   
    quantityImplantedAttribute.fireOnChange();      
  }
 
  /**
    * Called on change Part Override
    * @function Intake.EstimatedPart.OnChangePartOverride
    * @returns {void}
  */
  export function OnChangePartOverride(executionContext) {
    let formContext = executionContext.getFormContext();
    IsPartUsed(formContext);
  }

  /**
  * Shows warning if a part is already used in a case
  * @function Intake.EstimatedPart.IsPartUsed
  * @returns {void}
  */
  function IsPartUsed(formContext) {
    let product = formContext.getAttribute("ipg_productid").getValue();
    let incident = formContext.getAttribute("ipg_caseid").getValue();
    formContext.getControl('ipg_productid').clearNotification(IsPartUsed.name);
    if (product && incident) {
      Xrm.WebApi.retrieveMultipleRecords("ipg_estimatedcasepartdetail", "?$select=ipg_estimatedcasepartdetailid&$filter=_ipg_caseid_value eq " + Intake.Utility.removeCurlyBraces(incident[0].id) + " and  _ipg_productid_value eq " + Intake.Utility.removeCurlyBraces(product[0].id) + " and statecode eq 0").then(
        function success(results) {
          if (results.entities.length) {
            formContext.getControl("ipg_productid").setNotification("The part is already used in this case", IsPartUsed.name);
          }
        },
        function (error) {
          Xrm.Navigation.openAlertDialog({ text: error.message });
        }
      );
    }
  }

  /**
    * Check max quantity restriction of product
    * @function Intake.EstimatedPart.checkMaxValueOnPart
    * @returns {void}
  */
   function checkMaxValueOnPart(formContext) {
    let partLookupAttributeValue = formContext.getAttribute("ipg_productid").getValue();
    let quantityAttributeValue = formContext.getAttribute("ipg_quantity").getValue();
        
    if (partLookupAttributeValue) {
      Xrm.WebApi.retrieveRecord(partLookupAttributeValue[0].entityType, partLookupAttributeValue[0].id, "?$select=ipg_enforcemaxquantity,ipg_maxquantity").then(function (product) {        
        if (product.ipg_maxquantity && quantityAttributeValue  > product.ipg_maxquantity) {   
          let alertStrings = { 
            confirmButtonLabel: "Ok",
            text: "Quantity Implanted is exceeding the Max Quantity Per Case value. Max value is: " + product.ipg_maxquantity,
            title: "Warning" };
          let alertOptions = { height: 120, width: 260 };         
          Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);  
          if (product.ipg_enforcemaxquantity) {                
            formContext.getAttribute('ipg_quantity').setValue(0);  
          }                 
        }        
      }, function (error) {
        Xrm.Navigation.openErrorDialog({ message: error.message });
      });
    }
  }

}