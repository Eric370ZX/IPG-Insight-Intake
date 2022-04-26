/**
 * @namespace Intake.FacilityCarrierCPTRuleContract
 */
namespace Intake.FacilityCarrierCPTRuleContract {
  const todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
  const  effectiveDate = todayDate;
  /**
    * Called on Form Save event
    * @function Intake.FacilityCarrierCPTRuleContract.OnSaveForm
    * @returns {void}
  */
  export function OnSaveForm(executionContext) {
    let formContext = executionContext.getFormContext();
    if (CheckConflictingRule(formContext) || CheckDateRange(formContext)) {
      let saveEventArgs: Xrm.Events.SaveEventArguments = executionContext.getEventArgs();
      saveEventArgs.preventDefault();
    }
    else {
      updateName(formContext);
    }
  }

  function updateName(formContext: any) {
    if (formContext.getAttribute("ipg_entitlementid").getValue() != null &&
        formContext.getAttribute("ipg_effectivedate").getValue() != null &&
        formContext.getAttribute("ipg_expirationdate").getValue() != null) {
      var entitlementName = formContext.getAttribute("ipg_entitlementid").getValue()[0].name;
      var effectiveDate = formContext.getAttribute("ipg_effectivedate").getValue();
      var expirationDate = formContext.getAttribute("ipg_expirationdate").getValue();
      formContext.getAttribute("ipg_name").setValue(entitlementName + " - " + getODataUTCDateFilter(effectiveDate) + " - " + getODataUTCDateFilter(expirationDate));
    }
  }

  /**
  * Called on Form Load event
  * @function Intake.FacilityCarrierCPTRuleContract.OnLoadForm
  * @returns {void}
  */
  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();
    const EntitlementId: string = 'ipg_entitlementid';  

    if (formContext.ui.getFormType() == 1) {//Create
      let entitlementAttrbValue: any = formContext.getAttribute(EntitlementId).getValue();
      if (entitlementAttrbValue) {
        let entitlementId: string = entitlementAttrbValue[0].id;
        if (entitlementId) entitlementId = entitlementId.slice(1, -1);
        if (entitlementId != null) {
          Xrm.WebApi.retrieveRecord("entitlement", entitlementId, "?$select=startdate,enddate").then(
            function success(result) {
              if (result.startdate) {
                formContext.getAttribute("ipg_effectivedate").setValue(new Date(result.startdate));
              }
              if (result.enddate) {
                formContext.getAttribute("ipg_expirationdate").setValue(new Date(result.enddate));
              }
            },
            function (error) {
              console.log(error.message);
              // handle error conditions
            }
          );
        }
      }
    }
  }


  export function updateDate(executionContext) {
   
    let formContext = executionContext.getFormContext();
    if (formContext.getAttribute("ipg_effectivedate").getValue() != null) {
      let expirationDate = formContext.getAttribute("ipg_expirationdate").getValue();
      let localEffectiveDate = formContext.getAttribute("ipg_effectivedate").getValue();
      if (localEffectiveDate === null)
      {
        localEffectiveDate = effectiveDate;
      }
      if (localEffectiveDate => expirationDate) {
        formContext.getControl("ipg_expirationdate").setNotification(`Effective Date cannot be equal to greater than Expiration Date`);
      }
    }

  }

  /**
    * Called on change Effective Date
    * @function Intake.FacilityCarrierCPTRuleContract.OnChangeEffectiveDate
    * @returns {void}
  */
  export function OnChangeEffectiveDate(executionContext) {
    let formContext = executionContext.getFormContext();
    CheckConflictingRule(formContext);
    CheckDateRange(formContext);
  }

  /**
  * Called on change Expiration Date
  * @function Intake.FacilityCarrierCPTRuleContract.OnChangeExpirationDate
  * @returns {void}
*/
  export function OnChangeExpirationDate(executionContext) {
    let formContext = executionContext.getFormContext();
    CheckConflictingRule(formContext);
    CheckDateRange(formContext);
  }

  /**
* Called on change Expiration Date
* @function Intake.FacilityCarrierCPTRuleContract.OnChangeEntitlement
* @returns {void}
*/
  export function OnChangeEntitlement(executionContext) {
    let formContext = executionContext.getFormContext();
    CheckConflictingRule(formContext);
    CheckDateRange(formContext);
  }

  function getODataUTCDateFilter(date) {
    var monthString;
    var rawMonth = (date.getUTCMonth() + 1).toString();
    if (rawMonth.length == 1) {
      monthString = "0" + rawMonth;
    }
    else { monthString = rawMonth; }

    var dateString;
    var rawDate = date.getUTCDate().toString();
    if (rawDate.length == 1) {
      dateString = "0" + rawDate;
    }
    else { dateString = rawDate; }

    var DateFilter = "";
    DateFilter += date.getUTCFullYear() + "-";
    DateFilter += monthString + "-";
    DateFilter += dateString;
    return DateFilter;
  }

  /**
  * Shows warning if conflicting rule exists.
  * @function Intake.FacilityCarrierCPTRuleContract.CheckConflictingRule
  * @returns {void}
  */
  function CheckConflictingRule(formContext): boolean {
    const EffectiveDateField: string = 'ipg_effectivedate';
    const ExpirationDateField: string = 'ipg_expirationdate';
    const EntitlementId: string = 'ipg_entitlementid';

    if (formContext.getAttribute(EntitlementId).getValue() == null || formContext.getAttribute(EffectiveDateField).getValue() == null) return;

    let entitlementId: string = formContext.getAttribute(EntitlementId).getValue()[0].id;
    if (entitlementId) entitlementId = entitlementId.slice(1, -1);

    let effectiveDate: Date = formContext.getAttribute(EffectiveDateField).getValue();
    var effectiveDateFormat = getODataUTCDateFilter(effectiveDate);

    let expirationDate: Date = formContext.getAttribute(ExpirationDateField).getValue();
    if (expirationDate == null) expirationDate = new Date(9998, 12, 31);
    var expirationDateFormat = getODataUTCDateFilter(expirationDate);

    let query = `?$select=ipg_name&$filter=_ipg_entitlementid_value eq ${entitlementId}
                 and ${ExpirationDateField} ge ${effectiveDateFormat}
                 and ${EffectiveDateField} le ${expirationDateFormat}`;

    let facilityCPTRuleContractId: string = Xrm.Page.data.entity.getId();
    if (facilityCPTRuleContractId) {
      facilityCPTRuleContractId = facilityCPTRuleContractId.slice(1, -1);
      query += ` and ipg_facilitycarriercptrulecontractid ne ${facilityCPTRuleContractId}`;
    }

    Xrm.WebApi.retrieveMultipleRecords('ipg_facilitycarriercptrulecontract', query).then(function (result) {
      if (result.entities.length) {
        if (result.entities.length >= 1) {
          formContext.getControl(EntitlementId).setNotification(`There is a conflict with record ${result.entities[0].ipg_name}.`, EntitlementId);
          return true;
        }
      }
      formContext.getControl(EntitlementId).clearNotification();
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });
    return false;
  }

  /**
  * Shows warning if rule date range is outside entitlement's date range.
  * @function Intake.FacilityCarrierCPTRuleContract.CheckDateRange
  * @returns {void}
  */
  function CheckDateRange(formContext): boolean {
    const EffectiveDateField: string = 'ipg_effectivedate';
    const ExpirationDateField: string = 'ipg_expirationdate';
    const EntitlementId: string = 'ipg_entitlementid';
    if (formContext.getAttribute(EntitlementId).getValue() == null ||
      formContext.getAttribute(EffectiveDateField).getValue() == null ||
      formContext.getAttribute(ExpirationDateField).getValue() == null) return;

    let entitlementId: string = formContext.getAttribute(EntitlementId).getValue()[0].id;
    if (entitlementId) entitlementId = entitlementId.slice(1, -1);

    let effectiveDate: Date = formContext.getAttribute(EffectiveDateField).getValue();

    let expirationDate: Date = formContext.getAttribute(ExpirationDateField).getValue();
    if (expirationDate == null) expirationDate = new Date(9998, 12, 31);

    Xrm.WebApi.retrieveRecord('entitlement', entitlementId, "?$select=startdate,enddate").then(function (result) {
      let startDate = new Date(result.startdate);
      startDate.setHours(0, 0, 0, 0);
      let endDate = new Date(result.enddate);
      endDate.setHours(0, 0, 0, 0);
      if (startDate > effectiveDate) {
        formContext.getControl(EffectiveDateField).setNotification("Effective date is before Entitlement's start date.", EffectiveDateField);
        return true;
      }
      else if (endDate < expirationDate) {
        formContext.getControl(ExpirationDateField).setNotification("Expiration date is after Entitlement's end date.", ExpirationDateField);
        return true;
      }
      else {
        formContext.getControl(EffectiveDateField).clearNotification(EffectiveDateField);
        formContext.getControl(ExpirationDateField).clearNotification(ExpirationDateField);
        return false;
      }
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });
    return false;
  }

}
