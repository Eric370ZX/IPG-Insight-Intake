/**
 * @namespace Intake.Referral
 */
namespace Intake.Referral {
  const patientLookupKey = 'ipg_patientid';
  const referralGridKey = 'Referrals';
  const caseGridKey = 'Cases';
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Referral.SearchPatientsProcedures
   * @returns {void}
   */
  export function SearchPatientsProcedures(): void {
    const patientLookupAttribute: Xrm.Attributes.LookupAttribute = Xrm.Page.getAttribute(patientLookupKey);
    const referralGridControl: Xrm.Page.GridControl = Xrm.Page.getControl(referralGridKey);
    const caseGridControl: Xrm.Page.GridControl = Xrm.Page.getControl(caseGridKey);
    function filterSubGrids(): void {
      const patientLookupValue = patientLookupAttribute.getValue();
      let referralGridFetchXml = '<fetch><entity name="ipg_referral"><filter type="and"><condition attribute="ipg_referralid" operator="null" /></filter></entity></fetch>';
      let caseGridFetchXml = '<fetch><entity name="incident"><filter type="and"><condition attribute="incidentid" operator="null" /></filter></entity></fetch>';
      // Update filter based on Patient Lookup value.
      if (patientLookupValue && patientLookupValue.length) {
        const patientId = patientLookupValue[0].id;
        referralGridFetchXml = `<fetch><entity name="ipg_referral"><filter type="and"><condition attribute="ipg_patientid" operator="eq" value="${patientId}" /></filter></entity></fetch>`;
        caseGridFetchXml = `<fetch><entity name="incident"><filter type="and"><condition attribute="customerid" operator="eq" value="${patientId}" /></filter></entity></fetch>`;
      }
      // Change filter of the Referral Grid.
      referralGridControl.setFilterXml(referralGridFetchXml);
      referralGridControl.refresh();
      // Change filter of the Case Grid.
      caseGridControl.setFilterXml(caseGridFetchXml);
      caseGridControl.refresh();
    }
    if (patientLookupAttribute) {
      // Filter grids after init.
      filterSubGrids();
      // Add change listener to Patient Lookup Attribute.
      patientLookupAttribute.addOnChange(filterSubGrids);
    }
  }
}
