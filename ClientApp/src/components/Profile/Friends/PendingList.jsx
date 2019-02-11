import React, { Component } from 'react';
import { parse } from '../../../utils/auth';
import { RequestCard } from './RequestCard';
import shortid from 'shortid';
import defaultPhoto from '../../../content/images/default.png';
import _ from 'lodash';

export class PendingList extends Component {
  constructor(props) {
    super(props);
    this.state = {
      list: ''
    }
  }

  componentDidUpdate(prevProps) {
    if (prevProps === this.props)
      return;

    fetch(`http://68.183.213.191:5001/api/friendrequest/pending/${this.props.user.id}`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          list: res
        })
      });

  }

  render() {
    let list = _.map(this.state.list,
      (friend) => {
        return <RequestCard
          key={shortid()}
          id={friend.id}
          userName={friend.userName}
          avatar={this.props.images[`${friend.id}.png`] || defaultPhoto}
          accept={this.acceptRequest}
          decline={this.declineRequest}
          type="pending"
        />
      }
    );
    return (
      <div className="row">
        {list.length > 0 ? list : 'No pending requests!'}
      </div>
    );
  }

  acceptRequest(receiver) {
    const sender = parse();
    if (!sender)
      return;

    const model = {
      senderID: sender.id,
      receiverID: receiver
    }

    fetch('/api/friendrequest/accept', {
      method: 'POST',
      headers: {
        'Content-type': 'application/json'
      },
      body: JSON.stringify(model)
    })
      .then(res => {
        window.location.reload();
      });
  }

  declineRequest(receiver) {
    const sender = parse();
    if (!sender)
      return;

    const model = {
      senderID: receiver,
      receiverID: sender.id
    }

    fetch('/api/friendrequest/cancel', {
      method: 'POST',
      headers: {
        'Content-type': 'application/json'
      },
      body: JSON.stringify(model)
    })
      .then(res => {
        window.location.reload();
      });
  }


}