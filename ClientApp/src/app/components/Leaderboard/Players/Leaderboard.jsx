import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import shortid from 'shortid';
import _ from 'lodash';
import { Card } from './Card';
import leaderboardLogo from '../../../../content/images/leaderboard.png';
import { PlayerModal } from '../../PlayerModal/PlayerModal';
import { Loader } from '../../Loader';
import { EmptyJordan } from '../../EmptyJordan';

const { $ } = window;
const LOAD_COUNT = 30;

export class Leaderboard extends Component {
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

  componentDidMount() {
    $('#playerModal').on('hidden.bs.modal', () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
  }

  async componentDidMount() {
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/player?type=daily`)
      .then(res => res.json())
      .then((res) => {
        this.state.showButton.daily = res.length === LOAD_COUNT;
        this.setState({
          daily: res,
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

  async switchTab(e) {
    const type = e.target.id.split(/-/)[0];
    if (this.state.activeTab === type) { return; }

    this.setState({ activeTab: type });

    if (this.state[type].length === 0) {
      this.setState({ loader: true });

      await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/player?type=${type}`)
        .then(res => res.json())
        .then((res) => {
          this.state.showButton[type] = res.length === LOAD_COUNT;
          this.setState({
            [type]: res,
            loader: false
          });
        });
    }
  }

  async loadMore() {
    const type = this.state.activeTab;
    this.setState({ loadMore: true });

    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/player?type=${type}&from=${this.state[type].length}&limit=${LOAD_COUNT}`)
      .then(res => res.json())
      .then((res) => {
        this.state.showButton[type] = res.length === LOAD_COUNT;
        this.setState({
          [type]: this.state[type].concat(res),
          loadMore: false
        });
      });
  }

  seeMoreBtn(type) {
    return this.state.showButton[type] ? <button className="btn btn-primary mt-2" onClick={this.loadMore}>See more</button> : '';
  }

  render() {
    const daily = this.createPlayers(this.state.daily);
    const weekly = this.createPlayers(this.state.weekly);
    const monthly = this.createPlayers(this.state.monthly);
    const seeMoreBtn = this.state.loader || this.state.loadMore
      ? <Loader show={this.state.loader || this.state.loadMore} />
      : this.seeMoreBtn(this.state.activeTab);
    return (
      <div className="container bg-light">
        <div className="text-center">
          <img
            src={leaderboardLogo}
            alt="Leaderboard Logo"
            width="60rem"
          />
          <h3>Top NBA Players</h3>
        </div>
        <ul className="nav nav-pills justify-content-center mx-auto" id="myTab" role="tablist" style={{ width: '40%' }}>
          <li className="nav-item">
            <Link className="nav-link active tab-no-outline" id="daily-tab" data-toggle="tab" to="#daily" role="tab" onClick={this.switchTab}>Daily</Link>
          </li>
          <li className="nav-item">
            <Link className="nav-link tab-no-outline" id="weekly-tab" data-toggle="tab" to="#weekly" role="tab" onClick={this.switchTab}>Weekly</Link>
          </li>
          <li className="nav-item">
            <Link className="nav-link tab-no-outline" id="monthly-tab" data-toggle="tab" to="#monthly" role="tab" onClick={this.switchTab}>Monthly</Link>
          </li>
        </ul>
        <div className="tab-content" id="myTabContent">
          <div className="pt-4 pb-1 tab-pane show active animated bounceInUp" id="daily" role="tabpanel">
            {!this.state.loader
              ? daily.length > 0
                ? daily
                : <EmptyJordan message="Such empty..." />
              : ''}
            <div className="text-center">
              {seeMoreBtn}
            </div>
          </div>
          <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="weekly" role="tabpanel">
            {!this.state.loader
              ? weekly.length > 0
                ? weekly
                : <EmptyJordan message="Such empty..." />
              : ''}
            <div className="text-center">
              {seeMoreBtn}
            </div>
          </div>
          <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="monthly" role="tabpanel">
            {!this.state.loader
              ? monthly.length > 0
                ? monthly
                : <EmptyJordan message="Such empty..." />
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
      </div>
    );
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
}
