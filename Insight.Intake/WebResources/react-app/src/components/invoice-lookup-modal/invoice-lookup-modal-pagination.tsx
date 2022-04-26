import { IconButton } from "@material-ui/core";
import { KeyboardArrowLeft, KeyboardArrowRight } from "@material-ui/icons";
import React from "react";

interface IProps {
    amount: number;
    rowsPerPage: number;

    onChangePage: Function;
}

interface IState {
    page: number;
}

export class InvoiceLookupModalPagination extends React.Component<IProps, IState> {
    constructor(props: IProps) {
        super(props);
    }

    handleFirstPageButtonClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        this.setState({
            page: 1
        });

        this.props.onChangePage(event, 1);
    };

    handleBackButtonClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        const page = this.state.page - 1;
        this.setState({
            page: page
        });

        this.props.onChangePage(event, page);
    };

    handleNextButtonClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        const page = this.state.page + 1;
        this.setState({
            page: page
        });

        this.props.onChangePage(event, page);
    };

    handleLastPageButtonClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        const page = Math.max(0, Math.ceil(this.props.amount / this.props.rowsPerPage) - 1);
        this.setState({
            page: page
        });

        this.props.onChangePage(event, page);
    };

    render() {
        return (
            <div>
                <IconButton
                    onClick={this.handleFirstPageButtonClick}
                    disabled={this.state.page === 0}
                    aria-label="first page"
                >
                </IconButton>
                <IconButton onClick={this.handleBackButtonClick} disabled={this.state.page === 0} aria-label="previous page">
                    <KeyboardArrowLeft />
                </IconButton>
                <IconButton
                    onClick={this.handleNextButtonClick}
                    disabled={this.state.page >= Math.ceil(this.props.amount / this.props.rowsPerPage) - 1}
                    aria-label="next page"
                >
                    <KeyboardArrowRight />
                </IconButton>
                <IconButton
                    onClick={this.handleLastPageButtonClick}
                    disabled={this.state.page >= Math.ceil(this.props.amount / this.props.rowsPerPage) - 1}
                    aria-label="last page"
                >
                </IconButton>
            </div>
        );
    }
}