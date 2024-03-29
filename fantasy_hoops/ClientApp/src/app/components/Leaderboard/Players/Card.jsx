import React, { PureComponent } from 'react';
import Img from 'react-image';

export class Card extends PureComponent {
  constructor(props) {
    super(props);
    this.showModal = this.showModal.bind(this);
  }

  showModal() {
    const { showModal, player } = this.props;
    showModal(player.nbaId);
  }

  render() {
    const { className } = this.props;
    return (
      <div className={`PlayerLeaderboardCard card rounded ${className}`}>
        <div
          className="PlayerLeaderboardCard__body card-body"
          role="button"
          tabIndex="-1"
          data-toggle="modal"
          data-target="#playerModal"
          onClick={this.showModal}
          onKeyDown={this.showModal}
          style={{ cursor: 'pointer' }}
          title="Click for stats"
        >
          <div className="PlayerLeaderboardCard__body-item PlayerLeaderboardCard__player-ranking">
            {this.props.index + 1}
          </div>
          <div className="PlayerLeaderboardCard__body-item">
            <div
              className="PlayerLeaderboardCard__player-photo--background"
              style={{ backgroundColor: `${this.props.player.teamColor}`, cursor: 'pointer' }}
            >
              <Img
                className="PlayerLeaderboardCard__player-photo--image"
                alt=""
                src={[
                  `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.player.nbaId}.png`,
                  this.props.image
                ]}
                decode={false}
              />
            </div>
          </div>
          <div className="PlayerLeaderboardCard__body-item PlayerLeaderboardCard__player-name--abbr">
            {this.props.player.abbrName}
          </div>
          <div className="PlayerLeaderboardCard__body-item PlayerLeaderboardCard__FP">
            <div className="PlayerLeaderboardCard__FP--text">
              {`${this.props.player.fp.toFixed(1)} `}
              <span style={{ fontSize: '1rem', fontWeight: 400 }}>FP</span>
              {this.props.season ? <div className="UserScoreCard__date">{this.props.player.shortDate}</div> : ''}
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default Card;