import React, { PureComponent } from 'react';
import { Link } from 'react-router-dom';
import Img from 'react-image';
import _ from 'lodash';
import shortid from 'shortid';
import CardPlayer from './CardPlayer';
import defaultPhoto from '../../../../content/images/default.png';
import { loadImage } from '../../../utils/loadImage';

export default class Card extends PureComponent {
  _isMounted = false;

  constructor(props) {
    super(props);
    this.state = {
      avatar: ''
    };
    this.showModal = this.showModal.bind(this);
  }

  async componentDidMount() {
    const avatar = await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${this.props.user.userID}.png`, defaultPhoto);
    this._isMounted = true;
    if (this._isMounted) {
      this.setState({
        avatar
      });
    }
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  getPlayers() {
    return _.map(
      this.props.user.lineup,
      player => (
        <CardPlayer
          showModal={this.showModal}
          key={shortid()}
          player={player}
        />
      )
    );
  }

  showModal(player) {
    this.props.showModal(player);
  }

  render() {
    if (this.props.isDaily) {
      return (
        <div className="UserLeaderboardCard UserLeaderboardCard--daily card bg-white rounded">
          <div className="UserLeaderboardCard__body--daily card-body">
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__user-ranking UserLeaderboardCard__user-ranking--daily">
              {this.props.index + 1}
            </div>
            <Link to={`/profile/${this.props.user.userName}`} className="UserLeaderboardCard__body-item UserLeaderboardCard__user-photo">
              <Img
                className="UserLeaderboardCard__user-photo--image"
                alt={this.props.user.userName}
                src={this.state.avatar}
                decode={false}
              />
            </Link>
            <Link
              to={`/profile/${this.props.user.userName}`}
              className="UserLeaderboardCard__body-item UserLeaderboardCard__username UserLeaderboardCard__username--daily"
            >
              {this.props.user.userName}
            </Link>
            <div title="Fantasy Points" className="UserLeaderboardCard__body-item UserLeaderboardCard__FP UserLeaderboardCard__FP--daily">
              {`${this.props.user.fp.toFixed(1)} `}
              <span style={{ fontSize: '1rem', fontWeight: 400 }}>FP</span>
              <div className="UserScoreCard__date" style={{ fontSize: '1rem' }}>{this.props.user.shortDate}</div>
            </div>
            {this.getPlayers()}
          </div>
        </div>
      );
    }

    return (
      <div className="UserLeaderboardCard card bg-white rounded">
        <div className="UserLeaderboardCard__body card-body">
          <div className="UserLeaderboardCard__body-item UserLeaderboardCard__user-ranking">
            {this.props.index + 1}
          </div>
          <div className="UserLeaderboardCard__body-item">
            <Link to={`/profile/${this.props.user.userName}`}>
              <Img
                className="UserLeaderboardCard__user-photo--image"
                alt={this.props.user.userName}
                src={this.state.avatar}
                decode={false}
              />
            </Link>
          </div>
          <Link to={`/profile/${this.props.user.userName}`} className="UserLeaderboardCard__body-item UserLeaderboardCard__username">{this.props.user.userName}</Link>
          <div className="UserLeaderboardCard__body-item UserLeaderboardCard__FP UserLeaderboardCard__FP--grey">
            {`${this.props.user.fp.toFixed(1)} `}
            <span style={{ fontSize: '1rem', fontWeight: 400 }}>FP</span>
          </div>
        </div>
      </div>
    );
  }
}
