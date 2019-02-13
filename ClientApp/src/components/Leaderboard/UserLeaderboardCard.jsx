import React, { Component } from 'react';

export class UserLeaderboardCard extends Component {
  render() {
    let image;
    try {
      image = require(`./content/images/avatars/${this.props.injury.player.nbaID}.png`);
    } catch (err) {
      image = require(`../../content/images/default.png`);
    }
    return (
      <div className="card bg-white rounded mt-1 mx-auto" style={{ width: '20rem', height: '4.5rem' }}>
        <div className="card-body">
          <div className="d-inline-block align-middle mr-1">
            <h4>{this.props.index + 1}</h4>
          </div>
          <a href={`/profile/${this.props.userName}`} >
            <div className="d-inline-block position-absolute ml-3" style={{ top: '0.2rem' }}>
              <img
                className="user-card-player"
                src={image}
                alt={this.props.userName} />
            </div>
            <div className="d-inline-block">
              <p className="align-middle player-name" style={{ paddingLeft: '5rem', paddingTop: '0.3rem' }}>{this.props.userName}</p>
            </div>
          </a>
          <div className="d-inline-block float-right" style={{ paddingTop: '0.3rem' }}>
            <h5>{Math.round(this.props.fp * 100) / 100} FP</h5>
          </div>
        </div>
      </div>
    );
  }
}