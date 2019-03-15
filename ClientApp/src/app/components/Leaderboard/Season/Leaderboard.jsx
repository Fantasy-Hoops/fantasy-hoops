import React, { PureComponent } from 'react';
import { Link } from 'react-router-dom';
import shortid from 'shortid';
import _ from 'lodash';
import { Card as UserCard } from '../Users/Card';
import { Card as PlayerCard } from '../Players/Card';
import leaderboardLogo from '../../../../content/images/leaderboard.png';
import { Loader } from '../../Loader';
import { EmptyJordan } from '../../EmptyJordan';
import { PlayerModal } from '../../PlayerModal/PlayerModal';

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
  }

  async componentDidMount() {
    $('#playerModal').on('hidden.bs.modal', () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/season/lineups`)
      .then(res => res.json())
      .then((res) => {
        this.setState({
          lineups: res,
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

  switchTab(e) {
    const { activeTab } = this.state;
    const type = e.target.id.split(/-/)[0];

    if (activeTab === type) { return; }

    this.setState({ activeTab: type });

    if (this.state[type].length === 0) {
      fetch(`${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/season/${type}`)
        .then(res => res.json())
        .then((res) => {
          this.setState({
            [type]: res,
            loader: false
          });
        });
    }
  }

  async showModal(player) {
    this.setState({ modalLoader: true });
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/stats/${player.nbaID}`)
      .then(res => res.json())
      .then((res) => {
        this.setState({
          stats: res,
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
      lineups, players, renderChild, modalLoader, stats
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
          <h3>Top 10 Season Performances</h3>
        </div>
        <ul className="nav nav-pills justify-content-center mx-auto" id="myTab" role="tablist" style={{ width: '30%' }}>
          <li className="nav-item">
            <Link className="nav-link active tab-no-outline" id="lineups-tab" data-toggle="tab" to="#lineups" role="tab" onClick={this.switchTab}>Lineups</Link>
          </li>
          <li className="nav-item">
            <Link className="nav-link tab-no-outline" id="players-tab" data-toggle="tab" to="#players" role="tab" onClick={this.switchTab}>Players</Link>
          </li>
        </ul>
        <div className="tab-content" id="myTabContent">
          <div className="pt-4 pb-1 tab-pane fade show active animated bounceInUp" id="lineups" role="tabpanel">
            <div className="text-center">
              {!this.state.loader
                ? lineupCards.length > 0
                  ? lineupCards
                  : <EmptyJordan message="Such empty..." />
                : <Loader show={this.state.loader} />}
            </div>
          </div>
          <div className="pt-4 pb-1 tab-pane fade animated bounceInUp" id="players" role="tabpanel">
            <div className="text-center">
              {!this.state.loader
                ? playerCards.length > 0
                  ? playerCards
                  : <EmptyJordan message="Such empty..." />
                : <Loader show={this.state.loader} />}
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
