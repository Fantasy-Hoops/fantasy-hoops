import React, { PureComponent } from 'react';
import { Link } from 'react-router-dom';
import shortid from 'shortid';
import _ from 'lodash';
import { parse } from '../../../utils/auth';
import Card from './Card';
import leaderboardLogo from '../../../../content/images/leaderboard.png';
import { Loader } from '../../Loader';
import { EmptyJordan } from '../../EmptyJordan';
import { PlayerModal } from '../../PlayerModal/PlayerModal';

const loggedInUser = parse();
const LOAD_COUNT = 10;
const { $ } = window;

export default class Leaderboard extends PureComponent {
  _isMounted = false;

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
    };
  }

  async componentDidMount() {
    $('#playerModal').on('hidden.bs.modal', () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
    const { showButton } = this.state;
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user?type=daily`)
      .then(res => res.json())
      .then((res) => {
        showButton.daily = res.length === LOAD_COUNT;
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

  async toggleFriendsOnly() {
    const { friendsOnly, activeTab, showButton } = this.state;
    this.setState({ friendsOnly: !friendsOnly });
    const type = activeTab + (!friendsOnly ? 'Friends' : '');

    if (this.state[type].length === 0) {
      this.setState({ loader: true });

      const url = !friendsOnly
        ? `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user/${loggedInUser.id}?type=${activeTab}`
        : `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user?type=${activeTab}`;

      await fetch(url)
        .then(res => res.json())
        .then((res) => {
          showButton[type] = res.length === LOAD_COUNT;
          this.setState({
            [type]: res,
            loader: false
          });
        });
    }
  }

  async switchTab(e) {
    const { friendsOnly, activeTab, showButton } = this.state;
    const activeTabURL = e.target.id.split(/-/)[0];
    const type = friendsOnly ? `${activeTabURL}Friends` : activeTabURL;

    if (activeTab === activeTabURL) { return; }

    this.setState({ activeTab: activeTabURL });

    if (this.state[type].length === 0) {
      const url = friendsOnly
        ? `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user/${loggedInUser.id}?type=${activeTabURL}`
        : `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user?type=${activeTabURL}`;

      this.setState({ loader: true });
      await fetch(url)
        .then(res => res.json())
        .then((res) => {
          showButton[type] = res.length === LOAD_COUNT;
          this.setState({
            [type]: res,
            loader: false
          });
        });
    }
  }

  async loadMore() {
    const { activeTab, friendsOnly, showButton } = this.state;
    const type = friendsOnly ? `${activeTab}Friends` : activeTab;

    this.setState({ loadMore: true });

    const url = friendsOnly
      ? `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user/${loggedInUser.id}?type=${activeTab}&from=${this.state[type].length}&limit=${LOAD_COUNT}`
      : `${process.env.REACT_APP_SERVER_NAME}/api/leaderboard/user?type=${activeTab}&from=${this.state[type].length}&limit=${LOAD_COUNT}`;

    await fetch(url)
      .then(res => res.json())
      .then((res) => {
        showButton[type] = res.length === LOAD_COUNT;
        this.setState({
          [type]: this.state[type].concat(res),
          loadMore: false
        });
      });
  }

  seeMoreBtn(type) {
    const { showButton } = this.state;
    return showButton[type] ? <button type="button" className="btn btn-primary mt-2" onClick={this.loadMore}>See more</button> : '';
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

  createUsers(users, isDaily) {
    return _.map(
      users,
      (user, index) => (
        <Card
          isDaily={isDaily}
          index={index}
          key={shortid()}
          user={user}
          showModal={this.showModal}
        />
      )
    );
  }

  render() {
    const {
      friendsOnly,
      activeTab,
      daily,
      dailyFriends,
      weekly,
      weeklyFriends,
      monthly,
      monthlyFriends,
      loader,
      loadMore,
      renderChild,
      modalLoader,
      stats
    } = this.state;

    const activeType = friendsOnly ? `${activeTab}Friends` : activeTab;
    const dailyUsers = this.createUsers(friendsOnly ? dailyFriends : daily, true);
    const weeklyUsers = this.createUsers(friendsOnly ? weeklyFriends : weekly);
    const monthlyUsers = this.createUsers(friendsOnly ? monthlyFriends : monthly);
    const seeMoreBtn = loader || loadMore
      ? <Loader show={loader || loadMore} />
      : this.seeMoreBtn(activeType);
    return (
      <div className="container bg-light">
        <div className="text-center">
          <img
            src={leaderboardLogo}
            alt="Leaderboard Logo"
            width="60rem"
          />
          <h3>Top Users</h3>
        </div>
        <div className="row justify-content-center">
          <div className="col-xs">
            <div style={{ transform: 'scale(0.7, 0.7)' }}>
              <label className="switch">
                <input type="checkbox" checked={friendsOnly} onChange={this.toggleFriendsOnly} />
                <span className="slider round" />
              </label>
            </div>
          </div>
          <div className="col-xs pt-2">
            <h6>Friends only</h6>
          </div>
        </div>
        <ul className="nav nav-pills justify-content-center mx-auto" id="myTab" role="tablist" style={{ width: '30%' }}>
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
          <div className="pt-4 pb-1 tab-pane animated bounceInUp show active" id="daily" role="tabpanel">
            {!loader
              ? dailyUsers.length > 0
                ? dailyUsers
                : <EmptyJordan message="Such empty..." />
              : ''}
            <div className="text-center">
              {seeMoreBtn}
            </div>
          </div>
          <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="weekly" role="tabpanel">
            {!loader
              ? weeklyUsers.length > 0
                ? weeklyUsers
                : <EmptyJordan message="Such empty..." />
              : ''}
            <div className="text-center">
              {seeMoreBtn}
            </div>
          </div>
          <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="monthly" role="tabpanel">
            {!loader
              ? monthlyUsers.length > 0
                ? monthlyUsers
                : <EmptyJordan message="Such empty..." />
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
      </div>
    );
  }
}
