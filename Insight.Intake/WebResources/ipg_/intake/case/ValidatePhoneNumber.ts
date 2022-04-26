/**
 * @namespace Intake.Case
 */

namespace Intake.Case {
    const patientWorkPhoneKey = 'ipg_patientworkphone';
    const patientHomePhoneKey = 'ipg_patienthomephone';
    const patientCellPhoneKey = 'ipg_patientcellphone';

    const patientWorkPhoneAttr: Xrm.Attributes.Attribute = Xrm.Page.getAttribute(patientWorkPhoneKey);
    const patientHomePhoneAttr: Xrm.Attributes.Attribute = Xrm.Page.getAttribute(patientHomePhoneKey);
    const patientCellPhoneAttr: Xrm.Attributes.Attribute = Xrm.Page.getAttribute(patientCellPhoneKey);

    /**
     * Called when the form is loaded. See D365 configuration for details.
     * @function Intake.Case.ValidatePhoneNumber
     * @returns {void}
     */
    export function ValidatePhoneNumber(): void {
        
        function changeListener(): void {
            var workPhone = patientWorkPhoneAttr.getValue();
            var homePhone = patientHomePhoneAttr.getValue();
            var cellPhone = patientCellPhoneAttr.getValue();

            if (workPhone == null && homePhone == null && cellPhone == null) {
                patientWorkPhoneAttr.setRequiredLevel("required");
                patientHomePhoneAttr.setRequiredLevel("required");
                patientCellPhoneAttr.setRequiredLevel("required");
            }
            else {
                patientWorkPhoneAttr.setRequiredLevel("none");
                patientHomePhoneAttr.setRequiredLevel("none");
                patientCellPhoneAttr.setRequiredLevel("none");
            }
        }
        
        [patientWorkPhoneAttr, patientHomePhoneAttr, patientCellPhoneAttr].forEach(x => x.addOnChange(changeListener));

        ValidatePhoneNumber();
    }
}
