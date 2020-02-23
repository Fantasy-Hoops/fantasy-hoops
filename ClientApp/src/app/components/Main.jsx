import React, {Component, useEffect, useState} from 'react';
import {Link} from 'react-router-dom';
import shortid from 'shortid';
import _ from 'lodash';
import $ from 'jquery';
import Routes from '../routes/routes';
import {Card} from './Leaderboard/Players/Card';
import {getPlayersLeaderboard} from '../utils/networkFunctions';
import {registerPush} from '../utils/push';

import './Main.css';
import Button from "@material-ui/core/Button";
import {useStyles} from "./MainStyle";
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../utils/helpers";

const positions = {
    PG: require('../../content/images/positions/pg.png'),
    SG: require('../../content/images/positions/sg.png'),
    SF: require('../../content/images/positions/sf.png'),
    PF: require('../../content/images/positions/pf.png'),
    C: require('../../content/images/positions/c.png')
};

function Main() {
    const [daily, setDaily] = useState([]);
    const classes = useStyles();

    useEffect(() => {
        if (typeof Notification !== 'undefined' && Notification.permission !== 'denied') {
            registerPush();
        }
        // Redundant after navbar
        $('#PlayNowBtn').on('click', () => {
            $('.navbar-collapse').removeClass('show');
        });
        document.querySelector('body').classList.add('Main__Background');

        async function handleGetPlayersLeaderboard() {
            await getPlayersLeaderboard({type: 'daily', limit: 3})
                .then(res => setDaily(res.data))
                .catch(err => console.error(err.message));
        }

        handleGetPlayersLeaderboard();

        return () => {
            document.querySelector('body').classList.remove('Main__Background');
        }
    }, []);

    useEffect(() => {
        let deferredPrompt;
        if (document.querySelector('.A2HS-Button')) {
            window.addEventListener('beforeinstallprompt', (e) => {
                const addBtn = document.querySelector('.A2HS-Button');
                if (!addBtn) {
                    return;
                }
                addBtn.style.display = 'none';
                // Prevent Chrome 67 and earlier from automatically showing the prompt
                e.preventDefault();
                // Stash the event so it can be triggered later.
                deferredPrompt = e;
                // Update UI to notify the user they can add to home screen
                addBtn.style.display = 'block';

                addBtn.addEventListener('click', () => {
                    // hide our user interface that shows our A2HS button
                    addBtn.style.display = 'none';
                    // Show the prompt
                    deferredPrompt.prompt();
                    // Wait for the user to respond to the prompt
                    deferredPrompt.userChoice.then((choiceResult) => {
                        if (choiceResult.outcome === 'accepted') {
                            console.log('User accepted the A2HS prompt');
                        } else {
                            console.log('User dismissed the A2HS prompt');
                        }
                        deferredPrompt = null;
                    });
                });
            });
        }
    });

    function createPlayers(players) {
        return _.map(
            players,
            (player, index) => (
                <Card
                    className="Main__TopPlayersCard"
                    index={index}
                    key={shortid()}
                    player={player}
                    image={positions[player.position]}
                />
            )
        );
    }

    const topPlayers = daily && daily.length > 0
        ? (
            <div className="Main__TopPlayers">
                <h2 className="Main__TopPlayersHeading">Top NBA Players Today</h2>
                {createPlayers(daily)}
            </div>
        )
        : null;

    return (
        <>
            <Helmet>
                <title>{Meta.TITLE}</title>
                <meta name="description" content={Meta.DESCRIPTION}/>
                <link rel="canonical" href={Canonicals.MAIN}/>
            </Helmet>
            <div className="Main__Background">
                {/*<button type="button" className="btn btn-outline-success A2HS-Button">*/}
                {/*  <i className="far fa-bookmark" />*/}
                {/*  {' Save'}*/}
                {/*</button>*/}
                <div className="Main__Logo--placeholder">
                    <img
                        className="Main__Logo"
                        alt="Fantasy Hoops"
                        src={`${require('../../content/logo/fh.svg')}`}
                    />
                </div>
                <div className="Main__PlayNowButton">
                    <Button
                        id="PlayNowBtn"
                        className={classes.button}
                        color="primary"
                        component={Link}
                        to={Routes.LINEUP}
                    >
                        Play Now!
                    </Button>
                </div>
                {topPlayers}
            </div>
        </>
    );
}

export default Main;
