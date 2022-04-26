namespace Intake.AuthorizationRule {
  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();

    let expirationdate = new Date('12/31/9999');
    let todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    if (!formContext.getAttribute("ipg_effectivedate").getValue()) {
      formContext.getAttribute("ipg_effectivedate").setValue(todayDate);
    }
    if (!formContext.getAttribute("ipg_expirationdate").getValue()) {
      formContext.getAttribute("ipg_expirationdate").setValue(expirationdate);
    }

    UpdateRuleType(executionContext);
  }

  export function SetAuthorizationNonCPTRuleName(executionContext) {


    let formContext = executionContext.getFormContext();
    let entity = formContext.data.entity.getEntityName();
    let ruleName;
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

  function UpdateRuleType(executionContext) {
    let formContext = executionContext.getFormContext();
    let entity;
    let ruleType;
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
    let formContext = executionContext.getFormContext();

    let healthPlanNetwork = Xrm.Page.getAttribute("ipg_carriernetworksid").getValue();
    if (healthPlanNetwork) {
      let healthPlanNetworkId = healthPlanNetwork[0].id.replace("{", "").replace("}", "");
      Xrm.WebApi.retrieveRecord("ipg_carriernetwork", healthPlanNetworkId, "?$select=ipg_name").then(function success(carrResults) {
        if (carrResults != null) {
          let carrier = carrResults["ipg_name"]
          let ruleName = carrier + " - " + cptCode;
          formContext.getAttribute("ipg_name").setValue(ruleName);
        }
      }, function (error) {

      });
    }
  }

  function UpdateCarrierCPTRuleName(executionContext, cptCode) {
    let formContext = executionContext.getFormContext();

    let carrier = Xrm.Page.getAttribute("ipg_carrieridnew").getValue();
    if (carrier) {
      let carrierId = carrier[0].id.replace("{", "").replace("}", "");
      Xrm.WebApi.retrieveRecord("account", carrierId, "?$select=name").then(function success(carrResults) {
        if (carrResults != null) {
          let carrier = carrResults["name"]
          let ruleName = carrier + " - " + cptCode;
          formContext.getAttribute("ipg_name").setValue(ruleName);
        }
      }, function (error) {

      });
    }
  }

  function UpdateHighDollarAcuityCPTRuleName(executionContext, cptCode) {
    let formContext = executionContext.getFormContext();
    let ruleName = cptCode + " - " + "High Acuity";
    formContext.getAttribute("ipg_name").setValue(ruleName);
  }

  function UpdatePropelCPTRuleName(executionContext, cptCode) {
    let formContext = executionContext.getFormContext();
    let ruleName = "Propel CPT" + " - " + cptCode;
    formContext.getAttribute("ipg_name").setValue(ruleName);
    formContext.getControl("ipg_name").setDisabled(true);
  }

  function UpdateRuleName(executionContext, cptCodeFieldName, updateRuleNameFunction) {
    let cptCodeObject = Xrm.Page.getAttribute(cptCodeFieldName).getValue();
    if (cptCodeObject) {
      let guid = cptCodeObject[0].id.replace("{", "").replace("}", "");
      if (cptCodeObject != null) {
        Xrm.WebApi.retrieveRecord("ipg_cptcode", guid, "?$select=ipg_cptcode").then(function success(results) {
          if (results != null) {
            let cptCode = results["ipg_cptcode"];
            updateRuleNameFunction(executionContext, cptCode);
          }
        }, function (error) {

        });
      }
    }
  }

  export function SetAuthorizationRuleName(executionContext) {

    let formContext = executionContext.getFormContext();
    let entity = formContext.data.entity.getEntityName();

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
}


