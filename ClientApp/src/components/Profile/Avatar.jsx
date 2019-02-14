import React, { Component } from 'react';
import { ChangeAvatar } from '../Inputs/ChangeAvatar';
import defaultPhoto from '../../content/images/default.png';
import { FriendRequest } from './FriendRequest';
import Img from 'react-image';
import { Loader } from '../Loader';

export class Avatar extends Component {
  render() {
    const user = this.props.user;
    if (!user)
      return <div className="col-lg-4 order-lg-1"><div className="">
        <Loader show={!user} />
      </div> </div>;
    return (
      <div className="col-lg-4 order-lg-1">
        <div className="row">
          <h3 className="mt-3 mx-auto">{user.userName}</h3>
        </div>
        <Img
          alt={user.userName}
          className="mx-auto img-fluid img-circle d-block round-img"
          style={{ width: '10rem' }}
          src={[
            `http://fantasyhoops.org/content/images/avatars/${user.id}.png`,
            defaultPhoto
          ]}
          loader={<img width='10rem' src={require(`../../content/images/imageLoader2.gif`)} alt="Loader" />}
        />
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
