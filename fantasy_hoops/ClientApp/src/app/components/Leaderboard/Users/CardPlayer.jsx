import React, { PureComponent } from 'react';
import Img from 'react-image';

const { $ } = window;

export default class CardPlayer extends PureComponent {
  constructor(props) {
    super(props);
    this.showModal = this.showModal.bind(this);
  }

  showModal() {
    this.props.showModal(this.props.player.nbaID);
  }

  render() {
    return (
      <div className="UserLeaderboardCard__body-item UserLeaderboardCard__player">
        <div
          role="button"
          tabIndex="-1"
          className="UserLeaderboardCard__player-photo--background"
          style={{ backgroundColor: this.props.teamColor, cursor: 'pointer' }}
          data-toggle="modal"
          data-target="#playerModal"
          onClick={this.showModal}
          onKeyDown={this.showModal}
          title="Click for stats"
        >
          <Img
            className="UserLeaderboardCard__player-photo--image"
            alt={this.props.player.fullName}
            src={[
              `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.player.nbaID}.png`,
              require(`../../../../content/images/positions/${this.props.player.position.toLowerCase()}.png`)
            ]}
            decode={false}
          />
        </div>
        <p className="UserLeaderboardCard__player-lastname">
          {this.props.player.lastName}
        </p>
        <p className="UserLeaderboardCard__player-fp">
          {this.props.fp.toFixed(1)}
        </p>
      </div>
    );
  }
}
