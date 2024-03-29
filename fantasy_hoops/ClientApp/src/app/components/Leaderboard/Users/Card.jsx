import React, { PureComponent } from 'react';
import { Link } from 'react-router-dom';
import Img from 'react-image';
import _ from 'lodash';
import shortid from 'shortid';
import CardPlayer from './CardPlayer';
import defaultPhoto from '../../../../content/images/default.png';
import { loadImage } from '../../../utils/loadImage';
import Routes from '../../../routes/routes';

export class Card extends PureComponent {
  _isMounted = false;

  constructor(props) {
    super(props);
    this.state = {
      avatar: ''
    };
    this.showModal = this.showModal.bind(this);
  }

  async componentDidMount() {
    const avatar = await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${this.props.user.avatarUrl}.png`, defaultPhoto);
    this._isMounted = true;
    if (this._isMounted) {
      this.setState({
        avatar
      });
    }
  }

  getPlayers() {
    return _.map(
      this.props.user.lineup,
      player => (
        <CardPlayer
          showModal={this.showModal}
          key={shortid()}
          player={player.player}
          teamColor={player.teamColor}
          fp={player.fp}
          price={player.price}
        />
      )
    );
  }

  showModal(nbaID) {
    this.props.showModal(nbaID);
  }

  render() {
    const { className } = this.props;
    
    if (this.props.isDaily) {
      return (
        <div className={`UserLeaderboardCard UserLeaderboardCard--daily card bg-white rounded ${className}`}>
          <div className="UserLeaderboardCard__body--daily card-body">
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__user-ranking UserLeaderboardCard__user-ranking--daily">
              {this.props.index + 1}
            </div>
            <Link to={`${Routes.PROFILE}/${this.props.user.username}`} className="UserLeaderboardCard__body-item UserLeaderboardCard__user-photo">
              <Img
                className="UserLeaderboardCard__user-photo--image Avatar--round"
                alt={this.props.user.username}
                src={this.state.avatar}
                decode={false}
              />
            </Link>
            <Link
              to={`${Routes.PROFILE}/${this.props.user.username}`}
              className="UserLeaderboardCard__body-item UserLeaderboardCard__username UserLeaderboardCard__username--daily"
            >
              {this.props.user.username}
            </Link>
            <div title="Fantasy Points" className="UserLeaderboardCard__body-item UserLeaderboardCard__FP UserLeaderboardCard__FP--daily">
              {`${this.props.user.fp.toFixed(1)} `}
              <span style={{ fontSize: '1rem', fontWeight: 400 }}>FP</span>
              <div className="UserLeaderboardCard__Date">{this.props.user.shortDate}</div>
            </div>
            {this.getPlayers()}
          </div>
        </div>
      );
    }

    return (
      <div className={`UserLeaderboardCard card bg-white rounded ${className}`}>
        <div className="UserLeaderboardCard__body card-body">
          <div className="UserLeaderboardCard__body-item UserLeaderboardCard__user-ranking">
            {this.props.index + 1}
          </div>
          <div className="UserLeaderboardCard__body-item">
            <Link to={`${Routes.PROFILE}/${this.props.user.username}`}>
              <Img
                className="UserLeaderboardCard__user-photo--image Avatar--round"
                alt={this.props.user.username}
                src={this.state.avatar}
                decode={false}
              />
            </Link>
          </div>
          <Link to={`${Routes.PROFILE}/${this.props.user.username}`} className="UserLeaderboardCard__body-item UserLeaderboardCard__username">{this.props.user.username}</Link>
          <div className="UserLeaderboardCard__body-item UserLeaderboardCard__FP UserLeaderboardCard__FP--grey">
            {`${this.props.user.fp.toFixed(1)} `}
            <span style={{ fontSize: '1rem', fontWeight: 400 }}>FP</span>
          </div>
        </div>
      </div>
    );
  }
}

export default Card;
