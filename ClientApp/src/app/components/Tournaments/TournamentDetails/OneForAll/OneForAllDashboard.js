import React from 'react';

import './OneForAllDashboard.css';
import {ContestsTabs} from "../ContestsTabs";
import moment from "moment";
import Paper from "@material-ui/core/Paper";
import Standings from "../Standings";
import Typography from "@material-ui/core/Typography";
import {useStyles} from "./OneForAllDashboardStyle";

export default function OneForAllDashboard(props) {
    const classes = useStyles();
    const {tournament} = props;
    const initialTab = tournament.contests
        .map((contest) => moment(contest.contestStart).isBefore() && moment(contest.contestEnd).isAfter())
        .indexOf(true);
    const [value, setValue] = React.useState(initialTab > -1 ? initialTab : 0);

    const handleChange = (event, newValue) => {
        setValue(newValue);
    };

    return (
        <>
            <div className="OneForAllDashboard__Layout">
                <div className="OneForAllDashboard__ContentContainer OneForAllDashboard__Tabs">
                    <ContestsTabs tournament={tournament} handleChange={handleChange} value={value}/>
                </div>
                <div className="OneForAllDashboard__ContentContainer OneForAllDashboard__Contest"></div>
                <div className="OneForAllDashboard__ContentContainer OneForAllDashboard__Standings">
                    <Paper className={classes.standingsHeading} elevation={3}>
                        <Typography variant="h4" className={classes.standingsLineHeight}>
                            Standings
                        </Typography>
                    </Paper>
                    <Standings standings={tournament.standings}/>
                </div>
            </div>
        </>
    );
}