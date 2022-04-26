/**
 * @namespace Intake.CasePartDetail
 */
 namespace Intake.CasePartDetail {

  export enum order_statecode
  {
    Closed = 3
  }

  export enum order_statuscode
  {
    InvoiceReceived = 923720006,
    InReview = 923720007,
    VerifiedForPayment = 923720008,
    Partial = 923720010
  }

  /**
   * Called on load quick create form
   * @function Intake.CasePartDetail.OnLoadQuickCreateForm
   * @returns {void}
  */
  export function OnLoadQuickCreateForm(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    RunEBVOnCreate(formContext);

    if (formContext.ui.getFormType() === 1) {
      formContext.getAttribute("ipg_quantity").setValue(1);
      formContext.getAttribute("ipg_quantity").setRequiredLevel("required");
    }
    SetAvailablePOTypes(formContext);

    const potypeAttr = formContext.getAttribute("ipg_potypecode");
    potypeAttr?.addOnChange(OnPoTypeChange);
    potypeAttr?.fireOnChange();
  }
  /**
   * Called on load form
   * @function Intake.CasePartDetail.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();

    RunEBVOnCreate(formContext);

    formContext.getAttribute("ipg_quantity").setRequiredLevel("required");
    SetAvailablePOTypes(formContext);

    const potypeAttr = formContext.getAttribute("ipg_potypecode");
    potypeAttr?.addOnChange(OnPoTypeChange);
    potypeAttr?.fireOnChange();

    ShowNotificationIfEnteredUnitCostGreaterThanMSRP(formContext);
    SetPOTypeIfConvertedPart(formContext);
    
    LockActualPart(formContext);    

    formContext.getAttribute("ipg_quantity").fireOnChange();
  }
  /**
    * Called on change EnteredUnitCost
    * @function Intake.CasePartDetail.OnChangeEnteredUnitCost
    * @returns {void}
  */
  export function OnChangeEnteredUnitCost(executionContext) {
    let formContext = executionContext.getFormContext();
    ShowNotificationIfEnteredUnitCostGreaterThanMSRP(formContext);
  }
  /**
    * Called on change Quantity Implanted
    * @function Intake.CasePartDetail.OnChangeQuantityImplanted
    * @returns {void}
  */
  export function OnChangeQuantityImplanted(executionContext) {
    let formContext = executionContext.getFormContext();
    checkMaxValueOnPart(formContext);
  }

  /**
    * Called on change Part Override
    * @function Intake.CasePartDetail.OnChangePartOverride
    * @returns {void}
  */
  export async function OnChangePartOverride(executionContext) {
    let formContext = executionContext.getFormContext();
    CheckPartStatus(formContext);
    CheckPartSupported(formContext);
    CheckPartQuantity(formContext);
    if (defaultPOTypeOptions) {
      ResetPOTypeOptions(formContext);
    }
    SetAvailablePOTypes(formContext);
    //await ShowNewPriceListMessage(formContext);
    IsPartUsed(formContext);
  }

  /**
    * Called on Form Save event
    * @function Intake.CasePartDetail.OnSaveForm
    * @returns {void}
  */
  export function OnSaveForm(executionContext: Xrm.Events.SaveEventContext) {
    prohibitUnsupportedParts(executionContext);
  }

  /**
	 * Called on change Case
    * @function Intake.CasePartDetail.OnChangeCase
    * @returns {void}
  */
  export function OnChangeCase(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    if (defaultPOTypeOptions) {
      ResetPOTypeOptions(formContext);
    }
    SetAvailablePOTypes(formContext);
  }

  /**					   												 	 
    * Called on Form Save event. Checks if the chosen part is unsupported by the carrier or not
    * based on ipg_unsupportedpart table
    * @function prohibitUnsupportedParts
    * @returns {void}
  */
  let skipUnsupportedPartCheck: boolean = false;
  function prohibitUnsupportedParts(executionContext: Xrm.Events.SaveEventContext) {
    //debugger;

    if (skipUnsupportedPartCheck) { //we need this check because formContext.data.entity.save triggers this handler again
      skipUnsupportedPartCheck = false;
      return;
    }

    //disable saving before the async calls below because preventDefault does not work from async calls
    let saveEventArgs: Xrm.Events.SaveEventArguments = executionContext.getEventArgs();
    saveEventArgs.preventDefault();

    let formContext: Xrm.FormContext = executionContext.getFormContext();
    let caseLookupAttributeValue: Xrm.LookupValue[] = formContext.getAttribute("ipg_caseid").getValue();

    Xrm.WebApi.retrieveRecord(caseLookupAttributeValue[0].entityType, caseLookupAttributeValue[0].id, "?$select=incidentid&$expand=ipg_CarrierId($select=accountid),ipg_HomePlanCarrierId($select=accountid)").then(function (caseEntity) {

      if (!caseEntity.ipg_CarrierId || !caseEntity.ipg_CarrierId.accountid) {
        Xrm.Navigation.openErrorDialog({ message: 'Error! Could not find Case Primary Carrier and consequently check Unsupported parts' });
        return;
      }

      let productLookupAttributeValue: Xrm.LookupValue[] = formContext.getAttribute("ipg_productid").getValue();
      let todayDateFormatted: string = Intake.Utility.FormatODataDate(new Date()/*today*/);

      //build filter expression
      let filter: string = `?$filter=ipg_ProductId/productid eq ${Intake.Utility.removeCurlyBraces(productLookupAttributeValue[0].id)}` +
        ` and ipg_effectivedate le ${todayDateFormatted} and statecode eq 0 and (ipg_CarrierId/accountid eq ${caseEntity.ipg_CarrierId.accountid}`;
      if (caseEntity.ipg_HomePlanCarrierId && caseEntity.ipg_HomePlanCarrierId.accountid) {
        filter += ` or ipg_CarrierId/accountid eq ${caseEntity.ipg_HomePlanCarrierId.accountid}`;
      }
      filter += ')';

      //check if unsupported parts exist
      Xrm.WebApi.retrieveMultipleRecords('ipg_unsupportedpart', filter + '&$select=ipg_unsupportedpartid').then(function (unsupportedParts) {
        const notificationId: string = 'unsupportedPartError';
        if (unsupportedParts.entities.length) {
          formContext.ui.setFormNotification('This part is unsupported by the carrier (primary or home plan)', 'ERROR', notificationId);
        }
        else {
          formContext.ui.clearFormNotification(notificationId);
          skipUnsupportedPartCheck = true;
          formContext.data.entity.save();
        }
      }, function (error) {
        Xrm.Navigation.openErrorDialog({ message: error.message });
      });

    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });
  }
  /**
    * Calculate Total value
    * @function Intake.CasePartDetail.CalculateTotal
    * @returns {void}
  */
  export function CalculateTotal(executionContext) {
    let formContext = executionContext.getFormContext();
    let quantityAttributeValue: number = formContext.getAttribute("ipg_quantity").getValue();
    let enteredUnitCostAttributeValue: number = formContext.getAttribute("ipg_enteredunitcost").getValue();
    let enteredTaxAttributeValue: number = formContext.getAttribute("ipg_enteredtax").getValue();
    let enteredShippingAttributeValue: number = formContext.getAttribute("ipg_enteredshipping").getValue();

    let total: number = ((enteredUnitCostAttributeValue || 0) + (enteredTaxAttributeValue || 0) + (enteredShippingAttributeValue || 0)) * (quantityAttributeValue || 0);

    formContext.getAttribute("ipg_totaljscalculated").setValue(total);
  }

  /**
    * show notification if EnteredUnitCost greater than MSRP more than 10%
    * @function Intake.CasePartDetail.showNotificationIfEnteredUnitCostGreaterThanMSRP
    * @returns {void}
  */
  function ShowNotificationIfEnteredUnitCostGreaterThanMSRP(formContext) {
    let partLookupAttributeValue = formContext.getAttribute("ipg_productid").getValue();
    if (partLookupAttributeValue) {
      Xrm.WebApi.retrieveRecord(partLookupAttributeValue[0].entityType, partLookupAttributeValue[0].id, "?$select=ipg_msrp").then(function (result) {
        if (result.ipg_msrp) {
          let msrp = result.ipg_msrp;
          let enteredUnitCostAttributeValue = formContext.getAttribute("ipg_enteredunitcost").getValue();
          if (msrp * 1.1 < enteredUnitCostAttributeValue) {
            Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "The Entered Unit Cost is greater more than 10%!" });
          }
        }
      }, function (error) {
        Xrm.Navigation.openErrorDialog({ message: error.message });
      });
    }
  }
  /**
    * check max quantity restriction of product
    * @function Intake.CasePartDetail.checkMaxValueOnPart
    * @returns {void}
  */
  function checkMaxValueOnPart(formContext) {
    let partLookupAttributeValue = formContext.getAttribute("ipg_productid").getValue();
    let quantityAttributeValue = formContext.getAttribute("ipg_quantity").getValue();
        
    if (partLookupAttributeValue) {
      Xrm.WebApi.retrieveRecord(partLookupAttributeValue[0].entityType, partLookupAttributeValue[0].id, "?$select=ipg_enforcemaxquantity,ipg_maxquantity").then(function (product) {        
        if (product.ipg_maxquantity && quantityAttributeValue  > product.ipg_maxquantity) {   
          let alertStrings = { 
            confirmButtonLabel: "Ok",
            text: "Quantity Implanted is exceeding the Max Quantity Per Case value. Max value is: " + product.ipg_maxquantity,
            title: "Warning" };
          let alertOptions = { height: 120, width: 260 };                    
          if (product.ipg_enforcemaxquantity) {                
            formContext.getAttribute('ipg_quantity').setValue(0);  
            alertStrings.title = "Error";
          }                 
          Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
        }        
      }, function (error) {
        Xrm.Navigation.openErrorDialog({ message: error.message });
      });
    }
  }

  /**
  * Shows warning if part approval status is pending
  * @function Intake.CasePartDetail.CheckPartStatus
  * @returns {void}
  */
  function CheckPartStatus(formContext) {
    let product = formContext.getAttribute("ipg_productid").getValue();
    if (product) {
      Xrm.WebApi.retrieveRecord("product", product[0].id, "?$select=name,ipg_status").then(
        function success(result) {
          //when form on Create stage Product status pending - disable PO type and don't show notification
          if (formContext.ui.getFormType() == 1 && result.ipg_status == 923720001) {
            formContext.getAttribute("ipg_potypecode").setRequiredLevel("none");
          }
          else {
            if (result.ipg_status == 923720001)
              formContext.getControl("ipg_productid").setNotification("The part " + result.name + " hasn't been approved yet", "PartNotification");
            else formContext.getControl("ipg_productid").clearNotification("PartNotification");
          }
        }
      );
    }
    else
      Xrm.Page.ui.clearFormNotification("PartNotification");
  }

  /**
  * Shows warning if a part is not supported by Carrier
  * @function Intake.CasePartDetail.CheckPartSupported
  * @returns {void}
  */
  function CheckPartSupported(formContext) {
    let product = formContext.getAttribute("ipg_productid").getValue();
    let caseid = formContext.getAttribute("ipg_caseid").getValue();
    formContext.getControl('ipg_productid').clearNotification("unsupportedpart");
    Xrm.WebApi.retrieveRecord("incident", caseid[0].id, "?$select=_ipg_carrierid_value").then(function (result) {
      let carrier = result._ipg_carrierid_value;
      if (product && carrier) {
        var query = "?$select=ipg_unsupportedpartid&$filter=_ipg_productid_value eq '" + Intake.Utility.removeCurlyBraces(product[0].id) + "' and _ipg_carrierid_value eq '" + carrier + "'";
        Xrm.WebApi.retrieveMultipleRecords("ipg_unsupportedpart", query)
          .then((result) => {
            if (result.entities.length > 0) {
              Xrm.WebApi.retrieveRecord("account", carrier, "?$select=name").then((result) => {
                formContext.getControl('ipg_productid').setNotification("This part is not supported by carrier " + result.name, "unsupportedpart");
              },
                function (error) {
                  Xrm.Navigation.openErrorDialog({ message: error.message });
                }
              );
            }
          },
            function (error) {
              Xrm.Navigation.openErrorDialog({ message: error.message });
            }
          );
      }
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });
  }

  /**
  * set available PO Types by Surgery Date and DTM Member of Facility
  * @function Intake.CasePartDetail.SetAvailablePOTypes
  * @returns {void}
  */
  let defaultPOTypeOptions = null;
  function SetAvailablePOTypes(formContext: Xrm.FormContext) {
    let caseValue: Array<Xrm.LookupValue> = formContext.getAttribute("ipg_caseid").getValue();
    let productValue: Array<Xrm.LookupValue> = formContext.getAttribute("ipg_productid").getValue();
    let poTypepPickList = formContext.getControl<Xrm.Controls.OptionSetControl>("ipg_potypecode");
    
    if (poTypepPickList)
      defaultPOTypeOptions = poTypepPickList.getOptions();
    if (!productValue && poTypepPickList) {
      poTypepPickList.clearOptions();
      return;
    }
    if (poTypepPickList) {
      var currentPOType = Number(formContext.getAttribute("ipg_potypecode").getValue());
      if (currentPOType != 923720003)
        poTypepPickList.removeOption(923720003);
    }
    if (caseValue && caseValue.length && caseValue[0] && caseValue[0].id) {
      Xrm.WebApi.retrieveRecord(caseValue[0].entityType, caseValue[0].id, "?$select=ipg_surgerydate&$expand=ipg_FacilityId($select=ipg_dtmmember,ipg_cpaonlyfacility)")
        .then(
        (caseResult) => {
          Xrm.WebApi.retrieveRecord(productValue[0].entityType, productValue[0].id, "?$select=ipg_boxquantity")
            .then(
            (productResult) => {
              let excludedOptions: Array<number> = null;
              if (caseResult) {
                let dateNow = new Date();
                let currentDate = new Date(dateNow.getFullYear(), dateNow.getMonth(), dateNow.getDate());
                if (caseResult.ipg_surgerydate && new Date(caseResult.ipg_surgerydate) < currentDate) {
                  if (caseResult.ipg_FacilityId && caseResult.ipg_FacilityId.ipg_cpaonlyfacility) {
                    excludedOptions = [923720000, 923720001, 923720003, 923720004];
                  }
                  else if (productResult && productResult.ipg_boxquantity > 1) {
                    excludedOptions = [923720000, 923720001, 923720004];
                  }
                  else {
                    if (caseResult.ipg_FacilityId && caseResult.ipg_FacilityId.ipg_dtmmember)
                    {
                      excludedOptions = [923720000];
                    }
                    else
                    {
                      excludedOptions = [923720000, 923720001, 923720003, 923720004];
                    }
                  }
                }
                else
                {
                  excludedOptions = [923720001, 923720002, 923720003, 923720004];
                }

                if (poTypepPickList && excludedOptions) {
                  var oldvalue = poTypepPickList.getAttribute().getValue();
                  excludedOptions.forEach((item) => {
                    item != oldvalue && poTypepPickList.removeOption(item);
                  });
                }
              }
            })
          });
    }
  }

  function OnPoTypeChange(executionContext: Xrm.Events.EventContext)
  {
    const formContext = executionContext.getFormContext();
    const poTypepPickListVal = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_potypecode")?.getValue();
    const controlledFields = ["ipg_enteredunitcost", "ipg_enteredtax", "ipg_enteredshipping"]

    const disable = !poTypepPickListVal || [923720000,923720004,923720001].indexOf(poTypepPickListVal) > -1; /**TPO,MPO,ZPO */
    controlledFields.forEach(fieldname => formContext.getControl(fieldname)?.setDisabled(disable));
  }

  /**
  * reset PO Type Options
  * @function Intake.CasePartDetail.ResetPOTypeOptions
  * @returns {void}
  */
  function ResetPOTypeOptions(formContext: Xrm.FormContext) {
    let poTypepPickList = formContext.getControl("ipg_potypecode");
    poTypepPickList.clearOptions();
    defaultPOTypeOptions.forEach((item) => {
      poTypepPickList.addOption(item);
    });
  }

  /**
  * Checks if max quantity limit reached
  * @function Intake.CasePartDetail.CheckPartQuantity
  * @returns {void}
  */
  function CheckPartQuantity(formContext) {
    let product = formContext.getAttribute("ipg_productid").getValue();
    let caseid = formContext.getAttribute("ipg_caseid").getValue();
    formContext.getControl('ipg_productid').clearNotification("quantity");
    formContext.ui.clearFormNotification("quantityAllowed");
    if (product) {
      Xrm.WebApi.retrieveRecord("product", product[0].id, "?$select=ipg_enforcemaxquantity,ipg_maxquantity").then(function (result) {
        if ((result.ipg_enforcemaxquantity) && ((result.ipg_maxquantity == 0) || (result.ipg_maxquantity == null)))
          formContext.getControl('ipg_productid').setNotification("You can't add this part because max quanity per case is set to 0", "quantity");
        else if ((result.ipg_enforcemaxquantity) && (result.ipg_maxquantity > 0)) {
          let maxQuantity = result.ipg_maxquantity;
          Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=ipg_caseid/incidentid eq '" + caseid[0].id + "' and ipg_productid/productid eq " + product[0].id).then(function (result) {
            if (result.entities.length >= maxQuantity)
              formContext.getControl('ipg_productid').setNotification("The maximum quantity of this part per case was reached", "quantity");
          }, function (error) {
            Xrm.Navigation.openErrorDialog({ message: error.message });
          });
        }
        else if ((!result.ipg_enforcemaxquantity) && (result.ipg_maxquantity > 0)) {
          let maxQuantity = result.ipg_maxquantity;
          Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=ipg_caseid/incidentid eq '" + caseid[0].id + "' and ipg_productid/productid eq " + product[0].id).then(function (result) {
            if (result.entities.length >= maxQuantity)
              formContext.ui.setFormNotification("The maximum quantity of this part per case was reached, but you can add this part into the case", "WARNING", "quantityAllowed");
          }, function (error) {
            Xrm.Navigation.openErrorDialog({ message: error.message });
          });
        }
      }, function (error) {
        Xrm.Navigation.openErrorDialog({ message: error.message });
      });
    }
  }

  //Commented out because we cannot determine manufacturers for Price List https://eti-ipg.atlassian.net/browse/CPI-3627.
  //This code can be used later when we associate Price List with manufacturers
  ///**
  //* Shows warning if there is a new approved price list
  //* @function Intake.CasePartDetail.ShowNewPriceListMessage
  //* @returns {void}
  //*/
  //async function ShowNewPriceListMessage(formContext: Xrm.FormContext) {
  //  //debugger;

  //  formContext.ui.clearFormNotification("NewPriceList");

  //  let productIdValue = formContext.getAttribute("ipg_productid").getValue();
  //  if (!productIdValue || !productIdValue.length) {
  //    return;
  //  }

  //  //get Global Setting
  //  const settingName: string = 'NEW_PRICE_LIST_MESSAGE_DAYS';
  //  let numberOfDays: number|null = await Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", `?$select=ipg_value&$filter=ipg_name eq '${settingName}'`)
  //    .then((result: Xrm.RetrieveMultipleResult): number|null => {

  //      if (result.entities.length > 0 && result.entities[0].ipg_value) {
  //        return Number(result.entities[0].ipg_value);
  //      }
  //      else {
  //        Xrm.Navigation.openErrorDialog({ message: `${settingName} global setting does not exist or empty` });
  //      }
  //    },
  //      function (error) {
  //        console.log(error.message);
  //      }
  //  );
  //  if (numberOfDays == null || numberOfDays == NaN) {
  //    Xrm.Navigation.openErrorDialog({ message: `Could not parse a number from ${settingName} global setting` });
  //    return;
  //  }

  //  //get Facility
  //  let caseid = formContext.getAttribute("ipg_caseid").getValue();
  //  let facilityId: string = await Xrm.WebApi.retrieveRecord("incident", caseid[0].id, "?$select=_ipg_facilityid_value").then(function (result) {
  //    return result._ipg_facilityid_value;
  //  }, function (error) {
  //    Xrm.Navigation.openErrorDialog({ message: error.message });
  //  });
  //  if (!facilityId) {
  //    return;
  //  }

  //  //get recently approved price lists
  //  //todo: get Manufacturer from the part
  //  //todo: get manufacturers related the price list (how?)
  //  //todo: filter by Manufacturer
  //  const priceListAbbreviation = 'PRL';
  //  const approvedReviewStatusValue: number = 427880001;
  //  let priceLists = await Xrm.WebApi.retrieveMultipleRecords("ipg_document",
  //    `?$filter=(ipg_ipg_document_account/any(f:f/accountid eq '${facilityId}'))` +
  //      ` and ipg_DocumentTypeId/ipg_documenttypeabbreviation eq '${priceListAbbreviation}'` +
  //      ` and ipg_reviewstatus eq ${approvedReviewStatusValue} and Microsoft.Dynamics.CRM.LastXDays(PropertyName=@p1,PropertyValue=@p2)&@p1='ipg_reviewdate'&@p2=${numberOfDays}` +
  //    '&$select=ipg_documentid')
  //    .then((result: Xrm.RetrieveMultipleResult) => {
  //      return result;
  //    },
  //    function (error) {
  //      console.log(error.message);
  //    }
  //  );
  //  if (priceLists.entities.length > 0) {
  //    formContext.ui.setFormNotification('A new Price List is now in effect', 'WARNING', "NewPriceList");
  //  }
  //}

/**
* Runs auto EBV Request from system
* @function Intake.CasePartDetail.RunEBVOnCreate
* @returns {void}
*/
  function RunEBVOnCreate(formContext:Xrm.FormContext)
  {
    const formType = formContext.ui.getFormType();
    if (formType === XrmEnum.FormType.Create || formType === XrmEnum.FormType.QuickCreate)
    {
      let arguments = { IsUserGenerated: false, CarrierNumber: 1 };
      let caseIdField = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_caseid");
      let caseIdValue = caseIdField && caseIdField.getValue();

      if (caseIdValue?.length > 0 && caseIdValue[0].id) {
        Intake.Utility.CallActionProcessAsync("incidents", Intake.Utility.removeCurlyBraces(caseIdValue[0].id), "ipg_IPGCaseActionsVerifyBenefits", arguments);
      }
    }
  }

  export function SetPOTypeIfConvertedPart(formContext: Xrm.FormContext) {

    var casePartDetailID = Xrm.Page.data.entity.getId();

    if (casePartDetailID) {

      Xrm.WebApi.online.retrieveRecord("ipg_casepartdetail", Intake.Utility.removeCurlyBraces(casePartDetailID), "?$select=ipg_convertedfromestimated").then(

        function success(result) {
          var ipg_convertedfromestimated = result["ipg_convertedfromestimated"];

          if (ipg_convertedfromestimated) {
            if (formContext.getControl("ipg_potypecode")) {
              formContext.getControl("ipg_potypecode").setDisabled(true);
            }
          }
        },
        function (error) {
          Xrm.Navigation.openAlertDialog({ text: error.message });
        }
      );
    }
  }

  /**
  * Shows warning if a part is already used in a case
  * @function Intake.CasePartDetail.IsPartUsed
  * @returns {void}
  */
  function IsPartUsed(formContext) {
    let product = formContext.getAttribute("ipg_productid").getValue();
    let incident = formContext.getAttribute("ipg_caseid").getValue();
    formContext.getControl('ipg_productid').clearNotification(IsPartUsed.name);
    if (product && incident) {
      Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=_ipg_caseid_value eq " + Intake.Utility.removeCurlyBraces(incident[0].id) + " and  _ipg_productid_value eq " + Intake.Utility.removeCurlyBraces(product[0].id) + " and statecode eq 0").then(
        function success(results) {
          if (results.entities.length) {
            formContext.getControl("ipg_productid").setNotification("The part is already used in this case", IsPartUsed.name);
          }
        },
        function (error) {
          Xrm.Navigation.openAlertDialog({ text: error.message });
        }
      );
    }
  }

  async function LockActualPart(formContext: Xrm.FormContext)
  {
    const isPartLocked = formContext.getAttribute("ipg_islocked").getValue();
    if (isPartLocked)
    {
      formContext.ui.controls.forEach((control: Xrm.Controls.StandardControl) => {
        if (control && control.getDisabled && !control.getDisabled()) {
          control.setDisabled(true);
        }
      });
    }
  }
  
}