namespace Intake.Configs.DuplicateButton {
  const _enabledEntities: string[] = [
    "ipg_unsupportedprocedure",
    "ipg_staterule",
    "ipg_carrierstaterelationship",
    "ipg_carrierpart",
    "product",
    "entitlement",
    "ipg_carriernetwork",
    "ipg_carrierfeeschedule",
    "ipg_carrierprecertcpt",
    "ipg_carrierprecerthcpcs",
    "ipg_carrierpricinginformation",
    "ipg_chargecenter",
    "ipg_claimstatuscode",
    "ipg_cptcode",
    "ipg_documenttype",
    "ipg_dxcode",
    "account",
    "ipg_facilitycarriercptrule",
    "ipg_facilitycpt",
    "ipg_facilitymanufacturerrelationship",
    "ipg_feeschedule",
    "ipg_homeplancarriermap",
    "ipg_masterhcpcs",
    "ipg_casepartdetail",
    "contact",
    "ipg_procedurename"
  ]

  export function GetEnabledEntities() {
    return _enabledEntities;
  }
}
