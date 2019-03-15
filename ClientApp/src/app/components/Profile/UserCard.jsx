import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Img from 'react-image';

export class UserCard extends Component {
  constructor(props) {
    super(props);
    this.state = {
    };
  }

  render() {
    const { user } = this.props;
    return (
      <a href={`/profile/${user.userName}`} className="friend-card m-3" style={{ backgroundColor: `${user.color}`, width: '8rem' }}>
        <canvas className="header-bg" />
        <div className="avatar">
          <Img
            alt={user.userName}
            src={`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${user.id}.png`}
            loader={<img width="500px" src={require('../../../content/images/imageLoader2.gif')} alt="Loader" />}
            decode={false}
          />
        </div>
        <div className="content badge badge-dark" style={{ marginTop: '1rem', marginBottom: '0.5rem', fontSize: '1rem' }}>
          <span>{user.userName}</span>
        </div>
      </a>
    );
  }
}

UserCard.propTypes = {
  user: PropTypes.shape({
    id: PropTypes.string.isRequired,
    userName: PropTypes.string.isRequired,
    color: PropTypes.string
  }).isRequired
};

export default UserCard;
