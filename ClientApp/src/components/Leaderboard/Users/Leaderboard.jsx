import React, { Component } from 'react';
import { parse } from '../../../utils/auth';
import { Card } from './Card';
import leaderboardLogo from '../../../content/images/leaderboard.png';
import shortid from 'shortid';
import _ from 'lodash';
import { Loader } from '../../Loader';
import { EmptyJordan } from '../../EmptyJordan';
import { PlayerModal } from '../../PlayerModal/PlayerModal';
const user = parse();
const LOAD_COUNT = 10;
const $ = window.$;

export class Leaderboard extends Component {
  constructor(props) {
    super(props);
    this.toggleFriendsOnly = this.toggleFriendsOnly.bind(this);
    this.switchTab = this.switchTab.bind(this);
    this.loadMore = this.loadMore.bind(this);
    this.showModal = this.showModal.bind(this);

    this.state = {
      activeTab: 'daily',
      friendsOnly: false,
      daily: [],
      weekly: [],
      monthly: [],
      dailyFriends: [],
      weeklyFriends: [],
      monthlyFriends: [],
      loader: true,
      loadMore: false,
      showButton: {
        daily: false,
        dailyFriends: false,
        weekly: false,
        weeklyFriends: false,
        monthly: false,
        monthlyFriends: false
      },
      stats: '',
      modalLoader: true,
      renderChild: false
    }
  }

  async componentWillMount() {
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user?type=daily`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.state.showButton.daily = res.length === LOAD_COUNT;
        this.setState({
          daily: res,
          activeTab: 'daily',
          loader: false
        });
      })
    await this.setState({
      PG: require(`../../../content/images/positions/pg.png`),
      SG: require(`../../../content/images/positions/sg.png`),
      SF: require(`../../../content/images/positions/sf.png`),
      PF: require(`../../../content/images/positions/pf.png`),
      C: require(`../../../content/images/positions/c.png`)
    });
  }

  componentDidMount() {
    $("#playerModal").on("hidden.bs.modal", () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
  }

  async toggleFriendsOnly() {
    const friendsOnly = this.state.friendsOnly;
    this.setState({ friendsOnly: !this.state.friendsOnly });
    const type = this.state.activeTab + (!friendsOnly ? 'Friends' : '');

    if (this.state[type].length === 0) {
      this.setState({ loader: true });

      const url = !friendsOnly
        ? `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user/${user.id}?type=${this.state.activeTab}`
        : `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user?type=${this.state.activeTab}`;

      await fetch(url)
        .then(res => {
          return res.json()
        })
        .then(res => {
          this.state.showButton[type] = res.length === LOAD_COUNT;
          this.setState({
            [type]: res,
            loader: false
          });
        })
    }
  }

  async switchTab(e) {
    const activeTab = e.target.id.split(/-/)[0];
    const type = this.state.friendsOnly ? activeTab + "Friends" : activeTab;

    if (this.state.activeTab === activeTab)
      return;

    this.setState({ activeTab: activeTab });

    if (this.state[type].length === 0) {
      const url = this.state.friendsOnly
        ? `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user/${user.id}?type=${activeTab}`
        : `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user?type=${activeTab}`;

      this.setState({ loader: true });

      await fetch(url)
        .then(res => {
          return res.json()
        })
        .then(res => {
          this.state.showButton[type] = res.length === LOAD_COUNT;
          this.setState({
            [type]: res,
            loader: false
          });
        })
    }
  }

  async loadMore() {
    const activeTab = this.state.activeTab;
    const type = this.state.friendsOnly ? activeTab + "Friends" : activeTab;

    this.setState({ loadMore: true });

    const url = this.state.friendsOnly
      ? `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user/${user.id}?type=${activeTab}&from=${this.state[type].length}&limit=${LOAD_COUNT}`
      : `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user?type=${activeTab}&from=${this.state[type].length}&limit=${LOAD_COUNT}`;

    await fetch(url)
      .then(res => {
        return res.json()
      })
      .then(res => {
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

  async showModal(player) {
    this.setState({ modalLoader: true })
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/stats/${player.nbaID}`)
      .then(res => res.json())
      .then(res => {
        this.setState({
          stats: res,
          modalLoader: false,
          renderChild: true
        });
      });
  }

  render() {
    const activeType = this.state.friendsOnly ? this.state.activeTab + "Friends" : this.state.activeTab;
    const daily = this.createUsers(this.state.friendsOnly ? this.state.dailyFriends : this.state.daily, true);
    const weekly = this.createUsers(this.state.friendsOnly ? this.state.weeklyFriends : this.state.weekly);
    const monthly = this.createUsers(this.state.friendsOnly ? this.state.monthlyFriends : this.state.monthly);
    const seeMoreBtn = this.state.loader || this.state.loadMore
      ? <Loader show={this.state.loader || this.state.loadMore} />
      : this.seeMoreBtn(activeType);
    return (
      <div className="container bg-light pt-3 p-0">
        <div className="text-center pb-1">
          <img src={leaderboardLogo}
            alt="Leaderboard Logo"
            width="60rem"
          />
          <h3>Top Users</h3>
        </div>
        <div className="row justify-content-center">
          <div className="col-xs">
            <div style={{ transform: 'scale(0.7, 0.7)' }}>
              <label className="switch">
                <input type="checkbox" checked={this.state.friendsOnly} onChange={this.toggleFriendsOnly} />
                <span className="slider round"></span>
              </label>
            </div>
          </div>
          <div className="col-xs pt-2">
            <h6>Friends only</h6>
          </div>
        </div>
        <ul className="nav nav-pills justify-content-center mx-auto" id="myTab" role="tablist" style={{ width: '30%' }}>
          <li className="nav-item">
            <a className="nav-link active tab-no-outline" id="daily-tab" data-toggle="tab" href="#daily" role="tab" onClick={this.switchTab}>Daily</a>
          </li>
          <li className="nav-item">
            <a className="nav-link tab-no-outline" id="weekly-tab" data-toggle="tab" href="#weekly" role="tab" onClick={this.switchTab}>Weekly</a>
          </li>
          <li className="nav-item">
            <a className="nav-link tab-no-outline" id="monthly-tab" data-toggle="tab" href="#monthly" role="tab" onClick={this.switchTab}>Monthly</a>
          </li>
        </ul>
        <div className="tab-content" id="myTabContent">
          <div className="pt-4 pb-1 tab-pane fade show active animated bounceInUp" id="daily" role="tabpanel">
            {!this.state.loader
              ? daily.length > 0
                ? daily
                : <EmptyJordan message="Such empty..." />
              : ''}
            <div className="text-center">
              {seeMoreBtn}
            </div>
          </div>
          <div className="pt-4 pb-1 tab-pane fade animated bounceInUp" id="weekly" role="tabpanel">
            {!this.state.loader
              ? weekly.length > 0
                ? weekly
                : <EmptyJordan message="Such empty..." />
              : ''}
            <div className="text-center">
              {seeMoreBtn}
            </div>
          </div>
          <div className="pt-4 pb-1 tab-pane fade animated bounceInUp" id="monthly" role="tabpanel">
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

  createUsers(users, isDaily) {
    return _.map(
      users,
      (user, index) => {
        return <Card
          isDaily={isDaily}
          index={index}
          key={shortid()}
          user={user}
          showModal={this.showModal}
        />
      }
    );
  }
}