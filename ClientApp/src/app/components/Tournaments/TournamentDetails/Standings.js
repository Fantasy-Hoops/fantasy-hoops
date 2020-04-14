import React from 'react';
import Paper from '@material-ui/core/Paper';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TablePagination from '@material-ui/core/TablePagination';
import TableRow from '@material-ui/core/TableRow';
import {Avatar} from "@material-ui/core";
import {useStyles} from "./StandingsStyle";

import defaultPhoto from '../../../../content/images/default.png';

const columns = [
    {id: 'pos', label: '#', minWidth: 10},
    {id: 'username', label: 'User', minWidth: 100},
    {
        id: 'w',
        label: 'W',
        minWidth: 50,
        format: (value) => value.toLocaleString(),
    },
    {
        id: 'l',
        label: 'L',
        minWidth: 50,
        format: (value) => value.toLocaleString(),
    },
];

function createData(pos, username, avatarUrl, w, l) {
    return {pos, username, avatarUrl, w, l};
}

export default function Standings(props) {
    const classes = useStyles();
    const [page, setPage] = React.useState(0);
    const [rowsPerPage, setRowsPerPage] = React.useState(10);
    const {standings} = props;
    const rows = standings.map(user => createData(user.position, user.username, user.avatarURL, user.w, user.l));

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(+event.target.value);
        setPage(0);
    };

    return (
        <Paper className={classes.root}>
            <TableContainer className={classes.container}>
                <Table stickyHeader aria-label="sticky table">
                    <TableHead>
                        <TableRow>
                            {columns.map((column) => (
                                <TableCell
                                    key={column.id}
                                    align={column.align}
                                    style={{minWidth: column.minWidth}}
                                >
                                    <span>{column.label}</span>
                                </TableCell>
                            ))}
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {rows.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((row, index) => {
                            return (
                                <TableRow hover role="checkbox" tabIndex={-1} key={index}>
                                    {columns.map((column, index) => {
                                        const value = row[column.id];
                                        return (
                                            <TableCell className={classes.cell} key={column.id} align={column.align}>
                                                <span className={classes.flexRow}>
                                                {
                                                    index === 1 &&
                                                    <Avatar
                                                        className={classes.avatar}
                                                        src={`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${row['avatarUrl']}.png`}
                                                    >
                                                        <img className={classes.avatar} alt="" src={defaultPhoto}/>
                                                    </Avatar>
                                                }
                                                    {column.format && typeof value === 'number'
                                                        ? <span>{column.format(value)}</span>
                                                        : <span className={index === 1 ? classes.cellValue : ''}>{value}</span>}
                                                </span>
                                            </TableCell>
                                        );
                                    })}
                                </TableRow>
                            );
                        })}
                    </TableBody>
                </Table>
            </TableContainer>
            <TablePagination
                classes={{
                    toolbar: classes.pagination,
                    caption: classes.halfWidth,
                    input: classes.halfWidth,
                    actions: classes.halfWidth
                }}
                rowsPerPageOptions={[10, 25, 100]}
                component="div"
                count={rows.length}
                rowsPerPage={rowsPerPage}
                page={page}
                onChangePage={handleChangePage}
                onChangeRowsPerPage={handleChangeRowsPerPage}
            />
        </Paper>
    );
}
