/**
 * @namespace Intake.AddNewCarrierToCase
 */
namespace Intake.AddNewCarrierToCase {

  enum CarrierTypes {
    Auto = 427880000,
    Commercial, value = 427880002,
    DME = 427880004,
    Government = 923720006,
    IPA = 427880003,
    Other = 923720011,
    SelfPay = 427880005,
    WorkersComp = 427880001
  }

  export const OnLoad = async (executionContext: Xrm.Events.SaveEventContext) => {
    const formContext: Xrm.FormContext = executionContext.getFormContext();
    const caseId = formContext.getAttribute("ipg_caseid")?.getValue();

    if (caseId == null) return;

    await IfPrimaryCarrierExists(caseId[0].id) ? AddSecondaryPreSearch(formContext) : AddPrimaryPreSearch(formContext)
  }
  const AddPrimaryPreSearch = (formContext) => formContext.getControl("ipg_carrierid")?. addPreSearch(
      () => { ApplyPrimaryCarriersFilters(formContext) })
  
  const AddSecondaryPreSearch = (formContext) => formContext.getControl("ipg_carrierid")?.addPreSearch(
      () => { ApplySecondaryCarrierFilters(formContext) });
   
  const IfPrimaryCarrierExists = async (caseId: string) => {
      const incident = await Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=_ipg_carrierid_value")
      if (incident) {
        return incident?._ipg_carrierid_value;
      }
      return false;
    };
  const ApplyPrimaryCarriersFilters = (formContext: Xrm.FormContext) => {
      let filter = ExcludeCarrietTypes(CarrierTypes.IPA, CarrierTypes.Government, CarrierTypes.Auto);
      formContext.getControl<Xrm.Controls.LookupControl>("ipg_carrierid").addCustomFilter(filter, "account");
    };
  const ApplySecondaryCarrierFilters = (formContext: Xrm.FormContext) => {
      let filter = ExcludeCarrietTypes(CarrierTypes.IPA, CarrierTypes.WorkersComp);
      formContext.getControl<Xrm.Controls.LookupControl>("ipg_carrierid").addCustomFilter(filter, "account");
    };  
  const ExcludeCarrietTypes = (...carrierTypes: number[]) => {
      return `<filter type='and'>
                    <condition attribute='ipg_carriertype' operator='not-in'>
                      ${carrierTypes.map(typeToExclude => `<value>${typeToExclude}</value>`).join(' ')}
                    </condition>
                </filter>`
    };
  }
