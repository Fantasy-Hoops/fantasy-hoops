import React, { Component } from 'react';
import Img from 'react-image';

export class CardPlayer extends Component {
  constructor(props) {
    super(props);
    this.state = {
    }
  }

  render() {
    return (
      <div className="UserLeaderboardCard__body-item UserLeaderboardCard__player">
        <div className="UserLeaderboardCard__player-photo--background" style={{ backgroundColor: this.props.player.teamColor }}>
          <Img
            className="UserLeaderboardCard__player-photo--image"
            alt={this.props.player.fullName}
            src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.player.nbaID}.png`]}
            decode={false}
          />
        </div>
        <p className="UserLeaderboardCard__player-lastname">
          {this.props.player.lastName}
        </p>
        <p className="UserLeaderboardCard__player-fp">
          {this.props.player.fp.toFixed(1)}
        </p>
      </div>
    );
  }
}
