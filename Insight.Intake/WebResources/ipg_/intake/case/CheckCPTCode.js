/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        /**
        * Shows warning if CPT code is unsupported
        * @function Intake.Case.CheckCPTCode
        * @returns {void}
        */
        function CheckCPTCode(executionContext) {
            var formContext = executionContext.getFormContext();
            var attribute = executionContext.getEventSource();
            var fieldName = attribute.getName();
            var CPTCode = formContext.data.entity.attributes.getByName(fieldName).getValue();
            if (CPTCode) {
                Xrm.WebApi.retrieveRecord("ipg_cptcode", CPTCode[0].id, "?$select=ipg_name,ipg_supportedcpt").then(function success(result) {
                    if (result.ipg_supportedcpt == false)
                        formContext.getControl(attribute.getName()).setNotification("The CPT code " + result.ipg_name + " is unsupported", fieldName);
                    else
                        formContext.getControl(attribute.getName()).clearNotification(fieldName);
                });
            }
            else
                formContext.getControl(attribute.getName()).clearNotification(fieldName);
        }
        //deprecated  CPI-18406
        function CPTCodeOnChange(executionContext) {
            //CheckCPTCode(executionContext);
        }
        Case.CPTCodeOnChange = CPTCodeOnChange;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
