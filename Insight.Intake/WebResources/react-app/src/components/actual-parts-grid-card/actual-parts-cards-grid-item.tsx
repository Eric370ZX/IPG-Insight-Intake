import React from 'react';

import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import { Button, LinearProgress, Snackbar } from '@material-ui/core';
import { Alert } from '@material-ui/lab';
import Modal from '@material-ui/core/Modal';

import './actual-parts-cards-grid-item.css';

import { ConfirmationDialog } from "../confirmation-dialog/confirmation-dialog";
import { InvoiceLookupModal } from "../invoice-lookup-modal/invoice-lookup-modal";
import { IActualPartsGridCardProps } from '../../models/properties/IActualPartsGridCardProps';
import { IActualPartsGridCardState } from "./IActualPartsGridCardState";
import { CasePartDetailService } from '../../services/case-part-detail-service';

import { CustomUpdateTextField } from '../custom-update-text-field/custom-update-text-field';
import { CustomUpdateCheckbox } from '../custom-update-checkbox/custom-update-checkbox';
import { IEditableTextFieldProp, EditableTextFieldPropType} from '../../models/IEditableTextFieldProp';
import { POType } from '../../enums/po-types';
import { IActualPart } from '../../models/IActualPart';
import NumberFormat from 'react-number-format';
import { IGenericPart } from '../../models/IGenericPart';
import {SnackbarOrigin} from '@material-ui/core/Snackbar';
import {Lock } from "@material-ui/icons";


export class ActualPartsCardsGridItem extends React.Component<IActualPartsGridCardProps, IActualPartsGridCardState> {
    constructor(props: IActualPartsGridCardProps){
        super(props);

        this.state =
        {
            isDialogOpen: false,
            dialogTitle: "Confirm action",
            dialogCancelLabel: "Cancel",
            dialogConfirmLabel: "Confirm",
            dialogMessage: "",
            dialogCancelAction: () => { },
            dialogConfirmAction: () => { },
    
            isLoaderEnabled: false,
            isInvoiceLookupOpened: false,
            actualPart: this.props.actualPart,
            dirtyAttributes: this.props.dirtyAttributes
        };   
    }
    CalculatePartTotal = (actualpart:IGenericPart):number =>
    {
        let PartTotal:number = 0;
        const isEstimatedPart = this.props.tempService.isNotActualPart();

        const producttypecode = actualpart.ipg_potypecode;
        const quantity = actualpart.ipg_quantity || 0;
        const msrp = actualpart.Part?.ipg_manufacturerdiscountpricemoney || actualpart.Part?.ipg_msrp || 0;
        const costoverride = isEstimatedPart ? actualpart.ipg_unitcost : actualpart.ipg_enteredunitcost || 0;
        const shipping =  isEstimatedPart ? actualpart.ipg_unitshipping : actualpart.ipg_enteredshipping || 0;
        const tax = isEstimatedPart ? actualpart.ipg_unittax : actualpart.ipg_enteredtax || 0; 

        if(!producttypecode)
        {
            PartTotal = 0;
        }
        else if(producttypecode == POType.CPA)
        {
            PartTotal = quantity * (costoverride + shipping +  tax);
        }
        else
        {
            PartTotal = quantity * msrp;
        }

        return PartTotal;
    }

    validPOTypes = ():POType[] => this.props.tempService.getValidPOTypes(this.props.actualPart, this.props.case);
    
    OnfieldChanged = (fieldName:string, value:any) =>
    {          
        if(fieldName == "ipg_quantity" && this.state.actualPart.Part?.ipg_maxquantity && value > this.state.actualPart.Part?.ipg_maxquantity){
            let part;
            let message = "Quantity Implanted is exceeding the Max Quantity Per Case value. Max value is: " + this.state.actualPart.Part?.ipg_maxquantity;
            if(this.state.actualPart.Part?.ipg_enforcemaxquantity){
                part = {...this.state.actualPart, [fieldName]: 0}
                
                this.props.setNotification({sevirity:"error", message: message});
                return;
            }
            else
            {
                part = {...this.state.actualPart, [fieldName]: value}
                this.props.setNotification({sevirity:"warning", message: message});
            }
        }
        else if(fieldName == "ipg_potypecode" && !this.validPOTypes().includes(Number(value) as POType)
            || this.state.actualPart.Part?.producttypecode && (Number(value) as POType) != POType.CPA)
        {
            let part = {...this.state.actualPart}
            this.props.setNotification({sevirity:"error", message: "PO type is not valid!"});
            this.setState({actualPart: part});
            return;
        }
        else if (fieldName == "ipg_enteredunitcost" || fieldName == "ipg_potypecode") {
            let actualpart = this.state.actualPart as IActualPart;
      
            if (actualpart.ipg_potypecode == POType.CPA && actualpart.ipg_isapprovedoverrideprice != true) {
              let part = actualpart.Part;
              let mfg = part?.Manufacturer;
              let costoverride = (actualpart.ipg_enteredunitcost || 0);
              let msrp = (part?.ipg_msrp || 0);
              let discountPrice = (part?.ipg_manufacturerdiscountpricemoney || 0);
      
              //ipg_manufacturerisparticipating - Direct Bill
              if ((!mfg?.ipg_manufacturerisparticipating || mfg?.ipg_manufacturerisparticipating && discountPrice == 0)
                && costoverride > (msrp * 0.2 + msrp)
                || mfg?.ipg_manufacturerisparticipating && costoverride > discountPrice) {
                this.props.setNotification(
                  {
                    message: `Actual Part  ${part?.ipg_manufacturerpartnumber} has price of ${discountPrice == 0 ? "MSRP" : "Discount Price"} which exceeds threshold`,
                    sevirity: 'warning'
                  });
              }
            }
        }

        this.UpdateField(fieldName, value);         
    }

    UpdateField = (fieldName:string, value:any) =>  {
        if(!this.props.tempService.isValuesEqual(this.state.actualPart[fieldName],value))
        {
            if(!this.props.tempService.isValuesEqual(this.props.actualPart[fieldName], value))
            {
                !this.state.dirtyAttributes.includes(fieldName) && this.state.dirtyAttributes.push(fieldName);
                this.props.tempService.addFieldForUpdate(this.state.actualPart, fieldName, value);
            }
        };

        let part = {...this.state.actualPart, [fieldName]:value};

        this.setState({
            dirtyAttributes: this.state.dirtyAttributes,
            actualPart: part});
    }     

    CheckPOType = (actualpart:IGenericPart, ...params: POType[]):boolean =>
    {
        return params.find(type => type == actualpart.ipg_potypecode) != null;
    }

    private readonly _firstColumnFields:IEditableTextFieldProp[] =
    [
        { LogicalName: "Part.ipg_manufacturerpartnumber", Label: "Part #", isNotModifiable:true},
        { LogicalName: "Part.description", Label: "Part Description", isNotModifiable:true},
        { LogicalName: "Part._ipg_manufacturerid_value@OData.Community.Display.V1.FormattedValue", Label: "Manufacturer Name", isNotModifiable:true},
        { LogicalName: "Part.ipg_boxquantity", Label: "Box Qty", isNotModifiable:true},
        { LogicalName: "Part.Keyword", Label: "Keyword", isNotModifiable:true},
        { LogicalName: "Part.PriceBook", Label: "Price Book", isNotModifiable:true},
        { LogicalName: "Part.ClaimTier", Label: "Tier", isNotModifiable:true},
    ];
    private readonly _secondColumnFields: IEditableTextFieldProp[] = [
        { LogicalName: "ipg_quantity", Label: "Qty Implanted", Type: EditableTextFieldPropType.Number},
        { LogicalName: "ipg_quantitywasted", Label: "Qty Wasted", Type: EditableTextFieldPropType.Number, isReadOnly:this.props.tempService.isNotActualPart},
        { LogicalName: "ipg_serialnumber", Label: "Serial #", Type: EditableTextFieldPropType.Text, isReadOnly:this.props.tempService.isNotActualPart },
        { LogicalName: "ipg_lotnumber", Label: "Lot #", Type: EditableTextFieldPropType.Text, isReadOnly:this.props.tempService.isNotActualPart }
    ];

    private readonly _thirdColumnFields: IEditableTextFieldProp[] = [
        { LogicalName: "ipg_potypecode", Label: "PO Type", Type: EditableTextFieldPropType.OptionSet, filteredOptions:this.validPOTypes},
        { LogicalName: this.props.tempService.isNotActualPart() ? "ipg_unitcost" : "ipg_enteredunitcost", Label: "Unit Cost Override", Type: EditableTextFieldPropType.Сurrency, isReadOnly: (actualpart:IActualPart) => this.CheckPOType(actualpart, POType.MPO,POType.ZPO, POType.TPO) },
        { LogicalName: this.props.tempService.isNotActualPart() ? "ipg_unitshipping" : "ipg_enteredshipping", Label: "Unit Shipping", Type: EditableTextFieldPropType.Сurrency, isReadOnly: (actualpart:IActualPart) => this.CheckPOType(actualpart, POType.MPO,POType.ZPO, POType.TPO)  },
        { LogicalName: this.props.tempService.isNotActualPart() ? "ipg_unittax" : "ipg_enteredtax", Label: "Unit Tax", Type: EditableTextFieldPropType.Сurrency, isReadOnly: (actualpart:IActualPart) => this.CheckPOType(actualpart, POType.MPO,POType.ZPO, POType.TPO) },
        { LogicalName: "Part.ipg_msrp", Label: "MSRP", Type: EditableTextFieldPropType.Сurrency, isNotModifiable: true  },
        { LogicalName: "Part.ipg_manufacturerdiscountprice", Label: "IPG Discount Price", Type: EditableTextFieldPropType.Сurrency, isNotModifiable: true  },
        { LogicalName: "Part.ipg_averageinvoicecost", Label: "AIC", Type: EditableTextFieldPropType.Сurrency,isNotModifiable: true },
    ];


    voidPart = () => {
        this.setState({
            dialogTitle: "Confirm action",
            dialogCancelLabel: "Cancel",
            dialogConfirmLabel: "Confirm",
            dialogMessage: "Quantity of a part will be set to 0 and an order related to the part will be voided. CalcRev must be regenerated after removal of the part.",
            dialogConfirmAction: this.confirmVoidPart,
            dialogCancelAction: this.cancelConfirmDialog,

            isDialogOpen: true
        });
    }

    confirmVoidPart = async () => {
        if (this.props.actualPart.ipg_casepartdetailid) {
            this._crmActionHandler(async () => {
                const casePartService = new CasePartDetailService();
                return await casePartService.voidPartAction(this.props.actualPart.ipg_casepartdetailid) as any;
            });
        }
    }

    cancelConfirmDialog = () => {
        this.setState({
            isDialogOpen: false
        });
    }

    removePart = () => {
        this.setState({
            dialogTitle: "Confirm action",
            dialogCancelLabel: "Cancel",
            dialogConfirmLabel: "Confirm",
            dialogMessage: "Part will be removed from the case",
            dialogConfirmAction: this.confirmRemovePart,
            dialogCancelAction: this.cancelConfirmDialog,
            isDialogOpen: true
        });
    }

    confirmRemovePart = async () => {
        this.setState({
            isLoaderEnabled: true,
            isDialogOpen: false
        });

        const id = this.props.actualPart?.[this.props.tempService._service._keyField];

        if(await this.props.tempService._service.isOrderExistForPart(id))
        {
            this.props.setNotification({message:"You can't remove a part that become part of a PO", sevirity:"error"});
        }
        else if (id && this.props.tempService.isNotActualPart()) 
        {
            this._crmActionHandler(async () => {
                return await this.props.tempService._service.remove(id) as any;
            });
        }
        else 
        {
            this.props.actualPart.Part && this.props.tempService.removePart((this.props.actualPart as any) as IActualPart) 
        }
        
        this.props.refreshGrid();
    }

    openInvoiceLookup = () => {
        this.setState({
            isInvoiceLookupOpened: true
        });
    }

    onCloseInvoiceLookup = (costOverride?:Number) => {
        this.setState((prevState, props) =>{
        return {...prevState,isInvoiceLookupOpened: false}});
        
        if(costOverride)
        {
            this.OnfieldChanged("ipg_enteredunitcost", costOverride);
        }
    }

    GetValueByLogicalName =(name: string, actualPart:IGenericPart):any =>
    {
         return (name.indexOf("Part.") > -1 ? 
         actualPart.Part && (actualPart.Part as any)[name.replace("Part.", "")] 
         : actualPart[name]) || "";
    }
    defaultPoType = () => {
            const validPoTypes = this.validPOTypes();
             return validPoTypes.includes(923720000) // TPO 
                ? 923720000 // TPO 
                : validPoTypes.length == 1
                    ? validPoTypes[0]
                    : null;
    }
    private async _crmActionHandler(f: Function) { 
        this.setState({
            isLoaderEnabled: true,
            isDialogOpen: false
        });

        if (this.props.actualPart[this.props.tempService._service._keyField]) {
            this.setState({
                isLoaderEnabled: true,
                isDialogOpen: false
            });

            try {
                const response = await f() as any;

                this.setState({
                    isLoaderEnabled: false
                });

                if (response.IsSuccess) {
                    this.props.refreshGrid();
                }
                else {
                    this.props.setNotification({sevirity:"error", message: response.Message 
                    || (response.error && response.error.message)});
                }
            }
            catch (e) {
                this.props.setNotification({sevirity:"error", message: (e as any).Message || (e as any).message});
                this.setState({isLoaderEnabled: false});
            }
        }
    }

  render() {
        const isLocked = (this.state.actualPart as IActualPart)?.ipg_islocked;
        const isReadOnly = isLocked || this.props.disabled || this.state.isLoaderEnabled;
        const partTotal = this.CalculatePartTotal(this.state.actualPart) || 0;
        var position:SnackbarOrigin = {vertical: 'top',horizontal: 'center',}
        return (
            <Paper className={`actual-parts-cards-grid-item ${this.props.actualPart[this.props.tempService._service._keyField] ? "" : "temp-part"}`}>
                { this.state.isLoaderEnabled && <LinearProgress className="line-loader" />}
                {isLocked && <Lock/>}
                <Grid container direction="row">
                    <Grid container item  xs={3} direction="column" spacing={1}>
                    {this._firstColumnFields.map(x => (
                        <Grid key={`${this.props.actualPart.Part?.id}_${x.LogicalName}`}container item direction="row" spacing={1}>
                        <Grid item className="line-label" xs={5}>{x.Label}</Grid>
                        <Grid item className="line-value" xs={7}>{this.GetValueByLogicalName(x.LogicalName, this.props.actualPart)}</Grid>
                    </Grid>))}
                    </Grid>
                    <Grid container item  xs={3} direction="column" spacing={1}>
                    {this._secondColumnFields.map(x => (
                        <Grid container item direction="row" spacing={1}>
                        <Grid item className="line-label" xs={5}>{x.Label}</Grid>
                        <Grid item xs={7}><CustomUpdateTextField
                                    key={`${this.props.actualPart.Part?.id}_${x.LogicalName}`}
                                    isDirty={this.state.dirtyAttributes.findIndex(a => a === x.LogicalName) > -1}
                                    defaultValue={this.state.actualPart[x.LogicalName]}
                                    attributeLogicalName={x.LogicalName}
                                    type={x.Type}
                                    disabled={x.isReadOnly && x.isReadOnly(this.state.actualPart) || isReadOnly}
                              OnValueChange={this.OnfieldChanged} ></CustomUpdateTextField></Grid>
                    </Grid>))}
                    </Grid>
                    <Grid container item  xs={3} direction="column" spacing={1}>
                    {this._thirdColumnFields.map(x => (
                        <Grid container item direction="row" spacing={1}>
                        <Grid item className="line-label" xs={5}>{x.Label}</Grid>
                        <Grid item xs={7}> 
                        <CustomUpdateTextField
                            key={`${this.props.actualPart.Part?.id}_${x.LogicalName}`}
                            isDirty={this.state.dirtyAttributes.findIndex(a => a === x.LogicalName) > -1}
                            defaultValue= {this.GetValueByLogicalName(x.LogicalName, this.state.actualPart)}
                            attributeLogicalName={x.LogicalName}
                            type={x.Type}
                            disabled={x.isReadOnly && x.isReadOnly(this.state.actualPart) || isReadOnly}
                            isNotModifiable={x.isNotModifiable}
                            OnValueChange={this.OnfieldChanged}/>
                        </Grid>
                    </Grid>))}
                    </Grid>
                    <Grid container item xs={3} direction="column" spacing={1}>
                        <Grid item>
                            <Button
                                className="grid-card-button"
                                variant="outlined"
                                onClick={this.openInvoiceLookup}
                                disabled={isReadOnly || this.props.tempService.isNotActualPart() || !this.CheckPOType(this.state.actualPart, POType.CPA)}>
                                Invoice Lookup
                            </Button></Grid>
                        <Grid item> 
                            <Button
                                className="grid-card-button"
                                variant="outlined"
                                disabled={isReadOnly}
                                onClick={this.removePart}>
                                Remove Part
                            </Button></Grid>
                        <Grid item> 
                            <Button
                                className="grid-card-button"
                                variant="outlined"
                                onClick={this.voidPart}
                                disabled={isReadOnly || this.props.tempService.isNotActualPart()}>
                                Void Part
                            </Button></Grid>
                        <Grid item> 
                            <CustomUpdateCheckbox
                                key={`${this.props.actualPart.Part?.id}_ipg_iscourtesyclaimplan`}
                                tempService={this.props.tempService}
                                isDirty={this.state.dirtyAttributes.findIndex(a => a === "ipg_iscourtesyclaimplan") > -1}
                                actualPart={this.props.actualPart as IActualPart}
                                caseId={this.props.caseId}
                                defaultValue={this.props.actualPart.ipg_iscourtesyclaimplan}
                                attributeLogicalName={"ipg_iscourtesyclaimplan"}
                                label="Courtesy Claim Part"
                                disabled={this.props.tempService.isNotActualPart() || isReadOnly || !this.props.case.ipg_iscourtesyclaimcase || !this.CheckPOType(this.state.actualPart, POType.CPA)} 
                                OnValueChange={this.OnfieldChanged} />
                        </Grid>
                        <Grid item container direction='row' spacing={1}>
                            <Grid item className="line-label">Part Total</Grid>
                            <Grid item><NumberFormat 
                            key={`${this.props.actualPart.Part?.id}_PartTotal`}
                            value={partTotal}
                            displayType={"text"}
                            prefix={"$"}
                            thousandSeparator={true}
                            decimalScale={2}
                            fixedDecimalScale={true}  
                            /></Grid>
                        </Grid>
                    </Grid>
                </Grid>
                <ConfirmationDialog
                    isOpen={this.state.isDialogOpen}
                    title={this.state.dialogTitle}
                    cancelLabel={this.state.dialogCancelLabel}
                    confirmLabel={this.state.dialogConfirmLabel}
                    message={this.state.dialogMessage}
                    confirmAction={this.state.dialogConfirmAction}
                    closeAction={this.state.dialogCancelAction}>
                </ConfirmationDialog>
                <Modal
                    onClose={()=>{}}
                    open={this.state.isInvoiceLookupOpened}
                    disableBackdropClick={true}>
                    <InvoiceLookupModal
                        case={this.props.case}
                        actualPart={this.props.actualPart as IActualPart}
                        onClose={this.onCloseInvoiceLookup}></InvoiceLookupModal>
                </Modal>
            </Paper>
        );
    }
}
