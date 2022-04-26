function SetBalanceToZero(executionContext) {

  let formContext = executionContext.getFormContext();
  if (formContext.ui.tabs.get("PaymentTransactions_tab") != null) {
    if (formContext.getAttribute("ipg_remainingcarrierbalance").getValue() <= ".99") {
      formContext.getAttribute("ipg_remainingcarrierbalance").setValue(0);
    }
  }
}
