import React, {Component} from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import {UserScore} from './UserScore';
import {PlayerModal} from '../PlayerModal/PlayerModal';
import {parse} from '../../utils/auth';
import {getPlayerStats, getRecentLineups} from '../../utils/networkFunctions';
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../../utils/helpers";
import Container from "@material-ui/core/Container";
import PlayerDialog from "../PlayerModal/PlayerDialog";

const {$} = window;
const user = parse();
const LOAD_COUNT = 5;

export class LineupHistory extends Component {
    constructor(props) {
        super(props);
        this.loadMore = this.loadMore.bind(this);
        this.showModal = this.showModal.bind(this);
        this.handlePlayerDialogOpen = this.handlePlayerDialogOpen.bind(this);
        this.handlePlayerDialogClose = this.handlePlayerDialogClose.bind(this);

        this.state = {
            stats: '',
            loadCounter: 0,
            user,
            recentActivity: [],
            loader: true,
            modalLoader: true,
            playerDialogOpen: false
        };
    }

    async componentDidMount() {
        await getRecentLineups(user.id, {count: 10})
            .then((res) => {
                this.setState({
                    recentActivity: res.data,
                    loader: false,
                    readOnly: false
                });
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

    async showModal(playerId) {
        this.setState({modalLoader: true});
        this.handlePlayerDialogOpen();
        await getPlayerStats(playerId)
            .then((res) => {
                this.setState({
                    stats: res.data,
                    modalLoader: false,
                    renderChild: true
                });
            });
    }

    async loadMore() {
        this.setState({
            loadCounter: this.state.loadCounter + 1,
            loader: true
        });
        await getRecentLineups(user.id, {start: this.state.recentActivity.length, count: LOAD_COUNT})
            .then((res) => {
                this.setState({
                    recentActivity: this.state.recentActivity.concat(res.data),
                    loader: false
                });
            });
    }

    render() {
        const recentActivity = _.map(
            this.state.recentActivity,
            activity => (
                <UserScore
                    key={shortid()}
                    activity={activity}
                    showModal={this.showModal}
                    center="0 auto"
                />
            )
        );

        const btn = this.state.loadCounter * LOAD_COUNT + 10 > this.state.recentActivity.length
            ? ''
            : <button className="btn btn-primary m-3" onClick={this.loadMore}>See more</button>;

        return (
            <>
                <Helmet>
                    <title>Lineup History | Fantasy Hoops</title>
                    <meta property="title" content="Lineup History | Fantasy Hoops"/>
                    <meta property="og:title" content="Lineup History | Fantasy Hoops"/>
                    <meta name="description" content={Meta.DESCRIPTION}/>
                    <meta property="og:description" content={Meta.DESCRIPTION}/>
                    <meta name="robots" content="noindex,nofollow"/>
                    <link rel="canonical" href={Canonicals.LINEUPS_HISTORY}/>
                </Helmet>
                <h1 className="text-center p-3">
                    <span className="fa fa-history"/>
                    {' '}
                    Your lineup history
                </h1>
                {recentActivity}
                {this.state.loader ? <div className="Loader"/> : null}
                <div className="text-center">
                    {!this.state.loader ? btn : ''}
                </div>
                {
                    this.state.playerDialogOpen &&
                    <PlayerDialog
                        renderChild={this.state.renderChild}
                        loader={this.state.modalLoader}
                        stats={this.state.stats}
                        open={this.state.playerDialogOpen}
                        onDialogOpen={this.handlePlayerDialogOpen}
                        onDialogClose={this.handlePlayerDialogClose}
                    />
                }
            </>
        );
    }
}
