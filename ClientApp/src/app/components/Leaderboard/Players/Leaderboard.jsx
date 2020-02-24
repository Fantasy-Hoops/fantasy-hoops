import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import shortid from 'shortid';
import _ from 'lodash';
import {Card} from './Card';
import leaderboardLogo from '../../../../content/icons/1021175-winning/svg/006-winner-5.svg';
import {PlayerModal} from '../../PlayerModal/PlayerModal';
import EmptyJordan from '../../EmptyJordan';
import {getPlayersLeaderboard, getPlayerStats} from '../../../utils/networkFunctions';
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../../../utils/helpers";
import {Container} from "@material-ui/core";

const {$} = window;
const LOAD_COUNT = 30;

export default class Leaderboard extends Component {
    constructor(props) {
        super(props);
        this.showModal = this.showModal.bind(this);
        this.switchTab = this.switchTab.bind(this);
        this.loadMore = this.loadMore.bind(this);

        this.state = {
            activeTab: 'daily',
            daily: [],
            weekly: [],
            monthly: [],
            stats: '',
            modalLoader: true,
            renderChild: true,
            loader: true,
            showButton: {
                daily: false,
                weekly: false,
                monthly: false
            }
        };
    }

    async componentDidMount() {
        $('#playerModal').on('hidden.bs.modal', () => {
            this.setState({
                modalLoader: true,
                renderChild: false
            });
        });

        await getPlayersLeaderboard({type: 'daily'})
            .then((res) => {
                this.state.showButton.daily = res.data.length === LOAD_COUNT;
                this.setState({
                    daily: res.data,
                    activeTab: 'daily',
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

    async showModal(nbaID) {
        this.setState({modalLoader: true});
        await getPlayerStats(nbaID)
            .then((res) => {
                this.setState({
                    stats: res.data,
                    modalLoader: false,
                    renderChild: true
                });
            });
    }

    async switchTab(e) {
        const type = e.target.id.split(/-/)[0];
        if (this.state.activeTab === type) {
            return;
        }

        this.setState({activeTab: type});

        if (this.state[type].length === 0) {
            this.setState({loader: true});


            await getPlayersLeaderboard({type})
                .then((res) => {
                    this.state.showButton[type] = res.data.length === LOAD_COUNT;
                    this.setState({
                        [type]: res.data,
                        loader: false
                    });
                });
        }
    }

    async loadMore() {
        const type = this.state.activeTab;
        this.setState({loadMore: true});
        await getPlayersLeaderboard({type, from: this.state[type].length, limit: LOAD_COUNT})
            .then((res) => {
                this.state.showButton[type] = res.data.length === LOAD_COUNT;
                this.setState({
                    [type]: this.state[type].concat(res.data),
                    loadMore: false
                });
            });
    }

    seeMoreBtn(type) {
        return this.state.showButton[type] ?
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
        const daily = this.createPlayers(this.state.daily);
        const weekly = this.createPlayers(this.state.weekly);
        const monthly = this.createPlayers(this.state.monthly);
        const seeMoreBtn = this.state.loader || this.state.loadMore
            ? <div className="Loader"/>
            : this.seeMoreBtn(this.state.activeTab);
        return (
            <>
                <Helmet>
                    <title>NBA Players Leaderboard | Fantasy Hoops</title>
                    <meta name="description" content={Meta.DESCRIPTION}/>
                    <link rel="canonical" href={Canonicals.PLAYERS_LEADERBOARD}/>
                </Helmet>
                <Container maxWidth="md">
                    <div className="text-center">
                        <img
                            src={leaderboardLogo}
                            alt="Leaderboard Logo"
                            width="60rem"
                        />
                        <h1>Top NBA Players</h1>
                    </div>
                    <ul className="nav nav-pills justify-content-center mx-auto" id="myTab" role="tablist">
                        <li className="nav-item">
                            <a className="nav-link active tab-no-outline" id="daily-tab" data-toggle="tab" href="#daily"
                               role="tab" onClick={this.switchTab}>Daily</a>
                        </li>
                        <li className="nav-item">
                            <a className="nav-link tab-no-outline" id="weekly-tab" data-toggle="tab" href="#weekly"
                               role="tab" onClick={this.switchTab}>Weekly</a>
                        </li>
                        <li className="nav-item">
                            <a className="nav-link tab-no-outline" id="monthly-tab" data-toggle="tab" href="#monthly"
                               role="tab" onClick={this.switchTab}>Monthly</a>
                        </li>
                    </ul>
                    <div className="tab-content" id="myTabContent">
                        <div className="pt-4 pb-1 tab-pane show active animated bounceInUp" id="daily" role="tabpanel">
                            {!this.state.loader
                                ? daily.length > 0
                                    ? daily
                                    : <EmptyJordan message="Such empty..."/>
                                : ''}
                            <div className="text-center">
                                {seeMoreBtn}
                            </div>
                        </div>
                        <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="weekly" role="tabpanel">
                            {!this.state.loader
                                ? weekly.length > 0
                                    ? weekly
                                    : <EmptyJordan message="Such empty..."/>
                                : ''}
                            <div className="text-center">
                                {seeMoreBtn}
                            </div>
                        </div>
                        <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="monthly" role="tabpanel">
                            {!this.state.loader
                                ? monthly.length > 0
                                    ? monthly
                                    : <EmptyJordan message="Such empty..."/>
                                : ''}
                            <div className="text-center">
                                {seeMoreBtn}
                            </div>
                        </div>
                    </div>
                    <PlayerModal
                        renderChild={this.state.renderChild}
                        loader={this.state.modalLoader}
                        stats={this.state.stats}
                    />
                </Container>
            </>
        );
    }
}
