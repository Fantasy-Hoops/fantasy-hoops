import React, {useEffect, useState} from 'react';

import './Leaderboards.css';
import {useStyles} from "./LeaderboarsStyle";
import {
    getBestLineups,
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
import {UserScore} from "../Profile/UserScore";

const googleAd = (
    <>
        <script async src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js"></script>
        <ins className="adsbygoogle"
             style={{display: "block"}}
             data-ad-client="ca-pub-6391166063453559"
             data-ad-slot="3271809564"
             data-ad-format="auto"
             data-full-width-responsive="true"></ins>
        <script>
            (adsbygoogle = window.adsbygoogle || []).push({});
        </script>
    </>
);

function Leaderboards(props) {
    const classes = useStyles();
    const [sectionWidth, setSectionWidth] = useState(500);
    const [weeklyUsers, setTopWeeklyUsers] = useState(null);
    const [topPlayers, setTopNBAPlayers] = useState(null);
    const [topSeasonLineups, setTopSeasonLineups] = useState(null);
    const [mostSelectedPlayers, setMostSelectedPlayers] = useState(null);
    const [bestLineups, setBestLineups] = useState(null);

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

        async function handleGetBestLineups() {
            await getBestLineups({limit: 3})
                .then(response => setBestLineups(response.data))
                .catch(err => console.error(err.message));
        }

        handleGetUserLeaderboard();
        handleGetPlayersLeaderboard();
        handleGetTopSeasonPerformers();
        handleGetMostSelectedPlayers();
        handleGetBestLineups();

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

    const bestLineupsCards = _.map(
        bestLineups,
        lineup => (
            <UserScore
                className="no-pointer-events"
                key={shortid()}
                activity={lineup}
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
                <meta property="title" content="Leaderboards | Fantasy Hoops"/>
                <meta property="og:title" content="Leaderboards | Fantasy Hoops"/>
                <meta name="description" content={Intro.SUBTITLE}/>
                <meta property="og:description" content={Intro.SUBTITLE}/>
                <meta name="robots" content="index,follow"/>
                <link rel="canonical" href={Canonicals.LEADERBOARDS}/>
            </Helmet>
            <article className="PageIntro">
                <h1 className="PageTitle">{Intro.TITLE}</h1>
                <h5 className="PageSubtitle">{Intro.SUBTITLE}</h5>
            </article>
            {googleAd}
            <section id="Leaderboards__activeUsers" className="Content__Container">
                <article className="Content--left">
                    <h2 className="Content__Title">{Intro.USERS_TITLE}</h2>
                    <p className="Content__Subtitle">{Intro.USERS_SUBTITLE}</p>
                    <Link to={Routes.LEADERBOARD_USERS}>
                        <Button
                            className={clsx(classes.button, classes.buttonLeft)}
                            variant="contained"
                            color="secondary"
                        >
                            {Intro.USERS_BUTTON_TITLE}
                        </Button>
                    </Link>
                </article>
                <article className="Content--right">
                    {weeklyUsers
                        ? weeklyUsersCards
                        : <CustomLoader maxWidth={sectionWidth - 25}>
                            <TopLeaderboardLoader maxWidth={sectionWidth - 25}/>
                        </CustomLoader>}
                </article>
            </section>
            <section id="Leaderboards__NBAPlayers" className="Content__Container Content__Container--reverse">
                <article className="Content--left">
                    {topPlayers
                        ? topPlayersCards
                        : <CustomLoader maxWidth={sectionWidth - 25}>
                            <TopLeaderboardLoader maxWidth={sectionWidth - 25}/>
                        </CustomLoader>}
                </article>
                <article className="Content--right">
                    <h2 className="Content__Title">{Intro.PLAYERS_TITLE}</h2>
                    <p className="Content__Subtitle">{Intro.PLAYERS_SUBTITLE}</p>
                    <Link
                        to={Routes.LEADERBOARD_PLAYERS}>
                        <Button
                            className={`${classes.button} ${classes.buttonRight}`}
                            variant="contained"
                            color="secondary"
                        >
                            {Intro.PLAYERS_BUTTON_TITLE}
                        </Button>
                    </Link>
                </article>
            </section>
            <section id="Leaderboards__bestLineups" className="Content__Container">
                <article className="Content--left">
                    <h2 className="Content__Title">{Intro.BEST_LINEUPS_TITLE}</h2>
                    <p className="Content__Subtitle">{Intro.BEST_LINEUPS_SUBTITLE}</p>
                    <Link
                        to={Routes.LEADERBOARD_BEST_LINEUPS}>
                        <Button
                            className={`${classes.button} ${classes.buttonLeft}`}
                            variant="contained"
                            color="secondary"
                        >
                            {Intro.BEST_LINEUPS_BUTTON_TITLE}
                        </Button>
                    </Link>
                </article>
                <article className="Content--right">
                    {bestLineups
                        ? bestLineupsCards
                        : <CustomLoader maxWidth={sectionWidth - 25}>
                            <TopLeaderboardLoader maxWidth={sectionWidth - 25}/>
                        </CustomLoader>}
                </article>
            </section>
            <section id="Leaderboards__seasonPerformers" className="Content__Container Content__Container--reverse">
                <article className="Content--left">
                    {topSeasonLineups
                        ? seasonLineupsCards
                        : <CustomLoader maxWidth={sectionWidth - 25}>
                            <TopLeaderboardLoader maxWidth={sectionWidth - 25}/>
                        </CustomLoader>}
                </article>
                <article className="Content--right">
                    <h2 className="Content__Title">{Intro.SEASON_TITLE}</h2>
                    <p className="Content__Subtitle">{Intro.SEASON_SUBTITLE}</p>
                    <Link to={Routes.LEADERBOARD_SEASON}>
                        <Button
                            className={`${classes.button} ${classes.buttonRight}`}
                            variant="contained"
                            color="secondary"
                        >
                            {Intro.SEASON_BUTTON_TITLE}
                        </Button>
                    </Link>
                </article>
            </section>
            <section id="Leaderboards__NBAPlayersSelections" className="Content__Container">
                <article className="Content--left">
                    <h2 className="Content__Title">{Intro.SELECTED_TITLE}</h2>
                    <p className="Content__Subtitle">{Intro.SELECTED_SUBTITLE}</p>
                    <Link to={Routes.LEADERBOARD_SELECTED}>
                        <Button
                            className={`${classes.button} ${classes.buttonLeft}`}
                            variant="contained"
                            color="secondary"
                        >
                            {Intro.SELECTED_BUTTON_TITLE}
                        </Button>
                    </Link>
                </article>
                <article className="Content--right">
                    {mostSelectedPlayers
                        ? mostSelectedPlayersCards
                        : <CustomLoader maxWidth={sectionWidth - 25}>
                            <TopLeaderboardLoader maxWidth={sectionWidth - 25}/>
                        </CustomLoader>}
                </article>
            </section>
        </>
    );
}

export default Leaderboards;