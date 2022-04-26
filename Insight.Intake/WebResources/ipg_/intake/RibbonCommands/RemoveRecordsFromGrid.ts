/**
 * @namespace Intake.Commands
 */
namespace Intake.RibbonCommands {
  class RelationShip
  {
    PrimaryEntity: string;
    RelatedEntity: string;
    SchemaName: string;
  }
  const _webApi: Xrm.WebApi = Xrm.WebApi;
  const _utility: Xrm.Utility = Xrm.Utility;
  const _clientUrl: string = Xrm.Page.context.getClientUrl();
  const _webApiVersion = 'v9.1';
  /**
   * Remove Records From SubGrid
   * @function Intake.Commands.RemoveRecords
   * @returns {void}
  */

  function RemoveRecord(entityname, entityId, parentField) {
    return fetch(`${_clientUrl}/api/data/${_webApiVersion}/${entityname}s(${entityId})/${parentField}`, {
      method: 'DELETE',
      headers: {
        "Accept": "application/json",
        "Content-Type": "application/json; charset=utf-8",
        "OData-MaxVersion": "4.0",
        "OData-Version": "4.0"
      }
    });
  }
  export function RemoveRecords(entityname: string, entityIds: string[], selectedControl: any, parententityName: string)
  {
    const parentField = `_${parententityName}id_value`

    if (entityname && entityIds && entityIds.length > 0 && selectedControl && parententityName && parentField) {
      _utility.showProgressIndicator(null);

      Promise.all(entityIds.map(id => RemoveRecord(entityname, id, parentField).catch(error => console.log(error))))
        .then((r) => {
        console.log(r);
        selectedControl.refresh();
        _utility.closeProgressIndicator();
      });
    }
  }
}
