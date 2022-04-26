/**
 * @namespace Intake.Incident.Ribbon
 */
 namespace Intake.Incident.Ribbon {
  enum CaseStates {
    Intake = 923720000,
    Authorizations = 923720001,
    CaseManagement = 923720002,
    Billing = 923720003,
    CarrierCollections = 923720004,
    PatientCollections = 923720005,
    Finance = 923720006
  };

  const caseManager = function (form: Xrm.FormContext) {
    const caseHoldField = form.getAttribute<Xrm.Attributes.BooleanAttribute>("ipg_casehold");
    const caseStateField = form.getAttribute<Xrm.Attributes.OptionSetAttribute>("statecode");
    const setCaseHold = (value: boolean) => {
      caseHoldField.setValue(value);
      caseHoldField.setSubmitMode("always");
      form.data.save().then(() => {
        if (!value) {
          form.ui.clearFormNotification("casehold");
        }
        form.ui.refreshRibbon();
      }, error => {
        caseHoldField.setValue(caseHoldField.getInitialValue());
        form.ui.refreshRibbon();
        Xrm.Utility.alertDialog(`Error while saving the case: ${error.message}`, null);
      });
    }
    const isOnHold = (): boolean => caseHoldField && caseHoldField.getValue() === true;
    const isOpen = (): boolean => caseStateField && caseStateField.getValue() === 0;

    const enableCaseHold = (): boolean =>  isOpen() && !isOnHold();
    const enableRemoveHold = (): boolean => isOpen() && isOnHold();

    const caseHold = (): void => {
      if (!caseHoldField) {
        return;
      }
      setCaseHold(true);
    };
    const removeHold = (): void => {
      if (!caseHoldField) {
        return;
      }
      setCaseHold(false);
    };
    return {
      EnableCaseHold: enableCaseHold,
      EnableRemoveHold: enableRemoveHold,
      CaseHold: caseHold,
      RemoveHold: removeHold,
    };
  };
  function caseHold(formContext: Xrm.FormContext, action: string) {
    formContext.data.refresh(true).then(p => {
      const data = { caseId: formContext.data.entity.getId(), action: action };
      const storageKey = `holdCase-${data.caseId}`;
      localStorage.setItem(storageKey, "false");
      window.addEventListener("storage", (ev) => {
        if (ev.key === storageKey && ev.newValue === "true") {
          formContext.data.refresh(false).then(p => {
            formContext.ui.refreshRibbon();

            const caseHoldField = formContext.getAttribute<Xrm.Attributes.BooleanAttribute>("ipg_casehold");
            caseHoldField.fireOnChange();
            if (!caseHoldField.getValue()) {
              formContext.ui.clearFormNotification("casehold");
            }
          });
        }
      });
      Xrm.Navigation.openWebResource("ipg_/intake/incident/holdCase.html", null, JSON.stringify(data));
    });
  }
  export function CaseHold(formContext: Xrm.FormContext) {
    caseHold(formContext, "hold");
  }
  export function RemoveHold(formContext: Xrm.FormContext) {
    caseHold(formContext, "unhold");
  }
  export function ChangeHoldReason(formContext: Xrm.FormContext) {
    caseHold(formContext, "changehold");
  }
  export function EnableCaseHold(formContext: Xrm.FormContext) {
    return caseManager(formContext).EnableCaseHold();
  }
  export function EnableRemoveHold(formContext: Xrm.FormContext) {
    return caseManager(formContext).EnableRemoveHold();

  }

  export function CloseCase(formContext: Xrm.FormContext) {
    formContext.data.refresh(true).then(p => {
      const data = formContext.data.entity.getId();
      const storageKey = `closeCase-${data}`;
      localStorage.setItem(storageKey, "false");
      window.addEventListener("storage", (ev) => {
        if (ev.key === storageKey && ev.newValue === "true") {
          formContext.data.refresh(false);
        }
      });
      const windowsOption = { openInNewWindow: false, height: 400, width: 400 };
      Xrm.Navigation.openWebResource("ipg_/intake/incident/CloseCase.html", null, data);
    });

  }
  export function CloseCaseValidation(formContext: Xrm.FormContext): Promise<boolean> {
    return new Promise<boolean>(async (resolve, reject) => {
      const ipg_casestatus = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_casestatus");
      const isClosed = ipg_casestatus.getValue() === 923720001;//Closed
      if (isClosed) {
        resolve(false);
        return;
      }
      const isCasemanagementTeam = await IsTeamMember("Case Management");
      resolve(isCasemanagementTeam);
    });
  }
  function IsTeamMember(teamName: string): Promise<boolean> {
    return new Promise<boolean>(async (resolve, reject) => {
      try {
        const userTeams = await GetuserTeams();
        const isTeamMember = userTeams.filter(p => p == teamName).length > 0;
        resolve(isTeamMember);
      } catch (ex) {
        reject(ex.message);
      }
    });
  }
  async function GetuserTeams(): Promise<string[]> {
    const fetchXml = `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">
              <entity name="team">
                <attribute name="name" />
                <order attribute="name" descending="false" />
                <link-entity name="teammembership" from="teamid" to="teamid" visible="false" intersect="true">
                  <link-entity name="systemuser" from="systemuserid" to="systemuserid" alias="ai">
                    <filter type="and">
                      <condition attribute="systemuserid" operator="eq-userid" />
                    </filter>
                  </link-entity>
                </link-entity>
              </entity>
            </fetch>`;
    const teams = await Xrm.WebApi.online.retrieveMultipleRecords("team", `?fetchXml=${fetchXml}`);
    return teams.entities.map(p => p.name);
  }
  export function BVF(form: Xrm.FormContext): void {

    let formParams: Xrm.Utility.FormOpenParameters = {
      "ipg_parentcaseid": form.data.entity.getId(),
      "ipg_parentcaseidname": form.data.entity.getPrimaryAttributeValue(),
      "ipg_parentcaseidtype": form.data.entity.getEntityName(),
    };
    Xrm.Utility.openEntityForm("ipg_benefitsverificationform", null, formParams);
  }
  export function BVFValidation(form: Xrm.FormContext): boolean {
    let result = true;
    if (!form.data.entity.getId())
      result = false;
    return result;
  }
  /**
   * Called on Submit button click
   * @function Intake.Incident.Ribbon.OnSubmitClick
   * @returns {void}
  */
  export function OnSubmitClick(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    var caseStatus = formContext.getAttribute("ipg_casestatus")?.getValue();
    if (caseStatus && caseStatus == 923720001){
      Xrm.Navigation.openAlertDialog({ confirmButtonLabel: "Ok", text: "You are not able to submit closed case." }).then(() => {
        return;
      });
    }
    else{
      Xrm.Utility.showProgressIndicator("Processing...");
      formContext.data.save().then(
        async function () {
          const formData = { Id: formContext.data.entity.getId(), EntityName: formContext.data.entity.getEntityName(), UserId: Xrm.Utility.getGlobalContext().userSettings.userId };

          if ((await ConfirmSubmit(formData.Id)) && (await ConfirmSubmitGate11(formData.Id))) {
            ProceedGating(formData, formContext);
            //MVP real time processing gating
            //ProceedGatingWithIncrementalView(formData, formContext);
          }
          else {
            Xrm.Utility.closeProgressIndicator();
          }
        },
        function (error) {
          Xrm.Utility.closeProgressIndicator();
          console.log(error.message);
        }
      );
    }
  }

  async function ConfirmSubmit(caseId: string): Promise<boolean>
  {
    const fetchXml = `<fetch top="1" >
  <entity name="ipg_gateprocessingrule" >
    <attribute name="ipg_gateprocessingruleid" />
    <filter>
      <condition entityname="nexLfStep" attribute="ipg_gateconfigurationidname" operator="eq" value="Gate 9" />
      <condition entityname="LfStep" attribute="ipg_gateconfigurationidname" operator="eq" value="Gate 8" />
    </filter>
    <link-entity name="ipg_lifecyclestep" from="ipg_lifecyclestepid" to="ipg_lifecyclestepid" link-type="inner" alias="LfStep" >
      <link-entity name="incident" from="ipg_lifecyclestepid" to="ipg_lifecyclestepid" >
        <filter>
          <condition attribute="incidentid" operator="eq" value="${caseId}"  />
        </filter>
      </link-entity>
    </link-entity>
    <link-entity name="ipg_lifecyclestep" from="ipg_lifecyclestepid" to="ipg_nextlifecyclestepid" link-type="inner" alias="nexLfStep" />
  </entity>
</fetch>`;

    const result = await Xrm.WebApi.online.retrieveMultipleRecords("ipg_gateprocessingrule", `?fetchXml=${fetchXml}`);

    //if case about go to Billing - show confirm dialog
    return result.entities && result.entities.length > 0 ?
      await (await Xrm.Navigation.openConfirmDialog(
        {
          title: "Confirmation",
          text: "Please note that by clicking OK you are promoting this case to Billing.  If this in error, click ‘Cancel’ to return to the case."
        })).confirmed
      : true;;
  }

  async function ConfirmSubmitGate11(caseId: string): Promise<boolean> {
    const result = await Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=_ipg_lifecyclestepid_value");
    return (result["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"] && (<string>result["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"]).startsWith("Claim Remittance")) ?
      await (await Xrm.Navigation.openConfirmDialog(
        {
          title: "Confirmation",
          text: "Are you sure you want to promote the Case?"
        })).confirmed
      : true;;
  }
  function ProceedGatingWithIncrementalView(formData: any, formContext: Xrm.FormContext) {

    const data = { 
      caseId: formData.Id
      ,InitiatingUser: formData.UserId};

    const pageInput: Xrm.Navigation.PageInputHtmlWebResource = {
      pageType: "webresource",
      webresourceName: "ipg_/intake/case/GateExecutionStatus.html",
      data: JSON.stringify(data)
  };

  const navigationOptions: Xrm.Navigation.NavigationOptions = {
      target: 2,
      width: 650,
      height: 400,
      position: 1,

  };
  
  const refreshForm = ()=> {
    formContext.data.refresh(false)
    formContext.ui.refreshRibbon();
    Xrm.Utility.closeProgressIndicator();
  };

  Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(refreshForm,refreshForm);
}
  async function ProceedGating(formData: any, formContext: Xrm.FormContext) {
    const gatingVersion = await Xrm.WebApi.retrieveMultipleRecords("ipg_globalsetting", `?$select=ipg_value&$filter=ipg_name eq 'GatingVersion'`);
    var parameters = {
      "Target": {
        "incidentid": formData.Id,
        "@odata.type": "Microsoft.Dynamics.CRM." + formData.EntityName
      },
      "InitiatingUser": {
        "systemuserid": formData.UserId,
        "@odata.type": "Microsoft.Dynamics.CRM.systemuser"
      },
      IsManual: true
    };
    const mainGatingCallback = (results) => {
      if (results.Output != null && results.Output != "") {
        var alertStrings = { confirmButtonLabel: "OK", text: results.Output };
        Xrm.Navigation.openAlertDialog(alertStrings, { height: 400, width: 600 }).then(
          function success(result) {
          },
          function (error) {
            console.log(error.message);
          });
      } else {
        Xrm.WebApi.retrieveRecord("incident", formData.Id, "?$select=ipg_casestatus,_ipg_gateconfigurationid_value,ipg_statecode").then(
          function success(result) {
            if ((<string>result["_ipg_gateconfigurationid_value@OData.Community.Display.V1.FormattedValue"]).startsWith("Gate 11") && (result["ipg_casestatus"] != 923720001)) {
              let alertStrings = { confirmButtonLabel: "Ok", text: `Case was promoted to ${result["ipg_statecode@OData.Community.Display.V1.FormattedValue"]}`, title: "Case state" };
              Xrm.Navigation.openAlertDialog(alertStrings);
            }
            if (result && result["_ipg_gateconfigurationid_value"]) {
              Intake.Case.showTabsByGate(formContext, { entityType: "ipg_gateconfiguration", id: result["_ipg_gateconfigurationid_value"], name: "" });
            }      
          },
          function (error) {
            Xrm.Utility.alertDialog(error.message, null);
          }
        );
      }
    };

    callAction("ipg_IPGGatingStartGateProcessing", parameters, true,
      function (results) {
        formContext.data.refresh(true).then(async () => {
          const warningSeverityLevel: number = 923720001;
          Xrm.Utility.closeProgressIndicator();
          if (gatingVersion.entities[0].ipg_value == "2") {
            if (!results.SeverityLevel) {
              var alertStrings = { confirmButtonLabel: "OK", text: results.Output };
              Xrm.Navigation.openAlertDialog(alertStrings, { height: 400, width: 600 }).then(
                function success(result) {
                },
                function (error) {
                  console.log(error.message);
                });
            }
            switch (results.SeverityLevel) {
              case 923720003: //critical
                const confirmStr: Xrm.Navigation.ConfirmStrings = { title: "Critical", text: `At least one critical issue found. Do you wish to proceed with closing this Case?`, confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
                const confirmResult = await Xrm.Navigation.openConfirmDialog(confirmStr, { height: 400, width: 600 });
                if (confirmResult.confirmed) {
                  gatePostAction(formContext, formData, mainGatingCallback);
                }
                break;
              case 923720002: //error
                var alertStr = { title: "Error", text: `The following errors were found. Case cannot proceed to the next step until these errors are corrected and Case is reprocessed through the 'Submit' action. \n${results.Output}`, confirmButtonLabel: "Close" };
                Xrm.Navigation.openAlertDialog(alertStr, { height: 400, width: 600 }).then(() => {
                  gatePostAction(formContext, formData, mainGatingCallback);
                });
                break;
              case 923720001: //warning
                let nextLifeCycleStep = await getNextLifeCycleStep(formContext);
                alertStr = { title: "Warning", text: `The following warnings were found. Case promoted to ${nextLifeCycleStep} step. \n${results.Output}`, confirmButtonLabel: "Close" };
                Xrm.Navigation.openAlertDialog(alertStr, { height: 400, width: 600 }).then(() => {
                  gatePostAction(formContext, formData, mainGatingCallback);
                });
                break;
              case 923720000: //info
                nextLifeCycleStep = await getNextLifeCycleStep(formContext);
                alertStr = { title: "Success", text: `No issues found. Case promoted to ${nextLifeCycleStep}`, confirmButtonLabel: "Close" };
                Xrm.Navigation.openAlertDialog(alertStr, { height: 400, width: 600 }).then(() => {
                  mainGatingCallback({});
                });
                break;
            }
          }
          else {
            if (results.AllowReject) {
              const confirmText = results.SeverityLevel === warningSeverityLevel
                ? "There is at least one business rule warning flagged. Would you like to promote the Case anyway?"
                : "Gating was completed with error. Do you want to continue with closing actions or cancel the process and do required actions";
              const confirmStr: Xrm.Navigation.ConfirmStrings = { title: "Approval required", text: confirmText };
              const confirmReslt = await Xrm.Navigation.openConfirmDialog(confirmStr, null);
              if (confirmReslt.confirmed) {
                gatePostAction(formContext, formData, mainGatingCallback);
              }
            } else {
              mainGatingCallback(results)
            }
          }
        });
      });
  }
  export function OpenAssignDialog(context, selectedRecordsIds: string[]) {
    let primatyContext: Xrm.FormContext = typeof context?.getFormContext == "function"
      ? context.getFormContext()
      : context;

    const data = { selectedRecordsIds: selectedRecordsIds};
    const pageInput: Xrm.Navigation.PageInputHtmlWebResource = {
      pageType: "webresource",
      webresourceName: "ipg_/intake/case/assignCaseModal.html",
      data: JSON.stringify(data)
    };
    const navigationOptions: Xrm.Navigation.NavigationOptions = {
      target: 2,
      width: 550,
      height: 350,
      position: 1,

    };
    Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(
      (success) => ((primatyContext as Xrm.FormContext)?.data ?? (primatyContext as any)).refresh(),
      (error) => ((primatyContext as Xrm.FormContext)?.data ?? (primatyContext as any)).refresh()
    );
  }
  /**
    * call Custom action
    * @function Intake.Incident.Ribbon.callAction
    * @returns {void}
  */
  function callAction(actionName, parameters, async, successCallback) {
    var req = new XMLHttpRequest();
    req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + actionName, async);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {
      if (this.readyState === 4) {
        req.onreadystatechange = null;
        if (this.status === 200) {
          successCallback(JSON.parse(this.response));
        } else {
          Xrm.Utility.closeProgressIndicator();
          alert(JSON.parse(this.response).error.message);
        }
      }
    };
    req.send(JSON.stringify(parameters));
  }

  /**
   * enable rule for Generate PO
   * @function Intake.Incident.Ribbon.hasActualParts
   * @returns {boolean}
   */
  export async function hasActualParts(primaryControl: Xrm.FormContext, caseId) {
    const res = await Xrm.WebApi.retrieveMultipleRecords('ipg_casepartdetail', `?$filter=_ipg_caseid_value eq ${caseId.replace("{", "").replace("}", "").toLowerCase()}`);
    if (res.entities.length > 0)
      return true;
    return false;
  }

  /**
  * enable rule for Generate PO
  * @function Intake.Incident.Ribbon.caseIsOpened
  * @returns {boolean}
  */
  export async function caseIsOpened(primaryControl: Xrm.FormContext, caseId) {
    const caseStatus = await (Xrm.WebApi.retrieveRecord('incident', `${caseId.replace("{", "").replace("}", "").toLowerCase()}`, '?$select=ipg_casestatus'));
    if (caseStatus["ipg_casestatus"] == 923720000)
      return true;
    else
      return false;
  }

  /**
   * Called on PDL button click
   * @function Intake.Incident.Ribbon.OnPDLClick
   * @returns {void}
  */
  export function OnPDLClick(primaryControl) {
    let formContext = primaryControl;
    let ProformParameters =
    {
      caseId: formContext.data.entity.getId()
    };
    let webResourceName = "ipg_/intake/incident/PDL.html";
    var customParameters = "params=" + JSON.stringify(ProformParameters);
    Xrm.Utility.openWebResource(webResourceName, customParameters);
  }

  /**
 * enable rule for PIF Step 2
 * @function Intake.Incident.Ribbon.isPIFStep2Form
 * @returns {boolean}
 */
  export function isPIFStep2Form(primaryControl: Xrm.FormContext): boolean {
    const lockCase = primaryControl.getAttribute("ipg_islocked");
    if (lockCase?.getValue())
      return false;

    var isStep2 = Xrm.Utility.getGlobalContext().getQueryStringParameters().p_isStep2;
    if (isStep2) {
      return false;
    }

    return true;
  }

  export function SubmitButtonEnable(primaryControl: Xrm.FormContext): boolean {
    const lockCase = primaryControl.getAttribute("ipg_islocked");
    return !lockCase?.getValue();
  }

  export function ShowLegacyAuditInformation(primaryControl) {
    let env = Intake.Utility.GetEnvironment();
    let envSuffix: string;
    if (env) {
      envSuffix = '-' + env;
    }
    else {
      envSuffix = '';
    }
    let formContext: Xrm.FormContext = primaryControl;
    var incidentId = formContext.data.entity.getId();
    incidentId = incidentId.replace(/[{}]/g, "");

    Xrm.Navigation.openUrl("https://insight-auditinfo" + envSuffix + ".azurewebsites.net/patientprocedureauditinfo/index/" + incidentId);
  }

  /**
   * Called on Add Patient Payment button click
   * @function Intake.Incident.Ribbon.OnAddPatientPaymentClick
   * @returns {void}
  */
  export function OnAddPatientPaymentClick(primaryControl) {
    //This function is depriciated
    //It was used in Case Ribbon (AddPatientPayment command)
    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId();
    if (!caseId)
      return;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_payment";
    entityFormOptions["formId"] = "05a46600-f1c0-450e-9846-73e3e82829eb";

    Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title").then(function (incident) {
      let formParameters = {};
      formParameters["ipg_caseid"] = caseId;
      formParameters["ipg_caseidname"] = incident.title;
      formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
      Xrm.Navigation.openForm(entityFormOptions, formParameters);
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });

  }

  /**
   * Called on Add Adjustment button click
   * @function Intake.Incident.Ribbon.OnAddAdjustmentClick
   * @returns {void}
  */
  export function OnAddAdjustmentClick(primaryControl) {

    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId();
    if (!caseId)
      return;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_adjustment";

    Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
      let formParameters = {};
      formParameters["ipg_caseid"] = caseId;
      formParameters["ipg_caseidname"] = incident.title;
      formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
      formParameters["ipg_percent"] = 100;
      formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
      formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
      formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
      formParameters["ipg_casebalance"] = incident.ipg_casebalance;
      Xrm.Navigation.openForm(entityFormOptions, formParameters);
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });

  }

  export function OnRetrieveBenefitsClick(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId();
    if (!caseId)
      return;

    let memberid = formContext.getAttribute("ipg_memberidnumber").getValue();
    if (!memberid) {
      var alertStrings = { confirmButtonLabel: "OK", text: "Member Id is empty!" };
      var alertOptions = { height: 150, width: 300 };
      Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
      return;
    }

    Xrm.Utility.showProgressIndicator("Processing...");
    let arguments: VerifyBenefitsArguments = { IsUserGenerated: true };
    Intake.Utility.CallActionProcess("incidents", Intake.Utility.removeCurlyBraces(caseId), "ipg_IPGCaseActionsVerifyBenefits", arguments).then(
      () => {
        formContext.data.refresh(false);
        Xrm.Utility.closeProgressIndicator();
      });
  }

  /**
   * Called on Write-Off Small Balance button click
   * @function Intake.Incident.Ribbon.OnWriteOffSmallBalanceClick
   * @returns {void}
  */
  export function OnWriteOffSmallBalanceClick(primaryControl) {

    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId();
    if (!caseId)
      return;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_adjustment";

    Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
      let formParameters = {};
      formParameters["ipg_caseid"] = caseId;
      formParameters["ipg_caseidname"] = incident.title;
      formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
      formParameters["ipg_percent"] = 100;
      formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
      formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
      formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
      formParameters["ipg_casebalance"] = incident.ipg_casebalance;
      formParameters["ipg_applyto"] = 427880002;
      formParameters["ipg_adjustmenttype"] = 427880000;
      formParameters["ipg_reason"] = 427880007;
      Xrm.Navigation.openForm(entityFormOptions, formParameters);
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });

  }

  /**
   * Called on Patient Discount button click
   * @function Intake.Incident.Ribbon.OnPatientDiscountClick
   * @returns {void}
  */
  export function OnPatientDiscountClick(primaryControl) {

    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId();
    if (!caseId)
      return;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_adjustment";

    Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
      let formParameters = {};
      formParameters["ipg_caseid"] = caseId;
      formParameters["ipg_caseidname"] = incident.title;
      formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
      formParameters["ipg_percent"] = 100;
      formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
      formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
      formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
      formParameters["ipg_casebalance"] = incident.ipg_casebalance;
      formParameters["ipg_applyto"] = 427880002;
      formParameters["ipg_adjustmenttype"] = 427880000;
      formParameters["ipg_reason"] = 427880004;
      formParameters["ipg_percent"] = 0;
      Xrm.Navigation.openForm(entityFormOptions, formParameters);
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });

  }

  /**
 * Called on Write-Off Small Balance button click
 * @function Intake.Incident.Ribbon.OnWriteOffSmallBalanceClick
 * @returns {void}
*/
  export function OnWriteOffPatientBalanceClick(primaryControl) {

    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId();
    if (!caseId)
      return;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_adjustment";

    Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
      let formParameters = {};
      formParameters["ipg_caseid"] = caseId;
      formParameters["ipg_caseidname"] = incident.title;
      formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
      formParameters["ipg_percent"] = 100;
      formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
      formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
      formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
      formParameters["ipg_casebalance"] = incident.ipg_casebalance;
      formParameters["ipg_applyto"] = 427880000;
      formParameters["ipg_adjustmenttype"] = 427880000;
      formParameters["ipg_reason"] = 427880020;
      Xrm.Navigation.openForm(entityFormOptions, formParameters);
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });
  }

  export function OpenCalcRev(primaryControl: Xrm.FormContext): void {
    //debugger;

    //get CalcRev application environment suffix
    let env: string = Intake.Utility.GetEnvironment();
    let envSuffix: string;
    if (env) {
      envSuffix = '-' + env;
    }
    else {
      envSuffix = '';
    }

    //get incidentId
    let incidentId: string = primaryControl.data.entity.getId();
    incidentId = incidentId.replace(/[{}]/g, ""); //delete curly brackets

    let windowSizeAndPosition: string = Intake.Utility.GetCalcRevWindowSizeAndPosition();

    //finally open the window
    window.open(`https://insight-calcrev${envSuffix}.azurewebsites.net/Case/CalcRev/${incidentId}`, 'calcRevWindow', windowSizeAndPosition);
  }

  export function OpenEstimatedCalcRev(primaryControl: Xrm.FormContext): void {
    //debugger;

    //get CalcRev application environment suffix
    let env: string = Intake.Utility.GetEnvironment();
    let envSuffix: string;
    if (env) {
      envSuffix = '-' + env;
    }
    else {
      envSuffix = '';
    }

    //get incidentId
    let incidentId: string = primaryControl.data.entity.getId();
    incidentId = incidentId.replace(/[{}]/g, ""); //delete curly brackets

    let windowSizeAndPosition: string = Intake.Utility.GetCalcRevWindowSizeAndPosition();
    
    //finally open the window
    window.open(`https://insight-calcrev${envSuffix}.azurewebsites.net/Case/Estimate#/${incidentId}`, 'calcRevWindow', windowSizeAndPosition);
  }

  export function ShouldEnableCalcRevButton(primaryControl: Xrm.FormContext): boolean {
    const lockCase = primaryControl.getAttribute("ipg_islocked");
    if (lockCase?.getValue())
    {
      return false;
    }
    else
    {
      return lifeCycleStepForCalcRevButton(primaryControl);
    }
  }

  export function isCaseOnHoldView(selectedControl): boolean {
    return selectedControl.getViewSelector
      && selectedControl.getViewSelector().getCurrentView().name.toLowerCase().indexOf('hold') > -1
  }

  export function ReleaseHold(entityIds: string[], selectedControl: any)
  {
    if (selectedControl && entityIds && entityIds.length > 0) {
      const data = { caseId: entityIds, action: "unhold" };
      Xrm.Navigation.openWebResource("ipg_/intake/incident/holdCase.html", null, JSON.stringify(data));

      window.addEventListener("storage", (ev) => {
        if (ev.key === "RefreshHoldCaseRibbon" && ev.newValue === "true") {
          selectedControl.refresh();
          localStorage.removeItem("RefreshHoldCaseRibbon");
        }
      });
    }
  }

  export async function IsCaseClosedById(entityTypeName: string, entityId: string){
    if (entityTypeName === 'incident'){
      var incident = await Xrm.WebApi.retrieveRecord(entityTypeName, entityId, '?$select=ipg_casestatus');
      return !(incident.ipg_casestatus && incident.ipg_casestatus == 923720001);
    }
  }

  function gatePostAction(formContext: Xrm.FormContext, formData, mainGatingCallback = undefined) {
    var parametersConfirm = {
      "Target": {
        "incidentid": formData.Id,
        "@odata.type": "Microsoft.Dynamics.CRM." + formData.EntityName
      }
    };
    Xrm.Utility.showProgressIndicator("Processing...");
    callAction("ipg_IPGGatingGateProcessingPostAction", parametersConfirm, true, (resultsConfirm) => {
      formContext.data.refresh(true).then(() => {
        formContext.ui.refreshRibbon();
        Xrm.Utility.closeProgressIndicator();
        typeof mainGatingCallback === "function" && mainGatingCallback(resultsConfirm);
      });
    });
  }

  export async function IsCaseLocked(entityTypeName: string, entityId: string) {
    if (entityTypeName === 'incident') {
      const incident = await Xrm.WebApi.retrieveRecord(entityTypeName, entityId, '?$select=ipg_islocked');
      return !(incident?.ipg_islocked == true);
    }
  }

  export async function OpenCase(formContext: Xrm.FormContext)
  {
    Xrm.Utility.showProgressIndicator("");
    
    if(formContext?.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_casestatus")?.getValue() === 923720001) //closed
    {
      var ipg_IPGCaseOpenRequest = {
        entity: {entityType:"incident", id:formContext.data.entity.getId()},
    
        getMetadata: function() {
            return {
                boundParameter: "entity",
                parameterTypes: {
                    "entity": {
                        "typeName": "mscrm.incident",
                        "structuralProperty": 5
                    }
                },
                operationType: 0,
                operationName: "ipg_IPGCaseOpen"
            };
        }
    };
    
    try
    {
      await Xrm.WebApi.online.execute(ipg_IPGCaseOpenRequest);
      await formContext.data.refresh(false);
      formContext.ui.refreshRibbon();
    }
    catch(e)
    {
      Xrm.Navigation.openErrorDialog({message:e.message ?? "There is error. Please try later or Contact System Administrator!"});
    }
  }

    Xrm.Utility.closeProgressIndicator();
  }

  async function getNextLifeCycleStep(formContext: Xrm.FormContext) {
    let entity = await Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(formContext.data.entity.getId()), `?$select=_ipg_lifecyclestepid_value`);
    return entity["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"];
  }

  export async function GenerateClaimOverride(primaryControl:Xrm.FormContext, caseId: string){
    var entityFormOptions = {
    entityName : "ipg_claimgenerationoverride",
    useQuickCreateForm : true
  }; 
    var carrierLookup = primaryControl.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_carrierid")?.getValue();
    var carrier = await Xrm.WebApi.retrieveRecord(carrierLookup[0].entityType, carrierLookup[0].id, "?$select=ipg_claimtype");
    var claimType = carrier.ipg_claimtype;

    var formParameters = {};
    var caseLookupValue = new Array();
    caseLookupValue[0] = new Object();
    caseLookupValue[0].id = caseId;
    caseLookupValue[0].name = primaryControl.getAttribute("title").getValue();
    caseLookupValue[0].entityType = "incident";
    formParameters["ipg_caseid"] = caseLookupValue;
    formParameters["ipg_carrierid"] = carrierLookup;
    formParameters["ipg_claimformtype"] = claimType;

    var claimGenerationOverrides = await Xrm.WebApi.retrieveMultipleRecords("ipg_claimgenerationoverride", "?$select=ipg_claimtogenerate,ipg_claimtoreplace,ipg_claimicn,ipg_claimnotes&$filter=_ipg_caseid_value eq " + caseId);
    if (claimGenerationOverrides && claimGenerationOverrides.entities.length > 0){
      formParameters["ipg_claimtogenerate"] = claimGenerationOverrides.entities[0].ipg_claimtogenerate;
      formParameters["ipg_claimtoreplace"] = claimGenerationOverrides.entities[0].ipg_claimtoreplace;
      formParameters["ipg_claimicn"] = claimGenerationOverrides.entities[0].ipg_claimicn;
      formParameters["ipg_claimnotes"] = claimGenerationOverrides.entities[0].ipg_claimnotes;
    }
    else {
      var claims = await Xrm.WebApi.retrieveMultipleRecords("invoice", "?$select=name,ipg_icn&$filter=_ipg_caseid_value eq " + caseId + " and _customerid_value eq " + carrierLookup[0].id);
      var recentClaim;
      if (claims && claims.entities.length == 1){
        recentClaim = claims.entities[0];
      }
      else if (claims.entities.length > 1) {
        recentClaim = claims.entities[0];
        claims.entities.forEach((element) => {
          if(element.name.substr(element.name.length - 1) > recentClaim.name.substr(recentClaim.name.length - 1)){
            recentClaim = element;
          }
        });
      }
      else {
        throw Error("There is no claims with current Carrier.");
      }
      formParameters["ipg_claimtoreplace"] = recentClaim.name;
      formParameters["ipg_claimicn"] = recentClaim.ipg_icn;
    }
    Xrm.Navigation.openForm(entityFormOptions, formParameters);
  }

  export async function IfCaseHasAssociatedClaim(caseId: string){
    var claims = await Xrm.WebApi.retrieveMultipleRecords("invoice", "?$select=invoiceid&$filter=_ipg_caseid_value eq " + caseId);
    return claims && claims.entities.length > 0;
  }

  export function IsLifeCycleStep(primaryControl: Xrm.FormContext, lifeCycleStepString: string) {
    if (lifeCycleStepString && lifeCycleStepString.length > 0) {
      var lifeCycleStepLookup = primaryControl.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_lifecyclestepid")?.getValue();
      if (lifeCycleStepLookup && lifeCycleStepLookup.length > 0) {
        return lifeCycleStepLookup[0].name === lifeCycleStepString;
      }
    }
  }

  export function showCalcRev(primaryControl: Xrm.FormContext) {
    return lifeCycleStepForCalcRevButton(primaryControl);
  }

  function lifeCycleStepForCalcRevButton(primaryControl: Xrm.FormContext) {
    return (IsLifeCycleStep(primaryControl, "Calculate Revenue") || IsLifeCycleStep(primaryControl, "Generate PO"));
  }

  interface VerifyBenefitsArguments {
    IsUserGenerated: boolean;
  }

  export async function AddExistingOrder(selectedItems, primaryControl: Xrm.FormContext, selectedControl) {
    var partEntityName = selectedItems[0].TypeName;
    var orderAttributeSchemaName = "ipg_PurchaseOrderId";
    if (partEntityName === "ipg_estimatedcasepartdetail") {
      orderAttributeSchemaName = "ipg_purchaseorderid";
    }
    var parts = [];
    var caseId = primaryControl.data.entity.getId().replace('{', '').replace('}','').toLowerCase();
    for (var partItem of selectedItems) {
      var part = await Xrm.WebApi.retrieveRecord(partItem.TypeName, partItem.Id, "?$select=_ipg_purchaseorderid_value&$expand=" + orderAttributeSchemaName + "($select=statuscode,_ipg_caseid_value)");
      if (part._ipg_purchaseorderid_value == null || part[orderAttributeSchemaName]?._ipg_caseid_value !== caseId || part[orderAttributeSchemaName].statuscode === 923720011 /*Voided*/)
      {
        parts.push(partItem);
        console.log(part);
      }
    }

    if (parts.length == 0)
    {
      var alertStrings = { confirmButtonLabel: "OK", text: "Part cannot be added to a PO since it is already on another PO on the Case.", title: "Action Cancelled" };
      var alertOptions = { height: 120, width: 260 };
      Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
      return;
    }
   
    var lookupOptions = 
    {
      defaultEntityType: "salesorder",
      entityTypes: ["salesorder"],
        allowMultiSelect: false,
        disableMru: true,
      filters: [{filterXml: '<filter type="and"><condition attribute="ipg_caseid" operator="eq" uitype="incident" value="' + caseId + '" /></filter>', entityLogicalName: "salesorder"}]
    };

    Xrm.Utility.lookupObjects(lookupOptions).then(
      function(orders){
       
        var data;
        if (partEntityName === "ipg_estimatedcasepartdetail") {
          data =
          {
            "ipg_purchaseorderid@odata.bind" : "/salesorders("+ orders[0].id.replace('{', '').replace('}', '') +")"
          }
        }
        else {
          data =
          {
            "ipg_PurchaseOrderId@odata.bind" : "/salesorders("+ orders[0].id.replace('{', '').replace('}', '') +")",
            "ipg_ischanged": false,
            "ipg_islocked": true
          }
        }
         
        for (var p of parts) {
          Xrm.WebApi.updateRecord(p.TypeName, p.Id, data).then(
              function success(result) {
                  var alertStrings = { confirmButtonLabel: "OK", text: "Associated successfully", title: "Success" };
                  var alertOptions = { height: 120, width: 260 };
                  Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
                  selectedControl.refresh();
                  selectedControl.refreshRibbon(true);
              },
              function (error) {
                  console.log(error.message);
              }
            );
        }
      },
      function(error){console.log(error);});
    return;
  }

  export function RefreshGrid(selectedControl) {
    selectedControl.refresh();
  }

  export function UnlockCase(primaryControl: Xrm.FormContext)
  {
    var pageInput:Xrm.Navigation.PageInputHtmlWebResource = {
      pageType: 'webresource',
      webresourceName: 'ipg_/intake/incident/UnlockCase.html',
      data: primaryControl.data.entity.getId().replace(/[{}]/g, "")
    };

    var navigationOptions:Xrm.Navigation.NavigationOptions = {
        target: 2,
        height: 450,
        width: 600,
        position: 1,       
    };

    Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(() => {
      primaryControl.data.refresh(false);
      primaryControl.ui.refreshRibbon();
    });
  }

  export async function EnableUnlockCase(primaryControl: Xrm.FormContext):Promise<boolean>
  {
    var caseid = primaryControl.data.entity.getId();
    var userId = Xrm.Utility.getGlobalContext().userSettings.userId.replace(/[{}]/g, "").toLowerCase();
    var isAdmin1role = Xrm.Utility.getGlobalContext().userSettings.roles.get(r=>r.name?.toLowerCase() == 'admin1').length > 0;

    var incident = await Xrm.WebApi.retrieveMultipleRecords("incident", `?$select=ipg_islocked&$expand=ipg_FacilityId($select=_ipg_facilitycasemgrid_value)&$filter=(incidentid eq ${caseid})`);

    if(incident.entities[0].ipg_islocked 
      && (isAdmin1role
      || incident.entities[0].ipg_FacilityId?._ipg_facilitycasemgrid_value
      && incident.entities[0].ipg_FacilityId._ipg_facilitycasemgrid_value.replace(/[{}]/g, "").toLowerCase() == userId))
    {
      return true;
    }
    else
    {
      return false;
    }
  }
}
