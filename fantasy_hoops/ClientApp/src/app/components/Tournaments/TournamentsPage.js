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
import Fab from "@material-ui/core/Fab";
import AddIcon from '@material-ui/icons/Add';
import {useStyles} from "./TournamentsPageStyle";
import Link from "react-router-dom/Link";
import Routes from "../../routes/routes";
import FullscreenLoader from "../FullscreenLoader";
import Button from "@material-ui/core/Button";

/**
 * @return {null}
 */
export default function TournamentsPage() {
    const user = parse();
    const classes = useStyles();
    const {enqueueSnackbar} = useSnackbar();
    const [userTournaments, setUserTournaments] = useState({
        created: [],
        joined: []
    });
    const [loader, setLoader] = useState(false);

    useEffect(() => {
        async function handleGetUserTournaments() {
            setLoader(true);
            getUserTournaments(user.id).then(response => {
                setUserTournaments(response.data);
                setLoader(false);
            }).catch(error => {
                enqueueSnackbar(error.message, {variant: 'error'});
                setLoader(false);
            });
        }

        handleGetUserTournaments();
    }, []);

    return (
        <>
            <Helmet>
                <title>Tournaments | Fantasy Hoops</title>
                <meta name="description" content={TournamentsMain.SUBTITLE}/>
                <link rel="canonical" href={Canonicals.TOURNAMENTS}/>
            </Helmet>
            <article className="PageIntro">
                <div className="Tournaments__TitleWrapper">
                    <h1 className="PageTitle">{TournamentsMain.TITLE}</h1>
                    <Link to={Routes.TOURNAMENTS_CREATE}>
                        <Fab className={classes.fab} variant="extended" color="primary">
                            <AddIcon className={classes.extendedIcon} />
                            Create New
                        </Fab>
                    </Link>
                </div>
                <Link to={Routes.TOURNAMENT_INVITATIONS}>
                    <Button>
                        Invitations
                    </Button>
                </Link>
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
                    <h2 className="TournamentsGroup__Title">{TournamentsMain.JOINED_TOURNAMENTS_TITLE}</h2>
                    {
                        _.map(userTournaments.joined, (tournament, index) => (
                            <TournamentListCard key={index} tournament={tournament}/>
                        ))
                    }
                </div>
            )}
            {loader && <FullscreenLoader />}
        </>
    );
}