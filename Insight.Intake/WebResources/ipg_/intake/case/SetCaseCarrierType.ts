namespace Intake.Case {

  export function GetCarrierType(executionContext) {
 
    let formContext = executionContext.getFormContext();

    let tabObj = formContext.ui.tabs.get("PatientProcedureDetails"); //the tab where your section is
    let sectionObj = tabObj.sections.get("additionalInfornaionSection"); // section name
    sectionObj.setVisible(false);

    let carrierName = formContext.getAttribute("ipg_carrierid").getValue();
    let carrierObject = carrierName[0];
    let guid = carrierObject.id;;
    if (carrierName != null) {
      Xrm.WebApi.retrieveRecord("account", guid, "?$select=ipg_carriersupportedplantypes").then(function success(results) {
        if (results) {
          if ((results["ipg_carriersupportedplantypes@OData.Community.Display.V1.FormattedValue"] === "Auto") || (results["ipg_carriersupportedplantypes@OData.Community.Display.V1.FormattedValue"] === "Workers Comp")) {

            sectionObj.setVisible(true);
            formContext.getAttribute("ipg_autodateofincident")?.setRequiredLevel("required");  
            formContext.getAttribute("ipg_autoadjustername")?.setRequiredLevel("required");
            formContext.getAttribute("ipg_adjusterphone")?.setRequiredLevel("required");
            formContext.getControl("ipg_homeplancarrierid").setVisible(false);
            formContext.getControl("ipg_primarycarriergroupidnumber").setVisible(false);
            formContext.getControl("ipg_plansponsor").setVisible(false);
          }
        }
      }, function (error) {
        console.log(error.message);
      });
    }

  }


  export function GetReferralType(executionContext) {
 
    let formContext = executionContext.getFormContext();

    let carrierName = formContext.getAttribute("ipg_carrierid").getValue();
    if (carrierName != null) {
      let carrierObject = carrierName[0];
      let guid = carrierObject.id;;
      Xrm.WebApi.retrieveRecord("account", guid, "?$select=ipg_carriertype").then(function success(results) {
        if (results) {
          if ((results["ipg_carriertype@OData.Community.Display.V1.FormattedValue"] === "Auto") || (results["ipg_carriertype@OData.Community.Display.V1.FormattedValue"] === "Workers Comp")) {
            formContext.getAttribute("ipg_autodateofincident")?.setRequiredLevel("required");
            formContext.getAttribute("ipg_autoadjustername")?.setRequiredLevel("required");
            formContext.getAttribute("ipg_autoadjusterphone")?.setRequiredLevel("required");

          }
        }
      }, function (error) {
        console.log(error.message);
      });
    }

  }
















}

