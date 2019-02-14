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
    let image;
    try {
      image = require(`./content/images/avatars/${this.props.user.id}.png`)
    } catch (err) {
      console.log(err);
    }
    try {
      image = require(`/content/images/avatars/${this.props.user.id}.png`)
    } catch (err) {
      console.log(err);

    }
    try {
      image = require(`content/images/avatars/${this.props.user.id}.png`)
    } catch (err) {
      console.log(err);

    }
    return (
      <a href={`/profile/${this.props.user.userName}`} className="friend-card m-3" style={{ backgroundColor: `${this.props.user.color}`, width: '8rem' }}>
        <canvas className="header-bg"></canvas>
        <div className="avatar">
          <Img
            alt={this.props.user.userName}
            src={[
              image,
              defaultPhoto
            ]}
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
