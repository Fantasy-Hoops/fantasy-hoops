import React, { Component } from 'react';
import { ChangeAvatar } from '../Inputs/ChangeAvatar';
import defaultPhoto from '../../../content/images/default.png';
import FriendRequest from './FriendRequest';
import Img from 'react-image';
import { loadImage } from '../../utils/loadImage';
const $ = window.$;

export class Avatar extends Component {
  constructor(props) {
    super(props);

    this.state = {
    };
  }

  async componentDidMount() {
    const user = this.props.user;
    this.setState({
      avatar: await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${user.id}.png`, defaultPhoto)
    });

    $('#changeImage').on('hidden.bs.modal', () => {
      return this.refs.changeAvatar.clear();
    });
  }

  render() {
    const user = this.props.user;
    return (
      <div>
        <div className="row mx-auto">
          <div className="Profile__UserName mx-auto">{user.userName}</div>
        </div>
        <Img
          alt={user.userName}
          className="Profile__Avatar mx-auto img-fluid img-circle d-block"
          style={{ maxWidth: '12rem' }}
          src={this.state.avatar}
          loader={<img src={require(`../../../content/images/imageLoader.gif`)} alt="Loader" />}
        />
        <FriendRequest user={this.props.user} readOnly={this.props.readOnly} />
        {!this.props.readOnly &&
          <div className="row">
            <button type="button" className="btn btn-outline-primary mx-auto" data-toggle="modal" data-target="#changeImage">
              Change avatar
            </button>
            <ChangeAvatar ref='changeAvatar' {...this.props} />
          </div>
        }
      </div>
    );
  }
}
