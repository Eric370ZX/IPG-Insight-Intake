/**
 * @namespace Intake.Payment.Ribbon
 */
namespace Intake.Payment.Ribbon {

  /**
   * Called on 'Add Manual Payment' button clickn
   * @function Intake.Payment.Ribbon.AddManualPayment
   * @returns {void}
  */
  export function AddManualPayment(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let formParameters =
    {
      incidentId: formContext.data.entity.getId()
    };
    let webResourceName = "ipg_/intake/claim/PaymentProcessing.html";
    var customParameters = encodeURIComponent("params=" + JSON.stringify(formParameters));
    Xrm.Navigation.openWebResource(webResourceName, null, customParameters);

  }

  /**
   * Called on Additional Details button click
   * @function Intake.Payment.Ribbon.OnAdditionalDetailsClick
   * @returns {void}
  */
  export function OnAdditionalDetailsClick(firstSelectedItemId, primaryControl) {

    let formContext: Xrm.FormContext = primaryControl;
    if (firstSelectedItemId == null)
      return;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_payment";
    entityFormOptions["entityId"] = firstSelectedItemId.replace(/[{}]/g, "");
    entityFormOptions["formId"] = "4c6e5b46-79fd-4f21-acb7-ca1269d5a1b2";
    Xrm.Navigation.openForm(entityFormOptions);

  }

  /**
   * Called on Add Adjustment button click
   * @function Intake.Payment.Ribbon.OnAddAdjustmentClick
   * @returns {void}
  */
  export function OnAddAdjustmentClick(primaryControl) {

    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.getAttribute("ipg_caseid").getValue();
    if (!caseId) {
      return;
    }

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_adjustment";

    Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId[0].id), "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
      let formParameters = {};
      formParameters["ipg_caseid"] = Intake.Utility.removeCurlyBraces(caseId[0].id);
      formParameters["ipg_caseidname"] = incident.title;
      formParameters["ipg_caseidtype"] = caseId[0].entityType;
      formParameters["ipg_percent"] = 100;
      formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
      formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
      formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
      formParameters["ipg_casebalance"] = incident.ipg_casebalance;
      Xrm.Navigation.openForm(entityFormOptions, formParameters);
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });

  }

  /**
   * Called on Cancel button click
   * @function Intake.Payment.Ribbon.OnCancelClick
   * @returns {void}
  */
  export function OnCancelClick(primaryControl) {
    window.history.back();
  }

  /**
   * Called on Save button click
   * @function Intake.Payment.Ribbon.OnSaveClick
   * @returns {void}
  */
  export function OnSaveClick(formContext) {
    formContext.data.save().then(
      function (success) {
        callValidatePaymentAction(formContext);
        checkAdjustmentTask(formContext);
      },
      function (error) {
        console.log(error.message);
      });
  }

  async function callValidatePaymentAction(formContext) {
    const target: { ipg_paymentid: string } = {
      ipg_paymentid: Intake.Utility.removeCurlyBraces(formContext.data.entity.getId())
    };
    target["@odata.type"] = "Microsoft.Dynamics.CRM.ipg_payment";
    const parameters: { Target: any } = {
      Target: target,
    };

    var ipg_IPGPaymentPreValidatePayment = {
      Target: parameters.Target,

      getMetadata: function () {
        return {
          boundParameter: null,
          parameterTypes: {
            "Target": {
              "typeName": "mscrm.ipg_payment",
              "structuralProperty": 5
            }
          },
          operationType: 0,
          operationName: "ipg_IPGPaymentPreValidatePayment"
        };
      }
    };

    Xrm.WebApi.online.execute(ipg_IPGPaymentPreValidatePayment).then(
      async function success(result) {
        if (result.ok) {
          const response = await result.json();
          if (response.Message) {
            Xrm.Utility.alertDialog(response.Message, null);
          }
        }
      },
      function (error) {
        Xrm.Utility.alertDialog(error.message, null);
      }
    );
  }

  function checkAdjustmentTask(formContext) {
    let caseId = formContext.getAttribute("ipg_caseid").getValue();
    if (caseId) {
      Xrm.WebApi.online.retrieveMultipleRecords("task", `?$select=activityid,subject&$filter=_regardingobjectid_value eq ${Intake.Utility.removeCurlyBraces(caseId[0].id)} and ipg_tasktypecode eq 427880065 and statecode eq 0`).then(
        function success(results) {
          if (results.entities.length) {
            if (confirm(`The task "${results.entities[0]["subject"]}" is assigned to the case ${caseId[0].name}. Do you want to open it?`)) {
              let entityFormOptions = {};
              entityFormOptions["entityName"] = "task";
              entityFormOptions["entityId"] = results.entities[0]["activityid"];
              Xrm.Navigation.openForm(entityFormOptions);
            }
          }
        },
        function (error) {
          Xrm.Utility.alertDialog(error.message, null);
        }
      );
    }
  }

}
