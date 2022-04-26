
namespace Intake.Case {
  export function OnCaseZipCodeChange(executionContext, cityFieldKey, stateFieldKey, stateFieldAttribute) {
    let  formContext = executionContext.getFormContext();
    let  PatientZipCode = formContext.getAttribute(stateFieldAttribute);
    if (PatientZipCode.getValue()) {
      let zipId = PatientZipCode.getValue()[0].id;
      let zipName = PatientZipCode.getValue()[0].name;
    //  formContext.getAttribute("ipg_displayzipcodeid").setValue(zipId);
   //   formContext.getAttribute("ipg_displayzipcodename").setValue(zipName);
      Xrm.WebApi.retrieveRecord("ipg_melissazipcode", Intake.Utility.removeCurlyBraces(PatientZipCode.getValue()[0].id), "?$select=ipg_city&$expand=ipg_stateid($select=ipg_name)").then(function success(result) {
        let  City = result.ipg_city;
        let State = result.ipg_stateid.ipg_name;
       
        if (City)
          formContext.getAttribute(cityFieldKey).setValue(City);
        if (State)
          formContext.getAttribute(stateFieldKey).setValue(State);


      }, function (error) {
        Xrm.Utility.alertDialog(error.message, null);
      });
    }
    else {
      formContext.getAttribute(stateFieldKey).setValue(null);
      formContext.getAttribute(cityFieldKey).setValue(null);
    }
  }

  export function SetZipCodeFields(executionContext) {

    let formContext = executionContext.getFormContext();
    let CaseNumber = formContext.getAttribute("title").getValue();

    Xrm.WebApi.retrieveMultipleRecords("ipg_referral", "?$select=ipg_displayzipcodeid,ipg_displayzipcodename&$filter=ipg_referralcasenumber eq '" + CaseNumber + "'").then(function success(results) {
      let ZipCodeId = results.entities[0]["ipg_displayzipcodeid"];
      let ZipCodeName = results.entities[0]["ipg_displayzipcodename"];
      let object = new Array();
      object[0] = new Object();
      object[0].id = ZipCodeId;
      object[0].name = ZipCodeName;
      object[0].entityType = "ipg_melissazipcode";


      formContext.getAttribute("ipg_casepatientzipcodeid").setValue(null);
      formContext.getAttribute("ipg_casepatientzipcodeid").setValue(object);
      OnCaseZipCodeChange(executionContext, "ipg_patientcity", "ipg_patientstate", "ipg_casepatientzipcodeid")

      formContext.getAttribute("ipg_caseinsuredzipcodeid").setValue(null);
      formContext.getAttribute("ipg_caseinsuredzipcodeid").setValue(object);
      OnCaseZipCodeChange(executionContext, "ipg_insuredcity", "ipg_insuredstate", "ipg_caseinsuredzipcodeid")

      formContext.getAttribute("ipg_melissacsecondaryzipcodeid").setValue(null);
      formContext.getAttribute("ipg_melissacsecondaryzipcodeid").setValue(object);
      OnCaseZipCodeChange(executionContext, "ipg_secondaryinsuredcity", "ipg_secondaryinsuredstate", "ipg_melissacsecondaryzipcodeid")

      formContext.getAttribute("ipg_casedemographicszipcodeid").setValue(null);
      formContext.getAttribute("ipg_casedemographicszipcodeid").setValue(object);
      OnCaseZipCodeChange(executionContext, "ipg_patientcity", "ipg_patientstate", "ipg_casedemographicszipcodeid");

      formContext.getAttribute("ipg_melissacaseremainigzipcodeid").setValue(null);
      formContext.getAttribute("ipg_melissacaseremainigzipcodeid").setValue(object);
      OnCaseZipCodeChange(executionContext, "ipg_patientcity", "ipg_patientstate", "ipg_melissacaseremainigzipcodeid");


    }, function (error) {
      Xrm.Utility.alertDialog(error.message, null);
    });
  }


}
