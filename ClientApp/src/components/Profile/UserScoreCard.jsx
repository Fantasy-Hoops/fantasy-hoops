import React, { Component } from 'react';
import Img from 'react-image';
const $ = window.$;

export class UserScoreCard extends Component {
  constructor(props) {
    super(props);
    this.showModal = this.showModal.bind(this);
  }

  componentDidMount() {
    $(document).ready(function () {
      $('[data-toggle=tooltip]').tooltip({ trigger: "hover" });
    });
  }

  showModal() {
    $('[data-toggle="tooltip"]').tooltip("hide");
    this.props.showModal(this.props.player);
  }

  render() {
    return (
      <div className="UserScoreCard__body-item UserScoreCard__player">
        <a
          data-toggle="tooltip"
          data-placement="top"
          title="Click for stats"
        >
          <div
            data-toggle="modal"
            data-target="#playerModal"
            onClick={this.showModal}
            style={{ overflow: 'hidden', cursor: 'default' }}>
            <div className="UserScoreCard__player-photo--background" style={{ backgroundColor: this.props.player.teamColor }}
            >
              <Img
                className="UserScoreCard__player-photo--image"
                alt={this.props.player.lastName}
                src={[
                  `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.player.nbaID}.png`,
                  require(`../../content/images/positions/${this.props.player.position.toLowerCase()}.png`)
                ]}
                loader={<img className='UserScoreCard__loader' src={require(`../../content/images/imageLoader.gif`)} alt="Loader" />}
                decode={false}
              />
            </div>
          </div>
        </a>
        <div className="UserScoreCard__player-lastname">{this.props.player.lastName}</div>
        <p className="UserScoreCard__player-FP">{this.props.player.fp.toFixed(1)}</p>
      </div>
    );
  }
}