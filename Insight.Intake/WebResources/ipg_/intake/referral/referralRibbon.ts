/**
* @namespace Intake.Referral.Ribbon
*/
namespace Intake.Referral.Ribbon {
  let CaseStatusesEnum = {
    Close: 923720001,
    Open: 923720000,
  };
  let CaseReasonsEnum = {
    Rejected: 923720002,
  };

  let ipg_CaseOutcomeCodes = {
    Gate1Fail: 427880000,
    Gate2Fail: 427880002,
    GateEhrFail: 427880020
  };

  let StructualPropertyTypes = {
    Unknown: 0,
    PrimitiveType: 1,
    ComplexType: 2,
    EnumerationType: 3,
    Collection: 4,
    EntityType: 5,
  }

  /**
  * Enable rule for Reopen Referral button on referral form
  * @function Intake.Referral.Ribbon.IsEnabledAddExistingButton
  */

  export async function IsEnabledAddExistingButton(firstPrimaryItemId, primaryEntityTypeName, selectedControl, allowedMultipleAssociationDocTypes: string) {
    if (primaryEntityTypeName === "ipg_document") {
      if (firstPrimaryItemId.replace("{", "").replace("}", "")) {
        try {
          var document = await Xrm.WebApi.retrieveRecord(
            "ipg_document",
            firstPrimaryItemId,
            "?$select=_ipg_referralid_value,_ipg_caseid_value,ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation, ipg_name)"
          );
          let relatedEntitiesCount = selectedControl.getGrid().getTotalRecordCount();
          var docTypesAbbr = allowedMultipleAssociationDocTypes.split(',');
          if(document.ipg_DocumentTypeId?.ipg_documenttypeabbreviation === 'PIF')
          {
            return false;
          }
          return isDocumentTypeAllowslMultipleCaseOrReferralAssociation(document, relatedEntitiesCount, docTypesAbbr);
        } catch (error) {
          Xrm.Navigation.openAlertDialog(error?.message);
        }
      }
    }
    return true;
  }

  function isDocumentTypeAllowslMultipleCaseOrReferralAssociation(result, relatedEntitiesCount, docTypesAbbr: string[]) {
    return docTypesAbbr.find(abbr => abbr === result.ipg_DocumentTypeId?.ipg_documenttypeabbreviation)
      || (result._ipg_referralid_value == null && result._ipg_caseid_value == null && relatedEntitiesCount === 0  
      && result.ipg_DocumentTypeId?.ipg_documenttypeabbreviation != "FAX" && result.ipg_DocumentTypeId?.ipg_name != "User Uploaded Generic Document");
  }


  /**
  * Enable rule for Reopen Referral button on referral form
  * @function Intake.Referral.Ribbon.ShowReopenReferralButtonOnForm
  */
  export function ShowReopenReferralButtonOnForm(primaryControl) {
    return false; //this button is deprecated
  }

  /**
  * Enable rule for Reopen Referral button
  * @function Intake.Referral.Ribbon.ShowReopenReferralButton
  */
  export function ShowReopenReferralButton() {
    return false; //this button is deprecated
  }

  /**
  * Enable rule for Reopen Referral button on referral form
  * @function Intake.Referral.Ribbon.IsEnabledReopenRefferalButton
  */
  export function IsEnabledReopenRefferalButton(primaryControl) {
    return false; //this button is deprecated
  }

  /**
  * Enable rule for Reopen Referral button
  * @function Intake.Referral.Ribbon.EnableReopenRefferalButton
  */
  export function EnableReopenRefferalButton(referralId) {
    return false; //this button is deprecated
  }

  /**
  * Called on Reopen Referral button click on referral form
  * @function Intake.Referral.Ribbon.OnReopenReferralClickOnForm
  * @returns {void}
  */
  export function OnReopenReferralClickOnForm(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let referralId = formContext.data.entity.getId();
    OnReopenReferralClick(referralId);
  }

  /**
  * Called on Reopen Referral button click
  * @function Intake.Referral.Ribbon.OnReopenReferralClick
  * @returns {void}
  */
  export function OnReopenReferralClick(selectedReferralId) {
    Xrm.WebApi.retrieveRecord(
      "ipg_referral",
      selectedReferralId.replace("{", "").replace("}", "").toLowerCase(),
      "?$select=_ipg_associatedcaseid_value, ipg_casestatus"
    )
      .then(function success(referral) {
        if (referral) {
          let referralStatus = referral?.ipg_casestatus;
          let assosiatedCase = referral?._ipg_associatedcaseid_value;
          if (
            referralStatus === CaseStatusesEnum.Close &&
            assosiatedCase != null
          ) {
            Xrm.Navigation.openAlertDialog({
              text: "Referral cannot be re-open",
            });
            return;
          }
          return referral;
        }
      })
      .then((referral) => {
        if (referral) {
          let reqObject = {
            Referral: {
              "@odata.type": "Microsoft.Dynamics.CRM.ipg_referral",
              ipg_referralid: referral.ipg_referralid,
            },
            getMetadata: function () {
              return {
                boundParameter: null,
                operationType: 0,
                operationName: "ipg_IPGReferralReopenReferral",
                parameterTypes: {
                  Referral: {
                    typeName: "mscrm.ipg_referral",
                    structuralProperty: 5,
                  },
                },
              };
            },
          };
          Xrm.Utility.showProgressIndicator("Intake");
          Xrm.WebApi.online.execute(reqObject).then(
            (response) => {
              Xrm.Utility.closeProgressIndicator();
              if (response.ok) {
                let entityFormOptions = {};
                entityFormOptions["formId"] = PifStep2Form;
                entityFormOptions["entityName"] = "ipg_referral";
                entityFormOptions["entityId"] = referral.ipg_referralid;
                Xrm.Navigation.openForm(entityFormOptions);
              }
            },
            () => {
              Xrm.Utility.closeProgressIndicator();
            }
          );
        }
      });
  }

  /**
  * Called on Submit button click
  * @function Intake.Referral.Ribbon.OnSubmitClick
  * @returns {void}
  */
  export function OnSubmitClick(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    var caseStatus = formContext.getAttribute("ipg_casestatus")?.getValue();
    if (caseStatus && caseStatus == 923720001) {
      Xrm.Navigation.openAlertDialog({
        confirmButtonLabel: "Ok",
        text: "You are not able to submit closed referral.",
      }).then(() => {
        return;
      });
    } else {

      saveAndStartGating(formContext);
    }
  }

  async function saveAndStartGating(formContext: Xrm.FormContext) {

    Xrm.Utility.showProgressIndicator("Processing...");
    Intake.Referral.showWarningMessage = false;
    formContext.data.save().then(
      async function () {
        let referralid: string = formContext.data.entity.getId();
        var parameters = {
          Target: {
            ipg_referralid: formContext.data.entity.getId(),
            "@odata.type":
              "Microsoft.Dynamics.CRM." +
              formContext.data.entity.getEntityName(),
          },
          InitiatingUser: {
            systemuserid: Xrm.Utility.getGlobalContext().userSettings.userId,
            "@odata.type": "Microsoft.Dynamics.CRM.systemuser",
          },
          IsManual: true,
        };
        const mainGatingCallback = (results) => {
          Intake.Referral.showWarningMessage = true;
          if (results.Output != null && results.Output != "") {
            var alertStrings = {
              confirmButtonLabel: "Close Referral",
              cancelButtonLabel: "Cancel",
              text: results.Output,
              title: "Errors",
            };

            var alertOptions = { height: 150, width: 300 };
            Xrm.Navigation.openConfirmDialog(alertStrings, alertOptions).then(
              function success(result) {
                if (result.confirmed) {
                  Xrm.WebApi.updateRecord("ipg_referral", referralid, {
                    statecode: 1,
                    statuscode: 2,
                  }).then(() => {
                    formContext.data.refresh(false).then((rslt) => {
                      checkIfRejected(formContext);
                    });
                  });
                }
              },
              function (error) {
                console.log(error.message);
              }
            );
          } else {
            let caseLookupValue = formContext
              .getAttribute("ipg_associatedcaseid")
              .getValue();
            let formParameters: Xrm.Utility.OpenParameters = {};
            formParameters["p_isStep2"] = "1";
            formContext.data.save().then(
              function () {
                if (caseLookupValue && caseLookupValue.length) {
                  Xrm.Navigation.openForm(
                    {
                      entityName: caseLookupValue[0].entityType,
                      entityId: caseLookupValue[0].id,
                    },
                    formParameters
                  );
                } else {
                  Xrm.Navigation.openForm({
                    entityId: formContext.data.entity.getId(),
                    entityName: formContext.data.entity.getEntityName(),
                  });
                }
              },
              function (error) {
                console.log(error.message);
              }
            );
          }
        };

        callAction(
          "ipg_IPGGatingStartGateProcessing",
          parameters,
          true,
          function (results) {
            formContext.data.refresh(true).then(async () => {
              Xrm.Utility.closeProgressIndicator();
              if (!results.SeverityLevel) {
                var alertStrings = {
                  confirmButtonLabel: "OK",
                  text: results.Output,
                };
                Xrm.Navigation.openAlertDialog(alertStrings, {
                  height: 400,
                  width: 600,
                }).then(
                  function success(result) { },
                  function (error) {
                    console.log(error.message);
                  }
                );
              }
              switch (results.SeverityLevel) {
                case 923720003: //critical
                  const confirmStr: Xrm.Navigation.ConfirmStrings = {
                    title: "Alert!",
                    text: `The following critical issues have occurred. This Referral cannot be considered with the current information entered. Select ‘Close Referral’ to proceed with the closing of this Referral or select ‘Continue’ to return and continue making changes to this Referral and resubmit for consideration.\n${results.Output}`,
                    confirmButtonLabel: "Close Referral",
                    cancelButtonLabel: "Continue",
                  };
                  const confirmResult =
                    await Xrm.Navigation.openConfirmDialog(confirmStr, {
                      height: 400,
                      width: 600,
                    });
                  if (confirmResult.confirmed) {
                    gatePostAction(formContext, mainGatingCallback);
                  }
                  break;
                case 923720002: //error
                  var alertStr = {
                    title: "Error",
                    text: `The following errors were found. Case cannot proceed to the next step until these errors are corrected and Referral is reprocessed through the 'Submit' action. \n${results.Output}`,
                    confirmButtonLabel: "Close",
                  };
                  Xrm.Navigation.openAlertDialog(alertStr, {
                    height: 400,
                    width: 600,
                  }).then(() => {
                    gatePostAction(formContext, mainGatingCallback);
                  });
                  break;
                case 923720001: //warning
                  Xrm.WebApi.retrieveRecord(
                    "ipg_referral",
                    Intake.Utility.removeCurlyBraces(
                      formContext.data.entity.getId()
                    ),
                    "?$select=_ipg_lifecyclestepid_value"
                  ).then(function success(referral) {
                    if (referral) {
                      alertStr = {
                        title: "Warning",
                        text: `The following warnings were found. Referral promoted to ${referral["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"]} step. \n${results.Output}`,
                        confirmButtonLabel: "Close",
                      };
                      Xrm.Navigation.openAlertDialog(alertStr, {
                        height: 400,
                        width: 600,
                      }).then(() => {
                        gatePostAction(formContext, mainGatingCallback);
                      });
                    }
                  });
                  break;
                case 923720000: //info
                  Xrm.WebApi.retrieveRecord(
                    "ipg_referral",
                    Intake.Utility.removeCurlyBraces(
                      formContext.data.entity.getId()
                    ),
                    "?$select=_ipg_lifecyclestepid_value"
                  ).then(function success(referral) {
                    if (referral) {
                      alertStr = {
                        title: "Success",
                        text: `No issues found. Referral promoted to ${referral["_ipg_lifecyclestepid_value@OData.Community.Display.V1.FormattedValue"]}`,
                        confirmButtonLabel: "Close",
                      };
                      Xrm.Navigation.openAlertDialog(alertStr, {
                        height: 400,
                        width: 600,
                      }).then(() => {
                        mainGatingCallback({});
                      });
                    }
                  });
                  break;
              }  
            });
          }
        );
      },
      function (error) {
        Xrm.Utility.closeProgressIndicator();
        Intake.Referral.showWarningMessage = true;
        console.log(error.message);
      }
    );
  }

  /**
  * call Custom action
  * @function Intake.Referral.Ribbon.callAction
  * @returns {void}
  */
  function callAction(actionName, parameters, async, successCallback) {
    var req = new XMLHttpRequest();
    req.open(
      "POST",
      Xrm.Utility.getGlobalContext().getClientUrl() +
      "/api/data/v9.1/" +
      actionName,
      async
    );
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {
      if (this.readyState === 4) {
        req.onreadystatechange = null;
        if (this.status === 200 || this.status === 204) {
          if (this.response == "") successCallback();
          else successCallback(JSON.parse(this.response));
        } else {
          Xrm.Utility.closeProgressIndicator();
          alert(JSON.parse(this.response).error.message);
        }
      }
    };
    req.send(JSON.stringify(parameters));
  }
  function checkIfRejected(formCtx: Xrm.FormContext) {
    if (formCtx.getAttribute("ipg_casestatus")?.getValue() == 923720001) {
      //Closed
      Xrm.Navigation.navigateTo({
        pageType: "entitylist",
        entityName: "ipg_document",
      });
    }
  }
  /**
  * Do not show Submit button if this referral has an associated case already
  * @param primaryControl
  */
  export async function isShowSubmitButton(primaryControl): Promise<boolean> {
    let formContext: Xrm.FormContext = primaryControl;

    if (formContext.ui.getFormType() == 1) {
      return true;
    }

    let referralId = formContext.data.entity.getId();
    if (referralId) {
      return Xrm.WebApi.retrieveRecord(
        "ipg_referral",
        referralId,
        "?$select=_ipg_associatedcaseid_value"
      ).then(
        function success(result): boolean {
          if (result._ipg_associatedcaseid_value) {
            return false;
          } else {
            return true;
          }
        },
        function (error) {
          Xrm.Navigation.openAlertDialog(error.message);
        }
      );
    }

    return false;
  }
  const PifStep2Form = "7266869e-f61a-45e5-84f9-c7c5446dbfc5"; //PIF Intake Form - Step 2 form GUID
  export function isShowCancelButton(primaryControl: Xrm.FormContext) {
    let formId = primaryControl.ui.formSelector.getCurrentItem().getId();
    return formId !== PifStep2Form;
  }

  export function isReferralClosedAndHasAssociatedCase(
    primaryControl: Xrm.FormContext
  ) {
    var formContext: Xrm.FormContext = primaryControl;
    if (formContext.data.entity.getEntityName() == "ipg_referral") {
      var caseStatus = formContext.getAttribute("ipg_casestatus")?.getValue();
      var associatedCase = formContext
        .getAttribute("ipg_associatedcaseid")
        ?.getValue();
      if (associatedCase && caseStatus && caseStatus == 923720001) {
        return false;
      }
    }
    return true;
  }

  /**
  * Show EHR CPT Override button button if this referral has been rejected at EHR Gate
  * @param primaryControl
  */
  export function isEnableEhrCptOverrideButton(primaryControl): boolean {
    let formContext: Xrm.FormContext = primaryControl;

    //case status = closed
    //no associated case
    //gate outcome = EHRFail

    let caseStatusAttribute = formContext.getAttribute('ipg_casestatus');
    let associatedCaseAttribute = formContext.getAttribute('ipg_associatedcaseid');
    let caseOutcomeAttribute = formContext.getAttribute('ipg_caseoutcome');
    if (caseStatusAttribute && associatedCaseAttribute && caseOutcomeAttribute) {
      let caseStatusValue: number = caseStatusAttribute.getValue();
      let associatedCaseValue: Xrm.LookupValue[] = associatedCaseAttribute.getValue();
      let caseOutcomeValue: number = caseOutcomeAttribute.getValue();
      if (caseStatusValue == CaseStatusesEnum.Close
        && (!associatedCaseValue || associatedCaseValue.length == 0)
        && caseOutcomeValue == ipg_CaseOutcomeCodes.GateEhrFail) {

        return true;
      }
    }

    return false;
  }

  export async function OnOverrideEhrCptClick(primaryControl) {
    //debugger;
    let formContext: Xrm.FormContext = primaryControl;
    let referralId = formContext.data.entity.getId().replace('{', '').replace('}', '');
    let reqObject = {
      entity: {
        entityType: "ipg_referral",
        id: referralId,
      },
      getMetadata: function () {
        return {
          boundParameter: "entity",
          operationType: 0,
          operationName: "ipg_IPGReferralActionsEhrCptOverride",
          parameterTypes: {
            entity: {
              typeName: "mscrm.ipg_referral",
              structuralProperty: StructualPropertyTypes.EntityType,
            },
          },
        };
      },
    };
    Xrm.Utility.showProgressIndicator("Please wait...");
    let overrideResult = await Xrm.WebApi.online.execute(reqObject).then(
      async (response) => {
        //debugger;
        Xrm.Utility.closeProgressIndicator();
        if (response.ok) {
          return response.json();
        }
      },
      () => {
        Xrm.Utility.closeProgressIndicator();
      }
    );
    if (overrideResult.IsSuccess) {
      let caseStatusAttribute = formContext.getAttribute('ipg_casestatus');
      if (caseStatusAttribute) {
        caseStatusAttribute.setValue(CaseStatusesEnum.Open); //set Open status because Closed referral cannot be saved
      }
      await saveAndStartGating(formContext);
    }
  }

  function gatePostAction(
    formContext: Xrm.FormContext,
    mainGatingCallback = undefined
  ) {
    var parametersConfirm = {
      Target: {
        ipg_referralid: formContext.data.entity.getId(),
        "@odata.type":
          "Microsoft.Dynamics.CRM." + formContext.data.entity.getEntityName(),
      },
    };
    Xrm.Utility.showProgressIndicator("Processing...");
    callAction(
      "ipg_IPGGatingGateProcessingPostAction",
      parametersConfirm,
      true,
      (resultsConfirm) => {
        formContext.data.refresh(true).then(() => {
          formContext.getControl("Timeline") &&
            formContext.getControl("Timeline").refresh();
          Xrm.Utility.closeProgressIndicator();
          typeof mainGatingCallback === "function" &&
            mainGatingCallback(resultsConfirm);
        });
      }
    );
  }

}
