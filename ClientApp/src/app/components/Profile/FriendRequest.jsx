import React, { PureComponent } from 'react';
import { parse } from '../../utils/auth';
import { loadImage } from '../../utils/loadImage';
import defaultImage from '../../../content/images/default.png';
import { AlertNotification as Alert } from '../AlertNotification';
import {
  updateFriendRequestStatus,
  sendFriendRequest,
  sendPushNotification,
  acceptFriendRequest,
  cancelFriendRequest,
  removeFriendRequest
} from '../../utils/networkFunctions';

export default class FriendRequest extends PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      status: -2,
      alertText: '',
      alertType: ''
    };
  }

  async componentDidMount() {
    const sender = parse();
    if (!sender || !this.props.user) { return; }

    const receiver = this.props.user;
    if (receiver.id === sender.id) {
      this.setState({
        status: -2
      });
      return;
    }

    const model = {
      senderID: sender.id,
      receiverID: receiver.id
    };

    await updateFriendRequestStatus(model)
      .then((res) => {
        this.setState({
          status: res.data
        });
      });
  }

  async sendFriendRequest(receiver) {
    const sender = parse();
    if (!sender) { return; }

    const model = {
      senderID: sender.id,
      receiverID: receiver.id
    };

    await sendFriendRequest(model)
      .then((res) => {
        this.setState({
          status: 0,
          alertType: 'success',
          alertText: res.data
        });
      })
      .catch((err) => {
        this.setState({
          alertType: 'danger',
          alertText: err.response.data
        });
      })
      .then(this.refs.alert.addNotification);

    const notification = {
      title: 'Fantasy Hoops Friend Request',
      body: `User '${sender.username}' sent you a friend request`,
      icon: await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${sender.avatarURL}.png`, defaultImage),
      tag: `${sender.username}_friend_request`,
      actions: [
        { action: 'accept', title: '✔️ Accept' },
        { action: 'decline', title: '❌ Decline' }],
      data: {
        senderID: sender.id,
        senderUsername: sender.username,
        receiverID: receiver.id,
        receiverUsername: receiver.userName
      }
    };

    await sendPushNotification(receiver.id, notification);
  }

  async acceptFriendRequest(sender) {
    const receiver = parse();
    if (!receiver && !sender) { return; }

    const model = {
      senderID: sender.id,
      receiverID: receiver.id
    };

    await acceptFriendRequest(model)
      .then((res) => {
        this.setState({
          status: 1,
          alertType: 'success',
          alertText: res.data
        });
      })
      .catch((err) => {
        this.setState({
          alertType: 'danger',
          alertText: err.response.data
        });
      })
      .then(this.refs.alert.addNotification);
  }

  async cancelFriendRequest(receiver) {
    const sender = parse();
    if (!sender) { return; }

    const model = {
      senderID: sender.id,
      receiverID: receiver.id
    };

    await cancelFriendRequest(model)
      .then((res) => {
        this.setState({
          status: 3,
          alertType: 'success',
          alertText: res.data
        });
      })
      .catch((err) => {
        this.setState({
          alertType: 'danger',
          alertText: err.response.data
        });
      })
      .then(this.refs.alert.addNotification);
  }

  async removeFriend(receiver) {
    const sender = parse();
    if (!sender) { return; }

    const model = {
      senderID: sender.id,
      receiverID: receiver.id
    };

    await removeFriendRequest(model)
      .then((res) => {
        this.setState({
          status: 3,
          alertType: 'success',
          alertText: res.data
        });
      })
      .catch((err) => {
        this.setState({
          alertType: 'danger',
          alertText: err.response.data
        });
      })
      .then(this.refs.alert.addNotification);
  }

  changeButton(e, className, text) {
    e.target.className = `btn mx-auto ${className}`;
    e.target.innerText = text;
  }

  render() {
    let btn = null;
    switch (this.state.status) {
      case -2:
        break;
      case 0:
        btn = (
          <button
            type="button"
            onMouseEnter={e => this.changeButton(e, 'btn-danger', 'Cancel Request')}
            onMouseLeave={e => this.changeButton(e, 'btn-warning', 'Pending Request')}
            onClick={() => this.cancelFriendRequest(this.props.user)}
            className="btn btn-warning mx-auto"
          >
            {'Pending Request'}
          </button>
        );
        break;
      case 1:
        btn = (
          <button
            type="button"
            onMouseEnter={e => this.changeButton(e, 'btn-danger', 'Remove Friend')}
            onMouseLeave={e => this.changeButton(e, 'btn-info', 'Friends')}
            onClick={() => this.removeFriend(this.props.user)}
            className="btn btn-info mx-auto"
          >
            {'Friends'}
          </button>
        );
        break;
      case 200:
        btn = <button type="button" onClick={() => this.acceptFriendRequest(this.props.user)} className="btn btn-outline-info mx-auto">Accept Request</button>;
        break;
      default:
        btn = <button type="button" onClick={() => this.sendFriendRequest(this.props.user)} className="btn btn-outline-secondary mx-auto">Send Friend Request</button>;
        break;
    }

    return (
      <div className="row">
        <Alert
          ref="alert"
          {...this.props}
          type={this.state.alertType}
          text={this.state.alertText}
        />
        {btn}
      </div>
    );
  }
}
