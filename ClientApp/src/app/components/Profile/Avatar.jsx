import React, { Component } from 'react';
import Img from 'react-image';
import $ from 'jquery';
import { ChangeAvatar } from '../Inputs/ChangeAvatar';
import defaultPhoto from '../../../content/images/default.png';
import FriendRequest from './FriendRequest';
import { loadImage } from '../../utils/loadImage';

export class Avatar extends Component {
  constructor(props) {
    super(props);

    this.state = {
    };
  }

  async componentDidMount() {
    const { user } = this.props;
    this.setState({
      avatar: await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${user.avatarURL}.png`, defaultPhoto)
    });

    $('#changeImage').on('hidden.bs.modal', () => this.refs.changeAvatar.clear());
  }

  async componentWillReceiveProps(nextProps) {
    if (this.props === nextProps) { return; }
    const { user } = nextProps;
    this.setState({
      avatar: await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${user.avatarURL}.png`, defaultPhoto)
    });
  }

  render() {
    const { user, readOnly } = this.props;
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
          loader={<img src={require('../../../content/images/imageLoader.gif')} alt="Loader" />}
        />
        <FriendRequest user={user} readOnly={readOnly} />
        {!readOnly
          && (
            <div className="row">
              <button type="button" className="btn btn-outline-primary mx-auto" data-toggle="modal" data-target="#changeImage">
                {'Change avatar'}
              </button>
              <ChangeAvatar ref="changeAvatar" {...this.props} />
            </div>
          )
        }
      </div>
    );
  }
}

export default Avatar;
