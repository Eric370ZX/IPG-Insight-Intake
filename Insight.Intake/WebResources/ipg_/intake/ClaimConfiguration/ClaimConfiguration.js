/**
 * @namespace Intake.ClaimConfiguration
 */
var Intake;
(function (Intake) {
    var ClaimConfiguration;
    (function (ClaimConfiguration) {
        /**
         * Called on load form
         * @function Intake.ClaimConfiguration.OnLoad
         * @returns {void}
        */
        function OnLoad(executionContext) {
            var formContext = executionContext.getFormContext();
            RemoveSubEvents(formContext);
        }
        ClaimConfiguration.OnLoad = OnLoad;
        /**
        * Removes unused claim subevents
        * @function Intake.ClaimConfiguration.RemoveSubEvents
        * @returns {void}
        */
        var removedOptions = [];
        function RemoveSubEvents(formContext) {
            var event = formContext.getAttribute("ipg_claimevent").getValue();
            if (event) {
                var subeventControl = formContext.getControl("ipg_claimsubevent");
                if (subeventControl) {
                    var query = "?$filter=ipg_claimevent eq " + event;
                    Xrm.WebApi.online.retrieveMultipleRecords("ipg_claimeventsmapping", query).then(function success(results) {
                        if (results.entities.length > 0) {
                            var _loop_1 = function () {
                                subeventList = results.entities[0].ipg_claimsubevent;
                                if (subeventList) {
                                    subeventsString = subeventList.split(",");
                                    subevents = [];
                                    subeventsString.forEach(function (value) { subevents.push(parseInt(value)); });
                                    var subeventOptions_1 = subeventControl.getOptions();
                                    var _loop_2 = function (i_1) {
                                        if ((subevents.findIndex(function (x) { return x == subeventOptions_1[i_1].value; }) < 0) && !isNaN(subeventOptions_1[i_1].value)) {
                                            subeventControl.removeOption(subeventOptions_1[i_1].value);
                                            removedOptions.push(subeventOptions_1[i_1]);
                                        }
                                    };
                                    for (var i_1 = 0; i_1 < subeventOptions_1.length; i_1++) {
                                        _loop_2(i_1);
                                    }
                                }
                                else
                                    ClearSubEvents(subeventControl);
                            };
                            var subeventList, subeventsString, subevents;
                            for (var i = 0; i < results.entities.length; i++) {
                                _loop_1();
                            }
                        }
                        else
                            ClearSubEvents(subeventControl);
                    }, function (error) {
                        console.log(error.message);
                    });
                }
            }
        }
        /*
         * Clear SubEvents
         * @function Intake.ClaimConfiguration.ClearSubEvents
         * @returns {void}
         */
        function ClearSubEvents(subeventControl) {
            var subeventOptions = subeventControl.getOptions();
            for (var i = 0; i < subeventOptions.length; i++) {
                if (!isNaN(subeventOptions[i].value)) {
                    subeventControl.removeOption(subeventOptions[i].value);
                    removedOptions.push(subeventOptions[i]);
                }
            }
        }
        /*
         * Called on change of claim event
         * @function Intake.ClaimConfiguration.OnClaimEventChange
         * @returns {void}
         */
        function OnClaimEventChange(executionContext) {
            var formContext = executionContext.getFormContext();
            var subeventControl = formContext.getControl("ipg_claimsubevent");
            removedOptions.forEach(function (value) {
                subeventControl.addOption(value);
            });
            removedOptions = [];
            RemoveSubEvents(formContext);
        }
        ClaimConfiguration.OnClaimEventChange = OnClaimEventChange;
    })(ClaimConfiguration = Intake.ClaimConfiguration || (Intake.ClaimConfiguration = {}));
})(Intake || (Intake = {}));
