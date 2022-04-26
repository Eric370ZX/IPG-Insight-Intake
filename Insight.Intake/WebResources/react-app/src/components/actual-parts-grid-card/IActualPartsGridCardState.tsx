import { IGenericPart } from "../../models/IGenericPart";
import { INotification } from "../../models/INotification";

export interface IActualPartsGridCardState {
    isDialogOpen: boolean;
    dialogTitle: string;
    dialogCancelLabel: string;
    dialogConfirmLabel: string;
    dialogMessage: string;
    dialogConfirmAction: Function;
    dialogCancelAction: Function;

    isLoaderEnabled: boolean;

    isInvoiceLookupOpened: boolean;
    actualPart: IGenericPart;
    dirtyAttributes: string[];
}