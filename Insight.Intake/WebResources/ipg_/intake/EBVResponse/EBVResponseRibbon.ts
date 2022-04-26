
/**
 * @namespace Intake.EBVInquiry
 */
namespace Intake.EBVResponse {

  /**
   * Opens EBVResponse\ViewDetails.html.
   * @function Intake.EBVResponse.ViewDetails
   * @returns {void}
   */
  export function ViewDetails(firstSelectedItemId: string) {

    if (!firstSelectedItemId) {
      alert('EBV Response ID is required to view the response details');
      return;
    }

    Xrm.Navigation.openWebResource('ipg_/intake/EBVResponse/ViewDetails.html', { width: 1000, height: 800, openInNewWindow: true }, firstSelectedItemId);
  }
}
