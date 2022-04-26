import React from "react";
import { TableHead, TableRow, TableCell, TableSortLabel } from "@material-ui/core";
import { IActualPart } from "../../models/IActualPart";
import { TableOrder } from "../../types/table-order";
import { IHeadCell } from "./actual-parts-grid";

interface IEnhancedTableProps {
    onRequestSort: (event: React.MouseEvent<unknown>, property: keyof IActualPart) => void;
    order: TableOrder;
    orderBy: keyof IActualPart;
    headCells: IHeadCell[];
}

export class EnhancedTableHead extends React.Component<IEnhancedTableProps> {
    private _createSortHandler = (property: keyof IActualPart) => (event: React.MouseEvent<unknown>) => {
        this.props.onRequestSort(event, property);
    };

    render() {
        return (
            <TableHead>
                <TableRow>
                    {this.props.headCells.map((headCell) => (
                        <TableCell
                            key={headCell.id}
                            sortDirection={this.props.orderBy === headCell.id ? this.props.order : false}
                        >
                            <TableSortLabel
                                active={this.props.orderBy === headCell.id}
                                direction={this.props.orderBy === headCell.id ? this.props.order : 'asc'}
                                onClick={this._createSortHandler(headCell.id)}
                            >
                                {headCell.label}
                            </TableSortLabel>
                        </TableCell>
                    ))}
                </TableRow>
            </TableHead>
        );
    }
}