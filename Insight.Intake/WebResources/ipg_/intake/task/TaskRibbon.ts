/**
 * @namespace Intake.Task
 * 
 */
 namespace Intake.Task {

  /**
   * the function to call custom CloseTaskDialog 
   * @function Intake.Task.OpenCloseTaskDialog
   * @returns {void}
   */
  export function OpenCloseTaskDialog(formContext: Xrm.FormContext) {
    formContext.data.refresh(true).then(p => {
      const data = { taskId: formContext.data.entity.getId()};
      const pageInput: Xrm.Navigation.PageInputHtmlWebResource = {
        pageType: "webresource",
        webresourceName: "ipg_/intake/task/closeTask.html",
        data: JSON.stringify(data)
    };

    const navigationOptions: Xrm.Navigation.NavigationOptions = {
        target: 2,
        width: 600,
        height: 350,
        position: 1,

    };

    const refreshForm = ()=> {
      formContext.ui.refreshRibbon(true);
      formContext.data.refresh(false)
    };
    Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(refreshForm,refreshForm); 
      //Xrm.Navigation.openWebResource(pageInput.webresourceName,  { width: 800, height: 670, openInNewWindow: true } , JSON.stringify(data));
    });
  }

  export function Assign(formContext: Xrm.FormContext, savedqueryId: string) {
    formContext.data.refresh(true).then(p => {
      const data = { taskId: formContext.data.entity.getId(), savedqueryId:savedqueryId};
      const pageInput: Xrm.Navigation.PageInputHtmlWebResource = {
        pageType: "webresource",
        webresourceName: "ipg_/intake/task/assignTask.html",
        data: JSON.stringify(data)
    };

    const navigationOptions: Xrm.Navigation.NavigationOptions = {
        target: 2,
        width: 500,
        height: 220,
        position: 1,

    };

    const refreshForm = ()=> formContext.data.refresh(false);
    Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(refreshForm,refreshForm); 
  });
  
}

  export function OpenRescheduleTaskDialog(formContext: Xrm.FormContext) {
    formContext.data.refresh(true).then(p => {
      const data = { taskId: formContext.data.entity.getId()};      
      Xrm.Navigation.openWebResource("ipg_/intake/task/rescheduleTask.html",  { width: 800, height: 670, openInNewWindow: true } , JSON.stringify(data));
    });
  }

  /**
   * the function to call custom CloseTaskDialog 
   * @function Intake.Task.RunClaimGeneration
   * @returns {void}
   */
  export function RunClaimGeneration(formContext: Xrm.FormContext) {
    const reqObject = {
      getMetadata: function () {
        return {
          boundParameter: null,
          operationType: 0,
          operationName: "ipg_IPGIntakeActionsRunClaimGenerationJob",
          parameterTypes: {
          }
        }
      }
    }

    Xrm.Utility.showProgressIndicator("Running Claim Generation Job...");
    Xrm.WebApi.online.execute(reqObject)
      .then((response) => {
        Xrm.Utility.closeProgressIndicator();
        if (response.ok) {
          return response.json();
        }
        else {
          Xrm.Navigation.openAlertDialog({ text: response.statusText });
          return;
        }
      },
        (error) => {
          Xrm.Utility.closeProgressIndicator();
          console.log(error.message);
          Xrm.Navigation.openAlertDialog({ text: error.message });
        })
      .then((result) => {
        if (result.HasErrors == true) {
          Xrm.Navigation.openAlertDialog({ text: result.Message });
        }
        else {
          console.log("ok");
          formContext.data.refresh(false);
        }
      });
  }

  /**
   * the function to open a manual adjustment 
   * @function Intake.Task.OpenAdjustmentForm
   * @returns {void}
   */
  export function OpenAdjustmentForm(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.getAttribute("regardingobjectid").getValue();
    if (caseId) {
      var entityFormOptions = {};
      entityFormOptions["entityName"] = "ipg_adjustment";

      Xrm.WebApi.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId[0].id), "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
        let formParameters = {};
        formParameters["ipg_caseid"] = Intake.Utility.removeCurlyBraces(caseId[0].id);
        formParameters["ipg_caseidname"] = incident.title;
        formParameters["ipg_caseidtype"] = "incident";
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
  }

  /**
   * return true if a task is a batch generation task 
   * @function Intake.Task.IsBatchGenerationTask
   * @returns {boolean}
   */
  export async function IsBatchGenerationTask(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;

    let taskType = formContext.getAttribute("ipg_tasktypeid").getValue();
    if (taskType) {
      const CMS1500_TASK: string = 'Task.InstitutionalClaimsReadyToPrint';
      var result = await Xrm.WebApi.retrieveMultipleRecords('ipg_globalsetting', `?$select=ipg_value&$filter=ipg_name eq '${CMS1500_TASK}'`);
      if (result.entities.length > 0 && result.entities[0].ipg_value) {
        if (result.entities[0].ipg_value == taskType[0].name) {
          return true;
        }
      }

      const UB04_TASK: string = 'Task.ProfessionalClaimsReadyToPrint';
      var result = await Xrm.WebApi.retrieveMultipleRecords('ipg_globalsetting', `?$select=ipg_value&$filter=ipg_name eq '${UB04_TASK}'`);
      if (result.entities.length > 0 && result.entities[0].ipg_value) {
        if (result.entities[0].ipg_value == taskType[0].name) {
          return true;
        }
      }
    }
    return false;
  }

  /**
    * call Custom action
    * @function Intake.Task.callAction
    * @returns {void}
  */
  function callAction(actionName, parameters, async, successCallback) {
    var req = new XMLHttpRequest();
    req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/" + actionName, async);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {
      if (this.readyState === 4) {
        req.onreadystatechange = null;
        if (this.status === 200 || this.status === 204) {
          if (this.response == "") successCallback()
          else successCallback(JSON.parse(this.response));
        } else {
          Xrm.Utility.closeProgressIndicator();
          alert(JSON.parse(this.response).error.message);
        }
      }
    };
    req.send(JSON.stringify(parameters));
  }

  /**
   * Generates batch
   * @function Intake.Task.GenerateBatch
   * @returns {void}
   */
  export async function GenerateBatch(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;

    let parameters = {
      Target: {
        "activityid": formContext.data.entity.getId(),
        "@odata.type": "Microsoft.Dynamics.CRM." + formContext.data.entity.getEntityName()
      },
      TaskType: formContext.getAttribute("ipg_tasktypeid").getValue()[0].name
    };

    Xrm.Utility.showProgressIndicator("Generating Batch...");
    callAction("ipg_IPGIntakeActionsGenerateClaimsBatch", parameters, true,
      function (result) {
        formContext.data.refresh(true).then(async () => {
          Xrm.Utility.closeProgressIndicator();
          formContext.ui.refreshRibbon();
          let blob = b64toBlob(result.PdfFileBase64, "application/pdf");
          let blobUrl = URL.createObjectURL(blob);
          window.open(blobUrl, "_blank");
        });
      });
  }

  function b64toBlob(b64Data, contentType, sliceSize = 512) {
    let byteCharacters = atob(b64Data);
    let byteArrays = [];

    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
      let slice = byteCharacters.slice(offset, offset + sliceSize);
      let byteNumbers = new Array(slice.length);

      for (var i = 0; i < slice.length; i++) {
        byteNumbers[i] = slice.charCodeAt(i);
      }

      let byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }

    const blob = new Blob(byteArrays, { type: contentType });
    return blob;
  }

  export function OpenMultipleCloseTaskDialog(data, gridcontrol:Xrm.Controls.GridControl) {
      const pageInput: Xrm.Navigation.PageInputHtmlWebResource = {
        pageType: "webresource",
        webresourceName: "ipg_/intake/task/closeTask.html",
        data: JSON.stringify(data)
    };

    const navigationOptions: Xrm.Navigation.NavigationOptions = {
        target: 2,
        width: 550,
        height: 350,
        position: 1,

    };

    let refreshGrid = () =>{gridcontrol.refresh(); gridcontrol.refreshRibbon();};

    Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(refreshGrid,refreshGrid); 
  }

  export function NewDocumentTask(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    var regardingRef = {
        entityType: formContext.data.entity.getEntityName(),
        id: formContext.data.entity.getId()
      };
  
    Xrm.WebApi.retrieveMultipleRecords(
        "ipg_taskcategory",
        "?$filter=ipg_name eq 'Missing Information'"
    ).then(
        (result) => {
            if (result.entities.length > 0)
            {
                var parameters = {};
                parameters["ipg_taskcategoryid"] = result.entities[0]['ipg_taskcategoryid'];
                parameters["ipg_taskcategoryidname"] = "Missing Information";
                parameters["ipg_taskcategoryidtype"] = "ipg_taskcategory";
    
                Xrm.Utility.openQuickCreate('task',regardingRef,parameters);
            }
        },
        (error) => {
          console.log(error);
        }
    )
  }

  export async function OnlyOpenTasksSelected(selectedTaskIds: Array<string>) {
    if (selectedTaskIds && selectedTaskIds.length > 0) {
      var fetchXml = generateFetchXmlToRetrieveTasksByIds(selectedTaskIds);
      var tasks = await Xrm.WebApi.retrieveMultipleRecords("task", fetchXml);
      if (tasks && tasks.entities.length > 0) {
        return tasks.entities.every(function (d) {
          return (d["statecode"] === 0);
        });
      }
    }
    return false;
  }

  function generateFetchXmlToRetrieveTasksByIds(taskIds) {
    var filterValues = "";
    taskIds.forEach((id) => {
      filterValues += "\n<value>" + id + "</value>";
    });
    var fetchXml =
      `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">
                  <entity name="task">
                    <attribute name="subject" />
                    <attribute name="statecode" />
                    <filter type="and">
                      <condition attribute="activityid" operator="in">` +
      filterValues +
      `
                      </condition>
                    </filter>
                  </entity>
                </fetch>`;
    fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
    return fetchXml;
  }
}
