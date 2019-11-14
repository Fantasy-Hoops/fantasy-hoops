import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Img from 'react-image';
import { Link } from 'react-router-dom';
import { loadImage } from '../../utils/loadImage';
import defaultPhoto from '../../../content/images/default.png';

export class UserCard extends Component {
  constructor(props) {
    super(props);
    this.state = {
    };
  }

  async componentWillMount() {
    const { user } = this.props;
    this.setState({
      avatar: await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${user.avatarURL}.png`, defaultPhoto)
    });
  }

  render() {
    const { user } = this.props;
    return (
      <Link to={`/profile/${user.userName}`} className="UserCard" style={{ backgroundColor: `${user.color}` }}>
        <canvas className="header-bg" />
        <div className="avatar">
          <Img
            alt={user.userName}
            src={this.state.avatar}
            loader={<img width="500px" src={require('../../../content/images/imageLoader2.gif')} alt="Loader" />}
            decode={false}
          />
        </div>
        <div className="UserCard__UserName content badge badge-dark">
          <span className="UserCard__UserName--text">{user.userName}</span>
        </div>
      </Link>
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
