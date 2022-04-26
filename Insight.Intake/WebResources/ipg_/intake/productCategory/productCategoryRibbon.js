/**
 * @namespace Intake.ProductCategory.Ribbon
 */
var Intake;
(function (Intake) {
    var ProductCategory;
    (function (ProductCategory) {
        var Ribbon;
        (function (Ribbon) {
            function SaveAndNew(formContext) {
                formContext.data.save().then(function () {
                    Xrm.Navigation.navigateTo({ pageType: "entityrecord", entityName: "ipg_productcategory" }, { target: 2, position: 1, width: { value: 50, unit: "%" } });
                }, function (e) { console.log(e); });
            }
            Ribbon.SaveAndNew = SaveAndNew;
            function NewInModal(formContext) {
                Xrm.Navigation.navigateTo({ pageType: "entityrecord", entityName: "ipg_productcategory" }, { target: 2, position: 1, width: { value: 50, unit: "%" } });
            }
            Ribbon.NewInModal = NewInModal;
        })(Ribbon = ProductCategory.Ribbon || (ProductCategory.Ribbon = {}));
    })(ProductCategory = Intake.ProductCategory || (Intake.ProductCategory = {}));
})(Intake || (Intake = {}));
