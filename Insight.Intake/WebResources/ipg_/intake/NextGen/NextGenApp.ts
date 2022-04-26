/**
 * @namespace Intake.NextGen
 */
namespace Intake.NextGen {

  function buildNextGenUrl(environment: string, hrefWithoutHost: string): string {
    let envSuffix: string = '';
    if (environment) {
      envSuffix = '-' + environment;
    }

    return `https://insight-calcrev${envSuffix}.azurewebsites.net` + hrefWithoutHost;
  }

  export function InitNextGenIFrame(): void {

    let environment: string = Intake.Utility.GetEnvironment();

    let hrefWithoutHost: string = '';
    let dataParam: string = Intake.Utility.GetDataParam();
    if (dataParam) {
      let dataParams: string[][] = Intake.Utility.ParseSearchParameters(decodeURIComponent(dataParam));
      let param: string = Intake.Utility.GetParamValueByName(dataParams, 'hrefWithoutHost');
      if (param) {
        hrefWithoutHost = param;
      }
    }

    (<HTMLIFrameElement>document.getElementById('nextGenIFrame')).src = buildNextGenUrl(environment, hrefWithoutHost);
  }
  
}
