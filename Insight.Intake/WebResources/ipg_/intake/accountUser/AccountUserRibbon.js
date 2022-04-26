/**
 * @namespace Intake.AccountUser.Ribbon
 */
var Intake;
(function (Intake) {
    var AccountUser;
    (function (AccountUser) {
        var Ribbon;
        (function (Ribbon) {
            /**
            * Called on Add Case Manager button click
            * @function Intake.AccountUser.Ribbon.addCaseManager
            * @returns {void}
            */
            function addCaseManager(primaryControl, roleCode) {
                var formContext = primaryControl;
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "ipg_accountuser";
                entityFormOptions["useQuickCreateForm"] = true;
                var formParameters = {};
                formParameters["ipg_accountid"] = formContext.data.entity.getId();
                formParameters["ipg_accountidname"] = formContext.getAttribute("name").getValue();
                formParameters["ipg_rolecode"] = roleCode;
                Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
                    //formContext.data.refresh(true);
                    console.log(success);
                }, function (error) {
                    console.log(error);
                });
            }
            Ribbon.addCaseManager = addCaseManager;
            /**
            * enable rule for "Add Case Manager Button"
            * @function Intake.AccountUser.Ribbon.isUserInCaseManagerTeam
            * @returns {void}
            */
            var userIsCaseManager = null;
            function isUserInCaseManagerTeam(primaryControl, selectedControl) {
                var formContext = primaryControl;
                if (userIsCaseManager) {
                    return true;
                }
                else if (userIsCaseManager === null) {
                    var userId = Xrm.Utility.getGlobalContext().getUserId().replace("{", "").replace("}", "");
                    Xrm.WebApi.online.retrieveMultipleRecords("systemuser", "?$expand=teammembership_association($select=teamid,name)&$filter=systemuserid eq " + userId).then(function (results) {
                        if (results && results.entities && results.entities.length) {
                            var teammembership_associations = results.entities[0].teammembership_association;
                            var caseManagerTeam = teammembership_associations.filter(function (team) {
                                return team.name === "Case Management";
                            });
                            if (caseManagerTeam.length) {
                                userIsCaseManager = true;
                                formContext.getControl("FacilityUsers").refreshRibbon();
                            }
                        }
                    }, function (error) {
                        Xrm.Navigation.openErrorDialog(error);
                    });
                }
                return false;
            }
            Ribbon.isUserInCaseManagerTeam = isUserInCaseManagerTeam;
        })(Ribbon = AccountUser.Ribbon || (AccountUser.Ribbon = {}));
    })(AccountUser = Intake.AccountUser || (Intake.AccountUser = {}));
})(Intake || (Intake = {}));
