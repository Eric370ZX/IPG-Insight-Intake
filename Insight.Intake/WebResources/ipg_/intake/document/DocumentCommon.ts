
/**
 * @namespace Intake.Document
 */ 
namespace Intake.Document {

  let xrm: Xrm.XrmStatic = window.Xrm || parent.Xrm;

  /**
   * Open Document Preview HTML web resource.
   * @function Intake.Document.OpenDocumentPreview
   * @returns {void}
   */
  export function previewById(ipgDocumentId: string): void {
    if (ipgDocumentId) {
      let env = Intake.Utility.GetEnvironment();
      let docAppEnvSuffix: string;
      if (env) {
        docAppEnvSuffix = '-' + env;
      }
      else {
        docAppEnvSuffix = '';
      }

      //todo: use single URI in the web app to get rid of the annotation request below
      xrm.WebApi.retrieveMultipleRecords("annotation", "?$filter=_objectid_value eq '" + ipgDocumentId + "'")
        .then((result) => {
          const height: number = 600;
          const width: number = 800;
          
          if (result && result.entities.length) {
            let annotationId = result.entities[0].annotationid;
            xrm.Navigation.openUrl(`https://insight-documents${docAppEnvSuffix}.azurewebsites.net/documents/${annotationId}`, { height: height, width: width });
          }
          else {
            xrm.Navigation.openUrl(`https://insight-documents${docAppEnvSuffix}.azurewebsites.net/legacydocuments/${ipgDocumentId}`, { height: height, width: width });
          }
        },
        (error) => {
          xrm.Navigation.openErrorDialog({ message: error.message });
        });
    }
  }

}
