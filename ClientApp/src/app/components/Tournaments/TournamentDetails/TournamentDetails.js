import React, {useEffect, useState} from 'react';
import {getLastLocationSlug} from "../../../utils/locationSlug";
import {getTournamentDetails} from "../../../utils/networkFunctions";
import FullscreenLoader from "../../FullscreenLoader";
import {Error} from "../../Error";
import _ from 'lodash';
import {Helmet} from "react-helmet";
import {Canonicals} from "../../../utils/helpers";
import {TournamentsMain} from "../utils";
import OneForAllDashboard from "./OneForAllDashboard";
import MatchupsDashboard from "./Matchups/MatchupsDashboard";
import IconButton from "@material-ui/core/IconButton";
import SettingsIcon from '@material-ui/icons/Settings';

import './TournamentDetails.css';
import {TournamentSettings} from "./TournamentSettings";

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
        return <Error message="You haven't accepted invite to this tournament."/>;
    }

    function renderDashboard() {
        switch (tournament.type) {
            case 0:
                return <OneForAllDashboard tournament={tournament}/>;
            case 1:
                return <MatchupsDashboard tournament={tournament}/>;
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
                <h1 className="PageTitle">
                    {tournament.title}
                    {' '}
                    {tournament.isCreator
                        ? (
                            <IconButton onClick={handleSettingsOpen}>
                                <SettingsIcon/>
                            </IconButton>
                        )
                        : null}
                </h1>
                <h5 className="PageSubtitle">{tournament.description}</h5>
            </article>
            {renderDashboard()}
            {tournament.isCreator
            && <TournamentSettings tournamentId={tournamentId} tournament={tournament} isSettingsOpen={isSettingsOpen}
                                   handleSettingsClose={handleSettingsClose}/>}
        </>
    );
}