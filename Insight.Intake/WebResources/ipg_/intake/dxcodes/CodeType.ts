function SetDXTitleCode(executionContext) {
  let formContext = executionContext.getFormContext();
  let dxCode = formContext.getAttribute("ipg_dxcode").getValue();
  let dxName = formContext.getAttribute("ipg_dxname").getValue();

  if (dxCode != null && dxName != null) {
    formContext.getAttribute("ipg_name").setValue(dxCode + " - " + dxName);
  }

}
