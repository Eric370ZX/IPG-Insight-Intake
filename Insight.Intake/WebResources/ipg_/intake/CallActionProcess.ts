/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.CallActionProcess
   * @returns {void}
   */
  export function CallActionProcess<T>(entityName: string, entityId: string = null, actionName: string, actionArguments: object = null): Promise<T> {
    const globalContext = Xrm.Utility.getGlobalContext();
    const clientUrl = globalContext.getClientUrl();
    if (!entityId) {
      const currentEntityId = Xrm.Page.data.entity.getId();
      entityId = Intake.Utility.removeCurlyBraces(currentEntityId);
    }
    const requestOptions: Intake.Utility.HttpRequestOptions<{}> = {
      path: `${clientUrl}/api/data/v9.0/${entityName}(${entityId})/Microsoft.Dynamics.CRM.${actionName}`,
      body: actionArguments || {},
      headers: {
        'OData-MaxVersion': '4.0',
        'OData-Version': '4.0',
        'Accept': 'application/json',
        'Content-Type': 'application/json',
      },
    };
    return Intake.Utility.HttpRequest
      .post(requestOptions)
      .then((response: T | any) => response)
      .catch((response: { error: { message: string } }) => {
        return Xrm.Navigation
          .openErrorDialog({ message: response.error.message })
          .then(() => null);
      });
  }

  export async function CallActionProcessAsync<T>(entityName: string, entityId: string = null, actionName: string, actionArguments: object = null): Promise<T | any> {
    if (!entityId) {
      const currentEntityId = Xrm.Page.data.entity.getId();
      entityId = Intake.Utility.removeCurlyBraces(currentEntityId);
    }

    var path = `/api/data/v9.0/${entityName}(${entityId})/Microsoft.Dynamics.CRM.${actionName}`;

    var json = {};
    try
    {
      var resp =  await fetch(path,
        {
          method: 'POST',
          headers:{
          'OData-MaxVersion': '4.0',
          'OData-Version': '4.0',
          'Accept': 'application/json',
          'Content-Type': 'application/json',
          },
          body: JSON.stringify(actionArguments)});
      
      json = await resp.json;
    }
    catch
    {

    }

    return json;
  }
}
