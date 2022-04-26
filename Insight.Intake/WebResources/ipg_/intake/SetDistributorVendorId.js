/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        /**
         * Called on Distributor Name change
         * @function Intake.Utility.SetDistributorVendorId
         * @returns {void}
        */
        function SetDistributorVendorId(executionContext) {
            var formContext = executionContext.getFormContext();
            var distributorNameField = executionContext.getEventSource().getName();
            if (distributorNameField) {
                var distributorNameAttribute = formContext.getAttribute(distributorNameField);
                if (distributorNameAttribute) {
                    var distributorNameArray = distributorNameAttribute.getValue().split(" ");
                    var vendorID_1 = "D";
                    for (var _i = 0, distributorNameArray_1 = distributorNameArray; _i < distributorNameArray_1.length; _i++) {
                        var distName = distributorNameArray_1[_i];
                        vendorID_1 = vendorID_1.concat(distName.charAt(0));
                    }
                    if (vendorID_1) {
                        Xrm.WebApi.online.retrieveMultipleRecords("account", "?$select=ipg_vendoridnumber&$filter=startswith(ipg_vendoridnumber," + "'" + vendorID_1 + "'" + ")&$orderby=ipg_vendoridnumber asc").then(function success(results) {
                            //if there are more than one matches, order by the id number and then increment the integer suffix.
                            if (results && results.entities && results.entities.length > 1) {
                                var largestVendorID = results.entities[results.entities.length - 1].ipg_vendoridnumber;
                                var splitVendorId = largestVendorID.split(/(\d+)/).filter(Boolean);
                                if (splitVendorId.length > 1) {
                                    var vendorIDNumberSegment = splitVendorId[1];
                                    if (!isNaN(Number(vendorIDNumberSegment))) {
                                        var incremented = (Number(vendorIDNumberSegment)) + 1;
                                        vendorID_1 = vendorID_1 + String(incremented);
                                    }
                                }
                            }
                            //if there is only one match, this is the first duplicate
                            //so append a 1
                            else if (results && results.entities && results.entities.length === 1) {
                                vendorID_1 = vendorID_1 + "1";
                            }
                            //otherwise, just go ahead and set the vendor id.
                            var vendorIdField = formContext.getControl("ipg_vendoridnumber").getName();
                            if (vendorIdField) {
                                var vendorIdAttribute = formContext.getAttribute(vendorIdField);
                                if (vendorIdAttribute) {
                                    vendorIdAttribute.setValue(vendorID_1.toUpperCase());
                                }
                            }
                        }, function (error) {
                            Xrm.Navigation.openErrorDialog(error);
                        });
                    }
                    var vendorIdField = formContext.getControl("ipg_vendoridnumber").getName();
                    if (vendorID_1) {
                        var vendorIdAttribute = formContext.getAttribute(vendorIdField);
                        if (vendorIdAttribute) {
                            vendorIdAttribute.setValue(vendorID_1.toUpperCase());
                        }
                    }
                }
            }
        }
        Utility.SetDistributorVendorId = SetDistributorVendorId;
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
