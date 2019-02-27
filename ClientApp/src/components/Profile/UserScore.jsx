import React, { Component } from 'react';
import { UserScoreCard } from './UserScoreCard';
import shortid from 'shortid';
import _ from 'lodash';

export class UserScore extends Component {
  render() {
    let players = '';
    if (this.props.activity != null) {
      players = _.map(
        this.props.activity.players,
        (player) => {
          return (
            <UserScoreCard
              key={shortid()}
              player={player}
              showModal={this.props.showModal}
            />
          )
        });
    }

    return (
      <div className="UserScoreCard card bg-white rounded" style={{ width: `${this.props.width}`, margin: `${this.props.center}`, marginBottom: '0.5rem' }}>
        <div className="UserScoreCard__body card-body">
          {players}
          <div className="UserScoreCard__body-item">
            <p className="UserScoreCard__FP">{this.props.activity.score.toFixed(1)}</p>
            <p className="UserScoreCard__PTS">PTS</p>
            <p className="UserScoreCard__date">{this.props.activity.shortDate}</p>
          </div>
        </div>
      </div >
    );
  }
}