import React, { Component } from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import { UserScore } from './UserScore';
import { PlayerModal } from '../PlayerModal/PlayerModal';
import { parse } from '../../utils/auth';
import { getPlayerStats } from '../../utils/networkFunctions';
import { getUserData } from '../../utils/networkFunctions';

const { $ } = window;
const user = parse();
const LOAD_COUNT = 5;

export class LineupHistory extends Component {
  constructor(props) {
    super(props);
    this.loadMore = this.loadMore.bind(this);
    this.showModal = this.showModal.bind(this);

    this.state = {
      stats: '',
      loadCounter: 0,
      user,
      recentActivity: [],
      loader: true,
      modalLoader: true
    };
  }

  setModal() {
    $('#playerModal').on('hidden.bs.modal', () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
  }

  async componentDidMount() {
    await getUserData(user.id, { count: 10 })
      .then((res) => {
        this.setState({
          recentActivity: res.data.recentActivity,
          loader: false,
          readOnly: false
        });
      });
  }

  async showModal(player) {
    $('[data-toggle="tooltip"]').tooltip('hide');
    this.setState({ modalLoader: true });
    await getPlayerStats(player.nbaID)
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
    await getUserData(user.id, { start: this.state.recentActivity.length, count: LOAD_COUNT })
      .then((res) => {
        this.setState({
          recentActivity: this.state.recentActivity.concat(res.data.recentActivity),
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
      <div className="container">
        <h1 className="text-center p-3">
          <span className="fa fa-history" />
          {' '}
          Your lineup history
        </h1>
        {recentActivity}
        {this.state.loader ? <div className="Loader" /> : null}
        <div className="text-center">
          {!this.state.loader ? btn : ''}
        </div>
        <PlayerModal
          renderChild={this.state.renderChild}
          loader={this.state.modalLoader}
          stats={this.state.stats}
        />
      </div>
    );
  }
}
