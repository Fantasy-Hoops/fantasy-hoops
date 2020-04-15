import React, {useState} from 'react';

import './OneForAllDashboard.css';
import {ContestsTabs} from "../ContestsTabs";
import moment from "moment";
import Paper from "@material-ui/core/Paper";
import Standings from "../Standings";
import Typography from "@material-ui/core/Typography";
import {useStyles} from "./OneForAllDashboardStyle";
import {TournamentNotStarted} from "../TournamentNotStarted";
import {ContestState, getContestState, TOURNAMENT_DATE_FORMAT} from "../../../../utils/helpers";
import {parse} from "../../../../utils/auth";
import {TabPanel} from "../TabPanel";
import {Card as UserScoreCard} from "../../../Leaderboard/Users/Card";
import UserScore from "../../../Profile/UserScore";
import shortid from "shortid";
import {Link} from "react-router-dom";
import {Avatar, Button} from "@material-ui/core";
import Routes from "../../../../routes/routes";
import {ContestLeaderboard} from "./ContestLeaderboard";
import {MatchupDetails} from "../Matchups/MatchupDetails";
import defaultPhoto from "../../../../../content/images/default.png";

const user = parse();

const avatarStyle = {
    margin: "1rem auto",
    width: "8rem",
    height: "8rem",
};

function parseContestHeading(contest) {
    switch (getContestState(contest)) {
        case ContestState.FINISHED:
            return (
                contest.winner && <Typography variant="h5" className="m-2">
                    {`Winner: ${contest.winner.username}`}
                    <Avatar
                        src={`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${contest.winner.avatarUrl}.png`}
                        style={avatarStyle}
                    >
                        <img style={avatarStyle} alt="" src={defaultPhoto}/>
                    </Avatar>
                </Typography>
            );
        case ContestState.ACTIVE:
            return (
                <Typography variant="h5" className="m-2">
                    {`Contest ends ${moment(contest.contestEnd).format(TOURNAMENT_DATE_FORMAT)}`}
                </Typography>
            );
        case ContestState.NOT_STARTED:
            return (
                <Typography variant="h5" className="m-2">
                    {`Contest starts ${moment(contest.contestStart).format(TOURNAMENT_DATE_FORMAT)}`}
                </Typography>
            )
        default:
            return null;
    }
}

function parseContest(tournament, contest) {
    if (!tournament.isActive && !tournament.isFinished) {
        return <TournamentNotStarted contest={contest}/>
    }

    switch (getContestState(contest)) {
        case ContestState.FINISHED:
            return getPastContest(contest);
        case ContestState.ACTIVE:
            return getCurrentContest(tournament, contest);
        case ContestState.NOT_STARTED:
            return getFutureContest(contest);
        default:
            return null;
    }
}

function getPastContest(contest) {
    return (
        <>

        </>
    );
}

function getCurrentContest(tournament, contest) {
    return tournament.currentLineup
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
        );
}

function getFutureContest(contest) {
    return (
        <>
        </>
    )
}

export default function OneForAllDashboard(props) {
    const classes = useStyles();
    const {tournament} = props;
    const initialTab = tournament.contests
        .map((contest) => moment(contest.contestStart).isBefore() && moment(contest.contestEnd).isAfter())
        .indexOf(true);
    const [value, setValue] = React.useState(initialTab > -1 ? initialTab : 0);
    const [leaderboardOpen, setLeaderboardOpen] = useState(false);
    const [leaderboardContest, setLeaderboardContest] = useState(tournament.contests[initialTab]);

    const handleChange = (event, newValue) => {
        setValue(newValue);
        setLeaderboardContest(tournament.contests[newValue]);
    };

    const handleLeaderboardOpen = () => {
        setLeaderboardOpen(true);
    };

    const handleLeaderboardClose = () => {
        setLeaderboardOpen(false);
    };

    function getContestInfo(tournament, contest) {
        if (!tournament.isActive && !tournament.isFinished) {
            return null;
        }

        return (
            <>
                {parseContestHeading(contest)}
                {getContestState(contest) !== ContestState.NOT_STARTED &&
                <Button className="mx-auto"
                        onClick={handleLeaderboardOpen}
                        color="primary"
                        variant="contained">Leaderboard</Button>}
            </>
        );
    }

    return (
        <>
            <div className="OneForAllDashboard__Layout">
                <div className="OneForAllDashboard__ContentContainer OneForAllDashboard__Tabs">
                    <ContestsTabs tournament={tournament} handleChange={handleChange} value={value}/>
                </div>
                <div className="OneForAllDashboard__ContentContainer OneForAllDashboard__Contest">
                    {tournament.contests.map((contest, index) => {
                        return (
                            <TabPanel key={index} className={classes.tabPanel} value={value} index={index}>
                                {getContestInfo(tournament, contest, handleLeaderboardOpen)}
                                {parseContest(tournament, contest)}
                            </TabPanel>
                        );
                    })}
                </div>
                <div className="OneForAllDashboard__ContentContainer OneForAllDashboard__Standings">
                    <Paper className={classes.standingsHeading} elevation={3}>
                        <Typography variant="h4" className={classes.standingsLineHeight}>
                            Standings
                        </Typography>
                    </Paper>
                    <Standings tournament={tournament}/>
                </div>
            </div>
            <ContestLeaderboard open={leaderboardOpen} handleLeaderboardClose={handleLeaderboardClose}
                                matchups={leaderboardContest.matchups} index={value + 1}/>
        </>
    );
}