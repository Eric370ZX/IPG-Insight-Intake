/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  /**
   * Called on load form
   * @function Intake.Utility.OnLoad
   * @returns {void}
  */
  export function OnLoad(executionContext) {
    let formContext = executionContext.getFormContext();
    RemoveUnusedPlanTypes(formContext);
    SetEffectiveDate(formContext);
  }

  export function SetEffectiveDate(executionContext)
  {
    let formContext = executionContext._entityName ? executionContext : executionContext.getFormContext();
    let todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    let expirationDate = new Date("12/31/9999");

    let effectiveDateAttr = formContext.getAttribute("ipg_effectivedate");
    if (effectiveDateAttr && effectiveDateAttr.getValue() == null)
    {
      formContext.getAttribute("ipg_effectivedate").setValue(todayDate);
      formContext.getAttribute("ipg_expirationdate").setValue(expirationDate);
    }
  }

  /**
  * Removes unused plan types
  * @function Intake.Utility.RemoveUnusedPlanTypes
  * @returns {void}
  */
  function RemoveUnusedPlanTypes(formContext) {
    // field names in Case, Account and Health Plan Network entities
    let fields: string[] = ["ipg_primarycarrierplantype", "ipg_carriersupportedplantypes", "ipg_plantype", "ipg_excludedplantypemultiselect"];
    fields.forEach(function (fieldName) {
      var attribute = formContext.getAttribute(fieldName);
      if (attribute)
        RemovePlanTypes(formContext, attribute);
    });
  }

  /**
  * Gets disabled plan types
  * @function Intake.Utility.RemovePlanTypes
  * @returns {void}
  */
  function RemovePlanTypes(formContext, attribute) {
    var query = "?$select=ipg_disabledplantypes&$filter=ipg_name eq 'Disabled plan types'";
    Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", query)
      .then((result) => {
        if (result.entities.length > 0) {
          var disabledPlanTypes: string = result.entities[0].ipg_disabledplantypes;
          if (disabledPlanTypes) {
            var currentPlanType = attribute.getValue();
            var array = disabledPlanTypes.split(",");
            array.forEach(function (value) {
              if (currentPlanType != value)
                formContext.getControl(attribute._attributeName).removeOption(Number(value));
            });
          }
        }
      },
        function (error) {
          console.log(error.message);
        }
    );
  }
}
