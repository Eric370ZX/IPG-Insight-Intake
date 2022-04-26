namespace Intake.CarrierButtons {
  interface Parameters {
    carrierType: CarrierNumber;
  }

  enum CarrierNumber {
    First = 1,
    Second = 2,
    //Tertiary = 3
  }

  const CarriersMapping = {
    [CarrierNumber.First]: "ipg_carrierid",
    [CarrierNumber.Second]: "ipg_secondarycarrierid"
  };

  const CarrierCoinsuranceMapping = {
    [CarrierNumber.First]: "ipg_payercoinsurance",
    [CarrierNumber.Second]: "ipg_patientcoinsurance"
  };

  const PatientCoinsuranceMapping = {
    [CarrierNumber.First]: "ipg_carrier2carriercoinsurancedisplay",
    [CarrierNumber.Second]: "ipg_carrier2patientcoinsurancedisplay"
  };

  var Xrm: Xrm.XrmStatic = Xrm || parent.Xrm;

  let refreshBenefitsFunctionReference: () => void;

  export async function Init() {
    const isLocked: boolean = Xrm.Page.getAttribute("ipg_islocked")?.getValue();
    //@ts-ignore
    document.getElementById('manualBenefitsButton').disabled = isLocked;

    await SetEbvButtonAvailability();

    addEventHandlers();
  }

  async function SetEbvButtonAvailability() {
    let disableEbv: boolean = false;

    const isLocked = Xrm.Page.getAttribute("ipg_islocked")?.getValue();
    if (isLocked) {
      disableEbv = true;
    }
    else {
      const params: Parameters = Intake.Utility.GetWindowJsonParameters();
      const carrierIdAttributeName = CarriersMapping[params.carrierType];

      const carrierAttribute = Xrm.Page.getAttribute(carrierIdAttributeName);
      if (carrierAttribute) {
        const carrierIdValue = carrierAttribute.getValue();
        if (carrierIdValue && carrierIdValue.length) {
          let carrierId = carrierIdValue[0].id.replace("{", "").replace("}", "");

          const select = `$select=ipg_enableebvzirmed`;
          const queryPath = `/api/data/v9.1/accounts(${carrierId})?${select}`;

          const response = await fetch(queryPath, {
            method: "GET"
          });
          const carrier = await response.json();

          if (!carrier.ipg_enableebvzirmed) {
            disableEbv = true;
          }
        }
      }
    }

    //@ts-ignore
    document.getElementById('ebvButton').disabled = disableEbv;
  }

  export async function VerifyBenefits(): Promise<void> {
    //debugger;

    let caseId = Xrm.Page.data.entity.getId();
    if (!caseId) {
      throw new Error("Could not get Case ID");
    }

    caseId = caseId.replace('{', '').replace('}', '');

    const params: Parameters = Intake.Utility.GetWindowJsonParameters();

    let ebvCarrierNumber: number; //todo: carrier ID
    if (params.carrierType == CarrierNumber.First) {
      ebvCarrierNumber = 1;
    }
    else if (params.carrierType == CarrierNumber.Second) {
      ebvCarrierNumber = 2;
    }
    else {
      throw new Error("Unexpected carrier type");
    }
    
    let memberId: string = null;
    let memberidAttribute = Xrm.Page.getAttribute("ipg_memberidnumber");
    if (memberidAttribute) {
      memberId = memberidAttribute.getValue();
    }
    if (!memberId) {
      var alertStrings: Xrm.Navigation.AlertStrings = { confirmButtonLabel: "OK", text: "Member Id is empty!" };
      var alertOptions: Xrm.Navigation.DialogSizeOptions = { height: 150, width: 300 };
      Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
      return;
    }

    var postObject = { IsUserGenerated: true, CarrierNumber: ebvCarrierNumber };
    Xrm.Utility.showProgressIndicator("Processing...");
    (<any>parent).$.ajax({
      method: "POST",
      url: '/api/data/v9.0/incidents(' + caseId + ')/Microsoft.Dynamics.CRM.ipg_IPGCaseActionsVerifyBenefits',
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(postObject),
      dataType: 'json',
      success: async function (response) {
        await Xrm.Page.data.refresh(false);
        if (refreshBenefitsFunctionReference) {
          refreshBenefitsFunctionReference();
        }
        Xrm.Utility.closeProgressIndicator();
      },
      error: function (xhr, ajaxOptions, thrownError) {
        Xrm.Navigation.openErrorDialog({ message: "Benefits verification failed" });
        Xrm.Utility.closeProgressIndicator();
      }
    });
  }

  export async function OpenBvf() {
    const params: Parameters = Intake.Utility.GetWindowJsonParameters();

    const carrierIdAttributeName = CarriersMapping[params.carrierType];

    let carrierIdAttribute = Xrm.Page.getAttribute(carrierIdAttributeName);
    if (carrierIdAttribute) {
      let carrierIdValue = carrierIdAttribute.getValue();
      if (carrierIdValue && carrierIdValue.length) {
        let carrierId: string = carrierIdValue[0].id.toString().replace("{", "").replace("}", "");

        let carrier = await Xrm.WebApi.retrieveRecord('account', carrierId, '?$select=ipg_carriertype');
        if (carrier) {

          let entityFormOptions: Xrm.Navigation.EntityFormOptions = {
            openInNewWindow: true,
            entityName: "ipg_benefitsverificationform",
            createFromEntity: {
              id: Xrm.Page.data.entity.getId(),
              name: Xrm.Page.data.entity.getPrimaryAttributeValue(),
              entityType: Xrm.Page.data.entity.getEntityName(),
            }
          };

          let formParams: any = {};

          let carrierCoinsuranceAttr = Xrm.Page.getAttribute(CarrierCoinsuranceMapping[params.carrierType]);
          if (carrierCoinsuranceAttr) {
            formParams["ipg_carriercoinsurance"] = carrierCoinsuranceAttr.getValue();
          }
          let patientCoinsuranceAttr = Xrm.Page.getAttribute(PatientCoinsuranceMapping[params.carrierType]);
          if (patientCoinsuranceAttr) {
            formParams["ipg_patientcoinsurance"] = patientCoinsuranceAttr.getValue();
          }

          if (carrier.ipg_carriertype == 427880000 /*Auto*/) {
            entityFormOptions.formId = "d1560309-5c0d-49d6-bddc-64495e8fc285"; //Auto Benefits Form

            formParams["ipg_autocarrierid"] = carrierIdValue[0].id;
            formParams["ipg_autocarrieridname"] = carrierIdValue[0].name;
            formParams["ipg_autocarrieridtype"] = carrierIdValue[0].entityType;

            formParams["ipg_carrierid"] = carrierIdValue[0].id;
            formParams["ipg_carrieridname"] = carrierIdValue[0].name;
            formParams["ipg_carrieridtype"] = carrierIdValue[0].entityType;

            let otherCarrierAttributeName;
            if (params.carrierType == CarrierNumber.First) {
              otherCarrierAttributeName = CarriersMapping[CarrierNumber.Second];
            }
            else if (params.carrierType == CarrierNumber.Second) {
              otherCarrierAttributeName = CarriersMapping[CarrierNumber.First];
            }
            else {
              throw new Error("Unexpected carrier type: " + params.carrierType);
            }
            let otherCarrierAttribute = Xrm.Page.getAttribute(otherCarrierAttributeName);
            if (otherCarrierAttribute) {
              let otherCarrierValue = otherCarrierAttribute.getValue();
              if (otherCarrierValue && otherCarrierValue.length) {
                formParams["ipg_othercarriername1id"] = otherCarrierValue[0].id;
                formParams["ipg_othercarriername1idname"] = otherCarrierValue[0].name;
                formParams["ipg_othercarriername1idtype"] = otherCarrierValue[0].entityType;
              }
            }
          }
          else if (carrier.ipg_carriertype == 427880001 /*Workers Comp*/) {
            entityFormOptions.formId = "b304dbab-c574-4cc2-87a9-87f123f5dfb0"; //Workers Comp Form

            formParams["ipg_carrierid"] = carrierIdValue[0].id;
            formParams["ipg_carrieridname"] = carrierIdValue[0].name;
            formParams["ipg_carrieridtype"] = carrierIdValue[0].entityType;

          }
          else {
            entityFormOptions.formId = "8BBB1658-323B-4BCC-81EA-6E68FB6C4AA1"; //General Health BVF 

            formParams["ipg_carrierid"] = carrierIdValue[0].id;
            formParams["ipg_carrieridname"] = carrierIdValue[0].name;
            formParams["ipg_carrieridtype"] = carrierIdValue[0].entityType;
          }

          if (Xrm.Page.getAttribute<Xrm.Attributes.StringAttribute>("ipg_memberidnumber").getValue().toLowerCase().startsWith("jqu"))
          {
            entityFormOptions.formId = "782375fa-3b65-4251-8e2a-897c40864220"; //DME BVF
            formParams["ipg_formtype"] = 427880003; //DME form type
            formParams["ipg_benefittypecode"] = 427880040; //DME benefit type
          }
          
          Xrm.Navigation.openForm(entityFormOptions, formParams); //Default Information Form
        }
      }
    }
  }

  export function setRefreshBenefitsFunctionReference(reference: () => void) {
    refreshBenefitsFunctionReference = reference;
  }

  export async function unload() {
    removeEventHandlers();
  }

  function addEventHandlers() {
    const params: Parameters = Intake.Utility.GetWindowJsonParameters();
    const carrierIdAttributeName = CarriersMapping[params.carrierType];
    var carrierAttribute = Xrm.Page.getAttribute(carrierIdAttributeName);
    if (carrierAttribute) {
      carrierAttribute.addOnChange(SetEbvButtonAvailability);
    }
  }

  function removeEventHandlers() {
    const params: Parameters = Intake.Utility.GetWindowJsonParameters();
    const carrierIdAttributeName = CarriersMapping[params.carrierType];
    var carrierAttribute = Xrm.Page.getAttribute(carrierIdAttributeName);
    if (carrierAttribute) {
      carrierAttribute.removeOnChange(SetEbvButtonAvailability);
    }
  }
}
