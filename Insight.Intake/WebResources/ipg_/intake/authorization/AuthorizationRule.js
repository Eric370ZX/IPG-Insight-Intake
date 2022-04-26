var Intake;
(function (Intake) {
    var AuthorizationRule;
    (function (AuthorizationRule) {
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var expirationdate = new Date('12/31/9999');
            var todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
            if (!formContext.getAttribute("ipg_effectivedate").getValue()) {
                formContext.getAttribute("ipg_effectivedate").setValue(todayDate);
            }
            if (!formContext.getAttribute("ipg_expirationdate").getValue()) {
                formContext.getAttribute("ipg_expirationdate").setValue(expirationdate);
            }
            UpdateRuleType(executionContext);
        }
        AuthorizationRule.OnLoadForm = OnLoadForm;
        function SetAuthorizationNonCPTRuleName(executionContext) {
            var formContext = executionContext.getFormContext();
            var entity = formContext.data.entity.getEntityName();
            var ruleName;
            switch (entity) {
                case "ipg_carriernamerule":
                    var carrier = formContext.getAttribute("ipg_carrierid").getValue();
                    if (carrier) {
                        ruleName = carrier[0].name + "  - Carrier Name";
                    }
                    break;
                case "ipg_carriertyperule":
                    var carrierType = formContext.getAttribute("ipg_carriertypenew");
                    var optionText = carrierType.getText();
                    ruleName = optionText + " - Carrier Type";
                    break;
                case "ipg_caseplantyperule":
                    var benefitType = formContext.getAttribute("ipg_benefitplantype");
                    var optionText = benefitType.getText();
                    ruleName = optionText + " - Case Plan Type";
                    break;
                case "ipg_memberidprefixrule":
                    ruleName = formContext.getAttribute("ipg_prefix").getValue() + " - Member ID Prefix";
                    break;
                case "ipg_propelprocedurerule":
                    var procedure = formContext.getAttribute("ipg_procedurenameid").getValue();
                    if (procedure) {
                        ruleName = procedure[0].name + " - Propel Procedure";
                    }
                    break;
            }
            formContext.getAttribute("ipg_name").setValue(ruleName);
        }
        AuthorizationRule.SetAuthorizationNonCPTRuleName = SetAuthorizationNonCPTRuleName;
        function UpdateRuleType(executionContext) {
            var formContext = executionContext.getFormContext();
            var entity;
            var ruleType;
            entity = formContext.data.entity.getEntityName();
            switch (entity) {
                case "ipg_carriernetworkcptrule":
                    ruleType = "Carrier Network CPT";
                    break;
                case "ipg_carriercptrule":
                    ruleType = "Carrier CPT";
                    break;
                case "ipg_carriernamerule":
                    ruleType = "Carrier Name";
                    break;
                case "ipg_carriertyperule":
                    ruleType = "Carrier Type";
                    break;
                case "ipg_carrierplantypes":
                    ruleType = "Carrier Plan Type";
                    break;
                case "ipg_memberidprefixrule":
                    ruleType = "Member ID Prefix";
                    break;
                case "ipg_propelcptrule":
                    ruleType = "Propel CPT";
                    break;
                case "ipg_propelprocedurerule":
                    ruleType = "Propel Procedure";
                    break;
                case "ipg_propelprocedurerule":
                    ruleType = "Propel Procedure";
                    break;
                case "ipg_caseplantyperule":
                    ruleType = "Case Plan Type";
                    break;
            }
            if (ruleType != null) {
                formContext.getAttribute("ipg_ruletype").setValue(ruleType);
            }
        }
        function UpdateCarrierNetworkRuleName(executionContext, cptCode) {
            var formContext = executionContext.getFormContext();
            var healthPlanNetwork = Xrm.Page.getAttribute("ipg_carriernetworksid").getValue();
            if (healthPlanNetwork) {
                var healthPlanNetworkId = healthPlanNetwork[0].id.replace("{", "").replace("}", "");
                Xrm.WebApi.retrieveRecord("ipg_carriernetwork", healthPlanNetworkId, "?$select=ipg_name").then(function success(carrResults) {
                    if (carrResults != null) {
                        var carrier = carrResults["ipg_name"];
                        var ruleName = carrier + " - " + cptCode;
                        formContext.getAttribute("ipg_name").setValue(ruleName);
                    }
                }, function (error) {
                });
            }
        }
        function UpdateCarrierCPTRuleName(executionContext, cptCode) {
            var formContext = executionContext.getFormContext();
            var carrier = Xrm.Page.getAttribute("ipg_carrieridnew").getValue();
            if (carrier) {
                var carrierId = carrier[0].id.replace("{", "").replace("}", "");
                Xrm.WebApi.retrieveRecord("account", carrierId, "?$select=name").then(function success(carrResults) {
                    if (carrResults != null) {
                        var carrier_1 = carrResults["name"];
                        var ruleName = carrier_1 + " - " + cptCode;
                        formContext.getAttribute("ipg_name").setValue(ruleName);
                    }
                }, function (error) {
                });
            }
        }
        function UpdateHighDollarAcuityCPTRuleName(executionContext, cptCode) {
            var formContext = executionContext.getFormContext();
            var ruleName = cptCode + " - " + "High Acuity";
            formContext.getAttribute("ipg_name").setValue(ruleName);
        }
        function UpdatePropelCPTRuleName(executionContext, cptCode) {
            var formContext = executionContext.getFormContext();
            var ruleName = "Propel CPT" + " - " + cptCode;
            formContext.getAttribute("ipg_name").setValue(ruleName);
            formContext.getControl("ipg_name").setDisabled(true);
        }
        function UpdateRuleName(executionContext, cptCodeFieldName, updateRuleNameFunction) {
            var cptCodeObject = Xrm.Page.getAttribute(cptCodeFieldName).getValue();
            if (cptCodeObject) {
                var guid = cptCodeObject[0].id.replace("{", "").replace("}", "");
                if (cptCodeObject != null) {
                    Xrm.WebApi.retrieveRecord("ipg_cptcode", guid, "?$select=ipg_cptcode").then(function success(results) {
                        if (results != null) {
                            var cptCode = results["ipg_cptcode"];
                            updateRuleNameFunction(executionContext, cptCode);
                        }
                    }, function (error) {
                    });
                }
            }
        }
        function SetAuthorizationRuleName(executionContext) {
            var formContext = executionContext.getFormContext();
            var entity = formContext.data.entity.getEntityName();
            switch (entity) {
                case "ipg_carriernetworkcptrule":
                    UpdateRuleName(executionContext, "ipg_cptcodeid", UpdateCarrierNetworkRuleName);
                    break;
                case "ipg_carriercptrule":
                    UpdateRuleName(executionContext, "ipg_cptcodeid", UpdateCarrierCPTRuleName);
                    break;
                case "ipg_highdollaracuitycptrule":
                    UpdateRuleName(executionContext, "ipg_cptcodeid", UpdateHighDollarAcuityCPTRuleName);
                    break;
                case "ipg_propelcptrule":
                    UpdateRuleName(executionContext, "ipg_cptcodeid", UpdatePropelCPTRuleName);
                    break;
            }
        }
        AuthorizationRule.SetAuthorizationRuleName = SetAuthorizationRuleName;
    })(AuthorizationRule = Intake.AuthorizationRule || (Intake.AuthorizationRule = {}));
})(Intake || (Intake = {}));
