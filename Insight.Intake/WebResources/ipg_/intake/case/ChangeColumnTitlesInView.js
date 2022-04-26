/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        /**
        * Changes column text in a view
        * @function Intake.Case.ChangeColumnText
        * @returns {void}
        */
        function ChangeColumnText() {
            var title = "Network Type Name";
            var div = parent.document.querySelector('[id$="ipg_network"]');
            div.setAttribute("title", title);
            var divElements = div.getElementsByTagName("*");
            for (var i = 0; i < divElements.length; i++) {
                if (divElements[i].className == "headerCaption") {
                    divElements[i].innerHTML = title;
                    break;
                }
            }
        }
        Case.ChangeColumnText = ChangeColumnText;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
