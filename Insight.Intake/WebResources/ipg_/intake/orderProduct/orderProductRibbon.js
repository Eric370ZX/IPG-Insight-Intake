/**
 * @namespace Intake.OrderProduct.Ribbon
 */
var Intake;
(function (Intake) {
    var OrderProduct;
    (function (OrderProduct) {
        var Ribbon;
        (function (Ribbon) {
            /**
             * on click button "Invoices". It shows Invoice table and filter invoices by order product
             * @function Intake.OrderProduct.Ribbon.showInvoicesGrid
             * @returns {void}
             */
            function showInvoicesGrid(primaryControl, selectedControl, selectedItemReferences) {
                var formContext = primaryControl;
                if (selectedControl.getGrid) {
                    var grid = selectedControl.getGrid();
                    var selectedRows = grid.getSelectedRows();
                    if (selectedRows && selectedRows.getLength() == 1) {
                        setFilterToInvoiceGrid(formContext, selectedItemReferences[0]);
                        setInvoiceSectionVisible(formContext, true);
                    }
                }
            }
            Ribbon.showInvoicesGrid = showInvoicesGrid;
            /**
             * if the count of selected record greater than one, hide Invoices grid
             * @function Intake.OrderProduct.Ribbon.hideInvoicesGrid
             * @returns {boolean}
             */
            function hideInvoicesGrid(primaryControl, selectedControl) {
                var formContext = primaryControl;
                if (selectedControl.getGrid) {
                    var grid = selectedControl.getGrid();
                    var selectedRows = grid.getSelectedRows();
                    if (selectedRows && selectedRows.getLength() != 1) {
                        setInvoiceSectionVisible(formContext, false);
                    }
                    else {
                        return true;
                    }
                }
                return false;
            }
            Ribbon.hideInvoicesGrid = hideInvoicesGrid;
            /**
            * set invoice section Visible
            * @function Intake.OrderProduct.Ribbon.setInvoiceSectionVisible
            * @returns {boolean}
            */
            function setInvoiceSectionVisible(formContext, visible) {
                var summaryTab = formContext.ui.tabs.get("summary_tab");
                if (summaryTab) {
                    var invoiceSection = summaryTab.sections.get("invoiceSection");
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
            function setFilterToInvoiceGrid(formContext, selectedItem) {
                Xrm.WebApi.retrieveRecord(selectedItem.TypeName, selectedItem.Id, "?$select=_productid_value").then(function (result) {
                    if (result && result._productid_value) {
                        var facility = formContext.getAttribute("ipg_facilityid").getValue();
                        var invoicesGridControl = formContext.getControl("Invoices");
                        var filterObject = {
                            "ViewType": "Invoices",
                            "FacilityId": facility[0].id,
                            "ProductId": result._productid_value
                        };
                        var filter = "<filter type='and'>\n                        <condition attribute = 'ipg_linkedentitiesfilter' operator = 'eq' value = '" + JSON.stringify(filterObject) + "' />\n                      </filter>";
                        invoicesGridControl.setFilterXml(filter);
                        invoicesGridControl.refresh();
                    }
                });
            }
        })(Ribbon = OrderProduct.Ribbon || (OrderProduct.Ribbon = {}));
    })(OrderProduct = Intake.OrderProduct || (Intake.OrderProduct = {}));
})(Intake || (Intake = {}));
