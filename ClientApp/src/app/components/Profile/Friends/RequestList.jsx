import React, { Component } from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import { parse } from '../../../utils/auth';
import RequestCard from './RequestCard';
import {
  getUserFriendRequests,
  acceptFriendRequest,
  cancelFriendRequest
} from '../../../utils/networkFunctions';

export default class RequestList extends Component {
  constructor(props) {
    super(props);
    this.state = {
      requests: ''
    };
  }

  async componentDidMount() {
    const { user } = this.props;

    await getUserFriendRequests(user.id)
      .then((res) => {
        this.setState({ requests: res.data });
      });
  }

  async cancelRequest(receiver) {
    const sender = parse();
    if (!sender) {
      return;
    }

    const model = {
      senderID: sender.id,
      receiverID: receiver
    };
    await cancelFriendRequest(model)
      .then(() => window.location.reload);
  }

  async acceptRequest(receiver) {
    const sender = parse();
    if (!sender) {
      return;
    }

    const model = {
      senderID: receiver,
      receiverID: sender.id
    };
    await acceptFriendRequest(model)
      .then(() => window.location.reload());
  }

  async declineRequest(receiver) {
    const sender = parse();
    if (!sender) {
      return;
    }

    const model = {
      senderID: receiver,
      receiverID: sender.id
    };

    await cancelFriendRequest(model)
      .then(() => window.location.reload());
  }

  render() {
    const { requests } = this.state;
    const list = _.map(requests,
      friend => (
        <RequestCard
          key={shortid()}
          id={friend.id}
          userName={friend.userName}
          cancel={this.cancelRequest}
          accept={this.acceptRequest}
          decline={this.declineRequest}
          type={friend.status}
          avatarURL={friend.avatarURL}
        />
      ));
    return (
      <div className="row">
        {list.length > 0 ? list : 'You have no requests!'}
      </div>
    );
  }
}
