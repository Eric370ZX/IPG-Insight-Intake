export interface IConfirmationDialogProps {
    title: string;
    message: string;
    confirmLabel: string;
    cancelLabel: string;

    isOpen: boolean;
    closeAction: Function;
    confirmAction: Function;
}