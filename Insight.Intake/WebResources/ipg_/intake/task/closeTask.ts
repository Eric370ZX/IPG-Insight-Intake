namespace Intake.Task.CloseTask {
  interface closeTaskRequest
  {
    taskid:string;
    closeReason: number;
    taskReason?: string; 
    closeNote: string; 
    produceTaskNote: boolean;
  }

  interface entityREf{ entityType:string,  id: string }

  enum CloseReason {
    Resolved = 5,
    Cancelled = 923720001,
    ExceptionApproved = 923720000
  }

  const Xrm =  window.parent.Xrm;
  const data = JSON.parse(getUrlParameter("data"));
  const taskId = data.taskId as string;
  var multiple = false;
  let tasks = [];
    if (taskId !== null && taskId !== undefined) {
      tasks.push(taskId);
    }
    else if (data.length > 0) {
      tasks = data;
      multiple = true;
    }

    for (let i =0; i < tasks.length; i++){
      tasks[i] = tasks[i].replace('{', '').replace('}', '');
    }
  
  let isOutreachTaskClosure = false;
  let istaskReasoReq = false;
  
  export async function init() {
    await initCloseReasons();
    if (taskId !== null && taskId !== undefined) {
      await initTaskReasons();
    }
    else {
      await initAllTaskReasons();
    }
    Intake.Task.CloseTask.CloseReasonChange();
  }


  async function initCloseReasons()
  {
    var supervisorrole = Xrm.Utility.getGlobalContext().userSettings.roles.get(r => r.name.trim().toLowerCase() == "supervisor");
    
    if(await Xrm.Utility.getGlobalContext().getCurrentAppName() != "Collections" || supervisorrole.length > 0)
    {
      $("#closeReason > option:nth-child(2)").css("display","block");
      $("#closeReason > option:nth-child(3)").css("display","block");
    }

    $("#closeReason").trigger("change");
  }
  async function  initTaskReasons() {
    const task =  await  Xrm.WebApi.retrieveRecord('task', taskId, '?$select=_ipg_taskcategoryid_value');
    
    if (task['_ipg_taskcategoryid_value@OData.Community.Display.V1.FormattedValue'])
    {
      isOutreachTaskClosure = (task['_ipg_taskcategoryid_value@OData.Community.Display.V1.FormattedValue'] as string).toLowerCase().indexOf("outreach") > -1;
      const taskreasons = await Xrm.WebApi.retrieveMultipleRecords('ipg_taskreason', "?$select=ipg_name&$filter=ipg_usedbyactioncodes eq null and (_ipg_taskcategory_value eq " + task['_ipg_taskcategoryid_value'] + " or _ipg_taskcategory_value eq null)");       
  
      if (taskreasons.entities.length > 0)
      {
        var $selectWrapper = $('#taskreasonid');
        taskreasons.entities.map(function (taskReason) { return $selectWrapper.append($("<option/>", { value: taskReason.ipg_taskreasonid, text: taskReason.ipg_name })); });
      }
    }
  }

  async function  initAllTaskReasons() {

    var tasksCondition = tasks.map(t => `<value>${t}</value>`);
    
    var fetchXml = [
  "<fetch distinct='true'>",
  "  <entity name='ipg_taskreason'>",
  "    <attribute name='ipg_name' />",
  "    <attribute name='ipg_taskreasonid' />",
  "    <filter>",
  "      <condition attribute='ipg_usedbyactioncodes' operator='null' />",
  "      <filter type='or'>",
  "        <condition attribute='ipg_taskcategory' operator='null' />",
  "        <condition entityname='task' attribute='activityid' operator='in'>",tasksCondition,
  "        </condition>",
  "      </filter>",
  "    </filter>",
  "    <link-entity name='task' from='ipg_taskcategoryid' to='ipg_taskcategory' link-type='outer' alias='task' />",
  "  </entity>",
  "</fetch>",
    ].join("");

    const taskreasons = await Xrm.WebApi.retrieveMultipleRecords('ipg_taskreason', `?fetchXml=${encodeURIComponent(fetchXml)}`);       
    if (taskreasons.entities.length > 0)
    {
      var $selectWrapper = $('#taskreasonid');
      taskreasons.entities.map(function (taskReason) { return $selectWrapper.append($("<option/>", { value: taskReason.ipg_taskreasonid, text: taskReason.ipg_name })); });
    }
  }

  export function ProcessCloseTask() {
    const closeReason = $("#closeReason").val();
    const closeNote = $("#closeNote").val();
    const taskReason = $('#taskreasonid').val();
    if (!closeReason || closeNote == '') {
      Xrm.Navigation.openAlertDialog({text:"You need to populate mandatory fields", title: "Validation ERROR"});
      return;
    }

    const req:closeTaskRequest = 
    {
      taskid:null,
      closeReason:closeReason as number,
      closeNote: closeNote as string,
      produceTaskNote: true
    };

    if((!taskReason || taskReason == '0')  && istaskReasoReq)
    {
      Xrm.Navigation.openAlertDialog({text:"You need to populate Task Reason", title: "Validation ERROR"});
      return;
    }
    else {
      req.taskReason = taskReason != '0' ? taskReason as string : null;
    }

    let ids = tasks.join(" or activityid eq ") + ')';
    Xrm.WebApi.retrieveMultipleRecords('task', '?$select=activityid, subject&$filter=statecode eq 0 and (activityid eq ' + ids).then((openTasks) => {
      if (!(openTasks?.entities?.length > 0)) {
        Xrm.Navigation.openConfirmDialog({text:"All selected Tasks are already closed"}).then(() => {
          window.close();
        });
        return;
      }
      for (let i = 0; i < openTasks.entities.length; i++) { 
        req.taskid = openTasks.entities[i].activityid as string;
        const onSuccess = () => {
          Xrm.Utility.closeProgressIndicator();
          Xrm.Navigation.openAlertDialog({text:"Task '" + openTasks.entities[i].subject + "' closed succesfully"}).then(() => {
            window.close();
          });
        };
    
        Xrm.Utility.showProgressIndicator("Closing...");
        closeTaskRequest(req, onSuccess, (message) => {
          Xrm.Utility.closeProgressIndicator(); 
          Xrm.Navigation.openErrorDialog({message:`Error while closing the task:${message}`})
        });
    }}, (e) => {console.log(e)});       
  }
  export function CloseTaskWindow() {
    window.close();
  }

  export function CloseReasonChange() {    
    var $selectWrapper = $('#taskReason');
    const closeReason = $("#closeReason").val();
    var taskReason  = $('#taskreasonid')?.[0] as HTMLSelectElement
    var taskReasonLbl = $('#lbltaskReason')?.[0]
    if (closeReason && closeReason == CloseReason.Cancelled || isOutreachTaskClosure || multiple) {
      $selectWrapper.css("display","flex");
      istaskReasoReq = true;
      if (closeReason == CloseReason.Cancelled) {
        for (let i = 0; i< taskReason.options.length; i++) {
            if (taskReason.options[i].text === "Not Needed") {
              taskReason.value = taskReason.options[i].value;
            }
        }
      }
      return;
    }
    else {
      $selectWrapper.css("display", "none");
      $('#taskreasonid option[value=0]').prop('selected', 'selected').trigger("change");
      istaskReasoReq = false;
      taskReasonLbl.innerText = taskReasonLbl.innerText.replace('*', '')
      return;
    }
  }

  function closeTaskRequest(req: closeTaskRequest, onSuccess: () => void, onError: (message: string) => void) {
    const target: { entityType:string,  id: string } = {
      entityType: "task",
      id: req.taskid
    };    
    const parameters: { Target: any, CloseReason: number, CloseNote: string, ProduceTaskNote: boolean, TaskReason?:entityREf } = {
      Target: target,
      CloseReason: req.closeReason,
      CloseNote: req.closeNote,
      ProduceTaskNote: req.produceTaskNote
    };

     parameters.TaskReason = req.taskReason ? {entityType: "ipg_taskreason", id: req.taskReason} : null;

    var ipg_IPGTaskActionsCloseTaskRequest = {
      entity: parameters.Target,
      CloseReason: parameters.CloseReason,
      CloseNote: parameters.CloseNote,
      ProduceTaskNote: parameters.ProduceTaskNote,
      TaskReason : parameters.TaskReason,
      getMetadata: function () {
        return {
          boundParameter: "entity",
          parameterTypes: {
            entity: {
              "typeName": "mscrm.task",
              "structuralProperty": 5
            },
            "CloseReason": {
              "typeName": "Edm.Int32",
              "structuralProperty": 1
            },
            "CloseNote": {
              "typeName": "Edm.String",
              "structuralProperty": 1
            },
            "ProduceTaskNote": {
              "typeName": "Edm.Boolean",
              "structuralProperty": 1
            },
            "TaskReason": {
              "typeName": "mscrm.ipg_taskreason",
              "structuralProperty": 5
            }
          },
          operationType: 0,
          operationName: "ipg_IPGTaskActionsCloseTask"
        };
      }
    };

    Xrm.WebApi.online.execute(ipg_IPGTaskActionsCloseTaskRequest).then(
      function success(result) {
        if (result.ok) {
          onSuccess();
        }
      },
      function (error) {
        onError(error.message);
      }
    );
  }
  function getUrlParameter(sParam) {
    let sPageURL = window.location.search.substring(1),
      sURLVariables = sPageURL.split('&'),
      sParameterName,
      i;

    for (i = 0; i < sURLVariables.length; i++) {
      sParameterName = sURLVariables[i].split('=');

      if (sParameterName[0] === sParam) {
        return sParameterName[1] === undefined ? "" : decodeURIComponent(sParameterName[1]);
      }
    }
  };
}
$(() => {
  $("#okBtn").on('click',Intake.Task.CloseTask.ProcessCloseTask);
  $("#cancelBtn").on('click', Intake.Task.CloseTask.CloseTaskWindow);
  $("#closeReason").on('change', Intake.Task.CloseTask.CloseReasonChange);
  Intake.Task.CloseTask.init();
});
