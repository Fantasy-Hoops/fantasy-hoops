import React, { Component } from 'react';
import { Card as UserCard } from '../Users/Card';
import { Card as PlayerCard } from '../Players/Card';
import leaderboardLogo from '../../../content/images/leaderboard.png';
import shortid from 'shortid';
import _ from 'lodash';
import { Loader } from '../../Loader';
import { EmptyJordan } from '../../EmptyJordan';

export class Leaderboard extends Component {
  constructor(props) {
    super(props);
    this.state = {
      activeTab: 'lineups',
      lineups: '',
      players: '',
      loader: true
    }
    this.loadLineups = this.loadLineups.bind(this);
    this.loadPlayers = this.loadPlayers.bind(this);
    this.switchTab = this.switchTab.bind(this);
  }

  async componentWillMount() {
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/season/lineups`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          lineups: res,
          loader: false
        });
      })
  }

  async switchTab(e) {
    const type = e.target.id.split(/-/)[0];

    if (this.state.activeTab === type)
      return;

    this.setState({ activeTab: type });

    if (this.state[type].length === 0) {
      this.setState({ loader: true });

      await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/season/${type}`)
        .then(res => {
          return res.json()
        })
        .then(res => {
          this.setState({
            [type]: res,
            loader: false
          });
        })
    }
  }

  render() {
    const lineups = this.loadLineups(this.state.lineups);
    const players = this.loadPlayers(this.state.players);
    return (
      <div className="container bg-light pt-3 p-0">
        <div className="text-center pb-1">
          <img src={leaderboardLogo}
            alt="Leaderboard Logo"
            width="60rem"
          />
          <h3>Top 10 Season Performances</h3>
        </div>
        <ul className="nav nav-pills justify-content-center mx-auto" id="myTab" role="tablist" style={{ width: '30%' }}>
          <li className="nav-item">
            <a className="nav-link active tab-no-outline" id="lineups-tab" data-toggle="tab" href="#lineups" role="tab" onClick={this.switchTab}>Lineups</a>
          </li>
          <li className="nav-item">
            <a className="nav-link tab-no-outline" id="players-tab" data-toggle="tab" href="#players" role="tab" onClick={this.switchTab}>Players</a>
          </li>
        </ul>
        <div className="tab-content" id="myTabContent">
          <div className="pt-4 pb-1 tab-pane fade show active animated bounceInUp" id="lineups" role="tabpanel">
            <div className="text-center">
              {!this.state.loader
                ? lineups.length > 0
                  ? lineups
                  : <EmptyJordan message="Such empty..." />
                : <Loader show={this.state.loader} />}
            </div>
          </div>
          <div className="pt-4 pb-1 tab-pane fade animated bounceInUp" id="players" role="tabpanel">
            <div className="text-center">
              {!this.state.loader
                ? players.length > 0
                  ? players
                  : <EmptyJordan message="Such empty..." />
                : <Loader show={this.state.loader} />}
            </div>
          </div>
        </div>
      </div >
    );
  }

  loadLineups(users) {
    return _.map(
      users,
      (user, index) => {
        return <UserCard
          isDaily={true}
          index={index}
          key={shortid()}
          user={user}
        />
      }
    );
  }

  loadPlayers(players) {
    return _.map(
      players,
      (player, index) => {
        return <PlayerCard
          index={index}
          key={shortid()}
          player={player}
          showModal={this.showModal}
        />
      }
    );
  }
}