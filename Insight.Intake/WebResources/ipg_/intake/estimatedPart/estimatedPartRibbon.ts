/**
 * @namespace Intake.EstimaredPart.Ribbon
 */
namespace Intake.EstimaredPart.Ribbon {
  export async function OnClcikConvertToActualPart(
    primaryControl: Xrm.FormContext,
    selectedItems: Array<string>
  ) {
    let converted = 0;

    Xrm.Utility.showProgressIndicator("");

    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity
      .getId()
      .replace("{", "")
      .replace("}", "");

    //Call EBV Before Adding/Manage Parts
    let arguments = { IsUserGenerated: false, CarrierNumber: 1 };
    Intake.Utility.CallActionProcessAsync(
      "incidents",
      Intake.Utility.removeCurlyBraces(caseId),
      "ipg_IPGCaseActionsVerifyBenefits",
      arguments
    );

    const estimatedParts = await Xrm.WebApi.retrieveMultipleRecords(
      "ipg_estimatedcasepartdetail",
      `?$filter=_ipg_caseid_value eq ${caseId}
      and _ipg_actualpartid_value eq null and ipg_ranumber eq null`
    );

    if (
      estimatedParts &&
      estimatedParts.entities &&
      estimatedParts.entities.length
    ) {
      for (let i = 0; i < estimatedParts.entities.length; i++) {
        const item = estimatedParts.entities[i];
        if (selectedItems.indexOf(item.ipg_estimatedcasepartdetailid) != -1) {
          try {
            var entity = {};
            entity["ipg_potypecode"] = item.ipg_potypecode;
            entity[
              "ipg_caseid@odata.bind"
            ] = `/incidents(${item._ipg_caseid_value})`;
            entity[
              "ipg_productid@odata.bind"
            ] = `/products(${item._ipg_productid_value})`;
            entity["ipg_quantity"] = item.ipg_quantity;
            entity["ipg_enteredunitcost"] = item.ipg_unitcost;
            entity["ipg_enteredshipping"] = item.ipg_unitshipping;
            entity["ipg_enteredtax"] = item.ipg_unittax;

            if (item._ipg_uomid_value) {
              entity[
                "ipg_uomid@odata.bind"
              ] = `/uoms(${item._ipg_uomid_value})`;
            }

            const actualPart = await Xrm.WebApi.createRecord(
              "ipg_casepartdetail",
              entity
            );
            await Xrm.WebApi.updateRecord(
              "ipg_estimatedcasepartdetail",
              item.ipg_estimatedcasepartdetailid,
              {
                "ipg_actualpartid@odata.bind": `/ipg_casepartdetails(${actualPart.id})`,
              }
            );

            converted++;
          } catch (e) {}
        }
      }
      let customGrid = formContext.getControl(
        "WebResource_CaseActualPartsGrid"
      ) as any;

      if (customGrid) {
        var src = customGrid.getSrc();

        var aboutBlank = "about:blank";
        customGrid.setSrc(aboutBlank);

        setTimeout(function () {
          customGrid.setSrc(src);
        }, 1000);
      }

      formContext.getControl("ActualParts")?.refresh();
      formContext.getControl("EstimatedParts")?.refresh();
    }

    Xrm.Utility.closeProgressIndicator();
    Xrm.Navigation.openAlertDialog({
      text: `Conversion of Estimated Parts to Actual Parts process is complete. ${converted} of ${selectedItems.length} successfully converted.`,
    });
  }
}
