/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.ZipCodeLookup
   * @returns {void}
  */
  export function ZipCodeLookup(executionContext, zipCodeFieldKey: string, zipCodeLookupFieldKey = "none", cityFieldKey: string, stateFieldKey: string, entityType: string, useAbbreviation = true, zipCodeSelectorKey: string, countryFieldKey = "none"): void {
    let formContext = executionContext.getFormContext();
    const zipCodeattribute: Xrm.Attributes.StringAttribute = formContext.getAttribute(zipCodeFieldKey);
    const cityAttribute: Xrm.Attributes.StringAttribute = formContext.getAttribute(cityFieldKey);
    const stateAttribute: Xrm.Attributes.Attribute = formContext.getAttribute(stateFieldKey);
    const countryAttribute: Xrm.Attributes.StringAttribute = formContext.getAttribute(countryFieldKey);

    //Soumitra: Please do not remove/change this line of code. The reason why this attribute(zipCodeattribute) is null
    //is because the field passed from the form load event does not exists on the form anymore. 
   // const zipCodeAttributeName = zipCodeattribute.getName();
    var zipCodeAttributeName;
    if (zipCodeattribute) {
       zipCodeAttributeName = zipCodeattribute.getName();
    }

    const zipCodeSelectorAttribute: Xrm.Attributes.StringAttribute = formContext.getAttribute(zipCodeSelectorKey);


    // Listener.
    function onChange(): void {
      let zip = formContext.getAttribute(zipCodeAttributeName).getValue(); 
      if (zip) {
        Xrm.WebApi.retrieveRecord('ipg_zipcode', zip[0].id, '?$expand=ipg_StateId($select=ipg_name,ipg_abbreviation),ipg_stateid2($select=ipg_name,ipg_abbreviation),ipg_stateid3($select=ipg_name,ipg_abbreviation),ipg_stateid4($select=ipg_name,ipg_abbreviation),ipg_stateid5($select=ipg_name,ipg_abbreviation),ipg_stateid6($select=ipg_name,ipg_abbreviation),ipg_stateid7($select=ipg_name,ipg_abbreviation),ipg_stateid8($select=ipg_name,ipg_abbreviation)').then((response): void => {
          if (response) {
            if (response.ipg_city2) {
              let optionset = formContext.getControl(zipCodeSelectorKey);
              optionset.clearOptions();
              var city = { value: 923720000, text: JoinCityState(response.ipg_city, response.ipg_StateId)};
              var city2 = { value: 923720001, text: JoinCityState(response.ipg_city2, response.ipg_stateid2) };
              optionset.addOption(city);
              optionset.addOption(city2);
              formContext.getAttribute(zipCodeSelectorKey).setValue(923720000);
              onChangeZipCodeSelector(false);
              if (response.ipg_city3) {
                var city3 = { value: 923720002, text: JoinCityState(response.ipg_city3, response.ipg_stateid3) };
                optionset.addOption(city3);
              }
              if (response.ipg_city4) {
                var city4 = { value: 923720003, text: JoinCityState(response.ipg_city4, response.ipg_stateid4) };
                optionset.addOption(city4);
              }
              if (response.ipg_city5) {
                var city5 = { value: 923720004, text: JoinCityState(response.ipg_city5, response.ipg_stateid5) };
                optionset.addOption(city5);
              }
              if (response.ipg_city6) {
                var city6 = { value: 923720005, text: JoinCityState(response.ipg_city6, response.ipg_stateid6) };
                optionset.addOption(city6);
              }
              if (response.ipg_city7) {
                var city7 = { value: 923720006, text: JoinCityState(response.ipg_city7, response.ipg_stateid7) };
                optionset.addOption(city7);
              }
              if (response.ipg_city8) {
                var city8 = { value: 923720007, text: JoinCityState(response.ipg_city8, response.ipg_stateid8) };
                optionset.addOption(city8);
              }
              
              optionset.setVisible(true);

              formContext.getControl(zipCodeSelectorKey).addNotification({ messages: ['Zip code links to more than one city'], notificationLevel: 'RECOMMENDATION', uniqueId: zipCodeSelectorKey });
            }
            else {
              // formContext.getControl(zipCodeSelectorKey).setVisible(false);
              cityAttribute.setValue(response.ipg_city);

              if (countryAttribute)
                countryAttribute.setValue(response.ipg_county);

              if (stateAttribute.getAttributeType() == "lookup") {
                let lookUp: Xrm.LookupValue =
                {
                  id: response.ipg_StateId.ipg_stateid,
                  name: response.ipg_StateId.ipg_name,
                  entityType: "ipg_state"
                };
                stateAttribute.setValue([lookUp]);
              }
              else if (useAbbreviation)
                stateAttribute.setValue(response.ipg_StateId.ipg_abbreviation);
              else
                stateAttribute.setValue(response.ipg_StateId.ipg_name);
            }
          }
        })
      }
    }

    function JoinCityState(city:string, state?:{ipg_name:string}):string
    {
      return `${city}${state?.ipg_name ? ', ' + state.ipg_name : ""}`
    }

    // Listener.
    function onChangeZipCodeSelector(clearNotification: boolean = false): void {
      let zip = formContext.getAttribute(zipCodeFieldKey).getValue();
      let selector = formContext.getAttribute(zipCodeSelectorKey)?.getValue();

      if (selector) {
        Xrm.WebApi.retrieveRecord('ipg_zipcode', zip[0].id, '?$expand=ipg_StateId($select=ipg_name,ipg_abbreviation),ipg_stateid2($select=ipg_name,ipg_abbreviation),ipg_stateid3($select=ipg_name,ipg_abbreviation),ipg_stateid4($select=ipg_name,ipg_abbreviation),ipg_stateid5($select=ipg_name,ipg_abbreviation),ipg_stateid6($select=ipg_name,ipg_abbreviation),ipg_stateid7($select=ipg_name,ipg_abbreviation),ipg_stateid8($select=ipg_name,ipg_abbreviation)').then((response): void => {
          if (response) {
            if (clearNotification) {
              formContext.getControl(zipCodeSelectorKey).clearNotification(zipCodeSelectorKey); 
            }

            let city = response.ipg_city;
            let state = response.ipg_StateId;
            switch (selector) {
              case 923720001: {
                city = response.ipg_city2;
                state = response.ipg_stateid2;
                break;
              }
              case 923720002: {
                city = response.ipg_city3;
                state = response.ipg_stateid3;
                break;
              }
              case 923720003: {
                city = response.ipg_city4;
                state = response.ipg_stateid4;
                break;
              }
              case 923720004: {
                city = response.ipg_city5;
                state = response.ipg_stateid5;
                break;
              }
              case 923720005: {
                city = response.ipg_city6;
                state = response.ipg_stateid6;
                break;
              }
              case 923720006: {
                city = response.ipg_city7;
                state = response.ipg_stateid7;
                break;
              }
              case 923720007: {
                city = response.ipg_city8;
                state = response.ipg_stateid8;
                break;
              }
            }

            cityAttribute.setValue(city);

            if (stateAttribute.getAttributeType() == "lookup") {
              let lookUp: Xrm.LookupValue =
              {
                id: state.ipg_stateid,
                name: state.ipg_name,
                entityType: "ipg_state"
              };
              stateAttribute.setValue([lookUp]);
            }
            else if (useAbbreviation)
              stateAttribute.setValue(state.ipg_abbreviation);
            else
              stateAttribute.setValue(state.ipg_name);
          }
        })
      }
    }

      zipCodeattribute?.addOnChange(onChange);
      zipCodeSelectorAttribute?.addOnChange(function () { onChangeZipCodeSelector(true); });
  }

  /**
     * Called when the form is saved. See D365 configuration for details.
     * @function Intake.Utility.ZipCodeSave
     * @returns {void}
    */
  export function ZipCodeSave(executionContext, zipCodeSelectorKey: string): void {
    let formContext = executionContext.getFormContext();
    if (zipCodeSelectorKey != 'ipg_insuredzipcodeidselector') {
      var selector = formContext.getAttribute(zipCodeSelectorKey);
      if (selector) {
        formContext.getControl(zipCodeSelectorKey).clearNotification(zipCodeSelectorKey);
      }
    }
  }
}

