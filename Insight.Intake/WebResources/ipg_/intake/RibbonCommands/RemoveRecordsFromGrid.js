/**
 * @namespace Intake.Commands
 */
var Intake;
(function (Intake) {
    var RibbonCommands;
    (function (RibbonCommands) {
        var RelationShip = /** @class */ (function () {
            function RelationShip() {
            }
            return RelationShip;
        }());
        var _webApi = Xrm.WebApi;
        var _utility = Xrm.Utility;
        var _clientUrl = Xrm.Page.context.getClientUrl();
        var _webApiVersion = 'v9.1';
        /**
         * Remove Records From SubGrid
         * @function Intake.Commands.RemoveRecords
         * @returns {void}
        */
        function RemoveRecord(entityname, entityId, parentField) {
            return fetch(_clientUrl + "/api/data/" + _webApiVersion + "/" + entityname + "s(" + entityId + ")/" + parentField, {
                method: 'DELETE',
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json; charset=utf-8",
                    "OData-MaxVersion": "4.0",
                    "OData-Version": "4.0"
                }
            });
        }
        function RemoveRecords(entityname, entityIds, selectedControl, parententityName) {
            var parentField = "_" + parententityName + "id_value";
            if (entityname && entityIds && entityIds.length > 0 && selectedControl && parententityName && parentField) {
                _utility.showProgressIndicator(null);
                Promise.all(entityIds.map(function (id) { return RemoveRecord(entityname, id, parentField).catch(function (error) { return console.log(error); }); }))
                    .then(function (r) {
                    console.log(r);
                    selectedControl.refresh();
                    _utility.closeProgressIndicator();
                });
            }
        }
        RibbonCommands.RemoveRecords = RemoveRecords;
    })(RibbonCommands = Intake.RibbonCommands || (Intake.RibbonCommands = {}));
})(Intake || (Intake = {}));
