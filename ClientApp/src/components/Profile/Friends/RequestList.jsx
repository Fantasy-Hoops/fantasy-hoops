import React, { Component } from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import { parse } from '../../../utils/auth';
import RequestCard from './RequestCard';

export default class RequestList extends Component {
  constructor(props) {
    super(props);
    this.state = {
      requests: ''
    };
  }

  componentDidMount() {
    const { user } = this.props;

    fetch(`${process.env.REACT_APP_SERVER_NAME}/api/friendrequest/requests/${user.id}`)
      .then(res => res.json())
      .then(res => this.setState({ requests: res }));
  }

  cancelRequest(receiver) {
    const sender = parse();
    if (!sender) {
      return;
    }

    const model = {
      senderID: sender.id,
      receiverID: receiver
    };

    fetch('/api/friendrequest/cancel', {
      method: 'POST',
      headers: {
        'Content-type': 'application/json'
      },
      body: JSON.stringify(model)
    })
      .then(() => window.location.reload());
  }

  acceptRequest(receiver) {
    const sender = parse();
    if (!sender) {
      return;
    }

    const model = {
      senderID: receiver,
      receiverID: sender.id
    };

    fetch('/api/friendrequest/accept', {
      method: 'POST',
      headers: {
        'Content-type': 'application/json'
      },
      body: JSON.stringify(model)
    })
      .then(() => window.location.reload());
  }

  declineRequest(receiver) {
    const sender = parse();
    if (!sender) {
      return;
    }

    const model = {
      senderID: receiver,
      receiverID: sender.id
    };

    fetch('/api/friendrequest/cancel', {
      method: 'POST',
      headers: {
        'Content-type': 'application/json'
      },
      body: JSON.stringify(model)
    })
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
        />
      ));
    return (
      <div className="row">
        {list.length > 0 ? list : 'You have no requests!'}
      </div>
    );
  }
}
