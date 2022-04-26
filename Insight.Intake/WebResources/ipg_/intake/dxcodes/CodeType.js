function SetDXTitleCode(executionContext) {
    var formContext = executionContext.getFormContext();
    var dxCode = formContext.getAttribute("ipg_dxcode").getValue();
    var dxName = formContext.getAttribute("ipg_dxname").getValue();
    if (dxCode != null && dxName != null) {
        formContext.getAttribute("ipg_name").setValue(dxCode + " - " + dxName);
    }
}
