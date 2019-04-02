import React, { PureComponent } from 'react';
import Img from 'react-image';

const { $ } = window;

export class Card extends PureComponent {
  constructor(props) {
    super(props);
    this.showModal = this.showModal.bind(this);
  }

  componentDidMount() {
    $('[data-toggle="tooltip"]').tooltip();
  }

  showModal() {
    $('[data-toggle="tooltip"]').tooltip('hide');
    const { showModal, player } = this.props;
    showModal(player);
  }

  render() {
    const {
      player, className, image, index
    } = this.props;
    return (
      <div className={`PlayerLeaderboardCard card rounded ${className}`}>
        <a
          data-toggle="tooltip"
          data-placement="top"
          title="Click for stats"
          style={{ height: '100%' }}
        >
          <div
            role="button"
            tabIndex="-1"
            className="PlayerLeaderboardCard__body card-body"
            data-toggle="modal"
            data-target="#playerModal"
            onClick={this.showModal}
            onKeyDown={this.showModal}
            style={{ cursor: 'pointer' }}
          >
            <div className="PlayerLeaderboardCard__body-item PlayerLeaderboardCard__player-ranking">
              {index + 1}
            </div>
            <div className="PlayerLeaderboardCard__body-item">
              <div
                className="PlayerLeaderboardCard__player-photo--background"
                style={{ backgroundColor: `${player.teamColor}`, cursor: 'pointer' }}
              >
                <Img
                  className="PlayerLeaderboardCard__player-photo--image"
                  alt=""
                  src={[
									  `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${player.nbaID}.png`,
									  image
                  ]}
                  decode={false}
                />
              </div>
            </div>
            <div className="PlayerLeaderboardCard__body-item PlayerLeaderboardCard__player-name--abbr">
              {player.abbrName}
            </div>
            <div className="PlayerLeaderboardCard__body-item PlayerLeaderboardCard__FP">
              <div className="PlayerLeaderboardCard__FP--text">
                {`x${player.count} `}
              </div>
            </div>
          </div>
        </a>
      </div>
    );
  }
}

export default Card;
