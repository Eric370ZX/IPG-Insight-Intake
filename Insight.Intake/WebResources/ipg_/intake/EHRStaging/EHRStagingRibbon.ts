
/**
 * @namespace Intake.EHRStaging
 */
namespace Intake.EHRStaging {

  /**
   * Resubmits selected EHRStaging records to be processed again.
   * @function Intake.EHRStaging.Resubmit
   * @returns {void}
   */
  export async function Resubmit(selectedItemIds: string[], primaryControl: Xrm.Controls.Control): Promise<void> {

    //debugger;

    if (!selectedItemIds || !selectedItemIds.length) {
      alert('EHRStaging record ID is required');
      return;
    }

    await DoActionWithProgressIndicator('EHRResubmitURL', selectedItemIds, primaryControl);
  }

  export async function OkPurge(selectedItemIds: string[], primaryControl: Xrm.Controls.Control): Promise<void> {
    if (!selectedItemIds || !selectedItemIds.length) {
      alert('EHRStaging record ID is required');
      return;
    }

    await DoActionWithProgressIndicator('EHROkPurgeURL', selectedItemIds, primaryControl);
  }

  export function isRejectedView(primaryControl: Xrm.Controls.Control): boolean {
    return primaryControl.getViewSelector
      && primaryControl.getViewSelector().getCurrentView().name.toLowerCase().indexOf('rejected') > -1
  }

  export async function AreResubmittableRecords(selectedItemIds: string[], primaryControl: Xrm.Controls.Control): Promise<boolean> {
    //debugger;
    if (!selectedItemIds || !selectedItemIds.length) {
      return false;
    }

    var filterValues = "";
    selectedItemIds.forEach(id => {
      filterValues += "\n<value>" + id + "</value>";
    });
    
    let fetchXml = '?fetchXml=' + encodeURIComponent(`<fetch>
  <entity name="ipg_ehrstaging">
    <attribute name="ipg_ehrstagingid" />
    <attribute name="ipg_status" />
    <filter>
      <condition attribute="ipg_ehrstagingid" operator="in">` +
      filterValues +
      `</condition>
    </filter>
  </entity>
</fetch>`);

    let retrievalResult = await Xrm.WebApi.retrieveMultipleRecords('ipg_ehrstaging', fetchXml);
    return retrievalResult.entities.every(e => e.ipg_status == 'ERROR');
  }


  async function DoActionWithProgressIndicator(urlSettingName: string, selectedItemIds: string[], primaryControl: Xrm.Controls.Control) {
    Xrm.Utility.showProgressIndicator("Processing...");

    try {
      await DoAction(urlSettingName, selectedItemIds, primaryControl);
    } catch (error) {
      Xrm.Utility.closeProgressIndicator();
      Xrm.Navigation.openErrorDialog({ message: error.message || error.responseText });
    }

    Xrm.Utility.closeProgressIndicator();
  }

  async function DoAction(urlSettingName: string, selectedItemIds: string[], primaryControl: Xrm.Controls.Control)
  {
    let retrievalResult = await Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", `?$filter=ipg_name eq '${urlSettingName}'`);
    if (retrievalResult.entities.length == 0) {
      Xrm.Navigation.openErrorDialog({ message: `${urlSettingName} global setting is required` });
    }

    let uri: string = retrievalResult.entities[0].ipg_value + '&EHRIds=' + selectedItemIds.join(',');

    await fetch(uri,
    {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json; charset=utf-8'
      },
    })
    .then(function (response) {
      if (response.status == 200) {
        primaryControl.refresh();
      }
    });
  }
}
