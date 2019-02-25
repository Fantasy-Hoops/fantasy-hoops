import React, { Component } from 'react';
import Img from 'react-image';
import defaultPhoto from '../../content/images/default.png';
import { loadImage } from '../../utils/loadImage';

export class UserLeaderboardCard extends Component {
  constructor(props) {
    super(props);
    this.state = {
    }
  }

  async componentWillMount() {
    this.setState({
      avatar: await loadImage(`http://fantasyhoops.org/content/images/avatars/${this.props.userid}.png`, defaultPhoto)
    });
  }

  render() {
    return (
      <div className="card bg-white rounded mt-1 mx-auto" style={{ width: '20rem', height: '4.5rem' }}>
        <div className="card-body">
          <div className="d-inline-block align-middle mr-1">
            <h4>{this.props.index + 1}</h4>
          </div>
          <a href={`/profile/${this.props.userName}`} >
            <div className="d-inline-block position-absolute ml-3" style={{ top: '0.2rem' }}>
              <Img
                className="user-card-player"
                alt={this.props.userName}
                src={this.state.avatar}
                decode={false}
              />
            </div>
            <div className="d-inline-block">
              <p className="align-middle player-name" style={{ paddingLeft: '5rem', paddingTop: '0.3rem' }}>{this.props.userName}</p>
            </div>
          </a>
          <div className="d-inline-block float-right" style={{ paddingTop: '0.3rem' }}>
            <h5>{this.props.fp.toFixed(1)} FP</h5>
          </div>
        </div>
      </div>
    );
  }
}