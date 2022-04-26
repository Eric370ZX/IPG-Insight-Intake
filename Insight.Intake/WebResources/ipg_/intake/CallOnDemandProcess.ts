/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.CallOnDemandProcess
   * @returns {void}
   */
  export function CallOnDemandProcess(entityTypeCode: number, processId: string): void {
    const globalContext = Xrm.Utility.getGlobalContext();
    const clientUrl = globalContext.getClientUrl();
    const entityId = Xrm.Page.data.entity.getId();
    let promise: Promise<any> = Promise.resolve();
    const requestOptions: Intake.Utility.HttpRequestOptions<{ EntityId: string }> = {
      path: `${clientUrl}/api/data/v9.0/workflows(${processId})/Microsoft.Dynamics.CRM.ExecuteWorkflow`,
      body: {
        EntityId: Intake.Utility.removeCurlyBraces(entityId),
      },
      headers: {
        'OData-MaxVersion': '4.0',
        'OData-Version': '4.0',
        'Accept': 'application/json',
        'Content-Type': 'application/json',
      },
    };
    if (Xrm.Page.data.entity.getIsDirty()) {
      promise = promise.then(() => {
        return Xrm.Navigation
          .openConfirmDialog({
            title: 'Unsaved Changes',
            text: 'Your changes have not been saved. To stay on the page so that you can save your changes, click Cancel.',
          });
      });
    }
    promise.then((payload: { confirmed: boolean }) => {
      if (payload && !payload.confirmed) {
        return;
      }
      Xrm.Utility.showProgressIndicator(null);
      Intake.Utility.HttpRequest
        .post(requestOptions)
        .then(() => {
          const closeProgressIndicatorAndRefreshRibbon = () => {
            Xrm.Utility.closeProgressIndicator();
            Xrm.Page.ui.refreshRibbon();
          };
          Xrm.Page.data.refresh(false)
            .then(closeProgressIndicatorAndRefreshRibbon)
            .catch(closeProgressIndicatorAndRefreshRibbon);
        })
        .catch((response: { error: { message: string } }) => {
          Xrm.Utility.closeProgressIndicator();
          Xrm.Navigation.openErrorDialog({ message: response.error.message });
        });
    });
  }
}
