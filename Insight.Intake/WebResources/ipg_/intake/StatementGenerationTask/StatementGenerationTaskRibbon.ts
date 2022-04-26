/**
 * @namespace Intake.StatementGenerationTask
 */
 namespace Intake.StatementGenerationTask {

    export async function OpenDocumentPreview(statementGenerationTaskId: string): Promise<void> {
        let currentTaskId: string = statementGenerationTaskId.replace((/[{|}]/g), '');
        if (currentTaskId){
            let statementGenerationTask = await Xrm.WebApi.retrieveRecord("ipg_statementgenerationtask", currentTaskId, "?$select=_ipg_documentid_value");
            if (statementGenerationTask && statementGenerationTask._ipg_documentid_value){
                await Intake.Document.OpenDocumentPreview(null, statementGenerationTask._ipg_documentid_value);
            }
            else{
                Xrm.Navigation.openAlertDialog({text: 'Statement Generation Task doesn`t have document.' });
            }
        }
    }

    export async function IsEnableToPreviewDocument(statementGenerationTaskId: string): Promise<boolean> {
        let currentTaskId: string = statementGenerationTaskId.replace((/[{|}]/g), '');
        if (currentTaskId){
            let statementGenerationTask = await Xrm.WebApi.retrieveRecord("ipg_statementgenerationtask", currentTaskId, "?$select=_ipg_documentid_value");
            if (statementGenerationTask && statementGenerationTask._ipg_documentid_value){
                return true;
            }
        }
        return false;
    }
 }
