import React, {Component} from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import {Card} from './Card';
import leaderboardLogo from '../../../../content/icons/1021175-winning/svg/006-winner-5.svg';
import {PlayerModal} from '../../PlayerModal/PlayerModal';
import EmptyJordan from '../../EmptyJordan';
import {getSelectedPlayersLeaderboard, getPlayerStats} from '../../../utils/networkFunctions';
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../../../utils/helpers";
import {Container} from "@material-ui/core";
import InfoDialog from "../../Lineup/InfoDialog";
import PlayerDialog from "../../PlayerModal/PlayerDialog";

const {$} = window;
const LOAD_COUNT = 30;

const Intro = {
    TITLE: "Most Selected NBA Players Leaderboard | Fantasy Hoops",
    DESCRIPTION: "Leaderboard of the NBA players that Fantasy Hoops users tend to choose most often. Find out who " +
        "are the most popular selections of all time and make a decision whether you are going to follow the trends " +
        "or build a lineup of underdogs."
};

export default class Leaderboard extends Component {
    constructor(props) {
        super(props);
        this.showModal = this.showModal.bind(this);
        this.loadMore = this.loadMore.bind(this);
        this.handlePlayerDialogOpen = this.handlePlayerDialogOpen.bind(this);
        this.handlePlayerDialogClose = this.handlePlayerDialogClose.bind(this);

        this.state = {
            players: [],
            stats: '',
            modalLoader: true,
            renderChild: true,
            loader: true,
            showButton: false,
            playerDialogOpen: false
        };
    }

    async componentDidMount() {
        await getSelectedPlayersLeaderboard({limit: LOAD_COUNT})
            .then((res) => {
                this.state.showButton = res.data.length === LOAD_COUNT;
                this.setState({
                    players: res.data,
                    loader: false
                });
            });
        await this.setState({
            PG: require('../../../../content/images/positions/pg.png'),
            SG: require('../../../../content/images/positions/sg.png'),
            SF: require('../../../../content/images/positions/sf.png'),
            PF: require('../../../../content/images/positions/pf.png'),
            C: require('../../../../content/images/positions/c.png')
        });
    }

    handlePlayerDialogOpen() {
        this.setState({
            playerDialogOpen: true
        })
    }

    handlePlayerDialogClose() {
        this.setState({
            playerDialogOpen: false
        })
    }

    async showModal(player) {
        this.setState({modalLoader: true});
        this.handlePlayerDialogOpen();
        await getPlayerStats(player.nbaId)
            .then((res) => {
                this.setState({
                    stats: res.data,
                    modalLoader: false,
                    renderChild: true
                });
            });
    }

    async loadMore() {
        const {players} = this.state;
        this.setState({loadMore: true});
        await getSelectedPlayersLeaderboard({from: players.length, limit: LOAD_COUNT})
            .then((res) => {
                this.setState({
                    players: players.concat(res.data),
                    loadMore: false,
                    showButton: res.data.length === LOAD_COUNT
                });
            });
    }

    seeMoreBtn() {
        const {showButton} = this.state;
        return showButton ?
            <button type="button" className="btn btn-primary mt-2" onClick={this.loadMore}>See more</button> : '';
    }

    createPlayers(players) {
        return _.map(
            players,
            (player, index) => (
                <Card
                    index={index}
                    key={shortid()}
                    player={player}
                    showModal={this.showModal}
                    image={this.state[player.position]}
                />
            )
        );
    }

    render() {
        const {
            players, loader, loadMore, renderChild, modalLoader, stats
        } = this.state;
        const playerCards = this.createPlayers(players);
        const seeMoreBtn = loader || loadMore
            ? <div className="Loader"/>
            : this.seeMoreBtn();
        return (
            <>
                <Helmet>
                    <title>{Intro.TITLE}</title>
                    <meta property="title" content={Intro.TITLE}/>
                    <meta property="og:title" content={Intro.TITLE}/>
                    <meta name="description" content={Intro.DESCRIPTION}/>
                    <meta property="og:description" content={Intro.DESCRIPTION}/>
                    <meta name="robots" content="index,follow"/>
                    <link rel="canonical" href={Canonicals.SELECTED_PLAYERS_LEADERBOARD}/>
                </Helmet>
                <article className="PageIntro--centered">
                    <img
                        src={leaderboardLogo}
                        alt="Leaderboard Logo"
                        width="60rem"
                    />
                    <h1 className="PageTitle">Most selected NBA Players</h1>
                </article>
                <div className="tab-content" id="myTabContent">
                    <div className="pt-4 pb-1 tab-pane show active animated bounceInUp" id="daily" role="tabpanel">
                        {!loader
                            ? players.length > 0
                                ? playerCards
                                : <EmptyJordan message="Such empty..."/>
                            : ''}
                        <div className="text-center">
                            {seeMoreBtn}
                        </div>
                    </div>
                </div>
                <PlayerDialog
                    renderChild={this.state.renderChild}
                    loader={this.state.modalLoader}
                    stats={this.state.stats}
                    open={this.state.playerDialogOpen}
                    onDialogOpen={this.handlePlayerDialogOpen}
                    onDialogClose={this.handlePlayerDialogClose}
                />
            </>
        );
    }
}
