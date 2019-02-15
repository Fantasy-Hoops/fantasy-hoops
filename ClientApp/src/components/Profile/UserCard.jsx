import React, { Component } from 'react';
import defaultPhoto from '../../content/images/default.png';
import Img from 'react-image';

export class UserCard extends Component {
  constructor(props) {
    super(props);
    this.state = {
    }
  }

  render() {
    let img = new Image();
    let avatar;
    if (this.props.user) {
      img.src = `http://fantasyhoops.org/content/images/avatars/${this.props.user.id}.png`;
      avatar = img.height !== 0 ? img.src : defaultPhoto;
    }
    return (
      <a href={`/profile/${this.props.user.userName}`} className="friend-card m-3" style={{ backgroundColor: `${this.props.user.color}`, width: '8rem' }}>
        <canvas className="header-bg"></canvas>
        <div className="avatar">
          <Img
            alt={this.props.user.userName}
            src={avatar}
            loader={<img width='500px' src={require(`../../content/images/imageLoader2.gif`)} alt="Loader" />}
            decode={false}
          />
        </div>
        <div className="content badge badge-dark" style={{ marginTop: '1rem', marginBottom: '0.5rem', fontSize: '1rem' }}>
          <span>{this.props.user.userName}</span>
        </div>
      </a>
    );
  }
}
