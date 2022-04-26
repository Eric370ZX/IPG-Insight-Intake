/**
 * @namespace Intake.FacilityCarrierCPTRule
 */
namespace Intake.FacilityCarrierCPTRule {

  /**
    * Called on Form Save event
    * @function Intake.FacilityCarrierCPTRule.OnSaveForm
    * @returns {void}
  */
  export function OnSaveForm(executionContext) {
    let formContext = executionContext.getFormContext();
    updateName(formContext);
    if (CheckConflictingRule(formContext) || CheckDateRange(formContext)) {
      let saveEventArgs: Xrm.Events.SaveEventArguments = executionContext.getEventArgs();
      saveEventArgs.preventDefault();
    }
  }

  function updateName(formContext: any) {
    if (formContext.getAttribute("ipg_cptid").getValue() != null) {
      let cptCodeAttrbValue: any = formContext.getAttribute("ipg_cptid").getValue();
      if (cptCodeAttrbValue) {
        let cptCodeId: string = cptCodeAttrbValue[0].id;
        if (cptCodeId) cptCodeId = cptCodeId.slice(1, -1);
        if (cptCodeId != null) {
          Xrm.WebApi.retrieveRecord("ipg_cptcode", cptCodeId, "?$select=ipg_cptcode").then(
            function success(result) {
              if (result.ipg_cptcode) {
                formContext.getAttribute("ipg_name").setValue(result.ipg_cptcode);
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

  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();
    const FacilityCarrierCPTRuleContractId: string = 'ipg_facilitycarriercptrulecontractid';

    if (formContext.ui.getFormType() == 1) {//Create
      let facilityCarrierCPTRuleContractAttrbValue: any = formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue();
      if (facilityCarrierCPTRuleContractAttrbValue) {
        let facilityCarrierCPTContract: string = facilityCarrierCPTRuleContractAttrbValue[0].id;
        if (facilityCarrierCPTContract) facilityCarrierCPTContract = facilityCarrierCPTContract.slice(1, -1);
        if (facilityCarrierCPTContract != null) {
          Xrm.WebApi.retrieveRecord("ipg_facilitycarriercptrulecontract", facilityCarrierCPTContract, "?$select=ipg_effectivedate,ipg_expirationdate").then(
            function success(result) {
              if (result.ipg_effectivedate) {
                formContext.getAttribute("ipg_effectivedate").setValue(new Date(result.ipg_effectivedate));
              }
              if (result.ipg_expirationdate) {
                formContext.getAttribute("ipg_expirationdate").setValue(new Date(result.ipg_expirationdate));
              }

              if (result.ipg_expirationdate <= result.ipg_effectivedate) {
                formContext.getControl("ipg_expirationdate").setNotification(`Effective Date cannot be equal to greater than Expiration Date`);
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

  /**
    * Called on change Effective Date
    * @function Intake.FacilityCarrierCPTRule.OnChangeEffectiveDate
    * @returns {void}
  */
  export function OnChangeEffectiveDate(executionContext) {
    let formContext = executionContext.getFormContext();
    CheckConflictingRule(formContext);
    CheckDateRange(formContext);
  }

  /**
  * Called on change Expiration Date
  * @function Intake.FacilityCarrierCPTRule.OnChangeExpirationDate
  * @returns {void}
*/
  export function OnChangeExpirationDate(executionContext) {
    let formContext = executionContext.getFormContext();
    CheckConflictingRule(formContext);
    CheckDateRange(formContext);
  }

  /**
* Called on change Expiration Date
* @function Intake.FacilityCarrierCPTRule.OnChangeFacilityCarrierCPTRuleContract
* @returns {void}
*/
  export function OnChangeFacilityCarrierCPTRuleContract(executionContext) {
    let formContext = executionContext.getFormContext();
    CheckConflictingRule(formContext);
    CheckDateRange(formContext);
  }

  /**
* Called on change type
* @function Intake.FacilityCarrierCPTRule.OnChangeCPTCode
* @returns {void}
*/
  export function OnChangeCPTCode(executionContext) {
    let formContext = executionContext.getFormContext();
    updateName(formContext);
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
  * @function Intake.FacilityCarrierCPTRule.CheckConflictingRule
  * @returns {void}
  */
  function CheckConflictingRule(formContext): boolean {
    const EffectiveDateField: string = 'ipg_effectivedate';
    const ExpirationDateField: string = 'ipg_expirationdate';
    const FacilityCarrierCPTRuleContractId: string = 'ipg_facilitycarriercptrulecontractid';
    const CPTCodeId: string = 'ipg_cptid';
    
    if (formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue() == null || formContext.getAttribute(EffectiveDateField).getValue() == null || formContext.getAttribute(CPTCodeId).getValue() == null) return;

    let cptId: string = formContext.getAttribute(CPTCodeId).getValue()[0].id;
    if (cptId) cptId = cptId.slice(1, -1);

    let facilityCarrierCPTContract: string = formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue()[0].id;
    if (facilityCarrierCPTContract) facilityCarrierCPTContract = facilityCarrierCPTContract.slice(1, -1);

    let effectiveDate: Date = formContext.getAttribute(EffectiveDateField).getValue();
    var effectiveDateFormat = getODataUTCDateFilter(effectiveDate);

    let expirationDate: Date = formContext.getAttribute(ExpirationDateField).getValue();
    if (expirationDate == null) expirationDate = new Date(9998, 12, 31);
    var expirationDateFormat = getODataUTCDateFilter(expirationDate);

    let query = `?$select=ipg_name&$filter=_ipg_facilitycarriercptrulecontractid_value eq ${facilityCarrierCPTContract}
                 and _ipg_cptid_value eq ${cptId}
                 and ${ExpirationDateField} ge ${effectiveDateFormat}
                 and ${EffectiveDateField} le ${expirationDateFormat}`;

    let facilityCPTRuleId: string = Xrm.Page.data.entity.getId();
    if (facilityCPTRuleId) {
      facilityCPTRuleId = facilityCPTRuleId.slice(1, -1);
      query += ` and ipg_facilitycarriercptruleid ne ${facilityCPTRuleId}`;
    }

    Xrm.WebApi.retrieveMultipleRecords('ipg_facilitycarriercptrule', query).then(function (result) {
      if (result.entities.length) {
        if (result.entities.length >= 1) {
          formContext.getControl(CPTCodeId).setNotification(`There is a conflict with record ${result.entities[0].ipg_name}.`, CPTCodeId);
          return true;
        }
      }
      formContext.getControl(CPTCodeId).clearNotification();
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });
    return false;
  }

  /**
  * Shows warning if rule date range is outside Facility Carrier CPT Contract's date range.
  * @function Intake.FacilityCarrierCPTRule.CheckDateRange
  * @returns {void}
  */
  function CheckDateRange(formContext): boolean {
    const EffectiveDateField: string = 'ipg_effectivedate';
    const ExpirationDateField: string = 'ipg_expirationdate';
    const FacilityCarrierCPTRuleContractId: string = 'ipg_facilitycarriercptrulecontractid';
    if (formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue() == null ||
      formContext.getAttribute(EffectiveDateField).getValue() == null ||
      formContext.getAttribute(ExpirationDateField).getValue() == null) return;

    let facilityCarrierCPTRuleContract: string = formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue()[0].id;
    if (facilityCarrierCPTRuleContract) facilityCarrierCPTRuleContract = facilityCarrierCPTRuleContract.slice(1, -1);

    let effectiveDate: Date = formContext.getAttribute(EffectiveDateField).getValue();

    let expirationDate: Date = formContext.getAttribute(ExpirationDateField).getValue();
    if (expirationDate == null) expirationDate = new Date(9998, 12, 31);

    if (expirationDate <= effectiveDate) {
      formContext.getControl("ipg_expirationdate").setNotification(`Effective Date cannot be equal to greater than Expiration Date`);
    }

    Xrm.WebApi.retrieveRecord('ipg_facilitycarriercptrulecontract', facilityCarrierCPTRuleContract, "?$select=ipg_effectivedate,ipg_expirationdate").then(function (result) {
      let startDate = new Date(result.startdate);
      startDate.setHours(0, 0, 0, 0);
      let endDate = new Date(result.enddate);
      endDate.setHours(0, 0, 0, 0);
      if (startDate > effectiveDate) {
        formContext.getControl(EffectiveDateField).setNotification("Effective date is before Contracts's start date.", EffectiveDateField);
        return true;
      }
      else if (endDate < expirationDate) {
        formContext.getControl(ExpirationDateField).setNotification("Expiration date is after Contracts's end date.", ExpirationDateField);
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
