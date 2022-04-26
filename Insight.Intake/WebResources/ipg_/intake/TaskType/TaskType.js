var Intake;
(function (Intake) {
    var TaskType;
    (function (TaskType) {
        var attributes = {
            assignToTeam: "ipg_assigntoteam",
            portal: "ipg_isportal"
        };
        var portalTeamId = "f6a56f2c-c41d-eb11-a813-00224806f885";
        function OnLoad(executionContext) {
            var formContext = executionContext.getFormContext();
            var assignedToTeam = formContext.getAttribute(attributes.assignToTeam);
            assignedToTeam.addOnChange(function () {
                SetPortalByAssignedTeam(formContext);
            });
            assignedToTeam.fireOnChange();
        }
        TaskType.OnLoad = OnLoad;
        function SetPortalByAssignedTeam(formContext) {
            var _a, _b;
            var assignedToTeam = (_a = formContext.getAttribute(attributes.assignToTeam)) === null || _a === void 0 ? void 0 : _a.getValue();
            var assignedToTeamId = assignedToTeam
                ? Intake.Utility.GetFormattedId((_b = assignedToTeam[0]) === null || _b === void 0 ? void 0 : _b.id)
                : null;
            if (assignedToTeamId == portalTeamId) {
                formContext.getAttribute(attributes.portal).setValue(true);
            }
        }
    })(TaskType = Intake.TaskType || (Intake.TaskType = {}));
})(Intake || (Intake = {}));
