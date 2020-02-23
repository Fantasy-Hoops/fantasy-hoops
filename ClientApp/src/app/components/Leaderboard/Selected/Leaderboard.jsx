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

const {$} = window;
const LOAD_COUNT = 30;

export default class Leaderboard extends Component {
    constructor(props) {
        super(props);
        this.showModal = this.showModal.bind(this);
        this.loadMore = this.loadMore.bind(this);

        this.state = {
            players: [],
            stats: '',
            modalLoader: true,
            renderChild: true,
            loader: true,
            showButton: false
        };
    }

    async componentDidMount() {
        $('#playerModal').on('hidden.bs.modal', () => {
            this.setState({
                modalLoader: true,
                renderChild: false
            });
        });

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

    async showModal(player) {
        this.setState({modalLoader: true});
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
                    <title>Most Selected NBA Players Leaderboard | Fantasy Hoops</title>
                    <meta name="description" content={Meta.DESCRIPTION}/>
                    <link rel="canonical" href={Canonicals.SELECTED_PLAYERS_LEADERBOARD}/>
                </Helmet>
                <Container maxWidth="md">
                    <div className="text-center">
                        <img
                            src={leaderboardLogo}
                            alt="Leaderboard Logo"
                            width="60rem"
                        />
                        <h1>Most selected NBA Players</h1>
                    </div>
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
                    <PlayerModal
                        renderChild={renderChild}
                        loader={modalLoader}
                        stats={stats}
                    />
                </Container>
            </>
        );
    }
}
