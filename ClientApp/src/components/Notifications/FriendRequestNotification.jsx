import React, { Component } from 'react';
import moment from 'moment';
import defaultPhoto from '../../content/images/default.png';
import Img from 'react-image';

export class FriendRequestNotification extends Component {
  constructor(props) {
    super(props);
    this.select = this.select.bind(this);
  }

  async select() {
    await this.props.toggleNotification(this.props.notification);
    window.location.href = `/profile/${this.props.notification.friend.userName}`
  }

  getDate() {
    const date = new Date(this.props.notification.dateCreated);
    const serverTime = new Date(this.props.serverTime);
    const diff = Math.abs(serverTime.getTimezoneOffset()) / 60;
    return date.setHours(date.getHours() + diff);
  }

  render() {
    let read = "card-body text-primary";
    if (this.props.notification.readStatus)
      read = "card-body text-muted";
    const img = new Image();
    let avatar;
    if (this.props.notification) {
      img.src = `http://fantasyhoops.org/content/images/avatars/${this.props.notification.friendID}.png`;
      avatar = img.height !== 0 ? img.src : defaultPhoto;
    }
    return (
      <a onClick={this.select} className="card cursor-pointer link mx-auto" style={{ maxWidth: `${this.props.width}` }}>
        <div className={read} style={{ margin: '-0.6rem' }}>
          <div className="row">
            <div className="col-1 mr-3">
              <Img
                className="mt-2"
                width="40rem"
                height="40rem"
                alt={this.props.notification.friend.userName}
                src={avatar}
                loader={<img width='40px' src={require(`../../content/images/imageLoader2.gif`)} alt="Loader" />}
                decode={false}
              />
            </div>
            <div className="col ml-1">
              <h5 className="card-title">{this.props.notification.friend.userName}</h5>
              <p className="card-text"
                style={{ marginTop: '-0.7rem', fontWeight: '400' }}
              >{this.props.notification.requestMessage}
              </p>
              <p style={{ margin: '-1rem 0 0 0' }}>
                {moment(this.getDate()).fromNow()}
              </p>
            </div>
          </div>
        </div>
      </a>
    );
  }
}
