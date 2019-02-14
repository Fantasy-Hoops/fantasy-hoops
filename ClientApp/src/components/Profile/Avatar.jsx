import React, { Component } from 'react';
import { ChangeAvatar } from '../Inputs/ChangeAvatar';
import defaultPhoto from '../../content/images/default.png';
import { FriendRequest } from './FriendRequest';
import { Loader } from '../Loader';
import Img from 'react-image';

export class Avatar extends Component {
  render() {
    const user = this.props.user;
    let avatar = '';
    try {
      avatar = <Img
        alt={user.userName}
        className="mx-auto img-fluid img-circle d-block round-img"
        style={{ width: '10rem' }}
        src={[`http://fantasyhoops.org/content/images/avatars/${user.id}.png`,
          defaultPhoto]}
        loader={<Loader show={true} />}
      />;
    } catch (err) {
    }
    return (
      <div className="col-lg-4 order-lg-1">
        <div className="row">
          <h3 className="mt-3 mx-auto">{user.userName}</h3>
        </div>
        {avatar}
        <FriendRequest user={user} readOnly={this.props.readOnly} />
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
