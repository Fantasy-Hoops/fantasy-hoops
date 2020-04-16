import React, {useEffect, useState} from 'react';
import {getLastLocationSlug} from "../../utils/locationSlug";
import {getTournamentInvitations} from "../../utils/networkFunctions";
import _ from 'lodash';
import TournamentListCard from "./TournamentListCard";
import FullscreenLoader from "../FullscreenLoader";
import {Helmet} from "react-helmet";
import {TournamentsInvitations, TournamentsMain} from "./utils";
import {Canonicals, Meta} from "../../utils/helpers";
import EmptyJordan from "../EmptyJordan";

export function TournamentInvitations(props) {
    const tournamentId = getLastLocationSlug(props.location.pathname);
    const [tournamentInvitations, setTournamentInvitations] = useState([]);
    const [invitationExists, setInvitationExists] = useState(false);
    const [errorResponse, setErrorResponse] = useState({});
    const [loader, setLoader] = useState(true);

    useEffect(() => {
        async function handleGetTournament() {
            getTournamentInvitations().then(response => {
                setTournamentInvitations(response.data);
                setLoader(false);
            }).catch(error => {
                setErrorResponse({
                    status: error.response.status,
                    message: error.response.data
                });
                setLoader(false);
                setInvitationExists(false);
            })
        }

        handleGetTournament();
    }, []);
    
    if (!loader && _.isEmpty(tournamentInvitations)) {
        return <EmptyJordan message="You have no tournament invitations."/>;
    }

    return (
        <>
            <Helmet>
                <title>Tournament Invitations | Fantasy Hoops</title>
                <meta name="description" content={Meta.DESCRIPTION}/>
                <link rel="canonical" href={Canonicals.TOURNAMENTS_INVITATIONS}/>
            </Helmet>
            <article className="PageIntro">
                <h1 className="PageTitle">{TournamentsInvitations.TITLE}</h1>
            </article>
            {tournamentInvitations.map((tournament, index) => (
                <TournamentListCard key={index} tournament={tournament} isInvitation clickable />
            ))}
            {loader && <FullscreenLoader/>}
        </>
    );
}