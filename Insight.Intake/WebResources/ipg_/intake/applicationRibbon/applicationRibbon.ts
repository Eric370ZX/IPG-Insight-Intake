namespace Intake.ApplicationRibbon {
    const _duplicateKeyPrefix = "ipg-crm-duplicate";
    const _duplicateSaveEvent = 666;
    const _dateOptions = { year: 'numeric', month: 'numeric', day: 'numeric' };
    const _stringComparisonRegex = /[^a-zA-Z0-9]/g;

    const _duplicateIgnoreAttributes = [
        "address1_addressid",
        "address2_addressid",
        "ipg_externalid",
        "ipg_zirmedid"
    ];

    var _isOnSaveValidationFired = false;

    export function OpenDuplicateForm(primaryControl: Xrm.FormContext, recordId: string, entityName: string) {
        let formParameters = {};
      //Xrm.Page.getAttribute("ipg_effectivedate").setValue(null);
      //Xrm.Page.getAttribute("ipg_expirationdate").setValue(null);
        primaryControl.data.entity.attributes.forEach(x => {
            //if (primaryControl.getControl(x.getName()).getVisible()) {
            if (_duplicateIgnoreAttributes.indexOf(x.getName()) === -1)
                formParameters[x.getName()] = x.getValue();
            //}
        });

        sessionStorage.setItem(`${_duplicateKeyPrefix}-${entityName}`, recordId);

        Xrm.Navigation.navigateTo({
            pageType: "entityrecord",
            entityName: entityName,
            data: formParameters
        }, {
            position: 1,
            target: 1
        });
    }

    export function IsDuplicateEnabled(primaryControl: Xrm.FormContext, recordId: string, entityName: string) {
        const entities = Intake.Configs.DuplicateButton.GetEnabledEntities();
        return entities?.indexOf(entityName) > -1;
    }

    export function SetOnSaveValidation(primaryControl: Xrm.FormContext, recordId: string, entityName: string) {
        if (_isOnSaveValidationFired)
            return false;

        _isOnSaveValidationFired = true;

        async function onSaveDuplicateValidation(executionContext: Xrm.Events.SaveEventContext) {
            const saveMode = executionContext.getEventArgs().getSaveMode();
            if (saveMode === _duplicateSaveEvent) {
                primaryControl.data.entity.removeOnSave(onSaveDuplicateValidation);
                return;
            }

            const originalRecordId = sessionStorage.getItem(`${_duplicateKeyPrefix}-${entityName}`);
            if (originalRecordId) {
                executionContext.getEventArgs().preventDefault();

                const originalRecord = await Xrm.WebApi.retrieveRecord(entityName, originalRecordId);
                const attributes = primaryControl.data.entity.attributes.get();

                let isCompleteMatch = true;

                for (const attr of attributes) {
                    if (_duplicateIgnoreAttributes.indexOf(attr.getName()) > -1)
                        continue;

                    if (!IsFormAttributeEqualToValueFromObject(attr, originalRecord)) {
                        isCompleteMatch = false;
                        break;
                    }
                }

                if (isCompleteMatch) {
                    const alertSettings = {
                        confirmButtonLabel: "Ok",
                        text: "In order to create a record via duplicate functionality you should change at least one field on the form",
                        title: "Record duplication"
                    };

                    const alertOptions = { height: 120, width: 260 };

                    Xrm.Navigation.openAlertDialog(alertSettings, alertOptions);
                }
                else {
                    primaryControl.data.entity.removeOnSave(onSaveDuplicateValidation);
                    sessionStorage.removeItem(`${_duplicateKeyPrefix}-${entityName}`);

                    const saveOptions = {
                        saveMode: _duplicateSaveEvent
                    };

                    primaryControl.data.save(saveOptions);
                }
            }
        }

        primaryControl.data.entity.removeOnSave(onSaveDuplicateValidation);
        primaryControl.data.entity.addOnSave(onSaveDuplicateValidation);

        //This return is required because this function is used as enable rule
        return false;
    }

    export function EmptyFunction() {
        return true;
    }

    export function GoToNextGen(): void {
      //debugger;

      //get CalcRev application environment suffix
      let env: string = getEnvironment();
      let envSuffix: string;
      if (env) {
        envSuffix = '-' + env;
      }
      else {
        envSuffix = '';
      }

      location.href = `https://insight-calcrev${envSuffix}.azurewebsites.net`;
    }

    function IsFormAttributeEqualToValueFromObject(attr: Xrm.Attributes.Attribute, record: any): boolean {
        switch (attr.getAttributeType() as string) {
            case "string":
            case "memo":
                return record[attr.getName()]?.replace(_stringComparisonRegex, '').toLowerCase() === attr.getValue()?.replace(_stringComparisonRegex, '').toLowerCase();
            case "multiselectoptionset": {
                const value: Array<number> = attr.getValue();
                const recordValue: Array<string> = record[attr.getName()]?.split(',');

                if (value && recordValue)
                    return value.every(v => recordValue.indexOf(v.toString()) > -1);
                    
                return !value && !recordValue;
            }
            case "datetime": {
                const attrValue = attr.getValue();
                const apiValue = record[attr.getName()];

                if (attrValue && apiValue)
                    return attrValue.toLocaleDateString('en-US', _dateOptions) === new Date(apiValue).toLocaleDateString('en-US', _dateOptions);

                return !attrValue && !apiValue;
            }
            case "lookup": {
                const apiAttributeName = `_${attr.getName()}_value`;
                const lookupValue = attr.getValue();

                if (lookupValue && lookupValue.length > 0) {
                    const id = lookupValue[0].id.replace("}", "").replace("{", "").toLowerCase();

                    return id === record[apiAttributeName];
                }

                return !lookupValue && !record[apiAttributeName];
            }
            default:
                return record[attr.getName()] === attr.getValue();
        }
    }

    function getEnvironment() {

      if (location.host.indexOf('-dev') >= 0) {
        return 'dev';
      }
      else if (location.host.indexOf('-qa') >= 0) {
        return 'qa';
      }
      else if (location.host.indexOf('-prd') >= 0) {
        return 'prd';
      }
      else {
        return '';
      }
    }
}
