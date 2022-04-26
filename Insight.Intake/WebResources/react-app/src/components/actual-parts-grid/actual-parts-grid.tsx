import React from "react";

import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';

import { EnhancedTableHead } from "./actual-parts-grid-table-head";
import { ActualPartsGridHeader } from "./actual-parts-grid-header";
import { IActualPartsGridProps } from '../../models/properties/IActualPartsGridProps';
import { IActualPart } from "../../models/IActualPart";
import { TableOrder } from "../../types/table-order";

export interface IHeadCell {
    disablePadding: boolean;
    id: keyof IActualPart;
    label: string;
    numeric: boolean;
}

interface IState {
    order: TableOrder,
    orderBy: keyof IActualPart,
    actualParts: IActualPart[],
    filteredParts: IActualPart[]
}

function descendingComparator<T>(a: T, b: T, orderBy: keyof T): number {
    if (b[orderBy] < a[orderBy]) {
        return -1;
    }
    if (b[orderBy] > a[orderBy]) {
        return 1;
    }
    return 0;
}

function getComparator(
    order: TableOrder,
    orderBy: keyof IActualPart,
): (a: IActualPart, b: IActualPart) => number {
    return order === 'desc'
        ? (a, b) => descendingComparator(a, b, orderBy)
        : (a, b) => -descendingComparator(a, b, orderBy);
}

function stableSort<T>(array: T[], comparator: (a: T, b: T) => number) {
    const stabilizedThis = array.map((el, index) => [el, index] as [T, number]);
    stabilizedThis.sort((a, b) => {
        const order = comparator(a[0], b[0]);
        if (order !== 0) return order;
        return a[1] - b[1];
    });

    return stabilizedThis.map((el) => el[0]);
}

export class ActualPartsGrid extends React.Component<IActualPartsGridProps, IState> {
    state: IState = {
        order: "asc",
        orderBy: "Manufacturer",
        actualParts: [],
        filteredParts: []
    };

    headCells: IHeadCell[] = [];

    constructor(props: IActualPartsGridProps) {
        super(props);

        this.headCells = [
            { id: "Manufacturer", numeric: false, disablePadding: true, label: "Manufacturer" },
            { id: "PartNumber", numeric: false, disablePadding: true, label: "Part #" },
            { id: "PartDescription", numeric: false, disablePadding: true, label: "Description" },
            { id: "ipg_quantity", numeric: true, disablePadding: true, label: "QTY" },
            { id: "ipg_truecost", numeric: true, disablePadding: true, label: "PO Unit Cost" },
            { id: "ipg_extcost", numeric: true, disablePadding: true, label: "PO Total Cost" },
            { id: "ipg_potypecode@OData.Community.Display.V1.FormattedValue", numeric: false, disablePadding: true, label: "Type" },
            { id: "Keyword", numeric: false, disablePadding: true, label: "Keyword" },
            { id: "_ipg_hcpcscode_value@OData.Community.Display.V1.FormattedValue", numeric: false, disablePadding: true, label: "HCPCS" },
            { id: "modifiedon", numeric: false, disablePadding: true, label: "Modified Date"}
        ];

        const sortedParts = stableSort<IActualPart>(props.actualParts, getComparator("asc", "Manufacturer"))

        this.state = {
            order: "asc",
            orderBy: "Manufacturer",
            actualParts: props.actualParts,
            filteredParts: sortedParts
        };
    }

    handleRequestSort = (event: React.MouseEvent<unknown>, property: keyof IActualPart) => {
        const isAsc = this.state.orderBy === property && this.state.order === 'asc';
        const order = isAsc ? 'desc' : 'asc';

        const sortedParts = stableSort<IActualPart>(this.state.filteredParts, getComparator(order, property));

        this.setState({
            order: order,
            orderBy: property,
            filteredParts: sortedParts
        })
    }

    GetFormattedValue = (value:any):string =>
    {
        if(!isNaN(Number(value)))
        {
            return value;
        }
        else if(!isNaN(Date.parse(value)))
        {
            return new Date(value).toLocaleDateString();
        }
        else if(value?.name)
        {
            return value?.name;
        }
        else
        {
            return value ?? "";
        }
    }

    handleFilter = (filterText: string) => {
        if (!filterText) {
            this.setState({
                filteredParts: stableSort<IActualPart>(this.state.actualParts, getComparator(this.state.order, this.state.orderBy))
            });
        }
        else {
            filterText = filterText && filterText.toLowerCase();

            const filteredParts = this.state.actualParts.filter(part => {
                for (let [key, value] of Object.entries(part)) {
                    if (value) {
                        if (value.constructor === Date && value.toLocaleString)
                            value = value.toLocaleString();
                        else if (value.toString)
                            value = value.toString();
                        
                        if (value.toLowerCase)
                            value = value.toLowerCase();

                        if (value.includes && value.includes(filterText))
                            return true;
                    }
                }

                return false;
            });

            const sortedParts = stableSort<IActualPart>(filteredParts, getComparator(this.state.order, this.state.orderBy));

            this.setState({
                filteredParts: sortedParts
            })
        }
    }

    render() {
        return (
            <div>
                <ActualPartsGridHeader onFilterChange={this.handleFilter} />
                <TableContainer component={Paper}>
                    <Table className="actualPartsTable" size="small">
                        <EnhancedTableHead
                            headCells={this.headCells}
                            order={this.state.order}
                            orderBy={this.state.orderBy}
                            onRequestSort={this.handleRequestSort}>
                        </EnhancedTableHead>
                        <TableBody>
                            {this.state.filteredParts.map((part) => (
                                <TableRow key={part.ipg_casepartdetailid} hover>
                                    {this.headCells.map(cell => (<TableCell>{this.GetFormattedValue(part[cell.id])}</TableCell>))}
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </div>
        );
    }
}