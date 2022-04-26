/**
 * @namespace Intake.Utility
 */



namespace Intake.Utility {
  export class Address {
    zip: string;
    state: string;
    city: string;
    street: string;
  };

  export class RemittanceAddress {
    name: string;
    address: string;
    zip: string;
    state: string;
    city: string;
    phone: string;
  };

  export class Zip {
    id: string;
    zipName: string;
    city: string;
  };

  export class State {
    id: string;
    stateName: string;
  }

  export type Listener<TPayload> = (payload: TPayload) => void;
  export interface EventListener<TPayload> {
    event: string;
    listener: Listener<TPayload>
  }
  /**
   * @class EventEmitter
   */
  export class EventEmitter {
    /**
     * @property {number} EventEmitter.attemptsLimit
     * @static
     */
    static attemptsLimit: number = 50;
    /**
     * @property {Array<EventListener<*>>} EventEmitter#listeners
     * @private
     */
    private readonly listeners: Array<EventListener<any>>;
    /**
     * @constructor
     */
    constructor() {
      this.listeners = [];
    }
    /**
     * Adds an event listener for the specified event.
     * @method EventEmitter#addEventListener
     * @template TPayload :: Type of payload.
     * @param {string} event :: Event name.
     * @param {Listener<TPayload>} listener :: Callback function.
     * @returns {void}
     * @throws {TypeError} If the listener is not a function, it throws a type error.
     */
    addEventListener<TPayload>(event: string, listener: Listener<TPayload>): void {
      if ('function' !== typeof listener) {
        throw new TypeError('Listener should be function.');
      }
      this.listeners.push({ event, listener });
    }
    /**
     * Removes a listener for the specified event.
     * @method EventEmitter#removeEventListener
     * @template TPayload :: Type of payload.
     * @param {string} event :: Event name.
     * @param {Listener<TPayload>} listener :: Callback function.
     * @returns {void}
     */
    removeEventListener<TPayload>(event: string, listener: Listener<TPayload>): void {
      for (let index = 0, length = this.listeners.length; index < length; index++) {
        const eventListener: EventListener<TPayload> = this.listeners[index];
        if (eventListener.event === event && eventListener.listener === listener) {
          this.listeners.splice(index, 1);
        }
      }
    }
    /**
     * Emits an event executing all appropriate listeners.
     * @method EventEmitter#emit
     * @template TPayload :: Type of payload.
     * @param {string} event :: Event name.
     * @param {TPayload} payload :: Payload.
     * @returns {void}
     */
    emit<TPayload>(event: string, payload: TPayload = (<TPayload>{})): void {
      let numberOfExistingListeners = 0;
      let numberOfAttempts = 0;
      const emitClosure = () => {
        numberOfAttempts++;
        for (let index = 0, length = this.listeners.length; index < length; index++) {
          const eventListener: EventListener<TPayload> = this.listeners[index];
          if (eventListener.event === event) {
            eventListener.listener(payload);
            numberOfExistingListeners++;
          }
        }
        if (numberOfExistingListeners === 0 && (numberOfAttempts - 1) < EventEmitter.attemptsLimit) {
          setTimeout(emitClosure, 100);
        }
      };
      emitClosure();
    }
  }
  /**
   * Return the search parameters of the current page.
   * @function Intake.Utility.getPageParams
   * @template TParams :: Search parameters.
   * @returns {TParams}
   */
  export function getPageParams<TParams>(): TParams {
    let params = [];
    const result = <TParams>{};
    if (location.search) {
      params = location.search.substr(1).split('&');
      for (let index = 0, length = params.length; index < length; index++) {
        const keyValuePair = params[index].replace(/\+/g, ' ').split('=');
        result[keyValuePair[0]] = keyValuePair[1];
      }
    }
    return result;
  }
  const curlyBracesRegExp = (/[{|}]/g);
  /**
   * Remove curly braces from provided string.
   * @function Intake.Utility.removeCurlyBraces
   * @param {string} str :: Any string.
   * @returns {string}
   */
  export function removeCurlyBraces(str: string): string {
    return curlyBracesRegExp.test(str) ? str.replace(curlyBracesRegExp, '') : str;
  }
  /**
   * Check to is it JSON string or not.
   * @param {string} jsonString :: Expected JSON string
   * @returns {boolean}
   */
  export function isJson(jsonString: string): boolean {
    try {
      JSON.parse(jsonString);
      return true;
    }
    catch (error) {
      return false;
    }
  }
  /**
   * @enum HttpRequestMethod
   */
  export enum HttpRequestMethod {
    Options = 'OPTIONS',
    Head = 'HEAD',
    Get = 'GET',
    Post = 'POST',
    Put = 'PUT',
    Patch = 'PATCH',
    Delete = 'DELETE',
  }
  /**
   * @enum HttpRequestContentType
   */
  export enum HttpRequestContentType {
    Json = 'application/json',
  }
  /**
   * @interface HttpRequestOptions
   * @template TRequest :: Request type
   */
  export interface HttpRequestOptions<TRequest> {
    path: string;
    method?: HttpRequestMethod;
    body?: TRequest;
    parameters?: { [key: string]: string };
    headers?: { [key: string]: string };
  }
  /**
   * @class HttpRequest
   * @template TRequest :: Request type
   * @template TResponse :: Response type
   */
  export class HttpRequest<TRequest, TResponse> {
    /**
     * @method HttpRequest.get
     * @template TRequest :: Request type
     * @template TResponse :: Response type
     * @param {Intake.Utility.HttpRequestOptions} options :: Request options
     * @returns {Promise}
     * @static
     */
    static get<TRequest, TResponse>(options: HttpRequestOptions<TRequest>): Promise<TResponse> {
      const httpRequest = new HttpRequest<TRequest, TResponse>({
        ...options,
        method: HttpRequestMethod.Get,
      });
      return httpRequest.promise;
    }
    /**
     * @method HttpRequest.delete
     * @template TRequest :: Request type
     * @template TResponse :: Response type
     * @param {Intake.Utility.HttpRequestOptions} options :: Request options
     * @returns {Promise}
     * @static
     */
    static delete<TRequest, TResponse>(options: HttpRequestOptions<TRequest>): Promise<TResponse> {
      const httpRequest = new HttpRequest<TRequest, TResponse>({
        ...options,
        method: HttpRequestMethod.Delete,
      });
      return httpRequest.promise;
    }
    /**
     * @method HttpRequest.post
     * @template TRequest :: Request type
     * @template TResponse :: Response type
     * @param {Intake.Utility.HttpRequestOptions} options :: Request options
     * @returns {Promise}
     * @static
     */
    static post<TRequest, TResponse>(options: HttpRequestOptions<TRequest>): Promise<TResponse> {
      const httpRequest = new HttpRequest<TRequest, TResponse>({
        ...options,
        method: HttpRequestMethod.Post,
      });
      return httpRequest.promise;
    }
    /**
     * @property {HttpRequestOptions} HttpRequest#options
     * @readonly
     * @private
     */
    private readonly options: HttpRequestOptions<TRequest>;
    /**
     * @property {Promise} HttpRequest#promise
     * @private
     */
    private promise: Promise<TResponse>;
    /**
     * @property {XMLHttpRequest} HttpRequest#requestContext
     * @private
     */
    private requestContext: XMLHttpRequest;
    /**
     * @constructor
     * @param {HttpRequestOptions} options :: Request options
     */
    constructor(options: HttpRequestOptions<TRequest>) {
      this.options = options;
      this.promise = new Promise<TResponse>((resolve, reject) => {
        this.requestContext = this.getRequestContext();
        this.requestContext.onreadystatechange = this.onReadyStateChangeCallback(resolve, reject).bind(this);
      });
    }
    /**
     * @method HttpRequest#getRequestContext
     * @returns {XMLHttpRequest}
     * @private
     */
    private getRequestContext(): XMLHttpRequest {
      const request = new XMLHttpRequest();
      let path = this.options.path; // Mutable, see below
      let body: string;
      // Prepare a list of parameters for the request
      if (this.options.parameters) {
        const parameters = [];
        const parametersKeys = Object.keys(this.options.parameters);
        for (let index = 0, length = parametersKeys.length; index < length; index++) {
          const parameterKey = parametersKeys[index];
          const encodedKey = encodeURIComponent(parameterKey);
          const encodedValue = encodeURIComponent(this.options.parameters[parameterKey]);
          parameters.push(`${encodedKey}=${encodedValue}`);
        }
        path += `?${parameters.join('&')}`;
      }
      // Prepare body for the request
      if (this.options.method !== HttpRequestMethod.Get) {
        body = JSON.stringify(this.options.body);
      }
      // Open request
      request.open(this.options.method, path, true);
      // Prepare a list of headers for the request
      if (this.options.headers) {
        const headersKeys = Object.keys(this.options.headers);
        for (let index = 0, length = headersKeys.length; index < length; index++) {
          const headerKey = headersKeys[index];
          const headerValue = this.options.headers[headerKey];
          request.setRequestHeader(headerKey, headerValue);
        }
      }
      // Send prepared body
      request.send(body);
      return request;
    }
    /**
     * @method HttpRequest#onReadyStateChangeCallback
     * @private
     */
    private onReadyStateChangeCallback(resolve, reject): () => void {
      return (): void => {
        if (this.requestContext.readyState !== 4) {
          return;
        }
        let response: any = this.requestContext.responseText;
        if (isJson(this.requestContext.responseText)) {
          response = JSON.parse(this.requestContext.responseText) as TResponse;
        }
        if (this.requestContext.status === 200 || this.requestContext.status === 204) {
          resolve(response);
        }
        else {
          reject(response);
        }
      };
    }
  }

   /**
  * Called on Form Load event
  * @function Intake.Utility.OnLoadForm
  * @returns {void}
  */
    export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
      let formContext = executionContext.getFormContext();
      
      SetEmptyCityState(formContext);
      formContext.getAttribute('ipg_zipcodeid')?.addOnChange(() => {
        SetEmptyCityState(formContext);
      });     
      
    }
    
  /**
   * Refresh Ribbon After Saving. Can be used as a Function in the Form OnSave event.
   * @function Intake.Utility.RefreshRibbonAfterSaving
   * @retuns {void}
   */
  export function RefreshRibbonAfterSaving() {
    Xrm.Page.ui.refreshRibbon();
  }

  /*
   * Determines and returns environment short name (for example, dev, qa or prd)
   * @returns {string}
   */
  export function GetEnvironment() {

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

  /**
    * Formats OData date
    * @function getTodayDateFormatted
    * @returns {string}
  */
  export function FormatODataDate(date: Date): string {
    let dd: string = String(date.getDate());
    let mm: string = String(date.getMonth() + 1); //January is 0!
    let yyyy: number = date.getFullYear();

    return yyyy + '-' + mm + '-' + dd;
  }
   /**
    * Formats OData "In" operator (does value contains in arrey)
    * Use it for example for retrieving some records by some ids
    * @function FormatODataLogicIn
    * @returns {string}
  */
    export function FormatODataLogicIn(attributeName:string, values: string[]): string {
      return `(${ values.map(value=> `${attributeName} eq ${value}`).join(" or ")})`;
    }

  /*
   * Parses HTTP search parameters (the string after ? symbol) and returns as string[][].
   * string[i][0] - parameter name,
   * string[i][1] - parameter value.
   * returns {string[][]}
  */
  export function ParseSearchParameters(searchParameters: string): string[][] {
    let paramArray: string[] = searchParameters.split("&");
    let paramKeyValueArray: string[][] = [];
    for (let i in paramArray) {
      paramKeyValueArray[i] = paramArray[i].replace(/\+/g, " ").split("=");
    }

    return paramKeyValueArray;
  }
  export function GetFormattedId(id: string) {
    if (id) {
      return id.replace("{", "").replace("}", "").toLowerCase();
    }
  }
  /*
   * Finds the parameter value by its name
   * returns {string}
  */
  export function GetParamValueByName(vals: string[][], paramName: string): string {
    for (let i in vals) {
      if (vals[i][0].toLowerCase() == paramName.toLowerCase()) {
        return vals[i][1];
      }
    }

    return null;
  }

  /*
   * Gets data search parameter.
   * returns {string}
  */
  export function GetDataParam(): string {
    //Get the query string parameters and load them
    //into the vals array

    let windowWithParam: Window = (window.parent || window);
    if (windowWithParam.location.search != '') {
      let vals: string[][] = ParseSearchParameters(windowWithParam.location.search.substr(1));

      //look for the parameter named 'data'
      return GetParamValueByName(vals, 'data');
    }

    return null;
  }

  export function GetWindowJsonParameters(): any {
    const params = new URLSearchParams(window.location.search);
    const recordId = params.get("id");

    let args = JSON.parse(params.get("data") || "{}");
    args.recordId = recordId;

    return args;
  }

  /*
   * Check crm field on update.
   * returns void
   * Handler Properties Configuration
   * Pass context as first parameter
   * Passed parameters: "^[\\d]{9}$", "The value must be 9 digits!"
  */
  export function CheckFieldOnChangeByRegEx(executionContext: Xrm.Events.EventContext, regExp, error: string) {
    if (executionContext && regExp && error) {
      let regexp: RegExp = new RegExp(regExp)
      let fieldCodeAttribute = executionContext.getEventSource() as Xrm.Attributes.Attribute;
      let value = fieldCodeAttribute.getValue();
      let name = fieldCodeAttribute.getName();
      let AttributeControl = <Xrm.Controls.StandardControl>executionContext.getFormContext().getControl(name);
      if (fieldCodeAttribute && value) {
        if (AttributeControl) {
          if (!regexp.test(value)) {
            AttributeControl.setNotification(error, name);
            return;
          }
        }
      }

      AttributeControl.clearNotification(name);
    }
  }

  export async function GetIPGAddress(WebApi: Xrm.WebApi): Promise<Address> {
    const defaultAddress = "ZIP:30009;Street:2300 Lakeview Parkway - Suite 500;State:GA;City:Alpharetta";
    const key = "IPG Address";
    const result = await WebApi.retrieveMultipleRecords('ipg_globalsetting'
      , '?$top=1&$select=ipg_value'
      + `&$filter=ipg_name eq '${key}'`);

    let findValue = (source, key) => {
      let temp = source.find(s => s && s.toLowerCase().indexOf(key) > -1);
      return temp && temp.split(':')[1];
    }

    if (result.entities.length === 0 || !result.entities[0].ipg_value) {
      if (result.entities.length > 0) {
        await WebApi.updateRecord('ipg_globalsetting', result.entities[0].ipg_globalsettingid, { ipg_value: defaultAddress });
      }
      else {
        await WebApi.createRecord('ipg_globalsetting', { ipg_value: defaultAddress, ipg_name: key });
      }
    }

    let value: string = result.entities.length > 0 && result.entities[0].ipg_value || defaultAddress || defaultAddress;

    let addressArray = value.split(';');

    let zip = findValue(addressArray, 'zip');
    let state = findValue(addressArray, 'state');
    let city = findValue(addressArray, 'city');
    let street = findValue(addressArray, 'street');

    return { zip: zip, state: state, city: city, street: street };
  }

  export async function GetRemittanceAddress(WebApi: Xrm.WebApi): Promise<RemittanceAddress> {
    const defaultAddress = "Name:IPG - Payer Account;Address:Dept. 2399, P.O. Box 122399;State:TX;City:Dallas;Zip:75312-2399;Phone:8667530046";
    const key = "Remittance Address";
    const result = await WebApi.retrieveMultipleRecords('ipg_globalsetting'
      , '?$top=1&$select=ipg_value'
      + `&$filter=ipg_name eq '${key}'`);
    let value = defaultAddress;
    let findValue = (source, key) => {
      let temp = source.find(s => s && s.toLowerCase().indexOf(key) > -1);
      return temp && temp.split(':')[1];
    }

    if (result.entities.length === 0 || !result.entities[0].ipg_value) {
      if (result.entities.length > 0) {
        await WebApi.updateRecord('ipg_globalsetting', result.entities[0].ipg_globalsettingid, { ipg_value: defaultAddress });
      }
      else {
        await WebApi.createRecord('ipg_globalsetting', { ipg_value: defaultAddress, ipg_name: key });
      }
    }
    else if (result.entities.length > 0 && result.entities[0].ipg_value) {
      value = result.entities[0].ipg_value;
    }

    let addressArray = value.split(';');

    let name = findValue(addressArray, 'name');
    let address = findValue(addressArray, 'address');
    let city = findValue(addressArray, 'city');
    let state = findValue(addressArray, 'state');
    let zip = findValue(addressArray, 'zip');
    let phone = findValue(addressArray, 'phone');

    return { name: name, address: address, city: city, state: state, zip: zip, phone: phone };
  }

  export function GetCalcRevWindowSizeAndPosition(): string {
    let w: number = window.innerWidth;
    if (!w && window.parent) {
      w = window.parent.innerWidth;
    }
    w -= 100;
    let h: number = window.innerHeight;
    if (!h && window.parent) {
      h = window.parent.innerHeight;
    }
    h -= 100;

    let left: number = 50;
    let top: number = 50;
    let screenX: number = window.screenX + left;
    return 'location=0, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left + ', screenX=' + screenX + ', screenY=' + top;
  }

  /// <summary>

  /// Returns true of the current user has the given security role

  /// </summary>

  export function isUserInRole(roleName: string) {
    if (!roleName) {
      const entityName = Xrm.Page && Xrm.Page.data && Xrm.Page.data.entity && Xrm.Page.data.entity.getEntityName();
      console.error(`roleName cannot be empty in isUserInRole! Please check the Ribbon for ${entityName}`);
    }

    const userRoles = Xrm.Utility.getGlobalContext().userSettings.roles;
    return !!userRoles.get(lk => lk.name && lk.name.toLowerCase().trim() == roleName.toLowerCase().trim())[0];
  }

  export function getAppName(): PromiseLike<string> {
    return Xrm.Utility.getGlobalContext().getCurrentAppName();
  }

  export function isUserRoleInApp(roleName: string, appName: string): Promise<boolean> {
    return new Promise(function (resolve, reject) {
      getAppName()
        .then(name => resolve(!name || name.toLowerCase().trim() != appName.toLowerCase().trim() || isUserInRole(roleName))
          , error => reject(error));
    });
  }

  export function isAppWithName(appName: string): Promise<boolean> {
    return new Promise(function (resolve, reject) {
      getAppName()
        .then(name => resolve(name.toLowerCase().trim() == appName.toLowerCase().trim()), error => reject(error));
    })
  }

  export async function GetZip(WebApi: Xrm.WebApi, key: string): Promise<Zip> {
    const result = await WebApi.retrieveMultipleRecords('ipg_zipcode'
      , '?$top=1&$select=ipg_zipcodeid,ipg_name,ipg_city'
      + `&$filter=ipg_name eq '${key}'`);

    if (result.entities.length > 0 && result.entities[0].ipg_zipcodeid && result.entities[0].ipg_name) {
      let id = result.entities[0].ipg_zipcodeid;
      let zipName = result.entities[0].ipg_name;
      let city = result.entities[0].ipg_city;
      return { id: id, zipName: zipName, city: city};
    }
    else {
      return null;
    }
  }

  export async function GetState(WebApi: Xrm.WebApi, key: string): Promise<State> {
    const result = await WebApi.retrieveMultipleRecords('ipg_state'
      , '?$top=1&$select=ipg_stateid,ipg_name'
      + `&$filter=ipg_name eq '${key}'`);
    
    if (result.entities.length > 0 && result.entities[0].ipg_stateid && result.entities[0].ipg_name) {
      let id = result.entities[0].ipg_stateid;
      let stateName = result.entities[0].ipg_name;
      return { id: id, stateName: stateName};
    }
    else {
      return null;
    }
  }


  // Export EventEmitter instance to global scope.
  const globalScope = window.parent || window;
  globalScope.__EventEmitterInstance = globalScope.__EventEmitterInstance || new EventEmitter();
  export const Events: EventEmitter = globalScope.__EventEmitterInstance;

   /**
  * Set empty City State
  * @function Intake.Utility.SetEmptyCityState
  * @returns {void}
  */
  function SetEmptyCityState(formContext: Xrm.FormContext) {           
    let zipcode = formContext.getAttribute("ipg_zipcodeid");  
    let city = formContext.getAttribute("address1_city");
    let state = formContext.getAttribute("ipg_stateid");
    if(zipcode?.getValue() == null || zipcode?.getValue() === "") {      
      city?.setValue(null);   
      state?.setValue(null);
    } 
  }

}