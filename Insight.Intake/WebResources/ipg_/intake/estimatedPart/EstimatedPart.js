/**
 * @namespace Intake.EstimatedPart
 */
var Intake;
(function (Intake) {
    var EstimatedPart;
    (function (EstimatedPart) {
        /**
         * Called on load form
         * @function Intake.EstimatedPart.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            OnChangeQuantityImplanted(formContext);
        }
        EstimatedPart.OnLoadForm = OnLoadForm;
        /**
           * OnChange handler.
           * @function Intake.EstimatedPart.OnChangeHandler
           * @returns {void}
           */
        function OnChangeQuantityImplanted(formContext) {
            var quantityImplantedAttribute = formContext.getAttribute("ipg_quantity");
            quantityImplantedAttribute.addOnChange(function () { checkMaxValueOnPart(formContext); });
            quantityImplantedAttribute.fireOnChange();
        }
        /**
          * Called on change Part Override
          * @function Intake.EstimatedPart.OnChangePartOverride
          * @returns {void}
        */
        function OnChangePartOverride(executionContext) {
            var formContext = executionContext.getFormContext();
            IsPartUsed(formContext);
        }
        EstimatedPart.OnChangePartOverride = OnChangePartOverride;
        /**
        * Shows warning if a part is already used in a case
        * @function Intake.EstimatedPart.IsPartUsed
        * @returns {void}
        */
        function IsPartUsed(formContext) {
            var product = formContext.getAttribute("ipg_productid").getValue();
            var incident = formContext.getAttribute("ipg_caseid").getValue();
            formContext.getControl('ipg_productid').clearNotification(IsPartUsed.name);
            if (product && incident) {
                Xrm.WebApi.retrieveMultipleRecords("ipg_estimatedcasepartdetail", "?$select=ipg_estimatedcasepartdetailid&$filter=_ipg_caseid_value eq " + Intake.Utility.removeCurlyBraces(incident[0].id) + " and  _ipg_productid_value eq " + Intake.Utility.removeCurlyBraces(product[0].id) + " and statecode eq 0").then(function success(results) {
                    if (results.entities.length) {
                        formContext.getControl("ipg_productid").setNotification("The part is already used in this case", IsPartUsed.name);
                    }
                }, function (error) {
                    Xrm.Navigation.openAlertDialog({ text: error.message });
                });
            }
        }
        /**
          * Check max quantity restriction of product
          * @function Intake.EstimatedPart.checkMaxValueOnPart
          * @returns {void}
        */
        function checkMaxValueOnPart(formContext) {
            var partLookupAttributeValue = formContext.getAttribute("ipg_productid").getValue();
            var quantityAttributeValue = formContext.getAttribute("ipg_quantity").getValue();
            if (partLookupAttributeValue) {
                Xrm.WebApi.retrieveRecord(partLookupAttributeValue[0].entityType, partLookupAttributeValue[0].id, "?$select=ipg_enforcemaxquantity,ipg_maxquantity").then(function (product) {
                    if (product.ipg_maxquantity && quantityAttributeValue > product.ipg_maxquantity) {
                        var alertStrings = {
                            confirmButtonLabel: "Ok",
                            text: "Quantity Implanted is exceeding the Max Quantity Per Case value. Max value is: " + product.ipg_maxquantity,
                            title: "Warning"
                        };
                        var alertOptions = { height: 120, width: 260 };
                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                        if (product.ipg_enforcemaxquantity) {
                            formContext.getAttribute('ipg_quantity').setValue(0);
                        }
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
    })(EstimatedPart = Intake.EstimatedPart || (Intake.EstimatedPart = {}));
})(Intake || (Intake = {}));
