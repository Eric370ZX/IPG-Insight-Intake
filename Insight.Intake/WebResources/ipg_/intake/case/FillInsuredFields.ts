/**
 * @namespace Intake.Case
 * 
 */
namespace Intake.Case {

  /**
  * Called when a user changes field "Relation to Insured"
  * @function Intake.Case.RelationToInsuredOnChange
  * @returns {void}
  */
  export function RelationToInsuredOnChange(executionContext) {
    FillInusredFields(executionContext);
    SetFieldsRequiredLevel(executionContext);
  }

  /**
    * Fills insured fields
    * @function Intake.Case.GeneratePartNumber
    * @returns {void}
    */
  function FillInusredFields(executionContext) {
    var formContext = executionContext.getFormContext();
    var relationToInsured = formContext.getAttribute("ipg_relationtoinsured").getValue();
    interface Fields {
      patientField: string;
      insuredField: string;
    }

    var fieldsArray: Fields[] = [
      { patientField: 'ipg_patientlastname', insuredField: 'ipg_insuredlastname' },
      { patientField: 'ipg_patientfirstname', insuredField: 'ipg_insuredfirstname' },
      { patientField: 'ipg_patientmiddlename', insuredField: 'ipg_insuredmiddlename' },
      { patientField: 'ipg_patientdateofbirth', insuredField: 'ipg_insureddateofbirth' },
     // { patientField: 'ipg_patientgender', insuredField: 'ipg_insuredgender' }
    ];
    var addressFieldsArray: Fields[] = [
      { patientField: 'ipg_patientaddress', insuredField: 'ipg_insuredaddress' },
      { patientField: 'ipg_patientcity', insuredField: 'ipg_insuredcity' },
      { patientField: 'ipg_patientstate', insuredField: 'ipg_insuredstate' },
      { patientField: 'ipg_patientzipcodeid', insuredField: 'ipg_insuredzipcodeid' }
    ];
    var phoneFieldsArray: Fields[] = [
      { patientField: 'ipg_patienthomephone', insuredField: 'ipg_insuredphone' },
      { patientField: 'ipg_patientworkphone', insuredField: 'ipg_insuredphone' },
      { patientField: 'ipg_patientcellphone', insuredField: 'ipg_insuredphone' }
    ];
    if (relationToInsured == 100000000) {
      fieldsArray.forEach(function (value) {
        formContext.getAttribute(value.insuredField).setValue(formContext.getAttribute(value.patientField).getValue());
      });
    }
    else {
      fieldsArray.forEach(function (value) {
        formContext.getAttribute(value.insuredField).setValue(null);
      });
    }

    addressFieldsArray.forEach(function (value) {
      formContext.getAttribute(value.insuredField).setValue(formContext.getAttribute(value.patientField)?.getValue());
    });
    
    for (var i = 0; i < phoneFieldsArray.length; i ++){
      var phone = formContext.getAttribute(phoneFieldsArray[i].patientField)?.getValue();
      if (phone && phone != ""){
        formContext.getAttribute(phoneFieldsArray[i].insuredField).setValue(phone);
        break;
      }
    }
  }

  /**
    * Set required level
    * @function Intake.Case.SetFieldsRequiredLevel
    * @returns {void}
    */
  function SetFieldsRequiredLevel(executionContext) {
    let formContext = executionContext.getFormContext();
    let relationToInsured = formContext.getAttribute("ipg_relationtoinsured").getValue();
    const fieldsArray = [
      'ipg_insuredlastname',
      'ipg_insuredfirstname',
      'ipg_insuredmiddlename',
      'ipg_insuredgender'
    ];
    if (relationToInsured == 100000000) {
      fieldsArray.forEach(field =>{
        formContext.getAttribute(field)?.setRequiredLevel("none");
      })
    }
    else {
      fieldsArray.forEach(field =>{
        formContext.getAttribute(field)?.setRequiredLevel("required");
      })
    }
  }
}
