/**
 * @namespace Intake.Account
 */
namespace Intake.Account {
  const associatedCptCodesGridKey = 'AssociatedCPTCodes';
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Account.FilterCPTGrid
   * @returns {void}
   */
  export function FilterCPTGrid() {
    const blockedCptCodesGridControl: Xrm.Page.GridControl = Xrm.Page.getControl(associatedCptCodesGridKey);
    function filter() {
      const accountId = Xrm.Page.data.entity.getId();
      const blockedCptCodesGridFetchXml = `<fetch><entity name="ipg_associatedcpt"><filter type="or"><condition attribute="ipg_carrierid" operator="eq" value="${accountId}" /><condition attribute="ipg_facilityid" operator="eq" value="${accountId}" /></filter></entity></fetch>`;
      blockedCptCodesGridControl.setFilterXml(blockedCptCodesGridFetchXml);
      blockedCptCodesGridControl.refresh();
    }
    filter();
  }
}
