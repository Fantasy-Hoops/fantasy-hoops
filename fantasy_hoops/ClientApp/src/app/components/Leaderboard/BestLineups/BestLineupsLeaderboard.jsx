import React, {useEffect, useState} from "react";
import {
    getBestLineups,
    getPlayerStats,
    getUserFriendsOnlyLeaderboard,
    getUsersLeaderboard
} from "../../../utils/networkFunctions";
import _ from "lodash";
import {UserScore} from "../../Profile/UserScore";
import CustomDatePicker from "../../Inputs/DatePicker/CustomDatePicker";
import {DatePickerTypes} from "../../Inputs/DatePicker/utils";
import {datePickerStyles} from "../Users/LeaderboardStyle";
import moment, {max} from "moment";
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../../../utils/helpers";
import {Intro} from "../utils";
import leaderboardLogo from "../../../../content/icons/1021175-winning/svg/006-winner-5.svg";
import {useSnackbar} from "notistack";
import EmptyJordan from "../../EmptyJordan";
import FullscreenLoader from "../../FullscreenLoader";
import PlayerDialog from "../../PlayerModal/PlayerDialog";

const LEADERBOARD_SUPPORT_START_DATE = '2020-02-22';

export function BestLineupsLeaderboard(props) {
    const {enqueueSnackbar} = useSnackbar();
    const minDate = new Date(LEADERBOARD_SUPPORT_START_DATE);
    const [maxDate, setMaxDate] = useState(null);

    const [bestLineups, setBestLineups] = useState(null);
    const [date, setDate] = useState(maxDate);
    const [loader, setLoader] = useState(true);
    const [modalLoader, setModalLoader] = useState(true);
    const [playerDialogOpen, setPlayerDialogOpen] = useState(false);
    const [stats, setStats] = useState('');
    const [renderChild, setRenderChild] = useState(false);
    
    useEffect(() => {
        async function handleGetBestLineups() {
            await getBestLineups()
                .then(response => {
                    if (response.data && response.data[0]) {
                        setBestLineups(response.data);
                        setDate(response.data[0].date);
                        setMaxDate(new Date(response.data[0].date));
                    }
                    setLoader(false);
                })
                .catch(error => {
                    enqueueSnackbar(error.message, {variant: "error"});
                    setLoader(false);
                });
        };

        handleGetBestLineups();
    }, []);

    function handlePlayerDialogOpen() {
        setPlayerDialogOpen(true);
    }

    function handlePlayerDialogClose() {
        setPlayerDialogOpen(false);
    }

    async function onDateChange(date) {
        setLoader(true);
        setDate(date);
        await getBestLineups({date: moment(date).format('YYYYMMDD')})
            .then(response => {
                setBestLineups(response.data);
                setLoader(false);
            })
            .catch(error => {
                enqueueSnackbar(error.message, {variant: "error"});
                setLoader(false);
            });
    }

    const parseBestLineupsCards = (bestLineups) => bestLineups.map(
        (lineup, index) => (
            <UserScore
                key={index}
                activity={lineup}
                showModal={showModal}
            />
        )
    );

    async function showModal(nbaID) {
        handlePlayerDialogOpen();
        await getPlayerStats(nbaID)
            .then((res) => {
                setStats(res.data);
                setModalLoader(false);
                setRenderChild(true);
            });
    }

    return (
        <>
            <Helmet>
                <title>Best Lineups | Fantasy Hoops</title>
                <meta property="title" content="Best Lineups | Fantasy Hoops"/>
                <meta property="og:title" content="Best Lineups | Fantasy Hoops"/>
                <meta name="description" content={Meta.DESCRIPTION}/>
                <meta property="og:description" content={Meta.DESCRIPTION}/>
                <meta name="robots" content="index,follow"/>
                <link rel="canonical" href={Canonicals.BEST_LINEUPS_LEADERBOARD}/>
            </Helmet>
            <article className="PageIntro--centered">
                <img
                    src={leaderboardLogo}
                    alt="Leaderboard Logo"
                    width="60rem"
                />
                <h1 className="PageTitle">{Intro.BEST_LINEUPS_TITLE}</h1>
            </article>
            <div className="DatePicker">
                {bestLineups && <CustomDatePicker
                    autoOk
                    type={DatePickerTypes.DAY}
                    label={"Select date"}
                    styles={datePickerStyles}
                    selectedDate={date}
                    minDate={minDate}
                    maxDate={maxDate || new Date()}
                    onDateChange={onDateChange}
                />}
            </div>
            {!_.isEmpty(bestLineups) && !loader
                ? parseBestLineupsCards(bestLineups)
                : <EmptyJordan message="No best lineups for selected date..."/>}
            <PlayerDialog
                renderChild={renderChild}
                loader={modalLoader}
                stats={stats}
                open={playerDialogOpen}
                onDialogOpen={handlePlayerDialogOpen}
                onDialogClose={handlePlayerDialogClose}
            />
            {loader && <FullscreenLoader/>}
        </>
    );
}