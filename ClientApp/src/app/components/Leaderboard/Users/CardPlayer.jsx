import React, { PureComponent } from 'react';
import Img from 'react-image';

const { $ } = window;

export default class CardPlayer extends PureComponent {
  constructor(props) {
    super(props);
    this.showModal = this.showModal.bind(this);
  }

  componentDidMount() {
    $('[data-toggle="tooltip"]').tooltip();
  }

  showModal() {
    $('[data-toggle="tooltip"]').tooltip('hide');
    this.props.showModal(this.props.player);
  }

  render() {
    return (
      <div className="UserLeaderboardCard__body-item UserLeaderboardCard__player">
        <a
          data-toggle="tooltip"
          data-placement="top"
          title="Click for stats"
          style={{ height: '100%' }}
        >
          <div
            role="button"
            tabIndex="0"
            className="UserLeaderboardCard__player-photo--background"
            style={{ backgroundColor: this.props.player.teamColor }}
            data-toggle="modal"
            data-target="#playerModal"
            onClick={this.showModal}
            onKeyDown={this.showModal}
          >
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
        </a>
      </div>
    );
  }
}
