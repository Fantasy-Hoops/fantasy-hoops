import React, { Component } from 'react';
import { PlayerLeaderboardCard as Card } from './PlayerLeaderboardCard';
import leaderboardLogo from '../../content/images/leaderboard.png';
import shortid from 'shortid';
import { PlayerModal } from '../PlayerModal/PlayerModal';
import { Loader } from '../Loader';
import { EmptyJordan } from '../EmptyJordan';
import _ from 'lodash';

const $ = window.$;
const LOAD_COUNT = 10;

export class PlayerLeaderboard extends Component {
  constructor(props) {
    super(props);
    this.state = {
      dailyPlayers: [],
      weeklyPlayers: [],
      monthlyPlayers: [],
      stats: '',
      dailyLoader: true,
      weeklyLoader: true,
      monthlyLoader: true,
      modalLoader: true,
      renderChild: true,
      loader: false,
      dailyLoadCounter: 0,
      weeklyLoadCounter: 0,
      monthlyLoadCounter: 0
    }

    this.showModal = this.showModal.bind(this);
    this.loadDaily = this.loadDaily.bind(this);
    this.loadWeekly = this.loadWeekly.bind(this);
    this.loadMonthly = this.loadMonthly.bind(this);
  }

  componentDidMount() {
    $("#playerModal").on("hidden.bs.modal", () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
  }

  async componentWillMount() {
    await fetch(`http://fantasyhoops.org/api/leaderboard/player?type=daily`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          dailyPlayers: res,
          dailyLoader: false
        });
      })
    await fetch(`http://fantasyhoops.org/api/leaderboard/player?type=weekly`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          weeklyPlayers: res,
          weeklyLoader: false
        });
      })
    await fetch(`http://fantasyhoops.org/api/leaderboard/player?type=monthly`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          monthlyPlayers: res,
          monthlyLoader: false
        });
      })
    await this.setState({
      PG: require(`../../content/images/positions/pg.png`),
      SG: require(`../../content/images/positions/sg.png`),
      SF: require(`../../content/images/positions/sf.png`),
      PF: require(`../../content/images/positions/pf.png`),
      C: require(`../../content/images/positions/c.png`)
    });
  }

  async showModal(player) {
    this.setState({ modalLoader: true })
    await fetch(`http://fantasyhoops.org/api/stats/${player.nbaID}`)
      .then(res => res.json())
      .then(res => {
        this.setState({
          stats: res,
          modalLoader: false,
          renderChild: true
        });
      });
  }

  async loadDaily() {
    this.setState({
      loader: true,
      dailyLoadCounter: this.state.dailyLoadCounter + 1
    });
    await fetch(`http://fantasyhoops.org/api/leaderboard/player?type=daily&from=${this.state.dailyPlayers.length}&limit=${LOAD_COUNT}`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          dailyPlayers: this.state.dailyPlayers.concat(res),
          loader: false
        });
      });
  }

  async loadWeekly() {
    this.setState({
      loader: true,
      weeklyLoadCounter: this.state.weeklyLoadCounter + 1
    });
    await fetch(`http://fantasyhoops.org/api/leaderboard/player?type=weekly&from=${this.state.weeklyPlayers.length}&limit=${LOAD_COUNT}`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          weeklyPlayers: this.state.weeklyPlayers.concat(res),
          loader: false
        });
      });
  }

  async loadMonthly() {
    this.setState({
      loader: true,
      monthlyLoadCounter: this.state.monthlyLoadCounter + 1
    });
    await fetch(`http://fantasyhoops.org/api/leaderboard/player?type=monthly&from=${this.state.monthlyPlayers.length}&limit=${LOAD_COUNT}`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          monthlyPlayers: this.state.monthlyPlayers.concat(res),
          loader: false
        });
      });
  }

  render() {
    const dailyPlayers = this.createPlayers(this.state.dailyPlayers);
    const weeklyPlayers = this.createPlayers(this.state.weeklyPlayers);
    const monthlyPlayers = this.createPlayers(this.state.monthlyPlayers);

    const dailyBtn = this.state.dailyLoadCounter * LOAD_COUNT + 10 > this.state.dailyPlayers.length
      ? ''
      : <button className="btn btn-primary mt-2" onClick={this.loadDaily}>See more</button>;

    const weeklyBtn = this.state.weeklyLoadCounter * LOAD_COUNT + 10 > this.state.weeklyPlayers.length
      ? ''
      : <button className="btn btn-primary mt-2" onClick={this.loadWeekly}>See more</button>;

    const monthlyBtn = this.state.monthlyLoadCounter * LOAD_COUNT + 10 > this.state.monthlyPlayers.length
      ? ''
      : <button className="btn btn-primary mt-2" onClick={this.loadMonthly}>See more</button>;

    return (
      <div className="container bg-light pt-2 pb-3">
        <div className="text-center pb-3">
          <img src={leaderboardLogo}
            alt=""
            width="60rem"
          />
          <h3>Top NBA Players</h3>
        </div>
        <ul className="nav nav-tabs justify-content-center mx-auto" id="myTab" role="tablist" style={{ width: '40%' }}>
          <li className="nav-item">
            <a className="nav-link active tab-no-outline" id="daily-tab" data-toggle="tab" href="#daily" role="tab">Daily</a>
          </li>
          <li className="nav-item">
            <a className="nav-link tab-no-outline" id="weekly-tab" data-toggle="tab" href="#weekly" role="tab">Weekly</a>
          </li><li className="nav-item">
            <a className="nav-link tab-no-outline" id="monthly-tab" data-toggle="tab" href="#monthly" role="tab">Monthly</a>
          </li>
        </ul>
        <div className="tab-content" id="myTabContent">
          <div className="pt-4 pb-1 tab-pane fade show active" id="daily" role="tabpanel">
            {!this.state.dailyLoader
              ? dailyPlayers.length > 0
                ? dailyPlayers
                : <EmptyJordan message="Such empty..." />
              : <Loader show={this.state.dailyLoader} />}
            <div className="text-center">
              {!this.state.loader ? dailyBtn : ''}
            </div>
            <Loader show={this.state.loader} />
          </div>
          <div className="pt-4 pb-1 tab-pane fade" id="weekly" role="tabpanel">
            {!this.state.weeklyLoader
              ? weeklyPlayers.length > 0
                ? weeklyPlayers
                : <EmptyJordan message="Such empty..." />
              : <Loader show={this.state.weeklyLoader} />}
            <div className="text-center">
              {!this.state.loader ? weeklyBtn : ''}
            </div>
            <Loader show={this.state.loader} />
          </div>
          <div className="pt-4 pb-1 tab-pane fade" id="monthly" role="tabpanel">
            {!this.state.monthlyLoader
              ? monthlyPlayers.length > 0
                ? monthlyPlayers
                : <EmptyJordan message="Such empty..." />
              : <Loader show={this.state.monthlyLoader} />}
            <div className="text-center">
              {!this.state.loader ? monthlyBtn : ''}
            </div>
            <Loader show={this.state.loader} />
          </div>
        </div>
        <PlayerModal
          renderChild={this.state.renderChild}
          loader={this.state.modalLoader}
          stats={this.state.stats}
        />
      </div>
    );
  }

  createPlayers(players) {
    return _.map(
      players,
      (player, index) => {
        return <Card
          index={index}
          key={shortid()}
          player={player}
          showModal={this.showModal}
          image={this.state[player.position]}
        />
      }
    );
  }
}
