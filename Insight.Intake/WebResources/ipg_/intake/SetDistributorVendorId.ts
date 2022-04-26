/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {

  /**
   * Called on Distributor Name change
   * @function Intake.Utility.SetDistributorVendorId
   * @returns {void}
  */
  export function SetDistributorVendorId(executionContext) {
    let formContext = executionContext.getFormContext();
    let distributorNameField: Xrm.Controls.StandardControl = <Xrm.Controls.StandardControl>executionContext.getEventSource().getName();

    if (distributorNameField) {
      let distributorNameAttribute: Xrm.Attributes.Attribute = formContext.getAttribute(distributorNameField);
      if (distributorNameAttribute) {
        let distributorNameArray = distributorNameAttribute.getValue().split(" ");
        let vendorID = "D";

        for (let distName of distributorNameArray) {
          vendorID = vendorID.concat(distName.charAt(0));
        }

        if (vendorID) {
          Xrm.WebApi.online.retrieveMultipleRecords("account", "?$select=ipg_vendoridnumber&$filter=startswith(ipg_vendoridnumber," + "'" + vendorID + "'" + ")&$orderby=ipg_vendoridnumber asc").then(
            function success(results) {

              //if there are more than one matches, order by the id number and then increment the integer suffix.
              if (results && results.entities && results.entities.length > 1) {

                let largestVendorID = results.entities[results.entities.length - 1].ipg_vendoridnumber;

                let splitVendorId = largestVendorID.split(/(\d+)/).filter(Boolean);

                if (splitVendorId.length > 1) {

                  let vendorIDNumberSegment = splitVendorId[1];
                  if (!isNaN(Number(vendorIDNumberSegment))) {

                    let incremented = (Number(vendorIDNumberSegment)) + 1;
                    vendorID = vendorID + String(incremented);
                  }
                }             
              }

              //if there is only one match, this is the first duplicate
              //so append a 1
              else if (results && results.entities && results.entities.length === 1) {
                vendorID = vendorID + "1";
              }

              //otherwise, just go ahead and set the vendor id.
              let vendorIdField: Xrm.Controls.StandardControl = <Xrm.Controls.StandardControl>formContext.getControl("ipg_vendoridnumber").getName();

              if (vendorIdField) {
                let vendorIdAttribute: Xrm.Attributes.Attribute = formContext.getAttribute(vendorIdField);
                if (vendorIdAttribute) {
                  vendorIdAttribute.setValue(vendorID.toUpperCase());
                }
              }
            },
            function (error) {
              Xrm.Navigation.openErrorDialog(error);
            }
          );
        }

        let vendorIdField: Xrm.Controls.StandardControl = <Xrm.Controls.StandardControl>formContext.getControl("ipg_vendoridnumber").getName();

        if (vendorID) {
        let vendorIdAttribute: Xrm.Attributes.Attribute = formContext.getAttribute(vendorIdField);
                if (vendorIdAttribute) {
                  vendorIdAttribute.setValue(vendorID.toUpperCase());
                }
              }
       }
      }
    }
  }

