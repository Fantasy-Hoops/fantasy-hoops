import React, { Component } from 'react';

export class UserCard extends Component {
  constructor(props) {
    super(props);
    this.state = {
    }
  }

  render() {
    let avatar;
    try {
      avatar = require(`content/images/avatars/${this.props.user.id}.png`);
    } catch (err) {
      avatar = require(`../../content/images/default.png`);
    }
    return (
      <a href={`/profile/${this.props.user.userName}`} className="friend-card m-3" style={{ backgroundColor: `${this.props.user.color}`, width: '8rem' }}>
        <canvas className="header-bg" width="250" height="70" id="header-blur"></canvas>
        <div className="avatar">
          <img src={avatar} alt="" />
        </div>
        <div className="content badge badge-dark" style={{ marginTop: '1rem', marginBottom: '0.5rem', fontSize: '1rem' }}>
          <span>{this.props.user.userName}</span>
        </div>
      </a>
    );
  }
}
