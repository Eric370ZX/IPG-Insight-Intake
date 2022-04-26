/**
 * @namespace Intake.CasePartDetail.EditableGrid
 */
namespace Intake.CasePartDetail.EditableGrid {

  /**
   * Called on salect row in grid
   * @function Intake.CasePartDetail.EditableGrid.OnSelectRow
   * @returns {void}
  */
  export function OnSelectRow(executionContext: Xrm.Events.EventContext) {
    let entityObject = executionContext.getFormContext().data.entity;
    let potype = entityObject.attributes.get("ipg_potypecode");
  }

  export function OnChange(executionContext: Xrm.Events.EventContext) {
    let entityObject = executionContext.getFormContext().data.entity;
    let potype = entityObject.attributes.get("ipg_potypecode");
    let potypevalue = potype.getValue();
    var potypecontrol = potype.controls.getByIndex(0);
    if (potypevalue === 923720001) {
      let acctionCollection = {
        actions: null
      };
      acctionCollection.actions = [function () {
        potypecontrol.clearNotification("1");
      }];

      potypecontrol.addNotification({
        messages: ["Select another option!"],
        notificationLevel: "ERROR",
        uniqueId: "1",
        actions: [acctionCollection]
      });
      return false;
    }
    else {
      potypecontrol.clearNotification("1");
    }
  }

  export function OnSave(executionContext: Xrm.Events.EventContext) {

  }

  /**
  * set available PO Types by Surgery Date and DTM Member of Facility
  * @function Intake.CasePartDetail.SetAvailablePOTypes
  * @returns {void}
  */
  let defaultPOTypeOptions = null;
  function SetAvailablePOTypes(formContext: Xrm.FormContext) {
    let caseValue: Array<Xrm.LookupValue> = formContext.getAttribute("ipg_caseid").getValue();
    let productValue: Array<Xrm.LookupValue> = formContext.getAttribute("ipg_productid").getValue();
    let poTypepPickList = formContext.getControl("ipg_potypecode");
    if (poTypepPickList)
      defaultPOTypeOptions = poTypepPickList.getOptions();
    if (!productValue && poTypepPickList) {
      poTypepPickList.clearOptions();
      return;
    }
    if (poTypepPickList) {
      var currentPOType = Number(formContext.getAttribute("ipg_potypecode").getValue());
      if (currentPOType != 923720003)
        poTypepPickList.removeOption(923720003);
    }
    if (caseValue && caseValue.length && caseValue[0] && caseValue[0].id) {
      Xrm.WebApi.retrieveRecord(caseValue[0].entityType, caseValue[0].id, "?$select=ipg_surgerydate&$expand=ipg_FacilityId($select=ipg_dtmmember)")
        .then(
          (caseResult) => {
            Xrm.WebApi.retrieveRecord(productValue[0].entityType, productValue[0].id, "?$select=ipg_boxquantity")
              .then(
                (productResult) => {
                  let excludedOptions: Array<number> = null;
                  if (caseResult) {
                    let dateNow = new Date();
                    let currentDate = new Date(dateNow.getFullYear(), dateNow.getMonth(), dateNow.getDate());
                    if (caseResult.ipg_surgerydate && new Date(caseResult.ipg_surgerydate) < currentDate) {
                      if (productResult && productResult.ipg_boxquantity > 1) {
                        excludedOptions = [923720000, 923720001, 923720004];
                      }
                      else {
                        if (caseResult.ipg_FacilityId && caseResult.ipg_FacilityId.ipg_dtmmember)
                          excludedOptions = [923720000];
                        else
                          excludedOptions = [923720000, 923720001, 923720004];
                      }
                    }
                    else
                      excludedOptions = [923720001, 923720002, 923720004];

                    if (poTypepPickList && excludedOptions) {
                      excludedOptions.forEach((item) => {
                        poTypepPickList.removeOption(item);
                      });
                    }
                  }
                })
          });
    }
  }
}
