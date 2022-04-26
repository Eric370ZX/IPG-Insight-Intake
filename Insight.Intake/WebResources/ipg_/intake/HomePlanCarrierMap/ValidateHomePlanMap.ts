namespace Intake.HomePlanCarrierMap {
  export function OnSave(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    getHomePlan(formContext);

  }

  export function formatDate(date) {
    let d = new Date(date),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [month, day, year].join('/');
  }


  export function getHomePlan(formContext) {

    let nameEntry = formContext.getAttribute("ipg_name").getValue();
    let code = formContext.getAttribute("ipg_code").getValue();
    let carrierName = formContext.getAttribute("ipg_carrierid").getValue();
    let effectiveDate = formatDate(formContext.getAttribute("ipg_effectivedate").getValue());
    let endDate = formatDate(formContext.getAttribute("ipg_enddate").getValue());
    let carrierNameObject = formContext.getAttribute("ipg_carrierid").getValue();
    let guid = carrierNameObject[0].id.replace("{", "").replace("}", "");
    let effDt;
    let endDt;

    if (formContext.ui.getFormType() == 1) { 
      if (code != null && carrierName != null && effectiveDate != null && endDate != null) {
        Xrm.WebApi.retrieveMultipleRecords("ipg_homeplancarriermap", "?$select=ipg_CarrierId,ipg_code ,ipg_effectivedate,ipg_name,ipg_enddate&$top=1&$expand=ipg_CarrierId($select=name,accountid)&$filter= and ipg_CarrierId/accountid eq " + guid).then(function success(results) {
          let i = 0;
          for (i = 0; i < results.entities.length; i++) {
            effDt = formatDate(results.entities[i]["ipg_effectivedate@OData.Community.Display.V1.FormattedValue"]);
            endDt = formatDate(results.entities[i]["ipg_enddate@OData.Community.Display.V1.FormattedValue"]);
            let lCode = results.entities[i]["ipg_code"];
            let lName = results.entities[i]["ipg_name"];
            if (effDt == effectiveDate && endDt == endDate && lCode == code && lName == nameEntry) {
              let alertStrings = { confirmButtonLabel: "Close", text: "This Prefix to Home Plan relationship already exists for this date range", title: "Home Plan Mapping" };
              let alertOptions = { height: 120, width: 260 };
              Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                return;
              }, function (error) {
                console.log(error.message);
              });
            }
          }
        }, function (error) {
          Xrm.Navigation.openErrorDialog(error);
        });

      }
    }
  }
}
