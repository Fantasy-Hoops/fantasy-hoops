import React, { PureComponent } from 'react';
import { Link } from 'react-router-dom';
import shortid from 'shortid';
import _ from 'lodash';
import UserCard from '../Users/Card';
import { Card as PlayerCard } from '../Players/Card';
import leaderboardLogo from '../../../../content/images/leaderboard.png';
import EmptyJordan from '../../EmptyJordan';
import { PlayerModal } from '../../PlayerModal/PlayerModal';
import { getSeasonLineupsLeaderboard, getSeasonPlayersLeaderboard, getPlayerStats } from '../../../utils/networkFunctions';

const { $ } = window;

export class Leaderboard extends PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      activeTab: 'lineups',
      lineups: '',
      players: '',
      loader: true,
      stats: '',
      modalLoader: true,
      renderChild: false
    };
    this.showModal = this.showModal.bind(this);
    this.loadLineups = this.loadLineups.bind(this);
    this.loadPlayers = this.loadPlayers.bind(this);
    this.switchTab = this.switchTab.bind(this);
    this.onLineupsDateChange = this.onLineupsDateChange.bind(this);
    this.onPlayersDateChange = this.onPlayersDateChange.bind(this);
  }

  async componentDidMount() {
    $('#playerModal').on('hidden.bs.modal', () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
    await getSeasonLineupsLeaderboard()
      .then((res) => {
        this.setState({
          lineups: res.data,
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

  async onLineupsDateChange(e) {
    const lineupsYear = e.target.value;
    if (!lineupsYear) {
      this.setState({ lineupsYear });
      return;
    }

    this.setState({ loader: true });
    const lineups = await getSeasonLineupsLeaderboard({ year: lineupsYear });
    this.setState({
      lineups: lineups.data,
      loader: false,
      lineupsYear
    });
  }

  async onPlayersDateChange(e) {
    const playersYear = e.target.value;
    if (!playersYear) {
      this.setState({ playersYear });
      return;
    }

    this.setState({ loader: true });
    const players = await getSeasonPlayersLeaderboard({ year: playersYear });
    this.setState({
      players: players.data,
      loader: false,
      playersYear
    });
  }

  async switchTab(e) {
    const { activeTab } = this.state;
    const type = e.target.id.split(/-/)[0];

    if (activeTab === type) { return; }

    this.setState({ activeTab: type });

    if (this.state[type].length === 0) {
      await getSeasonPlayersLeaderboard()
        .then((res) => {
          this.setState({
            [type]: res.data,
            loader: false
          });
        });
    }
  }

  async showModal(player) {
    this.setState({ modalLoader: true });
    await getPlayerStats(player.nbaId)
      .then((res) => {
        this.setState({
          stats: res.data,
          modalLoader: false,
          renderChild: true
        });
      });
  }

  loadLineups(users) {
    return _.map(
      users,
      (user, index) => (
        <UserCard
          isDaily
          index={index}
          key={shortid()}
          user={user}
          showModal={this.showModal}
        />
      )
    );
  }

  loadPlayers(players) {
    return _.map(
      players,
      (player, index) => (
        <PlayerCard
          index={index}
          key={shortid()}
          player={player}
          showModal={this.showModal}
          image={this.state[player.position]}
          season
        />
      )
    );
  }

  render() {
    const {
      lineups, players, renderChild, modalLoader, stats, lineupsYear, playersYear
    } = this.state;
    const lineupCards = this.loadLineups(lineups);
    const playerCards = this.loadPlayers(players);
    return (
      <div className="container bg-light">
        <div className="text-center pb-1">
          <img
            src={leaderboardLogo}
            alt="Leaderboard Logo"
            width="60rem"
          />
          <h1>Top 10 Season Performances</h1>
        </div>
        <ul className="nav nav-pills justify-content-center mx-auto" id="myTab" role="tablist">
          <li className="nav-item">
            <a className="nav-link active tab-no-outline" id="lineups-tab" data-toggle="tab" href="#lineups" role="tab" onClick={this.switchTab}>Lineups</a>
          </li>
          <li className="nav-item">
            <a className="nav-link tab-no-outline" id="players-tab" data-toggle="tab" href="#players" role="tab" onClick={this.switchTab}>Players</a>
          </li>
        </ul>
        <div className="tab-content" id="myTabContent">
          <div className="pt-4 pb-1 tab-pane show active animated bounceInUp" id="lineups" role="tabpanel">
            {!this.state.loader
              ? (
                <select
                  className="form-control custom-select select select-year"
                  value={lineupsYear}
                  onChange={this.onLineupsDateChange}
                >
                  <option value={-1}>All time</option>
                  <option value={2018}>2018/2019</option>
                  <option value={2019}>2019/2020</option>
                </select>
              )
              : null}
            <div className="text-center">
              {!this.state.loader
                ? lineupCards.length > 0
                  ? lineupCards
                  : <EmptyJordan message="Such empty..." />
                : <div className="Loader" />}
            </div>
          </div>
          <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="players" role="tabpanel">
            {!this.state.loader
              ? (
                <select
                  className="form-control custom-select select select-year"
                  value={playersYear}
                  onChange={this.onPlayersDateChange}
                >
                  <option value={-1}>All time</option>
                  <option value={2018}>2018/2019</option>
                  <option value={2019}>2019/2020</option>
                </select>
              )
              : null}
            <div className="text-center">
              {!this.state.loader
                ? playerCards.length > 0
                  ? playerCards
                  : <EmptyJordan message="Such empty..." />
                : <div className="Loader" />}
            </div>
          </div>
        </div>
        <PlayerModal
          renderChild={renderChild}
          loader={modalLoader}
          stats={stats}
        />
      </div>
    );
  }
}

export default Leaderboard;
