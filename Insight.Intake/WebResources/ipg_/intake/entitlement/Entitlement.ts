/**
 * @namespace Intake.Entitlement
 */
namespace Intake.Entitlement {

  /**
  * Shows warning if Expiration Date is less than Effective Date
  * @function Intake.Entitlement.DateOnChange
  * @returns {boolean}
  */
  export function DateOnChange(executionContext) {
    var formContext = executionContext.getFormContext();
    var startdate = formContext.getAttribute("startdate").getValue();
    var enddate = formContext.getAttribute("enddate").getValue();
    if ((startdate) && (enddate)) {
      if (enddate < startdate) {
        formContext.getControl("enddate").setNotification("Effective Date must be less than Expiration Date.", "enddate");
        formContext.getAttribute("enddate").setValue(null);
      }
      else
        formContext.getControl("enddate").clearNotification("enddate");
    }
    else
      formContext.getControl("enddate").clearNotification("enddate");
  }

  /**
* Chooses form depending on 'Relation Type' field value
* @function Intake.Entitlement.ChooseForm
* @returns {void}
*/
  function ChooseForm(formContext) {
    var entitlementType = formContext.getAttribute("ipg_entitlementtype").getValue();
    if (entitlementType) {
      var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
      interface entitlementTypesValues {
        name: string;
        value: number;
        formId: string;
      }
      var entitlementTypes: entitlementTypesValues[] = [
        { name: 'facilitycarrier', value: 923720001, formId: '482AC91C-B6FE-4B8C-92A9-0F24356E416F'.toLocaleLowerCase() },
      ];

      var currentEntitlementType = entitlementTypes.find(x => x.value == entitlementType);
      if (currentEntitlementType != undefined) {
        if (currentFormId != currentEntitlementType.formId) {
          var items = formContext.ui.formSelector.items.get();
          for (var i in items) {
            if (items[i].getId() == currentEntitlementType.formId)
              items[i].navigate();
          }
        }
      }
    }
  }


  /**
  * Set MAIC Status
  * @function Intake.Entitlement.MAICRuleDateOnChange
  * @returns {boolean}
  */
  export function MAICRuleDateOnChange(executionContext) {
    var formContext = executionContext.getFormContext();
    var MAICRuleDate = formContext.getAttribute("ipg_macruledate").getValue();
    if (MAICRuleDate) {
      var currentDate = new Date();
      currentDate.setHours(0, 0, 0, 0);
      if (MAICRuleDate <= currentDate)
        formContext.getAttribute("ipg_disablemaic").setValue(true);
      else
        formContext.getAttribute("ipg_disablemaic").setValue(false);
    }
  }

  /**
  * Shows warning if Facility field Signed BSA is set to no
  * @function Intake.Entitlement.FacilityOnChange
  * @returns {boolean}
  */
  export function FacilityOnChange(executionContext) {
    var fieldName = "ipg_facilityid";
    var formContext = executionContext.getFormContext();
    var facility = formContext.getAttribute(fieldName).getValue();
    if (facility) {
      formContext.getControl("ipg_facilityid").setDisabled(true);
      Xrm.WebApi.retrieveRecord("account", facility[0].id, "?$select=ipg_facilitysignedbsa").then(
        function success(result) {
          if (result.ipg_facilitysignedbsa == 923720002) {
            formContext.getControl(fieldName).setNotification("Signed BSA is set to NO. Therefore, a carrier contract cannot be added to this facility.", fieldName);
          }
          else {
            formContext.getControl(fieldName).clearNotification(fieldName);
          }
        }
        , function (error) {
          Xrm.Navigation.openErrorDialog({ message: error.message });
        });
    }
  }

  function updateName(formContext: any) {
    if (formContext.getAttribute("ipg_carrierid").getValue() != null && formContext.getAttribute("ipg_facilityid").getValue() != null) {
      var cptCarrierName = formContext.getAttribute("ipg_carrierid").getValue()[0].name;
      var cptFacilityName = formContext.getAttribute("ipg_facilityid").getValue()[0].name;
      formContext.getAttribute("name").setValue(cptCarrierName + " - " + cptFacilityName);
    }
  }

  /**
    * Called on Form Load event
    * @function Intake.Entitlement.OnLoadForm
    * @returns {void}
  */
  export function OnLoadForm(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getControl("ipg_entitlementtype").setDisabled(true);
    NetworkStatus(formContext);

    var facility = formContext.getAttribute("ipg_facilityid").getValue();   
    if (facility) formContext.getControl("ipg_facilityid").setDisabled(true);  

    
    var carrier = formContext.getAttribute("ipg_carrierid").getValue();
    if (carrier) formContext.getControl("ipg_carrierid").setDisabled(true);

    var carrierParentContractControl = formContext.getControl("ipg_carrierparentcontractid");
    if (carrierParentContractControl)  formContext.getControl("ipg_carrierparentcontractid").setVisible(false);

    if (formContext.ui.getFormType() == 2) { //Update

      let entitlementId = Xrm.Page.data.entity.getId();

      formContext.getControl("ipg_facilityid").setDisabled(true);
      formContext.getControl("ipg_carrierid").setDisabled(true);

      Xrm.WebApi.retrieveMultipleRecords("ipg_facilitycarriercptrulecontract", "?$select=ipg_name&$filter=_ipg_entitlementid_value eq " + entitlementId).then(
        function success(result) {

          if (result.entities.length > 0) {
            var controls = Xrm.Page.ui.controls.get();
            for (var i in controls) {

              var control = controls[i];
              control.setDisabled(true);

            }
          }
        },
        function (error) {
          console.log(error.message);
        }
      );


    }
    ChooseForm(formContext);
    RemoveUnusedByCarrierPlanTypes(formContext);
    SetDefaultStartDate();
    FacilityOnChange(executionContext);
  }

  var startDateKey = 'startdate';
  var endDateKey = 'enddate';

  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Document.SetDefaultPublishDate
   * @returns {void}
   */
  function SetDefaultStartDate() {
    var startDateAttribute = Xrm.Page.getAttribute(startDateKey);
    var endDateAttribute = Xrm.Page.getAttribute(endDateKey);

    var startDateValue = startDateAttribute.getValue();
    if (!startDateValue) {
      var todayDate = new Date();
      startDateAttribute.setValue(todayDate);
    }

    var endDateValue = endDateAttribute.getValue();
    if (!endDateValue) {
      var defaultExpirationDate = new Date('12/31/9999');
      endDateAttribute.setValue(defaultExpirationDate);
    }
  }
/**
* Called on save form
* @function Intake.Entitlement.NetworkStatus
* @returns {void}
*/

  export function NetworkStatus(formContext) {

    let carrierObject = Xrm.Page.getAttribute("ipg_carrierid").getValue();

    if (carrierObject != null) {
      let carrierId = carrierObject[0];
      let guid = carrierId.id.replace("{", "").replace("}", "");
      Xrm.WebApi.retrieveRecord("account", guid, "?$select=ipg_contract").then(function success(results) {
        var contracted = results["ipg_contract@OData.Community.Display.V1.FormattedValue"];

        if (contracted == "Yes") {
          formContext.getAttribute("ipg_carrier_network_status").setValue(427880000);
        }
        else {
          formContext.getAttribute("ipg_carrier_network_status").setValue(427880001);
        }

      },
        function (error) {
        });
    }
  }


  /**
 * Called on save form
 * @function Intake.Entitlement.OnSaveForm
 * @returns {void}
*/
  export function OnSaveForm(executionContext) {
    let formContext = executionContext.getFormContext();
    SetEntitlementName(formContext);
  }

  function SetEntitlementName(formContext) {
    let carrierName = formContext.getAttribute("ipg_carrierid")?.getValue()[0]?.name;
    let facilityName = formContext.getAttribute("ipg_facilityid")?.getValue()[0]?.name;
    let entitlementName = " - ";
    if (!carrierName || !facilityName) {
      entitlementName = "";
    }
    if (carrierName) {
      entitlementName = carrierName.concat(entitlementName);
    }
    if (facilityName) {
      entitlementName += facilityName;
    }
    formContext.getAttribute("name").setValue(entitlementName);
  }

  /**
  * Removes unused by carrier plan types
  * @function Intake.Entitlement.RemoveUnusedByCarrierPlanTypes
  * @returns {void}
  */
  var removedOptions: object[] = [];
  function RemoveUnusedByCarrierPlanTypes(formContext) {
    var carrierAttr = formContext.getAttribute("ipg_carrierid");
    var fieldName = "ipg_excludedplantypemultiselect";

    if (carrierAttr) {
      var carrier = formContext.getAttribute("ipg_carrierid").getValue();
      if (carrier) {
        Xrm.WebApi.retrieveRecord("account", carrier[0].id, "?$select=ipg_carriersupportedplantypes").then(
          function success(result) {
            var supportedPlanTypes: string = result.ipg_carriersupportedplantypes;
            if (supportedPlanTypes) {
              var currentPlanType = Number(formContext.getAttribute(fieldName).getValue());
              var array = supportedPlanTypes.split(",");
              for (var i = 0; i < formContext.getAttribute(fieldName).getOptions().length; i++) {
                var value = formContext.getAttribute(fieldName).getOptions()[i].value;
                if ((currentPlanType != value) && (array.indexOf(String(value)) == -1)) {
                  formContext.getControl(fieldName).removeOption(Number(value));
                  removedOptions.push(formContext.getAttribute(fieldName).getOptions()[i]);
                }
              }
            }
          }
        );
      }
    }
  }

  /**
  * Called on Facility Change event
  * @function Intake.Entitlement.OnChangeFacility
  * @returns {void}
*/
  export function OnChangeFacility(executionContext) {
    var formContext = executionContext.getFormContext();

    var facility = formContext.getAttribute("ipg_facilityid").getValue();
    if (facility) formContext.getControl("ipg_facilityid").setDisabled(true);
  }

  /**
    * Called on Carrier Change event
    * @function Intake.Entitlement.OnChangeCarrier
    * @returns {void}
  */
  export function OnChangeCarrier(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getAttribute("customerid").setValue(formContext.getAttribute("ipg_carrierid").getValue());

    var carrier = formContext.getAttribute("ipg_carrierid").getValue();
    if (carrier) formContext.getControl("ipg_carrierid").setDisabled(true);
  }

}
