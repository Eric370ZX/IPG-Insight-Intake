/**
 * @namespace Intake.ClaimConfiguration
 */
namespace Intake.ClaimConfiguration {
  /**
   * Called on load form
   * @function Intake.ClaimConfiguration.OnLoad
   * @returns {void}
  */
  export function OnLoad(executionContext) {
    let formContext = executionContext.getFormContext();
    RemoveSubEvents(formContext);
  }

  /**
  * Removes unused claim subevents
  * @function Intake.ClaimConfiguration.RemoveSubEvents
  * @returns {void}
  */
  var removedOptions: object[] = [];
  function RemoveSubEvents(formContext) {
    var event = formContext.getAttribute("ipg_claimevent").getValue();
    if (event) {
      var subeventControl: Xrm.Controls.Control = formContext.getControl("ipg_claimsubevent");
      if (subeventControl) {
        var query = "?$filter=ipg_claimevent eq " + event;
        Xrm.WebApi.online.retrieveMultipleRecords("ipg_claimeventsmapping", query).then(
          function success(results) {
            if (results.entities.length > 0) {
              for (var i = 0; i < results.entities.length; i++) {
                var subeventList = results.entities[0].ipg_claimsubevent;
                if (subeventList) {
                  var subeventsString: string[] = subeventList.split(",");
                  var subevents: number[] = [];
                  subeventsString.forEach(function (value) { subevents.push(parseInt(value)) });
                  let subeventOptions: Xrm.OptionSetValue[] = subeventControl.getOptions();
                  for (let i: number = 0; i < subeventOptions.length; i++) {
                    if ((subevents.findIndex(x => x == subeventOptions[i].value) < 0) && !isNaN(subeventOptions[i].value)) {
                      subeventControl.removeOption(subeventOptions[i].value);
                      removedOptions.push(subeventOptions[i]);
                    }
                  }
                }
                else
                  ClearSubEvents(subeventControl);
              }
            }
            else
              ClearSubEvents(subeventControl);
          },
          function (error) {
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
    let subeventOptions: Xrm.OptionSetValue[] = subeventControl.getOptions();
    for (let i: number = 0; i < subeventOptions.length; i++) {
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
  export function OnClaimEventChange(executionContext) {
    let formContext = executionContext.getFormContext();

    var subeventControl: Xrm.Controls.Control = formContext.getControl("ipg_claimsubevent");
    removedOptions.forEach(function (value) {
      subeventControl.addOption(value);
    });
    removedOptions = [];

    RemoveSubEvents(formContext);
  }

}
