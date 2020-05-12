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
import BlockIcon from '@material-ui/icons/Block';

import defaultPhoto from '../../../../content/images/default.png';

const oneForAllColumns = [
    {id: 'pos', label: '#', minWidth: 10},
    {id: 'username', label: 'User', minWidth: 100},
    {
        id: 'points',
        label: 'Points',
        maxWidth: 50,
        format: (value) => value.toLocaleString(),
        align: 'right'
    }
];

const matchupsColumns = [
    {id: 'pos', label: '#', minWidth: 10},
    {id: 'username', label: 'User', minWidth: 100},
    {
        id: 'w',
        label: 'W',
        minWidth: 50,
        format: (value) => value.toLocaleString(),
        align: 'right'
    },
    {
        id: 'l',
        label: 'L',
        minWidth: 50,
        format: (value) => value.toLocaleString(),
        align: 'right'
    }
];

function createData(pos, username, avatarUrl, w, l, points, isEliminated) {
    return {pos, username, avatarUrl, w, l, points, isEliminated};
}

export default function Standings(props) {
    const classes = useStyles();
    const [page, setPage] = React.useState(0);
    const [rowsPerPage, setRowsPerPage] = React.useState(10);
    const {tournament} = props;
    const standings = tournament.standings;
    const rows = standings.map(user => createData(user.position, user.username, user.avatarUrl, user.w, user.l, user.points, user.isEliminated));

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(+event.target.value);
        setPage(0);
    };

    const columns = tournament.type === 0
        ? oneForAllColumns
        : matchupsColumns;
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
                                <TableRow hover role="checkbox" tabIndex={-1} key={index}
                                          className={row.isEliminated ? classes.userEliminated : ''}>
                                    {columns.map((column, index) => {
                                        const value = row[column.id];
                                        const columnValue = column.format && typeof value === 'number'
                                            ? <span>{column.format(value)}</span>
                                            : <span
                                                className={index === 1 ? classes.cellValue : ''}>{value}</span>;
                                        const avatarUrl = `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${row['avatarUrl']}.png`;
                                        return (
                                            <TableCell className={classes.cell} key={column.id} align={column.align}>
                                                {
                                                    index === 0
                                                        ? row.isEliminated ? <BlockIcon className={classes.blockIcon} /> : columnValue
                                                        : index === 1
                                                        ? (
                                                            <span className={classes.flexRow}>
                                                                <Avatar
                                                                    className={classes.avatar}
                                                                    alt=""
                                                                    src={avatarUrl}
                                                                >
                                                                    <img className={classes.avatar} alt=""
                                                                         src={defaultPhoto}/>
                                                                </Avatar>
                                                                {columnValue} 
                                                            </span>
                                                        )
                                                        : columnValue
                                                }
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
