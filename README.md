# IPG-Insight-Intake
Repo support custom code for D365 Intake solution

## Prerequisites
- [VisualStudio](https://visualstudio.microsoft.com/ru/downloads/) - VisualStudio version >= `2015`
- [NodeJS and npm](http://nodejs.org/) - NodeJS version >= `8.11.3`

### Early Bound Classes Generation
Please use XRM Toolbox and EarlyBoundGenerator plugin to generate Early Bound Classes. 
Congfiguration file for EarlyBoundGenerator:
EarlyBoundGenerator.InsightIntakeSettings.xml

## Developing
- Open terminal or cmd.
- Change your working directory to `(PATH TO PROJECT)/Insight.Intake/WebResources/`
- Execute `npm install` or `npm i` to install project dependencies
- Execute `npm run build` to run the Typescript compiler
- Execute `npm run watch` to run the Typescript compiler in watch mode

### Web Resource Structure
```text
WebResources
|-- ipg_
|   `-- intake - Code related to D365 Intake solution
|       `-- referral - Code related to Referral entity
`-- types - Additional types (not provided by @types package) for Typescript
```