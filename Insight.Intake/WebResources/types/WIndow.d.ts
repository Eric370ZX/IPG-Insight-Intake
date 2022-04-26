declare namespace Intake {
    interface Utility {
        EventEmitter: Intake.Utility.EventEmitter;
        Events: Intake.Utility.EventEmitter;
    }
    interface IntakeStatic {
        Utility: Utility;
        Patient: Intake.Case.Patient
    }
}
declare namespace Ipg {
    namespace CustomControls {
        interface CCManager {
            getControl: (key: string) => any;
        }
    }
    interface CustomControls {
        CCManager: CustomControls.CCManager;
    }
    interface IpgStatic {
        CustomControls: CustomControls;
    }
}
declare interface Window {
    Intake: Intake.IntakeStatic;
    Ipg: Ipg.IpgStatic;
    __EventEmitterInstance: Intake.Utility.EventEmitter;
}
