import React, {useEffect, useState} from 'react';
import {getLastLocationSlug} from "../../../utils/locationSlug";
import {getTournamentById} from "../../../utils/networkFunctions";
import FullscreenLoader from "../../FullscreenLoader";
import {Error} from "../../Error";
import _ from 'lodash';
import {Helmet} from "react-helmet";
import {Canonicals} from "../../../utils/helpers";
import {TournamentsMain} from "../utils";

import './TournamentDetails.css';

export function TournamentDetails(props) {
    const tournamentId = getLastLocationSlug(props.location.pathname);
    const [loader, setLoader] = useState(true);
    const [tournamentExists, setTournamentExists] = useState(false);
    const [tournament, setTournament] = useState({});
    const [errorResponse, setErrorResponse] = useState({});
    
    useEffect(() => {
        async function handleGetTournament() {
            
            getTournamentById(tournamentId).then(response => {
                setTournament(response.data);
                setLoader(false);
            }).catch(error => {
                setErrorResponse({
                    status: error.response.status,
                    message: error.response.data
                })
                setLoader(false);
                setTournamentExists(false);
            })
        }
        
        handleGetTournament();
    }, []);

    if (loader) {
        return <FullscreenLoader />;
    }
    
    if (!tournamentExists && !_.isEmpty(errorResponse)) {
        return <Error status={errorResponse.status} message={errorResponse.message} />;
    }
    
    return (
        <>
            <Helmet>
                <title>{`${tournament.name} | Fantasy Hoops`}</title>
                <meta name="description" content={TournamentsMain.SUBTITLE}/>
                <link rel="canonical" href={Canonicals.TOURNAMENTS_SUMMARY}/>
            </Helmet>
            <article className="TournamentDetails__Intro">
                <h1 className="TournamentDetails__Title">{tournament.name}</h1>
            </article>
        </>
    );
}