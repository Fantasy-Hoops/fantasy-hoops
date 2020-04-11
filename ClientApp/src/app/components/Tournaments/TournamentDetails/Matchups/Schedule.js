import React from 'react';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogTitle from '@material-ui/core/DialogTitle';
import Stepper from "@material-ui/core/Stepper";
import Step from "@material-ui/core/Step";
import StepLabel from "@material-ui/core/StepLabel";
import StepContent from "@material-ui/core/StepContent";
import moment from "moment";
import {ContestState, getContestState, TOURNAMENT_DATE_FORMAT} from "../../../../utils/helpers";
import TableContainer from "@material-ui/core/TableContainer";
import Paper from "@material-ui/core/Paper";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableRow from "@material-ui/core/TableRow";
import TableCell from "@material-ui/core/TableCell";
import withStyles from "@material-ui/core/styles/withStyles";
import makeStyles from "@material-ui/core/styles/makeStyles";

const StyledTableCell = withStyles(theme => ({
    root: {
        minWidth: '10rem'
    },
    body: {
        fontSize: 14,
    },
}))(TableCell);

const StyledTableRow = withStyles(theme => ({
    root: {
        '&:nth-of-type(odd)': {
            backgroundColor: theme.palette.background.default,
        },
    },
}))(TableRow);

const useStyles = makeStyles({
    table: {
        maxWidth: '100%',
    },
});

export default function Schedule(props) {
    const {handleScheduleClose, open, contests} = props;
    const classes = useStyles();

    return (
        <Dialog
            open={open}
            onClose={handleScheduleClose}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
        >
            <DialogTitle id="alert-dialog-title">{"Schedule"}</DialogTitle>
            <DialogContent>
                <Stepper orientation="vertical">
                    {contests.map((contest, index) => {
                        return (
                            <Step active completed={moment(contest.contestEnd).isBefore()} key={index}>
                                <StepLabel>
                                    {moment(contest.contestEnd).isBefore()
                                        ? contest.winner
                                            ? `Winner: ${contest.winner.userName}`
                                            : 'Finished'
                                        : moment(contest.contestStart).format(TOURNAMENT_DATE_FORMAT)}
                                </StepLabel>
                                <StepContent>
                                    <TableContainer component={Paper}>
                                        <Table className={classes.table} aria-label="customized table">
                                            <TableBody>
                                                {contest.matchups.map((matchup, index) => (
                                                    <StyledTableRow key={index}>
                                                        <StyledTableCell align="center">
                                                            {matchup.firstUser.userName}
                                                        </StyledTableCell>
                                                        <StyledTableCell
                                                            align="center">{getContestState(contest) !== ContestState.NOT_STARTED ? `${matchup.firstUserScore} - ${matchup.secondUserScore}` : 'vs.'}</StyledTableCell>
                                                        <StyledTableCell
                                                            align="center">{matchup.secondUser.userName}</StyledTableCell>
                                                    </StyledTableRow>
                                                ))}
                                            </TableBody>
                                        </Table>
                                    </TableContainer>
                                </StepContent>
                            </Step>
                        );
                    })}
                </Stepper>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleScheduleClose} color="primary">
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    );
}