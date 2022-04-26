/**
 * @namespace Intake.ProductCategory.Ribbon
 */
namespace Intake.ProductCategory.Ribbon { 
    export function SaveAndNew(formContext: Xrm.FormContext) {
        formContext.data.save().then(()=>{
            Xrm.Navigation.navigateTo({pageType:"entityrecord", entityName:"ipg_productcategory"}, {target: 2, position: 1, width: {value: 50, unit:"%"}});
        }, (e)=>{console.log(e)});
    }

    export function NewInModal(formContext: Xrm.FormContext) {
        Xrm.Navigation.navigateTo({pageType:"entityrecord", entityName:"ipg_productcategory"}, {target: 2, position: 1, width: {value: 50, unit:"%"}});  
    }
}
  