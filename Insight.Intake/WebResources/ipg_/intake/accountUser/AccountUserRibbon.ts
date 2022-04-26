/**
 * @namespace Intake.AccountUser.Ribbon
 */
namespace Intake.AccountUser.Ribbon {
  /**
  * Called on Add Case Manager button click
  * @function Intake.AccountUser.Ribbon.addCaseManager
  * @returns {void}
  */
  export function addCaseManager(primaryControl, roleCode) {
    let formContext: Xrm.FormContext = primaryControl;

    let entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_accountuser";

    entityFormOptions["useQuickCreateForm"] = true;

    let formParameters = {};
    formParameters["ipg_accountid"] = formContext.data.entity.getId();
    formParameters["ipg_accountidname"] = formContext.getAttribute("name").getValue();
    formParameters["ipg_rolecode"] = roleCode;

    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
      function (success) {
        //formContext.data.refresh(true);
        console.log(success);
      },
      function (error) {
        console.log(error);
      });
  }

  /**
  * enable rule for "Add Case Manager Button"
  * @function Intake.AccountUser.Ribbon.isUserInCaseManagerTeam
  * @returns {void}
  */
  var userIsCaseManager = null;
  export function isUserInCaseManagerTeam(primaryControl, selectedControl): boolean{
    let formContext: Xrm.FormContext = primaryControl;

    if (userIsCaseManager) {
      return true;
    }
    else if (userIsCaseManager === null) {
      var userId = Xrm.Utility.getGlobalContext().getUserId().replace("{", "").replace("}", "");
      Xrm.WebApi.online.retrieveMultipleRecords("systemuser", "?$expand=teammembership_association($select=teamid,name)&$filter=systemuserid eq " + userId).then(
        (results) => {
          if (results && results.entities && results.entities.length) {
            var teammembership_associations = results.entities[0].teammembership_association;
            var caseManagerTeam = teammembership_associations.filter((team) => {
              return team.name === "Case Management";
            });
            if (caseManagerTeam.length) {
              userIsCaseManager = true;
              formContext.getControl("FacilityUsers").refreshRibbon();
            }
          }
        },
        (error: Xrm.Navigation.ErrorDialogOptions) => {
          Xrm.Navigation.openErrorDialog(error);
        }
      );
    }
    
    return false;
  }
}
