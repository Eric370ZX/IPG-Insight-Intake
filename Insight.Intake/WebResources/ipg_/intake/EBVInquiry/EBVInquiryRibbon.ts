
/**
 * @namespace Intake.EBVInquiry
 */
namespace Intake.EBVInquiry {

  /**
   * Opens EBVResponse\ViewDetails.html.
   * @function Intake.EBVInquiry.ViewResponseDetails
   * @returns {void}
   */
  export function ViewResponseDetails(firstSelectedItemId: string) {
    
    if (!firstSelectedItemId) {
      alert('EBV Inquiry ID is required to view the response details');
      return;
    }

    Xrm.WebApi.retrieveRecord("ipg_ebvinquiry", firstSelectedItemId, "?$select=ipg_ebvinquiryid,_ipg_responseid_value").then(
      function success(inquiry: any) {
        if (!inquiry._ipg_responseid_value) {
          alert('No response for the selected EBV Inquiry');
          return;
        }

        Xrm.Navigation.openWebResource('ipg_/intake/EBVResponse/ViewDetails.html', { width: 1000, height: 800, openInNewWindow: true }, inquiry._ipg_responseid_value);
      },
      (error: any) => {
        alert('Could not retrieve EBV Inquiry: ' + error.message);
      });
  }
}
