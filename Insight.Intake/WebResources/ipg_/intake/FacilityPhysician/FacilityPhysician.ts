namespace Intake.FacilityPhysician {

    export function OnLoadQuickCreateForm(executionContext: Xrm.Events.EventContext){
        const formContext = executionContext.getFormContext();
        let facilityAttr = formContext.getAttribute("ipg_facilityid");
        let facilityControl = formContext.getControl("ipg_facilityid");
        let physicianAttr = formContext.getAttribute("ipg_physicianid");
        let physicianControl = formContext.getControl("ipg_physicianid");
        if (facilityAttr && facilityControl &&
            physicianAttr && physicianControl){
                if (facilityAttr.getValue()){
                    facilityControl.setVisible(false);
                    physicianControl.setVisible(true);
                }
                else if (physicianAttr.getValue()){
                    facilityControl.setVisible(true);
                    physicianControl.setVisible(false);
                }
                else{
                    facilityControl.setVisible(true);
                    physicianControl.setVisible(true);
                }
            }
    }
}