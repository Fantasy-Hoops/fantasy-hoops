import React, {Component} from 'react';
import Countdown from 'react-countdown';
import {PlayerPool} from './PlayerPool';
import {PlayerCard} from './PlayerCard';
import {ProgressBar} from './ProgressBar';
import {parse} from '../../utils/auth';
import {AlertNotification as Alert} from '../AlertNotification';
import {PlayerModal} from '../PlayerModal/PlayerModal';
import EmptyJordan from '../EmptyJordan';
import {
    getNextGameInfo, getUserLineup, getPlayers, getPlayerStats, submitLineup
} from '../../utils/networkFunctions';
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../../utils/helpers";
import {Container} from "@material-ui/core";
import InfoDialog from "./InfoDialog";
import moment from "moment";
import PlayerDialog from "../PlayerModal/PlayerDialog";

const {$} = window;
const budget = 300; // thousands

export class Lineup extends Component {
    constructor() {
        super();

        this.selectPlayer = this.selectPlayer.bind(this);
        this.filter = this.filter.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.showModal = this.showModal.bind(this);
        this.handleDialogOpen = this.handleDialogOpen.bind(this);
        this.handleDialogClose = this.handleDialogClose.bind(this);
        this.handlePlayerDialogOpen = this.handlePlayerDialogOpen.bind(this);
        this.handlePlayerDialogClose = this.handlePlayerDialogClose.bind(this);

        this.state = {
            position: '',
            lineup: {
                pg: <PlayerCard filter={this.filter} status={0} position="PG"/>,
                sg: <PlayerCard filter={this.filter} status={0} position="SG"/>,
                sf: <PlayerCard filter={this.filter} status={0} position="SF"/>,
                pf: <PlayerCard filter={this.filter} status={0} position="PF"/>,
                c: <PlayerCard filter={this.filter} status={0} position="C"/>
            },
            loadedPlayers: false,
            alertType: '',
            alertText: '',
            nextGame: '',
            playerLoader: false,
            submit: true,
            isGame: true,
            modalLoader: true,
            poolLoader: true,
            renderChild: true,
            infoDialogOpen: false,
            playerDialogOpen: false
        };
    }

    handleDialogOpen() {
        this.setState({
            infoDialogOpen: true
        })
    }

    handleDialogClose() {
        this.setState({
            infoDialogOpen: false
        })
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

    async componentDidMount() {
        this.setState({
            playerLoader: true
        });
        await getNextGameInfo()
            .then((res) => {
                if (new Date(res.data.nextGame).getFullYear() !== 1) {
                    this.setState({
                        nextGame: res.data.nextGame,
                        playerPoolDate: res.data.playerPoolDate,
                        poolLoader: false
                    });
                } else {
                    this.setState({
                        isGame: false,
                        poolLoader: false
                    });
                }
            });

        if (!this.state.isGame) return;

        await getPlayers()
            .then((res) => {
                this.setState({
                    players: res.data,
                    playerLoader: false
                });
                this.filter('PG');
            });
    }

    async componentDidUpdate() {
        if (!this.state.isGame) return;

        if (!this.state.loadedPlayers && this.state.players) {
            const user = parse();
            const {players} = this.state;
            await getUserLineup(user.id)
                .then((res) => {
                    if (res.status !== 200) return;
                    const lineup = res.data;
                    players.filter(p => p.playerId === lineup.pgID || p.playerId === lineup.sgID || p.playerId === lineup.sfID
                        || p.playerId === lineup.pfID || p.playerId === lineup.cid)
                        .forEach(p => {
                            p.selected = true;
                            p.status = 2;
                            this.selectPlayer(p);
                        });
                });
            this.setState({
                loadedPlayers: true
            });
        }

        const btn = document.getElementById('submit');
        if (
            this.state.lineup.pg.props.player
            && this.state.lineup.sg.props.player
            && this.state.lineup.sf.props.player
            && this.state.lineup.pf.props.player
            && this.state.lineup.c.props.player
            && this.calculateRemaining() >= 0
            && this.state.playerPoolDate === this.state.nextGame
            && this.state.submit
            && btn !== null
        ) {
            btn.disabled = false;
            btn.className = 'Lineup__submit-button btn btn-primary btn-block';
        } else if (btn !== null) {
            btn.disabled = true;
            btn.className = 'Lineup__submit-button btn btn-outline-primary btn-block';
        }
    }

    getDate() {
        const dt = new Date();
        const tz = dt.getTimezoneOffset();
        return moment(this.state.nextGame).add(-tz, 'minutes').format();
    }

    render() {
        if (this.state.poolLoader) {
            return (
                <div className="p-5">
                    <div className="Loader"/>
                </div>
            );
        }

        if (!this.state.isGame) {
            return (
                <div className="p-5">
                    <EmptyJordan message="The game hasn't started yet..."/>
                </div>
            );
        }

        const remaining = this.calculateRemaining();
        const Completionist = () => (
            <span>The game already started. Come back soon!</span>
        );
        const renderer = ({
                              days, hours, minutes, seconds, completed
                          }) => {
            if (completed) {
                this.state.submit = false;
                return <Completionist/>;
            }
            if (this.state.playerPoolDate !== this.state.nextGame) {
                return <div className="Lineup__countdown">Please wait a moment until player pool is updated!</div>;
            }
            this.state.submit = true;

            days = this.getFormattedDateString(days, 'day');
            hours = this.getFormattedDateString(hours, 'hour');
            minutes = this.getFormattedDateString(minutes, 'minute');
            seconds = this.getFormattedDateString(seconds, 'second');

            return (
                <span>
          Game starts in
                    {' '}
                    <strong>
            {days}
                        {hours}
                        {minutes}
                        {seconds}
          </strong>
        </span>
            );
        };
        const playerPool = () => {
            if (
                this.state.playerPoolDate !== this.state.nextGame
                && !this.state.playerLoader
            ) {
                return (
                    <div className="p-5">
                        <EmptyJordan message="Player pool is empty..."/>
                    </div>
                );
            }
            return (
                <PlayerPool
                    remaining={remaining}
                    lineup={this.state.lineup}
                    position={this.state.position}
                    players={this.state.players}
                    selectPlayer={this.selectPlayer}
                    showModal={this.showModal}
                />
            );
        };

        return (
            <>
                <Helmet>
                    <title>Lineup | Fantasy Hoops</title>
                    <meta property="title" content="Lineup | Fantasy Hoops"/>
                    <meta property="og:title" content="Lineup | Fantasy Hoops"/>
                    <meta property="og:description" content={Meta.DESCRIPTION}/>
                    <meta name="description" content={Meta.DESCRIPTION}/>
                    <meta name="robots" content="noindex,nofollow"/>
                    <link rel="canonical" href={Canonicals.LINEUP}/>
                </Helmet>
                <Alert
                    ref="alert"
                    {...this.props}
                    type={this.state.alertType}
                    text={this.state.alertText}
                />
                <div className="Lineup--sticky">
                    <div className="Lineup__countdown text-center">
                        {this.state.nextGame && <Countdown
                            now={moment}
                            date={this.getDate()}
                            zeroPadTime={3}
                            zeroPadDays={3}
                            renderer={renderer}
                        />}
                    </div>
                    <div className="Lineup__body">
                        {this.state.lineup.pg}
                        {this.state.lineup.sg}
                        {this.state.lineup.sf}
                        {this.state.lineup.pf}
                        {this.state.lineup.c}
                    </div>
                    <p
                        className="Lineup__moneyRemaining text-center m-2"
                        style={{color: remaining < 0 ? 'red' : 'black'}}
                    >
                        Remaining
                        {' '}
                        {remaining}
                        K
                    </p>
                    <ProgressBar players={this.state}/>
                    <div
                        className="text-center mt-3 pb-3 mx-auto position-relative"
                        style={{width: '50%'}}
                    >
                        <form onSubmit={this.handleSubmit}>
                            <button
                                id="submit"
                                disabled
                                className="Lineup__submit-button btn btn-outline-primary"
                            >
                                Submit
                            </button>
                        </form>
                        <button
                            type="button"
                            className="btn btn-primary absolute Lineup__info-button"
                            onClick={this.handleDialogOpen}
                        >
                            <i className="fa fa-info mx-auto" aria-hidden="true"/>
                        </button>
                    </div>
                </div>
                {this.state.playerLoader ? <div className="Loader"/> : null}
                {playerPool()}
                <InfoDialog
                    open={this.state.infoDialogOpen}
                    onDialogOpen={this.handleDialogOpen}
                    onDialogClose={this.handleDialogClose}
                />
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

    filter(pos) {
        this.setState({
            position: pos
        });
    }

    selectPlayer(player) {
        const pos = player.position.toLowerCase();
        const playerCard = player.selected ? (
            <PlayerCard
                status={2}
                filter={this.filter}
                player={player}
                selectPlayer={this.selectPlayer}
                position={player.position}
                showModal={this.showModal}
            />
        ) : (
            <PlayerCard status={0} filter={this.filter} position={player.position}/>
        );
        this.state.lineup[pos] = playerCard;
        this.setState({
            lineup: this.state.lineup
        });
    }

    async showModal(player) {
        this.setState({modalLoader: true});
        this.handlePlayerDialogOpen();
        await getPlayerStats(player.id)
            .then((res) => {
                this.setState({
                    stats: res.data,
                    modalLoader: false,
                    renderChild: true
                });
            });
    }

    calculateRemaining() {
        const remaining = budget
            - this.price(this.state.lineup.pg)
            - this.price(this.state.lineup.sg)
            - this.price(this.state.lineup.sf)
            - this.price(this.state.lineup.pf)
            - this.price(this.state.lineup.c);
        return remaining;
    }

    price(player) {
        const playerPrice = player.props.status === 2 ? parseInt(player.props.player.price, 10) : 0;
        return playerPrice;
    }

    async handleSubmit(e) {
        e.preventDefault();
        const user = parse();
        const data = {
            UserID: user.id,
            PgID: this.state.lineup.pg.props.player.playerId,
            SgID: this.state.lineup.sg.props.player.playerId,
            SfID: this.state.lineup.sf.props.player.playerId,
            PfID: this.state.lineup.pf.props.player.playerId,
            CID: this.state.lineup.c.props.player.playerId,
            PgPrice: this.state.lineup.pg.props.player.price,
            SgPrice: this.state.lineup.sg.props.player.price,
            SfPrice: this.state.lineup.sf.props.player.price,
            PfPrice: this.state.lineup.pf.props.player.price,
            CPrice: this.state.lineup.c.props.player.price
        };

        await submitLineup(data)
            .then((res) => {
                this.setState({
                    alertType: 'success',
                    alertText: res.data
                });
            })
            .catch((err) => {
                this.setState({
                    alertType: 'danger',
                    alertText: err.response.data
                });
            })
            .then(this.refs.alert.addNotification);
    }

    getFormattedDateString(value, word) {
        if (value === 1) {
            return `${value} ${word} `;
        }
        if (value > 1) {
            return `${value} ${word}s `;
        }
        return '';
    }
}

export default Lineup;
