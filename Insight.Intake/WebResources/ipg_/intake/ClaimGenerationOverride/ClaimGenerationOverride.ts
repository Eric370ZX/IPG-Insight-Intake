/**
 * @namespace Intake.ClaimGenerationOverride
 */
 namespace Intake.ClaimGenerationOverride {
    export function OnFormLoad(executionContext: Xrm.Events.EventContext){
        var formContext = executionContext.getFormContext();
        setClaimFieldsVisibility(formContext);
    }

    export function OnChangeClaimToGenerate(executionContext: Xrm.Events.EventContext){
        var formContext = executionContext.getFormContext();
        setClaimFieldsVisibility(formContext);
    }

    function setClaimFieldsVisibility(formContext: Xrm.FormContext){
        var claimToGenerate = formContext.getAttribute("ipg_claimtogenerate")?.getValue();
        if (claimToGenerate && claimToGenerate == 427880001){
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_claimtoreplace").setVisible(true);
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_claimicn").setVisible(true);
        }
        else{
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_claimtoreplace").setVisible(false);
            formContext.getControl<Xrm.Controls.StandardControl>("ipg_claimicn").setVisible(false);
        }
    }

    export async function OnSave(executionContext: Xrm.Events.SaveEventContext){
        var formContext = executionContext.getFormContext();
        var caseLookup = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_caseid")?.getValue();
        var claimGenerationOverrides = await Xrm.WebApi.retrieveMultipleRecords("ipg_claimgenerationoverride", "?$select=ipg_claimgenerationoverrideid&$filter=_ipg_caseid_value eq " + caseLookup[0].id);
        if (claimGenerationOverrides && claimGenerationOverrides.entities.length > 0){
            await Xrm.WebApi.deleteRecord("ipg_claimgenerationoverride", claimGenerationOverrides.entities[0].ipg_claimgenerationoverrideid);
        }
    }
 }
