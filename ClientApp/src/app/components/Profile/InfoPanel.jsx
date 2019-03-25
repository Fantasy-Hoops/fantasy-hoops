import React, { Component } from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import { Link } from 'react-router-dom';
import { UserScore } from './UserScore';
import { PlayerModal } from '../PlayerModal/PlayerModal';
import { getPlayerStats } from '../../utils/networkFunctions';
import Routes from '../../routes/routes';

const { $ } = window;

export class InfoPanel extends Component {
  constructor(props) {
    super(props);
    this.showModal = this.showModal.bind(this);

    this.state = {
      stats: '',
      modalLoader: true,
      renderChild: true
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

  async showModal(player) {
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

  getCurrentLineup() {
    const { user } = this.props;
    const liveBadge = user.currentLineup && user.currentLineup.isLive
      ? <span className="ml-2 alertPulse-css badge badge-danger" style={{ fontSize: '1.2rem' }}>LIVE</span>
      : null;
    return this.props.readOnly || !user.currentLineup
      ? null
      : (
        <div className="col-md-12">
          <h3 className="mt-2 d-inline-block">
            {' Current Lineup'}
          </h3>
          {liveBadge}
          <UserScore
            key={shortid()}
            activity={user.currentLineup}
            showModal={this.showModal}
            current
          />
        </div>
      );
  }

  render() {
    const { user } = this.props;
    const recentActivity = () => {
      if (!this.props.loader) {
        const activity = _.map(
          user.recentActivity,
          lineup => (
            <UserScore
              key={shortid()}
              activity={lineup}
              showModal={this.showModal}
            />
          )
        );
        return activity.length > 0
          ? (
            <div className="col-md-12">
              <h3 className="mt-2">
                <span className="fa fa-history" />
                {' Recent Activity'}
              </h3>
              {activity}
            </div>
          )
          : null;
      }
      return (
        <div className="p-5">
          <div className="Loader" />
        </div>
      );
    };

    const seeMore = () => {
      if (this.props.readOnly) { return ''; }
      return (
        <div className="pl-4 pt-3">
          <Link className="btn btn-outline-primary" to={Routes.LINEUP_HISTORY} role="button">History</Link>
        </div>
      );
    };

    return (
      <div className="tab-pane active" id="profile">
        <div className="row mx-auto">
          <div className="col-md-12">
            <div className="mx-auto mb-2">
              <div className="m-1 badge badge-indigo">
                <i className="fa fa-medal" />
                {` Record: ${user.userRecord} FP`}
              </div>
              <div className="m-1 badge badge-warning">
                <i className="fa fa-fire" />
                {` Streak: ${user.streak}`}
              </div>
              <br />
              <Link to={Routes.LEADERBOARD_USERS} className="m-1 badge badge-danger">
                <i className="fa fa-trophy" />
                {` Weekly Ranking: ${user.position}`}
              </Link>
              <Link to={Routes.LEADERBOARD_USERS} className="m-1 badge badge-info">
                <i className="fa fa-basketball-ball" />
                {` Weekly Score: ${Math.round(user.totalScore * 100) / 100} FP`}
              </Link>
            </div>
            {user.description
              && (
                <div>
                  <h3>About</h3>
                  <p className="Profile__About">
                    {user.description}
                  </p>
                </div>
              )
            }
          </div>
          <div className="col-md-12">
            <h3>Favorite team</h3>
            <div className="team-badge">
              <h1>
                <span
                  className="badge badge-dark badge-pill"
                  style={{ backgroundColor: user !== '' ? user.team.color : '' }}
                >
                  {user !== '' ? user.team.name : ''}
                </span>
              </h1>
            </div>
          </div>
          {this.getCurrentLineup()}
          {recentActivity()}
        </div>
        <PlayerModal
          renderChild={this.state.renderChild}
          loader={this.state.modalLoader}
          stats={this.state.stats}
        />
        {seeMore()}
      </div>
    );
  }
}

export default InfoPanel;
