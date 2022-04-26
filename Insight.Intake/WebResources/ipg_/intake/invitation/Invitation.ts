/**
 * @namespace Intake.Invitation
 */
namespace Intake.Invitation {
    /**
   * Called on Form Load event
   * @function Intake.Invitation.OnFormLoad
   * @returns {void}
 */
    export async function OnFormLoad(executionContext: Xrm.Events.SaveEventContext) {
        let formContext = executionContext.getFormContext();
        OnChangeRequester(formContext)
    }

    function OnChangeRequester(formContext: Xrm.FormContext) {    
        let requester = formContext.getAttribute("adx_invitercontact");
        requester.addOnChange(() => ChangeRequesterEmail(formContext));
        requester.fireOnChange();
    }

    async function ChangeRequesterEmail(formContext: Xrm.FormContext) {
        let requester = formContext.getAttribute("adx_invitercontact").getValue()
        if(requester == null)
        {
            formContext.getAttribute("ipg_requesteremail").setValue(null);
            return;
        }
        let requesterId = requester[0]?.id;
        let email = (await Xrm.WebApi.retrieveRecord(
            "contact",
            requesterId.toLocaleLowerCase().replace("{", "").replace("}", ""),
            "?$select=emailaddress1"
        ))?.emailaddress1;

        formContext.getAttribute("ipg_requesteremail").setValue(email);
    }
}
