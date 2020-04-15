import React, {useState} from 'react';
import Typography from '@material-ui/core/Typography';
import {useStyles} from "./MatchupsDashboardStyle";
import Paper from "@material-ui/core/Paper";
import {Button} from "@material-ui/core";
import UserScore from "../../../Profile/UserScore";
import Standings from "../Standings";
import moment from "moment";
import Schedule from "./Schedule";
import {ContestState, getContestState, TOURNAMENT_DATE_FORMAT} from "../../../../utils/helpers";
import {parse} from "../../../../utils/auth";
import shortid from "shortid";
import {Link} from "react-router-dom";
import {MatchupDetails} from "./MatchupDetails";

import './MatchupsDashboard.css';
import Routes from "../../../../routes/routes";
import {ContestsTabs} from "../ContestsTabs";
import {TournamentNotStarted} from "../TournamentNotStarted";
import {TabPanel} from "../TabPanel";

const user = parse();

function parseContest(tournament, contest) {
    if (!tournament.isActive) {
        return <TournamentNotStarted contest={contest} />
    }

    const matchup = contest.matchups.filter(matchup => matchup.firstUser.userId === user.id || matchup.secondUser.userId === user.id)[0];
    switch (getContestState(contest)) {
        case ContestState.FINISHED:
            return getPastContest(contest, matchup);
        case ContestState.ACTIVE:
            return getCurrentContest(tournament, contest, matchup);
        case ContestState.NOT_STARTED:
            return getFutureContest(contest);
        default:
            return null;
    }
}

function getPastContest(contest, matchup) {
    const winner = matchup.firstUserScore > matchup.secondUserScore
        ? matchup.firstUser.username
        : matchup.secondUser.username;
    const heading = `Winner: ${winner}`;
    return (
        <>
            <MatchupDetails heading={heading} matchup={matchup}/>
        </>
    );
}

function getCurrentContest(tournament, contest, matchup) {
    return (
        <>
            <MatchupDetails
                heading={`Contest ends ${moment(contest.contestEnd).format(TOURNAMENT_DATE_FORMAT)}`}
                matchup={matchup}/>
            {tournament.currentLineup
                ? (
                    <div className="MatchupsDashboard__CurrentLineup">
                        <Typography align="center" variant="h5">
                            Current lineup
                        </Typography>
                        <UserScore
                            className="mx-auto"
                            key={shortid()}
                            activity={tournament.currentLineup}
                            current
                        />
                    </div>
                )
                : (
                    <div className="MatchupsDashboard__CurrentLineup">
                        <p>You haven't selected your lineup yet!</p>
                        <Link to={Routes.LINEUP}>
                            <Button color="primary" variant="contained">Select lineup</Button>
                        </Link>
                    </div>
                )}
            <div className="MatchupsDashboard__NextOpponent">
                <Typography variant="subtitle2">
                    Next Opponnent: <Link
                    to={`/profile/${tournament.nextOpponent}`}>{tournament.nextOpponent}</Link>
                </Typography>
            </div>
        </>
    );
}

function getFutureContest(contest) {
    const heading = "Upcoming matchups";
    const matchups = contest.matchups.map((matchup, index) => (
        <MatchupDetails key={index} matchup={matchup} future/>
    ));

    return (
        <>
            <Typography variant="h5" className="MatchupDetails__Heading">
                {heading}
            </Typography>
            {matchups}
        </>
    )
}

export default function MatchupsDashboard(props) {
    const classes = useStyles();
    const {tournament} = props;
    const initialTab = tournament.contests
        .map((contest) => moment(contest.contestStart).isBefore() && moment(contest.contestEnd).isAfter())
        .indexOf(true);
    const [value, setValue] = React.useState(initialTab > -1 ? initialTab : 0);
    const [scheduleOpen, setScheduleOpen] = useState(false);

    const handleChange = (event, newValue) => {
        setValue(newValue);
    };

    const handleScheduleOpen = () => {
        setScheduleOpen(true);
    };

    const handleScheduleClose = () => {
        setScheduleOpen(false);
    };

    return (
        <>
            <div className="MatchupsDashboard__Layout">
                <div className="MatchupsDashboard__ContentContainer MatchupsDashboard__Tabs">
                    <ContestsTabs tournament={tournament} handleChange={handleChange} value={value}/>
                </div>
                <div className="MatchupsDashboard__ContentContainer MatchupsDashboard__Schedule">
                    <Button className={classes.scheduleButton} variant="contained" color="primary" fullWidth
                            onClick={handleScheduleOpen} disabled={!tournament.isActive}>
                        Schedule
                    </Button>
                </div>
                <div className="MatchupsDashboard__ContentContainer MatchupsDashboard__Contest">
                    {tournament.contests.map((contest, index) => {
                        return (
                            <TabPanel key={index} className={classes.tabPanel} value={value} index={index}>
                                {parseContest(tournament, contest)}
                            </TabPanel>
                        );
                    })}
                </div>
                <div className="MatchupsDashboard__ContentContainer MatchupsDashboard__Standings">
                    <Paper className={classes.standingsHeading} elevation={3}>
                        <Typography variant="h4" className={classes.standingsLineHeight}>
                            Standings
                        </Typography>
                    </Paper>
                    <Standings tournament={tournament}/>
                </div>
            </div>
            {tournament.isActive && <Schedule contests={tournament.contests} handleScheduleOpen={handleScheduleOpen}
                      handleScheduleClose={handleScheduleClose} open={scheduleOpen}/>}
        </>
    );
}
