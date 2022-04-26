/**
 * @namespace Intake.NextGen
 */
var Intake;
(function (Intake) {
    var NextGen;
    (function (NextGen) {
        function buildNextGenUrl(environment, hrefWithoutHost) {
            var envSuffix = '';
            if (environment) {
                envSuffix = '-' + environment;
            }
            return "https://insight-calcrev" + envSuffix + ".azurewebsites.net" + hrefWithoutHost;
        }
        function InitNextGenIFrame() {
            var environment = Intake.Utility.GetEnvironment();
            var hrefWithoutHost = '';
            var dataParam = Intake.Utility.GetDataParam();
            if (dataParam) {
                var dataParams = Intake.Utility.ParseSearchParameters(decodeURIComponent(dataParam));
                var param = Intake.Utility.GetParamValueByName(dataParams, 'hrefWithoutHost');
                if (param) {
                    hrefWithoutHost = param;
                }
            }
            document.getElementById('nextGenIFrame').src = buildNextGenUrl(environment, hrefWithoutHost);
        }
        NextGen.InitNextGenIFrame = InitNextGenIFrame;
    })(NextGen = Intake.NextGen || (Intake.NextGen = {}));
})(Intake || (Intake = {}));
