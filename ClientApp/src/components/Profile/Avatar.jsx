import React, { Component } from 'react';
import { ChangeAvatar } from '../Inputs/ChangeAvatar';
import defaultPhoto from '../../content/images/default.png';
import { FriendRequest } from './FriendRequest';
import Img from 'react-image';

export class Avatar extends Component {
  render() {
    const user = this.props.user;
    const img = new Image();
    img.src = `http://fantasyhoops.org/content/images/avatars/${user.id}.png`;
    let avatar;
    if (user)
      avatar = img.height !== 0 ? img.src : defaultPhoto;
    return (
      <div>
        <div className="row">
          <h3 className="mt-3 mx-auto">{user.userName}</h3>
        </div>
        <Img
          alt={user.userName}
          className="mx-auto img-fluid img-circle d-block round-img"
          style={{ width: '160px', height: '160px' }}
          src={avatar}
          loader={<img src={require(`../../content/images/imageLoader.gif`)} alt="Loader" />}
        />
        <FriendRequest user={this.props.user} readOnly={this.props.readOnly} />
        {!this.props.readOnly &&
          <div className="row">
            <button type="button" className="btn btn-outline-primary mx-auto" data-toggle="modal" data-target="#changeImage">
              Change avatar
            </button>
            <ChangeAvatar />
          </div>
        }
      </div>
    );
  }
}
