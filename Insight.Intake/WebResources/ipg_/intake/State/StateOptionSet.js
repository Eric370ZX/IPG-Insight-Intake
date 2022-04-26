function StatesExempt(formContext) {
    var optionText = "Exempt";
    var stateFieldNames = new Array("ipg_permimplanttaxtype", "ipg_extcomponenttaxtype", "ipg_surgicaltooltaxtype", "ipg_tempimplanttaxtype", "ipg_medicalsupplytaxtype", "ipg_extaccessorytaxtype", "ipg_kittaxtype");
    var options;
    for (var i = 0; i < stateFieldNames.length; i++) {
        options = stateFieldNames[i];
        var optionSet = Xrm.Page.getAttribute(options).getOptions();
        for (var ii = 0; ii < optionSet.length; ii++) {
            if (optionSet[ii].text == optionText)
                Xrm.Page.getControl(options).removeOption(optionSet[ii].value);
        }
    }
}
