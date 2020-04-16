import React, {useState} from "react";
import clsx from "clsx";
import format from "date-fns/format";
import isValid from "date-fns/isValid";
import isSameDay from "date-fns/isSameDay";
import endOfWeek from "date-fns/endOfWeek";
import startOfWeek from "date-fns/startOfWeek";
import isWithinInterval from "date-fns/isWithinInterval";
import {DatePicker, MuiPickersUtilsProvider} from "@material-ui/pickers";
import enLocale from "date-fns/locale/en-GB";
import {DatePickerTypes, makeJSDateObject} from "./utils";
import {IconButton} from "@material-ui/core";
import {useStyles} from "./CustomDatePickerStyle";
import DateFnsUtils from "@date-io/date-fns";

function CustomDatePicker(props) {
    const {styles, selectedDate, selectedWeek, label, type, autoOk, onDateChange, onWeekChange, minDate, maxDate, disableFuture} = props;
    const propsStyles = styles();
    const defaultStyles = useStyles();

    const classes = {...propsStyles, ...defaultStyles};

    function formatWeekSelectLabel(date, invalidLabel) {
        let dateClone = makeJSDateObject(date);

        return dateClone && isValid(dateClone)
            ? `Week of ${format(startOfWeek(dateClone, {locale: enLocale}), "MMM do")}`
            : invalidLabel;
    };

    function renderWrappedWeekDay(date, selectedDate, dayInCurrentMonth) {
        let dateClone = makeJSDateObject(date);
        let selectedDateClone = makeJSDateObject(selectedDate);

        const start = startOfWeek(selectedDateClone, {locale: enLocale});
        const end = endOfWeek(selectedDateClone, {locale: enLocale});

        const dayIsBetween = isWithinInterval(dateClone, {start, end});
        const isFirstDay = isSameDay(dateClone, start);
        const isLastDay = isSameDay(dateClone, end);

        const wrapperClassName = clsx({
            [classes.highlight]: dayIsBetween,
            [classes.firstHighlight]: isFirstDay,
            [classes.endHighlight]: isLastDay,
        });

        const dayClassName = clsx(classes.day, {
            [classes.nonCurrentMonthDay]: !dayInCurrentMonth,
            [classes.highlightNonCurrentMonthDay]: !dayInCurrentMonth && dayIsBetween,
        });

        return (
            <div className={wrapperClassName}>
                <IconButton className={dayClassName}>
                    <span> {format(dateClone, "d")} </span>
                </IconButton>
            </div>
        );
    };

    if (type === DatePickerTypes.DAY) {
        return (
            <MuiPickersUtilsProvider utils={DateFnsUtils} locale={enLocale}>
                <DatePicker
                    autoOk={autoOk}
                    disableFuture={disableFuture}
                    label={label}
                    value={selectedDate}
                    minDate={minDate}
                    maxDate={maxDate}
                    onChange={onDateChange}
                    animateYearScrolling
                />
            </MuiPickersUtilsProvider>
        );
    }

    if (type === DatePickerTypes.WEEK) {
        return (
            <MuiPickersUtilsProvider utils={DateFnsUtils} locale={enLocale}>
                <DatePicker
                    autoOk={autoOk}
                    disableFuture={disableFuture}
                    label={label}
                    value={selectedWeek}
                    minDate={minDate}
                    maxDate={maxDate}
                    onChange={onWeekChange}
                    renderDay={renderWrappedWeekDay}
                    labelFunc={formatWeekSelectLabel}
                    animateYearScrolling
                />
            </MuiPickersUtilsProvider>
        );
    }

    return (
        <div>Provide valid DatePicker type!</div>
    );
}

export default CustomDatePicker;