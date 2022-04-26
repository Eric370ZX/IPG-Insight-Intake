namespace Intake.Task.AssignTask {
    interface taskent{ taskid:string, ownerid: string, taskreasonid: string };
    const Xrm =  window.parent.Xrm;
    const data = JSON.parse(getUrlParameter("data"));
    
    export async function init() {
      Xrm.Utility.showProgressIndicator("");
      await initUsers();
      await initTaskReasons();
      Xrm.Utility.closeProgressIndicator();
    }
  
  
  
    async function  initUsers() {
        const users =  await  Xrm.WebApi.retrieveMultipleRecords('systemuser', `?savedQuery=${data.savedqueryId}`);
      
        var $selectWrapper = $('#assignTo');
        var $selecet = $('select', $selectWrapper);
        $selecet.append($("<option/>", { value: 0, text: '' }));
        users.entities.map(function (user) { return $selecet.append($("<option/>", { value: user.systemuserid, text: user.fullname })); });
        $selectWrapper.css('display', 'flex'); 
        }
  
        async function  initTaskReasons() {
          const task =  await  Xrm.WebApi.retrieveRecord('task', data.taskId, '?$select=_ipg_taskcategoryid_value');
          
          if (task['_ipg_taskcategoryid_value@OData.Community.Display.V1.FormattedValue'])
          {
                const assigningCode= 427880000;
                const taskreasons = await Xrm.WebApi.retrieveMultipleRecords('ipg_taskreason', `?$select=ipg_name&$filter=_ipg_taskcategory_value eq ${task['_ipg_taskcategoryid_value']} and Microsoft.Dynamics.CRM.ContainValues(PropertyName=@p1,PropertyValues=@p2)&@p1='ipg_usedbyactioncodes'&@p2='${assigningCode}'`);       
                if(taskreasons.entities.length > 0)
                {
                  var $selectWrapper = $('#taskreasonid');
                  var $selecet = $('select', $selectWrapper);
                  $selecet.append($("<option/>", { value: 0, text: '' }));
                  taskreasons.entities.map(function (taskReason) { return $selecet.append($("<option/>", { value: taskReason.ipg_taskreasonid, text: taskReason.ipg_name })); });
                  $selectWrapper.css('display', 'flex');
                }
              }}

    export function ProcessAssignTask() {
      const user = $('#assignTo>div>select').val() as string;
  
      if(!user || user == '0')
      {
        Xrm.Navigation.openAlertDialog({text:"You need to select User", title: "Validation ERROR"});
        return;
      }
      
      const taskreason = $('#taskreasonid>div>select').val() as string;
  
      const onSuccess = () => {
        Xrm.Utility.closeProgressIndicator();
        Xrm.Navigation.openAlertDialog({text:"Task Assigned successfully"}).then(() => {
          window.close();
        });
      };

      var task:taskent = {taskid:data.taskId, ownerid: user, taskreasonid: taskreason}

      Xrm.Utility.showProgressIndicator("");
      assignTaskRequest(task, onSuccess, (message) => {
        Xrm.Utility.closeProgressIndicator(); 
        Xrm.Navigation.openAlertDialog({text:`Error while Assigning the task:${message}`});
      });
    }
    export function CloseTaskWindow() {
      window.close();
    }
  
    function assignTaskRequest(task: taskent, onSuccess: () => void, onError: (message: string) => void) {    
      var taskupdateObj = {"ownerid@odata.bind":`/systemusers(${task.ownerid})`};
      
      if(task.taskreasonid && task.taskreasonid != '0')
      {
        taskupdateObj["ipg_taskreason_Task@odata.bind"] = `/ipg_taskreasons(${task.taskreasonid})`;
      }
      Xrm.WebApi.online.updateRecord("task",task.taskid, taskupdateObj).then(
        function success(result) {
          if (result.ok || result.id) {
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
    $("#okBtn").on('click',Intake.Task.AssignTask.ProcessAssignTask);
    $("#cancelBtn").on('click',Intake.Task.AssignTask.CloseTaskWindow);
    Intake.Task.AssignTask.init();
  });
