/**
 * @namespace Intake.CasePartDetailRibbon
 */
namespace Intake.CasePartDetailRibbon {

  export function OpenCalcRevHistory(primaryControl: Xrm.FormContext): void {
    //debugger;

    //get CalcRev application environment suffix
    let env: string = Intake.Utility.GetEnvironment();
    let envSuffix: string;
    if (env) {
      envSuffix = '-' + env;
    }
    else {
      envSuffix = '';
    }

    //get incidentId
    let incidentId: string = primaryControl.data.entity.getId();
    incidentId = incidentId.replace(/[{}]/g, ""); //delete curly brackets

    let windowSizeAndPosition: string = Intake.Utility.GetCalcRevWindowSizeAndPosition();

    //finally open the window
    window.open(`https://insight-calcrev${envSuffix}.azurewebsites.net/Case/History#/${incidentId}`, 'calcRevWindow', windowSizeAndPosition);
  }

  export function ShouldEnableCalcRevHistoryButton(primaryControl: Xrm.FormContext): boolean {
    //debugger;
    let actualRevenueAttribute: Xrm.Attributes.Attribute = primaryControl.getAttribute("ipg_actualrevenue");
    if (actualRevenueAttribute) {
      let actualRevenueAttributeValue: any = actualRevenueAttribute.getValue();
      if (typeof actualRevenueAttributeValue !== 'undefined' && actualRevenueAttributeValue != null) {
        return true;
      }
    }

    return false;
  }

  export function OpenManagePage(primaryControl, primaryRecordId, type) { 
    var params = {
        caseId: Intake.Utility.removeCurlyBraces(primaryRecordId),
        isEstimatedParts:0
    };
    
    if(type == "Estimated")
    {
        params.isEstimatedParts = 1;
    }

    if(params.isEstimatedParts == 0)
    {
      //Call EBV Before Adding/Manage Parts
      let arguments = { IsUserGenerated: false, CarrierNumber: 1 };
      Intake.Utility.CallActionProcessAsync("incidents", Intake.Utility.removeCurlyBraces(primaryRecordId), "ipg_IPGCaseActionsVerifyBenefits", arguments);
    }

  
    var windowParams = {
        width: 1320,
        height: 1024,
        openInNewWindow: true,
    };
    Xrm.Navigation.openWebResource('ipg_/intake/react-pages/case-actual-parts.html', windowParams, JSON.stringify(params));
}
  export function DisplayIcon(rowData, userLCID) {
    var str = JSON.parse(rowData);
    var quantity = str.ipg_quantity_Value || 0;
    var icon = "ipg_/intake/img/warning-yellow-32px.png";
    if (quantity == 0) {
        return [icon];
    }
  }
  export function IsCaseInCaseManagement(primaryControl: Xrm.FormContext):boolean
  {
    return primaryControl?.data?.entity.getEntityName() === "incident" && primaryControl.getAttribute("ipg_statecode")?.getValue() === 923720002; //Case Management
  }
}
