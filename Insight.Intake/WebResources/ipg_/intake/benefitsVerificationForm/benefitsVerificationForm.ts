/**
 * @namespace Intake.benefitsVerificationForm
 */
namespace Intake.benefitsVerificationForm {
  export namespace Form {
    export enum FormTypes {
      GeneralHealth = "8BBB1658-323B-4BCC-81EA-6E68FB6C4AA1",
      Auto = "d1560309-5c0d-49d6-bddc-64495e8fc285",
      WC = "b304dbab-c574-4cc2-87a9-87f123f5dfb0",
      DME = "782375fa-3b65-4251-8e2a-897c40864220"
    }
    export enum CarrierTypes {
    Auto= 427880000,
    Commercial = 427880002,
    DME= 427880004,
    Government= 923720006,
    IPA= 427880003,
    Other= 923720011,
    SelfP0ay= 427880005,
    WorkersComp= 427880001
    }
    enum BvfStateCodes {
      Active = 0,
      Inactive = 1
    }
    interface ICaseField {
      sourceField: string,
      targetField: string
    }
    export const attributes = {
      case: {
        insuredaddress: "ipg_insuredaddress",
        insuredcity: "ipg_insuredcity",
        insuredstate: "ipg_insuredstate",
        insuredzipcodeid: "ipg_insuredzipcodeid",
        insuredphone: "ipg_insuredphone",
        relationtoinsuredcode: "ipg_relationtoinsuredcode"
      },
      patient: {
        patientaddress: "ipg_patientaddress",
        patientcity: "ipg_patientcity",
        patientstate: "ipg_patientstate",
        patientzipcodeid: "ipg_patientzipcodeid",
        patienthomephone: "ipg_patienthomephone"
      },
      optionSets: {
        relationtoinsure: {
          self: 100000000
        }
      }
    }

    export const webResources = {
      sameAsPatientCheckbox: "WebResource_sameAsPatientCheckbox"
    }
    namespace InitLoader {
      export const exportFields: Array<ICaseField> = [
        //{ sourceField: "ipg_patientfirstname", targetField: "ipg_patientfirstname" },
        //{ sourceField: "ipg_patientlastname", targetField: "ipg_patientlastname" },
        //{ sourceField: "ipg_patientmiddlename", targetField: "ipg_patientmiddlename" },
        //{ sourceField: "ipg_patientdateofbirth", targetField: "ipg_patientdateofbirth" },
        //{ sourceField: "ipg_facilityid", targetField: "ipg_facilityid" },
        //{ sourceField: "ipg_procedureid", targetField: "ipg_procedureid" },
        //{ sourceField: "ipg_cptcodeid1", targetField: "ipg_cptcodeid1" },
        //{ sourceField: "ipg_cptcodeid2", targetField: "ipg_cptcodeid2" },
        //{ sourceField: "ipg_cptcodeid3", targetField: "ipg_cptcodeid3" },
        //{ sourceField: "ipg_cptcodeid4", targetField: "ipg_cptcodeid4" },
        //{ sourceField: "ipg_cptcodeid5", targetField: "ipg_cptcodeid5" },
        //{ sourceField: "ipg_cptcodeid6", targetField: "ipg_cptcodeid6" },
        //{ sourceField: "ipg_physicianid", targetField: "ipg_physicianid" },
        //{ sourceField: "ipg_patientid", targetField: "ipg_patientid" },
        { sourceField: "ipg_homeplancarrierid", targetField: "ipg_homeplancarrierid" },
        { sourceField: "ipg_primarycarriergroupidnumber", targetField: "ipg_primarycarriergroupidnumber" },
        { sourceField: "ipg_ipacarrierid", targetField: "ipg_ipacarrierid" },
        //{ sourceField: "ipg_primarycarrierplantype", targetField: "ipg_primarycarrierplantype" },
        { sourceField: "title", targetField: "ipg_caseid" },
        //{ sourceField: "ipg_carrierid", targetField: "ipg_carrierid" },
        { sourceField: "ipg_memberidnumber", targetField: "ipg_memberidnumber" },
        //{ sourceField: "ipg_autocarrierid", targetField: "ipg_autocarrierid" },
        { sourceField: "ipg_billingfax", targetField: "ipg_billingfax" },
        { sourceField: "ipg_autoclaimnumber", targetField: "ipg_claim" },
        { sourceField: "ipg_nursecasemgrname", targetField: "ipg_nursecasemgrname" },
        { sourceField: "ipg_nursecasemgrphone", targetField: "ipg_nursecasemgrphone" },
        { sourceField: "ipg_autoadjustername", targetField: "ipg_adjustername" },
        { sourceField: "ipg_adjusterphone", targetField: "ipg_adjusterphone" },
        { sourceField: "ipg_csrname", targetField: "ipg_csrname" },
        { sourceField: "ipg_csrphone", targetField: "ipg_csrphone" },
        { sourceField: "ipg_callreference", targetField: "ipg_callreference" },
        { sourceField: "ipg_plandescription", targetField: "ipg_plandescription" },
        { sourceField: "ipg_benefitsnotes", targetField: "ipg_benefitnotesmultiplelines" },
        { sourceField: "ipg_deductible", targetField: "ipg_deductible" },
        { sourceField: "ipg_deductiblemet", targetField: "ipg_deductiblemet" },
        { sourceField: "ipg_deductibleremaining", targetField: "ipg_deductibleremainingcalc" },
        { sourceField: "ipg_payercoinsurance", targetField: "ipg_carriercoinsurance" },
        { sourceField: "ipg_patientcoinsurance", targetField: "ipg_patientcoinsurance" },
        { sourceField: "ipg_oopmax", targetField: "ipg_oopmax" },
        { sourceField: "ipg_oopmet", targetField: "ipg_oopmaxmet" },
        { sourceField: "ipg_oopremaining", targetField: "ipg_oopmaxremainingcalc" },
        { sourceField: "ipg_primarycarrierexpirationdate", targetField: "ipg_coverageexpirationdate" },
        { sourceField: "ipg_benefitplantypecode", targetField: "ipg_plantypecode" },
      ];
      export const exportInsuredFields: Array<ICaseField> = [
        { sourceField: attributes.case.insuredaddress, targetField: attributes.case.insuredaddress },
        { sourceField: attributes.case.insuredcity, targetField: attributes.case.insuredcity },
        { sourceField: attributes.case.insuredstate, targetField: attributes.case.insuredstate },
        { sourceField: attributes.case.insuredzipcodeid, targetField: attributes.case.insuredzipcodeid },
        { sourceField: attributes.case.insuredphone, targetField: attributes.case.insuredphone },
      ];
      export const exportPatientFields: Array<ICaseField> = [
        { sourceField: attributes.patient.patientaddress, targetField: attributes.case.insuredaddress },
        { sourceField: attributes.patient.patientcity, targetField: attributes.case.insuredcity },
        { sourceField: attributes.patient.patientstate, targetField: attributes.case.insuredstate },
        { sourceField: attributes.patient.patientzipcodeid, targetField: attributes.case.insuredzipcodeid },
        { sourceField: attributes.patient.patienthomephone, targetField: attributes.case.insuredphone },
      ]
      export const autoBenefitsFields: Array<string> = [
        'ipg_dateofinjury',
        'ipg_autobenefitsexhausted',
        'ipg_facilityautoexhaustletteronfile',
        'ipg_claimopenandreviewingmedical',
        'ipg_pipavailable',
        'ipg_pipremaining',
        'ipg_medpayavailable',
        'ipg_medpayremaining',
        'ipg_csrphone',
        'ipg_csrname',
        'ipg_callreference',
        'ipg_benefitnotesmultiplelines',
        'ipg_coverageeffectivedate',
        'ipg_coverageexpirationdate',
        'ipg_deductible',
        'ipg_deductiblemet',
        'ipg_carriercoinsurance',
        'ipg_patientcoinsurance',
        'ipg_oopmax',
        'ipg_oopmaxmet'
      ]
    }

    const _defaultAttributeMappings = {
      ipg_deductible: 0.00,
      ipg_deductiblemet: 0.00,
      ipg_deductibleremainingcalc: 0.00,
      ipg_carriercoinsurance: 100,
      ipg_patientcoinsurance: 0.00,
      ipg_oopmax: 0.00,
      ipg_oopmaxmet: 0.00,
      ipg_oopmaxremainingcalc: 0.00,
      ipg_coverageeffectivedate: new Date(new Date().getFullYear(), new Date("9999-01-01").getMonth(), new Date("9999-01-01").getDate()),
      ipg_coverageexpirationdate: new Date(9999, new Date("9999-12-31").getMonth(), new Date("9999-12-31").getDate()),
      ipg_inn_or_oon_code: 427880000 // IIN
    };

    export async function OnLoad(executionContext: Xrm.Events.EventContext) {
      const formContext = executionContext.getFormContext();
    
      var currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
      if (currentFormId == "b304dbab-c574-4cc2-87a9-87f123f5dfb0") 
      {
        formContext.getAttribute("ipg_benefitnotesmultiplelines")?.setValue("Submit a copy of the Invoice with the Claim"); //Default notes

      }
          changeFieldPropertiesByRelation(formContext);
      setCSRFieldsRequired(formContext);
      setRequiredFieldsDependingOnBVF(formContext);
      await filterCarrierBenefitTypes(formContext);
      setTimeout(() => { passContextToWebResource(formContext) }, 2000);
      //if (form.ui.getFormType() == XrmEnum.FormType.Create) {
      try {
        Xrm.Utility.showProgressIndicator("Loading data");
        await initPopulateData(formContext);
        await setBenefitFieldsFromCarrier(formContext);
        blockFields(formContext);

        if (formContext.ui.getFormType() == XrmEnum.FormType.Create) {
          if (formContext.ui.formSelector.getCurrentItem().getId() == FormTypes.Auto) {
            await PopulateAutoBenefitsFromLastBvf(formContext);
          }
        }
        
        formContext.getAttribute(attributes.case.relationtoinsuredcode)?.addOnChange(() => {
          initPopulateData(formContext);
          changeFieldPropertiesByRelation(formContext);
          passContextToWebResource(formContext);
        });
      } catch (err) {
        Xrm.Navigation.openErrorDialog({ message: err.message ?? err });
      } finally {
        Xrm.Utility.closeProgressIndicator();
      }
      //}
    }

     

    export async function SameAsPatientChange(formContext: Xrm.FormContext, sameAsPatient: boolean) {
      if (sameAsPatient) {
        const parentCaseField = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_parentcaseid");
        const parentCase = await Xrm.WebApi.retrieveRecord("incident", parentCaseField.getValue()[0].id);
        let updatedExportFieldsArray = InitLoader.exportPatientFields;
       
        updateFields(updatedExportFieldsArray, parentCase, formContext);
      } else {
        clearIntakeFields(formContext);
      }
    }
    export function clearIntakeFields(formContext: Xrm.FormContext) {
      formContext.getAttribute(attributes.case.insuredaddress)?.setValue(null);
      formContext.getAttribute(attributes.case.insuredcity)?.setValue(null);
      formContext.getAttribute(attributes.case.insuredstate)?.setValue(null);
      formContext.getAttribute(attributes.case.insuredzipcodeid)?.setValue(null);
      formContext.getAttribute(attributes.case.insuredphone)?.setValue(null);
    }
    export function OnSave(executionContext: Xrm.Events.EventContext) {
      //CloneBVF(executionContext); Clone functionality is deprecated
    }
    function changeFieldPropertiesByRelation(formContext: Xrm.FormContext) {
      const relationValue = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>(attributes.case.relationtoinsuredcode)?.getValue();
      var isSelfStatus = relationValue == attributes.optionSets.relationtoinsure.self;

      displayInsureFields(formContext, !isSelfStatus)
      setInsureFieldsRequiredStatus(formContext, !isSelfStatus);
    }

    function setInsureFieldsRequiredStatus(formContext: Xrm.FormContext, isRequired: boolean) {
      if (isRequired) {
        formContext.getAttribute(attributes.case.insuredaddress)?.setRequiredLevel("required");
        formContext.getAttribute(attributes.case.insuredcity)?.setRequiredLevel("required");
        formContext.getAttribute(attributes.case.insuredstate)?.setRequiredLevel("required");
        formContext.getAttribute(attributes.case.insuredzipcodeid)?.setRequiredLevel("required");
        formContext.getAttribute(attributes.case.insuredphone)?.setRequiredLevel("required");
      } else {
        formContext.getAttribute(attributes.case.insuredaddress)?.setRequiredLevel("none");
        formContext.getAttribute(attributes.case.insuredcity)?.setRequiredLevel("none");
        formContext.getAttribute(attributes.case.insuredstate)?.setRequiredLevel("none");
        formContext.getAttribute(attributes.case.insuredzipcodeid)?.setRequiredLevel("none");
        formContext.getAttribute(attributes.case.insuredphone)?.setRequiredLevel("none");
      }
    }
    function displayInsureFields(formContext: Xrm.FormContext, isVisible: boolean) {
      formContext.getControl(attributes.case.insuredaddress)?.setVisible(isVisible);
      formContext.getControl(attributes.case.insuredcity)?.setVisible(isVisible);
      formContext.getControl(attributes.case.insuredstate)?.setVisible(isVisible);
      formContext.getControl(attributes.case.insuredzipcodeid)?.setVisible(isVisible);
      formContext.getControl(attributes.case.insuredphone)?.setVisible(isVisible);
      formContext.getControl(webResources.sameAsPatientCheckbox)?.setVisible(isVisible);
    }

    // This should be added to the forms OnLoad event
    function passContextToWebResource(formContext) {
      let webResouceControl = formContext.getControl(webResources.sameAsPatientCheckbox);
      if (webResouceControl) {
        webResouceControl.getContentWindow()
          .then((contentWindow) => {
            //call our function to pass Xrm and formContext
            contentWindow.setClientApiContext(Xrm, formContext);
          });
      }
    }

    function initPopulateData(formContext: Xrm.FormContext): Promise<void> {
      return new Promise<void>(async (resolve, reject) => {
        const parentCaseField = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_parentcaseid");
        if (!parentCaseField || !parentCaseField.getValue()) {
          setDefaultGenericValues(formContext);
          resolve()
        }

        try {
          const parentCase = await Xrm.WebApi.retrieveRecord("incident", parentCaseField.getValue()[0].id);

          if (formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>(attributes.case.relationtoinsuredcode)?.getValue() == attributes.optionSets.relationtoinsure.self) {
            updateFields(InitLoader.exportInsuredFields, parentCase, formContext);
          }
          updateFields(InitLoader.exportFields, parentCase, formContext);

          setDefaultGenericValues(formContext);

          if (parentCase.ipg_actualdos) {
            formContext.getAttribute<Xrm.Attributes.DateAttribute>("ipg_surgerydate")?.setValue(new Date(parentCase.ipg_actualdos));
          } else if (parentCase.ipg_surgerydate) {
            formContext.getAttribute<Xrm.Attributes.DateAttribute>("ipg_surgerydate")?.setValue(new Date(parentCase.ipg_surgerydate));
          }

          if (parentCase.ipg_medicalbenefitsexhausted == true)
            formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_autobenefitsexhausted")?.setValue(1);
          else
            formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_autobenefitsexhausted")?.setValue(0);

          if (formContext.getAttribute("ipg_formtype")?.getValue() == 427880003) { //if DME
            await PopulateFieldsWithLastDmeBenefitRecordValues(formContext, parentCase);
          }

          resolve();
        } catch (err) {
          reject(err.message);
        }
      });
    }

    function blockFields(form: Xrm.FormContext) {
      //const unblockIfEmptyFields = ["ipg_carrierid", "ipg_homeplancarrierid", "ipg_primarycarriergroupidnumber", "ipg_memberidnumber","ipg_primarycarrierplantype", "ipg_autocarrierid"];
      const unblockIfEmptyFields = ["ipg_carrierid", "ipg_memberidnumber", "ipg_autocarrierid"];
      unblockIfEmptyFields.forEach(field => {
        const trgAttr = form.getAttribute(field);

        if (trgAttr && trgAttr.getValue()) {
          trgAttr.controls.forEach(p => p.setDisabled(true));
        }
      });
    }

    function setBenefitFieldsFromCarrier(
      formContext: Xrm.FormContext): Promise<void> {
      return new Promise<void>(async (resolve, reject) => {
        const carrierField = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_carrierid");

        if (!carrierField || !carrierField.getValue()) {
          resolve();
        }

        try {
          const carrier = await Xrm.WebApi.retrieveRecord("account",carrierField.getValue()[0].id);

          if (formContext.ui.getFormType() == XrmEnum.FormType.Update) {
            switch (carrier["ipg_carriertype"]) {
              case CarrierTypes.WorkersComp:
                ChangeForm(formContext, FormTypes.WC);
                break;
              case CarrierTypes.Auto:
                ChangeForm(formContext, FormTypes.Auto);
                break;
              case CarrierTypes.DME:
                ChangeForm(formContext, FormTypes.DME);
                break;
              default:
                ChangeForm(formContext, FormTypes.GeneralHealth);
                break;
            }
          }

          if (formContext.ui.getFormType() == XrmEnum.FormType.Create) {
            formContext
              .getAttribute("ipg_billingcity")
              .setValue(carrier["ipg_billingaddress1_city"]);
            formContext
              .getAttribute("ipg_billingaddress")
              .setValue(carrier["address1_line2"]);

            if (carrier[`_ipg_melissabillingzipcodeid_value`]) {
              formContext.getAttribute("ipg_melissabillingzipcodeid").setValue([
                {
                  id: carrier[`_ipg_melissabillingzipcodeid_value`],
                  name: carrier[
                    `_ipg_melissabillingzipcodeid_value@OData.Community.Display.V1.FormattedValue`
                  ],
                  entityType:
                    carrier[
                      `_ipg_melissabillingzipcodeid_value@Microsoft.Dynamics.CRM.lookuplogicalname`
                    ],
                },
              ]);
            }
            if (carrier[`_ipg_billingstateid_value`]) {
              formContext.getAttribute("ipg_billingstateid").setValue([
                {
                  id: carrier[`_ipg_billingstateid_value`],
                  name: carrier[
                    `_ipg_billingstateid_value@OData.Community.Display.V1.FormattedValue`
                  ],
                  entityType:
                    carrier[
                      `_ipg_billingstateid_value@Microsoft.Dynamics.CRM.lookuplogicalname`
                    ],
                },
              ]);
            }

            if (
              formContext.getAttribute("ipg_formtype")?.getValue() != 427880003
            ) {
              if (carrier.ipg_carriertype === 427880006) {
                //Worker Comp BVF
                formContext.getAttribute("ipg_formtype").setValue(427880001); //worker comp BVF

                if (
                  !formContext
                    .getAttribute("ipg_benefitnotesmultiplelines")
                    .getValue()
                )
                  formContext
                    .getAttribute("ipg_benefitnotesmultiplelines")
                    .setValue("Submit a copy of the Invoice with the Claim.");
              } else if (carrier.ipg_carriertype == 427880000) {
                //Auto Benefit BVF
                formContext.getAttribute("ipg_formtype").setValue(427880000); //Auto BVF

                sleep(1000).then(async () => {
                  const parentCaseField =
                    formContext.getAttribute<Xrm.Attributes.LookupAttribute>(
                      "header_ipg_parentcaseid"
                    );
                  const incident = await Xrm.WebApi.retrieveRecord(
                    "incident",
                    parentCaseField.getValue()[0].id,
                    "?$select=ipg_facilityexhaustletteronfile"
                  );

                  if (incident.ipg_facilityexhaustletteronfile == true)
                    formContext
                      .getAttribute<Xrm.Attributes.OptionSetAttribute>(
                        "ipg_facilityautoexhaustletteronfile"
                      )
                      .setValue(1);
                  else
                    formContext
                      .getAttribute<Xrm.Attributes.OptionSetAttribute>(
                        "ipg_facilityautoexhaustletteronfile"
                      )
                      .setValue(0);
                });
              } else {
                formContext.getAttribute("ipg_formtype").setValue(427880002); //General Health
              }
            }
            formContext.getAttribute("ipg_source").setValue(427880000); //BVF
            if (carrier.ipg_claimtype) {
              let claimFormTypeAttribute =
                formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>(
                  "ipg_claimformtypenew"
                );
              claimFormTypeAttribute.setValue(carrier.ipg_claimtype);
            }
          }

          resolve();
        } catch (err) {
          reject(err.message);
        }
      });
    }
    //Register the function on onSave event on all benefit forms
    export function CloneBVF(executionContext: Xrm.Events.EventContext) {
      var formContext = executionContext.getFormContext();
      var entityName = formContext.data.entity.getEntityName();
      var currentForm = formContext.ui.formSelector.getCurrentItem();

      if (formContext.ui.getFormType() == XrmEnum.FormType.Update) {
        var recordId = formContext.data.entity.getId();
        var workflowId = "74b2ab0a-9687-4361-ae41-59656877ac07";
        executeWorkflow(recordId, workflowId, executionContext.getContext().getClientUrl());
      }
    }

    export function setDefaultGenericValues(formContext) {
      for (const attr in _defaultAttributeMappings) {
        if (formContext.getAttribute(attr) && !formContext.getAttribute(attr).getValue())
          formContext.getAttribute(attr).setValue(_defaultAttributeMappings[attr]);
      }
    }

    function executeWorkflow(recordId, workflowId, clientUrl) {
      var functionName = "executeWorkflow >>";
      var query = "";
      try {
        //Define the query to execute the action
        query = "workflows(" + workflowId.replace("}", "").replace("{", "") + ")/Microsoft.Dynamics.CRM.ExecuteWorkflow";

        var data = {
          "EntityId": recordId
        };

        var req = new XMLHttpRequest();
        req.open("POST", clientUrl + "/api/data/v9.1/" + query, false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");

        req.onreadystatechange = function () {

          if (this.readyState == 4 /* complete */) {
            req.onreadystatechange = null;

            if (this.status == 200 || this.status == 204) {
              //success callback this returns null since no return value available.
              //var result = JSON.parse(this.response);

              var alertStrings2 = { confirmButtonLabel: "Ok", text: "BVF record is cloned.", title: "Clone Record" };
              var alertOptions2 = { height: 120, width: 260 };
              Xrm.Navigation.openAlertDialog(alertStrings2, alertOptions2).then(function (success) { }, function (error) { });
            } else {
              //error callback
              var error = JSON.parse(this.response).error;
              var alertStrings1 = { confirmButtonLabel: "Ok", text: error, title: "Error Cloning Record" };
              var alertOptions1 = { height: 120, width: 260 };
              Xrm.Navigation.openAlertDialog(alertStrings1, alertOptions1).then(function (success) { }, function (error) { });
            }
          }
        };
        req.send(JSON.stringify(data));

      } catch (e) {
        var alertStrings = { confirmButtonLabel: "Ok", text: e, title: "Error Cloning Record" };
        var alertOptions = { height: 120, width: 260 };
        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) { }, function (error) { });
      }
    }

    function sleep(ms) {
      return new Promise(resolve => setTimeout(resolve, ms));
    }

    function ChangeForm(formContext: Xrm.FormContext, formId: FormTypes) {
      var currentForm = formContext.ui.formSelector.getCurrentItem();
      var availableForms = formContext.ui.formSelector.items.get();
      if (currentForm.getId() != formId) {
        for (var i in availableForms) {
          var form = availableForms[i];
          // try to find a form based on the name
          if (form.getId() == formId) {
            form.navigate();
            return true;
          }
        }
      }
    }

    export function CloseForm(formContext: Xrm.FormContext) {
      formContext.ui.close();
      //window.history.back();
    }
    export function DOIValueOnChange(executionContext: Xrm.Events.EventContext) {
      let formContext: Xrm.FormContext = executionContext.getFormContext();
      if (formContext) {
        var doiField = formContext.getAttribute("ipg_dateofinjury").getValue();
        var todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());

        if (doiField >= todayDate) {
          var alertStrings = { confirmButtonLabel: "Ok", text: "DOI must be prior to today’s date", title: "DOI Alert" };
          var alertOptions = { height: 120, width: 260 };
          Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
            function (success) {
              formContext.getAttribute("ipg_dateofinjury").setValue(null);
              console.log("Alert dialog closed");
            },
            function (error) {
              console.log(error.message);
            }
          );
        }
      }
    }

    async function PopulateAutoBenefitsFromLastBvf(formContext: Xrm.FormContext) {
      //debugger;

      let incidentId = getLookupFirstId(formContext, 'ipg_parentcaseid');
      let carrierId = getLookupFirstId(formContext, 'ipg_autocarrierid');

      let lastBvfResult = await Xrm.WebApi.retrieveMultipleRecords("ipg_benefitsverificationform",
        `?$top=1&$filter=_ipg_parentcaseid_value eq ${incidentId} and _ipg_carrierid_value eq ${carrierId} and statecode eq ${BvfStateCodes.Active}&$orderby=createdon desc`);
      if (lastBvfResult.entities.length) {
        let lastBvf = lastBvfResult.entities[0];

        for (let fieldName of InitLoader.autoBenefitsFields) {
          setFormFieldFromEntity(formContext, lastBvf, fieldName, fieldName);
        }
      }
    }
  }

  function setCSRFieldsRequired(formContext: Xrm.FormContext) {
    const formtypeid = formContext.ui.formSelector.getCurrentItem().getId();

    if (formtypeid == Form.FormTypes.GeneralHealth) {
      formContext.getAttribute("ipg_csrname")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_csrphone")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_callreference")?.setRequiredLevel("required");
    }
    else {
      formContext.getAttribute("ipg_csrname")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_csrphone")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_callreference")?.setRequiredLevel("none");
    }
  }

  function setRequiredFieldsDependingOnBVF(formContext: Xrm.FormContext) {
    const currentFormId = formContext.ui.formSelector.getCurrentItem().getId();
    if (currentFormId == Form.FormTypes.GeneralHealth) {
      const requiredfield = [
        "ipg_insuredname",
        "ipg_insuredgendercode",
        "ipg_relationtoinsuredcode",
        "ipg_benefittypecode",
        "ipg_inn_or_oon_code",
        "ipg_plantypecode",
        "ipg_coverageexpirationdate"];

      requiredfield.forEach(att => formContext.getAttribute(att)?.setRequiredLevel("required"));
    }
    else if (currentFormId == Form.FormTypes.DME) {
      const requiredfields = [
        "ipg_inn_or_oon_code",
        "ipg_plantypecode",
        "ipg_coverageexpirationdate",
        "ipg_deductibletypecode",
        "ipg_oopmaxtypecode",
        "ipg_csrname",
        "ipg_csrphone",
        "ipg_callreference"];

      requiredfields.forEach(att => formContext.getAttribute(att)?.setRequiredLevel("required"));
    }
  }

  async function filterCarrierBenefitTypes(formContext: Xrm.FormContext) {
   
    let benefitTypeAttribute = formContext.getAttribute('ipg_benefittypecode');
    if (benefitTypeAttribute) {
      if (benefitTypeAttribute.controls.getLength()) {
        let benefitTypeControl = benefitTypeAttribute.controls.get(0);

        let serviceTypes = await Xrm.WebApi.retrieveMultipleRecords('ipg_gwservicetypecode', '?$filter=ipg_displayonbvf eq true and statecode eq 0');

        benefitTypeAttribute.getOptions().forEach(function (option) {
          let isServiceTypeFound: boolean = serviceTypes.entities.some(b => b.ipg_benefittypecode == option.value);

          if (!isServiceTypeFound) {
            benefitTypeControl.removeOption(option.value);
          }
        });
      }
    }
  }

  function updateFields(fields: any, parentCase: any, formContext: Xrm.FormContext) {
  
    fields.forEach((val) => { 
      setFormFieldFromEntity(formContext, parentCase, val.sourceField, val.targetField);
    });
    
  }

  async function PopulateFieldsWithLastDmeBenefitRecordValues(formContext: Xrm.FormContext, parentCase: any) {
    var dmeBenefits = await Xrm.WebApi.retrieveMultipleRecords("ipg_benefit",
      "?$select=ipg_inoutnetwork,ipg_plansponsor,ipg_deductible,ipg_deductiblemet,ipg_carriercoinsurance,ipg_membercoinsurance,ipg_memberoopmax,ipg_memberoopmet,ipg_eligibilitystartdate,ipg_eligibilityenddate"
      + "&$filter=_ipg_caseid_value eq " + parentCase.incidentid + " and ipg_benefittype eq 427880040&$orderby=createdon desc");
    if (dmeBenefits.entities.length > 0) {
      var lastDmeBenefit = dmeBenefits.entities[0];
      formContext.getAttribute("ipg_inn_or_oon_code").setValue(lastDmeBenefit["ipg_inoutnetwork"] ? 427880000 : 427880001);
      formContext.getAttribute("ipg_plansponsor").setValue(lastDmeBenefit["ipg_plansponsor"]);
      formContext.getAttribute("ipg_coverageeffectivedate").setValue(new Date(lastDmeBenefit["ipg_eligibilitystartdate"]));
      formContext.getAttribute("ipg_coverageexpirationdate").setValue(new Date(lastDmeBenefit["ipg_eligibilityenddate"]));
      formContext.getAttribute("ipg_deductible").setValue(lastDmeBenefit["ipg_deductible"]);
      formContext.getAttribute("ipg_deductiblemet").setValue(lastDmeBenefit["ipg_deductiblemet"]);
      formContext.getAttribute("ipg_carriercoinsurance").setValue(lastDmeBenefit["ipg_carriercoinsurance"]);
      formContext.getAttribute("ipg_patientcoinsurance").setValue(lastDmeBenefit["ipg_membercoinsurance"]);
      formContext.getAttribute("ipg_oopmax").setValue(lastDmeBenefit["ipg_memberoopmax"]);
      formContext.getAttribute("ipg_oopmaxmet").setValue(lastDmeBenefit["ipg_memberoopmet"]);
    }
  }

  function getLookupFirstId(formContext: Xrm.FormContext, attributeName: string): string {
    const value = formContext.getAttribute<Xrm.Attributes.LookupAttribute>(attributeName)?.getValue();
    if (value) {
      return value[0].id;
    }

    return null;
  }

  function setFormFieldFromEntity(formContext: Xrm.FormContext, entity: any, sourceFieldName: string, targetFieldName: string) {
    if (entity[sourceFieldName] === undefined && entity[`_${sourceFieldName}_value`] === undefined) {
      return;
    }
    
    const trgAttr = formContext.getAttribute(targetFieldName);

    if (trgAttr) {
      switch (trgAttr.getAttributeType()) {
        case "string":
        case "optionset":
        case "money":
        case "memo":
          trgAttr.setValue(entity[sourceFieldName]);
          break;
        case "datetime":
          trgAttr.setValue(new Date(entity[sourceFieldName]));
          break;
        case "lookup":
          trgAttr.setValue([{
            id: entity[`_${sourceFieldName}_value`],
            name: entity[`_${sourceFieldName}_value@OData.Community.Display.V1.FormattedValue`],
            entityType: entity[`_${sourceFieldName}_value@Microsoft.Dynamics.CRM.lookuplogicalname`],
          }]);
          break;
      }
    }
  }
}
