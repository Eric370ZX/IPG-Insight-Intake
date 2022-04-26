import { ITask } from "../models/ITask";
import {IFactory} from "./factory";

export class TaskFactory implements IFactory {
    getFromWebApi(record: any): ITask {
        let task: ITask = record as ITask;

        task.id = task.activityid;
      
        return task;
    }
}