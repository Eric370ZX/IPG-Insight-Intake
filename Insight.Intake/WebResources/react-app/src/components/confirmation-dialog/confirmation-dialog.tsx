import { Button, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import Dialog from '@material-ui/core/Dialog/Dialog';
import React from 'react';

import { IConfirmationDialogProps } from "./IConfirmationDialogProps";
import { IConfirmationDialogState } from "./IConfirmationDialogState";

export class ConfirmationDialog extends React.Component<IConfirmationDialogProps, IConfirmationDialogState> {
    state: IConfirmationDialogState = {
        isOpen: this.props.isOpen
    }

    handleClose = () => {
        this.props.closeAction();
    };

    handleConfirm = () => {
        this.props.confirmAction();
    }

    render() {
        return (
            <div>
                <Dialog
                    open={this.props.isOpen}
                    onClose={this.handleClose}
                    aria-labelledby="alert-dialog-title"
                    aria-describedby="alert-dialog-description"
                >
                    <DialogTitle id="alert-dialog-title">{this.props.title}</DialogTitle>
                    <DialogContent>
                        <DialogContentText id="alert-dialog-description">
                            {this.props.message}
                        </DialogContentText>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={this.handleClose} color="primary" autoFocus>
                            {this.props.cancelLabel}
                        </Button>
                        <Button onClick={this.handleConfirm} color="primary">
                            {this.props.confirmLabel}
                        </Button>
                    </DialogActions>
                </Dialog>
            </div>
        );
    }
}