/**
 * @namespace Intake.Document
 */
namespace Intake.Document {

  declare let $: typeof import("jquery");
  if (typeof ($) === 'undefined') {
    $ = (<any>window.parent).$;
  }

  /**
   * Sets Inline Content Disposition for Note (Annotation entity) attachments. They will open in a new window from Insight Documents web service.
   * @function Intake.Document.SetInlineContentDisposition
   * @returns {void}
   */
  export function SetInlineContentDisposition() {
    const attachmentItemId = 'notescontrol-timeline_record_attachment_Item';

    $((window.parent || window).document).find('#editFormRoot').on('click', `[id*=${attachmentItemId}]`, function (ev) {
      let env = Intake.Utility.GetEnvironment();
      let envSuffix: string;
      if (env) {
        envSuffix = '-' + env;
      }
      else {
        envSuffix = '';
      }

      let docId = $(ev.currentTarget).data('id').replace(attachmentItemId, '');

      if (docId) {
        ev.stopPropagation();
        ev.preventDefault();

        window.open(`https://insight-documents${envSuffix}.azurewebsites.net/documents/${docId}`, '_blank');
      }
    });
  }
}
