import React, { Component } from 'react';
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
    let image;
    try {
      image = require(`../../content/images/players/${this.props.player.nbaID}.png`);
    } catch (err) {
      image = require(`../../content/images/positions/${this.props.player.position.toLowerCase()}.png`);
    }
    return (
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
          <div className="card-circle" style={{ backgroundColor: this.props.player.color }}
          >
            <img
              className="user-card-player"
              src={image}
              alt={this.props.player.lastName}
            />
          </div>
          <p className="player-usertitle">{this.props.player.lastName}</p>
          <p className="player-score">{this.props.player.fp}</p>
        </div>
      </a>
    );
  }
}