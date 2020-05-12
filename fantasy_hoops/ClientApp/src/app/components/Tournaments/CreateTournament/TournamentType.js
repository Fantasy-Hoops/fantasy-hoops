import React, {useEffect, useState} from 'react';
import MaterialSelect from "../../Inputs/MaterialSelect";
import moment from "moment";
import ClickAwayListener from "@material-ui/core/ClickAwayListener";
import Tooltip from "@material-ui/core/Tooltip";
import InfoIcon from '@material-ui/icons/Info';
import IconButton from "@material-ui/core/IconButton";

import './TournamentType.css';
import TableContainer from "@material-ui/core/TableContainer";
import Table from "@material-ui/core/Table";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TableBody from "@material-ui/core/TableBody";
import Paper from "@material-ui/core/Paper";
import {useStyles} from "./TournamentTypeStyle";
import TableCell from "@material-ui/core/TableCell";
import _ from "lodash";
import {TextField} from "@material-ui/core";
import {TOURNAMENT_DATE_FORMAT} from "../../../utils/helpers";
import BlockIcon from '@material-ui/icons/Block';

export default function TournamentType(props) {
    const classes = useStyles();
    const {tournamentTypes, initialContests, initialDroppedContests, startDates, formProps} = props;
    const {values, errors, touched, handleChange, setFieldTouched, setFieldValue} = formProps;
    
    const [scheduledDates, setScheduledDates] = useState([]);
    const [leftDates, setLeftDates] = useState([]);
    const [contests, setContests] = useState([]);
    const [droppedContests, setDroppedContests] = useState([]);
    const [droppedTooltipOpen, setDroppedTooltipOpen] = React.useState(false);
    const [startDateToolTipOpen, setStartDateToolTipOpen] = React.useState(false);

    const handleDroppedTooltipClose = () => {
        setDroppedTooltipOpen(false);
    };

    const handleDroppedTooltipOpen = () => {
        setDroppedTooltipOpen(true);
    };
    const handleStartDateTooltipClose = () => {
        setStartDateToolTipOpen(false);
    };

    const handleStartDateTooltipOpen = () => {
        setStartDateToolTipOpen(true);
    };

    useEffect(() => {
        setContests(initialContests);
        setDroppedContests(initialDroppedContests);
        toggleScheduledDates(startDates, values.contests);
    }, []);

    const change = (name, e) => {
        e.persist();
        handleChange(e);
        setFieldTouched(name, true, false);
    };

    const handleDateChange = (name, e) => {
        const {values, setFieldValue} = formProps;
        const datesLeft = startDates.filter(date => date.value >= e.target.value);
        setLeftDates(datesLeft);
        let contestsCount = values.contests;
        if (values.contests > datesLeft.length) {
            setFieldValue('contests', datesLeft.length);
            setFieldValue('droppedContests', datesLeft.length - 1);
            contestsCount = datesLeft.count;
        }
        setContests(initialContests.slice(0, datesLeft.length));
        setDroppedContests(initialDroppedContests.slice(0, contestsCount));
        toggleScheduledDates(datesLeft, contestsCount);
        change(name, e);
    };

    const handleContestsChange = (name, e) => {
        toggleScheduledDates(leftDates, e.target.value);
        change(name, e);
    };

    const handleDroppedContestsChange = (name, e) => {
        const {values} = formProps;
        toggleScheduledDates(leftDates, values.contests, e.target.value);
        change(name, e);
    };

    const toggleScheduledDates = (dates, contestsCount, droppedContests) => {
        const scheduledDates = dates.slice(0, contestsCount).map((date, index) => (
            <TableRow key={index} className={classes.tableRow}>
                <TableCell>
                    <div className={classes.scheduleEntry}>{moment(date.value).format(TOURNAMENT_DATE_FORMAT)}</div>
                    {index + 1 > contestsCount - droppedContests && contestsCount > 1
                        ? <BlockIcon className={classes.blockIcon}/> : null}
                </TableCell>
            </TableRow>
        ));
        if (contestsCount === 1) {
            setFieldValue('droppedContests', 0);
        }
        setDroppedContests(initialDroppedContests.slice(0, contestsCount));
        setScheduledDates(scheduledDates);
    };

    return (
        <>
            <p>
                {"Select tournament type, start date, number of contests, number of dropped contests and maximum " +
                "number of entrants (if applicable)."}
            </p>
            <MaterialSelect
                id="tournamentType"
                label="Tournament Type"
                values={tournamentTypes}
                value={values.tournamentType}
                onChange={change.bind(null, "tournamentType")}
                error={touched.tournamentType && !_.isEmpty(errors.tournamentType)}
                helperText={touched.tournamentType ? errors.tournamentType : ''}
                emptyOption
                required
            />
            <br/>
            <MaterialSelect
                id="startDate"
                label="Start Date"
                values={startDates}
                value={values.startDate}
                onChange={handleDateChange.bind(null, "startDate")}
                error={touched.startDate && !_.isEmpty(errors.startDate)}
                helperText={touched.startDate ? errors.startDate : ''}
                emptyOption
                required
            />
            <ClickAwayListener onClickAway={handleStartDateTooltipClose}>
                <Tooltip
                    PopperProps={{
                        disablePortal: true,
                    }}
                    onClose={handleStartDateTooltipClose}
                    open={startDateToolTipOpen}
                    disableFocusListener
                    disableHoverListener
                    disableTouchListener
                    title="All dates are in ECT"
                >
                    <IconButton onClick={handleStartDateTooltipOpen} className="TournamentType__Tooltip">
                        <InfoIcon/>
                    </IconButton>
                </Tooltip>
            </ClickAwayListener>
            <br/>
            <MaterialSelect
                id="contests"
                label="Contests"
                values={contests}
                value={values.contests}
                onChange={handleContestsChange.bind(null, "contests")}
                error={touched.contests && !_.isEmpty(errors.contests)}
                helperText={touched.contests ? errors.contests : ''}
                emptyOption
                required
            />
            <br/>
            <MaterialSelect
                id="droppedContests"
                label="Dropped Contests"
                values={droppedContests}
                value={values.droppedContests}
                onChange={handleDroppedContestsChange.bind(null, "droppedContests")}
                error={touched.droppedContests && !_.isEmpty(errors.droppedContests)}
                helperText={touched.droppedContests ? errors.droppedContests : ''}
                emptyOption
                disabled={droppedContests.length === 0 || values.contests === 1 || values.tournamentType === 1}
                required
            />
            <ClickAwayListener onClickAway={handleDroppedTooltipClose}>
                <Tooltip
                    PopperProps={{
                        disablePortal: true,
                    }}
                    onClose={handleDroppedTooltipClose}
                    open={droppedTooltipOpen}
                    disableFocusListener
                    disableHoverListener
                    disableTouchListener
                    title="Last player drops after dropout contest"
                >
                    <IconButton onClick={handleDroppedTooltipOpen} className="TournamentType__Tooltip">
                        <InfoIcon/>
                    </IconButton>
                </Tooltip>
            </ClickAwayListener>
            <br/>
            <TextField
                className={classes.textField}
                type="number"
                inputProps={{
                    min: values.droppedContests !== 0
                        ? values.droppedContests + 1
                        : 2,
                    max: "50",
                    step: "1"}}
                margin="normal"
                id="entrants"
                label="Entrants"
                name="entrants"
                value={values.entrants}
                onChange={change.bind(null, "entrants")}
                error={touched.entrants && !_.isEmpty(errors.entrants)}
                helperText={touched.entrants ? errors.entrants : ''}
                required
            />
            {(values.startDate && values.contests) && (
                <>
                    <br/>
                    <TableContainer component={Paper}>
                        <Table className={classes.table} aria-label="schedule table">
                            <TableHead>
                                <TableRow><TableCell>Schedule</TableCell></TableRow>
                            </TableHead>
                            <TableBody>
                                {scheduledDates}
                            </TableBody>
                        </Table>
                    </TableContainer>
                </>
            )}
        </>
    );
}