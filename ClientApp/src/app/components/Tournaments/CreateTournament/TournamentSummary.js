import React from 'react';
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import Paper from "@material-ui/core/Paper";
import {useStyles} from "./TournamentSummaryStyle";
import TableCell from "@material-ui/core/TableCell";
import TableRow from "@material-ui/core/TableRow";
import TableBody from "@material-ui/core/TableBody";
import TableContainer from "@material-ui/core/TableContainer";
import Table from "@material-ui/core/Table";
import {camelCaseToSentenceCase, getTournamentType} from "../../../utils/helpers";
import {Avatar} from "@material-ui/core";
import moment from "moment";

const DATE_FORMAT = 'MMMM Do YYYY, h:mm:ss a';

export default function TournamentSummary(props) {
    const classes = useStyles();
    const {formProps, handleBack} = props;
    const {values, submitForm} = formProps;
    
    function parseValue(key, value) {
        switch (key) {
            case 'tournamentIcon':
                return (
                    <Avatar alt={key} src={value} className={classes.small}/>
                );
            case 'startDate':
                return moment(value).format(DATE_FORMAT);
            case 'tournamentType':
                return getTournamentType(value);
            default:
                return value;
        }
    }
    
    return (
        <Paper square elevation={0} className={classes.resetContainer}>
            <Typography variant="h2" className={classes.title}>Tournament Summary</Typography>
            <TableContainer component={Paper}>
                <Table className={classes.table} size="small" aria-label="summary table">
                    <TableBody>
                        {Object.keys(values).map((key, index) => (
                            key !== 'userFriends' && <TableRow key={key}>
                                <TableCell component="th" scope="row">
                                    {camelCaseToSentenceCase(key)}
                                </TableCell>
                                <TableCell>
                                    {parseValue(key, values[key])}
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            <Button color="primary" variant="contained" onClick={submitForm} className={classes.button}>
                Submit
            </Button>
            <Button onClick={handleBack} className={classes.button}>
                Back
            </Button>
        </Paper>
    );
};
