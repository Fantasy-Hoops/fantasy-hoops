import React, { PureComponent } from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import { UserScoreCard } from './UserScoreCard';

export class UserScore extends PureComponent {
  render() {
    let players = '';
    if (this.props.activity != null) {
      players = _.map(
        this.props.activity.lineup,
        player => (
          <UserScoreCard
            key={shortid()}
            player={player.player}
            teamColor={player.teamColor}
            fp={player.fp}
            price={player.price}
            showModal={this.props.showModal}
          />
        )
      );
    }
    const { current, activity } = this.props;
    return (
      <div className={`UserScoreCard card bg-white rounded ${this.props.className}`} style={{ width: `${this.props.width}`, margin: `${this.props.center}`, marginBottom: '0.5rem' }}>
        <div className="UserScoreCard__body card-body">
          {players}
          <div className="UserScoreCard__body-item UserScoreCard__result">
            <div className="UserScoreCard__FP">
              {current
                ? _.map(this.props.activity.lineup, lineup => (lineup.fp))
                  .reduce((a, b) => a + b).toFixed(1)
                : this.props.activity.fp.toFixed(1)}
              <span style={{ fontSize: '1rem', fontWeight: 400, margin: '0 0 0 .3rem', display: 'table' }}>
                {' FP'}
              </span>
              <div className="UserScoreCard__date">{this.props.activity.shortDate}</div>
            </div>
            <div>
              {!current && activity.price ? <span>&#36;{activity.price}K</span> : null}
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default UserScore;
