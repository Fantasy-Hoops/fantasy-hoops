import React, {useEffect, useState} from 'react';
import {getLastLocationSlug} from "../../utils/locationSlug";
import {
    acceptTournamentInvitation,
    declineTournamentInvitation,
    getTournamentDetails
} from "../../utils/networkFunctions";
import _ from 'lodash';
import TournamentListCard from "./TournamentListCard";
import FullscreenLoader from "../FullscreenLoader";
import {Helmet} from "react-helmet";
import {TournamentsInvitations} from "./utils";
import {Canonicals, Meta} from "../../utils/helpers";
import {Error} from "../Error";
import {useSnackbar} from "notistack";
import Routes from "../../routes/routes";

export function TournamentInvitation(props) {
    const tournamentId = getLastLocationSlug(props.location.pathname);
    const [tournament, setTournament] = useState({});
    const [tournamentExists, setTournamentExists] = useState(true);
    const [errorResponse, setErrorResponse] = useState({});
    const [loader, setLoader] = useState(true);
    const {enqueueSnackbar} = useSnackbar();

    useEffect(() => {
        async function handleGetTournament() {
            getTournamentDetails(tournamentId, {checkForFriends: true}).then(response => {
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
    
    function handleAcceptInvitation() {
        setLoader(true);
        acceptTournamentInvitation({tournamentId})
            .then(response => {
                window.location.replace(Routes.TOURNAMENTS);
            }).catch(error => {
                enqueueSnackbar(error.message, {variant: "error"});
                setLoader(false);
        });
    }
    
    function handleDeclineInvitation() {
        setLoader(true);
        declineTournamentInvitation({tournamentId})
            .then(response => {
                enqueueSnackbar(response.data, {variant: "success"});
                window.location.replace('/tournaments/invitations');
                setLoader(false);
            }).catch(error => {
            enqueueSnackbar(error.message, {variant: "error"});
            window.location.reload('/tournaments/invitations');
            setLoader(false);
        });
    }

    if (!tournamentExists && !_.isEmpty(errorResponse)) {
        return <Error status={errorResponse.status} message={errorResponse.message}/>;
    }

    return (
        <>
            <Helmet>
                <title>Tournament Invitations | Fantasy Hoops</title>
                <meta name="description" content={Meta.DESCRIPTION}/>
                <link rel="canonical" href={Canonicals.TOURNAMENTS_INVITATIONS}/>
            </Helmet>
            <article className="PageIntro">
                <h1 className="PageTitle">{TournamentsInvitations.SINGLE_INVITATION_TITLE}</h1>
                <h5 className="PageSubtitle">{`User ${tournament.creator ? tournament.creator.username : ''} invited you to join the tournament.`}</h5>
            </article>
            {!_.isEmpty(tournament) && (
                <TournamentListCard
                    tournament={tournament}
                    handleAcceptInvitation={handleAcceptInvitation}
                    handleDeclineInvitation={handleDeclineInvitation}
                    isInvitation
                />
            )}
            {loader && <FullscreenLoader/>}
        </>
    );
}