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
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from '@material-ui/icons/Close';
import Typography from "@material-ui/core/Typography";
import Slide from "@material-ui/core/Slide";

const StyledTableCell = withStyles(theme => ({
    root: {
        width: '8rem',
        maxWidth: '8rem',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        padding: '1rem 0',
        '&:nth-of-type(1)': {
            paddingLeft: '.5rem'
        }
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

const useStyles = makeStyles(theme => ({
    table: {
        maxWidth: '100%',
    },
    appBar: {
        position: 'relative',
    },
    title: {
        marginLeft: theme.spacing(2),
        flex: 1,
    },
    stepper: {
        maxWidth: '70rem',
        margin: '2rem auto',
        padding: 0
    },
    tableContainer: {
        padding: 0
    }
}));

const Transition = React.forwardRef(function Transition(props, ref) {
    return <Slide direction="up" ref={ref} {...props} />;
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
            fullScreen
            TransitionComponent={Transition}
        >
            <AppBar className={classes.appBar}>
                <Toolbar>
                    <IconButton edge="start" color="inherit" onClick={handleScheduleClose} aria-label="close">
                        <CloseIcon/>
                    </IconButton>
                    <Typography variant="h6" className={classes.title}>
                        Schedule
                    </Typography>
                    <Button autoFocus color="inherit" onClick={handleScheduleClose}>
                        Close
                    </Button>
                </Toolbar>
            </AppBar>
            <DialogContent>
                <Stepper className={classes.stepper} orientation="vertical">
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
                                    <TableContainer component={Paper} className={classes.tableContainer}>
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
        </Dialog>
    );
}