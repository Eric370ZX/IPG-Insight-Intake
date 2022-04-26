/**
 * @namespace Intake.Order
 */
namespace Intake.Order {
  const _utility: Xrm.Utility = Xrm.Utility;
  const _clientUrl: string = Xrm.Page.context.getClientUrl();
  const _webApiVersion = 'v9.1';
  const _entityname = 'salesorder';

  const customActions = {
    manualPOCommunication: "ipg_ManualPOCommunication"
  }

  export enum statecode {
    Closed = 3
  }

  export enum statuscode {
    InvoiceReceived = 923720006,
    InReview = 923720007,
    VerifiedForPayment = 923720008,
    Partial = 923720010
  }

  export enum ipg_commstatus {
    Override = 427880000,
    ReadyToCommunicate = 923720003
  }

  export enum ipg_potypecode {
    TPO = 923720000,
    ZPO = 923720001,
    CPA = 923720002,
    MPO = 923720004
  }

  /**
   * Called on load form
   * @function Intake.Order.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    RemoveUnusedPOTypes(formContext);
    manufacturerVisibility(executionContext);
    formContext.data.addOnLoad(() => LockOrder(formContext));
    LockOrder(formContext);
  }

  /**
    * Called on change Invoice Subtotal
    * @function Intake.Order.OnChangeInvoiceSubtotal
    * @returns {void}
  */
  export function OnChangeInvoiceSubtotal(executionContext) {
    let formContext = executionContext.getFormContext();
    calculateInvoiceTotal(formContext);
  }

  /**
    * Called on change Invoice Tax
    * @function Intake.Order.OnChangeInvoiceTax
    * @returns {void}
  */
  export function OnChangeInvoiceTax(executionContext) {
    let formContext = executionContext.getFormContext();
    calculateInvoiceTotal(formContext);
  }

  /**
    * Called on change Invoice Shipping
    * @function Intake.Order.OnChangeInvoiceShipping
    * @returns {void}
  */
  export function OnChangeInvoiceShipping(executionContext) {
    let formContext = executionContext.getFormContext();
    calculateInvoiceTotal(formContext);
  }

  /**
    * Called on change Additional Invoice Total
    * @function Intake.Order.OnChangeAdditionalInvoiceTotal
    * @returns {void}
  */
  export function OnChangeAdditionalInvoiceTotal(executionContext) {
    let formContext = executionContext.getFormContext();
    calculateInvoiceTotal(formContext);
  }

  /**
    * Called on change Invoice Total
    * @function Intake.Order.OnChangeInvoiceTotal
    * @returns {void}
  */
  export function OnChangeInvoiceTotal(executionContext) {
    let formContext = executionContext.getFormContext();
    calculateInvoiceVariance(formContext);
  }

  export function OnChangeIsExcludedFromExceptionDashboard(executionContext) {
    let formContext = executionContext.getFormContext();
    changePOCommStatus(formContext);
  }

  /**
    * calculate Invoice Total
    * @function Intake.Order.calculateInvoiceTotal
    * @returns {void}
  */
  function calculateInvoiceTotal(formContext) {
    let invoiceSubtotal = formContext.getAttribute("ipg_invoicesubtotal").getValue();
    let invoiceShipping = formContext.getAttribute("ipg_invoiceshipping").getValue();
    let invoiceTax = formContext.getAttribute("ipg_invoicetax").getValue();
    let invoiceAdditionalTotal = formContext.getAttribute("ipg_additionalinvoicetotal").getValue();

    let invoiceTotal = (isNaN(invoiceSubtotal) ? 0 : invoiceSubtotal)
      + (isNaN(invoiceShipping) ? 0 : invoiceShipping)
      + (isNaN(invoiceTax) ? 0 : invoiceTax)
      + (isNaN(invoiceAdditionalTotal) ? 0 : invoiceAdditionalTotal);

    formContext.getAttribute("ipg_invoicetotal").setValue(invoiceTotal);
    formContext.getAttribute("ipg_invoicetotal").fireOnChange();
  }



  /**
    * calculate Invoice Variance
    * @function Intake.Order.calculateInvoiceVariance
    * @returns {void}
  */
  function calculateInvoiceVariance(formContext) {
    let invoiceTotal = formContext.getAttribute("ipg_invoicetotal").getValue();
    let orderTotal = formContext.getAttribute("totalamount").getValue();

    let invoiceVariance = (isNaN(orderTotal) ? 0 : orderTotal) - (isNaN(invoiceTotal) ? 0 : invoiceTotal);

    formContext.getAttribute("ipg_invoicevariance").setValue(invoiceVariance);
  }

  /**
  * Removes unused PO types
  * @function Intake.Order.RemoveUnusedPOTypes
  * @returns {void}
  */
  function RemoveUnusedPOTypes(formContext) {
    var currentPOType = Number(formContext.getAttribute("ipg_potypecode").getValue());
    if (currentPOType != 923720003)
      formContext.getControl("ipg_potypecode").removeOption(923720003);
  }

  export function CloseOrphans(entityIds: string[], selectedControl: any) {

    if (selectedControl && entityIds && entityIds.length > 0) {
      _utility.showProgressIndicator(null);

      Promise.all(entityIds.map(id =>
        fetch(`${_clientUrl}/api/data/${_webApiVersion}/${_entityname}s(${id})`, {
          method: 'PATCH',
          headers: {
            "Accept": "application/json",
            "Content-Type": "application/json; charset=utf-8",
            "OData-MaxVersion": "4.0",
            "OData-Version": "4.0"
          },
          body: JSON.stringify({
            statecode: 3,
            statuscode: 923720013,
            ipg_downloadedfromportal: false,
          })
        })))
        .then((r) => {
          console.log(r);
          selectedControl.refresh();
          _utility.closeProgressIndicator();
        }).catch(error => {
          console.log(error);
          selectedControl.refresh();
          _utility.closeProgressIndicator();
        });
    }
  }

  export function isOrphaneView(selectedControl): boolean {
    return selectedControl.getViewSelector
      && selectedControl.getViewSelector().getCurrentView().name.toLowerCase().indexOf('orphan') > -1
  }

  /**
    * Checks if PO is available for revision
    * @function Intake.Order.IsRevisionAvailable
    * @returns {boolean}
  */
  export async function IsRevisionAvailable(selectedItems) {
    let order = await Xrm.WebApi.retrieveRecord("salesorder", selectedItems[0].Id, "?$select=statecode,statuscode,ipg_isestimatedpo");
    return (((order.statecode != 3) || (order.statuscode != 923720004)) && !order.ipg_isestimatedpo);
  }

  /**
    * Checks if PO is available for Void operation
    * @function Intake.Order.IsVoidAvailable
    * @returns {boolean}
  */
  export async function IsVoidAvailable(selectedItems) {
    let order = await Xrm.WebApi.retrieveRecord("salesorder", selectedItems[0].Id, "?$select=statecode,statuscode,ipg_isestimatedpo");
    return ((order.statecode != 3) && !order.ipg_isestimatedpo);
  }


  export function GetCustomerId(formContext) {
    let Id = formContext.getAttribute("ipg_customerlookupid").getValue();
    let name = formContext.getAttribute("ipg_customerlookupname").getValue();

    if (Id) {
      let object = new Array();
      object[0] = new Object();
      object[0].id = Id;
      object[0].name = name;
      object[0].entityType = "salesorder";
      formContext.getAttribute("customerid").setValue(null);
      formContext.getAttribute("customerid").setValue(object);
    }



  }

  export function SetCustomerId(primaryControl) {
    var formContext = primaryControl;
    let entityFormOptions = {};
    entityFormOptions["entityName"] = "salesorder";
    let customerId = formContext.getAttribute("customerid").getValue();
    let formParameters = {};
    formParameters["ipg_customerlookupid"] = customerId[0].id;
    formParameters["ipg_customerlookupname"] = customerId[0].name;
    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
      console.log(success);
    }, function (error) {
      console.log(error);
    });

    
    
  }
  

  /**
    * Opens a revised PO form
    * @function Intake.Order.OpenPOForRevision
    * @returns {void}
  */
  export function OpenPOForRevision(selectedItems) {
    if (confirm("The selected PO will be moved to the history section. Do you want to proceed?")) {
      Xrm.Utility.showProgressIndicator("Please wait");
      var request = {
        "Record": {
          "@odata.type": "Microsoft.Dynamics.CRM.salesorder",
          "salesorderid": selectedItems[0].Id
        },
        getMetadata: function () {
          return {
            boundParamter: null,
            parameterTypes: {
              "Record": {
                "typeName": "mscrm.crmbaseentity",
                "structuralProperty": 5
              }
            },
            operationType: 0,
            operationName: "ipg_IPGIntakeActionsCloneRecord"
          };
        }
      };
      return Xrm.WebApi.online.execute(request)
        .then(function (response) {
          Xrm.Utility.closeProgressIndicator();
          if (response.ok) {
            var entity = { ipg_isrevised: true, ipg_revisiondate: new Date().toISOString() };
            Xrm.WebApi.updateRecord("salesorder", selectedItems[0].Id, entity).then(function success(result) {
              response.json().then(function (data) {
                var entityFormOptions = { entityName: "salesorder", entityId: data.salesorderid };
                Xrm.Navigation.openForm(entityFormOptions);
              });
            }, function (error) {
              Xrm.Utility.alertDialog(error.message, null);
            });
          }
          else {
            Xrm.Navigation.openErrorDialog({ message: response.statusText });
          }
        }, function (error) {
          Xrm.Navigation.openErrorDialog({ message: error.message });
        });
    }
  }

  /**
    * Voids PO
    * @function Intake.Order.VoidPO
    * @returns {void}
  */
  export function VoidPO(primaryControl, selectedItems) {
    if (confirm("The selected PO will be voided. Do you want to proceed?")) {
      let formContext: Xrm.FormContext = primaryControl;
      Xrm.Utility.showProgressIndicator("Please wait");
      var request = {
        entity: {
          entityType: "salesorder",
          id: selectedItems[0].Id
        },
        getMetadata: function () {
          return {
            boundParameter: "entity",
            parameterTypes: {
              entity: {
                typeName: "mscrm.salesorder",
                structuralProperty: 5
              }
            },
            operationType: 0,
            operationName: "ipg_IPGIntakeActionsVoidPO"
          };
        }
      };
      return Xrm.WebApi.online.execute(request)
        .then(function (response) {
          Xrm.Utility.closeProgressIndicator();
          if (response.ok) {
            let gridControl: Xrm.Page.GridControl = formContext.getControl("PurchaseOrders");
            gridControl.refresh();
          }
          else {
            Xrm.Navigation.openErrorDialog({ message: response.statusText });
          }
        }, function (error) {
          Xrm.Navigation.openErrorDialog({ message: error.message });
        });
    }
  }

  function LockOrder(formContext: Xrm.FormContext) {
    const statusReasoncode = formContext.getAttribute("statuscode")?.getValue();

    if (statusReasoncode === statuscode.InReview ||
      statusReasoncode === statuscode.InvoiceReceived ||
      statusReasoncode === statuscode.Partial ||
      statusReasoncode === statuscode.VerifiedForPayment) {
      formContext.ui.controls.forEach((control: Xrm.Controls.StandardControl) => {
        if (control && control.getDisabled && !control.getDisabled()) {
          control.setDisabled(true);
        }
      });
    }
  }

  function changePOCommStatus(formContext: Xrm.FormContext) {
    const ignore = formContext.getAttribute("ipg_isexcludedfromexceptionsdashboard")?.getValue();
    const commStatus = formContext.getAttribute("ipg_commstatus");

    if (ignore) {
      commStatus?.setValue(ipg_commstatus.Override);
    }
    else {
      commStatus?.setValue(ipg_commstatus.ReadyToCommunicate);
    }
  }

  export function manufacturerVisibility(executionContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    const manufacturer = formContext.getControl("ipg_manufacturer_id");
    const poType = formContext.getAttribute("ipg_potypecode")?.getValue();
    if (manufacturer)
      manufacturer.setVisible(poType !== ipg_potypecode.CPA)
  }

  export function OnClickCommunicatePORibbon(primaryContext: any) {
    let formContext: Xrm.FormContext = primaryContext;

    Xrm.Utility.showProgressIndicator("Please wait");
    const manualPOComunicationRequest = {
      OrderId: formContext.data.entity.getId()?.replace("{", "").replace("}", ""),
      getMetadata: () => {
        return {
          boundParameter: null,
          parameterTypes: {
            "OrderId": {
              "typeName": "Edm.String",
              "structuralProperty": 1
            },
          },
          operationType: 0,
          operationName: customActions.manualPOCommunication
        };
      }
    };

    Xrm.WebApi.online.execute(manualPOComunicationRequest).then(
      result => {
        if (result.ok) {
          result.json().then(function (data) {
            if (data.Success) {
              Xrm.Utility.closeProgressIndicator();
              formContext.data.refresh(true).then(() => formContext.ui.refreshRibbon());
              Xrm.Utility.alertDialog(data.Message, null)
            }
            else {
              Xrm.Utility.closeProgressIndicator();
              Xrm.Utility.alertDialog(data.Message, null);
            }
          });
        }     
      },
      error => {
        Xrm.Utility.closeProgressIndicator();
        Xrm.Utility.alertDialog(error.message, null);
      }
    );
  }

  export function IsEnableCommunicatePORibbon(primaryControl: any) {
    let formContext: Xrm.FormContext = primaryControl;
    const commStatus = formContext.getAttribute("ipg_commstatus").getValue();

    return commStatus == ipg_commstatus.ReadyToCommunicate;
  }
}

