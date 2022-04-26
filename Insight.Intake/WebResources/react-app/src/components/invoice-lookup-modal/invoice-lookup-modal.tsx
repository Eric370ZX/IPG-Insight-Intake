import React from "react";

import { Link, Paper, Table, TableBody, Grid, TableCell, TableHead, TableRow, TableContainer, TablePagination, TableFooter, LinearProgress, Checkbox, Button } from "@material-ui/core";

import "./invoice-lookup-modal.css";
import { DocumentsService } from "../../services/documents-service";
import { IDocument } from "../../models/IDocument";
import { IActualPart } from "../../models/IActualPart";
import { ICase } from "../../models/ICase";
import { uniqueId } from 'lodash';
import NumberFormat from 'react-number-format';

interface IState {
    invoiceDocuments: IDocument[];
    isInvoicesFetched: boolean;
    page: number;
    rowsPerPage: number;
    totalAmount: number;
    selectedInvoice: string;
}

interface IProps {
    case:ICase,
    actualPart: IActualPart,
    onClose: Function;
}

export class InvoiceLookupModal extends React.Component<IProps, IState> {
    _documentsService: DocumentsService;
    _env:string;

    constructor(props: IProps) {
        super(props);

        this._documentsService = new DocumentsService();
        const host:string = window.location.host;

        if (host.indexOf('-dev') >= 0) {
            this._env = '-dev';
          }
          else if (host.indexOf('-qa') >= 0) {
            this._env = '-qa';
          }
          else if (host.indexOf('-prd') >= 0) {
            this._env = '-prd';
          }
          else {
            this._env = '';
          }
    }

    state: IState = {
        invoiceDocuments: [],
        isInvoicesFetched: false,
        page: 0,
        rowsPerPage: 10,
        totalAmount: 0,
        selectedInvoice: ""
    };

    async componentDidMount() {
        this.setState({isInvoicesFetched:false}); 
        if (this.props.actualPart.Part?.productid) {
            const invoiceDocuments = await this._documentsService
                .getManufacturerInvoicesByProduct(this.props.case, this.props.actualPart.Part?.productid, this.state.rowsPerPage, this.state.page + 1);

            this.setState({
                invoiceDocuments: invoiceDocuments.value,
                totalAmount: invoiceDocuments["@odata.count"],
                isInvoicesFetched: true
            });
        }
    }

    handleChangePage = async (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
        if (this.props.actualPart.Part?.productid) {
            this.setState({
                isInvoicesFetched: false
            });

            const invoiceDocuments = await this._documentsService
                .getManufacturerInvoicesByProduct(this.props.case, this.props.actualPart.Part.id, this.state.rowsPerPage, newPage + 1);

            this.setState({
                page: newPage,
                invoiceDocuments: invoiceDocuments.value,
                isInvoicesFetched: true
            });
        }
    }

    changeSelected = async (invoiceId: string) => {
        this.setState({
            selectedInvoice: this.state.selectedInvoice === invoiceId ? "" : invoiceId
        });
    }

    applyButtonClick = async () => {
        this.setState({
            isInvoicesFetched: false
        });
        try {
            var doc = this.state.invoiceDocuments.find(doc => doc.Id == this.state.selectedInvoice);

            await this._documentsService.update(
                {"ipg_documentid": doc?.Id
                , "ipg_CaseId@odata.bind": `/incidents(${this.props.case.incidentid})`});
            
            this.props.onClose(doc?.["actualPart.ipg_enteredunitcost"]);
        }
        catch (exc) {
            alert(exc);
        }
        finally{
            this.setState({
                isInvoicesFetched: false
            });
        }
    }

    closeButtonClick = async () => {
        this.props.onClose();
    }
    openDoc = async (docid:string, docName:string) => {

        this.setState({isInvoicesFetched:false});
        
        try
        {
            var annotationid = await this._documentsService.getNoteFromDocument(docid);
            if (annotationid) {
                window.open(`https://insight-documents${this._env}.azurewebsites.net/documents/${annotationid}`, docName);
            }
            else {
                window.open(`https://insight-documents${this._env}.azurewebsites.net/legacydocuments/${docid}`, docName);
            }
        }
        catch(e)
        {
            console.log(e);
        }
        finally {
            this.setState({isInvoicesFetched:true});
        }
    }

    render() {
        const emptyRows = this.state.rowsPerPage - this.state.invoiceDocuments.length;

        return (
            <div className="invoice-lookup-modal">
                <div className="invoice-lookup-modal-header">
                    <h2>Invoice lookup</h2>
                </div>
                <Grid container spacing={1} className="part-info" component={Paper}>
                    <Grid item xs={2}>
                        <div className="line"><span className="line-label">Case ID</span></div>
                        <div className="line"><span className="line-label">Procedure Name</span></div>
                        <div className="line"><span className="line-label">Facility</span></div>
                    </Grid>
                    <Grid item xs={3}>
                        <div className="line"><span className="line-value">{this.props.case.title}</span></div>
                        <div className="line"><span className="line-value">{this.props.case["_ipg_procedureid_value@OData.Community.Display.V1.FormattedValue"]}</span></div>
                        <div className="line"><span className="line-value">{this.props.case?.ipg_FacilityId?.name}</span></div>
                    </Grid>
                    <Grid item xs={2}>
                        <div className="line"><span className="line-label">Part #</span></div>
                        <div className="line"><span className="line-label">Part Name</span></div>
                        <div className="line"><span className="line-label">Manufacturer</span></div>
                    </Grid>
                    <Grid item xs={3}>
                        <div className="line"><span className="line-value">{this.props.actualPart.Part?.ipg_manufacturerpartnumber}</span></div>
                        <div className="line"><span className="line-value">{this.props.actualPart.Part?.name}</span></div>
                        <div className="line"><span className="line-value">{this.props.actualPart.Part?.Manufacturer?.name}</span></div>
                    </Grid>
                    </Grid>
                <div className="invoice-lookup-table-progress-container">
                    {!this.state.isInvoicesFetched && <LinearProgress className="invoice-lookup-table-progress-bar" />}
                </div>
                <TableContainer className="invoice-lookup-table-container" component={Paper}>
                    <Table className="invoice-lookup-table" size="small">
                        <TableHead>
                            <TableRow>
                                <TableCell></TableCell>
                                <TableCell>Case ID</TableCell>
                                <TableCell>Procedure Name</TableCell>
                                <TableCell>PO #</TableCell>
                                <TableCell>Qty</TableCell>
                                <TableCell>Cost Override</TableCell>
                                <TableCell>Document</TableCell>
                                <TableCell>Document Create Date</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {this.state.invoiceDocuments
                                .map((invoice) => (
                                    <TableRow key={invoice.Id}>
                                        <TableCell padding="checkbox">
                                            <Checkbox
                                                disabled={!this.state.isInvoicesFetched}
                                                checked={this.state.selectedInvoice === invoice.Id}
                                                onClick={() => this.changeSelected(invoice.Id)} />
                                        </TableCell>    
                                        <TableCell component="th" scope="row">{invoice["as.title"]}</TableCell>
                                        <TableCell>{invoice?.["as.ipg_procedureid"]?.name}</TableCell>
                                        <TableCell>{invoice?.["actualPart.ipg_purchaseorderid"]?.name}</TableCell>
                                        <TableCell>{invoice["actualPart.ipg_quantity"] || 0}</TableCell>
                                        <TableCell><NumberFormat 
                                                    value={invoice["actualPart.ipg_enteredunitcost"] || 0}
                                                    displayType={"text"}
                                                    prefix={"$"}
                                                    thousandSeparator={true}
                                                    decimalScale={2}
                                                    fixedDecimalScale={true}  
                                                    />
                                        </TableCell>
                                        <TableCell><Link href="#" onClick={()=>this.openDoc(invoice?.Id, invoice?.Name)}>{invoice?.Name}</Link></TableCell>
                                        <TableCell>{invoice.CreatedOnFormatted}</TableCell>
                                    </TableRow>
                                ))}
                            {emptyRows > 0 && (
                                <TableRow key={uniqueId()} style={{ height: 33 * emptyRows }}>
                                    <TableCell colSpan={4} />
                                </TableRow>
                            )}
                        </TableBody>
                        <TableFooter className="invoice-lookup-table-footer">
                            <TableRow>
                                <TablePagination
                                    rowsPerPageOptions={[this.state.rowsPerPage]}
                                    count={this.state.totalAmount}
                                    rowsPerPage={this.state.rowsPerPage}
                                    page={this.state.page}
                                    onChangePage={this.handleChangePage}
                                />
                            </TableRow>
                        </TableFooter>
                    </Table>
                </TableContainer>
                <div className="invoice-lookup-modal-buttons">
                    <Button
                        id="modal-apply-button"
                        className="button modal-apply-button"
                        variant="outlined"
                        disabled={!this.state.isInvoicesFetched}
                        onClick={this.applyButtonClick}>
                        Apply
                    </Button>
                    <Button
                        id="modal-close-button"
                        className="button modal-close-button"
                        variant="outlined"
                        disabled={!this.state.isInvoicesFetched}
                        onClick={this.closeButtonClick}>
                        Close
                    </Button>
                </div>
            </div>)
    }
}