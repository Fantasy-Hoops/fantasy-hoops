import React, { Component } from 'react';
import { CardPlayer } from './CardPlayer';
import Img from 'react-image';
import defaultPhoto from '../../../content/images/default.png';
import { loadImage } from '../../../utils/loadImage';
import _ from 'lodash';
import shortid from 'shortid';

export class Card extends Component {
  constructor(props) {
    super(props);
    this.state = {
      avatar: ''
    }
    this.showModal = this.showModal.bind(this);
  }

  async componentWillMount() {
    this.setState({
      avatar: await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${this.props.user.userID}.png`, defaultPhoto)
    });
  }

  getPlayers() {
    return _.map(
      this.props.user.lineup,
      (player) => {
        return (
          <CardPlayer
            showModal={this.showModal}
            key={shortid()}
            player={player}
          />
        )
      });
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
            <a href={`/profile/${this.props.user.userName}`} className="UserLeaderboardCard__body-item UserLeaderboardCard__user-photo">
              <Img
                className="UserLeaderboardCard__user-photo--image"
                alt={this.props.user.userName}
                src={this.state.avatar}
                decode={false}
              />
            </a>
            <a
              href={`/profile/${this.props.user.userName}`}
              className="UserLeaderboardCard__body-item UserLeaderboardCard__username UserLeaderboardCard__username--daily"
            >
              {this.props.user.userName}
            </a>
            <div title="Fantasy Points" className="UserLeaderboardCard__body-item UserLeaderboardCard__FP UserLeaderboardCard__FP--daily">
              {`${this.props.user.score.toFixed(1)} `}<span style={{ fontSize: '0.7rem', fontWeight: 400 }}>FP</span>
            </div>
            {this.getPlayers()}
          </div>
        </div>
      );
    }
    else {
      return (
        <div className="UserLeaderboardCard card bg-white rounded">
          <div className="UserLeaderboardCard__body card-body">
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__user-ranking">
              {this.props.index + 1}
            </div>
            <div className="UserLeaderboardCard__body-item">
              <a href={`/profile/${this.props.user.userName}`} >
                <Img
                  className="UserLeaderboardCard__user-photo--image"
                  alt={this.props.user.userName}
                  src={this.state.avatar}
                  decode={false}
                />
              </a>
            </div>

            <a href={`/profile/${this.props.user.userName}`} className="UserLeaderboardCard__body-item UserLeaderboardCard__username">{this.props.user.userName}</a>
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__FP UserLeaderboardCard__FP--grey">
              {`${this.props.user.score.toFixed(1)} `}<span style={{ fontSize: '0.7rem', fontWeight: 400 }}>FP</span>
            </div>
          </div>
        </div>
      );
    }
  }
}