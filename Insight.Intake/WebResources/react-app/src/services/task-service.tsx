import { TaskFactory } from "../factories/task-factory";
import { ITask } from "../models/ITask";
import { BaseService } from "./base-service";
export class TaskService extends BaseService
{
    constructor(){
        super("task",
        [
            "activityid"
        ], new TaskFactory()
        )
    }

    async CreateTaskByTaskType(taskTypeid:string,caseid:string)
    {
        var openTask = await super.retrieve(`$top=1&$select=activityid&$filter=_regardingobjectid_value eq ${caseid} and _ipg_tasktypeid_value eq ${taskTypeid} and statecode eq 0`)
        
        if((openTask?.length || 0) == 0)
        {
            var task:ITask = {};
            task["ipg_tasktypeid_Task@odata.bind"] = `/ipg_tasktypes(${taskTypeid})`;
            task["regardingobjectid_incident@odata.bind"] = `/incidents(${caseid})`;

            await super.create(task);
        }
    }

    async getTaskTypeIdByName(taskTypeName:string):Promise<string>
    {
        var tasktypes = await super.retrieveentity('ipg_tasktypes',`$top=1&$select=ipg_tasktypeid&$filter=ipg_name eq '${taskTypeName}' and statecode eq 0`);
        return tasktypes.length > 0 && tasktypes[0]["ipg_tasktypeid"] || "";
    }
}