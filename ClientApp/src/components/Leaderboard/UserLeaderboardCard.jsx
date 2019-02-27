import React, { Component } from 'react';
import Img from 'react-image';
import defaultPhoto from '../../content/images/default.png';
import { loadImage } from '../../utils/loadImage';

export class UserLeaderboardCard extends Component {
  constructor(props) {
    super(props);
    this.state = {
    }
  }

  async componentWillMount() {
    this.setState({
      avatar: await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${this.props.user.id}.png`, defaultPhoto)
    });
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
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__player">
              <div className="UserLeaderboardCard__player-photo--background" style={{ backgroundColor: this.props.user.pg.teamColor }}>
                <Img
                  className="UserLeaderboardCard__player-photo--image"
                  alt={this.props.user.userName}
                  src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.user.pg.nbaID}.png`]}
                  decode={false}
                />
              </div>
              <p className="UserLeaderboardCard__player-lastname">
                {this.props.user.pg.lastName}
              </p>
              <p className="UserLeaderboardCard__player-fp">
                {this.props.user.pg.fp.toFixed(1)}
              </p>
            </div>
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__player">
              <div className="UserLeaderboardCard__player-photo--background" style={{ backgroundColor: this.props.user.sg.teamColor }}>
                <Img
                  className="UserLeaderboardCard__player-photo--image"
                  alt={this.props.user.userName}
                  src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.user.sg.nbaID}.png`]}
                  decode={false}
                />
              </div>
              <p className="UserLeaderboardCard__player-lastname">
                {this.props.user.sg.lastName}
              </p>
              <p className="UserLeaderboardCard__player-fp">
                {this.props.user.sg.fp.toFixed(1)}
              </p>
            </div>
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__player">
              <div className="UserLeaderboardCard__player-photo--background" style={{ backgroundColor: this.props.user.sf.teamColor }}>
                <Img
                  className="UserLeaderboardCard__player-photo--image"
                  alt={this.props.user.userName}
                  src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.user.sf.nbaID}.png`]}
                  decode={false}
                />
              </div>
              <p className="UserLeaderboardCard__player-lastname">
                {this.props.user.sf.lastName}
              </p>
              <p className="UserLeaderboardCard__player-fp">
                {this.props.user.sf.fp.toFixed(1)}
              </p>
            </div>
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__player">
              <div className="UserLeaderboardCard__player-photo--background" style={{ backgroundColor: this.props.user.pf.teamColor }}>
                <Img
                  className="UserLeaderboardCard__player-photo--image"
                  alt={this.props.user.userName}
                  src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.user.pf.nbaID}.png`]}
                  decode={false}
                />
              </div>
              <p className="UserLeaderboardCard__player-lastname">
                {this.props.user.pf.lastName}
              </p>
              <p className="UserLeaderboardCard__player-fp">
                {this.props.user.pf.fp.toFixed(1)}
              </p>
            </div>
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__player">
              <div className="UserLeaderboardCard__player-photo--background" style={{ backgroundColor: this.props.user.c.teamColor }}>
                <Img
                  className="UserLeaderboardCard__player-photo--image"
                  alt={this.props.user.userName}
                  src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.user.c.nbaID}.png`]}
                  decode={false}
                />
              </div>
              <p className="UserLeaderboardCard__player-lastname">
                {this.props.user.c.lastName}
              </p>
              <p className="UserLeaderboardCard__player-fp">
                {this.props.user.c.fp.toFixed(1)}
              </p>
            </div>
          </div>
        </div>
      );
    }
    else
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
            <div className="UserLeaderboardCard__body-item UserLeaderboardCard__FP">
              {this.props.user.score.toFixed(1)} FP
            </div>
          </div>
        </div>
      );
  }
}