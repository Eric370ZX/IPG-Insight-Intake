/**
 * @namespace Intake.OrderProduct.Ribbon
 */
namespace Intake.OrderProduct.Ribbon {
  /**
   * on click button "Invoices". It shows Invoice table and filter invoices by order product
   * @function Intake.OrderProduct.Ribbon.showInvoicesGrid
   * @returns {void}
   */
  export function showInvoicesGrid(primaryControl, selectedControl, selectedItemReferences) {
    let formContext: Xrm.FormContext = primaryControl;

    if (selectedControl.getGrid) {
      let grid = selectedControl.getGrid();
      let selectedRows = grid.getSelectedRows();
      if (selectedRows && selectedRows.getLength() == 1) {
        setFilterToInvoiceGrid(formContext, selectedItemReferences[0]);
        setInvoiceSectionVisible(formContext, true);
      }
    }
  }

  /**
   * if the count of selected record greater than one, hide Invoices grid
   * @function Intake.OrderProduct.Ribbon.hideInvoicesGrid
   * @returns {boolean}
   */
  export function hideInvoicesGrid(primaryControl, selectedControl): boolean {
    let formContext: Xrm.FormContext = primaryControl;

    if (selectedControl.getGrid) {
      let grid = selectedControl.getGrid();
      let selectedRows = grid.getSelectedRows();
      if (selectedRows && selectedRows.getLength() != 1) {
        setInvoiceSectionVisible(formContext, false);
      } else {
        return true;
      }
    }

    return false;
  }

  /**
  * set invoice section Visible
  * @function Intake.OrderProduct.Ribbon.setInvoiceSectionVisible
  * @returns {boolean}
  */
  function setInvoiceSectionVisible(formContext: Xrm.FormContext, visible: boolean) {
    let summaryTab = formContext.ui.tabs.get("summary_tab");
    if (summaryTab) {
      let invoiceSection = summaryTab.sections.get("invoiceSection");
      if (invoiceSection) {
        invoiceSection.setVisible(visible);
      }
    }
  }

  /**
  * set filters to grid "Invoices"
  * @function Intake.OrderProduct.Ribbon.setInvoiceSectionVisible
  * @returns {boolean}
  */
  function setFilterToInvoiceGrid(formContext: Xrm.FormContext, selectedItem) {
    Xrm.WebApi.retrieveRecord(selectedItem.TypeName, selectedItem.Id, "?$select=_productid_value").then((result) => {
      if (result && result._productid_value) {
        let facility: Xrm.LookupValue[] = formContext.getAttribute("ipg_facilityid").getValue();
        let invoicesGridControl: Xrm.Page.GridControl = formContext.getControl("Invoices");

        let filterObject = {
          "ViewType": "Invoices",
          "FacilityId": facility[0].id,
          "ProductId": result._productid_value
        };

        var filter = `<filter type='and'>
                        <condition attribute = 'ipg_linkedentitiesfilter' operator = 'eq' value = '${JSON.stringify(filterObject)}' />
                      </filter>`;

        invoicesGridControl.setFilterXml(filter);
        invoicesGridControl.refresh();
      }
    });
  }
}


