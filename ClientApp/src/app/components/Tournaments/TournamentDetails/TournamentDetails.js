import React, {useEffect, useState} from 'react';
import {getLastLocationSlug} from "../../../utils/locationSlug";
import {getTournamentDetails} from "../../../utils/networkFunctions";
import FullscreenLoader from "../../FullscreenLoader";
import {Error} from "../../Error";
import _ from 'lodash';
import {Helmet} from "react-helmet";
import {Canonicals, TournamentStatus} from "../../../utils/helpers";
import {TournamentsMain} from "../utils";
import OneForAllDashboard from "./OneForAll/OneForAllDashboard";
import MatchupsDashboard from "./Matchups/MatchupsDashboard";
import IconButton from "@material-ui/core/IconButton";
import SettingsIcon from '@material-ui/icons/Settings';
import {TournamentSettings} from "./TournamentSettings";
import Alert from "@material-ui/lab/Alert";
import {parse} from "../../../utils/auth";

import './TournamentDetails.css';

const user = parse();

export function TournamentDetails(props) {
    const tournamentId = getLastLocationSlug(props.location.pathname);
    const [loader, setLoader] = useState(true);
    const [tournamentExists, setTournamentExists] = useState(false);
    const [tournament, setTournament] = useState({});
    const [errorResponse, setErrorResponse] = useState({});
    const [isSettingsOpen, setIsSettingsOpen] = React.useState(false);

    const handleSettingsOpen = () => {
        setIsSettingsOpen(true);
    };
    const handleSettingsClose = () => {
        setIsSettingsOpen(false);
    };

    useEffect(() => {
        async function handleGetTournament() {
            getTournamentDetails(tournamentId).then(response => {
                setTournament(response.data);
                setLoader(false);
            }).catch(error => {
                setErrorResponse({
                    status: error.response.status,
                    message: error.response.data
                });
                setLoader(false);
                setTournamentExists(false);
            })
        }

        handleGetTournament();
    }, []);

    if (loader) {
        return <FullscreenLoader/>;
    }

    if (!tournamentExists && !_.isEmpty(errorResponse)) {
        return <Error status={errorResponse.status} message={errorResponse.message}/>;
    }

    if (!tournament.isCreator && !tournament.acceptedInvite) {
        return <Error status={404} message="You haven't accepted invite to this tournament."/>;
    }

    function renderDashboard() {
        if (tournament.status === TournamentStatus.CANCELLED && tournament.creator.userId !== user.id) {
            return <Alert severity="error">Tournament '{tournament.title}' has been cancelled!</Alert>;
        }
        
        const cancelledAlert = tournament.status === TournamentStatus.CANCELLED &&
                <Alert severity="error">Tournament is cancelled!</Alert>;

        switch (tournament.type) {
            case 0:
                return (
                    <>
                        {cancelledAlert}
                        <OneForAllDashboard tournament={tournament}/>
                    </>
                );
            case 1:
                return (
                    <>
                        {cancelledAlert}
                        <MatchupsDashboard tournament={tournament}/>
                    </>
                );
            default:
                return null;
        }
    }

    return (
        <>
            <Helmet>
                <title>{`${tournament.title} | Fantasy Hoops`}</title>
                <meta name="description" content={TournamentsMain.SUBTITLE}/>
                <link rel="canonical" href={Canonicals.TOURNAMENTS_SUMMARY}/>
            </Helmet>
            <article className="PageIntro">
                <div className="TournamentDetails__TitleWrapper">
                    <h1 className="TournamentDetails__Title PageTitle">
                        {tournament.title}
                    </h1>
                    {tournament.isCreator
                        ? (
                            <IconButton className="TournamentDetails__SettingsButton" onClick={handleSettingsOpen}>
                                <SettingsIcon/>
                            </IconButton>
                        )
                        : null}
                </div>
                <h5 className="PageSubtitle">{tournament.description}</h5>
            </article>
            {renderDashboard()}
            {tournament.isCreator
            && <TournamentSettings tournamentId={tournamentId} tournament={tournament} isSettingsOpen={isSettingsOpen}
                                   handleSettingsClose={handleSettingsClose}/>}
        </>
    );
}