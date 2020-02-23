import React, {useEffect, useState} from 'react';

import './Leaderboards.css';
import {useStyles} from "./LeaderboarsStyle";
import {Container} from "@material-ui/core";
import {
    getPlayersLeaderboard,
    getSeasonLineupsLeaderboard,
    getSelectedPlayersLeaderboard,
    getUsersLeaderboard
} from "../../utils/networkFunctions";
import moment from "moment";
import UserCard, {Card as UserLeaderboardCard} from "./Users/Card";
import {Card as PlayerLeaderboardCard} from "./Players/Card";
import shortid from "shortid";
import _ from "lodash";
import clsx from "clsx";
import Button from "@material-ui/core/Button";
import {Link} from "react-router-dom";
import Routes from "../../routes/routes";
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../../utils/helpers";
import CustomLoader from "../Loader/CustomLoader";
import {Card} from "./Selected/Card";
import {Intro, Positions} from "./utils";
import TopLeaderboardLoader from "../Loader/TopLeaderboardLoader";

const lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus id venenatis purus." +
    "Suspendisse condimentum ex felis, at rhoncus eros scelerisque non. Curabitur sed venenatis massa, ac feugiat augue." +
    "Vivamus congue nisl felis, vitae semper mi interdum in. Sed posuere lectus consequat lorem mattis condimentum." +
    "Nam non magna tempus, cursus diam lacinia, pharetra leo. Etiam nisi justo, ullamcorper ut mi non, pellentesque" +
    "mollis mauris. In hac habitasse platea dictumst. Fusce vitae tempor erat, quis rhoncus est. Suspendisse porttitor" +
    "finibus sapien, vitae fermentum lorem tempus id.";

const halfLorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus id venenatis purus." +
    "Suspendisse condimentum ex felis, at rhoncus eros scelerisque non. Curabitur sed venenatis massa, ac feugiat augue." +
    "Vivamus congue nisl felis, vitae semper mi interdum in. Sed posuere lectus consequat lorem mattis condimentum.";



function Leaderboards(props) {
    const classes = useStyles();
    const [sectionWidth, setSectionWidth] = useState(500);
    const [weeklyUsers, setTopWeeklyUsers] = useState(null);
    const [topPlayers, setTopNBAPlayers] = useState(null);
    const [topSeasonLineups, setTopSeasonLineups] = useState(null);
    const [mostSelectedPlayers, setMostSelectedPlayers] = useState(null);

    useEffect(() => {
        async function handleGetUserLeaderboard() {
            await getUsersLeaderboard({limit: 3})
                .then(response => {
                    if (response.data.length === 0) {
                        getUsersLeaderboard({limit: 3, weekNumber: moment().subtract(1, 'weeks').week()})
                            .then(response => setTopWeeklyUsers(response.data))
                            .catch(err => console.error(err.message))
                    } else {
                        setTopWeeklyUsers(response.data);
                    }
                })
                .catch(err => console.error(err.message))
        }

        async function handleGetPlayersLeaderboard() {
            await getPlayersLeaderboard({limit: 3, type: 'daily'})
                .then(response => setTopNBAPlayers(response.data))
                .catch(err => console.error(err.message))
        }

        async function handleGetTopSeasonPerformers() {
            await getSeasonLineupsLeaderboard({limit: 3})
                .then(response => setTopSeasonLineups(response.data))
                .catch(err => console.error(err.message))
        }

        async function handleGetMostSelectedPlayers() {
            await getSelectedPlayersLeaderboard({limit: 3})
                .then(response => setMostSelectedPlayers(response.data))
                .catch(err => console.error(err.message))
        }

        handleGetUserLeaderboard();
        handleGetPlayersLeaderboard();
        handleGetTopSeasonPerformers();
        handleGetMostSelectedPlayers();

        setSectionWidth(document.getElementsByClassName("Content--right")[0].clientWidth);
    }, []);

    const weeklyUsersCards = _.map(
        weeklyUsers,
        (user, index) => (
            <UserLeaderboardCard
                className="no-left-margin no-pointer-events"
                index={index}
                key={shortid()}
                user={user}
            />
        )
    );

    const topPlayersCards = _.map(
        topPlayers,
        (player, index) => (
            <PlayerLeaderboardCard
                className="no-left-margin no-pointer-events"
                index={index}
                key={shortid()}
                player={player}
            />
        )
    );

    const seasonLineupsCards = _.map(
        topSeasonLineups,
        (user, index) => (
            <UserCard
                className="no-pointer-events"
                index={index}
                key={shortid()}
                user={user}
            />
        )
    );

    const mostSelectedPlayersCards = _.map(
        mostSelectedPlayers,
        (player, index) => (
            <Card
                className="no-pointer-events"
                index={index}
                key={shortid()}
                player={player}
                image={Positions[player.position]}
            />
        )
    );

    return (
        <>
            <Helmet>
                <title>Leaderboards | Fantasy Hoops</title>
                <meta name="description" content={Intro.SUBTITLE}/>
                <link rel="canonical" href={Canonicals.LEADERBOARDS}/>
            </Helmet>
            <Container maxWidth="md">
                <article className="Leaderboards__Intro">
                    <h1 className="Leaderboards__Title">{Intro.TITLE}</h1>
                    <h5 className="Leaderboards__Subtitle">{Intro.SUBTITLE}</h5>
                </article>
                <section className="Content__Container">
                    <article className="Content--left">
                        <h2 className="Content__Title">{Intro.USERS_TITLE}</h2>
                        <p className="Content__Subtitle">{Intro.USERS_SUBTITLE}</p>
                        <Button
                            className={clsx(classes.button, classes.buttonLeft)}
                            variant="contained"
                            color="secondary"
                            component={Link}
                            to={Routes.LEADERBOARD_USERS}
                        >
                            {Intro.USERS_BUTTON_TITLE}
                        </Button>
                    </article>
                    <article className="Content--right">
                        {weeklyUsers
                            ? weeklyUsersCards
                            : <CustomLoader maxWidth={sectionWidth - 25}>
                                <TopLeaderboardLoader maxWidth={sectionWidth - 25} />
                            </CustomLoader>}
                    </article>
                </section>
                <section className="Content__Container Content__Container--reverse">
                    <article className="Content--left">
                        {topPlayers
                            ? topPlayersCards
                            : <CustomLoader maxWidth={sectionWidth - 25}>
                                <TopLeaderboardLoader maxWidth={sectionWidth - 25} />
                            </CustomLoader>}
                    </article>
                    <article className="Content--right">
                        <h2 className="Content__Title">{Intro.PLAYERS_TITLE}</h2>
                        <p className="Content__Subtitle">{Intro.PLAYERS_SUBTITLE}</p>
                        <Button
                            className={`${classes.button} ${classes.buttonRight}`}
                            variant="contained"
                            color="secondary"
                            component={Link}
                            to={Routes.LEADERBOARD_PLAYERS}
                        >
                            {Intro.PLAYERS_BUTTON_TITLE}
                        </Button>
                    </article>
                </section>
                <section className="Content__Container">
                    <article className="Content--left">
                        <h2 className="Content__Title">{Intro.SEASON_TITLE}</h2>
                        <p className="Content__Subtitle">{Intro.SEASON_SUBTITLE}</p>
                        <Button
                            className={`${classes.button} ${classes.buttonRight}`}
                            variant="contained"
                            color="secondary"
                            component={Link}
                            to={Routes.LEADERBOARD_SEASON}
                        >
                            {Intro.SEASON_BUTTON_TITLE}
                        </Button>
                    </article>
                    <article className="Content--right">
                        {topSeasonLineups
                            ? seasonLineupsCards
                            : <CustomLoader maxWidth={sectionWidth - 25}>
                                <TopLeaderboardLoader maxWidth={sectionWidth - 25} />
                            </CustomLoader>}
                    </article>
                </section>
                <section className="Content__Container Content__Container--reverse">
                    <article className="Content--left">
                        {mostSelectedPlayers
                            ? mostSelectedPlayersCards
                            : <CustomLoader maxWidth={sectionWidth - 25}>
                                <TopLeaderboardLoader maxWidth={sectionWidth - 25} />
                            </CustomLoader>}
                    </article>
                    <article className="Content--right">
                        <h2 className="Content__Title">{Intro.SELECTED_TITLE}</h2>
                        <p className="Content__Subtitle">{Intro.SELECTED_SUBTITLE}</p>
                        <Button
                            className={`${classes.button} ${classes.buttonRight}`}
                            variant="contained"
                            color="secondary"
                            component={Link}
                            to={Routes.LEADERBOARD_SELECTED}
                        >
                            {Intro.SELECTED_BUTTON_TITLE}
                        </Button>
                    </article>
                </section>
                {/*<section className="Content__Container">*/}
                {/*    <article className="Content--left">*/}
                {/*        <h2 className="Content__Title">BEST LINEUPS</h2>*/}
                {/*        <p>{halfLorem}</p>*/}
                {/*        <Button*/}
                {/*            className={`${classes.button} ${classes.buttonRight}`}*/}
                {/*            variant="contained"*/}
                {/*            color="secondary"*/}
                {/*            component={Link}*/}
                {/*            to={Routes.LEADERBOARD_BEST_LINEUP}*/}
                {/*        >*/}
                {/*            BEST LINEUPS LEADERBOARD*/}
                {/*        </Button>*/}
                {/*    </article>*/}
                {/*    <article className="Content--right">*/}
                {/*        <CustomLoader maxWidth={sectionWidth - 25}/>*/}
                {/*    </article>*/}
                {/*</section>*/}
            </Container>
        </>
    );
}

export default Leaderboards;