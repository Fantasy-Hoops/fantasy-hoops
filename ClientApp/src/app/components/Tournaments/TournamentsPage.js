import React, {useEffect, useState} from "react";
import {Helmet} from "react-helmet";
import {TournamentsMain} from "./utils";
import {Canonicals} from "../../utils/helpers";
import _ from 'lodash';

import './TournamentsPage.css';
import TournamentListCard from "./TournamentListCard";
import {getUserTournaments} from "../../utils/networkFunctions";
import {useSnackbar} from "notistack";
import {parse} from "../../utils/auth";

/**
 * @return {null}
 */
export default function TournamentsPage() {
    const user = parse();
    const {enqueueSnackbar} = useSnackbar();
    const [userTournaments, setUserTournaments] = useState({
        created: [],
        joined: []
    });

    useEffect(() => {
        async function handleGetUserTournaments() {
            getUserTournaments(user.id).then(response => {
                setUserTournaments(response.data);
            }).catch(error => enqueueSnackbar(error.message, {variant: 'error'}));
        }

        handleGetUserTournaments();
    }, []);


    if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
        return null;
    }

    return (
        <>
            <Helmet>
                <title>Tournaments | Fantasy Hoops</title>
                <meta name="description" content={TournamentsMain.SUBTITLE}/>
                <link rel="canonical" href={Canonicals.TOURNAMENTS}/>
            </Helmet>
            <article className="Tournaments__Intro">
                <h1 className="Tournaments__Title">{TournamentsMain.TITLE}</h1>
                <h5 className="Tournaments__Subtitle">{TournamentsMain.SUBTITLE}</h5>
            </article>
            {!_.isEmpty(userTournaments.created) && (
                <div className="Tournaments__Created">
                    <h2 className="TournamentsGroup__Title">{TournamentsMain.CREATED_TOURNAMENTS_TITLE}</h2>
                    {
                        _.map(userTournaments.created, (tournament, index) => (
                            <TournamentListCard key={index} tournament={tournament}/>
                        ))
                    }
                </div>

            )}
            {!_.isEmpty(userTournaments.joined) && (
                <div className="Tournaments__Joined">
                    <h2 className="TournamentsGroup__Title">{TournamentsMain.CREATED_TOURNAMENTS_TITLE}</h2>
                    {
                        _.map(userTournaments.joined, (tournament, index) => (
                            <TournamentListCard key={index} tournament={tournament}/>
                        ))
                    }
                </div>
            )}
        </>
    );
}