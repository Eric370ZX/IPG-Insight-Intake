import React from "react";

import { Checkbox, Paper, Table, TableBody, TableCell, TableContainer, TableFooter, TableHead, TablePagination, TableRow } from '@material-ui/core';

import "./advanced-search-results.css";

import { IPart } from "../../../models/IPart";
import { uniqueId } from "lodash";
import _ from "lodash";

interface State {
}

interface Props {
    parts: IPart[];
    totalAmount: number;
    page: number;
    rowsPerPage: number;

    selectedPartIds: {
        [key: string]: IPart;
    }

    onChangePage: Function;
    checkboxClicked: Function;
}

export class AdvancedSearchResults extends React.Component<Props, State> {
    public constructor(props: Props) {
        super(props);
    }

    private handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
        this.props.onChangePage(newPage);
    }

    private checkboxClicked = (part: IPart) => {
        this.props.checkboxClicked(part);
    }

    render() {
        const emptyRows = this.props.rowsPerPage - this.props.parts.length;

        return (
            <div className="advanced-search-results">
                <TableContainer className="advanced-search-results-container" component={Paper}>
                    <Table className="advanced-search-results-table" size="small">
                        <TableHead>
                            <TableRow>
                                <TableCell className="part-checkbox"></TableCell>
                                <TableCell className="part-number custom-overflow">Part #</TableCell>
                                <TableCell className="part-name custom-overflow">Part Name</TableCell>
                                <TableCell className="part-manufacturer custom-overflow">Manufacturer</TableCell>
                                <TableCell>HCPCS</TableCell>
                                <TableCell>Category</TableCell>
                                <TableCell>Keyword</TableCell>
                                <TableCell>Box Qty</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {this.props.parts
                                .map((part) => (
                                    <TableRow key={part.productid}>
                                        <TableCell size="small" padding="checkbox" className="part-checkbox">
                                            <Checkbox
                                                checked={!!this.props.selectedPartIds[part.productid] || false}
                                                onClick={() => this.checkboxClicked(part)} />
                                        </TableCell>
                                        <TableCell size="small" className="part-number custom-overflow">{part.ipg_manufacturerpartnumber} </TableCell>
                                        <TableCell size="small" className="part-name custom-overflow">{part.name}</TableCell>
                                        <TableCell size="small" className="part-manufacturer custom-overflow" >{part["_ipg_manufacturerid_value@OData.Community.Display.V1.FormattedValue"]}</TableCell>
                                        <TableCell size="small">{part["_ipg_hcpcscodeid_value@OData.Community.Display.V1.FormattedValue"]}</TableCell>
                                        <TableCell size="small">{part["producttypecode@OData.Community.Display.V1.FormattedValue"]}</TableCell>
                                        <TableCell size="small">{part["ipg_ipgpartnumber@OData.Community.Display.V1.FormattedValue"]}</TableCell>
                                        <TableCell size="small">{part.ipg_boxquantity}</TableCell>
                                    </TableRow>
                                ))}
                            {emptyRows > 0 && (
                                <TableRow key={uniqueId()} style={{ height: 33 * emptyRows }}>
                                    <TableCell colSpan={8} />
                                </TableRow>
                            )}
                        </TableBody>
                        <TableFooter className="actual-parts-table-footer">
                            <TableRow>
                                <TableCell colSpan={3}>
                                    {Object.keys(this.props.selectedPartIds).length} parts selected
                                </TableCell>
                                <TablePagination
                                    rowsPerPageOptions={[this.props.rowsPerPage]}
                                    count={this.props.totalAmount}
                                    rowsPerPage={this.props.rowsPerPage}
                                    page={this.props.page}
                                    onChangePage={this.handleChangePage}
                                />
                            </TableRow>
                        </TableFooter>
                    </Table>
                </TableContainer>
            </div >
        );
    }
}