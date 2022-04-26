/**
 * @namespace Intake.Task
 * 
 */
namespace Intake.Task {

  export function onChangeFilteredOwner(executionContext: Xrm.Events.EventContext)
  {
    var formContext = executionContext.getFormContext();
    let filteredOwnerAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("ipg_owner");
    if (filteredOwnerAttribute) {
      let filteredOwnerAttributeValue = filteredOwnerAttribute.getValue();
      if (filteredOwnerAttributeValue != null) {
        let id = filteredOwnerAttributeValue[0].id;
        let type = filteredOwnerAttributeValue[0].entityType;
        let name = filteredOwnerAttributeValue[0].name;        
        let ownerLookup: Xrm.Attributes.Attribute = formContext.getAttribute("ownerid");
        if (ownerLookup != null) {
          setSimpleLookupValue("ownerid", type, id, name);
        }
      }
    }
  }

  function setSimpleLookupValue(LookupId, Type, Id, Name) {
    /// <summary>
    /// Sets the value for lookup attributes that accept only a single entity reference.
    /// Use of this function to set lookups that allow for multiple references, 
    /// a.k.a 'partylist' lookups, will remove any other existing references and 
    /// replace it with just the single reference specified.
    /// </summary>
    /// <param name="LookupId" type="String" mayBeNull="false" optional="false" >
    /// The lookup attribute logical name
    /// </param>
    /// <param name="Type" type="String" mayBeNull="false" optional="false" >
    /// The logical name of the entity being set.
    /// </param>
    /// <param name="Id" type="String" mayBeNull="false" optional="false" >
    /// A string representation of the GUID value for the record being set.
    /// The expected format is "{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}".
    /// </param>
    /// <param name="Name" type="String" mayBeNull="false" optional="false" >
    /// The text to be displayed in the lookup.
    /// </param>
    var lookupReference = new Array();
    lookupReference[0] = new Object;
    lookupReference[0].id = Id;
    lookupReference[0].entityType = Type;
    lookupReference[0].name = Name;
    Xrm.Page.getAttribute(LookupId).setValue(lookupReference);
  }
}
