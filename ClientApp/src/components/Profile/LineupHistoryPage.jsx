import React, { Component } from 'react';
import shortid from 'shortid';
import { Loader } from '../Loader';
import { UserScore } from './UserScore';
import { PlayerModal } from '../PlayerModal/PlayerModal';
import icon from '../../content/images/basketball-player-scoring.svg';
import { parse } from '../../utils/auth';
import _ from 'lodash';
const $ = window.$;
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
      user: user,
      recentActivity: [],
      loader: true,
      modalLoader: true
    }
  }

  setModal() {
    $("#playerModal").on("hidden.bs.modal", () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
  }

  async showModal(player) {
    $('[data-toggle="tooltip"]').tooltip("hide");
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

  async componentWillMount() {
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/user/${user.id}?count=10`)
      .then(res => res.json())
      .then(res => {
        this.setState({
          recentActivity: res.recentActivity,
          loader: false,
          readOnly: false
        });
      });
  }

  async loadMore() {
    this.setState({
      loadCounter: this.state.loadCounter + 1,
      loader: true
    });
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/user/${user.id}?start=${this.state.recentActivity.length}&count=${LOAD_COUNT}`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          recentActivity: this.state.recentActivity.concat(res.recentActivity),
          loader: false
        });
      });
  }

  render() {
    const recentActivity = _.map(
      this.state.recentActivity,
      (activity) => {
        return (
          <UserScore
            key={shortid()}
            activity={activity}
            showModal={this.showModal}
            center='0 auto'
            width='40%'
          />
        )
      });

    const btn = this.state.loadCounter * LOAD_COUNT + 10 > this.state.recentActivity.length
      ? ''
      : <button className="btn btn-primary m-3" onClick={this.loadMore}>See more</button>;

    return (
      <div className="container">
        <h3 className="text-center pb-3"><span><img src={icon} width="65rem" alt="Basketball Player Scoring" /></span> Your lineup history</h3>
        {recentActivity}
        <Loader show={this.state.loader} />
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
