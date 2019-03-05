import React, { Component } from 'react';
import { Bar } from './Bar';

export class ProgressBar extends Component {
  render() {
    return (
      <div className="row mt-2 justify-content-center">
        <div className="progress bar" style={{ width: '90%', backgroundColor: 'white' }}>
          <Bar player={this.props.players.lineup.pg} />
          <Bar player={this.props.players.lineup.sg} />
          <Bar player={this.props.players.lineup.sf} />
          <Bar player={this.props.players.lineup.pf} />
          <Bar player={this.props.players.lineup.c} />
        </div>
      </div>
    );
  }
}
